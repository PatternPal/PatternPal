﻿using System.Linq;
using PatternPal.SyntaxTree.Abstractions;
using PatternPal.SyntaxTree.Abstractions.Members;
using PatternPal.SyntaxTree.Abstractions.Root;
using PatternPal.SyntaxTree.Models.Members.Constructor;
using PatternPal.SyntaxTree.Models.Members.Field;

namespace PatternPal.SyntaxTree.Models.Entities
{
    public class Class : AbstractEntity, IClass
    {
        private readonly List<IConstructor> _constructors = new List<IConstructor>();
        private readonly List<IField> _fields = new List<IField>();
        private readonly ClassDeclarationSyntax _typeDeclarationSyntax;

        public Class(ClassDeclarationSyntax typeDeclarationSyntax, IEntitiesContainer parent) : base(
            typeDeclarationSyntax, parent
        )
        {
            _typeDeclarationSyntax = typeDeclarationSyntax;

            foreach (var member in typeDeclarationSyntax.Members)
            {
                switch (member)
                {
                    case ConstructorDeclarationSyntax constructor:
                        _constructors.Add(new Constructor(constructor, this));
                        break;
                    case FieldDeclarationSyntax field:
                        _fields.Add(new Field(field, this));
                        break;
                }
            }
        }

        public override EntityType GetEntityType()
        {
            return EntityType.Class;
        }

        public IEnumerable<IConstructor> GetConstructors()
        {
            return _constructors.AsReadOnly();
        }

        public IEnumerable<IField> GetFields()
        {
            return _fields.AsReadOnly();
        }

        public override IEnumerable<IMethod> GetAllMethods()
        {
            var methods = new List<IMethod>(base.GetAllMethods());
            methods.AddRange(_constructors.Select(c => c.AsMethod()));
            return methods.AsReadOnly();
        }

        public override IEnumerable<IField> GetAllFields()
        {
            return base.GetAllFields().Concat(GetFields());
        }

        public override IEnumerable<IMember> GetMembers()
        {
            return base.GetMembers().Concat(_fields).Concat(_constructors);
        }

        public override IEnumerable<INode> GetChildren()
        {
            return base.GetChildren()
                .Concat(_constructors)
                .Concat(_fields);
        }
    }
}
