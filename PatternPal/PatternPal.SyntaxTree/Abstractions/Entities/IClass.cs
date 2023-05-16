using System.Collections.Generic;

using PatternPal.SyntaxTree.Abstractions.Members;

namespace PatternPal.SyntaxTree.Abstractions.Entities
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
