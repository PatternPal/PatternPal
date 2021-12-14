using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SyntaxTree.Abstractions.Members {
    public interface IMethod : IMember, IParameterized, IBodied {
        TypeSyntax GetReturnType();
    }
}
