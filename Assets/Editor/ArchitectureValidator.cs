using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public static class ArchitectureValidator
{
    [MenuItem("TiltFive/Validate Architecture")]
    public static void Validate()
    {
        var results = new List<string>();
        bool allPassed = true;

        // 1 — GameLifetimeScope in scene
        var lifetimeScope = Object.FindObjectOfType<GameLifetimeScope>();
        if (lifetimeScope != null)
            results.Add("✓ GameLifetimeScope found in scene");
        else
        {
            results.Add("✗ GameLifetimeScope NOT found in scene");
            allPassed = false;
        }

        // 2 — Trash-tagged objects have TrashPickupHandler
        var trashObjects = GameObject.FindGameObjectsWithTag("Trash");
        int missingHandler = 0;
        foreach (var t in trashObjects)
            if (t.GetComponentInParent<TrashPickupHandler>() == null)
                missingHandler++;

        if (missingHandler == 0)
            results.Add($"✓ All {trashObjects.Length} Trash objects have TrashPickupHandler");
        else
        {
            results.Add($"✗ {missingHandler}/{trashObjects.Length} Trash objects missing TrashPickupHandler");
            allPassed = false;
        }

        // 3 — Exactly one PickupInputHandler
        var handlers = Object.FindObjectsOfType<PickupInputHandler>();
        if (handlers.Length == 1)
            results.Add("✓ Exactly one PickupInputHandler in scene");
        else
        {
            results.Add($"✗ PickupInputHandler count: {handlers.Length} (expected 1)");
            allPassed = false;
        }

        // 4 — All service interfaces bound in GameLifetimeScope (static source check)
        string scopePath = "Assets/Scripts/GameLifetimeScope.cs";
        if (File.Exists(scopePath))
        {
            string src = File.ReadAllText(scopePath);
            string[] required = {
                "IWellnessService", "IAudioService", "IInputService",
                "ICoralSpawnService", "ITrashSpawnService", "IColorEffectService"
            };
            foreach (string iface in required)
            {
                if (Regex.IsMatch(src, iface))
                    results.Add($"✓ {iface} referenced in GameLifetimeScope");
                else
                {
                    results.Add($"✗ {iface} NOT found in GameLifetimeScope");
                    allPassed = false;
                }
            }
        }
        else
        {
            results.Add($"✗ GameLifetimeScope.cs not found at {scopePath}");
            allPassed = false;
        }

        string summary = string.Join("\n", results);
        Debug.Log("[ArchitectureValidator]\n" + summary);

        string status = allPassed ? "ALL CHECKS PASSED ✓" : "SOME CHECKS FAILED ✗";
        EditorUtility.DisplayDialog("Architecture Validation", $"{status}\n\n{summary}", "OK");
    }
}
