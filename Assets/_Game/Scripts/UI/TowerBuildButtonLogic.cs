using TowerDefense.Game.Data.Definitions;
using TowerDefense.Game.Data.Variables;
using UnityEngine;

namespace TowerDefense.Game.UI
{
    public sealed class TowerBuildButtonLogic
    {
        private readonly TowerDefinition definition;

        public TowerBuildButtonLogic(TowerDefinition definition)
        {
            this.definition = definition;
        }

        public string CostText => definition.GoldCost.ToString();

        public bool IsAffordable(int currentGold)
        {
            return currentGold >= definition.GoldCost;
        }

        public void Select(SelectionState selectionState, GameObject towerPrefab)
        {
            if (selectionState.SelectedTower == definition)
                selectionState.Clear();
            else
                selectionState.Select(definition, towerPrefab);
        }
    }
}
