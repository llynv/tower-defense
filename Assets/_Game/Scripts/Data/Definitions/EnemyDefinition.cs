using UnityEngine;

namespace TowerDefense.Game.Data.Definitions
{
    [CreateAssetMenu(menuName = "Tower Defense/Definitions/Enemy")]
    public sealed class EnemyDefinition : ScriptableObject
    {
        [Min(0f)]
        [SerializeField] private float moveSpeed;

        [Min(1)]
        [SerializeField] private int maxHealth;

        [Min(0)]
        [SerializeField] private int goldReward;

        public float MoveSpeed => moveSpeed;
        public int MaxHealth => maxHealth;
        public int GoldReward => goldReward;
    }
}
