using System.Globalization;

namespace Mil.Paperwork.Common.Helpers
{
    public static class SimpleNumberParser
    {
        /// <summary>
        /// Very small, permissive parser:
        /// - rejects empty or whitespace
        /// - rejects strings that contain both '.' and ','
        /// - replaces ',' with '.' and parses using InvariantCulture
        /// - accepts leading +/-, digits, optional single decimal separator, digits
        /// </summary>
        public static bool TryParse(string input, out decimal result)
        {
            result = 0m;
            if (string.IsNullOrWhiteSpace(input)) return false;

            string s = input.Trim();

            // quick reject: both separators present -> ambiguous
            if (s.IndexOf('.') >= 0 && s.IndexOf(',') >= 0)
                return false;

            // remove spaces (common when copying)
            s = s.Replace(" ", string.Empty);

            // accept parentheses style negative: (123,45) -> -123.45
            bool parenthesisNegative = false;
            if (s.StartsWith("(") && s.EndsWith(")"))
            {
                parenthesisNegative = true;
                s = s.Substring(1, s.Length - 2).Trim();
            }

            if (s.Length == 0) return false;

            // normalize comma to dot if present
            if (s.IndexOf(',') >= 0)
                s = s.Replace(',', '.');

            // final simple parse using InvariantCulture (dot as decimal separator)
            if (!decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out result))
                return false;

            if (parenthesisNegative && result > 0) result = -result;
            return true;
        }

        public static decimal Parse(string input)
        {
            if (!TryParse(input, out var r))
                throw new FormatException($"Invalid numeric input: '{input}'");
            return r;
        }
    }
}
