using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;

using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;
using UnicodeSrl.Framework.Data;
using System.Threading;

namespace UnicodeSrl.ScciCore
{
    public partial class ucRicercaSAC : UserControl, Interfacce.IViewUserControlMiddle
    {

        const int c_minletterericerca = 2;
        private List<PazienteSac> _ListPazienteSac = null;
        private UserControl _ucc = null;

        public ucRicercaSAC()
        {
            InitializeComponent();

            _ucc = (UserControl)this;
        }

        #region PROPS

        public easyStatics.easyRelativeDimensions TextBoxFontRelativeDimension
        {
            get
            {
                return this.utxtCognome.TextFontRelativeDimension;
            }
            set
            {
                this.utxtCognome.TextFontRelativeDimension = value;
                this.utxtNome.TextFontRelativeDimension = value;
                this.utxtLuogoNascita.TextFontRelativeDimension = value;
                this.utxtCF.TextFontRelativeDimension = value;
                this.dteDataNascita.TextFontRelativeDimension = value;
            }
        }

        public easyStatics.easyRelativeDimensions LabelsFontRelativeDimension
        {
            get
            {
                return this.lblCognome.TextFontRelativeDimension;
            }
            set
            {
                this.lblCognome.TextFontRelativeDimension = value;
                this.lblNome.TextFontRelativeDimension = value;
                this.lblLuogoNascita.TextFontRelativeDimension = value;
                this.lblCF.TextFontRelativeDimension = value;
                this.lblDataNascita.TextFontRelativeDimension = value;
            }
        }

        public easyStatics.easyRelativeDimensions GridDataRowFontRelativeDimension
        {
            get
            {
                return this.ucEasyGrid.DataRowFontRelativeDimension;
            }
            set
            {
                this.ucEasyGrid.DataRowFontRelativeDimension = value;
                this.ucEasyGridRecenti.DataRowFontRelativeDimension = value;
            }
        }

        public easyStatics.easyRelativeDimensions GridHeaderFontRelativeDimension
        {
            get
            {
                return this.ucEasyGrid.HeaderFontRelativeDimension;
            }
            set
            {
                this.ucEasyGrid.HeaderFontRelativeDimension = value;
                this.ucEasyGridRecenti.HeaderFontRelativeDimension = value;
            }
        }

        public easyStatics.easyRelativeDimensions ButtonFontRelativeDimension
        {
            get
            {
                return this.ubRicerca.TextFontRelativeDimension;
            }
            set
            {
                this.ubRicerca.TextFontRelativeDimension = value;
            }
        }

        public Keys ButtonShortcutKey
        {
            get
            {
                return this.ubRicerca.ShortcutKey;
            }
            set
            {
                this.ubRicerca.ShortcutKey = value;
            }
        }

