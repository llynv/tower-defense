using UnityEngine;
using UnityEngine.Rendering;

namespace TowerDefense.Game.Presentation.Sorting
{
    [RequireComponent(typeof(SortingGroup))]
    public sealed class SortingGroupBinder : MonoBehaviour
    {
        [SerializeField] private SortingGroup sortingGroup;
        [SerializeField] private string sortingLayerName = "Default";

        private void Reset()
        {
            sortingGroup = GetComponent<SortingGroup>();
        }

        private void Awake()
        {
            if (sortingGroup == null)
                sortingGroup = GetComponent<SortingGroup>();
            sortingGroup.sortingLayerName = sortingLayerName;
        }
    }
}
