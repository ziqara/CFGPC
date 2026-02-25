using System.Collections.Generic;

namespace DDMLib
{
    public interface IPsuRepository
    {
        List<Psu> ReadAllPsus();

        bool AddPsu(Psu psu);

        bool ExistsSamePsu(string name, string brand, string model);

        bool UpdatePsu(Psu psu);

        bool ExistsOtherSamePsu(string name, string brand, string model, int currentComponentId);

        bool DeleteById(int componentId);

        bool HasActiveOrders(int componentId);
    }
}