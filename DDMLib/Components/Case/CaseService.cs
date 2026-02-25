using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace DDMLib
{
    public class CaseService
    {
        private readonly ICaseRepository repo_;
        private readonly CaseValidator validator_;

        public CaseService(ICaseRepository repo, CaseValidator validator = null)
        {
            repo_ = repo ?? throw new ArgumentNullException(nameof(repo));
            validator_ = validator ?? new CaseValidator();
        }

        public List<Case> GetAllCases()
        {
            return repo_.ReadAllCases();
        }

        public string CreateCase(Case c)
        {
            List<string> errors = validator_.Validate(c);

            if (repo_.ExistsSameCase(c.Name, c.Brand, c.Model))
                errors.Add("Такой корпус уже существует (совпали Название/Бренд/Модель)");

            if (errors.Count > 0)
                return string.Join("\n", errors);

            bool ok = repo_.AddCase(c);
            if (!ok)
                return "Не удалось сохранить корпус (ошибка подключения БД)";

            return string.Empty;
        }

        public string UpdateCase(Case c)
        {
            if (c == null) throw new ArgumentNullException(nameof(c));

            List<string> errors = validator_.Validate(c);
            if (errors.Count > 0)
                return string.Join("\n", errors);

            if (repo_.ExistsOtherSameCase(c.Name, c.Brand, c.Model, c.ComponentId))
                return "Такой корпус уже есть (совпали Название/Бренд/Модель)";

            bool ok = repo_.UpdateCase(c);
            if (!ok)
                return "Не удалось сохранить изменения. Повторите попытку позже.";

            return string.Empty;
        }

        public string DeleteCase(int componentId)
        {
            try
            {
                if (repo_.HasActiveOrders(componentId))
                    return "Невозможно удалить: корпус участвует в незавершённых заказах";

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