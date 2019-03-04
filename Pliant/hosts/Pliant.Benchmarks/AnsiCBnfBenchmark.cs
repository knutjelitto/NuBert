using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using Pliant.Bnf;
using Pliant.Grammars;
using Pliant.Runtime;

namespace Pliant.Benchmarks
{
    [Config(typeof(PliantBenchmarkConfig))]
    public class AnsiCBnfBenchmark
    {
        [Benchmark]
        public bool Parse()
        {
            var parseEngine = new ParseEngine(this.grammar);
            var parseRunner = new ParseRunner(parseEngine, this.sampleBnf);

            return parseRunner.RunToEnd();
        }

        [GlobalSetup]
        public void Setup()
        {
            this.sampleBnf = File.ReadAllText(
                Path.Combine(Environment.CurrentDirectory, "AnsiC.bnf"));
            this.grammar = new BnfGrammar();
        }

        private Grammar grammar;
        private string sampleBnf;
    }
}