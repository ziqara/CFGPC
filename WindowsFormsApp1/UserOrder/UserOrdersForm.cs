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

namespace WindowsFormsApp1.UserOrder
{
    public partial class UserOrdersForm : Form
    {
        private readonly OrderService service_;
        private readonly string userEmail_;
        private List<DDMLib.Order.Order> allOrders_ = new List<DDMLib.Order.Order>();

        public UserOrdersForm(string email)
        {
            InitializeComponent();

            userEmail_ = email;
            service_ = new OrderService(new OrderRepository());

            this.Shown += UserOrdersForm_Shown;
            ThemeColor.ThemeChanged += ApplyTableTheme;
        }

        private void UserOrdersForm_Load(object sender, EventArgs e)
        {
            labelTitle.Text = $"Заказы пользователя: {userEmail_}";
            LoadOrders();
        }

        private void btnEditStatus_Click(object sender, EventArgs e)
        {
            if (ordersDataTable.CurrentRow == null)
            {
                MessageBox.Show("Выберите заказ.", "Нет выбранной строки");
                return;
            }

            var selected = ordersDataTable.CurrentRow.DataBoundItem as DDMLib.Order.Order;
            if (selected == null) return;

            using (var f = new EditOrderStatusForm(service_, selected))
            {
                if (f.ShowDialog(this) == DialogResult.OK)
                    LoadOrders();
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            LoadOrders();
        }
        private void LoadOrders()
        {
            try
            {
                allOrders_ = service_.GetOrdersByUserEmail(userEmail_) ?? new List<DDMLib.Order.Order>();

                if (allOrders_.Count == 0)
                {
                    ordersDataTable.Visible = false;
                    MessageBox.Show("У пользователя пока нет заказов", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                ordersDataTable.Visible = true;
                ordersDataTable.DataSource = null;
                ordersDataTable.AutoGenerateColumns = true;
                ordersDataTable.DataSource = allOrders_;
            }
            catch (Exception ex)
            {
                ordersDataTable.Visible = false;
                MessageBox.Show($"Вероятно, проблемы в соединении с БД.\n\n{ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UserOrdersForm_Shown(object sender, EventArgs e)
        {
            ApplyTableTheme();
        }

        private void ApplyTableTheme()
        {
            if (ordersDataTable == null || ordersDataTable.IsDisposed)
                return;

            foreach (Control btns in this.Controls)
            {
                if (btns.GetType() == typeof(Button))
                {
                    Button btn = (Button)btns;
                    btn.BackColor = ThemeColor.PrimaryColor;
                    btn.ForeColor = Color.White;
                    btn.FlatAppearance.BorderColor = ThemeColor.SecondaryColor;
                }
            }

            labelTitle.ForeColor = ThemeColor.PrimaryColor;

            ordersDataTable.BackgroundColor = Color.FromArgb(248, 249, 250);
            ordersDataTable.GridColor = Color.FromArgb(206, 212, 218);
            ordersDataTable.BorderStyle = BorderStyle.FixedSingle;
            ordersDataTable.CellBorderStyle = DataGridViewCellBorderStyle.Single;

            ordersDataTable.RowHeadersVisible = false;
            ordersDataTable.AllowUserToAddRows = false;
            ordersDataTable.AllowUserToDeleteRows = false;
            ordersDataTable.ReadOnly = true;
            ordersDataTable.MultiSelect = false;
            ordersDataTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            ordersDataTable.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            ordersDataTable.EnableHeadersVisualStyles = false;

            ordersDataTable.ColumnHeadersDefaultCellStyle.BackColor = ThemeColor.PrimaryColor;
            ordersDataTable.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            ordersDataTable.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            ordersDataTable.ColumnHeadersDefaultCellStyle.SelectionBackColor = ThemeColor.PrimaryColor;
            ordersDataTable.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
            ordersDataTable.ColumnHeadersHeight = 30;

            ordersDataTable.DefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            ordersDataTable.DefaultCellStyle.ForeColor = Color.FromArgb(52, 58, 64);
            ordersDataTable.DefaultCellStyle.Font = new Font("Arial", 9);

            ordersDataTable.DefaultCellStyle.SelectionBackColor = Color.FromArgb(206, 212, 218);
            ordersDataTable.DefaultCellStyle.SelectionForeColor = Color.FromArgb(52, 58, 64);

            ordersDataTable.AllowUserToResizeColumns = false;
            ordersDataTable.AllowUserToResizeRows = false;

            ordersDataTable.RowTemplate.Height = 25;
            foreach (DataGridViewRow row in ordersDataTable.Rows)
                row.Height = ordersDataTable.RowTemplate.Height;
        }
    }
}
