using TMPro;
using UnityEngine;
using TowerDefense.Game.Data.Variables;

namespace TowerDefense.Game.UI
{
    public sealed class WaveCounterDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text textComponent;
        [SerializeField] private IntVariable currentWaveVariable;
        [SerializeField] private IntVariable totalWaveVariable;

        private void OnEnable()
        {
            if (currentWaveVariable != null)
                currentWaveVariable.OnValueChanged += OnWaveValueChanged;
            if (totalWaveVariable != null)
                totalWaveVariable.OnValueChanged += OnWaveValueChanged;

            UpdateText();
        }

        private void OnDisable()
        {
            if (currentWaveVariable != null)
                currentWaveVariable.OnValueChanged -= OnWaveValueChanged;
            if (totalWaveVariable != null)
                totalWaveVariable.OnValueChanged -= OnWaveValueChanged;
        }

        private void OnWaveValueChanged(int _)
        {
            UpdateText();
        }

        private void UpdateText()
        {
            if (textComponent == null)
                return;

            int current = currentWaveVariable != null ? currentWaveVariable.Value : 0;
            int total = totalWaveVariable != null ? totalWaveVariable.Value : 0;
            textComponent.text = $"Wave {current}/{total}";
        }
    }
}
