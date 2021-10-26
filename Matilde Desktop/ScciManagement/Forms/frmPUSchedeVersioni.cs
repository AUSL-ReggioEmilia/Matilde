using System;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.ScciCore;
using UnicodeSrl.Scci;
using UnicodeSrl.DatiClinici.DC;
using UnicodeSrl.DatiClinici.Common;
using UnicodeSrl.DatiClinici.Gestore;
using System.Threading.Tasks;
using UnicodeSrl.ScciCommon2;
using UnicodeSrl.ScciCommon2.Extensions;
using System.Collections.Generic;
using UnicodeSrl.Scci.Enums;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmPUSchedeVersioni : Form, Interfacce.IViewFormPUView
    {
        public frmPUSchedeVersioni()
        {
            InitializeComponent();
        }

        #region Declare

        private PUDataBindings _DataBinds = new PUDataBindings();
        private Enums.EnumModalityPopUp _Modality;
        private Enums.EnumDataNames _ViewDataNamePU;
        private DialogResult _DialogResult = DialogResult.Cancel;

        #endregion

        #region Interface

        public PUDataBindings ViewDataBindings
        {
            get
            {
                return _DataBinds;
            }
            set
            {
                _DataBinds = value;
            }
        }

        public Enums.EnumDataNames ViewDataNamePU
        {
            get
            {
                return _ViewDataNamePU;
            }
            set
            {
                _ViewDataNamePU = value;
            }
        }

        public Image ViewImage
        {
            get
            {
                return this.PicImage.Image;
            }
            set
            {
                this.PicImage.Image = value;
            }
        }

        public Enums.EnumModalityPopUp ViewModality
        {
            get
            {
                return _Modality;
            }
            set
            {
                _Modality = value;
            }
        }

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
                this.Text = string.Format("{0} - {1}", MyStatics.GetDataNameModalityFormPU(_Modality), value);
            }
        }

        public void ViewInit()
        {

            this.SuspendLayout();

            this.InitializeUltraToolbarsManager();

            SetBindings();
            SetBindingsStruttura();

            switch (_Modality)
            {

                case Enums.EnumModalityPopUp.mpNuovo:
                    int nVer = int.Parse(DataBase.FindValue("IsNull(Max(Versione),0)", "T_SchedeVersioni", "CodScheda = '" + this.uteCodScheda.Text + "'", "0")) + 1;
                    this.uteVersione.Text = nVer.ToString();
                    this.UltraTabControl.Tabs["tab2"].Visible = false;
                    MyStatics.SetControls(this.UltraGroupBox.Controls, true);
                    this.uteCodScheda.Enabled = false;
                    this.uteVersione.Enabled = false;
                    this.ubApplica.Enabled = true;
                    break;

                case Enums.EnumModalityPopUp.mpModifica:
                    this.UltraTabControl.Tabs["tab2"].Visible = true;
                    MyStatics.SetControls(this.UltraGroupBox.Controls, true);
                    this.uteCodScheda.Enabled = false;
                    this.uteVersione.Enabled = false;
                    this.ubApplica.Enabled = false;
                    break;

                case Enums.EnumModalityPopUp.mpCancella:
                    this.UltraTabControl.Tabs["tab2"].Visible = true;
                    MyStatics.SetControls(this.UltraGroupBox.Controls, false);
                    this.ubApplica.Enabled = false;
                    break;

                case Enums.EnumModalityPopUp.mpVisualizza:
                    this.UltraTabControl.Tabs["tab2"].Visible = true;
                    MyStatics.SetControls(this.UltraGroupBox.Controls, false);
                    this.ubApplica.Enabled = false;
                    this.ubConferma.Enabled = false;
                    break;

                default:
                    break;

            }

            this.ResumeLayout();

        }

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

        #region Subroutine

        private void SetBindings()
        {

            DataColumn _dcol = null;

            try
            {

                _DataBinds.DataBindings.Add("Text", "CodScheda", this.uteCodScheda);
                _DataBinds.DataBindings.Add("Text", "Versione", this.uteVersione);
                _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione);
                _DataBinds.DataBindings.Add("Checked", "FlagAttiva", this.chkFlagAttiva);
                _DataBinds.DataBindings.Add("Checked", "Pubblicato", this.chkPubblicato);
                _DataBinds.DataBindings.Add("Value", "DtValI", this.udteDtValI);
                _DataBinds.DataBindings.Add("Value", "DtValF", this.udteDtValF);


                _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodScheda"];
                if (_dcol.MaxLength > 0) this.uteCodScheda.MaxLength = _dcol.MaxLength;
                _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                if (_dcol.MaxLength > 0) this.uteDescrizione.MaxLength = _dcol.MaxLength;

                _DataBinds.DataBindings.Load();

            }
            catch (Exception)
            {

            }

        }

        private void SetBindingsStruttura()
        {

            try
            {

                DcScheda oScheda = (DcScheda)Serializer.FromXmlString(_DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Struttura"].ToString(), typeof(DcScheda));
                if (oScheda == null)
                {
                    oScheda = new DcScheda();
                    oScheda.ID = this.uteCodScheda.Text;
                    oScheda.Descrizione = this.uteDescrizione.Text;
                }

                DcDecodifiche oDcDecodifiche = new DcDecodifiche();
                using (DataSet oDs = DataBase.GetDataSet("Select Codice, Descrizione From T_DCDecodifiche Order By Descrizione"))
                {
                    foreach (DataRow oDr in oDs.Tables[0].Rows)
                    {
                        DcObject oDcObject = new DcObject();
                        oDcObject.Key = oDr["Codice"].ToString();
                        oDcObject.Value = oDr["Descrizione"].ToString();
                        oDcDecodifiche.Add(oDcObject.Key, oDcObject);
                    }
                }

                DataSet oDsSottoSchede = DataBase.GetDataSet("Select Codice, Descrizione From T_Schede Where SchedaSemplice = 0 Order By Descrizione");
                DcDecodifiche oDcSottoSchede = new DcDecodifiche();
                foreach (DataRow oDr in oDsSottoSchede.Tables[0].Rows)
                {
                    DcObject oDcObject = new DcObject();
                    oDcObject.Key = oDr["Codice"].ToString();
                    oDcObject.Value = string.Format("{0} ({1})", oDr["Descrizione"].ToString(), oDr["Codice"].ToString());
                    oDcSottoSchede.Add(oDcObject.Key, oDcObject);
                }

                DcSchedaLayouts oSchedaLayouts = (DcSchedaLayouts)Serializer.FromXmlString(_DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Layout"].ToString(), typeof(DcSchedaLayouts));
                if (oSchedaLayouts == null)
                {
                    oSchedaLayouts = new DcSchedaLayouts();
                }

                this.ucDcDesigner.Carica(oScheda, oDcDecodifiche, oSchedaLayouts, oDcSottoSchede,
                        new List<KeyValuePair<string, object>>() {
                                            new KeyValuePair<string, object>(EnumAttributiScheda.LayerDWH.ToString(), typeof(bool)),
                                            new KeyValuePair<string, object>(EnumAttributiScheda.LayerDescrizioneDWH.ToString(), typeof(string)) });

                Gestore oGestore = CoreStatics.GetGestore(false, true);

                Scheda o_Scheda = new Scheda(this.uteCodScheda.Text, int.Parse(this.uteVersione.Text), DateTime.Now, MyStatics.Ambiente);
                o_Scheda.StrutturaXML = _DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Struttura"].ToString();
                o_Scheda.LayoutXML = _DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Layout"].ToString();
                o_Scheda.SetExpandXml(DateTime.Now);
                oGestore.SchedaXML = o_Scheda.StrutturaXML;
                oGestore.SchedaLayoutsXML = o_Scheda.LayoutXML;

                oGestore.Decodifiche = o_Scheda.DizionarioValori();
                oGestore.SchedaDati = new DcSchedaDati();
                oGestore.NuovaScheda();

                this.ucDcViewer.VisualizzaTitoloScheda = false;
                this.ucDcViewer.CaricaDati(oGestore);



            }
            catch (Exception)
            {

            }

        }

        private void UpdateBindings()
        {

            try
            {
                if (this.udteDtValI.Value == null) this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["DtValI"] = System.DBNull.Value;
                if (this.udteDtValF.Value == null) this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["DtValF"] = System.DBNull.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void UpdateBindingStruttura()
        {

            try
            {

                DcScheda oDcScheda = this.ucDcDesigner.SalvaScheda();
                DcSchedaLayouts oDcSchedaLayouts = this.ucDcDesigner.SalvaSchedaLayouts();

                oDcScheda.ID = this.uteCodScheda.Text;
                oDcScheda.Descrizione = this.uteDescrizione.Text;

                this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Struttura"] = Serializer.ToXmlString(oDcScheda);
                this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Layout"] = Serializer.ToXmlString(oDcSchedaLayouts);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        private async void UpdateBindingStrutturaV3()
        {

            try
            {

                EavDataStruct EavDataStructV3 = await EavExtensions.CreateEavDataStruct(this.uteCodScheda.Text, null, MyStatics.Configurazione.ConnectionString, versione: int.Parse(this.uteVersione.Text));
                this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["StrutturaV3"] = EavDataStructV3.EavSchema.ToXmlString();
                this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["LayoutV3"] = EavDataStructV3.EavLayout.ToXmlString();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        private bool CheckInput()
        {

            bool bRet = true;
            int Num;

            if (this.uteCodScheda.Text.Trim() == "")
            {
                MessageBox.Show(@"Inserire " + this.lblCodScheda.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.uteCodScheda.Focus();
                bRet = false;
            }
            if (this.uteVersione.Text.Trim() == "" || int.TryParse(this.uteVersione.Text.Trim(), out Num) == false)
            {
                MessageBox.Show(@"Inserire " + this.lblVersione.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.uteVersione.Focus();
                bRet = false;
            }
            if (this.uteDescrizione.Text.Trim() == "")
            {
                MessageBox.Show(@"Inserire " + this.lblDescrizione.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.uteDescrizione.Focus();
                bRet = false;
            }
            string[] arvCheckScheda = DataBase.CheckInputScheda(_DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Struttura"].ToString(), _DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Layout"].ToString());
            if (arvCheckScheda.Length > 0)
            {
                MessageBox.Show(string.Join("\n", arvCheckScheda), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                bRet = false;
            }

            return bRet;

        }

        private DataSet GetDataSetSchede()
        {


            string sSql = string.Empty;

            DataSet oDs = null;
            DataRelation dr = null;

            this.Cursor = Cursors.WaitCursor;

            try
            {

                sSql = "Select SV.CodScheda, S.Descrizione As [Descrizione Scheda], SV.Versione, SV.Descrizione As [Descrizione Versione], SV.DtValI" + Environment.NewLine +
"From T_SchedeVersioni SV" + Environment.NewLine +
"Inner join (Select CodScheda, Max(Versione) As Versione From T_SchedeVersioni Group by CodScheda) SVM" + Environment.NewLine +
"ON SV.CodScheda = SVM.CodScheda And SV.Versione = SVM.Versione" + Environment.NewLine +
"Inner join T_Schede S ON SV.CodScheda = S.Codice" + Environment.NewLine +
"Order By SV.CodScheda, SV.Versione" + Environment.NewLine;

                sSql += "Select SV.CodScheda, SV.Versione," + Environment.NewLine +
"pref.value('(ID/text())[1]', 'varchar(50)') As IDSezione," + Environment.NewLine +
"pref.value('(Descrizione/text())[1]', 'varchar(50)') As Descrizione," + Environment.NewLine +
"pref.value('(Ordine/text())[1]', 'integer') AS Ordine" + Environment.NewLine +
"From T_SchedeVersioni SV" + Environment.NewLine +
"CROSS APPLY Struttura.nodes('/DcScheda/Sezioni/Item/Value/DcSezione') AS Sezione(pref)" + Environment.NewLine +
"Inner join (Select CodScheda, Max(Versione) As Versione From T_SchedeVersioni Group by CodScheda) SVM" + Environment.NewLine +
"ON SV.CodScheda = SVM.CodScheda And SV.Versione = SVM.Versione" + Environment.NewLine +
"Inner join T_Schede S ON SV.CodScheda = S.Codice" + Environment.NewLine +
"Order By SV.CodScheda, SV.Versione, pref.value('(Ordine/text())[1]', 'integer')" + Environment.NewLine;

                sSql += "Select SV.CodScheda, SV.Versione," + Environment.NewLine +
"pref.value('(ID/text())[1]', 'varchar(50)') As IDSezione," + Environment.NewLine +
"prefcampi.value('(ID/text())[1]', 'varchar(50)') AS IDCampo," + Environment.NewLine +
"prefcampi.value('(Descrizione/text())[1]', 'varchar(255)') AS Descrizione," + Environment.NewLine +
"prefcampi.value('(Ordine/text())[1]', 'INTEGER') AS Ordine" + Environment.NewLine +
"From T_SchedeVersioni SV" + Environment.NewLine +
"CROSS APPLY Struttura.nodes('/DcScheda/Sezioni/Item/Value/DcSezione') AS Sezione(pref)" + Environment.NewLine +
"CROSS APPLY pref.nodes('Voci/Item/Value/DcVoce') AS Campi(prefcampi)" + Environment.NewLine +
"Inner join (Select CodScheda, Max(Versione) As Versione From T_SchedeVersioni Group by CodScheda) SVM" + Environment.NewLine +
"ON SV.CodScheda = SVM.CodScheda And SV.Versione = SVM.Versione" + Environment.NewLine +
"Inner join T_Schede S ON SV.CodScheda = S.Codice" + Environment.NewLine +
"Order By SV.CodScheda, SV.Versione, prefcampi.value('(Ordine/text())[1]', 'INTEGER')" + Environment.NewLine;

                oDs = DataBase.GetDataSet(sSql);
                oDs.Tables[0].TableName = "SchedeVersioni";
                oDs.Tables[1].TableName = "Sezioni";
                oDs.Tables[2].TableName = "Campi";

                dr = new DataRelation("SchedeVersioni_Sezioni",
new DataColumn[] { oDs.Tables["SchedeVersioni"].Columns["CodScheda"], oDs.Tables["SchedeVersioni"].Columns["Versione"] },
new DataColumn[] { oDs.Tables["Sezioni"].Columns["CodScheda"], oDs.Tables["Sezioni"].Columns["Versione"] }, false);
                oDs.Relations.Add(dr);
                dr = new DataRelation("Sezioni_Campi",
                                        new DataColumn[] { oDs.Tables["Sezioni"].Columns["CodScheda"], oDs.Tables["Sezioni"].Columns["Versione"], oDs.Tables["Sezioni"].Columns["IDSezione"] },
                                        new DataColumn[] { oDs.Tables["Campi"].Columns["CodScheda"], oDs.Tables["Campi"].Columns["Versione"], oDs.Tables["Campi"].Columns["IDSezione"] }, false);
                oDs.Relations.Add(dr);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            this.Cursor = Cursors.Default;

            return oDs;

        }

        private void AggiungiSezione(string idsezione, string struttura, string layout)
        {

            StringBuilder sb = new StringBuilder();

            DcScheda oSchedaIn = (DcScheda)Serializer.FromXmlString(struttura, typeof(DcScheda));

            DcSchedaLayouts oSchedaLayoutsIn = (DcSchedaLayouts)Serializer.FromXmlString(layout, typeof(DcSchedaLayouts));

            DcScheda oDcSchedaOut = this.ucDcDesigner.SalvaScheda();
            DcSchedaLayouts oDcSchedaLayoutsOut = this.ucDcDesigner.SalvaSchedaLayouts();

            string sSuffisso = string.Empty;
            while (oDcSchedaOut.Sezioni.ContainsKey(idsezione + sSuffisso))
            {
                sSuffisso += "A";
            }
            if (sSuffisso != string.Empty) { sb.AppendLine(string.Format("Sezione '{0}' rinominata in '{1}'", idsezione, idsezione + sSuffisso)); }

            DcSezione oDcSezioneNuova = new DcSezione(new DcObject(oDcSchedaOut));
            oDcSezioneNuova.ID = idsezione + sSuffisso;
            oDcSezioneNuova.Descrizione = oSchedaIn.Sezioni[idsezione].Descrizione;
            oDcSezioneNuova.Ordine = oDcSchedaOut.Sezioni.Count;

            oDcSezioneNuova.Abilitazione = oSchedaIn.Sezioni[idsezione].Abilitazione;
            oDcSezioneNuova.Attributi = oSchedaIn.Sezioni[idsezione].Attributi;
            oDcSezioneNuova.Default = oSchedaIn.Sezioni[idsezione].Default;
            oDcSezioneNuova.Formula = oSchedaIn.Sezioni[idsezione].Formula;
            oDcSezioneNuova.Value = oSchedaIn.Sezioni[idsezione].Value;
            oDcSchedaOut.Sezioni.Add(oDcSezioneNuova);

            foreach (DcVoce oDcVoceIn in oSchedaIn.Sezioni[idsezione].Voci.Values)
            {

                sSuffisso = string.Empty;
                while (oDcSchedaLayoutsOut.Layouts.ContainsKey(oDcVoceIn.ID + sSuffisso))
                {
                    sSuffisso += "A";
                }
                if (sSuffisso != string.Empty) { sb.AppendLine(string.Format("Voce '{0}' rinominata in '{1}'", oDcVoceIn.ID, oDcVoceIn.ID + sSuffisso)); }
                DcVoce oDcVoceNuova = new DcVoce(new DcObject(oDcSezioneNuova));
                oDcVoceNuova.ID = oDcVoceIn.ID + sSuffisso;
                oDcVoceNuova.Descrizione = oDcVoceIn.Descrizione;
                oDcVoceNuova.Ordine = oDcSezioneNuova.Voci.Count;

                oDcVoceNuova.Abilitazione = oDcVoceIn.Abilitazione;
                oDcVoceNuova.Attributi = oDcVoceIn.Attributi;
                oDcVoceNuova.Decodifica = oDcVoceIn.Decodifica;
                oDcVoceNuova.Default = oDcVoceIn.Default;
                oDcVoceNuova.Disattivato = oDcVoceIn.Disattivato;
                oDcVoceNuova.Formato = oDcVoceIn.Formato;
                oDcVoceNuova.Formula = oDcVoceIn.Formula;
                oDcVoceNuova.InRilievo = oDcVoceIn.InRilievo;
                oDcVoceNuova.Note = oDcVoceIn.Note;
                oDcVoceNuova.Didascalia = oDcVoceIn.Didascalia;
                oDcVoceNuova.Obbligatorio = oDcVoceIn.Obbligatorio;
                oDcVoceNuova.Tooltip = oDcVoceIn.Tooltip;
                oDcVoceNuova.Value = oDcVoceIn.Value;
                oDcVoceNuova.VisualizzaInRilievo = oDcVoceIn.VisualizzaInRilievo;
                oDcSezioneNuova.Voci.Add(oDcVoceNuova);

                DcLayout oDcLayoutNuovo = new DcLayout();
                oDcLayoutNuovo.ID = oDcVoceIn.ID + sSuffisso;

                oDcLayoutNuovo.Abilitazione = oSchedaLayoutsIn.Layouts[oDcVoceIn.ID].Abilitazione;
                oDcLayoutNuovo.Attributi = oSchedaLayoutsIn.Layouts[oDcVoceIn.ID].Attributi;
                oDcLayoutNuovo.Default = oSchedaLayoutsIn.Layouts[oDcVoceIn.ID].Default;
                oDcLayoutNuovo.Formula = oSchedaLayoutsIn.Layouts[oDcVoceIn.ID].Formula;
                oDcLayoutNuovo.TipoVoce = oSchedaLayoutsIn.Layouts[oDcVoceIn.ID].TipoVoce;
                oDcLayoutNuovo.Value = oSchedaLayoutsIn.Layouts[oDcVoceIn.ID].Value;
                oDcSchedaLayoutsOut.Layouts.Add(oDcLayoutNuovo);

            }

            this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Struttura"] = Serializer.ToXmlString(oDcSchedaOut);
            this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Layout"] = Serializer.ToXmlString(oDcSchedaLayoutsOut);

            if (sb.Length != 0) { MessageBox.Show(sb.ToString(), this.Text); }

        }

        #endregion

        #region Events

        private void ute_ValueChanged(object sender, EventArgs e)
        {
            this.ubApplica.Enabled = true;
        }

        private void chkFlagAttiva_CheckedChanged(object sender, EventArgs e)
        {
            this.chkPubblicato.Checked = this.chkFlagAttiva.Checked;
            this.ubApplica.Enabled = true;
        }

        private void UltraTabControlDatiClinici_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
        {
            if (e.Tab.Index == 1)
            {
                this.UpdateBindingStruttura();
                Scheda oScheda = new Scheda(this.uteCodScheda.Text, int.Parse(this.uteVersione.Text), DateTime.Now, MyStatics.Ambiente);
                oScheda.StrutturaXML = _DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Struttura"].ToString();
                oScheda.LayoutXML = _DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Layout"].ToString();
                oScheda.SetExpandXml(DateTime.Now);
                this.ucDcViewer.Gestore.SchedaXML = oScheda.StrutturaXML;
                this.ucDcViewer.Gestore.SchedaLayoutsXML = oScheda.LayoutXML;
                this.ucDcViewer.Gestore.Decodifiche = oScheda.DizionarioValori();
                this.ucDcViewer.Gestore.SchedaDati = new DcSchedaDati();
                this.ucDcViewer.Gestore.NuovaScheda();
                this.ucDcViewer.CaricaDati();
            }
            else if (e.Tab.Index == 2)
            {
                if (e.PreviousSelectedTab.Index == 0)
                {
                    this.UpdateBindingStruttura();
                    MovScheda oMovScheda = new MovScheda(this.uteCodScheda.Text, Scci.Enums.EnumEntita.SCH, "", "", "", "", int.Parse(this.uteVersione.Text), MyStatics.Ambiente);
                    oMovScheda.Scheda.StrutturaXML = _DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Struttura"].ToString();
                    oMovScheda.Scheda.LayoutXML = _DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Layout"].ToString();
                    oMovScheda.Scheda.SetExpandXml(DateTime.Now);
                    this.ucDcViewer.Gestore.SchedaXML = oMovScheda.Scheda.StrutturaXML;
                    this.ucDcViewer.Gestore.SchedaLayoutsXML = oMovScheda.Scheda.LayoutXML;
                    this.ucDcViewer.Gestore.Decodifiche = oMovScheda.Scheda.DizionarioValori();
                    this.ucDcViewer.Gestore.SchedaDati = new DcSchedaDati();
                    this.ucDcViewer.Gestore.NuovaScheda();
                    oMovScheda.DatiXML = this.ucDcViewer.Gestore.SchedaDatiXML;
                    this.rtbRichTextBox.Rtf = oMovScheda.AnteprimaRTF;
                }
                else if (e.PreviousSelectedTab.Index == 1)
                {
                    MovScheda oMovScheda = new MovScheda(this.uteCodScheda.Text, Scci.Enums.EnumEntita.SCH, "", "", "", "", int.Parse(this.uteVersione.Text), MyStatics.Ambiente);
                    oMovScheda.Scheda.StrutturaXML = _DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Struttura"].ToString();
                    oMovScheda.Scheda.LayoutXML = _DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Layout"].ToString();
                    oMovScheda.Scheda.SetExpandXml(DateTime.Now);
                    oMovScheda.DatiXML = this.ucDcViewer.Gestore.SchedaDatiXML;
                    this.rtbRichTextBox.Rtf = oMovScheda.AnteprimaRTF;
                }
            }

        }

        private void ubCopia_Click(object sender, EventArgs e)
        {

            try
            {

                frmZoomSchede f = new frmZoomSchede();
                f.ViewText = "Copia Sezioni";
                f.ViewIcon = this.ViewIcon;
                f.ViewDataSet = this.GetDataSetSchede();
                f.ViewInit();
                f.WindowState = FormWindowState.Maximized;
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {

                    string sStruttura = DataBase.FindValue("Struttura", "T_SchedeVersioni", "CodScheda = '" + f.ViewActiveRow.Cells["CodScheda"].Value + "' And Versione = " + f.ViewActiveRow.Cells["Versione"].Value, "");
                    string sLayout = DataBase.FindValue("Layout", "T_SchedeVersioni", "CodScheda = '" + f.ViewActiveRow.Cells["CodScheda"].Value + "' And Versione = " + f.ViewActiveRow.Cells["Versione"].Value, "");

                    this.AggiungiSezione(f.ViewActiveRow.Cells["IDSezione"].Value.ToString(), sStruttura, sLayout);

                    this.SetBindingsStruttura();

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void ubDizionarioQuick_Click(object sender, EventArgs e)
        {

            try
            {

                string sNuovoCodice = DataBase.DCDecodificheQuick();

                this.Cursor = Cursors.WaitCursor;

                if (sNuovoCodice != "")
                {

                    frmPUView2 oPUViewDiz = new frmPUView2();
                    oPUViewDiz.ViewDataNamePU = Enums.EnumDataNames.T_DCDecodifiche;
                    oPUViewDiz.ViewModality = Enums.EnumModalityPopUp.mpModifica;
                    oPUViewDiz.ViewText = "Dizionario Quick";
                    oPUViewDiz.ViewIcon = this.ViewIcon;
                    oPUViewDiz.ViewImage = this.ViewImage;
                    PUDataBindings oPUDataBindingsDiz = oPUViewDiz.ViewDataBindings;
                    DataBase.SetDataBinding(ref oPUDataBindingsDiz, Enums.EnumDataNames.T_DCDecodifiche, Enums.EnumModalityPopUp.mpModifica, sNuovoCodice);

                    oPUViewDiz.ViewInit();
                    oPUViewDiz.ShowDialog();

                    DataSet oDs = DataBase.GetDataSet("Select Codice, Descrizione From T_DCDecodifiche Order By Descrizione");
                    DcDecodifiche oDcDecodifiche = new DcDecodifiche();
                    foreach (DataRow oDr in oDs.Tables[0].Rows)
                    {
                        DcObject oDcObject = new DcObject();
                        oDcObject.Key = oDr["Codice"].ToString();
                        oDcObject.Value = oDr["Descrizione"].ToString();
                        oDcDecodifiche.Add(oDcObject.Key, oDcObject);
                    }

                    this.ucDcDesigner.CaricaDecodifiche(oDcDecodifiche);

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

        }

        private void ubAnnulla_Click(object sender, EventArgs e)
        {
            this.DialogResult = _DialogResult;
            this.Close();
        }

        private void ubApplica_Click(object sender, EventArgs e)
        {

            try
            {

                switch (this.ViewModality)
                {

                    case Enums.EnumModalityPopUp.mpNuovo:
                        if (CheckInput())
                        {
                            this.ViewDataBindings.DsLogPrima = null;
                            this.UpdateBindings();
                            this.UpdateBindingStruttura();
                            this.ViewDataBindings.DataBindings.Update(this.ViewDataBindings.SqlSelect.Sql, MyStatics.Configurazione.ConnectionString, false);
                            this.UpdateBindingStrutturaV3();
                            this.ViewDataBindings.DataBindings.Update(this.ViewDataBindings.SqlSelect.Sql, MyStatics.Configurazione.ConnectionString, false);
                            this.ViewDataBindings.SqlSelect.Where = "CodScheda = '" + this.uteCodScheda.Text + "' And Versione = " + this.uteVersione.Text;
                            this.ViewDataBindings.DsLogDopo = DataBase.GetDataSet(this.ViewDataBindings.SqlSelect.Sql);
                            MyStatics.LogManager(Enums.EnumModalityPopUp.mpNuovo, Enums.EnumEntitaLog.T_SchedeVersioni, this.ViewDataBindings.DsLogPrima, this.ViewDataBindings.DsLogDopo);
                            this.ViewDataBindings.DataBindings.DataSet = DataBase.GetDataSet(this.ViewDataBindings.SqlSelect.Sql);
                            this.ViewModality = Enums.EnumModalityPopUp.mpModifica;
                            this.ViewText = this.ViewText;
                            _DialogResult = System.Windows.Forms.DialogResult.OK;
                            this.ViewInit();
                        }
                        break;

                    case Enums.EnumModalityPopUp.mpModifica:
                        if (CheckInput())
                        {
                            this.ViewDataBindings.DsLogPrima = DataBase.GetDataSet(this.ViewDataBindings.SqlSelect.Sql);
                            this.UpdateBindings();
                            this.UpdateBindingStruttura();
                            this.ViewDataBindings.DataBindings.Update(this.ViewDataBindings.SqlSelect.Sql, MyStatics.Configurazione.ConnectionString, false);
                            this.UpdateBindingStrutturaV3();
                            this.ViewDataBindings.DataBindings.Update(this.ViewDataBindings.SqlSelect.Sql, MyStatics.Configurazione.ConnectionString, false);
                            this.ViewDataBindings.DsLogDopo = DataBase.GetDataSet(this.ViewDataBindings.SqlSelect.Sql);
                            MyStatics.LogManager(Enums.EnumModalityPopUp.mpModifica, Enums.EnumEntitaLog.T_SchedeVersioni, this.ViewDataBindings.DsLogPrima, this.ViewDataBindings.DsLogDopo);
                            _DialogResult = System.Windows.Forms.DialogResult.OK;
                            this.ViewInit();
                        }
                        break;

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void ubConferma_Click(object sender, EventArgs e)
        {

            try
            {

                switch (this.ViewModality)
                {

                    case Enums.EnumModalityPopUp.mpNuovo:
                        if (CheckInput())
                        {
                            this.ViewDataBindings.DsLogPrima = null;
                            this.UpdateBindings();
                            this.UpdateBindingStruttura();
                            this.ViewDataBindings.DataBindings.Update(this.ViewDataBindings.SqlSelect.Sql, MyStatics.Configurazione.ConnectionString, false);
                            this.UpdateBindingStrutturaV3();
                            this.ViewDataBindings.DataBindings.Update(this.ViewDataBindings.SqlSelect.Sql, MyStatics.Configurazione.ConnectionString, false);
                            this.ViewDataBindings.SqlSelect.Where = "CodScheda = '" + this.uteCodScheda.Text + "'";
                            this.ViewDataBindings.DsLogDopo = DataBase.GetDataSet(this.ViewDataBindings.SqlSelect.Sql);
                            MyStatics.LogManager(Enums.EnumModalityPopUp.mpNuovo, Enums.EnumEntitaLog.T_SchedeVersioni, this.ViewDataBindings.DsLogPrima, this.ViewDataBindings.DsLogDopo);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        break;

                    case Enums.EnumModalityPopUp.mpModifica:
                        if (CheckInput())
                        {
                            this.ViewDataBindings.DsLogPrima = DataBase.GetDataSet(this.ViewDataBindings.SqlSelect.Sql);
                            this.UpdateBindings();
                            this.UpdateBindingStruttura();
                            this.ViewDataBindings.DataBindings.Update(this.ViewDataBindings.SqlSelect.Sql, MyStatics.Configurazione.ConnectionString, false);
                            this.UpdateBindingStrutturaV3();
                            this.ViewDataBindings.DataBindings.Update(this.ViewDataBindings.SqlSelect.Sql, MyStatics.Configurazione.ConnectionString, false);
                            this.ViewDataBindings.DsLogDopo = DataBase.GetDataSet(this.ViewDataBindings.SqlSelect.Sql);
                            MyStatics.LogManager(Enums.EnumModalityPopUp.mpModifica, Enums.EnumEntitaLog.T_SchedeVersioni, this.ViewDataBindings.DsLogPrima, this.ViewDataBindings.DsLogDopo);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        break;

                    case Enums.EnumModalityPopUp.mpCancella:
                        if (MessageBox.Show("Confermi la cancellazione ?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
                        {
                            this.ViewDataBindings.DsLogPrima = DataBase.GetDataSet(this.ViewDataBindings.SqlSelect.Sql);
                            DataBase.ExecuteSql(this.ViewDataBindings.SqlDelete.Sql);
                            this.ViewDataBindings.DsLogDopo = null;
                            MyStatics.LogManager(Enums.EnumModalityPopUp.mpCancella, Enums.EnumEntitaLog.T_SchedeVersioni, this.ViewDataBindings.DsLogPrima, this.ViewDataBindings.DsLogDopo);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        break;

                    case Enums.EnumModalityPopUp.mpVisualizza:
                        this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                        this.Close();
                        break;

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        #endregion


    }
}
