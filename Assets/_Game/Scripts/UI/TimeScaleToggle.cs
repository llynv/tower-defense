using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.Game.UI
{
    public sealed class TimeScaleToggle : MonoBehaviour
    {
        [SerializeField] private Button toggleButton;

        private TimeScaleToggleLogic logic;

        private void Awake()
        {
            logic = new TimeScaleToggleLogic();

            if (toggleButton != null)
                toggleButton.interactable = logic.IsFastForwardAvailable();
        }
    }
}