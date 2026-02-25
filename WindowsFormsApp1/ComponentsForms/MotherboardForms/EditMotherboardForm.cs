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
    public partial class EditMotherboardForm : Form
    {
        private readonly MotherboardService mbService_;
        private readonly SupplierService supplierService_;
        private readonly Motherboard mb_;

        public EditMotherboardForm(MotherboardService mbService, Motherboard mb)
        {
            InitializeComponent();

            mbService_ = mbService ?? throw new ArgumentNullException(nameof(mbService));
            mb_ = mb ?? throw new ArgumentNullException(nameof(mb));
            supplierService_ = new SupplierService(new MySqlSupplierRepository());

            this.Shown += EditMotherboardForm_Shown;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int selectedInn = (int)cbxSupplier.SelectedValue;

            string ram = cbxRamType.SelectedItem?.ToString();
            if (ram == "—") ram = null;

            string pcie = cbxPcieVersion.SelectedItem?.ToString();
            if (pcie == "—") pcie = null;

            mb_.Name = txtName.Text?.Trim();
            mb_.Brand = string.IsNullOrWhiteSpace(txtBrand.Text) ? null : txtBrand.Text.Trim();
            mb_.Model = string.IsNullOrWhiteSpace(txtModel.Text) ? null : txtModel.Text.Trim();
            mb_.Description = string.IsNullOrWhiteSpace(txtDesc.Text) ? null : txtDesc.Text.Trim();
            mb_.PhotoUrl = string.IsNullOrWhiteSpace(txtPhoto.Text) ? null : txtPhoto.Text.Trim();

            mb_.Price = nudPrice.Value;
            mb_.StockQuantity = (int)nudStock.Value;
            mb_.IsAvailable = chkAvailable.Checked;

            mb_.SupplierInn = selectedInn == 0 ? (int?)null : selectedInn;

            mb_.Socket = string.IsNullOrWhiteSpace(txtSocket.Text) ? null : txtSocket.Text.Trim();
            mb_.Chipset = string.IsNullOrWhiteSpace(txtChipset.Text) ? null : txtChipset.Text.Trim();
            mb_.FormFactor = string.IsNullOrWhiteSpace(txtFormFactor.Text) ? null : txtFormFactor.Text.Trim();
            mb_.RamType = ram;
            mb_.PcieVersion = pcie;

            string result = mbService_.UpdateMotherboard(mb_);
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

        private void EditMotherboardForm_Shown(object sender, EventArgs e)
        {
            LoadSuppliersToCombo();
            SetupRamAndPcieCombos();
            ApplyDefaults();
            FillControls(mb_);
        }
        private void ApplyDefaults()
        {
            nudPrice.DecimalPlaces = 2;
            nudPrice.Minimum = 0;
            nudStock.Minimum = 0;
        }

        private void SetupRamAndPcieCombos()
        {
            cbxRamType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxRamType.Items.Clear();
            cbxRamType.Items.AddRange(new object[] { "—", "DDR4", "DDR5" });

            cbxPcieVersion.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxPcieVersion.Items.Clear();
            cbxPcieVersion.Items.AddRange(new object[] { "—", "3.0", "4.0", "5.0" });
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

            cbxSupplier.SelectedValue = mb_.SupplierInn.HasValue ? mb_.SupplierInn.Value : 0;
        }

        private void FillControls(Motherboard mb)
        {
            txtName.Text = mb.Name ?? "";
            txtBrand.Text = mb.Brand ?? "";
            txtModel.Text = mb.Model ?? "";
            txtDesc.Text = mb.Description ?? "";
            txtPhoto.Text = mb.PhotoUrl ?? "";

            nudPrice.Value = mb.Price < 0 ? 0 : mb.Price;
            nudStock.Value = mb.StockQuantity < 0 ? 0 : mb.StockQuantity;
            chkAvailable.Checked = mb.IsAvailable;

            txtSocket.Text = mb.Socket ?? "";
            txtChipset.Text = mb.Chipset ?? "";
            txtFormFactor.Text = mb.FormFactor ?? "";

            cbxRamType.SelectedItem = string.IsNullOrWhiteSpace(mb.RamType) ? "—" : mb.RamType;
            cbxPcieVersion.SelectedItem = string.IsNullOrWhiteSpace(mb.PcieVersion) ? "—" : mb.PcieVersion;
        }
    }
}
