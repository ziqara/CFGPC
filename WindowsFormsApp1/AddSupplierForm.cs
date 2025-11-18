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
            string innText = txtInn.Text?.Trim();
            if (!int.TryParse(innText, out int inn))
            {
                MessageBox.Show(
                    "ИНН должен быть целым числом.",
                    "Ошибка ввода",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // 2. Собираем объект Supplier из полей формы
            var supplier = new Supplier(inn)
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
                // 3. Бизнес-валидация + проверки уникальности в сервисе
                string errors = service_.CreateSupplier(supplier);

                if (!string.IsNullOrEmpty(errors))
                {
                    // Есть ошибки — показываем все разом
                    MessageBox.Show(
                        errors,
                        "Ошибки заполнения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // 4. Успех
                MessageBox.Show(
                    "Поставщик добавлен.",
                    "Успех",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // Сообщаем вызывающей форме, что всё ок, и закрываемся
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                // Ошибка на уровне БД
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
    }
}
