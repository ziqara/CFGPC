using System.Collections.Generic;

namespace DDMLib
{
    public class CpuValidator
    {
        public List<string> Validate(Cpu cpu)
        {
            var errors = new List<string>();

            if (cpu == null)
            {
                errors.Add("Пустые данные");
                return errors;
            }

            if (string.IsNullOrWhiteSpace(cpu.Name))
                errors.Add("Название обязательно");

            if (cpu.Price < 0)
                errors.Add("Цена не может быть отрицательной");

            if (cpu.StockQuantity < 0)
                errors.Add("Остаток не может быть отрицательным");

            if (cpu.Cores.HasValue && cpu.Cores.Value <= 0)
                errors.Add("Количество ядер должно быть > 0");

            if (cpu.Tdp.HasValue && cpu.Tdp.Value <= 0)
                errors.Add("TDP должно быть > 0");

            return errors;
        }
    }
}