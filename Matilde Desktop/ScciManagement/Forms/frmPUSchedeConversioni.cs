using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using UnicodeSrl.DatiClinici.DC;
using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.ScciCore;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmPUSchedeConversioni : Form, Interfacce.IViewFormBase
    {

        public frmPUSchedeConversioni()
        {
            InitializeComponent();
        }

        #region Declare

        private const string C_ELIMINATO = "Eliminato";
        private const string C_UGUALE = "Uguale";
        private const string C_MODIFICATO_FORMATO = "Modificato Formato";
        private const string C_AGGIUNTO = "Aggiunto";

        #endregion

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

            this.InitializeUltraToolbarsManager();
            this.InitializeUltraCombo();
            this.InitializeUltraGrid();

            this.uteCodScheda.Text = this.CodScheda;
            this.lblCodSchedaDes.Text = DataBase.FindValue("Descrizione", "T_Schede", "Codice = '" + this.CodScheda + "'", "");
            this.uteVersione.Text = this.Versione.ToString();
            if (this.uceVersioneDestinazione.Items.Count > 0) { this.uceVersioneDestinazione.SelectedIndex = 0; }

            this.udteDtCreazioneI.Value = DateTime.Now.Date;
            this.udteDtCreazioneF.Value = DateTime.Now.Date.AddDays(1).AddSeconds(-1);

            this.ResumeLayout();

        }

        #endregion

        #region Properties

        public string CodScheda { get; set; }

        public int Versione { get; set; }

        #endregion

        #region UltraToolBar

        private void InitializeUltraToolbarsManager()
        {

            try
            {

                MyStatics.SetUltraToolbarsManager(this.UltraToolbarsManager);

            }
            catch (Exception)
            {

            }

        }

        #endregion

        #region UltraComboEditor

        private void InitializeUltraCombo()
        {

            string sSql = string.Empty;
            DataSet oDs = null;

            try
            {

                MyStatics.SetUltraComboEditorLayout(ref this.uceVersioneDestinazione);
                this.uceVersioneDestinazione.ValueMember = "Versione";
                this.uceVersioneDestinazione.DisplayMember = "Descrizione";
                sSql = string.Format("{0} Where CodScheda = '{1}' And Versione > {2} Order By Versione",
                        DataBase.GetSqlPUView(Enums.EnumDataNames.T_SchedeVersioni),
                        this.CodScheda,
                        this.Versione);
                oDs = DataBase.GetDataSet(sSql);
                this.uceVersioneDestinazione.DataMember = oDs.Tables[0].TableName;
                this.uceVersioneDestinazione.DataSource = oDs;
                this.uceVersioneDestinazione.DataBind();

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "InitializeUltraCombo", this.Name);
            }

        }

        #endregion

        #region UltraGrid

        private void InitializeUltraGrid()
        {
            MyStatics.SetUltraGridLayout(ref this.UltraGridScheda, true, false);
        }

        private void LoadUltraGrid()
        {

            DataSet oDs = getDataSetScheda();

            try
            {

                Scheda oSchedaI = new Scheda(this.uteCodScheda.Text, int.Parse(this.uteVersione.Text), DateTime.Now, MyStatics.Ambiente);
                Gestore oGestoreI = CoreStatics.GetGestore();
                oGestoreI.SchedaXML = oSchedaI.StrutturaXML;
                oGestoreI.SchedaLayoutsXML = oSchedaI.LayoutXML;
                oGestoreI.Decodifiche = oSchedaI.DizionarioValori();

                Scheda oSchedaF = new Scheda(this.uteCodScheda.Text, int.Parse(this.uceVersioneDestinazione.Value.ToString()), DateTime.Now, MyStatics.Ambiente);
                Gestore oGestoreF = CoreStatics.GetGestore();
                oGestoreF.SchedaXML = oSchedaF.StrutturaXML;
                oGestoreF.SchedaLayoutsXML = oSchedaF.LayoutXML;
                oGestoreF.Decodifiche = oSchedaF.DizionarioValori();

                foreach (DcSezione oDcSezioneI in oGestoreI.Scheda.Sezioni.Values)
                {

                    foreach (DcVoce oDcVoceI in oDcSezioneI.Voci.Values)
                    {

                        DataRow _dr = oDs.Tables[0].NewRow();
                        _dr["ID"] = oDcVoceI.ID;
                        _dr["Descrizione"] = oDcVoceI.Descrizione;
                        _dr["Formato"] = oDcVoceI.Formato.ToString();

                        DcVoce oDcVoceF = oGestoreF.LeggeVoce(oDcVoceI.ID);
                        if (oDcVoceF == null)
                        {
                            _dr["Stato"] = C_ELIMINATO;
                        }
                        else
                        {
                            if (oDcVoceI.Formato == oDcVoceF.Formato)
                            {
                                _dr["Stato"] = C_UGUALE;
                            }
                            else
                            {
                                _dr["Stato"] = C_MODIFICATO_FORMATO;
                            }
                        }

                        oDs.Tables[0].Rows.Add(_dr);

                    }

                }

                foreach (DcSezione oDcSezioneF in oGestoreF.Scheda.Sezioni.Values)
                {

                    foreach (DcVoce oDcVoceF in oDcSezioneF.Voci.Values)
                    {

                        DcVoce oDcVoceI = oGestoreI.LeggeVoce(oDcVoceF.ID);
                        if (oDcVoceI == null)
                        {
                            DataRow _dr = oDs.Tables[0].NewRow();
                            _dr["ID"] = oDcVoceF.ID;
                            _dr["Descrizione"] = oDcVoceF.Descrizione;
                            _dr["Formato"] = oDcVoceF.Formato.ToString();
                            _dr["Stato"] = C_AGGIUNTO;
                            oDs.Tables[0].Rows.Add(_dr);
                        }

                    }

                }

                this.UltraGridScheda.DataSource = oDs;
                this.UltraGridScheda.Refresh();
                this.UltraGridScheda.Text = "Elenco campi scheda";

                this.UltraGridScheda.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private DataSet getDataSetScheda()
        {

            DataSet oDs = new DataSet();
            oDs.Tables.Add(new DataTable());
            oDs.Tables[0].Columns.Add("ID", typeof(string));
            oDs.Tables[0].Columns.Add("Descrizione", typeof(string));
            oDs.Tables[0].Columns.Add("Formato", typeof(string));
            oDs.Tables[0].Columns.Add("Stato", typeof(string));

            return oDs;

        }

        private void ConvertiScheda()
        {

            string xmlDati = "";
            string sSql = string.Empty;
            string sSqlUpdate = string.Empty;

            try
            {

                this.ubAnnulla.Enabled = false;
                this.ubConferma.Enabled = false;

                sSql = "Select * From T_MovSchede" + Environment.NewLine +
"Where " + getWhereSchede();
                DataSet oDsMovSchede = DataBase.GetDataSet(sSql);
                this.UltraProgressBar.Minimum = 0;
                this.UltraProgressBar.Maximum = oDsMovSchede.Tables[0].Rows.Count;
                this.UltraProgressBar.Visible = true;
                Application.DoEvents();
                foreach (DataRow oDrMovScheda in oDsMovSchede.Tables[0].Rows)
                {

                    this.UltraProgressBar.Value += 1;
                    Application.DoEvents();
                    xmlDati = "";
                    if (!oDrMovScheda.IsNull("Dati"))
                    {

                        MovScheda oMovScheda = new MovScheda(oDrMovScheda["ID"].ToString(), MyStatics.Ambiente);
                        MovScheda oMovSchedaNuova = new MovScheda(this.uteCodScheda.Text,
                                                                    (EnumEntita)Enum.Parse(typeof(EnumEntita), oDrMovScheda["CodEntita"].ToString()),
                                                                    oDrMovScheda["CodUA"].ToString(),
                                                                    "", "", "",
                                                                    (int)this.uceVersioneDestinazione.Value,
                                                                    MyStatics.Ambiente);
                        oMovSchedaNuova.CopiaDaOriginePerConversione(oMovScheda, (int)oDrMovScheda["Numero"]);

                        xmlDati = oMovSchedaNuova.DatiXML;


                        StoricizzaScheda(oDrMovScheda["ID"].ToString());
                        eseguiStoredAggiornaScheda(oDrMovScheda["ID"].ToString(), (int)this.uceVersioneDestinazione.Value, xmlDati);
                    }
                    else
                    {
                        StoricizzaScheda(oDrMovScheda["ID"].ToString());

                        eseguiStoredAggiornaScheda(oDrMovScheda["ID"].ToString(), (int)this.uceVersioneDestinazione.Value, xmlDati);
                    }

                    sSql = "Select * From T_MovSchede" + Environment.NewLine +
"Where CodScheda = '" + this.uteCodScheda.Text + "' And Versione = " + this.uteVersione.Text + " And Storicizzata = 1 " + Environment.NewLine +
"And CodEntita = '" + oDrMovScheda["CodEntita"].ToString() + "' And IDEntita = '" + oDrMovScheda["IDEntita"].ToString() + "' And Numero = " + oDrMovScheda["Numero"].ToString() + Environment.NewLine;
                    if (!oDrMovScheda.IsNull("IDSchedaPadre"))
                    {
                        sSql += "And IDSchedaPadre = '" + oDrMovScheda["IDSchedaPadre"].ToString() + "'";
                    }
                    DataSet oDsMovSchedeStorico = DataBase.GetDataSet(sSql);
                    foreach (DataRow oDrMovSchedaStorico in oDsMovSchedeStorico.Tables[0].Rows)
                    {

                        xmlDati = "";
                        if (!oDrMovSchedaStorico.IsNull("Dati"))
                        {

                            MovScheda oMovSchedaStorico = new MovScheda(oDrMovSchedaStorico["ID"].ToString(), MyStatics.Ambiente);
                            MovScheda oMovSchedaStoricoNuova = new MovScheda(this.uteCodScheda.Text,
                                                                            (EnumEntita)Enum.Parse(typeof(EnumEntita), oDrMovSchedaStorico["CodEntita"].ToString()),
                                                                            oDrMovSchedaStorico["CodUA"].ToString(),
                                                                            "", "", "",
                                                                            (int)this.uceVersioneDestinazione.Value,
                                                                            MyStatics.Ambiente);
                            oMovSchedaStoricoNuova.CopiaDaOriginePerConversione(oMovSchedaStorico, (int)oDrMovSchedaStorico["Numero"]);

                            xmlDati = oMovSchedaStoricoNuova.DatiXML;


                            StoricizzaScheda(oDrMovSchedaStorico["ID"].ToString());
                            eseguiStoredAggiornaScheda(oDrMovSchedaStorico["ID"].ToString(), (int)this.uceVersioneDestinazione.Value, xmlDati);

                        }

                    }

                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "ConvertiScheda", this.Name);
            }
            finally
            {
                this.UltraProgressBar.Visible = false;
                this.ubAnnulla.Enabled = true;
                this.ubConferma.Enabled = true;
                Application.DoEvents();
            }

        }

        private void eseguiStoredAggiornaScheda(string idScheda, int versione, string xmldati)
        {
            try
            {
                SqlParameter[] sqlParams = new SqlParameter[3];

                string sRet = string.Empty;
                Guid IDSchedaGuid;

                if (idScheda != null && idScheda != string.Empty && Guid.TryParse(idScheda.Trim(), out IDSchedaGuid)
                    && versione > 0)
                {






                    Parametri op = new Parametri(MyStatics.Ambiente);
                    op.Parametro.Add("IDScheda", idScheda);
                    op.Parametro.Add("Versione", versione.ToString());
                    op.Parametro.Add("Dati", xmldati);
                    op.TimeStamp.CodEntita = EnumEntita.SCH.ToString();
                    op.TimeStamp.CodAzione = EnumAzioni.MOD.ToString();
                    SqlParameterExt[] spcoll = new SqlParameterExt[1];
                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    xmlParam = XmlProcs.CheckXMLDati(xmlParam);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                    DataTable dt = Database.GetDataTableStoredProc("MSP_BO_AggiornaVersioneScheda", spcoll);

                }
                else
                    sRet = "";
            }
            catch (Exception ex)
            {
                throw new Exception(@"[eseguiUpdateStored] " + ex.Message, ex);
            }
        }

        private void StoricizzaScheda(string idscheda)
        {

            try
            {

                string sSql = "Insert Into T_StoricoVersioni_MovSchede " + Environment.NewLine +
                                "(ID, IDMovScheda, CodScheda, Versione, Numero, Dati, DataOraCopia)" + Environment.NewLine +
                                "Select '" + Guid.NewGuid().ToString() + "', ID, CodScheda, Versione, Numero, Dati, " + DataBase.SQLDateTimeInsert(DateTime.Now) + " From T_MovSchede" + Environment.NewLine +
                                "Where ID = '" + idscheda + "'";
                DataBase.ExecuteSql(sSql);

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "StoricizzaScheda", this.Name);
            }

        }

        private string getWhereSchede()
        {

            string s_sql = "CodScheda = '" + this.uteCodScheda.Text + "'" + Environment.NewLine +
                            "And Versione = " + this.uteVersione.Text + Environment.NewLine +
                            "And Storicizzata = 0" + Environment.NewLine +
                            (this.udteDtCreazioneI.Value == null ? "" : "And DataCreazione >= " + DataBase.SQLDateTime((DateTime)this.udteDtCreazioneI.Value) + Environment.NewLine) +
                            (this.udteDtCreazioneF.Value == null ? "" : "And DataCreazione <= " + DataBase.SQLDateTime((DateTime)this.udteDtCreazioneF.Value) + Environment.NewLine) +
                            (this.uteIDScheda.Text == "" ? "" : "And ID = '" + this.uteIDScheda.Text + "'" + Environment.NewLine);

            return s_sql;

        }

        #endregion

        #region Events

        private void uceVersioneDestinazione_ValueChanged(object sender, EventArgs e)
        {
            this.LoadUltraGrid();
        }

        private void UltraGridScheda_InitializeRow(object sender, InitializeRowEventArgs e)
        {

            try
            {

                switch (e.Row.Cells["Stato"].Text)
                {

                    case C_AGGIUNTO:
                        e.Row.Cells["Stato"].Appearance.BackColor = Color.Green;
                        break;

                    case C_ELIMINATO:
                        e.Row.Cells["Stato"].Appearance.BackColor = Color.Red;
                        break;

                    case C_MODIFICATO_FORMATO:
                        e.Row.Cells["Stato"].Appearance.BackColor = Color.Orange;
                        break;

                    case C_UGUALE:
                        break;

                }

            }
            catch (Exception)
            {

            }

        }

        private void ubAnnulla_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ubConferma_Click(object sender, EventArgs e)
        {

            try
            {

                if (MessageBox.Show(string.Format("Sei sicuro di voler convertire {2} schede (comprese quelle storicizzate) dalla versione {0} alla versione {1} ?",
                                                    this.uteVersione.Text,
                                                    this.uceVersioneDestinazione.Value,
                                                    DataBase.FindValue("Count(*)",
                                                                        "T_MovSchede",
                                                                        getWhereSchede(), "0")),
                                    "Conversione Schede",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    this.ConvertiScheda();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "ubConferma_Click", this.Name);
            }

        }

        #endregion

    }
}
