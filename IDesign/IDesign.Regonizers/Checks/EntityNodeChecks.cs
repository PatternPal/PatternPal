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
        public static bool ImplementsInterface(this IEntityNode node, string name)
        {
            return ClassImplementsInterface(node, name)
                ? true
                : CheckMinimalAmountOfRelationTypes(node, RelationType.Extends, 1) ? ImplementsInterface(GetExtends(node), name) : false;
        }

        public static bool ClassImplementsInterfaceMethod(this IEntityNode node, IMethod method)
        {
            foreach (var _ in from interFace in node.GetRelations()
                                                    .Where(x => x.GetRelationType() == RelationType.Implements)
                              where InterfaceImplementsMethod(interFace.GetDestination(), method)
                              select new { })
            {
                return true;
            }
            return false;
        }

        public static bool MethodInEntityNode(this IEntityNode node, string methodName, int amountOfParams)
        {
            if (node.GetMethodsAndProperties().Any(x => x.CheckMethodIdentifier(methodName) &&
            x.GetParameter().Parameters.Count == amountOfParams))
            {
                return true;
            }

            foreach (var _ in from relation in node.GetRelations().Where(x => x.GetRelationType() == RelationType.Extends ||
                              x.GetRelationType() == RelationType.Implements)
                              where relation.GetDestination()
                                            .MethodInEntityNode(methodName, amountOfParams)
                              select new { })
            {
                return true;
            }

            return false;
        }

        public static bool InterfaceImplementsMethod(this IEntityNode node, IMethod method)
        {
            return node.GetMethodsAndProperties().Any(x => x.IsEquals(method));
        }

        public static bool ExtendsClass(this IEntityNode node, string name)
        {
            if (CheckMinimalAmountOfRelationTypes(node, RelationType.Extends, 1))
            {
                return GetExtends(node).GetName() == name ? true : ExtendsClass(GetExtends(node), name);
            }
            return false;
        }

        public static bool ClassImplementsInterface(this IEntityNode node, string name)
        {
            return node.GetRelations()
                       .Any(x => x.GetRelationType() == RelationType.Implements && x.GetDestination().GetName() == name);
        }

        public static IEntityNode GetExtends(this IEntityNode node)
        {
            return node.GetRelations()
                       .Where(x => x.GetRelationType() == RelationType.Extends)
                       .FirstOrDefault()
                       .GetDestination();
        }

        public static bool Extends(this IEntityNode node)
        {
            return node.CheckMinimalAmountOfRelationTypes(RelationType.Extends, 1);
        }

        public static IEntityNode GetEdgeNode(this IEntityNode node, string name)
        {
            try
            {
                node = node.GetRelations()
                           .Where(x => x.GetDestination().GetName() == name)
                           .FirstOrDefault()
                           .GetDestination();
            }
            catch (Exception e)
            {
                _ = e.Message;
            }
            return node;
        }

        public static bool CheckModifier(this IEntityNode entityNode, string modifier)
        {
            return entityNode.GetModifiers().Any(x => x.ToString().CheckIfTwoStringsAreEqual(modifier));
        }

        public static bool CheckTypeDeclaration(this IEntityNode entityNode, EntityNodeType nodeType)
        {
            return entityNode.GetEntityNodeType().Equals(nodeType);
        }

        public static bool CheckEntityNodeType(this IEntityNode entityNode, EntityNodeType nodeType)
        {
            return entityNode.GetEntityNodeType().Equals(nodeType);
        }

        public static bool CheckEntityNodeModifier(this IEntityNode entityNode, string modifier)
        {
            return entityNode.GetTypeDeclarationSyntax().Modifiers.Any(x => x.ToString().CheckIfTwoStringsAreEqual(modifier));
        }

        public static bool CheckRelationType(this IEntityNode entityNode, RelationType relationType)
        {
            return entityNode.CheckMinimalAmountOfRelationTypes(relationType, 1);
        }

        public static bool CheckMinimalAmountOfRelationTypes(this IEntityNode entityNode, RelationType relationType, int amount)
        {
            return entityNode.GetRelations().Where(x => x.GetRelationType().Equals(relationType)).Count() >= amount;
        }

        public static bool CheckMinimalAmountOfMethodsWithParameter(this IEntityNode node, List<string> parameters, int amount)
        {
            return node.GetMethodsAndProperties().Where(x => x.CheckParameters(parameters)).Count() >= amount;
        }

        public static bool CheckMaximumAmountOfMethodsWithParameter(this IEntityNode node, List<string> parameters, int amount)
        {
            return node.GetMethodsAndProperties().Where(x => x.CheckParameters(parameters)).Count() < amount;
        }

        public static bool CheckMaximumAmountOfRelationTypes(this IEntityNode entityNode, RelationType relationType, int amount)
        {
            return entityNode.GetRelations().Where(x => x.GetRelationType().Equals(relationType)).Count() <= amount;
        }

        public static bool CheckMinimalAmountOfMethods(this IEntityNode node, int amount)
        {
            return node.GetMethodsAndProperties().Count() >= amount;
        }
    }
}

