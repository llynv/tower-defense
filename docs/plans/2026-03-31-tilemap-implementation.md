# Tilemap Visual Terrain Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Add visual tilemap terrain to PrototypeLane01 so the game looks like a game instead of invisible gizmos on a blank background.

**Architecture:** Manual hand-painted tilemaps using Tiny Swords pre-sliced tiles. Grid with 6 Tilemap child layers (BG_Water, WaterFoam, FlatGround, Shadow, ElevatedGround, PathOverlay). Decorations as SpriteRenderer GameObjects. No custom rendering scripts — pure Unity Tilemap + SpriteRenderer.

**Tech Stack:** Unity 6 Tilemap, Tile Palette, Tiny Swords asset pack, SpriteRenderers for decorations

**Design doc:** `docs/plans/2026-03-31-tilemap-design.md`

---

## Pre-Implementation Notes

### Existing Sorting Layers (ProjectSettings/TagManager.asset)

Already defined: Default (0), Terrain (1), Props (2), Units (3), Projectiles (4). These are NOT actively used — all gameplay prefabs currently use Default (sortingLayerID: 0). We will use these existing layers.

### Tile Assets

- Pre-sliced tiles: `Assets/Tiny Swords/Terrain/Tileset/Tilemap Settings/Sliced Tiles/Tilemap_color{1-5}_{0-43}.asset`
- Water animated: `Assets/Tiny Swords/Terrain/Tileset/Tilemap Settings/Water Tile animated.asset`
- Water BG: `Assets/Tiny Swords/Terrain/Tileset/Tilemap Settings/Water Background color.asset`
- Shadow: `Assets/Tiny Swords/Terrain/Tileset/Tilemap Settings/Shadow.asset`
- Grid cell size: 1x1x0, PPU: 64

### Camera Viewport

Orthographic size 6 at (0,0,-10). Visible area ~22x12 tiles. Lane waypoints: (-6,-3) to (6,3). Build nodes at: (-5,0), (-5,2), (-1,-2), (-1,2), (1,-2), (1,2), (5,0), (5,-2).

---

### Task 1: Assign Sorting Layers to Gameplay Prefabs

Move existing gameplay prefabs from Default to their proper sorting layers so tilemap terrain renders behind them.

**Files:**
- Modify: `Assets/_Game/Prefabs/Towers/ArcherTower.prefab` — SpriteRenderer → sorting layer Units
- Modify: `Assets/_Game/Prefabs/Enemies/BasicEnemy.prefab` — SpriteRenderer → sorting layer Units
- Modify: `Assets/_Game/Prefabs/Projectiles/Projectile.prefab` — SpriteRenderer → sorting layer Projectiles
- Modify: `Assets/_Game/Prefabs/UI/EnemyHealthBar.prefab` — Canvas → sorting layer Units, order 1 (above unit sprites)

**Step 1: Set ArcherTower SpriteRenderer sorting layer to "Units"**

Use MCP `manage_components` set_property on the ArcherTower prefab's SpriteRenderer:
- Open prefab stage for `Assets/_Game/Prefabs/Towers/ArcherTower.prefab`
- Set SpriteRenderer `sortingLayerName` = "Units", `sortingOrder` = 0
- Save and close prefab stage

**Step 2: Set BasicEnemy SpriteRenderer sorting layer to "Units"**

Same as above for `Assets/_Game/Prefabs/Enemies/BasicEnemy.prefab`.

**Step 3: Set Projectile SpriteRenderer sorting layer to "Projectiles"**

Same for `Assets/_Game/Prefabs/Projectiles/Projectile.prefab`, sorting layer "Projectiles".

**Step 4: Set EnemyHealthBar Canvas sorting layer to "Units" order 1**

The EnemyHealthBar uses a World Space Canvas. Set its sorting layer to "Units" with order 1.

**Step 5: Run all tests to verify no regressions**

Run: MCP `run_tests` (EditMode)
Expected: All 139 EditMode tests pass. Sorting layer changes are visual-only and should not affect test behavior.

**Step 6: Commit**

```
git add -A && git commit -m "Assign gameplay prefabs to proper sorting layers (Units, Projectiles)"
```

---

### Task 2: Create Grid and Tilemap Layer Hierarchy

Create the Grid GameObject with all 6 Tilemap child layers in the PrototypeLane01 scene, each configured with the correct sorting layer and order.

