using UnityEngine;

namespace TowerDefense.Game.Data.Definitions
{
    [CreateAssetMenu(menuName = "Tower Defense/Definitions/Wave")]
    public sealed class WaveDefinition : ScriptableObject
    {
        [SerializeField] private EnemyDefinition enemy;

        [Min(1)]
        [SerializeField] private int enemyCount;

        [Min(0.01f)]
        [SerializeField] private float spawnIntervalSeconds;

        public EnemyDefinition Enemy => enemy;
        public int EnemyCount => enemyCount;
        public float SpawnIntervalSeconds => spawnIntervalSeconds;
    }
}
