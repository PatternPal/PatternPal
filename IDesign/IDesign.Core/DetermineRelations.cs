using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
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
        ///     Function to determine relations between Entitynodes.
        ///     Extense and Implements based.
        /// </summary>
        public void GetEdgesOfEntityNode()
        {
            foreach (var entityNode in EntityNodes.Values)
            {
                if (entityNode.GetTypeDeclarationSyntax().BaseList != null)
                {
                    foreach (var child in entityNode.GetTypeDeclarationSyntax().BaseList.ChildNodes())
                    {
                        var stringname = child.ToString();
                        {
                            RelationType? relationType = null ;
                            RelationType? relationTypeInverse = null ;
                            switch (EntityNodes[stringname].GetEntityNodeType())
                            {
                                case EntityNodeType.Class:
                                    relationType = RelationType.Extends;
                                    relationTypeInverse = RelationType.Extended;
                                    break;
                                case EntityNodeType.Interface:
                                    relationType = RelationType.Implements;
                                    relationType = RelationType.Implemented;
                                    break;
                                default:
                                    break;
                            }



                           var edge = new EntityNodeEdges(EntityNodes[stringname], relationType.Value);
                            entityNode.GetRelations().Add(edge);
                            edge = new EntityNodeEdges(EntityNodes[stringname], relationTypeInverse.Value);
                            entityNode.GetRelations().Add(edge);
                        }
                    }
                }

                var childNodes = entityNode.GetTypeDeclarationSyntax().DescendantNodes();
                foreach (var creation in childNodes.OfType<ObjectCreationExpressionSyntax>())
                {
                  //  creation.Iden
                      //  var identifiers = creation.DescendantNodes().OfType<IdentifierNameSyntax>();

                   // foreach

                }

            }
        }
    }
}
