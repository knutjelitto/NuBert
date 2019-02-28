using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pliant.Tree
{
    public class TreeDumpVisitor : ITreeNodeVisitor
    {
        private readonly TextWriter writer;
        private string indent = string.Empty;

        public TreeDumpVisitor(TextWriter writer)
        {
            this.writer = writer;
        }

        public TreeDumpVisitor()
            : this(Console.Out)
        {
        }

        public void Visit(ITokenTreeNode node)
        {
            this.writer.WriteLine($"{this.indent}{node}");
        }

        public void Visit(IInternalTreeNode node)
        {
            this.writer.WriteLine($"{this.indent}{node}");
            this.indent += "   ";
            foreach (var child in node.Children)
            {
                child.Accept(this);
            }

            this.indent = this.indent.Substring(0, this.indent.Length - 3);
        }
    }
}
