using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.ScciCore;

namespace UnicodeSrl.Scci
{
    [Serializable()]
    public class CollegaCartella
    {
        #region declare

        public delegate void StepHandler(CollegaCartella c, EventArgs e);
        public event StepHandler Step;
        public EventArgs e = null;

        public string _statustext = string.Empty;
        public int _stepnumber = 0;
        public string _idcartelladestinazione = string.Empty;

        #endregion

        #region properties

        public string StatusText
        {
            get { return _statustext; }
        }

        public int StepNumber
        {
            get { return _stepnumber; }
        }

        public string IDCartellaDestinazione
        {
            get { return _idcartelladestinazione; }
        }

        #endregion

        #region methods

        public bool Collega(ScciAmbiente ambiente, string NumeroNuovaCartella, string AnnoNuovaCartella,
                                    string IDCartellaOrigine, string IDTrasferimentoOrigine, string IDEpisodioOrigine,
                                    string IDTrasferimentoDestinazione, string IDEpisodioDestinazione, string IDPaziente,
                                    bool creaPDFCartellaChiusa = true)
        {
            Trasferimento trasferimentoorigine = new Trasferimento(IDTrasferimentoOrigine, ambiente);
            Trasferimento trasferimentodestinazione = new Trasferimento(IDTrasferimentoDestinazione, ambiente);

            Cartella cartellaorigine = new Cartella(IDCartellaOrigine, string.Empty, ambiente);
            Episodio episodioorigine = new Episodio(IDEpisodioOrigine);
            Paziente paz = new Paziente(IDPaziente, IDEpisodioDestinazione);

            bool bcontinua = true;

            string annocartella = string.Empty;

            this._statustext = @"FASE 1 DI 5 : Creazione Nuova Cartella " + NumeroNuovaCartella;
            this._stepnumber = 1;
            if (Step != null) Step(this, e);

            if (bcontinua)
            {
                Parametri op = new Parametri(ambiente);
                op.Parametro.Add("IDTrasferimento", trasferimentodestinazione.ID);


                if (AnnoNuovaCartella.Length > 0 && AnnoNuovaCartella.Substring(0, 1) != @"/")
                    annocartella = @"/" + AnnoNuovaCartella;
                else
                    annocartella = AnnoNuovaCartella;

                op.Parametro.Add("NumeroCartella", NumeroNuovaCartella + annocartella);
                op.Parametro.Add("CodStatoCartella", EnumStatoCartella.AP.ToString());

                op.Parametro.Add("IDCartellaCollegata", IDCartellaOrigine);

                op.TimeStamp.CodEntita = EnumEntita.CAR.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                Database.ExecStoredProc("MSP_AggMovTrasferimentiCartella", spcoll);

                op = new Parametri(ambiente);
                op.Parametro.Add("CodUA", trasferimentodestinazione.CodUA);

                spcoll = new SqlParameterExt[1];

                xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                Database.ExecStoredProc("MSP_AggNumeroCartella", spcoll);

                bcontinua = true;
            }

            if (bcontinua)
            {
                this._statustext = @"FASE 2 DI 5 : Chiusura Cartella di Origine " + NumeroNuovaCartella + annocartella;
                this._stepnumber = 2;
                if (Step != null) Step(this, e);

                bcontinua = false;

                Parametri op = new Parametri(ambiente);
                op.Parametro.Add("IDTrasferimento", trasferimentoorigine.ID);
                op.Parametro.Add("IDCartella", cartellaorigine.ID);
                op.Parametro.Add("NumeroCartella", cartellaorigine.NumeroCartella);
                op.Parametro.Add("CodStatoCartella", EnumStatoCartella.CH.ToString());

                op.TimeStamp.CodEntita = EnumEntita.CAR.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.MOD.ToString();
                op.TimeStamp.IDEpisodio = episodioorigine.ID;
                op.TimeStamp.IDPaziente = IDPaziente;
                op.TimeStamp.IDTrasferimento = trasferimentoorigine.ID;

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                Database.ExecStoredProc("MSP_AggMovTrasferimentiCartella", spcoll);

                bcontinua = true;
            }
            else
            {
                this._statustext = @"ERRORE: Creazione Nuova Cartella";
                this._stepnumber = -1;
                if (Step != null) Step(this, e);
            }

            if (bcontinua)
            {
                this._statustext = @"FASE 3 DI 5 : Collegamento Cartelle";
                this._stepnumber = 3;
                if (Step != null) Step(this, e);

                bcontinua = false;
                string idTrasferimentosave = trasferimentodestinazione.ID;
                trasferimentodestinazione = new Trasferimento(idTrasferimentosave, ambiente);

                if (trasferimentodestinazione.IDCartella != null && trasferimentodestinazione.IDCartella.Trim() != "")
                {
                    Parametri op = new Parametri(ambiente);
                    op.Parametro.Add("IDCartellaOrigine", cartellaorigine.ID);
                    op.Parametro.Add("IDCartellaDestinazione", trasferimentodestinazione.IDCartella);

                    SqlParameterExt[] spcoll = new SqlParameterExt[1];

                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    Database.ExecStoredProc("MSP_CollegaCartella", spcoll);

                    bcontinua = true;

                }
                else
                {
                    this._statustext = @"ERRORE: Impossibile recuperare nuovo ID Cartella";
                    this._stepnumber = -1;
                    if (Step != null) Step(this, e);
                }

            }
            else
            {
                this._statustext = @"ERRORE: Chiusura Cartella di Origine";
                this._stepnumber = -1;
                if (Step != null) Step(this, e);
            }

            if (bcontinua && creaPDFCartellaChiusa)
            {
                this._statustext = @"FASE 4 DI 5 : Generazione Documento Cartella di Origine " + NumeroNuovaCartella + annocartella;
                this._stepnumber = 4;
                if (Step != null) Step(this, e);

                Report _report = new Report(Report.COD_REPORT_CARTELLA_PAZIENTE);
                if (_report != null)
                {
                    if (_report.NomePlugIn != null && _report.NomePlugIn.Trim() != "")
                    {
                        string path = System.Windows.Forms.Application.StartupPath + @"\Plugins\" + _report.NomePlugIn + @"\" + _report.NomePlugIn + @".dll";

                        bcontinua = cartellaorigine.generaearchiviaPDF(plugindllfullpath: path,
                                                                       firmaDigitale: false,
                                                                       utenteFirma: "",
                                                                       sessioneRemota: CoreStatics.CoreApplication.Sessione.Computer.SessioneRemota,
                                                                       isOSServer: CoreStatics.CoreApplication.Sessione.Computer.IsOSServer,
                                                                       evc: true,
                                                                       soloAllegaInCartella: true,
                                                                       allegati: true);
                    }
                }
            }
            else
            {
                this._statustext = @"ERRORE: Collegamento Cartelle";
                this._stepnumber = -1;
                if (Step != null) Step(this, e);
            }

            if (bcontinua)
            {
                this._statustext = @"FASE 5 DI 5 : Aggiornamento Dati Anagrafici Paziente";
                this._stepnumber = 5;
                if (Step != null) Step(this, e);

                paz.AggiornaDatiSAC();
            }
            else
            {
                this._statustext = @"ERRORE: Generazione Documento Cartella di Origine";
                this._stepnumber = -1;
                if (Step != null) Step(this, e);
            }

            if (bcontinua)
            {
                this._idcartelladestinazione = trasferimentodestinazione.IDCartella;
                this._statustext = @"FINE";
                this._stepnumber = 6;
                if (Step != null) Step(this, e);
            }
            else
            {
                this._idcartelladestinazione = string.Empty;
                this._statustext = @"ERRORE: Errore generico";
                this._stepnumber = -1;
                if (Step != null) Step(this, e);
            }

            return bcontinua;
        }


