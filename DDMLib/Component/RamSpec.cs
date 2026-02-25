using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDMLib.Component
{
    public class RamSpec
    {
        public int ComponentId { get; set; }
        public string RamType { get; set; }
        public int CapacityGb { get; set; }
        public int SpeedMhz { get; set; }
        public int SlotsNeeded { get; set; }
    }
}
