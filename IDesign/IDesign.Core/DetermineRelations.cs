using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace IDesign.Core
{
    /// <summary>
    ///     Class to determine relations between classes/interfaces.
    /// </summary>
    public class DetermineRelations
    {
        private Dictionary<TypeDeclarationSyntax, EntityNode> EntityNodes =
            new Dictionary<TypeDeclarationSyntax, EntityNode>();

        /// <summary>
        ///     Constructor of the Determine Relations class.
        /// </summary>
        /// <param name="entityNodes"></param>
        public DetermineRelations(Dictionary<TypeDeclarationSyntax, EntityNode> entityNodes)
        {
            EntityNodes = entityNodes;
        }

        /// <summary>
        ///     Function to determine relations between Entitynodes.
        ///     Extense and Implements based.
        /// </summary>
        public void GetEdgesOfEntityNode()
        {
            foreach (var entityNode in EntityNodes)
            {
                if (entityNode.Key.BaseList != null)
                {
                    foreach (var child in entityNode.Key.BaseList.ChildNodes())
                    {
                        var stringname = child.ToString();
                        foreach (var node in EntityNodes)
                        {
                            if (node.Key.Identifier.ToString() == stringname)
                            {
                                Edge = new EntityNodeEdges(node.Value);
                                entityNode.Value.EntityNodeEdgesList.Add(Edge);
                            }
                        }
                    }
                }
            }
        }

        public EntityNode EntityNode { get; set; }
        public EntityNodeEdges Edge { get; set; }
    }
}