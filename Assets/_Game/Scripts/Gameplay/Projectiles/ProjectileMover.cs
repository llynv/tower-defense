using UnityEngine;
using TowerDefense.Game.Data.Events;
using TowerDefense.Game.Gameplay.Combat;
using TowerDefense.Game.Gameplay.Enemies;

namespace TowerDefense.Game.Gameplay.Projectiles
{
    public sealed class ProjectileMover : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private float arrivalThreshold = 0.2f;

        private Transform target;
        private int damage;
        private EnemyRuntimeSet enemyRuntimeSet;
        private IntEventChannel enemyKilledRewardChannel;

        public void Initialize(
            Transform target,
            int damage,
            EnemyRuntimeSet enemyRuntimeSet,
            IntEventChannel enemyKilledRewardChannel)
        {
            this.target = target;
            this.damage = damage;
            this.enemyRuntimeSet = enemyRuntimeSet;
            this.enemyKilledRewardChannel = enemyKilledRewardChannel;
        }

        private void Update()
        {
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }

            Vector3 direction = target.position - transform.position;
            float distanceSqr = direction.sqrMagnitude;

            if (distanceSqr <= arrivalThreshold * arrivalThreshold)
            {
                DamageResolver.Resolve(target, damage, enemyRuntimeSet, enemyKilledRewardChannel);
                Destroy(gameObject);
                return;
            }

            transform.position += direction.normalized * (speed * Time.deltaTime);
        }
    }
}
