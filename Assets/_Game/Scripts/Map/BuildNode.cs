using UnityEngine;
using TowerDefense.Game.Gameplay.Building;

namespace TowerDefense.Game.Map
{
    public sealed class BuildNode : MonoBehaviour
    {
        [SerializeField] private Transform anchorPoint;

        private readonly BuildNodeOccupancy occupancy = new();

        public Vector3 PlacementPosition => anchorPoint != null ? anchorPoint.position : transform.position;
        public BuildNodeOccupancy Occupancy => occupancy;

        private void OnDrawGizmos()
        {
            Gizmos.color = occupancy.IsOccupied
                ? new Color(1f, 0f, 0f, 0.5f)
                : new Color(0f, 1f, 0f, 0.5f);
            Gizmos.DrawWireCube(PlacementPosition, new Vector3(0.8f, 0.8f, 0f));
        }
    }
}
