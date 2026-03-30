using UnityEngine;

namespace TowerDefense.Game.Gameplay.Towers
{
    public sealed class TowerAttackLogic
    {
        private readonly float attackInterval;
        private float timer;

        public TowerAttackLogic(float attackInterval)
        {
            this.attackInterval = Mathf.Max(0.01f, attackInterval);
        }

        public bool CanFire => timer >= attackInterval;

        public void Tick(float deltaTime)
        {
            timer += deltaTime;
        }

        public void ConsumeShot()
        {
            timer = 0f;
        }
    }
}
