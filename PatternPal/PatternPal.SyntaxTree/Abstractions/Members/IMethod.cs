using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PatternPal.SyntaxTree.Abstractions.Members
{
    public interface IMethod : IMember, IParameterized, IBodied
    {
    }
}
