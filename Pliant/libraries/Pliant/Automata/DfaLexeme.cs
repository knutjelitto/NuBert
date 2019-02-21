using System.Text;
using Pliant.Tokens;
using Pliant.Utilities;

namespace Pliant.Automata
{
    public class DfaLexeme : Lexeme
    {
        public DfaLexeme(DfaLexerRule dfaLexerRule, int position)
            : base(dfaLexerRule, position)
        {
            this.captureBuilder = ObjectPoolExtensions.Allocate(SharedPools.Default<StringBuilder>());
            this.currentState = dfaLexerRule.StartState;
        }

        public override string Value
        {
            get
            {
                if (IsStringBuilderAllocated())
                {
                    DeallocateStringBuilderAndAssignCapture();
                }

                return this.capture;
            }
        }

        public override bool IsAccepted()
        {
            return this.currentState.IsFinal;
        }

        public override bool Scan(char c)
        {
            foreach (var edge in this.currentState.Transitions)
            {
                if (edge.Terminal.IsMatch(c))
                {
                    if (!IsStringBuilderAllocated())
                    {
                        ReallocateStringBuilderFromCapture();
                    }

                    this.currentState = edge.Target;
                    this.captureBuilder.Append(c);
                    return true;
                }
            }

            return false;
        }

        private void DeallocateStringBuilderAndAssignCapture()
        {
            this.capture = this.captureBuilder.ToString();
            SharedPools.Default<StringBuilder>().ClearAndFree(this.captureBuilder);
            this.captureBuilder = null;
        }

        private bool IsStringBuilderAllocated()
        {
            return this.captureBuilder != null;
        }

        private void ReallocateStringBuilderFromCapture()
        {
            this.captureBuilder = ObjectPoolExtensions.Allocate(SharedPools.Default<StringBuilder>());
            if (!string.IsNullOrWhiteSpace(this.capture))
            {
                this.captureBuilder.Append(this.capture);
            }
        }

        private string capture;
        private StringBuilder captureBuilder;

        private DfaState currentState;
    }
}