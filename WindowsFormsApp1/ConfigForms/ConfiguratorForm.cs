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
using DDMLib.Component;
using DDMLib.ConfigForAdmin;

namespace WindowsFormsApp1.ConfigForms
{
    public partial class ConfiguratorForm : Form
    {
        private readonly ComponentServiceAdmin compService_;
        private readonly BuildService buildService_;

        public ConfiguratorForm()
        {
            InitializeComponent();
            InitTargetUse();
            compService_ = new ComponentServiceAdmin(new MySqlComponentRepository());
            buildService_ = new BuildService(new MySqlBuildRepository());

            this.Shown += ConfiguratorForm_Shown;

            cbMotherboard.SelectedIndexChanged += CbMotherboard_SelectedIndexChanged;
            cbCpu.SelectedIndexChanged += CbCpu_SelectedIndexChanged;
            cbGpu.SelectedIndexChanged += CbGpu_SelectedIndexChanged;

            cbRam.SelectedIndexChanged += (s, e) => UpdateTotal();
            cbStorage.SelectedIndexChanged += (s, e) => UpdateTotal();
            cbPsu.SelectedIndexChanged += (s, e) => UpdateTotal();
            cbCase.SelectedIndexChanged += (s, e) => UpdateTotal();
            cbCooling.SelectedIndexChanged += (s, e) => UpdateTotal();

            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

        }

        private void ConfiguratorForm_Shown(object sender, EventArgs e)
        {
            try
            {
                Bind(cbMotherboard, compService_.GetMotherboards());
                Bind(cbStorage, compService_.GetStorages());
                UpdateTotal();
                ApplyTheme();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Проблемы при загрузке компонентов:\n\n" + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CbMotherboard_SelectedIndexChanged(object sender, EventArgs e)
        {
            var mb = cbMotherboard.SelectedItem as ComponentItem;
            if (mb == null)
            {
                // Если выбор сброшен — очищаем все списки, зависящие от МП
                cbCpu.DataSource = null;
                cbRam.DataSource = null;
                cbGpu.DataSource = null;
                cbCase.DataSource = null;
                UpdateTotal();
                return;
            }

            Bind(cbCpu, compService_.GetCpusByMb(mb.ComponentId));
            Bind(cbRam, compService_.GetRamsByMb(mb.ComponentId));
            Bind(cbGpu, compService_.GetGpusByMb(mb.ComponentId));
            Bind(cbCase, compService_.GetCasesByMb(mb.ComponentId));

            cbCooling.DataSource = null;
            cbPsu.DataSource = null;
            UpdateTotal();
        }

        private void CbCpu_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cpu = cbCpu.SelectedItem as ComponentItem;
            if (cpu == null) return;

            Bind(cbCooling, compService_.GetCoolingsByCpu(cpu.ComponentId)); // Обновляем охлаждение по процессору

            RefreshPsuIfPossible(); // Обновляем БП по процессору и видеокарте
            UpdateTotal();
        }

        private void CbGpu_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cpu = cbCpu.SelectedItem as ComponentItem;
            if (cpu == null) return;

            // Обновляем охлаждения
            Bind(cbCooling, compService_.GetCoolingsByCpu(cpu.ComponentId));

            // Обновляем блок питания
            RefreshPsuIfPossible();

            // Обновляем итоговую цену
            UpdateTotal();
        }

        private void RefreshPsuIfPossible()
        {
            var cpu = cbCpu.SelectedItem as ComponentItem;
            var gpu = cbGpu.SelectedItem as ComponentItem;

            if (cpu == null || gpu == null)
            {
                cbPsu.DataSource = null; // Очищаем список блоков питания, если нет процессора или видеокарты
                return;
            }

            // Обновляем блок питания
            Bind(cbPsu, compService_.GetPsusByCpuGpu(cpu.ComponentId, gpu.ComponentId));
        }

        private void Bind(ComboBox cb, List<ComponentItem> items)
        {
            cb.DataSource = null;
            cb.DropDownStyle = ComboBoxStyle.DropDownList;
            cb.DataSource = items ?? new List<ComponentItem>();

            // ВАЖНО: устанавливаем -1, чтобы комбобокс был пустым
            cb.SelectedIndex = -1;
        }

        private decimal PriceOf(ComboBox cb)
        {
            var it = cb.SelectedItem as ComponentItem;
            return it == null ? 0m : it.Price;
        }

