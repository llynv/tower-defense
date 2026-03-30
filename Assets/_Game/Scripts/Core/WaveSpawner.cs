using System.Collections;
using UnityEngine;
using TowerDefense.Game.Data.Definitions;
using TowerDefense.Game.Data.Events;
using TowerDefense.Game.Gameplay.Enemies;
using TowerDefense.Game.Map;

namespace TowerDefense.Game.Core
{
    public sealed class WaveSpawner : MonoBehaviour
    {
        [SerializeField] private WaveDefinition waveDefinition;
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private LanePathAuthoring lanePathAuthoring;
        [SerializeField] private SpawnPoint spawnPoint;
        [SerializeField] private EnemyRuntimeSet enemyRuntimeSet;
        [SerializeField] private VoidEventChannel waveStartedChannel;

        [Header("Events Out")]
        [SerializeField] private MatchStateController matchStateController;

        private LanePath lanePath;
        private int spawnedCount;
        private bool spawning;

        private void Awake()
        {
            lanePath = lanePathAuthoring.BuildPath();
        }

        private void OnEnable()
        {
            if (waveStartedChannel != null)
                waveStartedChannel.RegisterListener(OnWaveStarted);
        }

        private void OnDisable()
        {
            if (waveStartedChannel != null)
                waveStartedChannel.UnregisterListener(OnWaveStarted);
        }

        private void Update()
        {
            if (!spawning || matchStateController.CurrentState != MatchState.WaveRunning)
                return;

            if (spawnedCount >= waveDefinition.EnemyCount && enemyRuntimeSet.Count == 0)
            {
                spawning = false;
                matchStateController.NotifyWaveComplete(hasMoreWaves: false);
            }
        }

        private void OnWaveStarted()
        {
            spawnedCount = 0;
            spawning = true;

            if (enemyRuntimeSet != null)
                enemyRuntimeSet.Clear();

            StartCoroutine(SpawnWaveCoroutine());
        }

        private IEnumerator SpawnWaveCoroutine()
        {
            for (int i = 0; i < waveDefinition.EnemyCount; i++)
            {
                SpawnEnemy();
                spawnedCount++;

                if (i < waveDefinition.EnemyCount - 1)
                    yield return new WaitForSeconds(waveDefinition.SpawnIntervalSeconds);
            }
        }

        private void SpawnEnemy()
        {
            Vector3 spawnPos = spawnPoint != null ? spawnPoint.Position : Vector3.zero;
            GameObject go = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

            var mover = go.GetComponent<EnemyMover>();
            if (mover != null)
            {
                mover.Initialize(waveDefinition.Enemy, lanePath);

                if (enemyRuntimeSet != null)
                    enemyRuntimeSet.Add(mover);
            }

            var health = go.GetComponent<EnemyHealth>();
            if (health != null)
                health.Initialize(waveDefinition.Enemy);
        }
    }
}
