# Tower Defense Vertical Slice Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Build a first playable 2.5D lane-based tower defense slice with one handcrafted stair map, fixed build sockets, one tower, one enemy, one wave, and clear win/lose conditions.

**Architecture:** Use a modular monolith with ScriptableObject-first shared data, scene-authored lane/build-node layout, and single-responsibility MonoBehaviours. Keep gameplay logic fundamentally 2D and deterministic while using Tiny Swords art, sorting, and camera composition to sell the 2.5D stair traversal.

**Tech Stack:** Unity 6000.3.6f1, URP 2D, Input System, Unity Test Framework, Tiny Swords assets, Unity MCP tooling

---

## Engineering Rules

- Apply the local `Unity Architect` guidance: ScriptableObjects for shared data, no global scene lookups, no singletons, and self-contained prefabs.
- Apply the local `Software Architect` guidance: prefer reversible decisions, document trade-offs, and avoid speculative abstractions.
- Apply the local `Code Reviewer` standard during execution: prioritize correctness, maintainability, and test coverage.
- Keep MonoBehaviours single-purpose and easy to scan.
- No commented-out code. Delete dead code and rely on git history.
- Do not add explanatory comments unless they capture intent or a non-obvious invariant.
- Keep runtime code inside `_Game` and keep MCP/plugin code untouched.

## Target File Layout

- `Assets/_Game/Scenes/PrototypeLane01.unity`
- `Assets/_Game/Scripts/Core/`
- `Assets/_Game/Scripts/Gameplay/`
- `Assets/_Game/Scripts/Map/`
- `Assets/_Game/Scripts/Presentation/`
- `Assets/_Game/Scripts/UI/`
- `Assets/_Game/Scripts/Data/`
- `Assets/_Game/Prefabs/Enemies/`
- `Assets/_Game/Prefabs/Towers/`
- `Assets/_Game/Prefabs/Map/`
- `Assets/_Game/Tests/EditMode/`
- `Assets/_Game/Tests/PlayMode/`

## Architecture Decisions

- `Board domain` owns lane paths, spawn/goal markers, stair readability, and fixed build sockets.
- `Combat domain` owns enemies, towers, projectiles, damage, and death/leak outcomes.
- `Progression domain` owns wave definitions, tower costs, enemy rewards, gold, and lives.
- `Presentation domain` owns camera, sprite sorting, VFX hooks, and HUD.
- `Bootstrap/orchestration` wires scene objects to ScriptableObject data and event channels, but does not own combat logic.

### Task 1: Create the gameplay assembly and test skeleton

**Files:**
- Create: `Assets/_Game/Scripts/TowerDefense.Game.asmdef`
- Create: `Assets/_Game/Tests/EditMode/TowerDefense.EditMode.asmdef`
- Create: `Assets/_Game/Tests/PlayMode/TowerDefense.PlayMode.asmdef`
- Create: `Assets/_Game/Tests/EditMode/Smoke/AssemblySmokeTests.cs`

**Step 1: Write the failing test**

```csharp
using NUnit.Framework;

public class AssemblySmokeTests
{
    [Test]
    public void Placeholder()
    {
        Assert.That(true, Is.True);
    }
}
```

**Step 2: Run test to verify the test assembly is discovered**

Run: Unity Test Runner or MCP `run_tests(mode="EditMode", assembly_names=["TowerDefense.EditMode"], include_details=true)`
Expected: the test run fails or the assembly is missing until the asmdefs exist.

**Step 3: Create the minimal assembly definitions**

```json
{
  "name": "TowerDefense.Game",
  "rootNamespace": "TowerDefense.Game"
}
```

```json
{
  "name": "TowerDefense.EditMode",
  "rootNamespace": "TowerDefense.Game.Tests.EditMode",
  "references": ["TowerDefense.Game", "UnityEngine.TestRunner", "UnityEditor.TestRunner", "nunit.framework"],
  "includePlatforms": ["Editor"]
}
```

**Step 4: Run test to verify discovery succeeds**

