using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDMLib
{
    public interface IConfigurationRepository
    {
        List<ConfigurationCard> GetUserConfigurations(string userEmail);
        ConfigurationCard GetConfigurationById(int configId);
        bool HasRelatedOrders(int configId);
        bool DeleteConfiguration(int configId, string userEmail);
    }
}