        public bool ControllaNuovoNumero(ScciAmbiente ambiente, string NumeroNuovaCartella, string AnnoNuovaCartella,
                                            string IDTrasferimentoDestinazione, string IDEpisodioDestinazione, string IDPaziente)
        {
            bool bRet = false;

            try
            {
                Trasferimento trasferimentodestinazione = new Trasferimento(IDTrasferimentoDestinazione, ambiente);

                Parametri op = new Parametri(ambiente);
                op.Parametro.Add("CodUA", trasferimentodestinazione.CodUA);
                op.Parametro.Add("IDPaziente", IDPaziente);
                op.Parametro.Add("IDEpisodio", IDEpisodioDestinazione);
                op.Parametro.Add("IDTrasferimento", IDTrasferimentoDestinazione);
                op.Parametro.Add("NumeroCartella", NumeroNuovaCartella + AnnoNuovaCartella);

                op.TimeStamp.CodEntita = EnumEntita.CAR.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);


                DataSet ds = Database.GetDatasetStoredProc("MSP_ControlloNumeroCartella", spcoll);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    bRet = !Convert.ToBoolean(ds.Tables[0].Rows[0]["Usato"]);
                }
                else
                    bRet = true;
            }
            catch
            {
                bRet = false;
            }

            if (bRet)
            {
                this._statustext = string.Empty;
                this._stepnumber = 6;
            }
            else
            {
                this._statustext = "Numero Cartella già utilizzato";
                this._stepnumber = -1;
            }

            if (Step != null) Step(this, e);

            return bRet;
        }

        public bool ControllaCartellaDestinazione(ScciAmbiente ambiente, string IDCartellaOrigine,
                                    string IDTrasferimentoDestinazione, string IDPaziente)
        {
            bool bRet = false;

            try
            {
                Trasferimento trasferimentodestinazione = new Trasferimento(IDTrasferimentoDestinazione, ambiente);
                Cartella cartellaorigine = new Cartella(IDCartellaOrigine, string.Empty, ambiente);

                Parametri op = new Parametri(ambiente);

                op.TimeStamp.CodEntita = EnumEntita.CAR.ToString();
                op.TimeStamp.IDTrasferimento = IDTrasferimentoDestinazione;
                op.TimeStamp.IDPaziente = IDPaziente;

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_SelMovCartelleCollegabili", spcoll);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        if (row["IDCartella"].ToString() == IDCartellaOrigine)
                        {
                            bRet = true;
                            break;
                        }
                    }
                }
                else
                {
                    bRet = false;
                }
            }
            catch
            {
                bRet = false;
            }

            if (bRet)
            {
                this._statustext = string.Empty;
                this._stepnumber = 6;
            }
            else
            {
                this._statustext = "ID Cartella non collegabile";
                this._stepnumber = -1;
            }

            return bRet;
        }

        #endregion

    }
}
