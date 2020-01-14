using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Models;

namespace IDesign.Recognizers.Checks
{
    public static class MethodChecks
    {
        public static bool CheckReturnType(this IMethod methodSyntax, string returnType)
        {
            return methodSyntax.GetReturnType()
                               .CheckIfTwoStringsAreEqual(returnType);
        }

        public static bool CheckCreationalFunction(this IMethod methodSyntax)
        {
            return methodSyntax.GetBody()
                               .DescendantNodes()
                               .OfType<ObjectCreationExpressionSyntax>()
                               .Any();
        }

        public static bool CheckModifier(this IMethod methodSyntax, string modifier)
        {
            return methodSyntax.GetModifiers()
                               .Any(x => x.ToString().CheckIfTwoStringsAreEqual(modifier));
        }

        public static bool CheckCreationType(this IMethod methodSyntax, string creationType)
        {
            var body = methodSyntax.GetBody();

            if (body != null)
            {
                var creations = body.DescendantNodes().OfType<ObjectCreationExpressionSyntax>();
                foreach (var _ in from creationExpression in creations
                                  where creationExpression.Type is IdentifierNameSyntax name &&
          name.Identifier.ToString().CheckIfTwoStringsAreEqual(creationType)
                                  select new { })
                {
                    return true;
                }
            }
            return false;
        }

        public static bool CheckReturnTypeSameAsCreation(this IMethod methodSyntax)
        {
            return methodSyntax.CheckCreationType(methodSyntax.GetReturnType());
        }

        public static bool IsInterfaceMethod(this IMethod methodSyntax, IEntityNode currentClass)
        {
            return currentClass.ClassImlementsInterfaceMethod(methodSyntax);
        }

        public static IEnumerable<string> GetCreatedTypes(this IMethod methodSyntax)
        {
            var result = new List<string>();
            if (methodSyntax.GetBody() != null)
            {
                var creations = methodSyntax.GetBody().DescendantNodes().OfType<ObjectCreationExpressionSyntax>();
                foreach (var identifiers in from creation in creations
                                            let identifiers = creation.DescendantNodes().OfType<IdentifierNameSyntax>()
                                            select identifiers)
                {
                    result.AddRange(identifiers.Select(y => y.Identifier.ToString()));
                }
            }
            return result;
        }

        public static bool CheckIfMethodCallsMethodInNode(this IMethod method, IEntityNode node)
        {
            if (method.GetBody() != null)
            {
                var invocations = method.GetBody().DescendantNodes().OfType<InvocationExpressionSyntax>();
                foreach (var _ in from invocation in invocations
                                  let identifier = invocation.Expression.DescendantNodesAndSelf()
                                                                        .OfType<IdentifierNameSyntax>()
                                                                        .FirstOrDefault()
                                  let arguments = invocation.ArgumentList.Arguments.Count
                                  where identifier != null && node.MethodInEntityNode(identifier.ToString(), arguments)
                                  select new { })
                {
                    return true;
                }
            }
            return false;
        }

        public static bool CheckIfMethodCallsBase(this IMethod method)
        {
            if (method.GetBody() == null)
                return false;

            return method.GetBody().DescendantNodes().OfType<BaseExpressionSyntax>().Any();
        }


        public static bool CheckParameters(this IMethod methodSyntax, IEnumerable<string> parameters)
        {
            return parameters.Any(x => methodSyntax.GetParameterTypes()
                                                   .Any(y => x.Equals(y)));
        }

        public static bool CheckIfNameExists(this IMethod methodSyntax, IEnumerable<IMethod> methods)
        {
            return methods.Any(x => x.GetName().Equals(methodSyntax.GetName()));
        }

        public static bool CheckIfArgumentsExists(this IMethod methodSyntax, string argument)
        {
            var parameters = methodSyntax.GetParameters().Where(y => y.Type.ToString().Equals(argument));
            return parameters.Count() < 1 ? false : methodSyntax.GetArguments().Any(x => x.Equals(parameters.First().Identifier.ToString()));
        }

        public static bool CheckMethodIdentifier(this IMethod methodSyntax, string name)
        {
            return (methodSyntax.GetName().Equals(name) && methodSyntax.GetType() == typeof(Method));
        }

        public static bool CheckFieldIsUsed(this IMethod method, string fieldName)
        {
            return method.GetBody() == null
                ? false
                : method.GetBody()
                         .DescendantNodes()
                         .OfType<IdentifierNameSyntax>()
                         .Any(x => x.Identifier.ToString().CheckIfTwoStringsAreEqual(fieldName));
        }

        public static bool CheckMethodParameterTypes(this IMethod methodSyntax, string parameters)
        {
            return methodSyntax.GetParameter()
                               .ToString()
                               .Equals(parameters);
        }

        public static bool IsEquals(this IMethod methodSyntax, IMethod compareMethod)
        {
            return methodSyntax.CheckMethodIdentifier(compareMethod.GetName())
                   && methodSyntax.CheckMethodParameterTypes(compareMethod.GetParameter().ToString())
                   && methodSyntax.CheckReturnType(compareMethod.GetReturnType());
        }
    }
}
