using System;
using UnityEngine;

namespace TowerDefense.Game.Data.Definitions
{
    [Serializable]
    public struct WaveEntry
    {
        [SerializeField] private EnemyDefinition enemy;
        [SerializeField] private int count;

        public EnemyDefinition Enemy => enemy;
        public int Count => count;

        public WaveEntry(EnemyDefinition enemy, int count)
        {
            this.enemy = enemy;
            this.count = count;
        }
    }
}