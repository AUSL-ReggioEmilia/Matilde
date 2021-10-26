using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using Infragistics.Win.UltraWinTree;
using UnicodeSrl.ScciResource;

using Microsoft.Data.SqlClient;
using System.Globalization;
using UnicodeSrl.Framework.Data;
using Infragistics.Win.UltraWinGrid;

using UnicodeSrl.ScciCore.CustomControls.Infra;
using UnicodeSrl.Scci;

namespace UnicodeSrl.ScciCore
{
    public partial class frmTestiTipo : frmBaseModale, Interfacce.IViewFormlModal
    {
        public frmTestiTipo()
        {
            InitializeComponent();
        }

        #region INTERFACCIA

        public void Carica()
        {
            try
            {

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_TESTOPREDEFINITO_16);

                this.InizializzaControlli();
                this.CaricaTreeView();
                this.SetAnteprima();

                this.ShowDialog();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Carica", this.Name);
            }
        }

        #endregion

        #region PRIVATE

        private void InizializzaControlli()
        {

            try
            {
                this.rtfAnteprima.ViewInit();
                this.rtfTesto.ViewInit();
                                this.rtfTesto.ViewReadOnly = false;
                this.rtfTesto.ViewShowInsertImage = true;
                this.rtfTesto.ViewShowToolbar = true;
                this.rtfTesto.ViewToolbarStyle = Infragistics.Win.UltraWinToolbars.ToolbarStyle.Office2007;
                this.rtfTesto.ViewUseLargeImages = true;
                this.rtfTesto.ViewFont = DrawingProcs.getFontFromString(Database.GetConfigTable(EnumConfigTable.FontPredefinitoRTF));
                                this.rtfAnteprima.ViewReadOnly = true;
                this.rtfAnteprima.ViewShowInsertImage = false;
                this.rtfAnteprima.ViewShowToolbar = false;
                this.rtfAnteprima.ViewUseLargeImages = true;
                this.rtfAnteprima.ViewFont = DrawingProcs.getFontFromString(Database.GetConfigTable(EnumConfigTable.FontPredefinitoRTF));
                
                this.ubIncolla.TextFontRelativeDimension = easyStatics.easyRelativeDimensions.Large;
                this.ubIncolla.ShortcutKey = Keys.I;

                this.utvTesti.TextFontRelativeDimension = easyStatics.easyRelativeDimensions.Medium;
                this.utvTesti.PerformLayout();
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaControlli", this.Name);
            }
        }

        private void CaricaTreeView()
        {

            this.Cursor = Cursors.WaitCursor;
            try
            {

                UltraTreeNode oNode = null;
                UltraTreeNode oNodeParent = null;

                string sKey = @"";
                                this.utvTesti.Nodes.Clear();


                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodUA", CoreStatics.CoreApplication.MovTestoPredefinitoSelezionato.CodUA);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.MovTestoPredefinitoSelezionato.CodRuolo);
                op.Parametro.Add("CodAzione", EnumAzioni.VIS.ToString());
                op.Parametro.Add("CodEntitaTesto", CoreStatics.CoreApplication.MovTestoPredefinitoSelezionato.CodEntita);
                op.Parametro.Add("CodTipoEntita", CoreStatics.CoreApplication.MovTestoPredefinitoSelezionato.CodTipoEntita);
                op.Parametro.Add("IDCampo", CoreStatics.CoreApplication.MovTestoPredefinitoSelezionato.IDCampo);
                op.Parametro.Add("DatiEstesi", "0");

                op.TimeStamp.CodEntita = CoreStatics.CoreApplication.MovTestoPredefinitoSelezionato.CodEntita;
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString(); 
                                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_SelTestiPredefiniti", spcoll);

                                oNode = new UltraTreeNode(CoreStatics.TV_ROOT, CoreStatics.TV_ROOT_TESTI);
                oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_FOLDER_16));
                oNode.Tag = CoreStatics.TV_ROOT;
                this.utvTesti.Nodes.Add(oNode);

                                ds.Tables[0].DefaultView.RowFilter = "Path Is Not Null And Path <> ''";
                ds.Tables[0].DefaultView.Sort = "Path";
                foreach (DataRowView oDataRowV in ds.Tables[0].DefaultView)
                {

                    oNode = this.utvTesti.GetNodeByKey(CoreStatics.TV_ROOT);
                    sKey = @"";
                    Array s_split = oDataRowV["Path"].ToString().Split(@"\".ToCharArray());
                    for (int i = 0; i < s_split.Length; i++)
                    {

                        sKey += (sKey != "" ? @"\" : "") + s_split.GetValue(i).ToString();
                        oNodeParent = this.utvTesti.GetNodeByKey(sKey);
                        if (oNodeParent == null)
                        {

                            oNodeParent = new UltraTreeNode(sKey, s_split.GetValue(i).ToString());
                            oNodeParent.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_FOLDER_256));
                            oNodeParent.Tag = CoreStatics.TV_PATH;
                            oNode.Nodes.Add(oNodeParent);

                        }

                        oNode = oNodeParent;
                    }
                }

