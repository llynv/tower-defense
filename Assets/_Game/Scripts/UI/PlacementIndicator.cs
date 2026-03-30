using UnityEngine;
using TowerDefense.Game.Data.Variables;

namespace TowerDefense.Game.UI
{
    public sealed class PlacementIndicator : MonoBehaviour
    {
        [SerializeField] private SelectionState selectionState;

        private PlacementIndicatorLogic logic;

        private void Awake()
        {
            logic = new PlacementIndicatorLogic();
        }

        private void OnEnable()
        {
            if (selectionState != null)
                selectionState.SelectionChanged += OnSelectionChanged;

            Refresh();
        }

        private void OnDisable()
        {
            if (selectionState != null)
                selectionState.SelectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged()
        {
            Refresh();
        }

        private void Refresh()
        {
            bool active = logic.ShouldBeActive(selectionState);
            gameObject.SetActive(active);
        }
    }
}