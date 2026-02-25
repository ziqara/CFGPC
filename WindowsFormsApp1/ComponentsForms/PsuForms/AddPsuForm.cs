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

namespace WindowsFormsApp1.ComponentsForms.PsuForms
{
    public partial class AddPsuForm : Form
    {
        private readonly PsuService psuService_;
        private readonly SupplierService supplierService_;

        public AddPsuForm(PsuService psuService)
        {
            InitializeComponent();

            psuService_ = psuService ?? throw new ArgumentNullException(nameof(psuService));
            supplierService_ = new SupplierService(new MySqlSupplierRepository());

            this.Shown += AddPsuForm_Shown;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Psu p = BuildFromControls();

            string result = psuService_.CreatePsu(p);
            if (!string.IsNullOrEmpty(result))
            {
                MessageBox.Show(result, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private Psu BuildFromControls()
        {
            int selectedInn = (int)cbxSupplier.SelectedValue;

            string eff = cbxEfficiency.SelectedItem?.ToString();
            if (eff == "—") eff = null;

            return new Psu
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

                Wattage = nudWattage.Value == 0 ? (int?)null : (int)nudWattage.Value,
                Efficiency = eff
            };
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void AddPsuForm_Shown(object sender, EventArgs e)
        {
            LoadSuppliersToCombo();
            SetupEfficiencyCombo();
            ApplyDefaults();
        }

        private void ApplyDefaults()
        {
            chkAvailable.Checked = true;

            nudPrice.DecimalPlaces = 2;
            nudPrice.Minimum = 0;

            nudStock.Minimum = 0;
            nudWattage.Minimum = 0;
        }

        private void SetupEfficiencyCombo()
        {
            cbxEfficiency.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxEfficiency.Items.Clear();
            cbxEfficiency.Items.AddRange(new object[]
            {
                "—", "80+ Bronze", "80+ Gold", "80+ Platinum"
            });
            cbxEfficiency.SelectedIndex = 0;
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

        private void AddPsuForm_Load(object sender, EventArgs e)
        {

        }
    }
}
