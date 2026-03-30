using UnityEngine;

namespace TowerDefense.Game.Map
{
    public sealed class GoalPoint : MonoBehaviour
    {
        public Vector3 Position => transform.position;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, 0.3f);
        }
    }
}
