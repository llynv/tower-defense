using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using TowerDefense.Game.Data.Events;

namespace TowerDefense.Game.UI
{
    public sealed class PauseMenuController : MonoBehaviour
    {
        [SerializeField] private VoidEventChannel pauseRequestedChannel;
        [SerializeField] private GameObject overlayRoot;

        [Header("Buttons")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;

        private PauseMenuLogic logic;

        private void Awake()
        {
            logic = new PauseMenuLogic();
            SetOverlayVisible(false);
            SetButtonLabel(resumeButton, "Resume");
            SetButtonLabel(restartButton, "Restart");
            SetButtonLabel(mainMenuButton, "Main Menu");
        }

        private void OnEnable()
        {
            if (pauseRequestedChannel != null)
                pauseRequestedChannel.RegisterListener(OnPauseRequested);

            if (resumeButton != null)
                resumeButton.onClick.AddListener(OnResumeClicked);
            if (restartButton != null)
                restartButton.onClick.AddListener(OnRestartClicked);
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        }

        private void OnDisable()
        {
            if (pauseRequestedChannel != null)
                pauseRequestedChannel.UnregisterListener(OnPauseRequested);

            if (resumeButton != null)
                resumeButton.onClick.RemoveListener(OnResumeClicked);
            if (restartButton != null)
                restartButton.onClick.RemoveListener(OnRestartClicked);
            if (mainMenuButton != null)
                mainMenuButton.onClick.RemoveListener(OnMainMenuClicked);
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
                OnPauseRequested();
        }

        private void OnPauseRequested()
        {
            logic.Toggle();
            ApplyState();
        }

        private void OnResumeClicked()
        {
            logic.Resume();
            ApplyState();
        }

        private void OnRestartClicked()
        {
            logic.Resume();
            ApplyState();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void OnMainMenuClicked()
        {
            logic.Resume();
            ApplyState();
            SceneManager.LoadScene("MainMenu");
        }

        private void ApplyState()
        {
            Time.timeScale = logic.DesiredTimeScale;
            SetOverlayVisible(logic.IsPaused);
        }

        private void SetOverlayVisible(bool visible)
        {
            if (overlayRoot != null)
                overlayRoot.SetActive(visible);
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