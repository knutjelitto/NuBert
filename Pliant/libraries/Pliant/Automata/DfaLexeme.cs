using System.Text;
using Pliant.Tokens;
using Pliant.Utilities;

namespace Pliant.Automata
{
    public sealed class DfaLexeme : Lexeme
    {
        public DfaLexeme(DfaLexerRule lexerRule, int position)
            : base(lexerRule, position)
        {
            this.captureBuilder = ObjectPoolExtensions.Allocate(SharedPools.Default<StringBuilder>());
            this.currentState = lexerRule.Start;
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

        public override bool Scan(char character)
        {
            foreach (var transition in this.currentState.Transitions)
            {
                if (transition.Terminal.CanApply(character))
                {
                    if (!IsStringBuilderAllocated())
                    {
                        ReallocateStringBuilderFromCapture();
                    }

                    this.currentState = transition.Target;
                    this.captureBuilder.Append(character);
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