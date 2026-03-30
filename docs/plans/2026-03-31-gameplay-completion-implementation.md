# Gameplay Completion Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Fix three blocking issues (no tower placement input, single-wave hardcode, missing health bar init) then add content variety (2 tower types, 2 enemy types, 5 waves) to make a playable tower defense game.

**Architecture:** Pure C# logic classes for testability, thin MonoBehaviour wrappers. ScriptableObject-first shared data (event channels, variables, definitions). No singletons, no GameObject.Find(). Block namespaces only (`namespace X { }`). Tests use `ScriptableObject.CreateInstance<>()` with `SerializedObject`/`SerializedProperty` for SO field setup.

**Tech Stack:** Unity 6 (6000.3.6f1) URP 2D, Input System package, NUnit EditMode tests

**Design doc:** `docs/plans/2026-03-31-gameplay-completion-design.md`

---

## Pre-Implementation Notes

### Test pattern for SO field setup

Tests create SOs via `ScriptableObject.CreateInstance<T>()` and set private serialized fields via:
```csharp
var so = new SerializedObject(instance);
so.FindProperty("fieldName").intValue = 42;
so.ApplyModifiedPropertiesWithoutUndo();
```

### Running tests

Run via MCP: `run_tests` (EditMode). Do NOT use `assembly_names` filter — it returns 0 tests. Current count: 141 passing (139 EditMode + 2 PlayMode).

### Key existing types

- `MatchState` enum: `BuildPhase`, `WaveRunning`, `Victory`, `Defeat`
- `BuildNodeOccupancy` — pure C#, `IsOccupied`, `Occupy()`
- `TowerPlacementLogic` — pure C#, `TryPlace(BuildNodeOccupancy, int goldCost)`
- `SelectionState` SO — `HasSelection`, `SelectedTower`, `TowerPrefab`, `Select()`, `Clear()`
- `LevelDirectorLogic` — pure C#, `CurrentState`, `CurrentWaveIndex`, `StartWave()`, `CompleteWave(bool)`, `SetTotalWaves(int)`
- `EnemyHealth` — MonoBehaviour with `Initialize(EnemyDefinition)`, `Damaged` event
- `EnemyHealthBar` — MonoBehaviour with `Initialize(EnemyHealth)`

---

### Task 1: BuildNodeClickLogic + TowerPlacer Rework

Rework `TowerPlacer.TryPlaceAt` to accept parameters instead of using serialized fields. Create pure `BuildNodeClickLogic` for input handling logic. Create `BuildNodeClickHandler` MonoBehaviour wrapper.

**Files:**
- Modify: `Assets/_Game/Scripts/Gameplay/Building/TowerPlacer.cs`
- Create: `Assets/_Game/Scripts/Gameplay/Building/BuildNodeClickLogic.cs`
- Create: `Assets/_Game/Scripts/Gameplay/Building/BuildNodeClickHandler.cs`
- Create: `Assets/_Game/Tests/EditMode/Gameplay/BuildNodeClickTests.cs`

**Step 1: Write failing tests for BuildNodeClickLogic**

Create `Assets/_Game/Tests/EditMode/Gameplay/BuildNodeClickTests.cs`:

```csharp
using NUnit.Framework;
using UnityEngine;
using TowerDefense.Game.Core;
using TowerDefense.Game.Gameplay.Building;

namespace TowerDefense.Game.Tests.EditMode.Gameplay
{
    public class BuildNodeClickLogicTests
    {
        [Test]
        public void ShouldProcess_HasSelectionAndBuildPhase_ReturnsTrue()
        {
            Assert.That(BuildNodeClickLogic.ShouldProcess(true, MatchState.BuildPhase), Is.True);
        }

        [Test]
        public void ShouldProcess_NoSelection_ReturnsFalse()
        {
            Assert.That(BuildNodeClickLogic.ShouldProcess(false, MatchState.BuildPhase), Is.False);
        }

        [Test]
        public void ShouldProcess_WaveRunning_ReturnsFalse()
        {
            Assert.That(BuildNodeClickLogic.ShouldProcess(true, MatchState.WaveRunning), Is.False);
        }

        [Test]
        public void ShouldProcess_Victory_ReturnsFalse()
        {
            Assert.That(BuildNodeClickLogic.ShouldProcess(true, MatchState.Victory), Is.False);
        }

        [Test]
        public void ShouldProcess_Defeat_ReturnsFalse()
        {
            Assert.That(BuildNodeClickLogic.ShouldProcess(true, MatchState.Defeat), Is.False);
        }

        [Test]
        public void FindClosestNode_WithinRange_ReturnsIndex()
        {
            var positions = new Vector3[]
            {
                new(0, 0, 0),
                new(3, 0, 0),
                new(6, 0, 0)
            };

            int result = BuildNodeClickLogic.FindClosestNode(new Vector3(2.5f, 0, 0), positions, 1f);

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void FindClosestNode_OutOfRange_ReturnsNegativeOne()
        {
            var positions = new Vector3[]
            {
                new(0, 0, 0),
                new(3, 0, 0)
            };

            int result = BuildNodeClickLogic.FindClosestNode(new Vector3(10, 0, 0), positions, 1f);

            Assert.That(result, Is.EqualTo(-1));
        }

        [Test]
        public void FindClosestNode_EmptyArray_ReturnsNegativeOne()
        {
            int result = BuildNodeClickLogic.FindClosestNode(Vector3.zero, new Vector3[0], 1f);

            Assert.That(result, Is.EqualTo(-1));
        }

        [Test]
        public void FindClosestNode_MultipleInRange_ReturnsClosest()
        {
            var positions = new Vector3[]
            {
                new(0, 0, 0),
                new(1, 0, 0),
                new(2, 0, 0)
            };

            int result = BuildNodeClickLogic.FindClosestNode(new Vector3(0.8f, 0, 0), positions, 2f);

            Assert.That(result, Is.EqualTo(1));
        }
    }
}
```

