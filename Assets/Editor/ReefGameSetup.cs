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
        MigrateRiffController();
        MigratePlayerController();
        EditorUtility.SetDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects()[0]);
        Debug.Log("[ReefGame] Scene setup complete. Save the scene (Ctrl+S) to persist changes.");
    }

    // ── New objects ──────────────────────────────────────────────────────────

    private static void SetupLifetimeScope()
    {
        if (Object.FindObjectOfType<GameLifetimeScope>() != null)
        {
            Debug.Log("[ReefGame] GameLifetimeScope already in scene.");
            return;
        }

        var go = new GameObject("GameLifetimeScope");
        var scope = go.AddComponent<GameLifetimeScope>();
        AssignWellnessConfig(scope, "wellnessConfig");
        Undo.RegisterCreatedObjectUndo(go, "Setup GameLifetimeScope");
        Debug.Log("[ReefGame] GameLifetimeScope added.");
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
        Debug.Log("[ReefGame] CoralColorEffectService added.");
    }

    // ── Migrations ───────────────────────────────────────────────────────────

    private static void MigrateRiffController()
    {
        var old = Object.FindObjectOfType<RiffController>();
        if (old == null)
        {
            if (Object.FindObjectOfType<RiffGameLoop>() != null)
                Debug.Log("[ReefGame] RiffGameLoop already in scene.");
            else
                Debug.LogWarning("[ReefGame] No RiffController found — add RiffGameLoop manually.");
            return;
        }

        var go = old.gameObject;
        Undo.AddComponent<RiffGameLoop>(go);
        AssignWellnessConfig(go.GetComponent<RiffGameLoop>(), "config");

        Undo.DestroyObjectImmediate(old);
        Debug.Log("[ReefGame] RiffController → RiffGameLoop on '" + go.name + "'.");
    }

    private static void MigratePlayerController()
    {
        // Handle both PlayerControll and PlayerControll1
        MigratePlayerVariant<PlayerControll>();
        MigratePlayerVariant<PlayerControll1>();
    }

    private static void MigratePlayerVariant<T>() where T : MonoBehaviour
    {
        var old = Object.FindObjectOfType<T>();
        if (old == null) return;

        var go = old.gameObject;
        if (go.GetComponent<PlayerController>() != null)
        {
            Undo.DestroyObjectImmediate(old);
            Debug.Log("[ReefGame] Removed duplicate " + typeof(T).Name + " from '" + go.name + "'.");
            return;
        }

        var newComp = Undo.AddComponent<PlayerController>(go);

        // Copy speed field
        var oldSo  = new SerializedObject(old);
        var newSo  = new SerializedObject(newComp);
        newSo.FindProperty("speed").floatValue = oldSo.FindProperty("speed").floatValue;

        // Copy terrain reference if present
        var terrainProp = oldSo.FindProperty("terrain");
        if (terrainProp != null)
            newSo.FindProperty("terrain").objectReferenceValue = terrainProp.objectReferenceValue;

        newSo.ApplyModifiedProperties();
        Undo.DestroyObjectImmediate(old);
        Debug.Log("[ReefGame] " + typeof(T).Name + " → PlayerController on '" + go.name + "'.");
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static void AssignWellnessConfig(MonoBehaviour target, string propertyName)
    {
        var config = AssetDatabase.LoadAssetAtPath<WellnessConfig>(ConfigPath);
        if (config == null) return;

        var so = new SerializedObject(target);
        var prop = so.FindProperty(propertyName);
        if (prop != null)
        {
            prop.objectReferenceValue = config;
            so.ApplyModifiedProperties();
        }
    }
}
