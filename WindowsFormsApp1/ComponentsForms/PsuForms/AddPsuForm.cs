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
            txtPhoto.ReadOnly = true;
            this.Shown += AddPsuForm_Shown;
        }

        private string GetSavedPhotoPathForDb()
        {
            if (string.IsNullOrWhiteSpace(txtPhoto.Text))
                return null;

            var t = txtPhoto.Text.Trim();

            if (t.StartsWith("/Resources/", StringComparison.OrdinalIgnoreCase))
                return t;

            return PhotoStorage.SavePhotoToResources(t); // вернет /Resources/xxx.jpg и скопирует файл
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string savedPhoto = null;

            try
            {
                savedPhoto = GetSavedPhotoPathForDb();
                txtPhoto.Text = savedPhoto ?? "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось сохранить изображение.\n\n" + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Psu p = BuildFromControls(savedPhoto);

            string result = psuService_.CreatePsu(p);
            if (!string.IsNullOrEmpty(result))
            {
                MessageBox.Show(result, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private Psu BuildFromControls(string savedPhotoPath)
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

                PhotoUrl = string.IsNullOrWhiteSpace(savedPhotoPath) ? null : savedPhotoPath,

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

        private void btnBrowsePhoto_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Выберите изображение";
                ofd.Filter = "Изображения|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.webp";
                ofd.Multiselect = false;
                ofd.CheckFileExists = true;

                if (ofd.ShowDialog(this) == DialogResult.OK)
                    txtPhoto.Text = ofd.FileName; // временно абсолютный путь
            }
        }
    }
}
