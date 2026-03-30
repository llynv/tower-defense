using NUnit.Framework;
using TMPro;
using TowerDefense.Game.Core;
using TowerDefense.Game.Data.Variables;
using TowerDefense.Game.UI;
using UnityEditor;
using UnityEngine;

namespace TowerDefense.Game.Tests.EditMode.UI
{
    public class WaveCounterTests
    {
        [Test]
        public void LevelDirector_InitialWaveIndex_IsZero()
        {
            var director = new LevelDirectorLogic();

            Assert.That(director.CurrentWaveIndex, Is.EqualTo(0));
        }

        [Test]
        public void LevelDirector_SetTotalWaves_StoresTotalWaveCount()
        {
            var director = new LevelDirectorLogic();

            director.SetTotalWaves(5);

            Assert.That(director.TotalWaveCount, Is.EqualTo(5));
        }

        [Test]
        public void LevelDirector_StartWave_IncrementsWaveIndex()
        {
            var director = new LevelDirectorLogic();
            director.SetTotalWaves(3);

            director.StartWave();

            Assert.That(director.CurrentWaveIndex, Is.EqualTo(1));
        }

        [Test]
        public void LevelDirector_MultipleWaves_TracksCorrectly()
        {
            var director = new LevelDirectorLogic();
            director.SetTotalWaves(3);

            director.StartWave();
            director.CompleteWave(hasMoreWaves: true);
            director.StartWave();

            Assert.That(director.CurrentWaveIndex, Is.EqualTo(2));
        }

        [Test]
        public void WaveCounterDisplay_ShowsCorrectFormat()
        {
            var currentWave = ScriptableObject.CreateInstance<IntVariable>();
            var totalWaves = ScriptableObject.CreateInstance<IntVariable>();

            var go = new GameObject("WaveCounter");
            var text = go.AddComponent<TextMeshPro>();
            var display = go.AddComponent<WaveCounterDisplay>();

            var so = new SerializedObject(display);
            so.FindProperty("textComponent").objectReferenceValue = text;
            so.FindProperty("currentWaveVariable").objectReferenceValue = currentWave;
            so.FindProperty("totalWaveVariable").objectReferenceValue = totalWaves;
            so.ApplyModifiedPropertiesWithoutUndo();

            SimulateOnEnable(display);

            totalWaves.SetValue(10);
            currentWave.SetValue(3);

            Assert.That(text.text, Is.EqualTo("Wave 3/10"));

            SimulateOnDisable(display);
            Object.DestroyImmediate(go);
            Object.DestroyImmediate(currentWave);
            Object.DestroyImmediate(totalWaves);
        }

        [Test]
        public void WaveCounterDisplay_ShowsInitialValues_OnEnable()
        {
            var currentWave = ScriptableObject.CreateInstance<IntVariable>();
            var totalWaves = ScriptableObject.CreateInstance<IntVariable>();
            currentWave.SetValue(2);
            totalWaves.SetValue(5);

            var go = new GameObject("WaveCounter");
            var text = go.AddComponent<TextMeshPro>();
            var display = go.AddComponent<WaveCounterDisplay>();

            var so = new SerializedObject(display);
            so.FindProperty("textComponent").objectReferenceValue = text;
            so.FindProperty("currentWaveVariable").objectReferenceValue = currentWave;
            so.FindProperty("totalWaveVariable").objectReferenceValue = totalWaves;
            so.ApplyModifiedPropertiesWithoutUndo();

            SimulateOnEnable(display);

            Assert.That(text.text, Is.EqualTo("Wave 2/5"));

            SimulateOnDisable(display);
            Object.DestroyImmediate(go);
            Object.DestroyImmediate(currentWave);
            Object.DestroyImmediate(totalWaves);
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
