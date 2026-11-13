using UnityEngine;

public interface ICoralSpawnService
{
    GameObject SpawnCoral();
    GameObject SpawnCoralAt(Vector3 worldPosition);
    float GetScore();

    void SelectCoral(int index);
    int CoralCount { get; }
    string GetCoralName(int index);
    GameObject GetCoralPrefab(int index);
}
