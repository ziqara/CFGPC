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
            ((System.ComponentModel.ISupportInitialize)(this.ordersDataTable)).BeginInit();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Location = new System.Drawing.Point(223, 84);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(35, 13);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "label1";
            // 
            // ordersDataTable
            // 
            this.ordersDataTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ordersDataTable.Location = new System.Drawing.Point(71, 114);
            this.ordersDataTable.Name = "ordersDataTable";
            this.ordersDataTable.Size = new System.Drawing.Size(551, 179);
            this.ordersDataTable.TabIndex = 1;
            // 
            // btnEditStatus
            // 
            this.btnEditStatus.Location = new System.Drawing.Point(71, 299);
            this.btnEditStatus.Name = "btnEditStatus";
            this.btnEditStatus.Size = new System.Drawing.Size(75, 23);
            this.btnEditStatus.TabIndex = 2;
            this.btnEditStatus.Text = "button1";
            this.btnEditStatus.UseVisualStyleBackColor = true;
            this.btnEditStatus.Click += new System.EventHandler(this.btnEditStatus_Click);
            // 
            // btnReload
            // 
            this.btnReload.Location = new System.Drawing.Point(165, 299);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(75, 23);
            this.btnReload.TabIndex = 3;
            this.btnReload.Text = "button2";
            this.btnReload.UseVisualStyleBackColor = true;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // UserOrdersForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnReload);
            this.Controls.Add(this.btnEditStatus);
            this.Controls.Add(this.ordersDataTable);
            this.Controls.Add(this.labelTitle);
            this.Name = "UserOrdersForm";
            this.Text = "UserOrdersForm";
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
    }
}