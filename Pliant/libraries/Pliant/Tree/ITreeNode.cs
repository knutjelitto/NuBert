namespace Pliant.Tree
{
    public interface ITreeNode
    {
        int Origin { get; }
        int Location { get; }

        void Accept(ITreeNodeVisitor visitor);
    }
}