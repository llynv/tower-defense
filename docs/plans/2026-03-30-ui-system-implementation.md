# UI System Implementation Plan

**Design doc:** `docs/plans/2026-03-30-ui-system-design.md`
**Approach:** TDD, subagent-driven, one commit per task.
**Total tasks:** 12

---

## Task 1 — Add `OnValueChanged` callback to `IntVariable`

**Why:** Foundation for event-driven UI. `GoldDisplay` and `LivesDisplay` currently poll in `Update()`.

**Modify:** `Assets/_Game/Scripts/Data/Variables/IntVariable.cs`
- Add `public event System.Action<int> OnValueChanged`
- Fire from `SetValue()` and `ApplyChange()` — only when value actually changes

**New test file:** `Assets/_Game/Tests/EditMode/Data/IntVariableCallbackTests.cs`
- `SetValue_FiresOnValueChanged_WithNewValue`
- `ApplyChange_FiresOnValueChanged_WithResultingValue`
- `SetValue_SameValue_DoesNotFire`
- `UnsubscribedCallback_IsNotInvoked`

**Exit criteria:** All new + all 78 existing tests pass.

---

## Task 2 — Create `SelectionState` SO and `TowerBuildOption` struct

**Why:** Data types for the build menu and placement system. `SelectionState` holds the currently-selected tower (null = not in placement mode). `TowerBuildOption` pairs a `TowerDefinition` with a prefab reference.

**New files:**
- `Assets/_Game/Scripts/Data/Variables/SelectionState.cs` — SO with `TowerDefinition SelectedTower`, `GameObject TowerPrefab`, `Select(def, prefab)`, `Clear()`, `event Action SelectionChanged`
- `Assets/_Game/Scripts/Data/TowerBuildOption.cs` — `[System.Serializable] struct` with `TowerDefinition definition`, `GameObject towerPrefab`

**New test file:** `Assets/_Game/Tests/EditMode/Data/SelectionStateTests.cs`
- `Select_SetsSelectedTower`
- `Clear_NullsSelection`
- `Select_FiresSelectionChanged`
- `Clear_FiresSelectionChanged`
- `HasSelection_TrueWhenSelected_FalseWhenCleared`

**Exit criteria:** All new + existing tests pass.

---

## Task 3 — Refactor `GoldDisplay` / `LivesDisplay` to event-driven

**Why:** Remove `Update()` polling — subscribe to `IntVariable.OnValueChanged` instead.

**Modify:**
- `Assets/_Game/Scripts/UI/GoldDisplay.cs` — replace `Update()` with `OnEnable`/`OnDisable` subscribe/unsubscribe pattern
- `Assets/_Game/Scripts/UI/LivesDisplay.cs` — same pattern

**New test file:** `Assets/_Game/Tests/EditMode/UI/VariableDisplayTests.cs`
- Logic-level tests using extracted display logic or direct SO wiring
- `GoldDisplay_UpdatesText_WhenGoldChanges`
- `LivesDisplay_UpdatesText_WhenLivesChange`
- `Display_ShowsInitialValue_OnEnable`

**Exit criteria:** All tests pass, no `Update()` in either display script.

---

## Task 4 — Add `WaveCounterDisplay`, update wave tracking

**Why:** HUD needs "Wave X/Y" display. `LevelDirectorLogic` and `MatchStateController` don't currently track wave index or total.

**Modify:**
- `Assets/_Game/Scripts/Core/LevelDirectorLogic.cs` — add `CurrentWaveIndex` (int), `TotalWaveCount` (int), `SetTotalWaves(int)`, increment wave index on `StartWave()`
- `Assets/_Game/Scripts/Core/MatchStateController.cs` — add `waveCompletedChannel` (VoidEventChannel out), `currentWaveNumber`/`totalWaveCount` IntVariable refs, sync them
- `Assets/_Game/Scripts/UI/WaveStateDisplay.cs` — update to show "Wave X/Y" using IntVariables

**New files:**
- `Assets/_Game/Scripts/UI/WaveCounterDisplay.cs` — subscribes to `currentWaveNumber.OnValueChanged` and `totalWaveCount.OnValueChanged`, displays "Wave X/Y"

