namespace UnicodeSrl.ScciCore
{
    partial class ucDifferenceEngine
    {
                                private System.ComponentModel.IContainer components = null;

                                        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

                                        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ucEasyTableLayoutPanel1 = new UnicodeSrl.ScciCore.ucEasyTableLayoutPanel(this.components);
            this.lvDestination = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvSource = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ucEasyTableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
                                                this.ucEasyTableLayoutPanel1.ColumnCount = 2;
            this.ucEasyTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.ucEasyTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.ucEasyTableLayoutPanel1.Controls.Add(this.lvDestination, 1, 0);
            this.ucEasyTableLayoutPanel1.Controls.Add(this.lvSource, 0, 0);
            this.ucEasyTableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyTableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.ucEasyTableLayoutPanel1.Name = "ucEasyTableLayoutPanel1";
            this.ucEasyTableLayoutPanel1.RowCount = 1;
            this.ucEasyTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ucEasyTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.ucEasyTableLayoutPanel1.Size = new System.Drawing.Size(480, 351);
            this.ucEasyTableLayoutPanel1.TabIndex = 5;
                                                this.lvDestination.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
            this.lvDestination.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvDestination.FullRowSelect = true;
            this.lvDestination.HideSelection = false;
            this.lvDestination.Location = new System.Drawing.Point(243, 3);
            this.lvDestination.MultiSelect = false;
            this.lvDestination.Name = "lvDestination";
            this.lvDestination.Size = new System.Drawing.Size(234, 345);
            this.lvDestination.TabIndex = 5;
            this.lvDestination.UseCompatibleStateImageBehavior = false;
            this.lvDestination.View = System.Windows.Forms.View.Details;
            this.lvDestination.SelectedIndexChanged += new System.EventHandler(this.lvDestination_SelectedIndexChanged);
            this.lvDestination.Resize += new System.EventHandler(this.lvDestination_Resize);
                                                this.columnHeader3.Text = "Riga";
            this.columnHeader3.Width = 50;
                                                this.columnHeader4.Text = "Text (Destination)";
                                                this.lvSource.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lvSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvSource.FullRowSelect = true;
            this.lvSource.HideSelection = false;
            this.lvSource.Location = new System.Drawing.Point(3, 3);
            this.lvSource.MultiSelect = false;
            this.lvSource.Name = "lvSource";
            this.lvSource.Size = new System.Drawing.Size(234, 345);
            this.lvSource.TabIndex = 4;
            this.lvSource.UseCompatibleStateImageBehavior = false;
            this.lvSource.View = System.Windows.Forms.View.Details;
            this.lvSource.SelectedIndexChanged += new System.EventHandler(this.lvSource_SelectedIndexChanged);
            this.lvSource.Resize += new System.EventHandler(this.lvSource_Resize);
                                                this.columnHeader1.Text = "Riga";
            this.columnHeader1.Width = 50;
                                                this.columnHeader2.Text = "Text (Source)";
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ucEasyTableLayoutPanel1);
            this.Name = "ucDifferenceEngine";
            this.Size = new System.Drawing.Size(480, 351);
            this.ucEasyTableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ucEasyTableLayoutPanel ucEasyTableLayoutPanel1;
        private System.Windows.Forms.ListView lvDestination;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ListView lvSource;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
    }
}
