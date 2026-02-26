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
using WindowsFormsApp1.UserOrder;

namespace WindowsFormsApp1.Orders
{
    public partial class AllOrdersCardsForm : Form
    {
        private readonly OrderService service_;
        private List<Order> allOrders_ = new List<Order>();
        private class StatusItem
        {
            public string Text { get; set; }
            public OrderStatus? Value { get; set; }
            public override string ToString() => Text;
        }

        private enum DateSortMode { NewFirst, OldFirst }

        public AllOrdersCardsForm()
        {
            InitializeComponent();

            service_ = new OrderService(new OrderRepository());

            ThemeColor.ThemeChanged += ApplyTheme;

            cbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cbSortDate.DropDownStyle = ComboBoxStyle.DropDownList;

            cbStatus.SelectedIndexChanged += (s, e) => ApplyFiltersAndRender();
            cbSortDate.SelectedIndexChanged += (s, e) => ApplyFiltersAndRender();
            chkActiveOnly.CheckedChanged += (s, e) => ApplyFiltersAndRender();
            tbSearch.TextChanged += (s, e) => ApplyFiltersAndRender();
            
            btnReload.Click += (s, e) => LoadOrders();
            btnBack.Click += btnBack_Click;
            chkUnpaidOnly.CheckedChanged += (s, e) => ApplyFiltersAndRender();
            
            this.Shown += (s, e) => ApplyTheme();

            chkUnpaidOnly.Checked = true;
            chkActiveOnly.Checked = true;
        }

        private void AllOrdersCardsForm_Load(object sender, EventArgs e)
        {
            labelTitle.Text = "Все заказы";

            InitFilters();
            LoadOrders();
        }

        private void InitFilters()
        {
            cbStatus.Items.Clear();
            cbStatus.Items.Add(new StatusItem { Text = "Все", Value = null });
            cbStatus.Items.Add(new StatusItem { Text = OrderStatus.Pending.ToRussian(), Value = OrderStatus.Pending });
            cbStatus.Items.Add(new StatusItem { Text = OrderStatus.Processing.ToRussian(), Value = OrderStatus.Processing });
            cbStatus.Items.Add(new StatusItem { Text = OrderStatus.Assembled.ToRussian(), Value = OrderStatus.Assembled });
            cbStatus.Items.Add(new StatusItem { Text = OrderStatus.Shipped.ToRussian(), Value = OrderStatus.Shipped });
            cbStatus.Items.Add(new StatusItem { Text = OrderStatus.Delivered.ToRussian(), Value = OrderStatus.Delivered });
            cbStatus.Items.Add(new StatusItem { Text = OrderStatus.Cancelled.ToRussian(), Value = OrderStatus.Cancelled });
            cbStatus.SelectedIndex = 0;

            cbSortDate.Items.Clear();
            cbSortDate.Items.Add("Сначала новые");
            cbSortDate.Items.Add("Сначала старые");
            cbSortDate.SelectedIndex = 0;

            chkActiveOnly.Checked = false;
            tbSearch.Text = "";
        }

        private void LoadOrders()
        {
            try
            {
                allOrders_ = service_.GetAllOrders() ?? new List<Order>();
                ApplyFiltersAndRender();
            }
            catch (Exception ex)
            {
                flowOrders.Controls.Clear();
                labelCount.Text = "Найдено: 0";
                MessageBox.Show($"Вероятно, проблемы в соединении с БД.\n\n{ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyFiltersAndRender()
        {
            IEnumerable<Order> q = allOrders_ ?? Enumerable.Empty<Order>();

            // активные = всё, кроме Delivered и Cancelled (Shipped активный)
            if (chkActiveOnly.Checked)
            {
                q = q.Where(o => o.Status != OrderStatus.Delivered && o.Status != OrderStatus.Cancelled);
            }

            // только не оплаченные
            if (chkUnpaidOnly.Checked)
            {
                q = q.Where(o => !o.IsPaid);
            }

            // фильтр по статусу
            if (cbStatus.SelectedItem is StatusItem st && st.Value.HasValue)
            {
                q = q.Where(o => o.Status == st.Value.Value);
            }

            // поиск
            var s = (tbSearch.Text ?? "").Trim();
            if (!string.IsNullOrWhiteSpace(s))
            {
                q = q.Where(o =>
                    o.OrderId.ToString().Contains(s) ||
                    (o.UserEmail ?? "").IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (o.ConfigName ?? "").IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0
                );
            }

            // сортировка по дате
            var mode = (cbSortDate.SelectedIndex == 1) ? DateSortMode.OldFirst : DateSortMode.NewFirst;
            q = (mode == DateSortMode.NewFirst)
                ? q.OrderByDescending(o => o.OrderDate)
                : q.OrderBy(o => o.OrderDate);

            var list = q.ToList();
            RenderCards(list);

            labelCount.Text = $"Найдено: {list.Count}";
        }

        private void RenderCards(List<Order> orders)
        {
            flowOrders.SuspendLayout();
            flowOrders.Controls.Clear();

            foreach (var order in orders)
            {
                var card = new OrderCardControl(order)
                {
                    Width = 330,
                    Height = 170,
                    Margin = new Padding(8)
                };

                card.EditRequested += OnEditRequested;
                card.OpenConfigRequested += OnOpenConfigRequested;

                flowOrders.Controls.Add(card);
            }

            flowOrders.ResumeLayout();
        }

        private void OnEditRequested(Order order)
        {
            using (var f = new EditOrderStatusForm(service_, order))
            {
                if (f.ShowDialog(this) == DialogResult.OK)
                    LoadOrders();
            }
        }

        private void OnOpenConfigRequested(Order order)
        {
            using (var f = new ConfigDetailsForm(order.ConfigId, order.ConfigName))
                f.ShowDialog(this);
        }

        private void ApplyTheme()
        {
            // кнопки в стиле твоей темы
            foreach (Control c in panelTop.Controls)
            {
                if (c is Button btn)
                {
                    btn.BackColor = ThemeColor.PrimaryColor;
                    btn.ForeColor = Color.White;
                    btn.FlatAppearance.BorderColor = ThemeColor.SecondaryColor;
                }
            }

            labelTitle.ForeColor = ThemeColor.PrimaryColor;
            chkActiveOnly.Checked = true;
            flowOrders.BackColor = Color.FromArgb(248, 249, 250);
            panelTop.BackColor = Color.White;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            ResetFilters();
            ApplyFiltersAndRender();
        }

        private void ResetFilters()
        {
            // статус → "Все"
            cbStatus.SelectedIndex = 0;

            // сортировка → "Сначала новые"
            cbSortDate.SelectedIndex = 0;

            // только активные → выключить
            chkActiveOnly.Checked = false;

            // поиск → очистить
            tbSearch.Text = "";

            chkUnpaidOnly.Checked = false;
        }
    }
}
