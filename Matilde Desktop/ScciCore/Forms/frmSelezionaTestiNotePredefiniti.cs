using Infragistics.Win.UltraWinTree;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.ScciResource;

namespace UnicodeSrl.ScciCore
{
    public partial class frmSelezionaTestiNotePredefiniti : frmBaseModale, Interfacce.IViewFormlModal
    {

        public frmSelezionaTestiNotePredefiniti()
        {
            InitializeComponent();
        }

        #region Interface

        public void Carica()
        {

            try
            {

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_TESTOPREDEFINITO_256);
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_TESTOPREDEFINITO_256);

                this.InitializeUltraTree();
                this.LoadUltraTree();

                if ((this.UltraTree.Tag as DataTable).Rows.Count == 1)
                {
                    frmSelezionaTestiNotePredefiniti_PulsanteAvantiClick(this.ucBottomModale.ubAvanti, new PulsanteBottomClickEventArgs(EnumPulsanteBottom.Avanti));
                }
                else
                {
                    this.ShowDialog();
                }

            }
            catch (Exception)
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }


        }

        #endregion        

        #region UltraTree

        private void InitializeUltraTree()
        {

            this.UltraTree.LeftImagesSize = new Size(32, 32);

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
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                op.Parametro.Add("CodAzione", CoreStatics.CoreApplication.MovNoteAgendeSelezionata.Azione.ToString());

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataTable oDt = Database.GetDataTableStoredProc("MSP_SelTestiNotePredefiniti", spcoll);

                this.UltraTree.Tag = oDt;
                this.UltraTree.Nodes.Clear();

                                oNode = new UltraTreeNode(CoreStatics.TV_ROOT, CoreStatics.TV_ROOT_TESTI);
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
                            oNode.Tag = CoreStatics.TV_TESTO;
                            oNodeParent.Nodes.Add(oNode);

                        }
                    }
                    else
                    {
                        oNodeParent = this.UltraTree.GetNodeByKey(oDataRow["Path"].ToString());
                        if (oNodeParent != null)
                        {

                            oNode = new UltraTreeNode(oDataRow["Path"].ToString() + @"\" + oDataRow["Codice"].ToString(), oDataRow["Descrizione"].ToString());
                            oNode.Tag = CoreStatics.TV_TESTO;
                            oNodeParent.Nodes.Add(oNode);

                        }
                    }

                }

                this.UltraTree.PerformAction(UltraTreeAction.FirstNode, false, false);
                this.UltraTree.PerformAction(UltraTreeAction.ExpandAllNode, false, false);

                if (oDt.Rows.Count == 1)
                {
                    oNode = this.UltraTree.GetNodeByKey(oDt.Rows[0]["Path"].ToString() + @"\" + oDt.Rows[0]["Codice"].ToString());
                    oNode.Selected = true;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            this.Cursor = Cursors.Default;

        }

        #endregion

        #region Events Form

        private void frmSelezionaTestiNotePredefiniti_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {

            try
            {

                if (this.UltraTree.SelectedNodes.Count > 0 && (string)this.UltraTree.SelectedNodes[0].Tag == CoreStatics.TV_TESTO)
                {

                    DataTable dt = this.UltraTree.Tag as DataTable;
                    CoreStatics.g_split = CoreStatics.SetSplit(this.UltraTree.SelectedNodes[0].Key, @"\");
                    string codtesto = CoreStatics.g_split.GetValue(CoreStatics.g_split.Length - 1).ToString();
                    Boolean continua = true;

                    var query = (from p in dt.AsEnumerable()
                                 where p.Field<string>("Codice") == codtesto
                                 select p).First();

                    if ( (CoreStatics.CoreApplication.MovNoteAgendeSelezionata.Oggetto != string.Empty && CoreStatics.CoreApplication.MovNoteAgendeSelezionata.Oggetto != query["OggettoNota"].ToString())                        
                        ||
                        (CoreStatics.CoreApplication.MovNoteAgendeSelezionata.Descrizione != string.Empty && CoreStatics.CoreApplication.MovNoteAgendeSelezionata.Descrizione != query["DescrizioneNota"].ToString() )
                        ||
                        (CoreStatics.CoreApplication.MovNoteAgendeSelezionata.Colore != "Color [Empty]" && CoreStatics.CoreApplication.MovNoteAgendeSelezionata.Colore != query["Colore"].ToString())
                        )
                    {
                        if (easyStatics.EasyMessageBox("Sei sicuro di voler sovrascrivere i nuovi dati?", "Testo Predefinito", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            continua = true;
                        }
                        else
                        {
                            continua = false;
                        }
                            
                    }

                                        if (continua == true)
                    {
                        if (query["OggettoNota"].ToString() != "") { CoreStatics.CoreApplication.MovNoteAgendeSelezionata.Oggetto = query["OggettoNota"].ToString(); }
                        if (query["DescrizioneNota"].ToString() != "") { CoreStatics.CoreApplication.MovNoteAgendeSelezionata.Descrizione = query["DescrizioneNota"].ToString(); }
                        if (query["Colore"].ToString() != "" & query["Colore"].ToString() != "Color [Empty]" ) { CoreStatics.CoreApplication.MovNoteAgendeSelezionata.Colore = query["Colore"].ToString(); }
                    }

                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void frmSelezionaTestiNotePredefiniti_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        #endregion

    }
}
