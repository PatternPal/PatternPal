using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxTree.Abstractions.Members;

namespace PatternPal.Recognizers.Checks
{
    // TODO QA: XML-comment
    public static class FieldChecks
    {
        public static bool CheckFieldTypeGeneric(this IField fieldSyntax, IEnumerable<string> types)
        {
            return CheckFieldTypeGeneric(fieldSyntax.GetFieldType(), types);
        }

        private static bool CheckFieldTypeGeneric(TypeSyntax type, IEnumerable<string> types)
        {
            List<string> list = types.ToList();
            if (list.Any(x => x.Equals(type.ToString())))
            {
                return true;
            }

            GenericNameSyntax genericTypeIfApplicable = type as GenericNameSyntax;
            if (genericTypeIfApplicable == null)
            {
                return false;
            }

            foreach (TypeSyntax typeSyntax in genericTypeIfApplicable.TypeArgumentList.Arguments)
            {
                if (CheckFieldTypeGeneric(typeSyntax, list))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
