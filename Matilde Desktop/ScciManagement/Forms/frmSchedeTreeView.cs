using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.ScciResource;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinTree;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmSchedeTreeView : Form, Interfacce.IViewFormBase
    {
        public frmSchedeTreeView()
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

            MyStatics.SetUltraGroupBox(this.UltraGroupBox);
            MyStatics.SetUltraTree(this.UltraTree);
            this.UltraTree.AllowDrop = true;

            this.picView.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_SCHEDE, Enums.EnumImageSize.isz256));

            this.LoadUltraTreePath();
            this.LoadUltraTree();

            this.ResumeLayout();

        }

        #endregion

        #region UltraToolBar

        private void InitializeUltraToolbarsManager()
        {

            try
            {

                MyStatics.SetUltraToolbarsManager(this.UltraToolbarsManager);

                foreach (ToolBase oTool in this.UltraToolbarsManagerTreeView.Tools)
                {
                    oTool.SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(oTool.Key, Enums.EnumImageSize.isz32));
                    oTool.SharedProps.AppearancesSmall.Appearance.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(oTool.Key, Enums.EnumImageSize.isz16));
                }

            }
            catch (Exception)
            {

            }

        }

        private void SetUltraToolBarManager()
        {

            bool bNuovo = true;
            bool bModifica = true;
            bool bElimina = true;
            bool bAggiorna = true;

            if (this.UltraTree.ActiveNode != null)
            {

                switch (this.UltraTree.ActiveNode.Tag.ToString())
                {

                    case MyStatics.TV_PATH:
                        bModifica = false;
                        bElimina = false;
                        break;

                    case MyStatics.TV_ROOT:
                        bModifica = false;
                        bElimina = false;
                        bNuovo = false;
                        break;

                    default:
                        break;
                }

            }
            else
            {
                bNuovo = false;
                bModifica = false;
                bElimina = false;
            }

            this.UltraToolbarsManagerTreeView.Tools[MyStatics.GC_NUOVO].SharedProps.Enabled = bNuovo;
            this.UltraToolbarsManagerTreeView.Tools[MyStatics.GC_MODIFICA].SharedProps.Enabled = bModifica;
            this.UltraToolbarsManagerTreeView.Tools[MyStatics.GC_ELIMINA].SharedProps.Enabled = bElimina;

            this.UltraToolbarsManagerTreeView.Tools[MyStatics.GC_AGGIORNA].SharedProps.Enabled = bAggiorna;

        }

        private void ActionGridToolClick(ToolBase oTool)
        {

            string sCodSchede = "";
            string sPath = "";
            if (this.UltraTree.ActiveNode != null)
            {
                if (this.UltraTree.ActiveNode.Tag.ToString() == MyStatics.TV_PATH)
                {
                    sPath = this.UltraTree.ActiveNode.Key;
                }
                else if (this.UltraTree.ActiveNode.Tag.ToString() == MyStatics.TV_SCHEDA)
                {
                    string[] arrKey = this.UltraTree.ActiveNode.Key.Split('\\');
                    sCodSchede = arrKey[arrKey.Length - 1];
                    for (int i = arrKey.GetLowerBound(0); i < arrKey.GetUpperBound(0); i++)
                    {
                        if (sPath != "") sPath += @"\";
                        sPath += arrKey[i];
                    }
                }
            }

            switch (oTool.Key)
            {
                case MyStatics.GC_NUOVO:
                    if (MyStatics.ActionDataNameFormPU(Enums.EnumDataNames.T_Schede, Enums.EnumModalityPopUp.mpNuovo, this.ViewIcon, this.picView.Image, this.ViewText, sCodSchede, sPath) == DialogResult.OK)
                    {
                        this.LoadUltraTreePath();
                        this.LoadUltraTree();
                    }
                    break;

                case MyStatics.GC_MODIFICA:
                    if (MyStatics.ActionDataNameFormPU(Enums.EnumDataNames.T_Schede, Enums.EnumModalityPopUp.mpModifica, this.ViewIcon, this.picView.Image, this.ViewText, sCodSchede) == DialogResult.OK)
                    {
                        this.LoadUltraTreePath();
                        this.LoadUltraTree();
                    }
                    break;

                case MyStatics.GC_ELIMINA:
                    if (MyStatics.ActionDataNameFormPU(Enums.EnumDataNames.T_Schede, Enums.EnumModalityPopUp.mpCancella, this.ViewIcon, this.picView.Image, this.ViewText, sCodSchede) == DialogResult.OK)
                    {
                        this.LoadUltraTreePath();
                        this.LoadUltraTree();
                    }
                    break;

                case MyStatics.GC_AGGIORNA:
                    this.LoadUltraTreePath();
                    this.LoadUltraTree();

                    break;

                default:
                    break;

            }

        }

        #endregion

        #region UltraTree

        private void LoadUltraTreePath()
        {

            this.Cursor = Cursors.WaitCursor;

            try
            {

                UltraTreeNode oNode = null;
                UltraTreeNode oNodeParent = null;
                string sKey = @"";
                this.UltraTree.Nodes.Clear();
                oNode = new UltraTreeNode(MyStatics.TV_ROOT, MyStatics.TV_ROOT);
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_FOLDER, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_ROOT;
                this.UltraTree.Nodes.Add(oNode);
                string sSql = @"Select IsNull(Path,'') As Path From T_Schede Where IsNull(Path, '') <> '' Group By Path Order By Path";
                DataSet oDs = DataBase.GetDataSet(sSql);
                foreach (DataRow oDataRow in oDs.Tables[0].Rows)
                {

                    oNode = this.UltraTree.GetNodeByKey(MyStatics.TV_ROOT);
                    sKey = @"";
                    MyStatics.g_split = MyStatics.SetSplit(oDataRow["Path"].ToString(), @"\");
                    for (int i = 0; i < MyStatics.g_split.Length; i++)
                    {

                        sKey += (sKey != "" ? @"\" : "") + MyStatics.g_split.GetValue(i).ToString();
                        oNodeParent = this.UltraTree.GetNodeByKey(sKey);
                        if (oNodeParent == null)
                        {

                            oNodeParent = new UltraTreeNode(sKey, MyStatics.g_split.GetValue(i).ToString());
                            oNodeParent.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_FOLDER, Enums.EnumImageSize.isz16)));
                            oNodeParent.Tag = MyStatics.TV_PATH;
                            oNode.Nodes.Add(oNodeParent);

                        }

                        oNode = oNodeParent;

                    }

                }

                this.UltraTree.PerformAction(UltraTreeAction.FirstNode, false, false);
                this.UltraTree.PerformAction(UltraTreeAction.ExpandNode, false, false);

                this.Cursor = Cursors.Default;

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "LoadUltraTreePath", this.Text);
            }

            this.Cursor = Cursors.Default;

        }

        private void LoadUltraTree()
        {

            this.Cursor = Cursors.WaitCursor;

            try
            {

                UltraTreeNode oNode = null;
                UltraTreeNode oNodeParent = null;
                string sSql = @"Select * From T_Schede";
                DataSet oDs = DataBase.GetDataSet(sSql);
                foreach (DataRow oDataRow in oDs.Tables[0].Rows)
                {

                    if (oDataRow["Path"].ToString().Trim() == "")
                    {
                        oNodeParent = this.UltraTree.GetNodeByKey(MyStatics.TV_ROOT);
                        if (oNodeParent != null)
                        {

                            oNode = new UltraTreeNode(oDataRow["Path"].ToString() + @"\" + oDataRow["Codice"].ToString(), oDataRow["Descrizione"].ToString());
                            oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_SCHEDE, Enums.EnumImageSize.isz16)));
                            oNode.Tag = MyStatics.TV_SCHEDA;
                            oNodeParent.Nodes.Add(oNode);

                        }
                    }
                    else
                    {
                        oNodeParent = this.UltraTree.GetNodeByKey(oDataRow["Path"].ToString());
                        if (oNodeParent != null)
                        {

                            oNode = new UltraTreeNode(oDataRow["Path"].ToString() + @"\" + oDataRow["Codice"].ToString(), oDataRow["Descrizione"].ToString());
                            oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_SCHEDE, Enums.EnumImageSize.isz16)));
                            oNode.Tag = MyStatics.TV_SCHEDA;
                            oNodeParent.Nodes.Add(oNode);

                        }
                    }

                }

                this.UltraTree.PerformAction(UltraTreeAction.FirstNode, false, false);
                this.UltraTree.PerformAction(UltraTreeAction.ExpandNode, false, false);

                this.Cursor = Cursors.Default;

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "LoadUltraTree", this.Text);
            }

            this.Cursor = Cursors.Default;

        }

        #endregion

        #region Events

        private void UltraTree_AfterActivate(object sender, NodeEventArgs e)
        {
            this.SetUltraToolBarManager();
        }

        private void UltraTree_DragDrop(object sender, DragEventArgs e)
        {

            try
            {

                UltraTreeNode oUltraTreeNode = (UltraTreeNode)e.Data.GetData("Infragistics.Win.UltraWinTree.UltraTreeNode", true);
                UltraTree UltraTree = sender as UltraTree;
                Point ptClient = UltraTree.PointToClient(new Point(e.X, e.Y));
                UltraTreeNode oItem = UltraTree.GetNodeFromPoint(ptClient);

                if (oUltraTreeNode != null && oItem != null)
                {

                    MyStatics.g_split = MyStatics.SetSplit(oUltraTreeNode.Key, @"\");

                    UltraTreeNode oNode = new UltraTreeNode(oItem.Key + @"\" + MyStatics.g_split.GetValue(MyStatics.g_split.Length - 1), oUltraTreeNode.Text);
                    oNode.LeftImages.Add(oUltraTreeNode.LeftImages[0]);
                    oNode.Tag = oUltraTreeNode.Tag;
                    oItem.Nodes.Add(oNode);

                    string sSql = string.Format("Update T_Schede Set Path = '{0}' Where Codice = '{1}'", oItem.Key, MyStatics.g_split.GetValue(MyStatics.g_split.Length - 1));
                    DataBase.ExecuteSql(sSql);

                    oUltraTreeNode.Remove();

                }

            }
            catch (Exception)
            {

            }

        }

        private void UltraTree_DragOver(object sender, DragEventArgs e)
        {

            try
            {

                UltraTreeNode oUltraTreeNode = (UltraTreeNode)e.Data.GetData("Infragistics.Win.UltraWinTree.UltraTreeNode", true);
                UltraTree UltraTree = sender as UltraTree;
                Point ptClient = UltraTree.PointToClient(new Point(e.X, e.Y));
                UltraTreeNode oItem = UltraTree.GetNodeFromPoint(ptClient);

                if (oUltraTreeNode != null && oItem != null)
                {
                    if (oItem.Tag.ToString() == MyStatics.TV_PATH)
                    {
                        e.Effect = DragDropEffects.Move;
                    }
                    else
                    {
                        e.Effect = DragDropEffects.None;
                    }
                }

            }
            catch (Exception)
            {

            }

        }

        private void UltraTree_MouseMove(object sender, MouseEventArgs e)
        {

            try
            {

                UltraTree UltraTree = sender as UltraTree;
                if (e.Button == MouseButtons.Left && UltraTree.SelectedNodes.Count > 0)
                {
                    if (UltraTree.SelectedNodes[0].Tag.ToString() == MyStatics.TV_SCHEDA)
                    {
                        UltraTree.DoDragDrop(UltraTree.SelectedNodes[0], DragDropEffects.Move);
                    }
                }

            }
            catch (Exception)
            {

            }

        }

        private void UltraToolbarsManagerTreeView_ToolClick(object sender, ToolClickEventArgs e)
        {

            try
            {
                ActionGridToolClick(e.Tool);
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void ubChiudi_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

    }
}
