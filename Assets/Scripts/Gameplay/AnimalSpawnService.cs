using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class AnimalSpawnService : MonoBehaviour, IAnimalSpawnService
{
    [SerializeField] private GameObject crabPrefab;
    [SerializeField] private GameObject turtlePrefab;
    [SerializeField] private GameObject mantaRayPrefab;
    [SerializeField] private Transform[] spawnPoints;

    [Inject] private IWellnessService _wellness;

    public event Action<AnimalType> OnAnimalSpawned;
    public event Action<AnimalType> OnAnimalDespawned;

    private readonly Dictionary<AnimalType, GameObject> _active = new Dictionary<AnimalType, GameObject>();

    private void Start()
    {
        _wellness.OnScoreChanged += OnScoreChanged;
    }

    private void OnDestroy() => _wellness.OnScoreChanged -= OnScoreChanged;

    private void OnScoreChanged(float score)
    {
        TryManageAnimal(AnimalType.Crab,     score >= 1.2f, crabPrefab);
        TryManageAnimal(AnimalType.Turtle,   score >= 1.4f, turtlePrefab);
        TryManageAnimal(AnimalType.MantaRay, score >= 1.6f, mantaRayPrefab);
    }

    private void TryManageAnimal(AnimalType type, bool shouldExist, GameObject prefab)
    {
        bool exists = _active.TryGetValue(type, out GameObject current) && current != null;

        if (shouldExist && !exists)
        {
            if (prefab == null) return;
            Vector3 pos = spawnPoints != null && spawnPoints.Length > 0
                ? spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].position
                : transform.position;

            _active[type] = Instantiate(prefab, pos, Quaternion.identity);
            OnAnimalSpawned?.Invoke(type);
        }
        else if (!shouldExist && exists)
        {
            Destroy(current);
            _active.Remove(type);
            OnAnimalDespawned?.Invoke(type);
        }
    }
}