**New test file:** `Assets/_Game/Tests/EditMode/UI/WaveCounterTests.cs`
- `LevelDirector_TracksWaveIndex`
- `LevelDirector_IncrementOnStartWave`
- `WaveCounterDisplay_ShowsCorrectFormat`

**Exit criteria:** All tests pass. `MatchStateController` raises `waveCompletedChannel` on wave complete.

---

## Task 5 — Create `EnemyHealthBar`

**Why:** World-space health bars over enemies. Requires `MaxHealth` and `Damaged` event on `EnemyHealth`.

**Modify:**
- `Assets/_Game/Scripts/Gameplay/Enemies/EnemyHealthLogic.cs` — expose `MaxHealth` property
- `Assets/_Game/Scripts/Gameplay/Enemies/EnemyHealth.cs` — add `MaxHealth` property, add `event Action<int, int> Damaged` (current, max), fire from `TakeDamage()`

**New files:**
- `Assets/_Game/Scripts/UI/EnemyHealthBar.cs` — world-space bar controller, `Initialize(EnemyHealth)`, subscribes to `Damaged`, scales fill image, hides at full health, billboard in `LateUpdate()`

**New test file:** `Assets/_Game/Tests/EditMode/UI/EnemyHealthBarTests.cs`
- `EnemyHealthLogic_ExposesMaxHealth`
- `EnemyHealth_FiresDamaged_WithCurrentAndMax`
- `EnemyHealthBar_ScalesFill_OnDamage`
- `EnemyHealthBar_HiddenAtFullHealth`

**Exit criteria:** All tests pass. Existing enemy tests unaffected.

---

## Task 6 — Create `BuildMenuController` and `TowerBuildButton`

**Why:** Tower shop UI — shows available towers, grays out unaffordable, enters placement mode on click.

**New files:**
- `Assets/_Game/Scripts/UI/BuildMenuController.cs` — takes `TowerBuildOption[]`, `IntVariable goldVariable`, `SelectionState`, spawns `TowerBuildButton` instances
- `Assets/_Game/Scripts/UI/TowerBuildButton.cs` — displays tower icon + cost, `SetAffordable(bool)`, on click sets `SelectionState`

**New test file:** `Assets/_Game/Tests/EditMode/UI/BuildMenuTests.cs`
- `TowerBuildButton_ShowsCost`
- `TowerBuildButton_GraysOut_WhenUnaffordable`
- `TowerBuildButton_SetsSelectionState_OnClick`
- `BuildMenuController_SpawnsCorrectButtonCount`

**Exit criteria:** All tests pass.

---

## Task 7 — Create `PlacementIndicator`, `BuildNodeHighlighter`, `RangeIndicator`

**Why:** Visual feedback during tower placement mode.

**New files:**
- `Assets/_Game/Scripts/UI/PlacementIndicator.cs` — follows mouse, shows semi-transparent tower preview, active only when `SelectionState.HasSelection`
- `Assets/_Game/Scripts/UI/BuildNodeHighlighter.cs` — on each `BuildNode`, shows green/red highlight during placement mode
- `Assets/_Game/Scripts/UI/RangeIndicator.cs` — semi-transparent circle scaled by `TowerDefinition.attackRange`

**New test file:** `Assets/_Game/Tests/EditMode/UI/PlacementTests.cs`
- `PlacementIndicator_ActiveOnlyDuringPlacement`
- `BuildNodeHighlighter_GreenWhenAvailable_RedWhenOccupied`
- `RangeIndicator_ScalesToAttackRange`

**Exit criteria:** All tests pass.

---

## Task 8 — Create `TowerInfoPanel`

**Why:** Click-to-inspect popup showing tower stats with placeholder upgrade/sell buttons.

**New files:**
- `Assets/_Game/Scripts/UI/TowerInfoPanel.cs` — shows stats (damage, range, fire rate), upgrade button (disabled), sell button (disabled, shows 50% cost), closes on click-away or ESC

**New test file:** `Assets/_Game/Tests/EditMode/UI/TowerInfoPanelTests.cs`
- `TowerInfoPanel_DisplaysCorrectStats`
- `TowerInfoPanel_UpgradeButton_IsDisabled`
- `TowerInfoPanel_SellButton_Shows50PercentCost`

**Exit criteria:** All tests pass.

---

## Task 9 — Create `PauseButton`, `PauseMenuController`, `TimeScaleToggle`

