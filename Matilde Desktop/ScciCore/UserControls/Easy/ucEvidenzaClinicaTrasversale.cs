using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Globalization;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.ScciResource;

using Infragistics.Win.UltraWinGrid;

namespace UnicodeSrl.ScciCore
{
    public partial class ucEvidenzaClinicaTrasversale : UserControl, Interfacce.IViewUserControlMiddle
    {
        public ucEvidenzaClinicaTrasversale()
        {
            InitializeComponent();
            _ucc = (UserControl)this;
        }

        #region Declare

        private UserControl _ucc = null;
        private ucRichTextBox _ucRichTextBox = null;
        Graphics g = null;
        private Dictionary<int, byte[]> oIcone = new Dictionary<int, byte[]>();

        private bool _bRicerca = false;
        private bool _disableClick = false;

        private bool _editRefertoDaGrid = false;


        private DataTable _dtTipiEvidenzaClinica = null;


        #endregion

        #region Interface

        public void Aggiorna()
        {
            if (!_editRefertoDaGrid) this.AggiornaGriglia(true);
            setFocusDefault();
        }

        public void Carica()
        {

            try
            {
                _editRefertoDaGrid = false;
                _disableClick = false;
                this.InizializzaControlli();
                this.InizializzaFiltri();

                this.ubCartella.Enabled = false;

                _bRicerca = true;

                this.AggiornaGriglia();

                setFocusDefault();

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

                oIcone = new Dictionary<int, byte[]>();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Ferma", this.Name);
            }

        }

        #endregion

        #region private functions

