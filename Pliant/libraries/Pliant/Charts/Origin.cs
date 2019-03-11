using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Pliant.Charts
{
    public struct Location
    {
        public readonly int Index;

        public Location(int index)
        {
            this.Index = index;
        }

        public static Location Zero => new Location(0);

        public static Location operator +(Location location, int distance)
        {
            Debug.Assert(distance >= 0);
            return new Location(location.Index + distance);
        }

        public Location Next => new Location(this.Index + 1);

        public Location Pred => new Location(this.Index - 1);

        public bool HasPred => this.Index > 0;

        public Location Advance(int lenght) => new Location(this.Index + lenght);

        public bool NonNegative => this.Index >= 0;

        public bool Positive => this.Index > 0;
    }
}
