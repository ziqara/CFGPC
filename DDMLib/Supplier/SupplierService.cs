using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

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
                throw new ArgumentNullException("supplier");

            List<string> errors = validator_.Validate(supplier);
            if (errors.Count > 0)
            {
                return string.Join("\n", errors);
            }

            if (repo_.existsOtherByNameInsensitive(supplier.Name, supplier.Inn))
            {
                return "Поставщик с таким названием уже есть";
            }

            if (repo_.existsOtherByEmail(supplier.ContactEmail, supplier.Inn))
            {
                return "Email уже используется другим поставщиком";
            }

            bool ok = repo_.UpdateSupplier(supplier);
            if (!ok)
            {
                return "Не удалось сохранить изменения поставщика. Повторите попытку позже.";
            }

            return string.Empty;
        }

        public string DeleteSupplier(int inn)
        {
            try
            {
                if (repo_.HasActiveOrders(inn))
                {
                    return "Невозможно удалить: есть связанные незавершённые заказы";
                }

                bool ok = repo_.DeleteByInn(inn);

                if (ok)
                {
                    return string.Empty;
                }

                return "Запись не найдена";
            }
            catch (MySqlException ex)
            {
                string msg = ex.Message ?? string.Empty;

                if (msg.IndexOf("foreign key", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    msg.IndexOf("constraint fails", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return "Невозможно удалить: есть связанные записи";
                }

                return "Вероятно, проблемы в соединении с БД: " + msg;
            }
            catch (Exception ex)
            {
                return "Вероятно, проблемы в соединении с БД: " + ex.Message;
            }
        }
    }
}
