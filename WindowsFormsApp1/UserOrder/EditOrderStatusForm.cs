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
    public partial class EditOrderStatusForm : Form
    {
        private readonly OrderService service_;
        private readonly DDMLib.Order.Order order_;

        public EditOrderStatusForm(OrderService service, DDMLib.Order.Order order)
        {
            InitializeComponent();

            service_ = service;
            order_ = order;

            cbStatus.DropDownStyle = ComboBoxStyle.DropDownList;

            cbStatus.DisplayMember = "Text";
            cbStatus.ValueMember = "Value";

            cbStatus.Items.Clear();
            cbStatus.Items.Add(new ComboItem("Ожидает обработки", OrderStatus.Pending));
            cbStatus.Items.Add(new ComboItem("В обработке", OrderStatus.Processing));
            cbStatus.Items.Add(new ComboItem("Собран", OrderStatus.Assembled));
            cbStatus.Items.Add(new ComboItem("Отправлен", OrderStatus.Shipped));
            cbStatus.Items.Add(new ComboItem("Доставлен", OrderStatus.Delivered));
            cbStatus.Items.Add(new ComboItem("Отменён", OrderStatus.Cancelled));

            this.Shown += EditOrderStatusForm_Shown;
            ThemeColor.ThemeChanged += ApplyTheme;
        }

        private void EditOrderStatusForm_Load(object sender, EventArgs e)
        {
            lblOrder.Text = $"Заказ #{order_.OrderId}";
            chkPaid.Checked = order_.IsPaid;

            // ВЫБОР текущего статуса
            for (int i = 0; i < cbStatus.Items.Count; i++)
            {
                var item = cbStatus.Items[i] as ComboItem;
                if (item != null && item.Value == order_.Status)
                {
                    cbStatus.SelectedIndex = i;
                    break;
                }
            }

            // На всякий случай, если почему-то не нашли:
            if (cbStatus.SelectedIndex < 0 && cbStatus.Items.Count > 0)
                cbStatus.SelectedIndex = 0;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cbStatus.SelectedItem == null)
            {
                MessageBox.Show("Выберите статус.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var newStatus = ((ComboItem)cbStatus.SelectedItem).Value;
            bool paid = chkPaid.Checked;

            string result = service_.AdminUpdateOrderStatusAndPaid(order_.OrderId, newStatus, paid);
            if (string.IsNullOrEmpty(result))
            {
                order_.Status = newStatus;
                order_.IsPaid = paid;

                DialogResult = DialogResult.OK;
                Close();
                return;
            }

            MessageBox.Show(result, "Ошибка сохранения",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void EditOrderStatusForm_Shown(object sender, EventArgs e)
        {
            ApplyTheme();
        }

        private void ApplyTheme()
        {
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

            if (lblOrder != null)
                lblOrder.ForeColor = ThemeColor.PrimaryColor;
        }

        private class ComboItem
        {
            public string Text { get; set; }
            public OrderStatus Value { get; set; }

            public ComboItem(string text, OrderStatus value)
            {
                Text = text;
                Value = value;
            }

            public override string ToString() => Text;
        }
    }
}