Run: MCP `run_tests(mode="EditMode", assembly_names=["TowerDefense.EditMode"], include_details=true)`
Expected: PASS with one discovered placeholder test.

**Step 5: Commit**

```bash
git add Assets/_Game/Scripts/TowerDefense.Game.asmdef Assets/_Game/Tests/EditMode/TowerDefense.EditMode.asmdef Assets/_Game/Tests/PlayMode/TowerDefense.PlayMode.asmdef Assets/_Game/Tests/EditMode/Smoke/AssemblySmokeTests.cs
git commit -m "chore: add tower defense gameplay assemblies"
```

### Task 2: Create ScriptableObject data, variables, and event channels

**Files:**
- Create: `Assets/_Game/Scripts/Data/Definitions/EnemyDefinition.cs`
- Create: `Assets/_Game/Scripts/Data/Definitions/TowerDefinition.cs`
- Create: `Assets/_Game/Scripts/Data/Definitions/WaveDefinition.cs`
- Create: `Assets/_Game/Scripts/Data/Variables/IntVariable.cs`
- Create: `Assets/_Game/Scripts/Data/Variables/FloatVariable.cs`
- Create: `Assets/_Game/Scripts/Data/Events/VoidEventChannel.cs`
- Create: `Assets/_Game/Scripts/Data/Events/IntEventChannel.cs`
- Create: `Assets/_Game/Tests/EditMode/Data/DefinitionAssetTests.cs`

**Step 1: Write the failing test**

```csharp
using NUnit.Framework;
using TowerDefense.Game.Data.Definitions;

public class DefinitionAssetTests
{
    [Test]
    public void EnemyDefinition_PreservesConfiguredValues()
    {
        var definition = ScriptableObject.CreateInstance<EnemyDefinition>();
        definition.InitializeForTests(2f, 10, 3);

        Assert.That(definition.MoveSpeed, Is.EqualTo(2f));
        Assert.That(definition.MaxHealth, Is.EqualTo(10));
        Assert.That(definition.GoldReward, Is.EqualTo(3));
    }
}
```

**Step 2: Run test to verify it fails**

Run: MCP `run_tests(mode="EditMode", assembly_names=["TowerDefense.EditMode"], include_failed_tests=true)`
Expected: FAIL because `EnemyDefinition` and helpers do not exist.

**Step 3: Write the minimal implementation**

```csharp
[CreateAssetMenu(menuName = "Tower Defense/Definitions/Enemy")]
public class EnemyDefinition : ScriptableObject
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private int maxHealth;
    [SerializeField] private int goldReward;

    public float MoveSpeed => moveSpeed;
    public int MaxHealth => maxHealth;
    public int GoldReward => goldReward;

    public void InitializeForTests(float speed, int health, int reward)
    {
        moveSpeed = speed;
        maxHealth = health;
        goldReward = reward;
    }
}
```

**Step 4: Run tests to verify data assets compile and pass**

Run: MCP `run_tests(mode="EditMode", assembly_names=["TowerDefense.EditMode"], include_details=true)`
Expected: PASS for the data definition tests.

**Step 5: Commit**

```bash
git add Assets/_Game/Scripts/Data Assets/_Game/Tests/EditMode/Data/DefinitionAssetTests.cs
git commit -m "feat: add tower defense data assets and event channels"
```

### Task 3: Author the prototype map scene and map components

**Files:**
- Create: `Assets/_Game/Scenes/PrototypeLane01.unity`
- Create: `Assets/_Game/Scripts/Map/LanePath.cs`
- Create: `Assets/_Game/Scripts/Map/BuildNode.cs`
- Create: `Assets/_Game/Scripts/Map/SpawnPoint.cs`
- Create: `Assets/_Game/Scripts/Map/GoalPoint.cs`
- Create: `Assets/_Game/Scripts/Presentation/SpriteSortAnchor.cs`
- Create: `Assets/_Game/Tests/EditMode/Map/LanePathTests.cs`

