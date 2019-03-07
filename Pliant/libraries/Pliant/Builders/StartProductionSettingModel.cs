using Pliant.Grammars;

namespace Pliant.Builders
{
    public sealed class StartProductionSettingModel : SettingModel
    {
        public const string SettingKey = ":start";

        public StartProductionSettingModel(ProductionModel productionModel)
            : this(productionModel.LeftHandSide.NonTerminal.QualifiedName)
        {
        }

        public StartProductionSettingModel(QualifiedName qualifiedName)
            : base(qualifiedName)
        {
        }
    }
}