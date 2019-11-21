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
        public CompilationUnitSyntax root { get; set; }
        private SyntaxTree tree { get; set; }
        public List<EntityNode> EntityNodes = new List<EntityNode>();

        //Soorten classes
        public List<ClassDeclarationSyntax> ClassDeclarationSyntaxList = new List<ClassDeclarationSyntax>();
        public List<InterfaceDeclarationSyntax> InterfaceDeclarationSyntaxList = new List<InterfaceDeclarationSyntax>();

        //Inhoud van classes
        public List<MethodDeclarationSyntax> MethodDeclarationSyntaxList;
        public List<PropertyDeclarationSyntax> PropertyDeclarationSyntaxList;
        public List<ConstructorDeclarationSyntax> ConstructorDeclarationSyntaxList;
        public List<FieldDeclarationSyntax> FieldDeclarationSyntaxList;

        //Inhoud van namespaces
        public List<UsingDirectiveSyntax> UsingDirectiveSyntaxList = new List<UsingDirectiveSyntax>();
        public List<EnumDeclarationSyntax> EnumDeclarationSyntaxList = new List<EnumDeclarationSyntax>();
        public List<DelegateDeclarationSyntax> DelegateDeclarationSyntaxList = new List<DelegateDeclarationSyntax>();

        /// <summary>
        /// Constructor of the GenerateSyntaxTree class.
        /// Genarates a syntaxtree from a string
        /// </summary>
        /// <param name="file"></param>
        public GenerateSyntaxTree(string file)
        {
            tree = CSharpSyntaxTree.ParseText(file);
            root = tree.GetCompilationUnitRoot();
        }

        /// <summary>
        /// Adds all usings of the syntaxtree to a list
        /// </summary>
        /// <returns></returns>
        public List<UsingDirectiveSyntax> GetUsingsOfFile()
        {
            foreach (UsingDirectiveSyntax element in root.Usings)
            {
                UsingDirectiveSyntaxList.Add(element);
            }
            return UsingDirectiveSyntaxList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void GetAllClassesOfFile()
        {
            if (root.Members != null)
            {
                foreach (var node in root.Members)
                {
                    GetAllClassesOfFile(node);
                }
            }
        }

        /// <summary>
        /// Recursive function that adds all ClassNodes of the file to the ClassDeclarationSyntaxList
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private List<ClassDeclarationSyntax> GetAllClassesOfFile(SyntaxNode node)
        {

            if (node.Kind() == SyntaxKind.ClassDeclaration)
            {
                var classNode = (ClassDeclarationSyntax)node;
                ClassDeclarationSyntaxList.Add(classNode);
                var entityNode = new EntityNode();
                entityNode.InterfaceOrClassNode = classNode;
                EntityNodes.Add(entityNode);
            }
            if (node.ChildNodes() != null)
            {
                foreach (var i in node.ChildNodes())
                {
                    GetAllClassesOfFile(i);
                }
            }
            return ClassDeclarationSyntaxList;
        }

        /// <summary>
        /// 
        /// </summary>
        public void GetAllInterfacesOfFile()
        {
            if (root.Members != null)
            {
                foreach (var node in root.Members)
                {
                    GetAllInterfacesOfFile(node);
                }
            }
        }

        /// <summary>
        /// Recursive function that adds all InterfaceNodes of the file to the InterfaceDeclarationSyntaxList
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private List<InterfaceDeclarationSyntax> GetAllInterfacesOfFile(SyntaxNode node)
        {
            if (node.Kind() == SyntaxKind.InterfaceDeclaration)
            {
                var interfaceNode = (InterfaceDeclarationSyntax)node;
                InterfaceDeclarationSyntaxList.Add(interfaceNode);
                var entityNode = new EntityNode();
                entityNode.InterfaceOrClassNode = interfaceNode;
                EntityNodes.Add(entityNode);
            }
            if (node.ChildNodes() != null)
            {
                foreach (var i in node.ChildNodes())
                {
                    GetAllInterfacesOfFile(i);
                }
            }
            return InterfaceDeclarationSyntaxList;
        }

        /// <summary>
        /// 
        /// </summary>
        public void GetAllConstructorsOfAClass()
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
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private List<ConstructorDeclarationSyntax> GetAllConstructorsOfAClass(SyntaxNode node)
        {
            ConstructorDeclarationSyntaxList = new List<ConstructorDeclarationSyntax>();
            if (node.ChildNodes() != null)
            {
                foreach (var x in node.ChildNodes())
                {
                    if (x.Kind() == SyntaxKind.ConstructorDeclaration)
                    {
                        var constructorNode = (ConstructorDeclarationSyntax)x;
                        ConstructorDeclarationSyntaxList.Add(constructorNode);
                    }
                }
            }
            return ConstructorDeclarationSyntaxList;
        }

        /// <summary>
        /// 
        /// </summary>
        public void GetAllMethodsOfAClass()
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
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private List<MethodDeclarationSyntax> GetAllMethodsOfAClass(SyntaxNode node)
        {
            MethodDeclarationSyntaxList = new List<MethodDeclarationSyntax>();
            if (node.ChildNodes() != null)
            {
                foreach (var x in node.ChildNodes())
                {
                    if (x.Kind() == SyntaxKind.MethodDeclaration)
                    {
                        var methodNode = (MethodDeclarationSyntax)x;
                        MethodDeclarationSyntaxList.Add(methodNode);
                    }
                }
            }
            return MethodDeclarationSyntaxList;
        }

        /// <summary>
        /// 
        /// </summary>
        public void GetAllPropertiesOfAClass()
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
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private List<PropertyDeclarationSyntax> GetAllPropertiesOfAClass(SyntaxNode node)
        {
            PropertyDeclarationSyntaxList = new List<PropertyDeclarationSyntax>();
            if (node.ChildNodes() != null)
            {
                foreach (var x in node.ChildNodes())
                {
                    if (x.Kind() == SyntaxKind.PropertyDeclaration)
                    {
                        var propertyNode = (PropertyDeclarationSyntax)x;
                        PropertyDeclarationSyntaxList.Add(propertyNode);
                    }
                }
            }
            return PropertyDeclarationSyntaxList;
        }

        /// <summary>
        /// 
        /// </summary>
        public void GetAllFieldsOfAClass()
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
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private List<FieldDeclarationSyntax> GetAllFieldsOfAClass(SyntaxNode node)
        {
            FieldDeclarationSyntaxList = new List<FieldDeclarationSyntax>();
            if (node.ChildNodes() != null)
            {
                foreach (var x in node.ChildNodes())
                {
                    if (x.Kind() == SyntaxKind.FieldDeclaration)
                    {
                        var propertyNode = (FieldDeclarationSyntax)x;
                        FieldDeclarationSyntaxList.Add(propertyNode);
                    }
                }
            }
            return FieldDeclarationSyntaxList;
        }






    }
}
