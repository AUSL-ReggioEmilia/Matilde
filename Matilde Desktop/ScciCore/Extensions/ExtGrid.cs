using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using UnicodeSrl.ScciCore;

namespace UnicodeSrl.ScciCore.Extensions
{
    public static class GridExtensions
    {
        #region     Stili

        public static void ApplyStylSCCI(this ucEasyGrid grid, string rtfResizeColumn)
        {
            grid.DisplayLayout.Bands[0].Override.RowSizing = RowSizing.Fixed;

            grid.DisplayLayout.Bands[0].Override.RowAppearance.BackColor = Color.White;
            grid.DisplayLayout.Bands[0].Override.RowAppearance.ForeColor = Color.Black;

            grid.DisplayLayout.Bands[0].Override.RowAlternateAppearance.BackColor = Color.White;
            grid.DisplayLayout.Bands[0].Override.RowAlternateAppearance.ForeColor = Color.Black;

            grid.DisplayLayout.Bands[0].RowLayoutStyle = RowLayoutStyle.ColumnLayout;

            grid.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
            grid.DisplayLayout.Override.HeaderAppearance.TextHAlign = Infragistics.Win.HAlign.Left;

            grid.DisplayLayout.Override.ActiveRowAppearance.BackColor = Color.FromArgb(246, 246, 246);
            grid.DisplayLayout.Override.ActiveRowAppearance.ForeColor = Color.Black;

            grid.FilterRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
            grid.GridCaptionFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
            grid.HeaderFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
            grid.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;

            grid.ColonnaRTFResize = rtfResizeColumn;

        }

        #endregion


        #region Colonne e Gruppi

        public static UltraGridRow GridRowByID(this ucEasyGrid grid, string idPropName, string valueToFind)
        {
            IEnumerable<UltraGridRow> sel = grid.Rows.Where(x => x.Cells[idPropName].Value.ToString() == valueToFind);

            UltraGridRow active = null;
            if (sel.Count() > 0)
                active = sel.First();

            return active;

        }

        public static void HideAllColumns(this ucEasyGrid grid)
        {
            foreach (UltraGridColumn col in grid.DisplayLayout.Bands[0].Columns)
            {
                col.Hidden = true;
            }
        }


        public static void DisplayColumn(this UltraGridBand band, string columnName, string caption, int pos,
int sizeX = 0, float weightX = 0f,
VAlign valign = VAlign.Top, HAlign halign = HAlign.Left,
SortIndicator sort = SortIndicator.Disabled,
DefaultableBoolean multiline = DefaultableBoolean.False)

        {
            band.Columns[columnName].Hidden = false;
            band.Columns[columnName].Header.Caption = caption;
            band.Columns[columnName].Header.VisiblePosition = pos;
            band.Columns[columnName].Header.Appearance.TextHAlign = halign;

            band.Columns[columnName].CellAppearance.TextVAlign = valign;
            band.Columns[columnName].CellAppearance.TextHAlign = halign;
            band.Columns[columnName].CellMultiLine = multiline;

            band.Columns[columnName].RowLayoutColumnInfo.MinimumCellSize = new Size(sizeX, 0);
            band.Columns[columnName].RowLayoutColumnInfo.PreferredCellSize = new Size(sizeX, 0);
            band.Columns[columnName].RowLayoutColumnInfo.WeightX = weightX;

            band.Columns[columnName].SortIndicator = sort;

            band.ColHeadersVisible = true;
        }

        public static void DisplayColumn(this UltraGridGroup group, string columnName, int posX, int posY, int spanX, int spanY,
string headerCaption = "", SortIndicator sort = SortIndicator.Disabled,
int sizeX = 0, int sizeY = 0,
float weightX = 0f, float weightY = 0f,
DefaultableBoolean multiline = DefaultableBoolean.False)
        {
            UltraGridColumn col = group.Band.Columns[columnName];

            col.RowLayoutColumnInfo.ParentGroup = group;

            col.RowLayoutColumnInfo.OriginX = posX;
            col.RowLayoutColumnInfo.OriginY = posY;
            col.RowLayoutColumnInfo.SpanX = spanX;
            col.RowLayoutColumnInfo.SpanY = spanY;
            col.CellMultiLine = multiline;

            col.RowLayoutColumnInfo.MinimumCellSize = new Size(sizeX, sizeY);
            col.RowLayoutColumnInfo.PreferredCellSize = new Size(sizeX, sizeY);

            col.RowLayoutColumnInfo.WeightX = weightX;
            col.RowLayoutColumnInfo.WeightY = weightY;

            col.Header.Caption = headerCaption;

            col.SortIndicator = sort;
            col.Hidden = false;

        }


        #endregion 

    }

}
