using NUnit.Framework;
using TowerDefense.Game.Data.Events;
using TowerDefense.Game.UI;
using UnityEngine;

namespace TowerDefense.Game.Tests.EditMode.UI
{
    public class PauseMenuTests
    {
        [Test]
        public void PauseMenuLogic_Pause_SetsIsPausedTrue()
        {
            var logic = new PauseMenuLogic();

            logic.Pause();

            Assert.That(logic.IsPaused, Is.True);
        }

        [Test]
        public void PauseMenuLogic_Resume_SetsIsPausedFalse()
        {
            var logic = new PauseMenuLogic();
            logic.Pause();

            logic.Resume();

            Assert.That(logic.IsPaused, Is.False);
        }

        [Test]
        public void PauseMenuLogic_DesiredTimeScale_IsZero_WhenPaused()
        {
            var logic = new PauseMenuLogic();

            logic.Pause();

            Assert.That(logic.DesiredTimeScale, Is.EqualTo(0f));
        }

        [Test]
        public void PauseMenuLogic_DesiredTimeScale_IsOne_WhenResumed()
        {
            var logic = new PauseMenuLogic();
            logic.Pause();

            logic.Resume();

            Assert.That(logic.DesiredTimeScale, Is.EqualTo(1f));
        }

        [Test]
        public void PauseMenuLogic_Toggle_FlipsPauseState()
        {
            var logic = new PauseMenuLogic();

            logic.Toggle();
            Assert.That(logic.IsPaused, Is.True);

            logic.Toggle();
            Assert.That(logic.IsPaused, Is.False);
        }

        [Test]
        public void PauseMenuLogic_PauseRequested_Channel_RaisesEvent()
        {
            var channel = ScriptableObject.CreateInstance<VoidEventChannel>();
            bool raised = false;
            channel.RegisterListener(() => raised = true);

            channel.RaiseEvent();

            Assert.That(raised, Is.True);

            Object.DestroyImmediate(channel);
        }

        [Test]
        public void TimeScaleToggleLogic_IsFastForwardAvailable_ReturnsFalse()
        {
            var logic = new TimeScaleToggleLogic();

            Assert.That(logic.IsFastForwardAvailable(), Is.False);
        }
    }
}