using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Core.Models
{
    public class EntityNode : IEntityNode
    {
        public List<ConstructorDeclarationSyntax> ConstructorDeclarationSyntaxList =
            new List<ConstructorDeclarationSyntax>();
        public List<FieldDeclarationSyntax> FieldDeclarationSyntaxList = new List<FieldDeclarationSyntax>();
        public List<MethodDeclarationSyntax> MethodDeclarationSyntaxList = new List<MethodDeclarationSyntax>();
        public List<PropertyDeclarationSyntax> PropertyDeclarationSyntaxList = new List<PropertyDeclarationSyntax>();
        public List<IRelation> Relations = new List<IRelation>();
        public List<UsingDirectiveSyntax> UsingDeclarationSyntaxList = new List<UsingDirectiveSyntax>();
        public string NameSpace { get; set; }
        public string Name { get; set; }
        public string SourceFile { get; set; }
        public TypeDeclarationSyntax InterfaceOrClassNode { get; set; }

        public TypeDeclarationSyntax GetTypeDeclarationSyntax()
        {
            return InterfaceOrClassNode;
        }

        public string GetName()
        {
            return Name;
        }

        public string GetSourceFile()
        {
            return SourceFile;
        }

        public IEnumerable<IMethod> GetMethodsAndProperties()
        {
            var list = new List<IMethod>();
            list.AddRange(MethodDeclarationSyntaxList.Select(x => new Method(x)));
            foreach (var property in PropertyDeclarationSyntaxList)
            {
                if (!property.HasGetter()) continue;
                
                list.Add(new PropertyMethod(property));
            }
            return list;
        }

        public IEnumerable<IField> GetFields()
        {
            var listGetters = new List<IField>();
            foreach (var property in PropertyDeclarationSyntaxList.Where(property => property.AccessorList != null).Select(property => property))
            {
                var getters = property.AccessorList.Accessors.Where(x => x.Kind() == SyntaxKind.GetAccessorDeclaration && x.Body == null && x.ExpressionBody == null);
                listGetters.AddRange(getters.Select(x => new PropertyField(property)));
            }

            foreach (var field in FieldDeclarationSyntaxList)
            {
                listGetters.AddRange(field.Declaration.Variables.Select(x => new Field(field, x)));
            }
            return listGetters;
        }

        public IEnumerable<IMethod> GetConstructors()
        {
            var listConstructors = new List<IMethod>();
            listConstructors.AddRange(ConstructorDeclarationSyntaxList.Select(x => new Constructormethod(x)));
            return listConstructors;
        }

        public IEnumerable<IRelation> GetRelations()
        {
            return Relations;
        }

        public EntityNodeType GetEntityNodeType()
        {
            var declarationnode = GetTypeDeclarationSyntax();

            if (declarationnode.GetType() == typeof(ClassDeclarationSyntax))
            {
                return EntityNodeType.Class;
            }

            if (declarationnode.GetType() == typeof(InterfaceDeclarationSyntax))
            {
                return EntityNodeType.Interface;
            }

            return EntityNodeType.Class;
        }

        public string GetSuggestionName() => Name;

        public SyntaxNode GetSuggestionNode()
        {
            return GetTypeDeclarationSyntax();
        }

        public IEnumerable<ConstructorDeclarationSyntax> GetCostructorDeclarationSyntaxList()
        {
            return ConstructorDeclarationSyntaxList;
        }

        public List<UsingDirectiveSyntax> GetUsingDeclarationSyntaxList()
        {
            return UsingDeclarationSyntaxList;
        }

        public SyntaxTokenList GetModifiers()
        {
            return InterfaceOrClassNode.Modifiers;
        }
    }
    
    public static class PropertyExtensions {
        /// <summary>
        /// Check if a property has a getter
        /// <br/><br/>
        /// This is any of the following style:
        /// <code>
        /// public bool expression_style => true;
        /// //Setter can be included
        /// public bool inline_expression_style { get => true };
        /// public bool normal_style { get; };
        /// </code>
        /// </summary>
        /// <param name="property"></param>
        /// <returns>true if the property has a getter</returns>
        public static bool HasGetter(this PropertyDeclarationSyntax property) {
            if (property.ExpressionBody != null) return true;
            return property.AccessorList != null && 
                   property.AccessorList.Accessors
                       .Any(a => a.Kind() == SyntaxKind.GetAccessorDeclaration);
        }
    }
}