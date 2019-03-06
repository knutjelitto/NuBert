namespace Pliant.Tree
{
    public interface ITreeNode
    {
        int Location { get; }

        void Accept(ITreeNodeVisitor visitor);
    }
}