using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.ScciResource;

namespace UnicodeSrl.ScciCore
{
    public partial class frmPUProtocolloAttivita : frmBaseModale, Interfacce.IViewFormlModal
    {

        #region Declare

        private bool _bruntime = false;
                private ucSegnalibri _ucSegnalibri = null;

        ucDatiAggiuntiviPopUp _datiaggiuntivipopup = new ucDatiAggiuntiviPopUp();

        #endregion

        public frmPUProtocolloAttivita()
        {
            InitializeComponent();
        }

        #region Interface

        public void Carica()
        {
            try
            {
                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_WORKLIST_32);

                this.InizializzaControlli();
                this.InizializzaUltraGridLayout();

                this.Aggiorna();

                                this.ShowDialog();
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "Carica", this.Text);
            }
        }

        #endregion

        #region Functions

        private void InizializzaControlli()
        {
            try
            {
                this._bruntime = true;

                                this.udteDataOraInizio.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                this.lblDescProtocolloAttivita.Text = CoreStatics.CoreApplication.MovProtocolloAttivitaSelezionato.Descrizione;

                this._bruntime = false;
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "InizializzaControlli", this.Text);
            }
        }

        private void InizializzaUltraGridLayout()
        {
            try
            {
                CoreStatics.SetEasyUltraGridLayout(ref this.ucEasyGrid);
                this.ucEasyGrid.DisplayLayout.Override.RowSizing = RowSizing.AutoFree;
                this.ucEasyGrid.DisplayLayout.Override.RowSizingAutoMaxLines = 3;
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaUltraGridLayout", this.Name);
            }
        }

        public void Aggiorna()
        {
            try
            {
                _bruntime = true;

                this.CaricaGriglia();

                _bruntime = false;
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "Aggiorna", this.Text);
            }
        }

        public bool Salva()
        {
            bool bReturn = false;

            try
            {
                this.ImpostaCursore(enum_app_cursors.WaitCursor);
                enableControls(false);

                                CoreStatics.CoreApplication.MovProtocolloAttivitaSelezionato.Periodicita.Clear();

                foreach (UltraGridRow gr in this.ucEasyGrid.Rows)
                {
                    IntervalloTempiAttivita it = new IntervalloTempiAttivita(
                                                DateTime.Parse(gr.Cells["DataOraInizio"].Value.ToString()),
                                                gr.Cells["DescrizioneProtocollo"].Value.ToString(),
                                                gr.Cells["DescrizioneTempo"].Value.ToString(),
                                                gr.Cells["CodiceTask"].Value.ToString(),
                                                gr.Cells["DescrizioneTask"].Value.ToString());
                    CoreStatics.CoreApplication.MovProtocolloAttivitaSelezionato.Periodicita.Add(it);
                }

                bReturn = CoreStatics.CoreApplication.MovProtocolloAttivitaSelezionato.CreaTaskInfermieristici();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Salva", this.Text);
            }
            finally
            {
                                enableControls(true);
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }

            return bReturn;
        }

        private bool ControllaValori()
        {
            bool bOK = true;

                                                                                    
                                                                        
            return bOK;
        }

        private void enableControls(bool vEnable)
        {
            try
            {
                this.PulsanteAvantiAbilitato = vEnable;
                this.PulsanteIndietroAbilitato = vEnable;
                this.ucTopModale.Enabled = vEnable;
            }
            catch
            {
            }
        }

        private Control SetPopUpControl(string descrizione, EnumTipoDatoAggiuntivo tipo, string valore, string valori)
        {
            System.Windows.Forms.Control oret = null;

            try
            {
                _datiaggiuntivipopup.Descrizione = descrizione;
                _datiaggiuntivipopup.TipoDato = tipo;
                _datiaggiuntivipopup.ListaValori = valori;
                _datiaggiuntivipopup.Valore = valore;

                oret = _datiaggiuntivipopup;
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "SetPopUpControl", this.Text);
                oret = null;
            }

            return oret;
        }

        #endregion

        #region UltraGrid

        private void CaricaGriglia()
        {

            DateTime dtInizio = DateTime.MinValue;

            try
            {
                DateTime.TryParse(this.udteDataOraInizio.Value.ToString(), out dtInizio);

                if (dtInizio != DateTime.MinValue)
                {
                    CoreStatics.CoreApplication.MovProtocolloAttivitaSelezionato.DataOraInizio = dtInizio;

                    List<IntervalloTempiAttivita> tempi = CoreStatics.CoreApplication.MovProtocolloAttivitaSelezionato.GeneraPeriodicita();
                    this.lblElencoSomministrazioni.Text = "Elenco Somministrazioni (" + tempi.Count + ")";
                    if (tempi.Count > 0)
                        this.ucEasyGrid.DataSource = tempi;
                    else
                        this.ucEasyGrid.DataSource = null;
                    this.ucEasyGrid.Refresh();

                }
                else
                {
                    this.ucEasyGrid.DataSource = null;
                }

                this.ucEasyGrid.Refresh();

            }
            catch
            {
            }

        }

        #endregion

        #region Events Form

        private void frmPUTaskInfermieristici_ImmagineClick(object sender, ImmagineTopClickEventArgs e)
        {

            try
            {

                switch (e.Tipo)
                {

                    case EnumImmagineTop.Segnalibri:
                        if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Bookmarks > 0)
                        {
                            CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerSegnalibri);
                            _ucSegnalibri = new ucSegnalibri();
                            int iWidth = Convert.ToInt32(easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large)) * 40;
                            int iHeight = Convert.ToInt32((double)iWidth / 1.52D);
                            _ucSegnalibri.Size = new Size(iWidth, iHeight);
                            _ucSegnalibri.ViewInit();
                            _ucSegnalibri.Focus();
                            this.UltraPopupControlContainerSegnalibri.Show();
                        }
                        break;

                    case EnumImmagineTop.SegnalibroAdd:
                        break;

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void frmPUTaskInfermieristici_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            try
            {

                if (Salva())
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        private void frmPUTaskInfermieristici_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        #endregion

        #region Events

        private void ucEasyGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {


            int refWidth = (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XLarge) * 3;

            Graphics g = this.CreateGraphics();
            int refBtnWidth = Convert.ToInt32(CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(this.ucEasyGrid.DataRowFontRelativeDimension), g.DpiY) * 3);
            g.Dispose();
            g = null;
                        e.Layout.Bands[0].HeaderVisible = false;
            e.Layout.Bands[0].ColHeadersVisible = true;
            e.Layout.Bands[0].RowLayoutStyle = RowLayoutStyle.GroupLayout;
                        foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
            {

                oCol.SortIndicator = SortIndicator.Disabled;
                switch (oCol.Key)
                {

                    case "DataOraInizio":
                        oCol.Header.Caption = "Data/Ora Inizio";
                        oCol.Format = "dddd dd/MM/yyyy HH:mm";
                        oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                        try
                        {
                            oCol.MaxWidth = refWidth * 3;
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

                    case "DescrizioneTempo":
                        oCol.Hidden = false;
                        oCol.Header.Caption = "Descrizione";
                        oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                        try
                        {
                            oCol.MaxWidth = refWidth * 4;
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

                    case "DescrizioneTask":
                        oCol.Hidden = false;
                        oCol.Header.Caption = "Attività";
                        oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                        try
                        {
                            oCol.MaxWidth = refWidth * 4;
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

            if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_EDIT))
            {
                UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_EDIT);
                colEdit.Hidden = !this.ucEasyCheckEditorManuale.Checked;
                colEdit.Header.Caption = string.Empty;

                                colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                colEdit.CellActivation = Activation.AllowEdit;
                colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_MODIFICA_32);


                colEdit.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
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


                colEdit.RowLayoutColumnInfo.OriginX = 3;
                colEdit.RowLayoutColumnInfo.OriginY = 0;
                colEdit.RowLayoutColumnInfo.SpanX = 1;
                colEdit.RowLayoutColumnInfo.SpanY = 1;
            }
                        if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_EDIT + @"_SP"))
            {
                UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_EDIT + @"_SP");
                colEdit.Hidden = !this.ucEasyCheckEditorManuale.Checked;
                colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                colEdit.CellActivation = Activation.Disabled;
                colEdit.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                colEdit.Header.Caption = string.Empty;

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
                colEdit.RowLayoutColumnInfo.OriginX = 4;
                colEdit.RowLayoutColumnInfo.OriginY = 0;
                colEdit.RowLayoutColumnInfo.SpanX = 1;
                colEdit.RowLayoutColumnInfo.SpanY = 1;
            }
            if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_ANNULLA))
            {
                UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_ANNULLA);
                colEdit.Hidden = !this.ucEasyCheckEditorManuale.Checked;
                colEdit.Header.Caption = string.Empty;

                                colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                colEdit.CellActivation = Activation.AllowEdit;
                colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_ELIMINA_32);


                colEdit.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
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


                colEdit.RowLayoutColumnInfo.OriginX = 5;
                colEdit.RowLayoutColumnInfo.OriginY = 0;
                colEdit.RowLayoutColumnInfo.SpanX = 1;
                colEdit.RowLayoutColumnInfo.SpanY = 1;
            }
                        if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_ANNULLA + @"_SP"))
            {
                UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_ANNULLA + @"_SP");
                colEdit.Hidden = !this.ucEasyCheckEditorManuale.Checked;
                colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                colEdit.CellActivation = Activation.Disabled;
                colEdit.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                colEdit.Header.Caption = string.Empty;

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
                colEdit.RowLayoutColumnInfo.OriginX = 6;
                colEdit.RowLayoutColumnInfo.OriginY = 0;
                colEdit.RowLayoutColumnInfo.SpanX = 1;
                colEdit.RowLayoutColumnInfo.SpanY = 1;
            }
        }

        private void udteDataOraInizio_ValueChanged(object sender, EventArgs e)
        {
            this.CaricaGriglia();
            this.ucEasyCheckEditorManuale_CheckedChanged(this.ucEasyCheckEditorManuale, new EventArgs());
        }

        private void ucEasyCheckEditorManuale_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ucEasyGrid.DisplayLayout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_EDIT))
            {
                this.ucEasyGrid.DisplayLayout.Bands[0].Columns[CoreStatics.C_COL_BTN_EDIT].Hidden = !this.ucEasyCheckEditorManuale.Checked;
            }
            if (this.ucEasyGrid.DisplayLayout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_EDIT + @"_SP"))
            {
                this.ucEasyGrid.DisplayLayout.Bands[0].Columns[CoreStatics.C_COL_BTN_EDIT + @"_SP"].Hidden = !this.ucEasyCheckEditorManuale.Checked;
            }
            if (this.ucEasyGrid.DisplayLayout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_ANNULLA))
            {
                this.ucEasyGrid.DisplayLayout.Bands[0].Columns[CoreStatics.C_COL_BTN_ANNULLA].Hidden = !this.ucEasyCheckEditorManuale.Checked;
            }
            if (this.ucEasyGrid.DisplayLayout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_ANNULLA + @"_SP"))
            {
                this.ucEasyGrid.DisplayLayout.Bands[0].Columns[CoreStatics.C_COL_BTN_ANNULLA + @"_SP"].Hidden = !this.ucEasyCheckEditorManuale.Checked;
            }
        }

        private void ucEasyGrid_ClickCellButton(object sender, CellEventArgs e)
        {

            try
            {

                switch (e.Cell.Column.Key)
                {

                    case CoreStatics.C_COL_BTN_EDIT:

                        Rectangle rect = this.ucEasyGrid.RectangleToScreen(this.ucEasyGrid.ClientRectangle);
                        Point pt = new Point(0, 0);

                        if (this.ultraPopupControlContainer.IsDisplayed)
                        {
                                                        this.ultraPopupControlContainer.Close();
                        }

                        
                                                this.ultraPopupControlContainer.PopupControl = this.SetPopUpControl
                                                                        (
                                                                        e.Cell.Row.Cells["DescrizioneTask"].Value.ToString(),
                                                                        EnumTipoDatoAggiuntivo.DateTimeBox,
                                                                        e.Cell.Row.Cells["DataOraInizio"].Value.ToString(), ""
                                                                        );

                                                this.ultraPopupControlContainer.PopupControl.Tag = (e.Cell.Row.Index + 1).ToString();

                        pt = new Point(
                            rect.X + Convert.ToInt32(this.ucEasyGrid.Width / 2) - Convert.ToInt32(this._datiaggiuntivipopup.Width / 2),
                            rect.Y + Convert.ToInt32(this.ucEasyGrid.Height / 2) - Convert.ToInt32(this._datiaggiuntivipopup.Height / 2)
                            );

                        if (this.ultraPopupControlContainer.PopupControl != null)
                        {
                            this.ultraPopupControlContainer.Show((ucEasyGrid)sender, pt);
                        }
                        break;

                    case CoreStatics.C_COL_BTN_ANNULLA:
                        this.ucEasyGrid.Rows[e.Cell.Row.Index].Delete(false);
                        break;

                    default:
                        break;

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGrid_ClickCellButton", this.Name);
            }

        }

        #endregion

        #region UltraPopupControlContainerSegnalibri

        private void UltraPopupControlContainerSegnalibri_Closed(object sender, EventArgs e)
        {
            _ucSegnalibri.SegnalibriClick -= UltraPopupControlContainerSegnalibri_ModificaClick;
        }

        private void UltraPopupControlContainerSegnalibri_Opened(object sender, EventArgs e)
        {
            _ucSegnalibri.SegnalibriClick += UltraPopupControlContainerSegnalibri_ModificaClick;
            _ucSegnalibri.Focus();
        }

        private void UltraPopupControlContainerSegnalibri_Opening(object sender, CancelEventArgs e)
        {
            UltraPopupControlContainer popup = sender as UltraPopupControlContainer;
            popup.PopupControl = _ucSegnalibri;
        }

        private void UltraPopupControlContainerSegnalibri_ModificaClick(object sender, SegnalibriClickEventArgs e)
        {

            try
            {

                switch (e.Pulsante)
                {

                    case EnumPulsanteSegnalibri.Modifica:
                        this.UltraPopupControlContainerSegnalibri.Close();
                        this.ucTopModale.Focus();
                        CoreStatics.CaricaPopup(EnumMaschere.Scheda, EnumEntita.SCH, e.ID);
                        break;

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        #endregion

        #region UltraPopupContainer

        private void ultraPopupControlContainer_Opened(object sender, EventArgs e)
        {
            if (!this.ultraPopupControlContainer.PopupControl.ContainsFocus)
            {
                ((ucDatiAggiuntiviPopUp)this.ultraPopupControlContainer.PopupControl).Conferma_Click += ubConfermaPopupControl_Click;
                this.ultraPopupControlContainer.PopupControl.Focus();
            }
        }

        private void ultraPopupControlContainer_Closed(object sender, EventArgs e)
        {

            ((ucDatiAggiuntiviPopUp)this.ultraPopupControlContainer.PopupControl).Conferma_Click -= ubConfermaPopupControl_Click;

            CoreStatics.setCursor(enum_app_cursors.WaitCursor);

            int gridrowindex = 0;

            int.TryParse(this.ultraPopupControlContainer.PopupControl.Tag.ToString(), out gridrowindex);

            if (gridrowindex != null && gridrowindex > 0)
            {
                DateTime dtTemp = DateTime.MinValue;
                DateTime.TryParse(((ucDatiAggiuntiviPopUp)this.ultraPopupControlContainer.PopupControl).Valore, out dtTemp);

                if (dtTemp != DateTime.MinValue)
                {
                    this.ucEasyGrid.Rows[gridrowindex - 1].Cells["DataOraInizio"].Value = ((ucDatiAggiuntiviPopUp)this.ultraPopupControlContainer.PopupControl).Valore;
                }
            }

            CoreStatics.setCursor(enum_app_cursors.DefaultCursor);

        }

        private void ubConfermaPopupControl_Click(object sender, EventArgs e)
        {
            this.ultraPopupControlContainer.Close();
        }

        #endregion

    }
}
