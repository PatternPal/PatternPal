using IDesign.Recognizers;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace IDesign.Core
{
    /// <summary>
    ///     Class to determine relations between classes/interfaces.
    /// </summary>
    public class DetermineRelations
    {
        private Dictionary<string, EntityNode> EntityNodes;

        private Dictionary<RelationType, RelationType> reverserdTypes =
            new Dictionary<RelationType, RelationType>()
            {
                {RelationType.Implements, RelationType.ImplementedBy},
                {RelationType.Creates, RelationType.CreatedBy},
                {RelationType.Uses, RelationType.UsedBy},
                {RelationType.Extends, RelationType.ExtendedBy}
            };

        /// <summary>
        ///     Constructor of the Determine Relations class.
        /// </summary>
        /// <param name="entityNodes"></param>
        public DetermineRelations(Dictionary<string, EntityNode> entityNodes)
        {
            EntityNodes = entityNodes;
        }

        /// <summary>
        ///     Makes Edges for every node in the dictionary
        /// </summary>
        public void GetEdgesOfEntityNode()
        {
            foreach (var entityNode in EntityNodes.Values)
            {
                CreateParentClasses(entityNode);
                CreateCreationalEdges(entityNode);
                CreateUsingEdges(entityNode);
            }
        }

        private void AddRelation(EntityNode node, RelationType type, string destination)
        {
            AddRelation(node, type, GetNodeByName(node, destination));
        }

        private void AddRelation(EntityNode node, RelationType type, EntityNode edgeNode)
        {
            if (node == null || edgeNode == null)
            {
                return;
            }

            node.Relations.Add(new Relation(edgeNode, type));
            edgeNode.Relations.Add(new Relation(node, reverserdTypes[type]));
        }

        private void CreateCreationalEdges(EntityNode entityNode)
        {
            var childNodes = entityNode.GetTypeDeclarationSyntax().DescendantNodes();
            foreach (var creation in childNodes.OfType<ObjectCreationExpressionSyntax>())
            {
                if (creation.Type is IdentifierNameSyntax name)
                {
                    AddRelation(entityNode, RelationType.Creates, name.ToString());
                }
            }
        }

        private void CreateUsingEdges(EntityNode entityNode)
        {
            var childNodes = entityNode.GetTypeDeclarationSyntax().DescendantNodes();
            foreach (var identifier in childNodes.OfType<IdentifierNameSyntax>())
            {
                if (identifier is IdentifierNameSyntax name)
                {
                    AddRelation(entityNode, RelationType.Uses, name.ToString());
                }
            }
        }

        private EntityNode GetNodeByName(EntityNode node, string name)
        {
            var namespaces = new List<string>();
            namespaces.Add(node.NameSpace + ".");
            namespaces.AddRange(node.GetUsings().Select(x => x.Name.ToString() + "."));
            namespaces.Add("");
            foreach (var nameSpace in namespaces)
            {
                var key = nameSpace + name;
                if (EntityNodes.ContainsKey(key))
                {
                    return EntityNodes[key];
                }
            }
            return null;
        }

        private void CreateParentClasses(EntityNode entityNode)
        {
            if (entityNode.GetTypeDeclarationSyntax().BaseList != null)
            {
                foreach (var child in entityNode.GetTypeDeclarationSyntax().BaseList.ChildNodes())
                {
                    var stringname = child.ToString();
                    {
                        var edgeNode = GetNodeByName(entityNode, stringname);
                        if (edgeNode == null)
                        {
                            continue;
                        }

                        RelationType? relationType = null;

                        switch (edgeNode.GetEntityNodeType())
                        {
                            case EntityNodeType.Class:
                                relationType = RelationType.Extends;
                                break;
                            case EntityNodeType.Interface:
                                relationType = RelationType.Implements;
                                break;
                        }

                        AddRelation(entityNode, relationType.Value, edgeNode);
                    }
                }
            }
        }
    }
}
