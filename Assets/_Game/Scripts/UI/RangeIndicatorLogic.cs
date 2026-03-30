using UnityEngine;

namespace TowerDefense.Game.UI
{
    public sealed class RangeIndicatorLogic
    {
        public Vector3 ComputeScale(float attackRange)
        {
            float diameter = attackRange * 2f;
            return new Vector3(diameter, diameter, 1f);
        }
    }
}