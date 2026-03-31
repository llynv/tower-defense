using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TowerDefense.Game.Data.Events;

namespace TowerDefense.Game.UI
{
    public sealed class EndStatePanel : MonoBehaviour
    {
        [Header("Display")]
        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text messageText;

        [Header("Channels")]
        [SerializeField] private VoidEventChannel victoryChannel;
        [SerializeField] private VoidEventChannel defeatChannel;

        [Header("Buttons")]
        [SerializeField] private Button retryButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button nextLevelButton;

        private EndStatePanelLogic logic;
        private bool isVictory;

        private void Awake()
        {
            logic = new EndStatePanelLogic();
            SetButtonLabel(retryButton, "Retry");
            SetButtonLabel(mainMenuButton, "Main Menu");
            SetButtonLabel(nextLevelButton, "Next Level");

            if (panel != null)
                panel.SetActive(false);
        }

        private void OnEnable()
        {
            if (victoryChannel != null) victoryChannel.RegisterListener(OnVictory);
            if (defeatChannel != null) defeatChannel.RegisterListener(OnDefeat);

            if (retryButton != null)
                retryButton.onClick.AddListener(OnRetryClicked);
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        }

        private void OnDisable()
        {
            if (victoryChannel != null) victoryChannel.UnregisterListener(OnVictory);
            if (defeatChannel != null) defeatChannel.UnregisterListener(OnDefeat);

            if (retryButton != null)
                retryButton.onClick.RemoveListener(OnRetryClicked);
            if (mainMenuButton != null)
                mainMenuButton.onClick.RemoveListener(OnMainMenuClicked);
        }

        private void OnVictory()
        {
            isVictory = true;
            ShowPanel();
        }

        private void OnDefeat()
        {
            isVictory = false;
            ShowPanel();
        }

        private void ShowPanel()
        {
            string message = logic.GetMessage(isVictory);

            if (messageText != null)
                messageText.text = message;

            if (retryButton != null)
                retryButton.gameObject.SetActive(logic.ShouldShowRetry(isVictory));

            if (mainMenuButton != null)
                mainMenuButton.gameObject.SetActive(logic.ShouldShowMainMenu());

            if (nextLevelButton != null)
                nextLevelButton.interactable = logic.IsNextLevelAvailable();

            if (panel != null)
                panel.SetActive(true);
        }

        private void OnRetryClicked()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void OnMainMenuClicked()
        {
            SceneManager.LoadScene("MainMenu");
        }

        private static void SetButtonLabel(Button button, string text)
        {
            if (button == null) return;
            var label = button.GetComponent<TMP_Text>();
            if (label != null)
                label.text = text;
        }
    }
}
