using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JnD_Trainer.src
{

    class Address
    {
        public int HexAddress { get; }
        public bool ShouldFreeze { get; }
        public bool ShouldSkip { get; }

        public Address(int HexAddress, bool ShouldFreeze, bool ShouldSkip) {
            this.HexAddress = HexAddress;
            this.ShouldFreeze = ShouldFreeze;
            this.ShouldSkip = ShouldSkip;
        }
    }
}
