using UnityEngine;

namespace TowerDefense.Game.Presentation.Sorting
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class YSortAdapter : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer targetRenderer;
        [SerializeField] private float multiplier = 100f;

        private void Reset()
        {
            targetRenderer = GetComponent<SpriteRenderer>();
        }

        private void LateUpdate()
        {
            targetRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * multiplier);
        }
    }
}