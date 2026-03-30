using System;
using UnityEngine;
using TowerDefense.Game.Data.Definitions;

namespace TowerDefense.Game.Data.Variables
{
    [CreateAssetMenu(menuName = "Tower Defense/Variables/Selection State")]
    public sealed class SelectionState : ScriptableObject
    {
        private TowerDefinition selectedTower;
        private GameObject towerPrefab;

        public TowerDefinition SelectedTower => selectedTower;
        public GameObject TowerPrefab => towerPrefab;
        public bool HasSelection => selectedTower != null;

        public event Action SelectionChanged;

        public void Select(TowerDefinition definition, GameObject prefab)
        {
            selectedTower = definition;
            towerPrefab = prefab;
            SelectionChanged?.Invoke();
        }

        public void Clear()
        {
            selectedTower = null;
            towerPrefab = null;
            SelectionChanged?.Invoke();
        }
    }
}
