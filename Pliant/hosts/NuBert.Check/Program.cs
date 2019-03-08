using System;
using System.IO;
using Pliant.Ebnf;
using Pliant.Grammars;
using Pliant.Runtime;
using Pliant.Tree;

namespace NuBert.Check
{
    internal class Program
    {
        // ReSharper disable once UnusedParameter.Local
        private static void Main(string[] args)
        {
            var p = new Program();

            p.Expressions();

            p.AnyKey();
        }

        private void Expressions()
        {
            try
            {
                var result = new ExpressionParser().Parse("12*-23");

                var dumper = new TreeDumpVisitor();

                result.Accept(dumper);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private class ExpressionParser
        {
            public IInternalTreeNode Parse(string source)
            {
                var grammar = MakeGrammar();

                var parseEngine = new ParseEngine(
                    grammar,
                    new ParseEngineOptions(loggingEnabled:true));
                var parseRunner = new ParseRunner(parseEngine, source);

                while (!parseRunner.EndOfStream())
                {
                    if (!parseRunner.Read())
                    {
                        throw new Exception(
                            $"Unable to parse expression. Error at line {parseRunner.Line+1}, column {parseRunner.Column+1}.");
                    }

                    Console.WriteLine("-----");
                    Console.ReadKey(true);

                }
                if (!parseEngine.IsAccepted())
                {
                    throw new Exception(
                        $"expression parse not accepted. Error at line {parseRunner.Line+1}, column {parseRunner.Column+1}.");
                }

                return parseEngine.GetParseTree();
            }

            private Grammar MakeGrammar()
            {
                var parser = new EbnfParser();
                var source = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "Expression.nug"));

                var definition = parser.Parse(source);

                var grammar = new EbnfGrammarGenerator().Generate(definition);

                return grammar;
            }
        }

        private void AnyKey()
        {
            Console.Write("any key ...");
            Console.ReadKey(true);
        }
    }
}
