using NUnit.Framework;
using TowerDefense.Game.Data.Definitions;
using UnityEditor;
using UnityEngine;

namespace TowerDefense.Game.Tests.EditMode.Data
{
    public class LevelDefinitionTests
    {
        [Test]
        public void WaveEntry_Constructor_SetsEnemyAndCount()
        {
            var enemyDef = ScriptableObject.CreateInstance<EnemyDefinition>();

            var entry = new WaveEntry(enemyDef, 5);

            Assert.That(entry.Enemy, Is.SameAs(enemyDef));
            Assert.That(entry.Count, Is.EqualTo(5));

            Object.DestroyImmediate(enemyDef);
        }

        [Test]
        public void LevelDefinition_WaveCount_ReturnsNumberOfWaves()
        {
            var level = ScriptableObject.CreateInstance<LevelDefinition>();
            var wave1 = ScriptableObject.CreateInstance<WaveDefinition>();
            var wave2 = ScriptableObject.CreateInstance<WaveDefinition>();

            var so = new SerializedObject(level);
            var arrProp = so.FindProperty("waves");
            arrProp.arraySize = 2;
            arrProp.GetArrayElementAtIndex(0).objectReferenceValue = wave1;
            arrProp.GetArrayElementAtIndex(1).objectReferenceValue = wave2;
            so.ApplyModifiedPropertiesWithoutUndo();

            Assert.That(level.WaveCount, Is.EqualTo(2));

            Object.DestroyImmediate(level);
            Object.DestroyImmediate(wave1);
            Object.DestroyImmediate(wave2);
        }

        [Test]
        public void LevelDefinition_GetWave_ValidIndex_ReturnsWave()
        {
            var level = ScriptableObject.CreateInstance<LevelDefinition>();
            var wave = ScriptableObject.CreateInstance<WaveDefinition>();

            var so = new SerializedObject(level);
            var arrProp = so.FindProperty("waves");
            arrProp.arraySize = 1;
            arrProp.GetArrayElementAtIndex(0).objectReferenceValue = wave;
            so.ApplyModifiedPropertiesWithoutUndo();

            Assert.That(level.GetWave(0), Is.SameAs(wave));

            Object.DestroyImmediate(level);
            Object.DestroyImmediate(wave);
        }

        [Test]
        public void LevelDefinition_GetWave_OutOfRange_ReturnsNull()
        {
            var level = ScriptableObject.CreateInstance<LevelDefinition>();
            var wave = ScriptableObject.CreateInstance<WaveDefinition>();

            var so = new SerializedObject(level);
            var arrProp = so.FindProperty("waves");
            arrProp.arraySize = 1;
            arrProp.GetArrayElementAtIndex(0).objectReferenceValue = wave;
            so.ApplyModifiedPropertiesWithoutUndo();

            Assert.That(level.GetWave(-1), Is.Null);
            Assert.That(level.GetWave(1), Is.Null);

            Object.DestroyImmediate(level);
            Object.DestroyImmediate(wave);
        }

        [Test]
        public void LevelDefinition_StartingGold_ReturnsConfiguredValue()
        {
            var level = ScriptableObject.CreateInstance<LevelDefinition>();

            var so = new SerializedObject(level);
            so.FindProperty("startingGold").intValue = 50;
            so.ApplyModifiedPropertiesWithoutUndo();

            Assert.That(level.StartingGold, Is.EqualTo(50));

            Object.DestroyImmediate(level);
        }

        [Test]
        public void LevelDefinition_StartingLives_ReturnsConfiguredValue()
        {
            var level = ScriptableObject.CreateInstance<LevelDefinition>();

            var so = new SerializedObject(level);
            so.FindProperty("startingLives").intValue = 15;
            so.ApplyModifiedPropertiesWithoutUndo();

            Assert.That(level.StartingLives, Is.EqualTo(15));

            Object.DestroyImmediate(level);
        }

        [Test]
        public void WaveDefinition_BuildSpawnList_FallbackFields_ReturnsEnemyRepeated()
        {
            var enemyDef = ScriptableObject.CreateInstance<EnemyDefinition>();
            var waveDef = ScriptableObject.CreateInstance<WaveDefinition>();

            var so = new SerializedObject(waveDef);
            so.FindProperty("enemy").objectReferenceValue = enemyDef;
            so.FindProperty("enemyCount").intValue = 3;
            so.ApplyModifiedPropertiesWithoutUndo();

            var list = waveDef.BuildSpawnList();

            Assert.That(list.Count, Is.EqualTo(3));
            Assert.That(list[0], Is.SameAs(enemyDef));
            Assert.That(list[1], Is.SameAs(enemyDef));
            Assert.That(list[2], Is.SameAs(enemyDef));

            Object.DestroyImmediate(waveDef);
            Object.DestroyImmediate(enemyDef);
        }

        [Test]
        public void WaveDefinition_BuildSpawnList_WithEntries_ReturnsEntriesInOrder()
        {
            var enemyA = ScriptableObject.CreateInstance<EnemyDefinition>();
            var enemyB = ScriptableObject.CreateInstance<EnemyDefinition>();
            var waveDef = ScriptableObject.CreateInstance<WaveDefinition>();

            var so = new SerializedObject(waveDef);
            var entriesProp = so.FindProperty("entries");
            entriesProp.arraySize = 2;
            entriesProp.GetArrayElementAtIndex(0).FindPropertyRelative("enemy").objectReferenceValue = enemyA;
            entriesProp.GetArrayElementAtIndex(0).FindPropertyRelative("count").intValue = 2;
            entriesProp.GetArrayElementAtIndex(1).FindPropertyRelative("enemy").objectReferenceValue = enemyB;
            entriesProp.GetArrayElementAtIndex(1).FindPropertyRelative("count").intValue = 1;
            so.ApplyModifiedPropertiesWithoutUndo();

            var list = waveDef.BuildSpawnList();

            Assert.That(list.Count, Is.EqualTo(3));
            Assert.That(list[0], Is.SameAs(enemyA));
            Assert.That(list[1], Is.SameAs(enemyA));
            Assert.That(list[2], Is.SameAs(enemyB));

            Object.DestroyImmediate(waveDef);
            Object.DestroyImmediate(enemyA);
            Object.DestroyImmediate(enemyB);
        }

        [Test]
        public void WaveDefinition_BuildSpawnList_EmptyEntriesNoFallback_ReturnsEmptyList()
        {
            var waveDef = ScriptableObject.CreateInstance<WaveDefinition>();

            var list = waveDef.BuildSpawnList();

            Assert.That(list, Is.Empty);

            Object.DestroyImmediate(waveDef);
        }
    }
}