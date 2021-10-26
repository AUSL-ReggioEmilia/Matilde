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
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.ScciResource;

namespace UnicodeSrl.ScciCore
{
    public partial class ucPazientiSeguiti : UserControl, Interfacce.IViewUserControlBase
    {
        public ucPazientiSeguiti()
        {
            InitializeComponent();
        }

        #region Declare

        public event PazientiSeguitiClickHandler PazientiSeguitiClick;

        #endregion

        #region Interface

        public void ViewInit()
        {


            this.ubAdd.Appearance.Image = Properties.Resources.Aggiungi_256;
            this.ubAdd.PercImageFill = 0.75F;
            this.ubAdd.ShortcutKey = Keys.None;
            this.ubAdd.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.XSmall;
            this.ubAdd.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.ubAdd.Enabled = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.PazientiSeguiti_Inserisci);

            if (this.ubAdd.Enabled == true)
            {
                if (CoreStatics.CoreApplication.Paziente == null || CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.PazienteSeguito > 0)
                {
                    this.ubAdd.Enabled = false;
                }
            }

            this.ucEasyGridPazientiSeguiti.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.None;

            this.ucEasyGridPazientiSeguiti.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.ucEasyGridPazientiSeguiti.DisplayLayout.Override.RowFilterMode = RowFilterMode.AllRowsInBand;
            this.ucEasyGridPazientiSeguiti.DisplayLayout.Override.RowFilterMode = RowFilterMode.Default;
            this.ucEasyGridPazientiSeguiti.DisplayLayout.Override.FilterUIType = FilterUIType.FilterRow;

            this.ucEasyGridPazientiSeguiti.DisplayLayout.Override.FilterClearButtonLocation = FilterClearButtonLocation.Row;

            this.ucEasyGridPazientiSeguiti.DisplayLayout.Override.FilterRowAppearance.BackColor = Color.LightYellow;
            this.ucEasyGridPazientiSeguiti.DisplayLayout.Override.FilterComparisonType = FilterComparisonType.CaseInsensitive;

            this.ucEasyGridPazientiSeguiti.DisplayLayout.Override.FilterOperatorDefaultValue = FilterOperatorDefaultValue.Contains;
            this.ucEasyGridPazientiSeguiti.DisplayLayout.Override.FilterOperatorLocation = FilterOperatorLocation.Hidden;
            this.ucEasyGridPazientiSeguiti.DisplayLayout.Override.FilterOperandStyle = FilterOperandStyle.Edit;

            this.ucEasyGridPazientiSeguiti.DisplayLayout.Override.FilterRowPrompt = "";
            this.ucEasyGridPazientiSeguiti.DisplayLayout.Override.FilterRowPromptAppearance.BackColorAlpha = Infragistics.Win.Alpha.Opaque;
            this.ucEasyGridPazientiSeguiti.DisplayLayout.Override.SpecialRowSeparator = SpecialRowSeparator.FilterRow;

            this.ucEasyGridPazientiSeguitiDaAltri.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.None;

            this.LoadUltraGridPazientiSeguiti();

            if (CoreStatics.CoreApplication.Paziente != null)
            {
                this.ucEasyGridPazientiSeguitiDaAltri.Visible = true;
                this.ucEasyTableLayoutPanel.SetRowSpan(this.ucEasyGridPazientiSeguiti, 1);
            }
            else
            {
                this.ucEasyGridPazientiSeguitiDaAltri.Visible = false;
                this.ucEasyTableLayoutPanel.SetRowSpan(this.ucEasyGridPazientiSeguiti, 3);
            }

        }

        #endregion

        #region Subroutine

