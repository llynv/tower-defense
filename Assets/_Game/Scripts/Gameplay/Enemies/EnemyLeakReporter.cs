using UnityEngine;
using TowerDefense.Game.Data.Events;

namespace TowerDefense.Game.Gameplay.Enemies
{
    [RequireComponent(typeof(EnemyMover))]
    public sealed class EnemyLeakReporter : MonoBehaviour
    {
        [SerializeField] private IntEventChannel enemyLeakedChannel;
        [SerializeField] private EnemyRuntimeSet enemyRuntimeSet;

        private EnemyMover mover;
        private bool reported;

        private void Awake()
        {
            mover = GetComponent<EnemyMover>();
        }

        private void Update()
        {
            if (reported || !mover.HasReachedEnd)
                return;

            EnemyHealth health = GetComponent<EnemyHealth>();
            if (health != null && health.IsDead)
                return;

            reported = true;

            if (enemyRuntimeSet != null)
                enemyRuntimeSet.Remove(mover);

            if (enemyLeakedChannel != null)
                enemyLeakedChannel.RaiseEvent(1);

            Destroy(gameObject);
        }
    }
}
