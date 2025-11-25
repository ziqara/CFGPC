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
            throw new NotImplementedException();
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
    }
}
