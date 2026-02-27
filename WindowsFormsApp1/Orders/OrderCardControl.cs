using DDMLib.Order;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1.Orders
{
    public partial class OrderCardControl : UserControl
    {
        public Order Order { get; private set; }

        public event Action<Order> EditRequested;
        public event Action<Order> OpenConfigRequested;

        public OrderCardControl()
        {
            InitializeComponent();
            WireEvents();
        }

        private void ApplyTheme()
        {
            // Применяем стиль для кнопок
            btnEdit.BackColor = ThemeColor.PrimaryColor;
            btnEdit.ForeColor = Color.White;
            btnEdit.FlatStyle = FlatStyle.Flat;
            btnEdit.FlatAppearance.BorderColor = ThemeColor.SecondaryColor;
            btnEdit.FlatAppearance.BorderSize = 1;

            // Кнопка "Открыть конфиг"
            lnkConfig.LinkColor = ThemeColor.PrimaryColor;

            // Стиль для текста
            lblOrderId.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
            lblDate.Font = new Font("Segoe UI", 9f);
            lblStatus.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            lblPrice.Font = new Font("Segoe UI", 10f, FontStyle.Bold);

            // Выделение статуса с помощью цветов
            if (Order.Status == OrderStatus.Cancelled)
                lblStatus.ForeColor = Color.Firebrick;
            else if (Order.Status == OrderStatus.Delivered)
                lblStatus.ForeColor = Color.SeaGreen;
            else
                lblStatus.ForeColor = Color.FromArgb(52, 58, 64);
        }

        public OrderCardControl(Order order) : this()
        {
            Bind(order);
        }

        private void WireEvents()
        {
            btnEdit.Click += (s, e) =>
            {
                if (Order != null) EditRequested?.Invoke(Order);
            };

            lnkConfig.LinkClicked += (s, e) =>
            {
                if (Order != null) OpenConfigRequested?.Invoke(Order);
            };
        }

        public void Bind(Order order)
        {
            Order = order ?? throw new ArgumentNullException(nameof(order));

            lblOrderId.Text = $"Заказ №{Order.OrderId}";
            lblDate.Text = Order.OrderDate.ToString("dd.MM.yyyy HH:mm");
            lblEmail.Text = Order.UserEmail;

            lnkConfig.Text = string.IsNullOrWhiteSpace(Order.ConfigName) ? "(без названия)" : Order.ConfigName;

            lblStatus.Text = Order.Status.ToRussian();
            lblPrice.Text = $"{Order.TotalPrice:0.00} ₽";

            lblDelivery.Text = $"{Order.DeliveryMethod.ToRussian()}";
            lblAddress.Text = (Order.DeliveryMethod == DeliveryMethod.Courier)
                ? (string.IsNullOrWhiteSpace(Order.DeliveryAddress) ? "Адрес не указан" : Order.DeliveryAddress)
                : "—";

            lblPayment.Text = $"{Order.PaymentMethod.ToRussian()}";
            lblPaid.Text = Order.IsPaid ? "Оплачен" : "Не оплачен";

            // лёгкая визуальная подсветка статуса (без излишней магии)
            if (Order.Status == OrderStatus.Cancelled)
                lblStatus.ForeColor = Color.Firebrick;
            else if (Order.Status == OrderStatus.Delivered)
                lblStatus.ForeColor = Color.SeaGreen;
            else
                lblStatus.ForeColor = Color.FromArgb(52, 58, 64);

            ApplyTheme();
        }
    }
}
