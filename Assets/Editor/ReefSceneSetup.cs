using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class ReefSceneSetup
{
    [MenuItem("ReefGame/Setup Phase 1 Scene")]
    public static void SetupScene()
    {
        SetupPlayerComponents();
        SetupServiceObjects();
        SetupHUD();
        SetupTrashPrefabs();

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("[ReefSetup] Done. Save with Ctrl+S. Check AnimalSpawnService in Inspector for missing animal prefabs.");
    }

    // ── Player ──────────────────────────────────────────────────────────────

    static void SetupPlayerComponents()
    {
        var pc = Object.FindObjectOfType<PlayerController>();
        if (pc == null) { Debug.LogError("[ReefSetup] PlayerController not found — open Riff.unity first."); return; }

        GameObject p = pc.gameObject;
        AddIfMissing<TrashHighlightHandler>(p);

        if (p.GetComponent<CoralPlantHandler>() == null)
        {
            var cph = Undo.AddComponent<CoralPlantHandler>(p);
            Terrain t = Object.FindObjectOfType<Terrain>();
            if (t != null) SetField(cph, "terrain", t);
        }

        Debug.Log($"[ReefSetup] Player '{p.name}': TrashHighlightHandler + CoralPlantHandler done.");
    }

    // ── Service GameObjects ──────────────────────────────────────────────────

    static void SetupServiceObjects()
    {
        EnsureEventSystem();

        if (Object.FindObjectOfType<GameSessionController>() == null)
            CreateGO("GameSessionController").AddComponent<GameSessionController>();

        if (Object.FindObjectOfType<AnimalSpawnService>() == null)
        {
            var svc = CreateGO("AnimalSpawnService").AddComponent<AnimalSpawnService>();
            AutoAssignAnimalPrefabs(svc);
        }

        if (Object.FindObjectOfType<PickupFeedbackService>() == null)
        {
            var go  = CreateGO("PickupFeedbackService");
            go.AddComponent<PickupFeedbackService>();
            // pickupEffect left null — assign a particle prefab with a valid material manually
        }

        Debug.Log("[ReefSetup] Service objects done.");
    }

    static void AutoAssignAnimalPrefabs(AnimalSpawnService svc)
    {
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
        foreach (string guid in guids)
        {
            string path  = AssetDatabase.GUIDToAssetPath(guid);
            string lower = path.ToLower();
            var    prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) continue;

            if (lower.Contains("crab"))                        SetField(svc, "crabPrefab",     prefab);
            else if (lower.Contains("turtle"))                 SetField(svc, "turtlePrefab",   prefab);
            else if (lower.Contains("manta") || lower.Contains("ray")) SetField(svc, "mantaRayPrefab", prefab);
        }
    }

    // ── HUD Canvas ───────────────────────────────────────────────────────────

    static void SetupHUD()
    {
        if (Object.FindObjectOfType<HUDController>() != null)
        { Debug.Log("[ReefSetup] HUDController already in scene, skipping."); return; }

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
                 ?? Resources.GetBuiltinResource<Font>("Arial.ttf");

        // ── Canvas root
        GameObject canvasGo = CreateGO("HUD_Canvas");
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGo.AddComponent<GraphicRaycaster>();
        HUDController hud = canvasGo.AddComponent<HUDController>();

        // ── Wellness panel (top-left)
        GameObject wp = UIPanel(canvasGo, "WellnessPanel",
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1),
            new Vector2(20, -20), new Vector2(340, 70));
        wp.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.55f);

        var sliderRoot = DefaultControls.CreateSlider(new DefaultControls.Resources());
        sliderRoot.name = "WellnessSlider";
        sliderRoot.transform.SetParent(wp.transform, false);
        var sliderRt = sliderRoot.GetComponent<RectTransform>();
        sliderRt.anchorMin = new Vector2(0, 1); sliderRt.anchorMax = new Vector2(1, 1);
        sliderRt.pivot = new Vector2(0.5f, 1);
        sliderRt.anchoredPosition = new Vector2(0, -5);
        sliderRt.sizeDelta = new Vector2(-20, 25);
        var slider = sliderRoot.GetComponent<Slider>();
        slider.minValue = 0f; slider.maxValue = 1f; slider.value = 0.5f;

        GameObject wLabel = UIText(wp, "WellnessLabel", "Wellness: 1.00", font, 18,
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0),
            new Vector2(0, 5), new Vector2(-20, 25));

        // ── Animal return popup (top-center)
        GameObject arLabel = UIText(canvasGo, "AnimalReturnLabel", "", font, 24,
            new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1),
            new Vector2(0, -100), new Vector2(700, 50));
        arLabel.GetComponent<Text>().color = new Color(0.3f, 1f, 0.5f);
        arLabel.SetActive(false);

        // ── End Screen panel (center)
        GameObject endPanel = UIPanel(canvasGo, "EndScreenPanel",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            Vector2.zero, new Vector2(440, 340));
        endPanel.GetComponent<Image>().color = new Color(0f, 0.08f, 0.18f, 0.92f);

        GameObject resultLabel  = UIText(endPanel, "ResultLabel",  "Riff gerettet!", font, 30,
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1),
            new Vector2(0, -24), new Vector2(-20, 50));
        GameObject timeLabel    = UIText(endPanel, "TimeLabel",    "Zeit: 0s",       font, 22,
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1),
            new Vector2(0, -100), new Vector2(-20, 36));
        GameObject animalsLabel = UIText(endPanel, "AnimalsLabel", "Tiere: 0",       font, 22,
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1),
            new Vector2(0, -148), new Vector2(-20, 36));

        var btnRoot = DefaultControls.CreateButton(new DefaultControls.Resources());
        btnRoot.name = "RestartButton";
        btnRoot.transform.SetParent(endPanel.transform, false);
        var btnRt = btnRoot.GetComponent<RectTransform>();
        btnRt.anchorMin = new Vector2(0.5f, 0); btnRt.anchorMax = new Vector2(0.5f, 0);
        btnRt.pivot = new Vector2(0.5f, 0);
        btnRt.anchoredPosition = new Vector2(0, 24);
        btnRt.sizeDelta = new Vector2(220, 54);
        btnRoot.GetComponent<Image>().color = new Color(0.1f, 0.55f, 0.2f);
        var btnText = btnRoot.GetComponentInChildren<Text>();
        if (btnText != null) { btnText.text = "Nochmal spielen"; btnText.font = font; btnText.fontSize = 20; }

        endPanel.SetActive(false);

        // ── Wire HUDController references
        var so = new SerializedObject(hud);
        so.FindProperty("wellnessSlider").objectReferenceValue    = slider;
        so.FindProperty("wellnessLabel").objectReferenceValue     = wLabel.GetComponent<Text>();
        so.FindProperty("animalReturnLabel").objectReferenceValue = arLabel.GetComponent<Text>();
        so.FindProperty("endScreenPanel").objectReferenceValue    = endPanel;
        so.FindProperty("endResultLabel").objectReferenceValue    = resultLabel.GetComponent<Text>();
        so.FindProperty("endTimeLabel").objectReferenceValue      = timeLabel.GetComponent<Text>();
        so.FindProperty("endAnimalsLabel").objectReferenceValue   = animalsLabel.GetComponent<Text>();
        so.FindProperty("restartButton").objectReferenceValue     = btnRoot.GetComponent<Button>();
        so.ApplyModifiedProperties();

        Debug.Log("[ReefSetup] HUD Canvas created and wired.");
    }

    // ── Trash Prefabs ────────────────────────────────────────────────────────

    static void SetupTrashPrefabs()
    {
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
        int count = 0;

        foreach (string guid in guids)
        {
            string path   = AssetDatabase.GUIDToAssetPath(guid);
            var    prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) continue;

            bool hasHandler = prefab.GetComponent<TrashPickupHandler>() != null
                           || prefab.GetComponentInChildren<TrashPickupHandler>() != null;
            if (!hasHandler) continue;
            if (prefab.GetComponent<TrashHighlight>() != null) continue;

            using var scope = new PrefabUtility.EditPrefabContentsScope(path);
            scope.prefabContentsRoot.AddComponent<TrashHighlight>();
            count++;
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"[ReefSetup] TrashHighlight added to {count} trash prefab(s).");
    }

    // ── Generic helpers ──────────────────────────────────────────────────────

    static void EnsureEventSystem()
    {
        if (Object.FindObjectOfType<EventSystem>() != null) return;
        var go = CreateGO("EventSystem");
        go.AddComponent<EventSystem>();
        go.AddComponent<StandaloneInputModule>();
    }

    static T AddIfMissing<T>(GameObject go) where T : Component
    {
        var c = go.GetComponent<T>();
        if (c == null) c = Undo.AddComponent<T>(go);
        return c;
    }

    static GameObject CreateGO(string name)
    {
        var go = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(go, "Create " + name);
        return go;
    }

    static void SetField(Object target, string fieldName, Object value)
    {
        var so   = new SerializedObject(target);
        var prop = so.FindProperty(fieldName);
        if (prop != null) { prop.objectReferenceValue = value; so.ApplyModifiedProperties(); }
        else Debug.LogWarning($"[ReefSetup] Field '{fieldName}' not found on {target.GetType().Name}");
    }

    // ── UI factory ───────────────────────────────────────────────────────────

    static GameObject UIPanel(GameObject parent, string name,
        Vector2 anchMin, Vector2 anchMax, Vector2 pivot,
        Vector2 anchoredPos, Vector2 size)
    {
        var go = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(go, "Create " + name);
        go.transform.SetParent(parent.transform, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchMin; rt.anchorMax = anchMax; rt.pivot = pivot;
        rt.anchoredPosition = anchoredPos; rt.sizeDelta = size;
        go.AddComponent<Image>();
        return go;
    }

    static GameObject UIText(GameObject parent, string name, string content, Font font, int fontSize,
        Vector2 anchMin, Vector2 anchMax, Vector2 pivot, Vector2 anchoredPos, Vector2 size)
    {
        var go = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(go, "Create " + name);
        go.transform.SetParent(parent.transform, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchMin; rt.anchorMax = anchMax; rt.pivot = pivot;
        rt.anchoredPosition = anchoredPos; rt.sizeDelta = size;
        var t = go.AddComponent<Text>();
        t.text = content; t.font = font; t.fontSize = fontSize;
        t.alignment = TextAnchor.MiddleCenter; t.color = Color.white;
        return go;
    }
}
