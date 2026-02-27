using DDMLib;
using System.Drawing;
using System.Windows.Forms;
using System;

namespace WindowsFormsApp1.ConfigForms
{
    public partial class BuildCardControl : UserControl
    {
        public event EventHandler<int> DeleteRequested;
        private bool _hovered = false;
        private BuildCard card_;

        public BuildCardControl()
        {
            InitializeComponent();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (card_ == null) return;
            DeleteRequested?.Invoke(this, card_.ConfigId);
        }

        public void Bind(BuildCard card)
        {
            card_ = card;

            lblTitle.Text = $"#{card.ConfigId} · {card.ConfigName}";
            lblMeta.Text = $"Цена: {card.TotalPrice:N2}   |   {card.CreatedDate:yyyy-MM-dd HH:mm}   |   {card.UserEmail ?? "—"}";
            lblPreset.Text = card.IsPreset ? "Готовый пресет: Да" : "Готовый пресет: Нет";

            string orders = card.OrdersCount > 0
                ? $"Заказов: {card.OrdersCount} (удаление запрещено)"
                : "Заказов: 0 (можно удалить)";

            string avail = card.BadComponents > 0
                ? $"Недоступных компонентов: {card.BadComponents}"
                : "Все компоненты доступны";

            lblStatus.Text = orders + "   |   " + avail;

            btnDelete.Enabled = card.CanDelete;

            // легкая подсветка проблем
            if (card.HasAvailabilityProblems)
                BackColor = Color.FromArgb(255, 245, 245);
            else
                BackColor = Color.White;
        }

        internal void ApplyThemeCard()
        {
            if (IsDisposed) return;

            // Карточка
            this.BackColor = Color.White;
            this.Padding = new Padding(12);
            this.DoubleBuffered = true;

            // Шрифты (чуть лучше читаемость)
            lblTitle.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            lblMeta.Font = new Font("Segoe UI", 9f, FontStyle.Regular);
            lblPreset.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            lblStatus.Font = new Font("Segoe UI", 9f, FontStyle.Regular);

            // Цвета заголовка и "мета" (мета делаем нейтральной)
            lblTitle.ForeColor = ThemeColor.PrimaryColor;
            lblMeta.ForeColor = Color.FromArgb(110, 110, 110);

            // Кнопка "Удалить" в стиле темы
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.BackColor = ThemeColor.PrimaryColor;
            btnDelete.ForeColor = Color.White;
            btnDelete.FlatAppearance.BorderColor = ThemeColor.SecondaryColor;
            btnDelete.FlatAppearance.BorderSize = 1;

            // Подпишемся на hover (делаем один раз, но безопасно — можно и так)
            this.MouseEnter -= Card_MouseEnter;
            this.MouseLeave -= Card_MouseLeave;
            this.MouseEnter += Card_MouseEnter;
            this.MouseLeave += Card_MouseLeave;

            foreach (Control c in this.Controls)
            {
                c.MouseEnter -= Card_MouseEnter;
                c.MouseLeave -= Card_MouseLeave;
                c.MouseEnter += Card_MouseEnter;
                c.MouseLeave += Card_MouseLeave;
            }

            // Применяем “семантику” выделения (на основе card_)
            UpdateVisualEmphasis();

            Invalidate();
        }

        private void Card_MouseEnter(object sender, EventArgs e)
        {
            _hovered = true;
            Invalidate();
        }

        private void Card_MouseLeave(object sender, EventArgs e)
        {
            _hovered = false;
            Invalidate();
        }

        private void UpdateVisualEmphasis()
        {
            if (card_ == null) return;

            // Бейдж пресета (как "плашка")
            lblPreset.AutoSize = false;
            lblPreset.Height = 22;
            lblPreset.Padding = new Padding(8, 3, 8, 3);

            // Если пресет есть, зеленый фон и зеленый текст, если нет — красный фон и красный текст
            if (card_.IsPreset)
            {
                lblPreset.BackColor = Color.FromArgb(232, 245, 233);   // зеленоватый фон
                lblPreset.ForeColor = Color.FromArgb(46, 125, 50);      // зелёный текст
            }
            else
            {
                lblPreset.BackColor = Color.FromArgb(255, 238, 238);   // красный фон
                lblPreset.ForeColor = Color.FromArgb(198, 40, 40);     // красный текст
            }

            // Статус (заказы / доступность) — нейтральный цвет
            lblStatus.ForeColor = Color.FromArgb(90, 90, 90);  // Нейтральный цвет (серый)

            // Если проблемы с компонентами — подсветка карточки (очень лёгкая)
            if (card_.HasAvailabilityProblems)
                this.BackColor = Color.FromArgb(255, 250, 250); // Легкая подсветка для проблем
            else
                this.BackColor = Color.White;

            // Кнопку можно сделать "серой", если нельзя удалять
            if (!card_.CanDelete)
            {
                btnDelete.BackColor = Color.FromArgb(210, 210, 210);
                btnDelete.ForeColor = Color.FromArgb(80, 80, 80);
                btnDelete.FlatAppearance.BorderColor = Color.FromArgb(190, 190, 190);
            }
        }
    }
}