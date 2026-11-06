using UnityEditor;
using UnityEngine;

public static class ReefGameSetup
{
    private const string ConfigPath = "Assets/Settings/WellnessConfig.asset";

    [InitializeOnLoadMethod]
    private static void EnsureWellnessConfigExists()
    {
        if (AssetDatabase.LoadAssetAtPath<WellnessConfig>(ConfigPath) != null) return;

        System.IO.Directory.CreateDirectory("Assets/Settings");
        var config = ScriptableObject.CreateInstance<WellnessConfig>();
        AssetDatabase.CreateAsset(config, ConfigPath);
        AssetDatabase.SaveAssets();
        Debug.Log("[ReefGame] WellnessConfig created at " + ConfigPath);
    }

    [MenuItem("ReefGame/Setup Scene")]
    private static void SetupScene()
    {
        SetupLifetimeScope();
        SetupCoralColorEffectService();
        Debug.Log("[ReefGame] Scene setup complete.");
    }

    private static void SetupLifetimeScope()
    {
        if (Object.FindObjectOfType<GameLifetimeScope>() != null)
        {
            Debug.Log("[ReefGame] GameLifetimeScope already in scene.");
            return;
        }

        var go = new GameObject("GameLifetimeScope");
        var scope = go.AddComponent<GameLifetimeScope>();

        var config = AssetDatabase.LoadAssetAtPath<WellnessConfig>(ConfigPath);
        if (config != null)
        {
            var so = new SerializedObject(scope);
            so.FindProperty("wellnessConfig").objectReferenceValue = config;
            so.ApplyModifiedProperties();
        }

        Undo.RegisterCreatedObjectUndo(go, "Setup GameLifetimeScope");
        Debug.Log("[ReefGame] GameLifetimeScope added to scene.");
    }

    private static void SetupCoralColorEffectService()
    {
        if (Object.FindObjectOfType<CoralColorEffectService>() != null)
        {
            Debug.Log("[ReefGame] CoralColorEffectService already in scene.");
            return;
        }

        var go = new GameObject("CoralColorEffectService");
        go.AddComponent<CoralColorEffectService>();
        Undo.RegisterCreatedObjectUndo(go, "Setup CoralColorEffectService");
        Debug.Log("[ReefGame] CoralColorEffectService added to scene.");
    }
}
