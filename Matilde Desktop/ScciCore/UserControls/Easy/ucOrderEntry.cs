using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.ScciResource;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinListView;
using UnicodeSrl.Framework.Data;
using Infragistics.Win;
using UnicodeSrl.ScciCore.CustomControls.Infra;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;
using UnicodeSrl.ScciCore.WebSvc;
using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.Scci.PluginClient;

namespace UnicodeSrl.ScciCore
{
    public partial class ucOrderEntry : UserControl, Interfacce.IViewUserControlMiddle
    {
        #region Declare

        private UserControl _ucc = null;

        private bool _aggiornagriglia = true;

        private bool _percorsoambulatoriale = false;

        #endregion

        public ucOrderEntry()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.SupportsTransparentBackColor, true);

            _ucc = (UserControl)this;
        }

        #region Interface

        public void Aggiorna()
        {
            if (_aggiornagriglia)
            {
                this.VerificaSicurezza();

                SvuotaDettaglioOrdine();
                this.CaricaGriglia(string.Empty);
                this.FiltraGriglia();
            }
        }

        public void Carica()
        {
            try
            {
                this.InizializzaControlli();
                this.InizializzaUltraGridLayout();
                this.VerificaSicurezza();
                this.InizializzaFiltri();

                SvuotaDettaglioOrdine();

                this.CaricaGriglia(string.Empty);
                this.FiltraGriglia();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Carica", this.Name);
            }
        }

        public void Ferma()
        {

            try
            {

                CoreStatics.SetContesto(EnumEntita.OE, null);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Ferma", this.Name);
            }

        }

        #endregion

        #region Public Properties

        public bool PercorsoAmbulatoriale
        {
            get { return _percorsoambulatoriale; }
            set { _percorsoambulatoriale = value; }
        }

        #endregion

        #region private functions

        private void InizializzaControlli()
        {

            try
            {
                this.ubAdd.Appearance.Image = Properties.Resources.Aggiungi_256;
                this.uchkFiltro.UNCheckedImage = Risorse.GetImageFromResource(Risorse.GC_FILTRO_256);
                this.uchkFiltro.CheckedImage = Risorse.GetImageFromResource(Risorse.GC_FILTROAPPLICATO_256);
                this.uchkFiltro.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                this.uchkFiltro.Checked = false;

                this.ubAdd.PercImageFill = 0.75F;
                this.ubAdd.ShortcutKey = Keys.Add;
                this.ubAdd.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.XSmall;
                this.ubAdd.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.uchkFiltro.PercImageFill = 0.75F;
                this.uchkFiltro.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.XSmall;

                this.ubApplicaFiltro.TextFontRelativeDimension = easyStatics.easyRelativeDimensions.Large;

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaControlli", this.Name);
            }
        }

        private void InizializzaUltraGridLayout()
        {
            try
            {
                CoreStatics.SetEasyUltraGridLayout(ref this.ucEasyGrid);
                CoreStatics.SetEasyUltraGridLayout(ref this.ucEasyGridDettaglio);
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaUltraGridLayout", this.Name);
            }
        }

        private void VerificaSicurezza()
        {

            try
            {
                if (CoreStatics.CoreApplication.Cartella != null && CoreStatics.CoreApplication.Cartella.CartellaChiusa == true)
                {
                    this.ubAdd.Enabled = false;
                }
                else
                {
                    this.ubAdd.Enabled = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Ordini_Inserisci);
                }
            }
            catch
            {
            }
        }

        private void InizializzaFiltri()
        {
            try
            {
                this._aggiornagriglia = false;

                this.drFiltro.Value = null;
                this.udteFiltroDA.Value = null;
                this.udteFiltroA.Value = null;
                this.txtFiltroErogante.Text = string.Empty;

                Infragistics.Win.UltraWinListView.UltraListViewItem oitem = null;

                Array values = System.Enum.GetValues(typeof(OEStato));
                Array names = System.Enum.GetNames(typeof(OEStato));

                ucEasyListBoxFiltro.Items.Clear();

                for (int i = 0; i <= names.GetUpperBound(0); i++)
                {
                    switch ((OEStato)Enum.Parse(typeof(OEStato), values.GetValue(i).ToString()))
                    {
                        case OEStato.NN:
                            oitem = null;
                            break;
                        case OEStato.Cancellato:
                        case OEStato.Erogato:
                            oitem = new Infragistics.Win.UltraWinListView.UltraListViewItem(values.GetValue(i).ToString());
                            oitem.Value = names.GetValue(i).ToString();
                            break;

                        case OEStato.Errato:
                        case OEStato.InCarico:
                        case OEStato.Inoltrato:
                        case OEStato.Inserito:
                        case OEStato.Programmato:
                        case OEStato.Accettato:
                        case OEStato.Annullato:
                            oitem = new Infragistics.Win.UltraWinListView.UltraListViewItem(values.GetValue(i).ToString());
                            oitem.Value = names.GetValue(i).ToString();
                            oitem.CheckState = CheckState.Checked;
                            break;
                    }

                    if (oitem != null) ucEasyListBoxFiltro.Items.Add(oitem);

                }

                this._aggiornagriglia = true;

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaFiltri", this.Name);
            }

        }

        private void CaricaGriglia(string IDOrdine)
        {

            bool bFiltro = false;
            DataTable odt = null;
            SvcOrderEntry.ScciOrderEntryClient oeclient = null;
            Scci.DataContracts.OEOrdineTestata[] ordini = null;
            List<Scci.DataContracts.OEOrdineTestata> ordinilist = null;

            DateTime DataFiltroDA = DateTime.MinValue;
            DateTime DataFiltroA = DateTime.MaxValue;


            CoreStatics.SetNavigazione(false);

            try
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);
                CoreStatics.SetContesto(EnumEntita.OE, null);

                this.Refresh();

                this.ucEasyGrid.DisplayLayout.Appearance.ImageBackground = null;
                this.ucEasyGrid.DisplayLayout.Appearance.ImageBackground = Risorse.GetImageFromResource(Risorse.GC_WAIT_32);
                this.ucEasyGrid.DisplayLayout.Appearance.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Centered;
                this.ucEasyGrid.Refresh();

                this.SvuotaGrigliaOrdini();

                oeclient = ScciSvcRef.GetSvcOrderEntry();

                if (this.PercorsoAmbulatoriale)
                {
                    ordini = oeclient.CercaOrdiniPerPaziente(CoreStatics.CoreApplication.Ambiente.Codlogin,
                                            DateTime.MinValue, DateTime.MaxValue,
                                            CoreStatics.CoreApplication.Paziente.CodSAC);
                }
                else
                {
                    ordini = oeclient.CercaOrdiniPerNosologico(CoreStatics.CoreApplication.Ambiente.Codlogin,
                                            DateTime.MinValue, DateTime.MaxValue,
                                            CoreStatics.CoreApplication.Episodio.NumeroEpisodio,
                                            CoreStatics.CoreApplication.Episodio.NumeroListaAttesa);
                }

                odt = CoreStatics.CreateDataTable<Scci.DataContracts.OEOrdineTestata>();

                ordinilist = new List<Scci.DataContracts.OEOrdineTestata>();

                foreach (Scci.DataContracts.OEOrdineTestata ordine in ordini)
                    ordinilist.Add(ordine);

                CoreStatics.FillDataTable<Scci.DataContracts.OEOrdineTestata>(odt, ordinilist);


                odt.Columns.Add("PERMESSOMODIFICA", typeof(bool));
                odt.Columns.Add("PERMESSOCANCELLA", typeof(bool));
                odt.Columns.Add("PERMESSOCOPIA", typeof(bool));
                odt.Columns.Add("PERMESSOVISUALIZZA", typeof(bool));
                odt.Columns.Add("PERMESSOINOLTRA", typeof(bool));
                odt.Columns.Add("DescInserimento", typeof(string));
                odt.Columns.Add("UtenteInserimento", typeof(string));
                odt.Columns.Add("DescModifica", typeof(string));
                odt.Columns.Add("DescNumeroPrestazioni", typeof(string));
                odt.Columns.Add("StatoIcona", typeof(byte[]));

                bool oePermessoCancella = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Ordini_Cancella);
                bool oePermessoInserisci = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Ordini_Inserisci);
                bool oePermessoModifica = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Ordini_Modifica);
                bool oePermessoInoltra = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Ordini_Inoltra);
                bool oePermessoVisualizza = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Ordini_Visualizza);

                byte[] icoinprogres = CoreStatics.ImageToByte(Risorse.GetImageFromResource(Risorse.GC_INPROGRESS_256));
                byte[] icomodificacheck = CoreStatics.ImageToByte(Risorse.GetImageFromResource(Risorse.GC_INPROGRESS_256));
                byte[] icoeleimina = CoreStatics.ImageToByte(Risorse.GetImageFromResource(Risorse.GC_INPROGRESS_256));

                foreach (DataRow row in odt.Rows)
                {
                    switch ((OEStato)row["Stato"])
                    {
                        case OEStato.Erogato:
                        case OEStato.Errato:
                        case OEStato.InCarico:
                        case OEStato.Inoltrato:
                        case OEStato.NN:
                        case OEStato.Programmato:
                        case OEStato.Accettato:
                            row["PERMESSOMODIFICA"] = false;
                            row["PERMESSOCANCELLA"] = (oePermessoCancella && ((bool)row["Cancellabile"]) == true);
                            row["PERMESSOCOPIA"] = oePermessoInserisci;
                            row["PERMESSOVISUALIZZA"] = oePermessoVisualizza;
                            row["PERMESSOINOLTRA"] = false;
                            break;

                        case OEStato.Cancellato:
                        case OEStato.Annullato:
                            row["PERMESSOMODIFICA"] = false;
                            row["PERMESSOCANCELLA"] = false;
                            row["PERMESSOCOPIA"] = oePermessoInserisci;
                            row["PERMESSOVISUALIZZA"] = oePermessoVisualizza;
                            row["PERMESSOINOLTRA"] = false;
                            break;

                        case OEStato.Inserito:
                            row["PERMESSOMODIFICA"] = oePermessoModifica;
                            row["PERMESSOCANCELLA"] = oePermessoCancella;
                            row["PERMESSOCOPIA"] = oePermessoInserisci;
                            row["PERMESSOVISUALIZZA"] = oePermessoVisualizza;
                            row["PERMESSOINOLTRA"] = (oePermessoInoltra && ((OEValiditaOrdine)row["StatoValidazione"]) == OEValiditaOrdine.Valido && ((int)row["NumeroPrestazioni"]) > 0);
                            break;

                    }

                    switch ((OEStato)row["Stato"])
                    {

                        case OEStato.Inserito:
                        case OEStato.Inoltrato:
                        case OEStato.Accettato:
                            row["StatoIcona"] = icoinprogres;
                            break;

                        case OEStato.InCarico:
                        case OEStato.Programmato:
                        case OEStato.Erogato:
                            row["StatoIcona"] = icomodificacheck;
                            break;

                        case OEStato.Cancellato:
                        case OEStato.Errato:
                        case OEStato.Annullato:
                            row["StatoIcona"] = icoeleimina;
                            break;

                        case OEStato.NN:
                            break;

                    }

                    row["DescInserimento"] = "Inserito il: " + ((DateTime)row["DataOrdine"]).ToString("dd/MM/yyyy HH:mm");
                    row["UtenteInserimento"] = "Da: " + row["Operatore"].ToString();
                    row["DescModifica"] = (row["DataModifica"].ToString() != string.Empty ? "Modificato il: " + ((DateTime)row["DataModifica"]).ToString("dd/MM/yyyy HH:mm") : string.Empty);

                    row["DescNumeroPrestazioni"] = row["NumeroPrestazioni"].ToString() + " prest.";

                }

                icoinprogres = null;
                icomodificacheck = null;
                icoeleimina = null;

                foreach (DataColumn dcCol in odt.Columns)
                {
                    if (dcCol.ColumnName.ToUpper().IndexOf("PERMESSOMODIFICA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOCANCELLA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOCOPIA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOVISUALIZZA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOINOLTRA") == 0)
                        dcCol.ReadOnly = false;
                }

                DataView odv = odt.DefaultView;
                odv.Sort = "DataOraProgrammata DESC, NumeroOrdine DESC";

                this.ucEasyGrid.DisplayLayout.Appearance.ImageBackground = null;

                this.ucEasyGrid.DataSource = null;
                this.ucEasyGrid.DataSource = odv;
                this.ucEasyGrid.Refresh();

                this.ucEasyGrid.PerformAction(UltraGridAction.FirstRowInBand, false, false);

                this.uchkFiltro.Checked = bFiltro;

                if (IDOrdine != string.Empty)
                    CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "NumeroOrdine", IDOrdine);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Order Entry non disponibile - contattare amministratori di sistema", "CaricaGriglia", this.Name);
            }
            finally
            {
                if (oeclient != null && oeclient.State == System.ServiceModel.CommunicationState.Opened)
                    oeclient.Close();

                oeclient = null;
                ordini = null;
                ordinilist = null;
                odt = null;


                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
            }

            CoreStatics.SetNavigazione(true);

        }

        private void FiltraGriglia()
        {
            OEStato StatoRiga = OEStato.NN;
            string Eroganti = string.Empty;
            UltraListViewItem ulistitem = null;

            bool bStato = true;
            bool bEroganti = true;

            try
            {
                foreach (UltraGridRow orow in this.ucEasyGrid.Rows)
                {
                    if (this.ucEasyListBoxFiltro.CheckedItems.Count > 0)
                    {
                        StatoRiga = (OEStato)Enum.Parse(typeof(OEStato), orow.Cells["Stato"].Value.ToString());
                        ulistitem = this.ucEasyListBoxFiltro.Items[StatoRiga.ToString()];
                        bStato = this.ucEasyListBoxFiltro.CheckedItems.Contains(ulistitem);
                    }
                    else
                        bStato = false;

                    Eroganti = orow.Cells["Eroganti"].Value.ToString().ToLower();

                    if (this.txtFiltroErogante.Text != string.Empty)
                        bEroganti = Eroganti.Contains(this.txtFiltroErogante.Text.ToLower());
                    else
                        bEroganti = true;

                    bool bDataProgrammazione = true;
                    try
                    {
                        DateTime dataProgrammazione = DateTime.Parse(orow.Cells["DataOraProgrammata"].Value.ToString());
                        DateTime dataInizio = DateTime.MinValue;
                        DateTime dataFine = DateTime.MaxValue;

                        if (this.udteFiltroDA.Value != null)
                            dataInizio = (DateTime)this.udteFiltroDA.Value;

                        if (this.udteFiltroA.Value != null)
                            dataFine = (DateTime)this.udteFiltroA.Value;

                        bDataProgrammazione = dataProgrammazione.CompareTo(dataInizio) >= 0 && dataProgrammazione.CompareTo(dataFine) <= 0;

                    }
                    catch
                    {
                        bDataProgrammazione = true;
                    }

                    orow.Hidden = !bStato || !bEroganti || !bDataProgrammazione;

                    if (!this.uchkFiltro.Checked)
                        this.uchkFiltro.Checked = orow.Hidden;

                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "FiltraGriglia", this.Name);
            }
        }

        private void CaricaDettaglio()
        {
            DataRow orow = null;
            DataTable odt = null;

            try
            {
                if (ucEasyGrid.ActiveRow != null && ucEasyGrid.Rows.GetFilteredInNonGroupByRows().Contains(ucEasyGrid.ActiveRow) && !ucEasyGrid.ActiveRow.Hidden)
                {
                    CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

                    this.ucEasyGridDettaglio.DisplayLayout.Appearance.ImageBackground = null;
                    this.ucEasyGridDettaglio.DisplayLayout.Appearance.ImageBackground = Risorse.GetImageFromResource(Risorse.GC_WAIT_32);
                    this.ucEasyGridDettaglio.DisplayLayout.Appearance.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Centered;
                    this.ucEasyGridDettaglio.Refresh();

                    ScciAmbiente AmbienteOE = CoreStatics.CoreApplication.Ambiente;
                    AmbienteOE.Nomepc = CoreStatics.CoreApplication.Sessione.Computer.NomeDominioCompleto;

                    this.SvuotaDettaglioOrdine();

                    if (this.PercorsoAmbulatoriale)
                    {
                        CoreStatics.CoreApplication.MovOrdineSelezionato = new MovOrdine(ucEasyGrid.ActiveRow.Cells["NumeroOrdine"].Value.ToString(),
                                                         AmbienteOE, string.Empty,
                                                         "AMB",
                                                         string.Empty, string.Empty,
                                                         ucEasyGrid.ActiveRow.Cells["UnitaOperativaAziendaCodice"].Value.ToString(),
                                                         string.Empty,
                                                         ucEasyGrid.ActiveRow.Cells["UnitaOperativaCodice"].Value.ToString(),
                                                         CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata,
                                                         CoreStatics.CoreApplication.Paziente);
                    }
                    else
                    {
                        string sCodAziOrdine = string.Empty;
                        if (CoreStatics.CoreApplication.Trasferimento.CodAziTrasferimento != null && CoreStatics.CoreApplication.Trasferimento.CodAziTrasferimento != string.Empty)
                        { sCodAziOrdine = CoreStatics.CoreApplication.Trasferimento.CodAziTrasferimento; }
                        else
                        { sCodAziOrdine = CoreStatics.CoreApplication.Episodio.CodAzienda; }

                        CoreStatics.CoreApplication.MovOrdineSelezionato = new MovOrdine(ucEasyGrid.ActiveRow.Cells["NumeroOrdine"].Value.ToString(),
                                                         AmbienteOE, CoreStatics.CoreApplication.Episodio.ID,
                                                         CoreStatics.CoreApplication.Episodio.CodTipoEpisodio,
                                                         CoreStatics.CoreApplication.Episodio.NumeroEpisodio,
                                                         CoreStatics.CoreApplication.Episodio.NumeroListaAttesa,
                                                         sCodAziOrdine,
                                                         CoreStatics.CoreApplication.Trasferimento.ID,
                                                         CoreStatics.CoreApplication.Trasferimento.CodUO,
                                                         CoreStatics.CoreApplication.Trasferimento.CodUA,
                                                         CoreStatics.CoreApplication.Paziente);
                    }


                    if (CoreStatics.CoreApplication.MovOrdineSelezionato == null)
                    {
                        easyStatics.EasyMessageBox("Impossibile recuperare i dettagli del Numero Ordine [" + ucEasyGrid.ActiveRow.Cells["NumeroOrdine"].Value.ToString() + @"]." + Environment.NewLine + @"Impossibile aprire l'Order Entry.", "CaricaDettaglio", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (CoreStatics.CoreApplication.MovOrdineSelezionato.UltimaEccezioneGenerata != null)
                    {
                        Exception rex = CoreStatics.CoreApplication.MovOrdineSelezionato.UltimaEccezioneGenerata;
                        CoreStatics.ExGest(ref rex, @"Impossibile recuperare i dettagli del Numero Ordine [" + ucEasyGrid.ActiveRow.Cells["NumeroOrdine"].Value.ToString() + @"]." + Environment.NewLine + @"Impossibile aprire l'Order Entry.", "CaricaDettaglio", this.Name);
                    }
                    else
                    {
                        DateTime dataprogrammazione = DateTime.Parse(CoreStatics.CoreApplication.MovOrdineSelezionato.DataProgrammazione.ToString());
                        this.txtDataProgrammazione.Text = (dataprogrammazione != DateTime.MinValue ? dataprogrammazione.ToString("dd/MM/yyyy HH:mm") : string.Empty);

                        odt = new DataTable();
                        odt.Columns.Add("Erogante", typeof(string));
                        odt.Columns.Add("Prestazione", typeof(string));

                        if (CoreStatics.CoreApplication.MovOrdineSelezionato != null)
                        {
                            foreach (Scci.DataContracts.OEOrdinePrestazione prestazione in CoreStatics.CoreApplication.MovOrdineSelezionato.Prestazioni)
                            {
                                orow = odt.NewRow();
                                orow["Erogante"] = prestazione.Prestazione.Erogante.Descrizione;
                                orow["Prestazione"] = prestazione.Prestazione.Descrizione + @" (" + prestazione.Prestazione.Codice + @")";
                                if (prestazione.Prestazione.Tipo == OEPrestazioneTipo.ProfiloUtente)
                                {
                                    DataTable dtPrestazioni = CoreStatics.CoreApplication.MovOrdineSelezionato.CaricaAnteprimaEsplosioneProfilo(
                                        prestazione.Prestazione.Codice,
                                        prestazione.Prestazione.Erogante.Codice,
                                        prestazione.Prestazione.Erogante.CodiceAzienda,
                                        prestazione.Prestazione.Tipo);
                                    if (dtPrestazioni.Rows.Count > 0)
                                    {
                                        orow["Prestazione"] += " ";
                                        foreach (DataRow dr in dtPrestazioni.Rows)
                                        {
                                            orow["Prestazione"] += string.Format("[{0}] ", dr["Prestazione"].ToString());
                                        }
                                    }
                                }
                                odt.Rows.Add(orow);
                            }


                        }
                    }

                    this.ucEasyGridDettaglio.DisplayLayout.Appearance.ImageBackground = null;

                    this.ucEasyGridDettaglio.DataSource = null;
                    this.ucEasyGridDettaglio.DataSource = odt;
                    this.ucEasyGridDettaglio.Refresh();

                }
                else
                {
                    CoreStatics.CoreApplication.MovOrdineSelezionato = null;
                    this.SvuotaDettaglioOrdine();
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Order Entry non disponibile - contattare amministratori di sistema", "CaricaDettaglio", this.Name);
                CoreStatics.CoreApplication.MovOrdineSelezionato = null;
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
                orow = null;
                odt = null;
            }
        }

        private void NuovoOrdine()
        {

            ScciAmbiente AmbienteOE = CoreStatics.CoreApplication.Ambiente;
            AmbienteOE.Nomepc = CoreStatics.CoreApplication.Sessione.Computer.NomeDominioCompleto;


            string sCodAziOrdine = string.Empty;
            if (CoreStatics.CoreApplication.Trasferimento.CodAziTrasferimento != null && CoreStatics.CoreApplication.Trasferimento.CodAziTrasferimento != string.Empty)
            { sCodAziOrdine = CoreStatics.CoreApplication.Trasferimento.CodAziTrasferimento; }
            else
            { sCodAziOrdine = CoreStatics.CoreApplication.Episodio.CodAzienda; }

            CoreStatics.CoreApplication.MovOrdineSelezionato = new MovOrdine(AmbienteOE,
                                                                        CoreStatics.CoreApplication.Episodio.ID,
                                                                        CoreStatics.CoreApplication.Episodio.CodTipoEpisodio,
                                                                        CoreStatics.CoreApplication.Episodio.NumeroEpisodio,
                                                                        CoreStatics.CoreApplication.Episodio.NumeroListaAttesa,
                                                                        sCodAziOrdine,
                                                                        CoreStatics.CoreApplication.Trasferimento.ID,
                                                                        CoreStatics.CoreApplication.Trasferimento.CodUO,
                                                                        CoreStatics.CoreApplication.Trasferimento.CodUA,
                                                                        CoreStatics.CoreApplication.Paziente);

            if (CoreStatics.CoreApplication.MovOrdineSelezionato.CreaOrdine())
            {
                this._aggiornagriglia = false;

                if (CoreStatics.CoreApplication.MovOrdineSelezionato != null)
                    CoreStatics.CoreApplication.MovOrdineSelezionato.SkipDatiAggiuntivi = false;

                while (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingOrdine) == DialogResult.OK)
                {

                    if (CoreStatics.CoreApplication.MovOrdineSelezionato != null && CoreStatics.CoreApplication.MovOrdineSelezionato.SkipDatiAggiuntivi)
                    {
                        break;
                    }
                    else
                    {
                        if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingOrdineDatiAggiuntivi) == DialogResult.OK) break;
                    }

                }

                if (CoreStatics.CoreApplication.MovOrdineSelezionato != null)
                    CoreStatics.CoreApplication.MovOrdineSelezionato.SkipDatiAggiuntivi = false;

                this._aggiornagriglia = true;
            }
            else
            {
                easyStatics.EasyErrorMessageBox("Errore nella creazione ordine.", @"Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CoreStatics.CoreApplication.MovOrdineSelezionato = null;
            }
        }

        private void NuovoOrdineAmbulatoriale()
        {

            this._aggiornagriglia = false;

            if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.Ambulatoriale_SelezioneUOOrdini) == DialogResult.OK)
            {
                ScciAmbiente AmbienteOE = CoreStatics.CoreApplication.Ambiente;
                AmbienteOE.Nomepc = CoreStatics.CoreApplication.Sessione.Computer.NomeDominioCompleto;

                CoreStatics.CoreApplication.MovOrdineSelezionato = new MovOrdine(AmbienteOE,
                                                            string.Empty,
                                                            "AMB",
                                                            string.Empty, string.Empty,
                                                            UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.Azienda),
                                                            string.Empty,
                                                            CoreStatics.CoreApplication.CodUOSelezionata,
                                                            CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata,
                                                            CoreStatics.CoreApplication.Paziente);



                if (CoreStatics.CoreApplication.MovOrdineSelezionato.CreaOrdine())
                {
                    if (CoreStatics.CoreApplication.MovOrdineSelezionato != null)
                        CoreStatics.CoreApplication.MovOrdineSelezionato.SkipDatiAggiuntivi = false;

                    while (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.Ambulatoriale_EditingOrdine) == DialogResult.OK)
                    {
                        if (CoreStatics.CoreApplication.MovOrdineSelezionato != null && CoreStatics.CoreApplication.MovOrdineSelezionato.SkipDatiAggiuntivi)
                        {
                            break;
                        }
                        else
                        {
                            if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.Ambulatoriale_EditingOrdineDatiAggiuntivi) == DialogResult.OK) break;
                        }
                    }

                    if (CoreStatics.CoreApplication.MovOrdineSelezionato != null)
                        CoreStatics.CoreApplication.MovOrdineSelezionato.SkipDatiAggiuntivi = false;
                }
                else
                {
                    easyStatics.EasyErrorMessageBox("Errore nella creazione ordine ambulatoriale.", @"Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    CoreStatics.CoreApplication.MovOrdineSelezionato = null;
                }

            }

            this._aggiornagriglia = true;
        }

        private void SvuotaDettaglioOrdine()
        {
            this.txtDataProgrammazione.Text = String.Empty;
            this.ucEasyGridDettaglio.DataSource = null;
            this.ucEasyGridDettaglio.Refresh();

        }

        private void SvuotaGrigliaOrdini()
        {
            this.ucEasyGrid.DataSource = null;
            this.ucEasyGrid.Refresh();

            this.SvuotaDettaglioOrdine();
        }

        #endregion

        #region Events

        private void ucEasyButtonST_Click(object sender, EventArgs e)
        {
            this._aggiornagriglia = false;

            foreach (UltraListViewItem ulvitem in this.ucEasyListBoxFiltro.Items)
            {
                ulvitem.CheckState = CheckState.Checked;
            }

            this._aggiornagriglia = true;

            this.ucEasyListBoxFiltro_ItemCheckStateChanged(ucEasyListBoxFiltro, new ItemCheckStateChangedEventArgs(this.ucEasyListBoxFiltro.Items[0]));
        }

        private void ucEasyButtonDT_Click(object sender, EventArgs e)
        {
            this._aggiornagriglia = false;

            foreach (UltraListViewItem ulvitem in this.ucEasyListBoxFiltro.Items)
            {
                ulvitem.CheckState = CheckState.Unchecked;
            }

            this._aggiornagriglia = true;

            this.ucEasyListBoxFiltro_ItemCheckStateChanged(ucEasyListBoxFiltro, new ItemCheckStateChangedEventArgs(this.ucEasyListBoxFiltro.Items[0]));
        }

        private void ucEasyGridDettaglio_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].HeaderVisible = false;
            e.Layout.Bands[0].ColHeadersVisible = false;
            e.Layout.Bands[0].RowLayoutStyle = RowLayoutStyle.GroupLayout;

            foreach (Infragistics.Win.UltraWinGrid.UltraGridColumn ocol in e.Layout.Bands[0].Columns)
            {
                switch (ocol.Key)
                {
                    case "Prestazione":
                        ocol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                        break;

                    default:
                        ocol.Width = ocol.CalculateAutoResizeWidth(Infragistics.Win.UltraWinGrid.PerformAutoSizeType.AllRowsInBand, false);
                        break;

                }
            }
            e.Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
        }

        private void ucEasyGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                int refWidth = (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XLarge) * 3;

                Graphics g = this.CreateGraphics();
                int refBtnWidth = CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(this.ucEasyGrid.DataRowFontRelativeDimension), g.DpiY) * 4;
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

                        #region formattazione colonne griglia
                        switch (oCol.Key)
                        {
                            case "DataOraProgrammata":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Red;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.CellAppearance.FontData.Bold = DefaultableBoolean.True;
                                oCol.Format = "dd/MM/yyyy HH:mm";
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 1.5);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 0;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 2;
                                oCol.RowLayoutColumnInfo.SpanY = 1;
                                break;

                            case "StatoIcona":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 0.35);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 0;
                                oCol.RowLayoutColumnInfo.OriginY = 1;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 2;
                                break;

                            case "NumeroOrdine":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 1);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 1;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 2;

                                break;

                            case "DescInserimento":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 2);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 2;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 3;

                                break;

                            case "UtenteInserimento":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 2);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 2;
                                oCol.RowLayoutColumnInfo.OriginY = 1;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "DescModifica":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 2);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 2;
                                oCol.RowLayoutColumnInfo.OriginY = 2;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "Eroganti":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = this.ucEasyGrid.Width - Convert.ToInt32(refWidth * 4.5) - Convert.ToInt32(refBtnWidth * 5.5) - 30;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 3;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 2;

                                break;

                            case "DescNumeroPrestazioni":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = this.ucEasyGrid.Width - Convert.ToInt32(refWidth * 4.5) - Convert.ToInt32(refBtnWidth * 5.5) - 30;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 3;
                                oCol.RowLayoutColumnInfo.OriginY = 2;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;
                                break;

                            case "Stato":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.FontData.Italic = Infragistics.Win.DefaultableBoolean.True;
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = refWidth;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 4;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;
                                break;

                            case "PrioritaDesc":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.FontData.Italic = Infragistics.Win.DefaultableBoolean.True;
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = refWidth;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 4;
                                oCol.RowLayoutColumnInfo.OriginY = 2;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;
                                break;

                            default:
                                oCol.Hidden = true;
                                break;
                        }

                        #endregion

                    }
                    catch (Exception)
                    {
                    }

                }

                #region pulsante modifica

                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_EDIT))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_EDIT);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
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


                    colEdit.RowLayoutColumnInfo.OriginX = 5;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;
                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_EDIT + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_EDIT + @"_SP");
                    colEdit.Hidden = false;
                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                    colEdit.CellActivation = Activation.Disabled;
                    colEdit.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
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
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
                }

                #endregion

                #region pulsante cancellazione

                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_DEL))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_DEL);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
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


                    colEdit.RowLayoutColumnInfo.OriginX = 7;
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
                    colEdit.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
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
                    colEdit.RowLayoutColumnInfo.OriginX = 8;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
                }

                #endregion

                #region pulsante copia

                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_COPY))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_COPY);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_COPIA_32);

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


                    colEdit.RowLayoutColumnInfo.OriginX = 9;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;
                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_COPY + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_COPY + @"_SP");
                    colEdit.Hidden = false;
                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                    colEdit.CellActivation = Activation.Disabled;
                    colEdit.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
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
                    colEdit.RowLayoutColumnInfo.OriginX = 10;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
                }

                #endregion

                #region pulsante visualizzazione

                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_VIEW))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_VIEW);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_VISUALIZZA_32);

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


                    colEdit.RowLayoutColumnInfo.OriginX = 11;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;
                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_VIEW + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_VIEW + @"_SP");
                    colEdit.Hidden = false;
                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                    colEdit.CellActivation = Activation.Disabled;
                    colEdit.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
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
                    colEdit.RowLayoutColumnInfo.OriginX = 12;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
                }

                #endregion

                #region pulsante inoltro

                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_VALID))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_VALID);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_FRECCIADX_32);

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


                    colEdit.RowLayoutColumnInfo.OriginX = 13;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;
                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_VALID + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_VALID + @"_SP");
                    colEdit.Hidden = false;
                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                    colEdit.CellActivation = Activation.Disabled;
                    colEdit.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
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
                    colEdit.RowLayoutColumnInfo.OriginX = 14;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
                }

                #endregion

            }
            catch
            {
            }
        }

        private void ucEasyGrid_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            try
            {
                foreach (UltraGridCell ocell in e.Row.Cells)
                {
                    try
                    {
                        if (ocell.Column.Key == "DataModifica" || ocell.Column.Key == "DataOrdine" || ocell.Column.Key == "DataOraProgrammata")
                        {
                            DateTime datacella = DateTime.Parse(ocell.Value.ToString());
                            ocell.Hidden = datacella == DateTime.MinValue;
                        }
                    }
                    catch
                    {
                        ocell.Hidden = true;
                    }

                    if (CoreStatics.CoreApplication.Cartella != null && CoreStatics.CoreApplication.Cartella.CartellaChiusa == true)
                    {
                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_EDIT)
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_DEL)
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_COPY)
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_VIEW && !bool.Parse(ocell.Row.Cells["PERMESSOVISUALIZZA"].Value.ToString()))
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_VALID)
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                    }
                    else
                    {
                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_EDIT && !bool.Parse(ocell.Row.Cells["PERMESSOMODIFICA"].Value.ToString()))
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_DEL && !bool.Parse(ocell.Row.Cells["PERMESSOCANCELLA"].Value.ToString()))
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_COPY && !bool.Parse(ocell.Row.Cells["PERMESSOCOPIA"].Value.ToString()))
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_VIEW && !bool.Parse(ocell.Row.Cells["PERMESSOVISUALIZZA"].Value.ToString()))
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_VALID && !bool.Parse(ocell.Row.Cells["PERMESSOINOLTRA"].Value.ToString()))
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                    }
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGrid_InitializeRow", this.Name);
            }
        }

        private void ucEasyGrid_ClickCellButton(object sender, CellEventArgs e)
        {
            try
            {

                switch (e.Cell.Column.Key)
                {
                    case CoreStatics.C_COL_BTN_EDIT:
                        if (e.Cell.Row.Cells["PERMESSOMODIFICA"].Value != null && (bool)e.Cell.Row.Cells["PERMESSOMODIFICA"].Value)
                        {

                            this._aggiornagriglia = false;

                            if (CoreStatics.CoreApplication.MovOrdineSelezionato != null)
                                CoreStatics.CoreApplication.MovOrdineSelezionato.SkipDatiAggiuntivi = false;

                            if (this.PercorsoAmbulatoriale)
                            {
                                while (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.Ambulatoriale_EditingOrdine) == DialogResult.OK)
                                {
                                    if (CoreStatics.CoreApplication.MovOrdineSelezionato != null && CoreStatics.CoreApplication.MovOrdineSelezionato.SkipDatiAggiuntivi)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.Ambulatoriale_EditingOrdineDatiAggiuntivi) == DialogResult.OK) break;
                                    }

                                }
                            }
                            else
                            {
                                while (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingOrdine) == DialogResult.OK)
                                {
                                    if (CoreStatics.CoreApplication.MovOrdineSelezionato != null && CoreStatics.CoreApplication.MovOrdineSelezionato.SkipDatiAggiuntivi)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingOrdineDatiAggiuntivi) == DialogResult.OK) break;
                                    }
                                }
                            }

                            this._aggiornagriglia = true;

                            if (CoreStatics.CoreApplication.MovOrdineSelezionato != null)
                            {
                                CoreStatics.CoreApplication.MovOrdineSelezionato.SkipDatiAggiuntivi = false;
                                this.CaricaGriglia(CoreStatics.CoreApplication.MovOrdineSelezionato.NumeroOrdine);
                                this.FiltraGriglia();
                                this.CaricaDettaglio();
                            }
                            else
                            {
                                this.CaricaGriglia(string.Empty);
                                this.FiltraGriglia();
                                this.CaricaDettaglio();
                            }
                        }

                        break;

                    case CoreStatics.C_COL_BTN_DEL:
                        if (e.Cell.Row.Cells["PERMESSOCANCELLA"].Value != null && (bool)e.Cell.Row.Cells["PERMESSOCANCELLA"].Value)
                        {
                            if (easyStatics.EasyMessageBox("Sei sicuro di voler CANCELLARE" + Environment.NewLine + "l'ordine selezionato ?", "Cancellazione Ordini", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {

                                bool bDelete = true;
                                if (CoreStatics.CoreApplication.MovOrdineSelezionato.StatoOrdine != OEStato.Inserito)
                                {
                                    string sMsg = @"";
                                    sMsg += @"L'ordine è già stato inoltrato";
                                    if (CoreStatics.CoreApplication.MovOrdineSelezionato.UtenteInoltro != null && CoreStatics.CoreApplication.MovOrdineSelezionato.UtenteInoltro.Trim() != "")
                                        sMsg += @" da " + CoreStatics.CoreApplication.MovOrdineSelezionato.UtenteInoltro;
                                    if (CoreStatics.CoreApplication.MovOrdineSelezionato.DataInoltro > DateTime.MinValue)
                                        sMsg += @" in data/ora " + CoreStatics.CoreApplication.MovOrdineSelezionato.DataInoltro.ToString(@"dd/MM/yyyy HH:mm");
                                    sMsg += @"." + Environment.NewLine;
                                    sMsg += @"L'operazione potrebbe non andare a buon fine o bloccare l'operatività clinica." + Environment.NewLine;
                                    sMsg += @"Sei sicuro?";
                                    if (easyStatics.EasyMessageBox(sMsg, "Cancellazione Ordini", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                                        bDelete = false;
                                }

                                if (bDelete)
                                {
                                    if (CoreStatics.CoreApplication.MovOrdineSelezionato.CancellaOrdine())
                                    {
                                        this.CaricaGriglia(string.Empty);
                                        this.FiltraGriglia();
                                        this.CaricaDettaglio();
                                    }
                                }
                            }
                        }
                        break;

                    case CoreStatics.C_COL_BTN_COPY:
                        if (e.Cell.Row.Cells["PERMESSOCOPIA"].Value != null && (bool)e.Cell.Row.Cells["PERMESSOCOPIA"].Value)
                        {
                            if (easyStatics.EasyMessageBox("Sei sicuro di voler COPIARE" + Environment.NewLine + "l'ordine selezionato ?", "Copia Ordini", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                OEOrdineTestata ordinecopia = CoreStatics.CoreApplication.MovOrdineSelezionato.CopiaOrdine3(CoreStatics.getParameterCopiaOrdine(this.PercorsoAmbulatoriale));
                                if (ordinecopia != null)
                                {

                                    this.CaricaGriglia(ordinecopia.NumeroOrdine);
                                    this.FiltraGriglia();

                                    if (ucEasyGrid.ActiveRow != null)
                                    {
                                        CellEventArgs ecopy = new CellEventArgs(ucEasyGrid.ActiveRow.Cells[CoreStatics.C_COL_BTN_EDIT]);
                                        this.ucEasyGrid_ClickCellButton(this.ucEasyGrid, ecopy);
                                    }
                                }
                            }
                        }
                        break;

                    case CoreStatics.C_COL_BTN_VIEW:
                        if (e.Cell.Row.Cells["PERMESSOVISUALIZZA"].Value != null && (bool)e.Cell.Row.Cells["PERMESSOVISUALIZZA"].Value)
                        {
                            CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.VisualizzaOrdine);
                        }
                        break;

                    case CoreStatics.C_COL_BTN_VALID:
                        if (e.Cell.Row.Cells["PERMESSOINOLTRA"].Value != null && (bool)e.Cell.Row.Cells["PERMESSOINOLTRA"].Value)
                        {
                            if (CoreStatics.CoreApplication.MovOrdineSelezionato.DataProgrammazione > new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0))
                            {
                                if (CoreStatics.CoreApplication.MovOrdineSelezionato.InoltraOrdine())
                                {
                                    if (CoreStatics.CoreApplication.MovOrdineSelezionato != null && CoreStatics.CoreApplication.MovOrdineSelezionato.StatoValiditaOrdine != OEValiditaOrdine.Valido)
                                    {
                                        easyStatics.EasyMessageBox("Impossibile inoltrare ordine, ritornato stato validazione non valido.", "Inoltro Ordine", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    }
                                    else
                                    {
                                        Risposta oRispostaElaboraPrima = PluginClientStatics.PluginClient(EnumPluginClient.OE_INOLTRA_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovOrdineSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));
                                        if (oRispostaElaboraPrima.Successo == true)
                                        {
                                        }


                                    }
                                    if (CoreStatics.CoreApplication.MovOrdineSelezionato != null)
                                    {
                                        this.CaricaGriglia(CoreStatics.CoreApplication.MovOrdineSelezionato.NumeroOrdine);
                                        this.FiltraGriglia();
                                        this.CaricaDettaglio();
                                    }
                                    else
                                    {
                                        this.CaricaGriglia(string.Empty);
                                        this.FiltraGriglia();
                                        this.CaricaDettaglio();
                                    }
                                }
                            }
                            else
                            {
                                easyStatics.EasyMessageBox("Impossibile inoltrare un ordine con data di programmazione nel passato.", "Inoltro Ordine", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            }

                        }
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

        private void ucEasyGrid_AfterRowActivate(object sender, EventArgs e)
        {
            CoreStatics.SetContesto(EnumEntita.OE, this.ucEasyGrid.ActiveRow);
            this.CaricaDettaglio();
        }

        private void drFiltro_ValueChanged(object sender, EventArgs e)
        {
            this.udteFiltroDA.Value = drFiltro.DataOraDa;
            this.udteFiltroA.Value = drFiltro.DataOraA;
        }

        private void ubApplicaFiltro_Click(object sender, EventArgs e)
        {
            if (_aggiornagriglia)
            {
                this.FiltraGriglia();
                this.CaricaDettaglio();
            }
        }

        private void uchkFiltro_Click(object sender, EventArgs e)
        {

            if (!this.uchkFiltro.Checked)
            {
                this.InizializzaFiltri();

                this._aggiornagriglia = false;

                foreach (Infragistics.Win.UltraWinListView.UltraListViewItem oitem in this.ucEasyListBoxFiltro.Items)
                {
                    oitem.CheckState = CheckState.Checked;
                }

                this._aggiornagriglia = true;

                this.CaricaGriglia(string.Empty);
                this.FiltraGriglia();
                this.CaricaDettaglio();
            }
            else
                this.uchkFiltro.Checked = !this.uchkFiltro.Checked;
        }

        private void ubAdd_Click(object sender, EventArgs e)
        {

            try
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

                if (this.PercorsoAmbulatoriale)
                    this.NuovoOrdineAmbulatoriale();
                else
                    this.NuovoOrdine();

                if (CoreStatics.CoreApplication.MovOrdineSelezionato != null)
                {
                    this.CaricaGriglia(CoreStatics.CoreApplication.MovOrdineSelezionato.NumeroOrdine);
                    this.FiltraGriglia();
                    this.CaricaDettaglio();
                }
                else
                {
                    this.CaricaGriglia(string.Empty);
                    this.FiltraGriglia();
                    this.CaricaDettaglio();
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ubAdd_Click", this.Name);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
            }
        }

        private void ucOrderEntry_Enter(object sender, EventArgs e)
        {
            try
            {
                this.ucEasyGrid.PerformLayout();
            }
            catch (Exception)
            {
            }
        }

        private void ucEasyListBoxFiltro_ItemCheckStateChanged(object sender, ItemCheckStateChangedEventArgs e)
        {
            if (_aggiornagriglia)
            {
                this.FiltraGriglia();
                this.CaricaDettaglio();
            }
        }

        private void txtFiltroErogante_ValueChanged(object sender, EventArgs e)
        {
        }

        #endregion

    }
}
