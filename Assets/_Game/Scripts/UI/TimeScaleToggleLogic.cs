namespace TowerDefense.Game.UI
{
    public sealed class TimeScaleToggleLogic
    {
        private const float NormalSpeed = 1f;
        private const float FastSpeed = 2f;

        public bool IsActive { get; private set; }
        public float CurrentSpeed => IsActive ? FastSpeed : NormalSpeed;

        public bool IsFastForwardAvailable()
        {
            return true;
        }

        public void Toggle()
        {
            IsActive = !IsActive;
        }

        public void Reset()
        {
            IsActive = false;
        }
    }
}