**Step 2: Run tests to verify they fail**

Run: MCP `run_tests` (EditMode)
Expected: FAIL — `BuildNodeClickLogic` does not exist yet.

**Step 3: Implement BuildNodeClickLogic**

Create `Assets/_Game/Scripts/Gameplay/Building/BuildNodeClickLogic.cs`:

```csharp
using UnityEngine;
using TowerDefense.Game.Core;

namespace TowerDefense.Game.Gameplay.Building
{
    public static class BuildNodeClickLogic
    {
        public static bool ShouldProcess(bool hasSelection, MatchState matchState)
        {
            return hasSelection && matchState == MatchState.BuildPhase;
        }

        public static int FindClosestNode(Vector3 worldPos, Vector3[] nodePositions, float maxDistance)
        {
            int closest = -1;
            float closestDist = float.MaxValue;

            for (int i = 0; i < nodePositions.Length; i++)
            {
                float dist = Vector3.Distance(worldPos, nodePositions[i]);
                if (dist <= maxDistance && dist < closestDist)
                {
                    closestDist = dist;
                    closest = i;
                }
            }

            return closest;
        }
    }
}
```

**Step 4: Run tests to verify they pass**

Run: MCP `run_tests` (EditMode)
Expected: All new BuildNodeClickLogic tests pass. All 141+ tests pass.

**Step 5: Rework TowerPlacer to accept parameters**

Replace `Assets/_Game/Scripts/Gameplay/Building/TowerPlacer.cs` with:

```csharp
using UnityEngine;
using TowerDefense.Game.Core;
using TowerDefense.Game.Data.Definitions;
using TowerDefense.Game.Map;

namespace TowerDefense.Game.Gameplay.Building
{
    public sealed class TowerPlacer : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private MatchStateController matchStateController;

        private TowerPlacementLogic placementLogic;

        private void Start()
        {
            placementLogic = new TowerPlacementLogic(matchStateController.Resources);
        }

        public bool TryPlaceAt(BuildNode buildNode, TowerDefinition definition, GameObject prefab)
        {
            if (placementLogic == null || definition == null || prefab == null)
                return false;

            var occupancy = buildNode.Occupancy;
            if (occupancy == null)
                return false;

            if (!placementLogic.TryPlace(occupancy, definition.GoldCost))
                return false;

            Instantiate(prefab, buildNode.PlacementPosition, Quaternion.identity);
            return true;
        }
    }
}
```

**Step 6: Create BuildNodeClickHandler MonoBehaviour**

Create `Assets/_Game/Scripts/Gameplay/Building/BuildNodeClickHandler.cs`:

```csharp
using UnityEngine;
using UnityEngine.InputSystem;
using TowerDefense.Game.Core;
using TowerDefense.Game.Data.Variables;
using TowerDefense.Game.Map;

namespace TowerDefense.Game.Gameplay.Building
{
    public sealed class BuildNodeClickHandler : MonoBehaviour
    {
        [SerializeField] private SelectionState selectionState;
        [SerializeField] private MatchStateController matchStateController;
        [SerializeField] private TowerPlacer towerPlacer;
        [SerializeField] private BuildNode[] buildNodes;
        [SerializeField] private Camera gameCamera;

        [Header("Settings")]
        [SerializeField] private float snapDistance = 1f;

        private Vector3[] nodePositions;

        private void Start()
        {
            CacheNodePositions();
        }

        private void Update()
        {
            if (Mouse.current == null)
                return;

            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                selectionState.Clear();
                return;
            }

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                selectionState.Clear();
                return;
            }

            if (!Mouse.current.leftButton.wasPressedThisFrame)
                return;

            if (!BuildNodeClickLogic.ShouldProcess(selectionState.HasSelection, matchStateController.CurrentState))
                return;

            Vector3 worldPos = gameCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            worldPos.z = 0f;

            int index = BuildNodeClickLogic.FindClosestNode(worldPos, nodePositions, snapDistance);
            if (index < 0)
                return;

            bool placed = towerPlacer.TryPlaceAt(
                buildNodes[index],
                selectionState.SelectedTower,
                selectionState.TowerPrefab);

            if (placed)
                selectionState.Clear();
        }

        private void CacheNodePositions()
        {
            nodePositions = new Vector3[buildNodes.Length];
            for (int i = 0; i < buildNodes.Length; i++)
                nodePositions[i] = buildNodes[i].PlacementPosition;
        }
    }
}
```

