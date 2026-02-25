using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDMLib.Configuration
{
    public class ConfigurationService
    {
        private readonly IConfigurationRepository configurationRepository_;

        public ConfigurationService(IConfigurationRepository configurationRepository)
        {
            configurationRepository_ = configurationRepository ?? throw new ArgumentNullException(nameof(configurationRepository));
        }

        public List<ConfigurationDto> GetUserConfigurations(string userEmail)
        {
            return configurationRepository_.GetUserConfigurations(userEmail);
        }

        public string DeleteUserConfiguration(string userEmail, int configId)
        {
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                throw new ArgumentException("User email cannot be null or empty.", nameof(userEmail));
            }

            try
            {
                // Попытка удаления:
                bool ok = configurationRepository_.DeleteConfigurationByIdAndUser(userEmail, configId);

                // Обработка результата:
                if (ok)
                {
                    // Если ok == true → вернуть пустую строку "" (успех).
                    return "";
                }
                else
                {
                    // Если ok == false → вернуть: «Невозможно удалить: конфигурация не найдена или не принадлежит вам».
                    return "Невозможно удалить: конфигурация не найдена или не принадлежит вам";
                }
            }
            // Обработка исключений (в блоке try/catch):
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                // Любая MySqlException (ошибки соединения, синтаксиса, транзакции и т.п.):
                // «Вероятно, проблемы в соединении с БД: {ex.Message}».
                return $"Вероятно, проблемы в соединении с БД: {ex.Message}";
            }
            // Обработка прочих исключений (например, если connectionString неверный и т.п.)
            catch (Exception ex)
            {
                // На всякий случай, если возникнет другая ошибка, тоже возвращаем сообщение
                return $"Произошла ошибка при удалении: {ex.Message}";
            }
        }

        public List<ConfigurationDto> GetPresetConfigurations()
        {
            return configurationRepository_.GetPresetConfigurations();
        }

        public int CreateConfiguration(Configuration configuration, List<int> componentIds)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            if (string.IsNullOrWhiteSpace(configuration.ConfigName))
                throw new ArgumentException("Название конфигурации обязательно", nameof(configuration.ConfigName));

            if (string.IsNullOrWhiteSpace(configuration.UserEmail))
                throw new ArgumentException("Email пользователя обязателен", nameof(configuration.UserEmail));

            if (componentIds == null || !componentIds.Any())
                throw new ArgumentException("Должен быть выбран хотя бы один компонент", nameof(componentIds));

            try
            {
                // Устанавливаем значения по умолчанию
                configuration.CreatedDate = DateTime.Now;
                configuration.Status = "draft";
                configuration.IsPreset = false;

                // Рассчитываем общую сумму (если нужно)
                // Здесь можно добавить логику подсчета суммы из компонентов

                return configurationRepository_.CreateConfiguration(configuration, componentIds);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError("ConfigurationService.CreateConfiguration", ex.Message);
                throw;
            }
        }
    }
}
