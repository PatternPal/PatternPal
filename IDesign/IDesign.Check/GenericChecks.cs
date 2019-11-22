using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace IDesign.Checks
{
    public static class GenericChecks
    {
        /// <summary>
        /// Function that checks the modifier of a member
        /// </summary>
        /// <param name="membersyntax">The member witch it should check</param>
        /// <param name="modifier">The expected modifier</param>
        /// <returns>
        /// Return a boolean based on if the given member has an expected modifier
        /// </returns>
        public static bool CheckMemberModifier(this MemberDeclarationSyntax membersyntax, string modifier)
        {
            return membersyntax.Modifiers.Where(x => x.ToString().IsEqual(modifier)).Any();
        }
    }
}
