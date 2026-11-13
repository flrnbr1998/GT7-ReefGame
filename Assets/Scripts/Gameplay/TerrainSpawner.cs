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

    private int _selectedIndex;

    public void SelectCoral(int index) => _selectedIndex = Mathf.Clamp(index, 0, corals.Length - 1);
    public int CoralCount => corals != null ? corals.Length : 0;
    public string GetCoralName(int index)
    {
        if (corals == null || index < 0 || index >= corals.Length) return string.Empty;
        return corals[index].name.Replace("Prefab", "").Replace("Object", "").Trim();
    }

    public GameObject GetCoralPrefab(int index)
    {
        if (corals == null || index < 0 || index >= corals.Length) return null;
        return corals[index];
    }

    public GameObject SpawnCoral() => SpawnAt(GetRandomPosition(), UnityEngine.Random.Range(0, corals.Length));

    public GameObject SpawnCoralAt(Vector3 worldPosition) => SpawnAt(worldPosition, _selectedIndex);

    private GameObject SpawnAt(Vector3 spawnPos, int coralIndex)
    {
        GameObject obj = Instantiate(corals[coralIndex], spawnPos, Quaternion.identity);
        obj.transform.SetParent(transform);

        ISpawnable spawnable = obj.GetComponent<ISpawnable>();
        if (spawnable != null)
        {
            spawnable.SetStartPosition(spawnPos);
            spawnable.SetParent(obj);
        }

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
