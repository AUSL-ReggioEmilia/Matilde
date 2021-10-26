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
    public partial class frmUATreeView : Form, Interfacce.IViewFormBase
    {
        public frmUATreeView()
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

            this.picView.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_UNITAATOMICHE, Enums.EnumImageSize.isz256));

            this.LoadUltraTree(null);

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
            bool bElimina = false;
            bool bAggiorna = true;

            if (this.UltraTree.ActiveNode != null)
            {

                switch (this.UltraTree.ActiveNode.Tag.ToString())
                {

                    case MyStatics.TV_ROOT:
                        bModifica = false;
                        bElimina = false;
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


            string sCodUnitaAtomica = "";
            if (this.UltraTree.ActiveNode != null)
            {
                if (this.UltraTree.ActiveNode.Tag.ToString() == MyStatics.TV_UNITAATOMICHE)
                {
                    sCodUnitaAtomica = this.UltraTree.ActiveNode.Key;
                }
            }

            switch (oTool.Key)
            {
                case MyStatics.GC_NUOVO:
                    if (MyStatics.ActionDataNameFormPU(Enums.EnumDataNames.T_UnitaAtomiche, Enums.EnumModalityPopUp.mpNuovo, this.ViewIcon, this.picView.Image, this.ViewText, "", sCodUnitaAtomica) == DialogResult.OK)
                    {
                        this.LoadUltraTree(null);
                    }
                    break;

                case MyStatics.GC_MODIFICA:
                    if (MyStatics.ActionDataNameFormPU(Enums.EnumDataNames.T_UnitaAtomiche, Enums.EnumModalityPopUp.mpModifica, this.ViewIcon, this.picView.Image, this.ViewText, sCodUnitaAtomica) == DialogResult.OK)
                    {
                        this.LoadUltraTree(null);
                    }
                    break;

                case MyStatics.GC_ELIMINA:
                    if (MyStatics.ActionDataNameFormPU(Enums.EnumDataNames.T_UnitaAtomiche, Enums.EnumModalityPopUp.mpCancella, this.ViewIcon, this.picView.Image, this.ViewText, sCodUnitaAtomica) == DialogResult.OK)
                    {
                        this.LoadUltraTree(null);
                    }
                    break;

                case MyStatics.GC_AGGIORNA:
                    this.LoadUltraTree(null);
                    break;

                case MyStatics.GC_UNITAATOMICHE:
                    frmZoomUA f = new frmZoomUA();
                    f.ViewIcon = this.ViewIcon;
                    f.ViewText = this.UltraTree.ActiveNode.Text;
                    f.ViewParametro = this.UltraTree.ActiveNode.Key;
                    f.ViewInit();
                    f.ShowDialog();
                    f = null;
                    break;

                default:
                    break;

            }

        }

        #endregion

        #region UltraTree

        private void LoadUltraTree(UltraTreeNode oNodePadre)
        {

            this.Cursor = Cursors.WaitCursor;

            string sSql = @"";
            DataSet oDs = null;

            UltraTreeNode oNode = null;
            UltraTreeNode oNodeParent = null;

            try
            {

                if (oNodePadre == null)
                {

                    this.UltraTree.Nodes.Clear();
                    oNode = new UltraTreeNode(MyStatics.TV_ROOT, MyStatics.TV_ROOT);
                    oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_FOLDER, Enums.EnumImageSize.isz16)));
                    oNode.Tag = MyStatics.TV_ROOT;
                    this.UltraTree.Nodes.Add(oNode);
                    this.UltraTree.PerformAction(UltraTreeAction.FirstNode, false, false);
                    this.UltraTree.PerformAction(UltraTreeAction.ExpandNode, false, false);

                }
                else
                {

                    oNodePadre.Nodes.Clear();
                    sSql = string.Format("Select Codice, Descrizione From T_UnitaAtomiche Where IsNull(CodPadre,'') = '{0}' Order By Codice", (oNodePadre.Tag.ToString() == MyStatics.TV_ROOT ? "" : oNodePadre.Key));
                    oDs = DataBase.GetDataSet(sSql);
                    foreach (DataRow oDataRow in oDs.Tables[0].Rows)
                    {
                        oNodeParent = new UltraTreeNode(oDataRow["Codice"].ToString(), oDataRow["Descrizione"].ToString());
                        oNodeParent.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_UNITAATOMICHE, Enums.EnumImageSize.isz16)));
                        oNodeParent.Tag = MyStatics.TV_UNITAATOMICHE;
                        oNodePadre.Nodes.Add(oNodeParent);
                        oNodeParent.Expanded = true;
                    }

                }

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

        private void UltraTree_BeforeExpand(object sender, CancelableNodeEventArgs e)
        {
            this.LoadUltraTree(e.TreeNode);
        }

        private void UltraTree_DragDrop(object sender, DragEventArgs e)
        {

            try
            {





            }
            catch (Exception)
            {

            }

        }

        private void UltraTree_DragOver(object sender, DragEventArgs e)
        {

            try
            {



            }
            catch (Exception)
            {

            }

        }

        private void UltraTree_MouseMove(object sender, MouseEventArgs e)
        {

            try
            {


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
