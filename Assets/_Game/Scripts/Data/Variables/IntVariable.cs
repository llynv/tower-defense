using System;
using UnityEngine;

namespace TowerDefense.Game.Data.Variables
{
    [CreateAssetMenu(menuName = "Tower Defense/Variables/Int")]
    public sealed class IntVariable : ScriptableObject
    {
        [SerializeField] private int value;

        public int Value => value;

        public event Action<int> OnValueChanged;

        public void SetValue(int newValue)
        {
            if (value == newValue)
                return;

            value = newValue;
            OnValueChanged?.Invoke(value);
        }

        public void ApplyChange(int amount)
        {
            if (amount == 0)
                return;

            value += amount;
            OnValueChanged?.Invoke(value);
        }
    }
}
