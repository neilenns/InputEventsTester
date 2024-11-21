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

        public string SetRPN {  get
            {
                return $"(>B:{this.Name}_Set)";
            } 
        }

        public string IncRPN
        {
            get
            {
                return $"1 (>B:{this.Name}_Inc)";
            }
        }

        public string SetDec
        {
            get
            {
                return $"1 (>B:{this.Name}_Dec)";
            }
        }


        public override string ToString()
        {
            return Name ?? "";
        }
    }
}
