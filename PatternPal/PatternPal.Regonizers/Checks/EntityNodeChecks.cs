using System.Collections.Generic;
using System.Linq;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;

namespace PatternPal.Recognizers.Checks
{
    public static class EntityNodeChecks
    {
        public static bool ImplementsInterface(this IEntity node, string name)
        {
            return ClassImplementsInterface(node, name) || (
                CheckMinimalAmountOfRelationTypes(node, RelationType.Extends, 1) &&
                ImplementsInterface(GetExtends(node), name)
            );
        }

        public static bool ClassImplementsInterfaceMethod(this IEntity node, IMethod method)
        {
            return node.GetRelations()
                .Where(x => x.GetRelationType() == RelationType.Implements && x.Node2Entity != null)
                .Where(interFace => InterfaceImplementsMethod(interFace.Node2Entity, method))
                .Select(interFace => new { })
                .Any();
        }

        public static bool MethodInEntityNode(this IEntity node, string methodName, int amountOfParams)
        {
            if (node.GetAllMethods().Any(
                    x => x.CheckMethodIdentifier(methodName) &&
                         x.GetParameters().Count() == amountOfParams
                ))
            {
                return true;
            }

            return node.GetRelations()
                .Where(
                    x => (x.GetRelationType() == RelationType.Extends ||
                         x.GetRelationType() == RelationType.Implements) &&
                         x.Node2Entity != null
                )
                .Where(relation => relation.Node2Entity.MethodInEntityNode(methodName, amountOfParams))
                .Select(relation => new { })
                .Any();
        }

        public static bool InterfaceImplementsMethod(this IEntity node, IMethod method)
        {
            return node.GetAllMethods().Any(x => x.IsEquals(method));
        }

        public static bool ExtendsClass(this IEntity node, string name)
        {
            if (CheckMinimalAmountOfRelationTypes(node, RelationType.Extends, 1))
            {
                return GetExtends(node).GetName() == name || ExtendsClass(GetExtends(node), name);
            }

            return false;
        }

        public static bool ClassImplementsInterface(this IEntity node, string name)
        {
            return node.GetRelations()
                .Any(x => x.GetRelationType() == RelationType.Implements && 
                   x.Node2Entity != null && x.Node2Entity.GetName() == name);
        }

        public static IEntity GetExtends(this IEntity node)
        {
            return node
                .GetRelations()
                .FirstOrDefault(x => x.GetRelationType() == RelationType.Extends &&
                    x.Node2Entity != null)
                ?.Node2Entity;
        }

        public static bool Extends(this IEntity node)
        {
            return node.CheckMinimalAmountOfRelationTypes(RelationType.Extends, 1);
        }

        public static IEntity GetEdgeNode(this IEntity node, string name)
        {
            return node = node
                .GetRelations()
                .FirstOrDefault(x => x.Node2Entity != null && x.Node2Entity.GetName() == name)
                ?.Node2Entity;
        }

        public static bool CheckTypeDeclaration(this IEntity entityNode, EntityType nodeType)
        {
            return entityNode.GetEntityType().Equals(nodeType);
        }

        public static bool CheckEntityType(this IEntity entityNode, EntityType nodeType)
        {
            return entityNode.GetEntityType().Equals(nodeType);
        }

        public static bool CheckEntityNodeModifier(this IEntity entityNode, string modifier)
        {
            return entityNode.GetModifiers()
                .Any(x => x.ToString().CheckIfTwoStringsAreEqual(modifier));
        }

        public static bool CheckRelationType(this IEntity entityNode, RelationType relationType)
        {
            return entityNode.CheckMinimalAmountOfRelationTypes(relationType, 1);
        }

        public static bool CheckIsAbstractClass(this IEntity entityNode)
        {
            return entityNode.CheckTypeDeclaration(EntityType.Class) && entityNode.CheckModifier("abstract");
        }

        public static bool CheckIsAbstractClassOrInterface(this IEntity entityNode)
        {
            return entityNode.CheckTypeDeclaration(EntityType.Interface) || entityNode.CheckIsAbstractClass();
        }

        public static bool CheckMinimalAmountOfRelationTypes(
            this IEntity entityNode,
            RelationType relationType,
            int amount
        )
        {
            return entityNode.GetRelations()
                .Count(x => x.GetRelationType().Equals(relationType)) >= amount;
        }

        public static bool CheckMinimalAmountOfMethodsWithParameter(
            this IEntity node,
            List<string> parameters,
            int amount
        )
        {
            return node.GetAllMethods()
                .Count(x => x.CheckParameters(parameters)) >= amount;
        }

        public static bool CheckMaximumAmountOfMethodsWithParameter(
            this IEntity node,
            List<string> parameters,
            int amount
        )
        {
            return node.GetAllMethods()
                .Count(x => x.CheckParameters(parameters)) < amount;
        }

        public static bool CheckMaximumAmountOfRelationTypes(
            this IEntity entityNode,
            RelationType relationType,
            int amount
        )
        {
            return entityNode.GetRelations()
                .Count(x => x.GetRelationType().Equals(relationType)) <= amount;
        }

        public static bool CheckMinimalAmountOfMethods(this IEntity node, int amount)
        {
            return node.GetAllMethods().Count() >= amount;
        }
    }
}
