using System;
using System.Collections.Generic;
using System.Text;
using IDesign.Recognizers.Abstractions;
using System.Linq;
 using IDesign.Recognizers.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Recognizers.Checks
{
    public static class ClassChecks
    {
        /// <summary>
        ///     Return a boolean based on if node implements an interface with the given name.
        /// </summary>
        /// <param name="node">The node witch it should check</param>
        /// <param name="name">The expected name of the interface</param>
        /// <returns>The field is the type that is given in the function</returns>
        public static bool ImplementsInterface(this IEntityNode node, string name)
        {
            if(HasInterface(node, name))
                {
                return true;
            }
            if (Extends(node))
            {
                return ImplementsInterface(GetExtends(node), name);
            }
            else
            {
                return false;
            }
        }

        
        public static bool ClassImlementsInterface(this IEntityNode node, IMethod method)
        {
            var implements = false;
            foreach(var interFace in node.GetRelations().Where(x => x.GetRelationType() == RelationType.Implements))
            {
                if(InterfaceImplementsMethod(interFace.GetDestination(), method))
                {
                    implements = true;
                }
            }
            return implements;
        }
        public static bool InterfaceImplementsMethod(this IEntityNode node, IMethod method)
        {
            return node.GetMethods().Any(x => x.MethodComparing(method));
        }

        /// <summary>
        ///     Return a boolean based on if the given node extends the node with the name
        /// </summary>
        /// <param name="node">The node it should check</param>
        /// <param name="name">The expected name of the node it should extend</param>
        /// <returns>The field is the type that is given in the function</returns>
        public static bool ExtendsClass(this IEntityNode node, string name)
        {
            if (Extends(node))
            {
                if(GetExtends(node).GetName() == name)
                {
                    return true;
                }
                return ExtendsClass(GetExtends(node), name);
            }
            else { return false; }
        }

        //helper functions
        /// <summary>
        ///     Return a boolean based on if the given node directly implements the interface name given.
        /// </summary>
        /// <param name="node">The node witch it should check</param>
        /// <param name="name">The expected name of the interface</param>
        /// <returns>The node has the interface with that name</returns>
        public static bool HasInterface(this IEntityNode node, string name)
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
            return node.GetRelations().Where(x => x.GetRelationType() == RelationType.Extends).FirstOrDefault().GetDestination();
        }

        /// <summary>
        ///     Return a boolean based on if the given node extends a node
        /// </summary>
        /// <param name="node">The node witch it should check</param>
        /// <returns>The class has an extends/returns>
        public static bool Extends(this IEntityNode node)
        {
            return node.GetRelations().Any(x => x.GetRelationType() == RelationType.Extends);
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
    }
}
