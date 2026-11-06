using UnityEngine;

[CreateAssetMenu(fileName = "WellnessConfig", menuName = "ReefGame/WellnessConfig")]
public class WellnessConfig : ScriptableObject
{
    [Range(0.1f, 10f)] public float coralValue = 1f;
    [Range(0.1f, 10f)] public float trashValue = 6f;
    [Range(0f, 2f)]    public float bleachThreshold = 0.8f;
    [Range(0f, 2f)]    public float recolorThreshold = 1.1f;
    public float spawnCoralBase = 8f;
    public float spawnTrashBase = 16f;
}
