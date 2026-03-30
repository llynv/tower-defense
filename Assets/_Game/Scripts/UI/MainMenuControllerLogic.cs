namespace TowerDefense.Game.UI
{
    public sealed class MainMenuControllerLogic
    {
        private const string DefaultGameScene = "PrototypeLane01";

        public string GameSceneName => DefaultGameScene;

        public bool IsSettingsAvailable()
        {
            return false;
        }

        public bool ShouldShowQuit()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return false;
#else
            return true;
#endif
        }
    }
}