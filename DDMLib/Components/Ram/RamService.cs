using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace DDMLib
{
    public class RamService
    {
        private readonly IRamRepository repo_;
        private readonly RamValidator validator_;

        public RamService(IRamRepository repo, RamValidator validator = null)
        {
            repo_ = repo ?? throw new ArgumentNullException(nameof(repo));
            validator_ = validator ?? new RamValidator();
        }

        public List<Ram> GetAllRams()
        {
            return repo_.ReadAllRams();
        }

        public string CreateRam(Ram ram)
        {
            List<string> errors = validator_.Validate(ram);

            if (repo_.ExistsSameRam(ram.Name, ram.Brand, ram.Model))
                errors.Add("Такая оперативная память уже существует (совпали Название/Бренд/Модель)");

            if (errors.Count > 0)
                return string.Join("\n", errors);

            bool ok = repo_.AddRam(ram);
            if (!ok)
                return "Не удалось сохранить оперативную память (ошибка подключения БД)";

            return string.Empty;
        }

        public string UpdateRam(Ram ram)
        {
            if (ram == null)
                throw new ArgumentNullException(nameof(ram));

            List<string> errors = validator_.Validate(ram);
            if (errors.Count > 0)
                return string.Join("\n", errors);

            if (repo_.ExistsOtherSameRam(ram.Name, ram.Brand, ram.Model, ram.ComponentId))
                return "Такая оперативная память уже есть (совпали Название/Бренд/Модель)";

            bool ok = repo_.UpdateRam(ram);
            if (!ok)
                return "Не удалось сохранить изменения. Повторите попытку позже.";

            return string.Empty;
        }

        public string DeleteRam(int componentId)
        {
            try
            {
                if (repo_.HasActiveOrders(componentId))
                    return "Невозможно удалить: оперативная память участвует в незавершённых заказах";

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