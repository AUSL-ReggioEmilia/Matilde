using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnicodeSrl.Scci.Model;

namespace UnicodeSrl.ScciCore.UserControls.ScreenAndTiles
{
    internal enum en_ColState
    {
        Maximized = 0,
        Minimized = 1
    }

    public partial class ucTileHeader : UserControl, IDisposable
    {

        public event ScreenGridColumnEventHandler BeforeColumnMinimized;

        public event ScreenGridColumnEventHandler ColumnMinimized;


        public event ScreenGridColumnEventHandler BeforeColumnMaximized;

        public event ScreenGridColumnEventHandler ColumnMaximized;


        private const int K_MIN_WIDTH = 30;

        private T_ScreenTileRow m_tile;


        public ucTileHeader()
        {
            InitializeComponent();
        }

        public ucTileHeader(T_ScreenTileRow tileObject) : this()
        {
            this.TileObject = tileObject;
            this.ColumnState = en_ColState.Maximized;

            this.Caption = this.TileObject.NomeTile;

            if ((this.TileObject.NonCollassabile.HasValue) && (this.TileObject.NonCollassabile.Value == true))
                this.pb.Visible = false;
            else
                this.pb.Visible = true;

            setTooltip();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();

                if (this.ColumnMinimized != null) this.ColumnMinimized = null;
                if (this.ColumnMaximized != null) this.ColumnMaximized = null;

            }
            base.Dispose(disposing);
        }

        #region     Prop

        public String Caption
        {
            get { return this.lblCaption.Text; }
            set { this.lblCaption.Text = value; }
        }

        public T_ScreenTileRow TileObject
        {
            get { return m_tile; }
            private set
            {
                m_tile = value;
                setTooltip();
            }

        }

        public int GridColumnIndex
        {
            get
            {
                if (this.TileObject == null)
                    return -1;
                else
                {
                    if (this.TileObject.Fissa.Value == true)
                        return this.TileObject.Colonna + 1;
                    else
                        return this.TileObject.Colonna;

                }

            }
        }

        internal en_ColState ColumnState { get; private set; }

        private ucEasyTableLayoutPanel ParentTable
        {
            get
            {
                return (ucEasyTableLayoutPanel)this.Parent;
            }
        }

        public ucEasyTableLayoutPanel LinkedTable { get; set; }

        private float OriginalWidth { get; set; }


        #endregion  Prop



        private void setTooltip()
        {
            if (this.ColumnState == en_ColState.Maximized)
                this.tooltip.SetToolTip(this.pb, "Riduci: " + this.TileObject.NomeTile);
            else
                this.tooltip.SetToolTip(this.pb, "Espandi: " + this.TileObject.NomeTile);

        }

        public void Minimize()
        {
            if (this.ColumnState == en_ColState.Minimized)
                return;

            ScreenGridColumnEventArgs args = new ScreenGridColumnEventArgs(this.GridColumnIndex);

            this.BeforeColumnMinimized?.Invoke(this, args);

            this.OriginalWidth = this.ParentTable.ColumnStyles[this.GridColumnIndex].Width;

            setColumns(this.ParentTable, K_MIN_WIDTH, 0);

            if (this.LinkedTable != null)
                setColumns(this.LinkedTable, K_MIN_WIDTH, 0);

            this.ColumnState = en_ColState.Minimized;

            this.pb.Image = UnicodeSrl.ScciCore.Properties.Resources.FrecciaDxN_256;

            this.ColumnMinimized?.Invoke(this, args);

        }

        public void Maximize()
        {
            if (this.ColumnState == en_ColState.Maximized)
                return;

            ScreenGridColumnEventArgs args = new ScreenGridColumnEventArgs(this.GridColumnIndex);

            this.BeforeColumnMaximized?.Invoke(this, args);

            setColumns(this.ParentTable, this.OriginalWidth, this.OriginalWidth);

            if (this.LinkedTable != null)
                setColumns(this.LinkedTable, this.OriginalWidth, this.OriginalWidth);

            this.ColumnState = en_ColState.Maximized;

            this.pb.Image = UnicodeSrl.ScciCore.Properties.Resources.FrecciaSxN_256;

            this.ColumnMaximized?.Invoke(this, args);

        }

        private void setColumns(ucEasyTableLayoutPanel tlp, float width, float widthSpan)
        {
            int col = this.GridColumnIndex;
            int lastCol = col + this.TileObject.Larghezza;

            tlp.ColumnStyles[col].Width = width;

            if (this.TileObject.Larghezza > 1)
            {
                for (int i = col + 1; i < lastCol; i++)
                {
                    tlp.ColumnStyles[i].Width = widthSpan;
                }
            }
        }

        private void pb_Click(object sender, EventArgs e)
        {
            if (this.ColumnState == en_ColState.Maximized)
                this.Minimize();
            else
                this.Maximize();

            setTooltip();
        }

    }
}
