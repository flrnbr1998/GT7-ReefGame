using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class CoralRingInventory : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float ringRadius = 2.5f;
    [SerializeField] private float previewScale = 0.3f;
    [SerializeField] private float selectedScale = 0.5f;

    [Inject] private ICoralSpawnService _corals;

    public static bool IsVisible { get; private set; }

    private readonly List<GameObject> _previews = new List<GameObject>();
    private int _currentSelection;
    private bool _built;

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
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(2))
            Toggle();
    }

    private void Toggle()
    {
        if (IsVisible) Hide();
        else Show();
    }

    private void Show()
    {
        if (!_built || _previews.Count != _corals.CoralCount)
            BuildPreviews();

        PositionRing();
        foreach (var p in _previews)
            if (p != null) p.SetActive(true);

        UpdateHighlight(_currentSelection);
        IsVisible = true;
    }

    private void Hide()
    {
        foreach (var p in _previews)
            if (p != null) p.SetActive(false);
        IsVisible = false;
    }

    private void BuildPreviews()
    {
        foreach (var p in _previews)
            if (p != null) Destroy(p);
        _previews.Clear();

        int count = _corals.CoralCount;
        for (int i = 0; i < count; i++)
        {
            GameObject prefab = _corals.GetCoralPrefab(i);
            GameObject preview = prefab != null ? BuildFromPrefab(prefab, i) : BuildFallback(i);
            preview.name = $"CoralPreview_{i}";
            preview.SetActive(false);
            _previews.Add(preview);
        }
        _built = true;
    }

    private GameObject BuildFromPrefab(GameObject prefab, int index)
    {
        // Instantiate inactive (Awake runs synchronously; Start is deferred — disable all MBs
        // before next frame so the L-System growth in Start() never fires)
        GameObject preview = Instantiate(prefab);

        foreach (var mb in preview.GetComponentsInChildren<MonoBehaviour>(true))
        {
            mb.StopAllCoroutines();
            mb.enabled = false;
        }

        foreach (var col in preview.GetComponentsInChildren<Collider>(true))
            Destroy(col);

        SetLayerRecursive(preview, 2); // Ignore Raycast

        // Fallback sphere if the prefab has no baked mesh
        if (!HasVisibleMesh(preview))
        {
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Destroy(sphere.GetComponent<Collider>());
            sphere.transform.SetParent(preview.transform, false);
            sphere.transform.localPosition = Vector3.zero;
            sphere.transform.localScale = Vector3.one;
            sphere.layer = 2;
            sphere.GetComponent<Renderer>().material.color =
                Color.HSVToRGB(index / (float)Mathf.Max(_corals.CoralCount, 1), 0.7f, 0.9f);
        }

        return preview;
    }

    private GameObject BuildFallback(int index)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(go.GetComponent<Collider>());
        go.layer = 2;
        go.GetComponent<Renderer>().material.color =
            Color.HSVToRGB(index / (float)Mathf.Max(_corals.CoralCount, 1), 0.7f, 0.9f);
        return go;
    }

    private void PositionRing()
    {
        if (playerTransform == null) return;
        Vector3 center = playerTransform.position + Vector3.up * 1.2f;
        int count = _previews.Count;
        for (int i = 0; i < count; i++)
        {
            if (_previews[i] == null) continue;
            float rad = 360f / count * i * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad)) * ringRadius;
            _previews[i].transform.position = center + offset;
            _previews[i].transform.rotation = Quaternion.identity;
            _previews[i].transform.localScale = Vector3.one * previewScale;
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

    private static bool HasVisibleMesh(GameObject go)
    {
        foreach (var mf in go.GetComponentsInChildren<MeshFilter>(true))
            if (mf.sharedMesh != null && mf.sharedMesh.vertexCount > 0) return true;
        return false;
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
