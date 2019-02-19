using Pliant.Grammars;
using Pliant.Utilities;
using System.Collections.Generic;

namespace Pliant.Tokens
{
    public class LexemeFactoryRegistry : ILexemeFactoryRegistry
    {
        private readonly Dictionary<LexerRuleType, ILexemeFactory> _registry;        

        public LexemeFactoryRegistry()
        {
            this._registry = new Dictionary<LexerRuleType, ILexemeFactory>(
                new HashCodeEqualityComparer<LexerRuleType>());
        }

        public ILexemeFactory Get(LexerRuleType lexerRuleType)
        {
            if (!this._registry.TryGetValue(lexerRuleType, out var lexemeFactory))
            {
                return null;
            }

            return lexemeFactory;
        }

        public void Register(ILexemeFactory factory)
        {
            this._registry.Add(factory.LexerRuleType, factory);
        }
    }
}