using System.Collections.Generic;

namespace DDMLib
{
    public interface IGpuRepository
    {
        List<Gpu> ReadAllGpus();

        bool AddGpu(Gpu gpu);

        bool ExistsSameGpu(string name, string brand, string model);

        bool UpdateGpu(Gpu gpu);

        bool ExistsOtherSameGpu(string name, string brand, string model, int currentComponentId);

        bool DeleteById(int componentId);

        bool HasActiveOrders(int componentId);
    }
}