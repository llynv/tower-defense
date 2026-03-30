using TMPro;
using UnityEngine;
using TowerDefense.Game.Data.Variables;

namespace TowerDefense.Game.UI
{
    public sealed class LivesDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text textComponent;
        [SerializeField] private IntVariable livesVariable;

        private void Update()
        {
            if (textComponent != null && livesVariable != null)
                textComponent.text = livesVariable.Value.ToString();
        }
    }
}
