using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        }

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
                panelLogo.BackColor = ThemeColor.ChangeColorBrightness(color, -0.5);
                ThemeColor.SetTheme(color, ThemeColor.ChangeColorBrightness(color, -0.5));
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
                    previosBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
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
            ActivateButton(sender);
        }

        private void btnConfig_Click_1(object sender, EventArgs e)
        {
            ActivateButton(sender);
        }

        private void btnComponent_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);
        }

        private void btnClients_Click_1(object sender, EventArgs e)
        {
            ActivateButton(sender);
        }

        private void btnWarrious_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);
        }

        private void btnReviews_Click_1(object sender, EventArgs e)
        {
            ActivateButton(sender);
        }
    }
}
