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
    public partial class EditCoolingForm : Form
    {
        private readonly CoolingService coolingService_;
        private readonly SupplierService supplierService_;
        private readonly Cooling cooling_;

        public EditCoolingForm(CoolingService coolingService, Cooling item)
        {
            InitializeComponent();

            coolingService_ = coolingService ?? throw new ArgumentNullException(nameof(coolingService));
            cooling_ = item ?? throw new ArgumentNullException(nameof(item));
            supplierService_ = new SupplierService(new MySqlSupplierRepository());

            this.Shown += EditCoolingForm_Shown;
        }

        private void EditCoolingForm_Shown(object sender, EventArgs e)
        {
            LoadSuppliersToCombo();
            SetupCoolerTypeCombo();
            SetupSizeCombo();
            ApplyDefaults();
            FillControls(cooling_);
        }

        private void ApplyDefaults()
        {
            nudPrice.DecimalPlaces = 2;
            nudPrice.Minimum = 0;

            nudStock.Minimum = 0;

            nudTdpSupport.Minimum = 0;
            nudFanRpm.Minimum = 0;
        }

        private void SetupCoolerTypeCombo()
        {
            cbxCoolerType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxCoolerType.Items.Clear();
            cbxCoolerType.Items.AddRange(new object[] { "air", "liquid" });
        }

        private void SetupSizeCombo()
        {
            cbxSize.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxSize.Items.Clear();
            cbxSize.Items.AddRange(new object[] { "—", "full_tower", "mid_tower", "compact" });
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

            cbxSupplier.SelectedValue = cooling_.SupplierInn.HasValue ? cooling_.SupplierInn.Value : 0;
        }

        private void FillControls(Cooling c)
        {
            txtName.Text = c.Name ?? "";
            txtBrand.Text = c.Brand ?? "";
            txtModel.Text = c.Model ?? "";
            txtDesc.Text = c.Description ?? "";
            txtPhoto.Text = c.PhotoUrl ?? "";

            nudPrice.Value = c.Price < 0 ? 0 : c.Price;
            nudStock.Value = c.StockQuantity < 0 ? 0 : c.StockQuantity;
            chkAvailable.Checked = c.IsAvailable;

            cbxCoolerType.SelectedItem = string.IsNullOrWhiteSpace(c.CoolerType) ? "air" : c.CoolerType;

            cbxSize.SelectedItem = string.IsNullOrWhiteSpace(c.Size) ? "—" : c.Size;

            nudTdpSupport.Value = c.TdpSupport.HasValue ? c.TdpSupport.Value : 0;
            nudFanRpm.Value = c.FanRpm.HasValue ? c.FanRpm.Value : 0;

            chkRgb.Checked = c.IsRgb;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int selectedInn = (int)cbxSupplier.SelectedValue;

            string size = cbxSize.SelectedItem?.ToString();
            if (size == "—") size = null;

            cooling_.Name = txtName.Text?.Trim();
            cooling_.Brand = string.IsNullOrWhiteSpace(txtBrand.Text) ? null : txtBrand.Text.Trim();
            cooling_.Model = string.IsNullOrWhiteSpace(txtModel.Text) ? null : txtModel.Text.Trim();
            cooling_.Description = string.IsNullOrWhiteSpace(txtDesc.Text) ? null : txtDesc.Text.Trim();
            cooling_.PhotoUrl = string.IsNullOrWhiteSpace(txtPhoto.Text) ? null : txtPhoto.Text.Trim();

            cooling_.Price = nudPrice.Value;
            cooling_.StockQuantity = (int)nudStock.Value;
            cooling_.IsAvailable = chkAvailable.Checked;

            cooling_.SupplierInn = selectedInn == 0 ? (int?)null : selectedInn;

            cooling_.CoolerType = cbxCoolerType.SelectedItem?.ToString();
            cooling_.TdpSupport = nudTdpSupport.Value == 0 ? (int?)null : (int)nudTdpSupport.Value;
            cooling_.FanRpm = nudFanRpm.Value == 0 ? (int?)null : (int)nudFanRpm.Value;
            cooling_.Size = size;
            cooling_.IsRgb = chkRgb.Checked;

            string result = coolingService_.UpdateCooling(cooling_);
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
