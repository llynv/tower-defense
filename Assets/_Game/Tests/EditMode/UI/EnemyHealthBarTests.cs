using System;
using NUnit.Framework;
using TowerDefense.Game.Data.Definitions;
using TowerDefense.Game.Gameplay.Enemies;
using TowerDefense.Game.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.Game.Tests.EditMode.UI
{
    public class EnemyHealthBarTests
    {
        [Test]
        public void EnemyHealthLogic_ExposesMaxHealth()
        {
            var logic = new EnemyHealthLogic(25);

            Assert.That(logic.MaxHealth, Is.EqualTo(25));
        }

        [Test]
        public void EnemyHealthLogic_MaxHealth_UnchangedAfterDamage()
        {
            var logic = new EnemyHealthLogic(25);
            logic.TakeDamage(10);

            Assert.That(logic.MaxHealth, Is.EqualTo(25));
            Assert.That(logic.CurrentHealth, Is.EqualTo(15));
        }

        [Test]
        public void EnemyHealth_FiresDamaged_WithCurrentAndMax()
        {
            var definition = ScriptableObject.CreateInstance<EnemyDefinition>();
            SetSerializedValue(definition, "maxHealth", 20);

            var go = new GameObject("Enemy");
            var health = go.AddComponent<EnemyHealth>();
            health.Initialize(definition);

            int receivedCurrent = -1;
            int receivedMax = -1;
            health.Damaged += (current, max) =>
            {
                receivedCurrent = current;
                receivedMax = max;
            };

            health.TakeDamage(5);

            Assert.That(receivedCurrent, Is.EqualTo(15));
            Assert.That(receivedMax, Is.EqualTo(20));

            UnityEngine.Object.DestroyImmediate(go);
            UnityEngine.Object.DestroyImmediate(definition);
        }

        [Test]
        public void EnemyHealth_MaxHealth_MatchesDefinition()
        {
            var definition = ScriptableObject.CreateInstance<EnemyDefinition>();
            SetSerializedValue(definition, "maxHealth", 30);

            var go = new GameObject("Enemy");
            var health = go.AddComponent<EnemyHealth>();
            health.Initialize(definition);

            Assert.That(health.MaxHealth, Is.EqualTo(30));

            UnityEngine.Object.DestroyImmediate(go);
            UnityEngine.Object.DestroyImmediate(definition);
        }

        [Test]
        public void EnemyHealthBarLogic_ScalesFill_OnDamage()
        {
            var logic = new EnemyHealthBarLogic();

            float fill = logic.ComputeFillAmount(15, 20);

            Assert.That(fill, Is.EqualTo(0.75f).Within(0.001f));
        }

        [Test]
        public void EnemyHealthBarLogic_FullHealth_ReturnOne()
        {
            var logic = new EnemyHealthBarLogic();

            float fill = logic.ComputeFillAmount(20, 20);

            Assert.That(fill, Is.EqualTo(1f));
        }

        [Test]
        public void EnemyHealthBarLogic_ZeroHealth_ReturnZero()
        {
            var logic = new EnemyHealthBarLogic();

            float fill = logic.ComputeFillAmount(0, 20);

            Assert.That(fill, Is.EqualTo(0f));
        }

        [Test]
        public void EnemyHealthBarLogic_ShouldShow_FalseAtFullHealth()
        {
            var logic = new EnemyHealthBarLogic();

            Assert.That(logic.ShouldShow(20, 20), Is.False);
        }

        [Test]
        public void EnemyHealthBarLogic_ShouldShow_TrueWhenDamaged()
        {
            var logic = new EnemyHealthBarLogic();

            Assert.That(logic.ShouldShow(15, 20), Is.True);
        }

        private static void SetSerializedValue<TObject, TValue>(TObject target, string propertyName, TValue value)
            where TObject : UnityEngine.Object
        {
            var serializedObject = new SerializedObject(target);
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            Assert.That(property, Is.Not.Null, $"Property '{propertyName}' not found.");

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
