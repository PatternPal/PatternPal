using System.Collections.Generic;
using SyntaxTree.Abstractions.Entities;

namespace SyntaxTree.Abstractions.Members {
    public interface IConstructor : INode, IModified, IParameterized, IBodied, IChild<IClass> {
        string GetConstructorType();

        IEnumerable<string> GetArguments();

        IMethod AsMethod();
    }
}
