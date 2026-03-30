using NUnit.Framework;
using TowerDefense.Game.Presentation.Sorting;

namespace TowerDefense.Game.Tests.PlayMode.Presentation
{
    public class PrototypeLaneSmokeTests
    {
        [Test]
        public void YSortAdapter_TypeExists()
        {
            Assert.That(typeof(YSortAdapter), Is.Not.Null);
        }

        [Test]
        public void SortingGroupBinder_TypeExists()
        {
            Assert.That(typeof(SortingGroupBinder), Is.Not.Null);
        }
    }
}