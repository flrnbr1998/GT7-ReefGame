using UnityEngine;

public interface ICoralSpawnService
{
    GameObject SpawnCoral();
    GameObject SpawnCoralAt(Vector3 worldPosition);
    float GetScore();
}
