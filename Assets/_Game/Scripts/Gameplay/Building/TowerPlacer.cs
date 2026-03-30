using UnityEngine;
using TowerDefense.Game.Core;
using TowerDefense.Game.Data.Definitions;
using TowerDefense.Game.Map;

namespace TowerDefense.Game.Gameplay.Building
{
    public sealed class TowerPlacer : MonoBehaviour
    {
        [SerializeField] private MatchStateController matchStateController;

        private TowerPlacementLogic placementLogic;

        private void Start()
        {
            placementLogic = new TowerPlacementLogic(matchStateController.Resources);
        }

        public bool TryPlaceAt(BuildNode buildNode, TowerDefinition definition, GameObject prefab)
        {
            if (placementLogic == null || definition == null || prefab == null)
                return false;

            var occupancy = buildNode.Occupancy;
            if (occupancy == null)
                return false;

            if (!placementLogic.TryPlace(occupancy, definition.GoldCost))
                return false;

            Instantiate(prefab, buildNode.PlacementPosition, Quaternion.identity);
            return true;
        }
    }
}
