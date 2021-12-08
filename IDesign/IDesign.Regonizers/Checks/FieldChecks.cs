using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Recognizers.Checks
{
    public static class FieldChecks
    {
        public static bool CheckFieldType(this IField fieldSyntax, IEnumerable<string> types)
        {
            return types.Any(x => x.Equals(fieldSyntax.GetFieldType().ToString()));
        }

        public static bool CheckFieldTypeGeneric(this IField fieldSyntax, IEnumerable<string> types)
        {
            return CheckFieldTypeGeneric(fieldSyntax.GetFieldType(), types);
        }

        private static bool CheckFieldTypeGeneric(TypeSyntax type, IEnumerable<string> types)
        {
            var list = types.ToList();
            if (list.Any(x => x.Equals(type.ToString()))) return true;
            var genericTypeIfApplicable = type as GenericNameSyntax;
            if (genericTypeIfApplicable == null) return false;

            foreach (var typeSyntax in genericTypeIfApplicable.TypeArgumentList.Arguments)
            {
                if (CheckFieldTypeGeneric(typeSyntax, list)) return true;
            }

            return false;
        }

        public static bool CheckMemberModifier(this IField field, string modifier)
        {
            return field.GetModifiers().Any(x => x.ToString().CheckIfTwoStringsAreEqual(modifier));
        }
    }
}