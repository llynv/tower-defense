using UnityEngine;
using TowerDefense.Game.Data;
using TowerDefense.Game.Data.Variables;

namespace TowerDefense.Game.UI
{
    public sealed class BuildMenuController : MonoBehaviour
    {
        [SerializeField] private TowerBuildOption[] buildOptions;
        [SerializeField] private SelectionState selectionState;
        [SerializeField] private IntVariable goldVariable;
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private GameObject buttonPrefab;

        private BuildMenuControllerLogic logic;

        private void Start()
        {
            logic = new BuildMenuControllerLogic(buildOptions);
            PopulateButtons();
        }

        private void PopulateButtons()
        {
            if (buttonPrefab == null || buttonContainer == null)
                return;

            for (int i = 0; i < logic.OptionCount; i++)
            {
                TowerBuildOption option = logic.GetOption(i);
                GameObject buttonGo = Instantiate(buttonPrefab, buttonContainer);
                var buildButton = buttonGo.GetComponent<TowerBuildButton>();

                if (buildButton != null)
                    buildButton.Initialize(option.definition, option.towerPrefab,
                        selectionState, goldVariable);
            }
        }
    }
}
