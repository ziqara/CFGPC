using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DDMLib;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1
{
    public partial class AddSupplierForm : Form
    {
        private readonly SupplierService service_;

        public AddSupplierForm()
        {
            InitializeComponent();
            service_ = new SupplierService(new MySqlSupplierRepository());
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int inn = 0;
            string innText = txtInn.Text;
            int.TryParse(innText, out inn);

            Supplier supplier = new Supplier(inn)
            {
                Name = txtName.Text?.Trim(),
                ContactEmail = txtEmail.Text?.Trim(),
                Phone = string.IsNullOrWhiteSpace(txtPhone.Text)
                            ? null
                            : txtPhone.Text.Trim(),
                Address = string.IsNullOrWhiteSpace(txtAddress.Text)
                            ? null
                            : txtAddress.Text.Trim()
            };

            try
            {
                string errors = service_.CreateSupplier(supplier);

                if (!string.IsNullOrEmpty(errors))
                {
                    MessageBox.Show(
                        errors,
                        "Ошибки заполнения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show(
                    "Поставщик добавлен.",
                    "Успех",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            { 
                MessageBox.Show(
                    "Вероятно, проблемы в соединении с БД: " + ex.Message,
                    "Ошибка БД",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void txtPhone_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPhone.Text))
            {
                opcPhone.Visible = false;
            }
            else
            {
                opcPhone.Visible = true;
            }
        }

        private void txtAddress_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtAddress.Text))
            {
                opcAddres.Visible = false;
            }
            else
            {
                opcAddres.Visible = true;
            }
        }
    }
}
