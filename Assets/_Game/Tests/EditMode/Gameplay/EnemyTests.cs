using NUnit.Framework;
using UnityEngine;
using TowerDefense.Game.Gameplay.Enemies;

namespace TowerDefense.Game.Tests.EditMode.Gameplay
{
    public class EnemyMoverTests
    {
        [Test]
        public void Tick_AdvancesProgressBySpeedTimesPathRatio()
        {
            var mover = new EnemyMoverLogic(2f, 10f);

            mover.Tick(0.5f);

            Assert.That(mover.Progress, Is.EqualTo(0.1f).Within(0.001f));
        }

        [Test]
        public void Tick_AtZeroSpeed_DoesNotAdvance()
        {
            var mover = new EnemyMoverLogic(0f, 10f);

            mover.Tick(1f);

            Assert.That(mover.Progress, Is.EqualTo(0f));
        }

        [Test]
        public void Tick_ClampsProgressToOne()
        {
            var mover = new EnemyMoverLogic(100f, 10f);

            mover.Tick(1f);

            Assert.That(mover.Progress, Is.EqualTo(1f));
        }

        [Test]
        public void HasReachedEnd_FalseAtStart()
        {
            var mover = new EnemyMoverLogic(2f, 10f);

            Assert.That(mover.HasReachedEnd, Is.False);
        }

        [Test]
        public void HasReachedEnd_TrueWhenProgressIsOne()
        {
            var mover = new EnemyMoverLogic(10f, 10f);

            mover.Tick(1f);

            Assert.That(mover.HasReachedEnd, Is.True);
        }

        [Test]
        public void Progress_StartsAtZero()
        {
            var mover = new EnemyMoverLogic(5f, 20f);

            Assert.That(mover.Progress, Is.EqualTo(0f));
        }

        [Test]
        public void Tick_MultipleCalls_AccumulatesProgress()
        {
            var mover = new EnemyMoverLogic(2f, 10f);

            mover.Tick(0.5f);
            mover.Tick(0.5f);

            Assert.That(mover.Progress, Is.EqualTo(0.2f).Within(0.001f));
        }
    }

    public class EnemyHealthLogicTests
    {
        [Test]
        public void TakeDamage_ReducesCurrentHealth()
        {
            var health = new EnemyHealthLogic(10);

            health.TakeDamage(3);

            Assert.That(health.CurrentHealth, Is.EqualTo(7));
        }

        [Test]
        public void TakeDamage_DoesNotGoBelowZero()
        {
            var health = new EnemyHealthLogic(5);

            health.TakeDamage(10);

            Assert.That(health.CurrentHealth, Is.EqualTo(0));
        }

        [Test]
        public void IsDead_FalseWhenHealthAboveZero()
        {
            var health = new EnemyHealthLogic(10);

            Assert.That(health.IsDead, Is.False);
        }

        [Test]
        public void IsDead_TrueWhenHealthReachesZero()
        {
            var health = new EnemyHealthLogic(5);

            health.TakeDamage(5);

            Assert.That(health.IsDead, Is.True);
        }

        [Test]
        public void CurrentHealth_StartsAtMaxHealth()
        {
            var health = new EnemyHealthLogic(15);

            Assert.That(health.CurrentHealth, Is.EqualTo(15));
        }
    }
}
