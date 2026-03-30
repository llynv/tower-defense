using TMPro;
using UnityEngine;
using TowerDefense.Game.Data.Variables;

namespace TowerDefense.Game.UI
{
    public sealed class GoldDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text textComponent;
        [SerializeField] private IntVariable goldVariable;

        private void Update()
        {
            if (textComponent != null && goldVariable != null)
                textComponent.text = goldVariable.Value.ToString();
        }
    }
}
