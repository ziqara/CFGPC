using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using DDMLib;

public class MySqlPsuRepository : IPsuRepository
{
    public List<Psu> ReadAllPsus()
    {
        var list = new List<Psu>();

        try
        {
            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();

                const string sql = @"
                    SELECT c.componentId, c.name, c.brand, c.model, c.price, c.stockQuantity, c.description,
                           c.isAvailable, c.photoUrl, c.supplierInn,
                           p.wattage, p.efficiencyRating
                    FROM components c
                    JOIN psus p ON p.componentId = c.componentId
                    WHERE c.componentType = 'psu'
                    ORDER BY c.componentId;";

                using (var cmd = new MySqlCommand(sql, conn))
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        var psu = new Psu
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

                            Wattage = r.IsDBNull(10) ? (int?)null : r.GetInt32(10),
                            Efficiency = r.IsDBNull(11) ? null : r.GetString(11), // efficiencyRating
                        };

                        list.Add(psu);
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

    public bool ExistsSamePsu(string name, string brand, string model)
    {
        try
        {
            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();

                const string sql = @"
                    SELECT COUNT(*)
                    FROM components
                    WHERE componentType='psu'
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

    public bool ExistsOtherSamePsu(string name, string brand, string model, int currentComponentId)
    {
        try
        {
            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();

                const string sql = @"
                    SELECT COUNT(*)
                    FROM components
                    WHERE componentType='psu'
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

    public bool AddPsu(Psu p)
    {
        if (p == null) throw new ArgumentNullException(nameof(p));

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
                                (@name, @brand, @model, 'psu', @price, @qty, @desc, @avail, @photo, @inn);";

                        long newId;
                        using (var cmd1 = new MySqlCommand(sql1, conn, tx))
                        {
                            cmd1.Parameters.AddWithValue("@name", p.Name?.Trim());
                            cmd1.Parameters.AddWithValue("@brand", string.IsNullOrWhiteSpace(p.Brand) ? (object)DBNull.Value : p.Brand.Trim());
                            cmd1.Parameters.AddWithValue("@model", string.IsNullOrWhiteSpace(p.Model) ? (object)DBNull.Value : p.Model.Trim());
                            cmd1.Parameters.AddWithValue("@price", p.Price);
                            cmd1.Parameters.AddWithValue("@qty", p.StockQuantity);
                            cmd1.Parameters.AddWithValue("@desc", string.IsNullOrWhiteSpace(p.Description) ? (object)DBNull.Value : p.Description.Trim());
                            cmd1.Parameters.AddWithValue("@avail", p.IsAvailable ? 1 : 0);
                            cmd1.Parameters.AddWithValue("@photo", string.IsNullOrWhiteSpace(p.PhotoUrl) ? (object)DBNull.Value : p.PhotoUrl.Trim());
                            cmd1.Parameters.AddWithValue("@inn", p.SupplierInn.HasValue ? (object)p.SupplierInn.Value : DBNull.Value);

                            int rows = cmd1.ExecuteNonQuery();
                            if (rows != 1) { tx.Rollback(); return false; }

                            newId = cmd1.LastInsertedId;
                        }

                        const string sql2 = @"
                            INSERT INTO psus (componentId, wattage, efficiencyRating)
                            VALUES (@id, @w, @eff);";

                        using (var cmd2 = new MySqlCommand(sql2, conn, tx))
                        {
                            cmd2.Parameters.AddWithValue("@id", newId);
                            cmd2.Parameters.AddWithValue("@w", p.Wattage.HasValue ? (object)p.Wattage.Value : DBNull.Value);
                            cmd2.Parameters.AddWithValue("@eff", string.IsNullOrWhiteSpace(p.Efficiency) ? (object)DBNull.Value : p.Efficiency.Trim());

                            int rows2 = cmd2.ExecuteNonQuery();
                            if (rows2 != 1) { tx.Rollback(); return false; }
                        }

                        tx.Commit();
                        p.ComponentId = (int)newId;
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

    public bool UpdatePsu(Psu p)
    {
        if (p == null) throw new ArgumentNullException(nameof(p));

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
                            WHERE componentId=@id AND componentType='psu';";

                        using (var cmd1 = new MySqlCommand(sql1, conn, tx))
                        {
                            cmd1.Parameters.AddWithValue("@id", p.ComponentId);
                            cmd1.Parameters.AddWithValue("@name", p.Name?.Trim());
                            cmd1.Parameters.AddWithValue("@brand", string.IsNullOrWhiteSpace(p.Brand) ? (object)DBNull.Value : p.Brand.Trim());
                            cmd1.Parameters.AddWithValue("@model", string.IsNullOrWhiteSpace(p.Model) ? (object)DBNull.Value : p.Model.Trim());
                            cmd1.Parameters.AddWithValue("@price", p.Price);
                            cmd1.Parameters.AddWithValue("@qty", p.StockQuantity);
                            cmd1.Parameters.AddWithValue("@desc", string.IsNullOrWhiteSpace(p.Description) ? (object)DBNull.Value : p.Description.Trim());
                            cmd1.Parameters.AddWithValue("@avail", p.IsAvailable ? 1 : 0);
                            cmd1.Parameters.AddWithValue("@photo", string.IsNullOrWhiteSpace(p.PhotoUrl) ? (object)DBNull.Value : p.PhotoUrl.Trim());
                            cmd1.Parameters.AddWithValue("@inn", p.SupplierInn.HasValue ? (object)p.SupplierInn.Value : DBNull.Value);

                            int rows = cmd1.ExecuteNonQuery();
                            if (rows != 1) { tx.Rollback(); return false; }
                        }

                        const string sql2 = @"
                            UPDATE psus
                            SET wattage=@w,
                                efficiencyRating=@eff
                            WHERE componentId=@id;";

                        using (var cmd2 = new MySqlCommand(sql2, conn, tx))
                        {
                            cmd2.Parameters.AddWithValue("@id", p.ComponentId);
                            cmd2.Parameters.AddWithValue("@w", p.Wattage.HasValue ? (object)p.Wattage.Value : DBNull.Value);
                            cmd2.Parameters.AddWithValue("@eff", string.IsNullOrWhiteSpace(p.Efficiency) ? (object)DBNull.Value : p.Efficiency.Trim());

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

                const string sql = @"DELETE FROM components WHERE componentId=@id AND componentType='psu';";
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