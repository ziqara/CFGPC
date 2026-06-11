using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();

            // Поднимаемся из bin\Debug\netX.X в папку WindowsFormsApp1 
            // и заходим в вашу папку Resourses (через 's')
            // Убираем одну пару точек, чтобы остаться внутри папки WindowsFormsApp1
            string imagePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Resourses\image.png"));

            if (File.Exists(imagePath))
            {
                logobox.SizeMode = PictureBoxSizeMode.Zoom;
                logobox.Image = Image.FromFile(imagePath);
            }
            else
            {
                MessageBox.Show($"Опять не совпал путь. Программа искала файл тут:\n{imagePath}\n\nПроверьте, совпадает ли регистр букв и расширение (.png)!");
            }

        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            string enteredLogin = loginTextBox.Text;
            string enteredPassword = passwordTextBox.Text;

            if (enteredLogin == "admin" && enteredPassword == "admin123")
            {
                // Устанавливаем результат "OK" и закрываем форму
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль.");
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
           
        }
    }
}
