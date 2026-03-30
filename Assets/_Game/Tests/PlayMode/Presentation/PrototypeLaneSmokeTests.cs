using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering;
using TowerDefense.Game.Presentation.Sorting;

namespace TowerDefense.Game.Tests.PlayMode.Presentation
{
    public class PrototypeLaneSmokeTests
    {
        [Test]
        public void YSortAdapter_SetsSortingOrderFromYPosition()
        {
            var go = new GameObject("TestYSort");
            var sr = go.AddComponent<SpriteRenderer>();
            var adapter = go.AddComponent<YSortAdapter>();

            go.transform.position = new Vector3(0f, 3f, 0f);
            adapter.SendMessage("Awake");
            adapter.SendMessage("LateUpdate");

            Assert.That(sr.sortingOrder, Is.EqualTo(Mathf.RoundToInt(-3f * 100f)));

            Object.DestroyImmediate(go);
        }

        [Test]
        public void SortingGroupBinder_AppliesSortingLayerOnAwake()
        {
            var go = new GameObject("TestSortingBinder");
            var sg = go.AddComponent<SortingGroup>();
            var binder = go.AddComponent<SortingGroupBinder>();

            binder.SendMessage("Awake");

            Assert.That(sg.sortingLayerName, Is.EqualTo("Default"));

            Object.DestroyImmediate(go);
        }
    }
}
