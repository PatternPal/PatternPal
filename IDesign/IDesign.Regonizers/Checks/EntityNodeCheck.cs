using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDesign.Recognizers.Checks
{
    public static class EntityNodeCheck
    {
        /// <summary>
        ///     Function thats checks the type of a entitynode.
        /// </summary>
        /// <param name="entityNode">The entitynode witch it should check</param>
        /// <param name="nodeType">The expected entitynode type</param>
        /// <returns></returns>
        public static bool CheckEntityNodeType(this IEntityNode entityNode, EntityNodeType nodeType)
        {
            return entityNode.GetEntityNodeType().Equals(nodeType);
        }

        public static bool CheckEntityNodeModifier(this IEntityNode entityNode, string modifier)
        {
            return entityNode.GetTypeDeclarationSyntax().Modifiers.Any(x => x.ToString().IsEqual(modifier));
        }

        /// <summary>
        ///     Function thats checks the relation with another entitynode.
        /// </summary>
        /// <param name="entityNode">The entitynode witch it should check</param>
        /// <param name="relationType">The expected relation type</param>
        /// <returns></returns>
        public static bool CheckRelationType(this IEntityNode entityNode, RelationType relationType)
        {
            return entityNode.GetRelations().Any(x => x.GetRelationType().Equals(relationType));
        }

        public static bool CheckMinimalAmountOfRelationTypes(this IEntityNode entityNode, RelationType relationType, int amount)
        {
            return entityNode.GetRelations().Where(x => x.GetRelationType().Equals(relationType)).Count() >= amount;
        }
    }
}
