using NUnit.Framework;
using TowerDefense.Game.UI;

namespace TowerDefense.Game.Tests.EditMode.UI
{
    public class TimeScaleToggleLogicTests
    {
        [Test]
        public void IsFastForwardAvailable_ReturnsTrue()
        {
            var logic = new TimeScaleToggleLogic();

            Assert.That(logic.IsFastForwardAvailable(), Is.True);
        }

        [Test]
        public void IsActive_InitiallyFalse()
        {
            var logic = new TimeScaleToggleLogic();

            Assert.That(logic.IsActive, Is.False);
        }

        [Test]
        public void CurrentSpeed_Initially1()
        {
            var logic = new TimeScaleToggleLogic();

            Assert.That(logic.CurrentSpeed, Is.EqualTo(1f));
        }

        [Test]
        public void Toggle_ActivatesFastForward()
        {
            var logic = new TimeScaleToggleLogic();

            logic.Toggle();

            Assert.That(logic.IsActive, Is.True);
            Assert.That(logic.CurrentSpeed, Is.EqualTo(2f));
        }

        [Test]
        public void Toggle_Twice_DeactivatesFastForward()
        {
            var logic = new TimeScaleToggleLogic();

            logic.Toggle();
            logic.Toggle();

            Assert.That(logic.IsActive, Is.False);
            Assert.That(logic.CurrentSpeed, Is.EqualTo(1f));
        }

        [Test]
        public void Reset_DeactivatesFastForward()
        {
            var logic = new TimeScaleToggleLogic();
            logic.Toggle();

            logic.Reset();

            Assert.That(logic.IsActive, Is.False);
            Assert.That(logic.CurrentSpeed, Is.EqualTo(1f));
        }
    }
}
