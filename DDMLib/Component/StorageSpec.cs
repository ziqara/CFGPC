using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDMLib.Component
{
    public class StorageSpec
    {
        public int ComponentId { get; set; }
        public string Interface { get; set; }
        public int CapacityGb { get; set; }
    }
}
