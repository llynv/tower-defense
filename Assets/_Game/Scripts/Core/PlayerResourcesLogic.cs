using UnityEngine;

namespace TowerDefense.Game.Core
{
    public sealed class PlayerResourcesLogic
    {
        public PlayerResourcesLogic(int gold, int lives)
        {
            Gold = gold;
            Lives = lives;
        }

        public int Gold { get; private set; }
        public int Lives { get; private set; }
        public bool IsDefeated => Lives <= 0;

        public bool TrySpendGold(int amount)
        {
            if (Gold < amount)
                return false;

            Gold -= amount;
            return true;
        }

        public void AddGold(int amount)
        {
            Gold += amount;
        }

        public void LoseLife()
        {
            Lives = Mathf.Max(0, Lives - 1);
        }
    }
}
