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

    private void Update()
    {
        if (!Input.GetMouseButtonDown(1)) return;

        Camera cam = PS_TiltFive.Core.CameraHelper.Main;
        if (cam == null) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;
        if (hit.collider.GetComponentInParent<Terrain>() == null) return;
        if (!IsSandAt(hit.point)) return;

        _corals.SpawnCoralAt(hit.point);
        _wellness.Recalculate(_corals.GetScore(), _trash.GetScore());
        _feedback?.PlayPickupFeedback(hit.point);
    }

    private bool IsSandAt(Vector3 worldPos)
    {
        if (terrain == null) return true;

        TerrainData td = terrain.terrainData;
        Vector3 local = worldPos - terrain.transform.position;
        int px = Mathf.RoundToInt(Mathf.Clamp01(local.x / td.size.x) * (td.alphamapWidth  - 1));
        int py = Mathf.RoundToInt(Mathf.Clamp01(local.z / td.size.z) * (td.alphamapHeight - 1));

        float[,,] maps = td.GetAlphamaps(px, py, 1, 1);
        return maps[0, 0, sandTextureIndex] > 0.5f;
    }
}