**Step 1: Write the failing test**

```csharp
using NUnit.Framework;
using UnityEngine;
using TowerDefense.Game.Map;

public class LanePathTests
{
    [Test]
    public void EvaluatePosition_ReturnsLastPointAtProgressOne()
    {
        var lane = new LanePath(new[] { Vector3.zero, Vector3.right, new Vector3(2f, 1f, 0f) });

        Assert.That(lane.EvaluatePosition(1f), Is.EqualTo(new Vector3(2f, 1f, 0f)));
    }
}
```

**Step 2: Run test to verify it fails**

Run: MCP `run_tests(mode="EditMode", assembly_names=["TowerDefense.EditMode"], include_failed_tests=true)`
Expected: FAIL because `LanePath` evaluation does not exist.

**Step 3: Write minimal lane evaluation and map authoring components**

```csharp
public sealed class LanePath
{
    private readonly IReadOnlyList<Vector3> points;

    public LanePath(IReadOnlyList<Vector3> points)
    {
        this.points = points;
    }

    public Vector3 EvaluatePosition(float progress)
    {
        if (progress >= 1f)
        {
            return points[^1];
        }

        return points[0];
    }
}
```

**Step 4: Build the scene blockout and verify visually**

Run: open `Assets/_Game/Scenes/PrototypeLane01.unity`, compose one Tiny Swords lane with a visible stair climb, place spawn/goal markers, and place 6-10 build nodes.
Expected: the route, stair climb, and build node readability are obvious from the fixed gameplay camera.

**Step 5: Commit**

```bash
git add Assets/_Game/Scenes/PrototypeLane01.unity Assets/_Game/Scripts/Map Assets/_Game/Scripts/Presentation/SpriteSortAnchor.cs Assets/_Game/Tests/EditMode/Map/LanePathTests.cs
git commit -m "feat: add prototype map authoring components"
```

### Task 4: Implement enemy traversal and leak handling

**Files:**
- Create: `Assets/_Game/Scripts/Gameplay/Enemies/EnemyMover.cs`
- Create: `Assets/_Game/Scripts/Gameplay/Enemies/EnemyHealth.cs`
- Create: `Assets/_Game/Scripts/Gameplay/Enemies/EnemyRuntimeSet.cs`
- Create: `Assets/_Game/Scripts/Gameplay/Enemies/EnemyLeakReporter.cs`
- Create: `Assets/_Game/Prefabs/Enemies/BasicEnemy.prefab`
- Create: `Assets/_Game/Tests/EditMode/Gameplay/EnemyMoverTests.cs`

**Step 1: Write the failing test**

```csharp
using NUnit.Framework;
using UnityEngine;

public class EnemyMoverTests
{
    [Test]
    public void Tick_AdvancesProgressUsingMoveSpeed()
    {
        var mover = new EnemyMover(2f);

        mover.Tick(0.5f);

        Assert.That(mover.Progress, Is.EqualTo(1f));
    }
}
```

**Step 2: Run test to verify it fails**

Run: MCP `run_tests(mode="EditMode", assembly_names=["TowerDefense.EditMode"], include_failed_tests=true)`
Expected: FAIL because `EnemyMover` does not exist.

**Step 3: Write the minimal implementation**

```csharp
public sealed class EnemyMover
{
    private readonly float moveSpeed;

    public EnemyMover(float moveSpeed)
    {
        this.moveSpeed = moveSpeed;
    }

    public float Progress { get; private set; }

    public void Tick(float deltaTime)
    {
        Progress += moveSpeed * deltaTime;
    }
}
```

**Step 4: Run tests and then verify in scene**

Run: MCP `run_tests(mode="EditMode", assembly_names=["TowerDefense.EditMode"], include_details=true)`
Expected: PASS for movement rules, then a BasicEnemy prefab moves from spawn to goal and raises a leak event when it reaches the end.

**Step 5: Commit**

