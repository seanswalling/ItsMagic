using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dumbledore
{
    static class RegexStore
    {
        public static IEnumerable<string> Get(string pattern, string text)
        {
            Regex regex = new Regex(pattern);
            var matchCollection = regex.Matches(text);
            return matchCollection
                .Cast<Match>()
                .Select(m => m.Groups["capturegroup"].Value);
        }

        public static string Either(string pattern1, string pattern2)
        {
            return pattern1 + "|" + pattern2;
        }

        public static bool Contains(string pattern, string inputText)
        {
            Regex regex = new Regex(pattern);
            var matches = regex.Matches(inputText);
            if (matches.Count > 0)
            {
                return true;
            }
            return false;
        }

        public static string ReplaceLastOccurrence(string source, string find, string Replace)
        {
            int place = source.LastIndexOf(find, StringComparison.Ordinal);

            if (place == -1)
                return source;

            string result = source.Remove(place, find.Length).Insert(place, Replace);
            return result;
        }
    }
}
