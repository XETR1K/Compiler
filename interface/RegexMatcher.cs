using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace @interface
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    class RegexMatcher
    {
        private string pattern;

        public RegexMatcher(string pattern)
        {
            this.pattern = pattern;
        }

        public List<MatchResult> FindMatches(string text)
        {
            List<MatchResult> matchResults = new List<MatchResult>();

            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(text);

            foreach (Match match in matches)
            {
                matchResults.Add(new MatchResult(match.Value, match.Index));
            }

            return matchResults;
        }
    }

    class ManualMatcher
    {
        private string text;
        private int currentIndex;
        private int startIndex;
        List<MatchResult> matchResults;

        public ManualMatcher()
        {
            matchResults= new List<MatchResult>();
        }

        private bool s0()
        {
            if (currentIndex >= text.Length)
                return false;
            if (text[currentIndex] == '0')
            {
                currentIndex++;
                s0();
                return true;
            }
            else if (text[currentIndex] == '1')
            {
                currentIndex++;
                s1();
                return true;
            }
            return false;
        }

        private void s1()
        {
            if (currentIndex >= text.Length)
                return;
            if (text[currentIndex] == '2')
            {
                currentIndex++;
                s2();
            }
        }
        private void s2()
        {
            if (currentIndex >= text.Length)
                return;
            if (text[currentIndex] == '2')
            {
                currentIndex++;
                s2();
            }
            else if (text[currentIndex] == '0')
            {
                currentIndex++;
                s3();
            }
        }
        private void s3()
        {
            string result = text.Substring(startIndex, currentIndex - startIndex);
            matchResults.Add(new MatchResult(result, startIndex + 1));
        }

        public List<MatchResult> FindMatches(string _text)
        {
            text = _text;
            currentIndex = 0;
            startIndex = 0;
            while (currentIndex < text.Length)
            {
                startIndex = currentIndex;
                if (!s0())
                    currentIndex++;
            }
            return matchResults;
        }
    }

    class MatchResult
    {
        public string Substring { get; }
        public int StartIndex { get; }

        public MatchResult(string substring, int startIndex)
        {
            Substring = substring;
            StartIndex = startIndex;
        }
    }
}
