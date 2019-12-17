using System.Collections.Generic;
﻿using System.Linq;
using IDesign.Recognizers.Abstractions;

namespace IDesign.Recognizers.Checks
{
    public static class FieldChecks
    {
        /// <summary>
        ///     Return a boolean based on if the given field is an expected type.
        /// </summary>
        /// <param name="fieldSyntax">The field witch it should check</param>
        /// <param name="type">The expected type</param>
        /// <returns>The field is the type that is given in the function</returns>
        public static bool CheckFieldType(this IField fieldSyntax, IEnumerable<string> types)
        {
            return types.Any(x => x.Equals(fieldSyntax.GetFieldType().ToString()));
        }

        /// <summary>
        ///     Return a boolean based on if the given field has an expected modifier.
        /// </summary>
        /// <param name="field">The field witch it should check</param>
        /// <param name="modifier">The expected modifier</param>
        /// <returns>The field has the modifier that is given in the function</returns>
        public static bool CheckMemberModifier(this IField field, string modifier)
        {
            return field.GetModifiers().Any(x => x.ToString().IsEqual(modifier));
        }
    }
}