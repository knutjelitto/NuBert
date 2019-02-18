using Pliant.Diagnostics;
using Pliant.Forest;
using Pliant.Grammars;

namespace Pliant.Charts
{
    public abstract class State //: IState
    {
        protected State(IDottedRule dottedRule, int origin)
        {
            Assert.IsNotNull(dottedRule, nameof(dottedRule));
            Assert.IsGreaterThanEqualToZero(origin, nameof(origin));
            DottedRule = dottedRule;
            Origin = origin;
        }

        public IDottedRule DottedRule { get; }

        public int Origin { get; }

        public IForestNode ParseNode { get; set; }

        public abstract StateType StateType { get; }
    }
}