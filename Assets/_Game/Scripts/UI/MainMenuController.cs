using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TowerDefense.Game.UI
{
    public sealed class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;

        private MainMenuControllerLogic logic;

        private void Awake()
        {
            logic = new MainMenuControllerLogic();

            if (settingsButton != null)
                settingsButton.interactable = logic.IsSettingsAvailable();

            if (quitButton != null)
                quitButton.gameObject.SetActive(logic.ShouldShowQuit());

            if (playButton != null)
                playButton.onClick.AddListener(OnPlayClicked);

            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuitClicked);
        }

        private void OnDestroy()
        {
            if (playButton != null)
                playButton.onClick.RemoveListener(OnPlayClicked);

            if (quitButton != null)
                quitButton.onClick.RemoveListener(OnQuitClicked);
        }

        private void OnPlayClicked()
        {
            SceneManager.LoadScene(logic.GameSceneName);
        }

        private void OnQuitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}