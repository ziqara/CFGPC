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
    public partial class AddCpuForm : Form
    {
        private readonly CpuService cpuService_;
        private readonly SupplierService supplierService_;

        public AddCpuForm(CpuService cpuService)
        {
            InitializeComponent();

            cpuService_ = cpuService ?? throw new ArgumentNullException(nameof(cpuService));
            supplierService_ = new SupplierService(new MySqlSupplierRepository());
            txtPhoto.ReadOnly = true;
            this.Shown += AddCpuForm_Shown;
        }

        private void AddCpuForm_Load(object sender, EventArgs e)
        {

        }

        private void AddCpuForm_Shown(object sender, EventArgs e)
        {
            LoadSuppliersToCombo();
            ApplyDefaults();
        }

        private void ApplyDefaults()
        {
            chkAvailable.Checked = true;

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
            cbxSupplier.SelectedIndex = 0;
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

            Cpu cpu = BuildCpuFromControls(savedPhoto);

            string result = cpuService_.CreateCpu(cpu);
            if (!string.IsNullOrEmpty(result))
            {
                MessageBox.Show(result, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private Cpu BuildCpuFromControls(string savedPhotoPath)
        {
            int selectedInn = (int)cbxSupplier.SelectedValue;

            return new Cpu
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

                Socket = string.IsNullOrWhiteSpace(txtSocket.Text) ? null : txtSocket.Text.Trim(),
                Cores = nudCores.Value == 0 ? (int?)null : (int)nudCores.Value,
                Tdp = nudTdp.Value == 0 ? (int?)null : (int)nudTdp.Value
            };
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
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
                    txtPhoto.Text = ofd.FileName;
            }
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
    }
}
