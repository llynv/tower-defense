using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TowerDefense.Editor
{
    public static class DecorationPlacer
    {
        const string BushPath = "Assets/Tiny Swords/Terrain/Decorations/Bushes/";
        const string RockPath = "Assets/Tiny Swords/Terrain/Decorations/Rocks/";
        const string WaterRockPath = "Assets/Tiny Swords/Terrain/Decorations/Rocks in the Water/";
        const string CloudPath = "Assets/Tiny Swords/Terrain/Decorations/Clouds/";
        const string DuckPath = "Assets/Tiny Swords/Terrain/Decorations/Rubber Duck/";

        static GameObject GetDecorationsParent()
        {
            var go = GameObject.Find("Decorations");
            if (go == null)
                Debug.LogError("DecorationPlacer: Cannot find 'Decorations' GameObject in scene");
            return go;
        }

        static Sprite LoadSingleSprite(string path)
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite == null)
                Debug.LogError($"DecorationPlacer: Cannot load sprite at {path}");
            return sprite;
        }

        static Sprite LoadFirstSubSprite(string texturePath)
        {
            var allAssets = AssetDatabase.LoadAllAssetsAtPath(texturePath);
            foreach (var asset in allAssets)
            {
                if (asset is Sprite s)
                    return s;
            }
            Debug.LogError($"DecorationPlacer: No sub-sprites found in {texturePath}");
            return null;
        }

        static RuntimeAnimatorController LoadController(string path)
        {
            var controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(path);
            if (controller == null)
                Debug.LogError($"DecorationPlacer: Cannot load controller at {path}");
            return controller;
        }

        static GameObject CreateDecoration(string name, Transform parent, Vector3 position,
            Sprite sprite, string sortingLayer, int sortingOrder,
            RuntimeAnimatorController controller = null)
        {
            var go = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(go, $"Create {name}");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = position;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingLayerName = sortingLayer;
            sr.sortingOrder = sortingOrder;

            if (controller != null)
            {
                var animator = go.AddComponent<Animator>();
                animator.runtimeAnimatorController = controller;
            }

            return go;
        }

        [MenuItem("Tools/Tilemap/Place All Decorations")]
        static void PlaceAllDecorations()
        {
            var parent = GetDecorationsParent();
            if (parent == null) return;

            PlaceBushes(parent.transform);
            PlaceRocks(parent.transform);
            PlaceWaterRocks(parent.transform);
            PlaceClouds(parent.transform);
            PlaceRubberDuck(parent.transform);

            EditorSceneManager.MarkSceneDirty(parent.scene);
            Debug.Log("DecorationPlacer: All decorations placed successfully.");
        }

        [MenuItem("Tools/Tilemap/Clear Decorations")]
        static void ClearDecorations()
        {
            var parent = GetDecorationsParent();
            if (parent == null) return;

            while (parent.transform.childCount > 0)
            {
                Undo.DestroyObjectImmediate(parent.transform.GetChild(0).gameObject);
            }

            EditorSceneManager.MarkSceneDirty(parent.scene);
            Debug.Log("DecorationPlacer: All decorations cleared.");
        }

        static void PlaceBushes(Transform parent)
        {
            // Island: (-8,-5) to (8,5). Path through center. Build nodes at:
            // (-5,0), (-5,2), (-1,-2), (-1,2), (1,-2), (1,2), (5,0), (5,-2)
            // Place bushes on elevated ground, away from build nodes and path.
            // Bushes are 2x2 world units (128px at PPU 64).

            var bushData = new[]
            {
                new { Name = "Bush_1", Png = "Bush 1.png", Controller = "Bush 1 Animation/Bush 1.controller",
                    Pos = new Vector3(-7f, 4f, 0f) },
                new { Name = "Bush_2", Png = "Bush 2.png", Controller = "Bush 2 Animation/Bush 2.controller",
                    Pos = new Vector3(7f, 4f, 0f) },
                new { Name = "Bush_3", Png = "Bush 3.png", Controller = "Bush 3 Animation/Bush 3.controller",
                    Pos = new Vector3(-7f, -4f, 0f) },
                new { Name = "Bush_4", Png = "Bush 4.png", Controller = "Bush 4 Animation/Bush 4.controller",
                    Pos = new Vector3(7f, -4f, 0f) },
                new { Name = "Bush_5", Png = "Bush 1.png", Controller = "Bush 1 Animation/Bush 1.controller",
                    Pos = new Vector3(3f, -3f, 0f) },
            };

            foreach (var b in bushData)
            {
                var sprite = LoadFirstSubSprite(BushPath + b.Png);
                var controller = LoadController(BushPath + b.Controller);
                if (sprite == null || controller == null) continue;
                CreateDecoration(b.Name, parent, b.Pos, sprite, "Props", 0, controller);
            }

            Debug.Log($"DecorationPlacer: Placed {bushData.Length} bushes.");
        }

        static void PlaceRocks(Transform parent)
        {
            // Rocks are static, single sprites. Place near cliff edges on elevated ground.
            var rockData = new[]
            {
                new { Name = "Rock_1", File = "Rock1.png", Pos = new Vector3(-6f, -3.5f, 0f) },
                new { Name = "Rock_2", File = "Rock2.png", Pos = new Vector3(6f, 3.5f, 0f) },
                new { Name = "Rock_3", File = "Rock3.png", Pos = new Vector3(-3f, 4f, 0f) },
                new { Name = "Rock_4", File = "Rock4.png", Pos = new Vector3(4f, -4f, 0f) },
            };

            foreach (var r in rockData)
            {
                var sprite = LoadSingleSprite(RockPath + r.File);
                if (sprite == null) continue;
                CreateDecoration(r.Name, parent, r.Pos, sprite, "Props", 0);
            }

            Debug.Log($"DecorationPlacer: Placed {rockData.Length} rocks.");
        }

        static void PlaceWaterRocks(Transform parent)
        {
            // Animated water rocks in water near island edges.
            // Terrain layer order 6 (above all tilemap layers but below Props/Units).
            var waterRockData = new[]
            {
                new { Name = "WaterRock_1", Png = "Water Rocks_01.png",
                    Controller = "Rock 1 Animation/Rock 1.controller",
                    Pos = new Vector3(-10f, -2f, 0f) },
                new { Name = "WaterRock_2", Png = "Water Rocks_02.png",
                    Controller = "Rock 2 Animation/Rock 2.controller",
                    Pos = new Vector3(10f, 3f, 0f) },
                new { Name = "WaterRock_3", Png = "Water Rocks_03.png",
                    Controller = "Rock 1 Animation/Rock 1.controller",
                    Pos = new Vector3(-9f, 4f, 0f) },
            };

            foreach (var wr in waterRockData)
            {
                var sprite = LoadFirstSubSprite(WaterRockPath + wr.Png);
                var controller = LoadController(WaterRockPath + wr.Controller);
                if (sprite == null || controller == null) continue;
                CreateDecoration(wr.Name, parent, wr.Pos, sprite, "Terrain", 6, controller);
            }

            Debug.Log($"DecorationPlacer: Placed {waterRockData.Length} water rocks.");
        }

        static void PlaceClouds(Transform parent)
        {
            // Static clouds, Props layer order 1 (above bushes/rocks).
            // Place scattered across scene for atmosphere, partially off-screen is fine.
            var cloudData = new[]
            {
                new { Name = "Cloud_1", File = "Clouds_01.png", Pos = new Vector3(-5f, 6f, 0f) },
                new { Name = "Cloud_2", File = "Clouds_02.png", Pos = new Vector3(6f, 7f, 0f) },
                new { Name = "Cloud_3", File = "Clouds_03.png", Pos = new Vector3(-9f, -5f, 0f) },
                new { Name = "Cloud_4", File = "Clouds_05.png", Pos = new Vector3(9f, -6f, 0f) },
            };

            foreach (var c in cloudData)
            {
                var sprite = LoadSingleSprite(CloudPath + c.File);
                if (sprite == null) continue;
                CreateDecoration(c.Name, parent, c.Pos, sprite, "Props", 1);
            }

            Debug.Log($"DecorationPlacer: Placed {cloudData.Length} clouds.");
        }

        static void PlaceRubberDuck(Transform parent)
        {
            // Animated rubber duck Easter egg in water.
            // Terrain layer order 6.
            var sprite = LoadFirstSubSprite(DuckPath + "Rubber duck.png");
            var controller = LoadController(DuckPath + "Rubber duck.controller");
            if (sprite == null || controller == null) return;

            CreateDecoration("RubberDuck", parent, new Vector3(10f, -5f, 0f),
                sprite, "Terrain", 6, controller);

            Debug.Log("DecorationPlacer: Placed rubber duck.");
        }
    }
}
