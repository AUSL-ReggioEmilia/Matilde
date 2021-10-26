using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using UnicodeSrl.ScciResource;
using Infragistics.Win.UltraWinTree;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;
using UnicodeSrl.ScciCore.Common.ProtocolliPrescrizioni;
using Infragistics.Win.UltraWinListView;
using Infragistics.Win.UltraWinEditors;

namespace UnicodeSrl.ScciCore
{
    public partial class frmSelezionaTipoPrescrizione : frmBaseModale, Interfacce.IViewFormlModal
    {
        public frmSelezionaTipoPrescrizione()
        {
            InitializeComponent();
        }

        #region Declare

        private ucEasyListBox _ucEasyListBox = null;

        private List<string> _generatedPrestsIDs = null;

        #endregion

        #region Interface

        public new void Carica()
        {

            try
            {

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_PRESCRIZIONE_256);
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_PRESCRIZIONE_16);

                this.InitializeUltraTree();
                this.LoadUltraTree();
                LoadUltraTreeProtocolli();

                InitializeUltraTab();

                this.ShowDialog();

            }
            catch (Exception)
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }


        }

        public List<string> GeneratedPrestsIDs
        {
            get { return _generatedPrestsIDs; }
        }

        #endregion

        #region Functions
        private ucEasyListBox GetEasyListBoxForPopupControlContainer(object sender)
        {

            DateTime dt = (DateTime)((ucEasyDateTimeEditor)sender).Value;

            ucEasyListBox _ucEasyListBox = new ucEasyListBox();

            try
            {

                _ucEasyListBox.Size = new Size(150, 300);
                _ucEasyListBox.ItemSettings.SelectionType = Infragistics.Win.UltraWinListView.SelectionType.Single;
                _ucEasyListBox.TextFontRelativeDimension = ((ucEasyDateTimeEditor)sender).TextFontRelativeDimension;
                _ucEasyListBox.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
                _ucEasyListBox.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.Details;
                _ucEasyListBox.ViewSettingsDetails.AllowColumnMoving = false;
                _ucEasyListBox.ViewSettingsDetails.AllowColumnSizing = false;
                _ucEasyListBox.ViewSettingsDetails.AllowColumnSorting = false;
                _ucEasyListBox.ViewSettingsDetails.AutoFitColumns = Infragistics.Win.UltraWinListView.AutoFitColumns.ResizeAllColumns;
                _ucEasyListBox.ViewSettingsDetails.FullRowSelect = true;
                _ucEasyListBox.ViewSettingsDetails.ImageSize = new System.Drawing.Size(0, 0);
                _ucEasyListBox.ViewSettingsList.ImageSize = new System.Drawing.Size(0, 0);

                UltraListViewItem oVal = null;
                DateTime valoreitem = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);

                _ucEasyListBox.Items.Clear();

                for (int i = 0; i < 24; i++)
                {
                    oVal = new Infragistics.Win.UltraWinListView.UltraListViewItem(i.ToString());
                    oVal.Value = valoreitem.ToString("HH:mm");
                    _ucEasyListBox.Items.Add(oVal);
                    valoreitem = valoreitem.AddHours(1);
                }

            }
            catch (Exception)
            {

            }

            return _ucEasyListBox;

        }

        #endregion

        #region UltraTab

        private void InitializeUltraTab()
        {

        }

        #endregion

        #region UltraTree

        private void InitializeUltraTree()
        {

            this.UltraTree.LeftImagesSize = new Size(32, 32);
            UltraTreeProtocolli.LeftImagesSize = new Size(32, 32);

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
                op.Parametro.Add("CodUA", CoreStatics.CoreApplication.MovPrescrizioneSelezionata.CodUA);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                op.Parametro.Add("CodAzione", EnumAzioni.INS.ToString());
                op.Parametro.Add("DatiEstesi", "1");
                op.Parametro.Add("Descrizione", this.uteRicerca.Text);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataTable oDt = Database.GetDataTableStoredProc("MSP_SelTipoPrescrizione", spcoll);

                this.UltraTree.Nodes.Clear();

                                oNode = new UltraTreeNode(CoreStatics.TV_ROOT, CoreStatics.TV_ROOT_TIPOPRESCRIZIONI);
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
                                                        if (oDataRow.IsNull("Icona") == false)
                            {
                                oNode.LeftImages.Add(DrawingProcs.GetImageFromByte(oDataRow["Icona"], new Size(32, 32), true));
                            }
                            oNode.Tag = CoreStatics.TV_TIPOPRESCRIZIONE;
                            oNodeParent.Nodes.Add(oNode);

                        }
                    }
                    else
                    {
                        oNodeParent = this.UltraTree.GetNodeByKey(oDataRow["Path"].ToString());
                        if (oNodeParent != null)
                        {

                            oNode = new UltraTreeNode(oDataRow["Path"].ToString() + @"\" + oDataRow["Codice"].ToString(), oDataRow["Descrizione"].ToString());
                                                        if (oDataRow.IsNull("Icona") == false)
                            {
                                oNode.LeftImages.Add(DrawingProcs.GetImageFromByte(oDataRow["Icona"], new Size(32, 32), true));
                            }
                            oNode.Tag = CoreStatics.TV_TIPOPRESCRIZIONE;
                            oNodeParent.Nodes.Add(oNode);

                        }
                    }

                }

                this.UltraTree.PerformAction(UltraTreeAction.FirstNode, false, false);
                this.UltraTree.PerformAction(UltraTreeAction.ExpandAllNode, false, false);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            this.Cursor = Cursors.Default;

        }

        private void LoadUltraTreeProtocolli()
        {

            this.Cursor = Cursors.WaitCursor;

            try
            {

                UltraTreeNode oNode = null;
                UltraTreeNode oNodeParent = null;

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodUA", CoreStatics.CoreApplication.MovPrescrizioneSelezionata.CodUA);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                op.Parametro.Add("CodAzione", EnumAzioni.INS.ToString());
                                op.Parametro.Add("Icona", "1");
                op.Parametro.Add("ModelliPrescrizioni", "0");

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataTable oDt = Database.GetDataTableStoredProc("MSP_SelProtocolliPrescrizioni", spcoll);

                this.UltraTreeProtocolli.Nodes.Clear();

                                oNode = new UltraTreeNode(CoreStatics.TV_ROOT, CoreStatics.TV_ROOT_PROTOCOLLIPRESCRIZIONI);
                oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_FOLDER_16));
                oNode.Tag = CoreStatics.TV_ROOT;

                this.UltraTreeProtocolli.Nodes.Add(oNode);

                oNodeParent = oNode;

                foreach (DataRow oDataRow in oDt.Rows)
                {

                    oNode = new UltraTreeNode(oDataRow["Codice"].ToString(), oDataRow["Descrizione"].ToString());
                    if (oDataRow.IsNull("Icona") == false)
                    {
                        oNode.LeftImages.Add(DrawingProcs.GetImageFromByte(oDataRow["Icona"], new Size(32, 32), true));
                    }
                    oNode.Tag = oDataRow["DataOraInizioObbligatoria"].ToString();
                    oNodeParent.Nodes.Add(oNode);

                }

                this.UltraTreeProtocolli.PerformAction(UltraTreeAction.FirstNode, false, false);
                this.UltraTreeProtocolli.PerformAction(UltraTreeAction.ExpandAllNode, false, false);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            this.Cursor = Cursors.Default;
        }

        #endregion

        #region UltraPopupControlContainer ucEasyListBox

        private void UltraPopupControlContainer_Closed(object sender, EventArgs e)
        {
            _ucEasyListBox.ItemSelectionChanged -= ucEasyListBox_ItemSelectionChanged;
        }

        private void UltraPopupControlContainer_Opened(object sender, EventArgs e)
        {
            _ucEasyListBox.ItemSelectionChanged += ucEasyListBox_ItemSelectionChanged;
        }

        private void UltraPopupControlContainer_Opening(object sender, CancelEventArgs e)
        {
            Infragistics.Win.Misc.UltraPopupControlContainer popup = sender as Infragistics.Win.Misc.UltraPopupControlContainer;
            popup.PopupControl = _ucEasyListBox;
        }

        private void ucEasyListBox_ItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {

            ucEasyDateTimeEditor source = this.UltraPopupControlContainer.SourceControl as ucEasyDateTimeEditor;
            ucEasyListBox popup = this.UltraPopupControlContainer.PopupControl as ucEasyListBox;

            DateTime dt = (DateTime)source.Value;
            if (popup.ActiveItem != null)
            {
                string[] orari = popup.ActiveItem.Text.Split(':');
                DateTime newdt = new DateTime(dt.Year, dt.Month, dt.Day, int.Parse(orari[0]), int.Parse(orari[1]), 0);
                source.Value = newdt;
            }


            this.UltraPopupControlContainer.Close();
        }

        #endregion

        #region Events Form

        private void frmSelezionaTipoPrescrizione_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            try
            {

                switch (UltraTabControl.SelectedTab.Key)
                {
                    case "tab0":
                        if (this.UltraTree.SelectedNodes.Count > 0 && (string)this.UltraTree.SelectedNodes[0].Tag == CoreStatics.TV_TIPOPRESCRIZIONE)
                        {

                            CoreStatics.g_split = CoreStatics.SetSplit(this.UltraTree.SelectedNodes[0].Key, @"\");

                            Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                            op.Parametro.Add("CodUA", CoreStatics.CoreApplication.MovPrescrizioneSelezionata.CodUA);
                            op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                            op.Parametro.Add("CodAzione", EnumAzioni.INS.ToString());
                            op.Parametro.Add("DatiEstesi", "1");
                            op.Parametro.Add("Codice", CoreStatics.g_split.GetValue(CoreStatics.g_split.Length - 1).ToString());

                            SqlParameterExt[] spcoll = new SqlParameterExt[1];
                            string xmlParam = XmlProcs.XmlSerializeToString(op);
                            spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                            DataTable oDt = Database.GetDataTableStoredProc("MSP_SelTipoPrescrizione", spcoll);

                            if (oDt.Rows.Count == 1 && oDt.Rows[0][0].ToString() != string.Empty)
                            {

                                CoreStatics.CoreApplication.MovPrescrizioneSelezionata.CodTipoPrescrizione = oDt.Rows[0]["Codice"].ToString();
                                CoreStatics.CoreApplication.MovPrescrizioneSelezionata.DescrTipoPrescrizione = oDt.Rows[0]["Descrizione"].ToString();
                                CoreStatics.CoreApplication.MovPrescrizioneSelezionata.CodScheda = oDt.Rows[0]["CodScheda"].ToString();
                                CoreStatics.CoreApplication.MovPrescrizioneSelezionata.CodViaSomministrazione = oDt.Rows[0]["CodViaSomministrazione"].ToString();
                                CoreStatics.CoreApplication.MovPrescrizioneSelezionata.DescrViaSomministrazione = oDt.Rows[0]["DescrViaSomministrazione"].ToString();
                                CoreStatics.CoreApplication.MovPrescrizioneSelezionata.PrescrizioneASchema = Boolean.Parse(oDt.Rows[0]["PrescrizioneASchema"].ToString());

                                this.DialogResult = DialogResult.OK;
                                this.Close();

                            }

                        }
                        break;

                    case "tab1":
                                                if ((this.udteDataOraInizio.Enabled == true) && (this.udteDataOraInizio.Value == null))
                        {
                                                        easyStatics.EasyMessageBox("E' necessario impostare la Data/Ora inizio", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            this.udteDataOraInizio.Focus();
                        }
                        else
                        {
                                                        if (this.UltraTreeProtocolli.SelectedNodes.Count > 0 && (string)this.UltraTreeProtocolli.SelectedNodes[0].Tag != CoreStatics.TV_ROOT)
                            {

                                

                                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                                op.Parametro.Add("CodUA", CoreStatics.CoreApplication.MovPrescrizioneSelezionata.CodUA);
                                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                                op.Parametro.Add("CodAzione", EnumAzioni.INS.ToString());
                                op.Parametro.Add("Codice", this.UltraTreeProtocolli.SelectedNodes[0].Key);
                                op.Parametro.Add("Icona", "0");
                                op.Parametro.Add("ModelliPrescrizioni", "1");

                                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                                string xmlParam = XmlProcs.XmlSerializeToString(op);
                                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                                DataTable oDt = Database.GetDataTableStoredProc("MSP_SelProtocolliPrescrizioni", spcoll);

                                if (oDt.Rows.Count == 1 && oDt.Rows[0][0].ToString() != string.Empty)
                                {

                                    this.ImpostaCursore(enum_app_cursors.WaitCursor);

                                                                        ProtocolloPrescrizioni prot = new ProtocolloPrescrizioni();
                                    prot.NuovaPrescrizioneCreata += UpdateProgressBar;

                                    prot.Codice = oDt.Rows[0]["Codice"].ToString();
                                    prot.Descrizione = oDt.Rows[0]["Descrizione"].ToString();
                                    prot.ModelliPrescrizioni = SerializerProtocolli.DeSerializeModelli(oDt.Rows[0]["ModelliPrescrizioni"].ToString());
                                    prot.DataOraInizioObbligatoria = bool.Parse(oDt.Rows[0]["DataOraInizioObbligatoria"].ToString());
                                    prot.VersioneModello = int.Parse(oDt.Rows[0]["VersioneModello"].ToString());

                                                                        this.ucEasyProgressBarProtocolli.Minimum = 0;
                                    this.ucEasyProgressBarProtocolli.Maximum = prot.ModelliPrescrizioni.Count;
                                    this.ucEasyProgressBarProtocolli.Value = 0;
                                    this.ucEasyProgressBarProtocolli.Visible = true;
                                    this.ucEasyTableLayoutPanelProtocolli.Refresh();

                                                                        if (udteDataOraInizio.Enabled)
                                        prot.CreaPrescrizioni(CoreStatics.CoreApplication.MovPrescrizioneSelezionata.CodUA, CoreStatics.CoreApplication.Paziente.ID,
                                                              CoreStatics.CoreApplication.Episodio.ID, CoreStatics.CoreApplication.Trasferimento.ID,
                                                              (DateTime)udteDataOraInizio.Value, CoreStatics.CoreApplication.Ambiente);
                                    else
                                        prot.CreaPrescrizioni(CoreStatics.CoreApplication.MovPrescrizioneSelezionata.CodUA, CoreStatics.CoreApplication.Paziente.ID,
                                                              CoreStatics.CoreApplication.Episodio.ID, CoreStatics.CoreApplication.Trasferimento.ID,
                                                              DateTime.MinValue, CoreStatics.CoreApplication.Ambiente);

                                    prot.NuovaPrescrizioneCreata -= UpdateProgressBar;

                                    prot = null;

                                    this.ucEasyProgressBarProtocolli.Visible = false;

                                    this.ImpostaCursore(enum_app_cursors.DefaultCursor);

                                                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;

                                    CoreStatics.CoreApplication.ListaIDMovPrescrizioniCreate = _generatedPrestsIDs;

                                    this.DialogResult = DialogResult.OK;
                                    this.Close();

                                }

                            }
                        }


                        break;
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "frmSelezionaTipoPrescrizione_PulsanteAvantiClick", this.Name);
            }
        }

        private void frmSelezionaTipoPrescrizione_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        #endregion

        #region Events

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

        private void ubRicerca_Click(object sender, EventArgs e)
        {
            this.LoadUltraTree();
        }

        private void UpdateProgressBar(object sender, NuovaPrescrizioneEventArgs e)
        {
            this.ucEasyProgressBarProtocolli.Value = e.CurrentPrestIndex;
            if (_generatedPrestsIDs == null) _generatedPrestsIDs = new List<string>();
            _generatedPrestsIDs.Add(e.CurrentPrestID);
            this.ucEasyProgressBarProtocolli.Refresh();
        }

        private void UltraTree_AfterActivate(object sender, NodeEventArgs e)
        {
            if (e.TreeNode.HasNodes && !e.TreeNode.Expanded) e.TreeNode.Expanded = true;
        }

        private void UltraTreeProtocolli_AfterActivate(object sender, NodeEventArgs e)
        {

            bool bDataOraInizioObbligatoria = false;

            bool.TryParse(e.TreeNode.Tag.ToString(), out bDataOraInizioObbligatoria);

            udteDataOraInizio.Enabled = bDataOraInizioObbligatoria;
        }

        private void udteDataOraInizio_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            if (e.Button.Key == "Orari" && ((ucEasyDateTimeEditor)sender).Value != null)
            {
                CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainer);
                _ucEasyListBox = GetEasyListBoxForPopupControlContainer(sender);
                this.UltraPopupControlContainer.Show((ucEasyDateTimeEditor)sender);
            }
        }

        #endregion
    }
}
