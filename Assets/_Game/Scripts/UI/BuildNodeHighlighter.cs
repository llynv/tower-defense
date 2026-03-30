using UnityEngine;
using TowerDefense.Game.Data.Variables;
using TowerDefense.Game.Map;

namespace TowerDefense.Game.UI
{
    public sealed class BuildNodeHighlighter : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer highlightRenderer;
        [SerializeField] private Color availableColor = new Color(0f, 1f, 0f, 0.4f);
        [SerializeField] private Color occupiedColor = new Color(1f, 0f, 0f, 0.4f);
        [SerializeField] private SelectionState selectionState;

        private BuildNode buildNode;
        private BuildNodeHighlighterLogic logic;

        private void Awake()
        {
            logic = new BuildNodeHighlighterLogic();
            buildNode = GetComponentInParent<BuildNode>();
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
            bool inPlacementMode = selectionState != null && selectionState.HasSelection;

            if (!inPlacementMode || buildNode == null)
            {
                SetVisible(false);
                return;
            }

            bool available = logic.IsAvailable(buildNode.Occupancy.IsOccupied);
            SetVisible(true);

            if (highlightRenderer != null)
                highlightRenderer.color = available ? availableColor : occupiedColor;
        }

        private void SetVisible(bool visible)
        {
            if (highlightRenderer != null)
                highlightRenderer.enabled = visible;
        }
    }
}