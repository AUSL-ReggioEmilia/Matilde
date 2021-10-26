using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Data.SqlClient;
using System.Globalization;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.ScciResource;
using UnicodeSrl.ScciCore.CustomControls.Infra;
using Infragistics.Win.UltraWinGrid;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.PluginClient;

namespace UnicodeSrl.ScciCore
{
    public partial class frmValidazioni : frmBaseModale, Interfacce.IViewFormlModal
    {
        Graphics g = null;

        private ucRichTextBox _ucRichTextBox = null;

        private System.Windows.Forms.DialogResult _formDialogResult = DialogResult.Cancel;

        public frmValidazioni()
        {
            InitializeComponent();
        }

        #region INTERFACCIA

        public void Carica()
        {
            this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
            this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_FIRMA_16);

            _formDialogResult = DialogResult.Cancel;

            this.ucTopModale.PazienteVisibile = false;
            CoreStatics.SetEasyUltraGridLayout(ref this.ucEasyGrid);

            CaricaGriglia();

            this.ShowDialog();
        }

        private void CaricaGriglia()
        {
            try
            {

                this.ImpostaCursore(enum_app_cursors.WaitCursor);


                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodUtente", CoreStatics.CoreApplication.Sessione.Utente.Codice);
                op.Parametro.Add("CodStatoDiario", "IC");

                if (CoreStatics.CoreApplication.ListaIDMovDiarioClinicoSelezionati != null && CoreStatics.CoreApplication.ListaIDMovDiarioClinicoSelezionati.Count > 0)
                {
                    string[] ids = CoreStatics.CoreApplication.ListaIDMovDiarioClinicoSelezionati.ToArray();
                    op.ParametroRipetibile.Add("IDDiarioClinico", ids);
                }

                if (CoreStatics.CoreApplication.Paziente != null)
                {
                    op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Paziente.ID);
                }
                op.TimeStamp.CodEntita = EnumEntita.DCL.ToString(); op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dtGriglia = Database.GetDataTableStoredProc("MSP_SelMovDiarioClinicoTrasversale", spcoll).Copy();

                DataColumn colsp = new DataColumn(CoreStatics.C_COL_SPAZIO, typeof(string));
                colsp.AllowDBNull = true;
                colsp.DefaultValue = "";
                colsp.Unique = false;
                dtGriglia.Columns.Add(colsp);

                if (g == null) g = this.CreateGraphics();

                dtGriglia.DefaultView.Sort = "DescrPaziente ASC, DataEvento ASC";
                this.ucEasyGrid.DisplayLayout.Bands[0].Columns.ClearUnbound();
                this.ucEasyGrid.DataSource = dtGriglia.DefaultView;
                this.ucEasyGrid.Refresh();

                CoreStatics.ImpostaGroupByGriglia(ref this.ucEasyGrid, ref g, "DescrPaziente");
                this.ucEasyGrid.PerformLayout();

                if (this.ucEasyGrid.Rows.Count > 0 && this.ucEasyGrid.Rows[0].ChildBands.Count > 0 && this.ucEasyGrid.Rows[0].ChildBands[0].Rows.Count > 0)
                {
                    this.ucEasyGrid.Rows[0].ChildBands[0].Rows[0].Activate();
                    this.ucEasyGrid.Selected.Rows.Clear();
                    this.ucEasyGrid.Selected.Rows.Add(this.ucEasyGrid.Rows[0].ChildBands[0].Rows[0]);
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaGriglia", this.Text);
            }
            finally
            {

                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }
        }

