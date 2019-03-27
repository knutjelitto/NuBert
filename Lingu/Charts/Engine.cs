using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Lingu.Grammars;

namespace Lingu.Charts
{
    public class Engine
    {
        public Engine(Grammar grammar)
        {
            Grammar = grammar;
            DottedRules = new DottedRuleFactory(Grammar.Productions);
            EarleyItems = new EarleyItemFactory(DottedRules);
            Location = 0;
            Chart = new Chart();

            Initialize();
        }

        public Grammar Grammar { get; }

        public Chart Chart { get; }

        public int Location { get; private set; }

        public DottedRuleFactory DottedRules { get; }

        public EarleyItemFactory EarleyItems { get; }

        public bool IsAccepted =>
            Chart.Current.Completions.Any(completion => completion.Origin == 0 && completion.Head.Equals(Grammar.Start));

        public bool Pulse(IToken token)
        {
            ScanPass(Location, token);

            return Recognize();
        }

        private bool Recognize()
        {
            var tokenRecognized = Chart.Count > Location + 1;

            if (!tokenRecognized)
            {
                return false;
            }

            Location++;
            ReductionPass(Location);

            return true;
        }

        private void ScanPass(int location, IToken token)
        {
            var earleySet = Chart[location];

            foreach (var scanState in earleySet.Terminals)
            {
                Scan(scanState, location, token);
            }
        }

        private void Scan(TerminalItem scan, int location, IToken token)
        {
            var currentSymbol = scan.Dotted.PostDot as Terminal;

            if (token.IsFrom(currentSymbol))
            {
                var dottedRule = DottedRules.GetNext(scan.Dotted);
                if (Chart.Contains(location + 1, dottedRule, scan.Origin))
                {
                    return;
                }

                var nextState = EarleyItems.NewState(dottedRule, scan.Origin);

                if (Chart.Add(location + 1, nextState))
                {
                    //LogScan(location + 1, nextState, token);
                }
            }
        }

        public bool Pulse(IReadOnlyList<IToken> tokens)
        {
            foreach (var token in tokens)
            {
                ScanPass(Location, token);
            }

            return Recognize();
        }

        private void Initialize()
        {
            foreach (var startProduction in Grammar.ProductionsForStart())
            {
                var startState = EarleyItems.NewState(startProduction, 0, 0);

                if (Chart.Add(0, startState))
                {
                    //Log(startLogName, 0, startState);
                }
            }

            ReductionPass(Location);
        }

        private void ReductionPass(int location)
        {
            var earleySet = Chart[location];
            var resume = true;

            var p = 0;
            var c = 0;

            while (resume)
            {
                // is there a new completion?
                if (c < earleySet.Completions.Count)
                {
                    var completion = earleySet.Completions[c++];
                    Complete(completion, location);
                }
                // is there a new prediction?
                else if (p < earleySet.Nonterminals.Count)
                {
                    var evidence = earleySet.Nonterminals[p++];
                    Predict(evidence, location);
                }
                else
                {
                    resume = false;
                }
            }
        }

        private void Complete(CompletedItem completed, int location)
        {
            EarleyComplete(completed, location);
        }

        private void EarleyComplete(CompletedItem completed, int location)
        {
            var earleySet = Chart[completed.Origin];

            // Predictions may grow
            var p = 0;
            for (; p < earleySet.Nonterminals.Count; ++p)
            {
                var prediction = earleySet.Nonterminals[p];
                if (!prediction.IsSource(completed.Head))
                {
                    continue;
                }

                var dottedRule = DottedRules.GetNext(prediction.Dotted);
                var origin = prediction.Origin;

                if (Chart.Contains(location, dottedRule, origin))
                {
                    continue;
                }

                var nextState = EarleyItems.NewState(dottedRule, origin);

                if (Chart.Add(location, nextState))
                {
                    //Log(completeLogName, location, nextState);
                }
            }
        }

        private void Predict(NonterminalItem evidence, int location)
        {
            var nonTerminal = evidence.Dotted.PostDot as Nonterminal;
            Debug.Assert(nonTerminal != null);
            var rulesForNonTerminal = Grammar.ProductionsFor(nonTerminal);

            foreach (var production in rulesForNonTerminal)
            {
                PredictProduction(location, production);
            }

            var isNullable = Grammar.IsTransitiveNullable(nonTerminal);
            if (isNullable)
            {
                PredictAycockHorspool(evidence, location);
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
                //Log(predictionLogName, location, predictedState);
            }
        }

        private void PredictAycockHorspool(NonterminalItem evidence, int location)
        {
            var dottedRule = DottedRules.GetNext(evidence.Dotted);

            if (Chart.Contains(location, dottedRule, evidence.Origin))
            {
                return;
            }

            var aycockHorspoolState = EarleyItems.NewState(dottedRule, evidence.Origin);

            if (Chart.Add(location, aycockHorspoolState))
            {
                //Log(predictionLogName, location, aycockHorspoolState);
            }
        }
    }
}