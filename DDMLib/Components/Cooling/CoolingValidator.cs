using System.Collections.Generic;

namespace DDMLib
{
    public class CoolingValidator
    {
        public virtual List<string> Validate(Cooling c)
        {
            var errors = new List<string>();

            if (c == null)
            {
                errors.Add("Пустые данные");
                return errors;
            }

            if (string.IsNullOrWhiteSpace(c.Name))
                errors.Add("Название обязательно");

            if (c.Price < 0)
                errors.Add("Цена не может быть отрицательной");

            if (c.StockQuantity < 0)
                errors.Add("Остаток не может быть отрицательным");

            // coolerType NOT NULL enum('air','liquid')
            if (string.IsNullOrWhiteSpace(c.CoolerType))
            {
                errors.Add("Тип кулера обязателен");
            }
            else
            {
                bool ok = c.CoolerType == "air" || c.CoolerType == "liquid";
                if (!ok)
                    errors.Add("Тип кулера должен быть: air / liquid");
            }

            if (c.TdpSupport.HasValue && c.TdpSupport.Value <= 0)
                errors.Add("Поддержка TDP должна быть > 0");

            if (c.FanRpm.HasValue && c.FanRpm.Value <= 0)
                errors.Add("Обороты (RPM) должны быть > 0");

            // size nullable enum('full_tower','mid_tower','compact')
            if (!string.IsNullOrWhiteSpace(c.Size))
            {
                bool okSize = c.Size == "full_tower" || c.Size == "mid_tower" || c.Size == "compact";
                if (!okSize)
                    errors.Add("Размер должен быть: full_tower / mid_tower / compact");
            }

            return errors;
        }
    }
}