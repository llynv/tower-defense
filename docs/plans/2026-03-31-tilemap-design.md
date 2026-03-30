# Tilemap Visual Terrain Design

**Date:** 2026-03-31
**Status:** Approved
**Scope:** Visual-only tilemap rendering for PrototypeLane01 using Tiny Swords assets

## Goal

Add tilemap-based terrain rendering to the existing PrototypeLane01 scene. This is purely visual — the existing waypoint, build-node, spawn, and goal systems remain unchanged. The game should look like a game instead of invisible gizmos on a blank background.

## Approach: Manual Hand-Painted Tilemaps

Hand-paint each tilemap layer in the Unity Tile Palette editor using the 220+ pre-sliced tiles from the Tiny Swords asset pack. Decorations placed as SpriteRenderer GameObjects for animation support and positioning flexibility.

### Why Not RuleTiles or Programmatic Placement

- Tiny Swords elevated ground has two cliff variants (water-facing vs ground-facing) that depend on context below — hard to express in standard RuleTile rules
- Shadow and Water Foam are 128px oversized sprites on a 64px grid with intentional overlap — RuleTiles handle this poorly
- One level does not justify the engineering cost of automated tile placement
- The asset pack's tile guide is designed for manual placement (numbered pieces with examples)

## Tileset Facts

- **Tile size:** 64x64 pixels
- **Pixels Per Unit:** 64 (1 tile = 1 world unit)
- **Grid cell size:** 1x1x0, Rectangle layout
- **Shadow sprite:** 128x128, placed on 64 grid (overlaps neighbors), offset 1 tile down from elevated edges
- **Water Foam sprite:** 128x128, animated, placed on 64 grid at land-water boundaries
- **Color variants:** 5 tileset colors (color1 through color5), 44+ tiles per color
- **Flat Ground:** 16 tile pattern (edges, corners, inner corners, center)
- **Elevated Ground:** 24 tile pattern (same as flat + 8 cliff tiles in two variants)

## Architecture

A single `Grid` GameObject in the scene with child Tilemap GameObjects per layer. No custom scripts for rendering. Decorations are SpriteRenderer GameObjects in a separate container.

## Sorting Layers

Two new sorting layers added to the project:

| Sorting Layer | Purpose | Relative Order |
|---------------|---------|----------------|
| Background | Water, foam | Below Ground |
| Ground | Land tiles, shadows, path, decorations | Below Default |
| Default | Gameplay objects (enemies, towers, projectiles) | Existing |

## Tilemap Layer Stack

Bottom to top within the Grid:

| # | Layer Name | Sorting Layer | Order in Layer | Content | Notes |
|---|-----------|---------------|----------------|---------|-------|
| 0 | BG_Water | Background | 0 | Water Background color tile | Solid fill covering visible area + margin |
| 1 | WaterFoam | Background | 1 | Water Tile animated | 128px animated sprite at land-water edges |
| 2 | FlatGround | Ground | 0 | color1 flat ground tiles (16 types) | Lowest land elevation, path channel |
| 3 | Shadow | Ground | 1 | Shadow tile | 128px sprite below elevated cliff edges |
| 4 | ElevatedGround | Ground | 2 | color2 elevated tiles (24 types) | Main play area with cliffs |
| 5 | PathOverlay | Ground | 3 | color3 or color4 flat tiles | Visual dirt/road following enemy lane (optional, may use FlatGround directly) |

## Camera and Visible Area

- **Camera:** orthographic, size 6, position (0, 0, -10)
- **Visible area (16:9):** approximately X: -10.67 to +10.67, Y: -6 to +6
- **Visible tile range:** roughly 22x12 tiles
- **Grid origin:** (0,0) at world center

## Map Layout Concept

### Existing Gameplay Geometry (unchanged)

- **Lane waypoints:** (-6,-3) → (-3,-3) → (-3,0) → (0,0) → (3,0) → (3,3) → (6,3)
- **Build nodes:** (-5,0), (-5,2), (-1,-2), (-1,2), (1,-2), (1,2), (5,0), (5,-2)
- **Spawn:** (-6,-3), **Goal:** (6,3)

### Terrain Design

- Elevated grass island covers most of the play area (color2 elevated tiles)
- Enemy path is a flat ground channel cut through the island (color1 flat tiles at lower elevation)
- Water surrounds the island at the screen edges
- Build nodes sit on elevated ground adjacent to the path
- Water foam at all land-water boundaries
- Shadows below all elevated cliff edges

## Decorations

Placed as individual SpriteRenderer GameObjects (not tilemap tiles):

| Decoration | Variants | Animated | Placement |
|-----------|----------|----------|-----------|
| Bushes | 4 | Yes | On elevated grass, away from build nodes |
| Rocks | 4 | No | On grass edges, near cliffs |
| Rocks in Water | 4 | Yes | In water near land edges |
| Clouds | 8 | No | Scattered across the scene |
| Rubber Duck | 1 | Yes | In water (Easter egg) |

Decorations use the **Ground** sorting layer with order 4+ (above elevated ground) or **Background** layer for water rocks.

## Asset Organization

```
Assets/_Game/
  Tilemaps/
    Palettes/
      TinySwords.prefab     -- Tile Palette for painting
```

Existing Tiny Swords sliced tiles stay in their original location under `Assets/Tiny Swords/`.

## What Does NOT Change

- LanePathAuthoring / LanePath / waypoint transforms
- BuildNode positions and behavior
- SpawnPoint / GoalPoint positions
- Camera settings
- Gameplay scripts
- UI system
- Test suite (141 tests remain green)

## Testing Strategy

Primarily a visual/art task with minimal automated testing:

- **EditMode test:** Verify sorting layer configuration exists
- **Visual verification:** Manual inspection in editor and play mode
- **Regression:** Existing 141 tests must remain green
