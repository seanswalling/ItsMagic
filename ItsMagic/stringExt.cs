using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dumbledore
{
    public static class StringExt
    {
        public static string ReplaceLastOccurrence(this string source, string find, string Replace)
        {
            int place = source.LastIndexOf(find, StringComparison.Ordinal);

            if (place == -1)
                return source;

            string result = source.Remove(place, find.Length).Insert(place, Replace);
            return result;
        }
    }
}
