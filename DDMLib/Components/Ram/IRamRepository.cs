using System.Collections.Generic;

namespace DDMLib
{
    public interface IRamRepository
    {
        List<Ram> ReadAllRams();

        bool AddRam(Ram ram);

        bool ExistsSameRam(string name, string brand, string model);

        bool UpdateRam(Ram ram);

        bool ExistsOtherSameRam(string name, string brand, string model, int currentComponentId);

        bool DeleteById(int componentId);

        bool HasActiveOrders(int componentId);
    }
}