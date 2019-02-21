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
            this._visited = new HashSet<IForestNode>();
            this._writer = streamWriter;
        }

        public void Visit(ITokenForestNode tokenNode)
        {
            this._visited.Add(tokenNode);
        }

        public void Visit(IAndForestNode andNode)
        {
            for (var i = 0; i < andNode.Children.Count; i++)
            {
                var child = andNode.Children[i];
                child.Accept(this);
            }
        }

        public void Visit(IIntermediateForestNode node)
        {
            if (!this._visited.Add(node))
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
            if (!this._visited.Add(node))
            {
                return;
            }

            for (var a = 0; a < node.Children.Count; a++)
            {
                PrintNode(node);
                this._writer.Write(" ->");
                var andNode = node.Children[a];
                for (var c = 0; c < andNode.Children.Count; c++)
                {
                    var child = andNode.Children[c];
                    PrintNode(child);
                }

                this._writer.WriteLine();
            }

            for (var i = 0; i < node.Children.Count; i++)
            {
                var child = node.Children[i];
                Visit(child);
            }
        }

        public void Visit(ITerminalForestNode node)
        {
            throw new NotImplementedException();
        }

        private void PrintNode(IForestNode node)
        {
            switch (node.NodeType)
            {
                case ForestNodeType.Intermediate:
                    var intermediate = node as IIntermediateForestNode;
                    if (intermediate.Children.Count > 1)
                    {
                        throw new Exception("Intermediate node has more children than expected. ");
                    }

                    var flatList = GetFlattenedList(intermediate);
                    for (var i = 0; i < flatList.Count; i++)
                    {
                        this._writer.Write(" ");
                        PrintNode(flatList[i]);
                    }

                    break;

                case ForestNodeType.Symbol:
                    var symbolForestNode = node as ISymbolForestNode;
                    var symbolForestNodeString = GetSymbolNodeString(symbolForestNode);
                    this._writer.Write(" ");
                    this._writer.Write(symbolForestNodeString);
                    break;

                case ForestNodeType.Token:
                    var tokenForestNode = node as ITokenForestNode;
                    var tokenForestNodeString = GetTokenNodeString(tokenForestNode);
                    this._writer.Write(" ");
                    this._writer.Write(tokenForestNodeString);
                    break;
            }
        }


        private static IList<IForestNode> GetFlattenedList(IIntermediateForestNode intermediate)
        {
            var children = new List<IForestNode>();
            for (var a = 0; a < intermediate.Children.Count; a++)
            {
                var andNode = intermediate.Children[a];
                for (var c = 0; c < andNode.Children.Count; c++)
                {
                    var child = andNode.Children[c];
                    switch (child.NodeType)
                    {
                        case ForestNodeType.Intermediate:
                            var childList = GetFlattenedList(child as IIntermediateForestNode);
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
            return $"('{node.Token.Value}', {node.Origin}, {node.Location})";
        }

        private readonly HashSet<IForestNode> _visited;
        private readonly TextWriter _writer;
    }
}