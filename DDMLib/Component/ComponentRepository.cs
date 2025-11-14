using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDMLib.Component
{
    public class ComponentRepository : IComponentRepository
    {
        public List<ComponentDto> GetComponentsByCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                throw new ArgumentException("Категория не может быть пустой.", nameof(category));
            }

            HashSet<string> validCategories = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "cpu", "motherboard", "ram", "gpu", "storage", "psu", "case", "cooling"
            };

            if (!validCategories.Contains(category))
            {
                throw new ArgumentException("Недопустимое значение категории.", nameof(category));
            }

            List<ComponentDto> result = new List<ComponentDto>();

            using (MySqlConnection connection = new MySqlConnection(Config.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string componentQuery = @"
                        SELECT component_id, name, brand, model, type, price, stock_quantity, description, is_available, photo_url, supplier_id
                        FROM components
                        WHERE type = @category";

                    MySqlCommand command = new MySqlCommand(componentQuery, connection);
                    command.Parameters.AddWithValue("@category", category);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Component component = MapComponentFromReader(reader);

                            object spec = GetSpecificSpec(connection, component.ComponentId, category);

                            ComponentDto dto = new ComponentDto
                            {
                                Component = component,
                                Specs = spec
                            };

                            result.Add(dto);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogger.LogError("GetComponentsByCategory", ex.Message);
                    throw;
                }
            }
        }

        private Component MapComponentFromReader(MySqlDataReader reader)
        {
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

            Func<int, string> GetStringOrEmpty = i => reader.IsDBNull(i) ? string.Empty : reader.GetString(i);
            Func<int, int> GetInt32OrZero = i => reader.IsDBNull(i) ? 0 : reader.GetInt32(i);
            Func<int, decimal> GetDecimalOrZero = i => reader.IsDBNull(i) ? 0m : reader.GetDecimal(i);
            Func<int, bool> GetBoolOrFalse = i => reader.IsDBNull(i) ? false : reader.GetBoolean(i);

            return new Component
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

        private object GetSpecificSpec(MySqlConnection connection, int componentId, string category)
        {
            MySqlCommand specCommand;
            MySqlDataReader specReader;

            switch (category.ToLower())
            {

            }

            return null;
        }
    }
}