        private void InitializeRow(UltraGridRow eRow)
        {
            try
            {
                if (eRow.IsGroupByRow)
                {
                    eRow.ExpansionIndicator = ShowExpansionIndicator.Never;

                    if (g == null) g = this.CreateGraphics();
                    eRow.Height = CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Medium), g.DpiY) + 10;
                }
                else
                {
                    if (eRow.IsDataRow)
                    {

                        foreach (UltraGridCell ocell in eRow.Cells)
                        {
                            if (ocell.Column.Key == CoreStatics.C_COL_BTN_EDIT)
                            {
                                if (eRow.Cells.Exists("PermessoModifica") && ocell.Row.Cells["PermessoModifica"].Value.ToString() != "1")
                                    ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                            }
                            else if (ocell.Column.Key == CoreStatics.C_COL_BTN_VALID)
                            {
                                if (eRow.Cells.Exists("PermessoUAFirma") && ocell.Row.Cells["PermessoUAFirma"].Value.ToString() == "1")
                                    ocell.ButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_TESSERAFIRMA_32);
                                else
                                    ocell.ButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_FIRMA_32);
                            }
                        }
                    }
                }




            }
            catch (Exception)
            {
            }
        }

        private void setNavigazione(bool enable)
        {
            try
            {
                CoreStatics.SetNavigazione(enable);

                this.ucBottomModale.Enabled = enable;
                this.ucEasyGrid.Enabled = enable;

            }
            catch
            {
                CoreStatics.SetNavigazione(true);
                this.ucBottomModale.Enabled = true;
            }
        }

        #endregion

        #region EVENTI

        private void ucEasyGrid_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {
                int refWidth = (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XLarge) * 3;

                Graphics g = this.CreateGraphics();
                int refBtnWidth = (CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(this.ucEasyGrid.DataRowFontRelativeDimension), g.DpiY) * 4);
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

                            case "DescrPaziente":
                                oCol.Hidden = false;
                                oCol.Header.Caption = "";
                                break;

                            case "DataEvento":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Red;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.VertScrollBar = false;
                                oCol.Format = "dd/MM/yyyy HH:mm";
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 2.5);
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

                            case "DescrUtente":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Red;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 2.5);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 0;
                                oCol.RowLayoutColumnInfo.OriginY = 1;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;


                                break;

                            case "DataInserimento":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.FontData.Italic = Infragistics.Win.DefaultableBoolean.True;
                                oCol.CellAppearance.ForeColor = Color.Gray;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.Format = "(dd/MM/yyyy HH:mm)";
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 2.5);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }


                                oCol.RowLayoutColumnInfo.OriginX = 0;
                                oCol.RowLayoutColumnInfo.OriginY = 2;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;


                                break;

                            case CoreStatics.C_COL_SPAZIO:
                                oCol.Hidden = false;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XXSmall);
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 2.5);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.WeightY = 1;
                                oCol.RowLayoutColumnInfo.OriginX = 0;
                                oCol.RowLayoutColumnInfo.OriginY = 3;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "AnteprimaRTF":
                                oCol.Hidden = false;

                                RichTextEditor a = new RichTextEditor();
                                oCol.Editor = a;
                                oCol.CellActivation = Activation.ActivateOnly;
                                oCol.CellClickAction = CellClickAction.CellSelect;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.VertScrollBar = true;
                                try
                                {
                                    oCol.MaxWidth = this.ucEasyGrid.Width - Convert.ToInt32(refWidth * 2.5) - Convert.ToInt32(refBtnWidth * 4.75) - 40;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.CellAppearance.BorderAlpha = Infragistics.Win.Alpha.Opaque;
                                oCol.CellAppearance.BorderColor = Color.Red;
                                oCol.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.Opaque;
                                oCol.CellAppearance.BackColor = Color.WhiteSmoke;

                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 4;

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
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_VALID))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_VALID);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_FIRMA_32);


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


                    colEdit.RowLayoutColumnInfo.OriginX = 2;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_VALID + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_VALID + @"_SP");
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
                    colEdit.RowLayoutColumnInfo.OriginX = 3;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
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
                    colEdit.CellButtonAppearance.Image = Properties.Resources.Modifica_32;


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
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
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
                    colEdit.RowLayoutColumnInfo.OriginX = 5;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
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
                    colEdit.CellButtonAppearance.Image = Properties.Resources.Cancella_32;


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
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
                }
                if (!e.Layout.Bands[0].Columns.Exists(@"COLFINE_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(@"COLFINE_SP");
                    colEdit.Hidden = false;
                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                    colEdit.CellActivation = Activation.Disabled;
                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = Convert.ToInt32(refBtnWidth * 0.1);
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
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
                }

                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_CARTELLA))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_CARTELLA);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_CARTELLACLINICA_32);


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


                    colEdit.RowLayoutColumnInfo.OriginX = 8;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
                }

                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_CARTELLA + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_CARTELLA + @"_SP");
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
                    colEdit.RowLayoutColumnInfo.OriginX = 9;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
                }
            }
            catch (Exception ex)
            {
                string aa = ex.Message;
            }
        }

        private void ucEasyGrid_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            InitializeRow(e.Row);
        }

        private void ucEasyGrid_ClickCell(object sender, ClickCellEventArgs e)
        {

            try
            {

                switch (e.Cell.Column.Key)
                {

                    case "AnteprimaRTF":
                        CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainer);
                        _ucRichTextBox = CoreStatics.GetRichTextBoxForPopupControlContainer(e.Cell.Text);

                        Infragistics.Win.UIElement uie = e.Cell.GetUIElement();
                        Point oPoint = new Point(uie.Rect.Left, uie.Rect.Top);

                        this.UltraPopupControlContainer.Show(this.ucEasyGrid, oPoint);
                        break;

                    default:
                        break;

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGrid_ClickCell", this.Name);
            }
        }

        private void frmValidazioni_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            frmSmartCardProgress frmSC = null;
            bool bOK = false;
            try
            {
                if (this.ucEasyGrid.Rows.Count <= 0)
                {
                    easyStatics.EasyMessageBox("Non ci sono voci da Validare!", "Validazione Diario Clinico", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    if (easyStatics.EasyMessageBox("Sei sicuro di voler VALIDARE" + Environment.NewLine + "TUTTE la voci visualizzate?", "Validazione Diario Clinico", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {

                        this.ImpostaCursore(enum_app_cursors.WaitCursor);

                        SortedList<string, string> slIDs = new SortedList<string, string>();
                        int iCount = 0;
                        bool firmaDigitale = false;
                        foreach (UltraGridRow ugrRow in this.ucEasyGrid.Rows)
                        {
                            if (ugrRow.IsGroupByRow && ugrRow.ChildBands.Count > 0 && ugrRow.ChildBands[0].Rows.Count > 0)
                            {
                                foreach (UltraGridRow childRow in ugrRow.ChildBands[0].Rows)
                                {
                                    if (childRow.IsDataRow && !childRow.IsDeleted)
                                    {
                                        iCount += 1;
                                        string sInfo = childRow.Cells["PermessoUAFirma"].Value + @"|" + @"Diario " + childRow.Cells["DescrTipoDiario"].Text + @" del " + Convert.ToDateTime(childRow.Cells["DataInserimento"].Value).ToString("dd/MM/yyyy HH:mm");
                                        slIDs.Add(childRow.Cells["ID"].Text, sInfo);

                                        if (childRow.Cells["PermessoUAFirma"].Text.Trim() == "1")
                                        {
                                            firmaDigitale = true;
                                            iCount += 2;
                                        }
                                    }
                                }
                            }
                            else if (!ugrRow.IsGroupByRow && ugrRow.IsDataRow && !ugrRow.IsDeleted)
                            {
                                iCount += 1;
                                string sInfo = ugrRow.Cells["PermessoUAFirma"].Value + @"|" + @"Diario " + ugrRow.Cells["DescrTipoDiario"].Text + @" del " + Convert.ToDateTime(ugrRow.Cells["DataInserimento"].Value).ToString("dd/MM/yyyy HH:mm");
                                slIDs.Add(ugrRow.Cells["ID"].Text, sInfo);

                                if (ugrRow.Cells["PermessoUAFirma"].Text.Trim() == "1")
                                {
                                    firmaDigitale = true;
                                    iCount += 2;
                                }
                            }
                        }


                        if (firmaDigitale)
                        {
                            setNavigazione(false);
                            frmSC = new frmSmartCardProgress();
                            frmSC.InizializzaEMostra(0, iCount + 1, this);
                            frmSC.SetCursore(enum_app_cursors.WaitCursor);
                        }

                        for (int i = 0; i < slIDs.Count; i++)
                        {
                            bool bContinua = true;
                            string idMovDiarioClinico = slIDs.Keys[i];
                            string sDescrDiario = slIDs[idMovDiarioClinico].Split('|')[1];
                            bool permessoUAFirma = (slIDs[idMovDiarioClinico].Split('|')[0] == "1");

                            if (frmSC != null) frmSC.SetStato(@"Validazione " + sDescrDiario);

                            if (permessoUAFirma)
                            {
                                bContinua = false;

                                if (frmSC.TerminaOperazione)
                                {
                                    i = slIDs.Count + 1;
                                }
                                else
                                {
                                    try
                                    {
                                        frmSC.SetStato(@"Generazione Documento...");

                                        byte[] pdfContent = CoreStatics.GeneraPDFValidazioneDiario(idMovDiarioClinico, true);

                                        if (pdfContent == null || pdfContent.Length <= 0)
                                        {
                                            frmSC.SetLog(@"Errore Generazione documento", true);
                                        }
                                        else
                                        {
                                            bContinua = frmSC.ProvaAFirmare(ref pdfContent, EnumTipoDocumentoFirmato.DCLFM01, "Firma Digitale...", EnumEntita.DCL, idMovDiarioClinico);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        if (frmSC != null) frmSC.SetLog(@"Errore Generazione documento: " + ex.Message, true);
                                        bContinua = false;
                                    }
                                }
                            }
                            if (bContinua)
                            {
                                MovDiarioClinico movdc = new MovDiarioClinico(idMovDiarioClinico, CoreStatics.CoreApplication.Ambiente);
                                Risposta oRispostaElaboraPrima = new Risposta();
                                oRispostaElaboraPrima = PluginClientStatics.PluginClient(EnumPluginClient.DCL_VALIDA_PRIMA.ToString(), new object[1] { movdc }, CommonStatics.UAPadri(movdc.CodUA, CoreStatics.CoreApplication.Ambiente));
                                if (oRispostaElaboraPrima.Successo)
                                {
                                    bOK = movdc.Valida();
                                }
                                else
                                {
                                    bOK = false;
                                    easyStatics.EasyMessageBox(oRispostaElaboraPrima.ex.Message, "Validazione Diario Clinico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "frmValidazioni_PulsanteAvantiClick", this.Text);
            }
            finally
            {
                if (frmSC != null)
                {
                    frmSC.Close();
                    frmSC.Dispose();
                }

                setNavigazione(true);

                this.ImpostaCursore(enum_app_cursors.DefaultCursor);

            }

            if (bOK)
            {
                _formDialogResult = DialogResult.OK;
                this.DialogResult = _formDialogResult;
                this.Close();
            }
        }







        private void frmValidazioni_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            ChiudiSuIndietro();
        }

        private void ucEasyGrid_ClickCellButton(object sender, CellEventArgs e)
        {
            try
            {
                CoreStatics.CoreApplication.MovDiarioClinicoDaAnnullare = null;
                MovDiarioClinico movdc = null;
                bool NascondiPaziente = false;
                bool DiarioSenzaContesto = false;

                if (CoreStatics.CoreApplication.Paziente != null)
                {
                    NascondiPaziente = true;
                }

                switch (e.Cell.Column.Key)
                {
                    case CoreStatics.C_COL_BTN_CARTELLA:
                        string idCartella = string.Empty;
                        string idPaziente = string.Empty;
                        string idEpisodio = string.Empty;
                        string idTrasferimento = string.Empty;

                        idCartella = e.Cell.Row.Cells["IDCartella"].Text;
                        idPaziente = e.Cell.Row.Cells["IDPaziente"].Text;
                        idEpisodio = e.Cell.Row.Cells["IDEpisodio"].Text;
                        idTrasferimento = e.Cell.Row.Cells["IDTrasferimento"].Text;

                        if (idCartella != string.Empty && idPaziente != string.Empty)
                        {
                            ApriCartella(idCartella, idPaziente, idEpisodio, idTrasferimento);


                            _formDialogResult = DialogResult.OK;

                            ChiudiSuIndietro();
                        }

                        break;

                    case CoreStatics.C_COL_BTN_EDIT:

                        movdc = new MovDiarioClinico(e.Cell.Row.Cells["ID"].Text, EnumAzioni.MOD, CoreStatics.CoreApplication.Ambiente);
                        CoreStatics.CoreApplication.MovDiarioClinicoSelezionato = movdc;

                        if (CoreStatics.CoreApplication.Paziente == null)
                        {
                            DiarioSenzaContesto = true;
                            CaricaContestoDaDiario(CoreStatics.CoreApplication.MovDiarioClinicoSelezionato);
                        }

                        if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingVoceDiDiario) == DialogResult.OK)
                        {
                            if (DiarioSenzaContesto) SvuotaContesto();

                            CaricaGriglia();

                            this.ucEasyGrid.ActiveRow = null;
                            RowsCollection gridrows = this.ucEasyGrid.Rows;
                            if (CoreStatics.CoreApplication.MovDiarioClinicoSelezionato != null) CoreStatics.SelezionaRigaInGriglia(ref gridrows, "ID", CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.IDMovDiario);

                            _formDialogResult = DialogResult.OK;
                        }
                        if (DiarioSenzaContesto) SvuotaContesto();

                        break;

                    case CoreStatics.C_COL_BTN_DEL:
                        if (easyStatics.EasyMessageBox("Sei sicuro di voler CANCELLARE" + Environment.NewLine + "la voce corrente?", "Cancellazione Diario Clinico", MessageBoxButtons.YesNo, MessageBoxIcon.Question, NascondiPaziente) == DialogResult.Yes)
                        {
                            movdc = new MovDiarioClinico(e.Cell.Row.Cells["ID"].Text, CoreStatics.CoreApplication.Ambiente);
                            if (movdc.Cancella())
                            {
                                CaricaGriglia();

                                _formDialogResult = DialogResult.OK;
                            }
                        }
                        break;

                    case CoreStatics.C_COL_BTN_VALID:
                        if (easyStatics.EasyMessageBox("Sei sicuro di voler VALIDARE" + Environment.NewLine + "la voce corrente?", "Validazione Diario Clinico", MessageBoxButtons.YesNo, MessageBoxIcon.Question, NascondiPaziente) == DialogResult.Yes)
                        {
                            frmSmartCardProgress frmSC = null;
                            bool bContinua = true;
                            try
                            {
                                this.ImpostaCursore(enum_app_cursors.WaitCursor);

                                if (e.Cell.Row.Cells["PermessoUAFirma"].Text.Trim() == "1")
                                {
                                    bContinua = false;


                                    setNavigazione(false);
                                    frmSC = new frmSmartCardProgress();
                                    frmSC.InizializzaEMostra(0, 4, this);
                                    frmSC.SetCursore(enum_app_cursors.WaitCursor);

                                    frmSC.SetStato(@"Validazione Diario " + e.Cell.Row.Cells["DescrTipoDiario"].Text + @" del " + Convert.ToDateTime(e.Cell.Row.Cells["DataInserimento"].Value).ToString("dd/MM/yyyy HH:mm"));


                                    try
                                    {
                                        frmSC.SetStato(@"Generazione Documento...");

                                        byte[] pdfContent = CoreStatics.GeneraPDFValidazioneDiario(e.Cell.Row.Cells["ID"].Text, true);

                                        if (pdfContent == null || pdfContent.Length <= 0)
                                        {
                                            frmSC.SetLog(@"Errore Generazione documento", true);
                                        }
                                        else
                                        {
                                            bContinua = frmSC.ProvaAFirmare(ref pdfContent, EnumTipoDocumentoFirmato.DCLFM01, "Firma Digitale...", EnumEntita.DCL, e.Cell.Row.Cells["ID"].Text);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        if (frmSC != null) frmSC.SetLog(@"Errore Generazione documento: " + ex.Message, true);
                                        bContinua = false;
                                    }

                                }
                                if (bContinua)
                                {
                                    movdc = new MovDiarioClinico(e.Cell.Row.Cells["ID"].Text, CoreStatics.CoreApplication.Ambiente);
                                    Risposta oRispostaElaboraPrima = new Risposta();
                                    oRispostaElaboraPrima = PluginClientStatics.PluginClient(EnumPluginClient.DCL_VALIDA_PRIMA.ToString(), new object[1] { movdc }, CommonStatics.UAPadri(movdc.CodUA, CoreStatics.CoreApplication.Ambiente));
                                    if (oRispostaElaboraPrima.Successo)
                                    {
                                        bContinua = movdc.Valida();
                                    }
                                    else
                                    {
                                        bContinua = false;
                                        easyStatics.EasyMessageBox(oRispostaElaboraPrima.ex.Message, "Validazione Diario Clinico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }

                                if (bContinua) _formDialogResult = DialogResult.OK;

                            }
                            catch (Exception ex)
                            {
                                CoreStatics.ExGest(ref ex, "ucEasyGrid_ClickCellButton", this.Name);
                            }
                            finally
                            {
                                if (frmSC != null)
                                {
                                    frmSC.Close();
                                    frmSC.Dispose();
                                }

                                setNavigazione(true);

                                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
                            }

                            if (bContinua) CaricaGriglia();
                        }
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGrid_ClickCellButton", this.Text);
            }
        }

        #endregion

        #region UltraPopupControlContainer

        private void UltraPopupControlContainer_Closed(object sender, EventArgs e)
        {
            _ucRichTextBox.RtfClick -= ucRichTextBox_Click;
        }

        private void UltraPopupControlContainer_Opened(object sender, EventArgs e)
        {
            _ucRichTextBox.RtfClick += ucRichTextBox_Click;
        }

        private void UltraPopupControlContainer_Opening(object sender, CancelEventArgs e)
        {
            Infragistics.Win.Misc.UltraPopupControlContainer popup = sender as Infragistics.Win.Misc.UltraPopupControlContainer;
            popup.PopupControl = _ucRichTextBox;
        }

        private void ucRichTextBox_Click(object sender, EventArgs e)
        {
        }

        #endregion












        #region private functions
        private void CaricaContestoDaDiario(MovDiarioClinico Diario)
        {

            try
            {
                if (Diario != null)
                {
                    CoreStatics.CoreApplication.Paziente = new Paziente("", Diario.IDEpisodio);
                    CoreStatics.CoreApplication.Episodio = new Episodio(Diario.IDEpisodio);
                    CoreStatics.CoreApplication.Trasferimento = new Trasferimento(Diario.IDTrasferimento, CoreStatics.CoreApplication.Ambiente);
                }
            }
            catch (Exception)
            {
            }

        }

        private void ChiudiSuIndietro()
        {
            this.DialogResult = _formDialogResult;
            this.Close();
        }

        private void ApriCartella(string idCartella, string idPaziente, string idEpisodio, string idTrasferimento)
        {
            try
            {


                SvuotaContesto();




                CoreStatics.CoreApplication.Paziente = new Paziente(idPaziente, idEpisodio);
                if (idEpisodio.Trim() != string.Empty)
                {
                    CoreStatics.CoreApplication.Episodio = new Episodio(idEpisodio.Trim());
                }
                else
                {
                    CoreStatics.CoreApplication.Episodio = null;
                }
                if (idTrasferimento.Trim() != string.Empty)
                {
                    CoreStatics.CoreApplication.Trasferimento = new Trasferimento(idTrasferimento.Trim(), CoreStatics.CoreApplication.Ambiente);
                }
                else
                {
                    CoreStatics.CoreApplication.Trasferimento = null;
                }
                CoreStatics.CoreApplication.Cartella = new Cartella(idCartella.Trim(), "", CoreStatics.CoreApplication.Ambiente);

                if (CoreStatics.CoreApplication.Cartella != null)
                {

                    if (CoreStatics.CoreApplication.Cartella.CodStatoCartella == EnumStatoCartella.CH.ToString())
                    {
                        CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.CartellaPazienteChiusa);
                    }
                    else
                    {
                        CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.CartellaPaziente);
                    }

                    CoreStatics.CoreApplication.Navigazione.Maschere.RimuoviMaschereMassimizzabili();

                    CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.MascheraPartenza = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata;
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ApriCartella", this.Name);
            }
        }


        private void SvuotaContesto()
        {

            try
            {

                CoreStatics.CoreApplication.Paziente = null;
                CoreStatics.CoreApplication.Episodio = null;
                CoreStatics.CoreApplication.Trasferimento = null;
                CoreStatics.CoreApplication.Cartella = null;

            }
            catch (Exception)
            {
            }

        }

        #endregion
    }
}
