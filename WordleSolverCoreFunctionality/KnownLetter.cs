using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleSolverCoreFunctionality
{
    public class KnownLetter
    {
        public Char Letter { get; set; }

        public List<int> knownBadPositions { get; set; } = new List<int>();
        public List<int> knownGoodPositions { get; set;} = new List<int>(); 
    }
}
