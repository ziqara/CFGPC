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

namespace WindowsFormsApp1.ComponentsForms.CaseForms
{
    public partial class CaseForm : Form
    {
        private readonly CaseService service_;
        private List<Case> allCases_ = new List<Case>();

        public CaseForm()
        {
            InitializeComponent();

            service_ = new CaseService(new MySqlCaseRepository());

            caseDataTable.CellFormatting += Grid_CellFormatting;

            this.Shown += CaseForm_Shown;
            ThemeColor.ThemeChanged += ApplyTableTheme;
        }

        private void CaseForm_Load(object sender, EventArgs e)
        {
            LoadCases();
        }
        private void LoadCases()
        {
            try
            {
                var list = service_.GetAllCases();
                allCases_ = list ?? new List<Case>();

                if (allCases_.Count == 0)
                {
                    caseDataTable.Visible = false;
                    MessageBox.Show("Корпусов пока нет", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                caseDataTable.Visible = true;
                ShowCases(allCases_);
            }
            catch (Exception ex)
            {
                caseDataTable.Visible = false;
                MessageBox.Show($"Вероятно, проблемы в соединении с БД.\n\n{ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowCases(List<Case> list)
        {
            caseDataTable.DataSource = null;
            caseDataTable.AutoGenerateColumns = true;
            caseDataTable.DataSource = list;
        }

        private void Grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var col = caseDataTable.Columns[e.ColumnIndex].DataPropertyName;

            if (col == "Brand" || col == "Model" || col == "Description" || col == "PhotoUrl" || col == "FormFactor")
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


        private void CaseForm_Shown(object sender, EventArgs e)
        {
            ApplyTableTheme();
        }

        private void ApplyTableTheme()
        {
            if (caseDataTable == null || caseDataTable.IsDisposed)
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

            caseDataTable.BackgroundColor = Color.FromArgb(248, 249, 250);
            caseDataTable.GridColor = Color.FromArgb(206, 212, 218);
            caseDataTable.BorderStyle = BorderStyle.FixedSingle;
            caseDataTable.CellBorderStyle = DataGridViewCellBorderStyle.Single;

            caseDataTable.RowHeadersVisible = false;
            caseDataTable.AllowUserToAddRows = false;
            caseDataTable.AllowUserToDeleteRows = false;
            caseDataTable.ReadOnly = true;
            caseDataTable.MultiSelect = false;
            caseDataTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            caseDataTable.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            caseDataTable.EnableHeadersVisualStyles = false;

            caseDataTable.ColumnHeadersDefaultCellStyle.BackColor = ThemeColor.PrimaryColor;
            caseDataTable.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            caseDataTable.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            caseDataTable.ColumnHeadersHeight = 30;

            caseDataTable.DefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            caseDataTable.DefaultCellStyle.ForeColor = Color.FromArgb(52, 58, 64);
            caseDataTable.DefaultCellStyle.Font = new Font("Arial", 9);
            caseDataTable.DefaultCellStyle.SelectionBackColor = Color.FromArgb(206, 212, 218);
            caseDataTable.DefaultCellStyle.SelectionForeColor = Color.FromArgb(52, 58, 64);

            caseDataTable.AllowUserToResizeColumns = false;
            caseDataTable.AllowUserToResizeRows = false;

            caseDataTable.RowTemplate.Height = 25;
            foreach (DataGridViewRow row in caseDataTable.Rows)
                row.Height = caseDataTable.RowTemplate.Height;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new AddCaseForm(service_))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    LoadCases();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (caseDataTable.CurrentRow?.DataBoundItem == null)
            {
                MessageBox.Show("Выберите корпус.", "Нет выбранной строки");
                return;
            }

            var selected = caseDataTable.CurrentRow.DataBoundItem as Case;

            using (var form = new EditCaseForm(service_, selected))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    LoadCases();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (caseDataTable.CurrentRow?.DataBoundItem == null)
            {
                MessageBox.Show("Сначала выберите корпус в списке.",
                    "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var item = caseDataTable.CurrentRow.DataBoundItem as Case;
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
                string result = service_.DeleteCase(item.ComponentId);

                if (string.IsNullOrEmpty(result))
                {
                    MessageBox.Show("Корпус удалён.", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCases();
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
            if (allCases_ == null) return;

            string searchText = txtSearchName.Text?.Trim() ?? "";
            bool onlyAvailable = cbxOnlyAvailable.Checked;

            IEnumerable<Case> q = allCases_;

            if (!string.IsNullOrEmpty(searchText))
            {
                q = q.Where(c =>
                    (c.Name ?? "").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (c.Brand ?? "").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (c.Model ?? "").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (c.FormFactor ?? "").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (c.Size ?? "").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0
                );
            }

            if (onlyAvailable)
                q = q.Where(c => c.IsAvailable);

            ShowCases(q.ToList());
        }
    }
}
