using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using DDMLib;

public class MySqlMotherboardRepository : IMotherboardRepository
{
    public List<Motherboard> ReadAllMotherboards()
    {
        var list = new List<Motherboard>();

        try
        {
            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();

                const string sql = @"
                    SELECT c.componentId, c.name, c.brand, c.model, c.price, c.stockQuantity, c.description,
                           c.isAvailable, c.photoUrl, c.supplierInn,
                           m.socket, m.chipset, m.ramType, m.pcieVersion, m.formFactor
                    FROM components c
                    JOIN motherboards m ON m.componentId = c.componentId
                    WHERE c.componentType = 'motherboard'
                    ORDER BY c.componentId;";

                using (var cmd = new MySqlCommand(sql, conn))
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        var mb = new Motherboard
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
                            Chipset = r.IsDBNull(11) ? null : r.GetString(11),
                            RamType = r.IsDBNull(12) ? null : r.GetString(12),
                            PcieVersion = r.IsDBNull(13) ? null : r.GetString(13),
                            FormFactor = r.IsDBNull(14) ? null : r.GetString(14),
                        };

                        list.Add(mb);
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

    public bool ExistsSameMotherboard(string name, string brand, string model)
    {
        try
        {
            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();

                const string sql = @"
                    SELECT COUNT(*)
                    FROM components
                    WHERE componentType='motherboard'
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

    public bool ExistsOtherSameMotherboard(string name, string brand, string model, int currentComponentId)
    {
        try
        {
            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();

                const string sql = @"
                    SELECT COUNT(*)
                    FROM components
                    WHERE componentType='motherboard'
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

    public bool AddMotherboard(Motherboard mb)
    {
        if (mb == null) throw new ArgumentNullException(nameof(mb));

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
                                (@name, @brand, @model, 'motherboard', @price, @qty, @desc, @avail, @photo, @inn);";

                        long newId;
                        using (var cmd1 = new MySqlCommand(sql1, conn, tx))
                        {
                            cmd1.Parameters.AddWithValue("@name", mb.Name?.Trim());
                            cmd1.Parameters.AddWithValue("@brand", string.IsNullOrWhiteSpace(mb.Brand) ? (object)DBNull.Value : mb.Brand.Trim());
                            cmd1.Parameters.AddWithValue("@model", string.IsNullOrWhiteSpace(mb.Model) ? (object)DBNull.Value : mb.Model.Trim());
                            cmd1.Parameters.AddWithValue("@price", mb.Price);
                            cmd1.Parameters.AddWithValue("@qty", mb.StockQuantity);
                            cmd1.Parameters.AddWithValue("@desc", string.IsNullOrWhiteSpace(mb.Description) ? (object)DBNull.Value : mb.Description.Trim());
                            cmd1.Parameters.AddWithValue("@avail", mb.IsAvailable ? 1 : 0);
                            cmd1.Parameters.AddWithValue("@photo", string.IsNullOrWhiteSpace(mb.PhotoUrl) ? (object)DBNull.Value : mb.PhotoUrl.Trim());
                            cmd1.Parameters.AddWithValue("@inn", mb.SupplierInn.HasValue ? (object)mb.SupplierInn.Value : DBNull.Value);

                            int rows = cmd1.ExecuteNonQuery();
                            if (rows != 1) { tx.Rollback(); return false; }

                            newId = cmd1.LastInsertedId;
                        }

                        const string sql2 = @"
                            INSERT INTO motherboards (componentId, socket, chipset, ramType, pcieVersion, formFactor)
                            VALUES (@id, @socket, @chipset, @ramType, @pcie, @ff);";

                        using (var cmd2 = new MySqlCommand(sql2, conn, tx))
                        {
                            cmd2.Parameters.AddWithValue("@id", newId);
                            cmd2.Parameters.AddWithValue("@socket", string.IsNullOrWhiteSpace(mb.Socket) ? (object)DBNull.Value : mb.Socket.Trim());
                            cmd2.Parameters.AddWithValue("@chipset", string.IsNullOrWhiteSpace(mb.Chipset) ? (object)DBNull.Value : mb.Chipset.Trim());
                            cmd2.Parameters.AddWithValue("@ramType", string.IsNullOrWhiteSpace(mb.RamType) ? (object)DBNull.Value : mb.RamType.Trim());
                            cmd2.Parameters.AddWithValue("@pcie", string.IsNullOrWhiteSpace(mb.PcieVersion) ? (object)DBNull.Value : mb.PcieVersion.Trim());
                            cmd2.Parameters.AddWithValue("@ff", string.IsNullOrWhiteSpace(mb.FormFactor) ? (object)DBNull.Value : mb.FormFactor.Trim());

                            int rows2 = cmd2.ExecuteNonQuery();
                            if (rows2 != 1) { tx.Rollback(); return false; }
                        }

                        tx.Commit();
                        mb.ComponentId = (int)newId;
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

    public bool UpdateMotherboard(Motherboard mb)
    {
        if (mb == null) throw new ArgumentNullException(nameof(mb));

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
                            WHERE componentId=@id AND componentType='motherboard';";

                        using (var cmd1 = new MySqlCommand(sql1, conn, tx))
                        {
                            cmd1.Parameters.AddWithValue("@id", mb.ComponentId);
                            cmd1.Parameters.AddWithValue("@name", mb.Name?.Trim());
                            cmd1.Parameters.AddWithValue("@brand", string.IsNullOrWhiteSpace(mb.Brand) ? (object)DBNull.Value : mb.Brand.Trim());
                            cmd1.Parameters.AddWithValue("@model", string.IsNullOrWhiteSpace(mb.Model) ? (object)DBNull.Value : mb.Model.Trim());
                            cmd1.Parameters.AddWithValue("@price", mb.Price);
                            cmd1.Parameters.AddWithValue("@qty", mb.StockQuantity);
                            cmd1.Parameters.AddWithValue("@desc", string.IsNullOrWhiteSpace(mb.Description) ? (object)DBNull.Value : mb.Description.Trim());
                            cmd1.Parameters.AddWithValue("@avail", mb.IsAvailable ? 1 : 0);
                            cmd1.Parameters.AddWithValue("@photo", string.IsNullOrWhiteSpace(mb.PhotoUrl) ? (object)DBNull.Value : mb.PhotoUrl.Trim());
                            cmd1.Parameters.AddWithValue("@inn", mb.SupplierInn.HasValue ? (object)mb.SupplierInn.Value : DBNull.Value);

                            int rows = cmd1.ExecuteNonQuery();
                            if (rows != 1) { tx.Rollback(); return false; }
                        }

                        const string sql2 = @"
                            UPDATE motherboards
                            SET socket=@socket,
                                chipset=@chipset,
                                ramType=@ramType,
                                pcieVersion=@pcie,
                                formFactor=@ff
                            WHERE componentId=@id;";

                        using (var cmd2 = new MySqlCommand(sql2, conn, tx))
                        {
                            cmd2.Parameters.AddWithValue("@id", mb.ComponentId);
                            cmd2.Parameters.AddWithValue("@socket", string.IsNullOrWhiteSpace(mb.Socket) ? (object)DBNull.Value : mb.Socket.Trim());
                            cmd2.Parameters.AddWithValue("@chipset", string.IsNullOrWhiteSpace(mb.Chipset) ? (object)DBNull.Value : mb.Chipset.Trim());
                            cmd2.Parameters.AddWithValue("@ramType", string.IsNullOrWhiteSpace(mb.RamType) ? (object)DBNull.Value : mb.RamType.Trim());
                            cmd2.Parameters.AddWithValue("@pcie", string.IsNullOrWhiteSpace(mb.PcieVersion) ? (object)DBNull.Value : mb.PcieVersion.Trim());
                            cmd2.Parameters.AddWithValue("@ff", string.IsNullOrWhiteSpace(mb.FormFactor) ? (object)DBNull.Value : mb.FormFactor.Trim());

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
                const string sql = @"DELETE FROM components WHERE componentId=@id AND componentType='motherboard';";
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