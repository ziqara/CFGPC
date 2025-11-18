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
            ((System.ComponentModel.ISupportInitialize)(this.supplierDataTable)).BeginInit();
            this.SuspendLayout();
            // 
            // supplierDataTable
            // 
            this.supplierDataTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.supplierDataTable.Location = new System.Drawing.Point(277, 78);
            this.supplierDataTable.Name = "supplierDataTable";
            this.supplierDataTable.ReadOnly = true;
            this.supplierDataTable.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.supplierDataTable.Size = new System.Drawing.Size(544, 292);
            this.supplierDataTable.TabIndex = 0;
            // 
            // btnAddSupplier
            // 
            this.btnAddSupplier.Location = new System.Drawing.Point(692, 427);
            this.btnAddSupplier.Name = "btnAddSupplier";
            this.btnAddSupplier.Size = new System.Drawing.Size(75, 23);
            this.btnAddSupplier.TabIndex = 1;
            this.btnAddSupplier.Text = "Добавить";
            this.btnAddSupplier.UseVisualStyleBackColor = true;
            this.btnAddSupplier.Click += new System.EventHandler(this.btnAddSupplier_Click);
            // 
            // SupplierForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(883, 485);
            this.Controls.Add(this.btnAddSupplier);
            this.Controls.Add(this.supplierDataTable);
            this.Name = "SupplierForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Поставщики";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.supplierDataTable)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView supplierDataTable;
        private System.Windows.Forms.Button btnAddSupplier;
    }
}

