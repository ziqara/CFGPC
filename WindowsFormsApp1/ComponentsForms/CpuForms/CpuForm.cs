using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DDMLib;

namespace WindowsFormsApp1.ComponentsForms
{

    public partial class CpuForm : Form
    {
        private readonly CpuService service_;
        private List<Cpu> allCpus_ = new List<Cpu>();

        public CpuForm()
        {
            InitializeComponent();

            service_ = new CpuService(new MySqlCpuRepository());

            cpuDataTable.CellFormatting += CpuGridView_CellFormatting;

            this.Shown += CpuForm_Shown;
            ThemeColor.ThemeChanged += ApplyTableTheme;
        }

        private void CpuForm_Load(object sender, EventArgs e)
        {
            LoadCpus();
        }

        private void LoadCpus()
        {
            try
            {
                List<Cpu> cpus = service_.GetAllCpus();
                allCpus_ = cpus ?? new List<Cpu>();

                if (allCpus_.Count == 0)
                {
                    cpuDataTable.Visible = false;
                    MessageBox.Show("Процессоров пока нет", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                cpuDataTable.Visible = true;
                ShowCpus(allCpus_);
            }
            catch (Exception ex)
            {
                cpuDataTable.Visible = false;
                MessageBox.Show($"Вероятно, проблемы в соединении с БД.\n\n{ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowCpus(List<Cpu> cpus)
        {
            cpuDataTable.DataSource = null;
            cpuDataTable.AutoGenerateColumns = true;
            cpuDataTable.DataSource = cpus;
        }

        private void CpuGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var col = cpuDataTable.Columns[e.ColumnIndex].DataPropertyName;

            // Превращаем null/empty в "—"
            if (col == "Brand" || col == "Model" || col == "Description" || col == "PhotoUrl" || col == "Socket")
            {
                if (e.Value == null || string.IsNullOrWhiteSpace(e.Value.ToString()))
                {
                    e.Value = "—";
                    e.FormattingApplied = true;
                }
            }

            if (col == "SupplierInn")
            {
                // SupplierInn nullable
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

        private void FilterAndSearch()
        {
            if (allCpus_ == null) return;

            string searchText = txtSearchName.Text?.Trim() ?? string.Empty;
            bool onlyAvailable = cbxOnlyAvailable.Checked;

            IEnumerable<Cpu> query = allCpus_;

            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(c =>
                    !string.IsNullOrEmpty(c.Name) &&
                    c.Name.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (onlyAvailable)
                query = query.Where(c => c.IsAvailable);

            ShowCpus(query.ToList());
        }

        private void btnAddCpu_Click(object sender, EventArgs e)
        {
            using (AddCpuForm form = new AddCpuForm(service_))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    LoadCpus();
            }
        }

        private void btnEditCpu_Click(object sender, EventArgs e)
        {
            if (cpuDataTable.CurrentRow == null || cpuDataTable.CurrentRow.DataBoundItem == null)
            {
                MessageBox.Show("Выберите процессор.", "Нет выбранной строки");
                return;
            }

            Cpu selected = cpuDataTable.CurrentRow.DataBoundItem as Cpu;

            using (EditCpuForm editForm = new EditCpuForm(service_, selected))
            {
                if (editForm.ShowDialog(this) == DialogResult.OK)
                    LoadCpus();
            }
        }

        private void btnDeleteCpu_Click(object sender, EventArgs e)
        {
            if (cpuDataTable.CurrentRow == null || cpuDataTable.CurrentRow.DataBoundItem == null)
            {
                MessageBox.Show("Сначала выберите процессор в списке.",
                    "Удаление процессора", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Cpu cpu = cpuDataTable.CurrentRow.DataBoundItem as Cpu;
            if (cpu == null)
            {
                MessageBox.Show("Не удалось получить данные процессора из таблицы.",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                $"Удалить процессор \"{cpu.Name}\" (ID={cpu.ComponentId})?\nДействие нельзя отменить.",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                string result = service_.DeleteCpu(cpu.ComponentId);

                if (string.IsNullOrEmpty(result))
                {
                    MessageBox.Show("Процессор удалён.", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadCpus();
                }
                else
                {
                    MessageBox.Show(result, "Ошибка удаления",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось выполнить удаление. Вероятно, проблемы в соединении с БД: " + ex.Message,
                    "Критическая ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CpuForm_Shown(object sender, EventArgs e)
        {
            ApplyTableTheme();
        }

        private void ApplyTableTheme()
        {
            if (cpuDataTable == null || cpuDataTable.IsDisposed)
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

            label1.ForeColor = ThemeColor.PrimaryColor;

            cpuDataTable.BackgroundColor = Color.FromArgb(248, 249, 250);
            cpuDataTable.GridColor = Color.FromArgb(206, 212, 218);
            cpuDataTable.BorderStyle = BorderStyle.FixedSingle;
            cpuDataTable.CellBorderStyle = DataGridViewCellBorderStyle.Single;

            cpuDataTable.RowHeadersVisible = false;
            cpuDataTable.AllowUserToAddRows = false;
            cpuDataTable.AllowUserToDeleteRows = false;
            cpuDataTable.ReadOnly = true;
            cpuDataTable.MultiSelect = false;
            cpuDataTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            cpuDataTable.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            cpuDataTable.EnableHeadersVisualStyles = false;

            cpuDataTable.ColumnHeadersDefaultCellStyle.BackColor = ThemeColor.PrimaryColor;
            cpuDataTable.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            cpuDataTable.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            cpuDataTable.ColumnHeadersDefaultCellStyle.SelectionBackColor = ThemeColor.PrimaryColor;
            cpuDataTable.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
            cpuDataTable.ColumnHeadersHeight = 30;

            cpuDataTable.DefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            cpuDataTable.DefaultCellStyle.ForeColor = Color.FromArgb(52, 58, 64);
            cpuDataTable.DefaultCellStyle.Font = new Font("Arial", 9);

            cpuDataTable.DefaultCellStyle.SelectionBackColor = Color.FromArgb(206, 212, 218);
            cpuDataTable.DefaultCellStyle.SelectionForeColor = Color.FromArgb(52, 58, 64);

            cpuDataTable.AllowUserToResizeColumns = false;
            cpuDataTable.AllowUserToResizeRows = false;

            cpuDataTable.RowTemplate.Height = 25;
            foreach (DataGridViewRow row in cpuDataTable.Rows)
                row.Height = cpuDataTable.RowTemplate.Height;
        }
    }
}
