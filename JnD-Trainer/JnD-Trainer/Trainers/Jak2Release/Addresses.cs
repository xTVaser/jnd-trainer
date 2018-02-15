using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JnD_Trainer.src;

namespace JnD_Trainer.Trainers.Jak2Release
{
    class Addresses {

        // TODO fill out file as needed
        protected static readonly Address InfiniteJump = new Address(HexAddress: 0x0, ShouldFreeze: true, ShouldSkip: false);






        // addresses need to be made easily and human readable
        // they need to be made in hex
        // they also need to store info on whether or not they are values that need to be frozen
        // this way logic does not need to be appended to each and every address in the actual trainer, contained to this file
        protected static List<Address> FREEZE_ADDRESSES = new List<Address> {
            InfiniteJump
        };

        // for values that we want to read the value from as well, most addresses we want to see the value
        // so instead, we will track which ones we DONT want
        protected static List<Address> SKIP_VALUES = new List<Address> {
            InfiniteJump
        };
    }
}
