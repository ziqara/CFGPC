using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDMLib.Component
{
    public class GpuSpec
    {
        public int ComponentId { get; set; }
        public string PcieVersion { get; set; }
        public int Tdp { get; set; }
        public int VramGb { get; set; }
    }
}
