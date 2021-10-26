using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTree;

using Microsoft.Data.SqlClient;
using System.Globalization;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.ScciResource;
using UnicodeSrl.ScciCore.CustomControls.Infra;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;
using System.IO;

namespace UnicodeSrl.ScciCore
{
    public partial class ucFirmaCartelleAperte : UserControl, Interfacce.IViewUserControlMiddle
    {

        private UserControl _ucc = null;
        private ucRichTextBox _ucRichTextBox = null;

        private List<string> _tempfiles = new List<string>();

        public ucFirmaCartelleAperte()
        {
            InitializeComponent();

            _ucc = (UserControl)this;
        }

        #region INTERFACCIA

        public void Aggiorna()
        {

            this.ubFirma.Enabled = false;

            CoreStatics.SetNavigazione(false);

            try
            {

                string sID = string.Empty;
                if (this.ucEasyGrid.ActiveRow != null)
                {
                    sID = this.ucEasyGrid.ActiveRow.Cells["IDTrasferimento"].Text;
                }

                this.AggiornaGriglia();

                if (sID != string.Empty && this.ucEasyGrid.Rows.Count > 0)
                {
                    for (int iRow = 0; iRow < this.ucEasyGrid.Rows.Count; iRow++)
                    {
                        UltraGridRow item = this.ucEasyGrid.Rows[iRow];
                        if (item.IsDataRow && !item.IsFilteredOut && item.Cells["IDTrasferimento"].Text.Trim().ToUpper() == sID.Trim().ToUpper())
                        {
                            this.ucEasyGrid.ActiveRow = item;
                            iRow = this.ucEasyGrid.Rows.Count + 1;
                        }
                    }
                }
                else
                {
                    this.AggiornaPaziente();
                }


            }
            catch (Exception ex)
            {
                this.AggiornaPaziente();
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }

            CoreStatics.SetNavigazione(true);


        }

        public void Carica()
        {
            try
            {
                InizializzaControlli();
                InizializzaUltraGridCartelleLayout();
                VerificaSicurezza();
                InizializzaFiltri();

                this.AggiornaSelezione();
                this.CaricaGriglieFiltro();

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

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Ferma", this.Name);
            }

        }

        #endregion

        #region FUNZIONI

        private void InizializzaControlli()
        {

            try
            {
                CoreStatics.SetEasyUltraDockManager(ref this.ultraDockManager);
                this.ubCartella.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_PAZIENTEMASCHIO_256);
                this.ubCartella.PercImageFill = 0.75F;
                this.ubCartella.ShortcutKey = Keys.P;
                this.ubCartella.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                this.ubCartella.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;


                this.ubFirmaMassiva.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FIRMAMULTIPLA_256);
                this.ubFirmaMassiva.PercImageFill = 0.75F;
                this.ubFirmaMassiva.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                this.ubFirmaMassiva.ShortcutKey = Keys.M;
                this.ubFirmaMassiva.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;

                this.ubFirma.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_TESSERAFIRMA_256);
                this.ubFirma.PercImageFill = 0.75F;
                this.ubFirma.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                this.ubFirma.ShortcutKey = Keys.C;
                this.ubFirma.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;

                this.ubCartella.Enabled = false;
                this.ubFirma.Enabled = false;
                this.ubFirmaMassiva.Enabled = false;
                this.ubFirmaMassiva.Visible = false;

                this.uchkFiltro.UNCheckedImage = Risorse.GetImageFromResource(Risorse.GC_FILTRO_256);
                this.uchkFiltro.CheckedImage = Risorse.GetImageFromResource(Risorse.GC_FILTROAPPLICATO_256);
                this.uchkFiltro.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                this.uchkFiltro.PercImageFill = 0.75F;
                this.uchkFiltro.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;

