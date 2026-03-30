using UnityEngine;
using TowerDefense.Game.Data.Events;

namespace TowerDefense.Game.UI
{
    public sealed class HUDController : MonoBehaviour
    {
        [SerializeField] private GameObject startWaveButtonObject;
        [SerializeField] private VoidEventChannel waveStartedChannel;
        [SerializeField] private VoidEventChannel victoryChannel;
        [SerializeField] private VoidEventChannel defeatChannel;

        private void OnEnable()
        {
            SetStartWaveButtonActive(true);

            if (waveStartedChannel != null) waveStartedChannel.RegisterListener(OnWaveStarted);
            if (victoryChannel != null) victoryChannel.RegisterListener(OnMatchEnded);
            if (defeatChannel != null) defeatChannel.RegisterListener(OnMatchEnded);
        }

        private void OnDisable()
        {
            if (waveStartedChannel != null) waveStartedChannel.UnregisterListener(OnWaveStarted);
            if (victoryChannel != null) victoryChannel.UnregisterListener(OnMatchEnded);
            if (defeatChannel != null) defeatChannel.UnregisterListener(OnMatchEnded);
        }

        public void ShowStartWaveButton()
        {
            SetStartWaveButtonActive(true);
        }

        private void OnWaveStarted()
        {
            SetStartWaveButtonActive(false);
        }

        private void OnMatchEnded()
        {
            SetStartWaveButtonActive(false);
        }

        private void SetStartWaveButtonActive(bool active)
        {
            if (startWaveButtonObject != null)
                startWaveButtonObject.SetActive(active);
        }
    }
}
