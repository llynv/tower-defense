using TMPro;
using UnityEngine;
using TowerDefense.Game.Data.Events;

namespace TowerDefense.Game.UI
{
    public sealed class EndStatePanel : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private VoidEventChannel victoryChannel;
        [SerializeField] private VoidEventChannel defeatChannel;

        private void Awake()
        {
            if (panel != null)
                panel.SetActive(false);
        }

        private void OnEnable()
        {
            if (victoryChannel != null) victoryChannel.RegisterListener(OnVictory);
            if (defeatChannel != null) defeatChannel.RegisterListener(OnDefeat);
        }

        private void OnDisable()
        {
            if (victoryChannel != null) victoryChannel.UnregisterListener(OnVictory);
            if (defeatChannel != null) defeatChannel.UnregisterListener(OnDefeat);
        }

        private void OnVictory()
        {
            ShowPanel("Victory!");
        }

        private void OnDefeat()
        {
            ShowPanel("Defeat!");
        }

        private void ShowPanel(string message)
        {
            if (messageText != null)
                messageText.text = message;

            if (panel != null)
                panel.SetActive(true);
        }
    }
}
