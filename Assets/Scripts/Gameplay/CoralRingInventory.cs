using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class CoralRingInventory : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float ringRadius = 2.5f;
    [SerializeField] private float previewScale = 0.3f;
    [SerializeField] private float selectedScale = 0.5f;

    // Option A: assign a dedicated preview prefab per coral slot.
    // If a slot is left empty the coral grows in the background (Option B).
    [SerializeField] private GameObject[] coralPreviews;

    [Inject] private ICoralSpawnService _corals;

    public static bool IsVisible { get; private set; }

    private readonly List<GameObject> _previews = new List<GameObject>();
    private readonly List<bool> _previewIsGrowing = new List<bool>(); // true = Option B
    private int _currentSelection;

    // Far below terrain — Option B corals grow here invisibly while ring is closed
    private static readonly Vector3 HiddenPos = new Vector3(0f, -500f, 0f);

    private void OnEnable()  => CoralPlantHandler.OnSelectionChanged += HandleSelectionChanged;
    private void OnDisable() => CoralPlantHandler.OnSelectionChanged -= HandleSelectionChanged;

    private void Start()
    {
        if (playerTransform == null)
        {
            var p = GameObject.FindWithTag("Player");
            if (p != null) playerTransform = p.transform;
            else Debug.LogWarning("[CoralRingInventory] playerTransform not set and no 'Player' tag found.");
        }

        // Eager build — gives Option B corals time to grow before the ring is first opened
        BuildPreviews();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(2)) Toggle();
        if (IsVisible) PositionRing();
    }

    // ── Visibility ────────────────────────────────────────────────────────────

    private void Toggle()
    {
        if (IsVisible) Hide();
        else Show();
    }

    private void Show()
    {
        // Rebuild if the coral count changed (e.g. after domain reload)
        if (_previews.Count != _corals.CoralCount)
        {
            foreach (var p in _previews) if (p != null) Destroy(p);
            _previews.Clear();
            _previewIsGrowing.Clear();
            BuildPreviews();
        }

        for (int i = 0; i < _previews.Count; i++)
        {
            if (_previews[i] == null) continue;
            _previews[i].transform.localScale = Vector3.one * previewScale;
            if (!_previewIsGrowing[i]) _previews[i].SetActive(true);
            // Option B corals are always active — ring position is enough to show them
        }

        PositionRing();
        UpdateHighlight(_currentSelection);
        IsVisible = true;
    }

    private void Hide()
    {
        for (int i = 0; i < _previews.Count; i++)
        {
            if (_previews[i] == null) continue;
            if (_previewIsGrowing[i])
                // Keep active so L-System coroutines keep running; just move out of view
                _previews[i].transform.position = HiddenPos;
            else
                _previews[i].SetActive(false);
        }
        IsVisible = false;
    }

    // ── Build ─────────────────────────────────────────────────────────────────

    private void BuildPreviews()
    {
        int count = _corals.CoralCount;
        for (int i = 0; i < count; i++)
        {
            bool hasCustom = coralPreviews != null
                && i < coralPreviews.Length
                && coralPreviews[i] != null;

            GameObject preview;
            bool growing;

            if (hasCustom)
            {
                preview = BuildOptionA(coralPreviews[i]);
                growing = false;
            }
            else
            {
                preview = BuildOptionB(_corals.GetCoralPrefab(i), i);
                growing = true;
            }

            preview.name = $"CoralPreview_{i}";
            _previews.Add(preview);
            _previewIsGrowing.Add(growing);
        }
    }

    // Option A — static preview prefab assigned in inspector
    private static GameObject BuildOptionA(GameObject prefabAsset)
    {
        GameObject preview = Instantiate(prefabAsset);
        RemoveColliders(preview);
        SetLayerRecursive(preview, 2);
        preview.SetActive(false);
        return preview;
    }

    // Option B — let the real coral grow underground; show it once mesh is combined
    private GameObject BuildOptionB(GameObject prefab, int index)
    {
        if (prefab == null) return BuildFallbackSphere(index);

        GameObject preview = Instantiate(prefab);
        RemoveColliders(preview);
        SetLayerRecursive(preview, 2);

        // Compact the growth so it fits inside the ring radius when combined.
        // Reducing `length` keeps the shape representative while making it smaller.
        var growth = preview.GetComponent<CoralGrowthBase>();
        if (growth != null) growth.length *= previewScale;

        // ISpawnable: anchor the growth underground so tree/random corals start from there
        var spawnable = preview.GetComponent<ISpawnable>();
        if (spawnable != null)
        {
            spawnable.SetStartPosition(HiddenPos);
            spawnable.SetParent(preview);
        }
        // No SetColorEffectService — previews don't participate in bleaching/recoloring

        // Sphere placeholder, visible until CombineMeshesAfterGrowing produces a real mesh
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = "FallbackSphere";
        Destroy(sphere.GetComponent<Collider>());
        sphere.transform.SetParent(preview.transform, false);
        sphere.transform.localPosition = Vector3.zero;
        sphere.transform.localScale    = Vector3.one;
        sphere.layer = 2;
        sphere.GetComponent<Renderer>().material.color =
            Color.HSVToRGB(index / (float)Mathf.Max(_corals.CoralCount, 1), 0.7f, 0.9f);

        preview.transform.position   = HiddenPos;
        preview.transform.localScale = Vector3.one * previewScale;
        // Stay active — coroutines must keep running
        return preview;
    }

    private GameObject BuildFallbackSphere(int index)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(go.GetComponent<Collider>());
        go.layer = 2;
        go.GetComponent<Renderer>().material.color =
            Color.HSVToRGB(index / (float)Mathf.Max(_corals.CoralCount, 1), 0.7f, 0.9f);
        return go;
    }

    // ── Position & Highlight ──────────────────────────────────────────────────

    private void PositionRing()
    {
        if (playerTransform == null) return;
        Vector3 center = playerTransform.position + Vector3.up * 1.2f;
        int count = _previews.Count;

        Camera cam = PS_TiltFive.Core.CameraHelper.Main;

        // Direction from ring center toward camera (XZ only) — selected slot sits here
        Vector3 baseDir = Vector3.forward;
        if (cam != null)
        {
            Vector3 toCamera = cam.transform.position - center;
            toCamera.y = 0f;
            if (toCamera.sqrMagnitude > 0.001f) baseDir = toCamera.normalized;
        }

        float stepDeg = 360f / count;
        for (int i = 0; i < count; i++)
        {
            if (_previews[i] == null) continue;

            // Rotate baseDir by (i - selected) * step so selected = camera-nearest slot
            Vector3 slotDir = Quaternion.Euler(0f, (i - _currentSelection) * stepDeg, 0f) * baseDir;
            _previews[i].transform.position = center + slotDir * ringRadius;

            // Face each preview toward the camera
            if (cam != null)
            {
                Vector3 lookDir = cam.transform.position - _previews[i].transform.position;
                lookDir.y = 0f;
                if (lookDir.sqrMagnitude > 0.001f)
                    _previews[i].transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);
            }

            TryRemoveFallbackSphere(_previews[i]);
        }
    }

    private void UpdateHighlight(int index)
    {
        var mpb = new MaterialPropertyBlock();
        for (int i = 0; i < _previews.Count; i++)
        {
            if (_previews[i] == null) continue;
            bool selected = i == index;
            _previews[i].transform.localScale = Vector3.one * (selected ? selectedScale : previewScale);
            foreach (var r in _previews[i].GetComponentsInChildren<Renderer>())
            {
                r.GetPropertyBlock(mpb);
                mpb.SetColor("_EmissionColor", selected ? Color.white * 0.6f : Color.black);
                r.SetPropertyBlock(mpb);
            }
        }
    }

    private void HandleSelectionChanged(int index)
    {
        _currentSelection = index;
        if (IsVisible) UpdateHighlight(index);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static void TryRemoveFallbackSphere(GameObject preview)
    {
        var fallback = preview.transform.Find("FallbackSphere");
        if (fallback == null) return;
        var mf = preview.GetComponent<MeshFilter>();
        if (mf != null && mf.sharedMesh != null && mf.sharedMesh.vertexCount > 0)
            Destroy(fallback.gameObject);
    }

    private static void RemoveColliders(GameObject go)
    {
        foreach (var col in go.GetComponentsInChildren<Collider>(true))
            Destroy(col);
    }

    private static void SetLayerRecursive(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform)
            SetLayerRecursive(child.gameObject, layer);
    }

    private void OnDestroy()
    {
        foreach (var p in _previews)
            if (p != null) Destroy(p);
        IsVisible = false;
    }
}
