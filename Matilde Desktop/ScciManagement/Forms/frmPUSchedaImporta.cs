using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.DatiClinici.DC;
using UnicodeSrl.Scci;
using UnicodeSrl.ScciResource;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmPUSchedaImporta : Form, Interfacce.IViewFormBase
    {

        #region Declare

        private enum enumIconResult
        {
            None = 0,
            OK = 1,
            Error = 2,
            Warning = 3
        }

        private enum enumImportType
        {
            Scheda = 1,
            Versione = 2
        }

        #endregion

        public frmPUSchedaImporta()
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

        public string ViewText
        {
            get
            {
                return (string)this.Tag;
            }
            set
            {
                this.Tag = value;
                this.Text = value;
            }
        }

        public void ViewInit()
        {
            this.InitializeOpenFileDialog();

            this.chkVersione.Checked = false;

        }

        #endregion

        #region Public Properties

        public Image ViewImage
        {
            get
            {
                return this.picView.Image;
            }
            set
            {
                this.picView.Image = value;
            }
        }

        public string codiceNewScheda { get; set; }

        #endregion

        #region Initialize

        private void InitializeOpenFileDialog()
        {
            this.openFileDialog.FileName = string.Empty;
            this.openFileDialog.Filter = "File XML (*.xml)|*.xml|Tutti i file|*.*";
            this.openFileDialog.FilterIndex = 0;
            this.openFileDialog.CheckPathExists = true;
            this.openFileDialog.Title = "Seleziona il file di importazione";
        }

        #endregion

        #region Subroutine

        private bool AnalyzeImport()
        {
            bool bret = false;

            enumIconResult icon = enumIconResult.None;
            string result = string.Empty;
            List<string> codicidecodifiche = new List<string>();

            SchedaExport scexp = null;
            XmlSerializer ser = null;
            XmlReader reader = null;

            try
            {
                ser = new XmlSerializer(typeof(SchedaExport));
                reader = XmlReader.Create(this.uteFilePath.Text);

                try
                {
                    scexp = (SchedaExport)ser.Deserialize(reader);
                }
                catch
                {
                    scexp = null;
                }

                if (scexp != null)
                {
                    result += "Scheda " + scexp.Codice + " - " + scexp.Descrizione + Environment.NewLine;
                    result += Environment.NewLine;
                    result += "Versioni:" + Environment.NewLine;

                    SetResult(result, true, icon);

                    foreach (ExportVersion ver in scexp.Versioni)
                    {
                        result += ver.CodVersione + " - " + ver.Descrizione;
                        result += Environment.NewLine;

                        codicidecodifiche = codicidecodifiche.Union<string>(this.GetDecodifiche(ver)).ToList<string>();
                    }

                    if (codicidecodifiche.Count > 0)
                    {
                        result += Environment.NewLine;
                        result += "***********************************************************************" + Environment.NewLine;
                        result += "ATTENZIONE !!!!!" + Environment.NewLine;
                        result += "Rilevati i seguenti codici decodifiche in una o più versioni elaborate:" + Environment.NewLine + Environment.NewLine;

                        result += String.Join<string>(", ", codicidecodifiche.ToArray<String>()) + Environment.NewLine + Environment.NewLine;

                        result += "Tali codici decodifica dovranno essere presenti nel database di destinazione." + Environment.NewLine;
                        result += "***********************************************************************" + Environment.NewLine;
                        icon = enumIconResult.Warning;
                    }
                    else
                    {
                        icon = enumIconResult.OK;
                    }

                    result += Environment.NewLine;
                    result += @"Fare click su ""Conferma"" per eseguire l'importazione del file selezionato.";
                    result += Environment.NewLine;

                    bret = true;
                }
                else
                {
                    result = "File XML non riconosciuto come generato dalla procedura di esportazione schede di ScciManagement.";
                    icon = enumIconResult.Error;
                    bret = false;
                }

                SetResult(result, false, icon);
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                SetResult(ex.Message, false, enumIconResult.Error);
                bret = false;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }

                if (ser != null) ser = null;

                if (scexp != null) scexp = null;
            }

            return bret;

        }

        private bool Import()
        {
            bool bRet = false;
            List<string> sqlList = null;

            string result = string.Empty;

            SchedaExport scexp = null;
            XmlSerializer ser = null;
            XmlReader reader = null;

            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlTransaction tran = null;

            try
            {
                ser = new XmlSerializer(typeof(SchedaExport));
                reader = XmlReader.Create(this.uteFilePath.Text);

                try
                {
                    scexp = (SchedaExport)ser.Deserialize(reader);
                }
                catch
                {
                    scexp = null;
                }

                if (scexp != null)
                {
                    result += Environment.NewLine;
                    result += "Importazione Scheda " + scexp.Codice + " - " + scexp.Descrizione + Environment.NewLine;
                    result += Environment.NewLine;

                    SetResult(result, true, enumIconResult.None);

                    #region generazione sql per l'importazione

                    sqlList = new List<string>();

                    sqlList.Add(this.GetInsertStatement(scexp.Scheda.Fields, enumImportType.Scheda));

                    foreach (ExportVersion ver in scexp.Versioni)
                    {
                        sqlList.Add(this.GetInsertStatement(ver.Fields, enumImportType.Versione));
                    }

                    #endregion

                    #region lancio della insert transazionata

                    conn = new SqlConnection(MyStatics.Configurazione.ConnectionString);
                    conn.Open();

                    tran = conn.BeginTransaction("SchedaImportTransaction");

                    cmd = new SqlCommand();

                    cmd.Connection = conn;
                    cmd.Transaction = tran;
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.Text;

                    try
                    {
                        foreach (string ssql in sqlList)
                        {
                            cmd.CommandText = ssql;
                            cmd.ExecuteNonQuery();
                        }

                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            tran.Rollback();
                        }
                        catch (Exception inEx)
                        {
                            ex = new Exception(ex.Message + Environment.NewLine + inEx.Message, inEx);
                        }
                        throw ex;
                    }

                    #endregion

                }

                bRet = true;
                this.codiceNewScheda = scexp.Codice;
                MessageBox.Show(this, "Importazione terminata senza errori.", "Importazione scheda", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                this.codiceNewScheda = string.Empty;
                bRet = false;
                SetResult(ex.Message, false, enumIconResult.Error);
            }
            finally
            {
                #region chiusura oggetti

                if (reader != null && reader.ReadState != ReadState.Closed)
                {
                    reader.Close();
                    reader = null;
                }

                if (ser != null) ser = null;

                if (scexp != null) scexp = null;

                if (tran != null)
                {
                    tran.Dispose();
                    tran = null;
                }

                if (cmd != null)
                {
                    cmd.Dispose();
                    cmd = null;
                }

                if (conn != null || conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                    conn.Dispose();
                    conn = null;
                }

                #endregion
            }

            return bRet;
        }

        private bool ImportVersion()
        {

            bool bRet = false;
            string result = string.Empty;
            List<string> sqlList = null;

            SchedaExport scexp = getSchedaExport();
            string sVersione = this.uceVersioni.Value.ToString();

            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlTransaction tran = null;

            try
            {

                if (scexp != null)
                {

                    if (int.Parse(DataBase.FindValue("Count(*)", "T_Schede", "Codice = '" + scexp.Codice + "'", "0")) == 1)
                    {

                        int nVersioneMax = int.Parse(DataBase.FindValue("Max(Versione)", "T_SchedeVersioni", "CodScheda = '" + scexp.Codice + "'", "0"));

                        sqlList = new List<string>();
                        foreach (ExportVersion ver in scexp.Versioni)
                        {
                            if (sVersione == ver.CodVersione)
                            {
                                result += Environment.NewLine;
                                result += "Importazione Scheda " + scexp.Codice + " - " + scexp.Descrizione + ", Versione:" + sVersione + Environment.NewLine;
                                result += Environment.NewLine;
                                SetResult(result, true, enumIconResult.None);
                                foreach (ExportField fld in ver.Fields)
                                {
                                    if (fld.Name == "Versione")
                                    {
                                        fld.Value = (nVersioneMax + 1).ToString();
                                    }
                                }
                                sqlList.Add(this.GetInsertStatement(ver.Fields, enumImportType.Versione));
                            }
                        }

                        conn = new SqlConnection(MyStatics.Configurazione.ConnectionString);
                        conn.Open();

                        tran = conn.BeginTransaction("SchedaImportTransaction");

                        cmd = new SqlCommand();

                        cmd.Connection = conn;
                        cmd.Transaction = tran;
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.Text;

                        try
                        {
                            foreach (string ssql in sqlList)
                            {
                                cmd.CommandText = ssql;
                                cmd.ExecuteNonQuery();
                            }

                            tran.Commit();
                            bRet = true;
                            MessageBox.Show(this, "Importazione terminata senza errori." + Environment.NewLine +
                                                    "Nuova versione scheda: " + (nVersioneMax + 1) + Environment.NewLine +
                                                    "IMPORTANTE!!!" + Environment.NewLine + "Controllare le date di validità e il flag di attivazione!!!", "Importazione versione scheda", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.codiceNewScheda = scexp.Codice;
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                tran.Rollback();
                            }
                            catch (Exception inEx)
                            {
                                ex = new Exception(ex.Message + Environment.NewLine + inEx.Message, inEx);
                            }
                            throw ex;
                        }

                    }
                    else
                    {
                        result += Environment.NewLine;
                        result += "Codice Scheda " + scexp.Codice + " - " + scexp.Descrizione + "NON trovata!!!" + Environment.NewLine;
                        result += "Impossibile importare la versione selezionata!!!" + Environment.NewLine;
                        result += Environment.NewLine;
                        SetResult(result, true, enumIconResult.Error);
                    }

                }
                else
                {
                    MessageBox.Show(this, "Importazione CON errori.", "Importazione versione scheda", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                this.codiceNewScheda = scexp.Codice;
                bRet = false;
                SetResult(ex.Message, false, enumIconResult.Error);
            }

            return bRet;

        }

        private List<string> GetDecodifiche(ExportVersion ver)
        {
            List<string> list = null;
            string strutturaxml = string.Empty;
            string layoutxml = string.Empty;
            Gestore gest = null;

            try
            {
                strutturaxml = this.UnescapeXML(UnicodeSrl.Scci.Statics.Database.CheckStringNull(ver.Fields.Find(f => f.Name == "Struttura").Value, string.Empty));
                layoutxml = this.UnescapeXML(UnicodeSrl.Scci.Statics.Database.CheckStringNull(ver.Fields.Find(f => f.Name == "Layout").Value, string.Empty));

                if (strutturaxml != string.Empty && layoutxml != string.Empty)
                {
                    gest = new Gestore();
                    gest.SchedaXML = strutturaxml;
                    gest.SchedaLayoutsXML = layoutxml;

                    list = new List<string>();

                    foreach (DcSezione sez in gest.Scheda.Sezioni.Values)
                    {
                        foreach (DcVoce v in sez.Voci.Values)
                        {
                            if (!System.String.IsNullOrEmpty(v.Decodifica)) list.Add(v.Decodifica);
                        }
                    }
                }

            }
            catch
            {
                list = new List<string>();
            }

            return list;
        }

        private void SetResult(string text, bool appendText, enumIconResult icon)
        {
            if (appendText)
            {
                this.uteEsito.Text += text;
            }
            else
            {
                this.uteEsito.Text = text;
            }

            switch (icon)
            {
                case enumIconResult.Error:
                    this.picEsito.Image = Risorse.GetImageFromResource(Risorse.GC_CANCELLATONDO_256);
                    break;
                case enumIconResult.OK:
                    this.picEsito.Image = Risorse.GetImageFromResource(Risorse.GC_SI_256);
                    break;
                case enumIconResult.Warning:
                    this.picEsito.Image = Risorse.GetImageFromResource(Risorse.GC_ALERTALLERGIA_256);
                    break;
                case enumIconResult.None:
                default:
                    this.picEsito.Image = null;
                    break;
            }

            this.Refresh();

        }

        private string GetInsertStatement(List<ExportField> fields, enumImportType importType)
        {
            string sret = string.Empty;
            string sunescval = string.Empty;
            try
            {

                if (importType == enumImportType.Scheda)
                {
                    sret = "INSERT INTO T_Schede (";
                }
                else
                {
                    sret = "INSERT INTO T_SchedeVersioni (";
                }


                string[] columnnames = (from fld in fields select "[" + fld.Name + "]").ToArray();

                sret += String.Join<string>(", ", columnnames) + ")" + Environment.NewLine + "VALUES" + Environment.NewLine;

                sret += "(" + Environment.NewLine;

                foreach (ExportField fld in fields)
                {
                    if (System.String.IsNullOrEmpty(fld.Value))
                    {
                        sret += "NULL, ";
                    }
                    else
                    {
                        System.Type fldtype = Type.GetType(fld.DataType);
                        sunescval = this.UnescapeXML(fld.Value);

                        if (fldtype == typeof(System.String))
                        {
                            sret += "'" + UnicodeSrl.Scci.Statics.Database.testoSQL(sunescval) + "', ";
                        }
                        else if (fldtype == typeof(System.Int32))
                        {
                            if (Microsoft.VisualBasic.Information.IsNumeric(sunescval))
                                sret += sunescval + ", ";
                            else
                                sret += "NULL, ";
                        }
                        else if (fldtype == typeof(System.Boolean))
                        {
                            bool btemp = false;
                            bool.TryParse(sunescval, out btemp);

                            if (btemp)
                                sret += "1, ";
                            else
                                sret += "0, ";
                        }
                        else if (fldtype == typeof(System.DateTime))
                        {
                            DateTime dttemp = DateTime.MinValue;
                            DateTime.TryParse(sunescval, out dttemp);

                            if (dttemp != DateTime.MinValue)
                                sret += UnicodeSrl.Scci.Statics.Database.dataOraSQL(dttemp) + ", ";
                            else
                                sret += "NULL, ";
                        }
                        else
                        {
                            sret += "NULL, ";
                        }
                    }
                }

                sret = sret.Substring(0, sret.Length - 2) + Environment.NewLine;

                sret += ")" + Environment.NewLine;
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                sret = string.Empty;
            }

            return sret;

        }

        private string UnescapeXML(string s)
        {

            string returnString = s;

            if (!string.IsNullOrEmpty(s) || s.Contains('&'))
            {
                returnString = returnString.Replace("&apos;", "'");
                returnString = returnString.Replace("&quot;", "\"");
                returnString = returnString.Replace("&gt;", ">");
                returnString = returnString.Replace("&lt;", "<");
                returnString = returnString.Replace("&amp;", "&");
            }

            return returnString;

        }

        private void LoadVersioni()
        {

            SchedaExport scexp = getSchedaExport();

            try
            {

                if (this.chkVersione.Checked)
                {

                    if (scexp != null)
                    {
                        this.uceVersioni.DataMember = "Codice";
                        this.uceVersioni.DisplayMember = "Descrizione";
                        this.uceVersioni.DataSource = scexp.Versioni;
                        this.uceVersioni.Refresh();
                        if (scexp.Versioni.Count > 0)
                        {
                            this.uceVersioni.SelectedIndex = scexp.Versioni.Count - 1;
                        }
                    }

                }
                else
                {
                    this.uceVersioni.DataSource = null;
                    this.uceVersioni.Refresh();
                }

            }
            catch (Exception)
            {
                this.uceVersioni.DataSource = null;
                this.uceVersioni.Refresh();
            }
            finally
            {
                scexp = null;
            }

        }

        private SchedaExport getSchedaExport()
        {

            SchedaExport scexp = null;

            try
            {

                XmlSerializer ser = new XmlSerializer(typeof(SchedaExport));
                XmlReader reader = XmlReader.Create(this.uteFilePath.Text);

                try
                {
                    scexp = (SchedaExport)ser.Deserialize(reader);
                }
                catch
                {
                    scexp = null;
                }

            }
            catch (Exception)
            {
                scexp = null;
            }

            return scexp;
        }

        #endregion

        #region Events

        private void ubFilePath_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.Cursor = Cursors.WaitCursor;
                this.uteFilePath.Text = this.openFileDialog.FileName;
                this.Refresh();
                this.ubConferma.Enabled = this.AnalyzeImport();
                this.LoadVersioni();
                this.Cursor = Cursors.Default;
            }
        }

        private void chkVersione_CheckedChanged(object sender, EventArgs e)
        {
            this.uceVersioni.Enabled = this.chkVersione.Checked;
            this.LoadVersioni();
        }

        private void ubAnnulla_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void ubConferma_Click(object sender, EventArgs e)
        {

            this.Cursor = Cursors.WaitCursor;

            bool bRet;

            if (this.chkVersione.Checked)
            {
                bRet = this.ImportVersion();
            }
            else
            {
                bRet = this.Import();
            }

            if (bRet)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            else
            {
                this.ubConferma.Enabled = false;
            }

            this.Cursor = Cursors.Default;
        }

        #endregion

    }
}
