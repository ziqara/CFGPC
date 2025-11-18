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


        }
    }
}
