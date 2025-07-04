namespace AstronicAutoSupplyInventory.User
{
    partial class UserActivityForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lnkMyInventory = new System.Windows.Forms.LinkLabel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.pnlDateRange = new System.Windows.Forms.Panel();
            this.lblDateRange = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.dgvItems = new System.Windows.Forms.DataGridView();
            this.Column13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lnkHelp = new System.Windows.Forms.LinkLabel();
            this.pnlHelp = new System.Windows.Forms.Panel();
            this.panel46 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel47 = new System.Windows.Forms.Panel();
            this.label34 = new System.Windows.Forms.Label();
            this.panel48 = new System.Windows.Forms.Panel();
            this.label36 = new System.Windows.Forms.Label();
            this.panel49 = new System.Windows.Forms.Panel();
            this.label37 = new System.Windows.Forms.Label();
            this.panel44 = new System.Windows.Forms.Panel();
            this.label32 = new System.Windows.Forms.Label();
            this.panel45 = new System.Windows.Forms.Panel();
            this.label33 = new System.Windows.Forms.Label();
            this.panel2.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel7.SuspendLayout();
            this.pnlDateRange.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
            this.panel1.SuspendLayout();
            this.pnlHelp.SuspendLayout();
            this.panel46.SuspendLayout();
            this.panel48.SuspendLayout();
            this.panel44.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.LightBlue;
            this.panel2.Controls.Add(this.lnkMyInventory);
            this.panel2.Controls.Add(this.panel6);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(2);
            this.panel2.Size = new System.Drawing.Size(732, 49);
            this.panel2.TabIndex = 2;
            // 
            // lnkMyInventory
            // 
            this.lnkMyInventory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lnkMyInventory.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lnkMyInventory.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.lnkMyInventory.Location = new System.Drawing.Point(593, 2);
            this.lnkMyInventory.Name = "lnkMyInventory";
            this.lnkMyInventory.Size = new System.Drawing.Size(137, 45);
            this.lnkMyInventory.TabIndex = 2;
            this.lnkMyInventory.TabStop = true;
            this.lnkMyInventory.Text = "Filter by &date";
            this.lnkMyInventory.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lnkMyInventory.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkMyInventory_LinkClicked);
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.panel7);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel6.Location = new System.Drawing.Point(2, 2);
            this.panel6.Name = "panel6";
            this.panel6.Padding = new System.Windows.Forms.Padding(4);
            this.panel6.Size = new System.Drawing.Size(591, 45);
            this.panel6.TabIndex = 1;
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.White;
            this.panel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel7.Controls.Add(this.txtSearch);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(4, 4);
            this.panel7.Name = "panel7";
            this.panel7.Padding = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.panel7.Size = new System.Drawing.Size(583, 37);
            this.panel7.TabIndex = 2;
            // 
            // txtSearch
            // 
            this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSearch.CausesValidation = false;
            this.txtSearch.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSearch.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearch.ForeColor = System.Drawing.Color.Black;
            this.txtSearch.Location = new System.Drawing.Point(4, 2);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(573, 30);
            this.txtSearch.TabIndex = 0;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // pnlDateRange
            // 
            this.pnlDateRange.BackColor = System.Drawing.Color.LightBlue;
            this.pnlDateRange.Controls.Add(this.lblDateRange);
            this.pnlDateRange.Controls.Add(this.panel3);
            this.pnlDateRange.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDateRange.Location = new System.Drawing.Point(0, 49);
            this.pnlDateRange.Name = "pnlDateRange";
            this.pnlDateRange.Padding = new System.Windows.Forms.Padding(2);
            this.pnlDateRange.Size = new System.Drawing.Size(732, 37);
            this.pnlDateRange.TabIndex = 7;
            this.pnlDateRange.Visible = false;
            // 
            // lblDateRange
            // 
            this.lblDateRange.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDateRange.Font = new System.Drawing.Font("Calibri", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDateRange.ForeColor = System.Drawing.Color.DimGray;
            this.lblDateRange.Location = new System.Drawing.Point(2, 2);
            this.lblDateRange.Name = "lblDateRange";
            this.lblDateRange.Size = new System.Drawing.Size(697, 33);
            this.lblDateRange.TabIndex = 2;
            this.lblDateRange.Text = "Date Range";
            this.lblDateRange.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btnClose);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(699, 2);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.panel3.Size = new System.Drawing.Size(31, 33);
            this.panel3.TabIndex = 8;
            // 
            // btnClose
            // 
            this.btnClose.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(0, 2);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(31, 29);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "X";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // dgvItems
            // 
            this.dgvItems.AllowUserToAddRows = false;
            this.dgvItems.AllowUserToDeleteRows = false;
            this.dgvItems.BackgroundColor = System.Drawing.SystemColors.ControlDark;
            this.dgvItems.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvItems.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column13,
            this.Column1,
            this.Column2});
            this.dgvItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvItems.Location = new System.Drawing.Point(0, 86);
            this.dgvItems.Name = "dgvItems";
            this.dgvItems.ReadOnly = true;
            this.dgvItems.RowHeadersWidth = 20;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvItems.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvItems.Size = new System.Drawing.Size(732, 322);
            this.dgvItems.TabIndex = 10;
            // 
            // Column13
            // 
            this.Column13.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column13.HeaderText = "Action";
            this.Column13.Name = "Column13";
            this.Column13.ReadOnly = true;
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column1.HeaderText = "Date";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column2.HeaderText = "Performed by";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lnkHelp);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 408);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(2);
            this.panel1.Size = new System.Drawing.Size(732, 25);
            this.panel1.TabIndex = 11;
            // 
            // lnkHelp
            // 
            this.lnkHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkHelp.AutoSize = true;
            this.lnkHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lnkHelp.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.lnkHelp.Location = new System.Drawing.Point(656, 5);
            this.lnkHelp.Name = "lnkHelp";
            this.lnkHelp.Size = new System.Drawing.Size(71, 15);
            this.lnkHelp.TabIndex = 29;
            this.lnkHelp.TabStop = true;
            this.lnkHelp.Text = "Need &help?";
            this.lnkHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkHelp_LinkClicked);
            // 
            // pnlHelp
            // 
            this.pnlHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlHelp.BackColor = System.Drawing.Color.Silver;
            this.pnlHelp.Controls.Add(this.panel46);
            this.pnlHelp.Controls.Add(this.panel48);
            this.pnlHelp.Controls.Add(this.panel44);
            this.pnlHelp.Location = new System.Drawing.Point(464, 302);
            this.pnlHelp.Name = "pnlHelp";
            this.pnlHelp.Padding = new System.Windows.Forms.Padding(2);
            this.pnlHelp.Size = new System.Drawing.Size(262, 112);
            this.pnlHelp.TabIndex = 30;
            this.pnlHelp.Visible = false;
            // 
            // panel46
            // 
            this.panel46.BackColor = System.Drawing.SystemColors.Control;
            this.panel46.Controls.Add(this.label1);
            this.panel46.Controls.Add(this.panel47);
            this.panel46.Controls.Add(this.label34);
            this.panel46.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel46.Location = new System.Drawing.Point(2, 74);
            this.panel46.Name = "panel46";
            this.panel46.Padding = new System.Windows.Forms.Padding(2);
            this.panel46.Size = new System.Drawing.Size(258, 36);
            this.panel46.TabIndex = 45;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(87, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(169, 32);
            this.label1.TabIndex = 1;
            this.label1.Text = "Press \'ESC\' to cancel.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel47
            // 
            this.panel47.BackColor = System.Drawing.Color.DarkGray;
            this.panel47.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel47.Location = new System.Drawing.Point(84, 2);
            this.panel47.Name = "panel47";
            this.panel47.Size = new System.Drawing.Size(3, 32);
            this.panel47.TabIndex = 2;
            // 
            // label34
            // 
            this.label34.BackColor = System.Drawing.Color.MistyRose;
            this.label34.Dock = System.Windows.Forms.DockStyle.Left;
            this.label34.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label34.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label34.Location = new System.Drawing.Point(2, 2);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(82, 32);
            this.label34.TabIndex = 0;
            this.label34.Text = "ESC";
            this.label34.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel48
            // 
            this.panel48.BackColor = System.Drawing.SystemColors.Control;
            this.panel48.Controls.Add(this.label36);
            this.panel48.Controls.Add(this.panel49);
            this.panel48.Controls.Add(this.label37);
            this.panel48.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel48.Location = new System.Drawing.Point(2, 38);
            this.panel48.Name = "panel48";
            this.panel48.Padding = new System.Windows.Forms.Padding(2);
            this.panel48.Size = new System.Drawing.Size(258, 36);
            this.panel48.TabIndex = 44;
            // 
            // label36
            // 
            this.label36.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label36.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label36.Location = new System.Drawing.Point(87, 2);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(169, 32);
            this.label36.TabIndex = 1;
            this.label36.Text = "Press \'ALT + D\' to select date range.";
            this.label36.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel49
            // 
            this.panel49.BackColor = System.Drawing.Color.DarkGray;
            this.panel49.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel49.Location = new System.Drawing.Point(84, 2);
            this.panel49.Name = "panel49";
            this.panel49.Size = new System.Drawing.Size(3, 32);
            this.panel49.TabIndex = 2;
            // 
            // label37
            // 
            this.label37.BackColor = System.Drawing.Color.MistyRose;
            this.label37.Dock = System.Windows.Forms.DockStyle.Left;
            this.label37.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label37.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label37.Location = new System.Drawing.Point(2, 2);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(82, 32);
            this.label37.TabIndex = 0;
            this.label37.Text = "ALT + D";
            this.label37.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel44
            // 
            this.panel44.BackColor = System.Drawing.SystemColors.Control;
            this.panel44.Controls.Add(this.label32);
            this.panel44.Controls.Add(this.panel45);
            this.panel44.Controls.Add(this.label33);
            this.panel44.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel44.Location = new System.Drawing.Point(2, 2);
            this.panel44.Name = "panel44";
            this.panel44.Padding = new System.Windows.Forms.Padding(2);
            this.panel44.Size = new System.Drawing.Size(258, 36);
            this.panel44.TabIndex = 31;
            // 
            // label32
            // 
            this.label32.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label32.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label32.Location = new System.Drawing.Point(87, 2);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(169, 32);
            this.label32.TabIndex = 1;
            this.label32.Text = "Press \'ALT + S\' to focus on Search textbox.";
            this.label32.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel45
            // 
            this.panel45.BackColor = System.Drawing.Color.DarkGray;
            this.panel45.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel45.Location = new System.Drawing.Point(84, 2);
            this.panel45.Name = "panel45";
            this.panel45.Size = new System.Drawing.Size(3, 32);
            this.panel45.TabIndex = 2;
            // 
            // label33
            // 
            this.label33.BackColor = System.Drawing.Color.MistyRose;
            this.label33.Dock = System.Windows.Forms.DockStyle.Left;
            this.label33.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label33.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label33.Location = new System.Drawing.Point(2, 2);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(82, 32);
            this.label33.TabIndex = 0;
            this.label33.Text = "ALT + S";
            this.label33.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // UserActivityForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(732, 433);
            this.Controls.Add(this.pnlHelp);
            this.Controls.Add(this.dgvItems);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pnlDateRange);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "UserActivityForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "User Activity";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.UserActivityForm_Load);
            this.panel2.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.pnlDateRange.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.pnlHelp.ResumeLayout(false);
            this.panel46.ResumeLayout(false);
            this.panel48.ResumeLayout(false);
            this.panel44.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.LinkLabel lnkMyInventory;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Panel pnlDateRange;
        private System.Windows.Forms.Label lblDateRange;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.DataGridView dgvItems;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column13;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.LinkLabel lnkHelp;
        private System.Windows.Forms.Panel pnlHelp;
        private System.Windows.Forms.Panel panel44;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Panel panel45;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Panel panel48;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.Panel panel49;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.Panel panel46;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel47;
        private System.Windows.Forms.Label label34;
    }
}