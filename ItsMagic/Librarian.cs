using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dumbledore
{
    class Librarian : Regex
    {
        private string _input {get;}

        public Librarian(string pattern, string input) : base(pattern)
        {
            _input = input;
        }

        public IEnumerable<string> Get(string captureGroup = null)
        {
            var match = Matches(_input).Cast<Match>();

            if(captureGroup == null)
                return match
                .Select(m => m.Value);

            return match
                .Select(m => m.Groups[captureGroup].Value);
        }

        public bool Contains()
        {
            var matches = Matches(_input);
            if (matches.Count > 0)
            {
                return true;
            }
            return false;
        }

        public string ReplaceLastOccurrenceWith(string source, string find, string Replace)
        {
            int place = source.LastIndexOf(find, StringComparison.Ordinal);

            if (place == -1)
                return source;

            string result = source.Remove(place, find.Length).Insert(place, Replace);
            return result;
        }
    }
}
