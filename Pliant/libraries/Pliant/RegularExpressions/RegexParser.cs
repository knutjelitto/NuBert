using System;
using Pliant.Forest;
using Pliant.Runtime;
using Pliant.Tree;

namespace Pliant.RegularExpressions
{
    public class RegexParser
    {
        public Regex Parse(string regularExpression)
        {
            var grammar = new RegexGrammar();
            var parseEngine = new ParseEngine(grammar, new ParseEngineOptions());
            var parseRunner = new ParseRunner(parseEngine, regularExpression);
            while (!parseRunner.EndOfStream())
            {
                if (!parseRunner.Read())
                {
                    throw new Exception(
                        $"Unable to parse regular expression. Error at position {parseRunner.Position}.");
                }
            }

            if (!parseEngine.IsAccepted())
            {
                throw new Exception(
                    $"Error parsing regular expression. Error at position {parseRunner.Position}");
            }

            var parseTree = parseEngine.GetParseTree();

            var regexVisitor = new RegexVisitor();
            parseTree.Accept(regexVisitor);

            return regexVisitor.Regex;
        }
    }
}