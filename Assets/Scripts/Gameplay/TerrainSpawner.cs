using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

public class TerrainSpawner : MonoBehaviour, ICoralSpawnService
{
    public Terrain terrain;
    public int textureIndex;

    public GameObject[] corals;
    public GameObject player;
    public float maxDistance = 10;

    [Inject] private IColorEffectService _colorEffect;

    private List<Vector3> _positions;
    private readonly List<GameObject> _createdCorals = new List<GameObject>();

    void Start()
    {
        _positions = new TerrainTextureQuery(terrain, textureIndex).GetSpawnablePositions();
    }

    public GameObject SpawnCoral() => SpawnAt(GetRandomPosition());

    public GameObject SpawnCoralAt(Vector3 worldPosition) => SpawnAt(worldPosition);

    private GameObject SpawnAt(Vector3 spawnPos)
    {
        GameObject obj = Instantiate(corals[UnityEngine.Random.Range(0, corals.Length)], spawnPos, Quaternion.identity);
        obj.transform.SetParent(transform);

        ISpawnable spawnable = obj.GetComponent<ISpawnable>();
        if (spawnable == null)
        {
            Debug.LogWarning("[TerrainSpawner] Spawned coral has no ISpawnable.");
            return null;
        }

        spawnable.SetStartPosition(spawnPos);
        spawnable.SetParent(obj);
        _createdCorals.Add(obj);
        obj.GetComponent<CoralGrowthBase>()?.SetColorEffectService(_colorEffect);

        MeshRenderer mr = obj.GetComponent<MeshRenderer>();
        if (mr != null) mr.enabled = true;

        return obj;
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 playerPos = player.transform.position;
        float maxSqr = maxDistance * maxDistance;

        List<Vector3> nearby = _positions
            .Where(p => (p - playerPos).sqrMagnitude <= maxSqr)
            .ToList();

        return nearby.Count > 0
            ? nearby[Random.Range(0, nearby.Count)]
            : _positions[UnityEngine.Random.Range(0, _positions.Count)];
    }

    public float GetScore()
    {
        for (int i = _createdCorals.Count - 1; i >= 0; i--)
            if (_createdCorals[i] == null) _createdCorals.RemoveAt(i);

        return _createdCorals.Count;
    }
}
