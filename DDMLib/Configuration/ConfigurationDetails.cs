using System.Collections.Generic;
using System.ComponentModel;

namespace DDMLib.Configurations
{
    public class ConfigurationDetails
    {
        [DisplayName("ID конфигурации")]
        public int ConfigId { get; set; }

        [DisplayName("Название")]
        public string ConfigName { get; set; }

        public List<ConfigComponentInfo> Components { get; set; } = new List<ConfigComponentInfo>();
    }
}