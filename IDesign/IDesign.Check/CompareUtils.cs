using System;

namespace IDesign.Checks
{
    public static class CompareUtils
    {
        public static bool IsEqual(this string s1, string s2)
        {
            return string.Equals(s1, s2, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}