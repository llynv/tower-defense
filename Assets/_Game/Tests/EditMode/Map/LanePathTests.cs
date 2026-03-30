using NUnit.Framework;
using UnityEngine;
using TowerDefense.Game.Map;

namespace TowerDefense.Game.Tests.EditMode.Map
{
    public class LanePathTests
    {
        [Test]
        public void EvaluatePosition_AtZero_ReturnsFirstPoint()
        {
            var path = new LanePath(new[] { Vector3.zero, Vector3.right, new Vector3(2f, 1f, 0f) });

            Assert.That(path.EvaluatePosition(0f), Is.EqualTo(Vector3.zero));
        }

        [Test]
        public void EvaluatePosition_AtOne_ReturnsLastPoint()
        {
            var path = new LanePath(new[] { Vector3.zero, Vector3.right, new Vector3(2f, 1f, 0f) });

            Assert.That(path.EvaluatePosition(1f), Is.EqualTo(new Vector3(2f, 1f, 0f)));
        }

        [Test]
        public void EvaluatePosition_AtHalf_InterpolatesBetweenSegments()
        {
            var path = new LanePath(new[] { Vector3.zero, new Vector3(2f, 0f, 0f) });

            Assert.That(path.EvaluatePosition(0.5f), Is.EqualTo(new Vector3(1f, 0f, 0f)));
        }

        [Test]
        public void EvaluatePosition_MultiSegment_DistributesEvenly()
        {
            var path = new LanePath(new[] { Vector3.zero, new Vector3(1f, 0f, 0f), new Vector3(1f, 1f, 0f) });

            Vector3 midpoint = path.EvaluatePosition(0.5f);

            Assert.That(midpoint.x, Is.EqualTo(1f).Within(0.001f));
            Assert.That(midpoint.y, Is.EqualTo(0f).Within(0.001f));
        }

        [Test]
        public void EvaluatePosition_BeyondOne_ClampsToLastPoint()
        {
            var path = new LanePath(new[] { Vector3.zero, Vector3.right });

            Assert.That(path.EvaluatePosition(1.5f), Is.EqualTo(Vector3.right));
        }

        [Test]
        public void EvaluatePosition_BelowZero_ClampsToFirstPoint()
        {
            var path = new LanePath(new[] { Vector3.zero, Vector3.right });

            Assert.That(path.EvaluatePosition(-0.5f), Is.EqualTo(Vector3.zero));
        }

        [Test]
        public void TotalLength_TwoPoints_ReturnsSegmentLength()
        {
            var path = new LanePath(new[] { Vector3.zero, new Vector3(3f, 4f, 0f) });

            Assert.That(path.TotalLength, Is.EqualTo(5f).Within(0.001f));
        }

        [Test]
        public void TotalLength_MultipleSegments_ReturnsSumOfLengths()
        {
            var path = new LanePath(new[] { Vector3.zero, new Vector3(1f, 0f, 0f), new Vector3(1f, 1f, 0f) });

            Assert.That(path.TotalLength, Is.EqualTo(2f).Within(0.001f));
        }

        [Test]
        public void PointCount_ReturnsCorrectCount()
        {
            var path = new LanePath(new[] { Vector3.zero, Vector3.right, Vector3.up });

            Assert.That(path.PointCount, Is.EqualTo(3));
        }

        [Test]
        public void Constructor_SinglePoint_HasZeroLength()
        {
            var path = new LanePath(new[] { Vector3.zero });

            Assert.That(path.TotalLength, Is.EqualTo(0f));
            Assert.That(path.EvaluatePosition(0f), Is.EqualTo(Vector3.zero));
            Assert.That(path.EvaluatePosition(1f), Is.EqualTo(Vector3.zero));
        }

        [Test]
        public void EvaluatePosition_ThreeEqualSegments_QuarterProgress()
        {
            var path = new LanePath(new[]
            {
                Vector3.zero,
                new Vector3(1f, 0f, 0f),
                new Vector3(2f, 0f, 0f),
                new Vector3(3f, 0f, 0f)
            });

            Vector3 result = path.EvaluatePosition(0.25f);

            Assert.That(result.x, Is.EqualTo(0.75f).Within(0.001f));
        }
    }
}
