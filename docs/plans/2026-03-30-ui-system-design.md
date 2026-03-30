# Tower Defense UI System Design

**Goal:** Build a complete, polished UI system using Unity UI (uGUI) with full Tiny Swords art integration across 4 categories: HUD, Build Menu, Contextual UI, and Menus/Screens. Features without gameplay backing get disabled placeholder buttons.

**Tech:** Unity UI (uGUI), TextMeshPro, Tiny Swords UI Elements art pack, SO-first event-driven architecture.

---

## Architecture

- **SO Event Backbone + Prefab-per-Panel.** Each UI panel is a self-contained prefab with serialized SO references. No singletons, no FindObjectOfType, no cross-panel direct references.
- **Fix `IntVariable` to be event-driven** — add `OnValueChanged` callback so displays react to changes instead of polling in `Update()`.
- **One screen-space Canvas** for HUD/menus, **one world-space Canvas per enemy** for health bars.
- **Placement mode state** managed via a `SelectionState` SO — the build menu sets it, the game world reads it.

---

## 1. HUD (Heads-Up Display)

Screen layout (1920x1080 reference):

```
┌─────────────────────────────────────────────────────────┐
│ [🪙 Gold: 150]  [❤️ Lives: 20]  [Wave: 3/10]    [⏸][▶▶] │  ← Top bar
│                                                         │
│                    GAME WORLD                            │
│                                                         │
│  [Start Wave]                                           │  ← Bottom-left
│  ┌──────────────────────────────────────┐               │
│  │ [Archer 💰50] [Mage 💰100] [Bomb 💰75] │               │  ← Build menu bar
│  └──────────────────────────────────────┘               │
└─────────────────────────────────────────────────────────┘
```

**Top bar:**
- Gold display: WoodTable panel + coin icon + TMP_Text — event-driven via `IntVariable.OnValueChanged`
- Lives display: WoodTable panel + heart icon + TMP_Text — same pattern
- Wave tracker: WoodTable panel + "Wave X/Y" text — reads `currentWaveNumber` and `totalWaveCount` IntVariables
- Pause button: Small blue Tiny Swords button, top-right — raises `pauseRequested` VoidEventChannel
- Fast-forward toggle: Small blue button next to pause — placeholder, disabled until wired

**Bottom:**
- Start Wave button: Big blue Tiny Swords button, visible only during BuildPhase
- Build menu panel: Horizontal WoodTable panel with tower build buttons

---

## 2. Build Menu

- Horizontal panel at bottom with WoodTable background
- `TowerBuildButton` per tower type: big blue Tiny Swords button + tower icon + cost text
- Buttons gray out when `goldVariable.Value < towerDefinition.goldCost`
- Clicking a button enters placement mode (sets `SelectionState.SelectedTower`)
- `BuildMenuController` populates buttons from a `TowerBuildOption[]` array

**Placement flow:** Click tower button → cursor enters placement mode → valid build nodes highlight green, invalid red → click build node to place → deduct gold → exit placement mode.

---

## 3. Contextual UI

**Enemy Health Bars:**
- World-space Canvas child of enemy prefab
- Tiny Swords SmallBar_Base + SmallBar_Fill (layered Images)
- `EnemyHealthBar`: listens to damage, scales fill Image
- Billboard rotation in LateUpdate
- Hidden at full health, shown on first damage

**Placement Indicator:**
- World-space sprite following mouse cursor
- Semi-transparent tower preview
- `BuildNodeHighlighter` on each BuildNode: green=valid, red=invalid during placement mode

**Tower Range Indicator:**
- Semi-transparent circle sprite scaled by `TowerDefinition.attackRange`
- Shown during placement mode and when tower info panel is open

**Tower Info Panel:**
- Screen-space popup anchored near clicked tower
- WoodTable background, stats (damage, range, fire rate)
- Upgrade button: disabled placeholder, grayed out
- Sell button: disabled placeholder, shows 50% of cost
- Closes on click-away or ESC

---

## 4. Menus & Screens

**Main Menu (own scene: `MainMenu.unity`):**
- Paper/Banner background + Ribbon title decoration
- "Play" big blue button — loads gameplay scene
- "Settings" big blue button — disabled placeholder
- "Quit" big red button — `Application.Quit()`

**Pause Menu Overlay:**
- Semi-transparent dark background
- RegularPaper centered panel
- "Resume" — hides overlay, `Time.timeScale = 1`
- "Restart Level" — reloads current scene
- "Main Menu" — loads main menu scene
- Triggered by ESC key or pause button

