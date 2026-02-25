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
    public partial class AddStorageForm : Form
    {
        private readonly StorageService storageService_;
        private readonly SupplierService supplierService_;

        public AddStorageForm(StorageService storageService)
        {
            InitializeComponent();

            storageService_ = storageService ?? throw new ArgumentNullException(nameof(storageService));
            supplierService_ = new SupplierService(new MySqlSupplierRepository());

            this.Shown += AddStorageForm_Shown;
        }

        private void AddStorageForm_Shown(object sender, EventArgs e)
        {
            LoadSuppliersToCombo();
            SetupInterfaceCombo();
            ApplyDefaults();
        }

        private void ApplyDefaults()
        {
            chkAvailable.Checked = true;

            nudPrice.DecimalPlaces = 2;
            nudPrice.Minimum = 0;

            nudStock.Minimum = 0;
            nudCapacityGb.Minimum = 0;
        }

        private void SetupInterfaceCombo()
        {
            cbxInterface.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxInterface.Items.Clear();
            cbxInterface.Items.AddRange(new object[] { "—", "SATA", "NVMe" });
            cbxInterface.SelectedIndex = 0;
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

        private Storage BuildFromControls()
        {
            int selectedInn = (int)cbxSupplier.SelectedValue;

            string iface = cbxInterface.SelectedItem?.ToString();
            if (iface == "—") iface = null;

            return new Storage
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

                Interface = iface,
                CapacityGb = nudCapacityGb.Value == 0 ? (int?)null : (int)nudCapacityGb.Value
            };
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Storage st = BuildFromControls();

            string result = storageService_.CreateStorage(st);
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
