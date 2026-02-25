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

namespace WindowsFormsApp1.ComponentsForms.CaseForms
{
    public partial class AddCaseForm : Form
    {
        private readonly CaseService caseService_;
        private readonly SupplierService supplierService_;

        public AddCaseForm(CaseService caseService)
        {
            InitializeComponent();

            caseService_ = caseService ?? throw new ArgumentNullException(nameof(caseService));
            supplierService_ = new SupplierService(new MySqlSupplierRepository());

            this.Shown += AddCaseForm_Shown;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Case item = BuildFromControls();

            string result = caseService_.CreateCase(item);
            if (!string.IsNullOrEmpty(result))
            {
                MessageBox.Show(result, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private Case BuildFromControls()
        {
            int selectedInn = (int)cbxSupplier.SelectedValue;

            return new Case
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

                FormFactor = string.IsNullOrWhiteSpace(txtFormFactor.Text) ? null : txtFormFactor.Text.Trim(),
                Size = cbxSize.SelectedItem?.ToString()
            };
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void AddCaseForm_Shown(object sender, EventArgs e)
        {
            LoadSuppliersToCombo();
            SetupSizeCombo();
            ApplyDefaults();
        }

        private void ApplyDefaults()
        {
            chkAvailable.Checked = true;

            nudPrice.DecimalPlaces = 2;
            nudPrice.Minimum = 0;

            nudStock.Minimum = 0;
        }

        private void SetupSizeCombo()
        {
            cbxSize.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxSize.Items.Clear();
            cbxSize.Items.AddRange(new object[] { "full_tower", "mid_tower", "compact" });
            cbxSize.SelectedItem = "mid_tower"; // дефолт
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
