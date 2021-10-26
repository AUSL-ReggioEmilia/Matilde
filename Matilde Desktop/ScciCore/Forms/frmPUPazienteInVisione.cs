using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinListView;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.ScciCore
{
    public partial class frmPUPazienteInVisione : frmBaseModale, Interfacce.IViewFormlModal
    {
        public frmPUPazienteInVisione()
        {
            InitializeComponent();
        }

        #region Declare

        private ucEasyListBox _ucEasyListBox = null;

                private ucSegnalibri _ucSegnalibri = null;

        #endregion

        #region Interface

        public void Carica()
        {

            try
            {

                this.InizializzaControlli();

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

                                                                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodUA", CoreStatics.CoreApplication.Paziente.CodUAAmbulatoriale);

                op.TimeStamp.CodEntita = EnumEntita.CIV.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_SelRuoliCartelleInVisione", spcoll);

                this.ucEasyGridRuoli.DataSource = ds.Tables[0];
                this.ucEasyGridRuoli.Refresh();
                if (this.ucEasyGridRuoli.Rows.Count > 0)
                {
                    this.ucEasyGridRuoli.ActiveRow = null;
                    this.ucEasyGridRuoli.Selected.Rows.Clear();
                    CoreStatics.SelezionaRigaInGriglia(ref ucEasyGridRuoli, "Codice", CoreStatics.CoreApplication.MovPazienteInVisioneSelezionato.CodRuoloInVisione);
                }

                this.udteDataOraInizio.Value = CoreStatics.CoreApplication.MovPazienteInVisioneSelezionato.DataInizio;
                this.udteDataOraFine.Value = CoreStatics.CoreApplication.MovPazienteInVisioneSelezionato.DataFine;

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaControlli", this.Name);
            }

        }

        public bool Salva()
        {

            bool bReturn = false;

            try
            {

                if (ControllaValori())
                {

                    CoreStatics.CoreApplication.MovPazienteInVisioneSelezionato.CodRuoloInVisione = this.ucEasyGridRuoli.ActiveRow.Cells["Codice"].Text;
                    CoreStatics.CoreApplication.MovPazienteInVisioneSelezionato.DataInizio = (DateTime)this.udteDataOraInizio.Value;
                    CoreStatics.CoreApplication.MovPazienteInVisioneSelezionato.DataFine = (DateTime)this.udteDataOraFine.Value;

                    bReturn = CoreStatics.CoreApplication.MovPazienteInVisioneSelezionato.Salva();

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Salva", this.Text);
            }

            return bReturn;

        }

        public bool ControllaValori()
        {

            bool bOK = true;

            if (bOK && this.ucEasyGridRuoli.ActiveRow == null)
            {
                easyStatics.EasyMessageBox("Selezionare un Ruolo.", "Paziente in Visione", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.ucEasyGridRuoli.Focus();
                bOK = false;
            }

            if (bOK && this.udteDataOraInizio.Value == null)
            {
                easyStatics.EasyMessageBox("E' necessario definire Data/Ora Inizio.", "Paziente in Visione", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.udteDataOraInizio.Focus();
                bOK = false;
            }
            if (bOK && this.udteDataOraFine.Value == null)
            {
                easyStatics.EasyMessageBox("E' necessario definire Data/Ora Fine.", "Paziente in Visione", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.udteDataOraFine.Focus();
                bOK = false;
            }
            if (bOK && (DateTime)this.udteDataOraFine.Value <= (DateTime)this.udteDataOraInizio.Value)
            {
                easyStatics.EasyMessageBox("Date non coerenti: Data/Ora Inizio maggiore o uguale di Data/Ora Fine.", "Paziente in Visione", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.udteDataOraFine.Focus();
                bOK = false;
            }

            return bOK;

        }

        private ucEasyListBox GetEasyListBoxForPopupControlContainer(object sender)
        {

            DateTime dt = (DateTime)((ucEasyDateTimeEditor)sender).Value;

            ucEasyListBox _ucEasyListBox = new ucEasyListBox();

            try
            {

                _ucEasyListBox.Size = new Size(150, 300);
                _ucEasyListBox.ItemSettings.SelectionType = Infragistics.Win.UltraWinListView.SelectionType.Single;
                _ucEasyListBox.TextFontRelativeDimension = ((ucEasyDateTimeEditor)sender).TextFontRelativeDimension;
                _ucEasyListBox.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
                _ucEasyListBox.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.Details;
                _ucEasyListBox.ViewSettingsDetails.AllowColumnMoving = false;
                _ucEasyListBox.ViewSettingsDetails.AllowColumnSizing = false;
                _ucEasyListBox.ViewSettingsDetails.AllowColumnSorting = false;
                _ucEasyListBox.ViewSettingsDetails.AutoFitColumns = Infragistics.Win.UltraWinListView.AutoFitColumns.ResizeAllColumns;
                _ucEasyListBox.ViewSettingsDetails.FullRowSelect = true;
                _ucEasyListBox.ViewSettingsDetails.ImageSize = new System.Drawing.Size(0, 0);
                _ucEasyListBox.ViewSettingsList.ImageSize = new System.Drawing.Size(0, 0);

                UltraListViewItem oVal = null;
                DateTime valoreitem = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);

                _ucEasyListBox.Items.Clear();

                for (int i = 0; i < 24; i++)
                {
                    oVal = new Infragistics.Win.UltraWinListView.UltraListViewItem(i.ToString());
                    oVal.Value = valoreitem.ToString("HH:mm");
                    _ucEasyListBox.Items.Add(oVal);
                    valoreitem = valoreitem.AddHours(1);
                }

            }
            catch (Exception)
            {

            }

            return _ucEasyListBox;

        }

        #endregion

        #region Events Form

        private void frmPUPazienteInVisione_ImmagineClick(object sender, ImmagineTopClickEventArgs e)
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

        private void frmPUPazienteInVisione_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
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

        private void frmPUPazienteInVisione_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void frmPUPazienteInVisione_Shown(object sender, EventArgs e)
        {

        }

        #endregion

        #region Events

        private void ucEasyGridRuoli_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            e.Layout.Bands[0].HeaderVisible = false;
            foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
            {

                switch (oCol.Key)
                {

                    case "Descrizione":
                        oCol.Hidden = false;
                        oCol.Header.Caption = @"Ruolo";
                        break;

                    default:
                        oCol.Hidden = true;
                        break;

                }

            }

        }

        private void udteDataOraInizio_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

            if (e.Button.Key == "Orari" && ((ucEasyDateTimeEditor)sender).Value != null)
            {
                CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainer);
                _ucEasyListBox = GetEasyListBoxForPopupControlContainer(sender);
                this.UltraPopupControlContainer.Show((ucEasyDateTimeEditor)sender);
            }

        }
        private void udteDataOraInizio_Validating(object sender, CancelEventArgs e)
        {

                        if (this.udteDataOraInizio.Value != null && this.udteDataOraFine.Value != null &&
                Convert.ToDateTime(this.udteDataOraInizio.Value.ToString()) > Convert.ToDateTime(this.udteDataOraFine.Value))
                this.udteDataOraFine.Value = this.udteDataOraInizio.Value;

        }

        #endregion

        #region UltraPopupControlContainer ucEasyListBox

        private void UltraPopupControlContainer_Closed(object sender, EventArgs e)
        {
            _ucEasyListBox.ItemSelectionChanged -= ucEasyListBox_ItemSelectionChanged;
        }

        private void UltraPopupControlContainer_Opened(object sender, EventArgs e)
        {
            _ucEasyListBox.ItemSelectionChanged += ucEasyListBox_ItemSelectionChanged;
        }

        private void UltraPopupControlContainer_Opening(object sender, CancelEventArgs e)
        {
            Infragistics.Win.Misc.UltraPopupControlContainer popup = sender as Infragistics.Win.Misc.UltraPopupControlContainer;
            popup.PopupControl = _ucEasyListBox;
        }

        private void ucEasyListBox_ItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {

            ucEasyDateTimeEditor source = this.UltraPopupControlContainer.SourceControl as ucEasyDateTimeEditor;
            ucEasyListBox popup = this.UltraPopupControlContainer.PopupControl as ucEasyListBox;

            DateTime dt = (DateTime)source.Value;
            if (popup.ActiveItem != null)
            {
                string[] orari = popup.ActiveItem.Text.Split(':');
                DateTime newdt = new DateTime(dt.Year, dt.Month, dt.Day, int.Parse(orari[0]), int.Parse(orari[1]), 0);
                source.Value = newdt;
            }


            this.UltraPopupControlContainer.Close();
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

    }
}
