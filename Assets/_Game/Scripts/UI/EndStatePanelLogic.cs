namespace TowerDefense.Game.UI
{
    public sealed class EndStatePanelLogic
    {
        public string GetMessage(bool isVictory)
        {
            return isVictory ? "Victory!" : "Defeat!";
        }

        public bool IsNextLevelAvailable()
        {
            return false;
        }

        public bool ShouldShowRetry(bool isVictory)
        {
            return true;
        }

        public bool ShouldShowMainMenu()
        {
            return true;
        }
    }
}