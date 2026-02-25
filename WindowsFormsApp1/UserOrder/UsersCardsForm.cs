using DDMLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1.UserOrder
{
    public partial class UsersCardsForm : Form
    {

        private readonly AdminUserService service_;
        private List<User> allUsers_ = new List<User>();
        private Dictionary<string, bool> activeMap_ = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

        public UsersCardsForm()
        {
            InitializeComponent();

            service_ = new AdminUserService(new UserRepository());

            this.Shown += UsersCardsForm_Shown;
            ThemeColor.ThemeChanged += ApplyTheme;
        }

        private void UsersCardsForm_Load(object sender, EventArgs e)
        {
            LoadUsers();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            RenderUsersCards();
        }

        private void cbxActiveOrders_CheckedChanged(object sender, EventArgs e)
        {
            RenderUsersCards();
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            LoadUsers();
        }

        private void UsersCardsForm_Shown(object sender, EventArgs e)
        {
            ApplyTheme();
        }

        private void LoadUsers()
        {
            try
            {
                allUsers_ = service_.GetAllUsers() ?? new List<User>();
                activeMap_ = service_.GetActiveOrdersFlags() ?? new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

                flpUsers.Controls.Clear();

                if (allUsers_.Count == 0)
                {
                    MessageBox.Show("Пользователей пока нет", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                RenderUsersCards();
            }
            catch (Exception ex)
            {
                flpUsers.Controls.Clear();
                MessageBox.Show($"Вероятно, проблемы в соединении с БД.\n\n{ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RenderUsersCards()
        {
            if (allUsers_ == null) return;

            flpUsers.SuspendLayout();
            flpUsers.Controls.Clear();

            string search = txtSearch.Text?.Trim() ?? "";
            bool onlyActive = cbxActiveOrders.Checked;

            IEnumerable<User> query = allUsers_;

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u =>
                    (!string.IsNullOrEmpty(u.Email) && u.Email.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (!string.IsNullOrEmpty(u.FullName) && u.FullName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0));
            }

            foreach (var u in query)
            {
                bool hasActive = activeMap_.TryGetValue(u.Email, out var a) && a;
                if (onlyActive && !hasActive) continue;

                var vm = new UserCardVm { User = u, HasActiveOrders = hasActive };
                var card = new UserCardControl(vm);

                card.OrdersRequested += (s, v) =>
                {
                    using (var f = new UserOrdersForm(v.User.Email))
                        f.ShowDialog(this);
                };

                card.EditRequested += (s, v) =>
                {
                    using (var f = new EditUserForm(service_, v.User))
                    {
                        if (f.ShowDialog(this) == DialogResult.OK)
                            LoadUsers();
                    }
                };

                card.DeleteRequested += (s, v) =>
                {
                    DialogResult confirm = MessageBox.Show(
                        $"Удалить пользователя \"{v.User.FullName}\"?\n{v.User.Email}\nДействие нельзя отменить.",
                        "Подтверждение удаления",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (confirm != DialogResult.Yes)
                        return;

                    string result = service_.DeleteUser(v.User.Email);

                    if (string.IsNullOrEmpty(result))
                    {
                        MessageBox.Show("Пользователь удалён.", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadUsers();
                    }
                    else
                    {
                        MessageBox.Show(result, "Ошибка удаления",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                card.ApplyTheme();
                flpUsers.Controls.Add(card);
            }

            flpUsers.ResumeLayout();
        }

        private void ApplyTheme()
        {
            if (flpUsers == null || flpUsers.IsDisposed) return;

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

            labelTitle.ForeColor = ThemeColor.PrimaryColor;
            cbxActiveOrders.ForeColor = ThemeColor.PrimaryColor;

            flpUsers.BackColor = Color.FromArgb(248, 249, 250);
        }
    }
}