**Step 7: Run all tests**

Run: MCP `run_tests` (EditMode)
Expected: All tests pass (existing + new BuildNodeClickLogic tests).

**Step 8: Wire BuildNodeClickHandler in scene**

In PrototypeLane01 scene:
1. Create empty GameObject "BuildNodeClickHandler" under root
2. Add `BuildNodeClickHandler` component
3. Wire serialized references via YAML editing:
   - `selectionState` → SelectionState.asset
   - `matchStateController` → MatchStateController GO
   - `towerPlacer` → TowerPlacer GO
   - `buildNodes` → all 8 BuildNode GOs
   - `gameCamera` → Main Camera
   - `snapDistance` → 1.0

**Step 9: Commit**

```
git add -A && git commit -m "feat: add BuildNodeClickHandler for mouse-click tower placement, rework TowerPlacer to accept params"
```

---

### Task 2: LevelDefinition + WaveEntry + Multi-Wave WaveSpawner

Create `LevelDefinition` SO and `WaveEntry` struct, rework `WaveSpawner` to iterate waves from `LevelDefinition`, rework `MatchStateController` to read starting resources from `LevelDefinition`.

**Files:**
- Create: `Assets/_Game/Scripts/Data/Definitions/LevelDefinition.cs`
- Create: `Assets/_Game/Scripts/Data/Definitions/WaveEntry.cs`
- Modify: `Assets/_Game/Scripts/Data/Definitions/WaveDefinition.cs`
- Modify: `Assets/_Game/Scripts/Core/WaveSpawner.cs`
- Modify: `Assets/_Game/Scripts/Core/MatchStateController.cs`
- Create: `Assets/_Game/Tests/EditMode/Data/LevelDefinitionTests.cs`

**Step 1: Write failing tests for WaveEntry and LevelDefinition**

Create `Assets/_Game/Tests/EditMode/Data/LevelDefinitionTests.cs`:

```csharp
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using TowerDefense.Game.Data.Definitions;

namespace TowerDefense.Game.Tests.EditMode.Data
{
    public class WaveEntryTests
    {
        [Test]
        public void Constructor_SetsEnemyAndCount()
        {
            var enemy = ScriptableObject.CreateInstance<EnemyDefinition>();
            var entry = new WaveEntry(enemy, 5);

            Assert.That(entry.Enemy, Is.EqualTo(enemy));
            Assert.That(entry.Count, Is.EqualTo(5));

            Object.DestroyImmediate(enemy);
        }
    }

    public class LevelDefinitionTests
    {
        [Test]
        public void WaveCount_ReturnsNumberOfWaves()
        {
            var level = ScriptableObject.CreateInstance<LevelDefinition>();
            var wave1 = ScriptableObject.CreateInstance<WaveDefinition>();
            var wave2 = ScriptableObject.CreateInstance<WaveDefinition>();

            var so = new SerializedObject(level);
            var wavesProp = so.FindProperty("waves");
            wavesProp.arraySize = 2;
            wavesProp.GetArrayElementAtIndex(0).objectReferenceValue = wave1;
            wavesProp.GetArrayElementAtIndex(1).objectReferenceValue = wave2;
            so.ApplyModifiedPropertiesWithoutUndo();

            Assert.That(level.WaveCount, Is.EqualTo(2));

            Object.DestroyImmediate(level);
            Object.DestroyImmediate(wave1);
            Object.DestroyImmediate(wave2);
        }

        [Test]
        public void GetWave_ReturnsCorrectWave()
        {
            var level = ScriptableObject.CreateInstance<LevelDefinition>();
            var wave1 = ScriptableObject.CreateInstance<WaveDefinition>();
            var wave2 = ScriptableObject.CreateInstance<WaveDefinition>();

            var so = new SerializedObject(level);
            var wavesProp = so.FindProperty("waves");
            wavesProp.arraySize = 2;
            wavesProp.GetArrayElementAtIndex(0).objectReferenceValue = wave1;
            wavesProp.GetArrayElementAtIndex(1).objectReferenceValue = wave2;
            so.ApplyModifiedPropertiesWithoutUndo();

            Assert.That(level.GetWave(0), Is.EqualTo(wave1));
            Assert.That(level.GetWave(1), Is.EqualTo(wave2));

            Object.DestroyImmediate(level);
            Object.DestroyImmediate(wave1);
            Object.DestroyImmediate(wave2);
        }

        [Test]
        public void GetWave_OutOfRange_ReturnsNull()
        {
            var level = ScriptableObject.CreateInstance<LevelDefinition>();
            var so = new SerializedObject(level);
            so.FindProperty("waves").arraySize = 0;
            so.ApplyModifiedPropertiesWithoutUndo();

            Assert.That(level.GetWave(0), Is.Null);
            Assert.That(level.GetWave(-1), Is.Null);

            Object.DestroyImmediate(level);
        }

        [Test]
        public void StartingGold_ReturnsConfiguredValue()
        {
            var level = ScriptableObject.CreateInstance<LevelDefinition>();
            var so = new SerializedObject(level);
            so.FindProperty("startingGold").intValue = 50;
            so.ApplyModifiedPropertiesWithoutUndo();

            Assert.That(level.StartingGold, Is.EqualTo(50));

            Object.DestroyImmediate(level);
        }

        [Test]
        public void StartingLives_ReturnsConfiguredValue()
        {
            var level = ScriptableObject.CreateInstance<LevelDefinition>();
            var so = new SerializedObject(level);
            so.FindProperty("startingLives").intValue = 15;
            so.ApplyModifiedPropertiesWithoutUndo();

            Assert.That(level.StartingLives, Is.EqualTo(15));

            Object.DestroyImmediate(level);
        }
    }
}
```

