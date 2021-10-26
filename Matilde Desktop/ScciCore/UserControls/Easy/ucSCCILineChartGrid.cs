using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinChart;
using Infragistics.UltraChart.Core.Primitives;

namespace UnicodeSrl.ScciCore
{
    internal partial class ucSCCILineChartGrid : UserControl, Interfacce.IViewUserControlBase
    {
        #region DECLARE

        public enum enumMostraGraficoAutDati
        {
            mostraGrafico = 0,
            mostraDati = 1
        }

        private string _campodatapergrafico = string.Empty;

        #endregion

        #region PROPRIETA'
        
        public enumMostraGraficoAutDati Visualizza
        {
            get
            {
                enumMostraGraficoAutDati ret = enumMostraGraficoAutDati.mostraGrafico;
                if (this.utabcGraficoDati.Tabs["tabDati"].Selected) ret = enumMostraGraficoAutDati.mostraDati;
                return ret;
            }
            set
            {
                if (value == enumMostraGraficoAutDati.mostraGrafico)
                {
                                        this.utabcGraficoDati.Tabs["tabGrafico"].Selected = true;
                    this.utabcGraficoDati.Tabs["tabGrafico"].Active = true;
                                                        }
                else
                {
                                        this.utabcGraficoDati.Tabs["tabDati"].Selected = true;
                    this.utabcGraficoDati.Tabs["tabDati"].Active = true;
                }
            }
        }

        public ucEasyLabel Titolo
        {
            get { return this.lblTitolo; }
            set { this.lblTitolo = value; }
        }

        public ucEasyGrid Griglia
        {
            get { return this.ucEasyGridDati; }
            set { this.ucEasyGridDati = value; }
        }

        public UltraChart Grafico
        {
            get { return this.ultraChart; }
            set { this.ultraChart = value; }
        }

        public bool TitoloNascosto
        {
            get { return !this.lblTitolo.Visible; }
            set { this.lblTitolo.Visible = !value; }
        } 

        #endregion

        public ucSCCILineChartGrid()
        {
            InitializeComponent();
        }

        #region INTERFACCIA

        public void ViewInit()
        {

            InizializzaControlli();

        }

        #endregion

        #region METODI PUBBLICI

