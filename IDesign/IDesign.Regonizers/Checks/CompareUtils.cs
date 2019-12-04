using System;

namespace IDesign.Recognizers
{
    public static class CompareUtils
    {
        /// <summary>
        ///     Function that if two strings are equal.
        ///     Not case sensitive.
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool IsEqual(this string s1, string s2)
        {
            return string.Equals(s1, s2, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}