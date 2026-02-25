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

namespace WindowsFormsApp1.ComponentsForms.CoolingForms
{
    public partial class AddCoolingForm : Form
    {
        private readonly CoolingService coolingService_;
        private readonly SupplierService supplierService_;

        public AddCoolingForm(CoolingService coolingService)
        {
            InitializeComponent();

            coolingService_ = coolingService ?? throw new ArgumentNullException(nameof(coolingService));
            supplierService_ = new SupplierService(new MySqlSupplierRepository());

            this.Shown += AddCoolingForm_Shown;
        }

        private void AddCoolingForm_Shown(object sender, EventArgs e)
        {
            LoadSuppliersToCombo();
            SetupCoolerTypeCombo();
            SetupSizeCombo();
            ApplyDefaults();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Cooling item = BuildFromControls();

            string result = coolingService_.CreateCooling(item);
            if (!string.IsNullOrEmpty(result))
            {
                MessageBox.Show(result, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ApplyDefaults()
        {
            chkAvailable.Checked = true;

            nudPrice.DecimalPlaces = 2;
            nudPrice.Minimum = 0;

            nudStock.Minimum = 0;

            nudTdpSupport.Minimum = 0; // 0 = NULL
            nudFanRpm.Minimum = 0;     // 0 = NULL
        }

        private void SetupCoolerTypeCombo()
        {
            cbxCoolerType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxCoolerType.Items.Clear();
            cbxCoolerType.Items.AddRange(new object[] { "air", "liquid" });
            cbxCoolerType.SelectedItem = "air";
        }

        private void SetupSizeCombo()
        {
            cbxSize.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxSize.Items.Clear();
            cbxSize.Items.AddRange(new object[] { "—", "full_tower", "mid_tower", "compact" });
            cbxSize.SelectedIndex = 0; // — = NULL
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

        private Cooling BuildFromControls()
        {
            int selectedInn = (int)cbxSupplier.SelectedValue;

            string size = cbxSize.SelectedItem?.ToString();
            if (size == "—") size = null;

            return new Cooling
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

                CoolerType = cbxCoolerType.SelectedItem?.ToString(),
                TdpSupport = nudTdpSupport.Value == 0 ? (int?)null : (int)nudTdpSupport.Value,
                FanRpm = nudFanRpm.Value == 0 ? (int?)null : (int)nudFanRpm.Value,
                Size = size,
                IsRgb = chkRgb.Checked
            };
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
