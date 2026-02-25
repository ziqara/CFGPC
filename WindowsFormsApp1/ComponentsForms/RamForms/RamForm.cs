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

namespace WindowsFormsApp1.ComponentsForms.RamForms
{
    public partial class RamForm : Form
    {
        private readonly RamService service_;
        private List<Ram> allRams_ = new List<Ram>();

        public RamForm()
        {
            InitializeComponent();

            service_ = new RamService(new MySqlRamRepository());

            ramDataTable.CellFormatting += Grid_CellFormatting;

            this.Shown += RamForm_Shown;
            ThemeColor.ThemeChanged += ApplyTableTheme;
        }

        private void RamForm_Load(object sender, EventArgs e)
        {
            LoadRams();
        }

        private void txtSearchName_TextChanged(object sender, EventArgs e) => FilterAndSearch();

        private void cbxOnlyAvailable_CheckedChanged(object sender, EventArgs e) => FilterAndSearch();

        private void RamForm_Shown(object sender, EventArgs e)
        {
            ApplyTableTheme();
        }
        private void LoadRams()
        {
            try
            {
                var list = service_.GetAllRams();
                allRams_ = list ?? new List<Ram>();

                if (allRams_.Count == 0)
                {
                    ramDataTable.Visible = false;
                    MessageBox.Show("Оперативной памяти пока нет", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                ramDataTable.Visible = true;
                ShowRams(allRams_);
            }
            catch (Exception ex)
            {
                ramDataTable.Visible = false;
                MessageBox.Show($"Вероятно, проблемы в соединении с БД.\n\n{ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowRams(List<Ram> list)
        {
            ramDataTable.DataSource = null;
            ramDataTable.AutoGenerateColumns = true;
            ramDataTable.DataSource = list;
        }

        private void Grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var col = ramDataTable.Columns[e.ColumnIndex].DataPropertyName;

            if (col == "Brand" || col == "Model" || col == "Description" || col == "PhotoUrl" || col == "RamType")
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
        private void FilterAndSearch()
        {
            if (allRams_ == null) return;

            string searchText = txtSearchName.Text?.Trim() ?? "";
            bool onlyAvailable = cbxOnlyAvailable.Checked;

            IEnumerable<Ram> q = allRams_;

            if (!string.IsNullOrEmpty(searchText))
            {
                q = q.Where(r =>
                    (r.Name ?? "").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (r.Brand ?? "").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (r.Model ?? "").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (r.RamType ?? "").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (r.CapacityGb?.ToString() ?? "").Contains(searchText) ||
                    (r.SpeedMhz?.ToString() ?? "").Contains(searchText)
                );
            }

            if (onlyAvailable)
                q = q.Where(r => r.IsAvailable);

            ShowRams(q.ToList());
        }
        private void ApplyTableTheme()
        {
            if (ramDataTable == null || ramDataTable.IsDisposed)
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

            ramDataTable.BackgroundColor = Color.FromArgb(248, 249, 250);
            ramDataTable.GridColor = Color.FromArgb(206, 212, 218);
            ramDataTable.BorderStyle = BorderStyle.FixedSingle;
            ramDataTable.CellBorderStyle = DataGridViewCellBorderStyle.Single;

            ramDataTable.RowHeadersVisible = false;
            ramDataTable.AllowUserToAddRows = false;
            ramDataTable.AllowUserToDeleteRows = false;
            ramDataTable.ReadOnly = true;
            ramDataTable.MultiSelect = false;
            ramDataTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            ramDataTable.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            ramDataTable.EnableHeadersVisualStyles = false;

            ramDataTable.ColumnHeadersDefaultCellStyle.BackColor = ThemeColor.PrimaryColor;
            ramDataTable.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            ramDataTable.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            ramDataTable.ColumnHeadersHeight = 30;

            ramDataTable.DefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            ramDataTable.DefaultCellStyle.ForeColor = Color.FromArgb(52, 58, 64);
            ramDataTable.DefaultCellStyle.Font = new Font("Arial", 9);
            ramDataTable.DefaultCellStyle.SelectionBackColor = Color.FromArgb(206, 212, 218);
            ramDataTable.DefaultCellStyle.SelectionForeColor = Color.FromArgb(52, 58, 64);

            ramDataTable.AllowUserToResizeColumns = false;
            ramDataTable.AllowUserToResizeRows = false;

            ramDataTable.RowTemplate.Height = 25;
            foreach (DataGridViewRow row in ramDataTable.Rows)
                row.Height = ramDataTable.RowTemplate.Height;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new AddRamForm(service_))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    LoadRams();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (ramDataTable.CurrentRow?.DataBoundItem == null)
            {
                MessageBox.Show("Выберите оперативную память.", "Нет выбранной строки");
                return;
            }

            var selected = ramDataTable.CurrentRow.DataBoundItem as Ram;

            using (var form = new EditRamForm(service_, selected))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    LoadRams();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (ramDataTable.CurrentRow?.DataBoundItem == null)
            {
                MessageBox.Show("Сначала выберите оперативную память в списке.",
                    "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var ram = ramDataTable.CurrentRow.DataBoundItem as Ram;
            if (ram == null)
            {
                MessageBox.Show("Не удалось получить данные из таблицы.",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var confirm = MessageBox.Show(
                $"Удалить \"{ram.Name}\" (ID={ram.ComponentId})?\nДействие нельзя отменить.",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            try
            {
                string result = service_.DeleteRam(ram.ComponentId);

                if (string.IsNullOrEmpty(result))
                {
                    MessageBox.Show("Оперативная память удалена.", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadRams();
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


    }
}
