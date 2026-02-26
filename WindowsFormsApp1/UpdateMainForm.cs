using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.ComponentsForms;
using WindowsFormsApp1.ConfigForms;
using WindowsFormsApp1.Orders;
using WindowsFormsApp1.UserOrder;

namespace WindowsFormsApp1
{
    public partial class UpdateMainForm : Form
    {
        private Button currentButton;
        private Random random;
        private int tempIndex;
        private Form activeForm;
        public UpdateMainForm()
        {
            InitializeComponent();
            random = new Random();
            btnCloseChildForm.Visible = false;
            logobox.Visible = false;
            //this.Text = string.Empty;
            //this.ControlBox = false;
            this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
            this.lblTitile.MouseDown += panelTitle_MouseDown;
        }

        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int msg, int wParam, int lParam);

        private Color SelectThemeColor()
        {
            int index = random.Next(ThemeColor.ColorList.Count);
            while (tempIndex == index)
            {
                index = random.Next(ThemeColor.ColorList.Count);
            }
            tempIndex = index;
            string color = ThemeColor.ColorList[index];
            return ColorTranslator.FromHtml(color);
        }

        private void ActivateButton(object btnSender)
        {
            if (btnSender != null && currentButton != (Button)btnSender)
            {
                DisableButton();

                Color color = SelectThemeColor();
                currentButton = (Button)btnSender;

                currentButton.BackColor = color;
                currentButton.ForeColor = Color.White;
                currentButton.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular);

                panelTitle.BackColor = color;
                panelLogo.BackColor = ThemeColor.ChangeColorBrightness(color, -0.4);
                ThemeColor.SetTheme(color, ThemeColor.ChangeColorBrightness(color, -0.5));
                btnCloseChildForm.Visible = true;
                logobox.Visible = true;
            }
        }

        private void DisableButton()
        {
            foreach (Control previosBtn in panelMenu.Controls)
            {
                if(previosBtn.GetType() == typeof(Button))
                {
                    previosBtn.BackColor = Color.FromArgb(73, 80, 87);
                    previosBtn.ForeColor = Color.White;
                    previosBtn.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular);
                }
            }
        }

        private void OpenChildForm(Form childform, object btnSender)
        {
             if(activeForm != null)
            {
                activeForm.Close();
            }
            ActivateButton(btnSender);
            activeForm = childform;
            childform.TopLevel = false;
            childform.FormBorderStyle = FormBorderStyle.None;
            childform.Dock = DockStyle.Fill;
            this.panelDekstopPanel.Controls.Add(childform);
            this.panelDekstopPanel.Tag = childform;
            childform.BringToFront();
            childform.Show();

            lblTitile.Text = childform.Text;
        }

        private void btnSupplier_Click_1(object sender, EventArgs e)
        {
            OpenChildForm(new SupplierForm(), sender);
        }

        private void btnOrders_Click_2(object sender, EventArgs e)
        {
            OpenChildForm(new AllOrdersCardsForm(), sender);
        }

        private void btnConfig_Click_1(object sender, EventArgs e)
        {
            OpenChildForm(new BuildsForm(), sender);
        }

        private void btnComponent_Click(object sender, EventArgs e)
        {
            OpenChildForm(new MainFormForComponents(), sender);
        }

        private void btnClients_Click_1(object sender, EventArgs e)
        {
            OpenChildForm(new UsersCardsForm(), sender);
        }

        private void btnWarrious_Click(object sender, EventArgs e)
        {
            OpenChildForm(new MainForm(), sender);
        }

        private void btnReviews_Click_1(object sender, EventArgs e)
        {
            OpenChildForm(new MainForm(), sender);
        }

        private void btnCloseChildForm_Click(object sender, EventArgs e)
        {
            if (activeForm != null)
            {
                activeForm.Close();
                Reset();
            }
        }

        private void Reset()
        {
            DisableButton();
            lblTitile.Text = "Главная";
            panelTitle.BackColor = Color.FromArgb(73, 80, 87);
            panelLogo.BackColor = Color.FromArgb(33, 37, 41);
            currentButton = null;
            btnCloseChildForm.Visible = false;
            logobox.Visible = false;
        }

        private void panelTitle_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
    }
}