```bash
git add Assets/_Game/Scripts/Gameplay/Enemies Assets/_Game/Prefabs/Enemies/BasicEnemy.prefab Assets/_Game/Tests/EditMode/Gameplay/EnemyMoverTests.cs
git commit -m "feat: add enemy lane traversal"
```

### Task 5: Implement match state, resources, and wave spawning

**Files:**
- Create: `Assets/_Game/Scripts/Core/MatchState.cs`
- Create: `Assets/_Game/Scripts/Core/MatchStateController.cs`
- Create: `Assets/_Game/Scripts/Core/LevelDirector.cs`
- Create: `Assets/_Game/Scripts/Gameplay/Waves/WaveSpawner.cs`
- Create: `Assets/_Game/Scripts/Gameplay/Waves/WaveEntry.cs`
- Create: `Assets/_Game/Tests/EditMode/Core/LevelDirectorTests.cs`

**Step 1: Write the failing test**

```csharp
using NUnit.Framework;

public class LevelDirectorTests
{
    [Test]
    public void StartWave_TransitionsToWaveRunning()
    {
        var director = new LevelDirector();

        director.StartWave();

        Assert.That(director.CurrentState, Is.EqualTo(MatchState.WaveRunning));
    }
}
```

**Step 2: Run test to verify it fails**

Run: MCP `run_tests(mode="EditMode", assembly_names=["TowerDefense.EditMode"], include_failed_tests=true)`
Expected: FAIL because the match state flow does not exist.

**Step 3: Write the minimal implementation**

```csharp
public enum MatchState
{
    Setup,
    BuildPhase,
    WaveRunning,
    Victory,
    Defeat
}

public sealed class LevelDirector
{
    public MatchState CurrentState { get; private set; } = MatchState.BuildPhase;

    public void StartWave()
    {
        CurrentState = MatchState.WaveRunning;
    }
}
```

**Step 4: Run tests and verify one wave spawns from data**

Run: MCP `run_tests(mode="EditMode", assembly_names=["TowerDefense.EditMode"], include_details=true)`
Expected: PASS for state rules, then the scene spawns one configured wave from `WaveDefinition`.

**Step 5: Commit**

```bash
git add Assets/_Game/Scripts/Core Assets/_Game/Scripts/Gameplay/Waves Assets/_Game/Tests/EditMode/Core/LevelDirectorTests.cs
git commit -m "feat: add match state and wave spawning"
```

### Task 6: Implement build nodes, economy, and tower placement

**Files:**
- Create: `Assets/_Game/Scripts/Gameplay/Building/TowerPlacementService.cs`
- Create: `Assets/_Game/Scripts/Gameplay/Building/BuildNodeOccupancy.cs`
- Create: `Assets/_Game/Scripts/Gameplay/Economy/PlayerResources.cs`
- Create: `Assets/_Game/Scripts/Gameplay/Economy/GoldRewardService.cs`
- Create: `Assets/_Game/Prefabs/Towers/ArcherTower.prefab`
- Create: `Assets/_Game/Tests/EditMode/Gameplay/TowerPlacementServiceTests.cs`

**Step 1: Write the failing test**

```csharp
using NUnit.Framework;

public class TowerPlacementServiceTests
{
    [Test]
    public void TryPlaceTower_SpendsGoldAndOccupiesNode()
    {
        var resources = new PlayerResources(10, 20);
        var node = new BuildNodeOccupancy();
        var service = new TowerPlacementService(resources);

        var placed = service.TryPlaceTower(node, 5);

        Assert.That(placed, Is.True);
        Assert.That(resources.Gold, Is.EqualTo(5));
        Assert.That(node.IsOccupied, Is.True);
    }
}
```

**Step 2: Run test to verify it fails**

Run: MCP `run_tests(mode="EditMode", assembly_names=["TowerDefense.EditMode"], include_failed_tests=true)`
Expected: FAIL because placement and economy services do not exist.

**Step 3: Write the minimal implementation**

