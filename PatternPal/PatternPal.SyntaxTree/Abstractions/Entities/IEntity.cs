#region

using PatternPal.SyntaxTree.Abstractions.Members;
using PatternPal.SyntaxTree.Abstractions.Root;

#endregion

namespace PatternPal.SyntaxTree.Abstractions.Entities
{
    /// <summary>
    /// An <see cref="INode"/> which contains <see cref="IMember"/>s.
    /// </summary>
    public interface IEntity : IModified, IChild<IEntitiesContainer>, INamedEntitiesContainer
    {
        TypeDeclarationSyntax GetTypeDeclarationSyntax();
        /// <summary>
        /// Gets a list of <see cref="IMethod"/>s declared in this entity.
        /// </summary>
        IEnumerable<IMethod> GetMethods();

        /// <summary>
        /// Gets a list of <see cref="IProperty"/>s declared in this entity.
        /// </summary>
        IEnumerable<IProperty> GetProperties();

        /// <summary>
        /// Gets a list of all <see cref="IMember"/>s declared in this entity.
        /// </summary>
        IEnumerable<IMember> GetMembers();

        /// <summary>
        /// Gets a list of all <see cref="IMember"/>s declared in this entity.
        /// Also includes wrappers for example the getter and setter of a <see cref="IProperty"/> as a <see cref="IMethod"/>.
        /// </summary>
        IEnumerable<IMember> GetAllMembers();

        /// <summary>
        /// Gets a list of bases of this entity (overrides / implementations).
        /// </summary>
        IEnumerable<TypeSyntax> GetBases();

        /// <summary>
        /// Gets the type of this entity.
        /// </summary>
        EntityType GetEntityType();

        /// <summary>
        /// Gets the name of this entity, namespaces included.
        /// </summary>
        string GetFullName();

        /// <summary>
        /// Gets all <see cref="Relation"/>s that this entity has, filtered on the type of the destination node of the relation.
        /// </summary>
        IEnumerable<Relation> GetRelations(RelationTargetKind type);

        /// <summary>
        /// Gets all methods. This includes getters and setters from <see cref="IProperty"/>s and <see cref="IConstructor"/>s.
        /// </summary>
        IEnumerable<IMethod> GetAllMethods();

        /// <summary>
        /// Gets all fields. This includes getters from properties.
        /// </summary>
        IEnumerable<IField> GetAllFields();
    }
    
    /// <summary>
    /// The possible types of entities that can exist.
    /// </summary>
    public enum EntityType
    {
        Class, Interface
        //TODO ENUM, STRUCT
    }
}
