using NUnit.Framework;
using TowerDefense.Game.Data.Events;
using TowerDefense.Game.UI;
using UnityEngine;

namespace TowerDefense.Game.Tests.EditMode.UI
{
    public class HUDLogicTests
    {
        [Test]
        public void UITypes_Compile()
        {
            Assert.That(typeof(GoldDisplay), Is.Not.Null);
            Assert.That(typeof(LivesDisplay), Is.Not.Null);
            Assert.That(typeof(WaveStateDisplay), Is.Not.Null);
            Assert.That(typeof(StartWaveButton), Is.Not.Null);
            Assert.That(typeof(EndStatePanel), Is.Not.Null);
            Assert.That(typeof(HUDController), Is.Not.Null);
        }

        [Test]
        public void UITypes_AreMonoBehaviours()
        {
            Assert.That(typeof(GoldDisplay).IsSubclassOf(typeof(MonoBehaviour)), Is.True);
            Assert.That(typeof(LivesDisplay).IsSubclassOf(typeof(MonoBehaviour)), Is.True);
            Assert.That(typeof(WaveStateDisplay).IsSubclassOf(typeof(MonoBehaviour)), Is.True);
            Assert.That(typeof(StartWaveButton).IsSubclassOf(typeof(MonoBehaviour)), Is.True);
            Assert.That(typeof(EndStatePanel).IsSubclassOf(typeof(MonoBehaviour)), Is.True);
            Assert.That(typeof(HUDController).IsSubclassOf(typeof(MonoBehaviour)), Is.True);
        }

        [Test]
        public void VoidEventChannel_RaiseEvent_InvokesListener()
        {
            var channel = ScriptableObject.CreateInstance<VoidEventChannel>();
            bool invoked = false;
            channel.RegisterListener(() => invoked = true);

            channel.RaiseEvent();

            Assert.That(invoked, Is.True);
            Object.DestroyImmediate(channel);
        }

        [Test]
        public void VoidEventChannel_UnregisterListener_StopsReceivingEvents()
        {
            var channel = ScriptableObject.CreateInstance<VoidEventChannel>();
            int count = 0;
            System.Action listener = () => count++;
            channel.RegisterListener(listener);
            channel.RaiseEvent();
            channel.UnregisterListener(listener);

            channel.RaiseEvent();

            Assert.That(count, Is.EqualTo(1));
            Object.DestroyImmediate(channel);
        }
    }
}
