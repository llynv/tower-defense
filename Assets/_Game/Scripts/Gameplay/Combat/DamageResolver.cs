using UnityEngine;
using TowerDefense.Game.Data.Events;
using TowerDefense.Game.Gameplay.Enemies;

namespace TowerDefense.Game.Gameplay.Combat
{
    public static class DamageResolver
    {
        public static void Resolve(
            Transform target,
            int damage,
            EnemyRuntimeSet enemyRuntimeSet,
            IntEventChannel enemyKilledRewardChannel)
        {
            if (target == null)
                return;

            EnemyHealth health = target.GetComponent<EnemyHealth>();
            if (health == null || health.IsDead)
                return;

            health.TakeDamage(damage);

            if (!health.IsDead)
                return;

            EnemyMover mover = target.GetComponent<EnemyMover>();
            if (mover != null && enemyRuntimeSet != null)
                enemyRuntimeSet.Remove(mover);

            if (enemyKilledRewardChannel != null && health.GoldReward > 0)
                enemyKilledRewardChannel.RaiseEvent(health.GoldReward);

            Object.Destroy(target.gameObject);
        }
    }
}
