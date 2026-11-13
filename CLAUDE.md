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

Aktuelle Architektur: **VContainer Dependency Injection** (Clean Architecture, Branch `refactor/vcontainer-clean-arch`)

```
GameLifetimeScope          — VContainer-Composition-Root (Assets/Scripts/)
│
├── PS_TiltFive.Core       — Interfaces, Services, Config (kein VContainer-Import nötig)
│   ├── Interfaces/        — IWellnessService, ICoralSpawnService, ITrashSpawnService,
│   │                        IAudioService, IInputService, IColorEffectService,
│   │                        IColorable, ISpawnable, ICombinable
│   ├── Services/          — WellnessService (pure C#), CoralColorEffectService (MB)
│   └── Config/            — WellnessConfig (ScriptableObject)
│
├── PS_TiltFive.Gameplay   — RiffGameLoop, PlayerController, TrashPickupHandler, PickupInputHandler
├── PS_TiltFive.Audio      — StubAudioService (PC), AudioManager/FMODEvents (TiltFive-Build)
├── PS_TiltFive.Input      — PCInputService (PC), TiltFive-Impl noch ausstehend
│
└── Assembly-CSharp        — TerrainSpawner, TrashSpawnerScript, alte Scripts, GameLifetimeScope
```

**Alte Klassen (noch im Repo, ersetzt):** `RiffController` → `RiffGameLoop`, `PlayerControll` → `PlayerController`, `DestroyObject` → `TrashPickupHandler`, `CoralColorManager` → `CoralColorEffectService`

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

## Kurs-Anforderungen (GT7 Week 5)

### Abgabe-Termine
| Abgabe | Datum | Woche | Gewicht |
|--------|-------|-------|---------|
| Deliverable 1 | **08.01.2027** | Week 12 | 40 % |
| Deliverable 2 | **05.02.2027** | Week 16 | 60 % |

**Deliverable 1:** Core gameplay, Replayability, Architecture (VContainer/DI), Clean Code/SOLID, Git + GitHub Project Board  
**Deliverable 2:** UI/Animations/Juice, Meta-Game/Persistent Data, CI/CD, optional: Remote Config, Analytics/A-B-Tests

### Design Patterns (GoF → Unity-Mapping)

| Pattern | Kategorie | Unity-Umsetzung in diesem Projekt |
|---------|-----------|-----------------------------------|
| Singleton | Creational | `AudioManager` (FMOD-Wrapper); **vermeiden** für neue Services → DI bevorzugen |
| Prototype / Factory | Creational | Prefabs + `Instantiate()` in `TerrainSpawner` / `TrashSpawnerScript` |
| Object Pool | Creational | Noch nicht implementiert — sinnvoll für Trash-Prefabs bei hohem Spawn-Rate |
| Component | Structural | MonoBehaviours als Komponenten-Komposition |
| Observer | Behavioral | `IWellnessService.OnScoreChanged` (C# Event), VContainer-Injection statt UnityEvents |
| Command | Behavioral | Pickup-Aktion in `PickupInputHandler` / `TrashPickupHandler` |

Ressourcen: [Game Programming Patterns](https://gameprogrammingpatterns.com/contents.html) · [Unity SOLID eBook](https://unity.com/resources/design-patterns-solid-ebook)

### Domain Driven Design (DDD)

Das Projekt folgt einem layered DDD-Ansatz:

- **Ubiquitous Language:** Begriffe aus dem Riff-Domänen-Modell konsequent in Code verwenden: `Wellness`, `Coral`, `Trash`, `Bleach`, `Recolor` — nicht `health`, `plant`, `garbage`
- **Entities:** Korallen- und Müll-GameObjects (haben Identität + Lebenszyklus)
- **Value Objects:** `WellnessConfig` (unveränderliche Konfiguration)
- **Services:** `WellnessService`, `CoralColorEffectService` — zustandsbehaftete Domain-Logik ohne UI
- **Events:** `OnScoreChanged` → löst Bleaching/Recoloring/Audio-Änderungen aus (Domain Event)
- **Repository-Muster:** `TerrainSpawner` als Coral-Repository (kennt alle lebenden Korallen, gibt Score zurück)
- **Layered Architecture:** Core (Domain) → Gameplay/Audio/Input (Application) → Assembly-CSharp (Infrastructure/Composition Root)

Nächste Woche in der Vorlesung: **Event Storming + Physical Prototyping + Speccing**

---

## Git

Aktiver Branch: `refactor/vcontainer-clean-arch`  
Main-Branch: `main` — vor Merge `git pull origin main` ausführen
