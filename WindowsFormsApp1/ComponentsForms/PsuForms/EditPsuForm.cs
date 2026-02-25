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
    public partial class EditPsuForm : Form
    {
        private readonly PsuService psuService_;
        private readonly SupplierService supplierService_;
        private readonly Psu psu_;

        public EditPsuForm(PsuService psuService, Psu psu)
        {
            InitializeComponent();

            psuService_ = psuService ?? throw new ArgumentNullException(nameof(psuService));
            psu_ = psu ?? throw new ArgumentNullException(nameof(psu));
            supplierService_ = new SupplierService(new MySqlSupplierRepository());

            this.Shown += EditPsuForm_Shown;
        }

        private void EditPsuForm_Shown(object sender, EventArgs e)
        {
            LoadSuppliersToCombo();
            SetupEfficiencyCombo();
            ApplyDefaults();
            FillControls(psu_);
        }

        private void ApplyDefaults()
        {
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

            cbxSupplier.SelectedValue = psu_.SupplierInn.HasValue ? psu_.SupplierInn.Value : 0;
        }

        private void FillControls(Psu p)
        {
            txtName.Text = p.Name ?? "";
            txtBrand.Text = p.Brand ?? "";
            txtModel.Text = p.Model ?? "";
            txtDesc.Text = p.Description ?? "";
            txtPhoto.Text = p.PhotoUrl ?? "";

            nudPrice.Value = p.Price < 0 ? 0 : p.Price;
            nudStock.Value = p.StockQuantity < 0 ? 0 : p.StockQuantity;
            chkAvailable.Checked = p.IsAvailable;

            nudWattage.Value = p.Wattage.HasValue ? p.Wattage.Value : 0;
            cbxEfficiency.SelectedItem = string.IsNullOrWhiteSpace(p.Efficiency) ? "—" : p.Efficiency;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int selectedInn = (int)cbxSupplier.SelectedValue;

            string eff = cbxEfficiency.SelectedItem?.ToString();
            if (eff == "—") eff = null;

            psu_.Name = txtName.Text?.Trim();
            psu_.Brand = string.IsNullOrWhiteSpace(txtBrand.Text) ? null : txtBrand.Text.Trim();
            psu_.Model = string.IsNullOrWhiteSpace(txtModel.Text) ? null : txtModel.Text.Trim();
            psu_.Description = string.IsNullOrWhiteSpace(txtDesc.Text) ? null : txtDesc.Text.Trim();
            psu_.PhotoUrl = string.IsNullOrWhiteSpace(txtPhoto.Text) ? null : txtPhoto.Text.Trim();

            psu_.Price = nudPrice.Value;
            psu_.StockQuantity = (int)nudStock.Value;
            psu_.IsAvailable = chkAvailable.Checked;

            psu_.SupplierInn = selectedInn == 0 ? (int?)null : selectedInn;

            psu_.Wattage = nudWattage.Value == 0 ? (int?)null : (int)nudWattage.Value;
            psu_.Efficiency = eff;

            string result = psuService_.UpdatePsu(psu_);
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
