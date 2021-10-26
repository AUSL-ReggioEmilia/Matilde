namespace UnicodeSrl.ScciCore
{
    partial class frmPUPrescrizioniProtocolloGenerate
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
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinScrollBar.ScrollBarLook scrollBarLook1 = new Infragistics.Win.UltraWinScrollBar.ScrollBarLook();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            this.UltraGroupBoxZoom = new Infragistics.Win.Misc.UltraGroupBox();
            this.TableLayoutPanelZoom = new UnicodeSrl.ScciCore.ucEasyTableLayoutPanel(this.components);
            this.ucEasyGrid = new UnicodeSrl.ScciCore.ucEasyGrid();
            this.UltraPopupControlContainerMain = new Infragistics.Win.Misc.UltraPopupControlContainer(this.components);
            this.ucEasyProgressBar = new UnicodeSrl.ScciCore.ucEasyProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.UltraGroupBoxZoom)).BeginInit();
            this.UltraGroupBoxZoom.SuspendLayout();
            this.TableLayoutPanelZoom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ucEasyGrid)).BeginInit();
            this.SuspendLayout();
                                                this.ucBottomModale.Size = new System.Drawing.Size(784, 24);
                                                this.UltraGroupBoxZoom.Controls.Add(this.TableLayoutPanelZoom);
            this.UltraGroupBoxZoom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UltraGroupBoxZoom.Location = new System.Drawing.Point(0, 24);
            this.UltraGroupBoxZoom.Margin = new System.Windows.Forms.Padding(0);
            this.UltraGroupBoxZoom.Name = "UltraGroupBoxZoom";
            this.UltraGroupBoxZoom.Size = new System.Drawing.Size(784, 514);
            this.UltraGroupBoxZoom.TabIndex = 11;
            this.UltraGroupBoxZoom.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
                                                this.TableLayoutPanelZoom.BackColor = System.Drawing.Color.Transparent;
            this.TableLayoutPanelZoom.ColumnCount = 3;
            this.TableLayoutPanelZoom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 1F));
            this.TableLayoutPanelZoom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 98F));
            this.TableLayoutPanelZoom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 1F));
            this.TableLayoutPanelZoom.Controls.Add(this.ucEasyProgressBar, 1, 2);
            this.TableLayoutPanelZoom.Controls.Add(this.ucEasyGrid, 1, 1);
            this.TableLayoutPanelZoom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableLayoutPanelZoom.Location = new System.Drawing.Point(3, 3);
            this.TableLayoutPanelZoom.Name = "TableLayoutPanelZoom";
            this.TableLayoutPanelZoom.RowCount = 3;
            this.TableLayoutPanelZoom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.TableLayoutPanelZoom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 65F));
            this.TableLayoutPanelZoom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.TableLayoutPanelZoom.Size = new System.Drawing.Size(778, 508);
            this.TableLayoutPanelZoom.TabIndex = 0;
                                                this.ucEasyGrid.ColonnaRTFControlloContenuto = false;
            this.ucEasyGrid.ColonnaRTFResize = "";
            this.ucEasyGrid.DataRowFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            this.ucEasyGrid.DefaultAutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            appearance2.BackColor = System.Drawing.SystemColors.Window;
            appearance2.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.ucEasyGrid.DisplayLayout.Appearance = appearance2;
            this.ucEasyGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            this.ucEasyGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance3.FontData.BoldAsString = "True";
            appearance3.FontData.SizeInPoints = 22.50746F;
            this.ucEasyGrid.DisplayLayout.CaptionAppearance = appearance3;
            this.ucEasyGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance4.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance4.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance4.BorderColor = System.Drawing.SystemColors.Window;
            this.ucEasyGrid.DisplayLayout.GroupByBox.Appearance = appearance4;
            appearance5.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ucEasyGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance5;
            this.ucEasyGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.ucEasyGrid.DisplayLayout.GroupByBox.Hidden = true;
            this.ucEasyGrid.DisplayLayout.GroupByBox.Prompt = "Trascina un\'intestazione della colonna qui per raggrupparla.";
            appearance6.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance6.BackColor2 = System.Drawing.SystemColors.Control;
            appearance6.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance6.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ucEasyGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance6;
            this.ucEasyGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.ucEasyGrid.DisplayLayout.MaxRowScrollRegions = 1;
            appearance7.BackColor = System.Drawing.SystemColors.Window;
            appearance7.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ucEasyGrid.DisplayLayout.Override.ActiveCellAppearance = appearance7;
            appearance8.BackColor = System.Drawing.SystemColors.Highlight;
            appearance8.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.ucEasyGrid.DisplayLayout.Override.ActiveRowAppearance = appearance8;
            this.ucEasyGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.ucEasyGrid.DisplayLayout.Override.AllowColMoving = Infragistics.Win.UltraWinGrid.AllowColMoving.NotAllowed;
            this.ucEasyGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.ucEasyGrid.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            this.ucEasyGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.ucEasyGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.ucEasyGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance9.BackColor = System.Drawing.SystemColors.Window;
            this.ucEasyGrid.DisplayLayout.Override.CardAreaAppearance = appearance9;
            appearance10.BorderColor = System.Drawing.Color.Silver;
            appearance10.FontData.SizeInPoints = 22.50746F;
            appearance10.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.ucEasyGrid.DisplayLayout.Override.CellAppearance = appearance10;
            this.ucEasyGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.ucEasyGrid.DisplayLayout.Override.CellPadding = 0;
            this.ucEasyGrid.DisplayLayout.Override.ColumnAutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.SiblingRowsOnly;
            appearance11.FontData.SizeInPoints = 22.50746F;
            this.ucEasyGrid.DisplayLayout.Override.FilterRowAppearance = appearance11;
            appearance12.BackColor = System.Drawing.SystemColors.Control;
            appearance12.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance12.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance12.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance12.BorderColor = System.Drawing.SystemColors.Window;
            this.ucEasyGrid.DisplayLayout.Override.GroupByRowAppearance = appearance12;
            appearance13.FontData.BoldAsString = "True";
            appearance13.FontData.SizeInPoints = 22.50746F;
            appearance13.TextHAlignAsString = "Center";
            this.ucEasyGrid.DisplayLayout.Override.HeaderAppearance = appearance13;
            this.ucEasyGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortSingle;
            this.ucEasyGrid.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsVista;
            appearance14.BackColor = System.Drawing.Color.WhiteSmoke;
            appearance14.ForeColor = System.Drawing.Color.DarkBlue;
            this.ucEasyGrid.DisplayLayout.Override.RowAlternateAppearance = appearance14;
            appearance15.BackColor = System.Drawing.SystemColors.Window;
            appearance15.BorderColor = System.Drawing.Color.Silver;
            this.ucEasyGrid.DisplayLayout.Override.RowAppearance = appearance15;
            this.ucEasyGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.ucEasyGrid.DisplayLayout.Override.RowSizing = Infragistics.Win.UltraWinGrid.RowSizing.AutoFree;
            this.ucEasyGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.ucEasyGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance16.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ucEasyGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance16;
            scrollBarLook1.ViewStyle = Infragistics.Win.UltraWinScrollBar.ScrollBarViewStyle.Office2010;
            this.ucEasyGrid.DisplayLayout.ScrollBarLook = scrollBarLook1;
            this.ucEasyGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.ucEasyGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.ucEasyGrid.DisplayLayout.TabNavigation = Infragistics.Win.UltraWinGrid.TabNavigation.NextControl;
            this.ucEasyGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.ucEasyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyGrid.FattoreRidimensionamentoRTF = 21;
            this.ucEasyGrid.FilterRowFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            this.ucEasyGrid.GridCaptionFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            this.ucEasyGrid.HeaderFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            this.ucEasyGrid.Location = new System.Drawing.Point(10, 36);
            this.ucEasyGrid.Name = "ucEasyGrid";
            this.ucEasyGrid.ShowFilterRow = false;
            this.ucEasyGrid.ShowGroupByBox = false;
            this.ucEasyGrid.Size = new System.Drawing.Size(756, 434);
            this.ucEasyGrid.TabIndex = 9;
            this.ucEasyGrid.Text = "ucEasyGrid1";
            this.ucEasyGrid.UseFlatMode = Infragistics.Win.DefaultableBoolean.True;
            this.ucEasyGrid.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.ucEasyGrid.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ucEasyGrid_InitializeLayout);
            this.ucEasyGrid.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.ucEasyGrid_InitializeRow);
            this.ucEasyGrid.ClickCellButton += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.ucEasyGrid_ClickCellButton);
            this.ucEasyGrid.ClickCell += new Infragistics.Win.UltraWinGrid.ClickCellEventHandler(this.ucEasyGrid_ClickCell);
                                                this.UltraPopupControlContainerMain.Opening += new System.ComponentModel.CancelEventHandler(this.UltraPopupControlContainer_Opening);
            this.UltraPopupControlContainerMain.Opened += new System.EventHandler(this.UltraPopupControlContainer_Opened);
                                                appearance1.FontData.SizeInPoints = 15.58209F;
            this.ucEasyProgressBar.Appearance = appearance1;
            this.ucEasyProgressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyProgressBar.Location = new System.Drawing.Point(10, 476);
            this.ucEasyProgressBar.Name = "ucEasyProgressBar";
            this.ucEasyProgressBar.Size = new System.Drawing.Size(756, 29);
            this.ucEasyProgressBar.TabIndex = 25;
            this.ucEasyProgressBar.Text = "Elaborazione in corso: [Value] / [Maximum]";
            this.ucEasyProgressBar.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.ucEasyProgressBar.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.ucEasyProgressBar.Visible = false;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.UltraGroupBoxZoom);
            this.Name = "frmPUPrescrizioniProtocolloGenerate";
            this.PulsanteAvantiVisibile = true;
            this.PulsanteIndietroVisibile = true;
            this.Text = "frmPUPrescrizioniProtocolloGenerate";
            this.PulsanteIndietroClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.frmPUPrescrizioniProtocolloGenerate_PulsanteIndietroClick);
            this.PulsanteAvantiClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.frmPUPrescrizioniProtocolloGenerate_PulsanteAvantiClick);
            this.Controls.SetChildIndex(this.UltraGroupBoxZoom, 0);
            ((System.ComponentModel.ISupportInitialize)(this.UltraGroupBoxZoom)).EndInit();
            this.UltraGroupBoxZoom.ResumeLayout(false);
            this.TableLayoutPanelZoom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ucEasyGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox UltraGroupBoxZoom;
        private ucEasyTableLayoutPanel TableLayoutPanelZoom;
        private ucEasyGrid ucEasyGrid;
        private Infragistics.Win.Misc.UltraPopupControlContainer UltraPopupControlContainerMain;
        private ucEasyProgressBar ucEasyProgressBar;
    }
}