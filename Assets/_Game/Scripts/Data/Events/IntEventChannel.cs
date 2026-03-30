using System;
using UnityEngine;

namespace TowerDefense.Game.Data.Events
{
    [CreateAssetMenu(menuName = "Tower Defense/Events/Int Event Channel")]
    public sealed class IntEventChannel : ScriptableObject
    {
        private event Action<int> Raised;

        public void RaiseEvent(int value)
        {
            Raised?.Invoke(value);
        }

        public void RegisterListener(Action<int> listener)
        {
            Raised += listener;
        }

        public void UnregisterListener(Action<int> listener)
        {
            Raised -= listener;
        }
    }
}