        private void UpdateTotal()
        {
            decimal total =
                PriceOf(cbMotherboard) + PriceOf(cbCpu) + PriceOf(cbRam) + PriceOf(cbGpu) +
                PriceOf(cbStorage) + PriceOf(cbPsu) + PriceOf(cbCase) + PriceOf(cbCooling);

            lblTotal.Text = "Итого: " + total.ToString("N2");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var draft = new BuildDraft
            {
                ConfigName = txtName.Text?.Trim(),
                Description = txtDescription.Text?.Trim(),

                TargetUse = cbTargetUse.SelectedValue?.ToString(),
                Rgb = chbRgb.Checked,
                OtherOptions = txtOtherOptions.Text?.Trim(),

                MotherboardId = (cbMotherboard.SelectedItem as ComponentItem)?.ComponentId ?? 0,
                CpuId = (cbCpu.SelectedItem as ComponentItem)?.ComponentId ?? 0,
                RamId = (cbRam.SelectedItem as ComponentItem)?.ComponentId ?? 0,
                GpuId = (cbGpu.SelectedItem as ComponentItem)?.ComponentId ?? 0,
                StorageId = (cbStorage.SelectedItem as ComponentItem)?.ComponentId ?? 0,
                PsuId = (cbPsu.SelectedItem as ComponentItem)?.ComponentId ?? 0,
                CaseId = (cbCase.SelectedItem as ComponentItem)?.ComponentId ?? 0,
                CoolingId = (cbCooling.SelectedItem as ComponentItem)?.ComponentId ?? 0,
            };

            int createdId;
            string result = buildService_.CreatePreset(draft, out createdId);

            if (string.IsNullOrEmpty(result))
            {
                MessageBox.Show($"Сборка сохранена как пресет.\nID: {createdId}",
                    "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show(result, "Ошибка сохранения",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private class TargetUseItem
        {
            public string DisplayName { get; set; } // То, что видит юзер 
            public string Value { get; set; }       // То, что идет в БД 
        }

        private void InitTargetUse()
        {
            var options = new List<TargetUseItem>
            {
                new TargetUseItem { DisplayName = "Игровой", Value = "gaming" },
                new TargetUseItem { DisplayName = "Профессиональный", Value = "professional" },
                new TargetUseItem { DisplayName = "Офисный", Value = "office" },
                new TargetUseItem { DisplayName = "Студенческий", Value = "student" }
            };

            cbTargetUse.DataSource = options;
            cbTargetUse.DisplayMember = "DisplayName";
            cbTargetUse.ValueMember = "Value";
            cbTargetUse.SelectedIndex = -1;
        }

        private void ApplyTheme()
        {
            ApplyComboBoxStyle(cbTargetUse);
            txtOtherOptions.BackColor = Color.White;
            txtOtherOptions.Font = new Font("Arial", 9f);

            // Применяем стиль для заголовков и кнопок
            this.BackColor = Color.FromArgb(245, 245, 245); // Светлый фон

            // Заголовки
            lblTotal.ForeColor = ThemeColor.PrimaryColor;
            lblTotal.Font = new Font("Segoe UI", 10f, FontStyle.Bold);

            // Введем стиль для ComboBox (используем границы, шрифт)
            ApplyComboBoxStyle(cbMotherboard);
            ApplyComboBoxStyle(cbCpu);
            ApplyComboBoxStyle(cbGpu);
            ApplyComboBoxStyle(cbRam);
            ApplyComboBoxStyle(cbStorage);
            ApplyComboBoxStyle(cbPsu);
            ApplyComboBoxStyle(cbCase);
            ApplyComboBoxStyle(cbCooling);

            // Применяем стиль для кнопок
            ApplyButtonStyle(btnSave);
            ApplyButtonStyle(btnCancel);
        }

        private void ApplyComboBoxStyle(ComboBox comboBox)
        {
            comboBox.BackColor = Color.White;
            comboBox.ForeColor = Color.Black;
            comboBox.Font = new Font("Arial", 9f);
            comboBox.FlatStyle = FlatStyle.Flat;
        }

        private void ApplyButtonStyle(Button btn)
        {
            btn.BackColor = ThemeColor.PrimaryColor;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Arial", 9f, FontStyle.Bold);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderColor = ThemeColor.SecondaryColor;
            btn.FlatAppearance.BorderSize = 1;

            // Hover эффекты для кнопок
            btn.MouseEnter += (s, e) => btn.BackColor = ThemeColor.SecondaryColor;
            btn.MouseLeave += (s, e) => btn.BackColor = ThemeColor.PrimaryColor;
        }
    }
}