**Why:** Pause system — ESC or button opens overlay, resume/restart/main menu options.

**New files:**
- `Assets/_Game/Scripts/UI/PauseButton.cs` — raises `pauseRequested` VoidEventChannel
- `Assets/_Game/Scripts/UI/PauseMenuController.cs` — listens to `pauseRequested`, shows overlay, `Time.timeScale = 0`, resume/restart/main menu buttons
- `Assets/_Game/Scripts/UI/TimeScaleToggle.cs` — placeholder fast-forward button (disabled)

**New test file:** `Assets/_Game/Tests/EditMode/UI/PauseMenuTests.cs`
- `PauseMenuController_ShowsOverlay_OnPauseRequested`
- `PauseMenuController_SetsTimeScaleZero_OnPause`
- `PauseMenuController_RestoresTimeScale_OnResume`

**Exit criteria:** All tests pass.

---

## Task 10 — Expand `EndStatePanel`

**Why:** Victory/defeat screens need retry, main menu, and (disabled) next level buttons with Tiny Swords art.

**Modify:** `Assets/_Game/Scripts/UI/EndStatePanel.cs`
- Add retry button (reloads current scene)
- Add main menu button (loads MainMenu scene)
- Add next level button (disabled placeholder)
- Victory variant: show congratulations + decorations
- Defeat variant: show "Game Over" + retry prompt

**Exit criteria:** All tests pass. Existing `EndStatePanel` tests updated/extended.

---

## Task 11 — Create `MainMenuController` and `MainMenu` scene

**Why:** Entry point for the game — Play, Settings (disabled), Quit.

**New files:**
- `Assets/_Game/Scripts/UI/MainMenuController.cs` — Play button loads gameplay scene, Settings disabled, Quit calls `Application.Quit()`
- `Assets/_Game/Scenes/MainMenu.unity` — new scene with Canvas, Paper background, buttons

**New test file:** `Assets/_Game/Tests/EditMode/UI/MainMenuTests.cs`
- `MainMenuController_PlayButton_LoadsGameScene`
- `MainMenuController_SettingsButton_IsDisabled`

**Exit criteria:** All tests pass. Scene loads cleanly.

---

## Task 12 — Update `HUDController`, assemble prefabs, wire into scene

**Why:** Integration task — wire all new UI elements together, create prefabs, update the gameplay scene.

**Modify:**
- `Assets/_Game/Scripts/UI/HUDController.cs` — manage pause button, build menu visibility, wave counter
- `Assets/_Game/Scenes/PrototypeLane01.unity` — add all UI prefabs, wire SO references

**New prefabs:**
- `Assets/_Game/Prefabs/UI/HUD.prefab`
- `Assets/_Game/Prefabs/UI/BuildMenu.prefab`
- `Assets/_Game/Prefabs/UI/PauseMenu.prefab`
- `Assets/_Game/Prefabs/UI/EndStatePanel.prefab`
- `Assets/_Game/Prefabs/UI/TowerInfoPanel.prefab`
- `Assets/_Game/Prefabs/UI/EnemyHealthBar.prefab`

**New SO instances:**
- `pauseRequested` (VoidEventChannel)
- `resumeRequested` (VoidEventChannel)
- `waveCompletedChannel` (VoidEventChannel)
- `currentWaveNumber` (IntVariable)
- `totalWaveCount` (IntVariable)
- `selectionState` (SelectionState)

**Exit criteria:** Full test suite passes. Scene plays end-to-end in Unity Editor. All UI visible and functional (placeholders disabled as designed).

---

## Dependency graph

```
Task 1 (IntVariable callback)
  └→ Task 2 (SelectionState, TowerBuildOption)
  └→ Task 3 (GoldDisplay/LivesDisplay refactor) ← depends on Task 1
  └→ Task 4 (WaveCounter) ← depends on Task 1
  └→ Task 5 (EnemyHealthBar) ← independent
  └→ Task 6 (BuildMenu) ← depends on Tasks 2, 1
  └→ Task 7 (Placement visuals) ← depends on Task 2
  └→ Task 8 (TowerInfoPanel) ← depends on Task 2
  └→ Task 9 (Pause system) ← independent
  └→ Task 10 (EndStatePanel expand) ← independent
  └→ Task 11 (MainMenu) ← independent
  └→ Task 12 (Integration) ← depends on ALL above
```
