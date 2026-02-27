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


            cmbSort.DropDownStyle = ComboBoxStyle.DropDownList;

            // Подписка на события (RenderCards будет вызываться при смене индекса)
            chkOnlyPresets.CheckedChanged += (s, e) => RenderCards();
            cmbSort.SelectedIndexChanged += (s, e) => RenderCards();
            btnRefresh.Click += (s, e) => LoadCards();

            ThemeColor.ThemeChanged += ApplyTheme;
            this.Shown += BuildsForm_Shown;
        }

        private void BuildsForm_Shown(object sender, EventArgs e)
        {
            InitFilters(); // Сначала заполняем списки
            ApplyTheme();
            LoadCards();   // Внутри вызовется RenderCards
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
            // Безопасная проверка на случай, если данные еще не загружены
            if (all_ == null)
            {
                labelCount.Text = "Найдено: 0";
                return;
            }

            flpCards.SuspendLayout();
            flpCards.Controls.Clear();

            // 1. Фильтрация
            var query = all_.AsEnumerable();
            if (chkOnlyPresets.Checked)
            {
                query = query.Where(x => x.IsPreset);
            }

            // 2. Сортировка (switch-case)
            switch (cmbSort.SelectedIndex)
            {
                case 1: // Дешевые
                    query = query.OrderBy(x => x.TotalPrice);
                    break;
                case 2: // Дорогие
                    query = query.OrderByDescending(x => x.TotalPrice);
                    break;
                case 3: // Популярные (Заказы)
                    query = query.OrderByDescending(x => x.OrdersCount);
                    break;
                default: // Новые
                    query = query.OrderByDescending(x => x.CreatedDate);
                    break;
            }

            // 3. Получаем итоговый список
            var filteredList = query.ToList();

            // ОБНОВЛЯЕМ СЧЕТЧИК
            labelCount.Text = $"Найдено: {filteredList.Count}";

            // 4. Отрисовка
            foreach (var card in filteredList)
            {
                var c = new BuildCardControl();
                c.Bind(card);
                c.ApplyThemeCard();

                var btnInfo = new Button
                {
                    Text = "Подробнее",
                    Dock = DockStyle.Bottom,
                    Height = 30
                };
                btnInfo.Click += (s, e) => ShowBuildDetails(card);
                c.Controls.Add(btnInfo);

                c.DeleteRequested += Card_DeleteRequested;
                flpCards.Controls.Add(c);
            }

            flpCards.ResumeLayout();
        }

        private void ShowBuildDetails(BuildCard card)
        {
            // Открываем форму с подробной информацией о сборке
            var detailsForm = new BuildDetailsForm(card);
            detailsForm.ShowDialog();
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

        private void cmbSortByPrice_SelectedIndexChanged(object sender, EventArgs e)
        {
            RenderCards();
        }

        private void cmbSortByDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            RenderCards();
        }

        private void InitFilters()
        {
            cmbSort.Items.Clear();
            cmbSort.Items.Add("По умолчанию (Новинки)"); // Индекс 0
            cmbSort.Items.Add("Сначала дешевые");        // Индекс 1
            cmbSort.Items.Add("Сначала дорогие");        // Индекс 2
            cmbSort.Items.Add("Самые популярные");       // Индекс 3
            cmbSort.SelectedIndex = 0;

            labelCount.Text = "Найдено: 0";
        }

        private void btnCreate_Click_1(object sender, EventArgs e)
        {
            using (var f = new ConfiguratorForm())
            {
                if (f.ShowDialog(this) == DialogResult.OK)
                {
                    LoadCards();
                }
            }
        }
    }
}
