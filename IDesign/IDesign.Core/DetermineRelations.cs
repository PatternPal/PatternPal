using System.Collections.Generic;
using System.Linq;
using IDesign.Core.Models;
using IDesign.Recognizers.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Core
{
    /// <summary>
    ///     Class to determine relations between classes/interfaces.
    /// </summary>
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

        /// <summary>
        ///     Makes an relation based on a node, a relationtype and a destinationNode.
        /// </summary>
        /// <param name="node">the given node</param>
        /// <param name="type">the given relationtype</param>
        /// <param name="destination">the given destination name as a string</param>
        private void AddRelation(EntityNode node, RelationType type, string destination)
        {
            AddRelation(node, type, GetNodeByName(node, destination));
        }

        /// <summary>
        ///     Makes an relation based on a node, a reltiontype and a destinationNode as node
        /// </summary>
        /// <param name="node">the given node</param>
        /// <param name="type">the given relationtype</param>
        /// <param name="edgeNode">the given destination node</param>
        private void AddRelation(EntityNode node, RelationType type, EntityNode edgeNode)
        {
            if (edgeNode == null) return;

            //Make sure relation doesn't already exists
            if (node.Relations.Any(x => x.GetDestination() == edgeNode && x.GetRelationType() == type))
                return;

            node.Relations.Add(new Relation(edgeNode, type));
            edgeNode.Relations.Add(new Relation(node, reverserdTypes[type]));
        }

        /// <summary>
        ///     Determines what nodes this node creates
        /// </summary>
        /// <param name="entityNode">the node which creates the other nodes</param>
        private void CreateCreationalEdges(EntityNode entityNode)
        {
            var childNodes = entityNode.GetTypeDeclarationSyntax().DescendantNodes();
            foreach (var creation in childNodes.OfType<ObjectCreationExpressionSyntax>())
                if (creation.Type is IdentifierNameSyntax name)
                    AddRelation(entityNode, RelationType.Creates, name.ToString());
        }

        /// <summary>
        ///     Determines what nodes this node uses
        /// </summary>
        /// <param name="entityNode">the node which makes use of the other nodes</param>
        private void CreateUsingEdges(EntityNode entityNode)
        {
            var childNodes = entityNode.GetTypeDeclarationSyntax().Members.SelectMany(x => x.DescendantNodes());
            foreach (var identifier in childNodes.OfType<IdentifierNameSyntax>())
                if (identifier is IdentifierNameSyntax name)
                    AddRelation(entityNode, RelationType.Uses, name.ToString());
        }

        /// <summary>
        ///     Gets node from the dictionary by name
        /// </summary>
        /// <param name="node">the node from which you search</param>
        /// <param name="name">the name of the node that is searched for</param>
        /// <returns>the node which was searched for</returns>
        private EntityNode GetNodeByName(EntityNode node, string name)
        {
            var namespaces = new List<string>();
            namespaces.Add(node.NameSpace + ".");
            namespaces.AddRange(node.GetUsings().Select(x => x.Name + "."));
            namespaces.Add("");
            foreach (var nameSpace in namespaces)
            {
                var key = nameSpace + name;
                if (EntityNodes.ContainsKey(key)) return EntityNodes[key];
            }

            return null;
        }

        /// <summary>
        ///     Creates Parent relations of a given node
        /// </summary>
        /// <param name="entityNode">the given node</param>
        private void CreateParentClasses(EntityNode entityNode)
        {
            if (entityNode.GetTypeDeclarationSyntax().BaseList != null)
                foreach (var child in entityNode.GetTypeDeclarationSyntax().BaseList.ChildNodes())
                {
                    var stringname = child.ToString();
                    {
                        var edgeNode = GetNodeByName(entityNode, stringname);
                        if (edgeNode == null) continue;

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