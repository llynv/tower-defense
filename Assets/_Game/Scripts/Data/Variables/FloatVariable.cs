using UnityEngine;

namespace TowerDefense.Game.Data.Variables
{
    [CreateAssetMenu(menuName = "Tower Defense/Variables/Float")]
    public sealed class FloatVariable : ScriptableObject
    {
        [SerializeField] private float value;

        public float Value => value;

        public void SetValue(float newValue)
        {
            value = newValue;
        }

        public void ApplyChange(float amount)
        {
            value += amount;
        }
    }
}
