namespace WindowsFormsApp1.Orders
{
    partial class AllOrdersCardsForm
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
            this.flowOrders = new System.Windows.Forms.FlowLayoutPanel();
            this.panelTop = new System.Windows.Forms.Panel();
            this.labelCount = new System.Windows.Forms.Label();
            this.tbSearch = new System.Windows.Forms.TextBox();
            this.chkActiveOnly = new System.Windows.Forms.CheckBox();
            this.cbSortDate = new System.Windows.Forms.ComboBox();
            this.cbStatus = new System.Windows.Forms.ComboBox();
            this.btnReload = new System.Windows.Forms.Button();
            this.labelTitle = new System.Windows.Forms.Label();
            this.btnBack = new System.Windows.Forms.Button();
            this.chkUnpaidOnly = new System.Windows.Forms.CheckBox();
            this.panelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowOrders
            // 
            this.flowOrders.AutoScroll = true;
            this.flowOrders.Location = new System.Drawing.Point(0, 69);
            this.flowOrders.Name = "flowOrders";
            this.flowOrders.Padding = new System.Windows.Forms.Padding(8);
            this.flowOrders.Size = new System.Drawing.Size(733, 370);
            this.flowOrders.TabIndex = 1;
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.SystemColors.Control;
            this.panelTop.Controls.Add(this.flowOrders);
            this.panelTop.Controls.Add(this.chkUnpaidOnly);
            this.panelTop.Controls.Add(this.labelCount);
            this.panelTop.Controls.Add(this.tbSearch);
            this.panelTop.Controls.Add(this.chkActiveOnly);
            this.panelTop.Controls.Add(this.cbSortDate);
            this.panelTop.Controls.Add(this.cbStatus);
            this.panelTop.Controls.Add(this.btnReload);
            this.panelTop.Controls.Add(this.btnBack);
            this.panelTop.Controls.Add(this.labelTitle);
            this.panelTop.Location = new System.Drawing.Point(0, 1);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(733, 439);
            this.panelTop.TabIndex = 2;
            // 
            // labelCount
            // 
            this.labelCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCount.AutoSize = true;
            this.labelCount.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.labelCount.Location = new System.Drawing.Point(541, 42);
            this.labelCount.Name = "labelCount";
            this.labelCount.Size = new System.Drawing.Size(71, 15);
            this.labelCount.TabIndex = 7;
            this.labelCount.Text = "Найдено: 0";
            // 
            // tbSearch
            // 
            this.tbSearch.Font = new System.Drawing.Font("Arial", 9F);
            this.tbSearch.Location = new System.Drawing.Point(12, 40);
            this.tbSearch.Name = "tbSearch";
            this.tbSearch.Size = new System.Drawing.Size(204, 21);
            this.tbSearch.TabIndex = 1;
            // 
            // chkActiveOnly
            // 
            this.chkActiveOnly.AutoSize = true;
            this.chkActiveOnly.Checked = true;
            this.chkActiveOnly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkActiveOnly.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.chkActiveOnly.Location = new System.Drawing.Point(394, 19);
            this.chkActiveOnly.Name = "chkActiveOnly";
            this.chkActiveOnly.Size = new System.Drawing.Size(126, 19);
            this.chkActiveOnly.TabIndex = 4;
            this.chkActiveOnly.Text = "Только активные";
            this.chkActiveOnly.UseVisualStyleBackColor = true;
            // 
            // cbSortDate
            // 
            this.cbSortDate.Font = new System.Drawing.Font("Arial", 9F);
            this.cbSortDate.FormattingEnabled = true;
            this.cbSortDate.Location = new System.Drawing.Point(394, 40);
            this.cbSortDate.Name = "cbSortDate";
            this.cbSortDate.Size = new System.Drawing.Size(134, 23);
            this.cbSortDate.TabIndex = 3;
            // 
            // cbStatus
            // 
            this.cbStatus.Font = new System.Drawing.Font("Arial", 9F);
            this.cbStatus.FormattingEnabled = true;
            this.cbStatus.Location = new System.Drawing.Point(222, 40);
            this.cbStatus.Name = "cbStatus";
            this.cbStatus.Size = new System.Drawing.Size(166, 23);
            this.cbStatus.TabIndex = 2;
            // 
            // btnReload
            // 
            this.btnReload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReload.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnReload.Location = new System.Drawing.Point(112, 7);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(105, 28);
            this.btnReload.TabIndex = 5;
            this.btnReload.Text = "Обновить";
            this.btnReload.UseVisualStyleBackColor = true;
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(12, 12);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(99, 19);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "Все заказы";
            // 
            // btnBack
            // 
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBack.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnBack.Location = new System.Drawing.Point(624, 35);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(97, 28);
            this.btnBack.TabIndex = 6;
            this.btnBack.Text = "Сбросить";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // chkUnpaidOnly
            // 
            this.chkUnpaidOnly.AutoSize = true;
            this.chkUnpaidOnly.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.chkUnpaidOnly.Location = new System.Drawing.Point(273, 19);
            this.chkUnpaidOnly.Name = "chkUnpaidOnly";
            this.chkUnpaidOnly.Size = new System.Drawing.Size(115, 19);
            this.chkUnpaidOnly.TabIndex = 8;
            this.chkUnpaidOnly.Text = "Неоплаченные";
            this.chkUnpaidOnly.UseVisualStyleBackColor = true;
            // 
            // AllOrdersCardsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(733, 560);
            this.Controls.Add(this.panelTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AllOrdersCardsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Заказы";
            this.Load += new System.EventHandler(this.AllOrdersCardsForm_Load);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.FlowLayoutPanel flowOrders;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label labelCount;
        private System.Windows.Forms.TextBox tbSearch;
        private System.Windows.Forms.CheckBox chkActiveOnly;
        private System.Windows.Forms.ComboBox cbSortDate;
        private System.Windows.Forms.ComboBox cbStatus;
        private System.Windows.Forms.Button btnReload;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.CheckBox chkUnpaidOnly;
    }
}