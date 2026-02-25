using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using DDMLib;

namespace WindowsFormsApp1.ComponentsForms.CaseForms
{
    public partial class EditCaseForm : Form
    {
        private readonly CaseService caseService_;
        private readonly SupplierService supplierService_;
        private readonly Case case_;

        public EditCaseForm(CaseService caseService, Case item)
        {
            InitializeComponent();

            caseService_ = caseService ?? throw new ArgumentNullException(nameof(caseService));
            case_ = item ?? throw new ArgumentNullException(nameof(item));
            supplierService_ = new SupplierService(new MySqlSupplierRepository());

            this.Shown += EditCaseForm_Shown;
        }

        private void EditCaseForm_Shown(object sender, EventArgs e)
        {
            LoadSuppliersToCombo();
            SetupSizeCombo();
            ApplyDefaults();
            FillControls(case_);
        }

        private void ApplyDefaults()
        {
            nudPrice.DecimalPlaces = 2;
            nudPrice.Minimum = 0;

            nudStock.Minimum = 0;
        }

        private void SetupSizeCombo()
        {
            cbxSize.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxSize.Items.Clear();
            cbxSize.Items.AddRange(new object[] { "full_tower", "mid_tower", "compact" });
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

            cbxSupplier.SelectedValue = case_.SupplierInn.HasValue ? case_.SupplierInn.Value : 0;
        }

        private void FillControls(Case c)
        {
            txtName.Text = c.Name ?? "";
            txtBrand.Text = c.Brand ?? "";
            txtModel.Text = c.Model ?? "";
            txtDesc.Text = c.Description ?? "";
            txtPhoto.Text = c.PhotoUrl ?? "";
            txtFormFactor.Text = c.FormFactor ?? "";

            nudPrice.Value = c.Price < 0 ? 0 : c.Price;
            nudStock.Value = c.StockQuantity < 0 ? 0 : c.StockQuantity;
            chkAvailable.Checked = c.IsAvailable;

            cbxSize.SelectedItem = string.IsNullOrWhiteSpace(c.Size) ? "mid_tower" : c.Size;
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            int selectedInn = (int)cbxSupplier.SelectedValue;

            case_.Name = txtName.Text?.Trim();
            case_.Brand = string.IsNullOrWhiteSpace(txtBrand.Text) ? null : txtBrand.Text.Trim();
            case_.Model = string.IsNullOrWhiteSpace(txtModel.Text) ? null : txtModel.Text.Trim();
            case_.Description = string.IsNullOrWhiteSpace(txtDesc.Text) ? null : txtDesc.Text.Trim();
            case_.PhotoUrl = string.IsNullOrWhiteSpace(txtPhoto.Text) ? null : txtPhoto.Text.Trim();

            case_.Price = nudPrice.Value;
            case_.StockQuantity = (int)nudStock.Value;
            case_.IsAvailable = chkAvailable.Checked;

            case_.SupplierInn = selectedInn == 0 ? (int?)null : selectedInn;

            case_.FormFactor = string.IsNullOrWhiteSpace(txtFormFactor.Text) ? null : txtFormFactor.Text.Trim();
            case_.Size = cbxSize.SelectedItem?.ToString();

            string result = caseService_.UpdateCase(case_);
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
