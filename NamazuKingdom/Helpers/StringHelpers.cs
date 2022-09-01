using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamazuKingdom.Helpers
{
    public static class StringHelpers
    {
        public static string CleanTTSString(string toClean)
        {
            toClean = toClean.Trim();
            var startOfLessThan = toClean.IndexOf('<');
            if(startOfLessThan < 0) 
                return toClean; 
            var startOfGreaterThan = toClean.IndexOf('>');
            if (startOfGreaterThan < 0)
                return toClean;
            toClean = toClean.Substring(0, startOfLessThan) + toClean.Substring(startOfGreaterThan+1);
            return toClean;
        }
        //Too lazy to make this myself :^) https://stackoverflow.com/questions/11395775/clean-the-string-is-there-any-better-way-of-doing-it
        public static string CleanString(string dirtyString)
        {
            HashSet<char> removeChars = new HashSet<char>("?&^$#@!()+-,:;<>’\'-_*");
            StringBuilder result = new StringBuilder(dirtyString.Length);
            foreach (char c in dirtyString)
                if (!removeChars.Contains(c)) // prevent dirty chars
                    result.Append(c);
            return result.ToString();
        }
    }
}
