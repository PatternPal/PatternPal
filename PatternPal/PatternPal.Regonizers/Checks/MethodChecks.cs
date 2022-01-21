using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;
using SyntaxTree.Models.Members.Constructor;
using SyntaxTree.Models.Members.Method;

namespace PatternPal.Recognizers.Checks
{
    public static class MethodChecks
    {
        public static bool CheckReturnType(this IMethod methodSyntax, string returnType)
        {
            if (methodSyntax.GetReturnType() == null)
            {
                return returnType.Equals("void");
            }

            return methodSyntax.GetReturnType()
                .ToString()
                .CheckIfTwoStringsAreEqual(returnType);
        }

        public static bool CheckCreationalFunction(this IMethod methodSyntax)
        {
            return methodSyntax.GetBody()
                .DescendantNodes()
                .OfType<ObjectCreationExpressionSyntax>()
                .Any();
        }

        public static bool CheckModifier(this IModified methodSyntax, string modifier)
        {
            return methodSyntax.GetModifiers()
                .Any(x => x.ToString().CheckIfTwoStringsAreEqual(modifier));
        }

        public static bool CheckCreationType(this IMethod methodSyntax, string creationType)
        {
            var body = methodSyntax.GetBody();

            if (body == null) return false;

            var creations = body.DescendantNodes().OfType<ObjectCreationExpressionSyntax>();

            return creations
                .Where(
                    creationExpression => creationExpression.Type is IdentifierNameSyntax name &&
                                          name.Identifier.ToString().CheckIfTwoStringsAreEqual(creationType)
                )
                .Select(creationExpression => new { })
                .Any();
        }

        public static bool CheckReturnTypeSameAsCreation(this IMethod methodSyntax)
        {
            return methodSyntax.CheckCreationType(methodSyntax.GetReturnType()?.ToString() ?? "void");
        }

        public static bool IsInterfaceMethod(this IMethod methodSyntax, IEntity currentClass)
        {
            return currentClass.ClassImplementsInterfaceMethod(methodSyntax);
        }

        public static IEnumerable<string> GetCreatedTypes(this IMethod methodSyntax)
        {
            var result = new List<string>();
            if (methodSyntax.GetBody() == null) return result;

            var creations = methodSyntax.GetBody().DescendantNodes().OfType<ObjectCreationExpressionSyntax>();
            foreach (var identifiers in from creation in creations
                     let identifiers = creation.DescendantNodes().OfType<IdentifierNameSyntax>()
                     select identifiers)
            {
                result.AddRange(identifiers.Select(y => y.Identifier.ToString()));
            }

            return result;
        }

        public static bool CheckIfMethodCallsMethodInNode(this IMethod method, IEntity node)
        {
            if (method.GetBody() == null)
                return false;

            var invocations = method.GetBody().DescendantNodes().OfType<InvocationExpressionSyntax>();
            return invocations
                .Select(
                    invocation => new
                    {
                        invocation,
                        identifier = invocation.Expression.DescendantNodesAndSelf()
                            .OfType<IdentifierNameSyntax>()
                            .FirstOrDefault()
                    }
                )
                .Select(@t => new { @t, arguments = @t.invocation.ArgumentList.Arguments.Count })
                .Where(
                    @t => @t.@t.identifier != null && node.MethodInEntityNode(
                        @t.@t.identifier.ToString(), @t.arguments
                    )
                )
                .Select(@t => new { })
                .Any();
        }

        public static bool CheckIfMethodCallsBase(this IMethod method)
        {
            return method.GetBody() != null && method.GetBody().DescendantNodes().OfType<BaseExpressionSyntax>().Any();
        }

        public static bool CheckParameters(this IMethod methodSyntax, IEnumerable<string> parameters)
        {
            return parameters.Any(
                x => methodSyntax.GetParameters()
                    .Select(y => y.ToString())
                    .Any(y => x.Equals(y))
            );
        }

        public static bool CheckIfNameExists(this IMethod methodSyntax, IEnumerable<IMethod> methods)
        {
            return methods.Any(x => x.GetName().Equals(methodSyntax.GetName()));
        }

        public static bool CheckIfArgumentsExists(this IMethod methodSyntax, string argument)
        {
            if (!(methodSyntax is ConstructorMethod constructorMethod))
            {
                return false;
            }

            var constructor = constructorMethod.constructor;

            var parameters = constructor.GetParameters()
                .Where(y => y.ToString().Equals(argument))
                .ToList();

            return parameters.Any() && constructor.GetArguments().Any(x => parameters.Any(y => x.Equals(y.ToString())));
        }

        public static bool CheckMethodIdentifier(this IMethod methodSyntax, string name)
        {
            return methodSyntax.GetName().Equals(name) && methodSyntax.GetType() == typeof(Method);
        }

        public static bool CheckFieldIsUsed(this IMethod method, string fieldName)
        {
            return method.GetBody() != null && method.GetBody()
                .DescendantNodes()
                .OfType<IdentifierNameSyntax>()
                .Any(x => x.Identifier.ToString().CheckIfTwoStringsAreEqual(fieldName));
        }

        public static bool CheckMethodParameterTypes(this IMethod methodSyntax, IEnumerable<TypeSyntax> types)
        {
            return methodSyntax.GetParameters()
                .Select(t => t.ToString())
                .Zip(types, (s, t) => s.Equals(t.ToString()))
                .All(b => b);
        }

        public static bool IsEquals(this IMethod methodSyntax, IMethod compareMethod)
        {
            return methodSyntax.CheckMethodIdentifier(compareMethod.GetName())
                   && methodSyntax.CheckMethodParameterTypes(compareMethod.GetParameters())
                   && methodSyntax.CheckReturnType(compareMethod.GetReturnType()?.ToString() ?? "void");
        }
    }
}
