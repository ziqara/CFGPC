using System;
using System.Collections.Generic;
using DDMLib.Component;

namespace DDMLib
{
    public class ComponentService
    {
        private readonly IComponentRepository repo_;

        public ComponentService(IComponentRepository repo)
        {
            repo_ = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public List<ComponentItem> GetMotherboards() => repo_.ReadMotherboardsAvailable();
        public List<ComponentItem> GetCpusByMb(int mbId) => repo_.ReadCpusByMotherboard(mbId);
        public List<ComponentItem> GetRamsByMb(int mbId) => repo_.ReadRamsByMotherboard(mbId);
        public List<ComponentItem> GetGpusByMb(int mbId) => repo_.ReadGpusByMotherboard(mbId);
        public List<ComponentItem> GetCasesByMb(int mbId) => repo_.ReadCasesByMotherboard(mbId);
        public List<ComponentItem> GetCoolingsByCpu(int cpuId) => repo_.ReadCoolingsByCpu(cpuId);
        public List<ComponentItem> GetStorages() => repo_.ReadStoragesAvailable();
        public List<ComponentItem> GetPsusByCpuGpu(int cpuId, int gpuId) => repo_.ReadPsusByCpuGpu(cpuId, gpuId);
    }
}