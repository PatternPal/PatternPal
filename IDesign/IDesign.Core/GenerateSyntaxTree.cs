using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IDesign.Core
{
    public class GenerateSyntaxTree
    {
        public CompilationUnitSyntax Root { get; set; }
        private SyntaxTree Tree { get; set; }
        public List<EntityNode> EntityNodes = new List<EntityNode>();

        public List<UsingDirectiveSyntax> UsingDirectiveSyntaxList = new List<UsingDirectiveSyntax>();
        public List<ClassDeclarationSyntax> ClassDeclarationSyntaxList = new List<ClassDeclarationSyntax>();
        public List<InterfaceDeclarationSyntax> InterfaceDeclarationSyntaxList = new List<InterfaceDeclarationSyntax>();
        public List<MethodDeclarationSyntax> MethodDeclarationSyntaxList;
        public List<PropertyDeclarationSyntax> PropertyDeclarationSyntaxList;
        public List<ConstructorDeclarationSyntax> ConstructorDeclarationSyntaxList;
        public List<FieldDeclarationSyntax> FieldDeclarationSyntaxList;


        /// <summary>
        /// Constructor of the GenerateSyntaxTree class.
        /// Genarates a syntaxtree from a string
        /// </summary>
        /// <param name="file"></param>
        public GenerateSyntaxTree(string file)
        {
            Tree = CSharpSyntaxTree.ParseText(file);
            Root = Tree.GetCompilationUnitRoot();

            GetUsingsOfFile();
            GetAllClassesOfFile();
            GetAllInterfacesOfFile();
            GetAllConstructorsOfAClass();
            GetAllMethodsOfAClass();
            GetAllPropertiesOfAClass();
            GetAllFieldsOfAClass();
        }

        /// <summary>
        /// Adds all usings of the syntaxtree to a list
        /// </summary>
        /// <returns></returns>
        private List<UsingDirectiveSyntax> GetUsingsOfFile()
        {
            foreach (UsingDirectiveSyntax element in Root.Usings)
            {
                UsingDirectiveSyntaxList.Add(element);
            }
            return UsingDirectiveSyntaxList;
        }

        /// <summary>
        /// Parent function of the recursive function that adds all ClassNodes of the file to the ClassDeclarationSyntaxList
        /// </summary>
        private void GetAllClassesOfFile()
        {
            if (Root.Members != null)
            {
                foreach (var member in Root.Members)
                {
                    GetAllClassesOfFile(member);
                }
            }
        }

        /// <summary>
        /// Recursive function that adds all ClassNodes of the file to the ClassDeclarationSyntaxList
        /// </summary>
        /// <param name="node"></param>
        /// <returns>
        /// Returns a List with ClassDeclarationSyntaxes
        /// </returns>
        private List<ClassDeclarationSyntax> GetAllClassesOfFile(SyntaxNode node)
        {

            if (node.Kind() == SyntaxKind.ClassDeclaration)
            {
                ClassDeclarationSyntax classNode = (ClassDeclarationSyntax)node;
                ClassDeclarationSyntaxList.Add(classNode);
                EntityNode entityNode = new EntityNode
                {
                    InterfaceOrClassNode = classNode,
                    Name = classNode.Identifier.ToString()
                };
                EntityNodes.Add(entityNode);
            }
            if (node.ChildNodes() != null)
            {
                foreach (var childNode in node.ChildNodes())
                {
                    GetAllClassesOfFile(childNode);
                }
            }
            return ClassDeclarationSyntaxList;
        }

        /// <summary>
        /// Parent function of the recursive function that adds all InterfaceNodes of the file to the InterfaceDeclarationSyntaxList
        /// </summary>
        private void GetAllInterfacesOfFile()
        {
            if (Root.Members != null)
            {
                foreach (var member in Root.Members)
                {
                    GetAllInterfacesOfFile(member);
                }
            }
        }

        /// <summary>
        /// Recursive function that adds all InterfaceNodes of the file to the InterfaceDeclarationSyntaxList
        /// </summary>
        /// <param name="node"></param>
        /// <returns>
        /// Returns a List with InterfaceDeclarationSyntaxes
        /// </returns>
        private List<InterfaceDeclarationSyntax> GetAllInterfacesOfFile(SyntaxNode node)
        {
            if (node.Kind() == SyntaxKind.InterfaceDeclaration)
            {
                EntityNode entityNode = new EntityNode();
                InterfaceDeclarationSyntax interfaceNode = (InterfaceDeclarationSyntax)node;
                InterfaceDeclarationSyntaxList.Add(interfaceNode);

                entityNode.InterfaceOrClassNode = interfaceNode;
                entityNode.Name = interfaceNode.Identifier.ToString();
                EntityNodes.Add(entityNode);
            }
            if (node.ChildNodes() != null)
            {
                foreach (var childNode in node.ChildNodes())
                {
                    GetAllInterfacesOfFile(childNode);
                }
            }
            return InterfaceDeclarationSyntaxList;
        }

        /// <summary>
        /// Parent function of the recursive function that searches for all constructors in a class
        /// </summary>
        private void GetAllConstructorsOfAClass()
        {
            if (EntityNodes != null)
            {
                foreach (var classElement in EntityNodes)
                {
                    classElement.ConstructorDeclarationSyntaxList = GetAllConstructorsOfAClass(classElement.InterfaceOrClassNode);
                }
            }
        }

        /// <summary>
        ///  Recursive function that searches for all constructors in a class
        /// </summary>
        /// <param name="node"></param>
        /// <returns>
        /// Returns a list with ConstructorDeclarationSyntaxes
        /// </returns>
        private List<ConstructorDeclarationSyntax> GetAllConstructorsOfAClass(SyntaxNode node)
        {
            ConstructorDeclarationSyntaxList = new List<ConstructorDeclarationSyntax>();
            if (node.ChildNodes() != null)
            {
                foreach (var childNode in node.ChildNodes())
                {
                    if (childNode.Kind() == SyntaxKind.ConstructorDeclaration)
                    {
                        ConstructorDeclarationSyntax constructorNode = (ConstructorDeclarationSyntax)childNode;
                        ConstructorDeclarationSyntaxList.Add(constructorNode);
                    }
                }
            }
            return ConstructorDeclarationSyntaxList;
        }

        /// <summary>
        /// Parent of recursive function that searches for all methodes of a class
        /// </summary>
        private void GetAllMethodsOfAClass()
        {
            if (EntityNodes != null)
            {
                foreach (var classElement in EntityNodes)
                {
                    classElement.MethodDeclarationSyntaxList = GetAllMethodsOfAClass(classElement.InterfaceOrClassNode);
                }
            }
        }

        /// <summary>
        /// Recursive function that searches for all methodes of a class
        /// </summary>
        /// <param name="node"></param>
        /// <returns>
        /// Returns a list with MethodDeclarationSynates
        /// </returns>
        private List<MethodDeclarationSyntax> GetAllMethodsOfAClass(SyntaxNode node)
        {
            MethodDeclarationSyntaxList = new List<MethodDeclarationSyntax>();
            if (node.ChildNodes() != null)
            {
                foreach (var childeNode in node.ChildNodes())
                {
                    if (childeNode.Kind() == SyntaxKind.MethodDeclaration)
                    {
                        MethodDeclarationSyntax methodNode = (MethodDeclarationSyntax)childeNode;
                        MethodDeclarationSyntaxList.Add(methodNode);
                    }
                }
            }
            return MethodDeclarationSyntaxList;
        }

        /// <summary>
        /// Parent of recursive function that searches for all properties of a class
        /// </summary>
        private void GetAllPropertiesOfAClass()
        {
            if (EntityNodes != null)
            {
                foreach (var classElement in EntityNodes)
                {
                    classElement.PropertyDeclarationSyntaxList = GetAllPropertiesOfAClass(classElement.InterfaceOrClassNode);
                }
            }
        }

        /// <summary>
        /// Recursive function that searches for all properties of a class
        /// </summary>
        /// <param name="node"></param>
        /// <returns>
        /// Returns a list with PropertyDeclarationSyntax
        /// </returns>
        private List<PropertyDeclarationSyntax> GetAllPropertiesOfAClass(SyntaxNode node)
        {
            PropertyDeclarationSyntaxList = new List<PropertyDeclarationSyntax>();
            if (node.ChildNodes() != null)
            {
                foreach (var childNode in node.ChildNodes())
                {
                    if (childNode.Kind() == SyntaxKind.PropertyDeclaration)
                    {
                        PropertyDeclarationSyntax propertyNode = (PropertyDeclarationSyntax)childNode;
                        PropertyDeclarationSyntaxList.Add(propertyNode);
                    }
                }
            }
            return PropertyDeclarationSyntaxList;
        }

        /// <summary>
        /// Parent of recursive function that searches for all fields of a class
        /// </summary>
        private void GetAllFieldsOfAClass()
        {
            if (EntityNodes != null)
            {
                foreach (var classElement in EntityNodes)
                {
                    classElement.FieldDeclarationSyntaxList = GetAllFieldsOfAClass(classElement.InterfaceOrClassNode);
                }

            }
        }

        /// <summary>
        /// Recursive function that searches for all fields of a class
        /// </summary>
        /// <param name="node"></param>
        /// <returns>
        /// Returns a list with FieldDeclarationSyntax
        /// </returns>
        private List<FieldDeclarationSyntax> GetAllFieldsOfAClass(SyntaxNode node)
        {
            FieldDeclarationSyntaxList = new List<FieldDeclarationSyntax>();
            if (node.ChildNodes() != null)
            {
                foreach (var childNode in node.ChildNodes())
                {
                    if (childNode.Kind() == SyntaxKind.FieldDeclaration)
                    {
                        FieldDeclarationSyntax fieldNode = (FieldDeclarationSyntax)childNode;
                        FieldDeclarationSyntaxList.Add(fieldNode);
                    }
                }
            }
            return FieldDeclarationSyntaxList;
        }

    }
}
