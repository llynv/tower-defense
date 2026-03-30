using UnityEngine;
using TowerDefense.Game.Data.Events;

namespace TowerDefense.Game.UI
{
    public sealed class PauseButton : MonoBehaviour
    {
        [SerializeField] private VoidEventChannel pauseRequestedChannel;

        public void OnClick()
        {
            if (pauseRequestedChannel != null)
                pauseRequestedChannel.RaiseEvent();
        }
    }
}