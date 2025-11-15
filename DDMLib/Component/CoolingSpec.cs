using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDMLib.Component
{
    public class CoolingSpec
    {
        public string CoolerType { get; set; }
        public int TdpSupport { get; set; }
        public int FanRpm { get; set; }
        public string Size { get; set; }
        public bool IsRgb { get; set; }
    }
}
