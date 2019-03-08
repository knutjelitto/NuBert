using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using Pliant.Grammars;
using Pliant.Json;
using Pliant.Runtime;

namespace Pliant.Benchmarks
{
    [Config(typeof(PliantBenchmarkConfig))]
    public class JsonBenchmark
    {
        [Benchmark]
        public bool ParseEngineParse()
        {
            return RunParse(new ParseEngine(this.grammar), this.jsonSource);
        }

        [GlobalSetup]
        public void Setup()
        {
            this.jsonSource = File.ReadAllText(
                Path.Combine(Environment.CurrentDirectory, "10000.json"));
            this.grammar = new JsonGrammar();
        }

        private bool RunParse(IParseEngine parseEngine, string input)
        {
            var parseRunner = new ParseRunner(parseEngine, input);

            return parseRunner.RunToEnd();
        }

        private Grammar grammar;
        private string jsonSource;
    }
}