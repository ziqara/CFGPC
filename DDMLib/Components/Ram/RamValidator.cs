using System.Collections.Generic;

namespace DDMLib
{
    public class RamValidator
    {
        public virtual List<string> Validate(Ram ram)
        {
            var errors = new List<string>();

            if (ram == null)
            {
                errors.Add("Пустые данные");
                return errors;
            }

            if (string.IsNullOrWhiteSpace(ram.Name))
                errors.Add("Название обязательно");

            if (ram.Price < 0)
                errors.Add("Цена не может быть отрицательной");

            if (ram.StockQuantity < 0)
                errors.Add("Остаток не может быть отрицательным");

            // enum RAM типа: DDR4/DDR5
            if (!string.IsNullOrWhiteSpace(ram.RamType) &&
                ram.RamType != "DDR4" && ram.RamType != "DDR5")
            {
                errors.Add("Тип RAM должен быть DDR4 или DDR5");
            }

            if (ram.CapacityGb.HasValue && ram.CapacityGb.Value <= 0)
                errors.Add("Объём (GB) должен быть > 0");

            if (ram.SpeedMhz.HasValue && ram.SpeedMhz.Value <= 0)
                errors.Add("Частота (MHz) должна быть > 0");

            if (ram.SlotsNeeded.HasValue && ram.SlotsNeeded.Value <= 0)
                errors.Add("Слоты должны быть > 0");

            return errors;
        }
    }
}