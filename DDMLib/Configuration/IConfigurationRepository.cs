using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDMLib.Configuration
{
    public interface IConfigurationRepository
    {
        List<ConfigurationDto> GetUserConfigurations(string userEmail);
    }
}