**Files:**
- Modify: `Assets/_Game/Scenes/PrototypeLane01.unity`

**Step 1: Create Grid root GameObject**

Use MCP `manage_gameobject` create:
- Name: "Terrain"
- Position: (0, 0, 0)
- Add component: `Grid` (comes automatically with Grid)

The Grid component should have cellSize (1, 1, 0), cellLayout Rectangle, cellSwizzle XYZ — these are the defaults.

**Step 2: Create BG_Water tilemap layer**

Use MCP `manage_gameobject` create:
- Name: "BG_Water"
- Parent: "Terrain"
- Components: Tilemap, TilemapRenderer

Set TilemapRenderer sortingLayerName = "Terrain", sortingOrder = 0.

**Step 3: Create WaterFoam tilemap layer**

Same pattern:
- Name: "WaterFoam", Parent: "Terrain"
- TilemapRenderer: sortingLayerName = "Terrain", sortingOrder = 1

**Step 4: Create FlatGround tilemap layer**

- Name: "FlatGround", Parent: "Terrain"
- TilemapRenderer: sortingLayerName = "Terrain", sortingOrder = 2

**Step 5: Create Shadow tilemap layer**

- Name: "Shadow", Parent: "Terrain"
- TilemapRenderer: sortingLayerName = "Terrain", sortingOrder = 3

**Step 6: Create ElevatedGround tilemap layer**

- Name: "ElevatedGround", Parent: "Terrain"
- TilemapRenderer: sortingLayerName = "Terrain", sortingOrder = 4

**Step 7: Create PathOverlay tilemap layer**

- Name: "PathOverlay", Parent: "Terrain"
- TilemapRenderer: sortingLayerName = "Terrain", sortingOrder = 5

**Step 8: Create Decorations container**

Use MCP `manage_gameobject` create:
- Name: "Decorations"
- Position: (0, 0, 0)
- No special components — just a transform parent for decoration GameObjects

**Step 9: Save the scene**

Use MCP `manage_scene` save.

**Step 10: Run all tests**

Run: MCP `run_tests` (EditMode)
Expected: All 139 tests pass.

**Step 11: Commit**

```
git add -A && git commit -m "Add Grid with 6 tilemap layers and Decorations container to PrototypeLane01"
```

---

### Task 3: Create Tile Palette

Create a Tile Palette asset that references the Tiny Swords sliced tiles for use in the editor's Tile Palette window.

**Files:**
- Create: `Assets/_Game/Tilemaps/Palettes/` folder
- Create: Tile Palette asset via Unity

**Step 1: Create folder structure**

Use MCP `manage_asset` create_folder for `Assets/_Game/Tilemaps/Palettes`.

**Step 2: Create Tile Palette**

Use MCP `manage_asset` create or the menu item to create a Tile Palette prefab. The palette is a Grid+Tilemap prefab that the Tile Palette editor window uses. Create it at `Assets/_Game/Tilemaps/Palettes/TinySwords.prefab`.

Note: Tile Palettes are created via Window > 2D > Tile Palette in Unity. They are standard Grid prefabs. If MCP cannot create one directly, create a minimal Grid+Tilemap prefab and it will work as a palette.

**Step 3: Commit**

```
git add -A && git commit -m "Add Tile Palette for Tiny Swords tileset"
```

---

### Task 4: Paint BG_Water Layer

Fill the BG_Water tilemap with the Water Background color tile covering the entire visible area plus a generous margin.

**Files:**
- Modify: `Assets/_Game/Scenes/PrototypeLane01.unity`

**Step 1: Paint water background**

The visible area is approximately (-11,-7) to (11,7). Add 2 tiles of margin on each side: (-13,-9) to (13,9).

Use the Water Background color tile (`Assets/Tiny Swords/Terrain/Tileset/Tilemap Settings/Water Background color.asset`, GUID: find via MCP) to fill the entire BG_Water tilemap from (-13,-9) to (13,9).

This can be done:
- Via Tile Palette editor in Unity (box fill tool)
- Or via an editor script that programmatically sets tiles on the tilemap

**Step 2: Save scene**

**Step 3: Commit**

```
git add -A && git commit -m "Paint water background layer"
```

---

### Task 5: Paint ElevatedGround and FlatGround Layers

