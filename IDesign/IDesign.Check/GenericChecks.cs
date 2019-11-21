using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace IDesign.Checks
{
    public static class GenericChecks
    {
        /// <summary>
        /// Return a boolean based on if the given member has an expected modifier
        /// </summary>
        /// <param name="membersyntax">The member witch it should check</param>
        /// <param name="modifier">The expected modifier</param>
        /// <returns></returns>
        /// 
        public static bool CheckMemberModifier(this MemberDeclarationSyntax memberSyntax, string modifier)
        {
            return memberSyntax.Modifiers.Where(x => x.ToString() == modifier).Count() > 0;
        }
    }
}
