using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDMLib.Component
{
    public class ComponentDto
    {
        public Component Component { get; set; } = new Component();
        public object Specs { get; set; }
    }
}