        private void LoadUltraGridPazientiSeguiti()
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodUtente", CoreStatics.CoreApplication.Sessione.Utente.Codice);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                op.Parametro.Add("PazientiSeguitiDaUtente", "1");
                op.Parametro.Add("PazientiSeguitiDaAltri", "1");
                if (CoreStatics.CoreApplication.Paziente != null)
                {
                    op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Paziente.ID);
                }

                op.TimeStamp.CodEntita = EnumEntita.PZS.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_SelMovPazientiSeguiti", spcoll);

                DataTable dtEdit = ds.Tables[0].Copy();

                DataColumn colsp = new DataColumn(CoreStatics.C_COL_SPAZIO, typeof(string));
                colsp.AllowDBNull = true;
                colsp.DefaultValue = "";
                colsp.Unique = false;
                dtEdit.Columns.Add(colsp);

                foreach (DataColumn dcCol in dtEdit.Columns)
                {
                    if (dcCol.ColumnName.ToUpper().IndexOf("PERMESSOCANCELLA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("ICONA") == 0)
                        dcCol.ReadOnly = false;
                }

                if (this.ucEasyGridPazientiSeguiti.DisplayLayout != null)
                {
                    this.ucEasyGridPazientiSeguiti.DataSource = null;
                }

                this.ucEasyGridPazientiSeguiti.DataSource = dtEdit;
                this.ucEasyGridPazientiSeguiti.Text = string.Format("Numero Pazienti Seguiti : {0}", ds.Tables[0].Rows.Count.ToString());
                this.ucEasyGridPazientiSeguiti.Refresh();

                if (this.ucEasyGridPazientiSeguiti.Rows != null && this.ucEasyGridPazientiSeguiti.Rows.Count > 0 && CoreStatics.CoreApplication.Paziente != null)
                {
                    this.ucEasyGridPazientiSeguiti.ActiveRow = null;
                    this.ucEasyGridPazientiSeguiti.Selected.Rows.Clear();
                    CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGridPazientiSeguiti, "IDPaziente", CoreStatics.CoreApplication.Paziente.ID);
                }

                this.ucEasyGridPazientiSeguitiDaAltri.DataSource = ds.Tables[1];

                string nomepaziente = string.Empty;
                if (CoreStatics.CoreApplication.Paziente != null) nomepaziente = CoreStatics.CoreApplication.Paziente.Cognome + " " + CoreStatics.CoreApplication.Paziente.Nome;

                this.ucEasyGridPazientiSeguitiDaAltri.Text = string.Format("Altri Utenti che seguono il paziente {0}: {1}", nomepaziente, ds.Tables[1].Rows.Count.ToString());
                this.ucEasyGridPazientiSeguitiDaAltri.Refresh();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "LoadUltraGridPazientiSeguiti", "ucPazientiSeguiti");
            }

        }

        #endregion

        #region Events

        private void ucEasyGridPazientiSeguiti_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {

                int refWidth = (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XLarge) * 3;

                Graphics g = this.CreateGraphics();
                int refBtnWidth = Convert.ToInt32(CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(this.ucEasyGridPazientiSeguiti.DataRowFontRelativeDimension), g.DpiY) * 3);
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
                                oCol.CellAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
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
                                oCol.RowLayoutColumnInfo.SpanY = 2;
                                break;

                            case "Descrizione":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = this.ucEasyGridPazientiSeguiti.Width - (refWidth * 1) - Convert.ToInt32(refBtnWidth * 1.25) - 30;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }
                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 2;
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
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_ELIMINA_32);

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
                    colEdit.RowLayoutColumnInfo.SpanY = 2;

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
                    colEdit.RowLayoutColumnInfo.OriginX = 3;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 1;
                }

            }
            catch (Exception)
            {
            }

        }

        private void ucEasyGridPazientiSeguiti_InitializeRow(object sender, InitializeRowEventArgs e)
        {

            try
            {

                if (e.Row.Cells.Exists("Icona") == true)
                {
                    e.Row.Cells["Icona"].Value = CoreStatics.ImageToByte(Risorse.GetImageFromResource(Risorse.GC_CARTELLACLINICA_32));
                    e.Row.Update();
                }

                foreach (UltraGridCell ocell in e.Row.Cells)
                {

                    if (ocell.Column.Key == CoreStatics.C_COL_BTN_DEL && ocell.Row.Cells["PermessoCancella"].Value.ToString() == "0")
                    {
                        ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                    }

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGridPazientiSeguiti_InitializeRow", this.Name);
            }

        }

        private void ucEasyGridPazientiSeguiti_ClickCellButton(object sender, CellEventArgs e)
        {

            try
            {

                switch (e.Cell.Column.Key)
                {

                    case CoreStatics.C_COL_BTN_DEL:
                        if (e.Cell.Row.Cells["PermessoCancella"].Text == "1")
                        {
                            if (PazientiSeguitiClick != null) { PazientiSeguitiClick(sender, new PazientiSeguitiClickEventArgs(EnumPulsantePazientiSeguiti.Cancella, e.Cell.Row.Cells["ID"].Text)); }
                            this.LoadUltraGridPazientiSeguiti();
                        }
                        break;

                    default:
                        break;

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGridPazientiSeguiti_ClickCellButton", this.Name);
            }

        }

        private void ucEasyGridPazientiSeguitiDaAltri_AfterRowActivate(object sender, EventArgs e)
        {
            this.ucEasyGridPazientiSeguitiDaAltri.ActiveRow = null;
            this.ucEasyGridPazientiSeguitiDaAltri.Selected.Rows.Clear();

        }

        private void ucEasyGridPazientiSeguitiDaAltri_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {

                int refWidth = (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XLarge) * 3;

                Graphics g = this.CreateGraphics();
                int refBtnWidth = Convert.ToInt32(CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(this.ucEasyGridPazientiSeguitiDaAltri.DataRowFontRelativeDimension), g.DpiY) * 3);
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

                            case "Utente":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = (refWidth * 4);
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

                            case "Ruolo":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = this.ucEasyGridPazientiSeguitiDaAltri.Width - (refWidth * 6) - 30;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }
                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;
                                break;

                            case "Data":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.Format = "dd/MM/yyyy HH:mm";
                                try
                                {
                                    oCol.MaxWidth = refWidth * 2;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
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

            }
            catch (Exception)
            {
            }

        }

        private void ubAdd_Click(object sender, EventArgs e)
        {
            if (PazientiSeguitiClick != null) { PazientiSeguitiClick(sender, new PazientiSeguitiClickEventArgs(EnumPulsantePazientiSeguiti.Nuovo, "")); }

        }

        #endregion

    }
}
