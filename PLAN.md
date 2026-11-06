# GT7 ReefGame — Refactoring & Entwicklungsplan

**Kurs:** GT7 Unity3D Game Development Studio, HTW Berlin, WS 2026/27  
**Plattform:** PC (itch.io)  
**Branch:** `refactor/vcontainer-clean-arch`

---

## Abgabe 1 — Early January 2027 (40%)

### Block 0 — Pitch (diese Woche)
- [ ] Pitch-Dokument: Spielkonzept, Zielplattform, Architekturdiagramm

### Block A — VContainer Setup
- [x] VContainer zu `Packages/manifest.json` hinzugefügt
- [ ] `GameLifetimeScope` anlegen (`Assets/Scripts/Core/GameLifetimeScope.cs`)
- [ ] Unity öffnen & VContainer-Import bestätigen (kein Compile-Fehler)

### Block B — Interfaces
- [ ] `IWellnessService` — Score berechnen, `OnScoreChanged`-Event
- [ ] `ICoralSpawnService` — `SpawnCoral()`, `GetScore()`
- [ ] `ITrashSpawnService` — `SpawnTrash()`, `GetScore()`
- [ ] `IAudioService` — `SetMusicParameter()`, `PlayOneShot()`
- [ ] `IInputService` — `IsPickupTriggered()`, `GetPointerRay()`
- [ ] `IColorEffectService` — `TriggerBleach()`, `TriggerRecolor()`

### Block C — Implementierungen
- [ ] `WellnessService` — reine C#-Klasse, kein MonoBehaviour
- [ ] `StubAudioService` — No-op Implementierung (kein FMOD nötig für PC-Build)
- [ ] `PCInputService` — Maus/Keyboard ersetzt TiltFive
- [ ] `CoralColorEffectService` — aus `CoralColorManager` extrahiert

### Block D — MonoBehaviours refactoren
- [ ] `RiffGameLoop` — ersetzt `RiffController` (nur Timer, injizierte Services)
- [ ] `TrashPickupHandler` — ersetzt `DestroyObject` (kein TiltFive-Import)
- [ ] `PlayerController` — eine saubere Klasse aus den zwei `PlayerControll`-Duplikaten

### Block E — Replayability
- [ ] Restart-Mechanismus (Score reset → neue Runde starten)
- [ ] Schwierigkeitswahl beim Start (Easy/Medium/Hard → andere `trashValue`/`coralValue`)
- [ ] Zufälliger Seed für Spawn-Positionen (via `ISpawnConfig` ScriptableObject)

### Block F — Git & GitHub
- [x] Neues GitHub-Repo angelegt: `flrnbr1998/GT7-ReefGame`
- [x] `main` branch = stabiler Stand
- [x] `refactor/vcontainer-clean-arch` = Arbeits-Branch
- [ ] GitHub Project Board anlegen (Kanban: Backlog / In Progress / Done)
- [ ] PRs für jeden Block mergen

---

## Abgabe 2 — Late February 2027 (60%)

### UI / Juice
- [ ] Wellness-Score HUD (Anzeige im Spiel)
- [ ] Spielstart-Screen
- [ ] Game-Over-Screen (Score == 0)
- [ ] Bleaching-Screen-Effekt (roter Vignette-Flash)
- [ ] Coral-Pickup-Feedback (Partikel, Sound)

### Persistent Data
- [ ] Highscore speichern (JSON / PlayerPrefs)
- [ ] Lautstärke-Einstellungen persistent

### CI/CD
- [ ] GitHub Actions: Unity Build bei Push auf `main`
- [ ] itch.io Upload via Butler automatisieren

### Optional
- [ ] ScriptableObject Remote Config (Spawn-Werte ohne Rebuild)
- [ ] Analytics-Events ("Runde gestartet", "Score X", "Bleaching ausgelöst")

---

## Architektur-Übersicht (Zielzustand)

```
GameLifetimeScope (VContainer)
├── WellnessService          : IWellnessService
├── StubAudioService         : IAudioService
├── PCInputService           : IInputService
├── CoralColorEffectService  : IColorEffectService
├── CoralSpawnService        : ICoralSpawnService
└── TrashSpawnService        : ITrashSpawnService

MonoBehaviours (dünn, via [Inject]):
├── RiffGameLoop             ← Orchestrator
├── TrashPickupHandler       ← Pickup-Interaktion
└── PlayerController         ← Bewegung
```

**Wellness-Score-Flow:**
```
WellnessService.Recalculate()
  → OnScoreChanged event
    ├── RiffGameLoop        → Spawn-Intervalle anpassen
    ├── StubAudioService    → Musik-Parameter setzen
    ├── CoralColorEffect    → Bleaching / Recoloring triggern
    └── FogEffectService    → Nebeldichte anpassen
```
