using DDMLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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

            ShowAvatar(user_.Avatar);
        }

        private void ShowAvatar(byte[] avatarBytes)
        {
            // если PictureBox ещё не добавлен на форму — будет NullReference
            if (pbAvatar == null) return;

            // очищаем старое изображение чтобы не было утечек
            if (pbAvatar.Image != null)
            {
                var old = pbAvatar.Image;
                pbAvatar.Image = null;
                old.Dispose();
            }

            pbAvatar.Image = BytesToImageOrDefault(avatarBytes);
        }

        private Image BytesToImageOrDefault(byte[] bytes)
        {
            try
            {
                if (bytes == null || bytes.Length == 0)
                    return CreateDefaultAvatar();

                using (var ms = new MemoryStream(bytes))
                    return Image.FromStream(ms);
            }
            catch
            {
                return CreateDefaultAvatar();
            }
        }

        private Image CreateDefaultAvatar()
        {
            Bitmap bmp = new Bitmap(120, 120);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                using (var b = new SolidBrush(Color.FromArgb(220, 220, 220)))
                    g.FillEllipse(b, 10, 10, 100, 100);
            }
            return bmp;
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
            if (btnCancel != null && btnCancel.Visible)
            {
                btnCancel.Focus();
            }

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
        }

        private void btnChangeAvatar_Click(object sender, EventArgs e)
        {
            if (openFileDialog1 == null)
            {
                MessageBox.Show("Добавь OpenFileDialog на форму (openFileDialog1).");
                return;
            }

            openFileDialog1.Filter = "Изображения|*.png;*.jpg;*.jpeg;*.bmp;*.gif";
            openFileDialog1.Title = "Выберите аватар";

            if (openFileDialog1.ShowDialog(this) != DialogResult.OK)
                return;

            try
            {
                byte[] bytes = File.ReadAllBytes(openFileDialog1.FileName);

                // вызываем твой репозиторийный метод через IUserRepository напрямую
                // В AdminUserService пока нет UpdateAvatar — добавим ниже
                string res = service_.UpdateAvatar(user_.Email, bytes);

                if (!string.IsNullOrEmpty(res))
                {
                    MessageBox.Show(res, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                user_.Avatar = bytes;
                ShowAvatar(user_.Avatar);
                MessageBox.Show("Аватар обновлён.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось обновить аватар: " + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
