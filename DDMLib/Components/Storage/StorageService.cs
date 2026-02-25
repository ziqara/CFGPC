using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace DDMLib
{
    public class StorageService
    {
        private readonly IStorageRepository repo_;
        private readonly StorageValidator validator_;

        public StorageService(IStorageRepository repo, StorageValidator validator = null)
        {
            repo_ = repo ?? throw new ArgumentNullException(nameof(repo));
            validator_ = validator ?? new StorageValidator();
        }

        public List<Storage> GetAllStorages()
        {
            return repo_.ReadAllStorages();
        }

        public string CreateStorage(Storage s)
        {
            List<string> errors = validator_.Validate(s);

            if (repo_.ExistsSameStorage(s.Name, s.Brand, s.Model))
                errors.Add("Такой накопитель уже существует (совпали Название/Бренд/Модель)");

            if (errors.Count > 0)
                return string.Join("\n", errors);

            bool ok = repo_.AddStorage(s);
            if (!ok)
                return "Не удалось сохранить накопитель (ошибка подключения БД)";

            return string.Empty;
        }

        public string UpdateStorage(Storage s)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));

            List<string> errors = validator_.Validate(s);
            if (errors.Count > 0)
                return string.Join("\n", errors);

            if (repo_.ExistsOtherSameStorage(s.Name, s.Brand, s.Model, s.ComponentId))
                return "Такой накопитель уже есть (совпали Название/Бренд/Модель)";

            bool ok = repo_.UpdateStorage(s);
            if (!ok)
                return "Не удалось сохранить изменения. Повторите попытку позже.";

            return string.Empty;
        }

        public string DeleteStorage(int componentId)
        {
            try
            {
                if (repo_.HasActiveOrders(componentId))
                    return "Невозможно удалить: накопитель участвует в незавершённых заказах";

                bool ok = repo_.DeleteById(componentId);
                if (ok) return string.Empty;

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