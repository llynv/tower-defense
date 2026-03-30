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
        }

        public void OnPlayClicked()
        {
            SceneManager.LoadScene(logic.GameSceneName);
        }

        public void OnQuitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}