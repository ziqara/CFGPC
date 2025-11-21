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

        public SupplierService(ISupplierRepository repo, SupplierValidator validator = null)
        {
            if (repo == null)
                throw new ArgumentNullException(nameof(repo));

            repo_ = repo;
            // если валидатор не передали – создаём обычный
            validator_ = validator ?? new SupplierValidator();
        }

        public List<Supplier> GetAllSuppliers()
        {
            return repo_.ReadAllSuppliers();
        }

        public string CreateSupplier(Supplier supplier)
        {
            List<string> errors = validator_.Validate(supplier);

            if (repo_.existsByInn(supplier.Inn))
                errors.Add("Поставщик с таким ИНН уже существует");

            if (repo_.existsByNameInsensitive(supplier.Name))
                errors.Add("Поставщик с таким названием уже есть");

            if (repo_.existsByEmail(supplier.ContactEmail))
                errors.Add("Email уже используется");

            if (errors.Count > 0)
                return string.Join("\n", errors);

            bool ok = repo_.AddSupplier(supplier);
            if (!ok)
                return "Не удалось сохранить поставщика (ошибка подключения БД)";

            return string.Empty;
        }

        public string UpdateSupplier(Supplier supplier)
        {
            if (supplier == null)
                throw new ArgumentNullException(nameof(supplier));

            List<string> errors = validator_.Validate(supplier);
            if (errors.Count > 0)
            {
                return string.Join("\n", errors);
            }

            if (repo_.existsByNameInsensitive(supplier.Name))
            {
                return "Поставщик с таким названием уже есть";
            }

            if (repo_.existsByEmail(supplier.ContactEmail))
            {
                return "Email уже используется";
            }

            bool ok = repo_.UpdateSupplier(supplier);
            if (!ok)
            {
                return "Не удалось сохранить изменения поставщика. Повторите попытку позже.";
            }

            return string.Empty;
        }
    }
}
