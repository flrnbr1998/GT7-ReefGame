using UnityEngine;
using VContainer;

public class CoralPlantHandler : MonoBehaviour
{
    [SerializeField] private int sandTextureIndex = 0;
    [SerializeField] private Terrain terrain;

    [Inject] private ICoralSpawnService _corals;
    [Inject] private IWellnessService _wellness;
    [Inject] private ITrashSpawnService _trash;
    [Inject] private IFeedbackService _feedback;

    private Camera _cam;
    private GameObject _cursor;
    private Renderer _cursorRenderer;
    private Material _cursorMat;
    private float _cooldownUntil;
    private int _selectedIndex;

    private static readonly Color _colorValid   = new Color(0f, 1f, 0f, 0.4f);
    private static readonly Color _colorInvalid = new Color(1f, 0f, 0f, 0.4f);

    private void Start()
    {
        _cam = PS_TiltFive.Core.CameraHelper.Main;

        if (terrain == null)
        {
            terrain = Object.FindObjectOfType<Terrain>();
            if (terrain == null)
                Debug.LogWarning("[CoralPlantHandler] No Terrain found in scene.");
        }

        BuildCursor();
        SyncSelection(0);

        if (_corals.CoralCount == 0)
            Debug.LogWarning("[CoralPlantHandler] ICoralSpawnService reports 0 coral prefabs — assign corals array in TerrainSpawner inspector.");
        else
            Debug.Log($"[CoralPlantHandler] Ready — {_corals.CoralCount} coral type(s) available.");
    }

    private void BuildCursor()
    {
        _cursor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _cursor.name = "CoralPlacementCursor";
        Object.Destroy(_cursor.GetComponent<Collider>());
        _cursor.transform.localScale = Vector3.one * 0.5f;

        _cursorRenderer = _cursor.GetComponent<Renderer>();
        _cursorMat = new Material(Shader.Find("Standard"));
        _cursorMat.SetFloat("_Mode", 3f);
        _cursorMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        _cursorMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        _cursorMat.SetInt("_ZWrite", 0);
        _cursorMat.EnableKeyword("_ALPHABLEND_ON");
        _cursorMat.renderQueue = 3000;
        _cursorRenderer.material = _cursorMat;
        _cursor.SetActive(false);
    }

    private void Update()
    {
        if (_cam == null) _cam = PS_TiltFive.Core.CameraHelper.Main;
        if (_cam == null) return;

        HandleInventoryInput();

        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        bool foundTerrain = FindTerrainHit(ray, out RaycastHit terrainHit);

        if (foundTerrain)
        {
            _cursor.SetActive(true);
            _cursor.transform.position = terrainHit.point + Vector3.up * 0.25f;
            _cursorMat.color = _colorValid;

            if (Input.GetMouseButtonDown(1) && Time.time >= _cooldownUntil)
            {
                _corals.SpawnCoralAt(terrainHit.point);
                _wellness.Recalculate(_corals.GetScore(), _trash.GetScore());
                _feedback?.PlayPickupFeedback(terrainHit.point);
                _cooldownUntil = Time.time + 0.3f;
            }
        }
        else
        {
            _cursor.SetActive(false);
        }
    }

    private static bool FindTerrainHit(Ray ray, out RaycastHit terrainHit)
    {
        RaycastHit[] hits = Physics.RaycastAll(ray, 200f);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        foreach (RaycastHit h in hits)
        {
            if (h.collider.GetComponentInParent<Terrain>() != null)
            {
                terrainHit = h;
                return true;
            }
        }
        terrainHit = default;
        return false;
    }

    private void HandleInventoryInput()
    {
        if (!CoralRingInventory.IsVisible) return;
        int count = _corals.CoralCount;
        if (count == 0) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            SyncSelection((_selectedIndex + 1) % count);
            Debug.Log($"[CoralPlantHandler] Selected: {_corals.GetCoralName(_selectedIndex)} ({_selectedIndex})");
        }
        else if (scroll < 0f)
        {
            SyncSelection((_selectedIndex - 1 + count) % count);
            Debug.Log($"[CoralPlantHandler] Selected: {_corals.GetCoralName(_selectedIndex)} ({_selectedIndex})");
        }

        // Number keys 1–9
        for (int i = 0; i < Mathf.Min(count, 9); i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                SyncSelection(i);
        }
    }

    public static event System.Action<int> OnSelectionChanged;

    private void SyncSelection(int index)
    {
        _selectedIndex = index;
        _corals.SelectCoral(index);
        OnSelectionChanged?.Invoke(index);
    }

    private bool IsSandAt(Vector3 worldPos)
    {
        if (terrain == null)
        {
            Debug.LogWarning("[CoralPlantHandler] Terrain not assigned — sand check skipped.");
            return false;
        }

        TerrainData td = terrain.terrainData;
        Vector3 local = worldPos - terrain.transform.position;
        int px = Mathf.RoundToInt(Mathf.Clamp01(local.x / td.size.x) * (td.alphamapWidth  - 1));
        int py = Mathf.RoundToInt(Mathf.Clamp01(local.z / td.size.z) * (td.alphamapHeight - 1));

        float[,,] maps = td.GetAlphamaps(px, py, 1, 1);
        return maps[0, 0, sandTextureIndex] > 0.5f;
    }

    private void OnDestroy()
    {
        if (_cursor != null) Object.Destroy(_cursor);
        if (_cursorMat != null) Object.Destroy(_cursorMat);
    }
}
