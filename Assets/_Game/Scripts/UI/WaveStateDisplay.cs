using TMPro;
using UnityEngine;
using TowerDefense.Game.Data.Events;

namespace TowerDefense.Game.UI
{
    public sealed class WaveStateDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text textComponent;
        [SerializeField] private VoidEventChannel waveStartedChannel;
        [SerializeField] private VoidEventChannel victoryChannel;
        [SerializeField] private VoidEventChannel defeatChannel;

        private void OnEnable()
        {
            SetText("Build Phase");

            if (waveStartedChannel != null) waveStartedChannel.RegisterListener(OnWaveStarted);
            if (victoryChannel != null) victoryChannel.RegisterListener(OnVictory);
            if (defeatChannel != null) defeatChannel.RegisterListener(OnDefeat);
        }

        private void OnDisable()
        {
            if (waveStartedChannel != null) waveStartedChannel.UnregisterListener(OnWaveStarted);
            if (victoryChannel != null) victoryChannel.UnregisterListener(OnVictory);
            if (defeatChannel != null) defeatChannel.UnregisterListener(OnDefeat);
        }

        public void ResetToBuildPhase()
        {
            SetText("Build Phase");
        }

        private void OnWaveStarted()
        {
            SetText("Wave Running");
        }

        private void OnVictory()
        {
            SetText("Victory!");
        }

        private void OnDefeat()
        {
            SetText("Defeat!");
        }

        private void SetText(string message)
        {
            if (textComponent != null)
                textComponent.text = message;
        }
    }
}