                this.uchkFiltro.Checked = false;

            }
            catch (Exception)
            {
            }
        }

        private void VerificaSicurezza()
        {

            try
            {


            }
            catch (Exception)
            {
            }
        }

        private void InizializzaUltraGridCartelleLayout()
        {
            try
            {
                this.ucEasyGrid.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
                this.ucEasyGridFirme.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaUltraGridLayout", this.Name);
            }
        }

        private void InizializzaFiltri()
        {
            try
            {
                this.ubApplicaFiltro.PercImageFill = 0.75F;
                this.ubApplicaFiltro.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                this.ubApplicaFiltro.TextFontRelativeDimension = easyStatics.easyRelativeDimensions.Large;


                this.uteRicerca.Text = "";
                if (this.ucEasyGridFiltroUA.Rows.Count > 0)
                {
                    this.ucEasyGridFiltroUA.Selected.Rows.Clear();
                    this.ucEasyGridFiltroUA.ActiveRow = null;
                    CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGridFiltroUA, this.ucEasyGridFiltroUA.DisplayLayout.Bands[0].Columns[0].Key, CoreStatics.GC_TUTTI);
                }
                if (this.ucEasyGridFiltroTipoEpisodio.Rows.Count > 0)
                {
                    this.ucEasyGridFiltroTipoEpisodio.Selected.Rows.Clear();
                    this.ucEasyGridFiltroTipoEpisodio.ActiveRow = null;
                    CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGridFiltroTipoEpisodio, this.ucEasyGridFiltroTipoEpisodio.DisplayLayout.Bands[0].Columns[0].Key, CoreStatics.GC_TUTTI);
                }

            }
            catch (Exception)
            {
            }
        }

        private void AggiornaGriglia()
        {

            this.ubFirmaMassiva.Enabled = false;

            try
            {
                CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.WaitCursor);
                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);

                bool bFiltro = false;


                if (this.uteRicerca.Text != string.Empty)
                {

                    string filtrogenerico = string.Empty;

                    string[] ricerche = this.uteRicerca.Text.Split(' ');
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
                    bFiltro = true;

                }


                if (this.ucEasyGridFiltroUA.ActiveRow != null && this.ucEasyGridFiltroUA.ActiveRow.Cells["Codice"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                {
                    op.Parametro.Add("CodUA", this.ucEasyGridFiltroUA.ActiveRow.Cells["Codice"].Text);
                    bFiltro = true;
                }
                if (this.ucEasyGridFiltroTipoEpisodio.ActiveRow != null && this.ucEasyGridFiltroTipoEpisodio.ActiveRow.Cells["Codice"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                {
                    op.Parametro.Add("CodTipoEpisodio", this.ucEasyGridFiltroTipoEpisodio.ActiveRow.Cells["Codice"].Text);
                    bFiltro = true;
                }

                op.Parametro.Add("CodStatoCartella", EnumStatoCartella.AP.ToString());

                op.ParametroRipetibile.Add("CodStatoTrasferimento", new string[4]
                    { EnumStatoTrasferimento.TR.ToString(),
                      EnumStatoTrasferimento.DM.ToString(),
                      EnumStatoTrasferimento.AT.ToString(),
                      EnumStatoTrasferimento.PR.ToString()
                    });

                op.Parametro.Add("Ordinamento", "P.Cognome, P.Nome");

                op.Parametro.Add("SoloUltimoTrasferimentoCartellaNoSospesi", "1");

                this.uchkFiltro.Checked = bFiltro;

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_CercaEpisodio", spcoll);


                this.ucEasyGrid.DataSource = ds.Tables[0];
                this.ucEasyGrid.Refresh();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);
            }

        }

        private void CaricaGriglieFiltro()
        {

            try
            {
                CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.WaitCursor);







                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelUADaRuolo", spcoll);

                this.ucEasyGridFiltroUA.DataSource = CoreStatics.AggiungiTuttiDataTable(dt, true);
                this.ucEasyGridFiltroUA.DisplayLayout.Bands[0].Columns["Descrizione"].Header.Caption = "Filtro per Struttura";

                this.ucEasyGridFiltroUA.DisplayLayout.Bands[0].SortedColumns.Clear();
                this.ucEasyGridFiltroUA.DisplayLayout.Bands[0].SortedColumns.Add("Descrizione", false);

                this.ucEasyGridFiltroUA.Refresh();

                op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("DatiEstesi", "0");

                spcoll = new SqlParameterExt[1];

                xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                dt = Database.GetDataTableStoredProc("MSP_SelTipoEpisodio", spcoll);

                this.ucEasyGridFiltroTipoEpisodio.DataSource = CoreStatics.AggiungiTuttiDataTable(dt, true);
                this.ucEasyGridFiltroTipoEpisodio.DisplayLayout.Bands[0].Columns["Descrizione"].Header.Caption = "Tipo Episodio";

                this.ucEasyGridFiltroTipoEpisodio.DisplayLayout.Bands[0].SortedColumns.Clear();
                this.ucEasyGridFiltroTipoEpisodio.DisplayLayout.Bands[0].SortedColumns.Add("Descrizione", false);

                this.ucEasyGridFiltroTipoEpisodio.Refresh();



                ubApplicaFiltro_Click(this.ubApplicaFiltro, new EventArgs());

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);
            }

        }

        private void AggiornaPaziente()
        {

            try
            {


                CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.WaitCursor);
                if (this.ucEasyGrid.ActiveRow != null)
                {

                    if (this.ucEasyGrid.ActiveRow.Cells["IDCartella"].Text != "")
                        CoreStatics.CoreApplication.Cartella = new Cartella(this.ucEasyGrid.ActiveRow.Cells["IDCartella"].Text, this.ucEasyGrid.ActiveRow.Cells["NumeroCartella"].Text, CoreStatics.CoreApplication.Ambiente);

                    Paziente oPaz = new Paziente("", this.ucEasyGrid.ActiveRow.Cells["IDEpisodio"].Text);
                    this.ucEasyLabelPaziente.Text = oPaz.Descrizione;





                    CaricaElencoFirme();





                    if ((this.ucEasyGrid.ActiveRow.Cells["CodStatoCartella"].Text == EnumStatoCartella.AP.ToString() || this.ucEasyGrid.ActiveRow.Cells["CodStatoCartella"].Text == EnumStatoCartella.CH.ToString())
                    && CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Pazienti_Menu))
                    {
                        this.ubCartella.Enabled = true;
                    }
                    else
                    {
                        this.ubCartella.Enabled = false;
                    }

                    this.ubFirma.Enabled = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Firma_CartellaAperta);

                    if (this.ucEasyGrid.ActiveRow.Cells["CodStatoCartella"].Text == EnumStatoCartella.AP.ToString())
                        this.ubFirma.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_TESSERAFIRMA_256);
                    else
                        this.ubFirma.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOCHIUSO_256);

                }
                else
                {
                    ResetPaziente();
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "AggiornaPaziente", this.Name);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);
            }


        }

        private void CaricaElencoFirme()
        {
            try
            {

                if (CoreStatics.CoreApplication.Cartella != null && CoreStatics.CoreApplication.Cartella.ID != string.Empty)
                {

                    Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("IDCartella", CoreStatics.CoreApplication.Cartella.ID);
                    op.Parametro.Add("DatiEstesi", "0");

                    op.TimeStamp.CodEntita = EnumEntita.DCF.ToString();
                    op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                    SqlParameterExt[] spcoll = new SqlParameterExt[1];
                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovDocumentiFirmatiCartella", spcoll);



                    dt.DefaultView.Sort = "DataInserimento DESC, Numero DESC, UtenteInserimento";
                    this.ucEasyGridFirme.DataSource = dt.DefaultView;
                    this.ucEasyGridFirme.Refresh();

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        private void ResetPaziente()
        {

            try
            {

                this.ucEasyLabelPaziente.Text = "";

                this.ucEasyGridFirme.DataSource = null;



                this.ubFirma.Enabled = false;
                this.ubFirma.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FIRMA_256);
                this.ubCartella.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_PAZIENTEMASCHIO_256);
                this.ubCartella.Enabled = false;

                CoreStatics.CoreApplication.Cartella = null;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }


        }

        private void InitializeRow(UltraGridRow eRow)
        {
            try
            {
                if (eRow.Cells.Exists("ColoreStatoCartella") && eRow.Cells["ColoreStatoCartella"].Text != "")
                {
                    eRow.Appearance.BackColor = CoreStatics.GetColorFromString(eRow.Cells["ColoreStatoCartella"].Text);
                    eRow.Appearance.ForeColor = Color.Black;
                    foreach (UltraGridCell oCell in eRow.Cells)
                    {
                        if (!oCell.Hidden)
                        {
                            oCell.Appearance.ForeColor = Color.Black;
                            oCell.ActiveAppearance.BackColor = eRow.Appearance.BackColor;
                            oCell.ActiveAppearance.ForeColor = Color.Blue;
                            oCell.ActiveAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                        }
                    }
                }

            }
            catch (Exception)
            {
            }
        }

        private void AggiornaSelezione()
        {

        }

        #endregion

        #region EVENTI

        private void ubCartella_Click(object sender, EventArgs e)
        {
            try
            {
                UltraGridRow oUgr = this.ucEasyGrid.ActiveRow;
                if (oUgr != null)
                {
                    CoreStatics.CoreApplication.Paziente = new Paziente("", oUgr.Cells["IDEpisodio"].Text);
                    CoreStatics.CoreApplication.Episodio = new Episodio(oUgr.Cells["IDEpisodio"].Text);
                    CoreStatics.CoreApplication.Trasferimento = new Trasferimento(oUgr.Cells["IDTrasferimento"].Text, CoreStatics.CoreApplication.Ambiente);

                    if (oUgr.Cells["IDCartella"].Text != "") CoreStatics.CoreApplication.Cartella = new Cartella(oUgr.Cells["IDCartella"].Text, oUgr.Cells["NumeroCartella"].Text, CoreStatics.CoreApplication.Ambiente);





                    if (CoreStatics.CoreApplication.Cartella != null && CoreStatics.CoreApplication.Cartella.CartellaChiusa == true)
                    {
                        if (oUgr.Cells["CodStatoCartella"].Text == Scci.Enums.EnumStatoCartella.AP.ToString())
                        {
                            CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.CartellaPaziente);

                            CoreStatics.CoreApplication.Navigazione.Maschere.RimuoviMaschereMassimizzabili();
                        }
                        else
    if (oUgr.Cells["CodStatoCartella"].Text == Scci.Enums.EnumStatoCartella.CH.ToString())
                        {
                            CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.CartellaPaziente);

                            CoreStatics.CoreApplication.Navigazione.Maschere.RimuoviMaschereMassimizzabili();

                        }
                        else
                        {
                            easyStatics.EasyMessageBox("Cartella non ancora aperta.", "Chiusura Cartella", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }


                    }
                    else
                    {
                        if (oUgr.Cells["CodStatoCartella"].Text == Scci.Enums.EnumStatoCartella.AP.ToString())
                        {
                            CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.CartellaPaziente);
                        }
                        else
    if (oUgr.Cells["CodStatoCartella"].Text == Scci.Enums.EnumStatoCartella.CH.ToString())
                        {
                            CoreStatics.apriPDFCartella(CoreStatics.CoreApplication.Cartella);
                        }
                        else
                        {
                            easyStatics.EasyMessageBox("Cartella non ancora aperta.", "Chiusura Cartella", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ubCartella_Click", this.Name);
            }

        }

        private void ubFirmaMassiva_Click(object sender, EventArgs e)
        {

            frmProgress frmSC = null;
            int nitem = 0;

            if (easyStatics.EasyMessageBox(@"Sei sicuro di voler firmare le Cartelle visualizzate?", "Firma Cartelle Clinica", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {

                try
                {

                    frmSC = new frmProgress();
                    frmSC.InizializzaEMostra(0, this.ucEasyGrid.Rows.Count, this);
                    Form objForm = (Form)frmSC;
                    CoreStatics.impostaCursore(ref objForm, Scci.Enums.enum_app_cursors.WaitCursor);

                    foreach (UltraGridRow oDr in this.ucEasyGrid.Rows)
                    {
                        nitem += 1;
                        frmSC.SetInfo(string.Format("{4} - {0}{3}Sto firmando la cartella {1} di {2}", oDr.Cells["Paziente"].Text, nitem.ToString(), this.ucEasyGrid.Rows.Count.ToString(), Environment.NewLine, oDr.Cells["NumeroCartella"].Text), nitem);
                        Application.DoEvents();
                        this.ucEasyGrid.ActiveRow = oDr;
                        if (FirmaCartellaSmartCard(objForm.Height) == false)
                        {
                            break;
                        }

                    }
                    this.Aggiorna();
                    this.AggiornaSelezione();

                }
                catch (Exception ex)
                {
                    CoreStatics.ExGest(ref ex, "ubFirmaMassiva_Click", this.Name);
                }
                finally
                {

                    if (frmSC != null)
                    {
                        frmSC.Close();
                        frmSC.Dispose();
                    }
                    CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);

                }

            }

        }

        private void ubFirma_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.ucEasyGrid.ActiveRow != null && this.ucEasyGrid.ActiveRow.IsDataRow && !this.ucEasyGrid.ActiveRow.IsFilteredOut)
                {
                    if (this.ucEasyGrid.ActiveRow.Cells["CodStatoCartella"].Text.Trim().ToUpper() == EnumStatoCartella.AP.ToString())
                    {
                        if (easyStatics.EasyMessageBox(@"Sei sicuro di voler firmare la Cartella clinica di " + this.ucEasyGrid.ActiveRow.Cells["Paziente"].Text + @"?", "Firma Cartella Clinica", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            if (FirmaCartellaSmartCard()) Aggiorna();
                        }
                    }
                    else
                    {
                        easyStatics.EasyMessageBox("La Cartella selezionata è " + this.ucEasyGrid.ActiveRow.Cells["DescrStatoCartella"].Text + @"!", "Firma Cartella Clinica", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ubValida_Click", this.Name);
            }
        }

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

        private void ucEasyGrid_BeforeSortChange(object sender, Infragistics.Win.UltraWinGrid.BeforeSortChangeEventArgs e)
        {

            if (e.SortedColumns.Exists("Data Ingresso Data Ricovero") == true ||
                e.SortedColumns.Exists("Data Dimissione Data Trasferimento") == true)
            {
                e.Cancel = true;
            }

        }

        private void ucEasyGrid_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {
                int refWidth = (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large);

                Graphics g = this.CreateGraphics();
                int refBtnWidth = CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(this.ucEasyGrid.DataRowFontRelativeDimension), g.DpiY) * 4;
                g.Dispose();
                g = null;
                e.Layout.Bands[0].HeaderVisible = false;
                e.Layout.Bands[0].ColHeadersVisible = true;
                e.Layout.Bands[0].RowLayoutStyle = RowLayoutStyle.GroupLayout;


                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {
                    try
                    {
                        switch (oCol.Key)
                        {
                            case "NumeroCartella":

                                oCol.Hidden = false;
                                oCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.LockedWidth = true;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.Header.Caption = @"Cartella";
                                try
                                {
                                    oCol.MaxWidth = refWidth * 4;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.WeightY = 1;
                                oCol.RowLayoutColumnInfo.OriginX = 0;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "Paziente":
                                oCol.Hidden = false;
                                oCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.LockedWidth = true;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.Header.Caption = @"Paziente";
                                try
                                {
                                    oCol.MaxWidth = refWidth * 12;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.WeightY = 1;
                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "DescrUA":
                                oCol.Hidden = false;
                                oCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.LockedWidth = true;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.Header.Caption = @"Struttura";
                                try
                                {
                                    oCol.MaxWidth = refWidth * 6;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.WeightY = 1;
                                oCol.RowLayoutColumnInfo.OriginX = 2;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "UO - Settore":
                                oCol.Hidden = false;
                                oCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.LockedWidth = true;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.Header.Caption = @"UO - Settore";
                                try
                                {
                                    oCol.MaxWidth = refWidth * 8;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.WeightY = 1;
                                oCol.RowLayoutColumnInfo.OriginX = 3;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;
                                break;

                            case "Data Ingresso Data Ricovero":
                                oCol.Hidden = false;
                                oCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.LockedWidth = true;
                                oCol.Header.Caption = @"Data Ingresso /" + Environment.NewLine + @"Data Ricovero";
                                oCol.Format = "dd/MM/yyyy";
                                oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                try
                                {
                                    oCol.MaxWidth = refWidth * 6;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.WeightY = 1;
                                oCol.RowLayoutColumnInfo.OriginX = 4;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;
                                break;

                            case "Data Dimissione Data Trasferimento":
                                oCol.Hidden = false;
                                oCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.LockedWidth = true;
                                oCol.Header.Caption = @"Data Dimissione /" + Environment.NewLine + @"Data Trasferimento";
                                oCol.Format = "dd/MM/yyyy";
                                oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                try
                                {
                                    oCol.MaxWidth = refWidth * 6;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.WeightY = 1;
                                oCol.RowLayoutColumnInfo.OriginX = 5;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "DecrStato":
                                oCol.Hidden = false;
                                oCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.LockedWidth = true;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.Header.Caption = @"Stato";
                                try
                                {
                                    oCol.MaxWidth = refWidth * 5;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.WeightY = 1;
                                oCol.RowLayoutColumnInfo.OriginX = 6;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "DescEpisodio":
                                oCol.Hidden = false;
                                oCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.LockedWidth = true;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.Header.Caption = @"Tipo Episodio /" + Environment.NewLine + @"Nosologico";
                                oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                try
                                {
                                    oCol.MaxWidth = this.ucEasyGrid.Width - Convert.ToInt32(refWidth * 47) - Convert.ToInt32(refBtnWidth * 0) - 30;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.WeightY = 1;
                                oCol.RowLayoutColumnInfo.OriginX = 7;
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
            catch (Exception ex)
            {
                string aa = ex.Message;
            }
        }

        private void ucEasyGrid_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            InitializeRow(e.Row);
        }

        private void ucEasyGrid_AfterRowActivate(object sender, EventArgs e)
        {
            this.AggiornaSelezione();
            this.AggiornaPaziente();
        }

        private void ucEasyGridFirme_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {
                int refWidth = (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large);

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
                        switch (oCol.Key)
                        {
                            case "DataInserimento":
                                oCol.Hidden = false;
                                oCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.LockedWidth = true;
                                oCol.Format = "dd/MM/yyyy HH:mm:ss";
                                oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                try
                                {
                                    oCol.MaxWidth = refWidth * 10;
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


                            case "UtenteInserimento":
                                oCol.Hidden = false;
                                oCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.LockedWidth = true;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.Header.Caption = @"Utente Inserimento";
                                oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                try
                                {
                                    oCol.MaxWidth = refWidth * 20;
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

                            case "DescrStatoEntita":
                                oCol.Hidden = false;
                                oCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.LockedWidth = true;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.Header.Caption = @"Stato Cartella";
                                oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                try
                                {
                                    oCol.MaxWidth = this.ucEasyGridFirme.Width - Convert.ToInt32(refWidth * 30) - Convert.ToInt32(refBtnWidth * 2) - 30;
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



                if (e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_VIEW))
                    e.Layout.Bands[0].Columns[CoreStatics.C_COL_BTN_VIEW].Hidden = false;
                else
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
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_DOWNLOADDOCUMENTO_32);

                    colEdit.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = refBtnWidth * 2;
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
            catch (Exception ex)
            {
                string aa = ex.Message;
            }
        }

        private void ucEasyGridFirme_InitializeRow(object sender, InitializeRowEventArgs e)
        {
        }

        private void ucEasyGridFirme_ClickCellButton(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            try
            {
                switch (e.Cell.Column.Key)
                {
                    case CoreStatics.C_COL_BTN_VIEW:
                        CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.WaitCursor);

                        try
                        {
                            string sreftemp = System.IO.Path.Combine(FileStatics.GetSCCITempPath(), "Cartella" + DateTime.Now.ToString("yyyyMMddHHmmssfff"));
                            MovDocumentiFirmati tmpdoc = new MovDocumentiFirmati(e.Cell.Row.Cells["ID"].Value.ToString());
                            byte[] pdf = (tmpdoc.PDFNonFirmato == null ? tmpdoc.PDFFirmato : tmpdoc.PDFNonFirmato);
                            sreftemp = sreftemp + (tmpdoc.PDFNonFirmato == null ? @".pdf.p7m" : @".pdf");
                            if (pdf == null || pdf.Length <= 0)
                            {
                                CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);
                                easyStatics.EasyMessageBox("Documento non presente.", "Apertura Referto", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                if (UnicodeSrl.Framework.Utility.FileSystem.ByteArrayToFile(sreftemp, ref pdf))
                                {
                                    if (System.IO.File.Exists(sreftemp))
                                    {
                                        _tempfiles.Add(sreftemp);

                                        easyStatics.ShellExecute(sreftemp, "", (tmpdoc.PDFNonFirmato == null ? true : false), string.Empty, false);

                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);
                            CoreStatics.ExGest(ref ex, @"Apertura Documento non disponibile." + Environment.NewLine + @"Contattare amministratori di sistema", "ucEasyGridFirme_ClickCellButton", this.Name);
                        }

                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);
                CoreStatics.ExGest(ref ex, "ucEasyGridFirme_ClickCellButton", this.Name);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);
            }


        }

        private void uchkFiltro_Click(object sender, EventArgs e)
        {
            if (!this.uchkFiltro.Checked)
            {
                this.InizializzaFiltri();
                this.Aggiorna();
            }
            else
                this.uchkFiltro.Checked = !this.uchkFiltro.Checked;

        }

        private void ubApplicaFiltro_Click(object sender, EventArgs e)
        {
            if (this.ultraDockManager.FlyoutPane != null && !this.ultraDockManager.FlyoutPane.Pinned) this.ultraDockManager.FlyIn();
            this.Aggiorna();
            this.ucEasyGrid.Focus();
        }

        private void ucEasyGridFiltro_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Override.HeaderClickAction = HeaderClickAction.Select;
            foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
            {
                if (oCol.Key.ToUpper().IndexOf("DESC") < 0) oCol.Hidden = true;

            }
        }

        private void ucFirmaCartelleAperte_Enter(object sender, EventArgs e)
        {
            try
            {
                this.ucEasyGrid.PerformLayout();
            }
            catch (Exception)
            {
            }
        }

        private void ucRtfRilevanti_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {

                if (e.Layout.Bands[0].Columns.Exists("ID") == true)
                {
                    e.Layout.Bands[0].Columns["ID"].Hidden = true;
                }

                if (e.Layout.Bands[0].Columns.Exists("Descrizione") == true)
                {
                    e.Layout.Bands[0].Columns["Descrizione"].CellAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                    e.Layout.Bands[0].Columns["Descrizione"].CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                }

                if (e.Layout.Bands[0].Columns.Exists("AnteprimaRTF") == true)
                {
                    e.Layout.Bands[0].Columns["AnteprimaRTF"].Hidden = true;
                }

                if (e.Layout.Bands[0].Columns.Exists("DatiRilievoRTF") == true)
                {
                    e.Layout.Bands[0].Columns["DatiRilievoRTF"].CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    RichTextEditor a = new RichTextEditor();
                    e.Layout.Bands[0].Columns["DatiRilievoRTF"].Editor = a;
                }

                if (e.Layout.Bands[0].Columns.Exists("DatiObbligatoriMancantiRTF") == true)
                {
                    e.Layout.Bands[0].Columns["DatiObbligatoriMancantiRTF"].Hidden = true;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private void ucRtfMancanti_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {

                if (e.Layout.Bands[0].Columns.Exists("ID") == true)
                {
                    e.Layout.Bands[0].Columns["ID"].Hidden = true;
                }

                if (e.Layout.Bands[0].Columns.Exists("Descrizione") == true)
                {
                    e.Layout.Bands[0].Columns["Descrizione"].CellAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                    e.Layout.Bands[0].Columns["Descrizione"].CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                }

                if (e.Layout.Bands[0].Columns.Exists("AnteprimaRTF") == true)
                {
                    e.Layout.Bands[0].Columns["AnteprimaRTF"].Hidden = true;
                }

                if (e.Layout.Bands[0].Columns.Exists("DatiRilievoRTF") == true)
                {
                    e.Layout.Bands[0].Columns["DatiRilievoRTF"].Hidden = true;
                }

                if (e.Layout.Bands[0].Columns.Exists("DatiObbligatoriMancantiRTF") == true)
                {
                    e.Layout.Bands[0].Columns["DatiObbligatoriMancantiRTF"].CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    RichTextEditor a = new RichTextEditor();
                    e.Layout.Bands[0].Columns["DatiObbligatoriMancantiRTF"].Editor = a;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private void uteRicerca_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter) ubApplicaFiltro_Click(this.ubApplicaFiltro, new EventArgs());
            }
            catch (Exception)
            {
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

        private void ucRtf_ClickCell(object sender, ClickCellEventArgs e)
        {

            Infragistics.Win.UIElement uie;
            Point oPoint;

            try
            {

                switch (e.Cell.Column.Key)
                {

                    case "DatiRilievoRTF":
                    case "DatiObbligatoriMancantiRTF":
                        CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainer);
                        _ucRichTextBox = CoreStatics.GetRichTextBoxForPopupControlContainer(e.Cell.Text);

                        uie = e.Cell.GetUIElement();
                        oPoint = new Point(uie.Rect.Left + ((ucEasyGrid)sender).Parent.Parent.Parent.Location.X,
                                           uie.Rect.Top + ((ucEasyGrid)sender).Parent.Parent.Parent.Parent.Parent.Parent.Location.Y);

                        this.UltraPopupControlContainer.Show((ucEasyGrid)sender, oPoint);
                        break;

                    default:
                        break;

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucRtf_ClickCell", this.Name);
            }

        }

        #endregion

        #region FIRMA DIGITALE

        public bool FirmaCartellaSmartCard(int delta = 0)
        {

#if DEBUG
#endif


            bool bReturn = false;
            frmSmartCardProgress frmSC = null;

            try
            {

                if (this.ucEasyGrid.ActiveRow != null && this.ucEasyGrid.ActiveRow.IsDataRow && !this.ucEasyGrid.ActiveRow.IsFilteredOut)
                {
                    if (this.ucEasyGrid.ActiveRow.Cells["CodStatoCartella"].Text.Trim().ToUpper() == EnumStatoCartella.AP.ToString())
                    {

                        CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.WaitCursor);

                        setNavigazione(false);

                        bool bcontinua = true;
                        byte[] pdfContent = null;

                        frmSC = new frmSmartCardProgress();
                        frmSC.InizializzaEMostra(0, 3, this, delta);
                        Form objForm = (Form)frmSC;
                        CoreStatics.impostaCursore(ref objForm, Scci.Enums.enum_app_cursors.WaitCursor);


                        UnicodeSrl.SmartCard.SCStatus.SmartCardStatusEnum statoSC = SmartCard.SCStatus.SmartCardStatusEnum._undefined;

                        while (statoSC != SmartCard.SCStatus.SmartCardStatusEnum.allOK
                            && statoSC != SmartCard.SCStatus.SmartCardStatusEnum.cardReady
                            && !(frmSC != null && frmSC.TerminaOperazione))
                        {

                            Application.DoEvents();

                            if (frmSC != null)
                                statoSC = frmSC.StatoSmartCard;
                            else
                                statoSC = SmartCard.SCStatus.SmartCardStatusEnum.allOK;


                            bcontinua = (statoSC == SmartCard.SCStatus.SmartCardStatusEnum.allOK || statoSC == SmartCard.SCStatus.SmartCardStatusEnum.cardReady);



                            if (bcontinua) bcontinua = (frmSC == null || !frmSC.TerminaOperazione);

                            try
                            {
                                if (bcontinua)
                                {
                                    bcontinua = false;
                                    if (frmSC != null) frmSC.SetStato(@"Archiviazione Documento...");

                                    Report _report = null;
                                    foreach (Report rep in CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Reports.Elementi)
                                    {
                                        if (rep.Codice.ToUpper() == Report.COD_REPORT_CARTELLA_PAZIENTE)
                                            _report = rep;
                                    }
                                    if (_report != null)
                                    {
                                        if (_report.NomePlugIn != null && _report.NomePlugIn.Trim() != "")
                                        {
                                            string path = System.Windows.Forms.Application.StartupPath + @"\Plugins\" + _report.NomePlugIn + @"\" + _report.NomePlugIn + @".dll";

                                            bool firmaDigitale = (frmSC != null && !frmSC.TerminaOperazione);
                                            bcontinua = CoreStatics.CoreApplication.Cartella.generaearchiviaPDF(plugindllfullpath: path,
                                                                                                                firmaDigitale: firmaDigitale,
                                                                                                                utenteFirma: CoreStatics.CoreApplication.Sessione.Utente.Descrizione,
                                                                                                                sessioneRemota: CoreStatics.CoreApplication.Sessione.Computer.SessioneRemota,
                                                                                                                isOSServer: CoreStatics.CoreApplication.Sessione.Computer.IsOSServer,
                                                                                                                evc: true,
                                                                                                                soloAllegaInCartella: true,
                                                                                                                allegati: true,
                                                                                                                fileContent: out pdfContent);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                CoreStatics.ExGest(ref ex, "FirmaCartellaSmartCard", this.Name);
                            }

                            if (bcontinua) bcontinua = (frmSC == null || !frmSC.TerminaOperazione);


                            string sIDDocumentoFirmato = "";
                            try
                            {
                                if (bcontinua)
                                {
                                    if (frmSC != null)
                                    {
                                        bcontinua = false;

                                        frmSC.SetStato(@"Firma Digitale del Documento...");

                                        if (pdfContent != null)
                                        {
                                            byte[] bySigned = frmSC.FirmaDocumento(ref pdfContent);
                                            if (bySigned != null && !frmSC.TerminaOperazione)
                                            {
                                                MovDocumentiFirmati odocf = new MovDocumentiFirmati();
                                                odocf.Azione = EnumAzioni.INS;
                                                odocf.CodEntita = EnumEntita.CAR.ToString();
                                                odocf.CodTipoDocumentoFirmato = EnumTipoDocumentoFirmato.CARFM01.ToString();
                                                odocf.CodStatoEntita = EnumStatoCartella.AP.ToString();
                                                odocf.IDEntita = this.ucEasyGrid.ActiveRow.Cells["IDCartella"].Text;
                                                odocf.PDFFirmato = bySigned;
                                                odocf.PDFNonFirmato = pdfContent;
                                                bcontinua = (odocf.Salva());
                                                sIDDocumentoFirmato = odocf.IDDocumento;
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                CoreStatics.ExGest(ref ex, "FirmaCartellaSmartCard", this.Name);
                            }

                            try
                            {
                                if (bcontinua && frmSC != null)
                                {
                                    bcontinua = false;

                                    if (sIDDocumentoFirmato != null && sIDDocumentoFirmato.Trim() != "")
                                    {
                                        MovDocumentiFirmati odocf = new MovDocumentiFirmati(sIDDocumentoFirmato);
                                        if (odocf != null && odocf.IDDocumento != null && odocf.IDDocumento.Trim() != "" && odocf.PDFFirmato != null && odocf.PDFFirmato.Length > 0)
                                        {
                                            bcontinua = true;
                                            frmSC.TerminaOperazione = true;
                                        }
                                        else
                                        {
                                            frmSC.SetStato(@"Firma Digitale del Documento NON riuscita!");
                                            easyStatics.EasyMessageBox(this.ucEasyGrid.ActiveRow.Cells["Paziente"].Text + Environment.NewLine + @"Archiviazione documento firmato NON riuscita!", @"Firma Cartella", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(new Exception(this.ucEasyGrid.ActiveRow.Cells["Paziente"].Text + Environment.NewLine + @"Archiviazione documento firmato NON riuscita!"));
                                        }
                                    }

                                }
                            }
                            catch (Exception ex)
                            {
                                CoreStatics.ExGest(ref ex, "FirmaCartellaSmartCard", this.Name);
                            }

                            if (frmSC != null && frmSC.StatoSmartCard != SmartCard.SCStatus.SmartCardStatusEnum.cardReady) statoSC = frmSC.StatoSmartCard;

                        }


                        bReturn = bcontinua;

                    }
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "FirmaCartellaSmartCard", this.Text);
                bReturn = false;
            }
            finally
            {

                if (frmSC != null)
                {
                    frmSC.Close();
                    frmSC.Dispose();
                }

                setNavigazione(true);

                CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);
            }

            return bReturn;

        }

        private void setNavigazione(bool enable)
        {
            try
            {
                CoreStatics.SetNavigazione(enable);

                this.ubApplicaFiltro.Enabled = enable;
                this.ubCartella.Enabled = enable;

                this.ubFirmaMassiva.Enabled = false;
                this.ubFirmaMassiva.Visible = false;

                this.ubFirma.Enabled = enable;
                this.cmdRicerca.Enabled = enable;

                this.uteRicerca.Enabled = enable;
                this.ucEasyTableLayoutPanelFiltro1.Enabled = enable;
                this.ucEasyGrid.Enabled = enable;

            }
            catch
            {
                CoreStatics.SetNavigazione(true);
            }
        }

        #endregion
    }
}
