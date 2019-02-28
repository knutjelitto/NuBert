using Pliant.Grammars;

namespace Pliant.Builders
{
    public sealed class StartProductionSettingModel : SettingModel
    {
        public const string SettingKey = "start";

        public StartProductionSettingModel(ProductionModel productionModel)
            : base(productionModel.LeftHandSide.NonTerminal.Value)
        {
        }

        public StartProductionSettingModel(QualifiedName fullyQualifiedName)
            : base(fullyQualifiedName.FullName)
        {
        }
    }
}