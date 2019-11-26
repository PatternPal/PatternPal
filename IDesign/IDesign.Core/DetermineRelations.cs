using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace IDesign.Core
{
    /// <summary>
    ///     Class to determine relations between classes/interfaces
    /// </summary>
    public class DetermineRelations
    {
        private Dictionary<TypeDeclarationSyntax, EntityNode> EntityNodes =
            new Dictionary<TypeDeclarationSyntax, EntityNode>();

        /// <summary>
        ///     Constructor of the Determine Relations class
        /// </summary>
        /// <param name="entityNodes"></param>
        public DetermineRelations(Dictionary<TypeDeclarationSyntax, EntityNode> entityNodes)
        {
            EntityNodes = entityNodes;
        }

        public EntityNode EntityNode { get; set; }
        public EntityNodeEdges Edge { get; set; }
    }
}