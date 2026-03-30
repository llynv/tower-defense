namespace TowerDefense.Game.UI
{
    public sealed class BuildNodeHighlighterLogic
    {
        public bool IsAvailable(bool isOccupied)
        {
            return !isOccupied;
        }
    }
}