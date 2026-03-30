using NUnit.Framework;
using TowerDefense.Game.Data.Definitions;
using TowerDefense.Game.Data.Events;
using TowerDefense.Game.Data.Variables;
using UnityEditor;
using UnityEngine;

namespace TowerDefense.Game.Tests.EditMode.Data
{
public class DefinitionAssetTests
{
    [Test]
    public void EnemyDefinition_PreservesConfiguredValues()
    {
        var definition = ScriptableObject.CreateInstance<EnemyDefinition>();

        SetSerializedValue(definition, "moveSpeed", 2f);
        SetSerializedValue(definition, "maxHealth", 10);
        SetSerializedValue(definition, "goldReward", 3);

        Assert.That(definition.MoveSpeed, Is.EqualTo(2f));
        Assert.That(definition.MaxHealth, Is.EqualTo(10));
        Assert.That(definition.GoldReward, Is.EqualTo(3));

        Object.DestroyImmediate(definition);
    }

    [Test]
    public void TowerDefinition_PreservesConfiguredValues()
    {
        var definition = ScriptableObject.CreateInstance<TowerDefinition>();

        SetSerializedValue(definition, "goldCost", 8);
        SetSerializedValue(definition, "attackRange", 4.5f);
        SetSerializedValue(definition, "damagePerShot", 2);
        SetSerializedValue(definition, "attackIntervalSeconds", 0.75f);

        Assert.That(definition.GoldCost, Is.EqualTo(8));
        Assert.That(definition.AttackRange, Is.EqualTo(4.5f));
        Assert.That(definition.DamagePerShot, Is.EqualTo(2));
        Assert.That(definition.AttackIntervalSeconds, Is.EqualTo(0.75f));

        Object.DestroyImmediate(definition);
    }

    [Test]
    public void WaveDefinition_PreservesConfiguredValues()
    {
        var enemyDefinition = ScriptableObject.CreateInstance<EnemyDefinition>();
        var waveDefinition = ScriptableObject.CreateInstance<WaveDefinition>();

        SetSerializedObjectReference(waveDefinition, "enemy", enemyDefinition);
        SetSerializedValue(waveDefinition, "enemyCount", 5);
        SetSerializedValue(waveDefinition, "spawnIntervalSeconds", 1.25f);

        Assert.That(waveDefinition.Enemy, Is.SameAs(enemyDefinition));
        Assert.That(waveDefinition.EnemyCount, Is.EqualTo(5));
        Assert.That(waveDefinition.SpawnIntervalSeconds, Is.EqualTo(1.25f));

        Object.DestroyImmediate(waveDefinition);
        Object.DestroyImmediate(enemyDefinition);
    }

    [Test]
    public void IntVariable_SetAndApplyChange_UpdateValue()
    {
        var variable = ScriptableObject.CreateInstance<IntVariable>();

        variable.SetValue(4);
        variable.ApplyChange(3);

        Assert.That(variable.Value, Is.EqualTo(7));

        Object.DestroyImmediate(variable);
    }

    [Test]
    public void FloatVariable_SetAndApplyChange_UpdateValue()
    {
        var variable = ScriptableObject.CreateInstance<FloatVariable>();

        variable.SetValue(1.5f);
        variable.ApplyChange(0.25f);

        Assert.That(variable.Value, Is.EqualTo(1.75f));

        Object.DestroyImmediate(variable);
    }

    [Test]
    public void VoidEventChannel_Raise_InvokesRegisteredListener()
    {
        var channel = ScriptableObject.CreateInstance<VoidEventChannel>();
        int callCount = 0;

        channel.RegisterListener(OnRaised);
        channel.RaiseEvent();

        Assert.That(callCount, Is.EqualTo(1));
        channel.UnregisterListener(OnRaised);
        Object.DestroyImmediate(channel);

        void OnRaised()
        {
            callCount++;
        }
    }

    [Test]
    public void IntEventChannel_Raise_InvokesRegisteredListenerWithPayload()
    {
        var channel = ScriptableObject.CreateInstance<IntEventChannel>();
        int receivedValue = 0;

        channel.RegisterListener(OnRaised);
        channel.RaiseEvent(9);

        Assert.That(receivedValue, Is.EqualTo(9));
        channel.UnregisterListener(OnRaised);
        Object.DestroyImmediate(channel);

        void OnRaised(int value)
        {
            receivedValue = value;
        }
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

    private static void SetSerializedObjectReference<TObject>(TObject target, string propertyName, Object value)
        where TObject : Object
    {
        var serializedObject = new SerializedObject(target);
        SerializedProperty property = serializedObject.FindProperty(propertyName);

        Assert.That(property, Is.Not.Null);
        property.objectReferenceValue = value;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }
}
}
