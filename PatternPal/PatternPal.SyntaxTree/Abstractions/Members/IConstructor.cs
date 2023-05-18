using System.Collections.Generic;

using PatternPal.SyntaxTree.Abstractions.Entities;

namespace PatternPal.SyntaxTree.Abstractions.Members
{
    /// <summary>
    /// An <see cref="INode"/> which represents a constructor.
    /// </summary>
    public interface IConstructor : IMember, IParameterized, IBodied, IChild<IClass>
    {
        string GetConstructorType();

        IEnumerable<string> GetArguments();

        IMethod AsMethod();

        new IClass GetParent();
    }
}
