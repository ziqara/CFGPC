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

namespace WindowsFormsApp1
{
    public partial class EditSupplierForm : Form
    {
        private readonly SupplierService service_;
        private readonly Supplier supplier_;

        public EditSupplierForm(SupplierService service, Supplier supplier)
        {
            InitializeComponent();

            service_ = service ?? throw new ArgumentNullException(nameof(service));
            supplier_ = supplier ?? throw new ArgumentNullException(nameof(supplier));

            FillFromSupplier();
        }

        private void FillFromSupplier()
        {
            txtInn.Text = supplier_.Inn.ToString();
            txtName.Text = supplier_.Name;
            txtEmail.Text = supplier_.ContactEmail;
            txtPhone.Text = supplier_.Phone ?? "";
            txtAddress.Text = supplier_.Address ?? "";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            supplier_.Name = txtName.Text.Trim();
            supplier_.ContactEmail = txtEmail.Text.Trim();

            string phone = txtPhone.Text.Trim();
            supplier_.Phone = string.IsNullOrWhiteSpace(phone) ? null : phone;

            string addr = txtAddress.Text.Trim();
            supplier_.Address = string.IsNullOrWhiteSpace(addr) ? null : addr;

            try
            {
                string error = service_.UpdateSupplier(supplier_);
                if (!string.IsNullOrEmpty(error))
                {
                    MessageBox.Show(error, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show("Изменения сохранены", "Успешно",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Вероятно, проблемы в соединении с БД:\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
           this.Close();
        }
    }
}
