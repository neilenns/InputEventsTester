using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputEventsTester
{
    public class Helpers
    {
        public static ulong StringToULong(string stringValue)
        {
            if (ulong.TryParse(stringValue.Trim(), out ulong hashValue))
            {
                return hashValue;
            }
            else
            {
                return 0;
            }
        }

    }
}
