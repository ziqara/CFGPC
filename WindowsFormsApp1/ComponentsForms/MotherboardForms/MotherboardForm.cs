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

namespace WindowsFormsApp1.ComponentsForms.MotherboardForms
{
    public partial class MotherboardForm : Form
    {
        private readonly MotherboardService service_;
        private List<Motherboard> allMotherboards_ = new List<Motherboard>();

        public MotherboardForm()
        {
            InitializeComponent();

            service_ = new MotherboardService(new MySqlMotherboardRepository());

            mBoardDataTable.CellFormatting += Grid_CellFormatting;

            this.Shown += MotherboardForm_Shown;
            ThemeColor.ThemeChanged += ApplyTableTheme;
        }

        private void MotherboardForm_Load(object sender, EventArgs e)
        {
            LoadMotherboards();
        }

        private void LoadMotherboards()
        {
            try
            {
                List<Motherboard> list = service_.GetAllMotherboards();
                allMotherboards_ = list ?? new List<Motherboard>();

                if (allMotherboards_.Count == 0)
                {
                    mBoardDataTable.Visible = false;
                    MessageBox.Show("Материнских плат пока нет", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                mBoardDataTable.Visible = true;
                ShowMotherboards(allMotherboards_);
            }
            catch (Exception ex)
            {
                mBoardDataTable.Visible = false;
                MessageBox.Show($"Вероятно, проблемы в соединении с БД.\n\n{ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ShowMotherboards(List<Motherboard> list)
        {
            mBoardDataTable.DataSource = null;
            mBoardDataTable.AutoGenerateColumns = true;
            mBoardDataTable.DataSource = list;
        }

        private void Grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var col = mBoardDataTable.Columns[e.ColumnIndex].DataPropertyName;

            // Пустые строки -> "—"
            if (col == "Brand" || col == "Model" || col == "Description" || col == "PhotoUrl" ||
                col == "Socket" || col == "Chipset" || col == "RamType" || col == "PcieVersion" || col == "FormFactor")
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

        private void MotherboardForm_Shown(object sender, EventArgs e)
        {
            ApplyTableTheme();
        }

        private void ApplyTableTheme()
        {
            if (mBoardDataTable == null || mBoardDataTable.IsDisposed)
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

            mBoardDataTable.BackgroundColor = Color.FromArgb(248, 249, 250);
            mBoardDataTable.GridColor = Color.FromArgb(206, 212, 218);
            mBoardDataTable.BorderStyle = BorderStyle.FixedSingle;
            mBoardDataTable.CellBorderStyle = DataGridViewCellBorderStyle.Single;

            mBoardDataTable.RowHeadersVisible = false;
            mBoardDataTable.AllowUserToAddRows = false;
            mBoardDataTable.AllowUserToDeleteRows = false;
            mBoardDataTable.ReadOnly = true;
            mBoardDataTable.MultiSelect = false;
            mBoardDataTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            mBoardDataTable.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            mBoardDataTable.EnableHeadersVisualStyles = false;

            mBoardDataTable.ColumnHeadersDefaultCellStyle.BackColor = ThemeColor.PrimaryColor;
            mBoardDataTable.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            mBoardDataTable.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            mBoardDataTable.ColumnHeadersDefaultCellStyle.SelectionBackColor = ThemeColor.PrimaryColor;
            mBoardDataTable.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
            mBoardDataTable.ColumnHeadersHeight = 30;

            mBoardDataTable.DefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            mBoardDataTable.DefaultCellStyle.ForeColor = Color.FromArgb(52, 58, 64);
            mBoardDataTable.DefaultCellStyle.Font = new Font("Arial", 9);
            mBoardDataTable.DefaultCellStyle.SelectionBackColor = Color.FromArgb(206, 212, 218);
            mBoardDataTable.DefaultCellStyle.SelectionForeColor = Color.FromArgb(52, 58, 64);

            mBoardDataTable.AllowUserToResizeColumns = false;
            mBoardDataTable.AllowUserToResizeRows = false;

            mBoardDataTable.RowTemplate.Height = 25;
            foreach (DataGridViewRow row in mBoardDataTable.Rows)
                row.Height = mBoardDataTable.RowTemplate.Height;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new AddMotherboardForm(service_))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    LoadMotherboards();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (mBoardDataTable.CurrentRow?.DataBoundItem == null)
            {
                MessageBox.Show("Выберите материнскую плату.", "Нет выбранной строки");
                return;
            }

            var selected = mBoardDataTable.CurrentRow.DataBoundItem as Motherboard;

            using (var form = new EditMotherboardForm(service_, selected))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    LoadMotherboards();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (mBoardDataTable.CurrentRow?.DataBoundItem == null)
            {
                MessageBox.Show("Сначала выберите материнскую плату в списке.",
                    "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var mb = mBoardDataTable.CurrentRow.DataBoundItem as Motherboard;
            if (mb == null)
            {
                MessageBox.Show("Не удалось получить данные из таблицы.",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var confirm = MessageBox.Show(
                $"Удалить материнскую плату \"{mb.Name}\" (ID={mb.ComponentId})?\nДействие нельзя отменить.",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            try
            {
                string result = service_.DeleteMotherboard(mb.ComponentId);

                if (string.IsNullOrEmpty(result))
                {
                    MessageBox.Show("Материнская плата удалена.", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadMotherboards();
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

        private void cbxOnlyAvailable_TextChanged(object sender, EventArgs e)
        {
            FilterAndSearch();
        }

        private void FilterAndSearch()
        {
            if (allMotherboards_ == null) return;

            string searchText = txtSearchName.Text?.Trim() ?? "";
            bool onlyAvailable = cbxOnlyAvailable.Checked;

            IEnumerable<Motherboard> q = allMotherboards_;

            // Поиск по нескольким полям (удобнее)
            if (!string.IsNullOrEmpty(searchText))
            {
                q = q.Where(m =>
                    (m.Name ?? "").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (m.Brand ?? "").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (m.Model ?? "").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (m.Socket ?? "").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (m.Chipset ?? "").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (m.RamType ?? "").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (m.PcieVersion ?? "").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (m.FormFactor ?? "").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0
                );
            }

            if (onlyAvailable)
                q = q.Where(m => m.IsAvailable);

            ShowMotherboards(q.ToList());
        }

        private void cbxOnlyAvailable_CheckedChanged(object sender, EventArgs e)
        {
            FilterAndSearch();
        }
    }
}
