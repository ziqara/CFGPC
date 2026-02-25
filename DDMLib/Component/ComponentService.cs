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

        // DDMLib/Component/ComponentService.cs
        public T GetComponentSpec<T>(int componentId) where T : class
        {
            try
            {
                return componentRepository_.GetComponentSpec<T>(componentId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError($"GetComponentSpec<{typeof(T).Name}>", ex.Message);
                return null;
            }
        }

        public Component GetComponentById(int componentId)
        {
            try
            {
                return componentRepository_.GetComponentById(componentId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError("GetComponentById", ex.Message);
                return null;
            }
        }
    }
}
