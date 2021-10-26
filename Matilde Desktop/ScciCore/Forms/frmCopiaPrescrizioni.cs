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

using Infragistics.Win.UltraWinTree;

namespace UnicodeSrl.ScciCore
{
    public partial class frmCopiaPrescrizioni : frmBaseModale, Interfacce.IViewFormlModal
    {

        public frmCopiaPrescrizioni()
        {
            InitializeComponent();
        }

        #region Declare

        private Dictionary<int, byte[]> oIcone = new Dictionary<int, byte[]>();

        #endregion

        #region Interface

        public void Carica()
        {

            try
            {

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;

                this.ucEasyTreeView.Override.Multiline = Infragistics.Win.DefaultableBoolean.True;
                this.ucEasyTreeView.PerformLayout();
                this.ucEasyTreeViewTipo.Override.Multiline = Infragistics.Win.DefaultableBoolean.True;
                this.ucEasyTreeViewTipo.PerformLayout();
                this.ucEasyTreeViewVie.Override.Multiline = Infragistics.Win.DefaultableBoolean.True;
                this.ucEasyTreeViewVie.PerformLayout();

                this.ucAnteprimaRtf.rtbRichTextBox.ReadOnly = true;

                this.CaricaTreview(true);

                this.ShowDialog();

            }
            catch (Exception)
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }

        }

        #endregion

        #region SubRoutine

