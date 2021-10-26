using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using UnicodeSrl.ScciCore.CustomControls.Infra;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci;

namespace UnicodeSrl.ScciCore
{
    public partial class ucNews : UserControl, Interfacce.IViewUserControlMiddle
    {

        private bool _visualizzaRTF = false;

        public bool VisualizzaRTF
        {
            get { return _visualizzaRTF; }
            set { _visualizzaRTF = value; }
        }

        public ucNews()
        {
            InitializeComponent();
        }

        #region INTERFACCIA

        public void Aggiorna()
        {

            try
            {
                this.LoadUltraGrid();
            }
            catch (Exception)
            {
            }

        }

        public void Carica()
        {

            InitUltraGridNewsLayout();
            this.ugNews.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
            Aggiorna();

        }

        public void Ferma()
        {

            try
            {

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Ferma", this.Name);
            }

        }

        #endregion

        #region PRIVATE

        private void LoadUltraGrid()
        {
            try
            {
                DataSet oDs = new DataSet();
                DataTable oDt = CoreStatics.CreateDataTable<Notizia>();
                CoreStatics.CoreApplication.News.Aggiorna();
                CoreStatics.FillDataTable<Notizia>(oDt, CoreStatics.CoreApplication.News.Elementi);
                oDs.Tables.Add(oDt);















                if (_visualizzaRTF)
                    this.ugNews.ColonnaRTFResize = "TestoRTF";
                else
                    this.ugNews.ColonnaRTFResize = "";


                this.ugNews.DataSource = oDt;
                this.ugNews.Refresh();



                this.ugNews.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);
                this.ugNews.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        private void InitUltraGridNewsLayout()
        {

            this.ugNews.DisplayLayout.Appearance.BackColor = System.Drawing.SystemColors.Window;
            this.ugNews.DisplayLayout.Appearance.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.ugNews.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;

            this.ugNews.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;

            this.ugNews.DisplayLayout.CaptionAppearance.BackColor = Color.LightSteelBlue;
            this.ugNews.DisplayLayout.CaptionAppearance.BackColor2 = Color.Lavender;
            this.ugNews.DisplayLayout.CaptionAppearance.BackGradientStyle = Infragistics.Win.GradientStyle.ForwardDiagonal;

            this.ugNews.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;

            this.ugNews.DisplayLayout.GroupByBox.Hidden = true;

            this.ugNews.DisplayLayout.MaxColScrollRegions = 1;
            this.ugNews.DisplayLayout.MaxRowScrollRegions = 1;

            this.ugNews.DisplayLayout.Override.ActiveCellAppearance.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ugNews.DisplayLayout.Override.ActiveCellAppearance.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ugNews.DisplayLayout.Override.ActiveCellAppearance.BorderColor = System.Drawing.Color.Transparent;
            this.ugNews.DisplayLayout.Override.ActiveRowAppearance.BackColor = Color.WhiteSmoke;
            this.ugNews.DisplayLayout.Override.ActiveRowAppearance.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ugNews.DisplayLayout.Override.ActiveRowAppearance.BorderColor = System.Drawing.Color.Transparent;

            this.ugNews.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.ugNews.DisplayLayout.Override.AllowColMoving = Infragistics.Win.UltraWinGrid.AllowColMoving.NotAllowed;
            this.ugNews.DisplayLayout.Override.AllowColSizing = Infragistics.Win.UltraWinGrid.AllowColSizing.None;
            this.ugNews.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.ugNews.DisplayLayout.Override.AllowGroupBy = Infragistics.Win.DefaultableBoolean.False;
            this.ugNews.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            this.ugNews.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;

            this.ugNews.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.None;
            this.ugNews.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.None;

            this.ugNews.DisplayLayout.Override.CardAreaAppearance.BackColor = System.Drawing.SystemColors.Window;
            this.ugNews.DisplayLayout.Override.CellAppearance.BorderColor = System.Drawing.Color.Transparent;
            this.ugNews.DisplayLayout.Override.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.ugNews.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.CellSelect;
            this.ugNews.DisplayLayout.Override.CellPadding = 0;

            this.ugNews.DisplayLayout.Override.ColumnAutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.AllRowsInBand;

            this.ugNews.DisplayLayout.Override.HeaderAppearance.BackColor = Color.LightSteelBlue;
            this.ugNews.DisplayLayout.Override.HeaderAppearance.BackColor2 = Color.Lavender;
            this.ugNews.DisplayLayout.Override.HeaderAppearance.BackGradientStyle = Infragistics.Win.GradientStyle.ForwardDiagonal;
            this.ugNews.DisplayLayout.Override.HeaderAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
            this.ugNews.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.Default;
            this.ugNews.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.Default;

            this.ugNews.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.ugNews.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.None;

            this.ugNews.DisplayLayout.Override.RowAppearance.BackColor = System.Drawing.SystemColors.Window;
            this.ugNews.DisplayLayout.Override.RowAppearance.BorderColor = System.Drawing.Color.Silver;
            this.ugNews.DisplayLayout.Override.RowAlternateAppearance.BackColor = this.ugNews.DisplayLayout.Override.RowAppearance.BackColor; this.ugNews.DisplayLayout.Override.RowAlternateAppearance.ForeColor = this.ugNews.DisplayLayout.Override.RowAppearance.ForeColor; this.ugNews.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.ugNews.DisplayLayout.Override.RowSizing = Infragistics.Win.UltraWinGrid.RowSizing.AutoFixed;
            this.ugNews.DisplayLayout.Override.RowSpacingBefore = 10;

            this.ugNews.DisplayLayout.Override.TemplateAddRowAppearance.BackColor = System.Drawing.SystemColors.ControlLight;

            this.ugNews.DisplayLayout.RowConnectorStyle = RowConnectorStyle.None;

            this.ugNews.DisplayLayout.ScrollStyle = ScrollStyle.Immediate;
            this.ugNews.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;

            this.ugNews.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.ugNews.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;

            this.ugNews.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;

        }

        private void ApriPopupNews(UltraGridRow eRow)
        {

            try
            {
                if (eRow != null && eRow.IsDataRow)
                {
                    Notizia _notizia = CoreStatics.CoreApplication.News.Elementi.Single(Notizia => Notizia.ID == (decimal)eRow.Cells["ID"].Value);
                    if (_notizia != null)
                    {
                        try
                        {
                            Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                            op.Parametro.Add("ID", _notizia.ID.ToString());
                            op.Parametro.Add("DatiEstesi", "1");

                            SqlParameterExt[] spcoll = new SqlParameterExt[1];

                            string xmlParam = XmlProcs.XmlSerializeToString(op);
                            spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                            DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovNews", spcoll);

                            if (dt.Rows.Count == 1)
                            {
                                _notizia.TestoRTF = dt.Rows[0]["TestoRTF"].ToString();
                            }

                            CoreStatics.CoreApplication.News.NotiziaSelezionata = _notizia;
                            CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.DettaglioNews);

                        }
                        catch (Exception ex)
                        {
                            CoreStatics.ExGest(ref ex, "ApriPopupNews 1", this.Name);
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ApriPopupNews 0", this.Name);
            }
        }

        #endregion

        #region EVENTI

        private void ugNews_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {
                e.Layout.Bands[0].HeaderVisible = false;
                e.Layout.Bands[0].ColHeadersVisible = false;

                if (_visualizzaRTF)
                    e.Layout.Bands[0].RowLayoutStyle = RowLayoutStyle.GroupLayout;
                else
                    e.Layout.Bands[0].RowLayoutStyle = RowLayoutStyle.ColumnLayout;

                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {
                    oCol.SortIndicator = SortIndicator.Disabled;
                    switch (oCol.Key)
                    {
                        case "Titolo":
                            oCol.Hidden = false;
                            oCol.CellAppearance.ForeColor = Color.FromArgb(50, 50, 50);
                            oCol.CellAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                            oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                            oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                            oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                            oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                            oCol.CellAppearance.BackColor = Color.FromArgb(215, 231, 244);
                            oCol.MinWidth = ((UltraGrid)sender).Width * 2 / 3;

                            if (_visualizzaRTF)
                            {
                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;
                            }

                            oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand, AutoResizeColumnWidthOptions.IncludeCells);

                            break;

                        case "Data":
                            if (_visualizzaRTF)
                            {
                                oCol.Hidden = false;
                                oCol.CellAppearance.ForeColor = Color.Gray;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.CellAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.False;
                                oCol.VertScrollBar = false;
                                oCol.Format = "dd/MM/yyyy";
                                oCol.MinWidth = 50;

                                oCol.RowLayoutColumnInfo.OriginX = 0;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand, AutoResizeColumnWidthOptions.IncludeCells);
                            }
                            else
                                oCol.Hidden = true;

                            break;

                        case "DataOra":
                            oCol.Hidden = false;
                            oCol.CellAppearance.ForeColor = Color.Gray;
                            oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                            oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                            oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                            oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                            oCol.VertScrollBar = false;
                            if (_visualizzaRTF)
                            {
                                oCol.Format = "HH:mm:ss";
                                oCol.MinWidth = 50;

                                oCol.RowLayoutColumnInfo.OriginX = 0;
                                oCol.RowLayoutColumnInfo.OriginY = 1;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;
                            }
                            else
                            {
                                oCol.Format = "dd/MM/yyyy HH:mm:ss";
                            }


                            break;

                        case "TestoRTF":
                            if (_visualizzaRTF)
                            {
                                oCol.Hidden = false;

                                RichTextEditor a = new RichTextEditor();
                                oCol.Editor = a;
                                oCol.CellActivation = Activation.ActivateOnly;
                                oCol.CellClickAction = CellClickAction.CellSelect;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;




                                oCol.RowLayoutColumnInfo.WeightY = 1;
                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 1;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 2;

                                oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand, AutoResizeColumnWidthOptions.IncludeCells);
                            }
                            else
                                oCol.Hidden = true;
                            break;

                        default:
                            oCol.Hidden = true;
                            break;
                    }
                }
            }
            catch (Exception)
            {
            }
        }


        private void ugNews_ClickCell(object sender, ClickCellEventArgs e)
        {
            ApriPopupNews(e.Cell.Row);
        }

        #endregion

    }
}
