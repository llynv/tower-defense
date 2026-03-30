using NUnit.Framework;
using TMPro;
using TowerDefense.Game.Data.Variables;
using TowerDefense.Game.UI;
using UnityEditor;
using UnityEngine;

namespace TowerDefense.Game.Tests.EditMode.UI
{
    public class VariableDisplayTests
    {
        private IntVariable variable;

        [SetUp]
        public void SetUp()
        {
            variable = ScriptableObject.CreateInstance<IntVariable>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(variable);
        }

        [Test]
        public void GoldDisplay_UpdatesText_WhenGoldChanges()
        {
            var go = CreateDisplayObject<GoldDisplay>("goldVariable");

            var display = go.GetComponent<GoldDisplay>();
            var text = go.GetComponent<TMP_Text>();

            SimulateOnEnable(display);

            variable.SetValue(150);

            Assert.That(text.text, Is.EqualTo("150"));

            SimulateOnDisable(display);
            Object.DestroyImmediate(go);
        }

        [Test]
        public void LivesDisplay_UpdatesText_WhenLivesChange()
        {
            var go = CreateDisplayObject<LivesDisplay>("livesVariable");

            var display = go.GetComponent<LivesDisplay>();
            var text = go.GetComponent<TMP_Text>();

            SimulateOnEnable(display);

            variable.SetValue(5);

            Assert.That(text.text, Is.EqualTo("5"));

            SimulateOnDisable(display);
            Object.DestroyImmediate(go);
        }

        [Test]
        public void GoldDisplay_ShowsInitialValue_OnEnable()
        {
            variable.SetValue(42);

            var go = CreateDisplayObject<GoldDisplay>("goldVariable");
            var display = go.GetComponent<GoldDisplay>();
            var text = go.GetComponent<TMP_Text>();

            SimulateOnEnable(display);

            Assert.That(text.text, Is.EqualTo("42"));

            SimulateOnDisable(display);
            Object.DestroyImmediate(go);
        }

        [Test]
        public void GoldDisplay_StopsUpdating_AfterOnDisable()
        {
            var go = CreateDisplayObject<GoldDisplay>("goldVariable");
            var display = go.GetComponent<GoldDisplay>();
            var text = go.GetComponent<TMP_Text>();

            SimulateOnEnable(display);
            variable.SetValue(10);
            SimulateOnDisable(display);

            variable.SetValue(999);

            Assert.That(text.text, Is.EqualTo("10"));

            Object.DestroyImmediate(go);
        }

        private GameObject CreateDisplayObject<T>(string variableFieldName) where T : MonoBehaviour
        {
            var go = new GameObject("TestDisplay");
            var text = go.AddComponent<TextMeshPro>();
            var display = go.AddComponent<T>();

            var so = new SerializedObject(display);
            so.FindProperty("textComponent").objectReferenceValue = text;
            so.FindProperty(variableFieldName).objectReferenceValue = variable;
            so.ApplyModifiedPropertiesWithoutUndo();

            return go;
        }

        private static void SimulateOnEnable(MonoBehaviour component)
        {
            var method = component.GetType().GetMethod("OnEnable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method?.Invoke(component, null);
        }

        private static void SimulateOnDisable(MonoBehaviour component)
        {
            var method = component.GetType().GetMethod("OnDisable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method?.Invoke(component, null);
        }
    }
}
