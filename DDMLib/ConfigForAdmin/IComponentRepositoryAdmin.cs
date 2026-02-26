using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDMLib.ConfigForAdmin
{
    public interface IComponentRepositoryAdmin
    {
        List<ComponentItem> ReadMotherboardsAvailable();
        List<ComponentItem> ReadCpusByMotherboard(int motherboardId);
        List<ComponentItem> ReadRamsByMotherboard(int motherboardId);
        List<ComponentItem> ReadGpusByMotherboard(int motherboardId);
        List<ComponentItem> ReadCasesByMotherboard(int motherboardId);
        List<ComponentItem> ReadCoolingsByCpu(int cpuId);

        // Storage без строгой совместимости в твоей схеме (interface SATA/NVMe не привязано к MB),
        // поэтому просто выдаём доступные.
        List<ComponentItem> ReadStoragesAvailable();

        // PSU зависит от CPU+GPU (по TDP)
        List<ComponentItem> ReadPsusByCpuGpu(int cpuId, int gpuId);
    }
}
