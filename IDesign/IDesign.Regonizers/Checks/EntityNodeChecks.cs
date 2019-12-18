using System.Linq;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Recognizers.Checks
{
    public static class EntityNodeChecks
    {
        /// <summary>
        ///     Return a boolean based on if node implements an interface with the given name.
        /// </summary>
        /// <param name="node">The node witch it should check</param>
        /// <param name="name">The expected name of the interface</param>
        /// <returns>The field is the type that is given in the function</returns>
        public static bool ImplementsInterface(this IEntityNode node, string name)
        {
            if (ClassImlementsInterface(node, name)) return true;

            if (CheckMinimalAmountOfRelationTypes(node, RelationType.Extends, 1))
                return ImplementsInterface(GetExtends(node), name);
            return false;
        }

        /// <summary>
        /// Return a boolean based on if the given method is implemented in a interface of the given node
        /// </summary>
        /// <param name="node">The node which should have the interface which contains the method</param>
        /// <param name="method">The method which it should check</param>
        /// <returns>The method is from an interface of the given node</returns>
        public static bool ClassImlementsInterfaceMethod(this IEntityNode node, IMethod method)
        {
            foreach (var interFace in node.GetRelations().Where(x => x.GetRelationType() == RelationType.Implements))
            {
                if (InterfaceImplementsMethod(interFace.GetDestination(), method))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Return a boolean based on if the given method is implemented in a interface of the given node
        /// </summary>
        /// <param name="node">The node which should have the method</param>
        /// <param name="methodName">The name of the method</param>
        /// <param name="amountOfParams">The amount of parameters the method should have</param>
        /// <returns>The method is found in the node</returns>
        public static bool MethodInEntityNode(this IEntityNode node, string methodName, int amountOfParams){

            if (node.GetMethods().Any(x => x.CheckMethodIdentifier(methodName) && x.GetParameter().Parameters.Count == amountOfParams))
            {
                return true;
            }
            foreach (var relation in node.GetRelations().Where(x => x.GetRelationType() == RelationType.Extends || x.GetRelationType() == RelationType.Implements))
            {
                if (relation.GetDestination().MethodInEntityNode(methodName, amountOfParams))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Return a boolean based on if the given node has a method with that name
        /// </summary>
        /// <param name="node">The given node</param>
        /// <param name="method">The method that the node should have</param>
        /// <returns>The given node has the given method</returns>
        public static bool InterfaceImplementsMethod(this IEntityNode node, IMethod method)
        {
            return node.GetMethods().Any(x => x.IsEquals(method));
        }

        /// <summary>
        ///     Return a boolean based on if the given node extends the node with the name
        /// </summary>
        /// <param name="node">The node it should check</param>
        /// <param name="name">The expected name of the node it should extend</param>
        /// <returns>The field is the type that is given in the function</returns>
        public static bool ExtendsClass(this IEntityNode node, string name)
        {
            if (CheckMinimalAmountOfRelationTypes(node, RelationType.Extends, 1))
            {
                if (GetExtends(node).GetName() == name) return true;
                return ExtendsClass(GetExtends(node), name);
            }

            return false;
        }

        //helper functions
        /// <summary>
        ///     Return a boolean based on if the given node directly implements the interface name given.
        /// </summary>
        /// <param name="node">The node witch it should check</param>
        /// <param name="name">The expected name of the interface</param>
        /// <returns>The node has the interface with that name</returns>
        public static bool ClassImlementsInterface(this IEntityNode node, string name)
        {
            return node.GetRelations()
                .Any(x => x.GetRelationType() == RelationType.Implements && x.GetDestination().GetName() == name);
        }

        /// <summary>
        ///     Return a class which is extended by this node.
        /// </summary>
        /// <param name="node">The node witch it should check</param>
        /// <returns>The node that is extended by the given node</returns>
        public static IEntityNode GetExtends(this IEntityNode node)
        {
            return node.GetRelations().Where(x => x.GetRelationType() == RelationType.Extends).FirstOrDefault()
                .GetDestination();
        }

        /// <summary>
        ///     Return a node based on if the name is in the edges list
        /// </summary>
        /// <param name="node">The node witch it should check</param>
        /// <param name="name">The expected name</param>
        /// <returns>The node that has this name</returns>
        public static IEntityNode GetEdgeNode(this IEntityNode node, string name)
        {
            try
            {
                node = node.GetRelations().Where(x => x.GetDestination().GetName() == name).FirstOrDefault()
                             .GetDestination();
            }
            catch (Exception e)
            {
                _ = e.Message;
            }
            return node;

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

