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
using DDMLib.Configuration;

namespace WindowsFormsApp1.UserOrder
{
    public partial class ConfigDetailsForm : Form
    {
        private readonly string configName_;
        private readonly IConfigurationRepository repo_;
        private readonly int configId_;

        public ConfigDetailsForm(int configId, string configName)
        {
            InitializeComponent();

            configId_ = configId;
            configName_ = configName ?? "";
            repo_ = new ConfigurationRepository();

            this.Shown += (s, e) => ApplyTableTheme();
            ThemeColor.ThemeChanged += ApplyTableTheme;
        }

        private void ConfigDetailsForm_Load(object sender, EventArgs e)
        {
            lblConfig.Text = $"Конфигурация: {configName_}";
            LoadDetails();
        }

        private void LoadDetails()
        {
            try
            {
                var details = repo_.GetDetails(configId_);

                dgvComponents.DataSource = null;
                dgvComponents.AutoGenerateColumns = true;
                dgvComponents.DataSource = details.Components;

                if (dgvComponents.Columns["Price"] != null)
                    dgvComponents.Columns["Price"].DefaultCellStyle.Format = "N2";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось загрузить состав ПК.\n\n{ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ConfigDetailsForm_Shown(object sender, EventArgs e)
        {
            ApplyTableTheme();
        }

        private void ApplyTableTheme()
        {
            foreach (Control c in this.Controls)
            {
                if (c is Button btn)
                {
                    btn.BackColor = ThemeColor.PrimaryColor;
                    btn.ForeColor = Color.White;
                    btn.FlatAppearance.BorderColor = ThemeColor.SecondaryColor;
                }
            }

            if (lblConfig != null) lblConfig.ForeColor = ThemeColor.PrimaryColor;

            if (dgvComponents == null || dgvComponents.IsDisposed) return;

            dgvComponents.BackgroundColor = Color.FromArgb(248, 249, 250);
            dgvComponents.GridColor = Color.FromArgb(206, 212, 218);
            dgvComponents.BorderStyle = BorderStyle.FixedSingle;
            dgvComponents.CellBorderStyle = DataGridViewCellBorderStyle.Single;

            dgvComponents.RowHeadersVisible = false;
            dgvComponents.AllowUserToAddRows = false;
            dgvComponents.AllowUserToDeleteRows = false;
            dgvComponents.ReadOnly = true;
            dgvComponents.MultiSelect = false;
            dgvComponents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvComponents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgvComponents.EnableHeadersVisualStyles = false;

            dgvComponents.ColumnHeadersDefaultCellStyle.BackColor = ThemeColor.PrimaryColor;
            dgvComponents.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvComponents.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            dgvComponents.ColumnHeadersHeight = 30;

            dgvComponents.DefaultCellStyle.Font = new Font("Arial", 9);
            dgvComponents.DefaultCellStyle.SelectionBackColor = Color.FromArgb(206, 212, 218);
            dgvComponents.DefaultCellStyle.SelectionForeColor = Color.FromArgb(52, 58, 64);

            dgvComponents.AllowUserToResizeColumns = false;
            dgvComponents.AllowUserToResizeRows = false;

            dgvComponents.RowTemplate.Height = 25;
            foreach (DataGridViewRow row in dgvComponents.Rows)
                row.Height = dgvComponents.RowTemplate.Height;
        }
    }
}
