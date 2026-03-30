using System;
using NUnit.Framework;
using TowerDefense.Game.Data;
using TowerDefense.Game.Data.Definitions;
using TowerDefense.Game.Data.Variables;
using UnityEngine;

namespace TowerDefense.Game.Tests.EditMode.Data
{
    public class SelectionStateTests
    {
        private SelectionState selectionState;
        private TowerDefinition towerDef;

        [SetUp]
        public void SetUp()
        {
            selectionState = ScriptableObject.CreateInstance<SelectionState>();
            towerDef = ScriptableObject.CreateInstance<TowerDefinition>();
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(selectionState);
            UnityEngine.Object.DestroyImmediate(towerDef);
        }

        [Test]
        public void Select_SetsSelectedTower()
        {
            var prefab = new GameObject("TowerPrefab");

            selectionState.Select(towerDef, prefab);

            Assert.That(selectionState.SelectedTower, Is.SameAs(towerDef));
            Assert.That(selectionState.TowerPrefab, Is.SameAs(prefab));

            UnityEngine.Object.DestroyImmediate(prefab);
        }

        [Test]
        public void Clear_NullsSelection()
        {
            var prefab = new GameObject("TowerPrefab");
            selectionState.Select(towerDef, prefab);

            selectionState.Clear();

            Assert.That(selectionState.SelectedTower, Is.Null);
            Assert.That(selectionState.TowerPrefab, Is.Null);

            UnityEngine.Object.DestroyImmediate(prefab);
        }

        [Test]
        public void Select_FiresSelectionChanged()
        {
            var prefab = new GameObject("TowerPrefab");
            int callCount = 0;
            selectionState.SelectionChanged += () => callCount++;

            selectionState.Select(towerDef, prefab);

            Assert.That(callCount, Is.EqualTo(1));

            UnityEngine.Object.DestroyImmediate(prefab);
        }

        [Test]
        public void Clear_FiresSelectionChanged()
        {
            var prefab = new GameObject("TowerPrefab");
            selectionState.Select(towerDef, prefab);

            int callCount = 0;
            selectionState.SelectionChanged += () => callCount++;

            selectionState.Clear();

            Assert.That(callCount, Is.EqualTo(1));

            UnityEngine.Object.DestroyImmediate(prefab);
        }

        [Test]
        public void HasSelection_TrueWhenSelected_FalseWhenCleared()
        {
            Assert.That(selectionState.HasSelection, Is.False);

            var prefab = new GameObject("TowerPrefab");
            selectionState.Select(towerDef, prefab);

            Assert.That(selectionState.HasSelection, Is.True);

            selectionState.Clear();

            Assert.That(selectionState.HasSelection, Is.False);

            UnityEngine.Object.DestroyImmediate(prefab);
        }

        [Test]
        public void TowerBuildOption_HoldsDefinitionAndPrefab()
        {
            var prefab = new GameObject("TowerPrefab");

            var option = new TowerBuildOption
            {
                definition = towerDef,
                towerPrefab = prefab
            };

            Assert.That(option.definition, Is.SameAs(towerDef));
            Assert.That(option.towerPrefab, Is.SameAs(prefab));

            UnityEngine.Object.DestroyImmediate(prefab);
        }
    }
}
