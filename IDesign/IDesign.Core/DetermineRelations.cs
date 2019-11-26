using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Core
{
    /// <summary>
    /// Class to determine relations between classes/interfaces
    /// </summary>
    public class DetermineRelations
    {
        Dictionary<TypeDeclarationSyntax, EntityNode> EntityNodes = new Dictionary<TypeDeclarationSyntax, EntityNode>();
        public EntityNode EntityNode { get; set; }
        public EntityNodeEdges Edge { get; set; }

        /// <summary>
        /// Constructor of the Determine Relations class
        /// </summary>
        /// <param name="entityNodes"></param>
        public DetermineRelations(Dictionary<TypeDeclarationSyntax, EntityNode> entityNodes)
        {
            this.EntityNodes = entityNodes;
        }

    }
}
