using TowerDefense.Game.Data.Variables;

namespace TowerDefense.Game.UI
{
    public sealed class PlacementIndicatorLogic
    {
        public bool ShouldBeActive(SelectionState selectionState)
        {
            return selectionState != null && selectionState.HasSelection;
        }
    }
}