namespace WindowsFormsApp1.ConfigForms
{
    partial class BuildsForm
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
            this.flpCards = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCreate = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.chkOnlyPresets = new System.Windows.Forms.CheckBox();
            this.cmbSort = new System.Windows.Forms.ComboBox();
            this.labelCount = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // flpCards
            // 
            this.flpCards.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpCards.AutoScroll = true;
            this.flpCards.Location = new System.Drawing.Point(12, 100);
            this.flpCards.Name = "flpCards";
            this.flpCards.Size = new System.Drawing.Size(860, 499);
            this.flpCards.TabIndex = 0;
            // 
            // btnCreate
            // 
            this.btnCreate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreate.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnCreate.Location = new System.Drawing.Point(140, 24);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(154, 30);
            this.btnCreate.TabIndex = 1;
            this.btnCreate.Text = "Создать сборку";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click_1);
            // 
            // btnRefresh
            // 
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnRefresh.Location = new System.Drawing.Point(14, 24);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(120, 30);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "Обновить";
            this.btnRefresh.UseVisualStyleBackColor = true;
            // 
            // chkOnlyPresets
            // 
            this.chkOnlyPresets.AutoSize = true;
            this.chkOnlyPresets.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.chkOnlyPresets.Location = new System.Drawing.Point(17, 70);
            this.chkOnlyPresets.Name = "chkOnlyPresets";
            this.chkOnlyPresets.Size = new System.Drawing.Size(130, 20);
            this.chkOnlyPresets.TabIndex = 3;
            this.chkOnlyPresets.Text = "Готовый пресет";
            this.chkOnlyPresets.UseVisualStyleBackColor = true;
            // 
            // cmbSort
            // 
            this.cmbSort.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.cmbSort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSort.Font = new System.Drawing.Font("Arial", 9F);
            this.cmbSort.FormattingEnabled = true;
            this.cmbSort.Items.AddRange(new object[] {
            "Сначала дорогие",
            "Сначала дешевые"});
            this.cmbSort.Location = new System.Drawing.Point(489, 28);
            this.cmbSort.Name = "cmbSort";
            this.cmbSort.Size = new System.Drawing.Size(198, 23);
            this.cmbSort.TabIndex = 4;
            this.cmbSort.SelectedIndexChanged += new System.EventHandler(this.cmbSortByPrice_SelectedIndexChanged);
            // 
            // labelCount
            // 
            this.labelCount.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.labelCount.AutoSize = true;
            this.labelCount.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.labelCount.Location = new System.Drawing.Point(693, 32);
            this.labelCount.Name = "labelCount";
            this.labelCount.Size = new System.Drawing.Size(71, 15);
            this.labelCount.TabIndex = 8;
            this.labelCount.Text = "Найдено: 0";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(405, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 15);
            this.label1.TabIndex = 9;
            this.label1.Text = "Сортировка:";
            // 
            // BuildsForm
            // 
            this.ClientSize = new System.Drawing.Size(884, 600);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelCount);
            this.Controls.Add(this.cmbSort);
            this.Controls.Add(this.chkOnlyPresets);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.flpCards);
            this.Name = "BuildsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Сборки ПК";
            this.Shown += new System.EventHandler(this.BuildsForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpCards;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.CheckBox chkOnlyPresets;
        private System.Windows.Forms.CheckBox chkSortByPrice;           // Новый фильтр для сортировки по цене
        private System.Windows.Forms.ComboBox cmbSort;   // ComboBox для сортировки по цене
        private System.Windows.Forms.Label labelCount;
        private System.Windows.Forms.Label label1;
    }
}