Paint the main terrain: an elevated grass island with a flat ground path channel following the enemy lane.

**Files:**
- Modify: `Assets/_Game/Scenes/PrototypeLane01.unity`

**Step 1: Plan the elevated island shape**

The island should cover approximately (-8,-5) to (8,5) with the enemy path cutting through. The path is 2-3 tiles wide following the waypoints.

**Step 2: Paint FlatGround layer**

Using color1 tiles (Flat Ground, 16-tile pattern):
- Paint a 2-tile-wide channel following the lane waypoints: (-6,-3) → (-3,-3) → (-3,0) → (0,0) → (3,0) → (3,3) → (6,3)
- Use appropriate edge tiles (tiles 1-16 from the Flat Ground pattern in tile guide 05)
- The path sits at the lower elevation, directly on water

**Step 3: Paint ElevatedGround layer**

Using color2 tiles (Elevated Ground, 24-tile pattern):
- Paint the main island covering the play area, leaving gaps where the path channel is
- Use cliff tiles (tiles 17-24) on edges that face down toward flat ground or water
- Use grass top tiles (tiles 1-16) for the elevated surface
- Build node positions should be on solid elevated ground

**Step 4: Save scene**

**Step 5: Visual verification**

Enter Play mode briefly to verify:
- Water visible at edges
- Elevated island renders above water
- Path channel is visible at lower elevation
- Build nodes are on elevated ground
- Enemies would walk along the flat ground path

**Step 6: Commit**

```
git add -A && git commit -m "Paint elevated ground island and flat ground path"
```

---

### Task 6: Paint Shadow and WaterFoam Layers

Add depth shadows under elevated cliff edges and animated water foam at land-water boundaries.

**Files:**
- Modify: `Assets/_Game/Scenes/PrototypeLane01.unity`

**Step 1: Paint Shadow layer**

Using the Shadow tile (`Assets/Tiny Swords/Terrain/Tileset/Tilemap Settings/Shadow.asset`):
- Place shadow tiles below every elevated cliff edge (offset 1 tile down)
- The shadow is 128x128 (oversized on 64 grid) — it will overlap neighboring cells by design
- Per tile guide 03: shadows are placed on the bottom edge of the elevated ground cliffs, shifted down one full tile

**Step 2: Paint WaterFoam layer**

Using the Water Tile animated (`Assets/Tiny Swords/Terrain/Tileset/Tilemap Settings/Water Tile animated.asset`):
- Place at every boundary where land (flat or elevated) meets water
- The foam is 128x128 animated — it overlaps neighboring cells by design
- Per tile guide 04: foam is placed at all areas that touch the BG Color (water)

**Step 3: Save scene**

**Step 4: Visual verification**

- Shadows create depth illusion under elevated cliffs
- Water foam animates at land-water edges
- No visual gaps between layers

**Step 5: Commit**

```
git add -A && git commit -m "Paint shadow and water foam layers"
```

---

### Task 7: Place Decorations

Add decoration GameObjects (bushes, rocks, clouds, water rocks, rubber duck) as SpriteRenderers with appropriate sorting layers.

**Files:**
- Modify: `Assets/_Game/Scenes/PrototypeLane01.unity`

**Step 1: Place bushes on elevated ground**

Create 4-6 Bush GameObjects as children of "Decorations":
- Use SpriteRenderer with sprites from `Assets/Tiny Swords/Terrain/Decorations/Bushes/`
- Add Animator component with the corresponding AnimatorController (e.g., `Bush 1.controller`)
- Sorting layer: "Props", order: 0
- Place on elevated grass areas AWAY from build nodes and the path
- Suggested positions: corners and edges of the island where they won't obstruct gameplay

**Step 2: Place rocks on elevated ground edges**

Create 3-4 Rock GameObjects:
- Use SpriteRenderer with sprites from `Assets/Tiny Swords/Terrain/Decorations/Rocks/`
- Sorting layer: "Props", order: 0
- Place near cliff edges on elevated ground

**Step 3: Place rocks in water**

Create 2-3 Water Rock GameObjects:
- Use sprites from `Assets/Tiny Swords/Terrain/Decorations/Rocks in the Water/`
- Add Animator with corresponding controller
- Sorting layer: "Terrain", order: 6 (above all tilemap layers, but below gameplay)
- Place in water areas near the island edges

**Step 4: Place clouds**

