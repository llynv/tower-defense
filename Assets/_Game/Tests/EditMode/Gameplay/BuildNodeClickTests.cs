using NUnit.Framework;
using UnityEngine;
using TowerDefense.Game.Core;
using TowerDefense.Game.Gameplay.Building;

namespace TowerDefense.Game.Tests.EditMode.Gameplay
{
    public class BuildNodeClickLogicTests
    {
        [Test]
        public void ShouldProcess_HasSelectionAndBuildPhase_ReturnsTrue()
        {
            bool result = BuildNodeClickLogic.ShouldProcess(true, MatchState.BuildPhase);

            Assert.That(result, Is.True);
        }

        [Test]
        public void ShouldProcess_NoSelection_ReturnsFalse()
        {
            bool result = BuildNodeClickLogic.ShouldProcess(false, MatchState.BuildPhase);

            Assert.That(result, Is.False);
        }

        [Test]
        public void ShouldProcess_WaveRunning_ReturnsFalse()
        {
            bool result = BuildNodeClickLogic.ShouldProcess(true, MatchState.WaveRunning);

            Assert.That(result, Is.False);
        }

        [Test]
        public void ShouldProcess_Victory_ReturnsFalse()
        {
            bool result = BuildNodeClickLogic.ShouldProcess(true, MatchState.Victory);

            Assert.That(result, Is.False);
        }

        [Test]
        public void ShouldProcess_Defeat_ReturnsFalse()
        {
            bool result = BuildNodeClickLogic.ShouldProcess(true, MatchState.Defeat);

            Assert.That(result, Is.False);
        }

        [Test]
        public void ShouldProcess_NoSelectionAndWaveRunning_ReturnsFalse()
        {
            bool result = BuildNodeClickLogic.ShouldProcess(false, MatchState.WaveRunning);

            Assert.That(result, Is.False);
        }

        [Test]
        public void FindClosestNode_SingleNodeWithinRange_ReturnsIndex()
        {
            var clickPos = new Vector3(1f, 0f, 0f);
            var nodePositions = new[] { new Vector3(1.5f, 0f, 0f) };

            int result = BuildNodeClickLogic.FindClosestNode(clickPos, nodePositions, 1f);

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void FindClosestNode_SingleNodeOutOfRange_ReturnsNegativeOne()
        {
            var clickPos = new Vector3(0f, 0f, 0f);
            var nodePositions = new[] { new Vector3(5f, 0f, 0f) };

            int result = BuildNodeClickLogic.FindClosestNode(clickPos, nodePositions, 1f);

            Assert.That(result, Is.EqualTo(-1));
        }

        [Test]
        public void FindClosestNode_MultipleNodes_ReturnsClosest()
        {
            var clickPos = new Vector3(0f, 0f, 0f);
            var nodePositions = new[]
            {
                new Vector3(3f, 0f, 0f),
                new Vector3(0.5f, 0f, 0f),
                new Vector3(2f, 0f, 0f)
            };

            int result = BuildNodeClickLogic.FindClosestNode(clickPos, nodePositions, 5f);

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void FindClosestNode_EmptyArray_ReturnsNegativeOne()
        {
            var clickPos = new Vector3(0f, 0f, 0f);
            var nodePositions = new Vector3[0];

            int result = BuildNodeClickLogic.FindClosestNode(clickPos, nodePositions, 5f);

            Assert.That(result, Is.EqualTo(-1));
        }

        [Test]
        public void FindClosestNode_ExactlyAtMaxDistance_ReturnsIndex()
        {
            var clickPos = new Vector3(0f, 0f, 0f);
            var nodePositions = new[] { new Vector3(1f, 0f, 0f) };

            int result = BuildNodeClickLogic.FindClosestNode(clickPos, nodePositions, 1f);

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void FindClosestNode_AllOutOfRange_ReturnsNegativeOne()
        {
            var clickPos = new Vector3(0f, 0f, 0f);
            var nodePositions = new[]
            {
                new Vector3(5f, 0f, 0f),
                new Vector3(6f, 0f, 0f),
                new Vector3(7f, 0f, 0f)
            };

            int result = BuildNodeClickLogic.FindClosestNode(clickPos, nodePositions, 1f);

            Assert.That(result, Is.EqualTo(-1));
        }
    }
}