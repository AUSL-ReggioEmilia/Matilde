namespace UnicodeSrl.ScciCore
{
    partial class frmSelezionaTestiNotePredefiniti
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

        #region Windows Form Designer generated code

                                        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinTree.Override _override1 = new Infragistics.Win.UltraWinTree.Override();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            this.UltraGroupBoxZoom = new Infragistics.Win.Misc.UltraGroupBox();
            this.TableLayoutPanelZoom = new UnicodeSrl.ScciCore.ucEasyTableLayoutPanel(this.components);
            this.ucEasyTableLayoutImmagine = new UnicodeSrl.ScciCore.ucEasyTableLayoutPanel(this.components);
            this.PictureBox = new UnicodeSrl.ScciCore.ucEasyPictureBox();
            this.UltraTree = new UnicodeSrl.ScciCore.ucEasyTreeView();
            ((System.ComponentModel.ISupportInitialize)(this.UltraGroupBoxZoom)).BeginInit();
            this.UltraGroupBoxZoom.SuspendLayout();
            this.TableLayoutPanelZoom.SuspendLayout();
            this.ucEasyTableLayoutImmagine.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UltraTree)).BeginInit();
            this.SuspendLayout();
                                                this.ucBottomModale.Size = new System.Drawing.Size(784, 24);
                                                this.UltraGroupBoxZoom.Controls.Add(this.TableLayoutPanelZoom);
            this.UltraGroupBoxZoom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UltraGroupBoxZoom.Location = new System.Drawing.Point(0, 24);
            this.UltraGroupBoxZoom.Margin = new System.Windows.Forms.Padding(0);
            this.UltraGroupBoxZoom.Name = "UltraGroupBoxZoom";
            this.UltraGroupBoxZoom.Size = new System.Drawing.Size(784, 513);
            this.UltraGroupBoxZoom.TabIndex = 10;
            this.UltraGroupBoxZoom.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
                                                this.TableLayoutPanelZoom.BackColor = System.Drawing.Color.Transparent;
            this.TableLayoutPanelZoom.ColumnCount = 3;
            this.TableLayoutPanelZoom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13F));
            this.TableLayoutPanelZoom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 77F));
            this.TableLayoutPanelZoom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.TableLayoutPanelZoom.Controls.Add(this.UltraTree, 1, 1);
            this.TableLayoutPanelZoom.Controls.Add(this.ucEasyTableLayoutImmagine, 0, 1);
            this.TableLayoutPanelZoom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableLayoutPanelZoom.Location = new System.Drawing.Point(3, 3);
            this.TableLayoutPanelZoom.Name = "TableLayoutPanelZoom";
            this.TableLayoutPanelZoom.RowCount = 3;
            this.TableLayoutPanelZoom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.TableLayoutPanelZoom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90F));
            this.TableLayoutPanelZoom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.TableLayoutPanelZoom.Size = new System.Drawing.Size(778, 507);
            this.TableLayoutPanelZoom.TabIndex = 0;
                                                this.ucEasyTableLayoutImmagine.ColumnCount = 3;
            this.ucEasyTableLayoutImmagine.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.ucEasyTableLayoutImmagine.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 71.42857F));
            this.ucEasyTableLayoutImmagine.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.ucEasyTableLayoutImmagine.Controls.Add(this.PictureBox, 1, 1);
            this.ucEasyTableLayoutImmagine.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyTableLayoutImmagine.Location = new System.Drawing.Point(3, 28);
            this.ucEasyTableLayoutImmagine.Name = "ucEasyTableLayoutImmagine";
            this.ucEasyTableLayoutImmagine.RowCount = 3;
            this.ucEasyTableLayoutImmagine.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.ucEasyTableLayoutImmagine.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.ucEasyTableLayoutImmagine.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.ucEasyTableLayoutImmagine.Size = new System.Drawing.Size(95, 450);
            this.ucEasyTableLayoutImmagine.TabIndex = 2;
                                                this.PictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PictureBox.Location = new System.Drawing.Point(16, 93);
            this.PictureBox.Name = "PictureBox";
            this.PictureBox.ShortcutColor = System.Drawing.Color.Black;
            this.PictureBox.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.PictureBox.ShortcutKey = System.Windows.Forms.Keys.None;
            this.PictureBox.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.PictureBox.Size = new System.Drawing.Size(61, 264);
            this.PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PictureBox.TabIndex = 0;
            this.PictureBox.TabStop = false;
                                                appearance1.FontData.SizeInPoints = 27.70149F;
            this.UltraTree.Appearance = appearance1;
            this.UltraTree.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.UltraTree.DisplayStyle = Infragistics.Win.UltraWinTree.UltraTreeDisplayStyle.WindowsVista;
            this.UltraTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UltraTree.HideSelection = false;
            this.UltraTree.LeftImagesSize = new System.Drawing.Size(28, 28);
            this.UltraTree.Location = new System.Drawing.Point(104, 28);
            this.UltraTree.Name = "UltraTree";
            appearance2.BackColor = System.Drawing.Color.LightYellow;
            appearance2.BackColor2 = System.Drawing.Color.Orange;
            _override1.ActiveNodeAppearance = appearance2;
            _override1.SelectedNodeAppearance = appearance2;
            this.UltraTree.Override = _override1;
            this.UltraTree.RightImagesSize = new System.Drawing.Size(28, 28);
            this.UltraTree.Size = new System.Drawing.Size(593, 450);
            this.UltraTree.TabIndex = 22;
            this.UltraTree.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Large;
            this.UltraTree.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.UltraGroupBoxZoom);
            this.Name = "frmSelezionaTestiNotePredefiniti";
            this.PulsanteAvantiVisibile = true;
            this.PulsanteIndietroVisibile = true;
            this.Text = "frmSelezionaTestiNotePredefiniti";
            this.PulsanteIndietroClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.frmSelezionaTestiNotePredefiniti_PulsanteIndietroClick);
            this.PulsanteAvantiClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.frmSelezionaTestiNotePredefiniti_PulsanteAvantiClick);
            this.Controls.SetChildIndex(this.UltraGroupBoxZoom, 0);
            ((System.ComponentModel.ISupportInitialize)(this.UltraGroupBoxZoom)).EndInit();
            this.UltraGroupBoxZoom.ResumeLayout(false);
            this.TableLayoutPanelZoom.ResumeLayout(false);
            this.ucEasyTableLayoutImmagine.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UltraTree)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox UltraGroupBoxZoom;
        private ucEasyTableLayoutPanel TableLayoutPanelZoom;
        private ucEasyTableLayoutPanel ucEasyTableLayoutImmagine;
        private ucEasyPictureBox PictureBox;
        private ucEasyTreeView UltraTree;
    }
}