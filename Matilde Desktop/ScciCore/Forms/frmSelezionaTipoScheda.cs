using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.ScciResource;
using Infragistics.Win.UltraWinTree;

using Microsoft.Data.SqlClient;
using System.Globalization;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;
using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.DatiClinici.DC;
using UnicodeSrl.DatiClinici.Common.Enums;
using UnicodeSrl.Scci.RTF;
using UnicodeSrl.RTFLibrary.Core;
using UnicodeSrl.Scci.Model;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.ScciCore.Extensions;
using Infragistics.Win.UltraWinGrid;
using UnicodeSrl.ScciCore.CustomControls.Infra;

namespace UnicodeSrl.ScciCore
{
    public partial class frmSelezionaTipoScheda : frmBaseModale, Interfacce.IViewFormlModal
    {
                private bool _skipShowForm = false;

        public frmSelezionaTipoScheda()
        {
                        
                        
            InitializeComponent();

                        this.tlpStep1.Visible = false;
            this.tlpStep2.Visible = false;

            this.CurrentStep = 1;
        }

                                private List<String> XpFiltro_CodScheda { get; set; }

        #region Interface

        public new void Carica()
        {

            try
            {
                                                if ((this.CustomParamaters != null) && (this.CustomParamaters is List<String>))
                {
                    this.XpFiltro_CodScheda = this.CustomParamaters as List<String>;
                }

                _skipShowForm = false;
                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_SCHEDA_256);
                this.Icon = Risorse.GetIconFromResource(Risorse.GC_SCHEDA_16);

                this.setProgressBar(false, 0);
                this.InitializeUltraTree();
                this.LoadUltraTree();

                if (!_skipShowForm) this.ShowDialog();

            }
            catch (Exception)
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }

        }

        #endregion

        #region     Step e UI

        private int m_step = 0;

                                private int CurrentStep
        {
            get { return m_step; }
            set
            {
                if (value != m_step)
                    OnChange_CurrentStep(value);

                m_step = value;
            }
        }

                                private int MaxStep
        {
            get
            {
                if (this.SchedeDaSel == null) return 1;
                return this.SchedeDaSel.Count + 1;
            }
        }

                                        private void OnChange_CurrentStep(int newStep)
        {
            this.tlpStep1.Visible = false;
            this.tlpStep2.Visible = false;

            if (newStep == 1)
            {
                this.tlpStep1.Visible = true;
                this.tlpStep1.Dock = DockStyle.Fill;
                this.tlpCheckCopiaDa.Visible = true;
            }
            else
            if (newStep > 1)
            {
                this.tlpStep2.Visible = true;
                this.tlpStep2.Dock = DockStyle.Fill;
                this.tlpCheckCopiaDa.Visible = false;

                showStepScelta(newStep);
            }

        }

                                private void showStepScelta(int newStep)
        {
                        int idx = newStep - 2;

                        MovScheda movScheda = this.SchedeDaSel[idx];
            List<MSP_CercaSchedaPrecedente> listDaSel = this.SchedeCopia.First(x => x.Key.CodScheda == movScheda.CodScheda).Value;

                        this.lblSchedaNome.Text = $"{movScheda.DescrScheda}"; 
                        this.gridSchedeDaSel.DataSource = null;
            this.gridSchedeDaSel.DataSource = listDaSel;

                        this.gridSchedeDaSel.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
            this.gridSchedeDaSel.DisplayLayout.Override.HeaderAppearance.TextHAlign = Infragistics.Win.HAlign.Left;

            this.gridSchedeDaSel.FilterRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
            this.gridSchedeDaSel.GridCaptionFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
            this.gridSchedeDaSel.HeaderFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
            this.gridSchedeDaSel.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;

            this.gridSchedeDaSel.HideAllColumns();

            UltraGridBand b0 = this.gridSchedeDaSel.DisplayLayout.Bands[0];

            b0.DisplayColumn("DescrScheda", "Scheda", 0, sizeX: 150, weightX: 1f);

            b0.DisplayColumn("DescrUtenteRilevazione", "Autore", 1, sizeX: 150, weightX: 1f);
            b0.DisplayColumn("DataCreazione", "Data Creazione", 2, sizeX: 75, weightX: 0.5f);

            b0.DisplayColumn("DescrUltimaModifica", "Autore Modifica", 3, sizeX: 150, weightX: 1f);
            b0.DisplayColumn("DataUltimaModifica", "Data Modifica", 4, sizeX: 75, weightX: 0.5f);

                        b0.Columns["DataCreazione"].Format = "dd/MM/yyyy HH:mm";
            b0.Columns["DataUltimaModifica"].Format = "dd/MM/yyyy HH:mm";

                        b0.ColHeadersVisible = true;

            
                        MSP_CercaSchedaPrecedente selItem = listDaSel.FirstOrDefault(x => x.SelectedForCopy == true);
            if (selItem == null)
                selItem = listDaSel.First();

            selItem.SelectedForCopy = true;

            UltraGridRow rowActive = this.gridSchedeDaSel.GridRowByID("IDScheda", selItem.IDScheda.ToString());
            rowActive.Selected = true;
            this.gridSchedeDaSel.ActiveRow = rowActive;


                        this.rtf.Rtf = selItem.AnteprimaRTF;

        }

                                                private void setProgressBar(bool mostraProgressBar, int max)
        {
            try
            {
                if (mostraProgressBar)
                {
                    this.pbAvanzamento.Minimum = 0;
                    this.pbAvanzamento.Value = 0;
                    this.pbAvanzamento.Maximum = max;

                    this.TableLayoutPanelZoom.RowStyles[1] = new RowStyle(SizeType.Percent, 85);
                    this.TableLayoutPanelZoom.RowStyles[3] = new RowStyle(SizeType.Percent, 10);
                }
                else
                {
                    this.TableLayoutPanelZoom.RowStyles[3] = new RowStyle(SizeType.Percent, 5);
                    this.TableLayoutPanelZoom.RowStyles[1] = new RowStyle(SizeType.Percent, 90);
                }

                this.pbAvanzamento.Visible = mostraProgressBar;

                Application.DoEvents();

            }
            catch
            {
            }
        }

        private void incrementaProgressBar(int incremento)
        {
            try
            {
                if (this.pbAvanzamento.Value + incremento <= this.pbAvanzamento.Maximum)
                    this.pbAvanzamento.Value += incremento;
                else
                    this.pbAvanzamento.Value = this.pbAvanzamento.Maximum;

                Application.DoEvents();
            }
            catch
            {
            }
        }

                                        private void SetNavigazione(bool enable)
        {
            try
            {
                this.ucBottomModale.Enabled = enable;
                this.UltraTree.Enabled = enable;
                this.uceCopiaDaPrecedente.Enabled = enable;
                this.uceMultiSelezione.Enabled = enable;
                this.uteRicerca.ReadOnly = !enable;
                this.ubRicerca.Enabled = enable;
            }
            catch
            {
            }
        }

        #endregion  Step e UI

        #region UltraTree

        private void InitializeUltraTree()
        {
        }

        private void LoadUltraTree()
        {

            this.Cursor = Cursors.WaitCursor;

            try
            {

                UltraTreeNode oNode = null;
                UltraTreeNode oNodeParent = null;
                string sKey = @"";

                                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodEntita", CoreStatics.CoreApplication.MovSchedaSelezionata.CodEntita);
                op.Parametro.Add("IDEntita", CoreStatics.CoreApplication.MovSchedaSelezionata.IDEntita);
                op.Parametro.Add("IDSchedaPadre", CoreStatics.CoreApplication.MovSchedaSelezionata.IDSchedaPadre);
                op.Parametro.Add("Descrizione", this.uteRicerca.Text);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataTable oDt = Database.GetDataTableStoredProc("MSP_SelTipoScheda", spcoll);

                                if ((this.XpFiltro_CodScheda != null) && (this.XpFiltro_CodScheda.Count > 0))
                {
                    string filterByCod = "";

                    foreach (String codScheda in this.XpFiltro_CodScheda)
                    {
                        filterByCod += "'" + codScheda + "', ";
                    }

                    filterByCod = filterByCod.Substring(0, filterByCod.Length - 2);

                    DataView dvRows = new DataView(oDt);
                    dvRows.RowFilter = "Codice IN (" + filterByCod + ")";

                    oDt = dvRows.ToTable();

                }

                this.UltraTree.Nodes.Clear();

                                oNode = new UltraTreeNode(CoreStatics.TV_ROOT, CoreStatics.TV_ROOT_SCHEDE);
                oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_FOLDER_16));
                oNode.Tag = CoreStatics.TV_ROOT;
                this.UltraTree.Nodes.Add(oNode);

                                foreach (DataRow oDataRow in oDt.Rows)
                {
                    oNode = this.UltraTree.GetNodeByKey(CoreStatics.TV_ROOT);
                    sKey = @"";
                    CoreStatics.g_split = CoreStatics.SetSplit(oDataRow["Path"].ToString(), @"\");
                    for (int i = 0; i < CoreStatics.g_split.Length; i++)
                    {

                        sKey += (sKey != "" ? @"\" : "") + CoreStatics.g_split.GetValue(i).ToString();
                        if (sKey == "") sKey = CoreStatics.TV_ROOT;
                        oNodeParent = this.UltraTree.GetNodeByKey(sKey);
                        if (oNodeParent == null)
                        {

                            oNodeParent = new UltraTreeNode(sKey, CoreStatics.g_split.GetValue(i).ToString());
                            oNodeParent.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_FOLDER_16));
                            oNodeParent.Tag = CoreStatics.TV_PATH;
                            oNode.Nodes.Add(oNodeParent);

                        }

                        oNode = oNodeParent;

                    }

                }

                                foreach (DataRow oDataRow in oDt.Rows)
                {

                    if (oDataRow["Path"].ToString().Trim() == "")
                    {
                        oNodeParent = this.UltraTree.GetNodeByKey(CoreStatics.TV_ROOT);
                        if (oNodeParent != null)
                        {

                            oNode = new UltraTreeNode(oDataRow["Path"].ToString() + @"\" + oDataRow["Codice"].ToString(), oDataRow["Descrizione"].ToString());
                            if ((bool)oDataRow["CartellaAmbulatorialeCodificata"] == false)
                            {
                                oNode.LeftImages.Add(((bool)oDataRow["Contenitore"] == false ? Risorse.GetImageFromResource(Risorse.GC_SCHEDA_16) : Risorse.GetImageFromResource(Risorse.GC_FOLDER_16)));
                                oNode.Tag = CoreStatics.TV_SCHEDA;
                            }
                            else
                            {
                                oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_SCHEDACARTELLAAMBULATORIALE_16));
                                oNode.Tag = CoreStatics.TV_SCHEDA_AMBULATORIALE;
                            }

                            if (this.uceMultiSelezione.Checked)
                            {
                                                                oNode.Override.NodeStyle = NodeStyle.CheckBox;
                            }
                            else
                                oNode.Override.NodeStyle = NodeStyle.Standard;

                            oNodeParent.Nodes.Add(oNode);

                        }
                    }
                    else
                    {
                        oNodeParent = this.UltraTree.GetNodeByKey(oDataRow["Path"].ToString());
                        if (oNodeParent != null)
                        {

                            oNode = new UltraTreeNode(oDataRow["Path"].ToString() + @"\" + oDataRow["Codice"].ToString(), oDataRow["Descrizione"].ToString());
                            if ((bool)oDataRow["CartellaAmbulatorialeCodificata"] == false)
                            {
                                oNode.LeftImages.Add(((bool)oDataRow["Contenitore"] == false ? Risorse.GetImageFromResource(Risorse.GC_SCHEDA_16) : Risorse.GetImageFromResource(Risorse.GC_FOLDER_16)));
                                oNode.Tag = CoreStatics.TV_SCHEDA;
                            }
                            else
                            {
                                oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_SCHEDACARTELLAAMBULATORIALE_16));
                                oNode.Tag = CoreStatics.TV_SCHEDA_AMBULATORIALE;
                            }

                            if (this.uceMultiSelezione.Checked)
                            {
                                                                oNode.Override.NodeStyle = NodeStyle.CheckBox;
                            }
                            else
                                oNode.Override.NodeStyle = NodeStyle.Standard;

                            oNodeParent.Nodes.Add(oNode);

                        }
                    }

                }

                                                if (oDt.Rows.Count == 1)
                {
                                        this.UltraTree.PerformAction(UltraTreeAction.FirstNode, false, false);
                    this.UltraTree.PerformAction(UltraTreeAction.ExpandAllNode, true, true);

                    UltraTreeNode nodoDaSelezionare = this.UltraTree.GetNodeByKey(oDt.Rows[0]["Path"].ToString() + @"\" + oDt.Rows[0]["Codice"].ToString());
                    if (nodoDaSelezionare != null)
                    {
                        this.UltraTree.ActiveNode = nodoDaSelezionare;
                        this.UltraTree.ActiveNode.Selected = true;
                    }
                }
                else
                {
                                        this.UltraTree.PerformAction(UltraTreeAction.FirstNode, false, false);
                    this.UltraTree.PerformAction(UltraTreeAction.ExpandAllNode, false, false);
                }


            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            this.Cursor = Cursors.Default;

        }

                                        private void setTreeViewSelezioneMultipla(ref UltraTreeNode roNodeParent)
        {
            UltraTreeNode oNodeParent = roNodeParent;
            if (oNodeParent == null)
            {
                                oNodeParent = this.UltraTree.GetNodeByKey(CoreStatics.TV_ROOT);
            }

            if (oNodeParent != null)
            {
                foreach (UltraTreeNode node in oNodeParent.Nodes)
                {
                    if ((string)node.Tag == CoreStatics.TV_SCHEDA)
                    {
                        if (this.uceMultiSelezione.Checked)
                        {
                                                        node.Override.NodeStyle = NodeStyle.CheckBox;
                        }
                        else
                            node.Override.NodeStyle = NodeStyle.Standard;
                    }
                    else
                    {
                                                UltraTreeNode nodeCartella = node;
                        setTreeViewSelezioneMultipla(ref nodeCartella);
                    }
                }
            }

        }

                                        private void impostaMovSchedeSelezionate(ref UltraTreeNode roNodeParent)
        {
            UltraTreeNode oNodeParent = roNodeParent;
            if (oNodeParent == null)
            {
                                oNodeParent = this.UltraTree.GetNodeByKey(CoreStatics.TV_ROOT);
            }

            if (oNodeParent != null)
            {
                foreach (UltraTreeNode node in oNodeParent.Nodes)
                {
                    if ((string)node.Tag == CoreStatics.TV_SCHEDA)
                    {
                        if (node.CheckedState == CheckState.Checked)
                        {
                            CoreStatics.g_split = CoreStatics.SetSplit(node.Key, @"\");
                            MovScheda newScheda = CoreStatics.CoreApplication.MovSchedaSelezionata.Clona();

                            newScheda.CodScheda = CoreStatics.g_split.GetValue(CoreStatics.g_split.Length - 1).ToString();
                            newScheda.DescrScheda = node.Text;
                            CoreStatics.CoreApplication.MovSchedeSelezionate.Add(newScheda);
                        }
                    }
                    else
                    {
                                                UltraTreeNode nodeCartella = node;
                        impostaMovSchedeSelezionate(ref nodeCartella);
                    }
                }
            }
        }

        #endregion

        #region Proc Copia Schede

                                private List<MovScheda> SchedeDaSel { get; set; }

                                        private Dictionary<MovScheda, List<MSP_CercaSchedaPrecedente>> SchedeCopia { get; set; }

                                                        private bool checkSchedeDaCopiare()
        {
                        this.SchedeDaSel = new List<MovScheda>();
            this.SchedeCopia = new Dictionary<MovScheda, List<MSP_CercaSchedaPrecedente>>();

            bool check = false;

                        if (this.uceMultiSelezione.Checked == false)
            {
                                queueScheda(CoreStatics.CoreApplication.MovSchedaSelezionata);
            }

            else

            if (this.uceMultiSelezione.Checked == true)
            {
                                foreach (MovScheda item in CoreStatics.CoreApplication.MovSchedeSelezionate)
                {
                    queueScheda(item);
                }
            }

            check = (this.SchedeDaSel.Count > 0);
            return check;

        }

                                                private void queueScheda(MovScheda movScheda)
        {

                        using (FwDataConnection fdc = new FwDataConnection(Database.ConnectionString))
            {
                FwDataBufferedList<MSP_CercaSchedaPrecedente> listSchede = fdc.MSP_CercaSchedaPrecedente(
                                                                                movScheda.CodScheda,
                                                                                CoreStatics.CoreApplication.Paziente.ID,
                                                                                CoreStatics.CoreApplication.Ambiente);

                                if ((listSchede == null) || (listSchede.Buffer.Count == 0)) return;

                                                if ((movScheda.Scheda.CopiaPrecedenteSelezione) && (listSchede.Buffer.Count > 1))
                {
                    this.SchedeDaSel.Add(movScheda);

                }
                else
                {
                                        foreach (MSP_CercaSchedaPrecedente item in listSchede.Buffer)
                    {
                        item.SelectedForCopy = true;
                    }
                }

                                listSchede.Buffer.First().SelectedForCopy = true;

                                this.SchedeCopia.Add(movScheda, listSchede.Buffer);


            }

        }

                                        private bool CopiaDaSchedePrecedenti()
        {

            try
            {
                this.setProgressBar(true, this.SchedeCopia.Count + 1);

                                foreach (KeyValuePair<MovScheda, List<MSP_CercaSchedaPrecedente>> kvp in this.SchedeCopia)
                {
                    this.CopiaDaPrecedente(kvp.Key, kvp.Value);
                    incrementaProgressBar(1);
                }
            }
            catch (Exception ex)
            {

                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                easyStatics.EasyErrorMessageBox($"Si è verificato un errore: {ex.Message} ", "Errore Copia Schede");
                return false;
            }

            return true;

        }

                                                        private bool CopiaDaPrecedente(MovScheda movScheda, List<MSP_CercaSchedaPrecedente> listSchedePrec)
        {

            try
            {
                                List<MSP_CercaSchedaPrecedente> listSchede = listSchedePrec.Where(x => x.SelectedForCopy == true).ToList();

                                if ((listSchede == null) || (listSchede.Count == 0)) return false;

                                Gestore gestSel = this.GestoreDaScheda(movScheda);
                gestSel.NuovaScheda();

                                foreach (MSP_CercaSchedaPrecedente datiScheda in listSchede)
                {
                                        List<String> sezReset = new List<string>();

                                        MovScheda oMovSchedaPrecedente = new MovScheda(datiScheda.IDScheda.ToString(), CoreStatics.CoreApplication.Ambiente);

                                        Gestore gestPrec = this.GestoreDaScheda(oMovSchedaPrecedente);

                                                            var precIDs = from p in gestPrec.SchedaDati.Dati.Values
                                  group p by p.ID into g
                                  select new
                                  {
                                      ID = g.Key,
                                  };


                    foreach (var dato in precIDs)
                    {

                                                Func<KeyValuePair<string, DcDato>, bool> fpattern = new Func<KeyValuePair<string, DcDato>, bool>
                            (
                                x => ((x.Value.ID == dato.ID) && (x.Value.Value != null) && (x.Value.Value.ToString() != ""))
                            );

                                                var selMatch = gestSel.SchedaDati.Dati.Where(x => x.Value.ID == dato.ID);

                                                if (selMatch.Count() == 0) continue;

                                                DcDato datoSel = selMatch.First().Value;

                                                DcVoce voce = gestSel.LeggeVoce(dato.ID);
                        DcVoce vocePrec = gestPrec.LeggeVoce(dato.ID);

                        if (voce != null && vocePrec != null)
                        {

                                                        bool sezRipe = this.CheckSezioneRipetibile(gestSel.Scheda.Sezioni[voce.Padre.Key]);
                            bool sezRipPrec = this.CheckSezioneRipetibile(gestPrec.Scheda.Sezioni[vocePrec.Padre.Key]);

                            
                            DcLayout laySel = gestSel.SchedaLayouts.Layouts[dato.ID];
                            DcLayout layPrec = gestPrec.SchedaLayouts.Layouts[dato.ID];

                                                        List<KeyValuePair<string, DcDato>> listCpy = gestPrec.SchedaDati.Dati.Where(fpattern).OrderBy(x => x.Value.Sequenza).ToList();


                                                                                    if ((sezRipe == true) || (sezRipe == sezRipPrec))
                            {
                                string idSezione = voce.Padre.Key;

                                                                foreach (KeyValuePair<String, DcDato> kvp in listCpy)
                                {
                                    DcDato datoCpy = kvp.Value;

                                    if ((sezRipe) && (sezReset.Exists(x => x == idSezione) == false))
                                    {
                                                                                                                        int nMax = this.getMaxIndex(gestSel, voce);

                                        for (int i = 1; i <= nMax; i++)
                                        {
                                            gestSel.CancellaRiga(voce.Padre.Key, i);
                                        }

                                        sezReset.Add(idSezione);                                     }

                                                                        checkAndCreateSequence(gestSel, voce.Padre.Key, datoCpy);

                                    DcDato datoToAdd = gestSel.SchedaDati.Dati[datoCpy.Key];

                                                                                                            if ((String.IsNullOrEmpty(voce.Default) == false) &&
                                        (laySel.TipoVoce != enumTipoVoce.Marker))
                                    {
                                        continue;
                                    }

                                                                        if (laySel.TipoVoce == enumTipoVoce.Scheda)
                                    {
                                        gestPrec.AddGestore(datoCpy.Key);
                                        datoCpy.Value = getCopiaPrecedenteSottoScheda(gestSel.Gestori[datoCpy.Key], gestPrec.Gestori[datoCpy.Key]);
                                    }

                                                                        datoToAdd.Value = datoCpy.Value;

                                                                        
                                                                        if (laySel.TipoVoce == enumTipoVoce.Combo || laySel.TipoVoce == enumTipoVoce.Zoom || laySel.TipoVoce == enumTipoVoce.Multipla || laySel.TipoVoce == enumTipoVoce.ListaSingola)
                                    {
                                        datoToAdd.Transcodifica = datoCpy.Transcodifica;
                                    }

                                                                        gestSel.SchedaDati.Dati[datoCpy.Key] = datoToAdd;

                                    if ((laySel.TipoVoce == enumTipoVoce.Testo) && (layPrec.TipoVoce == enumTipoVoce.TestoRtf))
                                    {
                                                                                gestSel.ModificaValore(datoCpy.Key, getTestoRtfToTesto(datoCpy.Value));
                                    }
                                    else if ((laySel.TipoVoce == enumTipoVoce.TestoRtf) && (layPrec.TipoVoce == enumTipoVoce.Testo))
                                    {
                                                                                gestSel.ModificaValore(datoCpy.Key, getTestoToTestoRtf(datoCpy.Value));
                                    }

                                } 
                            }

                        }

                    } 
                }

                                checkAndFixSections(ref gestSel);

                                
                movScheda.DatiXML = gestSel.SchedaDatiXML;


            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CopiaDaPrecedente", this.Name);
            }

            return true;

        }

        private object getCopiaPrecedenteSottoScheda(Gestore gestSel, Gestore gestPrec)
        {

            try
            {

                                gestSel.NuovaScheda();

                                List<String> sezReset = new List<string>();

                                                var precIDs = from p in gestPrec.SchedaDati.Dati.Values
                              group p by p.ID into g
                              select new
                              {
                                  ID = g.Key,
                              };

                foreach (var dato in precIDs)
                {

                                        Func<KeyValuePair<string, DcDato>, bool> fpattern = new Func<KeyValuePair<string, DcDato>, bool>
                        (
                            x => ((x.Value.ID == dato.ID) && (x.Value.Value != null) && (x.Value.Value.ToString() != ""))
                        );

                                        var selMatch = gestSel.SchedaDati.Dati.Where(x => x.Value.ID == dato.ID);

                                        if (selMatch.Count() == 0) continue;

                                        DcDato datoSel = selMatch.First().Value;

                                        DcVoce voce = gestSel.LeggeVoce(dato.ID);
                    DcVoce vocePrec = gestPrec.LeggeVoce(dato.ID);

                    if (voce != null && vocePrec != null)
                    {

                                                bool sezRipe = this.CheckSezioneRipetibile(gestSel.Scheda.Sezioni[voce.Padre.Key]);
                        bool sezRipPrec = this.CheckSezioneRipetibile(gestPrec.Scheda.Sezioni[vocePrec.Padre.Key]);

                        
                        DcLayout laySel = gestSel.SchedaLayouts.Layouts[dato.ID];
                        DcLayout layPrec = gestPrec.SchedaLayouts.Layouts[dato.ID];

                                                List<KeyValuePair<string, DcDato>> listCpy = gestPrec.SchedaDati.Dati.Where(fpattern).OrderBy(x => x.Value.Sequenza).ToList();


                                                                        if ((sezRipe == true) || (sezRipe == sezRipPrec))
                        {
                            string idSezione = voce.Padre.Key;

                                                        foreach (KeyValuePair<String, DcDato> kvp in listCpy)
                            {
                                DcDato datoCpy = kvp.Value;

                                if ((sezRipe) && (sezReset.Exists(x => x == idSezione) == false))
                                {
                                                                                                            int nMax = this.getMaxIndex(gestSel, voce);

                                    for (int i = 1; i <= nMax; i++)
                                    {
                                        gestSel.CancellaRiga(voce.Padre.Key, i);
                                    }

                                    sezReset.Add(idSezione);                                 }

                                                                checkAndCreateSequence(gestSel, voce.Padre.Key, datoCpy);

                                DcDato datoToAdd = gestSel.SchedaDati.Dati[datoCpy.Key];

                                                                                                if ((String.IsNullOrEmpty(voce.Default) == false) &&
                                    (laySel.TipoVoce != enumTipoVoce.Marker))
                                {
                                    continue;
                                }

                                if (laySel.TipoVoce == enumTipoVoce.Scheda)
                                {
                                    datoCpy.Value = getCopiaPrecedenteSottoScheda(gestSel.Gestori[datoCpy.Key], gestPrec.Gestori[datoCpy.Key]);
                                }

                                                                datoToAdd.Value = datoCpy.Value;

                                                                
                                                                if (laySel.TipoVoce == enumTipoVoce.Combo || laySel.TipoVoce == enumTipoVoce.Zoom || laySel.TipoVoce == enumTipoVoce.Multipla || laySel.TipoVoce == enumTipoVoce.ListaSingola)
                                {
                                    datoToAdd.Transcodifica = datoCpy.Transcodifica;
                                }

                                                                gestSel.SchedaDati.Dati[datoCpy.Key] = datoToAdd;

                                if ((laySel.TipoVoce == enumTipoVoce.Testo) && (layPrec.TipoVoce == enumTipoVoce.TestoRtf))
                                {
                                                                        gestSel.ModificaValore(datoCpy.Key, getTestoRtfToTesto(datoCpy.Value));
                                }
                                else if ((laySel.TipoVoce == enumTipoVoce.TestoRtf) && (layPrec.TipoVoce == enumTipoVoce.Testo))
                                {
                                                                        gestSel.ModificaValore(datoCpy.Key, getTestoToTestoRtf(datoCpy.Value));
                                }

                            } 
                        }

                    }

                } 


                                checkAndFixSections(ref gestSel);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CopiaDaPrecedente", this.Name);
            }

            return gestSel.SchedaDati;

        }

                                                private int getMaxIndex(Gestore gest, DcVoce voce)
        {
                        List<DcDato> listDatiVoce = gest.SchedaDati.Dati.Values.Where(x => x.ID == voce.ID).ToList();

                        if ((listDatiVoce == null) || (listDatiVoce.Count == 0))
                return 0;

                        int max = listDatiVoce.Max(dato => dato.Sequenza);

            return max;
        }

                                        private void checkAndCreateSequence(Gestore gestSel, string sectionKey, DcDato datoCpy)
        {
                        KeyValuePair<String, DcDato> datoCheck = gestSel.SchedaDati.Dati.FirstOrDefault(x => x.Key == datoCpy.Key);

            if (datoCheck.Key == null)
                                gestSel.NuovaRiga(sectionKey, datoCpy.Sequenza);

        }

                                                private void checkAndFixSections(ref Gestore gestSel)
        {

            foreach (DcSezione dcSez in gestSel.Scheda.Sezioni.Values)
            {
                bool isRipetibile = this.CheckSezioneRipetibile(gestSel.Scheda.Sezioni[dcSez.Key]);

                if ((isRipetibile) && (dcSez.Voci.Count > 0))
                {
                                        DcVoce firstVoce = dcSez.Voci.Values.ToArray()[0];

                                        List<DcDato> listDatiVoce = gestSel.SchedaDati.Dati.Values.Where(x => x.ID == firstVoce.ID).ToList();
                    int min = listDatiVoce.Min(dato => dato.Sequenza);
                    int max = min;

                    if (min > 1)
                    {
                                                max = listDatiVoce.Max(dato => dato.Sequenza);
                        int newIndex = 1;

                        for (int i = min; i <= max; i++)
                        {
                            shiftSequenza(ref gestSel, dcSez, i, newIndex);
                            newIndex++;
                        }
                    } 
                                        min = listDatiVoce.Min(dato => dato.Sequenza);
                    max = listDatiVoce.Max(dato => dato.Sequenza);

                    int newRow = 0;

                    for (int i = min; i <= max; i++)
                    {
                        bool datoExist = listDatiVoce.Exists(dato => dato.Sequenza == i);

                        if (datoExist == false)
                        {
                            if (i == max)
                                                                gestSel.CancellaRigaAt(dcSez.Key, i);
                            else
                            {
                                                                var validRows = listDatiVoce.Where(x => x.Sequenza < (i + 1));
                                newRow = validRows.Max(x => x.Sequenza) + 1;
                                shiftSequenza(ref gestSel, dcSez, i + 1, newRow);
                            }
                        }

                    }

                } 
            } 
        }

                                                                private void shiftSequenza(ref Gestore gestSel, DcSezione dcSez, int fromIndex, int toIndex)
        {
                        IEnumerable<DcDato> enDatiToChange = gestSel.SchedaDati.Dati.Values.Where
                (
                    dato => ((dcSez.Voci.Values.FirstOrDefault(v => v.ID == dato.ID) != null) && (dato.Sequenza == fromIndex))
                );

            Dictionary<String, DcDato> keysToUpdate = new Dictionary<string, DcDato>();

            foreach (DcDato datoObj in enDatiToChange)
            {
                string oldKey = datoObj.Key;
                datoObj.Sequenza = toIndex;

                keysToUpdate.Add(oldKey, datoObj);
            }

                        foreach (KeyValuePair<String, DcDato> kvp in keysToUpdate)
            {
                gestSel.SchedaDati.Dati.Remove(kvp.Key);
                gestSel.SchedaDati.Dati.Add(kvp.Value);
            }

        }

                                                private Gestore GestoreDaScheda(MovScheda movScheda)
        {
                                                Gestore ptrg = CoreStatics.GetGestore();
            ptrg.SchedaXML = movScheda.Scheda.StrutturaXML;
            ptrg.SchedaLayoutsXML = movScheda.Scheda.LayoutXML;
            ptrg.Decodifiche = movScheda.Scheda.DizionarioValori();
            ptrg.SchedaDatiXML = movScheda.DatiXML;

            return ptrg;
        }

        private bool CheckSezioneRipetibile(DcSezione sezione)
        {

            bool bret = false;

            if (sezione.Attributi.ContainsKey(EnumAttributiSezione.Ripetibile.ToString()) == true)
            {
                bret = bool.Parse(((DcAttributo)sezione.Attributi[EnumAttributiSezione.Ripetibile.ToString()]).Value.ToString());
            }

            return bret;

        }

        private object getTestoToTestoRtf(object valore)
        {

            object oRet = valore;

            RtfFiles rtf = new RtfFiles();
            string rtfAnteprima = "";

            try
            {

                System.Drawing.Font f = rtf.getFontFromString(Database.GetConfigTable(EnumConfigTable.FontPredefinitoRTF), false, false);
                rtfAnteprima = rtf.initRtf(f);
                rtfAnteprima = rtf.appendRtfText(rtfAnteprima, valore.ToString(), f);

                oRet = rtfAnteprima;

            }
            catch (Exception)
            {

            }

            return oRet;

        }

        private object getTestoRtfToTesto(object valore)
        {

            object oRet = valore;

            RtfTree Tree = new RtfTree();

            try
            {

                Tree.LoadRtfText(valore.ToString());

                oRet = Tree.Text;

            }
            catch (Exception)
            {

            }

            return oRet;

        }



        #endregion  Proc Copia Schede

        #region Events Form

        private void frmSelezionaTipoScheda_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {

            try
            {
                bool bContinue = false;
                this.SetNavigazione(false);

                
                bContinue = false;

                                if (this.CurrentStep == 1)
                {
                    if (this.uceMultiSelezione.Checked)
                    {
                                                UltraTreeNode x = null;
                        impostaMovSchedeSelezionate(ref x);
                        bContinue = (CoreStatics.CoreApplication.MovSchedeSelezionate.Count > 0);
                    }
                    else
                    {
                                                if (this.UltraTree.SelectedNodes.Count > 0 && (string)this.UltraTree.SelectedNodes[0].Tag == CoreStatics.TV_SCHEDA)
                        {
                            CoreStatics.g_split = CoreStatics.SetSplit(this.UltraTree.SelectedNodes[0].Key, @"\");
                            CoreStatics.CoreApplication.MovSchedaSelezionata.CodScheda = CoreStatics.g_split.GetValue(CoreStatics.g_split.Length - 1).ToString();
                            CoreStatics.CoreApplication.MovSchedaSelezionata.DescrScheda = this.UltraTree.SelectedNodes[0].Text;
                            CoreStatics.CoreApplication.MovSchedaSelezionata.CartellaAmbulatorialeCodificata = false;
                            bContinue = true;
                        }
                        else if (this.UltraTree.SelectedNodes.Count > 0 && (string)this.UltraTree.SelectedNodes[0].Tag == CoreStatics.TV_SCHEDA_AMBULATORIALE)
                        {
                                                        CoreStatics.g_split = CoreStatics.SetSplit(this.UltraTree.SelectedNodes[0].Key, @"\");
                            CoreStatics.CoreApplication.MovSchedaSelezionata.CodScheda = CoreStatics.g_split.GetValue(CoreStatics.g_split.Length - 1).ToString();
                            CoreStatics.CoreApplication.MovSchedaSelezionata.DescrScheda = this.UltraTree.SelectedNodes[0].Text;
                            CoreStatics.CoreApplication.MovSchedaSelezionata.CartellaAmbulatorialeCodificata = true;
                            bContinue = true;
                        }
                    }

                                        if (bContinue == false)
                    {
                        easyStatics.EasyMessageBox("Nessun scheda selezionata.", "Nuova Scheda", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }


                this.ImpostaCursore(enum_app_cursors.WaitCursor);

                                                                                
                if ((this.uceCopiaDaPrecedente.Checked == true) && (this.CurrentStep == 1))
                {
                                        this.checkSchedeDaCopiare();
                }

                if ((this.uceCopiaDaPrecedente.Checked == true) && (this.CurrentStep < this.MaxStep))
                {
                                        this.CurrentStep += 1;
                    return;
                }

                                if ((this.CurrentStep == this.MaxStep) && (this.uceCopiaDaPrecedente.Checked == true))
                {
                    bContinue = this.CopiaDaSchedePrecedenti();
                }

                                if (bContinue)
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "frmSelezionaTipoScheda_PulsanteAvantiClick", this.Name);
            }
            finally
            {
                this.SetNavigazione(true);
                this.setProgressBar(false, 0);
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }

        }

        private void frmSelezionaTipoScheda_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
                        if (this.CurrentStep <= 1)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                this.Close();
            }
            else
                this.CurrentStep -= 1;

        }

        private void frmSelezionaTipoScheda_FormClosed(object sender, FormClosedEventArgs e)
        {
                        easyStatics.setTreeViewCheckBoxesStyle();
        }

        #endregion

        #region Events

        private void ubRicerca_Click(object sender, EventArgs e)
        {

            this.LoadUltraTree();
        }

        private void uteRicerca_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter && this.ubRicerca.Enabled)
                {
                    ubRicerca_Click(this.ubRicerca, new EventArgs());
                }
            }
            catch
            {
            }
        }

        private void frmSelezionaTipoScheda_Shown(object sender, EventArgs e)
        {
            try
            {
                this.uteRicerca.Focus();
            }
            catch
            {
            }
        }


        private void uceMultiSelezione_CheckedChanged(object sender, EventArgs e)
        {
                                                UltraTreeNode x = null;
            setTreeViewSelezioneMultipla(ref x);
        }

        private void UltraTree_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (this.uceMultiSelezione.Checked
                    && e.Clicks == 1
                    && e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    UltraTreeNode child = ((UltraTree)sender).GetNodeFromPoint(e.Location);
                    if (child != null && (string)child.Tag == CoreStatics.TV_SCHEDA)
                    {
                        int minleftchk = 48 + (30 * child.Level);

                        if (child.NodeStyleResolved == NodeStyle.CheckBox)
                        {
                            if (e.Location.X > minleftchk)
                            {
                                if (child.CheckedState == CheckState.Checked)
                                    child.CheckedState = CheckState.Unchecked;
                                else
                                    child.CheckedState = CheckState.Checked;
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

                                                private void gridSchedeDaSel_ClickCell(object sender, Infragistics.Win.UltraWinGrid.ClickCellEventArgs e)
        {
            
                                    
                                                                                    
        }

        private void gridSchedeDaSel_Resize(object sender, EventArgs e)
        {
                        this.gridSchedeDaSel.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);
        }

        private void gridSchedeDaSel_VisibleChanged(object sender, EventArgs e)
        {
                        this.gridSchedeDaSel.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);
        }

                                                private void gridSchedeDaSel_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            this.rtf.Clear();

            try
            {

                                MSP_CercaSchedaPrecedente boundObject = (MSP_CercaSchedaPrecedente)this.gridSchedeDaSel.ActiveRow.ListObject;
                this.rtf.Rtf = boundObject.AnteprimaRTF;

                                List<MSP_CercaSchedaPrecedente> list = (List<MSP_CercaSchedaPrecedente>)this.gridSchedeDaSel.DataSource;

                foreach (MSP_CercaSchedaPrecedente item in list)
                {
                    if (item.IDScheda == boundObject.IDScheda) item.SelectedForCopy = true;
                    else item.SelectedForCopy = false;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        #endregion


    }
}
