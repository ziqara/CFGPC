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

namespace WindowsFormsApp1.ComponentsForms
{
    public partial class EditCpuForm : Form
    {
        private readonly CpuService cpuService_;
        private readonly SupplierService supplierService_;
        private readonly Cpu cpu_;

        public EditCpuForm(CpuService cpuService, Cpu cpu)
        {
            InitializeComponent();

            cpuService_ = cpuService ?? throw new ArgumentNullException(nameof(cpuService));
            cpu_ = cpu ?? throw new ArgumentNullException(nameof(cpu));
            supplierService_ = new SupplierService(new MySqlSupplierRepository());

            this.Shown += EditCpuForm_Shown;
        }


        private void EditCpuForm_Shown(object sender, EventArgs e)
        {
            LoadSuppliersToCombo();
            FillControls(cpu_);
            ApplyDefaults();
        }

        private void ApplyDefaults()
        {
            nudPrice.DecimalPlaces = 2;
            nudPrice.Minimum = 0;

            nudStock.Minimum = 0;
            nudCores.Minimum = 0;
            nudTdp.Minimum = 0;
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

            cbxSupplier.SelectedValue = cpu_.SupplierInn.HasValue ? cpu_.SupplierInn.Value : 0;
        }

        private void FillControls(Cpu cpu)
        {
            txtName.Text = cpu.Name ?? "";
            txtBrand.Text = cpu.Brand ?? "";
            txtModel.Text = cpu.Model ?? "";
            txtDesc.Text = cpu.Description ?? "";
            txtPhoto.Text = cpu.PhotoUrl ?? "";

            nudPrice.Value = cpu.Price < 0 ? 0 : cpu.Price;
            nudStock.Value = cpu.StockQuantity < 0 ? 0 : cpu.StockQuantity;
            chkAvailable.Checked = cpu.IsAvailable;

            txtSocket.Text = cpu.Socket ?? "";
            nudCores.Value = cpu.Cores.HasValue ? cpu.Cores.Value : 0;
            nudTdp.Value = cpu.Tdp.HasValue ? cpu.Tdp.Value : 0;
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            int selectedInn = (int)cbxSupplier.SelectedValue;

            cpu_.Name = txtName.Text?.Trim();
            cpu_.Brand = string.IsNullOrWhiteSpace(txtBrand.Text) ? null : txtBrand.Text.Trim();
            cpu_.Model = string.IsNullOrWhiteSpace(txtModel.Text) ? null : txtModel.Text.Trim();
            cpu_.Description = string.IsNullOrWhiteSpace(txtDesc.Text) ? null : txtDesc.Text.Trim();
            cpu_.PhotoUrl = string.IsNullOrWhiteSpace(txtPhoto.Text) ? null : txtPhoto.Text.Trim();

            cpu_.Price = nudPrice.Value;
            cpu_.StockQuantity = (int)nudStock.Value;
            cpu_.IsAvailable = chkAvailable.Checked;

            cpu_.SupplierInn = selectedInn == 0 ? (int?)null : selectedInn;

            cpu_.Socket = string.IsNullOrWhiteSpace(txtSocket.Text) ? null : txtSocket.Text.Trim();
            cpu_.Cores = nudCores.Value == 0 ? (int?)null : (int)nudCores.Value;
            cpu_.Tdp = nudTdp.Value == 0 ? (int?)null : (int)nudTdp.Value;

            string result = cpuService_.UpdateCpu(cpu_);
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
