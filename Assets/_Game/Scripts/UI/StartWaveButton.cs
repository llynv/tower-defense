using UnityEngine;
using UnityEngine.UI;
using TowerDefense.Game.Data.Events;

namespace TowerDefense.Game.UI
{
    public sealed class StartWaveButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private VoidEventChannel startWaveRequestedChannel;

        private void OnEnable()
        {
            if (button != null)
                button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            if (button != null)
                button.onClick.RemoveListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            if (startWaveRequestedChannel != null)
                startWaveRequestedChannel.RaiseEvent();
        }
    }
}
