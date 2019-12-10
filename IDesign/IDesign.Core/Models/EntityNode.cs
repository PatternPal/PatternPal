using IDesign.Recognizers;
using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Models;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

namespace IDesign.Core
{
    public class EntityNode : IEntityNode
    {
        public List<ConstructorDeclarationSyntax> ConstructorDeclarationSyntaxList =
            new List<ConstructorDeclarationSyntax>();
        public string NameSpace { get; set; }
        public List<IRelation> Relations = new List<IRelation>();
        public List<FieldDeclarationSyntax> FieldDeclarationSyntaxList = new List<FieldDeclarationSyntax>();
        public List<UsingDirectiveSyntax> UsingDeclarationSyntaxList = new List<UsingDirectiveSyntax>();
        public List<MethodDeclarationSyntax> MethodDeclarationSyntaxList = new List<MethodDeclarationSyntax>();
        public List<PropertyDeclarationSyntax> PropertyDeclarationSyntaxList = new List<PropertyDeclarationSyntax>();
        public string Name { get; set; }
        public string SourceFile { get; set; }
        public TypeDeclarationSyntax InterfaceOrClassNode { get; set; }

        public TypeDeclarationSyntax GetTypeDeclarationSyntax()
        {
            return InterfaceOrClassNode;
        }

        /// <summary>
        ///     Get name of entitynode
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return Name;
        }

        /// <summary>
        ///     Get source file of an entitynode
        /// </summary>
        /// <returns></returns>
        public string GetSourceFile()
        {
            return SourceFile;
        }

        /// <summary>
        ///     Get all methods and properties of a class
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IMethod> GetMethods()
        {
            var list = new List<IMethod>();
            list.AddRange(MethodDeclarationSyntaxList.Select(x => new Method(x)));

            foreach (var property in PropertyDeclarationSyntaxList)
                if (property.AccessorList != null)
                {
                    var accessors = property.AccessorList.Accessors;
                    var getters = accessors.Where(x => x.Kind() == SyntaxKind.GetAccessorDeclaration);
                    getters = getters.Where(x => x.Body != null || x.ExpressionBody != null);
                    list.AddRange(getters.Select(x => new PropertyMethod(property, x)));
                }
            return list;
        }

        public IEnumerable<IField> GetFields()
        {
            var listGetters = new List<IField>();
            foreach (var property in PropertyDeclarationSyntaxList)
                if (property.AccessorList != null)
                {
                    var getters = property.AccessorList.Accessors.Where(x =>
                        x.Kind() == SyntaxKind.GetAccessorDeclaration && x.Body == null);
                    listGetters.AddRange(getters.Select(x => new PropertyField(property)));
                }

            foreach (var field in FieldDeclarationSyntaxList)
                listGetters.AddRange(field.Declaration.Variables.Select(x => new Field(field, x)));
            return listGetters;
        }

        public IEnumerable<IMethod> GetConstructors()
        {
            var listConstructors = new List<IMethod>();
            listConstructors.AddRange(ConstructorDeclarationSyntaxList.Select(x => new Constructormethod(x)));
            return listConstructors;
        }

        public IEnumerable<ConstructorDeclarationSyntax> GetCostructors()
        {
            return ConstructorDeclarationSyntaxList;
        }

        public IEnumerable<PropertyDeclarationSyntax> GetProperties()
        {
            return PropertyDeclarationSyntaxList;
        }

        public IEnumerable<IRelation> GetRelations()
        {
            return Relations;
        }

        public EntityNodeType GetEntityNodeType()
        {
            var declarationnode = this.GetTypeDeclarationSyntax();

            if (declarationnode.GetType() == typeof(ClassDeclarationSyntax))
            {
                return EntityNodeType.Class;
            }
            else if (declarationnode.GetType() == typeof(InterfaceDeclarationSyntax))
            {
                return EntityNodeType.Interface;
            }
            return EntityNodeType.Class;

        }

        public List<UsingDirectiveSyntax> GetUsings()
        {
            return UsingDeclarationSyntaxList;
        }

        public string GetSuggestionName()
        {
            return GetName();
        }

        public SyntaxNode GetSuggestionNode()
        {
            return InterfaceOrClassNode;
        }
    }
}
