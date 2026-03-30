using TowerDefense.Game.Data.Definitions;

namespace TowerDefense.Game.UI
{
    public readonly struct TowerStats
    {
        public readonly int damage;
        public readonly float range;
        public readonly float fireRate;

        public TowerStats(int damage, float range, float fireRate)
        {
            this.damage = damage;
            this.range = range;
            this.fireRate = fireRate;
        }
    }

    public sealed class TowerInfoPanelLogic
    {
        public TowerStats GetStats(TowerDefinition definition)
        {
            return new TowerStats(
                definition.DamagePerShot,
                definition.AttackRange,
                definition.AttackIntervalSeconds
            );
        }

        public bool IsUpgradeAvailable()
        {
            return false;
        }

        public bool IsSellAvailable()
        {
            return false;
        }

        public int ComputeSellCost(TowerDefinition definition)
        {
            return definition.GoldCost / 2;
        }
    }
}