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
        private Dictionary<string, EntityNode> EntityNodes =
            new Dictionary<string, EntityNode>();

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
            }
        }

        /// <summary>
        ///     Makes edges if a node creates another class
        /// </summary>
        /// <param name="entityNode"></param>
        private void CreateCreationalEdges(EntityNode entityNode)
        {
            var childNodes = entityNode.GetTypeDeclarationSyntax().DescendantNodes();
            foreach (var creation in childNodes.OfType<ObjectCreationExpressionSyntax>())
            {
                if (creation.Type is IdentifierNameSyntax name)
                {
                    var edgeNode = GetNodeByName(entityNode, name.ToString());
                    var edge = new Relation(edgeNode, RelationType.Creates);
                    entityNode.GetRelations().Add(edge);

                    edge = new Relation(entityNode, RelationType.CreatedBy);
                    edgeNode.GetRelations().Add(edge);
                }
            }
        }
        /// <summary>
        /// Returns an entitynode with this name
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <returns>entitynode with the given name</returns>
        private IEntityNode GetNodeByName(EntityNode node, string name)
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

        /// <summary>
        ///     Makes edges if a node implements or extends another class
        /// </summary>
        /// <param name="entityNode"></param>
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
                        RelationType? relationTypeInverse = null;
                        switch (edgeNode.GetEntityNodeType())
                        {
                            case EntityNodeType.Class:
                                relationType = RelationType.Extends;
                                relationTypeInverse = RelationType.ExtendedBy;
                                break;
                            case EntityNodeType.Interface:
                                relationType = RelationType.Implements;
                                relationTypeInverse = RelationType.ImplementedBy;
                                break;
                            default:
                                break;
                        }
                        var edge = new Relation(edgeNode, relationType.Value);
                        entityNode.GetRelations().Add(edge);

                        edge = new Relation(entityNode, relationTypeInverse.Value);
                        edgeNode.GetRelations().Add(edge);
                    }
                }
            }
        }
    }
}