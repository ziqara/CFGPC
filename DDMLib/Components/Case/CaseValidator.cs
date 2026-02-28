using System.Collections.Generic;

namespace DDMLib
{
    public class CaseValidator
    {
        public virtual List<string> Validate(Case c)
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

            // size NOT NULL enum('full_tower','mid_tower','compact')
            if (string.IsNullOrWhiteSpace(c.Size))
            {
                errors.Add("Размер корпуса обязателен");
            }
            else
            {
                bool ok = c.Size == "full_tower" || c.Size == "mid_tower" || c.Size == "compact";
                if (!ok)
                    errors.Add("Размер корпуса должен быть: full_tower / mid_tower / compact");
            }

            // formFactor varchar(20) nullable
            if (!string.IsNullOrWhiteSpace(c.FormFactor) && c.FormFactor.Trim().Length > 20)
                errors.Add("Форм-фактор не должен превышать 20 символов");

            return errors;
        }
    }
}