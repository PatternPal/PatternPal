using System;

namespace PatternPal.Recognizers.Checks
{
    public static class CompareUtils
    {
        public static bool CheckIfTwoStringsAreEqual(this string s1, string s2)
        {
            return string.Equals(s1, s2, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
