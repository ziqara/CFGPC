using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DDMLib;

namespace WindowsFormsApp1.ComponentsForms.CoolingForms
{
    public partial class CoolingForm : Form
    {
        private readonly CoolingService service_;
        private List<Cooling> allCoolings_ = new List<Cooling>();

        public CoolingForm()
        {
            InitializeComponent();

            service_ = new CoolingService(new MySqlCoolingRepository());

            coolingDataTable.CellFormatting += Grid_CellFormatting;

            this.Shown += CoolingForm_Shown;
            ThemeColor.ThemeChanged += ApplyTableTheme;
        }

        private void CoolingForm_Load(object sender, EventArgs e)
        {
            LoadCoolings();
        }

        private void LoadCoolings()
        {
            try
            {
                var list = service_.GetAllCoolings();
                allCoolings_ = list ?? new List<Cooling>();

                if (allCoolings_.Count == 0)
                {
                    coolingDataTable.Visible = false;
                    MessageBox.Show("Охлаждений пока нет", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                coolingDataTable.Visible = true;
                ShowCoolings(allCoolings_);
            }
            catch (Exception ex)
            {
                coolingDataTable.Visible = false;
                MessageBox.Show($"Вероятно, проблемы в соединении с БД.\n\n{ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowCoolings(List<Cooling> list)
        {
            coolingDataTable.DataSource = null;
            coolingDataTable.AutoGenerateColumns = true;
            coolingDataTable.DataSource = list;
        }

        private void Grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var col = coolingDataTable.Columns[e.ColumnIndex].DataPropertyName;

            if (col == "Brand" || col == "Model" || col == "Description" || col == "PhotoUrl" || col == "Size")
            {
                if (e.Value == null || string.IsNullOrWhiteSpace(e.Value.ToString()))
                {
                    e.Value = "—";
                    e.FormattingApplied = true;
                }
            }

            if (col == "SupplierInn")
            {
                if (e.Value == null || string.IsNullOrWhiteSpace(e.Value.ToString()))
                {
                    e.Value = "—";
                    e.FormattingApplied = true;
                }
            }
        }

        private void txtSearchName_TextChanged(object sender, EventArgs e)
        {
            FilterAndSearch();
        }

        private void cbxOnlyAvailable_CheckedChanged(object sender, EventArgs e)
        {
            FilterAndSearch();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new AddCoolingForm(service_))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    LoadCoolings();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (coolingDataTable.CurrentRow?.DataBoundItem == null)
            {
                MessageBox.Show("Выберите охлаждение.", "Нет выбранной строки");
                return;
            }

            var selected = coolingDataTable.CurrentRow.DataBoundItem as Cooling;

            using (var form = new EditCoolingForm(service_, selected))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    LoadCoolings();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (coolingDataTable.CurrentRow?.DataBoundItem == null)
            {
                MessageBox.Show("Сначала выберите охлаждение в списке.",
                    "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var item = coolingDataTable.CurrentRow.DataBoundItem as Cooling;
            if (item == null)
            {
                MessageBox.Show("Не удалось получить данные из таблицы.",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var confirm = MessageBox.Show(
                $"Удалить \"{item.Name}\" (ID={item.ComponentId})?\nДействие нельзя отменить.",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            try
            {
                string result = service_.DeleteCooling(item.ComponentId);

                if (string.IsNullOrEmpty(result))
                {
                    MessageBox.Show("Охлаждение удалено.", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCoolings();
                }
                else
                {
                    MessageBox.Show(result, "Ошибка удаления",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Вероятно, проблемы в соединении с БД: " + ex.Message,
                    "Критическая ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CoolingForm_Shown(object sender, EventArgs e)
        {
            ApplyTableTheme();
        }

        private void ApplyTableTheme()
        {
            if (coolingDataTable == null || coolingDataTable.IsDisposed)
                return;

            foreach (Control c in this.Controls)
            {
                if (c is Button btn)
                {
                    btn.BackColor = ThemeColor.PrimaryColor;
                    btn.ForeColor = Color.White;
                    btn.FlatAppearance.BorderColor = ThemeColor.SecondaryColor;
                }
            }

            label1.ForeColor = ThemeColor.PrimaryColor;

            coolingDataTable.BackgroundColor = Color.FromArgb(248, 249, 250);
            coolingDataTable.GridColor = Color.FromArgb(206, 212, 218);
            coolingDataTable.BorderStyle = BorderStyle.FixedSingle;
            coolingDataTable.CellBorderStyle = DataGridViewCellBorderStyle.Single;

            coolingDataTable.RowHeadersVisible = false;
            coolingDataTable.AllowUserToAddRows = false;
            coolingDataTable.AllowUserToDeleteRows = false;
            coolingDataTable.ReadOnly = true;
            coolingDataTable.MultiSelect = false;
            coolingDataTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            coolingDataTable.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            coolingDataTable.EnableHeadersVisualStyles = false;

            coolingDataTable.ColumnHeadersDefaultCellStyle.BackColor = ThemeColor.PrimaryColor;
            coolingDataTable.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            coolingDataTable.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            coolingDataTable.ColumnHeadersHeight = 30;

            coolingDataTable.DefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            coolingDataTable.DefaultCellStyle.ForeColor = Color.FromArgb(52, 58, 64);
            coolingDataTable.DefaultCellStyle.Font = new Font("Arial", 9);
            coolingDataTable.DefaultCellStyle.SelectionBackColor = Color.FromArgb(206, 212, 218);
            coolingDataTable.DefaultCellStyle.SelectionForeColor = Color.FromArgb(52, 58, 64);

            coolingDataTable.AllowUserToResizeColumns = false;
            coolingDataTable.AllowUserToResizeRows = false;

            coolingDataTable.RowTemplate.Height = 25;
            foreach (DataGridViewRow row in coolingDataTable.Rows)
                row.Height = coolingDataTable.RowTemplate.Height;
        }
        private void FilterAndSearch()
        {
            if (allCoolings_ == null) return;

            string searchText = txtSearchName.Text?.Trim() ?? "";
            bool onlyAvailable = cbxOnlyAvailable.Checked;

            IEnumerable<Cooling> q = allCoolings_;

            if (!string.IsNullOrEmpty(searchText))
            {
                q = q.Where(c =>
                    (c.Name ?? "").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (c.Brand ?? "").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (c.Model ?? "").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (c.CoolerType ?? "").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (c.Size ?? "").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (c.TdpSupport?.ToString() ?? "").Contains(searchText) ||
                    (c.FanRpm?.ToString() ?? "").Contains(searchText) ||
                    (c.IsRgb ? "rgb" : "").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0
                );
            }

            if (onlyAvailable)
                q = q.Where(c => c.IsAvailable);

            ShowCoolings(q.ToList());
        }
    }
}
