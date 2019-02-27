using Pliant.Grammars;

namespace Pliant.Builders
{
    public sealed class StartProductionSettingModel : SettingModel
    {
        public const string SettingKey = "start";

        public StartProductionSettingModel(ProductionModel productionModel)
            : base(SettingKey, productionModel.LeftHandSide.NonTerminal.Value)
        {
        }

        public StartProductionSettingModel(QualifiedName fullyQualifiedName)
            : base(SettingKey, fullyQualifiedName.FullName)
        {
        }
    }
}