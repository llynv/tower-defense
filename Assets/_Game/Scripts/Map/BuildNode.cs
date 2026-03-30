using UnityEngine;

namespace TowerDefense.Game.Map
{
    public sealed class BuildNode : MonoBehaviour
    {
        [SerializeField] private Transform anchorPoint;

        public Vector3 PlacementPosition => anchorPoint != null ? anchorPoint.position : transform.position;

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
            Gizmos.DrawWireCube(PlacementPosition, new Vector3(0.8f, 0.8f, 0f));
        }
    }
}
