using UnityEngine;
using UnityEngine.SceneManagement;
using TowerDefense.Game.Data.Events;

namespace TowerDefense.Game.UI
{
    public sealed class PauseMenuController : MonoBehaviour
    {
        [SerializeField] private VoidEventChannel pauseRequestedChannel;
        [SerializeField] private GameObject overlayRoot;

        private PauseMenuLogic logic;

        private void Awake()
        {
            logic = new PauseMenuLogic();
            SetOverlayVisible(false);
        }

        private void OnEnable()
        {
            if (pauseRequestedChannel != null)
                pauseRequestedChannel.RegisterListener(OnPauseRequested);
        }

        private void OnDisable()
        {
            if (pauseRequestedChannel != null)
                pauseRequestedChannel.UnregisterListener(OnPauseRequested);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                OnPauseRequested();
        }

        private void OnPauseRequested()
        {
            logic.Toggle();
            ApplyState();
        }

        public void OnResumeClicked()
        {
            logic.Resume();
            ApplyState();
        }

        public void OnRestartClicked()
        {
            logic.Resume();
            ApplyState();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void OnMainMenuClicked()
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
    }
}