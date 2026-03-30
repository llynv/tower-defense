using NUnit.Framework;
using TowerDefense.Game.UI;

namespace TowerDefense.Game.Tests.EditMode.UI
{
    public class MainMenuTests
    {
        [Test]
        public void MainMenuControllerLogic_IsSettingsAvailable_ReturnsFalse()
        {
            var logic = new MainMenuControllerLogic();

            Assert.That(logic.IsSettingsAvailable(), Is.False);
        }

        [Test]
        public void MainMenuControllerLogic_GameSceneName_ReturnsNonEmpty()
        {
            var logic = new MainMenuControllerLogic();

            Assert.That(logic.GameSceneName, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void MainMenuControllerLogic_ShouldShowQuit_ReturnsTrueOnNonWebGL()
        {
            var logic = new MainMenuControllerLogic();

            bool result = logic.ShouldShowQuit();

#if UNITY_WEBGL && !UNITY_EDITOR
            Assert.That(result, Is.False);
#else
            Assert.That(result, Is.True);
#endif
        }
    }
}