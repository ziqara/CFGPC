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

namespace WindowsFormsApp1.ComponentsForms.GpuForms
{
    public partial class AddGpuForm : Form
    {
        private readonly GpuService gpuService_;
        private readonly SupplierService supplierService_;

        public AddGpuForm(GpuService gpuService)
        {
            InitializeComponent();

            gpuService_ = gpuService ?? throw new ArgumentNullException(nameof(gpuService));
            supplierService_ = new SupplierService(new MySqlSupplierRepository());

            this.Shown += AddGpuForm_Shown;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Gpu gpu = BuildFromControls();

            string result = gpuService_.CreateGpu(gpu);
            if (!string.IsNullOrEmpty(result))
            {
                MessageBox.Show(result, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private Gpu BuildFromControls()
        {
            int selectedInn = (int)cbxSupplier.SelectedValue;

            string pcie = cbxPcieVersion.SelectedItem?.ToString();
            if (pcie == "—") pcie = null;

            return new Gpu
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

                PcieVersion = pcie,
                Tdp = nudTdp.Value == 0 ? (int?)null : (int)nudTdp.Value,
                VramGb = nudVramGb.Value == 0 ? (int?)null : (int)nudVramGb.Value
            };
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void AddGpuForm_Shown(object sender, EventArgs e)
        {
            LoadSuppliersToCombo();
            SetupPcieCombo();
            ApplyDefaults();
        }

        private void ApplyDefaults()
        {
            chkAvailable.Checked = true;

            nudPrice.DecimalPlaces = 2;
            nudPrice.Minimum = 0;

            nudStock.Minimum = 0;
            nudTdp.Minimum = 0;
            nudVramGb.Minimum = 0;
        }

        private void SetupPcieCombo()
        {
            cbxPcieVersion.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxPcieVersion.Items.Clear();
            cbxPcieVersion.Items.AddRange(new object[] { "—", "3.0", "4.0" });
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
    }
}
