using System;
using UnityEngine;
using TowerDefense.Game.Data.Definitions;

namespace TowerDefense.Game.Gameplay.Enemies
{
    public sealed class EnemyHealth : MonoBehaviour
    {
        private EnemyHealthLogic logic;
        private int goldReward;

        public int CurrentHealth => logic?.CurrentHealth ?? 0;
        public bool IsDead => logic?.IsDead ?? false;
        public int GoldReward => goldReward;

        public event Action Died;

        public void Initialize(EnemyDefinition definition)
        {
            logic = new EnemyHealthLogic(definition.MaxHealth);
            goldReward = definition.GoldReward;
        }

        public void TakeDamage(int amount)
        {
            if (logic == null || logic.IsDead)
                return;

            logic.TakeDamage(amount);

            if (logic.IsDead)
                Died?.Invoke();
        }
    }
}
