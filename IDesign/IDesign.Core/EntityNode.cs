using IDesign.Models;
using IDesign.Recognizers.Abstractions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace IDesign.Core
{
    public class EntityNode : IEntityNode
    {
        public string Name { get; set; }
        public TypeDeclarationSyntax InterfaceOrClassNode { get; set; }
        public List<MethodDeclarationSyntax> MethodDeclarationSyntaxList = new List<MethodDeclarationSyntax>();
        public List<ConstructorDeclarationSyntax> ConstructorDeclarationSyntaxList = new List<ConstructorDeclarationSyntax>();
        public List<PropertyDeclarationSyntax> PropertyDeclarationSyntaxList = new List<PropertyDeclarationSyntax>();
        public List<FieldDeclarationSyntax> FieldDeclarationSyntaxList = new List<FieldDeclarationSyntax>();
        public TypeDeclarationSyntax GetTypeDeclarationSyntax()
        {
            return InterfaceOrClassNode;
        }

        public string GetName()
        {
            return Name;
        }

        public IEnumerable<ConstructorDeclarationSyntax> GetCostructors()
        {
            return ConstructorDeclarationSyntaxList;
        }
        public IEnumerable<IMethod> GetMethods()
        {
            var list = new List<IMethod>();
            list.AddRange(MethodDeclarationSyntaxList.Select(x => new Method(x)));

            foreach (var property in PropertyDeclarationSyntaxList)
            {
                var getters = property.AccessorList.Accessors.Where(x => x.Kind() == SyntaxKind.GetAccessorDeclaration && x.Body != null);
                list.AddRange(getters.Select(x => new PropertyMethod(property, x)));
            }

            return list;
        }

        public IEnumerable<PropertyDeclarationSyntax> GetProperties()
        {
            return PropertyDeclarationSyntaxList;
        }

        public IEnumerable<IField> GetFields()
        {
            var listGetters = new List<IField>();
            foreach (var property in PropertyDeclarationSyntaxList)
            {
                var getters = property.AccessorList.Accessors.Where(x => x.Kind() == SyntaxKind.GetAccessorDeclaration && x.Body == null);
                listGetters.AddRange(getters.Select(x => new PropertyField(property)));
            }

            foreach (var field in FieldDeclarationSyntaxList)
            {
                listGetters.AddRange(field.Declaration.Variables.Select(x => new Field(field, x)));
            }
            return listGetters;
        }
    }
}
