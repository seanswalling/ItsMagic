using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Dumbledore
{
    public class Librarian : Regex
    {
        private readonly string _input;

        public Librarian(string pattern, string input = null) : base(pattern)
        {
            _input = input;
        }

        public Librarian(string pattern, string input, RegexOptions regexOptions) : base(pattern, regexOptions)
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
        
        public bool HasMatch()
        {
            var matches = Matches(_input);
            return matches.Count > 0;
        }
    }
}
