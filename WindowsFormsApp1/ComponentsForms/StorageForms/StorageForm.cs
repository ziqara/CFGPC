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

namespace WindowsFormsApp1.ComponentsForms.StorageForms
{
    public partial class StorageForm : Form
    {
        private readonly StorageService service_;
        private List<Storage> allStorages_ = new List<Storage>();

        public StorageForm()
        {
            InitializeComponent();

            service_ = new StorageService(new MySqlStorageRepository());

            storageDataTable.CellFormatting += Grid_CellFormatting;

            this.Shown += StorageForm_Shown;
            ThemeColor.ThemeChanged += ApplyTableTheme;
        }

        private void StorageForm_Load(object sender, EventArgs e)
        {
            LoadStorages();
        }

        private void LoadStorages()
        {
            try
            {
                var list = service_.GetAllStorages();
                allStorages_ = list ?? new List<Storage>();

                if (allStorages_.Count == 0)
                {
                    storageDataTable.Visible = false;
                    MessageBox.Show("Накопителей пока нет", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                storageDataTable.Visible = true;
                ShowStorages(allStorages_);
            }
            catch (Exception ex)
            {
                storageDataTable.Visible = false;
                MessageBox.Show($"Вероятно, проблемы в соединении с БД.\n\n{ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowStorages(List<Storage> list)
        {
            storageDataTable.DataSource = null;
            storageDataTable.AutoGenerateColumns = true;
            storageDataTable.DataSource = list;
        }

        private void Grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var col = storageDataTable.Columns[e.ColumnIndex].DataPropertyName;

            if (col == "Brand" || col == "Model" || col == "Description" || col == "PhotoUrl" || col == "Interface")
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

        private void StorageForm_Shown(object sender, EventArgs e)
        {
            ApplyTableTheme();
        }

        private void ApplyTableTheme()
        {
            if (storageDataTable == null || storageDataTable.IsDisposed)
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

            storageDataTable.BackgroundColor = Color.FromArgb(248, 249, 250);
            storageDataTable.GridColor = Color.FromArgb(206, 212, 218);
            storageDataTable.BorderStyle = BorderStyle.FixedSingle;
            storageDataTable.CellBorderStyle = DataGridViewCellBorderStyle.Single;

            storageDataTable.RowHeadersVisible = false;
            storageDataTable.AllowUserToAddRows = false;
            storageDataTable.AllowUserToDeleteRows = false;
            storageDataTable.ReadOnly = true;
            storageDataTable.MultiSelect = false;
            storageDataTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            storageDataTable.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            storageDataTable.EnableHeadersVisualStyles = false;

            storageDataTable.ColumnHeadersDefaultCellStyle.BackColor = ThemeColor.PrimaryColor;
            storageDataTable.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            storageDataTable.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            storageDataTable.ColumnHeadersHeight = 30;

            storageDataTable.DefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            storageDataTable.DefaultCellStyle.ForeColor = Color.FromArgb(52, 58, 64);
            storageDataTable.DefaultCellStyle.Font = new Font("Arial", 9);
            storageDataTable.DefaultCellStyle.SelectionBackColor = Color.FromArgb(206, 212, 218);
            storageDataTable.DefaultCellStyle.SelectionForeColor = Color.FromArgb(52, 58, 64);

            storageDataTable.AllowUserToResizeColumns = false;
            storageDataTable.AllowUserToResizeRows = false;

            storageDataTable.RowTemplate.Height = 25;
            foreach (DataGridViewRow row in storageDataTable.Rows)
                row.Height = storageDataTable.RowTemplate.Height;
        }

        private void btnAddCpu_Click(object sender, EventArgs e)
        {
            using (var form = new AddStorageForm(service_))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    LoadStorages();
            }
        }

        private void btnEditCpu_Click(object sender, EventArgs e)
        {
            if (storageDataTable.CurrentRow?.DataBoundItem == null)
            {
                MessageBox.Show("Выберите накопитель.", "Нет выбранной строки");
                return;
            }

            var selected = storageDataTable.CurrentRow.DataBoundItem as Storage;

            using (var form = new EditStorageForm(service_, selected))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    LoadStorages();
            }
        }

        private void btnDeleteCpu_Click(object sender, EventArgs e)
        {
            if (storageDataTable.CurrentRow?.DataBoundItem == null)
            {
                MessageBox.Show("Сначала выберите накопитель в списке.",
                    "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var st = storageDataTable.CurrentRow.DataBoundItem as Storage;
            if (st == null)
            {
                MessageBox.Show("Не удалось получить данные из таблицы.",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var confirm = MessageBox.Show(
                $"Удалить \"{st.Name}\" (ID={st.ComponentId})?\nДействие нельзя отменить.",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            try
            {
                string result = service_.DeleteStorage(st.ComponentId);

                if (string.IsNullOrEmpty(result))
                {
                    MessageBox.Show("Накопитель удалён.", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadStorages();
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
            if (allStorages_ == null) return;

            string searchText = txtSearchName.Text?.Trim() ?? "";
            bool onlyAvailable = cbxOnlyAvailable.Checked;

            IEnumerable<Storage> q = allStorages_;

            if (!string.IsNullOrEmpty(searchText))
            {
                q = q.Where(s =>
                    (s.Name ?? "").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (s.Brand ?? "").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (s.Model ?? "").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (s.Interface ?? "").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (s.CapacityGb?.ToString() ?? "").Contains(searchText)
                );
            }

            if (onlyAvailable)
                q = q.Where(s => s.IsAvailable);

            ShowStorages(q.ToList());
        }
    }
}
