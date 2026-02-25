using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace DDMLib.Component
{
    public interface IComponentRepository
    {
        List<ComponentDto> GetComponentsByCategory(string category);
        Component GetComponentById(int componentId);
        T GetComponentSpec<T>(int componentId) where T : class;
    }
}