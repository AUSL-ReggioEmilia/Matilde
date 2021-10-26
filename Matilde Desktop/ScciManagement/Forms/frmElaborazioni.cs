using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.ScciCore;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.Statics;
using Infragistics.Win.UltraWinGrid;
using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.DatiClinici.DC;
using UnicodeSrl.Scci.DataContracts;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmElaborazioni : Form, Interfacce.IViewFormBase
    {

        private enum enumTabElaborazioni
        {
            NonDefinito = 0,
            AperturaCartelle = 1,
            CreazioneSchede = 2,
            CreazioneAppuntamenti = 3,
            PazientiDatiSAC = 4,
            CollegaCartelle = 5
        }

        private enum enumStatoElaborazioni
        {
            DE = 0,
            ER = 1,
            EL = 2
        }

        private enum enumCodEntita
        {
            EPI = 1,
            PAZ = 2,
            CAR = 3
        }

        public frmElaborazioni()
        {
            InitializeComponent();
        }

        #region Interface

        public Icon ViewIcon
        {
            get
            {
                return this.Icon;
            }
            set
            {
                this.Icon = value;
            }
        }

        public Image ViewImage
        {
            get
            {
                return this.PicImage.Image;
            }
            set
            {
                this.PicImage.Image = value;
            }
        }

        public string ViewText
        {
            get
            {
                return this.Text;
            }
            set
            {
                this.Text = value;
            }
        }

        public void ViewInit()
        {

            this.SuspendLayout();

            MyStatics.SetUltraToolbarsManager(this.UltraToolbarsManager);
            this.InitializeUltraGrid();
            this.InitializeUltraProgressBar();

            this.UltraTabControl.PerformAction(Infragistics.Win.UltraWinTabControl.UltraTabControlAction.ActivateFirstTab);

            this.ResumeLayout();

        }

        #endregion

        #region Properties

        private enumTabElaborazioni ActiveUltraTab
        {
            get
            {
                enumTabElaborazioni tabsel = enumTabElaborazioni.NonDefinito;
                try
                {
                    tabsel = (enumTabElaborazioni)Enum.Parse(typeof(enumTabElaborazioni), this.UltraTabControl.ActiveTab.Key);
                }
                catch
                {
                    tabsel = enumTabElaborazioni.NonDefinito;
                }

                return tabsel;
            }
        }

        #endregion

        #region UltraGrid

        private void InitializeUltraGrid()
        {

            MyStatics.SetUltraGridLayout(ref this.UltraGrid, true, false);

            this.UltraGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;

        }

        private void LoadUltraGrid()
        {
            this.ucEasyTableGriglia.RowStyles[0].Height = 0;


            switch (this.ActiveUltraTab)
            {

                case enumTabElaborazioni.NonDefinito:
                    break;

                case enumTabElaborazioni.AperturaCartelle:
                    this.LoadUltraGridCartelle();
                    break;

                case enumTabElaborazioni.CreazioneSchede:
                    this.LoadUltraGridSchede();
                    break;

                case enumTabElaborazioni.CreazioneAppuntamenti:
                    this.LoadUltraGridAppuntamenti();
                    break;

                case enumTabElaborazioni.PazientiDatiSAC:
                    this.LoadUltraGridPazienti();
                    break;

                case enumTabElaborazioni.CollegaCartelle:
                    this.LoadUltraGridCollegaCartelle();
                    break;
            }

            this.RefreshGrid();

        }

        private void LoadUltraGridCartelle()
        {


            string sSql = string.Empty;

            this.Cursor = Cursors.WaitCursor;

            try
            {

                sSql = @"Select * From T_BO_Cartelle";

                this.UltraGrid.DataSource = null;
                this.UltraGrid.Refresh();
                this.UltraGrid.DataSource = DataBase.GetDataSet(sSql);
                this.UltraGrid.Refresh();
                this.UltraGrid.DisplayLayout.PerformAutoResizeColumns(false, Infragistics.Win.UltraWinGrid.PerformAutoSizeType.AllRowsInBand);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                this.UltraGrid.DataSource = null;
                this.UltraGrid.Refresh();
            }

            this.Cursor = Cursors.Default;

        }

        private void LoadUltraGridSchede()
        {


            string sSql = string.Empty;

            this.Cursor = Cursors.WaitCursor;

            try
            {

                sSql = @"Select * From T_BO_Schede";

                this.UltraGrid.DataSource = null;
                this.UltraGrid.Refresh();
                this.UltraGrid.DataSource = DataBase.GetDataSet(sSql);
                this.UltraGrid.Refresh();
                this.UltraGrid.DisplayLayout.PerformAutoResizeColumns(false, Infragistics.Win.UltraWinGrid.PerformAutoSizeType.AllRowsInBand);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                this.UltraGrid.DataSource = null;
                this.UltraGrid.Refresh();
            }

            this.Cursor = Cursors.Default;

        }

        private void LoadUltraGridAppuntamenti()
        {


            string sSql = string.Empty;

            this.Cursor = Cursors.WaitCursor;

            try
            {

                sSql = @"Select * From T_BO_Appuntamenti";

                this.UltraGrid.DataSource = null;
                this.UltraGrid.Refresh();
                this.UltraGrid.DataSource = DataBase.GetDataSet(sSql);
                this.UltraGrid.Refresh();
                this.UltraGrid.DisplayLayout.PerformAutoResizeColumns(false, Infragistics.Win.UltraWinGrid.PerformAutoSizeType.AllRowsInBand);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                this.UltraGrid.DataSource = null;
                this.UltraGrid.Refresh();
            }

            this.Cursor = Cursors.Default;

        }

        private void LoadUltraGridPazienti()
        {


            string sSql = string.Empty;

            this.Cursor = Cursors.WaitCursor;

            try
            {

                sSql = @"Select * From T_BO_Pazienti";

                this.UltraGrid.DataSource = null;
                this.UltraGrid.Refresh();
                this.UltraGrid.DataSource = DataBase.GetDataSet(sSql);
                this.UltraGrid.Refresh();
                this.UltraGrid.DisplayLayout.PerformAutoResizeColumns(false, Infragistics.Win.UltraWinGrid.PerformAutoSizeType.AllRowsInBand);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                this.UltraGrid.DataSource = null;
                this.UltraGrid.Refresh();
            }

            this.Cursor = Cursors.Default;

        }

        private void LoadUltraGridCollegaCartelle()
        {
            this.ucEasyTableGriglia.RowStyles[0].Height = 30;


            string sSql = string.Empty;

            this.Cursor = Cursors.WaitCursor;

            try
            {

                sSql = @"Select * From T_BO_CartelleCollega";

                this.UltraGrid.DataSource = null;
                this.UltraGrid.Refresh();
                this.UltraGrid.DataSource = DataBase.GetDataSet(sSql);
                this.UltraGrid.Refresh();
                this.UltraGrid.DisplayLayout.PerformAutoResizeColumns(false, Infragistics.Win.UltraWinGrid.PerformAutoSizeType.AllRowsInBand);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                this.UltraGrid.DataSource = null;
                this.UltraGrid.Refresh();
            }

            this.Cursor = Cursors.Default;

        }

        private void RefreshGrid()
        {

            switch (this.ActiveUltraTab)
            {

                case enumTabElaborazioni.NonDefinito:
                    this.UltraGrid.Text = string.Empty;
                    break;

                case enumTabElaborazioni.AperturaCartelle:
                    this.UltraGrid.Text = string.Format("{0} ({1:#,##0})", "Cartelle", this.UltraGrid.Rows.FilteredInRowCount);
                    break;

                case enumTabElaborazioni.CreazioneSchede:
                    this.UltraGrid.Text = string.Format("{0} ({1:#,##0})", "Schede", this.UltraGrid.Rows.FilteredInRowCount);
                    break;

                case enumTabElaborazioni.CreazioneAppuntamenti:
                    this.UltraGrid.Text = string.Format("{0} ({1:#,##0})", "Appuntamenti", this.UltraGrid.Rows.FilteredInRowCount);
                    break;

                case enumTabElaborazioni.PazientiDatiSAC:
                    this.UltraGrid.Text = string.Format("{0} ({1:#,##0})", "Pazienti", this.UltraGrid.Rows.FilteredInRowCount);
                    break;

            }

        }

        #endregion

        #region UltraProgressBar

        private void InitializeUltraProgressBar()
        {
            this.UltraProgressBar.Style = Infragistics.Win.UltraWinProgressBar.ProgressBarStyle.Office2007Continuous;
            this.UltraProgressBar.Visible = false;
        }
        private void InitializeUltraProgressBar(int MinValue, int MaxValue)
        {
            this.UltraProgressBar.Style = Infragistics.Win.UltraWinProgressBar.ProgressBarStyle.Office2007Continuous;
            this.UltraProgressBar.Minimum = MinValue;
            this.UltraProgressBar.Maximum = MaxValue;
            this.UltraProgressBar.Value = MinValue;
            this.UltraProgressBar.Visible = true;
        }

        private void UpdateProgressBar()
        {
            if (this.UltraProgressBar.Visible)
            {
                try
                {
                    int pbarnextval = this.UltraProgressBar.Value + 1;

                    if (pbarnextval <= this.UltraProgressBar.Maximum)
                        this.UltraProgressBar.Value = pbarnextval;

                }
                catch
                {
                }

                Application.DoEvents();
            }
        }

        #endregion

        #region Elaborazioni

        private void Elabora()
        {

            switch (this.ActiveUltraTab)
            {

                case enumTabElaborazioni.NonDefinito:
                    break;

                case enumTabElaborazioni.AperturaCartelle:
                    this.ElaboraCartelle();
                    break;

                case enumTabElaborazioni.CreazioneSchede:
                    this.ElaboraSchede();
                    break;

                case enumTabElaborazioni.CreazioneAppuntamenti:
                    this.ElaboraAppuntamenti();
                    break;

                case enumTabElaborazioni.PazientiDatiSAC:
                    this.ElaboraPazienti();
                    break;

                case enumTabElaborazioni.CollegaCartelle:
                    this.ElaboraCollegaCartelle();
                    break;

            }

        }

        private void ElaboraCartelle()
        {

            string IDEpisodio = string.Empty;
            string IDTrasferimento = string.Empty;

            string utenteelab = this.UtenteElaborazione();

            Episodio oEpisodio = null;
            Paziente oPaziente = null;
            Trasferimento oTrasferimento = null;

            try
            {
                this.InitializeUltraProgressBar(0, this.UltraGrid.Rows.FilteredInRowCount);
                foreach (UltraGridRow oRow in this.UltraGrid.Rows.GetFilteredInNonGroupByRows())
                {

                    this.UpdateProgressBar();

                    if (oRow.Cells["CodStatoElaborazione"].Value.ToString() != enumStatoElaborazioni.EL.ToString())
                    {

                        IDEpisodio = RicercaEpisodioByNumeroNosologico(oRow.Cells["NumeroNosologico"].Value.ToString());
                        if (IDEpisodio == string.Empty)
                        {

                            IDEpisodio = RicercaEpisodioByNumeroListaAttesa(oRow.Cells["NumeroListaAttesa"].Value.ToString());

                        }
                        if (IDEpisodio != string.Empty)
                        {

                            oEpisodio = new Episodio(IDEpisodio);
                            if (oEpisodio.Attivo == true)
                            {

                                oPaziente = new Paziente("", oEpisodio.ID);
                                if (oPaziente.Attivo == true)
                                {

                                    IDTrasferimento = RicercaTrasferimento(oEpisodio.ID, oRow.Cells["CodUA"].Value.ToString(), "AT");
                                    if (IDTrasferimento == string.Empty)
                                    {

                                        IDTrasferimento = RicercaTrasferimento(oEpisodio.ID, oRow.Cells["CodUA"].Value.ToString(), "PR");

                                    }
                                    if (IDTrasferimento != string.Empty)
                                    {

                                        oTrasferimento = new Trasferimento(IDTrasferimento, CoreStatics.CoreApplication.Ambiente);
                                        if (oTrasferimento.Attivo == true)
                                        {

                                            if (oTrasferimento.IDCartella == string.Empty)
                                            {

                                                if (!ControlloNumeroCartella(oRow.Cells["CodUA"].Value.ToString(), oPaziente.ID, oEpisodio.ID, oTrasferimento.ID, oRow.Cells["NumCartella"].Value.ToString()))
                                                {
                                                    if (this.AperturaCartella(oTrasferimento.ID, oRow.Cells["NumCartella"].Value.ToString(), utenteelab))
                                                    {
                                                        string scampi = string.Empty;
                                                        scampi += ", DataElaborazione = " + DataBase.SQLDateTimeInsert(DateTime.Now);
                                                        scampi += ", IDPaziente = '" + oPaziente.ID + "'";
                                                        scampi += ", IDEpisodio = '" + oEpisodio.ID.ToString() + "'";
                                                        scampi += ", IDTrasferimento = '" + oTrasferimento.ID.ToString() + "'";
                                                        scampi += ", IDCartella = '" + DataBase.FindValue("IDCartella", "T_MovTrasferimenti", "ID = '" + oTrasferimento.ID.ToString() + "'", "") + "'";

                                                        this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.EL.ToString(), "Episodio '" + IDEpisodio + "' ELABORATO CON SUCCESSO!", scampi);
                                                    }
                                                    else
                                                        this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Episodio '" + IDEpisodio + "' NON ELABORATO Errore di apertura cartella!");


                                                }
                                                else
                                                {
                                                    this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Numero Cartella '" + oRow.Cells["NumCartella"].Value.ToString() + "' già utilizzato !");
                                                }
                                            }
                                            else
                                            {
                                                this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Trasferimento dell episodio '" + IDEpisodio + "' già associato alla cartella " + oTrasferimento.NumeroCartella + " !");
                                            }

                                        }
                                        else
                                        {
                                            this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Trasferimento dell episodio '" + IDEpisodio + "' NON trovato!");
                                        }

                                    }
                                    else
                                    {
                                        this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Trasferimento dell episodio '" + IDEpisodio + "' NON trovato!");
                                    }

                                }
                                else
                                {
                                    this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Paziente dell episodio '" + IDEpisodio + "' NON trovato!");
                                }

                            }
                            else
                            {
                                this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Episodio '" + IDEpisodio + "' NON trovato!");
                            }

                        }
                        else
                        {
                            this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Episodio NON trovato!");
                        }

                    }

                }

                this.InitializeUltraProgressBar();

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void ElaboraSchede()
        {

            string IDPaziente = string.Empty;
            string IDEpisodio = string.Empty;
            string IDTrasferimento = string.Empty;

            Episodio oEpisodio = null;
            Paziente oPaziente = null;
            Trasferimento oTrasferimento = null;
            MovScheda oMovScheda = null;

            string utenteelab = this.UtenteElaborazione();
            string ruoloelab = this.RuoloElaborazione();

            try
            {
                this.InitializeUltraProgressBar(0, this.UltraGrid.Rows.FilteredInRowCount);
                foreach (UltraGridRow oRow in this.UltraGrid.Rows.GetFilteredInNonGroupByRows())
                {
                    this.UpdateProgressBar();
                    if (oRow.Cells["CodStatoElaborazione"].Value.ToString() != enumStatoElaborazioni.EL.ToString())
                    {

                        if (oRow.Cells["CodEntita"].Value.ToString() == EnumEntita.EPI.ToString())
                        {

                            IDEpisodio = RicercaEpisodioByNumeroNosologico(oRow.Cells["NumeroNosologico"].Value.ToString());
                            if (IDEpisodio == string.Empty)
                            {

                                IDEpisodio = RicercaEpisodioByNumeroListaAttesa(oRow.Cells["NumeroListaAttesa"].Value.ToString());

                            }
                            if (IDEpisodio != string.Empty)
                            {

                                oEpisodio = new Episodio(IDEpisodio);
                                if (oEpisodio.Attivo == true)
                                {

                                    oPaziente = new Paziente("", oEpisodio.ID);
                                    if (oPaziente.Attivo == true)
                                    {

                                        IDTrasferimento = RicercaTrasferimento(oEpisodio.ID, oRow.Cells["CodUA"].Value.ToString(), "AT");
                                        if (IDTrasferimento == string.Empty)
                                        {

                                            IDTrasferimento = RicercaTrasferimento(oEpisodio.ID, oRow.Cells["CodUA"].Value.ToString(), "PR");

                                        }
                                        if (IDTrasferimento != string.Empty)
                                        {

                                            oTrasferimento = new Trasferimento(IDTrasferimento, CoreStatics.CoreApplication.Ambiente);
                                            if (oTrasferimento.Attivo == true)
                                            {

                                                oMovScheda = RicercaScheda(EnumEntita.EPI.ToString(), oEpisodio.ID, oRow.Cells["CodScheda"].Value.ToString(), (int)oRow.Cells["Versione"].Value, (int)oRow.Cells["Numero"].Value);
                                                if (oMovScheda != null && bool.Parse(oRow.Cells["Aggiorna"].Value.ToString()) == false)
                                                {
                                                    this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Scheda dell episodio '" + IDEpisodio + "' ESISTENTE!");
                                                }
                                                else if (oMovScheda != null && bool.Parse(oRow.Cells["Aggiorna"].Value.ToString()) == true)
                                                {

                                                    if (oRow.Cells["Dati"].Value.ToString() != string.Empty)
                                                    {
                                                        oMovScheda.DatiXML = oRow.Cells["Dati"].Value.ToString();
                                                        oMovScheda.Ambiente.Codlogin = utenteelab;
                                                        oMovScheda.Ambiente.Codruolo = ruoloelab;
                                                        oMovScheda.Salva();

                                                        string scampi = string.Empty;
                                                        scampi += ", DataElaborazione = " + DataBase.SQLDateTimeInsert(DateTime.Now);
                                                        scampi += ", IDPaziente = '" + oPaziente.ID + "'";
                                                        scampi += ", IDEpisodio = '" + oEpisodio.ID + "'";
                                                        scampi += ", IDTrasferimento = '" + oTrasferimento.ID + "'";
                                                        scampi += ", IDScheda = '" + oMovScheda.IDMovScheda + "'";
                                                        if (oMovScheda.IDSchedaPadre != string.Empty)
                                                        {
                                                            scampi += ", IDSchedaPadre = '" + oMovScheda.IDSchedaPadre + "'";
                                                        }
                                                        this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.EL.ToString(), "Episodio '" + IDEpisodio + "' ELABORATO!", scampi);
                                                    }
                                                    else
                                                    {
                                                        this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Dati della scheda '" + oMovScheda.IDMovScheda + "' dell episodio '" + IDEpisodio + "' NON corretti!");
                                                    }

                                                }
                                                else
                                                {
                                                    oMovScheda = new MovScheda(oRow.Cells["CodScheda"].Value.ToString(),
                            EnumEntita.EPI,
                            oRow.Cells["CodUA"].Value.ToString(),
                            oPaziente.ID, oEpisodio.ID, oTrasferimento.ID,
                            CoreStatics.CoreApplication.Ambiente);

                                                    if (oMovScheda.Scheda.Versione == (int)oRow.Cells["Versione"].Value)
                                                    {

                                                        if (int.Parse(DataBase.FindValue("QtaSchedeDisponibili", "Q_SelMovSchedeNumerosita", "CodEntita = '" + EnumEntita.EPI.ToString() + "' And IDEntita = '" + oEpisodio.ID + "' And CodScheda = '" + oRow.Cells["CodScheda"].Value.ToString() + "'", "1")) > 0 &&
                                                            (int.Parse(DataBase.FindValue("MassimoNumero", "Q_SelMovSchedeNumerosita", "CodEntita = '" + EnumEntita.EPI.ToString() + "' And IDEntita = '" + oEpisodio.ID + "' And CodScheda = '" + oRow.Cells["CodScheda"].Value.ToString() + "'", "0")) + 1) == (int)oRow.Cells["Numero"].Value)
                                                        {

                                                            if (oRow.Cells["Dati"].Value.ToString() != string.Empty)
                                                            {

                                                                if (oRow.Cells["CodSchedaPadre"].Value.ToString() != string.Empty && oRow.Cells["VersioneSchedaPadre"].Value.ToString() != string.Empty && oRow.Cells["NumeroSchedaPadre"].Value.ToString() != string.Empty)
                                                                {
                                                                    MovScheda oMovSchedaPadre = RicercaScheda(EnumEntita.EPI.ToString(), oEpisodio.ID, oRow.Cells["CodSchedaPadre"].Value.ToString(), int.Parse(oRow.Cells["VersioneSchedaPadre"].Value.ToString()), int.Parse(oRow.Cells["NumeroSchedaPadre"].Value.ToString()));
                                                                    if (oMovSchedaPadre != null)
                                                                    {
                                                                        oMovScheda.IDSchedaPadre = oMovSchedaPadre.IDMovScheda;
                                                                        oMovScheda.IDEntita = oEpisodio.ID;
                                                                        oMovScheda.DatiXML = oRow.Cells["Dati"].Value.ToString();
                                                                        oMovScheda.Ambiente.Codlogin = utenteelab;
                                                                        oMovScheda.Ambiente.Codruolo = ruoloelab;
                                                                        oMovScheda.Salva();

                                                                        string scampi = string.Empty;
                                                                        scampi += ", DataElaborazione = " + DataBase.SQLDateTimeInsert(DateTime.Now);
                                                                        scampi += ", IDPaziente = '" + oPaziente.ID + "'";
                                                                        scampi += ", IDEpisodio = '" + oEpisodio.ID + "'";
                                                                        scampi += ", IDTrasferimento = '" + oTrasferimento.ID + "'";
                                                                        scampi += ", IDScheda = '" + oMovScheda.IDMovScheda + "'";
                                                                        scampi += ", IDSchedaPadre = '" + oMovScheda.IDSchedaPadre + "'";
                                                                        this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.EL.ToString(), "Episodio '" + IDEpisodio + "' ELABORATO!", scampi);

                                                                    }
                                                                    else
                                                                    {
                                                                        this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Scheda padre (" + oRow.Cells["CodSchedaPadre"].Value.ToString() + ") dell episodio '" + IDEpisodio + "' INESISTENTE!");
                                                                    }

                                                                }
                                                                else
                                                                {
                                                                    oMovScheda.IDEntita = oEpisodio.ID;
                                                                    oMovScheda.DatiXML = oRow.Cells["Dati"].Value.ToString();
                                                                    oMovScheda.Ambiente.Codlogin = utenteelab;
                                                                    oMovScheda.Ambiente.Codruolo = ruoloelab;
                                                                    oMovScheda.Salva();

                                                                    string scampi = string.Empty;
                                                                    scampi += ", DataElaborazione = " + DataBase.SQLDateTimeInsert(DateTime.Now);
                                                                    scampi += ", IDPaziente = '" + oPaziente.ID + "'";
                                                                    scampi += ", IDEpisodio = '" + oEpisodio.ID + "'";
                                                                    scampi += ", IDTrasferimento = '" + oTrasferimento.ID + "'";
                                                                    scampi += ", IDScheda = '" + oMovScheda.IDMovScheda + "'";
                                                                    if (oMovScheda.IDSchedaPadre != string.Empty)
                                                                    {
                                                                        scampi += ", IDSchedaPadre = '" + oMovScheda.IDSchedaPadre + "'";
                                                                    }
                                                                    this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.EL.ToString(), "Episodio '" + IDEpisodio + "' ELABORATO!", scampi);
                                                                }

                                                            }
                                                            else
                                                            {
                                                                this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Dati della scheda '" + oMovScheda.IDMovScheda + "' dell episodio '" + IDEpisodio + "' NON corretti!");
                                                            }

                                                        }
                                                        else
                                                        {
                                                            this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Numero (disponibile e/o massimo) della scheda '" + oMovScheda.IDMovScheda + "' dell episodio '" + IDEpisodio + "' NON corretto!");
                                                        }

                                                    }
                                                    else
                                                    {
                                                        this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Versione della scheda '" + oMovScheda.IDMovScheda + "' dell episodio '" + IDEpisodio + "' NON corretta!");
                                                    }
                                                }

                                            }
                                            else
                                            {
                                                this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Trasferimento dell episodio '" + IDEpisodio + "' NON trovato!");
                                            }

                                        }
                                        else
                                        {
                                            this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Trasferimento dell episodio '" + IDEpisodio + "' NON trovato!");
                                        }

                                    }
                                    else
                                    {
                                        this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Paziente dell episodio '" + IDEpisodio + "' NON trovato!");
                                    }

                                }
                                else
                                {
                                    this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Episodio '" + IDEpisodio + "' NON trovato!");
                                }

                            }
                            else
                            {
                                this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Episodio NON trovato!");
                            }

                        }
                        else if (oRow.Cells["CodEntita"].Value.ToString() == EnumEntita.PAZ.ToString())
                        {

                            IDPaziente = oRow.Cells["IDPazienteRiferimento"].Value.ToString();
                            oMovScheda = RicercaScheda(EnumEntita.PAZ.ToString(), IDPaziente, oRow.Cells["CodScheda"].Value.ToString(), (int)oRow.Cells["Versione"].Value, (int)oRow.Cells["Numero"].Value);
                            if (oMovScheda != null && bool.Parse(oRow.Cells["Aggiorna"].Value.ToString()) == false)
                            {
                                this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Scheda del paziente '" + IDPaziente + "' ESISTENTE!");
                            }
                            else if (oMovScheda != null && bool.Parse(oRow.Cells["Aggiorna"].Value.ToString()) == true)
                            {

                                if (oRow.Cells["Dati"].Value.ToString() != string.Empty)
                                {
                                    oMovScheda.DatiXML = oRow.Cells["Dati"].Value.ToString();
                                    oMovScheda.Ambiente.Codlogin = utenteelab;
                                    oMovScheda.Ambiente.Codruolo = ruoloelab;
                                    oMovScheda.Salva();

                                    string scampi = string.Empty;
                                    scampi += ", DataElaborazione = " + DataBase.SQLDateTimeInsert(DateTime.Now);
                                    scampi += ", IDPaziente = '" + IDPaziente + "'";
                                    scampi += ", IDScheda = '" + oMovScheda.IDMovScheda + "'";
                                    if (oMovScheda.IDSchedaPadre != string.Empty)
                                    {
                                        scampi += ", IDSchedaPadre = '" + oMovScheda.IDSchedaPadre + "'";
                                    }
                                    this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.EL.ToString(), "Paziente '" + IDPaziente + "' ELABORATO!", scampi);
                                }
                                else
                                {
                                    this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Dati della scheda '" + oMovScheda.IDMovScheda + "' del paziente '" + IDPaziente + "' NON corretti!");
                                }

                            }
                            else
                            {
                                oMovScheda = new MovScheda(oRow.Cells["CodScheda"].Value.ToString(),
                            EnumEntita.PAZ,
                            oRow.Cells["CodUA"].Value.ToString(),
                            IDPaziente, "", "",
                            CoreStatics.CoreApplication.Ambiente);

                                if (oMovScheda.Scheda.Versione == (int)oRow.Cells["Versione"].Value)
                                {

                                    if (int.Parse(DataBase.FindValue("QtaSchedeDisponibili", "Q_SelMovSchedeNumerosita", "CodEntita = '" + EnumEntita.PAZ.ToString() + "' And IDEntita = '" + IDPaziente + "' And CodScheda = '" + oRow.Cells["CodScheda"].Value.ToString() + "'", "1")) > 0 &&
                                        (int.Parse(DataBase.FindValue("MassimoNumero", "Q_SelMovSchedeNumerosita", "CodEntita = '" + EnumEntita.PAZ.ToString() + "' And IDEntita = '" + IDPaziente + "' And CodScheda = '" + oRow.Cells["CodScheda"].Value.ToString() + "'", "0")) + 1) == (int)oRow.Cells["Numero"].Value)
                                    {

                                        if (oRow.Cells["Dati"].Value.ToString() != string.Empty)
                                        {

                                            if (oRow.Cells["CodSchedaPadre"].Value.ToString() != string.Empty && oRow.Cells["VersioneSchedaPadre"].Value.ToString() != string.Empty && oRow.Cells["NumeroSchedaPadre"].Value.ToString() != string.Empty)
                                            {
                                                MovScheda oMovSchedaPadre = RicercaScheda(EnumEntita.PAZ.ToString(), IDPaziente, oRow.Cells["CodSchedaPadre"].Value.ToString(), int.Parse(oRow.Cells["VersioneSchedaPadre"].Value.ToString()), int.Parse(oRow.Cells["NumeroSchedaPadre"].Value.ToString()));
                                                if (oMovSchedaPadre != null)
                                                {
                                                    oMovScheda.IDSchedaPadre = oMovSchedaPadre.IDMovScheda;
                                                    oMovScheda.IDEntita = IDPaziente;
                                                    oMovScheda.DatiXML = oRow.Cells["Dati"].Value.ToString();
                                                    oMovScheda.Ambiente.Codlogin = utenteelab;
                                                    oMovScheda.Ambiente.Codruolo = ruoloelab;
                                                    oMovScheda.Salva();

                                                    string scampi = string.Empty;
                                                    scampi += ", DataElaborazione = " + DataBase.SQLDateTimeInsert(DateTime.Now);
                                                    scampi += ", IDPaziente = '" + IDPaziente + "'";
                                                    scampi += ", IDScheda = '" + oMovScheda.IDMovScheda + "'";
                                                    scampi += ", IDSchedaPadre = '" + oMovScheda.IDSchedaPadre + "'";
                                                    this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.EL.ToString(), "Paziente '" + IDPaziente + "' ELABORATO!", scampi);

                                                }
                                                else
                                                {
                                                    this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Scheda padre (" + oRow.Cells["CodSchedaPadre"].Value.ToString() + ") del paziente '" + IDPaziente + "' INESISTENTE!");
                                                }

                                            }
                                            else
                                            {
                                                oMovScheda.IDEntita = IDPaziente;
                                                oMovScheda.DatiXML = oRow.Cells["Dati"].Value.ToString();
                                                oMovScheda.Ambiente.Codlogin = utenteelab;
                                                oMovScheda.Ambiente.Codruolo = ruoloelab;
                                                oMovScheda.Salva();

                                                string scampi = string.Empty;
                                                scampi += ", DataElaborazione = " + DataBase.SQLDateTimeInsert(DateTime.Now);
                                                scampi += ", IDPaziente = '" + IDPaziente + "'";
                                                scampi += ", IDScheda = '" + oMovScheda.IDMovScheda + "'";
                                                if (oMovScheda.IDSchedaPadre != string.Empty)
                                                {
                                                    scampi += ", IDSchedaPadre = '" + oMovScheda.IDSchedaPadre + "'";
                                                }
                                                this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.EL.ToString(), "Paziente '" + IDPaziente + "' ELABORATO!", scampi);
                                            }

                                        }
                                        else
                                        {
                                            this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Dati della scheda '" + oMovScheda.IDMovScheda + "' del paziente '" + IDPaziente + "' NON corretti!");
                                        }

                                    }
                                    else
                                    {
                                        this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Numero (disponibile e/o massimo) della scheda '" + oMovScheda.IDMovScheda + "' del paziente '" + IDPaziente + "' NON corretto!");
                                    }

                                }
                                else
                                {
                                    this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Versione della scheda '" + oMovScheda.IDMovScheda + "' del paziente '" + IDPaziente + "' NON corretta!");
                                }
                            }

                        }

                    }

                }

                this.InitializeUltraProgressBar();

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void ElaboraAppuntamenti()
        {

            string IDEpisodio = string.Empty;
            string IDTrasferimento = string.Empty;

            Episodio oEpisodio = null;
            Paziente oPaziente = null;
            Trasferimento oTrasferimento = null;

            MovAppuntamento oAppuntamento = null;

            string utenteelab = this.UtenteElaborazione();
            string ruoloelab = this.RuoloElaborazione();

            bool belab = false;

            try
            {
                this.InitializeUltraProgressBar(0, this.UltraGrid.Rows.FilteredInRowCount);
                foreach (UltraGridRow oRow in this.UltraGrid.Rows.GetFilteredInNonGroupByRows())
                {
                    this.UpdateProgressBar();

                    belab = false;

                    IDEpisodio = string.Empty;
                    IDTrasferimento = string.Empty;
                    oEpisodio = null;
                    oPaziente = null;
                    oTrasferimento = null;
                    oAppuntamento = null;

                    if (oRow.Cells["CodStatoElaborazione"].Value.ToString() != enumStatoElaborazioni.EL.ToString())
                    {

                        if (oRow.Cells["CodEntita"].Value.ToString() == enumCodEntita.EPI.ToString())
                        {
                            IDEpisodio = RicercaEpisodioByNumeroNosologico(oRow.Cells["NumeroNosologico"].Value.ToString());
                            if (IDEpisodio == string.Empty)
                            {

                                IDEpisodio = RicercaEpisodioByNumeroListaAttesa(oRow.Cells["NumeroListaAttesa"].Value.ToString());

                            }
                            if (IDEpisodio != string.Empty)
                            {
                                oEpisodio = new Episodio(IDEpisodio);
                                if (oEpisodio.Attivo == true)
                                {
                                    oPaziente = new Paziente("", oEpisodio.ID);
                                    if (oPaziente.Attivo == true)
                                    {

                                        IDTrasferimento = RicercaTrasferimento(oEpisodio.ID, oRow.Cells["CodUA"].Value.ToString(), "AT");
                                        if (IDTrasferimento == string.Empty)
                                        {

                                            IDTrasferimento = RicercaTrasferimento(oEpisodio.ID, oRow.Cells["CodUA"].Value.ToString(), "PR");

                                        }
                                        if (IDTrasferimento != string.Empty)
                                        {

                                            oTrasferimento = new Trasferimento(IDTrasferimento, CoreStatics.CoreApplication.Ambiente);
                                            if (oTrasferimento.Attivo == true)
                                            {
                                                belab = true;
                                            }
                                            else
                                            {
                                                this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Trasferimento dell episodio '" + IDEpisodio + "' NON trovato!");
                                            }

                                        }
                                        else
                                        {
                                            this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Trasferimento dell episodio '" + IDEpisodio + "' NON trovato!");
                                        }

                                    }
                                    else
                                    {
                                        this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Paziente dell episodio '" + IDEpisodio + "' NON trovato!");
                                    }
                                }
                                else
                                {
                                    this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Episodio '" + IDEpisodio + "' NON trovato!");
                                }
                            }
                            else
                            {
                                this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Episodio NON trovato!");
                            }
                        }
                        else if (oRow.Cells["CodEntita"].Value.ToString() == enumCodEntita.PAZ.ToString())
                        {
                            oPaziente = new Paziente(oRow.Cells["IDPazienteRiferimento"].Value.ToString(), string.Empty);
                            if (oPaziente.Attivo == true)
                            {
                                belab = true;
                            }
                            else
                            {
                                this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "ID Paziente '" + oRow.Cells["IDPazienteRiferimento"].Value.ToString() + "' NON trovato in anagrafica Matilde !");
                            }
                        }

                        if (belab)
                        {

                            DateTime dataorainizio = DateTime.Parse(oRow.Cells["DataOraInizio"].Value.ToString());
                            DateTime dataorafine = DateTime.Parse(oRow.Cells["DataOraFine"].Value.ToString());
                            MovAppuntamento omovapp = RicercaAppuntamento(oPaziente.ID, IDEpisodio, IDTrasferimento, oRow.Cells["CodTipoAppuntamento"].Value.ToString(),
                                             oRow.Cells["CodAgenda"].Value.ToString(), dataorainizio, dataorafine);

                            if (omovapp == null)
                            {
                                oAppuntamento = new MovAppuntamento(oRow.Cells["CodUA"].Value.ToString(), oPaziente.ID, IDEpisodio, IDTrasferimento);
                                oAppuntamento.CodTipoAppuntamento = oRow.Cells["CodTipoAppuntamento"].Value.ToString();
                                oAppuntamento.CodScheda = DataBase.FindValue("CodScheda", "T_TipoAppuntamento", "Codice = '" + oRow.Cells["CodTipoAppuntamento"].Value.ToString() + "'", "");
                                if (oAppuntamento.CodScheda == oRow.Cells["CodScheda"].Value.ToString())
                                {
                                    if (oAppuntamento.MovScheda.Versione == (int)oRow.Cells["Versione"].Value)
                                    {
                                        CoreStatics.CoreApplication.Sessione = new Sessione(CoreStatics.CoreApplication.Ambiente);
                                        CoreStatics.CoreApplication.Sessione.Utente = new Utente(utenteelab);
                                        CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato = new Ruolo(ruoloelab, "", "", 0, 0, false, false);
                                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato = oAppuntamento;
                                        oAppuntamento.CaricaAgende();
                                        MovAppuntamentoAgende oItem = null;
                                        try
                                        {
                                            oItem = oAppuntamento.Elementi.Single(MovAppuntamentoAgende => MovAppuntamentoAgende.CodAgenda == oRow.Cells["CodAgenda"].Value.ToString());
                                        }
                                        catch (Exception)
                                        {
                                        }
                                        if (oItem != null)
                                        {
                                            oItem.Selezionata = true;
                                            oItem.Modificata = true;
                                            oAppuntamento.ElencoRisorse = oItem.Descrizione;
                                            oAppuntamento.DataInizio = dataorainizio;
                                            oAppuntamento.DataFine = dataorafine;

                                            oAppuntamento.MovScheda.DatiXML = this.CompilaMovScheda(oAppuntamento.MovScheda, oRow.Cells["CodCampoScheda"].Value.ToString(), oRow.Cells["Valore"].Value.ToString());
                                            if (oAppuntamento.MovScheda.DatiXML != string.Empty)
                                            {
                                                oAppuntamento.MovScheda.Ambiente.Codlogin = utenteelab;
                                                oAppuntamento.MovScheda.Ambiente.Codruolo = ruoloelab;

                                                if (oAppuntamento.Salva())
                                                {
                                                    string scampi = string.Empty;
                                                    scampi += ", DataElaborazione = " + DataBase.SQLDateTimeInsert(DateTime.Now);
                                                    scampi += ", IDPaziente = '" + oPaziente.ID + "'";
                                                    if (oRow.Cells["CodEntita"].Value.ToString() != EnumEntita.PAZ.ToString())
                                                    {
                                                        if (IDEpisodio != string.Empty) { scampi += ", IDEpisodio = '" + IDEpisodio + "'"; }
                                                        if (IDTrasferimento != string.Empty) { scampi += ", IDTrasferimento = '" + IDTrasferimento + "'"; }
                                                    }
                                                    scampi += ", IDAppuntamento = '" + oAppuntamento.IDAppuntamento + "'";
                                                    oAppuntamento.CaricaAgende();
                                                    oItem = oAppuntamento.Elementi.Single(MovAppuntamentoAgende => MovAppuntamentoAgende.CodAgenda == oRow.Cells["CodAgenda"].Value.ToString());
                                                    if (oItem != null)
                                                    {
                                                        scampi += ", IDAppuntamentoAgenda = '" + oItem.ID.ToString() + "'";
                                                    }
                                                    scampi += ", IDScheda = '" + oAppuntamento.IDScheda + "'";

                                                    this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.EL.ToString(), "Record ID '" + oRow.Cells["IDNum"].Value.ToString() + "' ELABORATO CON SUCCESSO !", scampi);
                                                }
                                                else
                                                {
                                                    this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Errore creazione appuntamento !");
                                                }
                                            }
                                            else
                                            {
                                                this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Campo " + oRow.Cells["CodCampoScheda"].Value.ToString() + " inesistente nella scheda " + oRow.Cells["CodScheda"].Value.ToString() + " versione " + oRow.Cells["Versione"].Value.ToString() + " !");
                                            }

                                        }
                                        else
                                        {
                                            this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Agenda " + oRow.Cells["CodAgenda"].Value.ToString() + " non selezionabile per il tipo appuntamento " + oRow.Cells["CodTipoAppuntamento"].Value.ToString() + " !");
                                        }
                                    }
                                    else
                                    {
                                        this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Versione " + oRow.Cells["Versione"].Value.ToString() + " della scheda " + oRow.Cells["CodScheda"].Value.ToString() + " non corrispondente con configurazione per tipo appuntamento " + oRow.Cells["CodTipoAppuntamento"].Value.ToString() + " !");
                                    }
                                }
                                else
                                {
                                    this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Scheda " + oRow.Cells["CodScheda"].Value.ToString() + " non corrispondente con configurazione per tipo appuntamento " + oRow.Cells["CodTipoAppuntamento"].Value.ToString() + " !");
                                }
                            }
                            else
                            {
                                this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Dati passati coddispondenti ad appuntamento " + omovapp.IDAppuntamento + " !");
                            }
                            omovapp = null;
                        }

                    }

                }

                this.InitializeUltraProgressBar();

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void ElaboraPazienti()
        {
            string IDEpisodio = string.Empty;

            Episodio oEpisodio = null;
            Paziente oPaziente = null;

            string utenteelab = this.UtenteElaborazione();
            string ruoloelab = this.RuoloElaborazione();

            bool belab = false;

            try
            {
                this.InitializeUltraProgressBar(0, this.UltraGrid.Rows.FilteredInRowCount);
                foreach (UltraGridRow oRow in this.UltraGrid.Rows.GetFilteredInNonGroupByRows())
                {
                    this.UpdateProgressBar();
                    IDEpisodio = string.Empty;
                    belab = false;
                    if (oRow.Cells["CodStatoElaborazione"].Value.ToString() != enumStatoElaborazioni.EL.ToString())
                    {

                        if (oRow.Cells["CodEntita"].Value.ToString() == enumCodEntita.EPI.ToString())
                        {
                            IDEpisodio = RicercaEpisodioByNumeroNosologico(oRow.Cells["NumeroNosologico"].Value.ToString());
                            if (IDEpisodio == string.Empty)
                            {

                                IDEpisodio = RicercaEpisodioByNumeroListaAttesa(oRow.Cells["NumeroListaAttesa"].Value.ToString());

                            }
                            if (IDEpisodio != string.Empty)
                            {
                                oEpisodio = new Episodio(IDEpisodio);
                                if (oEpisodio.Attivo == true)
                                {
                                    oPaziente = new Paziente("", oEpisodio.ID);
                                    if (oPaziente.Attivo == true)
                                    {

                                        belab = true;

                                    }
                                    else
                                    {
                                        this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Paziente dell episodio '" + IDEpisodio + "' NON trovato!");
                                    }
                                }
                                else
                                {
                                    this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Episodio '" + IDEpisodio + "' NON trovato!");
                                }
                            }
                            else
                            {
                                this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Episodio NON trovato!");
                            }
                        }
                        else if (oRow.Cells["CodEntita"].Value.ToString() == enumCodEntita.PAZ.ToString())
                        {
                            oPaziente = new Paziente(oRow.Cells["IDPazienteRiferimento"].Value.ToString(), string.Empty);
                            if (oPaziente.Attivo == true)
                            {
                                belab = true;
                            }
                            else
                            {
                                this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "ID Paziente '" + oRow.Cells["IDPazienteRiferimento"].Value.ToString() + "' NON trovato in anagrafica Matilde !");
                            }
                        }

                        if (belab)
                        {
                            try
                            {
                                CoreStatics.CoreApplication.Sessione = new Sessione(CoreStatics.CoreApplication.Ambiente);
                                CoreStatics.CoreApplication.Sessione.Utente = new Utente(utenteelab);
                                CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato = new Ruolo(ruoloelab, "", "", 0, 0, false, false);

                                oPaziente.AggiornaDatiSAC();

                                string scampi = string.Empty;
                                scampi += ", DataElaborazione = " + DataBase.SQLDateTimeInsert(DateTime.Now);
                                scampi += ", IDPaziente = '" + oPaziente.ID + "'";
                                if (IDEpisodio != string.Empty) { scampi += ", IDEpisodio = '" + IDEpisodio + "'"; }

                                this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.EL.ToString(), "Record ID '" + oRow.Cells["IDNum"].Value.ToString() + "' ELABORATO CON SUCCESSO !", scampi);
                            }
                            catch
                            {
                                this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), "Errore aggiornamento dati da SAC !");
                            }
                        }

                    }

                }

                this.InitializeUltraProgressBar();

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        private void ElaboraCollegaCartelle()
        {

            ScciAmbiente ambiente = new ScciAmbiente();

            Cartella cartellaorigine = null;
            Trasferimento trasferimentoorigine = null;

            Trasferimento trasferimentodestinazione = null;

            Paziente paziente = null;

            UnicodeSrl.Scci.CollegaCartella collegacart = new UnicodeSrl.Scci.CollegaCartella();

            try
            {

                ambiente.Codlogin = this.UtenteElaborazione();
                ambiente.Codruolo = this.RuoloElaborazione();

                this.InitializeUltraProgressBar(0, this.UltraGrid.Rows.FilteredInRowCount);
                foreach (UltraGridRow oRow in this.UltraGrid.Rows.GetFilteredInNonGroupByRows())
                {
                    if (oRow.Cells["CodStatoElaborazione"].Value.ToString() != enumStatoElaborazioni.EL.ToString())
                    {
                        cartellaorigine = new Cartella(oRow.Cells["IDCartellaOrigine"].Value.ToString(), string.Empty, ambiente);
                        trasferimentoorigine = new Trasferimento(oRow.Cells["IDTrasferimentoOrigine"].Value.ToString(), ambiente);

                        trasferimentodestinazione = new Trasferimento(oRow.Cells["IDTrasferimentoDestinazione"].Value.ToString(), ambiente);

                        paziente = new Paziente(string.Empty, trasferimentoorigine.IDEpisodio);

                        if (collegacart.ControllaNuovoNumero(ambiente, oRow.Cells["NumeroCartellaDestinazione"].Value.ToString(), oRow.Cells["AnnoCartellaDestinazione"].Value.ToString(),
                                trasferimentodestinazione.ID, trasferimentodestinazione.IDEpisodio, paziente.ID))
                        {
                            if (collegacart.ControllaCartellaDestinazione(ambiente, cartellaorigine.ID, trasferimentodestinazione.ID, paziente.ID))
                            {
                                if (trasferimentoorigine.IDCartella == cartellaorigine.ID)
                                {
                                    collegacart.Collega(ambiente, oRow.Cells["NumeroCartellaDestinazione"].Value.ToString(), oRow.Cells["AnnoCartellaDestinazione"].Value.ToString(),
                    cartellaorigine.ID, trasferimentoorigine.ID, trasferimentoorigine.IDEpisodio, trasferimentodestinazione.ID,
                    trasferimentodestinazione.IDEpisodio, paziente.ID, this.ucChkPDFCartelleChiuse.Checked);

                                    if (collegacart.StepNumber == 6)
                                    {
                                        trasferimentodestinazione = new Trasferimento(oRow.Cells["IDTrasferimentoDestinazione"].Value.ToString(), ambiente);

                                        string scampi = string.Empty;
                                        scampi += ", IDCartellaDestinazione = '" + trasferimentodestinazione.IDCartella.ToString() + "'";

                                        this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.EL.ToString(),
                                            "Record ID '" + oRow.Cells["IDNum"].Value.ToString() + "' ELABORATO CON SUCCESSO !", scampi);
                                    }
                                    else
                                    {
                                        this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), collegacart.StatusText);
                                    }
                                }
                                else
                                {
                                    this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(),
                                        "ID cartella trasferimento " + trasferimentoorigine.ID + " diverso da " + cartellaorigine.ID);
                                }
                            }
                            else
                            {
                                this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), collegacart.StatusText);
                            }
                        }
                        else
                        {
                            this.AggiornaRecord((int)oRow.Cells["IDNum"].Value, enumStatoElaborazioni.ER.ToString(), collegacart.StatusText);
                        }

                        this.UpdateProgressBar();
                    }



                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            finally
            {
                collegacart = null;
                ambiente = null;

                cartellaorigine = null;
                trasferimentoorigine = null;

                trasferimentodestinazione = null;

                paziente = null;

                this.InitializeUltraProgressBar();
            }

        }

        #endregion

        #region Sub Elaborazioni

        private string RicercaEpisodioByNumeroNosologico(string numeronosologico)
        {

            string sret = string.Empty;

            try
            {
                sret = DataBase.FindValue("ID", "T_MovEpisodi", "NumeroNosologico = '" + numeronosologico + "'", "");
            }
            catch (Exception)
            {
                sret = string.Empty;
            }

            return sret;

        }

        private string RicercaEpisodioByNumeroListaAttesa(string numerolistaattesa)
        {

            string sret = string.Empty;

            try
            {
                sret = DataBase.FindValue("ID", "T_MovEpisodi", "NumeroListaAttesa = '" + numerolistaattesa + "'", "");
            }
            catch (Exception)
            {
                sret = string.Empty;
            }

            return sret;

        }

        private string RicercaTrasferimento(string idepisodio, string codua, string codstato)
        {

            string sret = string.Empty;

            try
            {
                sret = DataBase.FindValue("ID", "T_MovTrasferimenti", "IDEpisodio = '" + idepisodio + "' And CodUA = '" + codua + "' And CodStatoTrasferimento = '" + codstato + "'", "");
            }
            catch (Exception)
            {
                sret = string.Empty;
            }

            return sret;

        }

        private MovScheda RicercaScheda(string codentita, string identita, string codscheda, int versione, int numero)
        {



            MovScheda oMovScheda = null;

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodEntita", codentita);
                op.Parametro.Add("IDEntita", identita);
                op.Parametro.Add("CodScheda", codscheda);
                op.Parametro.Add("Versione", versione.ToString());
                op.Parametro.Add("Numero", numero.ToString());
                op.Parametro.Add("Storicizzata", "NO");

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataTable odt = Database.GetDataTableStoredProc("MSP_SelMovSchedaAvanzato", spcoll);
                if (odt.Rows.Count == 1)
                {
                    oMovScheda = new MovScheda(odt.Rows[0]["ID"].ToString(), CoreStatics.CoreApplication.Ambiente);
                }

            }
            catch (Exception)
            {

            }

            return oMovScheda;

        }

        private MovAppuntamento RicercaAppuntamento(string idPaziente, string idEpisodio, string idTrasferimento,
                                         string codTipoAppuntamento, string codAgenda, DateTime dataInizio, DateTime dataFine)
        {


            MovAppuntamento movapp = null;
            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDPaziente", idPaziente);
                if (idEpisodio != string.Empty) op.Parametro.Add("IDEpisodio", idEpisodio);
                if (idTrasferimento != string.Empty) op.Parametro.Add("IDTrasferimento", idTrasferimento);
                op.Parametro.Add("CodTipoAppuntamento", codTipoAppuntamento);
                op.Parametro.Add("CodAgenda", codAgenda);

                op.Parametro.Add("DataInizio", UnicodeSrl.Scci.Statics.Database.dataOra105PerParametri(dataInizio));
                op.Parametro.Add("DataFine", UnicodeSrl.Scci.Statics.Database.dataOra105PerParametri(dataFine));


                op.Parametro.Add("DatiEstesi", "0");

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = UnicodeSrl.Scci.Statics.XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable odt = UnicodeSrl.Scci.Statics.Database.GetDataTableStoredProc("MSP_SelMovAppuntamenti", spcoll);

                if (odt != null && odt.Rows.Count > 0)
                    movapp = new MovAppuntamento(odt.Rows[0]["ID"].ToString());

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                movapp = null;
            }

            return movapp;
        }

        private void AggiornaRecord(int idnum, string stato, string note)
        {
            this.AggiornaRecord(idnum, stato, note, string.Empty);
        }
        private void AggiornaRecord(int idnum, string stato, string note, string campiaggiuntivi)
        {
            string sSql = string.Empty;

            try
            {

                switch (this.ActiveUltraTab)
                {

                    case enumTabElaborazioni.NonDefinito:
                        break;

                    case enumTabElaborazioni.AperturaCartelle:
                        sSql = "Update T_BO_Cartelle Set CodStatoElaborazione = '" + stato + "', Note = '" + DataBase.Ax2(note) + "'" + campiaggiuntivi + " Where IDNum = " + idnum.ToString();
                        DataBase.ExecuteSql(sSql);
                        break;

                    case enumTabElaborazioni.CreazioneSchede:
                        sSql = "Update T_BO_Schede Set CodStatoElaborazione = '" + stato + "', Note = '" + DataBase.Ax2(note) + "'" + campiaggiuntivi + " Where IDNum = " + idnum.ToString();
                        DataBase.ExecuteSql(sSql);
                        break;

                    case enumTabElaborazioni.CreazioneAppuntamenti:
                        sSql = "Update T_BO_Appuntamenti Set CodStatoElaborazione = '" + stato + "', Note = '" + DataBase.Ax2(note) + "'" + campiaggiuntivi + " Where IDNum = " + idnum.ToString();
                        DataBase.ExecuteSql(sSql);
                        break;

                    case enumTabElaborazioni.PazientiDatiSAC:
                        sSql = "Update T_BO_Pazienti Set CodStatoElaborazione = '" + stato + "', Note = '" + DataBase.Ax2(note) + "'" + campiaggiuntivi + " Where IDNum = " + idnum.ToString();
                        DataBase.ExecuteSql(sSql);
                        break;

                    case enumTabElaborazioni.CollegaCartelle:
                        sSql = "Update T_BO_CartelleCollega Set CodStatoElaborazione = '" + stato + "', DataElaborazione = " + Database.dataOraSQL(DateTime.Now) + ", Note = '" + DataBase.Ax2(note) + "'" + campiaggiuntivi + " Where IDNum = " + idnum.ToString();
                        DataBase.ExecuteSql(sSql);
                        break;

                }


            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        private bool ControlloNumeroCartella(string codUA, string idPaziente, string idEpisodio, string idTrasferimento, string NumeroCartella)
        {

            bool bret = true;

            Scci.DataContracts.ScciAmbiente ambiente = CoreStatics.CoreApplication.Ambiente;
            ambiente.Codlogin = UnicodeSrl.Framework.Utility.Windows.CurrentUser();

            try
            {
                Parametri op = new Parametri(ambiente);
                op.Parametro.Add("CodUA", codUA);
                op.Parametro.Add("IDPaziente", idPaziente);
                op.Parametro.Add("IDEpisodio", idEpisodio);
                op.Parametro.Add("IDTrasferimento", idTrasferimento);
                op.Parametro.Add("NumeroCartella", NumeroCartella);

                op.TimeStamp.CodEntita = EnumEntita.CAR.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = UnicodeSrl.Scci.Statics.XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);


                DataSet ds = UnicodeSrl.Scci.Statics.Database.GetDatasetStoredProc("MSP_ControlloNumeroCartella", spcoll);

                if (ds.Tables[0].Rows.Count > 0)
                    bret = Convert.ToBoolean(ds.Tables[0].Rows[0]["Usato"]);
                else
                    bret = false;
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                bret = false;
            }

            return bret;

        }

        private bool AperturaCartella(string idTrasferimento, string NumeroCartella, string UtenteElaborazione)
        {
            bool bret = false;

            Scci.DataContracts.ScciAmbiente ambiente = CoreStatics.CoreApplication.Ambiente;
            ambiente.Codlogin = UtenteElaborazione;

            try
            {

                Parametri op = new Parametri(ambiente);
                op.Parametro.Add("IDTrasferimento", idTrasferimento);
                op.Parametro.Add("NumeroCartella", NumeroCartella);
                op.Parametro.Add("CodStatoCartella", EnumStatoCartella.AP.ToString());

                op.TimeStamp.CodEntita = EnumEntita.CAR.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = UnicodeSrl.Scci.Statics.XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                UnicodeSrl.Scci.Statics.Database.ExecStoredProc("MSP_AggMovTrasferimentiCartella", spcoll);

                bret = true;

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                bret = false;
            }

            return bret;
        }

        private string UtenteElaborazione()
        {
            string sret = string.Empty;
            try
            {
                sret = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.ElabSistemiUserName);
            }
            catch
            {
                sret = string.Empty;
            }

            return sret;
        }

        private string RuoloElaborazione()
        {
            string sret = string.Empty;
            try
            {
                sret = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.ElabSistemiCodRuolo);
            }
            catch
            {
                sret = string.Empty;
            }

            return sret;
        }

        private MovScheda CreaMovScheda(string codTipoAppuntamento, string codEntita, string codUA, string idPaziente, string idEpisodio, string idTrasferimento)
        {

            MovScheda omovscheda = null;
            string codscheda = string.Empty;

            try
            {
                codscheda = DataBase.FindValue("CodScheda", "T_TipoAppuntamento", "Codice = '" + codTipoAppuntamento + "'", "");

                if (codscheda != string.Empty)
                {
                    omovscheda = new MovScheda(codscheda, (EnumEntita)Enum.Parse(typeof(EnumEntita), codEntita),
                                    codUA, idPaziente, idEpisodio, idTrasferimento, CoreStatics.CoreApplication.Ambiente);

                }

            }
            catch
            {
                omovscheda = null;
            }

            return omovscheda;

        }

        private string CompilaMovScheda(MovScheda movScheda, string codCampo, string valoreCampo)
        {
            string sret = string.Empty;

            Gestore gestore = new Gestore();

            try
            {

                gestore.SchedaXML = movScheda.Scheda.StrutturaXML;
                gestore.SchedaLayoutsXML = movScheda.Scheda.LayoutXML;

                gestore.Decodifiche = movScheda.Scheda.DizionarioValori();

                if (movScheda.DatiXML == string.Empty)
                {
                    gestore.SchedaDati = new DcSchedaDati();
                }
                else
                {
                    try
                    {
                        gestore.SchedaDatiXML = movScheda.DatiXML;
                    }
                    catch (Exception)
                    {
                        gestore.SchedaDati = new DcSchedaDati();
                    }
                }
                if (gestore.SchedaDati.Dati.Count == 0) { gestore.NuovaScheda(); }

                gestore.ModificaValore(codCampo, 1, valoreCampo);
                sret = gestore.SchedaDatiXML;

            }
            catch
            {
                sret = string.Empty;
            }

            return sret;
        }

        #endregion

        #region Events

        private void frmElaborazioni_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.UltraGrid != null) this.UltraGrid.Dispose();
        }

        private void UltraTabControl_ActiveTabChanged(object sender, Infragistics.Win.UltraWinTabControl.ActiveTabChangedEventArgs e)
        {
            this.LoadUltraGrid();
        }

        private void UltraGrid_AfterRowFilterChanged(object sender, AfterRowFilterChangedEventArgs e)
        {
            this.RefreshGrid();
        }

        private void ubElabora_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Vuoi elaborare i record selezionati non in stato EL ?", "Elaborazione", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
            {
                this.Elabora();
                this.LoadUltraGrid();
                MessageBox.Show("Finito!!!", "Elaborazione", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        #endregion

    }
}
