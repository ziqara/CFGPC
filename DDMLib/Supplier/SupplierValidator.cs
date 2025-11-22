using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDMLib
{
    public class SupplierValidator
    {
        public List<string> Validate(Supplier supplier)
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrWhiteSpace(supplier.Name))
            {
                errors.Add("Название обязательно");
            }
            else if (supplier.Name.Length > 50)
            {
                errors.Add("Название не должно превышать 50 символов");
            }

            if (string.IsNullOrWhiteSpace(supplier.ContactEmail))
            {
                errors.Add("Email обязателен");
            }
            else
            {
                string emailValidation = UserService.ValidateEmail(supplier.ContactEmail);
                if (!string.IsNullOrEmpty(emailValidation))
                {
                    errors.Add("Некорректный email");
                }
            }

            string phone = supplier.Phone?.Trim();
            if (string.IsNullOrEmpty(phone))
            {
                supplier.Phone = null;
            }
            else
            {
                if (!phone.All(char.IsDigit))
                {
                    errors.Add("Некорректный номер телефона");
                }
                else if (phone.Length != 11)
                {
                    errors.Add("Некорректный номер телефона");
                }
            }

            string innStr = supplier.Inn.ToString();

            if (innStr.Length != 9)
                errors.Add("ИНН должен состоять из 9 цифр");

            return errors;
        }
    }
}
