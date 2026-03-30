using NUnit.Framework;
using TowerDefense.Game.Data.Definitions;
using TowerDefense.Game.UI;
using UnityEngine;

namespace TowerDefense.Game.Tests.EditMode.UI
{
    public class TowerInfoPanelTests
    {
        [Test]
        public void TowerInfoPanelLogic_FormatsStats_FromDefinition()
        {
            var definition = ScriptableObject.CreateInstance<TowerDefinition>();
            var so = new UnityEditor.SerializedObject(definition);
            so.FindProperty("damagePerShot").intValue = 25;
            so.FindProperty("attackRange").floatValue = 3.5f;
            so.FindProperty("attackIntervalSeconds").floatValue = 0.8f;
            so.ApplyModifiedPropertiesWithoutUndo();

            var logic = new TowerInfoPanelLogic();
            var stats = logic.GetStats(definition);

            Assert.That(stats.damage, Is.EqualTo(25));
            Assert.That(stats.range, Is.EqualTo(3.5f).Within(0.001f));
            Assert.That(stats.fireRate, Is.EqualTo(0.8f).Within(0.001f));

            Object.DestroyImmediate(definition);
        }

        [Test]
        public void TowerInfoPanelLogic_UpgradeAvailable_ReturnsFalse()
        {
            var logic = new TowerInfoPanelLogic();

            Assert.That(logic.IsUpgradeAvailable(), Is.False);
        }

        [Test]
        public void TowerInfoPanelLogic_SellCost_IsHalfOfGoldCost()
        {
            var definition = ScriptableObject.CreateInstance<TowerDefinition>();
            var so = new UnityEditor.SerializedObject(definition);
            so.FindProperty("goldCost").intValue = 100;
            so.ApplyModifiedPropertiesWithoutUndo();

            var logic = new TowerInfoPanelLogic();
            int sellCost = logic.ComputeSellCost(definition);

            Assert.That(sellCost, Is.EqualTo(50));

            Object.DestroyImmediate(definition);
        }

        [Test]
        public void TowerInfoPanelLogic_SellCost_RoundsDown_ForOddCost()
        {
            var definition = ScriptableObject.CreateInstance<TowerDefinition>();
            var so = new UnityEditor.SerializedObject(definition);
            so.FindProperty("goldCost").intValue = 75;
            so.ApplyModifiedPropertiesWithoutUndo();

            var logic = new TowerInfoPanelLogic();
            int sellCost = logic.ComputeSellCost(definition);

            Assert.That(sellCost, Is.EqualTo(37));

            Object.DestroyImmediate(definition);
        }

        [Test]
        public void TowerInfoPanelLogic_IsSellAvailable_ReturnsFalse()
        {
            var logic = new TowerInfoPanelLogic();

            Assert.That(logic.IsSellAvailable(), Is.False);
        }
    }
}