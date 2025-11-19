namespace WindowsFormsApp1
{
    partial class UpdateMainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateMainForm));
            this.panelMenu = new System.Windows.Forms.Panel();
            this.btnReviews = new System.Windows.Forms.Button();
            this.btnWarrious = new System.Windows.Forms.Button();
            this.btnClients = new System.Windows.Forms.Button();
            this.btnSupplier = new System.Windows.Forms.Button();
            this.btnComponent = new System.Windows.Forms.Button();
            this.btnConfig = new System.Windows.Forms.Button();
            this.btnOrders = new System.Windows.Forms.Button();
            this.panelLogo = new System.Windows.Forms.Panel();
            this.logobox = new System.Windows.Forms.PictureBox();
            this.panelTitle = new System.Windows.Forms.Panel();
            this.lblTitile = new System.Windows.Forms.Label();
            this.panelDekstopPanel = new System.Windows.Forms.Panel();
            this.btnCloseChildForm = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panelMenu.SuspendLayout();
            this.panelLogo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logobox)).BeginInit();
            this.panelTitle.SuspendLayout();
            this.panelDekstopPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panelMenu
            // 
            this.panelMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(80)))), ((int)(((byte)(87)))));
            this.panelMenu.Controls.Add(this.btnReviews);
            this.panelMenu.Controls.Add(this.btnWarrious);
            this.panelMenu.Controls.Add(this.btnClients);
            this.panelMenu.Controls.Add(this.btnSupplier);
            this.panelMenu.Controls.Add(this.btnComponent);
            this.panelMenu.Controls.Add(this.btnConfig);
            this.panelMenu.Controls.Add(this.btnOrders);
            this.panelMenu.Controls.Add(this.panelLogo);
            this.panelMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelMenu.Location = new System.Drawing.Point(0, 0);
            this.panelMenu.Name = "panelMenu";
            this.panelMenu.Size = new System.Drawing.Size(220, 519);
            this.panelMenu.TabIndex = 0;
            // 
            // btnReviews
            // 
            this.btnReviews.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnReviews.FlatAppearance.BorderSize = 0;
            this.btnReviews.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReviews.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnReviews.ForeColor = System.Drawing.Color.White;
            this.btnReviews.Image = ((System.Drawing.Image)(resources.GetObject("btnReviews.Image")));
            this.btnReviews.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReviews.Location = new System.Drawing.Point(0, 440);
            this.btnReviews.Name = "btnReviews";
            this.btnReviews.Size = new System.Drawing.Size(220, 60);
            this.btnReviews.TabIndex = 7;
            this.btnReviews.Text = "     Отзывы";
            this.btnReviews.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReviews.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnReviews.UseVisualStyleBackColor = true;
            this.btnReviews.Click += new System.EventHandler(this.btnReviews_Click_1);
            // 
            // btnWarrious
            // 
            this.btnWarrious.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnWarrious.FlatAppearance.BorderSize = 0;
            this.btnWarrious.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnWarrious.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnWarrious.ForeColor = System.Drawing.Color.White;
            this.btnWarrious.Image = ((System.Drawing.Image)(resources.GetObject("btnWarrious.Image")));
            this.btnWarrious.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnWarrious.Location = new System.Drawing.Point(0, 380);
            this.btnWarrious.Name = "btnWarrious";
            this.btnWarrious.Size = new System.Drawing.Size(220, 60);
            this.btnWarrious.TabIndex = 6;
            this.btnWarrious.Text = "     Гарантия";
            this.btnWarrious.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnWarrious.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnWarrious.UseVisualStyleBackColor = true;
            this.btnWarrious.Click += new System.EventHandler(this.btnWarrious_Click);
            // 
            // btnClients
            // 
            this.btnClients.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnClients.FlatAppearance.BorderSize = 0;
            this.btnClients.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClients.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnClients.ForeColor = System.Drawing.Color.White;
            this.btnClients.Image = ((System.Drawing.Image)(resources.GetObject("btnClients.Image")));
            this.btnClients.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClients.Location = new System.Drawing.Point(0, 320);
            this.btnClients.Name = "btnClients";
            this.btnClients.Size = new System.Drawing.Size(220, 60);
            this.btnClients.TabIndex = 5;
            this.btnClients.Text = "     Клиенты";
            this.btnClients.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClients.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnClients.UseVisualStyleBackColor = true;
            this.btnClients.Click += new System.EventHandler(this.btnClients_Click_1);
            // 
            // btnSupplier
            // 
            this.btnSupplier.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnSupplier.FlatAppearance.BorderSize = 0;
            this.btnSupplier.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSupplier.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnSupplier.ForeColor = System.Drawing.Color.White;
            this.btnSupplier.Image = ((System.Drawing.Image)(resources.GetObject("btnSupplier.Image")));
            this.btnSupplier.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSupplier.Location = new System.Drawing.Point(0, 260);
            this.btnSupplier.Name = "btnSupplier";
            this.btnSupplier.Size = new System.Drawing.Size(220, 60);
            this.btnSupplier.TabIndex = 4;
            this.btnSupplier.Text = "     Поставщики";
            this.btnSupplier.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSupplier.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSupplier.UseVisualStyleBackColor = true;
            this.btnSupplier.Click += new System.EventHandler(this.btnSupplier_Click_1);
            // 
            // btnComponent
            // 
            this.btnComponent.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnComponent.FlatAppearance.BorderSize = 0;
            this.btnComponent.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnComponent.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnComponent.ForeColor = System.Drawing.Color.White;
            this.btnComponent.Image = ((System.Drawing.Image)(resources.GetObject("btnComponent.Image")));
            this.btnComponent.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnComponent.Location = new System.Drawing.Point(0, 200);
            this.btnComponent.Name = "btnComponent";
            this.btnComponent.Size = new System.Drawing.Size(220, 60);
            this.btnComponent.TabIndex = 3;
            this.btnComponent.Text = "     Компоненты";
            this.btnComponent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnComponent.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnComponent.UseVisualStyleBackColor = true;
            this.btnComponent.Click += new System.EventHandler(this.btnComponent_Click);
            // 
            // btnConfig
            // 
            this.btnConfig.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnConfig.FlatAppearance.BorderSize = 0;
            this.btnConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfig.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnConfig.ForeColor = System.Drawing.Color.White;
            this.btnConfig.Image = ((System.Drawing.Image)(resources.GetObject("btnConfig.Image")));
            this.btnConfig.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnConfig.Location = new System.Drawing.Point(0, 140);
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.Size = new System.Drawing.Size(220, 60);
            this.btnConfig.TabIndex = 2;
            this.btnConfig.Text = "     Сборки";
            this.btnConfig.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnConfig.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnConfig.UseVisualStyleBackColor = true;
            this.btnConfig.Click += new System.EventHandler(this.btnConfig_Click_1);
            // 
            // btnOrders
            // 
            this.btnOrders.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnOrders.FlatAppearance.BorderSize = 0;
            this.btnOrders.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOrders.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnOrders.ForeColor = System.Drawing.Color.White;
            this.btnOrders.Image = ((System.Drawing.Image)(resources.GetObject("btnOrders.Image")));
            this.btnOrders.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOrders.Location = new System.Drawing.Point(0, 80);
            this.btnOrders.Name = "btnOrders";
            this.btnOrders.Size = new System.Drawing.Size(220, 60);
            this.btnOrders.TabIndex = 1;
            this.btnOrders.Text = "     Заказы";
            this.btnOrders.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOrders.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnOrders.UseVisualStyleBackColor = true;
            this.btnOrders.Click += new System.EventHandler(this.btnOrders_Click_2);
            // 
            // panelLogo
            // 
            this.panelLogo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.panelLogo.Controls.Add(this.logobox);
            this.panelLogo.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelLogo.Location = new System.Drawing.Point(0, 0);
            this.panelLogo.Name = "panelLogo";
            this.panelLogo.Size = new System.Drawing.Size(220, 80);
            this.panelLogo.TabIndex = 1;
            // 
            // logobox
            // 
            this.logobox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logobox.Image = ((System.Drawing.Image)(resources.GetObject("logobox.Image")));
            this.logobox.Location = new System.Drawing.Point(0, 0);
            this.logobox.Name = "logobox";
            this.logobox.Size = new System.Drawing.Size(220, 80);
            this.logobox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.logobox.TabIndex = 0;
            this.logobox.TabStop = false;
            // 
            // panelTitle
            // 
            this.panelTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(80)))), ((int)(((byte)(87)))));
            this.panelTitle.Controls.Add(this.btnCloseChildForm);
            this.panelTitle.Controls.Add(this.lblTitile);
            this.panelTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTitle.Location = new System.Drawing.Point(220, 0);
            this.panelTitle.Name = "panelTitle";
            this.panelTitle.Size = new System.Drawing.Size(733, 80);
            this.panelTitle.TabIndex = 1;
            this.panelTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTitle_MouseDown);
            // 
            // lblTitile
            // 
            this.lblTitile.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblTitile.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblTitile.ForeColor = System.Drawing.Color.White;
            this.lblTitile.Location = new System.Drawing.Point(178, 27);
            this.lblTitile.Name = "lblTitile";
            this.lblTitile.Size = new System.Drawing.Size(374, 29);
            this.lblTitile.TabIndex = 0;
            this.lblTitile.Text = "Главная";
            this.lblTitile.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelDekstopPanel
            // 
            this.panelDekstopPanel.Controls.Add(this.pictureBox1);
            this.panelDekstopPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelDekstopPanel.Location = new System.Drawing.Point(220, 80);
            this.panelDekstopPanel.Name = "panelDekstopPanel";
            this.panelDekstopPanel.Size = new System.Drawing.Size(733, 439);
            this.panelDekstopPanel.TabIndex = 2;
            // 
            // btnCloseChildForm
            // 
            this.btnCloseChildForm.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnCloseChildForm.FlatAppearance.BorderSize = 0;
            this.btnCloseChildForm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCloseChildForm.Image = ((System.Drawing.Image)(resources.GetObject("btnCloseChildForm.Image")));
            this.btnCloseChildForm.Location = new System.Drawing.Point(0, 0);
            this.btnCloseChildForm.Name = "btnCloseChildForm";
            this.btnCloseChildForm.Size = new System.Drawing.Size(75, 80);
            this.btnCloseChildForm.TabIndex = 0;
            this.btnCloseChildForm.UseVisualStyleBackColor = true;
            this.btnCloseChildForm.Click += new System.EventHandler(this.btnCloseChildForm_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(33, 68);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(664, 265);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // UpdateMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(953, 519);
            this.Controls.Add(this.panelDekstopPanel);
            this.Controls.Add(this.panelTitle);
            this.Controls.Add(this.panelMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(969, 558);
            this.Name = "UpdateMainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Панель администрирования";
            this.panelMenu.ResumeLayout(false);
            this.panelLogo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.logobox)).EndInit();
            this.panelTitle.ResumeLayout(false);
            this.panelDekstopPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelMenu;
        private System.Windows.Forms.Panel panelLogo;
        private System.Windows.Forms.Button btnOrders;
        private System.Windows.Forms.Button btnComponent;
        private System.Windows.Forms.Button btnConfig;
        private System.Windows.Forms.Button btnWarrious;
        private System.Windows.Forms.Button btnClients;
        private System.Windows.Forms.Button btnSupplier;
        private System.Windows.Forms.Panel panelTitle;
        private System.Windows.Forms.Label lblTitile;
        private System.Windows.Forms.Panel panelDekstopPanel;
        private System.Windows.Forms.Button btnReviews;
        private System.Windows.Forms.PictureBox logobox;
        private System.Windows.Forms.Button btnCloseChildForm;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}