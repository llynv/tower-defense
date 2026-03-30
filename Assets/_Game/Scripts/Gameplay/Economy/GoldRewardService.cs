using UnityEngine;
using TowerDefense.Game.Data.Events;

namespace TowerDefense.Game.Gameplay.Economy
{
    public sealed class GoldRewardService : MonoBehaviour
    {
        [SerializeField] private IntEventChannel enemyKilledRewardChannel;
        [SerializeField] private IntEventChannel goldEarnedChannel;

        private void OnEnable()
        {
            if (enemyKilledRewardChannel != null)
                enemyKilledRewardChannel.RegisterListener(OnEnemyKilledReward);
        }

        private void OnDisable()
        {
            if (enemyKilledRewardChannel != null)
                enemyKilledRewardChannel.UnregisterListener(OnEnemyKilledReward);
        }

        private void OnEnemyKilledReward(int goldAmount)
        {
            if (goldAmount <= 0 || goldEarnedChannel == null)
                return;

            goldEarnedChannel.RaiseEvent(goldAmount);
        }
    }
}
