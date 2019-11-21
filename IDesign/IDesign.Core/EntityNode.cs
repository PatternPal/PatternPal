﻿using IDesign.Regonizers.Abstractions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

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
        public IEnumerable<MethodDeclarationSyntax> GetMethods()
        {
            return MethodDeclarationSyntaxList;
        }

        public IEnumerable<PropertyDeclarationSyntax> GetProperties()
        {
            return PropertyDeclarationSyntaxList;
        }
    }
}