**Step 2: Run tests to verify they fail**

Run: MCP `run_tests` (EditMode)
Expected: FAIL — `LevelDefinition`, `WaveEntry` do not exist.

**Step 3: Implement WaveEntry**

Create `Assets/_Game/Scripts/Data/Definitions/WaveEntry.cs`:

```csharp
using System;
using UnityEngine;

namespace TowerDefense.Game.Data.Definitions
{
    [Serializable]
    public struct WaveEntry
    {
        [SerializeField] private EnemyDefinition enemy;
        [SerializeField] private int count;

        public EnemyDefinition Enemy => enemy;
        public int Count => count;

        public WaveEntry(EnemyDefinition enemy, int count)
        {
            this.enemy = enemy;
            this.count = count;
        }
    }
}
```

**Step 4: Implement LevelDefinition**

Create `Assets/_Game/Scripts/Data/Definitions/LevelDefinition.cs`:

```csharp
using UnityEngine;

namespace TowerDefense.Game.Data.Definitions
{
    [CreateAssetMenu(menuName = "Tower Defense/Definitions/Level")]
    public sealed class LevelDefinition : ScriptableObject
    {
        [SerializeField] private WaveDefinition[] waves;

        [Min(0)]
        [SerializeField] private int startingGold = 20;

        [Min(1)]
        [SerializeField] private int startingLives = 10;

        public int WaveCount => waves != null ? waves.Length : 0;
        public int StartingGold => startingGold;
        public int StartingLives => startingLives;

        public WaveDefinition GetWave(int index)
        {
            if (waves == null || index < 0 || index >= waves.Length)
                return null;
            return waves[index];
        }
    }
}
```

**Step 5: Add WaveEntry[] entries to WaveDefinition**

Modify `Assets/_Game/Scripts/Data/Definitions/WaveDefinition.cs` to add entries support:

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense.Game.Data.Definitions
{
    [CreateAssetMenu(menuName = "Tower Defense/Definitions/Wave")]
    public sealed class WaveDefinition : ScriptableObject
    {
        [SerializeField] private EnemyDefinition enemy;

        [Min(1)]
        [SerializeField] private int enemyCount;

        [Min(0.01f)]
        [SerializeField] private float spawnIntervalSeconds;

        [SerializeField] private WaveEntry[] entries;

        public EnemyDefinition Enemy => enemy;
        public int EnemyCount => enemyCount;
        public float SpawnIntervalSeconds => spawnIntervalSeconds;

        public List<EnemyDefinition> BuildSpawnList()
        {
            var list = new List<EnemyDefinition>();

            if (entries != null && entries.Length > 0)
            {
                foreach (var entry in entries)
                {
                    for (int i = 0; i < entry.Count; i++)
                        list.Add(entry.Enemy);
                }
            }
            else if (enemy != null)
            {
                for (int i = 0; i < enemyCount; i++)
                    list.Add(enemy);
            }

            return list;
        }
    }
}
```

**Step 6: Write tests for WaveDefinition.BuildSpawnList**

Add to `Assets/_Game/Tests/EditMode/Data/LevelDefinitionTests.cs`:

```csharp
    public class WaveDefinitionSpawnListTests
    {
        [Test]
        public void BuildSpawnList_FallbackFields_ReturnsEnemyRepeated()
        {
            var wave = ScriptableObject.CreateInstance<WaveDefinition>();
            var enemy = ScriptableObject.CreateInstance<EnemyDefinition>();

            var so = new SerializedObject(wave);
            so.FindProperty("enemy").objectReferenceValue = enemy;
            so.FindProperty("enemyCount").intValue = 3;
            so.ApplyModifiedPropertiesWithoutUndo();

            var list = wave.BuildSpawnList();

            Assert.That(list.Count, Is.EqualTo(3));
            Assert.That(list[0], Is.EqualTo(enemy));

            Object.DestroyImmediate(wave);
            Object.DestroyImmediate(enemy);
        }

        [Test]
        public void BuildSpawnList_WithEntries_UsesEntries()
        {
            var wave = ScriptableObject.CreateInstance<WaveDefinition>();
            var enemyA = ScriptableObject.CreateInstance<EnemyDefinition>();
            var enemyB = ScriptableObject.CreateInstance<EnemyDefinition>();

            var so = new SerializedObject(wave);
            var entriesProp = so.FindProperty("entries");
            entriesProp.arraySize = 2;

            entriesProp.GetArrayElementAtIndex(0).FindPropertyRelative("enemy").objectReferenceValue = enemyA;
            entriesProp.GetArrayElementAtIndex(0).FindPropertyRelative("count").intValue = 2;
            entriesProp.GetArrayElementAtIndex(1).FindPropertyRelative("enemy").objectReferenceValue = enemyB;
            entriesProp.GetArrayElementAtIndex(1).FindPropertyRelative("count").intValue = 3;
            so.ApplyModifiedPropertiesWithoutUndo();

            var list = wave.BuildSpawnList();

            Assert.That(list.Count, Is.EqualTo(5));
            Assert.That(list[0], Is.EqualTo(enemyA));
            Assert.That(list[1], Is.EqualTo(enemyA));
            Assert.That(list[2], Is.EqualTo(enemyB));

            Object.DestroyImmediate(wave);
            Object.DestroyImmediate(enemyA);
            Object.DestroyImmediate(enemyB);
        }

        [Test]
        public void BuildSpawnList_EmptyEntriesAndNoFallback_ReturnsEmpty()
        {
            var wave = ScriptableObject.CreateInstance<WaveDefinition>();
            var so = new SerializedObject(wave);
            so.FindProperty("entries").arraySize = 0;
            so.ApplyModifiedPropertiesWithoutUndo();

            var list = wave.BuildSpawnList();

            Assert.That(list, Is.Empty);

            Object.DestroyImmediate(wave);
        }
    }
