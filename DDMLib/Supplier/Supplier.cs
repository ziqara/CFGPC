using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDMLib
{
    public class Supplier
    {
        public int Inn { get; }
        public string Name { get; set; }
        public string ContactEmail { get; set; }
        public string Phone { get; set; }  // может быть null
        public string Address { get; set; } // может быть null
    }

}
