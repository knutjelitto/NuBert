namespace Pliant.Automata
{
    public class IntervalTree
    {
        private IntervalNode _root;

        public bool Add(char character)
        {
            return Add(character, character);
        }

        public bool Add(char min, char max)
        {
            if (this._root == null)
            {
                this._root = new IntervalNode();
                return true;
            }
            return false;
        }
        
        public bool Contains(char character)
        {
            return false;
        }

        public int Count { get; private set; }
        
        private class IntervalNode
        {
            public char Key { get; set; }

            public Range Range { get; set; }

            public IntervalNode Left { get; set; }
            
            public IntervalNode Right { get; set; }            
        }
    }
}
