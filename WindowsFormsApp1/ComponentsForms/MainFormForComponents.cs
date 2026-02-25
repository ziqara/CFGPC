using System;
using System.Drawing;
using System.Windows.Forms;
using WindowsFormsApp1.ComponentsForms.CaseForms;
using WindowsFormsApp1.ComponentsForms.CoolingForms;
using WindowsFormsApp1.ComponentsForms.GpuForms;
using WindowsFormsApp1.ComponentsForms.MotherboardForms;
using WindowsFormsApp1.ComponentsForms.PsuForms;
using WindowsFormsApp1.ComponentsForms.RamForms;
using WindowsFormsApp1.ComponentsForms.StorageForms;

namespace WindowsFormsApp1.ComponentsForms
{
    public partial class MainFormForComponents : Form
    {
        public MainFormForComponents()
        {
            InitializeComponent();
            this.Load += MainFormForComponents_Load;
        }

        private void MainFormForComponents_Load(object sender, EventArgs e)
        {
            ApplyButtonsStyle();
            StyleButtons();
        }
        private void StyleButtons()
        {
            foreach (Control control in mainTable.Controls)
            {
                if (control is Button btn)
                {
                    btn.BackColor = ThemeColor.PrimaryColor;
                    btn.ForeColor = Color.White;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderColor = ThemeColor.SecondaryColor;
                    btn.FlatAppearance.BorderSize = 1;

                    btn.Font = new Font("Segoe UI", 12, FontStyle.Bold);

                    btn.MouseEnter += (s, e) =>
                    {
                        btn.BackColor = ThemeColor.SecondaryColor;
                    };

                    btn.MouseLeave += (s, e) =>
                    {
                        btn.BackColor = ThemeColor.PrimaryColor;
                    };
                }
            }
        }

        private void ApplyButtonsStyle()
        {
            foreach (Control control in this.Controls)
            {
                if (control is Button btn)
                {
                    btn.BackColor = ThemeColor.PrimaryColor;
                    btn.ForeColor = Color.White;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 1;
                    btn.FlatAppearance.BorderColor = ThemeColor.SecondaryColor;

                    btn.Font = new Font("Arial", 11, FontStyle.Bold);
                    btn.Width = 200;
                    btn.Height = 60;
                }
            }
        }

        private void cpuButton_Click(object sender, EventArgs e)
        {
            using (var form = new CpuForm())
            {
                form.ShowDialog(this);
            }
        }

        private void mBoardButton_Click_1(object sender, EventArgs e)
        {
            using (var form = new MotherboardForm())
            {
                form.ShowDialog(this);
            }
        }

        private void ramButton_Click(object sender, EventArgs e)
        {
            using (var form = new RamForm())
                form.ShowDialog(this);
        }

        private void gpuButton_Click(object sender, EventArgs e)
        {
            using (var form = new GpuForm())
                form.ShowDialog(this);
        }

        private void storageButton_Click(object sender, EventArgs e)
        {
            using (var form = new StorageForm())
                form.ShowDialog(this);
        }

        private void psuButton_Click(object sender, EventArgs e)
        {
            using (var form = new PsuForm())
                form.ShowDialog(this);
        }

        private void caseButton_Click(object sender, EventArgs e)
        {
            using (var form = new CaseForm())
                form.ShowDialog(this);
        }

        private void coolingButton_Click(object sender, EventArgs e)
        {
            using (var form = new CoolingForm())
                form.ShowDialog(this);
        }
    }
}