        private void InizializzaControlli()
        {

            try
            {

                CoreStatics.SetEasyUltraDockManager(ref this.ultraDockManager);
                CoreStatics.SetEasyUltraGridLayout(ref this.ucEasyGrid);

                this.ubCartella.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_CARTELLACLINICA_256);
                this.ubCartella.PercImageFill = 0.75F;
                this.ubCartella.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.chkSoloPazientiSeguiti.Visible = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.PazientiSeguiti_Visualizza);
                this.lblSoloPazientiSeguiti.Visible = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.PazientiSeguiti_Visualizza);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaControlli", this.Name);
            }
        }

        private void InizializzaFiltri()
        {

            try
            {

                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);

                op.Parametro.Add("DatiEstesi", "1");
                op.Parametro.Add("EvidenzaClinicaTrasversale", "1");

                op.TimeStamp.CodEntita = EnumEntita.EVC.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_SelMovEvidenzaClinica", spcoll);

                if (ds != null && ds.Tables.Count > 1) ds.Tables[1].DefaultView.Sort = "DescTipo";
                if (ds != null && ds.Tables.Count > 2) ds.Tables[2].DefaultView.Sort = "DescUA";
                if (ds != null && ds.Tables.Count > 3) ds.Tables[3].DefaultView.Sort = "DescUO";


                _dtTipiEvidenzaClinica = ds.Tables[1].Copy();
                this.ucEasyGridFiltroTipo.DataSource = DBUtils.getDatatableTipiEVCRaggruppato(ref _dtTipiEvidenzaClinica, true);

                this.ucEasyGridFiltroTipo.Refresh();
                if (this.ucEasyGridFiltroTipo.Rows.Count > 0)
                {
                    this.ucEasyGridFiltroTipo.ActiveRow = null;
                    this.ucEasyGridFiltroTipo.Selected.Rows.Clear();
                    CoreStatics.SelezionaRigaInGriglia(ref ucEasyGridFiltroTipo, "DescTipo", CoreStatics.GC_TUTTI);
                }

                this.ucEasyGridFiltroUO.DataSource = CoreStatics.AggiungiTuttiDataTable(ds.Tables[3], true);
                this.ucEasyGridFiltroUO.Refresh();
                if (this.ucEasyGridFiltroUO.Rows.Count > 0)
                {
                    this.ucEasyGridFiltroUO.ActiveRow = null;
                    this.ucEasyGridFiltroUO.Selected.Rows.Clear();
                    CoreStatics.SelezionaRigaInGriglia(ref ucEasyGridFiltroUO, "CodUO", CoreStatics.GC_TUTTI);
                }

                this.ucEasyGridFiltroUA.DataSource = CoreStatics.AggiungiTuttiDataTable(ds.Tables[2], true);
                this.ucEasyGridFiltroUA.Refresh();
                if (this.ucEasyGridFiltroUA.Rows.Count > 0)
                {
                    this.ucEasyGridFiltroUA.ActiveRow = null;
                    this.ucEasyGridFiltroUA.Selected.Rows.Clear();
                    CoreStatics.SelezionaRigaInGriglia(ref ucEasyGridFiltroUA, "CodUA", CoreStatics.GC_TUTTI);
                }

                this.ubApplicaFiltro.TextFontRelativeDimension = easyStatics.easyRelativeDimensions.Large;
                this.drFiltro.Value = null;
                this.drFiltro.DateFuture = true;
                this.udteFiltroDA.Value = null;
                this.udteFiltroA.Value = null;
                this.chkSoloDefinitivi.Checked = true;
                this.chkSoloDaVistare.Checked = true;
                this.chkSoloPazientiSeguiti.Checked = false;
                this.drFiltro.Value = ucEasyDateRange.C_RNG_24H;


                if (this.ucEasyComboEditorFiltriSpeciali.Items.Count <= 0)
                {
                    op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                    op.Parametro.Add("CodTipoFiltroSpeciale", EnumTipoFiltroSpeciale.EVCTRA.ToString());

                    spcoll = new SqlParameterExt[1];

                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    DataTable dt = Database.GetDataTableStoredProc("MSP_SelFiltriSpeciali", spcoll);

                    this.ucEasyComboEditorFiltriSpeciali.ValueMember = "Codice";
                    this.ucEasyComboEditorFiltriSpeciali.DisplayMember = "Descrizione";
                    this.ucEasyComboEditorFiltriSpeciali.DataSource = CoreStatics.AggiungiTuttiDataTable(dt, false);
                    this.ucEasyComboEditorFiltriSpeciali.Refresh();
                }
                this.ucEasyComboEditorFiltriSpeciali.SelectedIndex = 0;



            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaFiltri", this.Name);
            }

        }

        private void AggiornaGriglia(bool skipEasyGrid_Refresh = false)
        {
            CoreStatics.SetNavigazione(false);

            if (_bRicerca == true)
            {

                string codtiposelezionato = string.Empty;
                string codUOselezionato = string.Empty;
                string codUAselezionata = string.Empty;

                try
                {

                    CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);
                    CoreStatics.SetContesto(EnumEntita.EVC, null);

                    DataSet ds = GetEvidenzaClinicaDataset();

                    DataTable dtEdit = ds.Tables[0].Copy();

                    DataColumn colsp = new DataColumn(CoreStatics.C_COL_SPAZIO, typeof(string));
                    colsp.AllowDBNull = true;
                    colsp.DefaultValue = "";
                    colsp.Unique = false;
                    dtEdit.Columns.Add(colsp);

                    foreach (DataColumn dcCol in dtEdit.Columns)
                    {
                        if (dcCol.ColumnName.ToUpper().IndexOf("PERMESSOVISTA") == 0 ||
                            dcCol.ColumnName.ToUpper().IndexOf("PERMESSOGRAFICO") == 0 ||
                             dcCol.ColumnName.ToUpper().IndexOf("ICONA") == 0)
                            dcCol.ReadOnly = false;
                    }

                    this.ucEasyGrid.DataSource = null;
                    this.ucEasyGrid.DataSource = dtEdit;

                    if (!skipEasyGrid_Refresh) this.ucEasyGrid.Refresh();


                    if (g == null) g = this.CreateGraphics();
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
                    CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
                    CoreStatics.ExGest(ref ex, @"Errore Accesso Data Warehouse." + Environment.NewLine + @"Contattare amministratori di sistema.", "AggiornaGriglia", this.Name);
                }
                finally
                {
                    CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
                }

            }

            CoreStatics.SetNavigazione(true);
            _disableClick = false;


        }

        private void AggiornaGrigliaMantenendoPosizione(UltraGridCell eCell, bool pulisciAggiornaContesto)
        {

            string sIDMovEvidenzaClinicaSelezionato = "";
            string sIDEpisodioSelezionato = "";
            string sIDPazienteSelezionato = "";
            string sNextIDMovEvidenzaClinica = "";
            int iRowIndex = eCell.Row.Index;
            if (iRowIndex < eCell.Row.ParentRow.ChildBands[0].Rows.Count - 1)
            {
                sNextIDMovEvidenzaClinica = eCell.Row.ParentRow.ChildBands[0].Rows[iRowIndex + 1].Cells["IDSCCI"].Text;
            }

            if (sNextIDMovEvidenzaClinica.Trim() == "")
            {
                if (iRowIndex > 0)
                {
                    sNextIDMovEvidenzaClinica = eCell.Row.ParentRow.ChildBands[0].Rows[iRowIndex - 1].Cells["IDSCCI"].Text;
                }
            }

            if (sNextIDMovEvidenzaClinica.Trim() == "")
            {
                iRowIndex = eCell.Row.ParentRow.Index;
                if (iRowIndex < eCell.Row.ParentRow.Band.Layout.Grid.Rows.Count - 1)
                {
                    sNextIDMovEvidenzaClinica = eCell.Row.ParentRow.Band.Layout.Grid.Rows[iRowIndex + 1].ChildBands[0].Rows[0].Cells["IDSCCI"].Text;
                }
                else if (iRowIndex > 0)
                {
                    sNextIDMovEvidenzaClinica = eCell.Row.ParentRow.Band.Layout.Grid.Rows[iRowIndex - 1].ChildBands[0].Rows[0].Cells["IDSCCI"].Text;
                }
            }

            if (CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato != null)
            {
                sIDMovEvidenzaClinicaSelezionato = CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.IDMovEvidenzaClinica;
                sIDEpisodioSelezionato = CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.IDEpisodio;
                sIDPazienteSelezionato = CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.IDPaziente;
            }

            if (pulisciAggiornaContesto) PulisciAggiornaContesto();

            AggiornaGriglia();

            if (sIDMovEvidenzaClinicaSelezionato != null && sIDMovEvidenzaClinicaSelezionato.Trim() != "")
            {
                RowsCollection rows = this.ucEasyGrid.Rows;
                if (!CoreStatics.SelezionaRigaInGriglia(ref rows, "IDSCCI", sIDMovEvidenzaClinicaSelezionato))
                {

                    if (sNextIDMovEvidenzaClinica.Trim() == "" || !CoreStatics.SelezionaRigaInGriglia(ref rows, "IDSCCI", sNextIDMovEvidenzaClinica))
                    {
                        if (!CoreStatics.SelezionaRigaInGriglia(ref rows, "IDEpisodio", sIDEpisodioSelezionato))
                        {
                            if (!CoreStatics.SelezionaRigaInGriglia(ref rows, "IDPaziente", sIDPazienteSelezionato))
                            {
                                if (sNextIDMovEvidenzaClinica != "")
                                    CoreStatics.SelezionaRigaInGriglia(ref rows, "IDSCCI", sNextIDMovEvidenzaClinica);
                            }
                        }
                    }

                }
            }

            if (pulisciAggiornaContesto && CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato != null) CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato = null;

        }

        private void PulisciAggiornaContesto()
        {
            try
            {

                CoreStatics.CoreApplication.Paziente = null;
                CoreStatics.CoreApplication.Episodio = null;
                CoreStatics.CoreApplication.Trasferimento = null;
                CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato = null;
                CoreStatics.CoreApplication.DefinizioneGraficoSelezionata = null;
                CoreStatics.CoreApplication.Cartella = null;

                CoreStatics.RefreshUcTop();
            }
            catch
            {
            }
        }

        private DataSet GetEvidenzaClinicaDataset()
        {

            DataSet dsReturn = CreaEvidenzaClinicaDataset();

            List<string> tipi = new List<string>();

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);

                op.Parametro.Add("DatiEstesi", "0");
                op.Parametro.Add("EvidenzaClinicaTrasversale", "1");

                op.TimeStamp.CodEntita = EnumEntita.EVC.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                if (this.txtRicerca.Text != string.Empty)
                {

                    string filtrogenerico = string.Empty;

                    string[] ricerche = this.txtRicerca.Text.Split(' ');
                    foreach (string ricerca in ricerche)
                    {

                        string format = "dd/MM/yyyy";
                        DateTime dateTime;
                        if (DateTime.TryParseExact(ricerca, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                        {
                            op.Parametro.Add("DataNascita", ricerca);
                        }
                        else
                        {
                            filtrogenerico += ricerca + " ";
                        }

                    }

                    op.Parametro.Add("FiltroGenerico", filtrogenerico);

                }

                if (this.udteFiltroDA.Value != null)
                {
                    op.Parametro.Add("DataInizio", Database.dataOra105PerParametri((DateTime)this.udteFiltroDA.Value));
                }
                if (this.udteFiltroA.Value != null)
                {
                    op.Parametro.Add("DataFine", Database.dataOra105PerParametri((DateTime)this.udteFiltroA.Value));
                }

                tipi = getActiveRowCodiciTipi();
                if (tipi != null && tipi.Count > 0) op.ParametroRipetibile.Add("CodTipoEvidenzaClinica", tipi.ToArray());

                if (this.ucEasyGridFiltroUO.ActiveRow != null && this.ucEasyGridFiltroUO.ActiveRow.Cells["CodUO"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                {
                    op.Parametro.Add("CodUO", this.ucEasyGridFiltroUO.ActiveRow.Cells["CodUO"].Text);
                }
                if (this.ucEasyGridFiltroUA.ActiveRow != null && this.ucEasyGridFiltroUA.ActiveRow.Cells["CodUA"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                {
                    op.Parametro.Add("CodUA", this.ucEasyGridFiltroUA.ActiveRow.Cells["CodUA"].Text);
                }

                if (this.chkSoloDefinitivi.Checked)
                {
                    op.Parametro.Add("CodStatoEvidenzaClinica", "CM");
                }

                if (this.chkSoloDaVistare.Checked)
                {
                    op.Parametro.Add("CodStatoEvidenzaClinicaVisione", "DV");
                }

                op.Parametro.Add("SoloPazientiSeguiti", (this.chkSoloPazientiSeguiti.Checked == false ? "0" : "1"));

                if (this.ucEasyComboEditorFiltriSpeciali.Text.Trim() != CoreStatics.GC_TUTTI)
                {
                    op.Parametro.Add("CodFiltroSpeciale", this.ucEasyComboEditorFiltriSpeciali.Value.ToString());
                }

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet dsOrigine = Database.GetDatasetStoredProc("MSP_SelMovEvidenzaClinica", spcoll);

                if (dsOrigine != null && dsOrigine.Tables.Count > 0)
                {
                    foreach (DataRow drOrigine in dsOrigine.Tables[0].Rows)
                    {

                        DataRow drReferto = dsReturn.Tables[0].NewRow();

                        drReferto["IDSCCI"] = drOrigine["ID"];
                        drReferto["IDPaziente"] = drOrigine["IDPaziente"];
                        drReferto["IDEpisodio"] = drOrigine["IDEpisodio"];
                        drReferto["IDTrasferimento"] = drOrigine["IDTrasferimento"];

                        drReferto["DescrPaziente"] = drOrigine["DescrPaziente"];
                        drReferto["DataReferto"] = drOrigine["DataEvento"];
                        drReferto["Anteprima"] = drOrigine["Anteprima"];

                        drReferto["CodTipo"] = drOrigine["CodTipo"];
                        drReferto["DescrTipo"] = drOrigine["DescrTipo"];

                        drReferto["CodStato"] = drOrigine["CodStato"];
                        drReferto["DescrStato"] = drOrigine["DescrStato"];

                        drReferto["CodStatoVisione"] = drOrigine["CodStatoVisione"];
                        drReferto["DescrStatoVisione"] = drOrigine["DescrStatoVisione"];

                        drReferto["CodUtenteVisione"] = drOrigine["CodUtenteVisione"];
                        drReferto["DescrUtenteVisione"] = drOrigine["DescrUtenteVisione"];
                        drReferto["DataVisione"] = drOrigine["DataVisione"];

                        drReferto["PermessoVista"] = drOrigine["PermessoVista"];
                        drReferto["PermessoGrafico"] = drOrigine["PermessoGrafico"];

                        drReferto["EsistePDFDWH"] = drOrigine["EsistePDFDWH"];

                        drReferto["PDFDWH"] = drOrigine["PDFDWH"];
                        drReferto["IDIcona"] = drOrigine["IDIcona"];

                        drReferto["DataEventoDWH"] = drOrigine["DataEventoDWH"];

                        if (drOrigine.IsNull("DataEventoDWH"))
                        {
                            drReferto["DataEventoDWHGriglia"] = drOrigine["DataEvento"];
                        }
                        else
                        {
                            drReferto["DataEventoDWHGriglia"] = drOrigine["DataEventoDWH"];
                        }

                        dsReturn.Tables[0].Rows.Add(drReferto);

                    }

                }

                if (dsReturn != null && dsReturn.Tables.Count > 0) dsReturn.Tables[0].DefaultView.Sort = "DataReferto DESC";

            }
            catch (Exception)
            {
                throw;
            }

            return dsReturn;

        }

        private DataSet CreaEvidenzaClinicaDataset()
        {

            DataSet dsReturn = new DataSet();

            try
            {

                DataTable dtReferti = new DataTable("tableReferti");
                dtReferti.Columns.Add("IDSCCI", typeof(Guid));
                dtReferti.Columns.Add("IDPaziente", typeof(Guid));
                dtReferti.Columns.Add("IDEpisodio", typeof(Guid));
                dtReferti.Columns.Add("IDTrasferimento", typeof(Guid));
                dtReferti.Columns.Add("IDRefertoDWH", typeof(string));
                dtReferti.Columns.Add("DescrPaziente", typeof(string));
                dtReferti.Columns.Add("DataReferto", typeof(DateTime));
                dtReferti.Columns.Add("Anteprima", typeof(string));

                dtReferti.Columns.Add("CodTipo", typeof(string));
                dtReferti.Columns.Add("DescrTipo", typeof(string));

                dtReferti.Columns.Add("CodStato", typeof(string));
                dtReferti.Columns.Add("DescrStato", typeof(string));

                dtReferti.Columns.Add("CodStatoVisione", typeof(string));
                dtReferti.Columns.Add("DescrStatoVisione", typeof(string));

                dtReferti.Columns.Add("CodUtenteVisione", typeof(string));
                dtReferti.Columns.Add("DescrUtenteVisione", typeof(string));
                dtReferti.Columns.Add("DataVisione", typeof(DateTime));

                dtReferti.Columns.Add("PermessoVista", typeof(int));
                dtReferti.Columns.Add("PermessoGrafico", typeof(int));

                dtReferti.Columns.Add("EsistePDFDWH", typeof(int));

                dtReferti.Columns.Add("PDFDWH", typeof(byte[]));
                dtReferti.Columns.Add("Icona", typeof(byte[]));
                dtReferti.Columns.Add("IDIcona", typeof(int));

                dtReferti.Columns.Add("DataEventoDWH", typeof(DateTime));

                dtReferti.Columns.Add("DataEventoDWHGriglia", typeof(DateTime));

                dsReturn.Tables.Add(dtReferti);

            }
            catch (Exception)
            {
                throw;
            }

            return dsReturn;

        }

        private void AbilitaPulsanteCartella()
        {

            try
            {

                if (this.ucEasyGrid.ActiveRow != null && this.ucEasyGrid.ActiveRow.IsDataRow)
                {
                    this.ubCartella.Enabled = true;
                }
                else
                {
                    this.ubCartella.Enabled = false;
                }

            }
            catch (Exception)
            {
            }

        }

        internal void setFocusDefault()
        {
            try
            {
                this.txtRicerca.Focus();
                this.txtRicerca.SelectAll();
            }
            catch
            {
            }
        }

        #endregion

        #region Nuova Gestione Filtro Tipo

        private List<string> getActiveRowCodiciTipi()
        {
            List<string> lstRet = new List<string>();

            if (this.ucEasyGridFiltroTipo.ActiveRow != null
                && this.ucEasyGridFiltroTipo.ActiveRow.IsDataRow
                && !this.ucEasyGridFiltroTipo.ActiveRow.IsFilteredOut
                && this.ucEasyGridFiltroTipo.ActiveRow.Cells["DescTipo"].Text.Trim().ToUpper() != CoreStatics.GC_TUTTI.Trim().ToUpper()
                && _dtTipiEvidenzaClinica != null
                && _dtTipiEvidenzaClinica.Rows.Count > 0)
            {
                _dtTipiEvidenzaClinica.DefaultView.RowFilter = @"DescTipo = '" + Database.testoSQL(this.ucEasyGridFiltroTipo.ActiveRow.Cells["DescTipo"].Text) + @"'";
                if (_dtTipiEvidenzaClinica.DefaultView.Count > 0)
                {
                    foreach (DataRowView drv in _dtTipiEvidenzaClinica.DefaultView)
                    {
                        if (!lstRet.Contains(drv["CodTipo"].ToString())) lstRet.Add(drv["CodTipo"].ToString());
                    }
                }
                _dtTipiEvidenzaClinica.DefaultView.RowFilter = "";
            }

            return lstRet;
        }

        #endregion

        #region Events

        private void ultraDockManager_InitializePane(object sender, Infragistics.Win.UltraWinDock.InitializePaneEventArgs e)
        {
            e.Pane.Settings.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large);
            e.Pane.Settings.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FILTRO_32);

            int filtroWidth = 40 * (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large);
            this.ultraDockManager.ControlPanes[0].FlyoutSize = new Size(filtroWidth, this.ultraDockManager.ControlPanes[0].FlyoutSize.Height);
            this.ultraDockManager.ControlPanes[0].Size = new Size(filtroWidth, this.ultraDockManager.ControlPanes[0].Size.Height);
            this.ultraDockManager.DockAreas[0].Size = new Size(filtroWidth, this.ultraDockManager.DockAreas[0].Size.Height);
            this.pnlFiltro.Width = filtroWidth;
        }

        private void txtRicerca_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.AggiornaGriglia();
                    setFocusDefault();
                }
            }
            catch (Exception)
            {
            }
        }

        private void ucEasyGridFiltroTipo_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].HeaderVisible = false;
            foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
            {
                switch (oCol.Key)
                {
                    case "DescTipo":
                        oCol.Hidden = false;
                        oCol.Header.Caption = @"Tipo";
                        break;
                    default:
                        oCol.Hidden = true;
                        break;
                }
            }
        }

        private void ucEasyGridFiltroUO_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].HeaderVisible = false;
            foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
            {
                switch (oCol.Key)
                {
                    case "DescUO":
                        oCol.Hidden = false;
                        oCol.Header.Caption = @"Unità Operativa";
                        break;
                    default:
                        oCol.Hidden = true;
                        break;
                }
            }
        }

        private void ucEasyGridFiltroUA_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].HeaderVisible = false;
            foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
            {
                switch (oCol.Key)
                {
                    case "DescUA":
                        oCol.Hidden = false;
                        oCol.Header.Caption = @"Struttura";
                        break;
                    default:
                        oCol.Hidden = true;
                        break;
                }
            }
        }

        private void drFiltro_ValueChanged(object sender, EventArgs e)
        {
            this.udteFiltroDA.Value = drFiltro.DataOraDa;
            this.udteFiltroA.Value = drFiltro.DataOraA;
        }

        private void ubApplicaFiltro_Click(object sender, EventArgs e)
        {

            if (this.chkSoloDaVistare.Checked == true || (this.udteFiltroDA.Value != null && this.udteFiltroA.Value != null))
            {


                if (this.ultraDockManager.FlyoutPane != null && !this.ultraDockManager.FlyoutPane.Pinned) this.ultraDockManager.FlyIn();
                this.AggiornaGriglia();
                setFocusDefault();
            }
            else
            {
                easyStatics.EasyMessageBox(@"Impostare un intervallo di date!", "Evidenza Clinica Trasversale", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.drFiltro.Focus();
            }

        }

        private void ucEasyGrid_AfterRowActivate(object sender, EventArgs e)
        {
            AbilitaPulsanteCartella();
        }

        private void ucEasyGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {

                g = this.CreateGraphics();
                int refWidth = CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(this.ucEasyGrid.DataRowFontRelativeDimension), g.DpiY) * 4;

                int refBtnWidth = CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(this.ucEasyGrid.DataRowFontRelativeDimension), g.DpiY) * 4;
                g.Dispose();
                g = null;
                e.Layout.Bands[0].HeaderVisible = false;
                e.Layout.Bands[0].ColHeadersVisible = false;
                e.Layout.Bands[0].RowLayoutStyle = RowLayoutStyle.GroupLayout;

                e.Layout.Override.RowSizing = RowSizing.Fixed;
                e.Layout.Override.RowSizingArea = RowSizingArea.EntireRow;
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
                                oCol.RowLayoutColumnInfo.SpanY = 3;

                                break;

                            case "DataReferto":
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
                                oCol.Format = "dd/MM/yyyy";
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 1.5);
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

                            case "DataEventoDWH":
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

                                oCol.RowLayoutColumnInfo.OriginX = 2;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "DescrUtenteVisione":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 3.5);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 1;
                                oCol.RowLayoutColumnInfo.SpanX = 2;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "DataVisione":
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
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 3.5);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }
                                oCol.RowLayoutColumnInfo.WeightY = 1;
                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 2;
                                oCol.RowLayoutColumnInfo.SpanX = 2;
                                oCol.RowLayoutColumnInfo.SpanY = 1;
                                break;

                            case "Anteprima":
                                oCol.Hidden = false;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                try
                                {
                                    oCol.MaxWidth = this.ucEasyGrid.Width - Convert.ToInt32(refWidth * 4.6) - Convert.ToInt32(refBtnWidth * 4.85) - 30;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 3;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 3;

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


                    colEdit.RowLayoutColumnInfo.OriginX = 4;
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
                    colEdit.RowLayoutColumnInfo.OriginX = 5;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
                }

                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_VIEW))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_VIEW);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_EVIDENZACLINICA_32);


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
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_VIEW + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_VIEW + @"_SP");
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
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
                }

                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_REFERTPDF))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_REFERTPDF);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_DOWNLOADDOCUMENTO_32);


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
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_REFERTPDF + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_REFERTPDF + @"_SP");
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

                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_GRAPH))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_GRAPH);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_PARAMETRIVITALIGRAFICO_32);

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


                    colEdit.RowLayoutColumnInfo.OriginX = 10;
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
                    colEdit.RowLayoutColumnInfo.OriginX = 11;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
                }

            }
            catch (Exception)
            {
            }

        }

        private void ucEasyGrid_InitializeRow(object sender, InitializeRowEventArgs e)
        {

            try
            {

                if (e.Row.Cells.Exists("Icona") && e.Row.Cells.Exists("IDIcona") && e.Row.Cells["IDIcona"].Text.ToString() != string.Empty && e.Row.Cells["IDIcona"].Text.ToString() != "")
                {
                    if (!oIcone.ContainsKey(Convert.ToInt32(e.Row.Cells["IDIcona"].Value)))
                    {
                        oIcone.Add(Convert.ToInt32(e.Row.Cells["IDIcona"].Value), CoreStatics.GetImageForGrid(Convert.ToInt32(e.Row.Cells["IDIcona"].Value), 256));
                    }
                    byte[] icona = oIcone[Convert.ToInt32(e.Row.Cells["IDIcona"].Value)];
                    if (icona != null)
                    {
                        e.Row.Cells["Icona"].Value = icona;
                        e.Row.Update();
                    }
                }

                foreach (UltraGridCell ocell in e.Row.Cells)
                {
                    if (ocell.Column.Key.ToUpper().IndexOf("ICON") == 0 && e.Row.Cells.Exists("DescrTipo"))
                    {
                        ocell.ToolTipText = e.Row.Cells["DescrTipo"].Text;
                        if (e.Row.Cells.Exists("DescrStato")) ocell.ToolTipText += @" - " + e.Row.Cells["DescrStato"].Text;
                    }

                    if (ocell.Column.Key == CoreStatics.C_COL_BTN_VALID && ocell.Row.Cells["PermessoVista"].Value.ToString() == "0")
                        ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                    if (ocell.Column.Key == CoreStatics.C_COL_BTN_GRAPH && ocell.Row.Cells["PermessoGrafico"].Value.ToString() == "0")
                        ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGrid_InitializeRow", this.Name);
            }

        }

        private void ucEasyGrid_ClickCellButton(object sender, CellEventArgs e)
        {
            if (!_disableClick)
            {
                try
                {
                    _editRefertoDaGrid = true;

                    switch (e.Cell.Column.Key)
                    {
                        case CoreStatics.C_COL_BTN_VALID:
                            if (e.Cell.Row.Cells["PermessoVista"].Text == "1")
                            {
                                _disableClick = true;


                                CoreStatics.CoreApplication.Paziente = new Paziente(e.Cell.Row.Cells["IDPaziente"].Text, e.Cell.Row.Cells["IDEpisodio"].Text);
                                CoreStatics.CoreApplication.Episodio = new Episodio(e.Cell.Row.Cells["IDEpisodio"].Text);
                                CoreStatics.CoreApplication.Trasferimento = new Trasferimento(e.Cell.Row.Cells["IDTrasferimento"].Text, CoreStatics.CoreApplication.Ambiente);

                                CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato = new MovEvidenzaClinica(e.Cell.Row.Cells["IDSCCI"].Text, e.Cell.Row.Cells["IDRefertoDWH"].Text.Trim(), EnumAzioni.VAL);
                                if (CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.Vista())
                                {

                                    AggiornaGrigliaMantenendoPosizione(e.Cell, true);

                                }
                                CoreStatics.CoreApplication.Paziente = null;
                                CoreStatics.CoreApplication.Episodio = null;
                                CoreStatics.CoreApplication.Trasferimento = null;
                            }
                            break;

                        case CoreStatics.C_COL_BTN_VIEW:
                            bool bSkipLoadEpi = false;
                            if (e.Cell.Row.Cells["PermessoGrafico"].Text == "1")
                            {
                                bSkipLoadEpi = true;
                                CoreStatics.CoreApplication.Paziente = new Paziente(e.Cell.Row.Cells["IDPaziente"].Text, e.Cell.Row.Cells["IDEpisodio"].Text);
                                CoreStatics.CoreApplication.Episodio = new Episodio(e.Cell.Row.Cells["IDEpisodio"].Text);
                                CoreStatics.CoreApplication.Trasferimento = new Trasferimento(e.Cell.Row.Cells["IDTrasferimento"].Text, CoreStatics.CoreApplication.Ambiente);
                                if (CoreStatics.CoreApplication.Trasferimento != null && CoreStatics.CoreApplication.Trasferimento.IDCartella != "")
                                    CoreStatics.CoreApplication.Cartella = new Cartella(CoreStatics.CoreApplication.Trasferimento.IDCartella, "", CoreStatics.CoreApplication.Ambiente);

                                string idEpisodio = (CoreStatics.CoreApplication.Episodio != null ? CoreStatics.CoreApplication.Episodio.ID : "");
                                DateTime datada = (this.udteFiltroDA.Value != null ? (DateTime)this.udteFiltroDA.Value : DateTime.MinValue);
                                DateTime dataa = (this.udteFiltroA.Value != null ? (DateTime)this.udteFiltroA.Value : DateTime.MinValue);
                                DateTime dataricovero = (CoreStatics.CoreApplication.Episodio != null ? CoreStatics.CoreApplication.Episodio.DataRicovero : DateTime.MinValue);

                                dataa = DateTime.MinValue;
                                datada = DateTime.MinValue;

                                CoreStatics.CoreApplication.DefinizioneGraficoSelezionata = new ToolboxPerGrafici(idEpisodio, dataricovero, CoreStatics.CoreApplication.Paziente.CodSAC, EnumEntita.EVC, "", datada, dataa);

                            }

                            if (e.Cell.Row.Cells["IDSCCI"].Text.Trim() != "")
                            {
                                _disableClick = true;
                                if (!bSkipLoadEpi)
                                {
                                    CoreStatics.CoreApplication.Paziente = new Paziente(e.Cell.Row.Cells["IDPaziente"].Text, e.Cell.Row.Cells["IDEpisodio"].Text);
                                    CoreStatics.CoreApplication.Episodio = new Episodio(e.Cell.Row.Cells["IDEpisodio"].Text);
                                    CoreStatics.CoreApplication.Trasferimento = new Trasferimento(e.Cell.Row.Cells["IDTrasferimento"].Text, CoreStatics.CoreApplication.Ambiente);
                                    if (CoreStatics.CoreApplication.Trasferimento != null && CoreStatics.CoreApplication.Trasferimento.IDCartella != "")
                                        CoreStatics.CoreApplication.Cartella = new Cartella(CoreStatics.CoreApplication.Trasferimento.IDCartella, "", CoreStatics.CoreApplication.Ambiente);
                                }

                                CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato = new MovEvidenzaClinica(e.Cell.Row.Cells["IDSCCI"].Text, e.Cell.Row.Cells["IDRefertoDWH"].Text.Trim(), EnumAzioni.VIS);
                                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingEvidenzaClinica) == DialogResult.OK)
                                {
                                    CoreStatics.CoreApplication.Paziente = null;
                                    CoreStatics.CoreApplication.Episodio = null;
                                    CoreStatics.CoreApplication.Trasferimento = null;

                                    AggiornaGrigliaMantenendoPosizione(e.Cell, true);

                                }
                            }
                            else
                            {


                                _disableClick = true;
                                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);
                                CoreStatics.CoreApplication.Paziente = new Paziente(e.Cell.Row.Cells["IDPaziente"].Text, e.Cell.Row.Cells["IDEpisodio"].Text);
                                CoreStatics.CoreApplication.Episodio = new Episodio(e.Cell.Row.Cells["IDEpisodio"].Text);
                                CoreStatics.CoreApplication.Trasferimento = new Trasferimento(e.Cell.Row.Cells["IDTrasferimento"].Text, CoreStatics.CoreApplication.Ambiente);
                                if (CoreStatics.CoreApplication.Trasferimento != null && CoreStatics.CoreApplication.Trasferimento.IDCartella != "")
                                    CoreStatics.CoreApplication.Cartella = new Cartella(CoreStatics.CoreApplication.Trasferimento.IDCartella, "", CoreStatics.CoreApplication.Ambiente);

                                CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato = new MovEvidenzaClinica(e.Cell.Row.Cells["IDRefertoDWH"].Text, e.Cell.Row.Cells["CodTipo"].Text, e.Cell.Row.Cells["DescrTipo"].Text, e.Cell.Row.Cells["CodStato"].Text, e.Cell.Row.Cells["DescrStato"].Text, e.Cell.Row.Cells["Anteprima"].Text, (DateTime)e.Cell.Row.Cells["DataReferto"].Value, (DateTime)e.Cell.Row.Cells["DataEventoDWH"].Value);
                                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
                                CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingEvidenzaClinica);

                            }
                            CoreStatics.CoreApplication.Paziente = null;
                            CoreStatics.CoreApplication.Episodio = null;
                            CoreStatics.CoreApplication.Trasferimento = null;
                            CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato = null;
                            CoreStatics.CoreApplication.DefinizioneGraficoSelezionata = null;
                            CoreStatics.CoreApplication.Cartella = null;
                            break;

                        case CoreStatics.C_COL_BTN_REFERTPDF:

                            CoreStatics.CoreApplication.Paziente = new Paziente(e.Cell.Row.Cells["IDPaziente"].Text, e.Cell.Row.Cells["IDEpisodio"].Text);
                            CoreStatics.CoreApplication.Episodio = new Episodio(e.Cell.Row.Cells["IDEpisodio"].Text);
                            CoreStatics.CoreApplication.Trasferimento = new Trasferimento(e.Cell.Row.Cells["IDTrasferimento"].Text, CoreStatics.CoreApplication.Ambiente);
                            if (CoreStatics.CoreApplication.Trasferimento != null && CoreStatics.CoreApplication.Trasferimento.IDCartella != "")
                                CoreStatics.CoreApplication.Cartella = new Cartella(CoreStatics.CoreApplication.Trasferimento.IDCartella, "", CoreStatics.CoreApplication.Ambiente);


                            if (e.Cell.Row.Cells["IDSCCI"].Text.Trim() != "")
                            {
                                _disableClick = true;
                                CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato = new MovEvidenzaClinica(e.Cell.Row.Cells["IDSCCI"].Text, e.Cell.Row.Cells["IDRefertoDWH"].Text.Trim(), EnumAzioni.VIS);
                            }
                            else
                            {
                                _disableClick = true;
                                CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato = new MovEvidenzaClinica(e.Cell.Row.Cells["IDRefertoDWH"].Text, e.Cell.Row.Cells["CodTipo"].Text, e.Cell.Row.Cells["DescrTipo"].Text, e.Cell.Row.Cells["CodStato"].Text, e.Cell.Row.Cells["DescrStato"].Text, e.Cell.Row.Cells["Anteprima"].Text, (DateTime)e.Cell.Row.Cells["DataReferto"].Value, (DateTime)e.Cell.Row.Cells["DataEventoDWH"].Value);
                            }
                            if (CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.AbilitaAperturaPDF)
                            {
                                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);
                                string sreftemp = System.IO.Path.Combine(FileStatics.GetSCCITempPath(), "referto" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + @".pdf");
                                byte[] pdf = CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.PDF;
                                if (pdf == null || pdf.Length <= 0)
                                {
                                    CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
                                    easyStatics.EasyMessageBox("Documento non presente.", "Apertura Referto", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    if (UnicodeSrl.Framework.Utility.FileSystem.ByteArrayToFile(sreftemp, ref pdf))
                                    {
                                        if (System.IO.File.Exists(sreftemp))
                                        {
                                            bool bAbilitaVisto = false;
                                            if (e.Cell.Row.Cells["PermessoVista"].Text == "1") bAbilitaVisto = true;


                                            easyStatics.ShellExecute(sreftemp, "", false, string.Empty, bAbilitaVisto);

                                            if (bAbilitaVisto)
                                            {
                                                if (CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.CodStatoEvidenzaClinicaVisione == EnumStatoEvidenzaClinicaVisione.VS.ToString())
                                                {

                                                    AggiornaGrigliaMantenendoPosizione(e.Cell, true);
                                                }
                                            }


                                        }
                                    }
                                }
                                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
                            }

                            CoreStatics.CoreApplication.Paziente = null;
                            CoreStatics.CoreApplication.Episodio = null;
                            CoreStatics.CoreApplication.Trasferimento = null;
                            CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato = null;
                            CoreStatics.CoreApplication.Cartella = null;

                            break;

                        case CoreStatics.C_COL_BTN_GRAPH:
                            if (e.Cell.Row.Cells["PermessoGrafico"].Text == "1")
                            {

                                _disableClick = true;
                                CoreStatics.CoreApplication.Paziente = new Paziente(e.Cell.Row.Cells["IDPaziente"].Text, e.Cell.Row.Cells["IDEpisodio"].Text);
                                CoreStatics.CoreApplication.Episodio = new Episodio(e.Cell.Row.Cells["IDEpisodio"].Text);
                                CoreStatics.CoreApplication.Trasferimento = new Trasferimento(e.Cell.Row.Cells["IDTrasferimento"].Text, CoreStatics.CoreApplication.Ambiente);

                                string idEpisodio = (CoreStatics.CoreApplication.Episodio != null ? CoreStatics.CoreApplication.Episodio.ID : "");
                                DateTime datada = (this.udteFiltroDA.Value != null ? (DateTime)this.udteFiltroDA.Value : DateTime.MinValue);
                                DateTime dataa = (this.udteFiltroA.Value != null ? (DateTime)this.udteFiltroA.Value : DateTime.MinValue);
                                DateTime dataricovero = (CoreStatics.CoreApplication.Episodio != null ? CoreStatics.CoreApplication.Episodio.DataRicovero : DateTime.MinValue);

                                dataa = DateTime.MinValue;
                                datada = DateTime.MinValue;

                                CoreStatics.CoreApplication.DefinizioneGraficoSelezionata = new ToolboxPerGrafici(idEpisodio, dataricovero, CoreStatics.CoreApplication.Paziente.CodSAC, EnumEntita.EVC, "", datada, dataa);

                                CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.GraficiEvidenzaClinica);

                                CoreStatics.CoreApplication.Paziente = null;
                                CoreStatics.CoreApplication.Episodio = null;
                                CoreStatics.CoreApplication.Trasferimento = null;
                            }
                            break;

                        default:
                            break;

                    }

                    PulisciAggiornaContesto();

                }
                catch (Exception ex)
                {
                    CoreStatics.ExGest(ref ex, "ucEasyGrid_ClickCellButton", this.Name);
                }
                finally
                {
                    _editRefertoDaGrid = false;
                }

                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
                _disableClick = false;
            }

            setFocusDefault();
        }

        private void ucEasyGrid_ClickCell(object sender, ClickCellEventArgs e)
        {

            try
            {

                switch (e.Cell.Column.Key)
                {

                    case "Anteprima":
                        CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainer);
                        _ucRichTextBox = CoreStatics.GetRichTextBoxForPopupControlContainer(e.Cell.Text, false);

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

        private void ubCartella_Click(object sender, EventArgs e)
        {

            try
            {
                if (this.ucEasyGrid.ActiveRow != null && this.ucEasyGrid.ActiveRow.IsDataRow)
                {

                    string sIDPaziente = (this.ucEasyGrid.ActiveRow.Cells["IDPaziente"].Text == string.Empty ? "" : this.ucEasyGrid.ActiveRow.Cells["IDPaziente"].Text);
                    string sIDEpisodio = (this.ucEasyGrid.ActiveRow.Cells["IDEpisodio"].Text == string.Empty ? "" : this.ucEasyGrid.ActiveRow.Cells["IDEpisodio"].Text);
                    string sIDTrasferimento = (this.ucEasyGrid.ActiveRow.Cells["IDTrasferimento"].Text == string.Empty ? "" : this.ucEasyGrid.ActiveRow.Cells["IDTrasferimento"].Text);

                    CoreStatics.CoreApplication.Paziente = new Paziente(sIDPaziente, sIDEpisodio);

                    if (sIDEpisodio != string.Empty)
                    {
                        CoreStatics.CoreApplication.Episodio = new Episodio(sIDEpisodio);
                    }
                    else
                    {
                        CoreStatics.CoreApplication.Episodio = null;
                    }
                    if (sIDTrasferimento != string.Empty)
                    {
                        CoreStatics.CoreApplication.Trasferimento = new Trasferimento(sIDTrasferimento, CoreStatics.CoreApplication.Ambiente);
                    }
                    else
                    {
                        CoreStatics.CoreApplication.Trasferimento = null;
                    }
                    if (CoreStatics.CoreApplication.Trasferimento != null && CoreStatics.CoreApplication.Trasferimento.NumeroCartella != "")
                    {
                        CoreStatics.CoreApplication.Cartella = new Cartella(CoreStatics.CoreApplication.Trasferimento.IDCartella, CoreStatics.CoreApplication.Trasferimento.NumeroCartella, CoreStatics.CoreApplication.Ambiente);
                    }
                    else
                    {
                        CoreStatics.CoreApplication.Cartella = null;
                    }

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
                    }
                    else
                    {
                        CoreStatics.CoreApplication.Paziente = null;
                        CoreStatics.CoreApplication.Episodio = null;
                        CoreStatics.CoreApplication.Trasferimento = null;
                        CoreStatics.RefreshUcTop();
                    }

                    setFocusDefault();
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        private void ucEvidenzaClinicaTrasversale_Enter(object sender, EventArgs e)
        {
            setFocusDefault();
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

    }
}
