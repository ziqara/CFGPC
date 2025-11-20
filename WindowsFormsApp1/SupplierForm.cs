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

namespace WindowsFormsApp1
{
    public partial class SupplierForm : Form
    {
        private readonly SupplierService service_;

        public SupplierForm()
        {
            InitializeComponent();
            service_ = new SupplierService(new MySqlSupplierRepository());

            supplierDataTable.CellFormatting += SupplierGridView_CellFormatting;
            supplierDataTable.CellToolTipTextNeeded += SupplierGridView_CellToolTipTextNeeded;
            this.Shown += SupplierForm_Shown;
            ThemeColor.ThemeChanged += ApplyTableTheme;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadSuppliers();
        }

        private void LoadSuppliers()
        {
            try
            {
                List<Supplier> suppliers = service_.GetAllSuppliers();

                if (suppliers == null || suppliers.Count == 0)
                {
                    supplierDataTable.Visible = false;
                    MessageBox.Show("Поставщиков пока нет", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                supplierDataTable.DataSource = suppliers;
                supplierDataTable.Visible = true;
            }
            catch (Exception ex)
            {
                supplierDataTable.Visible = false;
                MessageBox.Show($"Вероятно, проблемы в соединении с БД.\n\n{ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SupplierGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var col = supplierDataTable.Columns[e.ColumnIndex].DataPropertyName;
            if (col == "Phone" || col == "Address")
            {
                var value = e.Value as string;
                if (string.IsNullOrEmpty(value))
                {
                    e.Value = "—";
                    e.FormattingApplied = true;
                }
            }
        }

        private void SupplierGridView_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var column = supplierDataTable.Columns[e.ColumnIndex];
            if (column.DataPropertyName != "Address") return;

            var supplier = supplierDataTable.Rows[e.RowIndex].DataBoundItem as Supplier;
            var address = supplier?.Address;

            e.ToolTipText = string.IsNullOrWhiteSpace(address) ? string.Empty : address;
        }

        private void btnAddSupplier_Click(object sender, EventArgs e)
        {
            using (AddSupplierForm form = new AddSupplierForm())
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    LoadSuppliers();
                }
            }
        }

        private void ApplyTableTheme()
        {
            if (supplierDataTable == null || supplierDataTable.IsDisposed)
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
            cbxPhone.ForeColor = ThemeColor.PrimaryColor;
            cbxAddres.ForeColor = ThemeColor.PrimaryColor;
            // Общие свойства таблицы
            supplierDataTable.BackgroundColor = Color.FromArgb(248, 249, 250);
            supplierDataTable.GridColor = Color.FromArgb(206, 212, 218);
            supplierDataTable.BorderStyle = BorderStyle.FixedSingle;
            supplierDataTable.CellBorderStyle = DataGridViewCellBorderStyle.Single;

            supplierDataTable.RowHeadersVisible = false;
            supplierDataTable.AllowUserToAddRows = false;
            supplierDataTable.AllowUserToDeleteRows = false;
            supplierDataTable.ReadOnly = true;
            supplierDataTable.MultiSelect = false;
            supplierDataTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            supplierDataTable.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            supplierDataTable.EnableHeadersVisualStyles = false;

            // Заголовки колонок
            supplierDataTable.ColumnHeadersDefaultCellStyle.BackColor = ThemeColor.PrimaryColor;
            supplierDataTable.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            supplierDataTable.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            supplierDataTable.ColumnHeadersDefaultCellStyle.SelectionBackColor = ThemeColor.PrimaryColor;
            supplierDataTable.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
            supplierDataTable.ColumnHeadersHeight = 30;

            // Строки
            supplierDataTable.DefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            supplierDataTable.DefaultCellStyle.ForeColor = Color.FromArgb(52, 58, 64);
            supplierDataTable.DefaultCellStyle.Font = new Font("Arial", 9);

            supplierDataTable.DefaultCellStyle.SelectionBackColor = Color.FromArgb(206, 212, 218);
            supplierDataTable.DefaultCellStyle.SelectionForeColor = Color.FromArgb(52, 58, 64);

            supplierDataTable.AllowUserToResizeColumns = false;
            supplierDataTable.AllowUserToResizeRows = false;

            supplierDataTable.RowTemplate.Height = 25;
            foreach (DataGridViewRow row in supplierDataTable.Rows)
            {
                row.Height = supplierDataTable.RowTemplate.Height;
            }
            supplierDataTable.ClearSelection();
        }

        private void SupplierForm_Shown(object sender, EventArgs e)
        {
            ApplyTableTheme();
        }
    }
}
