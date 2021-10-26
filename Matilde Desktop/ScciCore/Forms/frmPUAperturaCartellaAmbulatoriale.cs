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
    public partial class frmPUAperturaCartellaAmbulatoriale : frmBaseModale, Interfacce.IViewFormlModal
    {

        #region Declare

        Form _f = null;

        string s_numerocartella_copia = string.Empty;

        #endregion

        public frmPUAperturaCartellaAmbulatoriale()
        {
            InitializeComponent();
        }

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

        #endregion

        #region SubRoutine

        private void InizilizzaControlli()
        {
            InizializzaNuovaCartella();
        }

        private void InizializzaNuovaCartella()
        {

            this.lblConferma.Text = $"Conferma dell'apertura della {CoreStatics.CoreApplication.MovSchedaSelezionata.Scheda.Descrizione} per il paziente:";

            this.lblPaziente.Text = $"{CoreStatics.CoreApplication.Paziente.Cognome.ToUpper()} {CoreStatics.CoreApplication.Paziente.Nome.ToUpper()} ({CoreStatics.CoreApplication.Paziente.Sesso.ToUpper()})\n" +
                                    $"nato/a il {CoreStatics.CoreApplication.Paziente.DataNascita.ToString("dd/MM/yyyy")}  a {CoreStatics.CoreApplication.Paziente.ComuneNascita}";

                        Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
            op.Parametro.Add("CodContatore", CoreStatics.CoreApplication.MovSchedaSelezionata.Scheda.CodContatore);

            op.TimeStamp.CodEntita = EnumEntita.CAC.ToString();

                        SqlParameterExt[] spcoll = new SqlParameterExt[1];

            string xmlParam = XmlProcs.XmlSerializeToString(op);
            spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

            DataSet ds = Database.GetDatasetStoredProc("MSP_SelContatore", spcoll);

            if (ds.Tables[0].Rows.Count > 0)
            {
                this.utxtNumeroCartella.Text = (int.Parse(ds.Tables[0].Rows[0]["Valore"].ToString()) + 1).ToString();
                s_numerocartella_copia = this.utxtNumeroCartella.Text;
                this.lblAnnoCartella.Text = "/" + DateTime.Now.Year.ToString();
            }

        }

        public bool Salva()
        {

            bool bReturn = false;

            try
            {

                if (ControllaValori())
                {

                    CoreStatics.impostaCursore(ref _f, Scci.Enums.enum_app_cursors.WaitCursor);

                    if (s_numerocartella_copia == this.utxtNumeroCartella.Text)
                    {

                                                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                        op.Parametro.Add("CodContatore", CoreStatics.CoreApplication.MovSchedaSelezionata.Scheda.CodContatore);
                        op.Parametro.Add("Valore", this.utxtNumeroCartella.Text);

                        FwDataParametersList plist = new FwDataParametersList();
                        plist.Add("xParametri", XmlProcs.XmlSerializeToString(op), ParameterDirection.Input, DbType.Xml);
                        using (FwDataConnection conn = new FwDataConnection(Database.ConnectionString))
                        {
                            conn.ExecuteStored("MSP_AggContatore", ref plist);                                                  
                        }

                    }

                    CoreStatics.CoreApplication.CartellaAmbulatoriale.NumeroCartella = getNumeroCartella();
                    CoreStatics.CoreApplication.CartellaAmbulatoriale.CodStatoCartella = "AP";

                   bReturn = true;

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Salva", this.Text);
                bReturn = false;
            }
            finally
            {
                CoreStatics.impostaCursore(ref _f, Scci.Enums.enum_app_cursors.DefaultCursor);
            }

            return bReturn;

        }

        private bool ControllaValori()
        {

            bool bOK = true;

                        if (this.utxtNumeroCartella.Text == "")
            {
                easyStatics.EasyMessageBox("Inserire Numero Cartella!", "Apertura Cartella Ambulatoriale", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.utxtNumeroCartella.Focus();
                bOK = false;
            }

            if (bOK)
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodScheda", CoreStatics.CoreApplication.MovSchedaSelezionata.CodScheda);
                op.Parametro.Add("NumeroCartella", getNumeroCartella());
                op.TimeStamp.CodEntita = EnumEntita.CAC.ToString();

                FwDataParametersList plist = new FwDataParametersList();
                plist.Add("xParametri", XmlProcs.XmlSerializeToString(op), ParameterDirection.Input, DbType.Xml);
                using (FwDataConnection conn = new FwDataConnection(Database.ConnectionString))
                {

                    DataSet ds = conn.Query<DataSet>("MSP_ControlloNumeroCartellaAmbulatoriale", plist, CommandType.StoredProcedure);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        if (Convert.ToBoolean(ds.Tables[0].Rows[0]["Esito"]))
                        {
                            easyStatics.EasyMessageBox(ds.Tables[0].Rows[0]["Messaggio"].ToString(), "Apertura Cartella Ambulatoriale", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            bOK = false;
                        }
                        else
                        {
                            bOK = true;
                        }
                    }
                    else
                    {
                        bOK = false;
                    }

                }

            }

            return bOK;

        }

        private string getNumeroCartella()
        {
            return $"{this.utxtNumeroCartella.Text}{this.lblAnnoCartella.Text}";
        }

        #endregion

        #region Events Form

        private void frmPUAperturaCartellaAmbulatoriale_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
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

        private void frmPUAperturaCartellaAmbulatoriale_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        #endregion

    }
}
