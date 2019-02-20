﻿using Pliant.Diagnostics;
using Pliant.Forest;
using Pliant.Grammars;

namespace Pliant.Charts
{
    public abstract class State
    {
        protected State(DottedRule dottedRule, int origin)
        {
            Assert.IsNotNull(dottedRule, nameof(dottedRule));
            Assert.IsGreaterThanEqualToZero(origin, nameof(origin));
            DottedRule = dottedRule;
            Origin = origin;
        }

        public DottedRule DottedRule { get; }

        public int Origin { get; }

        public IForestNode ParseNode { get; set; }
    }
}