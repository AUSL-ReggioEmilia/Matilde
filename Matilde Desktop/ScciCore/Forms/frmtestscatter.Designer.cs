namespace UnicodeSrl.ScciCore
{
    partial class frmtestscatter
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
            Infragistics.UltraChart.Resources.Appearance.TimeAxisAppearance timeAxisAppearance1 = new Infragistics.UltraChart.Resources.Appearance.TimeAxisAppearance();
            Infragistics.UltraChart.Resources.Appearance.GradientEffect gradientEffect1 = new Infragistics.UltraChart.Resources.Appearance.GradientEffect();
            Infragistics.UltraChart.Resources.Appearance.ScatterChartAppearance scatterChartAppearance1 = new Infragistics.UltraChart.Resources.Appearance.ScatterChartAppearance();
            Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance chartTextAppearance1 = new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance();
            this.ultraChart1 = new Infragistics.Win.UltraWinChart.UltraChart();
            ((System.ComponentModel.ISupportInitialize)(this.ultraChart1)).BeginInit();
            this.SuspendLayout();
                        this.ultraChart1.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.ScatterChart;
                                                this.ultraChart1.Axis.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(248)))), ((int)(((byte)(220)))));
            this.ultraChart1.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ultraChart1.Axis.X.Labels.FontColor = System.Drawing.Color.DimGray;
            this.ultraChart1.Axis.X.Labels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            this.ultraChart1.Axis.X.Labels.ItemFormatString = "<DATA_VALUE:00.##>";
            this.ultraChart1.Axis.X.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ultraChart1.Axis.X.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.VerticalLeftFacing;
            this.ultraChart1.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ultraChart1.Axis.X.Labels.SeriesLabels.FontColor = System.Drawing.Color.DimGray;
            this.ultraChart1.Axis.X.Labels.SeriesLabels.FormatString = "";
            this.ultraChart1.Axis.X.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            this.ultraChart1.Axis.X.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ultraChart1.Axis.X.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.VerticalLeftFacing;
            this.ultraChart1.Axis.X.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ultraChart1.Axis.X.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ultraChart1.Axis.X.LineThickness = 1;
            this.ultraChart1.Axis.X.MajorGridLines.AlphaLevel = ((byte)(255));
            this.ultraChart1.Axis.X.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
            this.ultraChart1.Axis.X.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ultraChart1.Axis.X.MajorGridLines.Visible = true;
            this.ultraChart1.Axis.X.MinorGridLines.AlphaLevel = ((byte)(255));
            this.ultraChart1.Axis.X.MinorGridLines.Color = System.Drawing.Color.LightGray;
            this.ultraChart1.Axis.X.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ultraChart1.Axis.X.MinorGridLines.Visible = false;
            this.ultraChart1.Axis.X.TickmarkInterval = 10D;
            this.ultraChart1.Axis.X.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
            timeAxisAppearance1.TimeAxisStyle = Infragistics.UltraChart.Shared.Styles.RulerGenre.Discrete;
            this.ultraChart1.Axis.X.TimeAxisStyle = timeAxisAppearance1;
            this.ultraChart1.Axis.X.Visible = true;
            this.ultraChart1.Axis.X2.Labels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ultraChart1.Axis.X2.Labels.FontColor = System.Drawing.Color.Gray;
            this.ultraChart1.Axis.X2.Labels.HorizontalAlign = System.Drawing.StringAlignment.Far;
            this.ultraChart1.Axis.X2.Labels.ItemFormatString = "<DATA_VALUE:00.##>";
            this.ultraChart1.Axis.X2.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ultraChart1.Axis.X2.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.VerticalLeftFacing;
            this.ultraChart1.Axis.X2.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ultraChart1.Axis.X2.Labels.SeriesLabels.FontColor = System.Drawing.Color.Gray;
            this.ultraChart1.Axis.X2.Labels.SeriesLabels.FormatString = "";
            this.ultraChart1.Axis.X2.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Far;
            this.ultraChart1.Axis.X2.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ultraChart1.Axis.X2.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.VerticalLeftFacing;
            this.ultraChart1.Axis.X2.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ultraChart1.Axis.X2.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ultraChart1.Axis.X2.Labels.Visible = false;
            this.ultraChart1.Axis.X2.LineThickness = 1;
            this.ultraChart1.Axis.X2.MajorGridLines.AlphaLevel = ((byte)(255));
            this.ultraChart1.Axis.X2.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
            this.ultraChart1.Axis.X2.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ultraChart1.Axis.X2.MajorGridLines.Visible = true;
            this.ultraChart1.Axis.X2.MinorGridLines.AlphaLevel = ((byte)(255));
            this.ultraChart1.Axis.X2.MinorGridLines.Color = System.Drawing.Color.LightGray;
            this.ultraChart1.Axis.X2.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ultraChart1.Axis.X2.MinorGridLines.Visible = false;
            this.ultraChart1.Axis.X2.TickmarkInterval = 10D;
            this.ultraChart1.Axis.X2.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
            this.ultraChart1.Axis.X2.Visible = false;
            this.ultraChart1.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ultraChart1.Axis.Y.Labels.FontColor = System.Drawing.Color.DimGray;
            this.ultraChart1.Axis.Y.Labels.HorizontalAlign = System.Drawing.StringAlignment.Far;
            this.ultraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:00.##>";
            this.ultraChart1.Axis.Y.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ultraChart1.Axis.Y.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            this.ultraChart1.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ultraChart1.Axis.Y.Labels.SeriesLabels.FontColor = System.Drawing.Color.DimGray;
            this.ultraChart1.Axis.Y.Labels.SeriesLabels.FormatString = "";
            this.ultraChart1.Axis.Y.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Far;
            this.ultraChart1.Axis.Y.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ultraChart1.Axis.Y.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            this.ultraChart1.Axis.Y.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ultraChart1.Axis.Y.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ultraChart1.Axis.Y.LineThickness = 1;
            this.ultraChart1.Axis.Y.MajorGridLines.AlphaLevel = ((byte)(255));
            this.ultraChart1.Axis.Y.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
            this.ultraChart1.Axis.Y.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ultraChart1.Axis.Y.MajorGridLines.Visible = true;
            this.ultraChart1.Axis.Y.MinorGridLines.AlphaLevel = ((byte)(255));
            this.ultraChart1.Axis.Y.MinorGridLines.Color = System.Drawing.Color.LightGray;
            this.ultraChart1.Axis.Y.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ultraChart1.Axis.Y.MinorGridLines.Visible = false;
            this.ultraChart1.Axis.Y.TickmarkInterval = 20D;
            this.ultraChart1.Axis.Y.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
            this.ultraChart1.Axis.Y.Visible = true;
            this.ultraChart1.Axis.Y2.Labels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ultraChart1.Axis.Y2.Labels.FontColor = System.Drawing.Color.Gray;
            this.ultraChart1.Axis.Y2.Labels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            this.ultraChart1.Axis.Y2.Labels.ItemFormatString = "<DATA_VALUE:00.##>";
            this.ultraChart1.Axis.Y2.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ultraChart1.Axis.Y2.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            this.ultraChart1.Axis.Y2.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ultraChart1.Axis.Y2.Labels.SeriesLabels.FontColor = System.Drawing.Color.Gray;
            this.ultraChart1.Axis.Y2.Labels.SeriesLabels.FormatString = "";
            this.ultraChart1.Axis.Y2.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            this.ultraChart1.Axis.Y2.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ultraChart1.Axis.Y2.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            this.ultraChart1.Axis.Y2.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ultraChart1.Axis.Y2.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ultraChart1.Axis.Y2.Labels.Visible = false;
            this.ultraChart1.Axis.Y2.LineThickness = 1;
            this.ultraChart1.Axis.Y2.MajorGridLines.AlphaLevel = ((byte)(255));
            this.ultraChart1.Axis.Y2.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
            this.ultraChart1.Axis.Y2.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ultraChart1.Axis.Y2.MajorGridLines.Visible = true;
            this.ultraChart1.Axis.Y2.MinorGridLines.AlphaLevel = ((byte)(255));
            this.ultraChart1.Axis.Y2.MinorGridLines.Color = System.Drawing.Color.LightGray;
            this.ultraChart1.Axis.Y2.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ultraChart1.Axis.Y2.MinorGridLines.Visible = false;
            this.ultraChart1.Axis.Y2.TickmarkInterval = 20D;
            this.ultraChart1.Axis.Y2.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
            this.ultraChart1.Axis.Y2.Visible = false;
            this.ultraChart1.Axis.Z.Labels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ultraChart1.Axis.Z.Labels.FontColor = System.Drawing.Color.DimGray;
            this.ultraChart1.Axis.Z.Labels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            this.ultraChart1.Axis.Z.Labels.ItemFormatString = "";
            this.ultraChart1.Axis.Z.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ultraChart1.Axis.Z.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            this.ultraChart1.Axis.Z.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ultraChart1.Axis.Z.Labels.SeriesLabels.FontColor = System.Drawing.Color.DimGray;
            this.ultraChart1.Axis.Z.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            this.ultraChart1.Axis.Z.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ultraChart1.Axis.Z.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            this.ultraChart1.Axis.Z.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ultraChart1.Axis.Z.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ultraChart1.Axis.Z.Labels.Visible = false;
            this.ultraChart1.Axis.Z.LineThickness = 1;
            this.ultraChart1.Axis.Z.MajorGridLines.AlphaLevel = ((byte)(255));
            this.ultraChart1.Axis.Z.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
            this.ultraChart1.Axis.Z.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ultraChart1.Axis.Z.MajorGridLines.Visible = true;
            this.ultraChart1.Axis.Z.MinorGridLines.AlphaLevel = ((byte)(255));
            this.ultraChart1.Axis.Z.MinorGridLines.Color = System.Drawing.Color.LightGray;
            this.ultraChart1.Axis.Z.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ultraChart1.Axis.Z.MinorGridLines.Visible = false;
            this.ultraChart1.Axis.Z.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
            this.ultraChart1.Axis.Z.Visible = false;
            this.ultraChart1.Axis.Z2.Labels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ultraChart1.Axis.Z2.Labels.FontColor = System.Drawing.Color.Gray;
            this.ultraChart1.Axis.Z2.Labels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            this.ultraChart1.Axis.Z2.Labels.ItemFormatString = "";
            this.ultraChart1.Axis.Z2.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ultraChart1.Axis.Z2.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            this.ultraChart1.Axis.Z2.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ultraChart1.Axis.Z2.Labels.SeriesLabels.FontColor = System.Drawing.Color.Gray;
            this.ultraChart1.Axis.Z2.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            this.ultraChart1.Axis.Z2.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ultraChart1.Axis.Z2.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            this.ultraChart1.Axis.Z2.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ultraChart1.Axis.Z2.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ultraChart1.Axis.Z2.Labels.Visible = false;
            this.ultraChart1.Axis.Z2.LineThickness = 1;
            this.ultraChart1.Axis.Z2.MajorGridLines.AlphaLevel = ((byte)(255));
            this.ultraChart1.Axis.Z2.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
            this.ultraChart1.Axis.Z2.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ultraChart1.Axis.Z2.MajorGridLines.Visible = true;
            this.ultraChart1.Axis.Z2.MinorGridLines.AlphaLevel = ((byte)(255));
            this.ultraChart1.Axis.Z2.MinorGridLines.Color = System.Drawing.Color.LightGray;
            this.ultraChart1.Axis.Z2.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ultraChart1.Axis.Z2.MinorGridLines.Visible = false;
            this.ultraChart1.Axis.Z2.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
            this.ultraChart1.Axis.Z2.Visible = false;
            this.ultraChart1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ultraChart1.ColorModel.AlphaLevel = ((byte)(150));
            this.ultraChart1.ColorModel.ColorBegin = System.Drawing.Color.Pink;
            this.ultraChart1.ColorModel.ColorEnd = System.Drawing.Color.DarkRed;
            this.ultraChart1.ColorModel.ModelStyle = Infragistics.UltraChart.Shared.Styles.ColorModels.CustomLinear;
            this.ultraChart1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraChart1.Effects.Effects.Add(gradientEffect1);
            this.ultraChart1.Legend.Visible = true;
            this.ultraChart1.Location = new System.Drawing.Point(0, 0);
            this.ultraChart1.Name = "ultraChart1";
            chartTextAppearance1.ChartTextFont = new System.Drawing.Font("Arial", 7F);
            chartTextAppearance1.ClipText = false;
            chartTextAppearance1.Column = -2;
            chartTextAppearance1.ItemFormatString = "<DATA_VALUE:00.00>";
            chartTextAppearance1.Row = -2;
            chartTextAppearance1.Visible = true;
            scatterChartAppearance1.ChartText.Add(chartTextAppearance1);
            scatterChartAppearance1.ConnectWithLines = true;
            scatterChartAppearance1.Icon = Infragistics.UltraChart.Shared.Styles.SymbolIcon.Square;
            scatterChartAppearance1.IconSize = Infragistics.UltraChart.Shared.Styles.SymbolIconSize.Large;
            scatterChartAppearance1.NullHandling = Infragistics.UltraChart.Shared.Styles.NullHandling.DontPlot;
            this.ultraChart1.ScatterChart = scatterChartAppearance1;
            this.ultraChart1.Size = new System.Drawing.Size(905, 602);
            this.ultraChart1.TabIndex = 0;
            this.ultraChart1.Tooltips.HighlightFillColor = System.Drawing.Color.DimGray;
            this.ultraChart1.Tooltips.HighlightOutlineColor = System.Drawing.Color.DarkGray;
            this.ultraChart1.Tooltips.TooltipControl = null;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(905, 602);
            this.Controls.Add(this.ultraChart1);
            this.Name = "frmtestscatter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmtestscatter";
            this.Load += new System.EventHandler(this.frmtestscatter_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ultraChart1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinChart.UltraChart ultraChart1;
    }
}