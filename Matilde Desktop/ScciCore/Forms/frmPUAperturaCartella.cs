using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Data.SqlClient;
using System.Globalization;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.ScciResource;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;
namespace UnicodeSrl.ScciCore
{
    public partial class frmPUAperturaCartella : frmBaseModale, Interfacce.IViewFormlModal
    {
        public frmPUAperturaCartella()
        {
            InitializeComponent();
        }

        #region Declare

        private const string C_OPT_COLLEGA = "C";
        private const string C_OPT_NUOVA = "N";
        Form _f = null;

        #endregion

        #region Interface

        public void Carica()
        {
            try
            {

                _f = (Form)this;

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = Risorse.GetIconFromResource(Risorse.GC_NOTAAGGIUNGI);

                this.InizilizzaControlli();

                this.ShowDialog();

            }
            catch (Exception)
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }
        }

        public bool CollegaCartella
        {
            get
            {
                if (this.optCollegaCartella.Visible
                    && this.optCollegaCartella.CheckedItem != null
                    && this.optCollegaCartella.CheckedItem.DataValue.ToString() == C_OPT_COLLEGA)
                    return true;
                else
                    return false;
            }
        }

        #endregion

        #region SubRoutine

        private void InizilizzaControlli()
        {
            CoreStatics.CoreApplication.CartellaCollegabileSelezionata = null;
            bool bAbilitaCollegaCartella = false;

            Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
            op.Parametro.Add("IDCartella", "");
            op.TimeStamp.CodRuolo = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice;
            op.TimeStamp.CodLogin = CoreStatics.CoreApplication.Sessione.Utente.Codice;

            SqlParameterExt[] spcoll = new SqlParameterExt[1];

            string xmlParam = XmlProcs.XmlSerializeToString(op);
            spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

            DataTable oDt = Database.GetDataTableStoredProc("MSP_SelMovCartelleCollegabili", spcoll);

            if (oDt != null)
            {
                bAbilitaCollegaCartella = (oDt.Rows.Count > 0);
                oDt.Dispose();
                oDt = null;
            }

            this.ultraStatusBar.Visible = false;

            InizializzaNuovaCartella(bAbilitaCollegaCartella);
            InizializzaCollegaCartella(bAbilitaCollegaCartella);


        }

        private void InizializzaNuovaCartella(bool abilitaCollegaCartella)
        {
            bool bNumeroGiaUsato = false;
            this.txtCollegaNuova.Text = "";
            this.lblAttenzioneCollega.Visible = false;

            this.lblPaziente.Text = CoreStatics.CoreApplication.Paziente.Cognome.ToUpper() + " " +
    CoreStatics.CoreApplication.Paziente.Nome.ToUpper() +
    " (" + CoreStatics.CoreApplication.Paziente.Sesso.ToUpper() + ")" + Environment.NewLine;
            this.lblPaziente.Text += "nato/a il " + CoreStatics.CoreApplication.Paziente.DataNascita.ToString("dd/MM/yyyy") + " a " +
                CoreStatics.CoreApplication.Paziente.ComuneNascita + Environment.NewLine;
            this.lblPaziente.Text += "ricoverato/trasferito in " + CoreStatics.CoreApplication.Trasferimento.Descrizione + Environment.NewLine;
            this.lblPaziente.Text += "il " + CoreStatics.CoreApplication.Trasferimento.DataIngresso.ToString("dd/MM/yyyy") +
                " alle " + CoreStatics.CoreApplication.Trasferimento.DataIngresso.ToString("HH:mm");

            Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
            op.Parametro.Add("CodUA", CoreStatics.CoreApplication.Trasferimento.CodUA);
            op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Paziente.ID);
            op.Parametro.Add("IDEpisodio", CoreStatics.CoreApplication.Episodio.ID);
            op.Parametro.Add("IDTrasferimento", CoreStatics.CoreApplication.Trasferimento.ID);

            op.TimeStamp.CodEntita = EnumEntita.CAR.ToString();

            SqlParameterExt[] spcoll = new SqlParameterExt[1];

            string xmlParam = XmlProcs.XmlSerializeToString(op);
            spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

            DataSet ds = Database.GetDatasetStoredProc("MSP_SelNumeroCartella", spcoll);

