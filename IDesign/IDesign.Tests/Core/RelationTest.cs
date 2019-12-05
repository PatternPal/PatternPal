using System.Collections.Generic;
using System.Linq;
using IDesign.Core;
using IDesign.Recognizers;
using IDesign.Recognizers.Checks;
using IDesign.Tests.Utils;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace IDesign.Tests.Core
{
    public class RelationTest
    {

        [TestCase("RelationTestCase1.cs", "RelationTestCase1", "IRelationTestCase1", true)]
        [TestCase("RelationTestCase2.cs", "RelationTestCase2", "IRelationTestCase2", false)]
        [TestCase("RelationTestCase3.cs", "RelationTestCase3", "IRelationTestCase3", true)]
        [TestCase("RelationTestCase4.cs", "RelationTestCase4", "IRelationTestCase4", false)]
        [TestCase("RelationTestCase5.cs", "RelationTestCase5", "IRelationTestCase5", false)]
        [TestCase("RelationTestCase6.cs", "RelationTestCase6", "IRelationTestCase6", false)]
        public void BaseClass_Should_Implement_RelatedClass(string filename, string baseClass, string relatedClass, bool shouldBeValid)
        {
            
            string code = FileUtils.FileToString("RelationTestClasses\\" + filename);
            var entityNodes = new Dictionary<string, EntityNode>();
            var root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            var NameSpaceNode = root.Members[0] as NamespaceDeclarationSyntax;
            var nodes = NameSpaceNode.DescendantNodes().OfType<TypeDeclarationSyntax>();
            foreach(var node in nodes)
            {
                var tempnode = new EntityNode
                {
                    Name = node.Identifier.ToString(),
                    InterfaceOrClassNode = node,
                    NameSpace = NameSpaceNode.Name.ToString(),
                    MethodDeclarationSyntaxList =
               node.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList(),
                    FieldDeclarationSyntaxList =
               node.DescendantNodes().OfType<FieldDeclarationSyntax>().ToList(),
                    PropertyDeclarationSyntaxList =
               node.DescendantNodes().OfType<PropertyDeclarationSyntax>().ToList(),
                    ConstructorDeclarationSyntaxList =
               node.DescendantNodes().OfType<ConstructorDeclarationSyntax>().ToList()
                };
                entityNodes.Add(NameSpaceNode.Name.ToString() + "." + node.Identifier.ToString(), tempnode);
            }
            var createRelation = new DetermineRelations(entityNodes);
            createRelation.GetEdgesOfEntityNode();
           var interfaceCheck = entityNodes[NameSpaceNode.Name.ToString() + "." + baseClass].GetRelations()
                .Any(x => x.GetRelationType() == RelationType.Implements && x.GetDestination().GetName() == relatedClass);
            Assert.AreEqual(shouldBeValid, interfaceCheck);
        }

        [TestCase("RelationTestCase1.cs", "RelationTestCase1", "EReatlionTestCase1", true)]
        [TestCase("RelationTestCase2.cs", "RelationTestCase2", "ERelationTestCase2", false)]
        [TestCase("RelationTestCase3.cs", "RelationTestCase3", "IRelationTestCase3", false)]
        [TestCase("RelationTestCase4.cs", "RelationTestCase4", "ERelationTestCase4", true)]
        [TestCase("RelationTestCase5.cs", "RelationTestCase5", "ERelationTestCase5", false)]
        [TestCase("RelationTestCase6.cs", "RelationTestCase6", "ERelationTestCase6", true)]
        public void BaseClass_Should_Extend_RelatedClass(string filename, string baseClass, string relatedClass, bool shouldBeValid)
        {
            string code = FileUtils.FileToString("RelationTestClasses\\" + filename);
            var entityNodes = new Dictionary<string, EntityNode>();
            var root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            var NameSpaceNode = root.Members[0] as NamespaceDeclarationSyntax;
            var nodes = NameSpaceNode.DescendantNodes().OfType<TypeDeclarationSyntax>();
            foreach (var node in nodes)
            {
                var tempnode = new EntityNode
                {
                    Name = node.Identifier.ToString(),
                    InterfaceOrClassNode = node,
                    NameSpace = NameSpaceNode.Name.ToString(),
                    MethodDeclarationSyntaxList =
               node.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList(),
                    FieldDeclarationSyntaxList =
               node.DescendantNodes().OfType<FieldDeclarationSyntax>().ToList(),
                    PropertyDeclarationSyntaxList =
               node.DescendantNodes().OfType<PropertyDeclarationSyntax>().ToList(),
                    ConstructorDeclarationSyntaxList =
               node.DescendantNodes().OfType<ConstructorDeclarationSyntax>().ToList()
                };
                entityNodes.Add(NameSpaceNode.Name.ToString() + "." + node.Identifier.ToString(), tempnode);
            }
            var createRelation = new DetermineRelations(entityNodes);
            createRelation.GetEdgesOfEntityNode();
            var extendsCheck = entityNodes[NameSpaceNode.Name.ToString() + "." + baseClass].GetRelations()
                .Any(x => x.GetRelationType() == RelationType.Extends && x.GetDestination().GetName() == relatedClass);
            Assert.AreEqual(shouldBeValid, extendsCheck);
        }

        [TestCase("RelationTestCase1.cs", "RelationTestCase1", "CRelationTestCase1", true)]
        [TestCase("RelationTestCase2.cs", "RelationTestCase2", "CRelationTestCase2", true)]
        [TestCase("RelationTestCase3.cs", "RelationTestCase3", "CRelationTestCase2", false)]
        [TestCase("RelationTestCase4.cs", "RelationTestCase4", "CRelationTestCase4", false)]
        [TestCase("RelationTestCase5.cs", "RelationTestCase5", "CRelationTestCase5", false)]
        [TestCase("RelationTestCase6.cs", "RelationTestCase6", "ERelationTestCase6", true)]
        public void BaseClass_Should_Create_RelatedClass(string filename, string baseClass, string relatedClass, bool shouldBeValid)
        {
            string code = FileUtils.FileToString("RelationTestClasses\\" + filename);
            var entityNodes = new Dictionary<string, EntityNode>();
            var root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            var NameSpaceNode = root.Members[0] as NamespaceDeclarationSyntax;
            var nodes = NameSpaceNode.DescendantNodes().OfType<TypeDeclarationSyntax>();
            foreach (var node in nodes)
            {
                var tempnode = new EntityNode
                {
                    Name = node.Identifier.ToString(),
                    InterfaceOrClassNode = node,
                    NameSpace = NameSpaceNode.Name.ToString(),
                    MethodDeclarationSyntaxList =
               node.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList(),
                    FieldDeclarationSyntaxList =
               node.DescendantNodes().OfType<FieldDeclarationSyntax>().ToList(),
                    PropertyDeclarationSyntaxList =
               node.DescendantNodes().OfType<PropertyDeclarationSyntax>().ToList(),
                    ConstructorDeclarationSyntaxList =
               node.DescendantNodes().OfType<ConstructorDeclarationSyntax>().ToList()
                };
                entityNodes.Add(NameSpaceNode.Name.ToString() + "." + node.Identifier.ToString(), tempnode);
            }
            var createRelation = new DetermineRelations(entityNodes);
            createRelation.GetEdgesOfEntityNode();
            var createCheck = entityNodes[NameSpaceNode.Name.ToString() + "." + baseClass].GetRelations()
                .Any(x => x.GetRelationType() == RelationType.Creates && x.GetDestination().GetName() == relatedClass);
            Assert.AreEqual(shouldBeValid, createCheck);
        }


        [TestCase("RelationTestCase1.cs", "RelationTestCase1", "CRelationTestCase1", true)]
        [TestCase("RelationTestCase2.cs", "RelationTestCase2", "CRelationTestCase2", true)]
        [TestCase("RelationTestCase3.cs", "RelationTestCase3", "CRelationTestCase2", false)]
        [TestCase("RelationTestCase4.cs", "RelationTestCase4", "CRelationTestCase4", false)]
        [TestCase("RelationTestCase5.cs", "RelationTestCase5", "CRelationTestCase5", false)]
        [TestCase("RelationTestCase6.cs", "RelationTestCase6", "ERelationTestCase6", true)]
        [TestCase("RelationTestCase7.cs", "RelationTestCase7", "U1RelationTestCase7", true)]
        public void BaseClass_Should_Use_RelatedClass(string filename, string baseClass, string relatedClass, bool shouldBeValid)
        {
            string code = FileUtils.FileToString("RelationTestClasses\\" + filename);
            var entityNodes = new Dictionary<string, EntityNode>();
            var root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            var NameSpaceNode = root.Members[0] as NamespaceDeclarationSyntax;
            var nodes = NameSpaceNode.DescendantNodes().OfType<TypeDeclarationSyntax>();
            foreach (var node in nodes)
            {
                var tempnode = new EntityNode
                {
                    Name = node.Identifier.ToString(),
                    InterfaceOrClassNode = node,
                    NameSpace = NameSpaceNode.Name.ToString(),
                    MethodDeclarationSyntaxList =
               node.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList(),
                    FieldDeclarationSyntaxList =
               node.DescendantNodes().OfType<FieldDeclarationSyntax>().ToList(),
                    PropertyDeclarationSyntaxList =
               node.DescendantNodes().OfType<PropertyDeclarationSyntax>().ToList(),
                    ConstructorDeclarationSyntaxList =
               node.DescendantNodes().OfType<ConstructorDeclarationSyntax>().ToList()
                };
                entityNodes.Add(NameSpaceNode.Name.ToString() + "." + node.Identifier.ToString(), tempnode);
            }
            var createRelation = new DetermineRelations(entityNodes);
            createRelation.GetEdgesOfEntityNode();
            var usingCheck = entityNodes[NameSpaceNode.Name.ToString() + "." + baseClass].GetRelations()
                .Any(x => x.GetRelationType() == RelationType.Uses && x.GetDestination().GetName() == relatedClass);
            Assert.AreEqual(shouldBeValid, usingCheck);
        }
    }
}
