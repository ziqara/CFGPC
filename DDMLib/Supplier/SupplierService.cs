using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDMLib
{
    public class SupplierService
    {
        private readonly ISupplierRepository repo_;
        private readonly SupplierValidator validator_;

        public SupplierService(ISupplierRepository repo)
        {
            repo_ = repo;
            validator_ = new SupplierValidator();
        }

        public List<Supplier> GetAllSuppliers()
        {
            return repo_.ReadAllSuppliers();
        }

        public string CreateSupplier(Supplier supplier)
        {
            List<string> errors = validator_.Validate(supplier);
            if (errors.Count > 0)
            {
                return string.Join("\n", errors);
            }

            if (repo_.existsByInn(supplier.Inn)) 
            { 
                return "Поставщик с таким ИНН уже существует"; 
            }

            if (repo_.existsByNameInsensitive(supplier.Name))
            { 
                return "Поставщик с таким названием уже есть"; 
            }

            if (repo_.existsByEmail(supplier.ContactEmail))
            { 
                return "Email уже используется"; 
            }
        }
    }
}
