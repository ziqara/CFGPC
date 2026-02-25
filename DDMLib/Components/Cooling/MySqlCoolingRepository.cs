using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using DDMLib;

public class MySqlCoolingRepository : ICoolingRepository
{
    public List<Cooling> ReadAllCoolings()
    {
        var list = new List<Cooling>();

        try
        {
            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();

                const string sql = @"
                    SELECT c.componentId, c.name, c.brand, c.model, c.price, c.stockQuantity, c.description,
                           c.isAvailable, c.photoUrl, c.supplierInn,
                           cl.coolerType, cl.tdpSupport, cl.fanRpm, cl.size, cl.isRgb
                    FROM components c
                    JOIN coolings cl ON cl.componentId = c.componentId
                    WHERE c.componentType = 'cooling'
                    ORDER BY c.componentId;";

                using (var cmd = new MySqlCommand(sql, conn))
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        var item = new Cooling
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

                            CoolerType = r.GetString(10),
                            TdpSupport = r.IsDBNull(11) ? (int?)null : r.GetInt32(11),
                            FanRpm = r.IsDBNull(12) ? (int?)null : r.GetInt32(12),
                            Size = r.IsDBNull(13) ? null : r.GetString(13),
                            IsRgb = !r.IsDBNull(14) && r.GetBoolean(14)
                        };

                        list.Add(item);
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

    public bool ExistsSameCooling(string name, string brand, string model)
    {
        try
        {
            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();

                const string sql = @"
                    SELECT COUNT(*)
                    FROM components
                    WHERE componentType='cooling'
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

    public bool ExistsOtherSameCooling(string name, string brand, string model, int currentComponentId)
    {
        try
        {
            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();

                const string sql = @"
                    SELECT COUNT(*)
                    FROM components
                    WHERE componentType='cooling'
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

    public bool AddCooling(Cooling c)
    {
        if (c == null) throw new ArgumentNullException(nameof(c));

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
                                (@name, @brand, @model, 'cooling', @price, @qty, @desc, @avail, @photo, @inn);";

                        long newId;
                        using (var cmd1 = new MySqlCommand(sql1, conn, tx))
                        {
                            cmd1.Parameters.AddWithValue("@name", c.Name?.Trim());
                            cmd1.Parameters.AddWithValue("@brand", string.IsNullOrWhiteSpace(c.Brand) ? (object)DBNull.Value : c.Brand.Trim());
                            cmd1.Parameters.AddWithValue("@model", string.IsNullOrWhiteSpace(c.Model) ? (object)DBNull.Value : c.Model.Trim());
                            cmd1.Parameters.AddWithValue("@price", c.Price);
                            cmd1.Parameters.AddWithValue("@qty", c.StockQuantity);
                            cmd1.Parameters.AddWithValue("@desc", string.IsNullOrWhiteSpace(c.Description) ? (object)DBNull.Value : c.Description.Trim());
                            cmd1.Parameters.AddWithValue("@avail", c.IsAvailable ? 1 : 0);
                            cmd1.Parameters.AddWithValue("@photo", string.IsNullOrWhiteSpace(c.PhotoUrl) ? (object)DBNull.Value : c.PhotoUrl.Trim());
                            cmd1.Parameters.AddWithValue("@inn", c.SupplierInn.HasValue ? (object)c.SupplierInn.Value : DBNull.Value);

                            int rows = cmd1.ExecuteNonQuery();
                            if (rows != 1) { tx.Rollback(); return false; }

                            newId = cmd1.LastInsertedId;
                        }

                        const string sql2 = @"
                            INSERT INTO coolings (componentId, coolerType, tdpSupport, fanRpm, size, isRgb)
                            VALUES (@id, @type, @tdp, @rpm, @size, @rgb);";

                        using (var cmd2 = new MySqlCommand(sql2, conn, tx))
                        {
                            cmd2.Parameters.AddWithValue("@id", newId);
                            cmd2.Parameters.AddWithValue("@type", c.CoolerType?.Trim()); // NOT NULL
                            cmd2.Parameters.AddWithValue("@tdp", c.TdpSupport.HasValue ? (object)c.TdpSupport.Value : DBNull.Value);
                            cmd2.Parameters.AddWithValue("@rpm", c.FanRpm.HasValue ? (object)c.FanRpm.Value : DBNull.Value);
                            cmd2.Parameters.AddWithValue("@size", string.IsNullOrWhiteSpace(c.Size) ? (object)DBNull.Value : c.Size.Trim());
                            cmd2.Parameters.AddWithValue("@rgb", c.IsRgb ? 1 : 0);

                            int rows2 = cmd2.ExecuteNonQuery();
                            if (rows2 != 1) { tx.Rollback(); return false; }
                        }

                        tx.Commit();
                        c.ComponentId = (int)newId;
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
        catch { throw; }
    }

    public bool UpdateCooling(Cooling c)
    {
        if (c == null) throw new ArgumentNullException(nameof(c));

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
                            WHERE componentId=@id AND componentType='cooling';";

                        using (var cmd1 = new MySqlCommand(sql1, conn, tx))
                        {
                            cmd1.Parameters.AddWithValue("@id", c.ComponentId);
                            cmd1.Parameters.AddWithValue("@name", c.Name?.Trim());
                            cmd1.Parameters.AddWithValue("@brand", string.IsNullOrWhiteSpace(c.Brand) ? (object)DBNull.Value : c.Brand.Trim());
                            cmd1.Parameters.AddWithValue("@model", string.IsNullOrWhiteSpace(c.Model) ? (object)DBNull.Value : c.Model.Trim());
                            cmd1.Parameters.AddWithValue("@price", c.Price);
                            cmd1.Parameters.AddWithValue("@qty", c.StockQuantity);
                            cmd1.Parameters.AddWithValue("@desc", string.IsNullOrWhiteSpace(c.Description) ? (object)DBNull.Value : c.Description.Trim());
                            cmd1.Parameters.AddWithValue("@avail", c.IsAvailable ? 1 : 0);
                            cmd1.Parameters.AddWithValue("@photo", string.IsNullOrWhiteSpace(c.PhotoUrl) ? (object)DBNull.Value : c.PhotoUrl.Trim());
                            cmd1.Parameters.AddWithValue("@inn", c.SupplierInn.HasValue ? (object)c.SupplierInn.Value : DBNull.Value);

                            int rows = cmd1.ExecuteNonQuery();
                            if (rows != 1) { tx.Rollback(); return false; }
                        }

                        const string sql2 = @"
                            UPDATE coolings
                            SET coolerType=@type,
                                tdpSupport=@tdp,
                                fanRpm=@rpm,
                                size=@size,
                                isRgb=@rgb
                            WHERE componentId=@id;";

                        using (var cmd2 = new MySqlCommand(sql2, conn, tx))
                        {
                            cmd2.Parameters.AddWithValue("@id", c.ComponentId);
                            cmd2.Parameters.AddWithValue("@type", c.CoolerType?.Trim());
                            cmd2.Parameters.AddWithValue("@tdp", c.TdpSupport.HasValue ? (object)c.TdpSupport.Value : DBNull.Value);
                            cmd2.Parameters.AddWithValue("@rpm", c.FanRpm.HasValue ? (object)c.FanRpm.Value : DBNull.Value);
                            cmd2.Parameters.AddWithValue("@size", string.IsNullOrWhiteSpace(c.Size) ? (object)DBNull.Value : c.Size.Trim());
                            cmd2.Parameters.AddWithValue("@rgb", c.IsRgb ? 1 : 0);

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

                const string sql = @"DELETE FROM components WHERE componentId=@id AND componentType='cooling';";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", componentId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
        catch { throw; }
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
        catch { throw; }
    }
}