```

**Step 7: Run tests to verify all pass**

Run: MCP `run_tests` (EditMode)
Expected: All tests pass.

**Step 8: Rework WaveSpawner for multi-wave**

Replace `Assets/_Game/Scripts/Core/WaveSpawner.cs` with:

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerDefense.Game.Data.Definitions;
using TowerDefense.Game.Data.Events;
using TowerDefense.Game.Gameplay.Enemies;
using TowerDefense.Game.Map;
using TowerDefense.Game.UI;

namespace TowerDefense.Game.Core
{
    public sealed class WaveSpawner : MonoBehaviour
    {
        [SerializeField] private LevelDefinition levelDefinition;
        [SerializeField] private LanePathAuthoring lanePathAuthoring;
        [SerializeField] private SpawnPoint spawnPoint;
        [SerializeField] private EnemyRuntimeSet enemyRuntimeSet;
        [SerializeField] private VoidEventChannel waveStartedChannel;

        [Header("Events Out")]
        [SerializeField] private MatchStateController matchStateController;

        private LanePath lanePath;
        private int currentWaveIndex;
        private int spawnedCount;
        private int totalToSpawn;
        private bool spawning;

        private void Awake()
        {
            lanePath = lanePathAuthoring.BuildPath();

            if (levelDefinition != null)
                matchStateController.SetTotalWaves(levelDefinition.WaveCount);
        }

        private void OnEnable()
        {
            if (waveStartedChannel != null)
                waveStartedChannel.RegisterListener(OnWaveStarted);
        }

        private void OnDisable()
        {
            if (waveStartedChannel != null)
                waveStartedChannel.UnregisterListener(OnWaveStarted);
        }

        private void Update()
        {
            if (!spawning || matchStateController.CurrentState != MatchState.WaveRunning)
                return;

            if (spawnedCount >= totalToSpawn && enemyRuntimeSet.Count == 0)
            {
                spawning = false;
                bool hasMore = currentWaveIndex < levelDefinition.WaveCount - 1;
                matchStateController.NotifyWaveComplete(hasMoreWaves: hasMore);
            }
        }

        private void OnWaveStarted()
        {
            var wave = levelDefinition.GetWave(currentWaveIndex);
            if (wave == null)
                return;

            spawnedCount = 0;
            spawning = true;

            if (enemyRuntimeSet != null)
                enemyRuntimeSet.Clear();

            var spawnList = wave.BuildSpawnList();
            totalToSpawn = spawnList.Count;

            StartCoroutine(SpawnWaveCoroutine(spawnList, wave.SpawnIntervalSeconds));
            currentWaveIndex++;
        }

        private IEnumerator SpawnWaveCoroutine(List<EnemyDefinition> spawnList, float interval)
        {
            for (int i = 0; i < spawnList.Count; i++)
            {
                SpawnEnemy(spawnList[i]);
                spawnedCount++;

                if (i < spawnList.Count - 1)
                    yield return new WaitForSeconds(interval);
            }
        }

        private void SpawnEnemy(EnemyDefinition enemyDef)
        {
            Vector3 spawnPos = spawnPoint != null ? spawnPoint.Position : Vector3.zero;
            GameObject go = Instantiate(enemyDef.Prefab, spawnPos, Quaternion.identity);

            var mover = go.GetComponent<EnemyMover>();
            if (mover != null)
            {
                mover.Initialize(enemyDef, lanePath);

                if (enemyRuntimeSet != null)
                    enemyRuntimeSet.Add(mover);
            }

            var health = go.GetComponent<EnemyHealth>();
            if (health != null)
            {
                health.Initialize(enemyDef);

                var healthBar = go.GetComponentInChildren<EnemyHealthBar>();
                if (healthBar != null)
                    healthBar.Initialize(health);
            }
        }
    }
}
```

**Important:** This changes the spawner to get the prefab from `EnemyDefinition` instead of a serialized field. We need to add a `Prefab` field to `EnemyDefinition`.

**Step 9: Add Prefab field to EnemyDefinition**

