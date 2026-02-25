using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace DDMLib
{
    public class GpuService
    {
        private readonly IGpuRepository repo_;
        private readonly GpuValidator validator_;

        public GpuService(IGpuRepository repo, GpuValidator validator = null)
        {
            repo_ = repo ?? throw new ArgumentNullException(nameof(repo));
            validator_ = validator ?? new GpuValidator();
        }

        public List<Gpu> GetAllGpus()
        {
            return repo_.ReadAllGpus();
        }

        public string CreateGpu(Gpu gpu)
        {
            List<string> errors = validator_.Validate(gpu);

            if (repo_.ExistsSameGpu(gpu.Name, gpu.Brand, gpu.Model))
                errors.Add("Такая видеокарта уже существует (совпали Название/Бренд/Модель)");

            if (errors.Count > 0)
                return string.Join("\n", errors);

            bool ok = repo_.AddGpu(gpu);
            if (!ok)
                return "Не удалось сохранить видеокарту (ошибка подключения БД)";

            return string.Empty;
        }

        public string UpdateGpu(Gpu gpu)
        {
            if (gpu == null) throw new ArgumentNullException(nameof(gpu));

            List<string> errors = validator_.Validate(gpu);
            if (errors.Count > 0)
                return string.Join("\n", errors);

            if (repo_.ExistsOtherSameGpu(gpu.Name, gpu.Brand, gpu.Model, gpu.ComponentId))
                return "Такая видеокарта уже есть (совпали Название/Бренд/Модель)";

            bool ok = repo_.UpdateGpu(gpu);
            if (!ok)
                return "Не удалось сохранить изменения. Повторите попытку позже.";

            return string.Empty;
        }

        public string DeleteGpu(int componentId)
        {
            try
            {
                if (repo_.HasActiveOrders(componentId))
                    return "Невозможно удалить: видеокарта участвует в незавершённых заказах";

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