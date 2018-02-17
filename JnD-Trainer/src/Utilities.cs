using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JnD_Trainer.src
{
    class Utilities {
        private static int hexToInt(string hex) {
            // Remove all whitespace
            hex = Regex.Replace(hex, @"\s+", "");
            return int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        }
        
    }
}