Modify `Assets/_Game/Scripts/Data/Definitions/EnemyDefinition.cs`:

```csharp
using UnityEngine;

namespace TowerDefense.Game.Data.Definitions
{
    [CreateAssetMenu(menuName = "Tower Defense/Definitions/Enemy")]
    public sealed class EnemyDefinition : ScriptableObject
    {
        [Min(0f)]
        [SerializeField] private float moveSpeed;

        [Min(1)]
        [SerializeField] private int maxHealth;

        [Min(0)]
        [SerializeField] private int goldReward;

        [SerializeField] private GameObject prefab;

        public float MoveSpeed => moveSpeed;
        public int MaxHealth => maxHealth;
        public int GoldReward => goldReward;
        public GameObject Prefab => prefab;
    }
}
```

**Step 10: Rework MatchStateController to read from LevelDefinition**

Replace `startingGold`/`startingLives` serialized fields with `LevelDefinition`:

Modify `Assets/_Game/Scripts/Core/MatchStateController.cs` Awake:

Replace:
```csharp
[Header("Resources")]
[SerializeField] private int startingGold = 20;
[SerializeField] private int startingLives = 10;
```

With:
```csharp
[Header("Level")]
[SerializeField] private LevelDefinition levelDefinition;
```

And change Awake from:
```csharp
resources = new PlayerResourcesLogic(startingGold, startingLives);
```
To:
```csharp
resources = new PlayerResourcesLogic(
    levelDefinition != null ? levelDefinition.StartingGold : 20,
    levelDefinition != null ? levelDefinition.StartingLives : 10);
```

Add using:
```csharp
using TowerDefense.Game.Data.Definitions;
```

**Step 11: Run all tests**

Run: MCP `run_tests` (EditMode)
Expected: All tests pass. The WaveSpawner changes are MonoBehaviour-only and existing tests don't instantiate it.

**Step 12: Commit**

```
git add -A && git commit -m "feat: add LevelDefinition, WaveEntry, multi-wave WaveSpawner, EnemyHealthBar init"
```

---

### Task 3: TimeScaleToggle Implementation

Implement the time scale toggle that was previously stubbed out.

**Files:**
- Modify: `Assets/_Game/Scripts/UI/TimeScaleToggleLogic.cs`
- Modify: `Assets/_Game/Scripts/UI/TimeScaleToggle.cs`
- Create: `Assets/_Game/Tests/EditMode/UI/TimeScaleToggleTests.cs`

**Step 1: Write failing tests**

Create `Assets/_Game/Tests/EditMode/UI/TimeScaleToggleTests.cs`:

```csharp
using NUnit.Framework;
using TowerDefense.Game.UI;

namespace TowerDefense.Game.Tests.EditMode.UI
{
    public class TimeScaleToggleLogicTests
    {
        [Test]
        public void IsFastForwardAvailable_ReturnsTrue()
        {
            var logic = new TimeScaleToggleLogic();

            Assert.That(logic.IsFastForwardAvailable(), Is.True);
        }

        [Test]
        public void IsActive_InitiallyFalse()
        {
            var logic = new TimeScaleToggleLogic();

            Assert.That(logic.IsActive, Is.False);
        }

        [Test]
        public void CurrentSpeed_Initially1()
        {
            var logic = new TimeScaleToggleLogic();

            Assert.That(logic.CurrentSpeed, Is.EqualTo(1f));
        }

        [Test]
        public void Toggle_ActivatesFastForward()
        {
            var logic = new TimeScaleToggleLogic();

            logic.Toggle();

            Assert.That(logic.IsActive, Is.True);
            Assert.That(logic.CurrentSpeed, Is.EqualTo(2f));
        }

        [Test]
        public void Toggle_Twice_DeactivatesFastForward()
        {
            var logic = new TimeScaleToggleLogic();

            logic.Toggle();
            logic.Toggle();

            Assert.That(logic.IsActive, Is.False);
            Assert.That(logic.CurrentSpeed, Is.EqualTo(1f));
        }

        [Test]
        public void Reset_DeactivatesFastForward()
        {
            var logic = new TimeScaleToggleLogic();
            logic.Toggle();

            logic.Reset();

            Assert.That(logic.IsActive, Is.False);
            Assert.That(logic.CurrentSpeed, Is.EqualTo(1f));
        }
    }
}
```

**Step 2: Run tests to verify they fail**

Run: MCP `run_tests` (EditMode)
Expected: FAIL — `IsActive`, `CurrentSpeed`, `Toggle`, `Reset` don't exist.

**Step 3: Implement TimeScaleToggleLogic**

Replace `Assets/_Game/Scripts/UI/TimeScaleToggleLogic.cs`:

```csharp
namespace TowerDefense.Game.UI
{
    public sealed class TimeScaleToggleLogic
    {
        private const float NormalSpeed = 1f;
        private const float FastSpeed = 2f;

        public bool IsActive { get; private set; }
        public float CurrentSpeed => IsActive ? FastSpeed : NormalSpeed;

        public bool IsFastForwardAvailable()
        {
            return true;
        }

        public void Toggle()
        {
            IsActive = !IsActive;
        }

        public void Reset()
        {
            IsActive = false;
        }
    }
}
```

