using NUnit.Framework;
using TowerDefense.Game.Data.Definitions;
using TowerDefense.Game.Data.Variables;
using TowerDefense.Game.UI;
using UnityEditor;
using UnityEngine;

namespace TowerDefense.Game.Tests.EditMode.UI
{
    public class PlacementTests
    {
        [Test]
        public void RangeIndicatorLogic_ComputesScale_FromAttackRange()
        {
            var logic = new RangeIndicatorLogic();

            Vector3 scale = logic.ComputeScale(5f);

            Assert.That(scale.x, Is.EqualTo(10f).Within(0.001f));
            Assert.That(scale.y, Is.EqualTo(10f).Within(0.001f));
        }

        [Test]
        public void RangeIndicatorLogic_ZeroRange_ReturnsZeroScale()
        {
            var logic = new RangeIndicatorLogic();

            Vector3 scale = logic.ComputeScale(0f);

            Assert.That(scale.x, Is.EqualTo(0f));
        }

        [Test]
        public void PlacementIndicatorLogic_ShouldBeActive_WhenHasSelection()
        {
            var selectionState = ScriptableObject.CreateInstance<SelectionState>();
            var towerDef = ScriptableObject.CreateInstance<TowerDefinition>();
            var prefab = new GameObject("Prefab");

            selectionState.Select(towerDef, prefab);

            var logic = new PlacementIndicatorLogic();
            Assert.That(logic.ShouldBeActive(selectionState), Is.True);

            Object.DestroyImmediate(prefab);
            Object.DestroyImmediate(towerDef);
            Object.DestroyImmediate(selectionState);
        }

        [Test]
        public void PlacementIndicatorLogic_ShouldNotBeActive_WhenNoSelection()
        {
            var selectionState = ScriptableObject.CreateInstance<SelectionState>();

            var logic = new PlacementIndicatorLogic();
            Assert.That(logic.ShouldBeActive(selectionState), Is.False);

            Object.DestroyImmediate(selectionState);
        }

        [Test]
        public void BuildNodeHighlighterLogic_AvailableWhenNotOccupied()
        {
            var logic = new BuildNodeHighlighterLogic();

            Assert.That(logic.IsAvailable(false), Is.True);
            Assert.That(logic.IsAvailable(true), Is.False);
        }
    }
}
