using TowerDefense.Game.Core;

namespace TowerDefense.Game.Gameplay.Building
{
    public sealed class TowerPlacementLogic
    {
        private readonly PlayerResourcesLogic resources;

        public TowerPlacementLogic(PlayerResourcesLogic resources)
        {
            this.resources = resources;
        }

        public bool TryPlace(BuildNodeOccupancy node, int goldCost)
        {
            if (goldCost < 0 || node.IsOccupied)
                return false;

            if (!resources.TrySpendGold(goldCost))
                return false;

            if (!node.Occupy())
            {
                resources.AddGold(goldCost);
                return false;
            }

            return true;
        }
    }
}
