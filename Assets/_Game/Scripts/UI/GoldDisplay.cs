using TMPro;
using UnityEngine;
using TowerDefense.Game.Data.Variables;

namespace TowerDefense.Game.UI
{
    public sealed class GoldDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text textComponent;
        [SerializeField] private IntVariable goldVariable;

        private void OnEnable()
        {
            if (goldVariable != null)
            {
                goldVariable.OnValueChanged += UpdateText;
                UpdateText(goldVariable.Value);
            }
        }

        private void OnDisable()
        {
            if (goldVariable != null)
                goldVariable.OnValueChanged -= UpdateText;
        }

        private void UpdateText(int value)
        {
            if (textComponent != null)
                textComponent.text = value.ToString();
        }
    }
}
