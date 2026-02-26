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

namespace WindowsFormsApp1.ConfigForms
{
    public partial class BuildsForm : Form
    {
        private readonly BuildService buildService_;
        private List<BuildCard> all_ = new List<BuildCard>();

        public BuildsForm()
        {
            InitializeComponent();

            buildService_ = new BuildService(new MySqlBuildRepository());

            this.Shown += BuildsForm_Shown;
            chkOnlyPresets.CheckedChanged += (s, e) => RenderCards();
            btnRefresh.Click += (s, e) => LoadCards();
            ThemeColor.ThemeChanged += ApplyTheme;
        }

        private void BuildsForm_Shown(object sender, EventArgs e)
        {
            ApplyTheme();
            LoadCards();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            using (var f = new ConfiguratorForm())
            {
                if (f.ShowDialog(this) == DialogResult.OK)
                {
                    LoadCards();
                }
            }
        }

        private void LoadCards()
        {
            try
            {
                all_ = buildService_.GetBuildCards(false) ?? new List<BuildCard>();
                RenderCards();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Вероятно, проблемы в соединении с БД:\n\n" + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RenderCards()
        {
            bool onlyPresets = chkOnlyPresets.Checked;

            flpCards.SuspendLayout();
            flpCards.Controls.Clear();

            IEnumerable<BuildCard> items = all_;
            if (onlyPresets)
                items = all_.FindAll(x => x.IsPreset);

            foreach (var card in items)
            {
                var c = new BuildCardControl();
                c.Bind(card);
                c.ApplyThemeCard();


                c.DeleteRequested += Card_DeleteRequested;
                flpCards.Controls.Add(c);
            }

            flpCards.ResumeLayout();
        }


        private void Card_DeleteRequested(object sender, int configId)
        {
            var confirm = MessageBox.Show(
                $"Удалить сборку #{configId}?\nДействие нельзя отменить.",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            string result = buildService_.DeleteBuild(configId);

            if (string.IsNullOrEmpty(result))
            {
                MessageBox.Show("Сборка удалена.", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadCards();
            }
            else
            {
                MessageBox.Show(result, "Ошибка удаления",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                LoadCards();
            }
        }

        private void ApplyTheme()
        {
            if (this.IsDisposed) return;

            // Все кнопки на форме
            foreach (Control c in this.Controls)
            {
                if (c is Button btn)
                {
                    btn.BackColor = ThemeColor.PrimaryColor;
                    btn.ForeColor = Color.White;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderColor = ThemeColor.SecondaryColor;
                    btn.FlatAppearance.BorderSize = 1;
                }
            }

            // Чекбокс
            chkOnlyPresets.ForeColor = ThemeColor.PrimaryColor;

            // Фон панели карточек
            flpCards.BackColor = Color.FromArgb(248, 249, 250);
        }
    }
}
