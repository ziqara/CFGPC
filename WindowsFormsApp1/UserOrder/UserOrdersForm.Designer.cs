namespace WindowsFormsApp1.UserOrder
{
    partial class UserOrdersForm
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
            this.labelTitle = new System.Windows.Forms.Label();
            this.ordersDataTable = new System.Windows.Forms.DataGridView();
            this.btnEditStatus = new System.Windows.Forms.Button();
            this.btnReload = new System.Windows.Forms.Button();
            this.cancelBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ordersDataTable)).BeginInit();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelTitle.Location = new System.Drawing.Point(12, 15);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(54, 19);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "label1";
            // 
            // ordersDataTable
            // 
            this.ordersDataTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ordersDataTable.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ordersDataTable.Location = new System.Drawing.Point(0, 50);
            this.ordersDataTable.Name = "ordersDataTable";
            this.ordersDataTable.Size = new System.Drawing.Size(1236, 295);
            this.ordersDataTable.TabIndex = 1;
            this.ordersDataTable.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.ordersDataTable_CellDoubleClick);
            this.ordersDataTable.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.ordersDataTable_CellMouseEnter);
            this.ordersDataTable.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.ordersDataTable_CellMouseLeave);
            this.ordersDataTable.CellToolTipTextNeeded += new System.Windows.Forms.DataGridViewCellToolTipTextNeededEventHandler(this.ordersDataTable_CellToolTipTextNeeded);
            // 
            // btnEditStatus
            // 
            this.btnEditStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEditStatus.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnEditStatus.Location = new System.Drawing.Point(796, 9);
            this.btnEditStatus.Name = "btnEditStatus";
            this.btnEditStatus.Size = new System.Drawing.Size(237, 33);
            this.btnEditStatus.TabIndex = 2;
            this.btnEditStatus.Text = "Редактировать статус заказа";
            this.btnEditStatus.UseVisualStyleBackColor = true;
            this.btnEditStatus.Click += new System.EventHandler(this.btnEditStatus_Click);
            // 
            // btnReload
            // 
            this.btnReload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReload.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnReload.Location = new System.Drawing.Point(1039, 9);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(106, 33);
            this.btnReload.TabIndex = 3;
            this.btnReload.Text = "Обновить";
            this.btnReload.UseVisualStyleBackColor = true;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // cancelBtn
            // 
            this.cancelBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cancelBtn.Location = new System.Drawing.Point(1151, 9);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(73, 33);
            this.cancelBtn.TabIndex = 4;
            this.cancelBtn.Text = "Назад";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // UserOrdersForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1236, 345);
            this.ControlBox = false;
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.btnReload);
            this.Controls.Add(this.btnEditStatus);
            this.Controls.Add(this.ordersDataTable);
            this.Controls.Add(this.labelTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "UserOrdersForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.UserOrdersForm_Load);
            this.Shown += new System.EventHandler(this.UserOrdersForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.ordersDataTable)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.DataGridView ordersDataTable;
        private System.Windows.Forms.Button btnEditStatus;
        private System.Windows.Forms.Button btnReload;
        private System.Windows.Forms.Button cancelBtn;
    }
}