using System.Collections.Generic;
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

        [SerializeField] private WaveEntry[] entries;

        public EnemyDefinition Enemy => enemy;
        public int EnemyCount => enemyCount;
        public float SpawnIntervalSeconds => spawnIntervalSeconds;

        public List<EnemyDefinition> BuildSpawnList()
        {
            var list = new List<EnemyDefinition>();

            if (entries != null && entries.Length > 0)
            {
                foreach (var entry in entries)
                {
                    for (int i = 0; i < entry.Count; i++)
                        list.Add(entry.Enemy);
                }
            }
            else if (enemy != null)
            {
                for (int i = 0; i < enemyCount; i++)
                    list.Add(enemy);
            }

            return list;
        }
    }
}
