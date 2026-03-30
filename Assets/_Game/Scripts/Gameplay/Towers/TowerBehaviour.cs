using System.Collections.Generic;
using UnityEngine;
using TowerDefense.Game.Data.Definitions;
using TowerDefense.Game.Data.Events;
using TowerDefense.Game.Gameplay.Enemies;
using TowerDefense.Game.Gameplay.Projectiles;

namespace TowerDefense.Game.Gameplay.Towers
{
    public sealed class TowerBehaviour : MonoBehaviour
    {
        [SerializeField] private TowerDefinition definition;
        [SerializeField] private EnemyRuntimeSet enemyRuntimeSet;
        [SerializeField] private IntEventChannel enemyKilledRewardChannel;
        [SerializeField] private GameObject projectilePrefab;

        private TowerAttackLogic attackLogic;
        private readonly List<CandidateEnemy> candidates = new();

        private void Start()
        {
            attackLogic = new TowerAttackLogic(definition.AttackIntervalSeconds);
        }

        private void Update()
        {
            if (attackLogic == null || enemyRuntimeSet == null)
                return;

            attackLogic.Tick(Time.deltaTime);

            if (!attackLogic.CanFire)
                return;

            BuildCandidateList();

            if (!TowerTargetingLogic.TryGetTarget(
                    transform.position,
                    definition.AttackRange,
                    candidates,
                    out int bestIndex))
                return;

            attackLogic.ConsumeShot();
            FireProjectile(enemyRuntimeSet.Items[bestIndex]);
        }

        private void BuildCandidateList()
        {
            candidates.Clear();

            IReadOnlyList<EnemyMover> enemies = enemyRuntimeSet.Items;

            for (int i = 0; i < enemies.Count; i++)
            {
                EnemyMover mover = enemies[i];
                bool isNull = mover == null;

                EnemyHealth health = isNull ? null : mover.GetComponent<EnemyHealth>();
                bool isDead = isNull || (health != null && health.IsDead);

                candidates.Add(new CandidateEnemy
                {
                    Position = isNull ? Vector3.zero : mover.transform.position,
                    Progress = isNull ? -1f : mover.Progress,
                    IsDead = isDead
                });
            }
        }

        private void FireProjectile(EnemyMover targetMover)
        {
            if (projectilePrefab == null)
                return;

            GameObject go = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            ProjectileMover projectile = go.GetComponent<ProjectileMover>();

            if (projectile == null)
            {
                Destroy(go);
                return;
            }

            projectile.Initialize(
                targetMover.transform,
                definition.DamagePerShot,
                enemyRuntimeSet,
                enemyKilledRewardChannel);
        }
    }
}
