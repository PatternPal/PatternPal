using System;
using System.Collections.Generic;
using IDesign.Core.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace IDesign.Core
{
    public class SyntaxTreeGenerator
    {
        public List<ClassDeclarationSyntax> ClassDeclarationSyntaxList = new List<ClassDeclarationSyntax>();
        public List<ConstructorDeclarationSyntax> ConstructorDeclarationSyntaxList;
        public List<EntityNode> EntityNodes = new List<EntityNode>();
        public List<FieldDeclarationSyntax> FieldDeclarationSyntaxList;
        public List<InterfaceDeclarationSyntax> InterfaceDeclarationSyntaxList = new List<InterfaceDeclarationSyntax>();
        public List<MethodDeclarationSyntax> MethodDeclarationSyntaxList;
        public List<PropertyDeclarationSyntax> PropertyDeclarationSyntaxList;
        public List<UsingDirectiveSyntax> UsingDirectiveSyntaxList = new List<UsingDirectiveSyntax>();

        public SyntaxTreeGenerator(string content, string source, Dictionary<string, EntityNode> entityNodes)
        {
            File = source;
            Tree = CSharpSyntaxTree.ParseText(content);
            Root = Tree.GetCompilationUnitRoot();
            GetUsingsOfFile();
            GetAllClassesOfFile(entityNodes);
            GetAllInterfacesOfFile(entityNodes);
            GetAllConstructorsOfAClass(entityNodes);
            GetAllMethodsOfAClass(entityNodes);
            GetAllPropertiesOfAClass(entityNodes);
            GetAllFieldsOfAClass(entityNodes);
        }

        public string File { get; set; }
        public CompilationUnitSyntax Root { get; set; }
        public SyntaxTree Tree { get; }

        private List<UsingDirectiveSyntax> GetUsingsOfFile()
        {
            UsingDirectiveSyntaxList.AddRange(from element in Root.Usings
                                              select element);
            return UsingDirectiveSyntaxList;
        }

        private void GetAllClassesOfFile(Dictionary<string, EntityNode> entityNodes)
        {
            if (Root.Members != null)
            {
                foreach (var member in Root.Members)
                {
                    GetAllClassesOfFile(member, entityNodes);
                }
            }
        }

        private List<ClassDeclarationSyntax> GetAllClassesOfFile(SyntaxNode node,
            Dictionary<string, EntityNode> entityNodes)
        {
            if (node.Kind() == SyntaxKind.ClassDeclaration)
            {
                var classNode = (ClassDeclarationSyntax)node;
                ClassDeclarationSyntaxList.Add(classNode);
                var nameSpaceKey = "";
                if (classNode.Parent is NamespaceDeclarationSyntax nameSpace)
                {
                    nameSpaceKey += nameSpace.Name.ToString();
                }

                var keybinding = nameSpaceKey + "." + classNode.Identifier;
                if (!entityNodes.ContainsKey(keybinding))
                {
                    var entityNode = new EntityNode
                    {
                        InterfaceOrClassNode = classNode,
                        Name = classNode.Identifier.ToString(),
                        UsingDeclarationSyntaxList = new List<UsingDirectiveSyntax>(UsingDirectiveSyntaxList),
                        SourceFile = File,
                        NameSpace = nameSpaceKey
                    };
                    entityNodes.Add(keybinding, entityNode);
                }
            }

            if (node.ChildNodes() != null)
            {
                foreach (var childNode in node.ChildNodes())
                {
                    GetAllClassesOfFile(childNode, entityNodes);
                }
            }
            return ClassDeclarationSyntaxList;
        }

        private void GetAllInterfacesOfFile(Dictionary<string, EntityNode> entityNodes)
        {
            if (Root.Members != null)
            {
                foreach (var member in Root.Members)
                {
                    GetAllInterfacesOfFile(member, entityNodes);
                }
            }
        }

        private List<InterfaceDeclarationSyntax> GetAllInterfacesOfFile(SyntaxNode node,
            Dictionary<string, EntityNode> entityNodes)
        {
            if (node.Kind() == SyntaxKind.InterfaceDeclaration)
            {
                var interfaceNode = (InterfaceDeclarationSyntax)node;
                InterfaceDeclarationSyntaxList.Add(interfaceNode);
                var nameSpaceKey = "";
                if (interfaceNode.Parent is NamespaceDeclarationSyntax nameSpace)
                {
                    nameSpaceKey += nameSpace.Name.ToString();
                }

                var keybinding = nameSpaceKey + "." + interfaceNode.Identifier;
                if (!entityNodes.ContainsKey(keybinding))
                {
                    var entityNode = new EntityNode
                    {
                        InterfaceOrClassNode = interfaceNode,
                        Name = interfaceNode.Identifier.ToString(),
                        UsingDeclarationSyntaxList = new List<UsingDirectiveSyntax>(UsingDirectiveSyntaxList),
                        SourceFile = File,
                        NameSpace = nameSpaceKey
                    };
                    entityNodes.Add(keybinding, entityNode);
                }
            }

            if (node.ChildNodes() != null)
            {
                foreach (var childNode in node.ChildNodes())
                {
                    GetAllInterfacesOfFile(childNode, entityNodes);
                }
            }
            return InterfaceDeclarationSyntaxList;
        }

        private void GetAllConstructorsOfAClass(Dictionary<string, EntityNode> entityNodes)
        {
            if (EntityNodes != null)
            {
                foreach (var classElement in entityNodes)
                {
                    classElement.Value.ConstructorDeclarationSyntaxList =
                            GetAllConstructorsOfAClass(classElement.Value.InterfaceOrClassNode);
                }
            }
        }

        private List<ConstructorDeclarationSyntax> GetAllConstructorsOfAClass(SyntaxNode node)
        {
            ConstructorDeclarationSyntaxList = new List<ConstructorDeclarationSyntax>();
            if (node.ChildNodes() != null)
            {
                ConstructorDeclarationSyntaxList.AddRange(from childNode in node.ChildNodes()
                                                          where childNode.Kind() == SyntaxKind.ConstructorDeclaration
                                                          let constructorNode = (ConstructorDeclarationSyntax)childNode
                                                          select constructorNode);
            }
            return ConstructorDeclarationSyntaxList;
        }

        private void GetAllMethodsOfAClass(Dictionary<string, EntityNode> entityNodes)
        {
            if (EntityNodes != null)
            {
                foreach (var classElement in entityNodes)
                {
                    classElement.Value.MethodDeclarationSyntaxList =
                         GetAllMethodsOfAClass(classElement.Value.InterfaceOrClassNode);
                }
            }
        }

        private List<MethodDeclarationSyntax> GetAllMethodsOfAClass(SyntaxNode node)
        {
            MethodDeclarationSyntaxList = new List<MethodDeclarationSyntax>();
            if (node.ChildNodes() != null)
            {
                MethodDeclarationSyntaxList.AddRange(from childeNode in node.ChildNodes()
                                                     where childeNode.Kind() == SyntaxKind.MethodDeclaration
                                                     let methodNode = (MethodDeclarationSyntax)childeNode
                                                     select methodNode);
            }
            return MethodDeclarationSyntaxList;
        }

        private void GetAllPropertiesOfAClass(Dictionary<string, EntityNode> entityNodes)
        {
            if (EntityNodes != null)
            {
                foreach (var classElement in entityNodes)
                {
                    classElement.Value.PropertyDeclarationSyntaxList =
                           GetAllPropertiesOfAClass(classElement.Value.InterfaceOrClassNode);
                }
            }
        }

        private List<PropertyDeclarationSyntax> GetAllPropertiesOfAClass(SyntaxNode node)
        {
            PropertyDeclarationSyntaxList = new List<PropertyDeclarationSyntax>();
            if (node.ChildNodes() != null)
            {
                PropertyDeclarationSyntaxList.AddRange(from childNode in node.ChildNodes()
                                                       where childNode.Kind() == SyntaxKind.PropertyDeclaration
                                                       let propertyNode = (PropertyDeclarationSyntax)childNode
                                                       select propertyNode);
            }
            return PropertyDeclarationSyntaxList;
        }

        private void GetAllFieldsOfAClass(Dictionary<string, EntityNode> entityNodes)
        {
            if (EntityNodes != null)
            {
                foreach (var classElement in entityNodes)
                {
                    classElement.Value.FieldDeclarationSyntaxList =
                           GetAllFieldsOfAClass(classElement.Value.InterfaceOrClassNode);
                }
            }
        }

        private List<FieldDeclarationSyntax> GetAllFieldsOfAClass(SyntaxNode node)
        {
            FieldDeclarationSyntaxList = new List<FieldDeclarationSyntax>();
            if (node.ChildNodes() != null)
            {
                FieldDeclarationSyntaxList.AddRange(from childNode in node.ChildNodes()
                                                    where childNode.Kind() == SyntaxKind.FieldDeclaration
                                                    let fieldNode = (FieldDeclarationSyntax)childNode
                                                    select fieldNode);
            }
            return FieldDeclarationSyntaxList;
        }
    }
}