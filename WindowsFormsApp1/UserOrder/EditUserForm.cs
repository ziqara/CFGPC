using DDMLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1.UserOrder
{
    public partial class EditUserForm : Form
    {
        private readonly AdminUserService service_;
        private readonly User user_;

        public EditUserForm(AdminUserService service, User user)
        {
            InitializeComponent();
            service_ = service;
            user_ = user;

            this.Shown += EditUserForm_Shown;
            ThemeColor.ThemeChanged += ApplyTheme;
        }

        private void EditUserForm_Load(object sender, EventArgs e)
        {
            txtEmail.Text = user_.Email;
            txtEmail.ReadOnly = true;

            txtFullName.Text = user_.FullName ?? "";
            txtPhone.Text = user_.Phone ?? "";
            txtAddress.Text = user_.Address ?? "";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            user_.FullName = txtFullName.Text;
            user_.Phone = txtPhone.Text;
            user_.Address = txtAddress.Text;

            string result = service_.UpdateProfile(user_);
            if (string.IsNullOrEmpty(result))
            {
                DialogResult = DialogResult.OK;
                Close();
                return;
            }

            MessageBox.Show(result, "Ошибка сохранения",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void EditUserForm_Shown(object sender, EventArgs e)
        {
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            foreach (Control btns in this.Controls)
            {
                if (btns.GetType() == typeof(Button))
                {
                    Button btn = (Button)btns;
                    btn.BackColor = ThemeColor.PrimaryColor;
                    btn.ForeColor = Color.White;
                    btn.FlatAppearance.BorderColor = ThemeColor.SecondaryColor;
                }
            }

            if (labelTitle != null)
                labelTitle.ForeColor = ThemeColor.PrimaryColor;
        }
    }
}
