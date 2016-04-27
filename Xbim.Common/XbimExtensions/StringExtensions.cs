using System;

namespace Xbim.Common.XbimExtensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Compute Levenshtein distance. Code origin: http://stackoverflow.com/questions/6944056/c-sharp-compare-string-similarity
        /// </summary>
        /// <param name="s">String 1</param>
        /// <param name="t">String 2</param>
        /// <returns>Distance between the two strings.
        /// The larger the number, the bigger the difference.
        /// </returns>
        public static int LevenshteinDistance(this string s, string t)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.IsNullOrEmpty(t) ? 0 : t.Length;
            }

            if (string.IsNullOrEmpty(t))
            {
                return s.Length;
            }

            var n = s.Length;
            var m = t.Length;
            var d = new int[n + 1, m + 1];

            // initialize the top and right of the table to 0, 1, 2, ...
            for (var i = 0; i <= n; d[i, 0] = i++) { }
            for (var j = 1; j <= m; d[0, j] = j++) { }

            for (var i = 1; i <= n; i++)
            {
                for (var j = 1; j <= m; j++)
                {
                    var cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    var min1 = d[i - 1, j] + 1;
                    var min2 = d[i, j - 1] + 1;
                    var min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }
            return d[n, m];
        }
    }


}
