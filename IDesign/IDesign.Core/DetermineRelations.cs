using System.Collections.Generic;
using System.Linq;
using IDesign.Core.Models;
using IDesign.Recognizers.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Core
{
    public class DetermineRelations
    {
        private readonly Dictionary<string, EntityNode> EntityNodes;

        private readonly Dictionary<RelationType, RelationType> reverserdTypes =
            new Dictionary<RelationType, RelationType>
            {
                {RelationType.Implements, RelationType.ImplementedBy},
                {RelationType.Creates, RelationType.CreatedBy},
                {RelationType.Uses, RelationType.UsedBy},
                {RelationType.Extends, RelationType.ExtendedBy}
            };

        public DetermineRelations(Dictionary<string, EntityNode> entityNodes) => EntityNodes = entityNodes;

        public void CreateEdgesOfEntityNode()
        {
            foreach (var entityNode in EntityNodes.Values)
            {
                CreateParentClasses(entityNode);
                CreateCreationalEdges(entityNode);
                CreateUsingEdges(entityNode);
            }
        }

        /// <summary>
        ///     Makes a relation based on a node, a relationtype and a destinationNode.
        /// </summary>
        /// <param name="node">the given node</param>
        /// <param name="type">the given relationtype</param>
        /// <param name="destination">the given destination name as a string</param>
        private void AddRelation(EntityNode node, RelationType type, string destination) =>
            AddRelation(node, type, GetNodeByName(node, destination));

        /// <summary>
        ///     Makes a relation based on a node, a reltiontype and a destinationNode as node
        /// </summary>
        /// <param name="node">the given node</param>
        /// <param name="type">the given relationtype</param>
        /// <param name="edgeNode">the given destination node</param>
        private void AddRelation(EntityNode node, RelationType type, EntityNode edgeNode)
        {
            if (edgeNode != null)
            {
                //Make sure relation does not already exists
                if (node.Relations.Any(x => x.GetDestination() == edgeNode && x.GetRelationType() == type))
                {
                    return;
                }

                node.Relations.Add(new Relation(edgeNode, type));
                edgeNode.Relations.Add(new Relation(node, reverserdTypes[type]));
            }
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
            var childNodes = entityNode.GetTypeDeclarationSyntax().Members.SelectMany(x => x.DescendantNodes());
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
            var namespaces = new List<string>
            {
                node.NameSpace + "."
            };
            namespaces.AddRange(node.GetUsingDeclarationSyntaxList().Select(x => x.Name + "."));
            namespaces.Add("");
            foreach (var key in from nameSpace in namespaces
                                let key = nameSpace + name
                                where EntityNodes.ContainsKey(key)
                                select key)
            {
                return EntityNodes[key];
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
                        if (edgeNode != null)
                        {
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
}