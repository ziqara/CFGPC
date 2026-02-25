using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace DDMLib
{
    public class PsuService
    {
        private readonly IPsuRepository repo_;
        private readonly PsuValidator validator_;

        public PsuService(IPsuRepository repo, PsuValidator validator = null)
        {
            repo_ = repo ?? throw new ArgumentNullException(nameof(repo));
            validator_ = validator ?? new PsuValidator();
        }

        public List<Psu> GetAllPsus()
        {
            return repo_.ReadAllPsus();
        }

        public string CreatePsu(Psu p)
        {
            List<string> errors = validator_.Validate(p);

            if (repo_.ExistsSamePsu(p.Name, p.Brand, p.Model))
                errors.Add("Такой блок питания уже существует (совпали Название/Бренд/Модель)");

            if (errors.Count > 0)
                return string.Join("\n", errors);

            bool ok = repo_.AddPsu(p);
            if (!ok)
                return "Не удалось сохранить блок питания (ошибка подключения БД)";

            return string.Empty;
        }

        public string UpdatePsu(Psu p)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));

            List<string> errors = validator_.Validate(p);
            if (errors.Count > 0)
                return string.Join("\n", errors);

            if (repo_.ExistsOtherSamePsu(p.Name, p.Brand, p.Model, p.ComponentId))
                return "Такой блок питания уже есть (совпали Название/Бренд/Модель)";

            bool ok = repo_.UpdatePsu(p);
            if (!ok)
                return "Не удалось сохранить изменения. Повторите попытку позже.";

            return string.Empty;
        }

        public string DeletePsu(int componentId)
        {
            try
            {
                if (repo_.HasActiveOrders(componentId))
                    return "Невозможно удалить: блок питания участвует в незавершённых заказах";

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