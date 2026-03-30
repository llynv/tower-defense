using TowerDefense.Game.Data;

namespace TowerDefense.Game.UI
{
    public sealed class BuildMenuControllerLogic
    {
        private readonly TowerBuildOption[] options;

        public BuildMenuControllerLogic(TowerBuildOption[] options)
        {
            this.options = options;
        }

        public int OptionCount => options.Length;

        public TowerBuildOption GetOption(int index)
        {
            return options[index];
        }
    }
}
