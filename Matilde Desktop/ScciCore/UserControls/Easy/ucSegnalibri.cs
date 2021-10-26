using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win.UltraWinGrid;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.ScciResource;

namespace UnicodeSrl.ScciCore
{
    public partial class ucSegnalibri : UserControl, Interfacce.IViewUserControlBase
    {
        public ucSegnalibri()
        {
            InitializeComponent();
        }

        #region Declare

        public event SegnalibriClickHandler SegnalibriClick;

        #endregion

        #region Interface

        public void ViewInit()
        {
            this.LoadUltraGridSegnalibri();
        }

        #endregion

        #region Subroutine

        private void LoadUltraGridSegnalibri()
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                if (CoreStatics.CoreApplication.Paziente != null)
                {
                    op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Paziente.ID);
                }
                if (CoreStatics.CoreApplication.Cartella != null)
                {
                    op.Parametro.Add("IDCartella", CoreStatics.CoreApplication.Cartella.ID);
                }

                op.TimeStamp.CodEntita = EnumEntita.SGL.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_SelMovSegnalibri", spcoll);

                DataTable dtEdit = ds.Tables[0].Copy();

                DataColumn colsp = new DataColumn(CoreStatics.C_COL_SPAZIO, typeof(string));
                colsp.AllowDBNull = true;
                colsp.DefaultValue = "";
                colsp.Unique = false;
                dtEdit.Columns.Add(colsp);

                foreach (DataColumn dcCol in dtEdit.Columns)
                {
                    if (dcCol.ColumnName.ToUpper().IndexOf("PERMESSOMODIFICA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("ICONA") == 0)
                        dcCol.ReadOnly = false;
                }

                if (this.ucEasyGridSegnalibri.DisplayLayout != null)
                {
                    this.ucEasyGridSegnalibri.DataSource = null;
                }

                this.ucEasyGridSegnalibri.DataSource = dtEdit;
                this.ucEasyGridSegnalibri.Text = string.Format("Numero Segnalibri : {0}", ds.Tables[0].Rows.Count.ToString());
                this.ucEasyGridSegnalibri.Refresh();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "LoadUltraGridSegnalibri", "ucSegnalibri");
            }

        }

        private void CancellaSegnalibro(string id)
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDSegnalibro", id);

                op.TimeStamp.CodEntita = EnumEntita.SGL.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.CAN.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                Database.ExecStoredProc("MSP_DelMovSegnalibri", spcoll);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        #endregion

        #region Events

        private void ucEasyGridSegnalibri_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {

                int refWidth = (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XLarge) * 3;

                Graphics g = this.CreateGraphics();
                int refBtnWidth = Convert.ToInt32(CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(this.ucEasyGridSegnalibri.DataRowFontRelativeDimension), g.DpiY) * 3);
                g.Dispose();
                g = null;
                e.Layout.Bands[0].HeaderVisible = false;
                e.Layout.Bands[0].ColHeadersVisible = false;
                e.Layout.Bands[0].RowLayoutStyle = RowLayoutStyle.GroupLayout;
                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {

                    try
                    {

                        oCol.SortIndicator = SortIndicator.Disabled;
                        switch (oCol.Key)
                        {

                            case "Icona":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                                oCol.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellAppearance.ImageVAlign = Infragistics.Win.VAlign.Top;
                                oCol.VertScrollBar = false;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.LockedWidth = true;
                                try
                                {
                                    oCol.MaxWidth = refWidth;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }
                                oCol.RowLayoutColumnInfo.OriginX = 0;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;
                                break;


                            case "Descrizione":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = this.ucEasyGridSegnalibri.Width - (refWidth * 1) - Convert.ToInt32(refBtnWidth * 2.50) - 30;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }
                                oCol.RowLayoutColumnInfo.OriginX = 2;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;
                                break;


                            default:
                                oCol.Hidden = true;
                                break;

                        }

                    }
                    catch (Exception)
                    {
                    }

                }

                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_DEL))
                {

                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_DEL);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_SEGNALIBRODEL_32);

                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = refBtnWidth;
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;

                    colEdit.RowLayoutColumnInfo.OriginX = 4;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 1;

                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_DEL + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_DEL + @"_SP");
                    colEdit.Hidden = false;
                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                    colEdit.CellActivation = Activation.Disabled;
                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = Convert.ToInt32(refBtnWidth * 0.25);
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;
                    colEdit.RowLayoutColumnInfo.OriginX = 5;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 1;
                }

                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_EDIT))
                {

                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_EDIT);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_MODIFICA_32);

                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = refBtnWidth;
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;

                    colEdit.RowLayoutColumnInfo.OriginX = 6;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 1;

                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_EDIT + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_EDIT + @"_SP");
                    colEdit.Hidden = false;
                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                    colEdit.CellActivation = Activation.Disabled;
                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = Convert.ToInt32(refBtnWidth * 0.25);
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;
                    colEdit.RowLayoutColumnInfo.OriginX = 7;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 1;
                }

            }
            catch (Exception)
            {
            }

        }

        private void ucEasyGridSegnalibri_InitializeRow(object sender, InitializeRowEventArgs e)
        {

            try
            {

                if (e.Row.Cells.Exists("Icona") == true)
                {
                    e.Row.Cells["Icona"].Value = CoreStatics.ImageToByte(Risorse.GetImageFromResource(Risorse.GC_SCHEDA_32));
                    e.Row.Update();
                }

                foreach (UltraGridCell ocell in e.Row.Cells)
                {

                    if (ocell.Column.Key == CoreStatics.C_COL_BTN_EDIT && ocell.Row.Cells["PermessoModifica"].Value.ToString() == "0")
                    {
                        ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                    }

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGridSegnalibri_InitializeRow", this.Name);
            }

        }

        private void ucEasyGridSegnalibri_ClickCellButton(object sender, CellEventArgs e)
        {

            try
            {

                switch (e.Cell.Column.Key)
                {

                    case CoreStatics.C_COL_BTN_EDIT:
                        if (e.Cell.Row.Cells["PermessoModifica"].Text == "1")
                        {
                            if (SegnalibriClick != null) { SegnalibriClick(sender, new SegnalibriClickEventArgs(EnumPulsanteSegnalibri.Modifica, e.Cell.Row.Cells["IDScheda"].Text)); }
                        }
                        break;

                    case CoreStatics.C_COL_BTN_DEL:
                        this.CancellaSegnalibro(e.Cell.Row.Cells["ID"].Text);
                        this.ViewInit();
                        break;

                    default:
                        break;

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGridSegnalibri_ClickCellButton", this.Name);
            }

        }

        #endregion

    }
}
