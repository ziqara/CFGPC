using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using DDMLib;

public class MySqlCpuRepository : ICpuRepository
{
    public List<Cpu> ReadAllCpus()
    {
        var cpus = new List<Cpu>();

        try
        {
            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();

                const string sql = @"
                    SELECT c.componentId, c.name, c.brand, c.model, c.price, c.stockQuantity, c.description,
                           c.isAvailable, c.photoUrl, c.supplierInn,
                           cpu.socket, cpu.cores, cpu.tdp
                    FROM components c
                    JOIN cpus cpu ON cpu.componentId = c.componentId
                    WHERE c.componentType = 'cpu'
                    ORDER BY c.componentId;";

                using (var cmd = new MySqlCommand(sql, conn))
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        var cpu = new Cpu
                        {
                            ComponentId = r.GetInt32(0),
                            Name = r.GetString(1),
                            Brand = r.IsDBNull(2) ? null : r.GetString(2),
                            Model = r.IsDBNull(3) ? null : r.GetString(3),
                            Price = r.GetDecimal(4),
                            StockQuantity = r.GetInt32(5),
                            Description = r.IsDBNull(6) ? null : r.GetString(6),
                            IsAvailable = r.GetBoolean(7),
                            PhotoUrl = r.IsDBNull(8) ? null : r.GetString(8),
                            SupplierInn = r.IsDBNull(9) ? (int?)null : r.GetInt32(9),

                            Socket = r.IsDBNull(10) ? null : r.GetString(10),
                            Cores = r.IsDBNull(11) ? (int?)null : r.GetInt32(11),
                            Tdp = r.IsDBNull(12) ? (int?)null : r.GetInt32(12),
                        };

                        cpus.Add(cpu);
                    }
                }
            }

            return cpus;
        }
        catch
        {
            throw;
        }
    }

    public bool ExistsSameCpu(string name, string brand, string model)
    {
        try
        {
            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();

                const string sql = @"
                    SELECT COUNT(*)
                    FROM components
                    WHERE componentType='cpu'
                      AND LOWER(name) = LOWER(@name)
                      AND ( (brand IS NULL AND @brand IS NULL) OR LOWER(brand) = LOWER(@brand) )
                      AND ( (model IS NULL AND @model IS NULL) OR LOWER(model) = LOWER(@model) );";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@name", (name ?? "").Trim());
                    cmd.Parameters.AddWithValue("@brand", string.IsNullOrWhiteSpace(brand) ? (object)DBNull.Value : brand.Trim());
                    cmd.Parameters.AddWithValue("@model", string.IsNullOrWhiteSpace(model) ? (object)DBNull.Value : model.Trim());

                    long count = Convert.ToInt64(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }
        catch
        {
            throw;
        }
    }

    public bool ExistsOtherSameCpu(string name, string brand, string model, int currentComponentId)
    {
        try
        {
            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();

                const string sql = @"
                    SELECT COUNT(*)
                    FROM components
                    WHERE componentType='cpu'
                      AND componentId <> @id
                      AND LOWER(name) = LOWER(@name)
                      AND ( (brand IS NULL AND @brand IS NULL) OR LOWER(brand) = LOWER(@brand) )
                      AND ( (model IS NULL AND @model IS NULL) OR LOWER(model) = LOWER(@model) );";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", currentComponentId);
                    cmd.Parameters.AddWithValue("@name", (name ?? "").Trim());
                    cmd.Parameters.AddWithValue("@brand", string.IsNullOrWhiteSpace(brand) ? (object)DBNull.Value : brand.Trim());
                    cmd.Parameters.AddWithValue("@model", string.IsNullOrWhiteSpace(model) ? (object)DBNull.Value : model.Trim());

                    long count = Convert.ToInt64(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }
        catch
        {
            throw;
        }
    }

    public bool AddCpu(Cpu cpu)
    {
        if (cpu == null) throw new ArgumentNullException(nameof(cpu));

        try
        {
            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();

                using (var tx = conn.BeginTransaction())
                {
                    try
                    {
                        const string sql1 = @"
                            INSERT INTO components
                                (name, brand, model, componentType, price, stockQuantity, description, isAvailable, photoUrl, supplierInn)
                            VALUES
                                (@name, @brand, @model, 'cpu', @price, @qty, @desc, @avail, @photo, @inn);";

                        long newId;
                        using (var cmd1 = new MySqlCommand(sql1, conn, tx))
                        {
                            cmd1.Parameters.AddWithValue("@name", cpu.Name?.Trim());
                            cmd1.Parameters.AddWithValue("@brand", string.IsNullOrWhiteSpace(cpu.Brand) ? (object)DBNull.Value : cpu.Brand.Trim());
                            cmd1.Parameters.AddWithValue("@model", string.IsNullOrWhiteSpace(cpu.Model) ? (object)DBNull.Value : cpu.Model.Trim());
                            cmd1.Parameters.AddWithValue("@price", cpu.Price);
                            cmd1.Parameters.AddWithValue("@qty", cpu.StockQuantity);
                            cmd1.Parameters.AddWithValue("@desc", string.IsNullOrWhiteSpace(cpu.Description) ? (object)DBNull.Value : cpu.Description.Trim());
                            cmd1.Parameters.AddWithValue("@avail", cpu.IsAvailable ? 1 : 0);
                            cmd1.Parameters.AddWithValue("@photo", string.IsNullOrWhiteSpace(cpu.PhotoUrl) ? (object)DBNull.Value : cpu.PhotoUrl.Trim());
                            cmd1.Parameters.AddWithValue("@inn", cpu.SupplierInn.HasValue ? (object)cpu.SupplierInn.Value : DBNull.Value);

                            int rows = cmd1.ExecuteNonQuery();
                            if (rows != 1)
                            {
                                tx.Rollback();
                                return false;
                            }

                            newId = cmd1.LastInsertedId;
                        }

                        const string sql2 = @"
                            INSERT INTO cpus (componentId, socket, cores, tdp)
                            VALUES (@id, @socket, @cores, @tdp);";

                        using (var cmd2 = new MySqlCommand(sql2, conn, tx))
                        {
                            cmd2.Parameters.AddWithValue("@id", newId);
                            cmd2.Parameters.AddWithValue("@socket", string.IsNullOrWhiteSpace(cpu.Socket) ? (object)DBNull.Value : cpu.Socket.Trim());
                            cmd2.Parameters.AddWithValue("@cores", cpu.Cores.HasValue ? (object)cpu.Cores.Value : DBNull.Value);
                            cmd2.Parameters.AddWithValue("@tdp", cpu.Tdp.HasValue ? (object)cpu.Tdp.Value : DBNull.Value);

                            int rows2 = cmd2.ExecuteNonQuery();
                            if (rows2 != 1)
                            {
                                tx.Rollback();
                                return false;
                            }
                        }

                        tx.Commit();
                        cpu.ComponentId = (int)newId;
                        return true;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }
        catch
        {
            throw;
        }
    }

    public bool UpdateCpu(Cpu cpu)
    {
        if (cpu == null) throw new ArgumentNullException(nameof(cpu));

        try
        {
            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();

                using (var tx = conn.BeginTransaction())
                {
                    try
                    {
                        const string sql1 = @"
                            UPDATE components
                            SET name=@name,
                                brand=@brand,
                                model=@model,
                                price=@price,
                                stockQuantity=@qty,
                                description=@desc,
                                isAvailable=@avail,
                                photoUrl=@photo,
                                supplierInn=@inn
                            WHERE componentId=@id
                              AND componentType='cpu';";

                        int rows1;
                        using (var cmd1 = new MySqlCommand(sql1, conn, tx))
                        {
                            cmd1.Parameters.AddWithValue("@id", cpu.ComponentId);
                            cmd1.Parameters.AddWithValue("@name", cpu.Name?.Trim());
                            cmd1.Parameters.AddWithValue("@brand", string.IsNullOrWhiteSpace(cpu.Brand) ? (object)DBNull.Value : cpu.Brand.Trim());
                            cmd1.Parameters.AddWithValue("@model", string.IsNullOrWhiteSpace(cpu.Model) ? (object)DBNull.Value : cpu.Model.Trim());
                            cmd1.Parameters.AddWithValue("@price", cpu.Price);
                            cmd1.Parameters.AddWithValue("@qty", cpu.StockQuantity);
                            cmd1.Parameters.AddWithValue("@desc", string.IsNullOrWhiteSpace(cpu.Description) ? (object)DBNull.Value : cpu.Description.Trim());
                            cmd1.Parameters.AddWithValue("@avail", cpu.IsAvailable ? 1 : 0);
                            cmd1.Parameters.AddWithValue("@photo", string.IsNullOrWhiteSpace(cpu.PhotoUrl) ? (object)DBNull.Value : cpu.PhotoUrl.Trim());
                            cmd1.Parameters.AddWithValue("@inn", cpu.SupplierInn.HasValue ? (object)cpu.SupplierInn.Value : DBNull.Value);

                            rows1 = cmd1.ExecuteNonQuery();
                            if (rows1 != 1)
                            {
                                tx.Rollback();
                                return false;
                            }
                        }

                        const string sql2 = @"
                            UPDATE cpus
                            SET socket=@socket,
                                cores=@cores,
                                tdp=@tdp
                            WHERE componentId=@id;";

                        using (var cmd2 = new MySqlCommand(sql2, conn, tx))
                        {
                            cmd2.Parameters.AddWithValue("@id", cpu.ComponentId);
                            cmd2.Parameters.AddWithValue("@socket", string.IsNullOrWhiteSpace(cpu.Socket) ? (object)DBNull.Value : cpu.Socket.Trim());
                            cmd2.Parameters.AddWithValue("@cores", cpu.Cores.HasValue ? (object)cpu.Cores.Value : DBNull.Value);
                            cmd2.Parameters.AddWithValue("@tdp", cpu.Tdp.HasValue ? (object)cpu.Tdp.Value : DBNull.Value);

                            int rows2 = cmd2.ExecuteNonQuery();
                            if (rows2 != 1)
                            {
                                tx.Rollback();
                                return false;
                            }
                        }

                        tx.Commit();
                        return true;
                    }
                    catch
                    {
                        tx.Rollback();
                        return false;
                    }
                }
            }
        }
        catch
        {
            return false;
        }
    }

    public bool DeleteById(int componentId)
    {
        try
        {
            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();

                // cpus удалится каскадом, если FK ON DELETE CASCADE
                const string sql = @"DELETE FROM components WHERE componentId=@id AND componentType='cpu';";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", componentId);
                    int affected = cmd.ExecuteNonQuery();
                    return affected > 0;
                }
            }
        }
        catch
        {
            throw;
        }
    }

    public bool HasActiveOrders(int componentId)
    {
        try
        {
            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();

                const string sql = @"
                    SELECT COUNT(*)
                    FROM orders o
                    JOIN configurations cfg   ON o.configId = cfg.configId
                    JOIN config_components cc ON cfg.configId = cc.configId
                    WHERE cc.componentId = @id
                      AND o.status NOT IN ('delivered', 'cancelled');";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", componentId);
                    long count = Convert.ToInt64(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }
        catch
        {
            throw;
        }
    }
}