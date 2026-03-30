using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TowerDefense.Game.Data.Definitions;
using TowerDefense.Game.Data.Variables;

namespace TowerDefense.Game.UI
{
    public sealed class TowerBuildButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text costText;
        [SerializeField] private Image iconImage;
        [SerializeField] private CanvasGroup canvasGroup;

        private TowerBuildButtonLogic logic;
        private SelectionState selectionState;
        private IntVariable goldVariable;
        private GameObject towerPrefab;

        public void Initialize(TowerDefinition definition, GameObject prefab,
            SelectionState selection, IntVariable gold)
        {
            logic = new TowerBuildButtonLogic(definition);
            towerPrefab = prefab;
            selectionState = selection;
            goldVariable = gold;

            if (costText != null)
                costText.text = logic.CostText;

            if (goldVariable != null)
                goldVariable.OnValueChanged += OnGoldChanged;

            if (button != null)
                button.onClick.AddListener(OnClicked);

            UpdateAffordability(goldVariable != null ? goldVariable.Value : 0);
        }

        private void OnDestroy()
        {
            if (goldVariable != null)
                goldVariable.OnValueChanged -= OnGoldChanged;

            if (button != null)
                button.onClick.RemoveListener(OnClicked);
        }

        private void OnGoldChanged(int newGold)
        {
            UpdateAffordability(newGold);
        }

        private void OnClicked()
        {
            logic.Select(selectionState, towerPrefab);
        }

        private void UpdateAffordability(int currentGold)
        {
            bool affordable = logic.IsAffordable(currentGold);

            if (button != null)
                button.interactable = affordable;

            if (canvasGroup != null)
                canvasGroup.alpha = affordable ? 1f : 0.5f;
        }
    }
}
