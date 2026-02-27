using System;
using System.Collections.Generic;
using System.Linq;
using DDMLib.Configuration;
using DDMLib.Component;
using MySql.Data.MySqlClient;

// Псевдоним для устранения конфликта между пространством имен и классом
using ConfigModel = DDMLib.Configuration.Configuration;

namespace DDMLib
{
    public class MySqlBuildRepository : IBuildRepository
    {
        public List<BuildCard> ReadAllBuildCards(bool onlyPresets)
        {
            var list = new List<BuildCard>();

            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();

                string sql = @"
                SELECT 
                  cfg.configId, 
                  cfg.configName, 
                  cfg.totalPrice, 
                  cfg.isPreset, 
                  cfg.createdDate, 
                  cfg.userEmail,
                  COALESCE(o.ordersCount, 0) AS ordersCount,
                  COALESCE(b.badComponents, 0) AS badComponents
                FROM configurations cfg
                LEFT JOIN (
                   SELECT configId, COUNT(*) AS ordersCount 
                   FROM orders 
                   GROUP BY configId
                ) o ON o.configId = cfg.configId
                LEFT JOIN (
                   SELECT 
                     cc.configId, 
                     SUM(CASE WHEN c.isAvailable = 0 OR c.stockQuantity <= 0 THEN 1 ELSE 0 END) AS badComponents
                   FROM config_components cc
                   JOIN components c ON c.componentId = cc.componentId
                   GROUP BY cc.configId
                ) b ON b.configId = cfg.configId
                WHERE (@onlyPresets = 0 OR cfg.isPreset = 1)
                ORDER BY cfg.createdDate DESC, cfg.configId DESC;";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@onlyPresets", onlyPresets ? 1 : 0);

                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(new BuildCard
                            {
                                ConfigId = r.GetInt32("configId"),
                                ConfigName = r.GetString("configName"),
                                TotalPrice = r.IsDBNull(r.GetOrdinal("totalPrice")) ? 0m : r.GetDecimal("totalPrice"),
                                IsPreset = r.GetInt32("isPreset") == 1,
                                CreatedDate = r.GetDateTime("createdDate"),
                                UserEmail = r.IsDBNull(r.GetOrdinal("userEmail")) ? null : r.GetString("userEmail"),
                                OrdersCount = r.GetInt32("ordersCount"),
                                BadComponents = r.GetInt32("badComponents")
                            });
                        }
                    }
                }
            }

            return list;
        }

        public bool HasOrders(int configId)
        {
            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM orders WHERE configId=@id;", conn))
                {
                    cmd.Parameters.AddWithValue("@id", configId);
                    long count = Convert.ToInt64(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        public bool DeleteBuildIfNoOrders(int configId)
        {
            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();
                string sql = @"
                DELETE FROM configurations 
                WHERE configId = @id 
                  AND NOT EXISTS (SELECT 1 FROM orders WHERE configId = @id);";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", configId);
                    int affected = cmd.ExecuteNonQuery();
                    return affected > 0;
                }
            }
        }

        public bool AreAllComponentsAvailable(int[] componentIds, out int badCount)
        {
            badCount = 0;
            if (componentIds == null || componentIds.Length == 0) return false;

            string inClause = string.Join(",", componentIds.Select((x, i) => "@p" + i));

            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();
                string sql = $@"
                SELECT COUNT(*) AS bad 
                FROM components 
                WHERE componentId IN ({inClause}) 
                  AND (isAvailable = 0 OR stockQuantity <= 0);";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    for (int i = 0; i < componentIds.Length; i++)
                        cmd.Parameters.AddWithValue("@p" + i, componentIds[i]);

                    badCount = Convert.ToInt32(cmd.ExecuteScalar());
                    return badCount == 0;
                }
            }
        }

        public decimal SumComponentsPrice(int[] componentIds)
        {
            if (componentIds == null || componentIds.Length == 0) return 0m;

            string inClause = string.Join(",", componentIds.Select((x, i) => "@p" + i));

            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();
                string sql = $@"
                SELECT COALESCE(SUM(price),0) 
                FROM components 
                WHERE componentId IN ({inClause});";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    for (int i = 0; i < componentIds.Length; i++)
                        cmd.Parameters.AddWithValue("@p" + i, componentIds[i]);

                    return Convert.ToDecimal(cmd.ExecuteScalar());
                }
            }
        }

        public void EnsureAdminExists(string adminEmail)
        {
            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();
                string sql = @"
                INSERT IGNORE INTO users (email, passwordHash, fullName) 
                VALUES (@email, 'admin', 'Administrator');";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@email", adminEmail);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public int CreatePreset(BuildDraft draft, string adminEmail)
        {
            if (draft == null) throw new ArgumentNullException(nameof(draft));
            EnsureAdminExists(adminEmail);

            int[] ids = {
                draft.MotherboardId, draft.CpuId, draft.RamId, draft.GpuId,
                draft.StorageId, draft.PsuId, draft.CaseId, draft.CoolingId
            };

            decimal total = SumComponentsPrice(ids);

            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    try
                    {
                        string sqlInsertConfig = @"
                        INSERT INTO configurations 
                          (configName, description, totalPrice, isPreset, userEmail, status) 
                        VALUES 
                          (@name, @desc, @price, 1, @email, 'validated');";

                        int newId;
                        using (var cmd = new MySqlCommand(sqlInsertConfig, conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@name", draft.ConfigName);
                            cmd.Parameters.AddWithValue("@desc", (object)draft.Description ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@price", total);
                            cmd.Parameters.AddWithValue("@email", adminEmail);
                            cmd.ExecuteNonQuery();
                            newId = (int)cmd.LastInsertedId;
                        }

                        string sqlInsertComp = "INSERT INTO config_components (configId, componentId, quantity) VALUES (@cfgId, @compId, 1);";
                        using (var cmdComp = new MySqlCommand(sqlInsertComp, conn, tx))
                        {
                            cmdComp.Parameters.Add("@cfgId", MySqlDbType.Int32).Value = newId;
                            var pComp = cmdComp.Parameters.Add("@compId", MySqlDbType.Int32);

                            foreach (int compId in ids)
                            {
                                pComp.Value = compId;
                                cmdComp.ExecuteNonQuery();
                            }
                        }

                        tx.Commit();
                        return newId;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        public ConfigurationDto GetConfigurationDto(int configId)
        {
            var dto = new ConfigurationDto();

            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();

                // 1. Основная информация о конфигурации
                string sqlConfig = "SELECT * FROM configurations WHERE configId = @id";
                using (var cmd = new MySqlCommand(sqlConfig, conn))
                {
                    cmd.Parameters.AddWithValue("@id", configId);
                    using (var r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            dto.Configuration = new ConfigModel // Используем наш алиас
                            {
                                ConfigId = r.GetInt32("configId"),
                                ConfigName = r.GetString("configName"),
                                TotalPrice = r.GetDecimal("totalPrice"),
                                IsPreset = r.GetInt32("isPreset") == 1
                            };
                        }
                    }
                }

                // 2. Список компонентов (Используем верное имя колонки componentType)
                string sqlComponents = @"
            SELECT c.* FROM components c
            JOIN config_components cc ON c.componentId = cc.componentId
            WHERE cc.configId = @id";

                using (var cmd = new MySqlCommand(sqlComponents, conn))
                {
                    cmd.Parameters.AddWithValue("@id", configId);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            dto.Components.Add(new DDMLib.Component.Component
                            {
                                ComponentId = r.GetInt32("componentId"),
                                Name = r.GetString("name"),
                                Brand = r.IsDBNull(r.GetOrdinal("brand")) ? "" : r.GetString("brand"),
                                Model = r.IsDBNull(r.GetOrdinal("model")) ? "" : r.GetString("model"),
                                // ВОТ ТУТ ИСПРАВЛЕНИЕ: читаем из componentType
                                Type = r.GetString("componentType"),
                                Price = r.GetDecimal("price"),
                                StockQuantity = r.GetInt32("stockQuantity"),
                                IsAvailable = r.GetBoolean("isAvailable")
                            });
                        }
                    }
                }
            }

            return dto;
        }
    }
}