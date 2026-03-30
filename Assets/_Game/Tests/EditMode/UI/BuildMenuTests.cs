using NUnit.Framework;
using TowerDefense.Game.Data;
using TowerDefense.Game.Data.Definitions;
using TowerDefense.Game.Data.Variables;
using TowerDefense.Game.UI;
using UnityEditor;
using UnityEngine;

namespace TowerDefense.Game.Tests.EditMode.UI
{
    public class BuildMenuTests
    {
        private IntVariable goldVariable;
        private SelectionState selectionState;
        private TowerDefinition cheapTower;
        private TowerDefinition expensiveTower;

        [SetUp]
        public void SetUp()
        {
            goldVariable = ScriptableObject.CreateInstance<IntVariable>();
            selectionState = ScriptableObject.CreateInstance<SelectionState>();
            cheapTower = ScriptableObject.CreateInstance<TowerDefinition>();
            expensiveTower = ScriptableObject.CreateInstance<TowerDefinition>();

            SetSerializedValue(cheapTower, "goldCost", 50);
            SetSerializedValue(expensiveTower, "goldCost", 200);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(goldVariable);
            Object.DestroyImmediate(selectionState);
            Object.DestroyImmediate(cheapTower);
            Object.DestroyImmediate(expensiveTower);
        }

        [Test]
        public void TowerBuildButtonLogic_IsAffordable_WhenGoldSufficient()
        {
            var logic = new TowerBuildButtonLogic(cheapTower);

            Assert.That(logic.IsAffordable(100), Is.True);
        }

        [Test]
        public void TowerBuildButtonLogic_IsNotAffordable_WhenGoldInsufficient()
        {
            var logic = new TowerBuildButtonLogic(expensiveTower);

            Assert.That(logic.IsAffordable(100), Is.False);
        }

        [Test]
        public void TowerBuildButtonLogic_IsAffordable_WhenGoldExact()
        {
            var logic = new TowerBuildButtonLogic(cheapTower);

            Assert.That(logic.IsAffordable(50), Is.True);
        }

        [Test]
        public void TowerBuildButtonLogic_CostText_ReturnsFormattedCost()
        {
            var logic = new TowerBuildButtonLogic(cheapTower);

            Assert.That(logic.CostText, Is.EqualTo("50"));
        }

        [Test]
        public void TowerBuildButtonLogic_Select_SetsSelectionState()
        {
            var prefab = new GameObject("Prefab");
            var logic = new TowerBuildButtonLogic(cheapTower);

            logic.Select(selectionState, prefab);

            Assert.That(selectionState.SelectedTower, Is.SameAs(cheapTower));
            Assert.That(selectionState.TowerPrefab, Is.SameAs(prefab));

            Object.DestroyImmediate(prefab);
        }

        [Test]
        public void TowerBuildButtonLogic_Select_TogglesOff_WhenAlreadySelected()
        {
            var prefab = new GameObject("Prefab");
            var logic = new TowerBuildButtonLogic(cheapTower);

            logic.Select(selectionState, prefab);
            logic.Select(selectionState, prefab);

            Assert.That(selectionState.HasSelection, Is.False);

            Object.DestroyImmediate(prefab);
        }

        [Test]
        public void BuildMenuControllerLogic_ReturnsCorrectOptionCount()
        {
            var prefab = new GameObject("Prefab");
            var options = new[]
            {
                new TowerBuildOption { definition = cheapTower, towerPrefab = prefab },
                new TowerBuildOption { definition = expensiveTower, towerPrefab = prefab }
            };

            var logic = new BuildMenuControllerLogic(options);

            Assert.That(logic.OptionCount, Is.EqualTo(2));

            Object.DestroyImmediate(prefab);
        }

        [Test]
        public void BuildMenuControllerLogic_GetOption_ReturnsCorrectOption()
        {
            var prefab = new GameObject("Prefab");
            var options = new[]
            {
                new TowerBuildOption { definition = cheapTower, towerPrefab = prefab },
                new TowerBuildOption { definition = expensiveTower, towerPrefab = prefab }
            };

            var logic = new BuildMenuControllerLogic(options);

            Assert.That(logic.GetOption(0).definition, Is.SameAs(cheapTower));
            Assert.That(logic.GetOption(1).definition, Is.SameAs(expensiveTower));

            Object.DestroyImmediate(prefab);
        }

        private static void SetSerializedValue<TObject, TValue>(TObject target, string propertyName, TValue value)
            where TObject : Object
        {
            var serializedObject = new SerializedObject(target);
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            Assert.That(property, Is.Not.Null);

            switch (value)
            {
                case int intValue:
                    property.intValue = intValue;
                    break;
                case float floatValue:
                    property.floatValue = floatValue;
                    break;
                default:
                    Assert.Fail($"Unsupported value type {typeof(TValue).Name}.");
                    break;
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
