using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDMLib.Component;

namespace DDMLib.Configuration
{
    public class ConfigurationDto
    {
        public Configuration Configuration { get; set; }
        public List<DDMLib.Component.Component> Components { get; set; } = new List<DDMLib.Component.Component>();
    }
}
