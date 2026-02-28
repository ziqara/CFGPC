namespace WindowsFormsApp1.ConfigForms
{
    partial class ConfiguratorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblHeader = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblDesc = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbCooling = new System.Windows.Forms.ComboBox();
            this.cbCase = new System.Windows.Forms.ComboBox();
            this.cbPsu = new System.Windows.Forms.ComboBox();
            this.cbStorage = new System.Windows.Forms.ComboBox();
            this.cbGpu = new System.Windows.Forms.ComboBox();
            this.cbRam = new System.Windows.Forms.ComboBox();
            this.cbCpu = new System.Windows.Forms.ComboBox();
            this.cbMotherboard = new System.Windows.Forms.ComboBox();
            this.lblCooling = new System.Windows.Forms.Label();
            this.lblCase = new System.Windows.Forms.Label();
            this.lblPsu = new System.Windows.Forms.Label();
            this.lblStorage = new System.Windows.Forms.Label();
            this.lblGpu = new System.Windows.Forms.Label();
            this.lblRam = new System.Windows.Forms.Label();
            this.lblCpu = new System.Windows.Forms.Label();
            this.lblMb = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cbTargetUse = new System.Windows.Forms.ComboBox();
            this.chbRgb = new System.Windows.Forms.CheckBox();
            this.txtOtherOptions = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblHeader
            // 
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblHeader.Location = new System.Drawing.Point(270, 9);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(201, 23);
            this.lblHeader.TabIndex = 0;
            this.lblHeader.Text = "Создание сборки (пресет)";
            // 
            // lblName
            // 
            this.lblName.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblName.Location = new System.Drawing.Point(38, 39);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(79, 18);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "Название";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(118, 38);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(514, 20);
            this.txtName.TabIndex = 2;
            // 
            // lblDesc
            // 
            this.lblDesc.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblDesc.Location = new System.Drawing.Point(39, 93);
            this.lblDesc.Name = "lblDesc";
            this.lblDesc.Size = new System.Drawing.Size(79, 18);
            this.lblDesc.TabIndex = 3;
            this.lblDesc.Text = "Описание";
            this.lblDesc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(118, 93);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDescription.Size = new System.Drawing.Size(514, 60);
            this.txtDescription.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbCooling);
            this.groupBox1.Controls.Add(this.cbCase);
            this.groupBox1.Controls.Add(this.chbRgb);
            this.groupBox1.Controls.Add(this.cbPsu);
            this.groupBox1.Controls.Add(this.cbStorage);
            this.groupBox1.Controls.Add(this.cbGpu);
            this.groupBox1.Controls.Add(this.cbRam);
            this.groupBox1.Controls.Add(this.cbCpu);
            this.groupBox1.Controls.Add(this.cbMotherboard);
            this.groupBox1.Controls.Add(this.lblCooling);
            this.groupBox1.Controls.Add(this.lblCase);
            this.groupBox1.Controls.Add(this.lblPsu);
            this.groupBox1.Controls.Add(this.lblStorage);
            this.groupBox1.Controls.Add(this.lblGpu);
            this.groupBox1.Controls.Add(this.lblRam);
            this.groupBox1.Controls.Add(this.lblCpu);
            this.groupBox1.Controls.Add(this.lblMb);
            this.groupBox1.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox1.Location = new System.Drawing.Point(15, 228);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(637, 299);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Компоненты";
            // 
            // cbCooling
            // 
            this.cbCooling.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCooling.FormattingEnabled = true;
            this.cbCooling.Location = new System.Drawing.Point(123, 262);
            this.cbCooling.Name = "cbCooling";
            this.cbCooling.Size = new System.Drawing.Size(498, 24);
            this.cbCooling.TabIndex = 15;
            // 
            // cbCase
            // 
            this.cbCase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCase.FormattingEnabled = true;
            this.cbCase.Location = new System.Drawing.Point(123, 231);
            this.cbCase.Name = "cbCase";
            this.cbCase.Size = new System.Drawing.Size(498, 24);
            this.cbCase.TabIndex = 14;
            // 
            // cbPsu
            // 
            this.cbPsu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPsu.FormattingEnabled = true;
            this.cbPsu.Location = new System.Drawing.Point(123, 200);
            this.cbPsu.Name = "cbPsu";
            this.cbPsu.Size = new System.Drawing.Size(498, 24);
            this.cbPsu.TabIndex = 13;
            // 
            // cbStorage
            // 
            this.cbStorage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbStorage.FormattingEnabled = true;
            this.cbStorage.Location = new System.Drawing.Point(123, 169);
            this.cbStorage.Name = "cbStorage";
            this.cbStorage.Size = new System.Drawing.Size(498, 24);
            this.cbStorage.TabIndex = 12;
            // 
            // cbGpu
            // 
            this.cbGpu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbGpu.FormattingEnabled = true;
            this.cbGpu.Location = new System.Drawing.Point(123, 138);
            this.cbGpu.Name = "cbGpu";
            this.cbGpu.Size = new System.Drawing.Size(498, 24);
            this.cbGpu.TabIndex = 11;
            // 
            // cbRam
            // 
            this.cbRam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRam.FormattingEnabled = true;
            this.cbRam.Location = new System.Drawing.Point(123, 107);
            this.cbRam.Name = "cbRam";
            this.cbRam.Size = new System.Drawing.Size(498, 24);
            this.cbRam.TabIndex = 10;
            // 
            // cbCpu
            // 
            this.cbCpu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCpu.FormattingEnabled = true;
            this.cbCpu.Location = new System.Drawing.Point(123, 76);
            this.cbCpu.Name = "cbCpu";
            this.cbCpu.Size = new System.Drawing.Size(498, 24);
            this.cbCpu.TabIndex = 9;
            // 
            // cbMotherboard
            // 
            this.cbMotherboard.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMotherboard.FormattingEnabled = true;
            this.cbMotherboard.Location = new System.Drawing.Point(123, 45);
            this.cbMotherboard.Name = "cbMotherboard";
            this.cbMotherboard.Size = new System.Drawing.Size(498, 24);
            this.cbMotherboard.TabIndex = 8;
            // 
            // lblCooling
            // 
            this.lblCooling.Location = new System.Drawing.Point(6, 265);
            this.lblCooling.Name = "lblCooling";
            this.lblCooling.Size = new System.Drawing.Size(111, 18);
            this.lblCooling.TabIndex = 7;
            this.lblCooling.Text = "Охлаждение";
            // 
            // lblCase
            // 
            this.lblCase.Location = new System.Drawing.Point(6, 234);
            this.lblCase.Name = "lblCase";
            this.lblCase.Size = new System.Drawing.Size(111, 18);
            this.lblCase.TabIndex = 6;
            this.lblCase.Text = "Корпус";
            // 
            // lblPsu
            // 
            this.lblPsu.Location = new System.Drawing.Point(6, 203);
            this.lblPsu.Name = "lblPsu";
            this.lblPsu.Size = new System.Drawing.Size(111, 18);
            this.lblPsu.TabIndex = 5;
            this.lblPsu.Text = "Блок питания";
            // 
            // lblStorage
            // 
            this.lblStorage.Location = new System.Drawing.Point(6, 172);
            this.lblStorage.Name = "lblStorage";
            this.lblStorage.Size = new System.Drawing.Size(111, 18);
            this.lblStorage.TabIndex = 4;
            this.lblStorage.Text = "Накопитель";
            // 
            // lblGpu
            // 
            this.lblGpu.Location = new System.Drawing.Point(6, 141);
            this.lblGpu.Name = "lblGpu";
            this.lblGpu.Size = new System.Drawing.Size(111, 18);
            this.lblGpu.TabIndex = 3;
            this.lblGpu.Text = "Видеокарта";
            // 
            // lblRam
            // 
            this.lblRam.Location = new System.Drawing.Point(6, 110);
            this.lblRam.Name = "lblRam";
            this.lblRam.Size = new System.Drawing.Size(111, 18);
            this.lblRam.TabIndex = 2;
            this.lblRam.Text = "ОЗУ";
            // 
            // lblCpu
            // 
            this.lblCpu.Location = new System.Drawing.Point(6, 79);
            this.lblCpu.Name = "lblCpu";
            this.lblCpu.Size = new System.Drawing.Size(111, 18);
            this.lblCpu.TabIndex = 1;
            this.lblCpu.Text = "Процессор";
            // 
            // lblMb
            // 
            this.lblMb.Location = new System.Drawing.Point(6, 48);
            this.lblMb.Name = "lblMb";
            this.lblMb.Size = new System.Drawing.Size(111, 18);
            this.lblMb.TabIndex = 0;
            this.lblMb.Text = "Материнка";
            // 
            // lblTotal
            // 
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblTotal.Location = new System.Drawing.Point(15, 542);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(240, 23);
            this.lblTotal.TabIndex = 6;
            this.lblTotal.Text = "Итого: 0.00";
            // 
            // btnSave
            // 
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnSave.Location = new System.Drawing.Point(452, 539);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(200, 30);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Сохранить как пресет";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnCancel.Location = new System.Drawing.Point(246, 539);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(200, 30);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // cbTargetUse
            // 
            this.cbTargetUse.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTargetUse.FormattingEnabled = true;
            this.cbTargetUse.Location = new System.Drawing.Point(118, 64);
            this.cbTargetUse.Name = "cbTargetUse";
            this.cbTargetUse.Size = new System.Drawing.Size(514, 21);
            this.cbTargetUse.TabIndex = 9;
            // 
            // chbRgb
            // 
            this.chbRgb.AutoSize = true;
            this.chbRgb.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chbRgb.Location = new System.Drawing.Point(544, 19);
            this.chbRgb.Name = "chbRgb";
            this.chbRgb.Size = new System.Drawing.Size(77, 20);
            this.chbRgb.TabIndex = 10;
            this.chbRgb.Text = "Подсветка";
            this.chbRgb.UseVisualStyleBackColor = true;
            // 
            // txtOtherOptions
            // 
            this.txtOtherOptions.Location = new System.Drawing.Point(118, 161);
            this.txtOtherOptions.Multiline = true;
            this.txtOtherOptions.Name = "txtOtherOptions";
            this.txtOtherOptions.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOtherOptions.Size = new System.Drawing.Size(514, 60);
            this.txtOtherOptions.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(31, 161);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 18);
            this.label1.TabIndex = 12;
            this.label1.Text = "Доп. опции";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(25, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 18);
            this.label2.TabIndex = 13;
            this.label2.Text = "Назначение";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ConfiguratorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 581);
            this.ControlBox = false;
            this.Controls.Add(this.cbTargetUse);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtOtherOptions);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.lblDesc);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.lblHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ConfiguratorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Shown += new System.EventHandler(this.ConfiguratorForm_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblDesc;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbCooling;
        private System.Windows.Forms.ComboBox cbCase;
        private System.Windows.Forms.ComboBox cbPsu;
        private System.Windows.Forms.ComboBox cbStorage;
        private System.Windows.Forms.ComboBox cbGpu;
        private System.Windows.Forms.ComboBox cbRam;
        private System.Windows.Forms.ComboBox cbCpu;
        private System.Windows.Forms.ComboBox cbMotherboard;
        private System.Windows.Forms.Label lblCooling;
        private System.Windows.Forms.Label lblCase;
        private System.Windows.Forms.Label lblPsu;
        private System.Windows.Forms.Label lblStorage;
        private System.Windows.Forms.Label lblGpu;
        private System.Windows.Forms.Label lblRam;
        private System.Windows.Forms.Label lblCpu;
        private System.Windows.Forms.Label lblMb;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chbRgb;
        private System.Windows.Forms.TextBox txtOtherOptions;
        private System.Windows.Forms.ComboBox cbTargetUse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}