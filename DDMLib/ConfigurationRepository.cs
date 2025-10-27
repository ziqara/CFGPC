using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDMLib
{
    public class ConfigurationRepository : IConfigurationRepository
    {
        public List<ConfigurationCard> GetUserConfigurations(string userEmail)
        {
            throw new NotImplementedException();
        }

        public ConfigurationCard GetConfigurationById(int configId)
        {
            throw new NotImplementedException();
        }

        public bool HasRelatedOrders(int configId)
        {
            throw new NotImplementedException();
        }

        public bool DeleteConfiguration(int configId, string userEmail)
        {
            throw new NotImplementedException();
        }
    }
}
