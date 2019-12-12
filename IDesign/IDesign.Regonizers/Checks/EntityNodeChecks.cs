using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDesign.Recognizers.Checks
{
    public static class EntityNodeChecks
    {
        /// <summary>
        ///     Return a boolean based on if the interface .
        /// </summary>
        /// <param name="node">The node witch it should check</param>
        /// <param name="name">The expected type</param>
        /// <returns>The field is the type that is given in the function</returns>
        public static bool ImplementsInterface(this IEntityNode node, string name)
        {
            if (HasInterface(node, name))
            {
                return true;
            }
            if (CheckMinimalAmountOfRelationTypes(node, RelationType.Extends, 1))
            {
                return ImplementsInterface(GetExtends(node), name);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        ///     Return a boolean based on if the given field is an expected type.
        /// </summary>
        /// <param name="node">The field witch it should check</param>
        /// <param name="name">The expected type</param>
        /// <returns>The field is the type that is given in the function</returns>
        public static bool ExtendsClass(this IEntityNode node, string name)
        {
            if (CheckMinimalAmountOfRelationTypes(node, RelationType.Extends, 1))
            {
                if (GetExtends(node).GetName() == name)
                {
                    return true;
                }
                return ExtendsClass(GetExtends(node), name);
            }
            else { return false; }
        }

        //helper functions
        /// <summary>
        ///     Return a boolean based on if the given field is an expected type.
        /// </summary>
        /// <param name="node">The node witch it should check</param>
        /// <param name="name">The expected name of the interface</param>
        /// <returns>The node has the interface with that name</returns>
        public static bool HasInterface(this IEntityNode node, string name)
        {
            return node.GetRelations()
                .Any(x => x.GetRelationType() == RelationType.ImplementedBy && x.GetDestination().GetName() == name);
        }

        /// <summary>
        ///     Return a class which is extended by this node.
        /// </summary>
        /// <param name="node">The node witch it should check</param>
        /// <returns>The node that is extended by the given node</returns>
        public static IEntityNode GetExtends(this IEntityNode node)
        {
            return node.GetRelations().Where(x => x.GetRelationType() == RelationType.Extends).FirstOrDefault().GetDestination();
        }

        /// <summary>
        ///     Return a node based on if the name is in the edges list
        /// </summary>
        /// <param name="node">The node witch it should check</param>
        /// <param name="name">The expected name</param>
        /// <returns>The node that has this name</returns>
        public static IEntityNode GetEdgeNode(this IEntityNode node, string name)
        {
            return node.GetRelations().Where(x => x.GetDestination().GetName() == name).FirstOrDefault().GetDestination();
        }

        /// <summary>
        ///     Returns a boolean based on if the modifier is equal to the expected modifier
        /// </summary>
        /// <param name="entityNode"></param>
        /// <param name="modifier"></param>
        /// <returns></returns>
        public static bool CheckModifier(this IEntityNode entityNode, string modifier)
        {
            return entityNode.GetModifiers().Any(x => x.ToString().IsEqual(modifier));
        }

        /// <summary>
        ///     Returns a boolean based on if the TypeDeclaration of a node is equal to the expected TypeDeclaration
        /// </summary>
        /// <param name="entityNode"></param>
        /// <param name="nodeType"></param>
        /// <returns></returns>
        public static bool CheckTypeDeclaration(this IEntityNode entityNode, EntityNodeType nodeType)
        {
            return entityNode.GetEntityNodeType().Equals(nodeType);
        }

        /// <summary>
        ///     Function thats checks the relation with another entitynode.
        /// </summary>
        /// <param name="entityNode">The entitynode witch it should check</param>
        /// <param name="relationType">The expected relation type</param>
        /// <param name="amount">The expected relation type</param>
        /// <returns></returns>
        public static bool CheckMinimalAmountOfRelationTypes(this IEntityNode entityNode, RelationType relationType, int amount)
        {
            return entityNode.GetRelations().Where(x => x.GetRelationType().Equals(relationType)).Count() >= amount;
        }
    }
}
