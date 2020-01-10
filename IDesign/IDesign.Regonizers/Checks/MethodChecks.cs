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
        /// <summary>
        ///     Function thats checks the returntype of a method.
        /// </summary>
        /// <param name="methodSyntax">The method witch it should check</param>
        /// <param name="returnType">The expected return type</param>
        /// <returns></returns>
        public static bool CheckReturnType(this IMethod methodSyntax, string returnType)
        {
            return methodSyntax.GetReturnType().IsEqual(returnType);
        }

        /// <summary>
        ///     Return a boolean based on if the given method is creational.
        /// </summary>
        /// <param name="methodSyntax">The method witch it should check</param>
        /// <returns></returns>
        public static bool CheckCreationalFunction(this IMethod methodSyntax)
        {
            return methodSyntax.GetBody().DescendantNodes().OfType<ObjectCreationExpressionSyntax>().Any();
        }

        /// <summary>
        ///     Return a boolean based on if the given member has an expected modifier.
        /// </summary>
        /// <param name="methodSyntax">The method witch it should check</param>
        /// <param name="modifier">The expected modifier</param>
        /// <returns></returns>
        public static bool CheckModifier(this IMethod methodSyntax, string modifier)
        {
            return methodSyntax.GetModifiers().Any(x => x.ToString().IsEqual(modifier));
        }

        /// <summary>
        ///     Return a boolean based on if the given method creates an object with the given type.
        /// </summary>
        /// <param name="methodSyntax">The method witch it should check</param>
        /// <param name="creationType">The expected creational type</param>
        /// <returns></returns>
        public static bool CheckCreationType(this IMethod methodSyntax, string creationType)
        {
            var body = methodSyntax.GetBody();
            if (body == null)
                return false;
            var creations = body.DescendantNodes().OfType<ObjectCreationExpressionSyntax>();
            foreach (var creationExpression in creations)
                if (creationExpression.Type is IdentifierNameSyntax name &&
                    name.Identifier.ToString().IsEqual(creationType))
                    return true;
            return false;
        }

        /// <summary>
        ///     Return a boolean based on if the given method returns a object it creates.
        /// </summary>
        /// <param name="methodSyntax">The method witch it should check</param>
        /// <returns></returns>
        public static bool CheckReturnTypeSameAsCreation(this IMethod methodSyntax)
        {
            return methodSyntax.CheckCreationType(methodSyntax.GetReturnType());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <param name="currentClass"></param>
        /// <returns></returns>
        public static bool IsInterfaceMethod(this IMethod methodSyntax, IEntityNode currentClass)
        {
            return currentClass.ClassImlementsInterfaceMethod(methodSyntax);
        }

        //helper functions
        /// <summary>
        ///     Return a list of all types that this function makes as strings.
        /// </summary>
        /// <param name="methodSyntax">The method witch it should check</param>
        /// <returns>all types that are created</returns>
        public static IEnumerable<string> GetCreatedTypes(this IMethod methodSyntax)
        {
            var result = new List<string>();
            if (methodSyntax.GetBody() == null)
                return result;
                var creations = methodSyntax.GetBody().DescendantNodes().OfType<ObjectCreationExpressionSyntax>();
                foreach (var creation in creations)
                {
                    var identifiers = creation.DescendantNodes().OfType<IdentifierNameSyntax>();
                    result.AddRange(identifiers.Select(y => y.Identifier.ToString()));
                }
            return result;
        }


        /// <summary>
        ///     Checks if a method calls a method in the given noe
        /// </summary>
        /// <param name="method">The method</param>
        /// <param name="node">The node in witch the method could be</param>
        /// <returns></returns>
        public static bool CheckIfMethodCallsMethodInNode(this IMethod method, IEntityNode node)
        {
            if (method.GetBody() == null)
                return false;
            var invocations = method.GetBody().DescendantNodes().OfType<InvocationExpressionSyntax>();
            foreach (var invocation in invocations)
            {
                var identifier = invocation.Expression.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>().FirstOrDefault();
                var arguments = invocation.ArgumentList.Arguments.Count;

                if (identifier != null && node.MethodInEntityNode(identifier.ToString(), arguments))
                    return true;
            }

            return false;
        }

        public static bool CheckIfMethodCallsBase(this IMethod method)
        {
            if (method.GetBody() == null)
                return false;

            return method.GetBody().DescendantNodes().OfType<BaseExpressionSyntax>().Any();
        }

        /// <summary>
        ///     Function thats checks if the parameters exist.
        /// </summary>
        /// <param name="methodSyntax">The methodsyntax witch it should check</param>
        /// <param name="parameters">The expected parameters</param>
        /// <returns></returns>
        public static bool CheckParameters(this IMethod methodSyntax, IEnumerable<string> parameters)
        {
            return parameters.Any(x => methodSyntax.GetParameterTypes().Any(y => x.Equals(y)));
        }

        /// <summary>
        ///     Function thats checks if the methods exist.
        /// </summary>
        /// <param name="methodSyntax">The methodsyntax witch it should check</param>
        /// <param name="methods">The expected methods</param>
        /// <returns></returns>
        public static bool CheckName(this IMethod methodSyntax, IEnumerable<IMethod> methods)
        {
            return methods.Any(x => x.GetName().Equals(methodSyntax.GetName()));
        }

        /// <summary>
        ///     Function thats checks if the argument exist.
        /// </summary>
        /// <param name="methodSyntax">The methodsyntax witch it should check</param>
        /// <param name="argument">The expected argument</param>
        /// <returns></returns>
        public static bool CheckArguments(this IMethod methodSyntax, string argument)
        {
            var parameters = methodSyntax.GetParameters().Where(y => y.Type.ToString().Equals(argument));
            if (parameters.Count() < 1)
                return false;
            return methodSyntax.GetArguments().Any(x => x.Equals(parameters.First().Identifier.ToString()));
        }

        /// Return a boolean based on if the method has the same name as the given string
        /// </summary>
        /// <param name="methodSyntax">The given method which should have the name</param>
        /// <param name="name">The name the method should have</param>
        /// <returns></returns>
        public static bool CheckMethodIdentifier(this IMethod methodSyntax, string name)
        {
            return (methodSyntax.GetName().Equals(name) && methodSyntax.GetType() == typeof(Method));
        }

        public static bool CheckFieldIsUsed(this IMethod method, string fieldName)
        {
            if (method.GetBody() == null)
                return false;
            return method.GetBody().DescendantNodes().OfType<IdentifierNameSyntax>().Any(x => x.Identifier.ToString().IsEqual(fieldName));
        }

        /// <summary>
        /// Return a boolean based on if the Method parameters are the same as the given string
        /// </summary>
        /// <param name="methodSyntax">The method it should check the parameters from</param>
        /// <param name="parameters">The given parameters types it should have</param>
        /// <returns>The method has the same parameters as the given string</returns>
        public static bool CheckMethodParameterTypes(this IMethod methodSyntax, string parameters)
        {
            return methodSyntax.GetParameter().ToString().Equals(parameters);
        }

        /// <summary>
        /// Return a boolean based on if the given method is the same type as the other method
        /// </summary>
        /// <param name="methodSyntax">The method it should check</param>
        /// <param name="compareMethod">The given method it should compare to</param>
        /// <returns>The methods are the same type</returns>
        public static bool IsEquals(this IMethod methodSyntax, IMethod compareMethod)
        {
            return (methodSyntax.CheckMethodIdentifier(compareMethod.GetName())
                && methodSyntax.CheckMethodParameterTypes(compareMethod.GetParameter().ToString())
                && methodSyntax.CheckReturnType(compareMethod.GetReturnType()));
        }
    }
}
