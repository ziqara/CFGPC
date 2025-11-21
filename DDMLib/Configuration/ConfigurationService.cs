using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDMLib.Configuration
{
    public class ConfigurationService
    {
        private readonly IConfigurationRepository configurationRepository_;

        public ConfigurationService(IConfigurationRepository configurationRepository)
        {
            configurationRepository_ = configurationRepository ?? throw new ArgumentNullException(nameof(configurationRepository));
        }

        public List<ConfigurationDto> GetUserConfigurations(string userEmail)
        {
            return configurationRepository_.GetUserConfigurations(userEmail);
        }
    }
}
