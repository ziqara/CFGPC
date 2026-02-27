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
    public partial class BuildCardControl : UserControl
    {
        public event EventHandler<int> DeleteRequested;

        private BuildCard card_;

        public BuildCardControl()
        {
            InitializeComponent();
            btnDelete.Click += btnDelete_Click;
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

            string sales = card.SalesCount > 0
                ? $"Продаж: {card.SalesCount} (удаление запрещено)"
                : "Продаж: 0 (можно удалить)";

            string avail = card.BadComponents > 0
                ? $"⚠ Недоступных/нет на складе компонентов: {card.BadComponents}"
                : "✓ Все компоненты доступны";

            lblStatus.Text = sales + "   |   " + avail;

            btnDelete.Enabled = card.CanDelete;

            BackColor = card.HasAvailabilityProblems
                ? Color.FromArgb(255, 245, 245)
                : Color.White;
        }
    }
}
