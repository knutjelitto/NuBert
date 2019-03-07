using System;
using System.Collections.Generic;
using System.IO;
using Pliant.Forest;

namespace Pliant.Tests.Common.Forest
{
    public class LoggingForestNodeVisitor : IForestNodeVisitor
    {
        public LoggingForestNodeVisitor(TextWriter streamWriter)
        {
            this.visited = new HashSet<IForestNode>();
            this.writer = streamWriter;
        }

        public void Visit(ITokenForestNode tokenNode)
        {
            this.visited.Add(tokenNode);
        }

        public void Visit(AndForestNode andNode)
        {
            foreach (var child in andNode.Children)
            {
                child.Accept(this);
            }
        }

        public void Visit(IIntermediateForestNode node)
        {
            if (!this.visited.Add(node))
            {
                return;
            }

            for (var i = 0; i < node.Children.Count; i++)
            {
                var child = node.Children[i];
                Visit(child);
            }
        }

        public void Visit(ISymbolForestNode node)
        {
            if (!this.visited.Add(node))
            {
                return;
            }

            for (var a = 0; a < node.Children.Count; a++)
            {
                PrintNode(node);
                this.writer.Write(" ->");
                var andNode = node.Children[a];
                foreach (var child in andNode.Children)
                {
                    PrintNode(child);
                }

                this.writer.WriteLine();
            }

            foreach (var child in node.Children)
            {
                Visit(child);
            }
        }

        private void PrintNode(IForestNode node)
        {
            switch (node)
            {
                case IIntermediateForestNode intermediate:
                    if (intermediate.Children.Count > 1)
                    {
                        throw new Exception("Intermediate node has more children than expected. ");
                    }

                    var flatList = GetFlattenedList(intermediate);
                    for (var i = 0; i < flatList.Count; i++)
                    {
                        this.writer.Write(" ");
                        PrintNode(flatList[i]);
                    }

                    break;

                case ISymbolForestNode symbolForestNode:
                    var symbolForestNodeString = GetSymbolNodeString(symbolForestNode);
                    this.writer.Write(" ");
                    this.writer.Write(symbolForestNodeString);
                    break;

                case ITokenForestNode tokenForestNode:
                    var tokenForestNodeString = GetTokenNodeString(tokenForestNode);
                    this.writer.Write(" ");
                    this.writer.Write(tokenForestNodeString);
                    break;
            }
        }


        private static IList<IForestNode> GetFlattenedList(IIntermediateForestNode intermediate)
        {
            var children = new List<IForestNode>();
            for (var a = 0; a < intermediate.Children.Count; a++)
            {
                var andNode = intermediate.Children[a];
                foreach (var child in andNode.Children)
                {
                    switch (child)
                    {
                        case IIntermediateForestNode intermediateChild:
                            var childList = GetFlattenedList(intermediateChild);
                            children.AddRange(childList);
                            break;
                        default:
                            children.Add(child);
                            break;
                    }
                }
            }

            return children;
        }

        private static string GetSymbolNodeString(ISymbolForestNode node)
        {
            return $"({node.Symbol}, {node.Origin}, {node.Location})";
        }

        private static string GetIntermediateNodeString(IIntermediateForestNode node)
        {
            return $"({node.DottedRule}, {node.Origin}, {node.Location})";
        }

        private static string GetTokenNodeString(ITokenForestNode node)
        {
            return $"('{node.Token.Value}', {node.Location})";
        }

        private readonly HashSet<IForestNode> visited;
        private readonly TextWriter writer;
    }
}