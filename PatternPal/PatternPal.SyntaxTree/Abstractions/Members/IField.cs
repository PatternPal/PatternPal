using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PatternPal.SyntaxTree.Abstractions.Members
{
    /// <summary>
    /// An <see cref="INode"/> which represents a field.
    /// </summary>
    public interface IField : IMember
    {
        TypeSyntax GetFieldType();
    }
}
