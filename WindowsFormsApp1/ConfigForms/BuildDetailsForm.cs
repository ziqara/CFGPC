using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DDMLib;
using DDMLib.Configuration;
using DDMLib.Component;

namespace WindowsFormsApp1.ConfigForms
{
    public partial class BuildDetailsForm : Form
    {
        private readonly BuildCard _card;
        private readonly BuildService _buildService;
        private List<DDMLib.Component.Component> _components = new List<DDMLib.Component.Component>();

        public BuildDetailsForm(BuildCard card)
        {
            InitializeComponent();
            _card = card;
            _buildService = new BuildService(new MySqlBuildRepository());

            // Подписки на события по аналогии с SupplierForm
            dgvComponents.CellFormatting += DgvComponents_CellFormatting;
            this.Shown += BuildDetailsForm_Shown;
            ThemeColor.ThemeChanged += ApplyTableTheme;

            this.ClientSize = new System.Drawing.Size(600, 380); // Максимально сжатый размер

            // ВАЖНО: Отключаем растягивание от шрифтов системы
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;

            this.MaximumSize = new System.Drawing.Size(600, 380);
            this.MinimumSize = new System.Drawing.Size(600, 380);

            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        }

        private void DgvComponents_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            // Форматирование цены
            if (dgvComponents.Columns[e.ColumnIndex].Name == "Цена")
            {
                if (e.Value != null)
                {
                    e.Value = string.Format("{0:N2} руб.", e.Value);
                    e.FormattingApplied = true;
                }
            }

            // Подсветка отсутствующих товаров (строгий стиль)
            if (dgvComponents.Columns[e.ColumnIndex].Name == "Статус")
            {
                if (e.Value?.ToString() == "Отсутствует")
                {
                    e.CellStyle.ForeColor = Color.Red;
                    e.CellStyle.SelectionForeColor = Color.Red;
                }
            }
        }

        private void ApplyTableTheme()
        {
            if (dgvComponents == null) return;

            // Применяем цветовую схему из твоего проекта
            lblTitle.ForeColor = ThemeColor.PrimaryColor;
            btnClose.BackColor = ThemeColor.PrimaryColor;
            btnClose.ForeColor = Color.White;

            // Копируем стиль из SupplierForm
            dgvComponents.BackgroundColor = Color.White;
            dgvComponents.GridColor = Color.FromArgb(224, 224, 224);
            dgvComponents.BorderStyle = BorderStyle.FixedSingle;
            dgvComponents.RowHeadersVisible = false;
            dgvComponents.ReadOnly = true;
            dgvComponents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvComponents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvComponents.EnableHeadersVisualStyles = false;

            // Заголовки (как у поставщиков)
            dgvComponents.ColumnHeadersDefaultCellStyle.BackColor = ThemeColor.PrimaryColor;
            dgvComponents.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvComponents.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9F, FontStyle.Bold);
            dgvComponents.ColumnHeadersHeight = 30;

            // Строки (делаем их чуть выше, чтобы форма не казалась пустой)
            dgvComponents.DefaultCellStyle.Font = new Font("Arial", 9F);
            dgvComponents.RowTemplate.Height = 26;
        }

        private void BuildDetailsForm_Load(object sender, EventArgs e)
        {
            SetupDesign();
            LoadData();
            ApplyTheme(); // Применяем цвета при загрузке
        }

        private void SetupDesign()
        {
            this.Text = $"Детали сборки #{_card.ConfigId}";
            this.Size = new Size(950, 700);
            this.StartPosition = FormStartPosition.CenterParent;

            // Настройка FlowLayoutPanel или Panel, если они есть для фона
            dgvComponents.BackgroundColor = Color.White;
            dgvComponents.GridColor = Color.FromArgb(240, 240, 240);
            dgvComponents.BorderStyle = BorderStyle.None;
            dgvComponents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvComponents.AllowUserToAddRows = false;
            dgvComponents.ReadOnly = true;
            dgvComponents.RowHeadersVisible = false;
            dgvComponents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Современный стиль шрифтов
            dgvComponents.DefaultCellStyle.Font = new Font("Segoe UI", 10f);
            dgvComponents.ColumnHeadersHeight = 40;
            dgvComponents.EnableHeadersVisualStyles = false;
        }

        private void ApplyTheme()
        {
            // Заголовок в основном цвете темы
            lblTitle.Text = _card.ConfigName.ToUpper();
            lblTitle.ForeColor = ThemeColor.PrimaryColor;
            lblTitle.Font = new Font("Arial", 18f, FontStyle.Bold);

            // Стилизация таблицы под текущую тему
            dgvComponents.ColumnHeadersDefaultCellStyle.BackColor = ThemeColor.PrimaryColor;
            dgvComponents.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvComponents.ColumnHeadersDefaultCellStyle.SelectionBackColor = ThemeColor.PrimaryColor;
            dgvComponents.DefaultCellStyle.SelectionBackColor = Color.FromArgb(206, 212, 218);
            dgvComponents.DefaultCellStyle.SelectionForeColor = Color.FromArgb(52, 58, 64);

            // Кнопка закрытия или другие кнопки (если есть на форме)
            foreach (Control c in this.Controls)
            {
                if (c is Button btn)
                {
                    btn.BackColor = ThemeColor.PrimaryColor;
                    btn.ForeColor = Color.White;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                }

                if (c is Panel p && p.Name == "panelHeader")
                {
                    p.BackColor = Color.White;
                }
            }

            this.BackColor = Color.FromArgb(248, 249, 250); // Светло-серый фон как в заказах
        }

        private void LoadData()
        {
            try
            {
                ConfigurationDto dto = _buildService.GetBuildDetails(_card.ConfigId);

                if (dto == null || dto.Components == null)
                {
                    MessageBox.Show("Данные о компонентах не найдены.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Инфо-панель (Summary)
                lblSummary.Font = new Font("Segoe UI", 10f);
                lblSummary.Text = $"ID Сборки: {_card.ConfigId}   |   Клиент: {_card.UserEmail ?? "Пресет"}\n" +
                                 $"Дата: {_card.CreatedDate:f}\n" +
                                 $"Итоговая стоимость: {_card.TotalPrice:N2} руб.";

                // Словарь для перевода типов (опционально)
                var typeTranslations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                    {"cpu", "Процессор"}, {"gpu", "Видеокарта"}, {"motherboard", "Мат. плата"},
                    {"ram", "ОЗУ"}, {"storage", "Накопитель"}, {"psu", "Блок питания"},
                    {"case", "Корпус"}, {"cooling", "Охлаждение"}
                };

                // Мапим данные в таблицу
                var displayList = dto.Components.Select(c => new
                {
                    Тип = typeTranslations.ContainsKey(c.Type) ? typeTranslations[c.Type] : c.Type,
                    Компонент = $"{c.Brand} {c.Model}",
                    Цена = c.Price,
                    Наличие = c.StockQuantity > 0 ? $" {c.StockQuantity} шт." : "❌ Нет в наличии"
                }).ToList();

                dgvComponents.DataSource = displayList;

                // Настройка колонок после привязки данных
                dgvComponents.Columns["Цена"].DefaultCellStyle.Format = "N2";
                dgvComponents.Columns["Цена"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                // Подсвечиваем отсутствие товара
                foreach (DataGridViewRow row in dgvComponents.Rows)
                {
                    if (row.Cells["Наличие"].Value.ToString().Contains("❌"))
                    {
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 230, 230);
                        row.DefaultCellStyle.ForeColor = Color.DarkRed;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BuildDetailsForm_Shown(object sender, EventArgs e)
        {
            ApplyTableTheme();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}