using System.Collections.Generic;

using PatternPal.SyntaxTree.Abstractions.Entities;

namespace PatternPal.SyntaxTree.Abstractions.Members
{
    /// <summary>
    /// An <see cref="INode"/> which represents a constructor.
    /// </summary>
    public interface IConstructor : IMember, IParameterized, IBodied, IChild<IClass>
    {
        /// <summary>
        /// Gets the name of the <see cref="IClass"/> this constructor is a member of.
        /// </summary>
        string GetConstructorType();

        /// <summary>
        /// Return the constructor wrapped as a <see cref="IMethod"/>.
        /// </summary>
        IMethod AsMethod();

        /// <summary>
        /// Gets the parent of this <see cref="IConstructor"/>.
        /// </summary>
        new IClass GetParent();
    }
}
