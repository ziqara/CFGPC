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
using WindowsFormsApp1.ComponentsForms.GpuForms;

namespace WindowsFormsApp1.ComponentsForms.PsuForms
{
    public partial class PsuForm : Form
    {
        private readonly PsuService service_;
        private List<Psu> allPsus_ = new List<Psu>();

        public PsuForm()
        {
            InitializeComponent();

            service_ = new PsuService(new MySqlPsuRepository());

            psuDataTable.CellFormatting += Grid_CellFormatting;

            this.Shown += PsuForms_Shown;
            ThemeColor.ThemeChanged += ApplyTableTheme;
        }

        private void btnAddCpu_Click(object sender, EventArgs e)
        {
            using (var form = new AddPsuForm(service_))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    LoadPsus();
            }
        }

        private void btnEditCpu_Click(object sender, EventArgs e)
        {
            if (psuDataTable.CurrentRow?.DataBoundItem == null)
            {
                MessageBox.Show("Выберите блок питания.", "Нет выбранной строки");
                return;
            }

            var selected = psuDataTable.CurrentRow.DataBoundItem as Psu;

            using (var form = new EditPsuForm(service_, selected))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    LoadPsus();
            }
        }

        private void btnDeleteCpu_Click(object sender, EventArgs e)
        {

            if (psuDataTable.CurrentRow?.DataBoundItem == null)
            {
                MessageBox.Show("Сначала выберите блок питания в списке.",
                    "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var psu = psuDataTable.CurrentRow.DataBoundItem as Psu;
            if (psu == null)
            {
                MessageBox.Show("Не удалось получить данные из таблицы.",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var confirm = MessageBox.Show(
                $"Удалить \"{psu.Name}\" (ID={psu.ComponentId})?\nДействие нельзя отменить.",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            try
            {
                string result = service_.DeletePsu(psu.ComponentId);

                if (string.IsNullOrEmpty(result))
                {
                    MessageBox.Show("Блок питания удалён.", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadPsus();
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

        private void txtSearchName_TextChanged(object sender, EventArgs e)
        {
            FilterAndSearch();
        }

        private void cbxOnlyAvailable_CheckedChanged(object sender, EventArgs e)
        {
            FilterAndSearch();
        }

        private void FilterAndSearch()
        {
            if (allPsus_ == null) return;

            string searchText = txtSearchName.Text?.Trim() ?? "";
            bool onlyAvailable = cbxOnlyAvailable.Checked;

            IEnumerable<Psu> q = allPsus_;

            if (!string.IsNullOrEmpty(searchText))
            {
                q = q.Where(p =>
                    (p.Name ?? "").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (p.Brand ?? "").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (p.Model ?? "").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (p.Efficiency ?? "").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (p.Wattage?.ToString() ?? "").Contains(searchText)
                );
            }

            if (onlyAvailable)
                q = q.Where(p => p.IsAvailable);

            ShowPsus(q.ToList());
        }

        private void PsuForms_Shown(object sender, EventArgs e)
        {
            ApplyTableTheme();
        }

        private void ApplyTableTheme()
        {
            if (psuDataTable == null || psuDataTable.IsDisposed)
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

            psuDataTable.BackgroundColor = Color.FromArgb(248, 249, 250);
            psuDataTable.GridColor = Color.FromArgb(206, 212, 218);
            psuDataTable.BorderStyle = BorderStyle.FixedSingle;
            psuDataTable.CellBorderStyle = DataGridViewCellBorderStyle.Single;

            psuDataTable.RowHeadersVisible = false;
            psuDataTable.AllowUserToAddRows = false;
            psuDataTable.AllowUserToDeleteRows = false;
            psuDataTable.ReadOnly = true;
            psuDataTable.MultiSelect = false;
            psuDataTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            psuDataTable.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            psuDataTable.EnableHeadersVisualStyles = false;

            psuDataTable.ColumnHeadersDefaultCellStyle.BackColor = ThemeColor.PrimaryColor;
            psuDataTable.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            psuDataTable.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            psuDataTable.ColumnHeadersHeight = 30;

            psuDataTable.DefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            psuDataTable.DefaultCellStyle.ForeColor = Color.FromArgb(52, 58, 64);
            psuDataTable.DefaultCellStyle.Font = new Font("Arial", 9);
            psuDataTable.DefaultCellStyle.SelectionBackColor = Color.FromArgb(206, 212, 218);
            psuDataTable.DefaultCellStyle.SelectionForeColor = Color.FromArgb(52, 58, 64);

            psuDataTable.AllowUserToResizeColumns = false;
            psuDataTable.AllowUserToResizeRows = false;

            psuDataTable.RowTemplate.Height = 25;
            foreach (DataGridViewRow row in psuDataTable.Rows)
                row.Height = psuDataTable.RowTemplate.Height;
        }

        private void PsuForms_Load(object sender, EventArgs e)
        {
            LoadPsus();
        }

        private void LoadPsus()
        {
            try
            {
                var list = service_.GetAllPsus();
                allPsus_ = list ?? new List<Psu>();

                if (allPsus_.Count == 0)
                {
                    psuDataTable.Visible = false;
                    MessageBox.Show("Блоков питания пока нет", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                psuDataTable.Visible = true;
                ShowPsus(allPsus_);
            }
            catch (Exception ex)
            {
                psuDataTable.Visible = false;
                MessageBox.Show($"Вероятно, проблемы в соединении с БД.\n\n{ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowPsus(List<Psu> list)
        {
            psuDataTable.DataSource = null;
            psuDataTable.AutoGenerateColumns = true;
            psuDataTable.DataSource = list;
        }

        private void Grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var col = psuDataTable.Columns[e.ColumnIndex].DataPropertyName;

            if (col == "Brand" || col == "Model" || col == "Description" || col == "PhotoUrl" || col == "Efficiency")
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
    }
}
