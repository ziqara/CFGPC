using System.Collections.Generic;

namespace DDMLib
{
    public interface IMotherboardRepository
    {
        List<Motherboard> ReadAllMotherboards();

        bool AddMotherboard(Motherboard mb);

        bool ExistsSameMotherboard(string name, string brand, string model);

        bool UpdateMotherboard(Motherboard mb);

        bool ExistsOtherSameMotherboard(string name, string brand, string model, int currentComponentId);

        bool DeleteById(int componentId);

        bool HasActiveOrders(int componentId);
    }
}