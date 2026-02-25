using System.Collections.Generic;

namespace DDMLib
{
    public interface ICaseRepository
    {
        List<Case> ReadAllCases();

        bool AddCase(Case c);

        bool ExistsSameCase(string name, string brand, string model);

        bool UpdateCase(Case c);

        bool ExistsOtherSameCase(string name, string brand, string model, int currentComponentId);

        bool DeleteById(int componentId);

        bool HasActiveOrders(int componentId);
    }
}