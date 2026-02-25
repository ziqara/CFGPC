using System.Collections.Generic;

namespace DDMLib
{
    public class PsuValidator
    {
        public List<string> Validate(Psu p)
        {
            var errors = new List<string>();

            if (p == null)
            {
                errors.Add("Пустые данные");
                return errors;
            }

            if (string.IsNullOrWhiteSpace(p.Name))
                errors.Add("Название обязательно");

            if (p.Price < 0)
                errors.Add("Цена не может быть отрицательной");

            if (p.StockQuantity < 0)
                errors.Add("Остаток не может быть отрицательным");

            if (p.Wattage.HasValue && p.Wattage.Value <= 0)
                errors.Add("Мощность (W) должна быть > 0");

            if (!string.IsNullOrWhiteSpace(p.Efficiency))
            {
                bool ok =
                    p.Efficiency == "80+ Bronze" ||
                    p.Efficiency == "80+ Gold" ||
                    p.Efficiency == "80+ Platinum";

                if (!ok)
                    errors.Add("Сертификация должна быть: 80+ Bronze / 80+ Gold / 80+ Platinum");
            }

            return errors;
        }
    }
}