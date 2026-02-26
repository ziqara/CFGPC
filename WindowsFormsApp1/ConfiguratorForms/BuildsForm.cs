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

namespace WindowsFormsApp1.ConfiguratorForms
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
            btnCreate.Click += btnCreate_Click;

            cbSort.SelectedIndexChanged += (s, e) => RenderCards();

            // значения сортировки
            cbSort.Items.Clear();
            cbSort.Items.Add("По дате (новые)");
            cbSort.Items.Add("По цене (дешевле)");
            cbSort.Items.Add("По цене (дороже)");
            cbSort.Items.Add("По продажам (больше)");
            cbSort.SelectedIndex = 0;
        }

        private void BuildsForm_Shown(object sender, EventArgs e)
        {
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
                // грузим все, фильтры/сортировки делаем в UI
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

            IEnumerable<BuildCard> items = all_;
            if (onlyPresets)
                items = items.Where(x => x.IsPreset);

            // сортировка
            switch (cbSort.SelectedIndex)
            {
                case 1: // дешевле
                    items = items.OrderBy(x => x.TotalPrice).ThenByDescending(x => x.CreatedDate);
                    break;
                case 2: // дороже
                    items = items.OrderByDescending(x => x.TotalPrice).ThenByDescending(x => x.CreatedDate);
                    break;
                case 3: // продажи
                    items = items.OrderByDescending(x => x.SalesCount).ThenByDescending(x => x.CreatedDate);
                    break;
                default: // по дате
                    items = items.OrderByDescending(x => x.CreatedDate).ThenByDescending(x => x.ConfigId);
                    break;
            }

            flpCards.SuspendLayout();
            flpCards.Controls.Clear();

            foreach (var card in items)
            {
                var c = new BuildCardControl();
                c.Bind(card);
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
    }
}
