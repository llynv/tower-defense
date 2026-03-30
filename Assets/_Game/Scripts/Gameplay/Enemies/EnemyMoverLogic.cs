using UnityEngine;

namespace TowerDefense.Game.Gameplay.Enemies
{
    public sealed class EnemyMoverLogic
    {
        private readonly float moveSpeed;
        private readonly float pathLength;

        public EnemyMoverLogic(float moveSpeed, float pathLength)
        {
            this.moveSpeed = moveSpeed;
            this.pathLength = pathLength;
        }

        public float Progress { get; private set; }

        public bool HasReachedEnd => Progress >= 1f;

        public void Tick(float deltaTime)
        {
            if (pathLength <= 0f)
                return;

            Progress = Mathf.Clamp01(Progress + (moveSpeed * deltaTime / pathLength));
        }
    }
}
