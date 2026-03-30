using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TowerDefense.Game.UI
{
    public sealed class TimeScaleToggle : MonoBehaviour
    {
        [SerializeField] private Button toggleButton;
        [SerializeField] private TextMeshProUGUI speedLabel;

        private TimeScaleToggleLogic logic;

        private void Awake()
        {
            logic = new TimeScaleToggleLogic();
            UpdateVisual();
        }

        private void OnEnable()
        {
            if (toggleButton != null)
                toggleButton.onClick.AddListener(OnToggleClicked);
        }

        private void OnDisable()
        {
            if (toggleButton != null)
                toggleButton.onClick.RemoveListener(OnToggleClicked);

            logic?.Reset();
            Time.timeScale = 1f;
        }

        private void OnToggleClicked()
        {
            logic.Toggle();
            Time.timeScale = logic.CurrentSpeed;
            UpdateVisual();
        }

        private void UpdateVisual()
        {
            if (speedLabel != null)
                speedLabel.text = logic.IsActive ? "2x" : "1x";
        }
    }
}
