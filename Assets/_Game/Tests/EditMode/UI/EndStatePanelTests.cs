using NUnit.Framework;
using TowerDefense.Game.UI;

namespace TowerDefense.Game.Tests.EditMode.UI
{
    public class EndStatePanelTests
    {
        [Test]
        public void EndStatePanelLogic_VictoryMessage_ReturnsVictory()
        {
            var logic = new EndStatePanelLogic();

            Assert.That(logic.GetMessage(true), Is.EqualTo("Victory!"));
        }

        [Test]
        public void EndStatePanelLogic_DefeatMessage_ReturnsGameOver()
        {
            var logic = new EndStatePanelLogic();

            Assert.That(logic.GetMessage(false), Is.EqualTo("Defeat!"));
        }

        [Test]
        public void EndStatePanelLogic_IsNextLevelAvailable_ReturnsFalse()
        {
            var logic = new EndStatePanelLogic();

            Assert.That(logic.IsNextLevelAvailable(), Is.False);
        }

        [Test]
        public void EndStatePanelLogic_ShouldShowRetry_AlwaysTrue()
        {
            var logic = new EndStatePanelLogic();

            Assert.That(logic.ShouldShowRetry(true), Is.True);
            Assert.That(logic.ShouldShowRetry(false), Is.True);
        }

        [Test]
        public void EndStatePanelLogic_ShouldShowMainMenu_AlwaysTrue()
        {
            var logic = new EndStatePanelLogic();

            Assert.That(logic.ShouldShowMainMenu(), Is.True);
        }
    }
}