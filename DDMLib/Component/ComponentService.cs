using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDMLib.Component
{
    public class ComponentService
    {
        private readonly IComponentRepository componentRepository_;

        public ComponentService(IComponentRepository componentRepository)
        {
            componentRepository_ = componentRepository;
        }

        public List<ComponentDto> GetComponentsByCategory(string category)
        {
            return componentRepository_.GetComponentsByCategory(category);
        }
    }
}
