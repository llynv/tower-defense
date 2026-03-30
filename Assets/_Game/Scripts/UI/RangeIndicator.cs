using UnityEngine;
using TowerDefense.Game.Data.Variables;

namespace TowerDefense.Game.UI
{
    public sealed class RangeIndicator : MonoBehaviour
    {
        [SerializeField] private SelectionState selectionState;

        private RangeIndicatorLogic logic;

        private void Awake()
        {
            logic = new RangeIndicatorLogic();
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
            if (selectionState == null || !selectionState.HasSelection)
            {
                gameObject.SetActive(false);
                return;
            }

            float range = selectionState.SelectedTower != null
                ? selectionState.SelectedTower.AttackRange
                : 0f;

            transform.localScale = logic.ComputeScale(range);
            gameObject.SetActive(true);
        }
    }
}