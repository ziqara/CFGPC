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
    public partial class EditGpuForm : Form
    {
        private readonly GpuService gpuService_;
        private readonly SupplierService supplierService_;
        private readonly Gpu gpu_;

        public EditGpuForm(GpuService gpuService, Gpu gpu)
        {
            InitializeComponent();

            gpuService_ = gpuService ?? throw new ArgumentNullException(nameof(gpuService));
            gpu_ = gpu ?? throw new ArgumentNullException(nameof(gpu));
            supplierService_ = new SupplierService(new MySqlSupplierRepository());

            this.Shown += EditGpuForm_Shown;
        }

        private void EditGpuForm_Shown(object sender, EventArgs e)
        {
            LoadSuppliersToCombo();
            SetupPcieCombo();
            ApplyDefaults();
            FillControls(gpu_);
        }

        private void ApplyDefaults()
        {
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

            cbxSupplier.SelectedValue = gpu_.SupplierInn.HasValue ? gpu_.SupplierInn.Value : 0;
        }

        private void FillControls(Gpu g)
        {
            txtName.Text = g.Name ?? "";
            txtBrand.Text = g.Brand ?? "";
            txtModel.Text = g.Model ?? "";
            txtDesc.Text = g.Description ?? "";
            txtPhoto.Text = g.PhotoUrl ?? "";

            nudPrice.Value = g.Price < 0 ? 0 : g.Price;
            nudStock.Value = g.StockQuantity < 0 ? 0 : g.StockQuantity;
            chkAvailable.Checked = g.IsAvailable;

            cbxPcieVersion.SelectedItem = string.IsNullOrWhiteSpace(g.PcieVersion) ? "—" : g.PcieVersion;

            nudTdp.Value = g.Tdp.HasValue ? g.Tdp.Value : 0;
            nudVramGb.Value = g.VramGb.HasValue ? g.VramGb.Value : 0;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int selectedInn = (int)cbxSupplier.SelectedValue;

            string pcie = cbxPcieVersion.SelectedItem?.ToString();
            if (pcie == "—") pcie = null;

            gpu_.Name = txtName.Text?.Trim();
            gpu_.Brand = string.IsNullOrWhiteSpace(txtBrand.Text) ? null : txtBrand.Text.Trim();
            gpu_.Model = string.IsNullOrWhiteSpace(txtModel.Text) ? null : txtModel.Text.Trim();
            gpu_.Description = string.IsNullOrWhiteSpace(txtDesc.Text) ? null : txtDesc.Text.Trim();
            gpu_.PhotoUrl = string.IsNullOrWhiteSpace(txtPhoto.Text) ? null : txtPhoto.Text.Trim();

            gpu_.Price = nudPrice.Value;
            gpu_.StockQuantity = (int)nudStock.Value;
            gpu_.IsAvailable = chkAvailable.Checked;

            gpu_.SupplierInn = selectedInn == 0 ? (int?)null : selectedInn;

            gpu_.PcieVersion = pcie;
            gpu_.Tdp = nudTdp.Value == 0 ? (int?)null : (int)nudTdp.Value;
            gpu_.VramGb = nudVramGb.Value == 0 ? (int?)null : (int)nudVramGb.Value;

            string result = gpuService_.UpdateGpu(gpu_);
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