        public void CaricaDati(ref DataTable dtGriglia, ref DataTable dtGrafico)
        {
            try
            {
                CaricaDati(ref dtGriglia, ref dtGrafico, float.MinValue, float.MinValue, DateTime.MinValue, DateTime.MinValue, "");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CaricaDati(ref DataTable dtGriglia, ref DataTable dtGrafico, float inizioscala, float finescala, DateTime datamin, DateTime datamax, string campodatapergrafico)
        {

            _campodatapergrafico = campodatapergrafico;

            if (_campodatapergrafico == string.Empty || _campodatapergrafico.Trim() == "")
                _campodatapergrafico = CoreStatics.CoreApplication.DefinizioneGraficoSelezionata.CampoDataPerGrafico;

            try
            {
                                if (this.ucEasyGridDati != null)
                    this.ucEasyGridDati.DataSource = dtGriglia;
            }
            catch (Exception ex)
            {
                throw new Exception(@"<" + this.Name + @".CaricaDati(dati)>" + Environment.NewLine + ex.Message, ex);
            }


            try
            {
                                                                                
                DateTime datarif = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);


                                                                if (datamin > DateTime.MinValue && datamax > DateTime.MinValue)
                {
                    try
                    {
                        this.ultraChart.Axis.X.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
                                                long rangeMin = datamin.ToBinary();
                        long rangeMax = datamax.ToBinary();
                        long margin = 0;
                        if (rangeMax > rangeMin)
                            margin = ((rangeMax - rangeMin) / 100) * 3;
                        else
                            margin = (rangeMin / 100) * 3;
                                                this.ultraChart.Axis.X.RangeMin = rangeMin - margin;
                        this.ultraChart.Axis.X.RangeMax = rangeMax + margin;
                    }
                    catch (Exception ex)
                    {
                    }
                }
                else
                {
                    try
                    {
                        this.ultraChart.Axis.X.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
                                                long rangeMin = datarif.ToBinary();
                        long rangeMax = rangeMin;
                        long margin = 0;
                        if (rangeMax > rangeMin)
                            margin = ((rangeMax - rangeMin) / 100) * 3;
                        else
                            margin = (rangeMin / 100) * 3;
                                                this.ultraChart.Axis.X.RangeMin = rangeMin - margin;
                        this.ultraChart.Axis.X.RangeMax = rangeMax + margin;
                    }
                    catch (Exception ex)
                    {
                    }
                }
                
                                                                float minval = inizioscala;
                float maxval = finescala;
                if (campodatapergrafico != "" && dtGrafico.Rows.Count > 0)
                {
                    foreach (DataRow row in dtGrafico.Rows)
                    {
                        foreach (DataColumn col in dtGrafico.Columns)
                        {
                            if (col.ColumnName.ToUpper() != campodatapergrafico.ToUpper() && !row.IsNull(col.ColumnName))
                            {
                                if (minval == float.MinValue) minval = (float)row[col.ColumnName];
                                if (maxval == float.MinValue) maxval = (float)row[col.ColumnName];
                                                                if (minval > (float)row[col.ColumnName]) minval = (float)row[col.ColumnName];
                                if (maxval < (float)row[col.ColumnName]) maxval = (float)row[col.ColumnName];
                            }
                        }
                    }
                    if (minval > float.MinValue && maxval > float.MinValue)
                    {
                        minval -= 5;
                        maxval += 5;
                    }
                }
                if (minval > float.MinValue && maxval > float.MinValue)
                {
                    this.ultraChart.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
                    this.ultraChart.Axis.Y.RangeMin = minval;
                    this.ultraChart.Axis.Y.RangeMax = maxval;
                }


                                                                                if (dtGrafico.Rows.Count <= 1)
                {
                    this.ultraChart.LineChart.LineAppearances[0].Thickness = 0;
                }

                                                                                                this.ultraChart.Data.DataMember = dtGrafico.TableName;
                this.ultraChart.Data.DataSource = dtGrafico;
                this.ultraChart.Data.DataBind();


            }
            catch (Exception ex)
            {
                throw new Exception(@"<" + this.Name + @".CaricaDati(grafico)>" + Environment.NewLine + ex.Message, ex);
            }

        }

        #endregion

        #region PRIVATE

        private void InizializzaControlli()
        {
            InizializzaGrafico();
        }

        private void InizializzaGrafico()
        {
            try
            {
                this.ultraChart.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.LineChart;
                                this.ultraChart.Axis.X.Extent = 70;
                this.ultraChart.Axis.X.Labels.Font = new System.Drawing.Font(this.Font.Name, easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XXSmall));
                this.ultraChart.Axis.X.Labels.HorizontalAlign = System.Drawing.StringAlignment.Center;
                this.ultraChart.Axis.X.Labels.ItemFormatString = "<ITEM_LABEL:dd/MM/yy HH:mm>";
                this.ultraChart.Axis.X.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Custom;
                this.ultraChart.Axis.X.Labels.OrientationAngle = 75;
                                this.ultraChart.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font(this.Font.Name, easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XXSmall));
                this.ultraChart.Axis.X.Labels.SeriesLabels.FormatString = "";
                this.ultraChart.Axis.X.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Far;
                this.ultraChart.Axis.X.Labels.VerticalAlign = System.Drawing.StringAlignment.Far;
                this.ultraChart.Axis.X.LineThickness = 1;
                this.ultraChart.Axis.X.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
                this.ultraChart.Axis.X.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
                this.ultraChart.Axis.X.MajorGridLines.Visible = true;
                this.ultraChart.Axis.X.MinorGridLines.Color = System.Drawing.Color.LightGray;
                this.ultraChart.Axis.X.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
                this.ultraChart.Axis.X.MinorGridLines.Visible = false;
                this.ultraChart.Axis.X.Visible = true;
                                                this.ultraChart.Axis.X.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
                this.ultraChart.Axis.X.TimeAxisStyle.TimeAxisStyle = Infragistics.UltraChart.Shared.Styles.RulerGenre.Discrete;

                this.ultraChart.Axis.Y.Extent = 25;
                this.ultraChart.Axis.Y.Labels.Font = new System.Drawing.Font(this.Font.Name, easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XXSmall));
                this.ultraChart.Axis.Y.Labels.HorizontalAlign = System.Drawing.StringAlignment.Far;
                this.ultraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:00.00#>";
                this.ultraChart.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font(this.Font.Name, easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XXSmall));
                this.ultraChart.Axis.Y.Labels.SeriesLabels.FormatString = "";
                this.ultraChart.Axis.Y.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Far;
                this.ultraChart.Axis.Y.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
                this.ultraChart.Axis.Y.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
                this.ultraChart.Axis.Y.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
                this.ultraChart.Axis.Y.LineThickness = 1;
                this.ultraChart.Axis.Y.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
                this.ultraChart.Axis.Y.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
                this.ultraChart.Axis.Y.MajorGridLines.Visible = true;
                this.ultraChart.Axis.Y.MinorGridLines.Color = System.Drawing.Color.LightGray;
                this.ultraChart.Axis.Y.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
                this.ultraChart.Axis.Y.MinorGridLines.Visible = false;
                this.ultraChart.Axis.Y.TickmarkInterval = 1D;
                this.ultraChart.Axis.Y.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
                this.ultraChart.Axis.Y.Visible = true;
                                this.ultraChart.ColorModel.ColorBegin = System.Drawing.Color.Pink;
                this.ultraChart.ColorModel.ColorEnd = System.Drawing.Color.DarkRed;
                this.ultraChart.ColorModel.ModelStyle = Infragistics.UltraChart.Shared.Styles.ColorModels.CustomLinear;
                                this.ultraChart.Data.SwapRowsAndColumns = true;
                                this.ultraChart.Legend.BorderColor = System.Drawing.Color.Transparent;
                this.ultraChart.Legend.Font = new System.Drawing.Font(this.Font.Name, easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall));
                this.ultraChart.Legend.Margins.Bottom = 1;
                this.ultraChart.Legend.Margins.Left = 1;
                this.ultraChart.Legend.Margins.Right = 1;
                this.ultraChart.Legend.Margins.Top = 1;
                this.ultraChart.Legend.SpanPercentage = 15;
                this.ultraChart.Legend.Visible = true;
                                this.ultraChart.LineChart.ChartText[0].ChartTextFont = new System.Drawing.Font(this.Font.Name, easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall) - 1);
                this.ultraChart.LineChart.ChartText[0].ClipText = false;
                this.ultraChart.LineChart.ChartText[0].Column = -2;
                this.ultraChart.LineChart.ChartText[0].ItemFormatString = "<DATA_VALUE:00.00#>";
                this.ultraChart.LineChart.ChartText[0].Row = -2;
                this.ultraChart.LineChart.ChartText[0].VerticalAlign = StringAlignment.Far;
                this.ultraChart.LineChart.ChartText[0].HorizontalAlign = StringAlignment.Far;
                this.ultraChart.LineChart.ChartText[0].Visible = true;
                                this.ultraChart.LineChart.LineAppearances[0].Thickness = 4;
                this.ultraChart.LineChart.LineAppearances[0].SplineTension = 0.25F;
                this.ultraChart.LineChart.LineAppearances[0].IconAppearance.Icon = Infragistics.UltraChart.Shared.Styles.SymbolIcon.Square;
                this.ultraChart.LineChart.LineAppearances[0].IconAppearance.IconSize = Infragistics.UltraChart.Shared.Styles.SymbolIconSize.Large;
                                                this.ultraChart.LineChart.NullHandling = Infragistics.UltraChart.Shared.Styles.NullHandling.DontPlot;
                this.ultraChart.LineChart.Thickness = 4;
                this.ultraChart.LineChart.EndStyle = Infragistics.UltraChart.Shared.Styles.LineCapStyle.DiamondAnchor;
                this.ultraChart.LineChart.StartStyle = Infragistics.UltraChart.Shared.Styles.LineCapStyle.DiamondAnchor;
                this.ultraChart.LineChart.TreatDateTimeAsString = false;
            }
            catch (Exception ex)
            {
            }
        }

        #endregion

        #region EVENTI

        private void ucEasyGridDati_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {

                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {
                    if (oCol.Key == _campodatapergrafico)
                    {
                        oCol.Format = "dd/MM/yyyy HH:mm";
                        oCol.Header.Caption = "Data Ora";
                        oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                        oCol.Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    }
                    else
                    {
                        oCol.Format = "#,##0.000";
                        oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                        oCol.Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;
                    }
                    oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void ucSCCIChart_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
                this.ucEasyGridDati.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.None;
        }

        #endregion

    }
}
