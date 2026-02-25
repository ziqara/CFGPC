using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace DDMLib
{
    public class MotherboardService
    {
        private readonly IMotherboardRepository repo_;
        private readonly MotherboardValidator validator_;

        public MotherboardService(IMotherboardRepository repo, MotherboardValidator validator = null)
        {
            repo_ = repo ?? throw new ArgumentNullException(nameof(repo));
            validator_ = validator ?? new MotherboardValidator();
        }

        public List<Motherboard> GetAllMotherboards()
        {
            return repo_.ReadAllMotherboards();
        }

        public string CreateMotherboard(Motherboard mb)
        {
            List<string> errors = validator_.Validate(mb);

            if (repo_.ExistsSameMotherboard(mb.Name, mb.Brand, mb.Model))
                errors.Add("Такая материнская плата уже существует (совпали Название/Бренд/Модель)");

            if (errors.Count > 0)
                return string.Join("\n", errors);

            bool ok = repo_.AddMotherboard(mb);
            if (!ok)
                return "Не удалось сохранить материнскую плату (ошибка подключения БД)";

            return string.Empty;
        }

        public string UpdateMotherboard(Motherboard mb)
        {
            if (mb == null) throw new ArgumentNullException(nameof(mb));

            List<string> errors = validator_.Validate(mb);
            if (errors.Count > 0)
                return string.Join("\n", errors);

            if (repo_.ExistsOtherSameMotherboard(mb.Name, mb.Brand, mb.Model, mb.ComponentId))
                return "Такая материнская плата уже есть (совпали Название/Бренд/Модель)";

            bool ok = repo_.UpdateMotherboard(mb);
            if (!ok)
                return "Не удалось сохранить изменения. Повторите попытку позже.";

            return string.Empty;
        }

        public string DeleteMotherboard(int componentId)
        {
            try
            {
                if (repo_.HasActiveOrders(componentId))
                    return "Невозможно удалить: материнская плата участвует в незавершённых заказах";

                bool ok = repo_.DeleteById(componentId);
                if (ok) return string.Empty;

                return "Запись не найдена";
            }
            catch (MySqlException ex)
            {
                string msg = ex.Message ?? "";
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