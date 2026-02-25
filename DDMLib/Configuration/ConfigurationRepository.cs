using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDMLib.Component;
using DDMLib.Configurations;

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

            List<Configuration> configurations = new List<Configuration>();
            List<int> configIds = new List<int>();

            using (MySqlConnection connection = new MySqlConnection(Config.ConnectionString))
            {
                try
                {
                    connection.Open();

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
                            Configuration config = MapConfigurationFromReader(reader);
                            configurations.Add(config);
                            configIds.Add(config.ConfigId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogger.LogError("GetUserConfigurations", ex.Message);
                    throw; // Пробрасываем исключение выше
                }
            }

            // 2. Получаем компоненты для всех конфигураций
            // Создаём словарь: ConfigId -> List<Component>
            Dictionary<int, List<DDMLib.Component.Component>> componentsMap = new Dictionary<int, List<DDMLib.Component.Component>>();

            if (configIds.Any())
            {
                using (MySqlConnection connection = new MySqlConnection(Config.ConnectionString))
                {
                    connection.Open();

                    // Запрос для получения компонентов, связанных с конфигурациями пользователя
                    // Исправлено: c.supplierId -> c.supplierInn
                    string componentQuery = @"
                        SELECT cc.configId, c.componentId, c.name, c.brand, c.model, c.componentType, c.price, c.stockQuantity, c.description, c.isAvailable, c.photoUrl, c.supplierInn
                        FROM config_components cc
                        JOIN components c ON cc.componentId = c.componentId
                        WHERE cc.configId IN (" + string.Join(",", configIds) + ")";

                    MySqlCommand command = new MySqlCommand(componentQuery, connection);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int configId = reader.GetInt32("configId");
                            DDMLib.Component.Component component = MapComponentFromReader(reader);

                            if (!componentsMap.ContainsKey(configId))
                            {
                                componentsMap[configId] = new List<DDMLib.Component.Component>();
                            }
                            componentsMap[configId].Add(component);
                        }
                    }
                }
            }

            // 3. Объединяем конфигурации с их компонентами
            List<ConfigurationDto> result = new List<ConfigurationDto>();
            foreach (var config in configurations)
            {
                List<DDMLib.Component.Component> components = componentsMap.ContainsKey(config.ConfigId) ? componentsMap[config.ConfigId] : new List<DDMLib.Component.Component>();
                result.Add(new ConfigurationDto
                {
                    Configuration = config,
                    Components = components
                });
            }

            return result;
        }

        /// <summary>
        /// Возвращает все предустановленные конфигурации (у которых isPreset = 1)
        /// </summary>
        /// <returns>Список DTO предустановленных конфигураций</returns>
        public List<ConfigurationDto> GetPresetConfigurations()
        {
            List<Configuration> configurations = new List<Configuration>();
            List<int> configIds = new List<int>();

            using (MySqlConnection connection = new MySqlConnection(Config.ConnectionString))
            {
                try
                {
                    connection.Open();

                    string configurationQuery = @"
                        SELECT configId, configName, description, totalPrice, targetUse, status, isPreset, createdDate, userEmail, rgb, otherOptions
                        FROM configurations
                        WHERE isPreset = 1";

                    MySqlCommand command = new MySqlCommand(configurationQuery, connection);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Configuration config = MapConfigurationFromReader(reader);
                            configurations.Add(config);
                            configIds.Add(config.ConfigId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogger.LogError("GetPresetConfigurations", ex.Message);
                    throw; // Пробрасываем исключение выше
                }
            }

            // 2. Получаем компоненты для всех предустановленных конфигураций
            // Создаём словарь: ConfigId -> List<Component>
            Dictionary<int, List<DDMLib.Component.Component>> componentsMap = new Dictionary<int, List<DDMLib.Component.Component>>();

            if (configIds.Any())
            {
                using (MySqlConnection connection = new MySqlConnection(Config.ConnectionString))
                {
                    connection.Open();

                    // Запрос для получения компонентов, связанных с предустановленными конфигурациями
                    string componentQuery = @"
                        SELECT cc.configId, c.componentId, c.name, c.brand, c.model, c.componentType, c.price, c.stockQuantity, c.description, c.isAvailable, c.photoUrl, c.supplierInn
                        FROM config_components cc
                        JOIN components c ON cc.componentId = c.componentId
                        WHERE cc.configId IN (" + string.Join(",", configIds) + ")";

                    MySqlCommand command = new MySqlCommand(componentQuery, connection);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int configId = reader.GetInt32("configId");
                            DDMLib.Component.Component component = MapComponentFromReader(reader);

                            if (!componentsMap.ContainsKey(configId))
                            {
                                componentsMap[configId] = new List<DDMLib.Component.Component>();
                            }
                            componentsMap[configId].Add(component);
                        }
                    }
                }
            }

            // 3. Объединяем конфигурации с их компонентами
            List<ConfigurationDto> result = new List<ConfigurationDto>();
            foreach (var config in configurations)
            {
                List<DDMLib.Component.Component> components = componentsMap.ContainsKey(config.ConfigId) ? componentsMap[config.ConfigId] : new List<DDMLib.Component.Component>();
                result.Add(new ConfigurationDto
                {
                    Configuration = config,
                    Components = components
                });
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

        private DDMLib.Component.Component MapComponentFromReader(MySqlDataReader reader)
        {
            // Получаем индексы колонок
            int iId = reader.GetOrdinal("componentId");
            int iName = reader.GetOrdinal("name");
            int iBrand = reader.GetOrdinal("brand");
            int iModel = reader.GetOrdinal("model");
            int iType = reader.GetOrdinal("componentType");
            int iPrice = reader.GetOrdinal("price");
            int iStock = reader.GetOrdinal("stockQuantity");
            int iDesc = reader.GetOrdinal("description");
            int iAvailable = reader.GetOrdinal("isAvailable");
            int iPhoto = reader.GetOrdinal("photoUrl");
            int iSupplier = reader.GetOrdinal("supplierInn");

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

        public bool DeleteConfigurationByIdAndUser(string userEmail, int configId)
        {
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                throw new ArgumentException("User email cannot be null or empty.", nameof(userEmail));
            }

            using (MySqlConnection connection = new MySqlConnection(Config.ConnectionString))
            {
                try
                {
                    connection.Open();
                    using (MySqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Удаляем сначала связанные компоненты из config_components
                            string deleteComponentsQuery = "DELETE FROM config_components WHERE configId = @configId;";
                            using (MySqlCommand cmdComponents = new MySqlCommand(deleteComponentsQuery, connection, transaction))
                            {
                                cmdComponents.Parameters.AddWithValue("@configId", configId);
                                cmdComponents.ExecuteNonQuery(); // Результат не важен, если строка не найдена - это нормально
                            }

                            // Затем удаляем саму конфигурацию из configurations
                            string deleteConfigQuery = "DELETE FROM configurations WHERE configId = @configId AND userEmail = @userEmail;";
                            using (MySqlCommand cmdConfig = new MySqlCommand(deleteConfigQuery, connection, transaction))
                            {
                                cmdConfig.Parameters.AddWithValue("@configId", configId);
                                cmdConfig.Parameters.AddWithValue("@userEmail", userEmail);

                                int rowsAffected = cmdConfig.ExecuteNonQuery();

                                if (rowsAffected == 1)
                                {
                                    transaction.Commit(); // Успешно удалили и компоненты, и конфигурацию
                                    return true;
                                }
                                else
                                {
                                    transaction.Rollback();
                                    return false;
                                }
                            }
                        }
                        catch (MySqlException ex)
                        {
                            transaction.Rollback(); // Откатываем транзакцию при любой ошибке
                            throw; // Пробрасываем исключение выше
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    // Обработка ошибок подключения или других исключений на уровне соединения
                    ErrorLogger.LogError("DeleteConfigurationByIdAndUser", ex.Message);
                    throw; // Пробрасываем исключение выше для обработки в сервисе
                }
            }
        }

        public ConfigurationDetails GetDetails(int configId)
        {
            var details = new ConfigurationDetails { ConfigId = configId };

            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();

                // 1) Название конфигурации
                using (var cmd = new MySqlCommand(
                    "SELECT configName FROM configurations WHERE configId=@id LIMIT 1;", conn))
                {
                    cmd.Parameters.AddWithValue("@id", configId);
                    var name = cmd.ExecuteScalar();
                    details.ConfigName = name == null ? "" : Convert.ToString(name);
                }

                // 2) Состав конфигурации
                // Ожидаем таблицы:
                // config_components(configId, componentId, quantity)
                // components(componentId, name, brand, model, componentType, price, supplierInn)
                // suppliers(inn, name)
                string sql = @"
                    SELECT
                        c.componentType,
                        c.name,
                        c.brand,
                        c.model,
                        cc.quantity,
                        c.price,
                        s.name AS supplierName
                    FROM config_components cc
                    JOIN components c ON cc.componentId = c.componentId
                    LEFT JOIN suppliers s ON c.supplierInn = s.inn
                    WHERE cc.configId = @id
                    ORDER BY c.componentType, c.name;";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", configId);

                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            string typeRaw = r.IsDBNull(0) ? "" : r.GetString(0);

                            details.Components.Add(new ConfigComponentInfo
                            {
                                ComponentType = ToRussianComponentType(typeRaw),
                                ComponentName = r.IsDBNull(1) ? "" : r.GetString(1),
                                Brand = r.IsDBNull(2) ? "—" : r.GetString(2),
                                Model = r.IsDBNull(3) ? "—" : r.GetString(3),
                                Quantity = r.IsDBNull(4) ? 1 : r.GetInt32(4),
                                Price = r.IsDBNull(5) ? 0m : r.GetDecimal(5),
                                SupplierName = r.IsDBNull(6) ? "—" : r.GetString(6)
                            });
                        }
                    }
                }
            }

            return details;
        }

        private string ToRussianComponentType(string type)
        {
            switch ((type ?? "").Trim().ToLower())
            {
                case "cpu": return "Процессор";
                case "gpu": return "Видеокарта";
                case "ram": return "ОЗУ";
                case "motherboard": return "Материнская плата";
                case "storage": return "Накопитель";
                case "psu": return "Блок питания";
                case "case": return "Корпус";
                case "cooling": return "Охлаждение";
                default: return string.IsNullOrWhiteSpace(type) ? "—" : type;
            }
        }

        public int CreateConfiguration(Configuration configuration, List<int> componentIds)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            if (componentIds == null || !componentIds.Any())
                throw new ArgumentException("Должен быть выбран хотя бы один компонент", nameof(componentIds));

            int newConfigId = 0;

            using (MySqlConnection connection = new MySqlConnection(Config.ConnectionString))
            {
                try
                {
                    connection.Open();
                    using (MySqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // 1. Вставляем конфигурацию
                            string insertConfigQuery = @"
                        INSERT INTO configurations 
                            (configName, description, totalPrice, targetUse, status, isPreset, createdDate, userEmail, rgb, otherOptions)
                        VALUES 
                            (@configName, @description, @totalPrice, @targetUse, @status, @isPreset, @createdDate, @userEmail, @rgb, @otherOptions);
                        SELECT LAST_INSERT_ID();";

                            using (MySqlCommand cmdConfig = new MySqlCommand(insertConfigQuery, connection, transaction))
                            {
                                cmdConfig.Parameters.AddWithValue("@configName", configuration.ConfigName);
                                cmdConfig.Parameters.AddWithValue("@description", configuration.Description ?? (object)DBNull.Value);
                                cmdConfig.Parameters.AddWithValue("@totalPrice", configuration.TotalPrice);
                                cmdConfig.Parameters.AddWithValue("@targetUse", configuration.TargetUse);
                                cmdConfig.Parameters.AddWithValue("@status", configuration.Status ?? "draft");
                                cmdConfig.Parameters.AddWithValue("@isPreset", configuration.IsPreset);
                                cmdConfig.Parameters.AddWithValue("@createdDate", configuration.CreatedDate);
                                cmdConfig.Parameters.AddWithValue("@userEmail", configuration.UserEmail);
                                cmdConfig.Parameters.AddWithValue("@rgb", configuration.Rgb);
                                cmdConfig.Parameters.AddWithValue("@otherOptions", configuration.OtherOptions ?? (object)DBNull.Value);

                                newConfigId = Convert.ToInt32(cmdConfig.ExecuteScalar());
                            }

                            // 2. Вставляем связи с компонентами
                            if (newConfigId > 0 && componentIds.Any())
                            {
                                string insertComponentsQuery = @"
                            INSERT INTO config_components (configId, componentId, quantity)
                            VALUES (@configId, @componentId, 1)";

                                using (MySqlCommand cmdComponents = new MySqlCommand(insertComponentsQuery, connection, transaction))
                                {
                                    cmdComponents.Parameters.Add("@configId", MySqlDbType.Int32);
                                    cmdComponents.Parameters.Add("@componentId", MySqlDbType.Int32);

                                    foreach (int componentId in componentIds)
                                    {
                                        cmdComponents.Parameters["@configId"].Value = newConfigId;
                                        cmdComponents.Parameters["@componentId"].Value = componentId;
                                        cmdComponents.ExecuteNonQuery();
                                    }
                                }
                            }

                            transaction.Commit();
                            return newConfigId;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            ErrorLogger.LogError("CreateConfiguration Transaction", ex.Message);
                            throw;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogger.LogError("CreateConfiguration", ex.Message);
                    throw;
                }
            }
        }
    }
}