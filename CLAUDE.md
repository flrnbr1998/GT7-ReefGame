# PS_TiltFive_Riff — CLAUDE.md

## Projekt-Übersicht

Interaktives Riff-Erlebnis für das TiltFive AR-Board. Der Spieler bereinigt einen virtuellen Korallenriff von Müll und beobachtet, wie sich der Wellness-Score des Riffs verändert. Korallen wachsen prozedural per L-System.

**Unity:** 2021.3.11f1 LTS  
**Render Pipeline:** Built-in (kein URP)  
**Plattform:** TiltFive AR-Brille + Wand-Controller

---

## Wichtige Abhängigkeiten

| Paket | Speicherort | Hinweis |
|-------|-------------|---------|
| TiltFive SDK | `Assets/Tilt Five/` + `Assets/Plugins/` | AR-Brille + Wand-Input |
| FMOD Studio | `Assets/Plugins/FMOD/` | Audio-Engine |
| UniGLTF | `Assets/UniGLTF/` | GLTF-Tier-Modelle (Schildkröte, Hai, Krabbe, Rochen) |
| VRMShaders | `Assets/VRMShaders/` | Abhängigkeit von UniGLTF |

**Niemals** `Assets/Tilt Five/`, `Assets/Plugins/FMOD/`, `Assets/UniGLTF/`, `Assets/VRMShaders/` anfassen — diese sind binäre SDK-Dateien.

---

## Architektur

```
RiffController          — Spielschleife, Wellness-Score, Spawn-Timing
├── TerrainSpawner      — Korallen auf dem Terrain spawnen (via TerrainLayer-Textur-Erkennung)
├── TrashSpawnerScript  — Müll spawnen
├── CoralColorManager   — Bleaching / Recoloring der Korallen
└── AudioManager        — FMOD-Wrapper (Singleton)

PlayerControll          — TiltFive Wand-Bewegung auf dem Terrain
DestroyObject           — Müll-Objekt: fliegt bei Wand-Trigger zur Kamera und verschwindet
```

### Wellness-Formel

```
wellnessScore = 1f + (coralScore * 0.01 * coralValue) - (trashScore * 0.01 * trashValue)
```

- Startwert: `1.0` (neutral)
- Bereich: `0.0` – `2.0`
- `< 0.8` → Bleaching-Effekt ausgelöst
- `> 1.1` → Recoloring-Effekt ausgelöst
- `== 0` → alle DestroyObject-Objekte werden nach 60s zerstört, Score reset auf 1.0

### Spawn-Timing (dynamisch)

```
spawnCoralIntervalCalculated = spawnCoralInterval + wellnessScore
spawnTrashIntervalCalculated = spawnTrashInterval - wellnessScore
```

Je schlechter der Score, desto schneller spawnt Müll und langsamer Korallen.

---

## L-System Korallenwachstum

`Assets/GrowingCorals Kopie/` — Verschiedene Korallen-Typen:

| Klasse | Datei | Wachstums-Art |
|--------|-------|---------------|
| `CoralGrowthBase` | `CoralGrowthBase.cs` | Basisklasse |
| `BubbleCoralGrowth` | `BubbleCoralGrowth.cs` | Kugelförmig |
| `RandomCoralGrowth` | `RandomCoralGrowth.cs` | Zufällig |
| `TreeCoralGrowth` | `TreeCoralGrowth.cs` | Baumartig |
| `StaghornCoralGrowth` | `Stackhorn/TreeGrowth.cs` | Geweih-Koralle |

---

## TiltFive-Eingabe

`PlayerControll.cs` / `PlayerControll1.cs` verwenden:
- `TiltFive.Input.GetButton(TiltFive.Input.WandButton.Trigger, TiltFive.PlayerIndex.One)`
- `TiltFive.Wand.TryGetWandDevice(...)` für Wand-Position/-Rotation

Bei Portierung ohne TiltFive: diese durch Mouse/Keyboard-Input ersetzen (siehe GT7_Project `PlayerWandControl.cs` und `DestroyObject.cs` als Referenz).

---

## Audio (FMOD)

`AudioManager.cs` und `FMODEvents.cs` sind FMOD-abhängig.  
Bei Portierung ohne FMOD: durch Do-Nothing-Stubs ersetzen (Referenz: GT7_Project).

Musik-Paramter `"MusicToxic"` hat 10 Werte (A–J), gesteuert durch Wellness-Score-Schwellenwerte.

---

## Terrain

`Assets/Terrain/` — Terrain-Layers:
- `SandLayer.terrainlayer` → `SandAlbedo.psd`
- `RockLayer.terrainlayer` → `Rock_BaseColor.tif`

`TerrainSpawner.cs` liest den Textur-Index an einer Position (`terrainData.terrainLayers`) um zu bestimmen, ob dort Korallen wachsen dürfen.

---

## Szenen

| Szene | Inhalt |
|-------|--------|
| `Riff.unity` | **Hauptszene** — vollständiges Riff-Erlebnis |
| `L-System_Scene.unity` | Testszene für L-System Korallen |
| `StackhornCoral.unity` | Testszene für Staghorn-Koralle |
| `DinoScene.unity` | Alte Testszene |

---

## Bekannte Probleme / TODOs

- `Spawner.cs` enthält `using static UnityEditor.PlayerSettings;` — nur im Editor-Kontext funktional, nicht im Build
- `PlayerControll.cs` und `PlayerControll1.cs` sind redundant (zwei fast-identische Versionen)
- Tierprefabs (Schildkröte, Hai, Krabbe, Rochen) sind UniGLTF-abhängig

---

## Git

Branch: `main` (ist hinter `origin/main` — vor Arbeit `git pull` ausführen)  
Uncommitted: `Assets/Scenes/Riff.unity`, `Assets/Shaders/Hidden_UnderwaterEffect.mat`
