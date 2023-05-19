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

        /// <summary>
        /// Returns the arguments passed to constructor when it was invoked.
        /// TODO check whether this is correct, or just delete as it isn't used.
        /// </summary>
        IEnumerable<string> GetArguments();

        /// <summary>
        /// Return the constructor rapped as a <see cref="IMethod"/>.
        /// </summary>
        IMethod AsMethod();

        /// <summary>
        /// Gets the parent of this <see cref="IConstructor"/>.
        /// </summary>
        new IClass GetParent();
    }
}
