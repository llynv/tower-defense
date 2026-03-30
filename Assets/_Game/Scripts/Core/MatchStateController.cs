using UnityEngine;
using TowerDefense.Game.Data.Events;
using TowerDefense.Game.Data.Variables;

namespace TowerDefense.Game.Core
{
    public sealed class MatchStateController : MonoBehaviour
    {
        [Header("Resources")]
        [SerializeField] private int startingGold = 20;
        [SerializeField] private int startingLives = 10;

        [Header("Variables")]
        [SerializeField] private IntVariable goldVariable;
        [SerializeField] private IntVariable livesVariable;
        [SerializeField] private IntVariable currentWaveVariable;
        [SerializeField] private IntVariable totalWaveVariable;

        [Header("Events In")]
        [SerializeField] private VoidEventChannel startWaveRequested;
        [SerializeField] private IntEventChannel enemyLeakedChannel;
        [SerializeField] private IntEventChannel goldEarnedChannel;

        [Header("Events Out")]
        [SerializeField] private VoidEventChannel waveStartedChannel;
        [SerializeField] private VoidEventChannel waveCompletedChannel;
        [SerializeField] private VoidEventChannel victoryChannel;
        [SerializeField] private VoidEventChannel defeatChannel;

        private LevelDirectorLogic director;
        private PlayerResourcesLogic resources;

        public MatchState CurrentState => director?.CurrentState ?? MatchState.BuildPhase;
        public PlayerResourcesLogic Resources => resources;

        public void SetTotalWaves(int total)
        {
            director.SetTotalWaves(total);
            if (totalWaveVariable != null)
                totalWaveVariable.SetValue(total);
        }

        private void Awake()
        {
            director = new LevelDirectorLogic();
            resources = new PlayerResourcesLogic(startingGold, startingLives);
            SyncVariables();
        }

        private void OnEnable()
        {
            if (startWaveRequested != null) startWaveRequested.RegisterListener(OnStartWaveRequested);
            if (enemyLeakedChannel != null) enemyLeakedChannel.RegisterListener(OnEnemyLeaked);
            if (goldEarnedChannel != null) goldEarnedChannel.RegisterListener(OnGoldEarned);
        }

        private void OnDisable()
        {
            if (startWaveRequested != null) startWaveRequested.UnregisterListener(OnStartWaveRequested);
            if (enemyLeakedChannel != null) enemyLeakedChannel.UnregisterListener(OnEnemyLeaked);
            if (goldEarnedChannel != null) goldEarnedChannel.UnregisterListener(OnGoldEarned);
        }

        public void NotifyWaveComplete(bool hasMoreWaves)
        {
            director.CompleteWave(hasMoreWaves);

            if (waveCompletedChannel != null)
                waveCompletedChannel.RaiseEvent();

            if (director.CurrentState == MatchState.Victory && victoryChannel != null)
                victoryChannel.RaiseEvent();
        }

        private void OnStartWaveRequested()
        {
            director.StartWave();

            if (director.CurrentState == MatchState.WaveRunning)
            {
                SyncWaveVariables();

                if (waveStartedChannel != null)
                    waveStartedChannel.RaiseEvent();
            }
        }

        private void OnEnemyLeaked(int count)
        {
            for (int i = 0; i < count; i++)
                resources.LoseLife();

            SyncVariables();

            if (resources.IsDefeated)
            {
                director.TriggerDefeat();

                if (defeatChannel != null)
                    defeatChannel.RaiseEvent();
            }
        }

        private void OnGoldEarned(int amount)
        {
            resources.AddGold(amount);
            SyncVariables();
        }

        private void SyncVariables()
        {
            if (goldVariable != null) goldVariable.SetValue(resources.Gold);
            if (livesVariable != null) livesVariable.SetValue(resources.Lives);
        }

        private void SyncWaveVariables()
        {
            if (currentWaveVariable != null)
                currentWaveVariable.SetValue(director.CurrentWaveIndex);
            if (totalWaveVariable != null)
                totalWaveVariable.SetValue(director.TotalWaveCount);
        }
    }
}
