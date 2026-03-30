using UnityEngine;

namespace TowerDefense.Game.Gameplay.Enemies
{
    public sealed class EnemyHealthLogic
    {
        public EnemyHealthLogic(int maxHealth)
        {
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
        }

        public int MaxHealth { get; }
        public int CurrentHealth { get; private set; }

        public bool IsDead => CurrentHealth <= 0;

        public void TakeDamage(int amount)
        {
            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        }
    }
}
