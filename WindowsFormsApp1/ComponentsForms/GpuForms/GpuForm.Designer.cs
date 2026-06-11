namespace WindowsFormsApp1.ComponentsForms.GpuForms
{
    partial class GpuForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.txtSearchName = new System.Windows.Forms.TextBox();
            this.cbxOnlyAvailable = new System.Windows.Forms.CheckBox();
            this.gpuDataTable = new System.Windows.Forms.DataGridView();
            this.btnAddCpu = new System.Windows.Forms.Button();
            this.btnEditCpu = new System.Windows.Forms.Button();
            this.btnDeleteCpu = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gpuDataTable)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(14, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Поиск по названию:";
            // 
            // txtSearchName
            // 
            this.txtSearchName.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtSearchName.Location = new System.Drawing.Point(145, 15);
            this.txtSearchName.Name = "txtSearchName";
            this.txtSearchName.Size = new System.Drawing.Size(250, 25);
            this.txtSearchName.TabIndex = 1;
            this.txtSearchName.TextChanged += new System.EventHandler(this.txtSearchName_TextChanged);
            // 
            // cbxOnlyAvailable
            // 
            this.cbxOnlyAvailable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxOnlyAvailable.AutoSize = true;
            this.cbxOnlyAvailable.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cbxOnlyAvailable.Location = new System.Drawing.Point(1410, 17);
            this.cbxOnlyAvailable.Name = "cbxOnlyAvailable";
            this.cbxOnlyAvailable.Size = new System.Drawing.Size(124, 21);
            this.cbxOnlyAvailable.TabIndex = 2;
            this.cbxOnlyAvailable.Text = "Только в наличии";
            this.cbxOnlyAvailable.UseVisualStyleBackColor = true;
            this.cbxOnlyAvailable.CheckedChanged += new System.EventHandler(this.cbxOnlyAvailable_CheckedChanged);
            // 
            // gpuDataTable
            // 
            this.gpuDataTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gpuDataTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gpuDataTable.Location = new System.Drawing.Point(14, 52);
            this.gpuDataTable.Name = "gpuDataTable";
            this.gpuDataTable.Size = new System.Drawing.Size(1522, 485);
            this.gpuDataTable.TabIndex = 3;
            // 
            // btnAddCpu
            // 
            this.btnAddCpu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddCpu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddCpu.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnAddCpu.Location = new System.Drawing.Point(14, 553);
            this.btnAddCpu.Name = "btnAddCpu";
            this.btnAddCpu.Size = new System.Drawing.Size(110, 32);
            this.btnAddCpu.TabIndex = 4;
            this.btnAddCpu.Text = "Добавить";
            this.btnAddCpu.UseVisualStyleBackColor = true;
            this.btnAddCpu.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnEditCpu
            // 
            this.btnEditCpu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEditCpu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEditCpu.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnEditCpu.Location = new System.Drawing.Point(130, 553);
            this.btnEditCpu.Name = "btnEditCpu";
            this.btnEditCpu.Size = new System.Drawing.Size(140, 32);
            this.btnEditCpu.TabIndex = 5;
            this.btnEditCpu.Text = "Редактировать";
            this.btnEditCpu.UseVisualStyleBackColor = true;
            this.btnEditCpu.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnDeleteCpu
            // 
            this.btnDeleteCpu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDeleteCpu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteCpu.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnDeleteCpu.Location = new System.Drawing.Point(276, 553);
            this.btnDeleteCpu.Name = "btnDeleteCpu";
            this.btnDeleteCpu.Size = new System.Drawing.Size(110, 32);
            this.btnDeleteCpu.TabIndex = 6;
            this.btnDeleteCpu.Text = "Удалить";
            this.btnDeleteCpu.UseVisualStyleBackColor = true;
            this.btnDeleteCpu.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // GpuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1550, 600);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSearchName);
            this.Controls.Add(this.cbxOnlyAvailable);
            this.Controls.Add(this.gpuDataTable);
            this.Controls.Add(this.btnAddCpu);
            this.Controls.Add(this.btnEditCpu);
            this.Controls.Add(this.btnDeleteCpu);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MinimumSize = new System.Drawing.Size(1000, 500);
            this.Name = "GpuForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Видеокарты";
            this.Load += new System.EventHandler(this.GpuForm_Load);
            this.Shown += new System.EventHandler(this.GpuForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.gpuDataTable)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSearchName;
        private System.Windows.Forms.CheckBox cbxOnlyAvailable;
        private System.Windows.Forms.DataGridView gpuDataTable;
        private System.Windows.Forms.Button btnAddCpu;
        private System.Windows.Forms.Button btnEditCpu;
        private System.Windows.Forms.Button btnDeleteCpu;
    }
}