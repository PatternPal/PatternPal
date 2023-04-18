using System.Collections.Generic;

namespace SyntaxTree.Abstractions.Entities
{
    public interface IClass : IEntity
    {
        /// <summary>
        ///     Get a list of constructors declared in this node
        /// </summary>
        /// <returns>A list of constructors</returns>
        IEnumerable<IConstructor> GetConstructors();

        /// <summary>
        ///     Get a list of fields declared in this node
        /// </summary>
        /// <returns>A list of fields</returns>
        IEnumerable<IField> GetFields();
    }
}
