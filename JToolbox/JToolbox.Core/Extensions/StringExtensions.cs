using System;

namespace JToolbox.Core.Extensions
{
    public static class StringExtensions
    {
        public static string Truncate(this string value, int length)
        {
            return value.Substring(0, Math.Min(value.Length, length));
        }

        public static bool IgnoreCaseContains(this string val1, string val2)
        {
            return val1.IndexOf(val2, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static bool IgnoreCaseEquals(this string val1, string val2)
        {
            return val1.Equals(val2, StringComparison.OrdinalIgnoreCase);
        }
    }
}