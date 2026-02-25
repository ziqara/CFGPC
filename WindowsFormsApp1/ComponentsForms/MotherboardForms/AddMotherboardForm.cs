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
    public partial class AddMotherboardForm : Form
    {
        private readonly MotherboardService mbService_;
        private readonly SupplierService supplierService_;

        public AddMotherboardForm(MotherboardService mbService)
        {
            InitializeComponent();

            mbService_ = mbService ?? throw new ArgumentNullException(nameof(mbService));
            supplierService_ = new SupplierService(new MySqlSupplierRepository());

            this.Shown += AddMotherboardForm_Shown;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Motherboard mb = BuildFromControls();

            string result = mbService_.CreateMotherboard(mb);
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

        private void AddMotherboardForm_Shown(object sender, EventArgs e)
        {
            LoadSuppliersToCombo();
            SetupRamAndPcieCombos();
            ApplyDefaults();
        }
        private void ApplyDefaults()
        {
            chkAvailable.Checked = true;

            nudPrice.DecimalPlaces = 2;
            nudPrice.Minimum = 0;

            nudStock.Minimum = 0;
        }

        private void SetupRamAndPcieCombos()
        {
            cbxRamType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxRamType.Items.Clear();
            cbxRamType.Items.AddRange(new object[] { "—", "DDR4", "DDR5" });
            cbxRamType.SelectedIndex = 0;

            cbxPcieVersion.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxPcieVersion.Items.Clear();
            cbxPcieVersion.Items.AddRange(new object[] { "—", "3.0", "4.0", "5.0" });
            cbxPcieVersion.SelectedIndex = 0;
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

        private Motherboard BuildFromControls()
        {
            int selectedInn = (int)cbxSupplier.SelectedValue;

            string ram = cbxRamType.SelectedItem?.ToString();
            if (ram == "—") ram = null;

            string pcie = cbxPcieVersion.SelectedItem?.ToString();
            if (pcie == "—") pcie = null;

            return new Motherboard
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

                Socket = string.IsNullOrWhiteSpace(txtSocket.Text) ? null : txtSocket.Text.Trim(),
                Chipset = string.IsNullOrWhiteSpace(txtChipset.Text) ? null : txtChipset.Text.Trim(),
                FormFactor = string.IsNullOrWhiteSpace(txtFormFactor.Text) ? null : txtFormFactor.Text.Trim(),
                RamType = ram,
                PcieVersion = pcie
            };
        }
    }
}
