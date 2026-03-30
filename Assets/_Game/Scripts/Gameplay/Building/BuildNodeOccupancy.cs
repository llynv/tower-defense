namespace TowerDefense.Game.Gameplay.Building
{
    public sealed class BuildNodeOccupancy
    {
        public bool IsOccupied { get; private set; }

        public bool Occupy()
        {
            if (IsOccupied)
                return false;

            IsOccupied = true;
            return true;
        }
    }
}
