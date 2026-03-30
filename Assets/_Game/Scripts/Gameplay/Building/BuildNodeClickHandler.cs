using UnityEngine;
using UnityEngine.InputSystem;
using TowerDefense.Game.Core;
using TowerDefense.Game.Data.Variables;
using TowerDefense.Game.Map;

namespace TowerDefense.Game.Gameplay.Building
{
    public sealed class BuildNodeClickHandler : MonoBehaviour
    {
        [SerializeField] private SelectionState selectionState;
        [SerializeField] private MatchStateController matchStateController;
        [SerializeField] private TowerPlacer towerPlacer;
        [SerializeField] private BuildNode[] buildNodes;
        [SerializeField] private Camera sceneCamera;
        [SerializeField] private float snapDistance = 1f;

        private Vector3[] nodePositions;

        private void Start()
        {
            CacheNodePositions();
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;
            if (keyboard == null || mouse == null)
                return;

            if (keyboard.escapeKey.wasPressedThisFrame ||
                mouse.rightButton.wasPressedThisFrame)
            {
                selectionState.Clear();
                return;
            }

            if (!mouse.leftButton.wasPressedThisFrame)
                return;

            if (!BuildNodeClickLogic.ShouldProcess(
                    selectionState.HasSelection,
                    matchStateController.CurrentState))
                return;

            Vector3 worldPos = sceneCamera.ScreenToWorldPoint(mouse.position.ReadValue());
            worldPos.z = 0f;

            int index = BuildNodeClickLogic.FindClosestNode(worldPos, nodePositions, snapDistance);
            if (index < 0)
                return;

            bool placed = towerPlacer.TryPlaceAt(
                buildNodes[index],
                selectionState.SelectedTower,
                selectionState.TowerPrefab);

            if (placed)
                selectionState.Clear();
        }

        private void CacheNodePositions()
        {
            if (buildNodes == null)
            {
                nodePositions = new Vector3[0];
                return;
            }

            nodePositions = new Vector3[buildNodes.Length];
            for (int i = 0; i < buildNodes.Length; i++)
            {
                nodePositions[i] = buildNodes[i] != null
                    ? buildNodes[i].PlacementPosition
                    : Vector3.zero;
            }
        }
    }
}