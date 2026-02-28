using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DDMLib;

namespace WindowsFormsApp1
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Config.InitializeConfiguration();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // 1. Создаем и запускаем форму входа
            LoginForm loginForm = new LoginForm();

            // ShowDialog останавливает выполнение Main, пока LoginForm не закроется
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                // 2. Если вход успешен, запускаем основную форму
                Application.Run(new UpdateMainForm());
            }
            else
            {
                // Если пользователь просто закрыл окно логина — выходим
                Application.Exit();
            }
        }
    }
}
