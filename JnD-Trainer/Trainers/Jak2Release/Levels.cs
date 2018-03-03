using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JnD_Trainer.Trainers.Jak2Release
{
    class Levels
    {
        public static readonly SortedDictionary<int, string> levelDictionary = new SortedDictionary<int, string>() {
            { 0x0087_7424, "Ship Turret" },
            { 0x0087_7724, "Ship Portal"},
            { 0x0, "Unknown Checkpoint" }
        };
    }
}
