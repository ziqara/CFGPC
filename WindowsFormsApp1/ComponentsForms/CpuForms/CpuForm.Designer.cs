namespace WindowsFormsApp1.ComponentsForms
{
    partial class CpuForm
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
            this.cpuDataTable = new System.Windows.Forms.DataGridView();
            this.txtSearchName = new System.Windows.Forms.TextBox();
            this.cbxOnlyAvailable = new System.Windows.Forms.CheckBox();
            this.btnAddCpu = new System.Windows.Forms.Button();
            this.btnEditCpu = new System.Windows.Forms.Button();
            this.btnDeleteCpu = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.cpuDataTable)).BeginInit();
            this.SuspendLayout();
            // 
            // cpuDataTable
            // 
            this.cpuDataTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.cpuDataTable.Location = new System.Drawing.Point(12, 47);
            this.cpuDataTable.Name = "cpuDataTable";
            this.cpuDataTable.Size = new System.Drawing.Size(1232, 336);
            this.cpuDataTable.TabIndex = 0;
            // 
            // txtSearchName
            // 
            this.txtSearchName.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtSearchName.Location = new System.Drawing.Point(155, 21);
            this.txtSearchName.Name = "txtSearchName";
            this.txtSearchName.Size = new System.Drawing.Size(152, 22);
            this.txtSearchName.TabIndex = 1;
            this.txtSearchName.TextChanged += new System.EventHandler(this.txtSearchName_TextChanged);
            // 
            // cbxOnlyAvailable
            // 
            this.cbxOnlyAvailable.AutoSize = true;
            this.cbxOnlyAvailable.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cbxOnlyAvailable.Location = new System.Drawing.Point(1157, 21);
            this.cbxOnlyAvailable.Name = "cbxOnlyAvailable";
            this.cbxOnlyAvailable.Size = new System.Drawing.Size(87, 20);
            this.cbxOnlyAvailable.TabIndex = 2;
            this.cbxOnlyAvailable.Text = "Доступен";
            this.cbxOnlyAvailable.UseVisualStyleBackColor = true;
            this.cbxOnlyAvailable.CheckedChanged += new System.EventHandler(this.cbxOnlyAvailable_CheckedChanged);
            // 
            // btnAddCpu
            // 
            this.btnAddCpu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddCpu.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnAddCpu.Location = new System.Drawing.Point(12, 389);
            this.btnAddCpu.Name = "btnAddCpu";
            this.btnAddCpu.Size = new System.Drawing.Size(86, 30);
            this.btnAddCpu.TabIndex = 3;
            this.btnAddCpu.Text = "Добавить";
            this.btnAddCpu.UseVisualStyleBackColor = true;
            this.btnAddCpu.Click += new System.EventHandler(this.btnAddCpu_Click);
            // 
            // btnEditCpu
            // 
            this.btnEditCpu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEditCpu.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnEditCpu.Location = new System.Drawing.Point(104, 389);
            this.btnEditCpu.Name = "btnEditCpu";
            this.btnEditCpu.Size = new System.Drawing.Size(137, 30);
            this.btnEditCpu.TabIndex = 4;
            this.btnEditCpu.Text = "Редактировать";
            this.btnEditCpu.UseVisualStyleBackColor = true;
            this.btnEditCpu.Click += new System.EventHandler(this.btnEditCpu_Click);
            // 
            // btnDeleteCpu
            // 
            this.btnDeleteCpu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteCpu.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnDeleteCpu.Location = new System.Drawing.Point(247, 389);
            this.btnDeleteCpu.Name = "btnDeleteCpu";
            this.btnDeleteCpu.Size = new System.Drawing.Size(94, 30);
            this.btnDeleteCpu.TabIndex = 5;
            this.btnDeleteCpu.Text = "Удалить";
            this.btnDeleteCpu.UseVisualStyleBackColor = true;
            this.btnDeleteCpu.Click += new System.EventHandler(this.btnDeleteCpu_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(9, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 16);
            this.label1.TabIndex = 6;
            this.label1.Text = "Поиск по названию:";
            // 
            // CpuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1256, 429);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnDeleteCpu);
            this.Controls.Add(this.btnEditCpu);
            this.Controls.Add(this.btnAddCpu);
            this.Controls.Add(this.cbxOnlyAvailable);
            this.Controls.Add(this.txtSearchName);
            this.Controls.Add(this.cpuDataTable);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "CpuForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Процессоры";
            this.Load += new System.EventHandler(this.CpuForm_Load);
            this.Shown += new System.EventHandler(this.CpuForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.cpuDataTable)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView cpuDataTable;
        private System.Windows.Forms.TextBox txtSearchName;
        private System.Windows.Forms.CheckBox cbxOnlyAvailable;
        private System.Windows.Forms.Button btnAddCpu;
        private System.Windows.Forms.Button btnEditCpu;
        private System.Windows.Forms.Button btnDeleteCpu;
        private System.Windows.Forms.Label label1;
    }
}