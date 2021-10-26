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
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.ScciResource;
using UnicodeSrl.Framework.Data;

using Infragistics.Win.UltraWinGrid;

namespace UnicodeSrl.ScciCore
{
    public partial class frmPUProseguiTerapia : frmBaseModale, Interfacce.IViewFormlModal
    {

        public frmPUProseguiTerapia()
        {
            InitializeComponent();
        }

        #region Declare

        private UserControl _ucc = null;
        private bool bSkipEvent = false;
        private Dictionary<string, MovPrescrizioneTempi> dict_mpt = new Dictionary<string, MovPrescrizioneTempi>();

        private ucRichTextBox _ucRichTextBox = null;
        private ucEasyGrid _ucEasyGridOrari = null;
        private bool _bFirmaDigitale = false;

        #endregion

        #region Interface

        public void Carica()
        {

            try
            {

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_PROSEGUITERAPIA_256);

                _bFirmaDigitale = DBUtils.ModuloUAAbilitato(CoreStatics.CoreApplication.Trasferimento.CodUA, EnumUAModuli.FirmaD_Prescrizioni);

                this.InizializzaControlli();
                this.InizializzaUltraGridLayout();

                this.CaricaGriglia();

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

                this.uceRangeInizio.SelectedIndex = 0;
                this.uceRangeFine.SelectedIndex = 0;

                if (CoreStatics.CoreApplication.ProseguiTerapiaSelezionata != null)
                {
                    this.udteFiltroDA.Value = CoreStatics.CoreApplication.ProseguiTerapiaSelezionata.DataInizio;
                    this.udteFiltroA.Value = CoreStatics.CoreApplication.ProseguiTerapiaSelezionata.DataFine;
                    this.udteFiltroDAFine.Value = CoreStatics.CoreApplication.ProseguiTerapiaSelezionata.DataInizioProsecuzione;
                    this.udteFiltroAFine.Value = CoreStatics.CoreApplication.ProseguiTerapiaSelezionata.DataFineProsecuzione;
                }

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "InizializzaControlli", this.Text);
            }

        }

        private void setNavigazione(bool enable)
        {

            try
            {

                CoreStatics.SetNavigazione(enable);

                this.ucBottomModale.Enabled = enable;
                this.ucEasyTableLayoutPanel.Enabled = enable;
                this.ucEasyTableLayoutPanelFiltro.Enabled = enable;
                this.ucEasyTableLayoutPanelFiltroFine.Enabled = enable;

            }
            catch
            {
                CoreStatics.SetNavigazione(true);
                this.ucBottomModale.Enabled = true;
            }

        }

                                        
        private string CreaNuovaTempistica(MovPrescrizioneTempi mpt)
        {

            string bret = string.Empty;

                        
                                    
                        
                                                                                                                                                                                                                                                                                                                                                                                    if (mpt.Manuale == true)
            {
                bret += "Manuale";
            }
            else if (mpt.AlBisogno == true)
            {
                bret += "Al Bisogno";
            }
            else if (mpt.DataOraInizio != DateTime.MinValue)
            {
                if (mpt.PeriodicitaGiorni != 0 || mpt.PeriodicitaOre != 0 || mpt.PeriodicitaMinuti != 0 || mpt.CodTipoProtocollo == "ORA")
                {
                    bret += "Da: ";
                }
                bret += string.Format("{0:dd-MM-yyyy HH:mm}", mpt.DataOraInizio);
                if (mpt.DataOraFine != DateTime.MinValue)
                {
                    if (mpt.DataOraInizio != mpt.DataOraFine && mpt.DataOraInizio.Date == mpt.DataOraFine.Date &&
                        (mpt.PeriodicitaGiorni != 0 || mpt.PeriodicitaOre != 0 || mpt.PeriodicitaMinuti != 0 || mpt.CodTipoProtocollo == "ORA"))
                    {
                        bret += Environment.NewLine + string.Format("A:  {0:HH:mm}", mpt.DataOraFine);
                    }
                    else if (mpt.DataOraInizio != mpt.DataOraFine &&
                        (mpt.PeriodicitaGiorni != 0 || mpt.PeriodicitaOre != 0 || mpt.PeriodicitaMinuti != 0 || mpt.CodTipoProtocollo == "ORA"))
                    {
                        bret += Environment.NewLine + string.Format("A:  {0:dd-MM-yyyy HH:mm}", mpt.DataOraFine);
                    }
                }
            }

                                                                                                                                                                                                                                                                                                            if (mpt.Manuale == true)
            {
                bret += "";
            }
            else if (mpt.PeriodicitaGiorni != 0 || mpt.PeriodicitaOre != 0 || mpt.PeriodicitaMinuti != 0)
            {
                bret += Environment.NewLine + "Periodicità : ";
                if (mpt.PeriodicitaGiorni != 0) { bret += string.Format("{0} gg ", mpt.PeriodicitaGiorni); }
                if (mpt.PeriodicitaOre != 0) { bret += string.Format("{0} hh ", mpt.PeriodicitaOre); }
                if (mpt.PeriodicitaMinuti != 0) { bret += string.Format("{0} min ", mpt.PeriodicitaMinuti); }
            }

                                                                                                            if (mpt.Manuale == true)
            {
                bret += "";
            }
            else if (mpt.Continuita == true)
            {
                bret += Environment.NewLine + string.Format("Continua: {0} min", mpt.Durata);
            }
            return bret;

        }

        private bool Salva()
        {

            bool bReturn = true;

            frmSmartCardProgress frmSC = null;

            this.ImpostaCursore(enum_app_cursors.WaitCursor);

            try
            {

                if (_bFirmaDigitale)
                {
                                        int iCount = (dict_mpt.Count * 3) + 1;
                                        setNavigazione(false);
                    frmSC = new frmSmartCardProgress();
                    frmSC.InizializzaEMostra(0, iCount, this);
                                        frmSC.SetCursore(enum_app_cursors.WaitCursor);
                }

                foreach (KeyValuePair<string, MovPrescrizioneTempi> kvp in dict_mpt)
                {

                                        MovPrescrizioneTempi movprt = kvp.Value;

                                        if (movprt.Salva())
                    {

                        try
                        {

                            bool bContinua = true;

                            if (_bFirmaDigitale)
                            {
                                bContinua = false;
                                frmSC.SetStato(@"Validazione " + movprt.Posologia);

                                if (frmSC.TerminaOperazione)
                                {
                                                                        break;
                                }
                                else
                                {
                                                                        frmSC.SetStato(@"Generazione Documento...");

                                                                        byte[] pdfContent = CoreStatics.GeneraPDFPrescrizioneTempi(movprt.IDPrescrizioneTempi, EnumStatoPrescrizioneTempi.VA, true);

                                    if (pdfContent == null || pdfContent.Length <= 0)
                                    {
                                        frmSC.SetLog(@"Errore Generazione Documento", true);
                                    }
                                    else
                                    {
                                                                                bContinua = frmSC.ProvaAFirmare(ref pdfContent, EnumTipoDocumentoFirmato.PRTFM01, "Firma Digitale...", EnumEntita.PRT, movprt.IDPrescrizioneTempi);
                                    }
                                }
                            } 
                            if (bContinua)
                            {
                                                                movprt.CodStatoPrescrizioneTempi = EnumStatoPrescrizioneTempi.VA.ToString();
                                movprt.Azione = EnumAzioni.VAL;
                                if (movprt.Salva())
                                {
                                    movprt.CreaTaskInfermieristici(EnumCodSistema.PRF, Database.GetConfigTable(EnumConfigTable.TipoSchedaTaskDaPrescrizione), EnumTipoRegistrazione.A);
                                }

                            } 
                        }
                        catch (Exception ex)
                        {
                            bReturn = false;
                            if (frmSC != null) frmSC.SetLog(@"ERRORE " + ex.Message, true);
                        }

                    }
                    else
                    {
                        bReturn = false;
                    }

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Salva", this.Name);
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

            return bReturn;

        }

        private bool CheckDate()
        {

            bool bRet = true;

            if ((DateTime)this.udteFiltroDAFine.Value <= (DateTime)this.udteFiltroA.Value)
            {
                bRet = false;
            }

            if (bRet == true)
            {
                this.udteFiltroDAFine.Appearance.BackColor = Color.Empty;
                this.udteFiltroAFine.Appearance.BackColor = Color.Empty;
            }
            else
            {
                this.udteFiltroDAFine.Appearance.BackColor = Color.Red;
                this.udteFiltroAFine.Appearance.BackColor = Color.Red;
            }

            return bRet;

        }

        #endregion

        #region UltraGrid

        private void InizializzaUltraGridLayout()
        {
            try
            {
                CoreStatics.SetEasyUltraGridLayout(ref this.ucEasyGrid);
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaUltraGridLayout", this.Name);
            }
        }

        private void CaricaGriglia()
        {

            try
            {

                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

                dict_mpt = new Dictionary<string, MovPrescrizioneTempi>();

                                                                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Paziente.ID);
                op.Parametro.Add("IDEpisodio", CoreStatics.CoreApplication.Episodio.ID);
                op.Parametro.Add("IDTrasferimento", CoreStatics.CoreApplication.Trasferimento.ID);
                                if (this.udteFiltroDA.Value != null)
                {
                    op.Parametro.Add("DataInizio", ((DateTime)this.udteFiltroDA.Value).ToString("dd/MM/yyyy HH:mm").Replace(".", ":"));
                }
                if (this.udteFiltroA.Value != null)
                {
                    op.Parametro.Add("DataFine", ((DateTime)this.udteFiltroA.Value).ToString("dd/MM/yyyy HH:mm").Replace(".", ":"));
                }

                                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_SelMovPrescrizioniTempiProsegui", spcoll);

                DataTable dtEdit = ds.Tables[0].Copy();

                foreach (DataColumn dcCol in dtEdit.Columns)
                {
                    if (dcCol.ColumnName.ToUpper().IndexOf("PERMESSOAGGIUNGI") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOMODIFICA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOCANCELLA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("NUOVA TEMPISTICA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("NUOVA POSOLOGIA") == 0)
                        dcCol.ReadOnly = false;
                }

                if (this.ucEasyGrid.DisplayLayout != null)
                {
                    this.ucEasyGrid.DataSource = null;
                    this.ucEasyGrid.DataSource = dtEdit;
                    this.ucEasyGrid.Refresh();
                }

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "CaricaGriglia", this.Text);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
            }

        }

        private void RicalcolaGriglia()
        {

            try
            {

                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

                foreach (UltraGridRow oRow in this.ucEasyGrid.Rows)
                {

                    if (oRow.Cells["PermessoAggiungi"].Text == "0")
                    {

                        if (dict_mpt[oRow.Cells["ID"].Text].DataOraInizio != DateTime.MinValue)
                        {
                            dict_mpt[oRow.Cells["ID"].Text].DataOraInizio = new DateTime(((DateTime)udteFiltroDAFine.Value).Year,
                                                                    ((DateTime)udteFiltroDAFine.Value).Month,
                                                                    ((DateTime)udteFiltroDAFine.Value).Day,
                                                                    dict_mpt[oRow.Cells["ID"].Text].DataOraInizio.Hour,
                                                                    dict_mpt[oRow.Cells["ID"].Text].DataOraInizio.Minute,
                                                                    dict_mpt[oRow.Cells["ID"].Text].DataOraInizio.Second);
                        }
                        if (dict_mpt[oRow.Cells["ID"].Text].DataOraFine != DateTime.MinValue)
                        {
                            dict_mpt[oRow.Cells["ID"].Text].DataOraFine = new DateTime(((DateTime)udteFiltroAFine.Value).Year,
                                                                    ((DateTime)udteFiltroAFine.Value).Month,
                                                                    ((DateTime)udteFiltroAFine.Value).Day,
                                                                    dict_mpt[oRow.Cells["ID"].Text].DataOraFine.Hour,
                                                                    dict_mpt[oRow.Cells["ID"].Text].DataOraFine.Minute,
                                                                    dict_mpt[oRow.Cells["ID"].Text].DataOraFine.Second);
                        }
                        oRow.Cells["Nuova Tempistica"].Value = CreaNuovaTempistica(dict_mpt[oRow.Cells["ID"].Text]);

                    }

                }

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "RicalcolaGriglia", this.Text);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
            }

        }

        #endregion

        #region Events Form

        private void frmPUProseguiTerapia_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void frmPUProseguiTerapia_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {

            try
            {

                if (this.CheckDate() == true)
                {
                    if (dict_mpt.Count > 0)
                    {
                        if (easyStatics.EasyMessageBox("Confermi le prosecuzioni selezionate ?", "Imposta Prosecuzione Terapia", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            if (Salva())
                            {
                                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                                this.Close();
                            }
                            else
                            {
                                easyStatics.EasyMessageBox("Sono stati riscontrati errori in alcune prosecuzione!", "Imposta Prosecuzione Terapia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                                this.Close();
                            }
                        }
                    }
                    else
                    {
                        easyStatics.EasyMessageBox("Nessuna prosecuzione selezionata.", "Imposta Prosecuzione Terapia", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    easyStatics.EasyMessageBox("'Data inizio prosecuzione' deve essere maggiore di 'Data fine Intervallo' da Proseguire, impossibile validare le tempistiche!", "Imposta Prosecuzione Terapia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "frmPUProseguiTerapia_PulsanteAvantiClick", this.Text);
            }

        }

        #endregion

        #region Events

        private void udteFiltroDA_ValueChanged(object sender, EventArgs e)
        {
            if ((DateTime)this.udteFiltroDA.Value > (DateTime)this.udteFiltroA.Value)
            {
                this.udteFiltroA.Value = new DateTime(((DateTime)this.udteFiltroDA.Value).Year, ((DateTime)this.udteFiltroDA.Value).Month, ((DateTime)this.udteFiltroDA.Value).Day, 23, 59, 59);
            }
            else
            {
                if (bSkipEvent == false) { this.CaricaGriglia(); }
            }
            this.CheckDate();
        }

        private void udteFiltroA_ValueChanged(object sender, EventArgs e)
        {
            if ((DateTime)this.udteFiltroA.Value < (DateTime)this.udteFiltroDA.Value)
            {
                this.udteFiltroDA.Value = new DateTime(((DateTime)this.udteFiltroA.Value).Year, ((DateTime)this.udteFiltroA.Value).Month, ((DateTime)this.udteFiltroA.Value).Day, 0, 0, 0);
            }
            else
            {
                if (bSkipEvent == false) { this.CaricaGriglia(); }
            }
            this.CheckDate();

        }

        private void uceRangeInizio_ValueChanged(object sender, EventArgs e)
        {

            DateTime dtI = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            DateTime dtF = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);

            switch (this.uceRangeInizio.SelectedItem.DataValue.ToString())
            {
                case "Oggi":
                    break;

                case "Ieri":
                    dtI = dtI.AddDays(-1);
                    dtF = dtF.AddDays(-1);
                    break;

                case "Ultimi Due Giorni":
                    dtI = dtI.AddDays(-2);
                    dtF = dtF.AddDays(-2);
                    break;

                case "Domani":
                    dtI = dtI.AddDays(1);
                    dtF = dtF.AddDays(1);
                    break;

                case "Ultima Settimana":
                    dtI = dtI.AddDays(-7);
                    dtF = dtF.AddDays(-7);
                    break;
            }

            bSkipEvent = true;
            this.udteFiltroDA.Value = dtI;
            this.udteFiltroA.Value = dtF;
            bSkipEvent = false;

            this.CaricaGriglia();
        }

        private void udteFiltroDAFine_ValueChanged(object sender, EventArgs e)
        {
            if ((DateTime)this.udteFiltroDAFine.Value > (DateTime)this.udteFiltroAFine.Value)
            {
                this.udteFiltroAFine.Value = new DateTime(((DateTime)this.udteFiltroDAFine.Value).Year, ((DateTime)this.udteFiltroDAFine.Value).Month, ((DateTime)this.udteFiltroDAFine.Value).Day, 23, 59, 59);
            }
            else
            {
                if (bSkipEvent == false) { this.RicalcolaGriglia(); }
            }
            this.CheckDate();
        }

        private void udteFiltroAFine_ValueChanged(object sender, EventArgs e)
        {
            if ((DateTime)this.udteFiltroAFine.Value < (DateTime)this.udteFiltroDAFine.Value)
            {
                this.udteFiltroDAFine.Value = new DateTime(((DateTime)this.udteFiltroAFine.Value).Year, ((DateTime)this.udteFiltroAFine.Value).Month, ((DateTime)this.udteFiltroAFine.Value).Day, 0, 0, 0);
            }
            else
            {
                if (bSkipEvent == false) { this.RicalcolaGriglia(); }
            }
            this.CheckDate();
        }

        private void uceRangeFine_ValueChanged(object sender, EventArgs e)
        {

            DateTime dtI = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            DateTime dtF = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);

            switch (this.uceRangeFine.SelectedItem.DataValue.ToString())
            {

                case "Oggi1":
                    dtI = dtI.AddDays(1);
                    dtF = dtF.AddDays(1);
                    break;

                case "Oggi2":
                    dtI = dtI.AddDays(2);
                    dtF = dtF.AddDays(2);
                    break;

                case "Oggi3":
                    dtI = dtI.AddDays(3);
                    dtF = dtF.AddDays(3);
                    break;

            }

            bSkipEvent = true;
            this.udteFiltroDAFine.Value = dtI;
            this.udteFiltroAFine.Value = dtF;
            bSkipEvent = false;

            this.RicalcolaGriglia();

        }

        private void ucEasyGrid_ClickCell(object sender, Infragistics.Win.UltraWinGrid.ClickCellEventArgs e)
        {

            try
            {

                switch (e.Cell.Column.Key)
                {

                    case "Terapia":
                        CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerTerapia);
                        _ucRichTextBox = CoreStatics.GetRichTextBoxForPopupControlContainer(e.Cell.Row.Cells["TerapiaRTF"].Text);
                        Infragistics.Win.UIElement uie = e.Cell.GetUIElement();
                        Point oPoint = new Point(uie.Rect.Left, uie.Rect.Top);
                        this.UltraPopupControlContainerTerapia.Show(this.ucEasyGrid, oPoint);
                        break;

                    case "Tempistica":
                        this.PreparaGridValidati((UltraGrid)sender, e.Cell);
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

        private void ucEasyGrid_ClickCellButton(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {

            try
            {

                switch (e.Cell.Column.Key)
                {

                    case CoreStatics.C_COL_BTN_ADD:
                        MovPrescrizioneTempi oMptOrigine = new MovPrescrizioneTempi(e.Cell.Row.Cells["ID"].Text, e.Cell.Row.Cells["IDPrescrizione"].Text, CoreStatics.CoreApplication.Ambiente);
                        MovPrescrizioneTempi oMpt = new MovPrescrizioneTempi(new Guid(), CoreStatics.CoreApplication.Ambiente);
                                                if (oMptOrigine.DataOraInizio != DateTime.MinValue)
                        {
                            oMptOrigine.DataOraInizio = new DateTime(((DateTime)udteFiltroDAFine.Value).Year,
                                                                    ((DateTime)udteFiltroDAFine.Value).Month,
                                                                    ((DateTime)udteFiltroDAFine.Value).Day,
                                                                    oMptOrigine.DataOraInizio.Hour,
                                                                    oMptOrigine.DataOraInizio.Minute,
                                                                    oMptOrigine.DataOraInizio.Second);
                        }
                        if (oMptOrigine.DataOraFine != DateTime.MinValue)
                        {
                            oMptOrigine.DataOraFine = new DateTime(((DateTime)udteFiltroAFine.Value).Year,
                                                                    ((DateTime)udteFiltroAFine.Value).Month,
                                                                    ((DateTime)udteFiltroAFine.Value).Day,
                                                                    oMptOrigine.DataOraFine.Hour,
                                                                    oMptOrigine.DataOraFine.Minute,
                                                                    oMptOrigine.DataOraFine.Second);
                        }
                        oMpt.CopiaDaOrigine(oMptOrigine);
                                                oMpt.InMemoria = true;
                        dict_mpt.Add(e.Cell.Row.Cells["ID"].Text, oMpt);
                        e.Cell.Row.Cells["Nuova Tempistica"].Value = CreaNuovaTempistica(oMpt);
                        e.Cell.Row.Cells["Nuova Posologia"].Value = oMpt.Posologia;
                        e.Cell.Row.Cells["PermessoAggiungi"].Value = 0;
                        e.Cell.Row.Cells["PermessoModifica"].Value = 1;
                        e.Cell.Row.Cells["PermessoCancella"].Value = 1;
                        e.Cell.Row.Update();
                        break;

                    case CoreStatics.C_COL_BTN_EDIT:
                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = new MovPrescrizione(e.Cell.Row.Cells["IDPrescrizione"].Text, CoreStatics.CoreApplication.Ambiente);
                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata = dict_mpt[e.Cell.Row.Cells["ID"].Text];
                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Azione = EnumAzioni.MOD;
                        if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingPrescrizioneTempi) == DialogResult.OK)
                        {
                            dict_mpt[e.Cell.Row.Cells["ID"].Text] = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata;
                            e.Cell.Row.Cells["Nuova Tempistica"].Value = CreaNuovaTempistica(CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata);
                            e.Cell.Row.Cells["Nuova Posologia"].Value = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Posologia;
                        }
                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata = null;
                        break;

                    case CoreStatics.C_COL_BTN_DEL:
                        dict_mpt.Remove(e.Cell.Row.Cells["ID"].Text);
                        e.Cell.Row.Cells["Nuova Tempistica"].Value = string.Empty;
                        e.Cell.Row.Cells["Nuova Posologia"].Value = string.Empty;
                        e.Cell.Row.Cells["PermessoAggiungi"].Value = 1;
                        e.Cell.Row.Cells["PermessoModifica"].Value = 0;
                        e.Cell.Row.Cells["PermessoCancella"].Value = 0;
                        e.Cell.Row.Update();
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

        private void ucEasyGrid_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {

                int refWidth = (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small) * 3;

                Graphics g = this.CreateGraphics();
                int refBtnWidth = (CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(this.ucEasyGrid.DataRowFontRelativeDimension), g.DpiY) * 3);
                g.Dispose();
                g = null;
                                e.Layout.Bands[0].HeaderVisible = false;
                e.Layout.Bands[0].ColHeadersVisible = true;
                e.Layout.Bands[0].RowLayoutStyle = RowLayoutStyle.GroupLayout;
                                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {

                    try
                    {

                        oCol.SortIndicator = SortIndicator.Disabled;
                        switch (oCol.Key)
                        {

                            case "Terapia":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = refWidth * 5;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.RowLayoutColumnInfo.OriginX = 0;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;
                                break;

                            case "Tempistica":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = refWidth * 8;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 5;
                                break;

                            case "Posologia":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = refWidth * 5;
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

                            case CoreStatics.C_COL_BTN_ADD:
                                oCol.Hidden = false;
                                oCol.Header.Caption = "";
                                                                oCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                                oCol.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                                oCol.CellActivation = Activation.AllowEdit;
                                oCol.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                                oCol.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_NUOVO_32);
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                try
                                {
                                    oCol.MinWidth = refBtnWidth;
                                    oCol.MaxWidth = oCol.MinWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.LockedWidth = true;
                                oCol.RowLayoutColumnInfo.OriginX = 3;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 3;
                                break;

                            case "Nuova Tempistica":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = refWidth * 8;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.RowLayoutColumnInfo.OriginX = 4;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 5;
                                break;

                            case "Nuova Posologia":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = refWidth * 5;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.RowLayoutColumnInfo.OriginX = 5;
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

                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_EDIT))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_EDIT);
                    colEdit.Hidden = false;
                    colEdit.Header.Caption = "";
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
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
                }
                                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_EDIT + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_EDIT + @"_SP");
                    colEdit.Hidden = false;
                    colEdit.Header.Caption = "";
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

                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_DEL))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_DEL);
                    colEdit.Hidden = false;
                    colEdit.Header.Caption = "";
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
                    colEdit.RowLayoutColumnInfo.OriginX = 8;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_DEL + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_DEL + @"_SP");
                    colEdit.Hidden = false;
                    colEdit.Header.Caption = "";
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

                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_SPAZIO))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_SPAZIO);
                    colEdit.Hidden = false;
                    colEdit.Header.Caption = "";
                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                    colEdit.CellActivation = Activation.Disabled;
                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MaxWidth = this.ucEasyGrid.Width - (refWidth * 31) - Convert.ToInt32(refBtnWidth * 3.5) - 30;
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                    colEdit.LockedWidth = true;
                    colEdit.RowLayoutColumnInfo.OriginX = 10;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGrid_InitializeLayout", this.Name);
            }

        }

        private void ucEasyGrid_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {

            try
            {

                foreach (UltraGridCell ocell in e.Row.Cells)
                {

                                        
                    if (ocell.Column.Key == CoreStatics.C_COL_BTN_ADD)
                    {
                        ocell.Style = (ocell.Row.Cells["PermessoAggiungi"].Value.ToString() == "0" ? Infragistics.Win.UltraWinGrid.ColumnStyle.Integer : Infragistics.Win.UltraWinGrid.ColumnStyle.Button);
                    }

                    if (ocell.Column.Key == CoreStatics.C_COL_BTN_EDIT)
                    {
                        ocell.Style = (ocell.Row.Cells["PermessoModifica"].Value.ToString() == "0" ? Infragistics.Win.UltraWinGrid.ColumnStyle.Integer : Infragistics.Win.UltraWinGrid.ColumnStyle.Button);
                    }

                    if (ocell.Column.Key == CoreStatics.C_COL_BTN_DEL)
                    {
                        ocell.Style = (ocell.Row.Cells["PermessoCancella"].Value.ToString() == "0" ? Infragistics.Win.UltraWinGrid.ColumnStyle.Integer : Infragistics.Win.UltraWinGrid.ColumnStyle.Button);
                    }

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGrid_InitializeRow", this.Name);
            }

        }

        #endregion

        #region UltraPopupControlContainerTerapia

        private void UltraPopupControlContainerTerapia_Closed(object sender, EventArgs e)
        {
            _ucRichTextBox.RtfClick -= ucRichTextBox_Click;
        }

        private void UltraPopupControlContainerTerapia_Opened(object sender, EventArgs e)
        {
            _ucRichTextBox.RtfClick += ucRichTextBox_Click;
            _ucRichTextBox.Focus();
        }

        private void UltraPopupControlContainerTerapia_Opening(object sender, CancelEventArgs e)
        {
            Infragistics.Win.Misc.UltraPopupControlContainer popup = sender as Infragistics.Win.Misc.UltraPopupControlContainer;
            popup.PopupControl = _ucRichTextBox;
        }

        private void ucRichTextBox_Click(object sender, EventArgs e)
        {
                    }

        #endregion

        #region UltraPopupControlContainerTempistica

        private void UltraPopupControlContainerTempistica_Closed(object sender, EventArgs e)
        {
        }

        private void UltraPopupControlContainerTempistica_Opened(object sender, EventArgs e)
        {
            _ucEasyGridOrari.ClickCell += ucEasyGridOrari_ClickCell;
            _ucEasyGridOrari.Focus();
        }

        private void UltraPopupControlContainerTempistica_Opening(object sender, CancelEventArgs e)
        {
            Infragistics.Win.Misc.UltraPopupControlContainer popup = sender as Infragistics.Win.Misc.UltraPopupControlContainer;
            popup.PopupControl = _ucEasyGridOrari;
        }

        private void ucEasyGridOrari_ClickCell(object sender, ClickCellEventArgs e)
        {
            this.UltraPopupControlContainerTempistica.Close();
        }

        private void PreparaGridValidati(UltraGrid Grid, UltraGridCell Cell)
        {

            int altezza = 4;
            int larghezza = 6;

            try
            {

                _ucEasyGridOrari = null;
                _ucEasyGridOrari = CoreStatics.getGridOrariValidati(Cell.Row.Cells["ID"].Text, Cell.Row.Cells["IDPrescrizione"].Text);

                if (_ucEasyGridOrari != null && _ucEasyGridOrari.DataSource != null)
                {
                    
                    _ucEasyGridOrari.Size = new Size(Grid.Width * larghezza / 10, Grid.Height * altezza / 8);

                    _ucEasyGridOrari.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;

                    CoreStatics.SetEasyUltraGridLayout(ref _ucEasyGridOrari);
                    _ucEasyGridOrari.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.True;

                                        CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerTempistica);

                    Infragistics.Win.UIElement uie = Cell.GetUIElement();
                    Point oPoint = new Point(uie.Rect.Left, uie.Rect.Top);

                    this.UltraPopupControlContainerTempistica.Show(Grid, Grid.PointToScreen(oPoint));

                                        _ucEasyGridOrari.DisplayLayout.Bands[0].ClearGroupByColumns();

                    foreach (UltraGridColumn oCol in _ucEasyGridOrari.DisplayLayout.Bands[0].Columns)
                    {

                        oCol.SortIndicator = SortIndicator.Disabled;
                        switch (oCol.Key)
                        {
                            case "DataRiferimento":
                                oCol.Hidden = false;
                                oCol.Header.Caption = "Data";
                                oCol.Format = "dddd dd/MM/yyyy HH:mm";
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.Header.VisiblePosition = 1;
                                break;

                            case "DescStato":
                                oCol.Hidden = false;
                                oCol.Header.Caption = "Stato";
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.Header.VisiblePosition = 2;
                                break;

                            case "DescTipo":
                                oCol.Hidden = false;
                                oCol.Header.Caption = "Tipo";
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.Header.VisiblePosition = 3;
                                break;

                            default:
                                oCol.Hidden = true;
                                break;
                        }
                    }

                                        _ucEasyGridOrari.DisplayLayout.Bands[0].Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
                    _ucEasyGridOrari.Refresh();

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "PreparaGridValidati", this.Name);
            }

        }

        #endregion

    }
}
