using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputEventsTester
{
    public class InputEvent
    {
        public string? Name { get; set; }
        public ulong? Hash { get; set; }

        public override string ToString()
        {
            return Name ?? "";
        }
    }
}
