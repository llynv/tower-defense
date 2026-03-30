using UnityEngine;
using TowerDefense.Game.Core;
using TowerDefense.Game.Data.Definitions;
using TowerDefense.Game.Map;

namespace TowerDefense.Game.Gameplay.Building
{
    public sealed class TowerPlacer : MonoBehaviour
    {
        [Header("Tower")]
        [SerializeField] private TowerDefinition towerDefinition;
        [SerializeField] private GameObject towerPrefab;

        [Header("Dependencies")]
        [SerializeField] private MatchStateController matchStateController;

        private TowerPlacementLogic placementLogic;

        private void Start()
        {
            placementLogic = new TowerPlacementLogic(matchStateController.Resources);
        }

        public bool TryPlaceAt(BuildNode buildNode)
        {
            if (placementLogic == null || towerDefinition == null || towerPrefab == null)
                return false;

            var occupancy = buildNode.Occupancy;
            if (occupancy == null)
                return false;

            if (!placementLogic.TryPlace(occupancy, towerDefinition.GoldCost))
                return false;

            Instantiate(towerPrefab, buildNode.PlacementPosition, Quaternion.identity);
            return true;
        }
    }
}
