using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SyntaxTree.Abstractions.Members
{
    public interface IConstructor : IMember, IParameterized, IBodied, IChild<IClass>
    {
        string GetConstructorType();

        IEnumerable<string> GetArguments();

        IMethod AsMethod();

        new IClass GetParent();
    }
}
