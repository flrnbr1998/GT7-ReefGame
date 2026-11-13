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
        SetupPickupInputHandler();
        MigrateRiffController();
        MigratePlayerController();
        MigrateTrashPrefabs();
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

    private static void SetupPickupInputHandler()
    {
        if (Object.FindObjectOfType<PickupInputHandler>() != null)
        {
            Debug.Log("[ReefGame] PickupInputHandler already in scene.");
            return;
        }

        var go = new GameObject("PickupInputHandler");
        go.AddComponent<PickupInputHandler>();
        Undo.RegisterCreatedObjectUndo(go, "Setup PickupInputHandler");
        Debug.Log("[ReefGame] PickupInputHandler added.");
    }

    // ── Migrations ───────────────────────────────────────────────────────────

    private static void MigrateRiffController()
    {
        if (Object.FindObjectOfType<RiffGameLoop>() != null)
            Debug.Log("[ReefGame] RiffGameLoop already in scene.");
        else
            Debug.LogWarning("[ReefGame] RiffGameLoop not found — add it manually.");
    }

    private static void MigratePlayerController()
    {
        if (Object.FindObjectOfType<PlayerController>() != null)
            Debug.Log("[ReefGame] PlayerController already in scene.");
        else
            Debug.LogWarning("[ReefGame] PlayerController not found — add it manually.");
    }

    // ── Prefab migration ─────────────────────────────────────────────────────

    private static void MigrateTrashPrefabs()
    {
        // DestroyObject was already migrated to TrashPickupHandler in a previous commit.
        Debug.Log("[ReefGame] Trash prefab migration already complete.");
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
