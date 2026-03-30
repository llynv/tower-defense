using UnityEngine;
using TowerDefense.Game.Data.Definitions;
using TowerDefense.Game.Map;

namespace TowerDefense.Game.Gameplay.Enemies
{
    public sealed class EnemyMover : MonoBehaviour
    {
        private EnemyMoverLogic logic;
        private LanePath lanePath;

        public float Progress => logic?.Progress ?? 0f;
        public bool HasReachedEnd => logic?.HasReachedEnd ?? false;

        public void Initialize(EnemyDefinition definition, LanePath path)
        {
            lanePath = path;
            logic = new EnemyMoverLogic(definition.MoveSpeed, path.TotalLength);
        }

        private void Update()
        {
            if (logic == null || logic.HasReachedEnd)
                return;

            logic.Tick(Time.deltaTime);
            transform.position = lanePath.EvaluatePosition(logic.Progress);
        }
    }
}
