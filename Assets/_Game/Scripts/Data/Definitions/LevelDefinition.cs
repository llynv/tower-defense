using UnityEngine;

namespace TowerDefense.Game.Data.Definitions
{
    [CreateAssetMenu(menuName = "Tower Defense/Definitions/Level")]
    public sealed class LevelDefinition : ScriptableObject
    {
        [SerializeField] private WaveDefinition[] waves;
        [SerializeField] private int startingGold = 20;
        [SerializeField] private int startingLives = 10;

        public int WaveCount => waves != null ? waves.Length : 0;
        public int StartingGold => startingGold;
        public int StartingLives => startingLives;

        public WaveDefinition GetWave(int index)
        {
            if (waves == null || index < 0 || index >= waves.Length)
                return null;

            return waves[index];
        }
    }
}