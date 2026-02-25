using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using DDMLib;

public class MySqlStorageRepository : IStorageRepository
{
    public List<Storage> ReadAllStorages()
    {
        var list = new List<Storage>();

        try
        {
            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();

                const string sql = @"
                    SELECT c.componentId, c.name, c.brand, c.model, c.price, c.stockQuantity, c.description,
                           c.isAvailable, c.photoUrl, c.supplierInn,
                           s.interface, s.capacityGb
                    FROM components c
                    JOIN storages s ON s.componentId = c.componentId
                    WHERE c.componentType = 'storage'
                    ORDER BY c.componentId;";

                using (var cmd = new MySqlCommand(sql, conn))
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        var st = new Storage
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

                            Interface = r.IsDBNull(10) ? null : r.GetString(10),
                            CapacityGb = r.IsDBNull(11) ? (int?)null : r.GetInt32(11),
                        };

                        list.Add(st);
                    }
                }
            }

            return list;
        }
        catch { throw; }
    }

    public bool ExistsSameStorage(string name, string brand, string model)
    {
        try
        {
            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();

                const string sql = @"
                    SELECT COUNT(*)
                    FROM components
                    WHERE componentType='storage'
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

    public bool ExistsOtherSameStorage(string name, string brand, string model, int currentComponentId)
    {
        try
        {
            using (var conn = new MySqlConnection(Config.ConnectionString))
            {
                conn.Open();

                const string sql = @"
                    SELECT COUNT(*)
                    FROM components
                    WHERE componentType='storage'
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

    public bool AddStorage(Storage st)
    {
        if (st == null) throw new ArgumentNullException(nameof(st));

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
                                (@name, @brand, @model, 'storage', @price, @qty, @desc, @avail, @photo, @inn);";

                        long newId;
                        using (var cmd1 = new MySqlCommand(sql1, conn, tx))
                        {
                            cmd1.Parameters.AddWithValue("@name", st.Name?.Trim());
                            cmd1.Parameters.AddWithValue("@brand", string.IsNullOrWhiteSpace(st.Brand) ? (object)DBNull.Value : st.Brand.Trim());
                            cmd1.Parameters.AddWithValue("@model", string.IsNullOrWhiteSpace(st.Model) ? (object)DBNull.Value : st.Model.Trim());
                            cmd1.Parameters.AddWithValue("@price", st.Price);
                            cmd1.Parameters.AddWithValue("@qty", st.StockQuantity);
                            cmd1.Parameters.AddWithValue("@desc", string.IsNullOrWhiteSpace(st.Description) ? (object)DBNull.Value : st.Description.Trim());
                            cmd1.Parameters.AddWithValue("@avail", st.IsAvailable ? 1 : 0);
                            cmd1.Parameters.AddWithValue("@photo", string.IsNullOrWhiteSpace(st.PhotoUrl) ? (object)DBNull.Value : st.PhotoUrl.Trim());
                            cmd1.Parameters.AddWithValue("@inn", st.SupplierInn.HasValue ? (object)st.SupplierInn.Value : DBNull.Value);

                            int rows = cmd1.ExecuteNonQuery();
                            if (rows != 1) { tx.Rollback(); return false; }

                            newId = cmd1.LastInsertedId;
                        }

                        const string sql2 = @"
                            INSERT INTO storages (componentId, interface, capacityGb)
                            VALUES (@id, @iface, @cap);";

                        using (var cmd2 = new MySqlCommand(sql2, conn, tx))
                        {
                            cmd2.Parameters.AddWithValue("@id", newId);
                            cmd2.Parameters.AddWithValue("@iface", string.IsNullOrWhiteSpace(st.Interface) ? (object)DBNull.Value : st.Interface.Trim());
                            cmd2.Parameters.AddWithValue("@cap", st.CapacityGb.HasValue ? (object)st.CapacityGb.Value : DBNull.Value);

                            int rows2 = cmd2.ExecuteNonQuery();
                            if (rows2 != 1) { tx.Rollback(); return false; }
                        }

                        tx.Commit();
                        st.ComponentId = (int)newId;
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

    public bool UpdateStorage(Storage st)
    {
        if (st == null) throw new ArgumentNullException(nameof(st));

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
                            WHERE componentId=@id AND componentType='storage';";

                        using (var cmd1 = new MySqlCommand(sql1, conn, tx))
                        {
                            cmd1.Parameters.AddWithValue("@id", st.ComponentId);
                            cmd1.Parameters.AddWithValue("@name", st.Name?.Trim());
                            cmd1.Parameters.AddWithValue("@brand", string.IsNullOrWhiteSpace(st.Brand) ? (object)DBNull.Value : st.Brand.Trim());
                            cmd1.Parameters.AddWithValue("@model", string.IsNullOrWhiteSpace(st.Model) ? (object)DBNull.Value : st.Model.Trim());
                            cmd1.Parameters.AddWithValue("@price", st.Price);
                            cmd1.Parameters.AddWithValue("@qty", st.StockQuantity);
                            cmd1.Parameters.AddWithValue("@desc", string.IsNullOrWhiteSpace(st.Description) ? (object)DBNull.Value : st.Description.Trim());
                            cmd1.Parameters.AddWithValue("@avail", st.IsAvailable ? 1 : 0);
                            cmd1.Parameters.AddWithValue("@photo", string.IsNullOrWhiteSpace(st.PhotoUrl) ? (object)DBNull.Value : st.PhotoUrl.Trim());
                            cmd1.Parameters.AddWithValue("@inn", st.SupplierInn.HasValue ? (object)st.SupplierInn.Value : DBNull.Value);

                            int rows = cmd1.ExecuteNonQuery();
                            if (rows != 1) { tx.Rollback(); return false; }
                        }

                        const string sql2 = @"
                            UPDATE storages
                            SET interface=@iface,
                                capacityGb=@cap
                            WHERE componentId=@id;";

                        using (var cmd2 = new MySqlCommand(sql2, conn, tx))
                        {
                            cmd2.Parameters.AddWithValue("@id", st.ComponentId);
                            cmd2.Parameters.AddWithValue("@iface", string.IsNullOrWhiteSpace(st.Interface) ? (object)DBNull.Value : st.Interface.Trim());
                            cmd2.Parameters.AddWithValue("@cap", st.CapacityGb.HasValue ? (object)st.CapacityGb.Value : DBNull.Value);

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

                // storages удалится каскадом (FK ON DELETE CASCADE)
                const string sql = @"DELETE FROM components WHERE componentId=@id AND componentType='storage';";

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