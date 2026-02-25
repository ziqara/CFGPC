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
    public partial class AddRamForm : Form
    {
        private readonly RamService ramService_;
        private readonly SupplierService supplierService_;

        public AddRamForm(RamService ramService)
        {
            InitializeComponent();

            ramService_ = ramService ?? throw new ArgumentNullException(nameof(ramService));
            supplierService_ = new SupplierService(new MySqlSupplierRepository());

            this.Shown += AddRamForm_Shown;
        }

        private void AddRamForm_Shown(object sender, EventArgs e)
        {
            LoadSuppliersToCombo();
            SetupRamTypeCombo();
            ApplyDefaults();
        }

        private void ApplyDefaults()
        {
            chkAvailable.Checked = true;

            nudPrice.DecimalPlaces = 2;
            nudPrice.Minimum = 0;

            nudStock.Minimum = 0;

            nudCapacityGb.Minimum = 0;
            nudSpeedMhz.Minimum = 0;
            nudSlotsNeeded.Minimum = 0;
        }

        private void SetupRamTypeCombo()
        {
            cbxRamType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxRamType.Items.Clear();
            cbxRamType.Items.AddRange(new object[] { "—", "DDR4", "DDR5" });
            cbxRamType.SelectedIndex = 0;
        }

        private void LoadSuppliersToCombo()
        {
            List<Supplier> suppliers = supplierService_.GetAllSuppliers() ?? new List<Supplier>();

            suppliers.Insert(0, new Supplier(0)
            {
                Name = "— (не указан)",
                ContactEmail = "",
                Phone = null,
                Address = null
            });

            cbxSupplier.DataSource = suppliers;
            cbxSupplier.DisplayMember = "Name";
            cbxSupplier.ValueMember = "Inn";
            cbxSupplier.SelectedIndex = 0;
            cbxSupplier.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Ram ram = BuildFromControls();

            string result = ramService_.CreateRam(ram);
            if (!string.IsNullOrEmpty(result))
            {
                MessageBox.Show(result, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private Ram BuildFromControls()
        {
            int selectedInn = (int)cbxSupplier.SelectedValue;

            string ramType = cbxRamType.SelectedItem?.ToString();
            if (ramType == "—") ramType = null;

            return new Ram
            {
                Name = txtName.Text?.Trim(),
                Brand = string.IsNullOrWhiteSpace(txtBrand.Text) ? null : txtBrand.Text.Trim(),
                Model = string.IsNullOrWhiteSpace(txtModel.Text) ? null : txtModel.Text.Trim(),
                Description = string.IsNullOrWhiteSpace(txtDesc.Text) ? null : txtDesc.Text.Trim(),
                PhotoUrl = string.IsNullOrWhiteSpace(txtPhoto.Text) ? null : txtPhoto.Text.Trim(),

                Price = nudPrice.Value,
                StockQuantity = (int)nudStock.Value,
                IsAvailable = chkAvailable.Checked,

                SupplierInn = selectedInn == 0 ? (int?)null : selectedInn,

                RamType = ramType,
                CapacityGb = nudCapacityGb.Value == 0 ? (int?)null : (int)nudCapacityGb.Value,
                SpeedMhz = nudSpeedMhz.Value == 0 ? (int?)null : (int)nudSpeedMhz.Value,
                SlotsNeeded = nudSlotsNeeded.Value == 0 ? (int?)null : (int)nudSlotsNeeded.Value
            };
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