        private void CaricaTreview(bool datiEstesi)
        {

            UltraTreeNode oNodeRoot = null;
            UltraTreeNode oNodeEpi = null;
            UltraTreeNode oNode = null;

            try
            {

                                                                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Paziente.ID);
                op.Parametro.Add("IDEpisodio", CoreStatics.CoreApplication.Episodio.ID);
                op.Parametro.Add("IDTrasferimento", CoreStatics.CoreApplication.Trasferimento.ID);
                if (datiEstesi)
                {
                    op.Parametro.Add("DatiEstesi", "1");
                }
                else
                {
                    op.Parametro.Add("DatiEstesi", "0");

                                        Dictionary<string, string> listatipi = new Dictionary<string, string>();
                    foreach (UltraTreeNode oNodeTipi in this.ucEasyTreeViewTipo.Nodes[CoreStatics.GC_TUTTI].Nodes)
                    {
                        if (oNodeTipi.Override.NodeStyle == NodeStyle.CheckBox && oNodeTipi.CheckedState == CheckState.Checked)
                        {
                            listatipi.Add(oNodeTipi.Key, oNodeTipi.Text);
                        }

                    }
                    string[] codtipi = listatipi.Keys.ToArray();
                    op.ParametroRipetibile.Add("TipoPrescrizione", codtipi);

                                        Dictionary<string, string> listavie = new Dictionary<string, string>();
                    foreach (UltraTreeNode oNodeVie in this.ucEasyTreeViewVie.Nodes[CoreStatics.GC_TUTTI].Nodes)
                    {
                        if (oNodeVie.Override.NodeStyle == NodeStyle.CheckBox && oNodeVie.CheckedState == CheckState.Checked)
                        {
                            listavie.Add(oNodeVie.Key, oNodeVie.Text);
                        }

                    }
                    string[] codvie = listavie.Keys.ToArray();
                    op.ParametroRipetibile.Add("ViaSomministrazione", codvie);
                }

                                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_SelAlberoPrescrizioni", spcoll);

                                this.ucEasyTreeView.EventManager.SetEnabled(TreeEventIds.AfterCheck, false);
                this.ucEasyTreeView.Nodes.Clear();

                oNodeRoot = new UltraTreeNode(CoreStatics.CoreApplication.Paziente.ID, string.Format("{0} {1}", CoreStatics.CoreApplication.Paziente.Cognome, CoreStatics.CoreApplication.Paziente.Nome));
                oNodeRoot.LeftImages.Add(CoreStatics.CoreApplication.Paziente.Sesso.ToUpper() == "M" ? Risorse.GetImageFromResource(Risorse.GC_PAZIENTEMASCHIO_32) : Risorse.GetImageFromResource(Risorse.GC_PAZIENTEFEMMINA_32));

                foreach (DataRow oDr in ds.Tables[0].Rows)
                {

                    if (oNodeRoot.Nodes.Exists(oDr["IDEpisodio"].ToString()) == false)
                    {
                        oNodeEpi = new UltraTreeNode(oDr["IDEpisodio"].ToString(), oDr["Ricovero"].ToString());
                        oNodeEpi.LeftImages.Add((oDr["Dimesso"].ToString() == "0" ? Risorse.GetImageFromResource(Risorse.GC_LETTO_32) : Risorse.GetImageFromResource(Risorse.GC_LETTOD_32)));
                        oNodeRoot.Nodes.Add(oNodeEpi);
                    }
                    else
                    {
                        oNodeEpi = oNodeRoot.Nodes[oDr["IDEpisodio"].ToString()];
                    }
                    if (oDr["IDPrescrizione"].ToString() != string.Empty)
                    {
                        oNode = new UltraTreeNode(oDr["IDPrescrizione"].ToString(), oDr["DescrPrescrizione"].ToString());
                        if (oIcone.ContainsKey(Convert.ToInt32(oDr["IDIcona"].ToString())) == false)
                        {
                            oIcone.Add(Convert.ToInt32(oDr["IDIcona"].ToString()), CoreStatics.GetImageForGrid(Convert.ToInt32(oDr["IDIcona"].ToString()), 32));
                        }
                        oNode.LeftImages.Add(CoreStatics.ByteToImage(oIcone[Convert.ToInt32(oDr["IDIcona"].ToString())]));
                        oNode.Override.NodeStyle = (oDr["Selezionabile"].ToString() == "0" ? NodeStyle.Default : NodeStyle.CheckBox);
                        oNodeEpi.Nodes.Add(oNode);
                        if (oNode.Override.NodeStyle == NodeStyle.CheckBox)
                        {
                            oNode.Tag = oDr["AnteprimaRTF"].ToString();
                            oNodeEpi.Override.NodeStyle = NodeStyle.CheckBox;
                        }
                        else
                        {
                            oNode.Enabled = false;
                        }
                    }

                }
                this.ucEasyTreeView.Nodes.Add(oNodeRoot);
                this.ucEasyTreeView.ExpandAll();
                this.ucEasyTreeView.EventManager.SetEnabled(TreeEventIds.AfterCheck, true);

                if (datiEstesi)
                {

                                                                                this.ucEasyTreeViewTipo.EventManager.SetEnabled(TreeEventIds.AfterCheck, false);
                    this.ucEasyTreeViewTipo.Nodes.Clear();

                    oNodeRoot = new UltraTreeNode(CoreStatics.GC_TUTTI, "Tipi Terapia Farmacologica");
                    oNodeRoot.Override.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBox;
                    oNodeRoot.CheckedState = CheckState.Checked;

                    foreach (DataRow oDr in ds.Tables[1].Rows)
                    {
                        oNode = new UltraTreeNode(oDr["Codice"].ToString(), oDr["Descrizione"].ToString());
                        oNode.Override.NodeStyle = NodeStyle.CheckBox;
                        oNode.CheckedState = CheckState.Checked;
                        if (oDr["Icona"] != System.DBNull.Value) { oNode.LeftImages.Add(CoreStatics.ByteToImage((byte[])oDr["Icona"])); }
                        oNodeRoot.Nodes.Add(oNode);
                    }

                    this.ucEasyTreeViewTipo.Nodes.Add(oNodeRoot);
                    this.ucEasyTreeViewTipo.ExpandAll();
                    this.ucEasyTreeViewTipo.EventManager.SetEnabled(TreeEventIds.AfterCheck, true);

                                                                                this.ucEasyTreeViewVie.EventManager.SetEnabled(TreeEventIds.AfterCheck, false);
                    this.ucEasyTreeViewVie.Nodes.Clear();

                    oNodeRoot = new UltraTreeNode(CoreStatics.GC_TUTTI, "Vie di Somministrazione");
                    oNodeRoot.Override.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBox;
                    oNodeRoot.CheckedState = CheckState.Checked;

                    foreach (DataRow oDr in ds.Tables[2].Rows)
                    {
                        oNode = new UltraTreeNode(oDr["Codice"].ToString(), oDr["Descrizione"].ToString());
                        oNode.Override.NodeStyle = NodeStyle.CheckBox;
                        oNode.CheckedState = CheckState.Checked;
                        if (oDr["Icona"] != System.DBNull.Value) { oNode.LeftImages.Add(CoreStatics.ByteToImage((byte[])oDr["Icona"])); }
                        oNodeRoot.Nodes.Add(oNode);
                    }

                    this.ucEasyTreeViewVie.Nodes.Add(oNodeRoot);
                    this.ucEasyTreeViewVie.ExpandAll();
                    this.ucEasyTreeViewVie.EventManager.SetEnabled(TreeEventIds.AfterCheck, true);

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaTreview", this.Name);
            }

        }

