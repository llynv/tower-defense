using NUnit.Framework;
using TowerDefense.Game.Data.Variables;
using UnityEngine;

namespace TowerDefense.Game.Tests.EditMode.Data
{
    public class IntVariableCallbackTests
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
        public void SetValue_FiresOnValueChanged_WithNewValue()
        {
            int receivedValue = -1;
            variable.OnValueChanged += v => receivedValue = v;

            variable.SetValue(42);

            Assert.That(receivedValue, Is.EqualTo(42));
        }

        [Test]
        public void ApplyChange_FiresOnValueChanged_WithResultingValue()
        {
            variable.SetValue(10);

            int receivedValue = -1;
            variable.OnValueChanged += v => receivedValue = v;

            variable.ApplyChange(5);

            Assert.That(receivedValue, Is.EqualTo(15));
        }

        [Test]
        public void SetValue_SameValue_DoesNotFire()
        {
            variable.SetValue(7);

            int callCount = 0;
            variable.OnValueChanged += _ => callCount++;

            variable.SetValue(7);

            Assert.That(callCount, Is.EqualTo(0));
        }

        [Test]
        public void UnsubscribedCallback_IsNotInvoked()
        {
            int callCount = 0;
            void Callback(int _) => callCount++;

            variable.OnValueChanged += Callback;
            variable.OnValueChanged -= Callback;

            variable.SetValue(99);

            Assert.That(callCount, Is.EqualTo(0));
        }

        [Test]
        public void ApplyChange_SameResultingValue_DoesNotFire()
        {
            int callCount = 0;
            variable.OnValueChanged += _ => callCount++;

            variable.ApplyChange(0);

            Assert.That(callCount, Is.EqualTo(0));
        }
    }
}
