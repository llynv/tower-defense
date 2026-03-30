namespace TowerDefense.Game.UI
{
    public sealed class EnemyHealthBarLogic
    {
        public float ComputeFillAmount(int currentHealth, int maxHealth)
        {
            if (maxHealth <= 0)
                return 0f;

            return (float)currentHealth / maxHealth;
        }

        public bool ShouldShow(int currentHealth, int maxHealth)
        {
            return currentHealth < maxHealth && currentHealth > 0;
        }
    }
}