            if (ds.Tables[0].Rows.Count > 0)
            {
                bNumeroGiaUsato = Convert.ToBoolean(ds.Tables[0].Rows[0]["Usato"]);

                this.utxtNumeroCartella.Text = ds.Tables[0].Rows[0]["NumeroCartella"].ToString();
                this.lblAttenzione.Visible = bNumeroGiaUsato;
                this.lblAnnoCartella.Visible = !bNumeroGiaUsato;

                if (bNumeroGiaUsato)
                    this.lblAnnoCartella.Text = string.Empty;
                else
                    this.lblAnnoCartella.Text = "/" + DateTime.Now.Year.ToString();

                this.utxtNumeroCartella.ReadOnly = bNumeroGiaUsato;


                if (abilitaCollegaCartella)
                {
                    this.txtCollegaNuova.Text = ds.Tables[0].Rows[0]["NumeroCartella"].ToString();
                    this.lblAttenzioneCollega.Visible = bNumeroGiaUsato;
                    this.lblAnnoCartellaCollega.Visible = !bNumeroGiaUsato;

                    if (bNumeroGiaUsato)
                        this.lblAnnoCartellaCollega.Text = string.Empty;
                    else
                        this.lblAnnoCartellaCollega.Text = "/" + DateTime.Now.Year.ToString();

                    this.txtCollegaNuova.ReadOnly = bNumeroGiaUsato;

                    this.lblAttenzioneCollega.Text = "ATTENZIONE ! Paziente già preso in carico nell'episodio." + Environment.NewLine + "Sarà mantenuto lo stesso numero di cartella clinica.";
                }


            }

