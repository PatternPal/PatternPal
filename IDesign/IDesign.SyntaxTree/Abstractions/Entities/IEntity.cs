using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxTree.Abstractions.Members;
using SyntaxTree.Abstractions.Root;

namespace SyntaxTree.Abstractions.Entities {
    public interface IEntity : IModified, IChild<IEntitiesContainer>, INamedEntitiesContainer {
        /// <summary>
        ///     Get a list of methods declared in this node
        /// </summary>
        /// <returns>A list of methods</returns>
        IEnumerable<IMethod> GetMethods();

        /// <summary>
        ///     Get a list of properties declared in this node
        /// </summary>
        /// <returns>A list of properties</returns>
        IEnumerable<IProperty> GetProperties();

        /// <summary>
        ///     Gets a list of all methods, field, properties and constructors. Maybe more in the futher 
        /// </summary>
        /// <returns>A list of Members</returns>
        IEnumerable<IMember> GetMembers();

        /// <summary>
        ///     Get a list of bases of this note (overrides / implementations)
        /// </summary>
        /// <returns>A list of types</returns>
        IEnumerable<TypeSyntax> GetBases();

        /// <summary>
        ///     Gets the type of this entity
        /// </summary>
        EntityType GetEntityType();

        /// <summary>
        ///     Get the name with included namespace
        /// </summary>
        /// <returns></returns>
        string GetFullName();

        IEnumerable<IRelation> GetRelations();

        /// <summary>
        ///     Gets all methods this includes getter and setter from properties and constructors
        /// </summary>
        /// <returns>A list of methods</returns>
        IEnumerable<IMethod> GetAllMethods();

        /// <summary>
        ///     Gets all fields this includes getters from properties
        /// </summary>
        /// <returns>A list of methods</returns>
        IEnumerable<IField> GetAllFields();
    }

    public enum EntityType {
        Class, Interface
        //TODO ENUM, STRUCT
    }
}
