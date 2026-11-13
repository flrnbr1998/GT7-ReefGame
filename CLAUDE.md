# PS_TiltFive_Riff — CLAUDE.md

## Projekt-Übersicht

Interaktives Riff-Erlebnis. Der Spieler bereinigt einen virtuellen Korallenriff von Müll und beobachtet, wie sich der Wellness-Score des Riffs verändert. Korallen wachsen prozedural per L-System.

**Unity:** 2021.3.11f1 LTS  
**Render Pipeline:** Built-in (kein URP)  
**Plattform:** PC (Keyboard + Maus)

---

## Wichtige Abhängigkeiten

| Paket | Speicherort | Hinweis |
|-------|-------------|---------|
| TiltFive SDK | `Assets/Tilt Five/` + `Assets/Plugins/` | ~~AR-Brille~~ — nicht mehr Zielplattform, Dateien bleiben aber im Repo |
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
├── PS_TiltFive.Audio      — StubAudioService (Entwicklung/Test), AudioManager/FMODEvents (FMOD-Build)
├── PS_TiltFive.Input      — PCInputService (Keyboard + Maus)
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

## PC-Eingabe

Zielplattform ist **PC (Keyboard + Maus)**. `PCInputService` ist die produktive IInputService-Implementierung.

Alte TiltFive-Skripte (`PlayerControll.cs`, `DestroyObject.cs`) sind noch im Repo — enthalten `TiltFive.Input`-Aufrufe, dürfen nicht gebaut werden. Werden im nächsten Refactor-Schritt gelöscht.

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
- `PlayerControll.cs` enthält TiltFive-SDK-Aufrufe — nicht mehr Zielplattform, löschen sobald Scene-Audit bestätigt dass kein Prefab mehr darauf zeigt
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

## Code-Qualität (SOLID / KISS / DRY)

### Prinzipien im Überblick

| Prinzip | Status | Wichtigste Regeln |
|---------|--------|-------------------|
| **S** Single Responsibility | ⚠️ teilweise | `TerrainSpawner` (3 Verantwortlichkeiten), `RiffGameLoop` (Fog + Music + Score-Reset) |
| **O** Open/Closed | ✅ | CoralGrowthBase-Hierarchie offen für Erweiterung per Subklasse |
| **L** Liskov | ✅ | Alle Subklassen von CoralGrowthBase substituierbar; StubAudioService vollständig |
| **I** Interface Segregation | ✅ | Kleine Interfaces (IWellnessService 2 Methoden, IInputService 2 Methoden) |
| **D** Dependency Inversion | ⚠️ bekannte Verstöße | Siehe TODO-Kommentare unten |

### Bekannte D-Verstöße (mit TODO markiert)

- `TrashSpawnerScript.cs:44` — `AudioManager.instance.PlayOneShot(...)` direkt → muss über `IAudioService` laufen
- `CoralGrowthBase.cs:129` — `CoralColorManager.RegisterColorable(this)` → muss über injected `IColorEffectService` laufen

### Verbindliche Coding-Regeln

1. **Keine Editor-using in Runtime-Skripten** — nur in `Assets/Editor/` oder hinter `#if UNITY_EDITOR`
2. **Interfaces immer in `Core/Interfaces/`** — nie im Root oder in Gameplay-Ordnern
3. **Kein `FindObjectOfType<ConcreteType>()`** für injizierbare Services — `[Inject]` verwenden
4. **Kein Singleton-Zugriff** (`AudioManager.instance`, `FMODEvents.instance`) in neuen Scripts — IAudioService per DI
5. **Keine direkte MonoBehaviour-Abhängigkeit** in Services — nur Interface-Typen
6. **Private Felder:** `_camelCase`-Konvention (wie in `PlayerController.cs`)
7. **Keine Kommentare** außer bei nicht-offensichtlichem Why (Einschränkung, Workaround, Invariante)

### DRY-Checkliste

- **Camera-Zugriff:** `Camera.main ?? Camera.allCameras[0]` — an zentraler Stelle (CameraHelper) nicht duplizieren
- **Spawn-Logik:** Kein Copy-Paste zwischen Coral- und Trash-Spawner
- **PlayerControll.cs** — veraltet; `PlayerController.cs` ist der Ersatz. `PlayerControll1.cs` gelöscht (war vereinfachte Kopie ohne Terrain-Height-Sampling)

---

## Git

Aktiver Branch: `refactor/vcontainer-clean-arch`  
Main-Branch: `main` — vor Merge `git pull origin main` ausführen
