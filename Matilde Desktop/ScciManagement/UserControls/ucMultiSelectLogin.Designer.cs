
namespace UnicodeSrl.ScciManagement
{
    partial class ucMultiSelectLogin
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
            this.ucEasyTableLayoutPanel = new UnicodeSrl.ScciCore.ucEasyTableLayoutPanel(this.components);
            this.uceObsoleti = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ucMultiSelect = new UnicodeSrl.ScciCore.ucMultiSelect();
            this.ucEasyTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uceObsoleti)).BeginInit();
            this.SuspendLayout();
                                                this.ucEasyTableLayoutPanel.ColumnCount = 3;
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 44F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 44F));
            this.ucEasyTableLayoutPanel.Controls.Add(this.uceObsoleti, 2, 0);
            this.ucEasyTableLayoutPanel.Controls.Add(this.ucMultiSelect, 0, 1);
            this.ucEasyTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.ucEasyTableLayoutPanel.Name = "ucEasyTableLayoutPanel";
            this.ucEasyTableLayoutPanel.RowCount = 2;
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ucEasyTableLayoutPanel.Size = new System.Drawing.Size(800, 600);
            this.ucEasyTableLayoutPanel.TabIndex = 0;
                                                this.uceObsoleti.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.Office2007;
            this.uceObsoleti.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uceObsoleti.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.uceObsoleti.Location = new System.Drawing.Point(451, 3);
            this.uceObsoleti.Name = "uceObsoleti";
            this.uceObsoleti.Size = new System.Drawing.Size(346, 21);
            this.uceObsoleti.TabIndex = 5;
            this.uceObsoleti.ValueChanged += new System.EventHandler(this.uceObsoleti_ValueChanged);
                                                this.ucEasyTableLayoutPanel.SetColumnSpan(this.ucMultiSelect, 3);
            this.ucMultiSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucMultiSelect.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ucMultiSelect.GridDXCaption = "";
            this.ucMultiSelect.GridDXCaptionColumnKey = "";
            this.ucMultiSelect.GridDXFilterColumnIndex = 1;
            this.ucMultiSelect.GridSXCaption = "";
            this.ucMultiSelect.GridSXCaptionColumnKey = "";
            this.ucMultiSelect.GridSXFilterColumnIndex = 1;
            this.ucMultiSelect.Location = new System.Drawing.Point(3, 27);
            this.ucMultiSelect.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.ucMultiSelect.Name = "ucMultiSelect";
            this.ucMultiSelect.Size = new System.Drawing.Size(794, 570);
            this.ucMultiSelect.TabIndex = 1;
            this.ucMultiSelect.ViewDataSetDX = null;
            this.ucMultiSelect.ViewDataSetSX = null;
            this.ucMultiSelect.ViewDataViewDX = null;
            this.ucMultiSelect.ViewDataViewSX = null;
            this.ucMultiSelect.ViewShowAll = true;
            this.ucMultiSelect.ViewShowFind = true;
            this.ucMultiSelect.GridSXInitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ucMultiSelect_GridSXInitializeLayout);
            this.ucMultiSelect.GridSXInitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.ucMultiSelect_GridSXInitializeRow);
            this.ucMultiSelect.GridDXInitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ucMultiSelect_GridDXInitializeLayout);
            this.ucMultiSelect.GridDXInitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.ucMultiSelect_GridDXInitializeRow);
            this.ucMultiSelect.GridChange += new UnicodeSrl.ScciCore.ChangeEventHandler(this.ucMultiSelect_GridChange);
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ucEasyTableLayoutPanel);
            this.Name = "ucMultiSelectLogin";
            this.Size = new System.Drawing.Size(800, 600);
            this.ucEasyTableLayoutPanel.ResumeLayout(false);
            this.ucEasyTableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uceObsoleti)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ScciCore.ucEasyTableLayoutPanel ucEasyTableLayoutPanel;
        public ScciCore.ucMultiSelect ucMultiSelect;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor uceObsoleti;
    }
}
