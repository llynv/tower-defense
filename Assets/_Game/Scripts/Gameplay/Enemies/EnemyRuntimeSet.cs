using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense.Game.Gameplay.Enemies
{
    [CreateAssetMenu(menuName = "Tower Defense/Runtime/Enemy Runtime Set")]
    public sealed class EnemyRuntimeSet : ScriptableObject
    {
        private readonly List<EnemyMover> items = new();

        public IReadOnlyList<EnemyMover> Items => items;
        public int Count => items.Count;

        public void Add(EnemyMover enemy)
        {
            if (!items.Contains(enemy))
                items.Add(enemy);
        }

        public void Remove(EnemyMover enemy)
        {
            items.Remove(enemy);
        }

        public void Clear()
        {
            items.Clear();
        }
    }
}
