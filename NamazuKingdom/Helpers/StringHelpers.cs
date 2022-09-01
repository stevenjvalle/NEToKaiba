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
    }
}
