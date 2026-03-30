namespace TowerDefense.Game.UI
{
    public sealed class PauseMenuLogic
    {
        public bool IsPaused { get; private set; }

        public float DesiredTimeScale => IsPaused ? 0f : 1f;

        public void Pause()
        {
            IsPaused = true;
        }

        public void Resume()
        {
            IsPaused = false;
        }

        public void Toggle()
        {
            if (IsPaused)
                Resume();
            else
                Pause();
        }
    }
}