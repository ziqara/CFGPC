using System.Collections.Generic;

namespace DDMLib
{
    public interface ICpuRepository
    {
        List<Cpu> ReadAllCpus();

        bool AddCpu(Cpu cpu);

        bool ExistsSameCpu(string name, string brand, string model);

        bool UpdateCpu(Cpu cpu);

        bool ExistsOtherSameCpu(string name, string brand, string model, int currentComponentId);

        bool DeleteById(int componentId);

        bool HasActiveOrders(int componentId);
    }
}