using UnityEngine;

namespace TowerDefense.Game.Map
{
    public sealed class SpawnPoint : MonoBehaviour
    {
        public Vector3 Position => transform.position;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.3f);
        }
    }
}
