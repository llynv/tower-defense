using UnityEngine;
using TowerDefense.Game.Data.Events;

namespace TowerDefense.Game.UI
{
    public sealed class HUDController : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private GameObject startWaveButtonObject;

        [Header("Panels")]
        [SerializeField] private GameObject buildMenuObject;

        [Header("Event Channels")]
        [SerializeField] private VoidEventChannel waveStartedChannel;
        [SerializeField] private VoidEventChannel waveCompletedChannel;
        [SerializeField] private VoidEventChannel victoryChannel;
        [SerializeField] private VoidEventChannel defeatChannel;

        private void OnEnable()
        {
            SetStartWaveButtonActive(true);
            SetBuildMenuActive(true);

            if (waveStartedChannel != null) waveStartedChannel.RegisterListener(OnWaveStarted);
            if (waveCompletedChannel != null) waveCompletedChannel.RegisterListener(OnWaveCompleted);
            if (victoryChannel != null) victoryChannel.RegisterListener(OnMatchEnded);
            if (defeatChannel != null) defeatChannel.RegisterListener(OnMatchEnded);
        }

        private void OnDisable()
        {
            if (waveStartedChannel != null) waveStartedChannel.UnregisterListener(OnWaveStarted);
            if (waveCompletedChannel != null) waveCompletedChannel.UnregisterListener(OnWaveCompleted);
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
            SetBuildMenuActive(false);
        }

        private void OnWaveCompleted()
        {
            SetStartWaveButtonActive(true);
            SetBuildMenuActive(true);
        }

        private void OnMatchEnded()
        {
            SetStartWaveButtonActive(false);
            SetBuildMenuActive(false);
        }

        private void SetStartWaveButtonActive(bool active)
        {
            if (startWaveButtonObject != null)
                startWaveButtonObject.SetActive(active);
        }

        private void SetBuildMenuActive(bool active)
        {
            if (buildMenuObject != null)
                buildMenuObject.SetActive(active);
        }
    }
}
