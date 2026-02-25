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
            ordersDataTable.CellFormatting += OrdersDataTable_CellFormatting;
            this.Shown += UserOrdersForm_Shown;
            ThemeColor.ThemeChanged += ApplyTableTheme;

            ordersDataTable.CellFormatting += OrdersDataTable_CellFormatting;
            ordersDataTable.CellMouseEnter += ordersDataTable_CellMouseEnter;
            ordersDataTable.CellMouseLeave += ordersDataTable_CellMouseLeave;

            ordersDataTable.CellToolTipTextNeeded += ordersDataTable_CellToolTipTextNeeded;
        }

        private void OrdersDataTable_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var columnName = ordersDataTable.Columns[e.ColumnIndex].DataPropertyName;

            if (columnName == "Status" && e.Value is DDMLib.Order.OrderStatus status)
            {
                e.Value = status.ToRussian();
                e.FormattingApplied = true;
            }

            if (columnName == "PaymentMethod" && e.Value is DDMLib.Order.PaymentMethod pay)
            {
                e.Value = pay.ToRussian();
                e.FormattingApplied = true;
            }

            if (columnName == "DeliveryMethod" && e.Value is DDMLib.Order.DeliveryMethod del)
            {
                e.Value = del.ToRussian();
                e.FormattingApplied = true;
            }

            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var col = ordersDataTable.Columns[e.ColumnIndex];

            // "Конфигурация" как ссылка
            if (col.DataPropertyName == "ConfigName")
            {
                ordersDataTable.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.Blue;
                ordersDataTable.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.Font =
                    new Font(ordersDataTable.Font, FontStyle.Underline);
            }
        }

        private void UserOrdersForm_Load(object sender, EventArgs e)
        {
            labelTitle.Text = $"Заказы пользователя: {userEmail_}";
            LoadOrders();
            if (ordersDataTable.Columns["ConfigId"] != null)
                ordersDataTable.Columns["ConfigId"].Visible = false;
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

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void ordersDataTable_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var col = ordersDataTable.Columns[e.ColumnIndex];
            if (col.DataPropertyName == "ConfigName")
                ordersDataTable.Cursor = Cursors.Hand;
        }

        private void ordersDataTable_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            ordersDataTable.Cursor = Cursors.Default;
        }

        private void ordersDataTable_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var col = ordersDataTable.Columns[e.ColumnIndex];
            if (col.DataPropertyName == "ConfigName")
                e.ToolTipText = "Двойной клик — открыть состав ПК";
        }

        private void ordersDataTable_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var col = ordersDataTable.Columns[e.ColumnIndex];
            if (col.DataPropertyName != "ConfigName") return;

            var order = ordersDataTable.Rows[e.RowIndex].DataBoundItem as DDMLib.Order.Order;
            if (order == null) return;

            using (var f = new ConfigDetailsForm(order.ConfigId, order.ConfigName))
                f.ShowDialog(this);
        }
    }
}
