using UnityEngine;
using TowerDefense.Game.Core;

namespace TowerDefense.Game.Gameplay.Building
{
    public static class BuildNodeClickLogic
    {
        public static bool ShouldProcess(bool hasSelection, MatchState matchState)
        {
            return hasSelection && matchState == MatchState.BuildPhase;
        }

        public static int FindClosestNode(Vector3 worldPos, Vector3[] nodePositions, float maxDistance)
        {
            int closestIndex = -1;
            float closestDistSq = maxDistance * maxDistance;

            for (int i = 0; i < nodePositions.Length; i++)
            {
                float distSq = (nodePositions[i] - worldPos).sqrMagnitude;
                if (distSq <= closestDistSq)
                {
                    closestDistSq = distSq;
                    closestIndex = i;
                }
            }

            return closestIndex;
        }
    }
}