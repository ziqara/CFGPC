using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDMLib.Configurations;

namespace DDMLib.Configuration
{
    public interface IConfigurationRepository
    {
        List<ConfigurationDto> GetUserConfigurations(string userEmail);
        bool DeleteConfigurationByIdAndUser(string userEmail, int configId);
        List<ConfigurationDto> GetPresetConfigurations();
        int CreateConfiguration(Configuration configuration, List<int> componentIds);

        ConfigurationDetails GetDetails(int configId);
    }
}
