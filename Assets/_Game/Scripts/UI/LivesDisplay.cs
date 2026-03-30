using TMPro;
using UnityEngine;
using TowerDefense.Game.Data.Variables;

namespace TowerDefense.Game.UI
{
    public sealed class LivesDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text textComponent;
        [SerializeField] private IntVariable livesVariable;

        private void OnEnable()
        {
            if (livesVariable != null)
            {
                livesVariable.OnValueChanged += UpdateText;
                UpdateText(livesVariable.Value);
            }
        }

        private void OnDisable()
        {
            if (livesVariable != null)
                livesVariable.OnValueChanged -= UpdateText;
        }

        private void UpdateText(int value)
        {
            if (textComponent != null)
                textComponent.text = value.ToString();
        }
    }
}
