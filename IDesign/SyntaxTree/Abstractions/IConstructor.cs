using System.Collections.Generic;

namespace SyntaxTree.Abstractions {
    public interface IConstructor : INode, IModified, IParameterized, IBodied {
        string GetConstructorType();

        IEnumerable<string> GetArguments();

        IMethod AsMethod();
    }
}
