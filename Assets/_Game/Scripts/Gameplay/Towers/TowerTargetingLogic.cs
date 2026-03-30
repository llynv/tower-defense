using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense.Game.Gameplay.Towers
{
    public struct CandidateEnemy
    {
        public Vector3 Position;
        public float Progress;
        public bool IsDead;
    }

    public static class TowerTargetingLogic
    {
        public static bool TryGetTarget(
            Vector3 towerPosition,
            float range,
            IReadOnlyList<CandidateEnemy> candidates,
            out int bestIndex)
        {
            bestIndex = -1;
            float bestProgress = -1f;
            float rangeSqr = range * range;

            for (int i = 0; i < candidates.Count; i++)
            {
                CandidateEnemy c = candidates[i];

                if (c.IsDead)
                    continue;

                if ((c.Position - towerPosition).sqrMagnitude > rangeSqr)
                    continue;

                if (c.Progress > bestProgress)
                {
                    bestProgress = c.Progress;
                    bestIndex = i;
                }
            }

            return bestIndex >= 0;
        }
    }
}
