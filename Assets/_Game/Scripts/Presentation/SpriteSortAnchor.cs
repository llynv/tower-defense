using UnityEngine;

namespace TowerDefense.Game.Presentation
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class SpriteSortAnchor : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer targetRenderer;
        [SerializeField] private float yOffset;

        private void Reset()
        {
            targetRenderer = GetComponent<SpriteRenderer>();
        }

        private void LateUpdate()
        {
            float sortY = transform.position.y + yOffset;
            targetRenderer.sortingOrder = Mathf.RoundToInt(-sortY * 100f);
        }
    }
}
