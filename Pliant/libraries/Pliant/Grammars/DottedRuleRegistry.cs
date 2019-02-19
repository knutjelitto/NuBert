using System.Collections.Generic;
using Pliant.Collections;
using Pliant.Utilities;

namespace Pliant.Grammars
{
    public class DottedRuleRegistry : IDottedRuleRegistry
    {
        private readonly Dictionary<IProduction, Dictionary<int, IDottedRule>> _dottedRuleIndex;

        public DottedRuleRegistry()
        {
            this._dottedRuleIndex = new Dictionary<IProduction, Dictionary<int, IDottedRule>>(
                new HashCodeEqualityComparer<IProduction>());
        }

        public void Register(IDottedRule dottedRule)
        {
            var positionIndex = this._dottedRuleIndex.AddOrGetExisting(dottedRule.Production);
            positionIndex[dottedRule.Position] = dottedRule;
        }

        public IDottedRule Get(IProduction production, int position)
        {
            if (!this._dottedRuleIndex.TryGetValue(production, out var positionIndex))
            {
                return null;
            }

            if (!positionIndex.TryGetValue(position, out var dottedRule))
            {
                return null;
            }

            return dottedRule;
        }

        public IDottedRule GetNext(IDottedRule dottedRule)
        {
            return Get(dottedRule.Production, dottedRule.Position + 1);
        }        
    }
}
