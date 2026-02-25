using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using DDMLib;

public class MySqlRamRepository : IRamRepository
{
    public List<Ram> ReadAllRams()
    {
        var list = new List<Ram>();

        try
        {
            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();

                const string sql = @"
                    SELECT c.componentId, c.name, c.brand, c.model, c.price, c.stockQuantity, c.description,
                           c.isAvailable, c.photoUrl, c.supplierInn,
                           r.ramType, r.capacityGb, r.speedMhz, r.slotsNeeded
                    FROM components c
                    JOIN rams r ON r.componentId = c.componentId
                    WHERE c.componentType = 'ram'
                    ORDER BY c.componentId;";

                using (var cmd = new MySqlCommand(sql, conn))
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        var ram = new Ram
                        {
                            ComponentId = rd.GetInt32(0),
                            Name = rd.GetString(1),
                            Brand = rd.IsDBNull(2) ? null : rd.GetString(2),
                            Model = rd.IsDBNull(3) ? null : rd.GetString(3),
                            Price = rd.GetDecimal(4),
                            StockQuantity = rd.GetInt32(5),
                            Description = rd.IsDBNull(6) ? null : rd.GetString(6),
                            IsAvailable = rd.GetBoolean(7),
                            PhotoUrl = rd.IsDBNull(8) ? null : rd.GetString(8),
                            SupplierInn = rd.IsDBNull(9) ? (int?)null : rd.GetInt32(9),

                            RamType = rd.IsDBNull(10) ? null : rd.GetString(10),
                            CapacityGb = rd.IsDBNull(11) ? (int?)null : rd.GetInt32(11),
                            SpeedMhz = rd.IsDBNull(12) ? (int?)null : rd.GetInt32(12),
                            SlotsNeeded = rd.IsDBNull(13) ? (int?)null : rd.GetInt32(13)
                        };

                        list.Add(ram);
                    }
                }
            }

            return list;
        }
        catch
        {
            throw;
        }
    }

    public bool ExistsSameRam(string name, string brand, string model)
    {
        try
        {
            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();

                const string sql = @"
                    SELECT COUNT(*)
                    FROM components
                    WHERE componentType='ram'
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
        catch { throw; }
    }

    public bool ExistsOtherSameRam(string name, string brand, string model, int currentComponentId)
    {
        try
        {
            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();

                const string sql = @"
                    SELECT COUNT(*)
                    FROM components
                    WHERE componentType='ram'
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
        catch { throw; }
    }

    public bool AddRam(Ram ram)
    {
        if (ram == null) throw new ArgumentNullException(nameof(ram));

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
                                (@name, @brand, @model, 'ram', @price, @qty, @desc, @avail, @photo, @inn);";

                        long newId;
                        using (var cmd1 = new MySqlCommand(sql1, conn, tx))
                        {
                            cmd1.Parameters.AddWithValue("@name", ram.Name?.Trim());
                            cmd1.Parameters.AddWithValue("@brand", string.IsNullOrWhiteSpace(ram.Brand) ? (object)DBNull.Value : ram.Brand.Trim());
                            cmd1.Parameters.AddWithValue("@model", string.IsNullOrWhiteSpace(ram.Model) ? (object)DBNull.Value : ram.Model.Trim());
                            cmd1.Parameters.AddWithValue("@price", ram.Price);
                            cmd1.Parameters.AddWithValue("@qty", ram.StockQuantity);
                            cmd1.Parameters.AddWithValue("@desc", string.IsNullOrWhiteSpace(ram.Description) ? (object)DBNull.Value : ram.Description.Trim());
                            cmd1.Parameters.AddWithValue("@avail", ram.IsAvailable ? 1 : 0);
                            cmd1.Parameters.AddWithValue("@photo", string.IsNullOrWhiteSpace(ram.PhotoUrl) ? (object)DBNull.Value : ram.PhotoUrl.Trim());
                            cmd1.Parameters.AddWithValue("@inn", ram.SupplierInn.HasValue ? (object)ram.SupplierInn.Value : DBNull.Value);

                            int rows = cmd1.ExecuteNonQuery();
                            if (rows != 1) { tx.Rollback(); return false; }

                            newId = cmd1.LastInsertedId;
                        }

                        const string sql2 = @"
                            INSERT INTO rams (componentId, ramType, capacityGb, speedMhz, slotsNeeded)
                            VALUES (@id, @type, @cap, @speed, @slots);";

                        using (var cmd2 = new MySqlCommand(sql2, conn, tx))
                        {
                            cmd2.Parameters.AddWithValue("@id", newId);
                            cmd2.Parameters.AddWithValue("@type", string.IsNullOrWhiteSpace(ram.RamType) ? (object)DBNull.Value : ram.RamType.Trim());
                            cmd2.Parameters.AddWithValue("@cap", ram.CapacityGb.HasValue ? (object)ram.CapacityGb.Value : DBNull.Value);
                            cmd2.Parameters.AddWithValue("@speed", ram.SpeedMhz.HasValue ? (object)ram.SpeedMhz.Value : DBNull.Value);
                            cmd2.Parameters.AddWithValue("@slots", ram.SlotsNeeded.HasValue ? (object)ram.SlotsNeeded.Value : DBNull.Value);

                            int rows2 = cmd2.ExecuteNonQuery();
                            if (rows2 != 1) { tx.Rollback(); return false; }
                        }

                        tx.Commit();
                        ram.ComponentId = (int)newId;
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

    public bool UpdateRam(Ram ram)
    {
        if (ram == null) throw new ArgumentNullException(nameof(ram));

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
                            WHERE componentId=@id AND componentType='ram';";

                        using (var cmd1 = new MySqlCommand(sql1, conn, tx))
                        {
                            cmd1.Parameters.AddWithValue("@id", ram.ComponentId);
                            cmd1.Parameters.AddWithValue("@name", ram.Name?.Trim());
                            cmd1.Parameters.AddWithValue("@brand", string.IsNullOrWhiteSpace(ram.Brand) ? (object)DBNull.Value : ram.Brand.Trim());
                            cmd1.Parameters.AddWithValue("@model", string.IsNullOrWhiteSpace(ram.Model) ? (object)DBNull.Value : ram.Model.Trim());
                            cmd1.Parameters.AddWithValue("@price", ram.Price);
                            cmd1.Parameters.AddWithValue("@qty", ram.StockQuantity);
                            cmd1.Parameters.AddWithValue("@desc", string.IsNullOrWhiteSpace(ram.Description) ? (object)DBNull.Value : ram.Description.Trim());
                            cmd1.Parameters.AddWithValue("@avail", ram.IsAvailable ? 1 : 0);
                            cmd1.Parameters.AddWithValue("@photo", string.IsNullOrWhiteSpace(ram.PhotoUrl) ? (object)DBNull.Value : ram.PhotoUrl.Trim());
                            cmd1.Parameters.AddWithValue("@inn", ram.SupplierInn.HasValue ? (object)ram.SupplierInn.Value : DBNull.Value);

                            int rows = cmd1.ExecuteNonQuery();
                            if (rows != 1) { tx.Rollback(); return false; }
                        }

                        const string sql2 = @"
                            UPDATE rams
                            SET ramType=@type,
                                capacityGb=@cap,
                                speedMhz=@speed,
                                slotsNeeded=@slots
                            WHERE componentId=@id;";

                        using (var cmd2 = new MySqlCommand(sql2, conn, tx))
                        {
                            cmd2.Parameters.AddWithValue("@id", ram.ComponentId);
                            cmd2.Parameters.AddWithValue("@type", string.IsNullOrWhiteSpace(ram.RamType) ? (object)DBNull.Value : ram.RamType.Trim());
                            cmd2.Parameters.AddWithValue("@cap", ram.CapacityGb.HasValue ? (object)ram.CapacityGb.Value : DBNull.Value);
                            cmd2.Parameters.AddWithValue("@speed", ram.SpeedMhz.HasValue ? (object)ram.SpeedMhz.Value : DBNull.Value);
                            cmd2.Parameters.AddWithValue("@slots", ram.SlotsNeeded.HasValue ? (object)ram.SlotsNeeded.Value : DBNull.Value);

                            int rows2 = cmd2.ExecuteNonQuery();
                            if (rows2 != 1) { tx.Rollback(); return false; }
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

                // rams удалится каскадом при FK ON DELETE CASCADE (если он в дампе есть)
                const string sql = @"DELETE FROM components WHERE componentId=@id AND componentType='ram';";

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