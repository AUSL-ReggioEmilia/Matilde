using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnicodeSrl.Framework.UI.Controls;
using UnicodeSrl.Scci.Model;

namespace UnicodeSrl.ScciCore.UserControls.ScreenAndTiles
{
    public static class ExtensionsTLP
    {
        public static void AddTileControl(this ucEasyTableLayoutPanel tlp, ITileUserCtl tileControl, int row, int col, int rowSpan, int colSpan)
        {
            System.Windows.Forms.Control ctl = tileControl.Control;

            Control c = tlp.GetControlFromPosition(col, row);
            if (c != null)
            {
                tlp.Controls.Remove(c);
                c.Dispose();
            }

            tlp.Controls.Add(ctl, col, row);
            tlp.SetRowSpan(ctl, rowSpan);
            tlp.SetColumnSpan(ctl, colSpan);

            ctl.Dock = System.Windows.Forms.DockStyle.Fill;
            ctl.Margin = new System.Windows.Forms.Padding(1);
            ctl.Visible = true;
        }

        public static void AddTileControl(this ucEasyTableLayoutPanel tlp, ITileUserCtl tileControl, T_ScreenTileRow screenRow)
        {
            tlp.AddTileControl(tileControl, screenRow.Riga, screenRow.Colonna, screenRow.Altezza, screenRow.Larghezza);
        }

        public static void AddTileLabel(this ucEasyTableLayoutPanel tlp, T_ScreenTileRow tileRow, int row, int col, int rowSpan)
        {
            Control c = tlp.GetControlFromPosition(col, row);
            if (c != null) c.Visible = false;

            UniLabelExt ctl = new UniLabelExt();
            ctl.Visible = false;
            ctl.BackColor = System.Drawing.Color.Transparent;
            ctl.Font = new System.Drawing.Font("Calibri", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            ctl.FontAutoSizeCoeff = 1F;
            ctl.FontAutoSizeEnabled = true;
            ctl.ForeColor = System.Drawing.Color.Black;
            ctl.LabelText = tileRow.NomeTile;
            ctl.Location = new System.Drawing.Point(406, 99);
            ctl.Name = "TileLabelExt";
            ctl.ShortcutAlignment = System.Drawing.ContentAlignment.TopRight;
            ctl.ShortcutKeys = new List<Keys>();
            ctl.ShortcutPercentFill = 1.5F;
            ctl.ShortcutText = "";
            ctl.Size = new System.Drawing.Size(39, 259);
            ctl.TabIndex = 0;
            ctl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            ctl.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            ctl.TextRotation = System.Drawing.RotateFlipType.Rotate90FlipXY;
            ctl.UnderlineColor = System.Drawing.Color.Black;
            ctl.UnderlineOpacity = 1F;
            ctl.UnderlineVisible = false;
            ctl.UnderlineWidth = 1;
            ctl.UniUiCaption = false;
            ctl.UniUiCaptionBackground = UnicodeSrl.Framework.UI.Struct.en_StyleBackgroundType.BackgroundColor;
            ctl.UseStyle = false;

            tlp.Controls.Add(ctl, col, row);
            tlp.SetRowSpan(ctl, rowSpan);
            tlp.SetColumnSpan(ctl, tileRow.Larghezza);

            ctl.Dock = System.Windows.Forms.DockStyle.Fill;
            ctl.Margin = new System.Windows.Forms.Padding(1);
            ctl.Visible = true;
        }
    }
}
