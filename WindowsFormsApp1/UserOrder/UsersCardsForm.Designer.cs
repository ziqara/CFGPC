namespace WindowsFormsApp1.UserOrder
{
    partial class UsersCardsForm
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
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.cbxActiveOrders = new System.Windows.Forms.CheckBox();
            this.btnReload = new System.Windows.Forms.Button();
            this.flpUsers = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Location = new System.Drawing.Point(234, 110);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(35, 13);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "label1";
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(459, 102);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(100, 20);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // cbxActiveOrders
            // 
            this.cbxActiveOrders.AutoSize = true;
            this.cbxActiveOrders.Location = new System.Drawing.Point(327, 13);
            this.cbxActiveOrders.Name = "cbxActiveOrders";
            this.cbxActiveOrders.Size = new System.Drawing.Size(80, 17);
            this.cbxActiveOrders.TabIndex = 2;
            this.cbxActiveOrders.Text = "checkBox1";
            this.cbxActiveOrders.UseVisualStyleBackColor = true;
            this.cbxActiveOrders.CheckedChanged += new System.EventHandler(this.cbxActiveOrders_CheckedChanged);
            // 
            // btnReload
            // 
            this.btnReload.Location = new System.Drawing.Point(341, 102);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(75, 23);
            this.btnReload.TabIndex = 3;
            this.btnReload.Text = "button1";
            this.btnReload.UseVisualStyleBackColor = true;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // flpUsers
            // 
            this.flpUsers.AutoScroll = true;
            this.flpUsers.Location = new System.Drawing.Point(43, 131);
            this.flpUsers.Name = "flpUsers";
            this.flpUsers.Size = new System.Drawing.Size(808, 362);
            this.flpUsers.TabIndex = 4;
            // 
            // UsersCardsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(883, 485);
            this.Controls.Add(this.flpUsers);
            this.Controls.Add(this.btnReload);
            this.Controls.Add(this.cbxActiveOrders);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.labelTitle);
            this.Name = "UsersCardsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Пользователи";
            this.Load += new System.EventHandler(this.UsersCardsForm_Load);
            this.Shown += new System.EventHandler(this.UsersCardsForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.CheckBox cbxActiveOrders;
        private System.Windows.Forms.Button btnReload;
        private System.Windows.Forms.FlowLayoutPanel flpUsers;
    }
}