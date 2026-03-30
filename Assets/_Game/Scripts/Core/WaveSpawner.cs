using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerDefense.Game.Data.Definitions;
using TowerDefense.Game.Data.Events;
using TowerDefense.Game.Gameplay.Enemies;
using TowerDefense.Game.Map;
using TowerDefense.Game.UI;

namespace TowerDefense.Game.Core
{
    public sealed class WaveSpawner : MonoBehaviour
    {
        [SerializeField] private LevelDefinition levelDefinition;
        [SerializeField] private LanePathAuthoring lanePathAuthoring;
        [SerializeField] private SpawnPoint spawnPoint;
        [SerializeField] private EnemyRuntimeSet enemyRuntimeSet;
        [SerializeField] private VoidEventChannel waveStartedChannel;

        [Header("Events Out")]
        [SerializeField] private MatchStateController matchStateController;

        private LanePath lanePath;
        private int spawnedCount;
        private int totalToSpawn;
        private int currentWaveIndex;
        private bool spawning;

        private void Awake()
        {
            lanePath = lanePathAuthoring.BuildPath();
            matchStateController.SetTotalWaves(levelDefinition.WaveCount);
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

            if (spawnedCount >= totalToSpawn && enemyRuntimeSet.Count == 0)
            {
                spawning = false;
                bool hasMoreWaves = currentWaveIndex < levelDefinition.WaveCount;
                matchStateController.NotifyWaveComplete(hasMoreWaves: hasMoreWaves);
            }
        }

        private void OnWaveStarted()
        {
            WaveDefinition wave = levelDefinition.GetWave(currentWaveIndex);
            if (wave == null)
                return;

            List<EnemyDefinition> spawnList = wave.BuildSpawnList();
            totalToSpawn = spawnList.Count;
            spawnedCount = 0;
            spawning = true;

            if (enemyRuntimeSet != null)
                enemyRuntimeSet.Clear();

            StartCoroutine(SpawnWaveCoroutine(spawnList, wave.SpawnIntervalSeconds));
            currentWaveIndex++;
        }

        private IEnumerator SpawnWaveCoroutine(List<EnemyDefinition> spawnList, float interval)
        {
            for (int i = 0; i < spawnList.Count; i++)
            {
                SpawnEnemy(spawnList[i]);
                spawnedCount++;

                if (i < spawnList.Count - 1)
                    yield return new WaitForSeconds(interval);
            }
        }

        private void SpawnEnemy(EnemyDefinition enemyDef)
        {
            Vector3 spawnPos = spawnPoint != null ? spawnPoint.Position : Vector3.zero;
            GameObject go = Instantiate(enemyDef.Prefab, spawnPos, Quaternion.identity);

            var mover = go.GetComponent<EnemyMover>();
            if (mover != null)
            {
                mover.Initialize(enemyDef, lanePath);

                if (enemyRuntimeSet != null)
                    enemyRuntimeSet.Add(mover);
            }

            var health = go.GetComponent<EnemyHealth>();
            if (health != null)
            {
                health.Initialize(enemyDef);

                var healthBar = go.GetComponentInChildren<EnemyHealthBar>();
                if (healthBar != null)
                    healthBar.Initialize(health);
            }
        }
    }
}
