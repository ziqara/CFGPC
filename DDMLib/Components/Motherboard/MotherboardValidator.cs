using System.Collections.Generic;

namespace DDMLib
{
    public class MotherboardValidator
    {
        public List<string> Validate(Motherboard mb)
        {
            var errors = new List<string>();

            if (mb == null)
            {
                errors.Add("Пустые данные");
                return errors;
            }

            if (string.IsNullOrWhiteSpace(mb.Name))
                errors.Add("Название обязательно");

            if (mb.Price < 0)
                errors.Add("Цена не может быть отрицательной");

            if (mb.StockQuantity < 0)
                errors.Add("Остаток не может быть отрицательным");

            if (!string.IsNullOrWhiteSpace(mb.RamType) && mb.RamType != "DDR4" && mb.RamType != "DDR5")
                errors.Add("Тип RAM должен быть DDR4 или DDR5");

            if (!string.IsNullOrWhiteSpace(mb.PcieVersion) &&
                mb.PcieVersion != "3.0" && mb.PcieVersion != "4.0" && mb.PcieVersion != "5.0")
                errors.Add("PCIe должен быть 3.0 / 4.0 / 5.0");

            return errors;
        }
    }
}