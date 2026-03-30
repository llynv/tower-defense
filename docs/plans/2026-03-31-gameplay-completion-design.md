# Gameplay Completion Design

**Goal:** Fix the three blocking issues preventing playability, then add content variety to make a real tower defense experience with 5 waves, 2 tower types, and 2 enemy types.

## Problem Statement

The vertical slice has all the plumbing but three gaps prevent a real play session:

1. **No input handler for tower placement** — `TowerPlacer.TryPlaceAt(BuildNode)` exists but nothing calls it from player input. Players cannot build towers.
2. **Single-wave hardcode** — `WaveSpawner` references one `WaveDefinition` and always passes `hasMoreWaves: false`. Game immediately shows Victory after wave 1.
3. **EnemyHealthBar not initialized** — `WaveSpawner.SpawnEnemy()` never calls `EnemyHealthBar.Initialize()` on spawned enemies.

Secondary: `TimeScaleToggle` is stubbed out (`IsFastForwardAvailable` returns false).

## Design

### 1. BuildNodeClickHandler (New MonoBehaviour)

A scene-level input handler that bridges mouse clicks to tower placement.

**Logic (pure C# `BuildNodeClickLogic`):**
- `ShouldProcess(hasSelection, matchState)` → only process clicks during `BuildPhase` when a tower is selected
- `FindClickedNode(mouseWorldPos, buildNodes, maxDistance)` → find the closest BuildNode within snap range

**MonoBehaviour (`BuildNodeClickHandler`):**
- Serialized refs: `SelectionState`, `MatchStateController`, `BuildNode[]` (all 8 nodes), camera
- On mouse click (left button, Input System: `Mouse.current.leftButton.wasPressedThisFrame`): raycast to world position, find nearest BuildNode, call `TowerPlacer.TryPlaceAt`
- On successful placement: clear `SelectionState`
- On ESC or right-click: clear `SelectionState` (cancel placement)

**Problem with current `TowerPlacer`:** It has hardcoded `towerDefinition` and `towerPrefab` fields. The click handler needs to place whatever tower is selected in `SelectionState`. Rather than reworking `TowerPlacer` extensively, the click handler reads from `SelectionState` and delegates placement through a reworked `TowerPlacer` that accepts the definition and prefab as parameters.

**Rework `TowerPlacer`:** Change `TryPlaceAt` to accept `TowerDefinition` + `GameObject prefab` as parameters instead of using serialized fields. Remove the serialized `towerDefinition` and `towerPrefab` fields. The click handler passes `selectionState.SelectedTower` and `selectionState.TowerPrefab`.

### 2. LevelDefinition SO + Multi-Wave WaveSpawner

**New `LevelDefinition` ScriptableObject:**
- `WaveDefinition[] waves` — ordered sequence of waves
- `int startingGold` — moved from `MatchStateController` to be per-level
- `int startingLives` — moved from `MatchStateController` to be per-level
- Properties: `WaveCount`, `GetWave(int index)`

**Rework `WaveSpawner`:**
- Replace single `WaveDefinition waveDefinition` with `LevelDefinition levelDefinition`
- Track `currentWaveIndex` (starts at 0, incremented on each `OnWaveStarted`)
- On wave start: get `levelDefinition.GetWave(currentWaveIndex)` for current wave's enemy type, count, interval
- On wave complete: check `currentWaveIndex < levelDefinition.WaveCount - 1` to determine `hasMoreWaves`
- On Awake: call `matchStateController.SetTotalWaves(levelDefinition.WaveCount)`

**Rework `MatchStateController`:**
- Remove `startingGold`/`startingLives` serialized fields
- Add `LevelDefinition levelDefinition` serialized field
- Read starting gold/lives from `levelDefinition` in Awake

### 3. EnemyHealthBar Initialization

**Modify `WaveSpawner.SpawnEnemy()`:**
- After initializing `EnemyHealth`, find `EnemyHealthBar` on the spawned GO (via `GetComponentInChildren<EnemyHealthBar>`) and call `Initialize(health)`.

### 4. TimeScaleToggle Implementation

**Rework `TimeScaleToggleLogic`:**
- `IsFastForwardAvailable()` returns `true`
- `Toggle()` flips between 1x and 2x speed, returns current state
- `CurrentSpeed` property (1f or 2f)
- `IsActive` property

**Rework `TimeScaleToggle` MonoBehaviour:**
- On button click: call `logic.Toggle()`, apply `Time.timeScale`, update button visual
- Show "1x" / "2x" text

### 5. Additional Content (Tower + Enemy + Waves)

**New `CannonTower` TowerDefinition asset:**
- Higher damage, slower fire rate, higher cost than Archer
- Uses same `TowerBehaviour` script (it's already data-driven via `TowerDefinition`)
- Needs a new prefab with a different sprite

**New `FastEnemy` EnemyDefinition asset:**
- Higher speed, lower HP, lower gold reward than BasicEnemy
- Uses same enemy scripts (already data-driven via `EnemyDefinition`)
- Needs a new prefab with a different sprite

**Level01 LevelDefinition (5 waves):**
- Wave 1: 3 BasicEnemy, interval 1.5s
- Wave 2: 5 BasicEnemy, interval 1.2s
- Wave 3: 3 FastEnemy + 2 BasicEnemy (two WaveDefinitions — or we enhance WaveDefinition)
- Wave 4: 5 FastEnemy, interval 0.8s
- Wave 5: 8 mixed enemies, interval 1.0s

**Wave mixing approach:** Rather than complicating `WaveDefinition` with multiple enemy types per wave, keep it simple: each `WaveDefinition` has one enemy type. For mixed waves, the `LevelDefinition` can reference separate waves that play sequentially. The build phase only triggers between level-defined "rounds." This keeps the existing data model intact.

Actually, simpler: add `WaveEntry[] entries` to `WaveDefinition` where each entry is `(EnemyDefinition, count)`. This allows mixed waves without complicating the level structure.

**Revised `WaveDefinition`:**
- Add `WaveEntry[] entries` where `WaveEntry` is `{ EnemyDefinition enemy, int count }`
- Keep backward compatibility: if entries is empty, fall back to the existing `enemy`/`enemyCount` fields
- `SpawnIntervalSeconds` stays at the wave level

**`WaveSpawner` changes:** Iterate through `entries` (or fallback fields) to build the spawn list. Flatten all entries into a single spawn queue.

### 6. Prefab Approach for New Content

Both CannonTower and FastEnemy use the same MonoBehaviour scripts as their counterparts — the behavior is entirely data-driven through SO definitions. The only difference is the visual sprite and the SO asset values.

For the CannonTower prefab: duplicate ArcherTower prefab, swap sprite, assign new TowerDefinition.
For the FastEnemy prefab: duplicate BasicEnemy prefab, swap sprite, assign new EnemyDefinition.

Sprites: Use existing Tiny Swords assets. The asset pack has multiple unit types available.

## Architecture Decisions

1. **Pure C# logic for click handling** — `BuildNodeClickLogic` is testable without MonoBehaviour
2. **`LevelDefinition` owns starting resources** — per-level configuration, single source of truth
3. **`WaveEntry` struct for mixed waves** — minimal change to existing WaveDefinition
4. **No pathfinding** — enemies still follow the single lane
5. **No splash damage** — CannonTower is just higher single-target damage (splash would require area-of-effect system we don't have)
6. **Keep existing `TowerBehaviour`** — it's already fully data-driven

## Testing Strategy

New pure logic classes get EditMode unit tests:
- `BuildNodeClickLogic` — shouldProcess, findClickedNode
- `TimeScaleToggleLogic` — toggle state, speed values
- `LevelDefinition` — wave count, get wave by index
- `WaveEntry` iteration logic (if any pure logic extracted)

Existing tests must continue to pass (141 tests).
