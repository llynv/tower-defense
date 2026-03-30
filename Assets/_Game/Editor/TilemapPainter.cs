using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TowerDefense.Editor
{
    public static class TilemapPainter
    {
        const string TilePath = "Assets/Tiny Swords/Terrain/Tileset/Tilemap Settings/Sliced Tiles/";
        const string SpecialPath = "Assets/Tiny Swords/Terrain/Tileset/Tilemap Settings/";

        static TileBase LoadTile(string color, int index)
        {
            string path = $"{TilePath}Tilemap_{color}_{index}.asset";
            var tile = AssetDatabase.LoadAssetAtPath<TileBase>(path);
            if (tile == null) Debug.LogError($"TilemapPainter: Cannot load tile at {path}");
            return tile;
        }

        static TileBase LoadSpecial(string name)
        {
            string path = $"{SpecialPath}{name}.asset";
            var tile = AssetDatabase.LoadAssetAtPath<TileBase>(path);
            if (tile == null) Debug.LogError($"TilemapPainter: Cannot load tile at {path}");
            return tile;
        }

        static Tilemap GetTilemap(string layerName)
        {
            var go = GameObject.Find($"Terrain/{layerName}");
            if (go == null)
            {
                Debug.LogError($"TilemapPainter: Cannot find Terrain/{layerName}");
                return null;
            }
            var tm = go.GetComponent<Tilemap>();
            if (tm == null) Debug.LogError($"TilemapPainter: No Tilemap on {layerName}");
            return tm;
        }

        static void Fill(Tilemap tm, TileBase tile, int xMin, int yMin, int xMax, int yMax)
        {
            for (int x = xMin; x <= xMax; x++)
                for (int y = yMin; y <= yMax; y++)
                    tm.SetTile(new Vector3Int(x, y, 0), tile);
        }

        static void SetTile(Tilemap tm, TileBase tile, int x, int y)
        {
            tm.SetTile(new Vector3Int(x, y, 0), tile);
        }

        // ─── Flat ground tile indices (color1) ───
        // Sprite sheet: left 4 columns, reading L-R, T-B
        // Guide#  -> file index
        // Row 1: 1=0,  2=1,  3=2,  13=3
        // Row 2: 4=8,  5=9,  6=10, 14=11
        // Row 3: 7=16, 8=17, 9=18, 15=19
        // Row 4: 10=24,11=25,12=26,16=27

        // Guide numbers for flat ground:
        // 1=TL corner, 2=T edge, 3=TR corner, 13=inner TR corner
        // 4=L edge, 5=center, 6=R edge, 14=inner BR corner (rotated)
        // 7=BL corner, 8=B edge, 9=BR corner, 15=inner BL corner
        // 10=BL bottom, 11=B bottom, 12=BR bottom, 16=inner TL corner

        const int F_TL = 0;      // guide 1: top-left corner
        const int F_T = 1;       // guide 2: top edge
        const int F_TR = 2;      // guide 3: top-right corner
        const int F_ITR = 3;     // guide 13: inner top-right corner
        const int F_L = 8;       // guide 4: left edge
        const int F_C = 9;       // guide 5: center fill
        const int F_R = 10;      // guide 6: right edge
        const int F_IBR = 11;    // guide 14: inner bottom-right corner
        const int F_BL = 16;     // guide 7: bottom-left corner
        const int F_B = 17;      // guide 8: bottom edge
        const int F_BR = 18;     // guide 9: bottom-right corner
        const int F_IBL = 19;    // guide 15: inner bottom-left corner
        const int F_BBL = 24;    // guide 10: bottom-bottom-left
        const int F_BB = 25;     // guide 11: bottom-bottom
        const int F_BBR = 26;    // guide 12: bottom-bottom-right
        const int F_ITL = 27;    // guide 16: inner top-left corner

        // ─── Elevated ground tile indices (color2) ───
        // Sprite sheet: right 4 columns, reading L-R, T-B
        // Row 1: 1=4,  2=5,  3=6,  13=7
        // Row 2: 4=12, 5=13, 6=14, 14=15
        // Row 3: 7=20, 8=21, 9=22, 15=23
        // Row 4: 10=28,11=29,12=30,16=31
        // Row 5: 17=34,18=35,19=36,20=37 (cliff bottom facing water)
        // Row 6: 21=40,22=41,23=42,24=43 (cliff bottom facing walkable)

        const int E_TL = 4;      // guide 1: grass top-left corner
        const int E_T = 5;       // guide 2: grass top edge
        const int E_TR = 6;      // guide 3: grass top-right corner
        const int E_ITR = 7;     // guide 13: narrow/inner top
        const int E_L = 12;      // guide 4: grass left edge
        const int E_C = 13;      // guide 5: grass center fill
        const int E_R = 14;      // guide 6: grass right edge
        const int E_IR = 15;     // guide 14: narrow/inner
        const int E_BL = 20;     // guide 7: grass bottom-left with cliff
        const int E_B = 21;      // guide 8: grass bottom edge with cliff
        const int E_BR = 22;     // guide 9: grass bottom-right with cliff
        const int E_IB = 23;     // guide 15: narrow/inner bottom
        const int E_CL_L = 28;   // guide 10: cliff face left
        const int E_CL_C = 29;   // guide 11: cliff face center
        const int E_CL_R = 30;   // guide 12: cliff face right
        const int E_CL_I = 31;   // guide 16: cliff face inner
        const int E_WBL = 34;    // guide 17: cliff bottom-left (water side)
        const int E_WB = 35;     // guide 18: cliff bottom (water side)
        const int E_WBR = 36;    // guide 19: cliff bottom-right (water side)
        const int E_WBI = 37;    // guide 20: cliff bottom inner (water side)
        const int E_FBL = 40;    // guide 21: cliff bottom-left (flat ground side)
        const int E_FB = 41;     // guide 22: cliff bottom (flat ground side)
        const int E_FBR = 42;    // guide 23: cliff bottom-right (flat ground side)
        const int E_FBI = 43;    // guide 24: cliff bottom inner (flat ground side)

        [MenuItem("Tools/Tilemap/Paint BG Water")]
        public static void PaintBGWater()
        {
            var tm = GetTilemap("BG_Water");
            if (tm == null) return;
            var tile = LoadSpecial("Water Background color");
            if (tile == null) return;
            Undo.RecordObject(tm, "Paint BG Water");
            Fill(tm, tile, -13, -9, 13, 9);
            tm.RefreshAllTiles();
            EditorUtility.SetDirty(tm);
            Debug.Log("TilemapPainter: Painted BG_Water");
        }

        [MenuItem("Tools/Tilemap/Paint Flat Ground")]
        public static void PaintFlatGround()
        {
            var tm = GetTilemap("FlatGround");
            if (tm == null) return;
            Undo.RecordObject(tm, "Paint Flat Ground");
            tm.ClearAllTiles();

            // The enemy path: 3 tiles wide following waypoints
            // Waypoints: (-6,-3) -> (-3,-3) -> (-3,0) -> (0,0) -> (3,0) -> (3,3) -> (6,3)
            //
            // Path segments (3 tiles wide):
            // Segment 1: horizontal from x=-8 to x=-3, y=-4 to -2 (entering from left)
            // Segment 2: vertical from x=-4 to -2, y=-3 to 0 (going up)
            // Segment 3: horizontal from x=-3 to 3, y=-1 to 1 (middle)
            // Segment 4: vertical from x=2 to 4, y=0 to 3 (going up)
            // Segment 5: horizontal from x=3 to 8, y=2 to 4 (exiting right)

            // Build a bool grid for the path
            bool[,] isPath = new bool[30, 20]; // offset: x+13, y+9
            int ox = 13, oy = 9;

            void MarkPath(int xMin, int yMin, int xMax, int yMax)
            {
                for (int x = xMin; x <= xMax; x++)
                    for (int y = yMin; y <= yMax; y++)
                        if (x + ox >= 0 && x + ox < 30 && y + oy >= 0 && y + oy < 20)
                            isPath[x + ox, y + oy] = true;
            }

            // Horizontal entry: x=-8..-3, y=-4..-2
            MarkPath(-8, -4, -3, -2);
            // Vertical connector: x=-4..-2, y=-3..0
            MarkPath(-4, -3, -2, 0);
            // Horizontal middle: x=-3..3, y=-1..1
            MarkPath(-3, -1, 3, 1);
            // Vertical connector: x=2..4, y=0..3
            MarkPath(2, 0, 4, 3);
            // Horizontal exit: x=3..8, y=2..4
            MarkPath(3, 2, 8, 4);

            // Now paint tiles with proper edge detection
            for (int x = -13; x <= 13; x++)
            {
                for (int y = -9; y <= 9; y++)
                {
                    int gx = x + ox, gy = y + oy;
                    if (gx < 0 || gx >= 30 || gy < 0 || gy >= 20) continue;
                    if (!isPath[gx, gy]) continue;

                    bool up = gy + 1 < 20 && isPath[gx, gy + 1];
                    bool down = gy - 1 >= 0 && isPath[gx, gy - 1];
                    bool left = gx - 1 >= 0 && isPath[gx - 1, gy];
                    bool right = gx + 1 < 30 && isPath[gx + 1, gy];

                    int tileIdx = F_C; // default center

                    // Corners
                    if (!left && !up && right && down) tileIdx = F_TL;
                    else if (left && !up && right && down) tileIdx = F_T;
                    else if (left && !up && !right && down) tileIdx = F_TR;
                    else if (!left && up && !left && down) tileIdx = F_L;
                    else if (!left && up && right && down) tileIdx = F_L;
                    else if (left && up && !right && down) tileIdx = F_R;
                    else if (!left && up && right && !down) tileIdx = F_BL;
                    else if (left && up && right && !down) tileIdx = F_B;
                    else if (left && up && !right && !down) tileIdx = F_BR;
                    else if (!left && !up && right && !down) tileIdx = F_TL; // isolated top-left
                    else if (!left && !up && !right && down) tileIdx = F_TL; // narrow top
                    // Inner corners (all 4 neighbors present, but diagonal missing)
                    else if (left && up && right && down)
                    {
                        bool upLeft = (gx - 1 >= 0 && gy + 1 < 20) && isPath[gx - 1, gy + 1];
                        bool upRight = (gx + 1 < 30 && gy + 1 < 20) && isPath[gx + 1, gy + 1];
                        bool downLeft = (gx - 1 >= 0 && gy - 1 >= 0) && isPath[gx - 1, gy - 1];
                        bool downRight = (gx + 1 < 30 && gy - 1 >= 0) && isPath[gx + 1, gy - 1];

                        if (!upRight) tileIdx = F_ITR;
                        else if (!downRight) tileIdx = F_IBR;
                        else if (!downLeft) tileIdx = F_IBL;
                        else if (!upLeft) tileIdx = F_ITL;
                        else tileIdx = F_C;
                    }

                    SetTile(tm, LoadTile("color1", tileIdx), x, y);
                }
            }

            tm.RefreshAllTiles();
            EditorUtility.SetDirty(tm);
            Debug.Log("TilemapPainter: Painted FlatGround");
        }

        [MenuItem("Tools/Tilemap/Paint Elevated Ground")]
        public static void PaintElevatedGround()
        {
            var tm = GetTilemap("ElevatedGround");
            if (tm == null) return;
            Undo.RecordObject(tm, "Paint Elevated Ground");
            tm.ClearAllTiles();

            // The elevated island covers the play area MINUS the path channel
            // Island bounds: approximately (-8, -5) to (8, 5)
            // Path cuts through (same shape as flat ground)

            bool[,] isPath = new bool[30, 20];
            bool[,] isIsland = new bool[30, 20];
            int ox = 13, oy = 9;

            void MarkGrid(bool[,] grid, int xMin, int yMin, int xMax, int yMax)
            {
                for (int x = xMin; x <= xMax; x++)
                    for (int y = yMin; y <= yMax; y++)
                        if (x + ox >= 0 && x + ox < 30 && y + oy >= 0 && y + oy < 20)
                            grid[x + ox, y + oy] = true;
            }

            // Mark path (same as flat ground)
            MarkGrid(isPath, -8, -4, -3, -2);
            MarkGrid(isPath, -4, -3, -2, 0);
            MarkGrid(isPath, -3, -1, 3, 1);
            MarkGrid(isPath, 2, 0, 4, 3);
            MarkGrid(isPath, 3, 2, 8, 4);

            // Mark island (full rectangle)
            MarkGrid(isIsland, -8, -5, 8, 5);

            // Elevated = island AND NOT path
            bool[,] isElevated = new bool[30, 20];
            for (int gx = 0; gx < 30; gx++)
                for (int gy = 0; gy < 20; gy++)
                    isElevated[gx, gy] = isIsland[gx, gy] && !isPath[gx, gy];

            // Paint tiles
            for (int x = -13; x <= 13; x++)
            {
                for (int y = -9; y <= 9; y++)
                {
                    int gx = x + ox, gy = y + oy;
                    if (gx < 0 || gx >= 30 || gy < 0 || gy >= 20) continue;
                    if (!isElevated[gx, gy]) continue;

                    bool up = gy + 1 < 20 && isElevated[gx, gy + 1];
                    bool down = gy - 1 >= 0 && isElevated[gx, gy - 1];
                    bool left = gx - 1 >= 0 && isElevated[gx - 1, gy];
                    bool right = gx + 1 < 30 && isElevated[gx + 1, gy];

                    // What's below this tile? Water or flat ground?
                    bool downIsPath = gy - 1 >= 0 && isPath[gx, gy - 1];
                    bool downIsWater = !down && !downIsPath;

                    // Determine tile type based on neighbors
                    int tileIdx = E_C;

                    // Top grass surface detection
                    if (!left && !down && right && up) tileIdx = E_BL;   // bottom-left of grass (cliff edge)
                    else if (left && !down && right && up) tileIdx = E_B; // bottom edge (cliff)
                    else if (left && !down && !right && up) tileIdx = E_BR; // bottom-right (cliff)
                    else if (!left && down && up && right) tileIdx = E_L;
                    else if (left && down && up && !right) tileIdx = E_R;
                    else if (!left && !down && !right && up) tileIdx = E_B; // narrow bottom
                    else if (!left && up && right && !down) tileIdx = E_BL;
                    else if (!left && !up && right && down) tileIdx = E_TL;
                    else if (left && !up && right && down) tileIdx = E_T;
                    else if (left && !up && !right && down) tileIdx = E_TR;
                    else if (!left && !up && right && !down) tileIdx = E_TL; // isolated corner
                    else if (left && !up && !right && !down) tileIdx = E_TR;
                    else if (!left && up && !right && down) tileIdx = E_L; // narrow vertical
                    else if (!left && up && !right && !down) tileIdx = E_BL; // narrow bottom-left
                    else if (left && up && right && down)
                    {
                        // Check inner corners
                        bool upLeft = (gx - 1 >= 0 && gy + 1 < 20) && isElevated[gx - 1, gy + 1];
                        bool upRight = (gx + 1 < 30 && gy + 1 < 20) && isElevated[gx + 1, gy + 1];
                        bool downLeft = (gx - 1 >= 0 && gy - 1 >= 0) && isElevated[gx - 1, gy - 1];
                        bool downRight = (gx + 1 < 30 && gy - 1 >= 0) && isElevated[gx + 1, gy - 1];

                        if (!downRight) tileIdx = E_ITR; // using inner corner tiles
                        else if (!downLeft) tileIdx = E_IR;
                        else if (!upRight) tileIdx = E_IB;
                        else if (!upLeft) tileIdx = E_ITR;
                        else tileIdx = E_C;
                    }

                    SetTile(tm, LoadTile("color2", tileIdx), x, y);
                }
            }

            // Now paint the cliff face row: one row BELOW any bottom edge of elevated ground
            // The cliff face sits below the grass, showing the rock cliff
            for (int x = -13; x <= 13; x++)
            {
                for (int y = -9; y <= 9; y++)
                {
                    int gx = x + ox, gy = y + oy;
                    if (gx < 0 || gx >= 30 || gy < 0 || gy >= 20) continue;
                    if (isElevated[gx, gy]) continue; // skip cells that are already elevated

                    // Check if the cell directly above is elevated (this cell is cliff face)
                    bool aboveIsElevated = gy + 1 < 20 && isElevated[gx, gy + 1];
                    if (!aboveIsElevated) continue;

                    // Determine cliff face tile
                    bool leftAboveElevated = gx - 1 >= 0 && gy + 1 < 20 && isElevated[gx - 1, gy + 1];
                    bool rightAboveElevated = gx + 1 < 30 && gy + 1 < 20 && isElevated[gx + 1, gy + 1];
                    bool leftIsCliff = gx - 1 >= 0 && !isElevated[gx - 1, gy] && leftAboveElevated;
                    bool rightIsCliff = gx + 1 < 30 && !isElevated[gx + 1, gy] && rightAboveElevated;

                    // Is this cliff facing water or flat ground?
                    bool onPath = isPath[gx, gy];
                    int cliffTile;

                    if (onPath)
                    {
                        // Cliff facing flat ground (walkable)
                        bool leftElevated = gx - 1 >= 0 && isElevated[gx - 1, gy];
                        bool rightElevated = gx + 1 < 30 && isElevated[gx + 1, gy];

                        if (!leftAboveElevated && rightAboveElevated) cliffTile = E_FBL;
                        else if (leftAboveElevated && !rightAboveElevated) cliffTile = E_FBR;
                        else cliffTile = E_FB;
                    }
                    else
                    {
                        // Cliff facing water
                        if (!leftAboveElevated) cliffTile = E_WBL;
                        else if (!rightAboveElevated) cliffTile = E_WBR;
                        else cliffTile = E_WB;
                    }

                    SetTile(tm, LoadTile("color2", cliffTile), x, y);
                }
            }

            tm.RefreshAllTiles();
            EditorUtility.SetDirty(tm);
            Debug.Log("TilemapPainter: Painted ElevatedGround");
        }

        [MenuItem("Tools/Tilemap/Paint Shadow")]
        public static void PaintShadow()
        {
            var tm = GetTilemap("Shadow");
            if (tm == null) return;
            var shadowTile = LoadSpecial("Shadow");
            if (shadowTile == null) return;
            Undo.RecordObject(tm, "Paint Shadow");
            tm.ClearAllTiles();

            // Shadow goes below every cliff edge of elevated ground
            // The shadow is 128x128 on a 64 grid, so it overlaps by design
            // Place shadow tiles 1 row below every bottom edge of elevated ground

            bool[,] isPath = new bool[30, 20];
            bool[,] isIsland = new bool[30, 20];
            bool[,] isElevated = new bool[30, 20];
            int ox = 13, oy = 9;

            void MarkGrid(bool[,] grid, int xMin, int yMin, int xMax, int yMax)
            {
                for (int x = xMin; x <= xMax; x++)
                    for (int y = yMin; y <= yMax; y++)
                        if (x + ox >= 0 && x + ox < 30 && y + oy >= 0 && y + oy < 20)
                            grid[x + ox, y + oy] = true;
            }

            MarkGrid(isPath, -8, -4, -3, -2);
            MarkGrid(isPath, -4, -3, -2, 0);
            MarkGrid(isPath, -3, -1, 3, 1);
            MarkGrid(isPath, 2, 0, 4, 3);
            MarkGrid(isPath, 3, 2, 8, 4);
            MarkGrid(isIsland, -8, -5, 8, 5);

            for (int gx = 0; gx < 30; gx++)
                for (int gy = 0; gy < 20; gy++)
                    isElevated[gx, gy] = isIsland[gx, gy] && !isPath[gx, gy];

            // Place shadows below cliff edges (1 tile below bottom of elevated)
            for (int x = -13; x <= 13; x++)
            {
                for (int y = -9; y <= 9; y++)
                {
                    int gx = x + ox, gy = y + oy;
                    if (gx < 0 || gx >= 30 || gy < 0 || gy >= 20) continue;
                    if (isElevated[gx, gy]) continue;

                    // Check if the cell above is elevated (cliff edge)
                    bool aboveIsElevated = gy + 1 < 20 && isElevated[gx, gy + 1];
                    if (aboveIsElevated)
                    {
                        // Place shadow here (below the cliff)
                        SetTile(tm, shadowTile, x, y);
                    }
                }
            }

            tm.RefreshAllTiles();
            EditorUtility.SetDirty(tm);
            Debug.Log("TilemapPainter: Painted Shadow");
        }

        [MenuItem("Tools/Tilemap/Paint Water Foam")]
        public static void PaintWaterFoam()
        {
            var tm = GetTilemap("WaterFoam");
            if (tm == null) return;
            var foamTile = LoadSpecial("Water Tile animated");
            if (foamTile == null) return;
            Undo.RecordObject(tm, "Paint Water Foam");
            tm.ClearAllTiles();

            // Water foam goes at all land-water boundaries
            // Place foam on any water cell that is adjacent to land (elevated or flat ground)

            bool[,] isPath = new bool[30, 20];
            bool[,] isIsland = new bool[30, 20];
            bool[,] isLand = new bool[30, 20];
            int ox = 13, oy = 9;

            void MarkGrid(bool[,] grid, int xMin, int yMin, int xMax, int yMax)
            {
                for (int x = xMin; x <= xMax; x++)
                    for (int y = yMin; y <= yMax; y++)
                        if (x + ox >= 0 && x + ox < 30 && y + oy >= 0 && y + oy < 20)
                            grid[x + ox, y + oy] = true;
            }

            MarkGrid(isPath, -8, -4, -3, -2);
            MarkGrid(isPath, -4, -3, -2, 0);
            MarkGrid(isPath, -3, -1, 3, 1);
            MarkGrid(isPath, 2, 0, 4, 3);
            MarkGrid(isPath, 3, 2, 8, 4);
            MarkGrid(isIsland, -8, -5, 8, 5);

            // Land = island (elevated + path)
            for (int gx = 0; gx < 30; gx++)
                for (int gy = 0; gy < 20; gy++)
                    isLand[gx, gy] = isIsland[gx, gy];

            // Also include the path extending beyond the island
            for (int gx = 0; gx < 30; gx++)
                for (int gy = 0; gy < 20; gy++)
                    if (isPath[gx, gy]) isLand[gx, gy] = true;

            // Place foam at water cells adjacent to land
            for (int x = -13; x <= 13; x++)
            {
                for (int y = -9; y <= 9; y++)
                {
                    int gx = x + ox, gy = y + oy;
                    if (gx < 0 || gx >= 30 || gy < 0 || gy >= 20) continue;
                    if (isLand[gx, gy]) continue; // skip land cells

                    // Check if any neighbor is land
                    bool adjLand = false;
                    for (int dx = -1; dx <= 1 && !adjLand; dx++)
                        for (int dy = -1; dy <= 1 && !adjLand; dy++)
                        {
                            if (dx == 0 && dy == 0) continue;
                            int nx = gx + dx, ny = gy + dy;
                            if (nx >= 0 && nx < 30 && ny >= 0 && ny < 20 && isLand[nx, ny])
                                adjLand = true;
                        }

                    if (adjLand)
                        SetTile(tm, foamTile, x, y);
                }
            }

            tm.RefreshAllTiles();
            EditorUtility.SetDirty(tm);
            Debug.Log("TilemapPainter: Painted WaterFoam");
        }

        [MenuItem("Tools/Tilemap/Paint All Layers")]
        public static void PaintAllLayers()
        {
            PaintBGWater();
            PaintFlatGround();
            PaintElevatedGround();
            PaintShadow();
            PaintWaterFoam();

            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log("TilemapPainter: All layers painted!");
        }
    }
}
