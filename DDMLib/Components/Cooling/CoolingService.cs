using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace DDMLib
{
    public class CoolingService
    {
        private readonly ICoolingRepository repo_;
        private readonly CoolingValidator validator_;

        public CoolingService(ICoolingRepository repo, CoolingValidator validator = null)
        {
            repo_ = repo ?? throw new ArgumentNullException(nameof(repo));
            validator_ = validator ?? new CoolingValidator();
        }

        public List<Cooling> GetAllCoolings()
        {
            return repo_.ReadAllCoolings();
        }

        public string CreateCooling(Cooling c)
        {
            List<string> errors = validator_.Validate(c);

            if (repo_.ExistsSameCooling(c.Name, c.Brand, c.Model))
                errors.Add("Такое охлаждение уже существует (совпали Название/Бренд/Модель)");

            if (errors.Count > 0)
                return string.Join("\n", errors);

            bool ok = repo_.AddCooling(c);
            if (!ok)
                return "Не удалось сохранить охлаждение (ошибка подключения БД)";

            return string.Empty;
        }

        public string UpdateCooling(Cooling c)
        {
            if (c == null) throw new ArgumentNullException(nameof(c));

            List<string> errors = validator_.Validate(c);
            if (errors.Count > 0)
                return string.Join("\n", errors);

            if (repo_.ExistsOtherSameCooling(c.Name, c.Brand, c.Model, c.ComponentId))
                return "Такое охлаждение уже есть (совпали Название/Бренд/Модель)";

            bool ok = repo_.UpdateCooling(c);
            if (!ok)
                return "Не удалось сохранить изменения. Повторите попытку позже.";

            return string.Empty;
        }

        public string DeleteCooling(int componentId)
        {
            try
            {
                if (repo_.HasActiveOrders(componentId))
                    return "Невозможно удалить: охлаждение участвует в незавершённых заказах";

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