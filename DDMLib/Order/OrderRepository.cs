using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace DDMLib.Order
{
    public class OrderRepository : IOrderRepository
    {
        public List<Order> GetOrdersByUserEmail(string userEmail)
        {
            List<Order> orders = new List<Order>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Config.ConnectionString))
                {
                    connection.Open();

                    string sql = @"
                SELECT 
                    orderId, configId, userEmail, orderDate, status, 
                    totalPrice, deliveryAddress, deliveryMethod, paymentMethod, isPaid 
                FROM orders 
                WHERE userEmail = @UserEmail 
                ORDER BY orderDate DESC;";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UserEmail", userEmail);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Order order = new Order
                                {
                                    OrderId = reader.GetInt32(0),
                                    ConfigId = reader.GetInt32(1),
                                    UserEmail = reader.GetString(2),
                                    OrderDate = reader.GetDateTime(3),
                                    Status = (OrderStatus)Enum.Parse(typeof(OrderStatus), reader.GetString(4), ignoreCase: true),
                                    TotalPrice = reader.GetDecimal(5),
                                    DeliveryAddress = reader.IsDBNull(6) ? null : reader.GetString(6),
                                    DeliveryMethod = (DeliveryMethod)Enum.Parse(typeof(DeliveryMethod), reader.GetString(7), ignoreCase: true),
                                    PaymentMethod = (PaymentMethod)Enum.Parse(typeof(PaymentMethod), reader.GetString(8), ignoreCase: true),
                                    IsPaid = reader.GetBoolean(9)
                                };

                                orders.Add(order);
                            }
                        }
                    }
                }

                return orders;
            }
            catch
            {
                throw; // ошибка пробрасывается в ui
            }
        }

        public void AddOrder(Order order)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Config.ConnectionString))
                {
                    connection.Open();

                    string sql = @"
                INSERT INTO orders 
                    (configId, userEmail, orderDate, status, totalPrice, 
                     deliveryAddress, deliveryMethod, paymentMethod, isPaid)
                VALUES 
                    (@ConfigId, @UserEmail, @OrderDate, @Status, @TotalPrice, 
                     @DeliveryAddress, @DeliveryMethod, @PaymentMethod, @IsPaid);
                SELECT LAST_INSERT_ID();";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ConfigId", order.ConfigId);
                        command.Parameters.AddWithValue("@UserEmail", order.UserEmail);
                        command.Parameters.AddWithValue("@OrderDate", order.OrderDate);
                        command.Parameters.AddWithValue("@Status", order.Status.ToString());
                        command.Parameters.AddWithValue("@TotalPrice", order.TotalPrice);
                        command.Parameters.AddWithValue("@DeliveryAddress", order.DeliveryAddress ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@DeliveryMethod", order.DeliveryMethod.ToString());
                        command.Parameters.AddWithValue("@PaymentMethod", order.PaymentMethod.ToString());
                        command.Parameters.AddWithValue("@IsPaid", order.IsPaid);

                        order.OrderId = Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError("OrderRepository.AddOrder", ex.Message);
                throw;
            }
        }

        public bool ExistsById(int orderId)
        {
            using (var connection = new MySqlConnection(Config.ConnectionString))
            {
                connection.Open();
                const string sql = "SELECT COUNT(*) FROM orders WHERE orderId=@id;";

                using (var cmd = new MySqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@id", orderId);
                    return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        public bool UpdateOrderStatusAndPaid(int orderId, OrderStatus status, bool isPaid)
        {
            try
            {
                using (var connection = new MySqlConnection(Config.ConnectionString))
                {
                    connection.Open();

                    const string sql = @"
                        UPDATE orders
                        SET status=@status, isPaid=@paid
                        WHERE orderId=@id;";

                    using (var cmd = new MySqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", orderId);

                        // В БД у тебя обычно lowercase: pending/processing...
                        cmd.Parameters.AddWithValue("@status", status.ToString().ToLower());

                        cmd.Parameters.AddWithValue("@paid", isPaid);

                        return cmd.ExecuteNonQuery() == 1;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
