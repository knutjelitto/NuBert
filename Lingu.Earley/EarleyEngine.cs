using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Lingu.Grammars;

namespace Lingu.Earley
{
    //
    // http://loup-vaillant.fr/tutorials/earley-parsing/
    //

    public class EarleyEngine
    {
        public EarleyEngine(Grammar grammar)
        {
            Grammar = grammar;
            DottedRules = new DottedRuleFactory(Grammar.Productions);
            EarleyItems = new EarleyStateFactory(DottedRules);
            Location = 0;
            Chart = new Chart();

            Initialize();
        }

        public Chart Chart { get; }

        public Grammar Grammar { get; }

        public bool IsAccepted =>
            Chart.Current.CompletionStates.Any(completion => completion.Origin == 0 && completion.Head.Equals(Grammar.Start));

        private DottedRuleFactory DottedRules { get; }

        private EarleyStateFactory EarleyItems { get; }

        private int Location { get; set; }

        public bool Pulse(IToken token)
        {
            ScanPass(token);

            return Recognize();
        }

        public bool Pulse(IEnumerable<IToken> tokens)
        {
            foreach (var token in tokens)
            {
                ScanPass(token);
            }

            return Recognize();
        }

        private void Complete(CompletedState completed)
        {
            EarleyComplete(completed, Location);
        }

        private void EarleyComplete(CompletedState completed, int location)
        {
            var originSet = Chart[completed.Origin];

            // Predictions may grow
            var p = 0;
            for (; p < originSet.NonterminalStates.Count; ++p)
            {
                var nonterminal = originSet.NonterminalStates[p];
                if (!nonterminal.IsSource(completed.Head))
                {
                    continue;
                }

                var dottedRule = nonterminal.DottedRule.Next;
                var origin = nonterminal.Origin;

                if (Chart.Contains(location, dottedRule, origin))
                {
                    continue;
                }

                var nextState = EarleyItems.NewState(dottedRule, origin);

                if (Chart.Add(location, nextState))
                {
                    Log(compName, location, nextState);
                }
            }
        }

        private void Initialize()
        {
            foreach (var startProduction in Grammar.ProductionsForStart())
            {
                var startState = EarleyItems.NewState(startProduction, 0, 0);

                if (Chart.Add(0, startState))
                {
                    Log(startName, 0, startState);
                }
            }

            ReductionPass();
        }

        private void Log(string name, int origin, EarleyState item)
        {
            Console.WriteLine($"{name}: [{origin}] {item}");
        }

        private void LogScan(int origin, EarleyState item, IToken token)
        {
            Console.WriteLine($"{scanName}: [{origin}] {item} {token}");
        }

        private void Predict(NonterminalState evidence)
        {
            var nonTerminal = evidence.DottedRule.PostDot as Nonterminal;
            Debug.Assert(nonTerminal != null);
            var productionsForNonterminal = Grammar.ProductionsFor(nonTerminal);

            foreach (var production in productionsForNonterminal)
            {
                PredictProduction(Location, production);
            }

            if (nonTerminal.IsNullable)
            {
                PredictAycockHorspool(evidence, Location);
            }
        }

        private void PredictAycockHorspool(NonterminalState evidence, int location)
        {
            var dottedRule = evidence.DottedRule.Next;

            if (Chart.Contains(location, dottedRule, evidence.Origin))
            {
                return;
            }

            var aycockHorspoolState = EarleyItems.NewState(dottedRule, evidence.Origin);

            if (Chart.Add(location, aycockHorspoolState))
            {
                Log(predName, location, aycockHorspoolState);
            }
        }

        private void PredictProduction(int location, Production production)
        {
            var dottedRule = DottedRules.Get(production, 0);
            if (Chart.Contains(location, dottedRule, 0))
            {
                return;
            }

            var predictedState = EarleyItems.NewState(dottedRule, location);

            if (Chart.Add(location, predictedState))
            {
                Log(predName, location, predictedState);
            }
        }

        private bool Recognize()
        {
            var tokenRecognized = Chart.Count > Location + 1;

            if (!tokenRecognized)
            {
                return false;
            }

            Location++;
            ReductionPass();

            return true;
        }

        private void ReductionPass()
        {
            var earleySet = Chart[Location];
            var resume = true;

            var p = 0;
            var c = 0;

            while (resume)
            {
                // is there a new completion?
                if (c < earleySet.CompletionStates.Count)
                {
                    var completion = earleySet.CompletionStates[c++];
                    Complete(completion);
                }
                // is there a new prediction?
                else if (p < earleySet.NonterminalStates.Count)
                {
                    var evidence = earleySet.NonterminalStates[p++];
                    Predict(evidence);
                }
                else
                {
                    resume = false;
                }
            }
        }

        private void Scan(TerminalState terminal, IToken token)
        {
            var currentSymbol = terminal.DottedRule.PostDot as Terminal;

            if (token.IsFrom(currentSymbol))
            {
                var dottedRule = terminal.DottedRule.Next;
                if (Chart.Contains(Location + 1, dottedRule, terminal.Origin))
                {
                    return;
                }

                var nextState = EarleyItems.NewState(dottedRule, terminal.Origin);

                if (Chart.Add(Location + 1, nextState))
                {
                    LogScan(Location + 1, nextState, token);
                }
            }
        }

        private void ScanPass(IToken token)
        {
            var earleySet = Chart[Location];

            foreach (var terminalItem in earleySet.TerminalStates)
            {
                Scan(terminalItem, token);
            }
        }

        private const string startName = "strt";
        private const string predName = "pred";
        private const string compName = "comp";
        private const string scanName = "scan";
    }
}