using System.Collections.Generic;
﻿using System.Linq;
using IDesign.Recognizers.Abstractions;

namespace IDesign.Recognizers.Checks
{
    public static class FieldChecks
    {
        public static bool CheckFieldType(this IField fieldSyntax, IEnumerable<string> types)
        {
            return types.Any(x => x.Equals(fieldSyntax.GetFieldType().ToString()));
        }

        public static bool CheckMemberModifier(this IField field, string modifier)
        {
            return field.GetModifiers().Any(x => x.ToString().CheckIfTwoStringsAreEqual(modifier));
        }
    }
}