**End Game Screens (expand existing `EndStatePanel`):**
- Victory: Paper background, congratulatory text, Swords/Ribbons decoration, "Next Level" (disabled) + "Main Menu" button
- Defeat: Paper background, "Game Over" text, "Retry" + "Main Menu" buttons

---

## Data Layer Changes

**Modify:**
- `IntVariable` — add `System.Action<int> OnValueChanged` fired from `SetValue()` and `ApplyChange()`

**New SO instances (not new types):**
- `VoidEventChannel`: `pauseRequested`, `resumeRequested`, `waveCompletedChannel`
- `IntVariable`: `currentWaveNumber`, `totalWaveCount`

**New data type:**
- `TowerBuildOption` — serializable struct: `TowerDefinition` reference + tower prefab reference
- `SelectionState` — SO holding currently-selected `TowerDefinition` (null = not in placement mode)

---

## File Inventory

### Modified existing (8 files)
- `IntVariable.cs` — add `OnValueChanged`
- `GoldDisplay.cs` — event-driven
- `LivesDisplay.cs` — event-driven
- `EndStatePanel.cs` — expand with buttons + art
- `HUDController.cs` — manage pause, fast-forward
- `WaveStateDisplay.cs` — show "Wave X/Y"
- `MatchStateController.cs` — raise waveCompleted, sync wave count
- `BasicEnemy.prefab` — add health bar Canvas child

### New scripts (14 files)
- `Assets/_Game/Scripts/Data/Variables/SelectionState.cs`
- `Assets/_Game/Scripts/Data/TowerBuildOption.cs`
- `Assets/_Game/Scripts/UI/TowerBuildButton.cs`
- `Assets/_Game/Scripts/UI/BuildMenuController.cs`
- `Assets/_Game/Scripts/UI/PauseButton.cs`
- `Assets/_Game/Scripts/UI/PauseMenuController.cs`
- `Assets/_Game/Scripts/UI/TimeScaleToggle.cs`
- `Assets/_Game/Scripts/UI/WaveCounterDisplay.cs`
- `Assets/_Game/Scripts/UI/TowerInfoPanel.cs`
- `Assets/_Game/Scripts/UI/MainMenuController.cs`
- `Assets/_Game/Scripts/UI/EnemyHealthBar.cs`
- `Assets/_Game/Scripts/UI/BuildNodeHighlighter.cs`
- `Assets/_Game/Scripts/UI/PlacementIndicator.cs`
- `Assets/_Game/Scripts/UI/RangeIndicator.cs`

### New prefabs (6)
- `Assets/_Game/Prefabs/UI/HUD.prefab`
- `Assets/_Game/Prefabs/UI/BuildMenu.prefab`
- `Assets/_Game/Prefabs/UI/PauseMenu.prefab`
- `Assets/_Game/Prefabs/UI/EndStatePanel.prefab`
- `Assets/_Game/Prefabs/UI/TowerInfoPanel.prefab`
- `Assets/_Game/Prefabs/UI/EnemyHealthBar.prefab`

### New scenes (1)
- `Assets/_Game/Scenes/MainMenu.unity`

### New tests (6 files)
- `Assets/_Game/Tests/EditMode/Data/IntVariableCallbackTests.cs`
- `Assets/_Game/Tests/EditMode/UI/BuildMenuTests.cs`
- `Assets/_Game/Tests/EditMode/UI/TowerBuildButtonTests.cs`
- `Assets/_Game/Tests/EditMode/UI/PauseMenuTests.cs`
- `Assets/_Game/Tests/EditMode/UI/EnemyHealthBarTests.cs`
- `Assets/_Game/Tests/EditMode/UI/PlacementTests.cs`

---

## Decisions & Trade-offs

| Decision | Chosen | Alternative | Reason |
|----------|--------|-------------|--------|
| UI toolkit | uGUI | UI Toolkit | Already in project, better world-space support, wider community |
| Art level | Full Tiny Swords | Minimal/placeholder | User preference for polished result |
| Health bars | World-space Canvas | Screen-space tracking | Simpler, standard for TD, no jitter |
| Placement flow | Click-select, click-place | Click-node-first | Standard TD pattern, better visual feedback |
| Unimplemented features | Disabled placeholder buttons | Omit entirely | Complete visual layout, avoids rework |
| IntVariable polling | Add OnValueChanged callback | Keep polling | Architecture compliance, cleaner code |
