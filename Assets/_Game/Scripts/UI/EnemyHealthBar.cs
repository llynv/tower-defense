using UnityEngine;
using UnityEngine.UI;
using TowerDefense.Game.Gameplay.Enemies;

namespace TowerDefense.Game.UI
{
    public sealed class EnemyHealthBar : MonoBehaviour
    {
        [SerializeField] private GameObject barRoot;
        [SerializeField] private Image fillImage;

        private EnemyHealth enemyHealth;
        private EnemyHealthBarLogic logic;

        public void Initialize(EnemyHealth health)
        {
            enemyHealth = health;
            logic = new EnemyHealthBarLogic();

            enemyHealth.Damaged += OnDamaged;

            if (barRoot != null)
                barRoot.SetActive(false);
        }

        private void OnDestroy()
        {
            if (enemyHealth != null)
                enemyHealth.Damaged -= OnDamaged;
        }

        private void LateUpdate()
        {
            if (Camera.main == null)
                return;

            transform.rotation = Camera.main.transform.rotation;
        }

        private void OnDamaged(int current, int max)
        {
            bool show = logic.ShouldShow(current, max);

            if (barRoot != null)
                barRoot.SetActive(show);

            if (fillImage != null)
                fillImage.fillAmount = logic.ComputeFillAmount(current, max);
        }
    }
}