**Step 4: Run tests to verify they pass**

Run: MCP `run_tests` (EditMode)
Expected: All tests pass.

**Step 5: Update TimeScaleToggle MonoBehaviour**

Replace `Assets/_Game/Scripts/UI/TimeScaleToggle.cs`:

```csharp
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TowerDefense.Game.UI
{
    public sealed class TimeScaleToggle : MonoBehaviour
    {
        [SerializeField] private Button toggleButton;
        [SerializeField] private TextMeshProUGUI speedLabel;

        private TimeScaleToggleLogic logic;

        private void Awake()
        {
            logic = new TimeScaleToggleLogic();
            UpdateVisual();
        }

        private void OnEnable()
        {
            if (toggleButton != null)
                toggleButton.onClick.AddListener(OnToggleClicked);
        }

        private void OnDisable()
        {
            if (toggleButton != null)
                toggleButton.onClick.RemoveListener(OnToggleClicked);

            logic?.Reset();
            Time.timeScale = 1f;
        }

        private void OnToggleClicked()
        {
            logic.Toggle();
            Time.timeScale = logic.CurrentSpeed;
            UpdateVisual();
        }

        private void UpdateVisual()
        {
            if (speedLabel != null)
                speedLabel.text = logic.IsActive ? "2x" : "1x";
        }
    }
}
```

**Step 6: Run all tests**

Run: MCP `run_tests` (EditMode)
Expected: All tests pass.

**Step 7: Commit**

```
git add -A && git commit -m "feat: implement TimeScaleToggle with 1x/2x speed switching"
```

---

### Task 4: CannonTower Content

Create new TowerDefinition SO and prefab for CannonTower. No new scripts needed — `TowerBehaviour` is already data-driven.

**Files:**
- Create: `Assets/_Game/Data/Definitions/CannonTower.asset` (SO via MCP)
- Create: `Assets/_Game/Prefabs/Towers/CannonTower.prefab` (duplicate ArcherTower, change values)

**Step 1: Create CannonTower TowerDefinition asset**

Use MCP `manage_scriptable_object` action=create:
- type: `TowerDefense.Game.Data.Definitions.TowerDefinition`
- folder: `Assets/_Game/Data/Definitions`
- name: `CannonTower`
- patches: `goldCost=15`, `attackRange=2.5`, `damagePerShot=8`, `attackIntervalSeconds=1.5`

**Step 2: Duplicate ArcherTower prefab → CannonTower prefab**

Use MCP `manage_asset` action=duplicate:
- source: `Assets/_Game/Prefabs/Towers/ArcherTower.prefab`
- destination: `Assets/_Game/Prefabs/Towers/CannonTower.prefab`

Then modify the prefab to use a different sprite (pick a suitable one from Tiny Swords assets) and assign the CannonTower TowerDefinition.

**Step 3: Add CannonTower to BuildMenuController's TowerBuildOption[]**

Wire in the scene — add a second `TowerBuildOption` entry with `CannonTower` definition and `CannonTower` prefab.

**Step 4: Run all tests**

Run: MCP `run_tests` (EditMode)
Expected: All tests pass. Asset creation does not break tests.

**Step 5: Commit**

```
git add -A && git commit -m "feat: add CannonTower definition and prefab (high damage, slow fire, 15 gold)"
```

---

### Task 5: FastEnemy Content

Create new EnemyDefinition SO and prefab for FastEnemy.

**Files:**
- Create: `Assets/_Game/Data/Definitions/FastEnemy.asset` (SO via MCP)
- Create: `Assets/_Game/Prefabs/Enemies/FastEnemy.prefab` (duplicate BasicEnemy, change values)

**Step 1: Create FastEnemy EnemyDefinition asset**

Use MCP `manage_scriptable_object` action=create:
- type: `TowerDefense.Game.Data.Definitions.EnemyDefinition`
- folder: `Assets/_Game/Data/Definitions`
- name: `FastEnemy`
- patches: `moveSpeed=3.0`, `maxHealth=30`, `goldReward=3`, `prefab` → FastEnemy prefab (wire after creation)

**Step 2: Duplicate BasicEnemy prefab → FastEnemy prefab**

Use MCP `manage_asset` action=duplicate:
- source: `Assets/_Game/Prefabs/Enemies/BasicEnemy.prefab`
- destination: `Assets/_Game/Prefabs/Enemies/FastEnemy.prefab`

Swap sprite to a different Tiny Swords goblin/unit sprite.

**Step 3: Wire prefab references in SO assets**