        private bool Check()
        {

            bool bcheck = false;

            UltraTreeNode oNodeRoot = null;

            this.ultraStatusBar.Panels["ProgressBar"].ProgressBarInfo.Minimum = 0;
            this.ultraStatusBar.Panels["ProgressBar"].ProgressBarInfo.Value = 0;
            this.ultraStatusBar.Panels["ProgressBar"].ProgressBarInfo.Maximum = 1;

            try
            {

                if (this.ucEasyTreeView.Nodes.Exists(CoreStatics.CoreApplication.Paziente.ID) == true)
                {
                    oNodeRoot = this.ucEasyTreeView.Nodes[CoreStatics.CoreApplication.Paziente.ID];
                    foreach (UltraTreeNode oNodeEpi in oNodeRoot.Nodes)
                    {
                        if (oNodeEpi.Override.NodeStyle == NodeStyle.CheckBox)
                        {
                            foreach (UltraTreeNode oNode in oNodeEpi.Nodes)
                            {
                                if (oNode.Override.NodeStyle == NodeStyle.CheckBox && oNode.CheckedState == CheckState.Checked)
                                {
                                    bcheck = true;
                                    this.ultraStatusBar.Panels["ProgressBar"].ProgressBarInfo.Maximum += 1;
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            return bcheck;

        }

        private void Copia()
        {

            UltraTreeNode oNodeRoot = null;

            try
            {

                if (this.ucEasyTreeView.Nodes.Exists(CoreStatics.CoreApplication.Paziente.ID) == true)
                {
                    oNodeRoot = this.ucEasyTreeView.Nodes[CoreStatics.CoreApplication.Paziente.ID];
                    foreach (UltraTreeNode oNodeEpi in oNodeRoot.Nodes)
                    {
                        if (oNodeEpi.Override.NodeStyle == NodeStyle.CheckBox)
                        {
                            foreach (UltraTreeNode oNode in oNodeEpi.Nodes)
                            {
                                if (oNode.Override.NodeStyle == NodeStyle.CheckBox && oNode.CheckedState == CheckState.Checked)
                                {

                                    this.ultraStatusBar.Panels["ProgressBar"].ProgressBarInfo.Value += 1;

                                                                        MovPrescrizione oMovPrescrizioneNuova = new MovPrescrizione(CoreStatics.CoreApplication.Trasferimento.CodUA,
                                                                                                CoreStatics.CoreApplication.Paziente.ID,
                                                                                                CoreStatics.CoreApplication.Episodio.ID,
                                                                                                CoreStatics.CoreApplication.Trasferimento.ID,
                                                                                                CoreStatics.CoreApplication.Ambiente);
                                                                        MovPrescrizione oMovPrescrizione = new MovPrescrizione(oNode.Key, EnumAzioni.VIS, CoreStatics.CoreApplication.Ambiente);
                                                                        oMovPrescrizione.IDPaziente = CoreStatics.CoreApplication.Paziente.ID;
                                    oMovPrescrizione.IDEpisodio = CoreStatics.CoreApplication.Episodio.ID;
                                    oMovPrescrizione.IDTrasferimento = CoreStatics.CoreApplication.Trasferimento.ID;
                                    oMovPrescrizione.CodUA = CoreStatics.CoreApplication.Trasferimento.CodUA;
                                    oMovPrescrizione.DataEvento = DateTime.Now;
                                    oMovPrescrizione.CodStatoPrescrizione = EnumStatoPrescrizione.IC.ToString();
                                                                        oMovPrescrizioneNuova.CopiaDaOrigine(ref oMovPrescrizione);
                                                                        oMovPrescrizioneNuova.Salva();
                                                                        oMovPrescrizioneNuova = null;
                                    oMovPrescrizione = null;

                                }
                            }
                        }
                    }
                    this.ultraStatusBar.Panels["ProgressBar"].ProgressBarInfo.Value += 1;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        #endregion

        #region Events Form

        private void frmCopiaPrescrizioni_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {

            try
            {

                if (this.Check() == true)
                {
                    if (easyStatics.EasyMessageBox("Sei sicuro di voler copiare le Prescrizioni selezionate?", "Copia Prescrizioni", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        this.ImpostaCursore(enum_app_cursors.WaitCursor);
                        this.ucEasyTreeView.Enabled = false;
                        this.ucEasyTreeViewTipo.Enabled = false;
                        this.ucEasyTreeViewVie.Enabled = false;
                        this.ultraStatusBar.Visible = true;
                        this.Copia();
                        this.ImpostaCursore(enum_app_cursors.DefaultCursor);
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                        this.Close();
                    }
                }
                else
                {
                    easyStatics.EasyMessageBox("Selezionare almeno una Prescrizione!", "Copia Prescrizioni", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

            }
            catch (Exception ex)
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void frmCopiaPrescrizioni_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        #endregion

        #region Events

        private void ucEasyTreeView_AfterActivate(object sender, NodeEventArgs e)
        {

            try
            {

                if (e.TreeNode.Tag != null)
                {
                    this.ucAnteprimaRtf.rtbRichTextBox.Rtf = e.TreeNode.Tag.ToString();
                }
                else
                {
                    this.ucAnteprimaRtf.rtbRichTextBox.Rtf = string.Empty ;
                }

            }
            catch (Exception)
            {
                this.ucAnteprimaRtf.rtbRichTextBox.Rtf = string.Empty;
            }

        }

        private void ucEasyTreeView_AfterCheck(object sender, Infragistics.Win.UltraWinTree.NodeEventArgs e)
        {

            ((UltraTree)sender).EventManager.SetEnabled(TreeEventIds.AfterCheck, false);
            foreach (UltraTreeNode oNode in e.TreeNode.Nodes)
            {
                if (oNode.Override.NodeStyle == NodeStyle.CheckBox)
                {
                    oNode.CheckedState = e.TreeNode.CheckedState;
                }
            }
            ((UltraTree)sender).EventManager.SetEnabled(TreeEventIds.AfterCheck, true);

        }

        private void ucEasyTreeViewFiltro_AfterCheck(object sender, Infragistics.Win.UltraWinTree.NodeEventArgs e)
        {

            ((UltraTree)sender).EventManager.SetEnabled(TreeEventIds.AfterCheck, false);
            foreach (UltraTreeNode oNode in e.TreeNode.Nodes)
            {
                if (oNode.Override.NodeStyle == NodeStyle.CheckBox)
                {
                    oNode.CheckedState = e.TreeNode.CheckedState;
                }
            }
            ((UltraTree)sender).EventManager.SetEnabled(TreeEventIds.AfterCheck, true);

            this.CaricaTreview(false);

        }

        #endregion
        
    }
}
