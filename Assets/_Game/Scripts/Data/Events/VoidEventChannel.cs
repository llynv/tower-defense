using System;
using UnityEngine;

namespace TowerDefense.Game.Data.Events
{
    [CreateAssetMenu(menuName = "Tower Defense/Events/Void Event Channel")]
    public sealed class VoidEventChannel : ScriptableObject
    {
        private event Action Raised;

        public void RaiseEvent()
        {
            Raised?.Invoke();
        }

        public void RegisterListener(Action listener)
        {
            Raised += listener;
        }

        public void UnregisterListener(Action listener)
        {
            Raised -= listener;
        }
    }
}
