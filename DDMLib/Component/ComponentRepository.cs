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

            List<Component> components = new List<Component>();
            List<int> componentIds = new List<int>();

            using (MySqlConnection connection = new MySqlConnection(Config.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string componentQuery = @"
                        SELECT componentId, name, brand, model, componentType, price, stockQuantity, description, isAvailable, photoUrl, supplierInn
                        FROM components
                        WHERE componentType = @category";

                    MySqlCommand command = new MySqlCommand(componentQuery, connection);
                    command.Parameters.AddWithValue("@category", category);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Component component = MapComponentFromReader(reader);
                            components.Add(component);
                            componentIds.Add(component.ComponentId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogger.LogError("GetComponentsByCategory", ex.Message);
                    throw;
                }
            }

            Dictionary<int, object> specMap = new Dictionary<int, object>();

            if (componentIds.Any())
            {
                if (category == "gpu")
                {
                    using (MySqlConnection connection = new MySqlConnection(Config.ConnectionString))
                    {
                        connection.Open();
                        string specQuery = @"
                            SELECT componentId, pcieVersion, tdp, vramGb
                            FROM gpus
                            WHERE componentId IN (" + string.Join(",", componentIds) + ")";

                        MySqlCommand specCommand = new MySqlCommand(specQuery, connection);
                        using (MySqlDataReader specReader = specCommand.ExecuteReader())
                        {
                            while (specReader.Read())
                            {
                                int compId = specReader.GetInt32("componentId");
                                int iPcie = specReader.GetOrdinal("pcieVersion");
                                int iTdp = specReader.GetOrdinal("tdp");
                                int iVram = specReader.GetOrdinal("vramGb");

                                Func<int, string> GetStringOrEmpty = i => specReader.IsDBNull(i) ? string.Empty : specReader.GetString(i);
                                Func<int, int> GetInt32OrZero = i => specReader.IsDBNull(i) ? 0 : specReader.GetInt32(i);

                                var spec = new GpuSpec
                                {
                                    PcieVersion = GetStringOrEmpty(iPcie),
                                    Tdp = GetInt32OrZero(iTdp),
                                    VramGb = GetInt32OrZero(iVram)
                                };
                                specMap[compId] = spec;
                            }
                        }
                    }
                }
                else if (category == "ram")
                {
                    using (MySqlConnection connection = new MySqlConnection(Config.ConnectionString))
                    {
                        connection.Open();
                        string specQuery = @"
            SELECT componentId, ramType, capacityGb, speedMhz, slotsNeeded
            FROM rams
            WHERE componentId IN (" + string.Join(",", componentIds) + ")";

                        MySqlCommand specCommand = new MySqlCommand(specQuery, connection);
                        using (MySqlDataReader specReader = specCommand.ExecuteReader())
                        {
                            while (specReader.Read())
                            {
                                int compId = specReader.GetInt32("componentId");
                                int iType = specReader.GetOrdinal("ramType");
                                int iCap = specReader.GetOrdinal("capacityGb");
                                int iSpeed = specReader.GetOrdinal("speedMhz");
                                int iSlots = specReader.GetOrdinal("slotsNeeded");

                                Func<int, string> GetStringOrEmpty = i => specReader.IsDBNull(i) ? string.Empty : specReader.GetString(i);
                                Func<int, int> GetInt32OrZero = i => specReader.IsDBNull(i) ? 0 : specReader.GetInt32(i);

                                var spec = new RamSpec
                                {
                                    RamType = GetStringOrEmpty(iType), // Исправлено: RamType вместо Type
                                    CapacityGb = GetInt32OrZero(iCap),
                                    SpeedMhz = GetInt32OrZero(iSpeed),
                                    SlotsNeeded = GetInt32OrZero(iSlots)
                                };
                                specMap[compId] = spec;
                            }
                        }
                    }
                }
                else if (category == "cpu")
                {
                    using (MySqlConnection connection = new MySqlConnection(Config.ConnectionString))
                    {
                        connection.Open();
                        string specQuery = @"
                            SELECT componentId, socket, cores, tdp
                            FROM cpus
                            WHERE componentId IN (" + string.Join(",", componentIds) + ")";

                        MySqlCommand specCommand = new MySqlCommand(specQuery, connection);
                        using (MySqlDataReader specReader = specCommand.ExecuteReader())
                        {
                            while (specReader.Read())
                            {
                                int compId = specReader.GetInt32("componentId");
                                int iSocket = specReader.GetOrdinal("socket");
                                int iCores = specReader.GetOrdinal("cores");
                                int iTdp = specReader.GetOrdinal("tdp");

                                Func<int, string> GetStringOrEmpty = i => specReader.IsDBNull(i) ? string.Empty : specReader.GetString(i);
                                Func<int, int> GetInt32OrZero = i => specReader.IsDBNull(i) ? 0 : specReader.GetInt32(i);

                                var spec = new CpuSpec
                                {
                                    Socket = GetStringOrEmpty(iSocket),
                                    Cores = GetInt32OrZero(iCores),
                                    Tdp = GetInt32OrZero(iTdp)
                                };
                                specMap[compId] = spec;
                            }
                        }
                    }
                }
                else if (category == "motherboard")
                {
                    using (MySqlConnection connection = new MySqlConnection(Config.ConnectionString))
                    {
                        connection.Open();
                        string specQuery = @"
                            SELECT componentId, socket, chipset, ramType, pcieVersion, formFactor
                            FROM motherboards
                            WHERE componentId IN (" + string.Join(",", componentIds) + ")";

                        MySqlCommand specCommand = new MySqlCommand(specQuery, connection);
                        using (MySqlDataReader specReader = specCommand.ExecuteReader())
                        {
                            while (specReader.Read())
                            {
                                int compId = specReader.GetInt32("componentId");
                                int iSocket = specReader.GetOrdinal("socket");
                                int iChipset = specReader.GetOrdinal("chipset");
                                int iRamType = specReader.GetOrdinal("ramType");
                                int iPcie = specReader.GetOrdinal("pcieVersion");
                                int iFormFactor = specReader.GetOrdinal("formFactor");

                                Func<int, string> GetStringOrEmpty = i => specReader.IsDBNull(i) ? string.Empty : specReader.GetString(i);

                                var spec = new MotherboardSpec
                                {
                                    Socket = GetStringOrEmpty(iSocket),
                                    Chipset = GetStringOrEmpty(iChipset),
                                    RamType = GetStringOrEmpty(iRamType),
                                    PcieVersion = GetStringOrEmpty(iPcie),
                                    FormFactor = GetStringOrEmpty(iFormFactor)
                                };
                                specMap[compId] = spec;
                            }
                        }
                    }
                }
                else if (category == "storage")
                {
                    using (MySqlConnection connection = new MySqlConnection(Config.ConnectionString))
                    {
                        connection.Open();
                        string specQuery = @"
                            SELECT componentId, interface, capacityGb
                            FROM storages
                            WHERE componentId IN (" + string.Join(",", componentIds) + ")";

                        MySqlCommand specCommand = new MySqlCommand(specQuery, connection);
                        using (MySqlDataReader specReader = specCommand.ExecuteReader())
                        {
                            while (specReader.Read())
                            {
                                int compId = specReader.GetInt32("componentId");
                                int iInterface = specReader.GetOrdinal("interface");
                                int iCapacity = specReader.GetOrdinal("capacityGb");

                                Func<int, string> GetStringOrEmpty = i => specReader.IsDBNull(i) ? string.Empty : specReader.GetString(i);
                                Func<int, int> GetInt32OrZero = i => specReader.IsDBNull(i) ? 0 : specReader.GetInt32(i);

                                var spec = new StorageSpec
                                {
                                    Interface = GetStringOrEmpty(iInterface),
                                    CapacityGb = GetInt32OrZero(iCapacity)
                                };
                                specMap[compId] = spec;
                            }
                        }
                    }
                }
                else if (category == "psu")
                {
                    using (MySqlConnection connection = new MySqlConnection(Config.ConnectionString))
                    {
                        connection.Open();
                        string specQuery = @"
                            SELECT componentId, wattage, efficiencyRating
                            FROM psus
                            WHERE componentId IN (" + string.Join(",", componentIds) + ")";

                        MySqlCommand specCommand = new MySqlCommand(specQuery, connection);
                        using (MySqlDataReader specReader = specCommand.ExecuteReader())
                        {
                            while (specReader.Read())
                            {
                                int compId = specReader.GetInt32("componentId");
                                int iWattage = specReader.GetOrdinal("wattage");
                                int iEfficiency = specReader.GetOrdinal("efficiencyRating");

                                Func<int, string> GetStringOrEmpty = i => specReader.IsDBNull(i) ? string.Empty : specReader.GetString(i);
                                Func<int, int> GetInt32OrZero = i => specReader.IsDBNull(i) ? 0 : specReader.GetInt32(i);

                                var spec = new PsuSpec
                                {
                                    Wattage = GetInt32OrZero(iWattage),
                                    EfficiencyRating = GetStringOrEmpty(iEfficiency)
                                };
                                specMap[compId] = spec;
                            }
                        }
                    }
                }
                else if (category == "case")
                {
                    using (MySqlConnection connection = new MySqlConnection(Config.ConnectionString))
                    {
                        connection.Open();
                        string specQuery = @"
                            SELECT componentId, formFactor, size
                            FROM cases
                            WHERE componentId IN (" + string.Join(",", componentIds) + ")";

                        MySqlCommand specCommand = new MySqlCommand(specQuery, connection);
                        using (MySqlDataReader specReader = specCommand.ExecuteReader())
                        {
                            while (specReader.Read())
                            {
                                int compId = specReader.GetInt32("componentId");
                                int iFormFactor = specReader.GetOrdinal("formFactor");
                                int iSize = specReader.GetOrdinal("size");

                                Func<int, string> GetStringOrEmpty = i => specReader.IsDBNull(i) ? string.Empty : specReader.GetString(i);

                                var spec = new CaseSpec
                                {
                                    FormFactor = GetStringOrEmpty(iFormFactor),
                                    Size = GetStringOrEmpty(iSize)
                                };
                                specMap[compId] = spec;
                            }
                        }
                    }
                }
                else if (category == "cooling")
                {
                    using (MySqlConnection connection = new MySqlConnection(Config.ConnectionString))
                    {
                        connection.Open();
                        string specQuery = @"
                            SELECT componentId, coolerType, tdpSupport, fanRpm, size, isRgb
                            FROM coolings
                            WHERE componentId IN (" + string.Join(",", componentIds) + ")";

                        MySqlCommand specCommand = new MySqlCommand(specQuery, connection);
                        using (MySqlDataReader specReader = specCommand.ExecuteReader())
                        {
                            while (specReader.Read())
                            {
                                int compId = specReader.GetInt32("componentId");
                                int iCoolerType = specReader.GetOrdinal("coolerType");
                                int iTdpSupport = specReader.GetOrdinal("tdpSupport");
                                int iFanRpm = specReader.GetOrdinal("fanRpm");
                                int iSize = specReader.GetOrdinal("size");
                                int iIsRgb = specReader.GetOrdinal("isRgb");

                                Func<int, string> GetStringOrEmpty = i => specReader.IsDBNull(i) ? string.Empty : specReader.GetString(i);
                                Func<int, int> GetInt32OrZero = i => specReader.IsDBNull(i) ? 0 : specReader.GetInt32(i);
                                Func<int, bool> GetBoolOrFalse = i => specReader.IsDBNull(i) ? false : specReader.GetBoolean(i);

                                var spec = new CoolingSpec
                                {
                                    CoolerType = GetStringOrEmpty(iCoolerType),
                                    TdpSupport = GetInt32OrZero(iTdpSupport),
                                    FanRpm = GetInt32OrZero(iFanRpm),
                                    Size = GetStringOrEmpty(iSize),
                                    IsRgb = GetBoolOrFalse(iIsRgb)
                                };
                                specMap[compId] = spec;
                            }
                        }
                    }
                }
            }

            List<ComponentDto> result = new List<ComponentDto>();
            foreach (var comp in components)
            {
                object spec = specMap.ContainsKey(comp.ComponentId) ? specMap[comp.ComponentId] : null;
                result.Add(new ComponentDto
                {
                    Component = comp,
                    Specs = spec
                });
            }

            return result;
        }

        private Component MapComponentFromReader(MySqlDataReader reader)
        {
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

        public T GetComponentSpec<T>(int componentId) where T : class
        {
            string tableName = GetTableNameForType<T>();
            if (string.IsNullOrEmpty(tableName))
                return null;

            using (MySqlConnection connection = new MySqlConnection(Config.ConnectionString))
            {
                connection.Open();
                string sql = $"SELECT * FROM {tableName} WHERE componentId = @ComponentId";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ComponentId", componentId);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapSpecFromReader<T>(reader);
                        }
                    }
                }
            }
            return null;
        }

        // Эти методы теперь private, так как они не в интерфейсе
        private string GetTableNameForType<T>()
        {
            if (typeof(T) == typeof(CpuSpec)) return "cpus";
            if (typeof(T) == typeof(MotherboardSpec)) return "motherboards";
            if (typeof(T) == typeof(RamSpec)) return "rams";
            if (typeof(T) == typeof(GpuSpec)) return "gpus";
            if (typeof(T) == typeof(StorageSpec)) return "storages";
            if (typeof(T) == typeof(PsuSpec)) return "psus";
            if (typeof(T) == typeof(CaseSpec)) return "cases";
            if (typeof(T) == typeof(CoolingSpec)) return "coolings";
            return null;
        }

        private T MapSpecFromReader<T>(MySqlDataReader reader) where T : class
        {
            if (typeof(T) == typeof(CpuSpec))
            {
                return new CpuSpec
                {
                    ComponentId = reader.GetInt32("componentId"),
                    Socket = reader.GetString("socket"),
                    Cores = reader.GetInt32("cores"),
                    Tdp = reader.GetInt32("tdp")
                } as T;
            }
            if (typeof(T) == typeof(MotherboardSpec))
            {
                return new MotherboardSpec
                {
                    ComponentId = reader.GetInt32("componentId"),
                    Socket = reader.GetString("socket"),
                    Chipset = reader.GetString("chipset"),
                    RamType = reader.GetString("ramType"),
                    PcieVersion = reader.GetString("pcieVersion"),
                    FormFactor = reader.GetString("formFactor")
                } as T;
            }
            if (typeof(T) == typeof(RamSpec))
            {
                return new RamSpec
                {
                    ComponentId = reader.GetInt32("componentId"),
                    RamType = reader.GetString("ramType"), // Исправлено: RamType вместо Type
                    CapacityGb = reader.GetInt32("capacityGb"),
                    SpeedMhz = reader.GetInt32("speedMhz"),
                    SlotsNeeded = reader.GetInt32("slotsNeeded")
                } as T;
            }
            if (typeof(T) == typeof(GpuSpec))
            {
                return new GpuSpec
                {
                    ComponentId = reader.GetInt32("componentId"),
                    PcieVersion = reader.GetString("pcieVersion"),
                    Tdp = reader.GetInt32("tdp"),
                    VramGb = reader.GetInt32("vramGb")
                } as T;
            }
            if (typeof(T) == typeof(StorageSpec))
            {
                return new StorageSpec
                {
                    ComponentId = reader.GetInt32("componentId"),
                    Interface = reader.GetString("interface"),
                    CapacityGb = reader.GetInt32("capacityGb")
                } as T;
            }
            if (typeof(T) == typeof(PsuSpec))
            {
                return new PsuSpec
                {
                    ComponentId = reader.GetInt32("componentId"),
                    Wattage = reader.GetInt32("wattage"),
                    EfficiencyRating = reader.GetString("efficiencyRating")
                } as T;
            }
            if (typeof(T) == typeof(CaseSpec))
            {
                return new CaseSpec
                {
                    ComponentId = reader.GetInt32("componentId"),
                    FormFactor = reader.GetString("formFactor"),
                    Size = reader.GetString("size")
                } as T;
            }
            if (typeof(T) == typeof(CoolingSpec))
            {
                return new CoolingSpec
                {
                    ComponentId = reader.GetInt32("componentId"),
                    CoolerType = reader.GetString("coolerType"),
                    TdpSupport = reader.GetInt32("tdpSupport"),
                    FanRpm = reader.GetInt32("fanRpm"),
                    Size = reader.IsDBNull(reader.GetOrdinal("size")) ? null : reader.GetString("size"),
                    IsRgb = reader.GetBoolean("isRgb")
                } as T;
            }
            return null;
        }

        public Component GetComponentById(int componentId)
        {
            using (MySqlConnection connection = new MySqlConnection(Config.ConnectionString))
            {
                try
                {
                    connection.Open();

                    string sql = @"
                        SELECT componentId, name, brand, model, componentType, price, 
                               stockQuantity, description, isAvailable, photoUrl, supplierInn
                        FROM components 
                        WHERE componentId = @ComponentId";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ComponentId", componentId);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapComponentFromReader(reader);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogger.LogError("GetComponentById", ex.Message);
                    throw;
                }
            }
            return null;
        }
    }
}