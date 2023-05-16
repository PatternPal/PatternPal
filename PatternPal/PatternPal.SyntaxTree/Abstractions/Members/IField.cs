using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PatternPal.SyntaxTree.Abstractions.Members
{
    public interface IField : IMember
    {
        TypeSyntax GetFieldType();
    }
}
