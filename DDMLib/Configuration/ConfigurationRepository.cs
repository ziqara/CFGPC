using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDMLib.Configuration
{
    public class ConfigurationRepository : IConfigurationRepository
    {
        public List<ConfigurationDto> GetUserConfigurations(string userEmail)
        {
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                throw new ArgumentException("User email cannot be null or empty.", nameof(userEmail));
            }

            List<ConfigurationDto> result = new List<ConfigurationDto>();

            using (MySqlConnection connection = new MySqlConnection(Config.ConnectionString))
            {
                try
                {
                    connection.Open();

                    // Запрос для получения основных данных конфигураций
                    string configurationQuery = @"
                        SELECT configId, configName, description, totalPrice, targetUse, status, isPreset, createdDate, userEmail, rgb, otherOptions
                        FROM configurations
                        WHERE userEmail = @userEmail";

                    MySqlCommand command = new MySqlCommand(configurationQuery, connection);
                    command.Parameters.AddWithValue("@userEmail", userEmail);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Configuration configuration = MapConfigurationFromReader(reader);

                            // Получаем список компонентов для текущей конфигурации
                            List<DDMLib.Component.Component> components = GetComponentsForConfiguration(connection, configuration.ConfigId);

                            ConfigurationDto dto = new ConfigurationDto
                            {
                                Configuration = configuration,
                                Components = components
                            };

                            result.Add(dto);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogger.LogError("GetUserConfigurations", ex.Message);
                    throw; // Пробрасываем исключение выше
                }
            }

            return result;
        }

        private Configuration MapConfigurationFromReader(MySqlDataReader reader)
        {
            // Получаем индексы колонок
            int iId = reader.GetOrdinal("configId");
            int iName = reader.GetOrdinal("configName");
            int iDesc = reader.GetOrdinal("description");
            int iPrice = reader.GetOrdinal("totalPrice");
            int iTarget = reader.GetOrdinal("targetUse");
            int iStatus = reader.GetOrdinal("status");
            int iPreset = reader.GetOrdinal("isPreset");
            int iDate = reader.GetOrdinal("createdDate");
            int iEmail = reader.GetOrdinal("userEmail");
            int iRgb = reader.GetOrdinal("rgb");
            int iOther = reader.GetOrdinal("otherOptions");

            // Функции для безопасного получения значений
            Func<int, string> GetStringOrEmpty = i => reader.IsDBNull(i) ? string.Empty : reader.GetString(i);
            Func<int, decimal> GetDecimalOrZero = i => reader.IsDBNull(i) ? 0m : reader.GetDecimal(i);
            Func<int, bool> GetBoolOrFalse = i => reader.IsDBNull(i) ? false : reader.GetBoolean(i);
            Func<int, DateTime> GetDateTimeOrMin = i => reader.IsDBNull(i) ? DateTime.MinValue : reader.GetDateTime(i);

            return new Configuration
            {
                ConfigId = reader.GetInt32(iId),
                ConfigName = GetStringOrEmpty(iName),
                Description = GetStringOrEmpty(iDesc),
                TotalPrice = GetDecimalOrZero(iPrice),
                TargetUse = GetStringOrEmpty(iTarget),
                Status = GetStringOrEmpty(iStatus),
                IsPreset = GetBoolOrFalse(iPreset),
                CreatedDate = GetDateTimeOrMin(iDate),
                UserEmail = GetStringOrEmpty(iEmail),
                Rgb = GetBoolOrFalse(iRgb),
                OtherOptions = GetStringOrEmpty(iOther)
            };
        }

        private List<DDMLib.Component.Component> GetComponentsForConfiguration(MySqlConnection connection, int configId)
        {
            List<DDMLib.Component.Component> components = new List<DDMLib.Component.Component>();

            // Подзапрос для получения компонентов конкретной конфигурации
            string componentQuery = @"
                SELECT c.componentId, c.name, c.brand, c.model, c.type, c.price, c.stockQuantity, c.description, c.isAvailable, c.photoUrl, c.supplierId
                FROM config_components cc
                JOIN components c ON cc.componentId = c.componentId
                WHERE cc.configId = @configId";

            MySqlCommand command = new MySqlCommand(componentQuery, connection);
            command.Parameters.AddWithValue("@configId", configId);

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    DDMLib.Component.Component component = MapComponentFromReader(reader);
                    components.Add(component);
                }
            }

            return components;
        }

        private DDMLib.Component.Component MapComponentFromReader(MySqlDataReader reader)
        {
            // Получаем индексы колонок
            int iId = reader.GetOrdinal("componentId");
            int iName = reader.GetOrdinal("name");
            int iBrand = reader.GetOrdinal("brand");
            int iModel = reader.GetOrdinal("model");
            int iType = reader.GetOrdinal("type");
            int iPrice = reader.GetOrdinal("price");
            int iStock = reader.GetOrdinal("stockQuantity");
            int iDesc = reader.GetOrdinal("description");
            int iAvailable = reader.GetOrdinal("isAvailable");
            int iPhoto = reader.GetOrdinal("photoUrl");
            int iSupplier = reader.GetOrdinal("supplierId");

            // Функции для безопасного получения значений
            Func<int, string> GetStringOrEmpty = i => reader.IsDBNull(i) ? string.Empty : reader.GetString(i);
            Func<int, int> GetInt32OrZero = i => reader.IsDBNull(i) ? 0 : reader.GetInt32(i);
            Func<int, decimal> GetDecimalOrZero = i => reader.IsDBNull(i) ? 0m : reader.GetDecimal(i);
            Func<int, bool> GetBoolOrFalse = i => reader.IsDBNull(i) ? false : reader.GetBoolean(i);

            return new DDMLib.Component.Component
            {
                ComponentId = GetInt32OrZero(iId),
                Name = GetStringOrEmpty(iName),
                Brand = GetStringOrEmpty(iBrand),
                Model = GetStringOrEmpty(iModel),
                Type = GetStringOrEmpty(iType),
                Price = GetDecimalOrZero(iPrice),
                StockQuantity = GetInt32OrZero(iStock),
                Description = GetStringOrEmpty(iDesc),
                IsAvailable = GetBoolOrFalse(iAvailable),
                PhotoUrl = GetStringOrEmpty(iPhoto),
                SupplierId = GetInt32OrZero(iSupplier)
            };
        }
    }
}