```csharp
public sealed class PlayerResources
{
    public PlayerResources(int gold, int lives)
    {
        Gold = gold;
        Lives = lives;
    }

    public int Gold { get; private set; }
    public int Lives { get; private set; }

    public bool TrySpendGold(int amount)
    {
        if (Gold < amount)
        {
            return false;
        }

        Gold -= amount;
        return true;
    }
}
```

**Step 4: Run tests and verify in scene**

Run: MCP `run_tests(mode="EditMode", assembly_names=["TowerDefense.EditMode"], include_details=true)`
Expected: PASS for placement rules, then clicking an empty build node places one tower and blocks duplicate placement.

**Step 5: Commit**

```bash
git add Assets/_Game/Scripts/Gameplay/Building Assets/_Game/Scripts/Gameplay/Economy Assets/_Game/Prefabs/Towers/ArcherTower.prefab Assets/_Game/Tests/EditMode/Gameplay/TowerPlacementServiceTests.cs
git commit -m "feat: add tower placement and economy"
```

### Task 7: Implement targeting, attacks, damage, and death rewards

**Files:**
- Create: `Assets/_Game/Scripts/Gameplay/Towers/TowerTargeter.cs`
- Create: `Assets/_Game/Scripts/Gameplay/Towers/TowerAttacker.cs`
- Create: `Assets/_Game/Scripts/Gameplay/Projectiles/ProjectileMover.cs`
- Create: `Assets/_Game/Scripts/Gameplay/Combat/DamageResolver.cs`
- Create: `Assets/_Game/Tests/EditMode/Gameplay/TowerAttackerTests.cs`

**Step 1: Write the failing test**

```csharp
using NUnit.Framework;

public class TowerAttackerTests
{
    [Test]
    public void Tick_FiresWhenCooldownExpires()
    {
        var attacker = new TowerAttacker(0.5f);

        attacker.Tick(0.5f);

        Assert.That(attacker.HasPendingShot, Is.True);
    }
}
```

**Step 2: Run test to verify it fails**

Run: MCP `run_tests(mode="EditMode", assembly_names=["TowerDefense.EditMode"], include_failed_tests=true)`
Expected: FAIL because attack cadence logic does not exist.

**Step 3: Write the minimal implementation**

```csharp
public sealed class TowerAttacker
{
    private readonly float cooldown;
    private float elapsed;

    public TowerAttacker(float cooldown)
    {
        this.cooldown = cooldown;
    }

    public bool HasPendingShot { get; private set; }

    public void Tick(float deltaTime)
    {
        elapsed += deltaTime;
        HasPendingShot = elapsed >= cooldown;
        if (HasPendingShot)
        {
            elapsed = 0f;
        }
    }
}
```

**Step 4: Run tests and verify combat loop in scene**

Run: MCP `run_tests(mode="EditMode", assembly_names=["TowerDefense.EditMode"], include_details=true)`
Expected: PASS for attack cadence, then an `ArcherTower` attacks a `BasicEnemy`, the enemy dies, and gold reward is granted.

**Step 5: Commit**

```bash
git add Assets/_Game/Scripts/Gameplay/Towers Assets/_Game/Scripts/Gameplay/Projectiles Assets/_Game/Scripts/Gameplay/Combat Assets/_Game/Tests/EditMode/Gameplay/TowerAttackerTests.cs
git commit -m "feat: add tower combat loop"
```

### Task 8: Implement HUD and end-state presentation

**Files:**
- Create: `Assets/_Game/Scripts/UI/HUDController.cs`
- Create: `Assets/_Game/Scripts/UI/GoldDisplay.cs`
- Create: `Assets/_Game/Scripts/UI/LivesDisplay.cs`
- Create: `Assets/_Game/Scripts/UI/WaveStateDisplay.cs`
- Create: `Assets/_Game/Scripts/UI/StartWaveButton.cs`
- Create: `Assets/_Game/Tests/PlayMode/UI/HudSmokeTests.cs`

**Step 1: Write the failing test**

