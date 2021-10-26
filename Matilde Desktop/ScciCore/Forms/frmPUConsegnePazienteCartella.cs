using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnicodeSrl.Scci.Enums;

using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.DatiClinici.DC;
using UnicodeSrl.DatiClinici.Common;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci.PluginClient;
using Infragistics.Win.UltraWinGrid;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.Model;
using UnicodeSrl.ScciResource;

namespace UnicodeSrl.ScciCore
{
    public partial class frmPUConsegnePazienteCartella : frmBaseModale, Interfacce.IViewFormlModal
    {
        public frmPUConsegnePazienteCartella()
        {
            InitializeComponent();
        }

        #region Interface

        public new void Carica()
        {

            try
            {

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_CONSEGNEPAZIENTE_16);

                this.InizializzaControlli();
                this.Aggiorna();
                this.SetAction();

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

                this.ucDcViewer.VisualizzaTitoloScheda = false;

                CoreStatics.SetEasyUltraGridLayout(ref this.ugRuoli);
                this.ugRuoli.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Medium;

                this.ubNuovo.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_RUOLIAGGIUNGI_256);
                this.ubNuovo.PercImageFill = 0.75F;
                this.ubNuovo.Appearance.ImageHAlign = Infragistics.Win.HAlign.Left;
                this.ubNuovo.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.ubAnnulla.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_RUOLICANCELLA_256);
                this.ubAnnulla.PercImageFill = 0.75F;
                this.ubAnnulla.Appearance.ImageHAlign = Infragistics.Win.HAlign.Left;
                this.ubAnnulla.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

            }
            catch (Exception)
            {
            }

        }

        public void Aggiorna()
        {

            try
            {

                this.CaricaScheda();
                this.LoadUltraGrid();

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "Aggiorna", this.Text);
            }

        }

        private void CaricaScheda()
        {

            try
            {

                                                                Gestore oGestore = CoreStatics.GetGestore();

                                oGestore.SchedaXML = CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.MovScheda.Scheda.StrutturaXML;

                                oGestore.SchedaLayoutsXML = CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.MovScheda.Scheda.LayoutXML;

                                oGestore.Decodifiche = CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.MovScheda.Scheda.DizionarioValori();

                                if (CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.MovScheda.DatiXML == string.Empty)
                {
                    oGestore.SchedaDati = new DcSchedaDati();
                }
                else
                {
                    try
                    {
                        oGestore.SchedaDatiXML = CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.MovScheda.DatiXML;
                    }
                    catch (Exception)
                    {
                        oGestore.SchedaDati = new DcSchedaDati();
                    }
                }
                if (oGestore.SchedaDati.Dati.Count == 0) { oGestore.NuovaScheda(); }

                this.ucDcViewer.VisualizzaTitoloScheda = false;

                this.ucDcViewer.CaricaDati(oGestore);

                this.ucDcViewer.RtfEvent -= ucDcViewer_RtfEvent;
                this.ucDcViewer.KeyEvent -= ucDcViewer_KeyEvent;
                this.ucDcViewer.ButtonEvent -= ucDcViewer_ButtonEvent;
                this.ucDcViewer.RtfEvent += ucDcViewer_RtfEvent;
                this.ucDcViewer.KeyEvent += ucDcViewer_KeyEvent;
                this.ucDcViewer.ButtonEvent += ucDcViewer_ButtonEvent;

            }
            catch (Exception ex)
            {
                throw new Exception(@"CaricaScheda()" + Environment.NewLine + ex.Message, ex);
            }

        }

        private void LoadUltraGrid()
        {

            try
            {

                this.ugRuoli.DataSource = null;
                this.ugRuoli.Refresh();
                this.ugRuoli.DataSource = CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.Elementi;
                this.ugRuoli.Refresh();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "LoadUltraGrid", this.Name);
            }

        }

        private void SetAction()
        {

            bool bNuovo = true;
            bool bModifica = false;

            try
            {

                if (this.ugRuoli.ActiveRow != null)
                {

                    switch (CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.Azione)
                    {

                        case EnumAzioni.INS:
                            bModifica = true;
                            break;

                        case EnumAzioni.MOD:
                            MovConsegnaPazienteRuoli row = (MovConsegnaPazienteRuoli)this.ugRuoli.ActiveRow.ListObject;
                            if (row.CodStatoConsegnaPazienteRuolo != @"VS" || row.CodStatoConsegnaPazienteRuolo != @"CA")
                            {
                                bModifica = true;
                            }
                            break;

                        default:
                            break;

                    }

                }

            }
            catch (Exception)
            {

            }

            this.ubNuovo.Enabled = bNuovo;
            this.ubAnnulla.Enabled = bModifica;

        }

        public bool Salva()
        {

            bool bReturn = false;

            try
            {

                if (ControllaValori())
                {
                    if (SalvaScheda())
                    {
                        bReturn = CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.Salva(false);
                    }
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Salva", this.Text);
            }

            return bReturn;

        }

        private bool SalvaScheda()
        {

            bool bReturn = true;

            try
            {

                CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.MovScheda.DatiXML = this.ucDcViewer.Gestore.SchedaDatiXML;

                if (CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.MovScheda.DatiObbligatoriMancantiRTF != string.Empty
                && CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.MovScheda.DatiObbligatoriMancantiRTF.Trim() != string.Empty)
                {
                    if (easyStatics.EasyMessageBox(@"Non sono stati compilati alcuni valori obbligatori della scheda!" + Environment.NewLine + @"Vuoi continuare col salvataggio?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                        bReturn = false;
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Salva", this.Text);

                if (easyStatics.EasyMessageBox(@"Si è verificato un errore nel salvataggio della scheda!" + Environment.NewLine + @"Vuoi uscire ugualmente?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                    bReturn = false;
            }

            return bReturn;

        }

        private bool ControllaValori()
        {

            bool bOK = true;

                        if (bOK && this.ugRuoli.Rows.Count == 0)
            {
                easyStatics.EasyMessageBox("Inserire almeno un ruolo!", "Consegna Paziente", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.ugRuoli.Focus();
                bOK = false;
            }

            return bOK;

        }

        #endregion

        #region Events Form

        private void frmPUConsegnePazienteCartella_Shown(object sender, EventArgs e)
        {
            this.CaricaScheda();
        }

        private void frmPUConsegnePazienteCartella_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
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

        private void frmPUConsegnePazienteCartella_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        #endregion

        #region Events

        private void ugRuoli_AfterRowActivate(object sender, EventArgs e)
        {
            this.SetAction();
        }

        private void ugRuoli_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {

                int refWidth = (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small) * 4;

                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {

                    switch (oCol.Key)
                    {

                        case "StatoIcona":
                            oCol.Hidden = false;
                            oCol.Header.Caption = "";
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
                            oCol.RowLayoutColumnInfo.SpanY = 1;
                            break;

                        case "StatoConsegna":
                            oCol.Hidden = false;
                            oCol.Header.Caption = "Stato";
                            oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                            oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                            break;

                        case "Ruolo":
                            oCol.Hidden = false;
                            oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                            oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                            break;

                        default:
                            oCol.Hidden = true;
                            break;

                    }

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ugRuoli_InitializeLayout", this.Name);
            }

        }

        private void ubNuovo_Click(object sender, EventArgs e)
        {

            using (FwDataConnection conn = new FwDataConnection(Database.ConnectionString))
            {

                                FwDataBufferedList<T_RuoliRow> oRuoli = conn.T_RuoliConsegneRow(CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.CodTipoConsegnaPaziente);

                                object lstRuoli = easyStatics.EasyFormZoom("Elenco Ruoli", Risorse.GC_CONSEGNEPAZIENTE_256, oRuoli, true);

                if (lstRuoli != null)
                {

                    foreach (T_RuoliRow oRuolo in (List<object>)lstRuoli)
                    {

                                                MovConsegnaPazienteRuoli oMovConsegnaPazienteRuoli = CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.Elementi.SingleOrDefault<MovConsegnaPazienteRuoli>(M => M.CodRuolo == oRuolo.Codice);
                        if (oMovConsegnaPazienteRuoli == null)
                        {
                            oMovConsegnaPazienteRuoli = new MovConsegnaPazienteRuoli(CoreStatics.CoreApplication.Ambiente);
                            oMovConsegnaPazienteRuoli.CodRuolo = oRuolo.Codice;
                            oMovConsegnaPazienteRuoli.IDMovConsegnaPaziente = CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.IDMovConsegnaPaziente;
                            oMovConsegnaPazienteRuoli.Azione = EnumAzioni.INS;
                            oMovConsegnaPazienteRuoli.CodStatoConsegnaPazienteRuolo = "IC";
                            CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.Elementi.Add(oMovConsegnaPazienteRuoli);
                        }
                        else
                        {
                            oMovConsegnaPazienteRuoli.CodStatoConsegnaPazienteRuolo = "IC";
                        }

                    }

                    this.LoadUltraGrid();
                    this.SetAction();

                }

            }

        }

        private void ubAnnulla_Click(object sender, EventArgs e)
        {

            if (this.ugRuoli.ActiveRow != null)
            {


                MovConsegnaPazienteRuoli row = (MovConsegnaPazienteRuoli)this.ugRuoli.ActiveRow.ListObject;

                switch (CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.Azione)
                {

                    case EnumAzioni.INS:
                    case EnumAzioni.MOD:
                        if (easyStatics.EasyMessageBox("Vuoi eliminare il ruolo selezionato?", "Eliminare Ruolo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            if (row.IDMovConsegnaPazienteRuoli != "")
                            {
                                row.CodStatoConsegnaPazienteRuolo = "AN";
                            }
                            else
                            {
                                CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.Elementi.Remove(row);
                            }
                            this.LoadUltraGrid();
                        }
                        break;


                    default:
                        break;

                }

                this.SetAction();

            }

        }

        #endregion

        #region Events UserControl ucDcViewer

        void ucDcViewer_KeyEvent(System.Windows.Input.Key key)
        {

            try
            {

                if (key == System.Windows.Input.Key.F1)
                {
                    frmPUConsegnePazienteCartella_PulsanteIndietroClick(null, new PulsanteBottomClickEventArgs(EnumPulsanteBottom.Indietro));
                }
                else if (key == System.Windows.Input.Key.F12)
                {
                    frmPUConsegnePazienteCartella_PulsanteAvantiClick(null, new PulsanteBottomClickEventArgs(EnumPulsanteBottom.Avanti));
                }

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ucDcViewer_KeyEvent", this.Text);
            }

        }

        void ucDcViewer_RtfEvent(string id)
        {

            try
            {

                string codua = "";

                                if (CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata != null)
                {
                    codua = CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.CodUA;
                }

                                CoreStatics.CoreApplication.MovTestoPredefinitoSelezionato = new MovTestoPredefinito(UnicodeSrl.Scci.Enums.EnumEntita.CSP.ToString(), codua, CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice,
                                                                                                    CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.CodTipoConsegnaPaziente, id);
                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(Scci.Enums.EnumMaschere.TestiPredefiniti) == System.Windows.Forms.DialogResult.OK)
                {
                    string sRTFOriginale = this.ucDcViewer.Gestore.LeggeValore(id).ToString();
                    string sRTFDaAccodare = CoreStatics.CoreApplication.MovTestoPredefinitoSelezionato.RitornoRTF;
                    UnicodeSrl.Scci.RTF.RtfFiles rtf = new UnicodeSrl.Scci.RTF.RtfFiles();
                    sRTFOriginale = rtf.joinRtf(sRTFDaAccodare, sRTFOriginale, true);
                    rtf = null;
                    this.ucDcViewer.Gestore.ModificaValore(id, sRTFOriginale);
                }

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ucDcViewer_RtfEvent", this.Text);
            }

        }

        void ucDcViewer_ButtonEvent(string id)
        {

            string _codua = string.Empty;

            try
            {

                this.ImpostaCursore(enum_app_cursors.WaitCursor);
                this.Tag = id;
                if (CoreStatics.CoreApplication.Trasferimento != null)
                {
                    _codua = CoreStatics.CoreApplication.Trasferimento.CodUA;
                }
                else
                {
                    _codua = CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.CodUAAmbulatorialeSelezionata;
                }
                string[] azioni = id.Split('.');
                string[] campo = azioni[2].Split('_');
                string azione = string.Format("CSP{0}.{1}", CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.CodTipoConsegnaPaziente, campo[0]);
                                object[] myparam = new object[5] { this, campo[0], int.Parse(campo[1]), this.ucDcViewer.Gestore, azioni[0] };

                Risposta oRisposta = PluginClientStatics.PluginClient(azione, myparam, CommonStatics.UAPadri(_codua, CoreStatics.CoreApplication.Ambiente));
                if (oRisposta.Successo == true)
                {
                }
                else
                {
                    if (oRisposta.ex != null)
                    {
                        Exception rex = oRisposta.ex;
                        CoreStatics.ExGest(ref rex, @"Si è verificato un errore nell'elaborazione della procedura.", "ucDcViewer_ButtonEvent", this.Text, true, this.Name, "Errore", MessageBoxIcon.Warning, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);
                    }
                    else
                    {
                        easyStatics.EasyMessageBox(oRisposta.Parameters[0].ToString(), azione, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                this.Tag = null;


            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ucDcViewer_ButtonEvent", this.Text);
            }
            finally
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }

        }

        #endregion

    }
}
