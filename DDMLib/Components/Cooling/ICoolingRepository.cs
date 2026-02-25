using System.Collections.Generic;

namespace DDMLib
{
    public interface ICoolingRepository
    {
        List<Cooling> ReadAllCoolings();

        bool AddCooling(Cooling c);

        bool ExistsSameCooling(string name, string brand, string model);

        bool UpdateCooling(Cooling c);

        bool ExistsOtherSameCooling(string name, string brand, string model, int currentComponentId);

        bool DeleteById(int componentId);

        bool HasActiveOrders(int componentId);
    }
}