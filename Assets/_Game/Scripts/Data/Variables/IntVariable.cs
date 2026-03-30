using UnityEngine;

namespace TowerDefense.Game.Data.Variables
{
    [CreateAssetMenu(menuName = "Tower Defense/Variables/Int")]
    public sealed class IntVariable : ScriptableObject
    {
        [SerializeField] private int value;

        public int Value => value;

        public void SetValue(int newValue)
        {
            value = newValue;
        }

        public void ApplyChange(int amount)
        {
            value += amount;
        }
    }
}
