using System.Collections.Generic;

namespace DDMLib
{
    public class StorageValidator
    {
        public List<string> Validate(Storage s)
        {
            var errors = new List<string>();

            if (s == null)
            {
                errors.Add("Пустые данные");
                return errors;
            }

            if (string.IsNullOrWhiteSpace(s.Name))
                errors.Add("Название обязательно");

            if (s.Price < 0)
                errors.Add("Цена не может быть отрицательной");

            if (s.StockQuantity < 0)
                errors.Add("Остаток не может быть отрицательным");

            if (!string.IsNullOrWhiteSpace(s.Interface) &&
                s.Interface != "SATA" && s.Interface != "NVMe")
            {
                errors.Add("Интерфейс должен быть SATA или NVMe");
            }

            if (s.CapacityGb.HasValue && s.CapacityGb.Value <= 0)
                errors.Add("Объём (GB) должен быть > 0");

            return errors;
        }
    }
}