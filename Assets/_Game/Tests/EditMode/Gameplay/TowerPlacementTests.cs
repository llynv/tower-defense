using NUnit.Framework;
using TowerDefense.Game.Core;
using TowerDefense.Game.Gameplay.Building;

namespace TowerDefense.Game.Tests.EditMode.Gameplay
{
    public class BuildNodeOccupancyTests
    {
        [Test]
        public void IsOccupied_StartsUnoccupied()
        {
            var node = new BuildNodeOccupancy();

            Assert.That(node.IsOccupied, Is.False);
        }

        [Test]
        public void Occupy_MarksAsOccupied()
        {
            var node = new BuildNodeOccupancy();

            bool result = node.Occupy();

            Assert.That(result, Is.True);
            Assert.That(node.IsOccupied, Is.True);
        }

        [Test]
        public void Occupy_SecondCall_ReturnsFalse()
        {
            var node = new BuildNodeOccupancy();
            node.Occupy();

            bool result = node.Occupy();

            Assert.That(result, Is.False);
        }
    }

    public class TowerPlacementLogicTests
    {
        [Test]
        public void TryPlace_UnoccupiedAndSufficientGold_ReturnsTrue()
        {
            var resources = new PlayerResourcesLogic(100, 10);
            var placement = new TowerPlacementLogic(resources);
            var node = new BuildNodeOccupancy();

            bool result = placement.TryPlace(node, 50);

            Assert.That(result, Is.True);
        }

        [Test]
        public void TryPlace_Success_DeductsGold()
        {
            var resources = new PlayerResourcesLogic(100, 10);
            var placement = new TowerPlacementLogic(resources);
            var node = new BuildNodeOccupancy();

            placement.TryPlace(node, 50);

            Assert.That(resources.Gold, Is.EqualTo(50));
        }

        [Test]
        public void TryPlace_Success_MarksNodeOccupied()
        {
            var resources = new PlayerResourcesLogic(100, 10);
            var placement = new TowerPlacementLogic(resources);
            var node = new BuildNodeOccupancy();

            placement.TryPlace(node, 50);

            Assert.That(node.IsOccupied, Is.True);
        }

        [Test]
        public void TryPlace_OccupiedNode_ReturnsFalse()
        {
            var resources = new PlayerResourcesLogic(100, 10);
            var placement = new TowerPlacementLogic(resources);
            var node = new BuildNodeOccupancy();
            node.Occupy();

            bool result = placement.TryPlace(node, 50);

            Assert.That(result, Is.False);
        }

        [Test]
        public void TryPlace_OccupiedNode_DoesNotDeductGold()
        {
            var resources = new PlayerResourcesLogic(100, 10);
            var placement = new TowerPlacementLogic(resources);
            var node = new BuildNodeOccupancy();
            node.Occupy();

            placement.TryPlace(node, 50);

            Assert.That(resources.Gold, Is.EqualTo(100));
        }

        [Test]
        public void TryPlace_InsufficientGold_ReturnsFalse()
        {
            var resources = new PlayerResourcesLogic(10, 10);
            var placement = new TowerPlacementLogic(resources);
            var node = new BuildNodeOccupancy();

            bool result = placement.TryPlace(node, 50);

            Assert.That(result, Is.False);
        }

        [Test]
        public void TryPlace_InsufficientGold_DoesNotMarkOccupied()
        {
            var resources = new PlayerResourcesLogic(10, 10);
            var placement = new TowerPlacementLogic(resources);
            var node = new BuildNodeOccupancy();

            placement.TryPlace(node, 50);

            Assert.That(node.IsOccupied, Is.False);
        }

        [Test]
        public void TryPlace_InsufficientGold_DoesNotDeductGold()
        {
            var resources = new PlayerResourcesLogic(10, 10);
            var placement = new TowerPlacementLogic(resources);
            var node = new BuildNodeOccupancy();

            placement.TryPlace(node, 50);

            Assert.That(resources.Gold, Is.EqualTo(10));
        }

        [Test]
        public void TryPlace_NegativeCost_ReturnsFalse()
        {
            var resources = new PlayerResourcesLogic(100, 10);
            var placement = new TowerPlacementLogic(resources);
            var node = new BuildNodeOccupancy();

            bool result = placement.TryPlace(node, -10);

            Assert.That(result, Is.False);
            Assert.That(resources.Gold, Is.EqualTo(100));
            Assert.That(node.IsOccupied, Is.False);
        }

        [Test]
        public void TryPlace_ZeroCost_Succeeds()
        {
            var resources = new PlayerResourcesLogic(0, 10);
            var placement = new TowerPlacementLogic(resources);
            var node = new BuildNodeOccupancy();

            bool result = placement.TryPlace(node, 0);

            Assert.That(result, Is.True);
            Assert.That(node.IsOccupied, Is.True);
        }
    }
}
