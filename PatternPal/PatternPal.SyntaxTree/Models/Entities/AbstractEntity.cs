using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using PatternPal.SyntaxTree.Abstractions;
using PatternPal.SyntaxTree.Abstractions.Entities;
using PatternPal.SyntaxTree.Abstractions.Members;
using PatternPal.SyntaxTree.Abstractions.Root;
using PatternPal.SyntaxTree.Models.Members.Constructor;
using PatternPal.SyntaxTree.Models.Members.Method;
using PatternPal.SyntaxTree.Models.Members.Property;
using PatternPal.SyntaxTree.Utils;

namespace PatternPal.SyntaxTree.Models.Entities
{
    /// <summary>
    /// Base implementation of an Entity.
    /// </summary>
    public abstract class AbstractEntity : AbstractNode, IEntity
    {
        private readonly List<TypeSyntax> _bases;
        private readonly List<IEntity> _entities = new();

        private readonly List<IMethod> _methods = new();

        private readonly IEntitiesContainer _parent;
        private readonly List<IProperty> _properties = new();
        private readonly TypeDeclarationSyntax _typeDeclarationSyntax;

        /// <summary>
        /// Returns an instance of <see cref="AbstractEntity"/>.
        /// </summary>
        protected AbstractEntity(TypeDeclarationSyntax node, IEntitiesContainer parent) : base(
            node, parent.GetRoot()
        )
        {
            _typeDeclarationSyntax = node;
            _parent = parent;

            foreach (var member in node.Members)
            {
                switch (member)
                {
                    case MethodDeclarationSyntax method:
                        _methods.Add(new Method(method, this));
                        break;
                    case PropertyDeclarationSyntax property:
                        _properties.Add(new Property(property, this));
                        break;
                    case TypeDeclarationSyntax type:
                        _entities.Add(type.ToEntity(this));
                        break;
                }
            }

            _bases = node.BaseList?.Types.Select(t => t.Type).ToList() ?? new List<TypeSyntax>();
        }

        /// <summary>
        /// Returns the Roslyn type declaration of this <see cref="IEntity"/>.
        /// </summary>
        public TypeDeclarationSyntax GetTypeDeclarationSyntax()
        {
            return _typeDeclarationSyntax;
        }

        /// <inheritdoc />
        public override string GetName()
        {
            return _typeDeclarationSyntax.Identifier.Text;
        }

        /// <inheritdoc />
        public IEnumerable<IModifier> GetModifiers()
        {
            return _typeDeclarationSyntax.Modifiers.ToModifiers();
        }

        /// <inheritdoc />
        public IEnumerable<IMethod> GetMethods()
        {
            return _methods.AsReadOnly();
        }

        /// <inheritdoc />
        public IEnumerable<IProperty> GetProperties()
        {
            return _properties.AsReadOnly();
        }

        /// <inheritdoc />
        public virtual IEnumerable<IMember> GetMembers() { return _methods.Cast<IMember>().Concat(_properties); }

        /// <inheritdoc />
        public IEnumerable<IMember> GetAllMembers()
        {
            var members = GetMembers().ToList();
            
            foreach (var property in members.OfType<Property>()) {
                if (property.IsField()) members.Add(property.GetField());
                if (property.HasGetter()) members.Add(property.GetGetter());
                if (property.HasSetter()) members.Add(property.GetSetter());
            }
            
            foreach (var constructor in members.OfType<Constructor>())
            {
                members.Add(constructor.AsMethod());
            }
            
            return members;
        }

        /// <inheritdoc />
        public IEnumerable<IEntity> GetEntities()
        {
            return _entities.AsReadOnly();
        }

        /// <inheritdoc />
        public IEnumerable<TypeSyntax> GetBases()
        {
            return _bases;
        }

        /// <inheritdoc />
        public abstract EntityType GetEntityType();

        /// <inheritdoc />
        public Dictionary<string, IEntity> GetAllEntities()
        {
            return _entities.OfType<IEntitiesContainer>()
                .Select(e => e.GetAllEntities())
                .Append(_entities.ToDictionary(e => e.GetFullName()))
                .SelectMany(d => d)
                .ToDictionary(p => p.Key, p => p.Value);
        }

        /// <inheritdoc />
        public string GetFullName()
        {
            if (_parent is INamespace names)
            {
                return $"{names.GetNamespace()}.{GetName()}";
            }

            return GetName();
        }

        /// <summary>
        /// Gets all <see cref="Relation"/>s that this <see cref="INode"/> has, filtered on the type of the destination node of the relation.
        /// </summary>
        public IEnumerable<Relation> GetRelations(RelationTargetKind type)
        {
            return _parent.GetRoot().GetRelations(this, type);
        }

        /// <inheritdoc />
        public virtual IEnumerable<IMethod> GetAllMethods()
        {
            var methods = new List<IMethod>(GetMethods());
            foreach (var property in GetProperties())
            {
                if (property.HasGetter())
                {
                    methods.Add(property.GetGetter());
                }

                if (property.HasSetter())
                {
                    methods.Add(property.GetSetter());
                }
            }

            return methods.AsReadOnly();
        }

        /// <inheritdoc />
        public virtual IEnumerable<IField> GetAllFields()
        {
            return GetProperties()
                .Where(p => p.IsField())
                .Select(p => p.GetField());
        }

        /// <inheritdoc />
        public string GetNamespace()
        {
            if (GetParent() is INamedEntitiesContainer name)
            {
                return $"{name.GetNamespace()}.{GetName()}";
            }

            return GetName();
        }

        /// <inheritdoc />
        public IEntitiesContainer GetParent()
        {
            return _parent;
        }

        public override string ToString()
        {
            return GetName();
        }

        /// <inheritdoc />
        public virtual IEnumerable<INode> GetChildren()
        {
            return _entities.Cast<INode>()
                .Concat(_methods)
                .Concat(_properties);
        }
    }
}
