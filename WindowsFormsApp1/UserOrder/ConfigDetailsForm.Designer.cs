namespace WindowsFormsApp1.UserOrder
{
    partial class ConfigDetailsForm
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
            this.lblConfig = new System.Windows.Forms.Label();
            this.dgvComponents = new System.Windows.Forms.DataGridView();
            this.btnOk = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvComponents)).BeginInit();
            this.SuspendLayout();
            // 
            // lblConfig
            // 
            this.lblConfig.AutoSize = true;
            this.lblConfig.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblConfig.Location = new System.Drawing.Point(12, 241);
            this.lblConfig.Name = "lblConfig";
            this.lblConfig.Size = new System.Drawing.Size(46, 16);
            this.lblConfig.TabIndex = 1;
            this.lblConfig.Text = "label2";
            // 
            // dgvComponents
            // 
            this.dgvComponents.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvComponents.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgvComponents.Location = new System.Drawing.Point(0, 0);
            this.dgvComponents.Name = "dgvComponents";
            this.dgvComponents.Size = new System.Drawing.Size(855, 230);
            this.dgvComponents.TabIndex = 2;
            // 
            // btnOk
            // 
            this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOk.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnOk.Location = new System.Drawing.Point(390, 238);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(85, 27);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "Закрыть";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // ConfigDetailsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(855, 277);
            this.ControlBox = false;
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.dgvComponents);
            this.Controls.Add(this.lblConfig);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ConfigDetailsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.ConfigDetailsForm_Load);
            this.Shown += new System.EventHandler(this.ConfigDetailsForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvComponents)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblConfig;
        private System.Windows.Forms.DataGridView dgvComponents;
        private System.Windows.Forms.Button btnOk;
    }
}