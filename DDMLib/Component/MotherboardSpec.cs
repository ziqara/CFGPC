using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDMLib.Component
{
    public class MotherboardSpec
    {
        public string Socket { get; set; }
        public string Chipset { get; set; }
        public string RamType { get; set; }
        public string PcieVersion { get; set; }
        public string FormFactor { get; set; }
    }
}