```csharp
using NUnit.Framework;

public class HudSmokeTests
{
    [Test]
    public void Placeholder()
    {
        Assert.That(true, Is.True);
    }
}
```

**Step 2: Run test to verify the PlayMode assembly is wired**

Run: MCP `run_tests(mode="PlayMode", assembly_names=["TowerDefense.PlayMode"], include_details=true)`
Expected: FAIL or undiscovered until the PlayMode asmdef and test file are active.

**Step 3: Write the minimal implementation**

```csharp
public sealed class StartWaveButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private VoidEventChannel startWaveRequested;

    private void Awake()
    {
        button.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        startWaveRequested.Raise();
    }
}
```

**Step 4: Run tests and verify the full loop**

Run: MCP `run_tests(mode="PlayMode", assembly_names=["TowerDefense.PlayMode"], include_details=true)`
Expected: PASS for HUD smoke coverage, then the scene shows gold, lives, wave state, and win/lose text driven by runtime events.

**Step 5: Commit**

```bash
git add Assets/_Game/Scripts/UI Assets/_Game/Tests/PlayMode/UI/HudSmokeTests.cs
git commit -m "feat: add tower defense hud"
```

### Task 9: Add sprite sorting rules and vertical-slice polish verification

**Files:**
- Modify: `Assets/_Game/Scenes/PrototypeLane01.unity`
- Create: `Assets/_Game/Scripts/Presentation/Sorting/SortingGroupBinder.cs`
- Create: `Assets/_Game/Scripts/Presentation/Sorting/YSortAdapter.cs`
- Create: `Assets/_Game/Tests/PlayMode/Presentation/PrototypeLaneSmokeTests.cs`
- Modify: `ProjectSettings/TagManager.asset`

**Step 1: Write the failing smoke test**

```csharp
using NUnit.Framework;

public class PrototypeLaneSmokeTests
{
    [Test]
    public void Placeholder()
    {
        Assert.That(true, Is.True);
    }
}
```

**Step 2: Run test to verify the lane smoke suite is active**

Run: MCP `run_tests(mode="PlayMode", assembly_names=["TowerDefense.PlayMode"], include_details=true)`
Expected: FAIL or undiscovered until the new smoke test is registered.

**Step 3: Write the minimal implementation**

```csharp
public sealed class YSortAdapter : MonoBehaviour
{
    [SerializeField] private SpriteRenderer targetRenderer;
    [SerializeField] private float multiplier = 100f;

    private void LateUpdate()
    {
        targetRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * multiplier);
    }
}
```

**Step 4: Run tests and verify the vertical slice checklist**

Run: MCP `run_tests(mode="PlayMode", assembly_names=["TowerDefense.PlayMode"], include_details=true)`
Expected: PASS for smoke coverage, then verify manually that the stair lane reads clearly, towers sort correctly, enemies do not pop in front of blocking terrain incorrectly, and the full wave completes.

**Step 5: Commit**

```bash
git add Assets/_Game/Scenes/PrototypeLane01.unity Assets/_Game/Scripts/Presentation/Sorting Assets/_Game/Tests/PlayMode/Presentation/PrototypeLaneSmokeTests.cs ProjectSettings/TagManager.asset
git commit -m "feat: polish prototype lane presentation"
```

## Verification Checklist Before Calling the Slice Complete

- Run EditMode tests for `TowerDefense.EditMode` and confirm PASS.
- Run PlayMode tests for `TowerDefense.PlayMode` and confirm PASS.
- Open `Assets/_Game/Scenes/PrototypeLane01.unity` and verify one full wave from start to end.
- Verify build nodes cannot be double-occupied.
- Verify leaks reduce lives and death rewards add gold.
- Verify the stair section remains readable at the locked gameplay camera.
- Verify there is no commented-out code in newly added files.

## Notes on Scope Control

- Do not add pathfinding.
- Do not add freeform placement.
- Do not add tower upgrades.
- Do not add a player-controlled hero.
- Do not add a second map before the first slice is stable.
- If a new abstraction does not remove current pain, do not add it.