        public PazienteSac RigaPazienteSelezionato
        {
            get
            {
                if (_ListPazienteSac != null && this.ucEasyGrid.ActiveRow != null)
                {

                    try
                    {
                        PazienteSac oPazienteSac = _ListPazienteSac.Find(PazienteSac => PazienteSac.CodSAC == this.ucEasyGrid.ActiveRow.Cells["CodSAC"].Text.ToString());
                        return oPazienteSac;
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                else if (_ListPazienteSac == null && this.ucEasyGrid.ActiveRow != null && this.uchkSoloPazientiSeguiti.Checked == true)
                {

                    try
                    {
                        PazienteSac oPazienteSac = DBUtils.get_RicercaPazientiSACByID(this.ucEasyGrid.ActiveRow.Cells["CodSAC"].Text.ToString());
                        return oPazienteSac;
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                else if (_ListPazienteSac == null && this.ucEasyGrid.ActiveRow != null && this.uchkSoloPazientiSeguiti.Checked == false && this.ucEasyComboEditorFiltriSpeciali.Text.Trim() != CoreStatics.GC_TUTTI)
                {

                    try
                    {
                        PazienteSac oPazienteSac = DBUtils.get_RicercaPazientiSACByID(this.ucEasyGrid.ActiveRow.Cells["CodSAC"].Text.ToString());
                        return oPazienteSac;
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                else if (this.ucEasyGridRecenti.ActiveRow != null)
                {

                    try
                    {
                        PazienteSac oPazienteSac = DBUtils.get_RicercaPazientiSACByID(this.ucEasyGridRecenti.ActiveRow.Cells["CodSAC"].Text.ToString());
                        return oPazienteSac;
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region INTERFACCIA

        public void Aggiorna()
        {
            CaricaGriglia(true);

        }

        public void Carica()
        {
            CaricaGriglia(false);
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

        internal void setFocusDefault()
        {
            try
            {
                this.utxtCognome.Focus();
                this.utxtCognome.SelectAll();
            }
            catch
            {
            }
        }

        #endregion

        #region PRIVATE

        private void CaricaGriglia(bool applicafiltri)
        {

            Parametri op = null;
            SqlParameterExt[] spcoll = null;
            string xmlParam = string.Empty;


            if (Interlocked.Equals(Maschere._navigare, 0))
            {
                return;
            }


            CoreStatics.SetNavigazione(false);

            CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

            try
            {

                op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodUtente", CoreStatics.CoreApplication.Sessione.Utente.Codice);

                spcoll = new SqlParameterExt[1];

                xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovPazientiRecenti", spcoll);

                this.ucEasyGridRecenti.Visible = (dt.Rows.Count == 0 ? false : true);
                this.ucEasyGridRecenti.DataSource = dt;
                this.ucEasyGridRecenti.Refresh();
                this.ucEasyGridRecenti.Text = string.Format("Ultimi {0} pazienti ricercati.", dt.Rows.Count);

                bool bnorecords = true;

                DateTime dataNasc = DateTime.MinValue;

                if (applicafiltri)
                {
                    if (this.uchkSoloPazientiSeguiti.Checked == false && this.ucEasyComboEditorFiltriSpeciali.Text.Trim() == CoreStatics.GC_TUTTI)
                    {

                        if (criteriminimidiricerca())
                        {
                            bnorecords = false;

                            if (this.dteDataNascita.Value != null) dataNasc = (DateTime)this.dteDataNascita.Value;

                        }

                    }
                    else
                    {
                        bnorecords = false;

                        if (this.dteDataNascita.Value != null) dataNasc = (DateTime)this.dteDataNascita.Value;

                    }
                }
                else
                {
                    op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                    op.Parametro.Add("CodTipoFiltroSpeciale", EnumTipoFiltroSpeciale.PAZAMB.ToString());
                    op.Parametro.Add("CodUA", CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.CodUAAmbulatorialeSelezionata);

                    spcoll = new SqlParameterExt[1];

                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    dt = Database.GetDataTableStoredProc("MSP_SelFiltriSpeciali", spcoll);

                    this.ucEasyComboEditorFiltriSpeciali.ValueMember = "Codice";
                    this.ucEasyComboEditorFiltriSpeciali.DisplayMember = "Descrizione";
                    this.ucEasyComboEditorFiltriSpeciali.DataSource = CoreStatics.AggiungiTuttiDataTable(dt, false);
                    this.ucEasyComboEditorFiltriSpeciali.Refresh();
                    this.ucEasyComboEditorFiltriSpeciali.SelectedIndex = 0;
                }


                if (bnorecords)
                {
                    this.ucEasyGrid.DataSource = null;
                    this.ucEasyGrid.Refresh();
                }
                else
                {
                    if (this.uchkSoloPazientiSeguiti.Checked == false && this.ucEasyComboEditorFiltriSpeciali.Text.Trim() == CoreStatics.GC_TUTTI)
                    {
                        _ListPazienteSac = DBUtils.get_RicercaPazientiSAC(this.utxtCognome.Text, this.utxtNome.Text, dataNasc, this.utxtLuogoNascita.Text, this.utxtCF.Text);

                        DataSet oDs = new DataSet();
                        DataTable oDt = CoreStatics.CreateDataTable<PazienteSac>();
                        CoreStatics.FillDataTable<PazienteSac>(oDt, _ListPazienteSac);

                        oDs.Tables.Add(oDt);

                        oDs.Tables[0].DefaultView.Sort = "Paziente";

                        this.ucEasyGrid.DataSource = oDt;

                        this.ucEasyGrid.Refresh();
                    }
                    else if (this.uchkSoloPazientiSeguiti.Checked == false && this.ucEasyComboEditorFiltriSpeciali.Text.Trim() != CoreStatics.GC_TUTTI)
                    {

                        op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                        op.Parametro.Add("CodUtente", CoreStatics.CoreApplication.Sessione.Utente.Codice);
                        op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);

                        if (this.utxtCognome.Text != string.Empty)
                        {
                            op.Parametro.Add("Cognome", this.utxtCognome.Text);
                        }
                        if (this.utxtNome.Text != string.Empty)
                        {
                            op.Parametro.Add("Nome", this.utxtNome.Text);
                        }
                        if (this.utxtLuogoNascita.Text != string.Empty)
                        {
                            op.Parametro.Add("LuogoNascita", this.utxtLuogoNascita.Text);
                        }
                        if (this.utxtCF.Text != string.Empty)
                        {
                            op.Parametro.Add("CodiceFiscale", this.utxtCF.Text);
                        }

                        if (dataNasc != DateTime.MinValue)
                        {
                            op.Parametro.Add("DataNascita", dataNasc.ToString("dd/MM/yyyy"));
                        }

                        if (this.ucEasyComboEditorFiltriSpeciali.Text.Trim() != CoreStatics.GC_TUTTI)
                        {
                            op.Parametro.Add("CodFiltroSpeciale", this.ucEasyComboEditorFiltriSpeciali.Value.ToString());
                        }

                        op.TimeStamp.CodEntita = EnumEntita.PZS.ToString();
                        op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                        spcoll = new SqlParameterExt[1];

                        xmlParam = XmlProcs.XmlSerializeToString(op);
                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                        DataSet ds = Database.GetDatasetStoredProc("MSP_SelPazientiFiltriSpeciali", spcoll);

                        this.ucEasyGrid.DataSource = ds;

                        this.ucEasyGrid.Refresh();

                    }
                    else
                    {

                        op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                        op.Parametro.Add("CodUtente", CoreStatics.CoreApplication.Sessione.Utente.Codice);
                        op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                        op.Parametro.Add("PazientiSeguitiDaUtente", "1");
                        op.Parametro.Add("PazientiSeguitiDaAltri", "0");

                        if (this.utxtCognome.Text != string.Empty)
                        {
                            op.Parametro.Add("Cognome", this.utxtCognome.Text);
                        }
                        if (this.utxtNome.Text != string.Empty)
                        {
                            op.Parametro.Add("Nome", this.utxtNome.Text);
                        }
                        if (this.utxtLuogoNascita.Text != string.Empty)
                        {
                            op.Parametro.Add("LuogoNascita", this.utxtLuogoNascita.Text);
                        }
                        if (this.utxtCF.Text != string.Empty)
                        {
                            op.Parametro.Add("CodiceFiscale", this.utxtCF.Text);
                        }

                        if (dataNasc != DateTime.MinValue)
                        {
                            op.Parametro.Add("DataNascita", dataNasc.ToString("dd/MM/yyyy"));
                        }

                        if (this.ucEasyComboEditorFiltriSpeciali.Text.Trim() != CoreStatics.GC_TUTTI)
                        {
                            op.Parametro.Add("CodFiltroSpeciale", this.ucEasyComboEditorFiltriSpeciali.Value.ToString());
                        }

                        op.TimeStamp.CodEntita = EnumEntita.PZS.ToString();
                        op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                        spcoll = new SqlParameterExt[1];

                        xmlParam = XmlProcs.XmlSerializeToString(op);
                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                        DataSet ds = Database.GetDatasetStoredProc("MSP_SelMovPazientiSeguiti", spcoll);

                        this.ucEasyGrid.DataSource = ds;

                        this.ucEasyGrid.Refresh();
                    }

                    int nRec = int.Parse(Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceSACNumRecords));
                    if (nRec > 0 && nRec == this.ucEasyGrid.Rows.Count)
                    {
                        this.ucEasyGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.True;
                        this.ucEasyGrid.DisplayLayout.CaptionAppearance.ForeColor = Color.Red;
                        this.ucEasyGrid.Text = "*** Numero casi troppo elevato, visualizzate solo le prime " + nRec.ToString() + " righe. Si consiglia di affinare il filtro. *** ";
                    }
                    else
                    {
                        this.ucEasyGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
                    }

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaGriglia", this.Name);
                CoreStatics.SetNavigazione(true);
            }

            CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
            CoreStatics.SetNavigazione(true);

            setFocusDefault();
        }

        private bool controllaFiltri()
        {
            bool bReturn = true;


            if (this.uchkSoloPazientiSeguiti.Checked == false && this.ucEasyComboEditorFiltriSpeciali.Text.Trim() == CoreStatics.GC_TUTTI)
            {

                if (bReturn)
                {
                    if (this.utxtCF.Text.Trim().Length < 16 && this.utxtCognome.Text.Trim().Length < c_minletterericerca)
                    {
                        bReturn = false;
                        easyStatics.EasyMessageBox("Inserire almeno " + c_minletterericerca.ToString() + @" lettere per la ricerca del Cognome!", "Ricerca SAC", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.utxtCognome.Focus();
                    }
                }

                if (bReturn)
                {
                    if (this.utxtCF.Text.Trim().Length < 16 && this.utxtNome.Text.Trim().Length < c_minletterericerca)
                    {
                        bReturn = false;
                        easyStatics.EasyMessageBox("Inserire almeno " + c_minletterericerca.ToString() + @" lettere per la ricerca del Nome!", "Ricerca SAC", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.utxtNome.Focus();
                    }
                }

                if (bReturn)
                {
                    if (this.utxtCF.Text.Trim().Length < 16 && this.utxtLuogoNascita.Text.Trim().Length > 0 && this.utxtLuogoNascita.Text.Trim().Length < c_minletterericerca)
                    {
                        bReturn = false;
                        easyStatics.EasyMessageBox("Inserire almeno " + c_minletterericerca.ToString() + @" lettere per la ricerca del Luogo Nascita!", "Ricerca SAC", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.utxtLuogoNascita.Focus();
                    }
                }

                if (bReturn)
                {
                    if (this.utxtCF.Text.Trim().Length > 0 && this.utxtCF.Text.Trim().Length < c_minletterericerca)
                    {
                        bReturn = false;
                        easyStatics.EasyMessageBox("Inserire almeno " + c_minletterericerca.ToString() + @" lettere per la ricerca del Codice Fiscale!", "Ricerca SAC", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.utxtCF.Focus();
                    }
                }

                if (bReturn)
                {
                    if (!criteriminimidiricerca())
                    {
                        easyStatics.EasyMessageBox("Inserire più criteri per la ricerca!", "Ricerca SAC", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        bReturn = false;
                    }
                }

            }

            return bReturn;
        }

        private bool criteriminimidiricerca()
        {
            bool breturn = false;
            int iCriteri = 0;

            if (this.utxtCognome.Text.Trim().Length >= c_minletterericerca) iCriteri += 1;
            if (this.utxtNome.Text.Trim().Length >= c_minletterericerca) iCriteri += 1;
            if (this.utxtCF.Text.Trim().Length >= c_minletterericerca) iCriteri += 1;
            if (this.utxtLuogoNascita.Text.Trim().Length >= c_minletterericerca) iCriteri += 1;
            if (this.dteDataNascita.Value != null) iCriteri += 1;
            if (this.utxtCF.Text.Trim().Length >= 16) iCriteri += 100;

            breturn = (iCriteri > 1);

            return breturn;
        }

        #endregion

        #region EVENTI

        private void utxt_Enter(object sender, EventArgs e)
        {
            ((ucEasyTextBox)sender).SelectAll();
        }

        private void uchkSoloPazientiSeguiti_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void ubRicerca_Click(object sender, EventArgs e)
        {
            if (Interlocked.Equals(Maschere._navigare, 0))
            {
                return;
            }

            if (controllaFiltri()) CaricaGriglia(true);
        }

        private void ucEasyGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridColumn oCol in e.Layout.Bands[0].Columns)
            {
                switch (oCol.Key)
                {

                    case @"Paziente":
                        oCol.Header.Caption = "Paziente";
                        oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                        break;

                    case @"Sesso":
                        oCol.Header.Caption = "Sesso";
                        oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                        break;


                    case @"NascitaDescrizione":
                        oCol.Header.Caption = "Data Luogo Nascita";
                        oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                        oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                        oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                        oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                        break;

                    case @"CodiceFiscale":
                        oCol.Header.Caption = "C.F.";
                        oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                        break;


                    case @"ComuneResidenza":
                        oCol.Header.Caption = "Residente a";
                        oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                        break;

                    default:
                        oCol.Hidden = true;
                        break;
                }
            }

            e.Layout.Bands[0].Columns["Paziente"].Header.VisiblePosition = 0;
            e.Layout.Bands[0].Columns["Sesso"].Header.VisiblePosition = 1;
            e.Layout.Bands[0].Columns["NascitaDescrizione"].Header.VisiblePosition = 2;
            e.Layout.Bands[0].Columns["CodiceFiscale"].Header.VisiblePosition = 3;
            e.Layout.Bands[0].Columns["ComuneResidenza"].Header.VisiblePosition = 4;
        }

        private void utxtCognome_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) ubRicerca_Click(this.ubRicerca, new EventArgs());
        }

        private void ucEasyGrid_AfterRowActivate(object sender, EventArgs e)
        {
            this.ucEasyGridRecenti.ActiveRow = null;
            this.ucEasyGridRecenti.Selected.Rows.Clear();
        }

        private void ucEasyGridRecenti_AfterRowActivate(object sender, EventArgs e)
        {
            this.ucEasyGrid.ActiveRow = null;
            this.ucEasyGrid.Selected.Rows.Clear();
        }

        #endregion

    }
}
