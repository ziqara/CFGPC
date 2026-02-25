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
    public partial class EditRamForm : Form
    {
        private readonly RamService ramService_;
        private readonly SupplierService supplierService_;
        private readonly Ram ram_;

        public EditRamForm(RamService ramService, Ram ram)
        {
            InitializeComponent();

            ramService_ = ramService ?? throw new ArgumentNullException(nameof(ramService));
            ram_ = ram ?? throw new ArgumentNullException(nameof(ram));
            supplierService_ = new SupplierService(new MySqlSupplierRepository());

            this.Shown += EditRamForm_Shown;
        }

        private void EditRamForm_Shown(object sender, EventArgs e)
        {
            LoadSuppliersToCombo();
            SetupRamTypeCombo();
            ApplyDefaults();
            FillControls(ram_);
        }

        private void ApplyDefaults()
        {
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
            cbxSupplier.DropDownStyle = ComboBoxStyle.DropDownList;

            cbxSupplier.SelectedValue = ram_.SupplierInn.HasValue ? ram_.SupplierInn.Value : 0;
        }

        private void FillControls(Ram r)
        {
            txtName.Text = r.Name ?? "";
            txtBrand.Text = r.Brand ?? "";
            txtModel.Text = r.Model ?? "";
            txtDesc.Text = r.Description ?? "";
            txtPhoto.Text = r.PhotoUrl ?? "";

            nudPrice.Value = r.Price < 0 ? 0 : r.Price;
            nudStock.Value = r.StockQuantity < 0 ? 0 : r.StockQuantity;
            chkAvailable.Checked = r.IsAvailable;

            cbxRamType.SelectedItem = string.IsNullOrWhiteSpace(r.RamType) ? "—" : r.RamType;

            nudCapacityGb.Value = r.CapacityGb.HasValue ? r.CapacityGb.Value : 0;
            nudSpeedMhz.Value = r.SpeedMhz.HasValue ? r.SpeedMhz.Value : 0;
            nudSlotsNeeded.Value = r.SlotsNeeded.HasValue ? r.SlotsNeeded.Value : 0;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int selectedInn = (int)cbxSupplier.SelectedValue;

            string ramType = cbxRamType.SelectedItem?.ToString();
            if (ramType == "—") ramType = null;

            ram_.Name = txtName.Text?.Trim();
            ram_.Brand = string.IsNullOrWhiteSpace(txtBrand.Text) ? null : txtBrand.Text.Trim();
            ram_.Model = string.IsNullOrWhiteSpace(txtModel.Text) ? null : txtModel.Text.Trim();
            ram_.Description = string.IsNullOrWhiteSpace(txtDesc.Text) ? null : txtDesc.Text.Trim();
            ram_.PhotoUrl = string.IsNullOrWhiteSpace(txtPhoto.Text) ? null : txtPhoto.Text.Trim();

            ram_.Price = nudPrice.Value;
            ram_.StockQuantity = (int)nudStock.Value;
            ram_.IsAvailable = chkAvailable.Checked;

            ram_.SupplierInn = selectedInn == 0 ? (int?)null : selectedInn;

            ram_.RamType = ramType;
            ram_.CapacityGb = nudCapacityGb.Value == 0 ? (int?)null : (int)nudCapacityGb.Value;
            ram_.SpeedMhz = nudSpeedMhz.Value == 0 ? (int?)null : (int)nudSpeedMhz.Value;
            ram_.SlotsNeeded = nudSlotsNeeded.Value == 0 ? (int?)null : (int)nudSlotsNeeded.Value;

            string result = ramService_.UpdateRam(ram_);
            if (!string.IsNullOrEmpty(result))
            {
                MessageBox.Show(result, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