            this.lblAttenzione.Text = "ATTENZIONE ! Paziente già preso in carico nell'episodio." + Environment.NewLine + "Sarà mantenuto lo stesso numero di cartella clinica.";


        }

        private void InizializzaCollegaCartella(bool abilitaCollegaCartella)
        {

            this.optCollegaCartella.Items.Clear();
            this.lblAlertCollegaCartella.Visible = abilitaCollegaCartella;
            this.ucEasyPictureBoxCollega.Visible = abilitaCollegaCartella;
            this.optCollegaCartella.Visible = abilitaCollegaCartella;
            this.txtCollegaPrecedente.Text = "";

            if (abilitaCollegaCartella)
            {
                this.lblAlertCollegaCartella.Text = @"Esistono già cartella aperte per il paziente selezionato. ";
                this.lblAlertCollegaCartella.Text += @"E' possibile collegare la nuova cartella ad una precedente o proseguire con la creazione di una nuova:";

                this.ucEasyPictureBoxCollega.Image = Risorse.GetImageFromResource(Risorse.GC_ALERTALLERGIA_256);

                Infragistics.Win.ValueListItem oValC = new Infragistics.Win.ValueListItem(C_OPT_COLLEGA, "Collega Cartella");
                this.optCollegaCartella.Items.Add(oValC);
                Infragistics.Win.ValueListItem oValN = new Infragistics.Win.ValueListItem(C_OPT_NUOVA, "Nuova Cartella");
                this.optCollegaCartella.Items.Add(oValN);

                this.optCollegaCartella.CheckedItem = this.optCollegaCartella.Items[1];



            }

        }

        private void collegacart_Step(UnicodeSrl.Scci.CollegaCartella c, EventArgs e)
        {

            this.ultraStatusBar.Panels[0].ProgressBarInfo.Label = c.StatusText;

            switch (c.StepNumber)
            {
                case -1:
                    easyStatics.EasyMessageBox(c.StatusText, @"Collega Cartella", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    break;

                case 0:
                    this.ultraStatusBar.Panels[0].ProgressBarInfo.Value = 0;
                    break;

                case 1:
                    this.ultraStatusBar.Panels[0].ProgressBarInfo.Value = 0;
                    break;

                case 2:
                    this.ultraStatusBar.Panels[0].ProgressBarInfo.Value = 5;
                    break;

                case 3:
                    this.ultraStatusBar.Panels[0].ProgressBarInfo.Value = 10;
                    break;

                case 4:
                    this.ultraStatusBar.Panels[0].ProgressBarInfo.Value = 40;
                    break;

                case 5:
                    this.ultraStatusBar.Panels[0].ProgressBarInfo.Value = 85;
                    break;

                case 6:
                    this.ultraStatusBar.Panels[0].ProgressBarInfo.Value = 100;
                    break;

            }
        }

        public bool Salva()
        {

            bool bReturn = false;

            try
            {

                this.ultraStatusBar.Panels[0].ProgressBarInfo.Minimum = 0;
                this.ultraStatusBar.Panels[0].ProgressBarInfo.Maximum = 100;
                this.ultraStatusBar.Panels[0].ProgressBarInfo.ShowLabel = true;
                this.ultraStatusBar.Panels[0].ProgressBarInfo.Appearance.FontData.SizeInPoints = this.lblAlertCollegaCartella.Appearance.FontData.SizeInPoints;
                this.ultraStatusBar.Panels[0].ProgressBarInfo.Value = 0;
                this.ultraStatusBar.Panels[0].ProgressBarInfo.Label = string.Empty;
                this.ultraStatusBar.Visible = (this.CollegaCartella);

                if (ControllaValori())
                {

                    CoreStatics.impostaCursore(ref _f, Scci.Enums.enum_app_cursors.WaitCursor);

                    if (this.CollegaCartella)
                    {

                        UnicodeSrl.Scci.CollegaCartella collegacart = new UnicodeSrl.Scci.CollegaCartella();
                        collegacart.Step += new UnicodeSrl.Scci.CollegaCartella.StepHandler(collegacart_Step);

                        bReturn = collegacart.Collega(CoreStatics.CoreApplication.Ambiente, this.txtCollegaNuova.Text, this.lblAnnoCartellaCollega.Text,
                                                            CoreStatics.CoreApplication.CartellaCollegabileSelezionata.ID,
                                                            CoreStatics.CoreApplication.TrasferimentoCollegabileSelezionato.ID, CoreStatics.CoreApplication.EpisodioCollegabileSelezionato.ID,
                                                            CoreStatics.CoreApplication.Trasferimento.ID, CoreStatics.CoreApplication.Episodio.ID, CoreStatics.CoreApplication.Paziente.ID);

                        collegacart.Step -= collegacart_Step;
                        collegacart = null;

                    }
                    else
                    {

                        Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                        op.Parametro.Add("IDTrasferimento", CoreStatics.CoreApplication.Trasferimento.ID);
                        op.Parametro.Add("NumeroCartella", this.utxtNumeroCartella.Text + this.lblAnnoCartella.Text);
                        op.Parametro.Add("CodStatoCartella", EnumStatoCartella.AP.ToString());

                        op.TimeStamp.CodEntita = EnumEntita.CAR.ToString();

                        SqlParameterExt[] spcoll = new SqlParameterExt[1];

                        string xmlParam = XmlProcs.XmlSerializeToString(op);
                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                        Database.ExecStoredProc("MSP_AggMovTrasferimentiCartella", spcoll);

                        op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                        op.Parametro.Add("CodUA", CoreStatics.CoreApplication.Trasferimento.CodUA);

                        spcoll = new SqlParameterExt[1];

                        xmlParam = XmlProcs.XmlSerializeToString(op);
                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                        Database.ExecStoredProc("MSP_AggNumeroCartella", spcoll);

                        CoreStatics.CoreApplication.Paziente.AggiornaDatiSAC();

                        bReturn = true;
                    }
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Salva", this.Text);
                bReturn = false;
            }
            finally
            {
                this.ultraStatusBar.Visible = false;
                CoreStatics.impostaCursore(ref _f, Scci.Enums.enum_app_cursors.DefaultCursor);
            }

            return bReturn;

        }

        private bool ControllaValori()
        {

            bool bOK = true;

            if (this.CollegaCartella)
            {             
                if (bOK)
                {
                    if (CoreStatics.CoreApplication.CartellaCollegabileSelezionata == null || this.txtCollegaPrecedente.Text.Trim() == "")
                    {
                        easyStatics.EasyMessageBox("Selezionare una cartella da collegare!", "Collega Cartella", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.ubSelezionaCartellaCollegata.Focus();
                        bOK = false;
                    }
                }
                if (bOK)
                {
                    if (this.txtCollegaNuova.Text.Trim() == "")
                    {
                        easyStatics.EasyMessageBox("Numero Nuova Cartella Assente!", "Collega Cartella", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.ubSelezionaCartellaCollegata.Focus();
                        bOK = false;
                    }
                }

                if (bOK)
                {
                    UnicodeSrl.Scci.CollegaCartella collegacart = new UnicodeSrl.Scci.CollegaCartella();
                    collegacart.Step += new UnicodeSrl.Scci.CollegaCartella.StepHandler(collegacart_Step);


                    bOK = collegacart.ControllaNuovoNumero(CoreStatics.CoreApplication.Ambiente,
                                                this.txtCollegaNuova.Text, this.lblAnnoCartellaCollega.Text,
                                                CoreStatics.CoreApplication.Trasferimento.ID,
                                                CoreStatics.CoreApplication.Episodio.ID,
                                                CoreStatics.CoreApplication.Paziente.ID);

                    collegacart.Step -= collegacart_Step;
                    collegacart = null;
                }

            }
            else
            {


                if (this.utxtNumeroCartella.Text == "")
                {
                    easyStatics.EasyMessageBox("Inserire Numero Cartella!", "Apertura Cartella", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    this.utxtNumeroCartella.Focus();
                    bOK = false;
                }

                if (bOK)
                {


                    Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("CodUA", CoreStatics.CoreApplication.Trasferimento.CodUA);
                    op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Paziente.ID);
                    op.Parametro.Add("IDEpisodio", CoreStatics.CoreApplication.Episodio.ID);
                    op.Parametro.Add("IDTrasferimento", CoreStatics.CoreApplication.Trasferimento.ID);
                    op.Parametro.Add("NumeroCartella", this.utxtNumeroCartella.Text + this.lblAnnoCartella.Text);

                    op.TimeStamp.CodEntita = EnumEntita.CAR.ToString();

                    SqlParameterExt[] spcoll = new SqlParameterExt[1];

                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);


                    DataSet ds = Database.GetDatasetStoredProc("MSP_ControlloNumeroCartella", spcoll);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        if (Convert.ToBoolean(ds.Tables[0].Rows[0]["Usato"]))
                        {
                            easyStatics.EasyMessageBox("Numero Cartella già utilizzato !", "Apertura Cartella", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            bOK = false;
                        }
                        else
                        {
                            bOK = true;
                        }
                    }
                    else
                        bOK = false;
                }
            }

            return bOK;

        }

        private string getNumeroNuovaCartellaCollegata(string idCartellaOrigine, string numCartellaOrigine)
        {
            string ret = "";

            Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
            op.Parametro.Add("IDCartella", idCartellaOrigine);

            op.TimeStamp.CodEntita = EnumEntita.CAR.ToString();

            SqlParameterExt[] spcoll = new SqlParameterExt[1];

            string xmlParam = XmlProcs.XmlSerializeToString(op);
            spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

            DataSet ds = Database.GetDatasetStoredProc("MSP_SelNumeroCartellaCollegata", spcoll);

            if (ds != null)
            {
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && !ds.Tables[0].Rows[0].IsNull("NumeroCartella"))
                {
                    ret = ds.Tables[0].Rows[0]["NumeroCartella"].ToString();
                }

                ds.Dispose();
                ds = null;
            }

            return ret;
        }

        #endregion

        #region Events

        private void frmPUAperturaCartella_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            try
            {

                if (this.Salva() == true)
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

        private void frmPUAperturaCartella_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void optCollegaCartella_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.utcNuovaCartella.Tabs[this.optCollegaCartella.CheckedItem.DataValue.ToString()].Selected = true;
                this.utcNuovaCartella.ActiveTab = this.utcNuovaCartella.Tabs[this.optCollegaCartella.CheckedItem.DataValue.ToString()];
            }
            catch
            {
            }
        }

        private void ubSelezionaCartellaCollegata_Click(object sender, EventArgs e)
        {

            try
            {
                if (this.CollegaCartella)
                {
                    CoreStatics.CoreApplication.EpisodioCollegabileSelezionato = null;
                    CoreStatics.CoreApplication.TrasferimentoCollegabileSelezionato = null;
                    CoreStatics.CoreApplication.CartellaCollegabileSelezionata = null;
                    if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezionaCartellaCollegabile) == DialogResult.OK && CoreStatics.CoreApplication.CartellaCollegabileSelezionata != null)
                    {
                        this.txtCollegaPrecedente.Text = CoreStatics.CoreApplication.CartellaCollegabileSelezionata.NumeroCartella;


                    }

                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, @"ubSelezionaCartellaCollegata_Click", this.Name);
                this.txtCollegaPrecedente.Text = @"";
                this.txtCollegaNuova.Text = @"";
            }
        }

        #endregion

    }
}
