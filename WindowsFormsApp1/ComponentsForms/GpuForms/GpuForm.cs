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

namespace WindowsFormsApp1.ComponentsForms.GpuForms
{
    public partial class GpuForm : Form
    {
        private readonly GpuService service_;
        private List<Gpu> allGpus_ = new List<Gpu>();

        public GpuForm()
        {
            InitializeComponent();

            service_ = new GpuService(new MySqlGpuRepository());

            gpuDataTable.CellFormatting += Grid_CellFormatting;

            this.Shown += GpuForm_Shown;
            ThemeColor.ThemeChanged += ApplyTableTheme;
        }

        private void GpuForm_Load(object sender, EventArgs e)
        {
            LoadGpus();
        }

        private void LoadGpus()
        {
            try
            {
                var list = service_.GetAllGpus();
                allGpus_ = list ?? new List<Gpu>();

                if (allGpus_.Count == 0)
                {
                    gpuDataTable.Visible = false;
                    MessageBox.Show("Видеокарт пока нет", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                gpuDataTable.Visible = true;
                ShowGpus(allGpus_);
            }
            catch (Exception ex)
            {
                gpuDataTable.Visible = false;
                MessageBox.Show($"Вероятно, проблемы в соединении с БД.\n\n{ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowGpus(List<Gpu> list)
        {
            gpuDataTable.DataSource = null;
            gpuDataTable.AutoGenerateColumns = true;
            gpuDataTable.DataSource = list;
        }

        private void Grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var col = gpuDataTable.Columns[e.ColumnIndex].DataPropertyName;

            if (col == "Brand" || col == "Model" || col == "Description" || col == "PhotoUrl" || col == "PcieVersion")
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

        private void GpuForm_Shown(object sender, EventArgs e)
        {
            ApplyTableTheme();
        }

        private void ApplyTableTheme()
        {
            if (gpuDataTable == null || gpuDataTable.IsDisposed)
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

            gpuDataTable.BackgroundColor = Color.FromArgb(248, 249, 250);
            gpuDataTable.GridColor = Color.FromArgb(206, 212, 218);
            gpuDataTable.BorderStyle = BorderStyle.FixedSingle;
            gpuDataTable.CellBorderStyle = DataGridViewCellBorderStyle.Single;

            gpuDataTable.RowHeadersVisible = false;
            gpuDataTable.AllowUserToAddRows = false;
            gpuDataTable.AllowUserToDeleteRows = false;
            gpuDataTable.ReadOnly = true;
            gpuDataTable.MultiSelect = false;
            gpuDataTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gpuDataTable.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gpuDataTable.EnableHeadersVisualStyles = false;

            gpuDataTable.ColumnHeadersDefaultCellStyle.BackColor = ThemeColor.PrimaryColor;
            gpuDataTable.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            gpuDataTable.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            gpuDataTable.ColumnHeadersHeight = 30;

            gpuDataTable.DefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            gpuDataTable.DefaultCellStyle.ForeColor = Color.FromArgb(52, 58, 64);
            gpuDataTable.DefaultCellStyle.Font = new Font("Arial", 9);
            gpuDataTable.DefaultCellStyle.SelectionBackColor = Color.FromArgb(206, 212, 218);
            gpuDataTable.DefaultCellStyle.SelectionForeColor = Color.FromArgb(52, 58, 64);

            gpuDataTable.AllowUserToResizeColumns = false;
            gpuDataTable.AllowUserToResizeRows = false;

            gpuDataTable.RowTemplate.Height = 25;
            foreach (DataGridViewRow row in gpuDataTable.Rows)
                row.Height = gpuDataTable.RowTemplate.Height;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new AddGpuForm(service_))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    LoadGpus();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (gpuDataTable.CurrentRow?.DataBoundItem == null)
            {
                MessageBox.Show("Выберите видеокарту.", "Нет выбранной строки");
                return;
            }

            var selected = gpuDataTable.CurrentRow.DataBoundItem as Gpu;

            using (var form = new EditGpuForm(service_, selected))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    LoadGpus();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (gpuDataTable.CurrentRow?.DataBoundItem == null)
            {
                MessageBox.Show("Сначала выберите видеокарту в списке.",
                    "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var gpu = gpuDataTable.CurrentRow.DataBoundItem as Gpu;
            if (gpu == null)
            {
                MessageBox.Show("Не удалось получить данные из таблицы.",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var confirm = MessageBox.Show(
                $"Удалить \"{gpu.Name}\" (ID={gpu.ComponentId})?\nДействие нельзя отменить.",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            try
            {
                string result = service_.DeleteGpu(gpu.ComponentId);

                if (string.IsNullOrEmpty(result))
                {
                    MessageBox.Show("Видеокарта удалена.", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadGpus();
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

        private void txtSearchName_TextChanged(object sender, EventArgs e) => FilterAndSearch();
        private void cbxOnlyAvailable_CheckedChanged(object sender, EventArgs e) => FilterAndSearch();

        private void FilterAndSearch()
        {
            if (allGpus_ == null) return;

            string searchText = txtSearchName.Text?.Trim() ?? "";
            bool onlyAvailable = cbxOnlyAvailable.Checked;

            IEnumerable<Gpu> q = allGpus_;

            if (!string.IsNullOrEmpty(searchText))
            {
                q = q.Where(g =>
                    (g.Name ?? "").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (g.Brand ?? "").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (g.Model ?? "").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (g.PcieVersion ?? "").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (g.VramGb?.ToString() ?? "").Contains(searchText) ||
                    (g.Tdp?.ToString() ?? "").Contains(searchText)
                );
            }

            if (onlyAvailable)
                q = q.Where(g => g.IsAvailable);

            ShowGpus(q.ToList());
        }
    }
}
