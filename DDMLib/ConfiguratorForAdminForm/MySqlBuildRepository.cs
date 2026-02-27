using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

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
                  COALESCE(o.salesCount, 0) AS salesCount,
                  COALESCE(b.badComponents, 0) AS badComponents
                FROM configurations cfg
                LEFT JOIN (
                   SELECT configId, COUNT(*) AS salesCount
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
                                SalesCount = r.GetInt32("salesCount"),
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
                    long count = (long)cmd.ExecuteScalar();
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

                    long bad = (long)cmd.ExecuteScalar();
                    badCount = (int)bad;
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

                    object scalar = cmd.ExecuteScalar();
                    return Convert.ToDecimal(scalar);
                }
            }
        }

        public void EnsureAdminExists(string adminEmail)
        {
            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();
                string sql = @"
                INSERT IGNORE INTO users (email, passwordHash, fullName, phone, address, role)
                VALUES (@email, 'admin', 'Administrator', NULL, NULL, 'admin');";

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

            int[] ids = new[]
            {
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
                        string sql1 = @"
                        INSERT INTO configurations
                          (configName, description, totalPrice, targetUse, status, isPreset, userEmail, rgb, otherOptions)
                        VALUES
                          (@name, @desc, @price, NULL, 'validated', 1, @email, 0, NULL);";

                        int newConfigId;
                        using (var cmd = new MySqlCommand(sql1, conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@name", draft.ConfigName);
                            cmd.Parameters.AddWithValue("@desc", string.IsNullOrWhiteSpace(draft.Description) ? (object)DBNull.Value : draft.Description);
                            cmd.Parameters.AddWithValue("@price", total);
                            cmd.Parameters.AddWithValue("@email", adminEmail);
                            cmd.ExecuteNonQuery();
                            newConfigId = (int)cmd.LastInsertedId;
                        }

                        string sql2 = @"
                        INSERT INTO config_components (configId, componentId, quantity)
                        VALUES (@cfgId, @compId, 1);";

                        using (var cmd2 = new MySqlCommand(sql2, conn, tx))
                        {
                            cmd2.Parameters.Add("@cfgId", MySqlDbType.Int32).Value = newConfigId;
                            var pComp = cmd2.Parameters.Add("@compId", MySqlDbType.Int32);

                            foreach (int compId in ids)
                            {
                                pComp.Value = compId;
                                cmd2.ExecuteNonQuery();
                            }
                        }

                        tx.Commit();
                        return newConfigId;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}