using NUnit.Framework;
using TowerDefense.Game.Core;

namespace TowerDefense.Game.Tests.EditMode.Core
{
    public class LevelDirectorTests
    {
        [Test]
        public void InitialState_IsBuildPhase()
        {
            var director = new LevelDirectorLogic();

            Assert.That(director.CurrentState, Is.EqualTo(MatchState.BuildPhase));
        }

        [Test]
        public void StartWave_TransitionsToWaveRunning()
        {
            var director = new LevelDirectorLogic();

            director.StartWave();

            Assert.That(director.CurrentState, Is.EqualTo(MatchState.WaveRunning));
        }

        [Test]
        public void CompleteWave_TransitionsToBuildPhase()
        {
            var director = new LevelDirectorLogic();
            director.StartWave();

            director.CompleteWave(hasMoreWaves: true);

            Assert.That(director.CurrentState, Is.EqualTo(MatchState.BuildPhase));
        }

        [Test]
        public void CompleteWave_NoMoreWaves_TransitionsToVictory()
        {
            var director = new LevelDirectorLogic();
            director.StartWave();

            director.CompleteWave(hasMoreWaves: false);

            Assert.That(director.CurrentState, Is.EqualTo(MatchState.Victory));
        }

        [Test]
        public void TriggerDefeat_TransitionsToDefeat()
        {
            var director = new LevelDirectorLogic();

            director.TriggerDefeat();

            Assert.That(director.CurrentState, Is.EqualTo(MatchState.Defeat));
        }

        [Test]
        public void StartWave_DuringWaveRunning_DoesNothing()
        {
            var director = new LevelDirectorLogic();
            director.StartWave();

            director.StartWave();

            Assert.That(director.CurrentState, Is.EqualTo(MatchState.WaveRunning));
        }

        [Test]
        public void StartWave_AfterVictory_DoesNothing()
        {
            var director = new LevelDirectorLogic();
            director.StartWave();
            director.CompleteWave(hasMoreWaves: false);

            director.StartWave();

            Assert.That(director.CurrentState, Is.EqualTo(MatchState.Victory));
        }
    }

    public class PlayerResourcesTests
    {
        [Test]
        public void Constructor_SetsInitialValues()
        {
            var resources = new PlayerResourcesLogic(20, 10);

            Assert.That(resources.Gold, Is.EqualTo(20));
            Assert.That(resources.Lives, Is.EqualTo(10));
        }

        [Test]
        public void TrySpendGold_SufficientFunds_ReturnsTrue()
        {
            var resources = new PlayerResourcesLogic(20, 10);

            bool result = resources.TrySpendGold(15);

            Assert.That(result, Is.True);
            Assert.That(resources.Gold, Is.EqualTo(5));
        }

        [Test]
        public void TrySpendGold_InsufficientFunds_ReturnsFalse()
        {
            var resources = new PlayerResourcesLogic(5, 10);

            bool result = resources.TrySpendGold(10);

            Assert.That(result, Is.False);
            Assert.That(resources.Gold, Is.EqualTo(5));
        }

        [Test]
        public void AddGold_IncreasesGold()
        {
            var resources = new PlayerResourcesLogic(10, 5);

            resources.AddGold(7);

            Assert.That(resources.Gold, Is.EqualTo(17));
        }

        [Test]
        public void LoseLife_DecrementsLives()
        {
            var resources = new PlayerResourcesLogic(10, 5);

            resources.LoseLife();

            Assert.That(resources.Lives, Is.EqualTo(4));
        }

        [Test]
        public void LoseLife_DoesNotGoBelowZero()
        {
            var resources = new PlayerResourcesLogic(10, 1);

            resources.LoseLife();
            resources.LoseLife();

            Assert.That(resources.Lives, Is.EqualTo(0));
        }

        [Test]
        public void IsDefeated_TrueWhenLivesReachZero()
        {
            var resources = new PlayerResourcesLogic(10, 1);

            resources.LoseLife();

            Assert.That(resources.IsDefeated, Is.True);
        }

        [Test]
        public void IsDefeated_FalseWhenLivesAboveZero()
        {
            var resources = new PlayerResourcesLogic(10, 5);

            Assert.That(resources.IsDefeated, Is.False);
        }
    }
}
