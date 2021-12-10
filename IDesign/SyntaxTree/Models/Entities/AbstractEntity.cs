﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;
using SyntaxTree.Abstractions.Root;
using SyntaxTree.Models.Members.Method;
using SyntaxTree.Models.Members.Property;
using SyntaxTree.Utils;

namespace SyntaxTree.Models.Entities {
    public abstract class AbstractEntity : AbstractNode, IEntity {
        private readonly TypeDeclarationSyntax _typeDeclarationSyntax;

        private readonly List<IMethod> _methods = new List<IMethod>();
        private readonly List<IProperty> _properties = new List<IProperty>();
        private readonly List<TypeSyntax> _bases;

        private readonly IRoot _parent;

        protected AbstractEntity(TypeDeclarationSyntax node, IRoot parent) : base(
            node, parent.GetRoot()
        ) {
            _typeDeclarationSyntax = node;
            _parent = parent;

            foreach (var member in node.Members) {
                switch (member) {
                    case MethodDeclarationSyntax method:
                        _methods.Add(new Method(method, this));
                        break;
                    case PropertyDeclarationSyntax property:
                        _properties.Add(new Property(property, this));
                        break;
                }
            }

            _bases = node.BaseList?.Types.Select(t => t.Type).ToList() ?? new List<TypeSyntax>();
        }

        public override string GetName() => _typeDeclarationSyntax.Identifier.Text;

        public IEnumerable<IModifier> GetModifiers() => _typeDeclarationSyntax.Modifiers.ToModifiers();
        public IEnumerable<IMethod> GetMethods() => _methods.AsReadOnly();
        public IEnumerable<IProperty> GetProperties() => _properties.AsReadOnly();
        public IEnumerable<TypeSyntax> GetBases() => _bases;

        public abstract EntityType GetEntityType();

        public string GetFullName() {
            if (_parent is INamespace names) return $"${names.GetNamespace()}.${GetName()}";
            return GetName();
        }

        public IEnumerable<IRelation> GetRelations() => _parent.GetRelations(this);

        public virtual IEnumerable<IMethod> GetAllMethods() {
            List<IMethod> methods = new List<IMethod>(GetMethods());
            foreach (var property in GetProperties()) {
                if (property.HasGetter()) methods.Add(property.GetGetter());
                if (property.HasSetter()) methods.Add(property.GetSetter());
            }

            return methods.AsReadOnly();
        }

        public IRoot GetParent() => _parent;
    }
}