using System.Collections.Generic;

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
