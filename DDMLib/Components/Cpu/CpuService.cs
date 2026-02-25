using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace DDMLib
{
    public class CpuService
    {
        private readonly ICpuRepository repo_;
        private readonly CpuValidator validator_;

        public CpuService(ICpuRepository repo, CpuValidator validator = null)
        {
            repo_ = repo ?? throw new ArgumentNullException(nameof(repo));
            validator_ = validator ?? new CpuValidator();
        }

        public List<Cpu> GetAllCpus()
        {
            return repo_.ReadAllCpus();
        }

        public string CreateCpu(Cpu cpu)
        {
            List<string> errors = validator_.Validate(cpu);

            if (repo_.ExistsSameCpu(cpu.Name, cpu.Brand, cpu.Model))
                errors.Add("Такой процессор уже существует (совпали Название/Бренд/Модель)");

            if (errors.Count > 0)
                return string.Join("\n", errors);

            bool ok = repo_.AddCpu(cpu);
            if (!ok)
                return "Не удалось сохранить процессор (ошибка подключения БД)";

            return string.Empty;
        }

        public string UpdateCpu(Cpu cpu)
        {
            if (cpu == null)
                throw new ArgumentNullException(nameof(cpu));

            List<string> errors = validator_.Validate(cpu);
            if (errors.Count > 0)
                return string.Join("\n", errors);

            if (repo_.ExistsOtherSameCpu(cpu.Name, cpu.Brand, cpu.Model, cpu.ComponentId))
                return "Такой процессор уже есть (совпали Название/Бренд/Модель)";

            bool ok = repo_.UpdateCpu(cpu);
            if (!ok)
                return "Не удалось сохранить изменения процессора. Повторите попытку позже.";

            return string.Empty;
        }

        public string DeleteCpu(int componentId)
        {
            try
            {
                if (repo_.HasActiveOrders(componentId))
                    return "Невозможно удалить: процессор участвует в незавершённых заказах";

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