Create 3-4 Cloud GameObjects:
- Use sprites from `Assets/Tiny Swords/Terrain/Decorations/Clouds/`
- Sorting layer: "Props", order: 1 (above bushes/rocks)
- Scatter across the scene for atmosphere

**Step 5: Place rubber duck**

Create 1 Rubber Duck GameObject:
- Use sprite from `Assets/Tiny Swords/Terrain/Decorations/Rubber Duck/`
- Add Animator with `Rubber duck.controller`
- Sorting layer: "Terrain", order: 6
- Place in a water area as an Easter egg

**Step 6: Save scene**

**Step 7: Visual verification**

- Decorations render on correct layers (above terrain, below/with gameplay)
- Animated decorations play their animations
- Nothing obstructs build nodes or the enemy path

**Step 8: Commit**

```
git add -A && git commit -m "Place decoration sprites (bushes, rocks, clouds, rubber duck)"
```

---

### Task 8: Paint PathOverlay Layer (Optional Refinement)

If the flat ground path needs visual distinction from the flat ground base, paint a path overlay using a different color variant.

**Files:**
- Modify: `Assets/_Game/Scenes/PrototypeLane01.unity`

**Step 1: Evaluate whether PathOverlay is needed**

After Tasks 5-7, evaluate visually:
- If the flat ground path is already visually distinct from the elevated ground → skip this task
- If the path blends too much with surroundings → paint color3 or color4 tiles as a road overlay

**Step 2: If needed, paint path overlay**

Using color3 or color4 flat ground tiles, paint a 1-2 tile wide road directly on the PathOverlay tilemap layer, following the lane waypoints.

**Step 3: Save and commit if changes were made**

```
git add -A && git commit -m "Paint path overlay for visual lane distinction"
```

---

### Task 9: Final Integration and Regression Testing

Verify everything works together: tilemap renders correctly, gameplay is unaffected, all tests pass.

**Files:**
- No new files

**Step 1: Run full test suite**

Run: MCP `run_tests` (EditMode)
Expected: All 139 EditMode tests pass.

Run: MCP `run_tests` (PlayMode)
Expected: All 2 PlayMode tests pass.

**Step 2: Enter Play mode and verify gameplay**

- Start a wave and verify enemies spawn, follow the path, and reach the goal
- Verify towers can be placed on build nodes
- Verify projectiles render above terrain
- Verify UI renders above everything
- Verify EnemyHealthBar renders above enemy sprites

**Step 3: Check scene hierarchy is clean**

Verify the scene has:
- Terrain (Grid) with 6 Tilemap children
- Decorations container with decoration GameObjects
- All existing gameplay objects unchanged

**Step 4: Commit any final adjustments**

```
git add -A && git commit -m "Complete tilemap terrain integration"
```

---

## Task Summary

| Task | Description | Automated? | Est. Effort |
|------|-------------|-----------|-------------|
| 1 | Assign sorting layers to gameplay prefabs | MCP | Small |
| 2 | Create Grid + 6 Tilemap layers + Decorations container | MCP | Small |
| 3 | Create Tile Palette | MCP/Editor | Small |
| 4 | Paint BG_Water (water background fill) | Editor/Script | Small |
| 5 | Paint ElevatedGround + FlatGround (main terrain) | Editor | Medium |
| 6 | Paint Shadow + WaterFoam layers | Editor | Medium |
| 7 | Place decorations (sprites) | MCP | Medium |
| 8 | Paint PathOverlay (optional refinement) | Editor | Small |
| 9 | Final integration + regression testing | MCP | Small |

## Important Notes for Implementer

1. **Tile painting (Tasks 4-6, 8)** is the core art work. This requires either:
   - Using Unity's Tile Palette editor (recommended for quality)
   - Writing a temporary editor script to programmatically place tiles (faster but less artistic control)
   - Using MCP to set tilemap tile data directly (if supported)

2. **The Tiny Swords tile guide images** at `Assets/Tiny Swords/Terrain/Tileset/Unity Tile Guide/` show exactly which numbered tile goes where. Reference these when painting.

3. **Shadow and WaterFoam are 128px on a 64 grid.** They intentionally overlap. This is by design per the tile guide.

4. **Do NOT move or modify** any existing gameplay objects (waypoints, build nodes, spawn/goal points).

5. **All 141 tests must remain green** after every task.