Both `BasicEnemy.asset` and `FastEnemy.asset` need their `prefab` field set to point to their respective prefabs. Edit YAML directly (MCP set_property doesn't support object references).

**Step 4: Run all tests**

Run: MCP `run_tests` (EditMode)
Expected: All tests pass.

**Step 5: Commit**

```
git add -A && git commit -m "feat: add FastEnemy definition and prefab (fast, low HP, 3 gold)"
```

---

### Task 6: Level01 with 5 Waves

Create WaveDefinition assets for all 5 waves and one LevelDefinition asset tying them together.

**Files:**
- Modify: `Assets/_Game/Data/Definitions/Wave01.asset` — update to use entries
- Create: `Assets/_Game/Data/Definitions/Wave02.asset` through `Wave05.asset`
- Create: `Assets/_Game/Data/Definitions/Level01.asset`

**Wave configuration:**
- Wave 1: 3x BasicEnemy, interval 1.5s
- Wave 2: 5x BasicEnemy, interval 1.2s
- Wave 3: 2x BasicEnemy + 3x FastEnemy, interval 1.0s
- Wave 4: 5x FastEnemy, interval 0.8s
- Wave 5: 4x BasicEnemy + 4x FastEnemy, interval 1.0s

**Level01 configuration:**
- startingGold: 20
- startingLives: 10
- waves: [Wave01, Wave02, Wave03, Wave04, Wave05]

**Step 1: Create/update all wave assets via MCP**

Use `manage_scriptable_object` for each wave. For waves with mixed enemies, set the `entries` array.

**Step 2: Create Level01 LevelDefinition asset**

Use MCP `manage_scriptable_object` action=create:
- type: `TowerDefense.Game.Data.Definitions.LevelDefinition`
- folder: `Assets/_Game/Data/Definitions`
- name: `Level01`
- Set waves array, startingGold=20, startingLives=10

**Step 3: Wire Level01 into scene**

Update WaveSpawner and MatchStateController in PrototypeLane01 scene to reference Level01 instead of the old single-wave fields.

**Step 4: Run all tests**

Run: MCP `run_tests` (EditMode)
Expected: All tests pass.

**Step 5: Commit**

```
git add -A && git commit -m "feat: add Level01 with 5 progressive waves mixing BasicEnemy and FastEnemy"
```

---

### Task 7: Scene Wiring + Final Integration

Wire all new components in the PrototypeLane01 scene and verify the complete gameplay loop works.

**Step 1: Verify all scene references are wired**

Check that the following are connected in scene YAML:
- `BuildNodeClickHandler` → all refs wired
- `WaveSpawner.levelDefinition` → Level01
- `MatchStateController.levelDefinition` → Level01
- `BuildMenuController` has both ArcherTower and CannonTower in its options
- `EnemyDefinition` assets have prefab references

**Step 2: Run all tests (EditMode + PlayMode)**

Run: MCP `run_tests` (EditMode), then `run_tests` (PlayMode)
Expected: All 141+ tests pass (original 141 + new tests from Tasks 1-3).

**Step 3: Enter Play Mode and verify**

Use MCP `manage_editor` action=play to enter play mode. Verify:
- Build menu shows 2 tower options (Archer + Cannon)
- Clicking a tower option then clicking a build node places the tower
- Start Wave button starts wave 1
- Enemies spawn and walk the path
- Health bars appear when enemies take damage
- After wave clears, build phase returns
- 5 waves total, victory after wave 5
- Time scale toggle works (1x/2x)

**Step 4: Commit**

```
git add -A && git commit -m "feat: wire gameplay completion — 2 towers, 2 enemies, 5 waves, playable loop"
```

---

## Summary of New/Modified Files

### New Scripts (4)
- `Assets/_Game/Scripts/Data/Definitions/LevelDefinition.cs`
- `Assets/_Game/Scripts/Data/Definitions/WaveEntry.cs`
- `Assets/_Game/Scripts/Gameplay/Building/BuildNodeClickLogic.cs`
- `Assets/_Game/Scripts/Gameplay/Building/BuildNodeClickHandler.cs`

### Modified Scripts (6)
- `Assets/_Game/Scripts/Data/Definitions/WaveDefinition.cs` — add entries, BuildSpawnList
- `Assets/_Game/Scripts/Data/Definitions/EnemyDefinition.cs` — add prefab field
- `Assets/_Game/Scripts/Core/WaveSpawner.cs` — multi-wave, health bar init
- `Assets/_Game/Scripts/Core/MatchStateController.cs` — LevelDefinition for resources
- `Assets/_Game/Scripts/Gameplay/Building/TowerPlacer.cs` — parameterized TryPlaceAt
- `Assets/_Game/Scripts/UI/TimeScaleToggleLogic.cs` — full implementation
- `Assets/_Game/Scripts/UI/TimeScaleToggle.cs` — full implementation

### New Test Files (3)
- `Assets/_Game/Tests/EditMode/Gameplay/BuildNodeClickTests.cs`
- `Assets/_Game/Tests/EditMode/Data/LevelDefinitionTests.cs`
- `Assets/_Game/Tests/EditMode/UI/TimeScaleToggleTests.cs`

### New SO Assets
- `Assets/_Game/Data/Definitions/CannonTower.asset`
- `Assets/_Game/Data/Definitions/FastEnemy.asset`
- `Assets/_Game/Data/Definitions/Wave02.asset` through `Wave05.asset`
- `Assets/_Game/Data/Definitions/Level01.asset`

### New Prefabs
- `Assets/_Game/Prefabs/Towers/CannonTower.prefab`
- `Assets/_Game/Prefabs/Enemies/FastEnemy.prefab`

## Expected Final Test Count

141 (existing) + 9 (BuildNodeClickLogic) + 8 (LevelDefinition + WaveEntry + WaveDefinition) + 6 (TimeScaleToggle) = ~164 tests
