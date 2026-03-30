using System;
using UnityEngine;
using TowerDefense.Game.Data.Definitions;

namespace TowerDefense.Game.Data
{
    [Serializable]
    public struct TowerBuildOption
    {
        public TowerDefinition definition;
        public GameObject towerPrefab;
    }
}