                                ds.Tables[0].DefaultView.RowFilter = "";
                ds.Tables[0].DefaultView.Sort = "";
                foreach (DataRow oDataRow in ds.Tables[0].Rows)
                {
                    if (oDataRow.IsNull("Path") || oDataRow["Path"].ToString().Trim() == "")
                    {
                        oNodeParent = this.utvTesti.GetNodeByKey(CoreStatics.TV_ROOT);
                        if (oNodeParent != null)
                        {

                            oNode = new UltraTreeNode(@"\" + oDataRow["Codice"].ToString(), oDataRow["Descrizione"].ToString());
                            oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_TESTOPREDEFINITO_256));
                            oNode.Tag = CoreStatics.TV_TESTO;
                            oNodeParent.Nodes.Add(oNode);

                        }
                    }
                    else
                    {
                        oNodeParent = this.utvTesti.GetNodeByKey(oDataRow["Path"].ToString());
                        if (oNodeParent != null)
                        {

                            oNode = new UltraTreeNode(oDataRow["Path"].ToString() + @"\" + oDataRow["Codice"].ToString(), oDataRow["Descrizione"].ToString());
                            oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_TESTOPREDEFINITO_256));
                            oNode.Tag = CoreStatics.TV_TESTO;
                            oNodeParent.Nodes.Add(oNode);

                        }
                    }
                }
                this.utvTesti.PerformAction(UltraTreeAction.FirstNode, false, false);
                this.utvTesti.PerformAction(UltraTreeAction.ExpandAllNode, false, false);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaTreeView", this.Name);
            }
            this.Cursor = Cursors.Default;
        }

        private void SetAnteprima()
        {
            try
            {
                this.rtfAnteprima.ViewRtf = "";
                this.ubIncolla.Enabled = false;

                if (this.utvTesti.ActiveNode != null && this.utvTesti.ActiveNode.Tag.ToString() == CoreStatics.TV_TESTO)
                { 
                                        string[] reportkeysplit = this.utvTesti.ActiveNode.Key.Split(@"\".ToCharArray());
                    string codtesto = reportkeysplit[reportkeysplit.Length - 1];

                    Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("Codice", codtesto);
                    op.Parametro.Add("CodUA", CoreStatics.CoreApplication.MovTestoPredefinitoSelezionato.CodUA);
                    op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.MovTestoPredefinitoSelezionato.CodRuolo);
                    op.Parametro.Add("CodAzione", EnumAzioni.VIS.ToString());
                    op.Parametro.Add("CodEntitaTesto", CoreStatics.CoreApplication.MovTestoPredefinitoSelezionato.CodEntita);
                    op.Parametro.Add("DatiEstesi", "1");

                    op.TimeStamp.CodEntita = CoreStatics.CoreApplication.MovTestoPredefinitoSelezionato.CodEntita;
                    op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString(); 
                                        SqlParameterExt[] spcoll = new SqlParameterExt[1];

                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    DataTable dt = Database.GetDataTableStoredProc("MSP_SelTestiPredefiniti", spcoll);

                    if (dt != null && dt.Rows.Count > 0 && !dt.Rows[0].IsNull("TestoRTF") && dt.Rows[0]["TestoRTF"].ToString() != "")
                    {
                        this.rtfAnteprima.ViewRtf = dt.Rows[0]["TestoRTF"].ToString();
                        this.ubIncolla.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "SetAnteprima", this.Name);
            }
        }

        #endregion

        #region EVENTI

        private void frmTestiTipo_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void frmTestiTipo_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            if (this.rtfTesto.rtbRichTextBox.Text == "")
            {
                easyStatics.EasyMessageBox("Non è stato selezionato alcun testo!", "Testi Predefiniti", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.utvTesti.Focus();
            }
            else
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                CoreStatics.CoreApplication.MovTestoPredefinitoSelezionato.RitornoRTF = this.rtfTesto.ViewRtf;
                this.Close();
            }
        } 

        private void ubIncolla_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.rtfAnteprima.ViewRtf != "")
                {
                    this.rtfTesto.rtbRichTextBox.SelectedRtf = this.rtfAnteprima.ViewRtf;
                    this.rtfTesto.rtbRichTextBox.SelectionLength = 0;
                    this.rtfTesto.rtbRichTextBox.SelectionStart = this.rtfTesto.rtbRichTextBox.TextLength;
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ubIncolla_Click", this.Name);
            }
        }

        private void utvTesti_AfterActivate(object sender, NodeEventArgs e)
        {
            this.SetAnteprima();
        }

        #endregion

    }
}
