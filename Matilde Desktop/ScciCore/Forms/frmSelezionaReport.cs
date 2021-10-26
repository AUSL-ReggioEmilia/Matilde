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
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.Enums;

namespace UnicodeSrl.ScciCore
{
    public partial class frmSelezionaReport : frmBaseModale, Interfacce.IViewFormlModal
    {
        public frmSelezionaReport()
        {
                                                
            InitializeComponent();
        }

        #region Interface

        public void Carica()
        {

            try
            {

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_REPORT_256);
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_REPORT_16);

                this.ubSS.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_REPORTHISTORY_128);
                this.ubSS.PercImageFill = 0.85F;

                this.InitializeUltraTree();

                                                if (CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.MascheraPartenza.Reports.Elementi.Count == 1)
                {
                                        if (CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.MascheraPartenza.Reports.Elementi[0].CodFormatoReport == Report.COD_FORMATO_REPORT_WORD ||
                        CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.MascheraPartenza.Reports.Elementi[0].CodFormatoReport == Report.COD_FORMATO_REPORT_PDF)
                        CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.MascheraPartenza.Reports.Elementi[0].CaricaModello();

                    CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.MascheraPartenza.Reports.ReportSelezionato = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.MascheraPartenza.Reports.Elementi[0];

                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();

                    CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.Report);

                }
                else
                {
                                        this.LoadUltraTree();

                    this.ShowDialog();
                    if (this.DialogResult == System.Windows.Forms.DialogResult.OK)
                    {

                        if (!this.uceMultiSelezione.Checked)
                        {
                            
                                                        string[] reportkeysplit = this.UltraTree.SelectedNodes[0].Key.Split(@"\".ToCharArray());
                            string sreportkey = reportkeysplit[reportkeysplit.Length - 1];

                            var item = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.MascheraPartenza.Reports.Elementi.Single(Report => Report.Codice == sreportkey);
                            if (item != null)
                            {
                                if (item.CodFormatoReport == Report.COD_FORMATO_REPORT_WORD || item.CodFormatoReport == Report.COD_FORMATO_REPORT_PDF)
                                {
                                    item.CaricaModello();
                                }
                                CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.MascheraPartenza.Reports.ReportSelezionato = item;

                                CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.Report);
                            }
                        }

                    }
                }

            }
            catch (Exception)
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }


        }

        #endregion

        #region Subroutines
        
                                        private void SetNavigazione(bool enable)
        {
            try
            {
                this.ucBottomModale.Enabled = enable;
                this.UltraTree.Enabled = enable;
                this.uceMultiSelezione.Enabled = enable;
                this.uteRicerca.ReadOnly = !enable;
                this.ubRicerca.Enabled = enable;
                this.ubSS.Enabled = enable;
            }
            catch
            {
            }
        }

        #endregion

        #region UltraTree

        private void InitializeUltraTree()
        {
        }

        private void LoadUltraTree()
        {

            this.Cursor = Cursors.WaitCursor;

            try
            {

                bool bAddNode = true;
                UltraTreeNode oNode = null;
                UltraTreeNode oNodeParent = null;

                string sKey = @"";
                                this.UltraTree.Nodes.Clear();

                                oNode = new UltraTreeNode(CoreStatics.TV_ROOT, CoreStatics.TV_ROOT_REPORT);
                oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_FOLDER_16));
                oNode.Tag = CoreStatics.TV_ROOT;
                this.UltraTree.Nodes.Add(oNode);

                                if (CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.MascheraPartenza.Reports.Elementi.Count > 0)
                {

                    foreach (Report oReport in CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.MascheraPartenza.Reports.Elementi)
                    {

                        bAddNode = true;

                                                                                                if (bAddNode)
                        {
                                                        if (this.uteRicerca.Text != "" && !oReport.Descrizione.ToUpper().Contains(this.uteRicerca.Text.ToUpper()))
                                bAddNode = false;
                        }
                        if (bAddNode)
                        {
                                                        if (this.uceMultiSelezione.Checked && oReport.CodFormatoReport == Report.COD_FORMATO_REPORT_REM)
                                bAddNode = false;
                        }
                        
                        if (bAddNode)
                        {

                            oNode = this.UltraTree.GetNodeByKey(CoreStatics.TV_ROOT);
                            sKey = @"";
                            Array s_split = oReport.Path.Split(@"\".ToCharArray());
                            for (int i = 0; i < s_split.Length; i++)
                            {

                                sKey += (sKey != "" ? @"\" : "") + s_split.GetValue(i).ToString();
                                if (sKey == "")
                                    oNodeParent = this.UltraTree.GetNodeByKey(CoreStatics.TV_ROOT);
                                else
                                    oNodeParent = this.UltraTree.GetNodeByKey(sKey);

                                if (oNodeParent == null)
                                {

                                    oNodeParent = new UltraTreeNode(sKey, s_split.GetValue(i).ToString());
                                    oNodeParent.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_FOLDER_16));
                                    oNodeParent.Tag = CoreStatics.TV_PATH;
                                    oNodeParent.Expanded = (this.uteRicerca.Text != string.Empty);
                                    oNode.Nodes.Add(oNodeParent);

                                }

                                oNode = oNodeParent;

                            }

                        }

                    }
                }

                                foreach (Report oReport in CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.MascheraPartenza.Reports.Elementi)
                {

                    bAddNode = true;

                                                                                if (bAddNode)
                    {
                                                if (this.uteRicerca.Text != "" && !oReport.Descrizione.ToUpper().Contains(this.uteRicerca.Text.ToUpper()))
                            bAddNode = false;
                    }
                    if (bAddNode)
                    {
                                                if (this.uceMultiSelezione.Checked && oReport.CodFormatoReport == Report.COD_FORMATO_REPORT_REM)
                            bAddNode = false;
                    }

                    if (bAddNode)
                    {

                        if (oReport.Path.Trim() == "")
                        {
                            oNodeParent = this.UltraTree.GetNodeByKey(CoreStatics.TV_ROOT);
                            if (oNodeParent != null)
                            {

                                oNode = new UltraTreeNode(oReport.Path + @"\" + oReport.Codice, oReport.Descrizione);
                                oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_REPORT_16));
                                oNode.Tag = CoreStatics.TV_REPORT;
                                oNode.Expanded = (this.uteRicerca.Text != string.Empty);
                                
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
                            oNodeParent = this.UltraTree.GetNodeByKey(oReport.Path);
                            if (oNodeParent != null)
                            {

                                oNode = new UltraTreeNode(oReport.Path + @"\" + oReport.Codice, oReport.Descrizione);
                                oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_REPORT_16));
                                oNode.Tag = CoreStatics.TV_REPORT;
                                oNode.Expanded = (this.uteRicerca.Text != string.Empty);

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

                }

                this.UltraTree.PerformAction(UltraTreeAction.FirstNode, false, false);
                this.UltraTree.PerformAction(UltraTreeAction.ExpandNode, false, false);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            this.Cursor = Cursors.Default;

        }

                                                private void recoursiveCaricaListReportSelezionati(ref UltraTreeNode roParentNode, ref List<Report> rlstReports)
        {
            if (rlstReports == null) rlstReports = new List<Report>();
            
            if (roParentNode != null)
            {
                for (int n = 0; n < roParentNode.Nodes.Count; n++)
                {
                    UltraTreeNode node = roParentNode.Nodes[n];

                    if ((string)node.Tag != CoreStatics.TV_REPORT)
                    {
                                                recoursiveCaricaListReportSelezionati(ref node, ref rlstReports);
                    }
                    else
                    {
                        if (node.NodeStyleResolved == NodeStyle.CheckBox
                            && node.CheckedState == CheckState.Checked)
                        {

                            string[] reportkeysplit = node.Key.Split(@"\".ToCharArray());
                            string sreportkey = reportkeysplit[reportkeysplit.Length - 1];

                            var item = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.MascheraPartenza.Reports.Elementi.Single(Report => Report.Codice == sreportkey);
                            if (item != null)
                            {
                                if (item.CodFormatoReport == Report.COD_FORMATO_REPORT_WORD || item.CodFormatoReport == Report.COD_FORMATO_REPORT_PDF)
                                {
                                    item.CaricaModello();
                                }

                                rlstReports.Add(item);
                            }
                        }
                    }
                }
                
            }

        }

        #endregion

        #region Events

        private void frmSelezionaReports_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            try
            {
                if (this.uceMultiSelezione.Checked)
                {
                    this.Cursor = CoreStatics.setCursor(enum_app_cursors.WaitCursor);
                    SetNavigazione(false);

                                        UltraTreeNode nodeRoot = this.UltraTree.GetNodeByKey(CoreStatics.TV_ROOT);
                    List<Report> lstReports = new List<Report>();
                    recoursiveCaricaListReportSelezionati(ref nodeRoot, ref lstReports);

                    if (lstReports.Count <= 0)
                    {
                        this.Cursor = CoreStatics.setCursor(enum_app_cursors.DefaultCursor);
                                                easyStatics.EasyMessageBox("Selezionare almeno un report!", "Report", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    else
                    {

                                                if (CoreStatics.stampaMultiplaDiretta(ref lstReports))
                        {
                                                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                            this.Close();
                        }

                    }
                }
                else
                {

                    
                    if (this.UltraTree.SelectedNodes.Count > 0 && (string)this.UltraTree.SelectedNodes[0].Tag == CoreStatics.TV_REPORT)
                    {
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                        this.Close();

                                                                    }
                    else
                    {
                                                easyStatics.EasyMessageBox("Selezionare un report!", "Report", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }


            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            finally
            {
                this.Cursor = CoreStatics.setCursor(enum_app_cursors.DefaultCursor);
                SetNavigazione(true);
                
            }
        }

        private void frmSelezionaReports_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void ubRicerca_Click(object sender, EventArgs e)
        {
                                                                                                                                    this.LoadUltraTree();
        }

                                
                                
        
        private void UltraTree_AfterActivate(object sender, NodeEventArgs e)
        {
            if (e.TreeNode.HasNodes && !e.TreeNode.Expanded) e.TreeNode.Expanded = true;
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

        private void ubSS_Click(object sender, EventArgs e)
        {

            try
            {

                CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.StoricoReport, false);

            }
            catch (Exception)
            {

            }

        }

        private void frmSelezionaReport_Shown(object sender, EventArgs e)
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
            if (this.uceMultiSelezione.Checked)
                this.PulsanteAvantiTesto = "STAMPA";
            else
                this.PulsanteAvantiTesto = "AVANTI";

            LoadUltraTree();
        }

        private void frmSelezionaReport_FormClosed(object sender, FormClosedEventArgs e)
        {
                        easyStatics.setTreeViewCheckBoxesStyle();
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
                    if (child != null && (string)child.Tag == CoreStatics.TV_REPORT)
                    {
                                                int minleftchk = 30 + (24 * child.Level);

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

        #endregion

    }
}
