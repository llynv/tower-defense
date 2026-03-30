using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense.Game.Map
{
    public sealed class LanePathAuthoring : MonoBehaviour
    {
        [SerializeField] private Transform[] waypoints;

        public LanePath BuildPath()
        {
            var positions = new List<Vector3>(waypoints.Length);

            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] != null)
                    positions.Add(waypoints[i].position);
            }

            return new LanePath(positions);
        }

        private void OnDrawGizmosSelected()
        {
            if (waypoints == null || waypoints.Length < 2)
                return;

            Gizmos.color = Color.cyan;

            for (int i = 0; i < waypoints.Length - 1; i++)
            {
                if (waypoints[i] != null && waypoints[i + 1] != null)
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }

            Gizmos.color = Color.yellow;
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] != null)
                    Gizmos.DrawWireSphere(waypoints[i].position, 0.15f);
            }
        }
    }
}
