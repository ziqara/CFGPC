using System.Collections.Generic;

namespace DDMLib
{
    public class GpuValidator
    {
        public virtual List<string> Validate(Gpu gpu)
        {
            var errors = new List<string>();

            if (gpu == null)
            {
                errors.Add("Пустые данные");
                return errors;
            }

            if (string.IsNullOrWhiteSpace(gpu.Name))
                errors.Add("Название обязательно");

            if (gpu.Price < 0)
                errors.Add("Цена не может быть отрицательной");

            if (gpu.StockQuantity < 0)
                errors.Add("Остаток не может быть отрицательным");

            if (!string.IsNullOrWhiteSpace(gpu.PcieVersion) &&
                gpu.PcieVersion != "3.0" && gpu.PcieVersion != "4.0")
            {
                errors.Add("PCIe должен быть 3.0 или 4.0");
            }

            if (gpu.Tdp.HasValue && gpu.Tdp.Value <= 0)
                errors.Add("TDP должно быть > 0");

            if (gpu.VramGb.HasValue && gpu.VramGb.Value <= 0)
                errors.Add("VRAM (GB) должно быть > 0");

            return errors;
        }
    }
}