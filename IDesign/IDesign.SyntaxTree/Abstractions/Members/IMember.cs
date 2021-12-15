using SyntaxTree.Abstractions.Entities;

namespace SyntaxTree.Abstractions.Members
{
    public interface IMember : INode, IModified, IChild<IEntity>
    {
    }
}
