using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace DDMLib.ConfigForAdmin
{
    public class MySqlComponentRepository : IComponentRepositoryAdmin
    {
        private static ComponentItem ReadItem(MySqlDataReader r)
        {
            return new ComponentItem
            {
                ComponentId = r.GetInt32("componentId"),
                Name = r.GetString("name"),
                Price = r.GetDecimal("price"),
                IsAvailable = r.GetInt32("isAvailable") == 1,
                StockQuantity = r.GetInt32("stockQuantity")
            };
        }

        public List<ComponentItem> ReadMotherboardsAvailable()
        {
            var list = new List<ComponentItem>();
            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();
                string sql = @"
                SELECT m.componentId, c.name, c.price, c.isAvailable, c.stockQuantity
                FROM motherboards m
                JOIN components c ON c.componentId = m.componentId
                WHERE c.isAvailable=1 AND c.stockQuantity>0
                ORDER BY c.price;";

                using (var cmd = new MySqlCommand(sql, conn))
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read()) list.Add(ReadItem(r));
                }
            }
            return list;
        }

        public List<ComponentItem> ReadCpusByMotherboard(int motherboardId)
        {
            var list = new List<ComponentItem>();
            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();
                string sql = @"
                SELECT cpu.componentId, c.name, c.price, c.isAvailable, c.stockQuantity
                FROM motherboards m
                JOIN cpus cpu ON cpu.socket = m.socket
                JOIN components c ON c.componentId = cpu.componentId
                WHERE m.componentId=@mbId AND c.isAvailable=1 AND c.stockQuantity>0
                ORDER BY c.price;";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@mbId", motherboardId);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read()) list.Add(ReadItem(r));
                    }
                }
            }
            return list;
        }

        public List<ComponentItem> ReadRamsByMotherboard(int motherboardId)
        {
            var list = new List<ComponentItem>();
            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();
                string sql = @"
                SELECT r.componentId, c.name, c.price, c.isAvailable, c.stockQuantity
                FROM motherboards m
                JOIN rams r ON r.ramType = m.ramType
                JOIN components c ON c.componentId = r.componentId
                WHERE m.componentId=@mbId AND c.isAvailable=1 AND c.stockQuantity>0
                ORDER BY c.price;";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@mbId", motherboardId);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read()) list.Add(ReadItem(r));
                    }
                }
            }
            return list;
        }

        public List<ComponentItem> ReadGpusByMotherboard(int motherboardId)
        {
            var list = new List<ComponentItem>();
            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();
                string sql = @"
                SELECT g.componentId, c.name, c.price, c.isAvailable, c.stockQuantity
                FROM motherboards m
                JOIN gpus g
                  ON (
                    (m.pcieVersion='3.0' AND g.pcieVersion='3.0')
                 OR (m.pcieVersion='4.0' AND g.pcieVersion IN ('3.0','4.0'))
                 OR (m.pcieVersion='5.0' AND g.pcieVersion IN ('3.0','4.0'))
                  )
                JOIN components c ON c.componentId = g.componentId
                WHERE m.componentId=@mbId AND c.isAvailable=1 AND c.stockQuantity>0
                ORDER BY c.price;";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@mbId", motherboardId);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read()) list.Add(ReadItem(r));
                    }
                }
            }
            return list;
        }

        public List<ComponentItem> ReadCasesByMotherboard(int motherboardId)
        {
            var list = new List<ComponentItem>();
            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();
                string sql = @"
                SELECT ca.componentId, c.name, c.price, c.isAvailable, c.stockQuantity
                FROM motherboards m
                JOIN cases ca ON ca.formFactor = m.formFactor
                JOIN components c ON c.componentId = ca.componentId
                WHERE m.componentId=@mbId AND c.isAvailable=1 AND c.stockQuantity>0
                ORDER BY c.price;";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@mbId", motherboardId);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read()) list.Add(ReadItem(r));
                    }
                }
            }
            return list;
        }

        public List<ComponentItem> ReadCoolingsByCpu(int cpuId)
        {
            var list = new List<ComponentItem>();
            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();
                string sql = @"
                SELECT co.componentId, c.name, c.price, c.isAvailable, c.stockQuantity
                FROM cpus cpu
                JOIN coolings co ON co.tdpSupport >= cpu.tdp
                JOIN components c ON c.componentId = co.componentId
                WHERE cpu.componentId=@cpuId AND c.isAvailable=1 AND c.stockQuantity>0
                ORDER BY co.tdpSupport, c.price;";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@cpuId", cpuId);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read()) list.Add(ReadItem(r));
                    }
                }
            }
            return list;
        }

        public List<ComponentItem> ReadStoragesAvailable()
        {
            var list = new List<ComponentItem>();
            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();
                string sql = @"
                SELECT s.componentId, c.name, c.price, c.isAvailable, c.stockQuantity
                FROM storages s
                JOIN components c ON c.componentId = s.componentId
                WHERE c.isAvailable=1 AND c.stockQuantity>0
                ORDER BY c.price;";

                using (var cmd = new MySqlCommand(sql, conn))
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read()) list.Add(ReadItem(r));
                }
            }
            return list;
        }

        public List<ComponentItem> ReadPsusByCpuGpu(int cpuId, int gpuId)
        {
            int cpuTdp = 0;
            int gpuTdp = 0;

            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();

                using (var cmd1 = new MySqlCommand("SELECT tdp FROM cpus WHERE componentId=@id;", conn))
                {
                    cmd1.Parameters.AddWithValue("@id", cpuId);
                    cpuTdp = Convert.ToInt32(cmd1.ExecuteScalar());
                }

                using (var cmd2 = new MySqlCommand("SELECT tdp FROM gpus WHERE componentId=@id;", conn))
                {
                    cmd2.Parameters.AddWithValue("@id", gpuId);
                    gpuTdp = Convert.ToInt32(cmd2.ExecuteScalar());
                }
            }

            // простая формула + запас
            int needWatt = (int)Math.Ceiling((cpuTdp + gpuTdp + 100) * 1.3);

            var list = new List<ComponentItem>();
            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();
                string sql = @"
                SELECT p.componentId, c.name, c.price, c.isAvailable, c.stockQuantity
                FROM psus p
                JOIN components c ON c.componentId = p.componentId
                WHERE p.wattage >= @needWatt
                  AND c.isAvailable=1 AND c.stockQuantity>0
                ORDER BY p.wattage, c.price;";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@needWatt", needWatt);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read()) list.Add(ReadItem(r));
                    }
                }
            }
            return list;
        }
    }
}
