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
    }
}
