using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using TowerDefense.Game.Data.Definitions;

namespace TowerDefense.Game.UI
{
    public sealed class TowerInfoPanel : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private TMP_Text damageText;
        [SerializeField] private TMP_Text rangeText;
        [SerializeField] private TMP_Text fireRateText;

        [Header("Buttons")]
        [SerializeField] private Button upgradeButton;
        [SerializeField] private Button sellButton;
        [SerializeField] private TMP_Text sellCostText;

        [Header("Panel")]
        [SerializeField] private GameObject panelRoot;

        private TowerInfoPanelLogic logic;
        private TowerDefinition currentDefinition;

        private void Awake()
        {
            logic = new TowerInfoPanelLogic();
            Hide();
        }

        public void Show(TowerDefinition definition)
        {
            currentDefinition = definition;
            Refresh();

            if (panelRoot != null)
                panelRoot.SetActive(true);
        }

        public void Hide()
        {
            currentDefinition = null;

            if (panelRoot != null)
                panelRoot.SetActive(false);
        }

        private void Update()
        {
            if (currentDefinition != null && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
                Hide();
        }

        private void Refresh()
        {
            if (currentDefinition == null)
                return;

            TowerStats stats = logic.GetStats(currentDefinition);

            if (damageText != null)
                damageText.text = stats.damage.ToString();

            if (rangeText != null)
                rangeText.text = stats.range.ToString("F1");

            if (fireRateText != null)
                fireRateText.text = stats.fireRate.ToString("F1") + "s";

            if (upgradeButton != null)
                upgradeButton.interactable = logic.IsUpgradeAvailable();

            if (sellButton != null)
                sellButton.interactable = logic.IsSellAvailable();

            if (sellCostText != null)
            {
                int sellCost = logic.ComputeSellCost(currentDefinition);
                sellCostText.text = sellCost.ToString();
            }
        }
    }
}