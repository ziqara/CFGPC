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
    public partial class EditStorageForm : Form
    {
        private readonly StorageService storageService_;
        private readonly SupplierService supplierService_;
        private readonly Storage st_;

        public EditStorageForm(StorageService storageService, Storage st)
        {
            InitializeComponent();

            storageService_ = storageService ?? throw new ArgumentNullException(nameof(storageService));
            st_ = st ?? throw new ArgumentNullException(nameof(st));
            supplierService_ = new SupplierService(new MySqlSupplierRepository());

            this.Shown += EditStorageForm_Shown;
        }

        private void EditStorageForm_Shown(object sender, EventArgs e)
        {
            LoadSuppliersToCombo();
            SetupInterfaceCombo();
            ApplyDefaults();
            FillControls(st_);
        }

        private void ApplyDefaults()
        {
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

            cbxSupplier.SelectedValue = st_.SupplierInn.HasValue ? st_.SupplierInn.Value : 0;
        }

        private void FillControls(Storage s)
        {
            txtName.Text = s.Name ?? "";
            txtBrand.Text = s.Brand ?? "";
            txtModel.Text = s.Model ?? "";
            txtDesc.Text = s.Description ?? "";
            txtPhoto.Text = s.PhotoUrl ?? "";

            nudPrice.Value = s.Price < 0 ? 0 : s.Price;
            nudStock.Value = s.StockQuantity < 0 ? 0 : s.StockQuantity;
            chkAvailable.Checked = s.IsAvailable;

            cbxInterface.SelectedItem = string.IsNullOrWhiteSpace(s.Interface) ? "—" : s.Interface;
            nudCapacityGb.Value = s.CapacityGb.HasValue ? s.CapacityGb.Value : 0;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int selectedInn = (int)cbxSupplier.SelectedValue;

            string iface = cbxInterface.SelectedItem?.ToString();
            if (iface == "—") iface = null;

            st_.Name = txtName.Text?.Trim();
            st_.Brand = string.IsNullOrWhiteSpace(txtBrand.Text) ? null : txtBrand.Text.Trim();
            st_.Model = string.IsNullOrWhiteSpace(txtModel.Text) ? null : txtModel.Text.Trim();
            st_.Description = string.IsNullOrWhiteSpace(txtDesc.Text) ? null : txtDesc.Text.Trim();
            st_.PhotoUrl = string.IsNullOrWhiteSpace(txtPhoto.Text) ? null : txtPhoto.Text.Trim();

            st_.Price = nudPrice.Value;
            st_.StockQuantity = (int)nudStock.Value;
            st_.IsAvailable = chkAvailable.Checked;

            st_.SupplierInn = selectedInn == 0 ? (int?)null : selectedInn;

            st_.Interface = iface;
            st_.CapacityGb = nudCapacityGb.Value == 0 ? (int?)null : (int)nudCapacityGb.Value;

            string result = storageService_.UpdateStorage(st_);
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
