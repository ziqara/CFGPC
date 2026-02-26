namespace WindowsFormsApp1.ConfigForms
{
    partial class BuildCardControl
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblMeta = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lblPreset = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoEllipsis = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(10, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(530, 22);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "#1 · Название сборки";
            // 
            // lblMeta
            // 
            this.lblMeta.AutoEllipsis = true;
            this.lblMeta.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblMeta.Location = new System.Drawing.Point(10, 36);
            this.lblMeta.Name = "lblMeta";
            this.lblMeta.Size = new System.Drawing.Size(530, 18);
            this.lblMeta.TabIndex = 1;
            this.lblMeta.Text = "Цена / дата / email";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoEllipsis = true;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblStatus.Location = new System.Drawing.Point(10, 76);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(530, 36);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "Статус";
            // 
            // btnDelete
            // 
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnDelete.Location = new System.Drawing.Point(550, 10);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(120, 30);
            this.btnDelete.TabIndex = 4;
            this.btnDelete.Text = "Удалить";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lblPreset
            // 
            this.lblPreset.AutoEllipsis = true;
            this.lblPreset.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPreset.Location = new System.Drawing.Point(10, 56);
            this.lblPreset.Name = "lblPreset";
            this.lblPreset.Size = new System.Drawing.Size(530, 18);
            this.lblPreset.TabIndex = 2;
            this.lblPreset.Text = "Готовый пресет: Да/Нет";
            // 
            // BuildCardControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblPreset);
            this.Controls.Add(this.lblMeta);
            this.Controls.Add(this.lblTitle);
            this.Name = "BuildCardControl";
            this.Size = new System.Drawing.Size(680, 120);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblMeta;
        private System.Windows.Forms.Label lblPreset;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnDelete;
    }
}
