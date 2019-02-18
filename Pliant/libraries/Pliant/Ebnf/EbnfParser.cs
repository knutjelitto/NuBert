﻿using Pliant.Forest;
using Pliant.Runtime;
using Pliant.Tree;
using System;

namespace Pliant.Ebnf
{
    public class EbnfParser
    {
        public EbnfDefinition Parse(string ebnf)
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

            var parseForest = parseEngine.GetParseForestRootNode();

            var parseTree = new InternalTreeNode(
                    parseForest,
                    new SelectFirstChildDisambiguationAlgorithm());

            var ebnfVisitor = new EbnfVisitor();
            parseTree.Accept(ebnfVisitor);
            return ebnfVisitor.Definition;            
        }
    }
}