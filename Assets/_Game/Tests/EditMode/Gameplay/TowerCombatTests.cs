using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using TowerDefense.Game.Gameplay.Towers;

namespace TowerDefense.Game.Tests.EditMode.Gameplay
{
    public class TowerTargetingLogicTests
    {
        [Test]
        public void TryGetTarget_PicksHighestProgressEnemyInRange()
        {
            var candidates = new List<CandidateEnemy>
            {
                new CandidateEnemy { Position = new Vector3(0, 0, 1), Progress = 0.2f, IsDead = false },
                new CandidateEnemy { Position = new Vector3(0, 0, 2), Progress = 0.8f, IsDead = false },
                new CandidateEnemy { Position = new Vector3(0, 0, 3), Progress = 0.5f, IsDead = false },
            };

            bool found = TowerTargetingLogic.TryGetTarget(Vector3.zero, 10f, candidates, out int bestIndex);

            Assert.That(found, Is.True);
            Assert.That(bestIndex, Is.EqualTo(1));
        }

        [Test]
        public void TryGetTarget_IgnoresOutOfRangeEnemies()
        {
            var candidates = new List<CandidateEnemy>
            {
                new CandidateEnemy { Position = new Vector3(0, 0, 100), Progress = 0.9f, IsDead = false },
                new CandidateEnemy { Position = new Vector3(0, 0, 1), Progress = 0.1f, IsDead = false },
            };

            bool found = TowerTargetingLogic.TryGetTarget(Vector3.zero, 5f, candidates, out int bestIndex);

            Assert.That(found, Is.True);
            Assert.That(bestIndex, Is.EqualTo(1));
        }

        [Test]
        public void TryGetTarget_IgnoresDeadEnemies()
        {
            var candidates = new List<CandidateEnemy>
            {
                new CandidateEnemy { Position = new Vector3(0, 0, 1), Progress = 0.9f, IsDead = true },
                new CandidateEnemy { Position = new Vector3(0, 0, 2), Progress = 0.3f, IsDead = false },
            };

            bool found = TowerTargetingLogic.TryGetTarget(Vector3.zero, 10f, candidates, out int bestIndex);

            Assert.That(found, Is.True);
            Assert.That(bestIndex, Is.EqualTo(1));
        }

        [Test]
        public void TryGetTarget_ReturnsFalseWhenNoValidTarget()
        {
            var candidates = new List<CandidateEnemy>
            {
                new CandidateEnemy { Position = new Vector3(0, 0, 100), Progress = 0.5f, IsDead = false },
                new CandidateEnemy { Position = new Vector3(0, 0, 1), Progress = 0.5f, IsDead = true },
            };

            bool found = TowerTargetingLogic.TryGetTarget(Vector3.zero, 5f, candidates, out int bestIndex);

            Assert.That(found, Is.False);
        }

        [Test]
        public void TryGetTarget_ReturnsFalseWhenCandidateListIsEmpty()
        {
            var candidates = new List<CandidateEnemy>();

            bool found = TowerTargetingLogic.TryGetTarget(Vector3.zero, 10f, candidates, out int bestIndex);

            Assert.That(found, Is.False);
        }

        [Test]
        public void TryGetTarget_AllOutOfRange_ReturnsFalse()
        {
            var candidates = new List<CandidateEnemy>
            {
                new CandidateEnemy { Position = new Vector3(0, 0, 50), Progress = 0.9f, IsDead = false },
                new CandidateEnemy { Position = new Vector3(0, 0, 60), Progress = 0.8f, IsDead = false },
            };

            bool found = TowerTargetingLogic.TryGetTarget(Vector3.zero, 5f, candidates, out int bestIndex);

            Assert.That(found, Is.False);
        }
    }

    public class TowerAttackLogicTests
    {
        [Test]
        public void CanFire_StartsFalse()
        {
            var attack = new TowerAttackLogic(1f);

            Assert.That(attack.CanFire, Is.False);
        }

        [Test]
        public void CanFire_TrueAfterCooldownElapses()
        {
            var attack = new TowerAttackLogic(1f);

            attack.Tick(1f);

            Assert.That(attack.CanFire, Is.True);
        }

        [Test]
        public void CanFire_FalseBeforeCooldownElapses()
        {
            var attack = new TowerAttackLogic(1f);

            attack.Tick(0.5f);

            Assert.That(attack.CanFire, Is.False);
        }

        [Test]
        public void ConsumeShot_ResetsCooldown()
        {
            var attack = new TowerAttackLogic(1f);
            attack.Tick(1f);

            attack.ConsumeShot();

            Assert.That(attack.CanFire, Is.False);
        }

        [Test]
        public void ConsumeShot_RequiresFullCooldownAgain()
        {
            var attack = new TowerAttackLogic(1f);
            attack.Tick(1f);
            attack.ConsumeShot();

            attack.Tick(0.5f);
            Assert.That(attack.CanFire, Is.False);

            attack.Tick(0.5f);
            Assert.That(attack.CanFire, Is.True);
        }

        [Test]
        public void Tick_PartialTicksAccumulateCorrectly()
        {
            var attack = new TowerAttackLogic(1f);

            attack.Tick(0.3f);
            attack.Tick(0.3f);
            attack.Tick(0.3f);

            Assert.That(attack.CanFire, Is.False);

            attack.Tick(0.1f);

            Assert.That(attack.CanFire, Is.True);
        }

        [Test]
        public void Tick_ZeroDeltaTime_DoesNotAdvance()
        {
            var attack = new TowerAttackLogic(1f);

            attack.Tick(0f);

            Assert.That(attack.CanFire, Is.False);
        }
    }
}
