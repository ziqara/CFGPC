using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class UserCardControl : UserControl
    {
        private readonly UserCardVm vm_;

        public event EventHandler<UserCardVm> OrdersRequested;
        public event EventHandler<UserCardVm> EditRequested;
        public event EventHandler<UserCardVm> DeleteRequested;

        private PictureBox pbAvatar;
        private Label lblName;
        private Label lblEmail;
        private Label lblActive;
        private Button btnOrders;
        private Button btnEdit;
        private Button btnDelete;

        public UserCardControl(UserCardVm vm)
        {
            vm_ = vm;
            InitializeComponent();

            BuildUi();
            BindData();

            this.DoubleClick += (s, e) => OrdersRequested?.Invoke(this, vm_);
            foreach (Control c in this.Controls)
                c.DoubleClick += (s, e) => OrdersRequested?.Invoke(this, vm_);
        }

        private void BuildUi()
        {
            Width = 360;
            Height = 110;
            Margin = new Padding(10);
            BorderStyle = BorderStyle.FixedSingle;
            BackColor = Color.White;

            pbAvatar = new PictureBox
            {
                Width = 72,
                Height = 72,
                Left = 12,
                Top = 18,
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle
            };

            lblName = new Label
            {
                Left = 96,
                Top = 12,
                Width = 250,
                Height = 18,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            lblEmail = new Label
            {
                Left = 96,
                Top = 34,
                Width = 250,
                Height = 16,
                Font = new Font("Arial", 9, FontStyle.Regular)
            };

            lblActive = new Label
            {
                Left = 96,
                Top = 54,
                Width = 250,
                Height = 16,
                Font = new Font("Arial", 9, FontStyle.Regular)
            };

            btnOrders = new Button
            {
                Text = "Заказы",
                Width = 80,
                Height = 26,
                Left = 96,
                Top = 76,
                FlatStyle = FlatStyle.Flat
            };
            btnOrders.Click += (s, e) => OrdersRequested?.Invoke(this, vm_);

            btnEdit = new Button
            {
                Text = "Изм.",
                Width = 60,
                Height = 26,
                Left = 182,
                Top = 76,
                FlatStyle = FlatStyle.Flat
            };
            btnEdit.Click += (s, e) => EditRequested?.Invoke(this, vm_);

            btnDelete = new Button
            {
                Text = "Удалить",
                Width = 80,
                Height = 26,
                Left = 248,
                Top = 76,
                FlatStyle = FlatStyle.Flat
            };
            btnDelete.Click += (s, e) => DeleteRequested?.Invoke(this, vm_);

            Controls.Add(pbAvatar);
            Controls.Add(lblName);
            Controls.Add(lblEmail);
            Controls.Add(lblActive);
            Controls.Add(btnOrders);
            Controls.Add(btnEdit);
            Controls.Add(btnDelete);
        }

        private void BindData()
        {
            var u = vm_.User;

            lblName.Text = string.IsNullOrWhiteSpace(u.FullName) ? "(без имени)" : u.FullName;
            lblEmail.Text = u.Email;
            lblActive.Text = "Активные заказы: " + (vm_.HasActiveOrders ? "Да" : "Нет");

            pbAvatar.Image = BytesToImageOrDefault(u.Avatar);
        }

        public void ApplyTheme()
        {
            btnOrders.BackColor = ThemeColor.PrimaryColor;
            btnEdit.BackColor = ThemeColor.PrimaryColor;
            btnDelete.BackColor = ThemeColor.PrimaryColor;

            btnOrders.ForeColor = Color.White;
            btnEdit.ForeColor = Color.White;
            btnDelete.ForeColor = Color.White;

            btnOrders.FlatAppearance.BorderColor = ThemeColor.SecondaryColor;
            btnEdit.FlatAppearance.BorderColor = ThemeColor.SecondaryColor;
            btnDelete.FlatAppearance.BorderColor = ThemeColor.SecondaryColor;

            lblName.ForeColor = ThemeColor.PrimaryColor;
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
            Bitmap bmp = new Bitmap(72, 72);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                using (var b = new SolidBrush(Color.FromArgb(220, 220, 220)))
                    g.FillEllipse(b, 6, 6, 60, 60);
            }
            return bmp;
        }
    }
}