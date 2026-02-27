namespace WindowsFormsApp1.Orders
{
    partial class OrderCardControl
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
            this.panelRoot = new System.Windows.Forms.Panel();
            this.btnEdit = new System.Windows.Forms.Button();
            this.lnkConfig = new System.Windows.Forms.LinkLabel();
            this.lblPaid = new System.Windows.Forms.Label();
            this.lblPayment = new System.Windows.Forms.Label();
            this.lblAddress = new System.Windows.Forms.Label();
            this.lblDelivery = new System.Windows.Forms.Label();
            this.lblPrice = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblEmail = new System.Windows.Forms.Label();
            this.lblDate = new System.Windows.Forms.Label();
            this.lblOrderId = new System.Windows.Forms.Label();
            this.panelRoot.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelRoot
            // 
            this.panelRoot.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelRoot.Controls.Add(this.btnEdit);
            this.panelRoot.Controls.Add(this.lnkConfig);
            this.panelRoot.Controls.Add(this.lblPaid);
            this.panelRoot.Controls.Add(this.lblPayment);
            this.panelRoot.Controls.Add(this.lblAddress);
            this.panelRoot.Controls.Add(this.lblDelivery);
            this.panelRoot.Controls.Add(this.lblPrice);
            this.panelRoot.Controls.Add(this.lblStatus);
            this.panelRoot.Controls.Add(this.lblEmail);
            this.panelRoot.Controls.Add(this.lblDate);
            this.panelRoot.Controls.Add(this.lblOrderId);
            this.panelRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRoot.Location = new System.Drawing.Point(0, 0);
            this.panelRoot.Name = "panelRoot";
            this.panelRoot.Padding = new System.Windows.Forms.Padding(10);
            this.panelRoot.Size = new System.Drawing.Size(330, 170);
            this.panelRoot.TabIndex = 0;
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEdit.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.btnEdit.Location = new System.Drawing.Point(210, 128);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(105, 30);
            this.btnEdit.TabIndex = 10;
            this.btnEdit.Text = "Статус/оплата";
            this.btnEdit.UseVisualStyleBackColor = true;
            // 
            // lnkConfig
            // 
            this.lnkConfig.AutoEllipsis = true;
            this.lnkConfig.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Underline);
            this.lnkConfig.Location = new System.Drawing.Point(13, 48);
            this.lnkConfig.Name = "lnkConfig";
            this.lnkConfig.Size = new System.Drawing.Size(195, 18);
            this.lnkConfig.TabIndex = 3;
            this.lnkConfig.TabStop = true;
            this.lnkConfig.Text = "Конфигурация";
            // 
            // lblPaid
            // 
            this.lblPaid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPaid.AutoSize = true;
            this.lblPaid.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.lblPaid.Location = new System.Drawing.Point(224, 110);
            this.lblPaid.Name = "lblPaid";
            this.lblPaid.Size = new System.Drawing.Size(75, 15);
            this.lblPaid.TabIndex = 9;
            this.lblPaid.Text = "Не оплачен";
            // 
            // lblPayment
            // 
            this.lblPayment.AutoSize = true;
            this.lblPayment.Font = new System.Drawing.Font("Arial", 9F);
            this.lblPayment.Location = new System.Drawing.Point(13, 110);
            this.lblPayment.Name = "lblPayment";
            this.lblPayment.Size = new System.Drawing.Size(49, 15);
            this.lblPayment.TabIndex = 8;
            this.lblPayment.Text = "Оплата";
            // 
            // lblAddress
            // 
            this.lblAddress.AutoEllipsis = true;
            this.lblAddress.Font = new System.Drawing.Font("Arial", 9F);
            this.lblAddress.Location = new System.Drawing.Point(13, 93);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(195, 15);
            this.lblAddress.TabIndex = 7;
            this.lblAddress.Text = "Адрес";
            // 
            // lblDelivery
            // 
            this.lblDelivery.AutoSize = true;
            this.lblDelivery.Font = new System.Drawing.Font("Arial", 9F);
            this.lblDelivery.Location = new System.Drawing.Point(13, 76);
            this.lblDelivery.Name = "lblDelivery";
            this.lblDelivery.Size = new System.Drawing.Size(59, 15);
            this.lblDelivery.TabIndex = 6;
            this.lblDelivery.Text = "Доставка";
            // 
            // lblPrice
            // 
            this.lblPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPrice.AutoSize = true;
            this.lblPrice.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.lblPrice.Location = new System.Drawing.Point(222, 95);
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new System.Drawing.Size(41, 15);
            this.lblPrice.TabIndex = 5;
            this.lblPrice.Text = "0.00 €";
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatus.Location = new System.Drawing.Point(13, 143);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(45, 15);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Статус";
            // 
            // lblEmail
            // 
            this.lblEmail.AutoEllipsis = true;
            this.lblEmail.Font = new System.Drawing.Font("Arial", 9F);
            this.lblEmail.Location = new System.Drawing.Point(13, 23);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(195, 15);
            this.lblEmail.TabIndex = 1;
            this.lblEmail.Text = "email";
            // 
            // lblDate
            // 
            this.lblDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDate.AutoSize = true;
            this.lblDate.Font = new System.Drawing.Font("Arial", 9F);
            this.lblDate.Location = new System.Drawing.Point(236, 4);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(33, 15);
            this.lblDate.TabIndex = 11;
            this.lblDate.Text = "дата";
            // 
            // lblOrderId
            // 
            this.lblOrderId.AutoSize = true;
            this.lblOrderId.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lblOrderId.Location = new System.Drawing.Point(11, 4);
            this.lblOrderId.Name = "lblOrderId";
            this.lblOrderId.Size = new System.Drawing.Size(74, 16);
            this.lblOrderId.TabIndex = 0;
            this.lblOrderId.Text = "Заказ №0";
            // 
            // OrderCardControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.panelRoot);
            this.Font = new System.Drawing.Font("Arial", 9F);
            this.Name = "OrderCardControl";
            this.Size = new System.Drawing.Size(330, 170);
            this.panelRoot.ResumeLayout(false);
            this.panelRoot.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelRoot;
        private System.Windows.Forms.Label lblOrderId;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.LinkLabel lnkConfig;
        private System.Windows.Forms.Label lblPrice;
        private System.Windows.Forms.Label lblDelivery;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.Label lblPayment;
        private System.Windows.Forms.Label lblPaid;
        private System.Windows.Forms.Button btnEdit;
    }
}
