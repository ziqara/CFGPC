namespace WindowsFormsApp1.ComponentsForms
{
    partial class MainFormForComponents
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
            this.cpuButton = new System.Windows.Forms.Button();
            this.mBoardButton = new System.Windows.Forms.Button();
            this.ramButton = new System.Windows.Forms.Button();
            this.gpuButton = new System.Windows.Forms.Button();
            this.storageButton = new System.Windows.Forms.Button();
            this.psuButton = new System.Windows.Forms.Button();
            this.caseButton = new System.Windows.Forms.Button();
            this.coolingButton = new System.Windows.Forms.Button();
            this.mainTable = new System.Windows.Forms.TableLayoutPanel();
            this.mainTable.SuspendLayout();
            this.SuspendLayout();
            // 
            // cpuButton
            // 
            this.cpuButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpuButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cpuButton.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cpuButton.Location = new System.Drawing.Point(55, 55);
            this.cpuButton.Margin = new System.Windows.Forms.Padding(15);
            this.cpuButton.Name = "cpuButton";
            this.cpuButton.Size = new System.Drawing.Size(367, 66);
            this.cpuButton.TabIndex = 0;
            this.cpuButton.Text = "Процессоры";
            this.cpuButton.UseVisualStyleBackColor = true;
            this.cpuButton.Click += new System.EventHandler(this.cpuButton_Click);
            // 
            // mBoardButton
            // 
            this.mBoardButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mBoardButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mBoardButton.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.mBoardButton.Location = new System.Drawing.Point(452, 55);
            this.mBoardButton.Margin = new System.Windows.Forms.Padding(15);
            this.mBoardButton.Name = "mBoardButton";
            this.mBoardButton.Size = new System.Drawing.Size(368, 66);
            this.mBoardButton.TabIndex = 1;
            this.mBoardButton.Text = "Материнские платы";
            this.mBoardButton.UseVisualStyleBackColor = true;
            this.mBoardButton.Click += new System.EventHandler(this.mBoardButton_Click_1);
            // 
            // ramButton
            // 
            this.ramButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ramButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ramButton.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ramButton.Location = new System.Drawing.Point(55, 151);
            this.ramButton.Margin = new System.Windows.Forms.Padding(15);
            this.ramButton.Name = "ramButton";
            this.ramButton.Size = new System.Drawing.Size(367, 66);
            this.ramButton.TabIndex = 2;
            this.ramButton.Text = "Оперативная память";
            this.ramButton.UseVisualStyleBackColor = true;
            this.ramButton.Click += new System.EventHandler(this.ramButton_Click);
            // 
            // gpuButton
            // 
            this.gpuButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpuButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.gpuButton.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.gpuButton.Location = new System.Drawing.Point(452, 151);
            this.gpuButton.Margin = new System.Windows.Forms.Padding(15);
            this.gpuButton.Name = "gpuButton";
            this.gpuButton.Size = new System.Drawing.Size(368, 66);
            this.gpuButton.TabIndex = 3;
            this.gpuButton.Text = "Видеокарты";
            this.gpuButton.UseVisualStyleBackColor = true;
            this.gpuButton.Click += new System.EventHandler(this.gpuButton_Click);
            // 
            // storageButton
            // 
            this.storageButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.storageButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.storageButton.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.storageButton.Location = new System.Drawing.Point(55, 247);
            this.storageButton.Margin = new System.Windows.Forms.Padding(15);
            this.storageButton.Name = "storageButton";
            this.storageButton.Size = new System.Drawing.Size(367, 66);
            this.storageButton.TabIndex = 4;
            this.storageButton.Text = "Накопители";
            this.storageButton.UseVisualStyleBackColor = true;
            this.storageButton.Click += new System.EventHandler(this.storageButton_Click);
            // 
            // psuButton
            // 
            this.psuButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.psuButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.psuButton.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.psuButton.Location = new System.Drawing.Point(452, 247);
            this.psuButton.Margin = new System.Windows.Forms.Padding(15);
            this.psuButton.Name = "psuButton";
            this.psuButton.Size = new System.Drawing.Size(368, 66);
            this.psuButton.TabIndex = 5;
            this.psuButton.Text = "Блок питания";
            this.psuButton.UseVisualStyleBackColor = true;
            this.psuButton.Click += new System.EventHandler(this.psuButton_Click);
            // 
            // caseButton
            // 
            this.caseButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.caseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.caseButton.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.caseButton.Location = new System.Drawing.Point(55, 343);
            this.caseButton.Margin = new System.Windows.Forms.Padding(15);
            this.caseButton.Name = "caseButton";
            this.caseButton.Size = new System.Drawing.Size(367, 66);
            this.caseButton.TabIndex = 6;
            this.caseButton.Text = "Корпусы";
            this.caseButton.UseVisualStyleBackColor = true;
            this.caseButton.Click += new System.EventHandler(this.caseButton_Click);
            // 
            // coolingButton
            // 
            this.coolingButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.coolingButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.coolingButton.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.coolingButton.Location = new System.Drawing.Point(452, 343);
            this.coolingButton.Margin = new System.Windows.Forms.Padding(15);
            this.coolingButton.Name = "coolingButton";
            this.coolingButton.Size = new System.Drawing.Size(368, 66);
            this.coolingButton.TabIndex = 7;
            this.coolingButton.Text = "Системы охлаждения процессора";
            this.coolingButton.UseVisualStyleBackColor = true;
            this.coolingButton.Click += new System.EventHandler(this.coolingButton_Click);
            // 
            // mainTable
            // 
            this.mainTable.ColumnCount = 2;
            this.mainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainTable.Controls.Add(this.cpuButton, 0, 0);
            this.mainTable.Controls.Add(this.coolingButton, 1, 3);
            this.mainTable.Controls.Add(this.mBoardButton, 1, 0);
            this.mainTable.Controls.Add(this.caseButton, 0, 3);
            this.mainTable.Controls.Add(this.ramButton, 0, 1);
            this.mainTable.Controls.Add(this.psuButton, 1, 2);
            this.mainTable.Controls.Add(this.gpuButton, 1, 1);
            this.mainTable.Controls.Add(this.storageButton, 0, 2);
            this.mainTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTable.Location = new System.Drawing.Point(0, 0);
            this.mainTable.Name = "mainTable";
            this.mainTable.Padding = new System.Windows.Forms.Padding(40);
            this.mainTable.RowCount = 4;
            this.mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.mainTable.Size = new System.Drawing.Size(875, 464);
            this.mainTable.TabIndex = 8;
            // 
            // MainFormForComponents
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(875, 464);
            this.Controls.Add(this.mainTable);
            this.Name = "MainFormForComponents";
            this.Text = "Компоненты";
            this.Load += new System.EventHandler(this.MainFormForComponents_Load);
            this.mainTable.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cpuButton;
        private System.Windows.Forms.Button mBoardButton;
        private System.Windows.Forms.Button ramButton;
        private System.Windows.Forms.Button gpuButton;
        private System.Windows.Forms.Button storageButton;
        private System.Windows.Forms.Button psuButton;
        private System.Windows.Forms.Button caseButton;
        private System.Windows.Forms.Button coolingButton;
        private System.Windows.Forms.TableLayoutPanel mainTable;
    }
}