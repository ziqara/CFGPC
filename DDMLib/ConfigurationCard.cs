using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDMLib
{
    public class ConfigurationCard
    {
        public int ConfigId { get; set; }
        public string ConfigName { get; set; }
        public string Description { get; set; }
        public decimal TotalPrice { get; set; }
        public string TargetUse { get; set; }
        public string Status { get; set; }
        public bool IsPreset { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserEmail { get; set; }
        public string Rgb { get; set; }
        public string OtherOptions { get; set; }
    }
}
