using Pliant.Runtime;
using Pliant.Tree;
using System;

namespace Pliant.Ebnf
{
    public class EbnfParser
    {
        public IEbnfDefinition Parse(string ebnf)
        {
            var grammar = new EbnfGrammar();
            var parseEngine = new ParseEngine(
                grammar, 
                new ParseEngineOptions());
            var parseRunner = new ParseRunner(parseEngine, ebnf);
            while (!parseRunner.EndOfStream())
            {
                if (!parseRunner.Read())
                {
                    throw new Exception(
                        $"Unable to parse Ebnf. Error at line {parseRunner.Line}, column {parseRunner.Column}.");
                }
            }
            if (!parseEngine.IsAccepted())
            {
                throw new Exception(
                    $"Ebnf parse not accepted. Error at line {parseRunner.Line}, column {parseRunner.Column}.");
            }

            var parseTree = parseEngine.GetParseTree();

            var ebnfVisitor = new EbnfVisitor();
            parseTree.Accept(ebnfVisitor);
            return ebnfVisitor.Definition;            
        }
    }
}
