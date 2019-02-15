using System.Text;
using Pliant.Tokens;
using Pliant.Utilities;

namespace Pliant.Automata
{
    public class DfaLexeme : LexemeBase<IDfaLexerRule>
    {
        public DfaLexeme(IDfaLexerRule dfaLexerRule, int position)
            : base(dfaLexerRule, position)
        {
            this._stringBuilder = SharedPools.Default<StringBuilder>().AllocateAndClear();
            this._currentState = dfaLexerRule.Start;
        }

        public override string Value
        {
            get
            {
                if (IsStringBuilderAllocated())
                {
                    DeallocateStringBuilderAndAssignCapture();
                }

                return this._capture;
            }
        }

        public override bool IsAccepted()
        {
            return this._currentState.IsFinal;
        }

        public override void Reset()
        {
            this._capture = null;
            if (IsStringBuilderAllocated())
            {
                this._stringBuilder.Clear();
            }

            this._currentState = ConcreteLexerRule.Start;
        }

        public override bool Scan(char c)
        {
            for (var e = 0; e < this._currentState.Transitions.Count; e++)
            {
                var edge = this._currentState.Transitions[e];
                if (edge.Terminal.IsMatch(c))
                {
                    if (!IsStringBuilderAllocated())
                    {
                        ReallocateStringBuilderFromCapture();
                    }

                    this._currentState = edge.Target;
                    this._stringBuilder.Append(c);
                    return true;
                }
            }

            return false;
        }

        private void DeallocateStringBuilderAndAssignCapture()
        {
            this._capture = this._stringBuilder.ToString();
            SharedPools.Default<StringBuilder>().ClearAndFree(this._stringBuilder);
            this._stringBuilder = null;
        }

        private bool IsStringBuilderAllocated()
        {
            return this._stringBuilder != null;
        }

        private void ReallocateStringBuilderFromCapture()
        {
            this._stringBuilder = SharedPools.Default<StringBuilder>().AllocateAndClear();
            if (!string.IsNullOrWhiteSpace(this._capture))
            {
                this._stringBuilder.Append(this._capture);
            }
        }

        private string _capture;

        private IDfaState _currentState;
        private StringBuilder _stringBuilder;
    }
}