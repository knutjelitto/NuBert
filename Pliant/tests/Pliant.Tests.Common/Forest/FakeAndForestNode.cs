using Pliant.Collections;
using Pliant.Forest;
using System.Collections.Generic;

namespace Pliant.Tests.Common.Forest
{
    public class FakeAndForestNode : AndForestNode
    {
        public FakeAndForestNode(params IForestNode[] children)
            : base(children)
        {
        }
    }
}
