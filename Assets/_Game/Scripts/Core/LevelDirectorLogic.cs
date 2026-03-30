namespace TowerDefense.Game.Core
{
    public sealed class LevelDirectorLogic
    {
        public MatchState CurrentState { get; private set; } = MatchState.BuildPhase;
        public int CurrentWaveIndex { get; private set; }
        public int TotalWaveCount { get; private set; }

        public void SetTotalWaves(int total)
        {
            TotalWaveCount = total;
        }

        public void StartWave()
        {
            if (CurrentState != MatchState.BuildPhase)
                return;

            CurrentWaveIndex++;
            CurrentState = MatchState.WaveRunning;
        }

        public void CompleteWave(bool hasMoreWaves)
        {
            if (CurrentState != MatchState.WaveRunning)
                return;

            CurrentState = hasMoreWaves ? MatchState.BuildPhase : MatchState.Victory;
        }

        public void TriggerDefeat()
        {
            if (CurrentState == MatchState.Victory || CurrentState == MatchState.Defeat)
                return;

            CurrentState = MatchState.Defeat;
        }
    }
}
