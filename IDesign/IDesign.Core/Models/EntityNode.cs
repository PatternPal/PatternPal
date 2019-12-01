using IDesign.Models;
using IDesign.Recognizers.Abstractions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace IDesign.Core
{
    public class EntityNode : IEntityNode
    {
        public List<ConstructorDeclarationSyntax> ConstructorDeclarationSyntaxList =
            new List<ConstructorDeclarationSyntax>();

        public List<EntityNodeEdges> EntityNodeEdgesList = new List<EntityNodeEdges>();
        public List<FieldDeclarationSyntax> FieldDeclarationSyntaxList = new List<FieldDeclarationSyntax>();
        public List<MethodDeclarationSyntax> MethodDeclarationSyntaxList = new List<MethodDeclarationSyntax>();
        public List<PropertyDeclarationSyntax> PropertyDeclarationSyntaxList = new List<PropertyDeclarationSyntax>();
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

        public IEnumerable<IMethod> GetMethods()
        {
            var list = new List<IMethod>();
            list.AddRange(MethodDeclarationSyntaxList.Select(x => new Method(x)));

            foreach (var property in PropertyDeclarationSyntaxList)
                if (property.AccessorList != null)
                {
                    var getters = property.AccessorList.Accessors.Where(x =>
                        x.Kind() == SyntaxKind.GetAccessorDeclaration && x.Body != null);
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
    }
}