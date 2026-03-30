using UnityEngine;

namespace TowerDefense.Game.Data.Definitions
{
    [CreateAssetMenu(menuName = "Tower Defense/Definitions/Tower")]
    public sealed class TowerDefinition : ScriptableObject
    {
        [Min(0)]
        [SerializeField] private int goldCost;

        [Min(0f)]
        [SerializeField] private float attackRange;

        [Min(1)]
        [SerializeField] private int damagePerShot;

        [Min(0.01f)]
        [SerializeField] private float attackIntervalSeconds;

        public int GoldCost => goldCost;
        public float AttackRange => attackRange;
        public int DamagePerShot => damagePerShot;
        public float AttackIntervalSeconds => attackIntervalSeconds;
    }
}
