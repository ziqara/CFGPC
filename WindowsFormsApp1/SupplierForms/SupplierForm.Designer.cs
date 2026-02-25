namespace WindowsFormsApp1
{
    partial class SupplierForm
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

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.supplierDataTable = new System.Windows.Forms.DataGridView();
            this.btnAddSupplier = new System.Windows.Forms.Button();
            this.txtSearchName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.cbxAddres = new System.Windows.Forms.CheckBox();
            this.cbxPhone = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.supplierDataTable)).BeginInit();
            this.SuspendLayout();
            // 
            // supplierDataTable
            // 
            this.supplierDataTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.supplierDataTable.Location = new System.Drawing.Point(27, 47);
            this.supplierDataTable.Name = "supplierDataTable";
            this.supplierDataTable.ReadOnly = true;
            this.supplierDataTable.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.supplierDataTable.Size = new System.Drawing.Size(672, 334);
            this.supplierDataTable.TabIndex = 0;
            // 
            // btnAddSupplier
            // 
            this.btnAddSupplier.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddSupplier.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnAddSupplier.Location = new System.Drawing.Point(186, 387);
            this.btnAddSupplier.Name = "btnAddSupplier";
            this.btnAddSupplier.Size = new System.Drawing.Size(99, 36);
            this.btnAddSupplier.TabIndex = 1;
            this.btnAddSupplier.Text = "Добавить";
            this.btnAddSupplier.UseVisualStyleBackColor = true;
            this.btnAddSupplier.Click += new System.EventHandler(this.btnAddSupplier_Click);
            // 
            // txtSearchName
            // 
            this.txtSearchName.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtSearchName.Location = new System.Drawing.Point(170, 20);
            this.txtSearchName.Name = "txtSearchName";
            this.txtSearchName.Size = new System.Drawing.Size(186, 22);
            this.txtSearchName.TabIndex = 2;
            this.txtSearchName.TextChanged += new System.EventHandler(this.txtSearchName_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(24, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "Поиск по названию:";
            // 
            // btnEdit
            // 
            this.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEdit.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnEdit.Location = new System.Drawing.Point(291, 387);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(134, 36);
            this.btnEdit.TabIndex = 4;
            this.btnEdit.Text = "Редактировать";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnDelete.Location = new System.Drawing.Point(431, 387);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(93, 36);
            this.btnDelete.TabIndex = 5;
            this.btnDelete.Text = "Удалить";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // cbxAddres
            // 
            this.cbxAddres.AutoSize = true;
            this.cbxAddres.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cbxAddres.Location = new System.Drawing.Point(633, 22);
            this.cbxAddres.Name = "cbxAddres";
            this.cbxAddres.Size = new System.Drawing.Size(66, 20);
            this.cbxAddres.TabIndex = 6;
            this.cbxAddres.Text = "Адрес";
            this.cbxAddres.UseVisualStyleBackColor = true;
            this.cbxAddres.CheckedChanged += new System.EventHandler(this.cbxAddres_CheckedChanged);
            // 
            // cbxPhone
            // 
            this.cbxPhone.AutoSize = true;
            this.cbxPhone.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cbxPhone.Location = new System.Drawing.Point(542, 21);
            this.cbxPhone.Name = "cbxPhone";
            this.cbxPhone.Size = new System.Drawing.Size(85, 20);
            this.cbxPhone.TabIndex = 7;
            this.cbxPhone.Text = "Телефон";
            this.cbxPhone.UseVisualStyleBackColor = true;
            this.cbxPhone.CheckedChanged += new System.EventHandler(this.cbxPhone_CheckedChanged);
            // 
            // SupplierForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(883, 485);
            this.Controls.Add(this.cbxPhone);
            this.Controls.Add(this.cbxAddres);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSearchName);
            this.Controls.Add(this.btnAddSupplier);
            this.Controls.Add(this.supplierDataTable);
            this.Name = "SupplierForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Поставщики";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.SupplierForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.supplierDataTable)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView supplierDataTable;
        private System.Windows.Forms.Button btnAddSupplier;
        private System.Windows.Forms.TextBox txtSearchName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.CheckBox cbxAddres;
        private System.Windows.Forms.CheckBox cbxPhone;
    }
}

