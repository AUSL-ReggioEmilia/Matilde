using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;

using Microsoft.Data.SqlClient;
using System.Globalization;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.ScciResource;
using UnicodeSrl.ScciCore.CustomControls.Infra;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;
using Infragistics.Win.UltraWinTree;
using Infragistics.Win.UltraWinListView;
using Infragistics.Win.Misc;
using UnicodeSrl.RTFLibrary.Core;

namespace UnicodeSrl.ScciCore
{
    public partial class ucAllegati : UserControl, Interfacce.IViewUserControlMiddle
    {

        #region Declare

        private UserControl _ucc = null;
        private ucRichTextBox _ucRichTextBox = null;

        private Dictionary<int, byte[]> oIcone = new Dictionary<int, byte[]>();
        private Dictionary<int, byte[]> oIconeFormato = new Dictionary<int, byte[]>();

        private ucEasyPopUpFolder _ucEasyPopUpFolder = null;

        #endregion

        public ucAllegati()
        {
            InitializeComponent();

            _ucc = (UserControl)this;

        }

        #region Interface

        public void Aggiorna()
        {
            if (this.IsDisposed == false)
            {
                VerificaSicurezza();
                ResetDettaglio();
                CaricaFolder(this.tvFolder, "ADD");
                this.AggiornaGriglia(true);
            }

        }

        public void Carica()
        {
            try
            {

                InizializzaControlli();
                InizializzaUltraGridLayout();
                VerificaSicurezza();
                InizializzaFiltri(true);

                ResetDettaglio();
                CaricaFolder(this.tvFolder, "ADD");
                CaricaGriglia(true);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Carica", this.Name);
            }
        }

        public void Ferma()
        {

            try
            {

                oIcone = new Dictionary<int, byte[]>();
                oIconeFormato = new Dictionary<int, byte[]>();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Ferma", this.Name);
            }

        }

        #endregion

        #region private functions

        private void InizializzaControlli()
        {

            try
            {

                CoreStatics.SetEasyUltraDockManager(ref this.UltraDockManager);

                this.ubAddElettronico.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_ALLEGATOELETTRONICO_AGGIUNGI_256);
                this.ubAddVirtuale.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_ALLEGATOVIRTUALE_AGGIUNGI_256);
                this.ubAddFolder.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FOLDERAGGIUNGI_256);

                this.uchkFiltro.UNCheckedImage = Risorse.GetImageFromResource(Risorse.GC_FILTRO_256);
                this.uchkFiltro.CheckedImage = Risorse.GetImageFromResource(Risorse.GC_FILTROAPPLICATO_256);
                this.uchkFiltro.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                this.uchkFiltro.Checked = false;

                this.ubAddElettronico.PercImageFill = 0.75F;
                this.ubAddElettronico.ShortcutKey = Keys.Add;
                this.ubAddElettronico.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.XSmall;
                this.ubAddElettronico.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.ubAddVirtuale.PercImageFill = 0.75F;
                this.ubAddVirtuale.ShortcutKey = Keys.V;
                this.ubAddVirtuale.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.XSmall;
                this.ubAddVirtuale.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.ubAddFolder.PercImageFill = 0.75F;
                this.ubAddFolder.ShortcutKey = Keys.None;
                this.ubAddFolder.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.XSmall;
                this.ubAddFolder.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.uchkFiltro.PercImageFill = 0.75F;
                this.uchkFiltro.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.XSmall;

                this.ubApplicaFiltro.TextFontRelativeDimension = easyStatics.easyRelativeDimensions.Large;

                this.ubModifica.PercImageFill = 0.75F;
                this.ubModifica.ShortcutKey = Keys.None;
                this.ubModifica.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                this.ubModifica.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_MODIFICA_32);

                this.ubCancella.PercImageFill = 0.75F;
                this.ubCancella.ShortcutKey = Keys.None;
                this.ubCancella.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                this.ubCancella.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_ELIMINA_32);

                this.ubSalva.PercImageFill = 0.75F;
                this.ubSalva.ShortcutKey = Keys.None;
                this.ubSalva.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                this.ubSalva.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_SALVA_32);

                this.ubVisualizza.PercImageFill = 0.75F;
                this.ubVisualizza.ShortcutKey = Keys.None;
                this.ubVisualizza.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                this.ubVisualizza.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_DOWNLOADDOCUMENTO_32);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaControlli", this.Name);
            }
        }

        private void InizializzaUltraGridLayout()
        {
            try
            {
                CoreStatics.SetEasyUltraGridLayout(ref this.ucEasyGrid);
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaUltraGridLayout", this.Name);
            }
        }

        private void VerificaSicurezza()
        {

            try
            {
                if (CoreStatics.CoreApplication.Cartella != null && CoreStatics.CoreApplication.Cartella.CartellaChiusa == true)
                {
                    this.ubAddElettronico.Enabled = false;
                    this.ubAddVirtuale.Enabled = false;
                }
                else
                {
                    this.ubAddElettronico.Enabled = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Allegati_Inserisci);
                    this.ubAddVirtuale.Enabled = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Allegati_Inserisci);
                }
            }
            catch (Exception)
            {
            }
        }
        private void InizializzaFiltri(Boolean bFiltriDefault = false)
        {
            if (this.IsDisposed == false)
            {
                try
                {

                    UltraTreeNode oNodeRoot = null;
                    UltraTreeNode oNode = null;

                    this.tvEntitaAssociate.Nodes.Clear();

                    oNodeRoot = new UltraTreeNode(CoreStatics.TV_ROOT, "Associati a:");
                    oNodeRoot.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_FOLDER_16));
                    oNodeRoot.Tag = CoreStatics.TV_ROOT;

                    oNode = new UltraTreeNode("PAZ", "Paziente");
                    oNode.Override.NodeStyle = NodeStyle.CheckBox;

                    if (bFiltriDefault == true && CoreStatics.CoreApplication.Episodio != null)
                        oNode.CheckedState = CheckState.Unchecked;
                    else
                        oNode.CheckedState = CheckState.Checked;

                    oNodeRoot.Nodes.Add(oNode);

                    if (CoreStatics.CoreApplication.Episodio != null)
                    {

                        oNode = new UltraTreeNode("CAR", "Cartella Corrente");
                        oNode.Override.NodeStyle = NodeStyle.CheckBox;

                        if (bFiltriDefault == true)
                            oNode.CheckedState = CheckState.Checked;
                        else
                            oNode.CheckedState = CheckState.Checked;

                        oNodeRoot.Nodes.Add(oNode);

                        oNode = new UltraTreeNode("ALTRE", "Altre Cartelle");
                        oNode.Override.NodeStyle = NodeStyle.CheckBox;

                        if (bFiltriDefault == true)
                            oNode.CheckedState = CheckState.Unchecked;
                        else
                            oNode.CheckedState = CheckState.Unchecked;

                        oNodeRoot.Nodes.Add(oNode);
                    }

                    this.tvEntitaAssociate.Nodes.Add(oNodeRoot);
                    this.tvEntitaAssociate.PerformAction(UltraTreeAction.FirstNode, false, false);
                    this.tvEntitaAssociate.PerformAction(UltraTreeAction.ExpandNode, false, false);

                    if (this.ucEasyGridFiltroUA.Rows.Count > 0)
                    {
                        this.ucEasyGridFiltroUA.ActiveRow = null;
                        this.ucEasyGridFiltroUA.Selected.Rows.Clear();
                        CoreStatics.SelezionaRigaInGriglia(ref ucEasyGridFiltroUA, "CodUA", CoreStatics.GC_TUTTI);
                    }

                    if (this.ucEasyGridFiltroTipo.Rows.Count > 0)
                    {
                        this.ucEasyGridFiltroTipo.ActiveRow = null;
                        this.ucEasyGridFiltroTipo.Selected.Rows.Clear();
                        CoreStatics.SelezionaRigaInGriglia(ref ucEasyGridFiltroTipo, "CodTipo", CoreStatics.GC_TUTTI);
                    }

                    this.utxtIDDocumento.Text = "";
                }
                catch (Exception ex)
                {
                    CoreStatics.ExGest(ref ex, "InizializzaFiltri", this.Name);
                }
            }


        }

        private void CaricaGriglia(bool datiEstesi)
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Paziente.ID);


                if (datiEstesi)
                    op.Parametro.Add("DatiEstesi", "1");
                else
                    op.Parametro.Add("DatiEstesi", "0");

                Dictionary<string, string> listaEA = new Dictionary<string, string>();
                foreach (UltraTreeNode oNode in this.tvEntitaAssociate.Nodes[CoreStatics.TV_ROOT].Nodes)
                {
                    if (oNode.Override.NodeStyle == NodeStyle.CheckBox && oNode.CheckedState == CheckState.Checked)
                    {
                        listaEA.Add(oNode.Key, oNode.Text);
                    }
                }
                string[] codEA = listaEA.Keys.ToArray();
                if (codEA.Length > 0)
                {
                    op.ParametroRipetibile.Add("EntitaAssociata", codEA);
                }

                op.TimeStamp.CodEntita = EnumEntita.ALL.ToString(); op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_SelMovAllegati", spcoll);

                DataTable dtEdit = ds.Tables[0].Copy();

                DataColumn colsp = new DataColumn(CoreStatics.C_COL_SPAZIO, typeof(string));
                colsp.AllowDBNull = true;
                colsp.DefaultValue = "";
                colsp.Unique = false;
                dtEdit.Columns.Add(colsp);

                foreach (DataColumn dcCol in dtEdit.Columns)
                {
                    if (dcCol.ColumnName.ToUpper().IndexOf("PERMESSOMODIFICA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOCANCELLA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOVISUALIZZA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("ICON") == 0)
                        dcCol.ReadOnly = false;
                }
                this.ucEasyGrid.DataSource = null;


                this.ucEasyGrid.DataSource = dtEdit;
                this.ucEasyGrid.Refresh();

                this.ucEasyGrid.PerformAction(UltraGridAction.FirstRowInBand, false, false);

                this.ucEasyGridFiltroUA.DataSource = CoreStatics.AggiungiTuttiDataTable(ds.Tables[2], true);
                this.ucEasyGridFiltroUA.Refresh();

                this.ucEasyGridFiltroTipo.DataSource = CoreStatics.AggiungiTuttiDataTable(ds.Tables[1], true);
                this.ucEasyGridFiltroTipo.Refresh();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaGriglia", this.Name);
            }
        }

        private void AggiornaGriglia(bool datiEstesi)
        {

            bool bFiltro = false;
            string coduaselezionato = string.Empty;
            string codtiposelezionato = string.Empty;

            CoreStatics.SetNavigazione(false);

            try
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Paziente.ID);


                if (this.ucEasyGridFiltroUA.ActiveRow != null && this.ucEasyGridFiltroUA.ActiveRow.Cells["CodUA"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                {
                    op.Parametro.Add("CodUA", this.ucEasyGridFiltroUA.ActiveRow.Cells["CodUA"].Text);
                    bFiltro = true;
                }

                if (this.ucEasyGridFiltroTipo.ActiveRow != null && this.ucEasyGridFiltroTipo.ActiveRow.Cells["CodTipo"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                {
                    op.Parametro.Add("CodTipoAllegato", this.ucEasyGridFiltroTipo.ActiveRow.Cells["CodTipo"].Text);
                    bFiltro = true;
                }
                if (this.utxtIDDocumento.Text.Trim() != "")
                {
                    op.Parametro.Add("FiltroGenerico", this.utxtIDDocumento.Text);
                    bFiltro = true;
                }

                if (datiEstesi && !bFiltro)
                    op.Parametro.Add("DatiEstesi", "1");
                else
                    op.Parametro.Add("DatiEstesi", "0");

                Dictionary<string, string> listaEA = new Dictionary<string, string>();
                foreach (UltraTreeNode oNode in this.tvEntitaAssociate.Nodes[CoreStatics.TV_ROOT].Nodes)
                {
                    if (oNode.Override.NodeStyle == NodeStyle.CheckBox && oNode.CheckedState == CheckState.Checked)
                    {
                        listaEA.Add(oNode.Key, oNode.Text);
                    }
                }
                string[] codEA = listaEA.Keys.ToArray();
                if (codEA.Length > 0)
                {
                    op.ParametroRipetibile.Add("EntitaAssociata", codEA);
                }

                this.uchkFiltro.Checked = bFiltro;

                op.TimeStamp.CodEntita = EnumEntita.ALL.ToString(); op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_SelMovAllegati", spcoll);

                DataTable dtEdit = ds.Tables[0].Copy();

                DataColumn colsp = new DataColumn(CoreStatics.C_COL_SPAZIO, typeof(string));
                colsp.AllowDBNull = true;
                colsp.DefaultValue = "";
                colsp.Unique = false;
                dtEdit.Columns.Add(colsp);

                foreach (DataColumn dcCol in dtEdit.Columns)
                {
                    if (dcCol.ColumnName.ToUpper().IndexOf("PERMESSOMODIFICA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOCANCELLA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOVISUALIZZA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("ICON") == 0)
                        dcCol.ReadOnly = false;
                }
                this.ucEasyGrid.DataSource = null;
                this.ucEasyGrid.DataSource = dtEdit;
                this.ucEasyGrid.Refresh();

                if (datiEstesi && !bFiltro)
                {

                    this.ucEasyGridFiltroUA.DisplayLayout.Bands[0].Columns.ClearUnbound();
                    if (this.ucEasyGridFiltroUA.ActiveRow != null && this.ucEasyGridFiltroUA.ActiveRow.Cells["CodUA"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                        coduaselezionato = this.ucEasyGridFiltroUA.ActiveRow.Cells["CodUA"].Text;
                    this.ucEasyGridFiltroUA.DataSource = CoreStatics.AggiungiTuttiDataTable(ds.Tables[2], true);
                    this.ucEasyGridFiltroUA.Refresh();
                    if (this.ucEasyGridFiltroUA.ActiveRow != null && this.ucEasyGridFiltroUA.ActiveRow.Cells["CodUA"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                        CoreStatics.SelezionaRigaInGriglia(ref ucEasyGridFiltroUA, "CodUA", coduaselezionato);

                    this.ucEasyGridFiltroTipo.DisplayLayout.Bands[0].Columns.ClearUnbound();
                    if (this.ucEasyGridFiltroTipo.ActiveRow != null && this.ucEasyGridFiltroTipo.ActiveRow.Cells["CodTipo"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                        codtiposelezionato = this.ucEasyGridFiltroTipo.ActiveRow.Cells["CodTipo"].Text;
                    this.ucEasyGridFiltroTipo.DataSource = CoreStatics.AggiungiTuttiDataTable(ds.Tables[1], true);
                    this.ucEasyGridFiltroTipo.Refresh();
                    if (this.ucEasyGridFiltroTipo.ActiveRow != null && this.ucEasyGridFiltroTipo.ActiveRow.Cells["CodTipo"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                        CoreStatics.SelezionaRigaInGriglia(ref ucEasyGridFiltroTipo, "CodTipo", codtiposelezionato);

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "AggiornaGriglia", this.Name);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
            }

            CoreStatics.SetNavigazione(true);

        }

        private void CaricaFolder(ucEasyTreeView utv, string azione)
        {

            UltraTreeNode oNodeRoot = null;

            try
            {

                utv.Nodes.Clear();

                oNodeRoot = new UltraTreeNode(CoreStatics.TV_ROOT, CoreStatics.TV_ROOT_ALLEGATI);
                oNodeRoot.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_FOLDER_16));
                oNodeRoot.Tag = CoreStatics.TV_ROOT;
                utv.Nodes.Add(oNodeRoot);

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Paziente.ID);

                if (CoreStatics.CoreApplication.Trasferimento == null)
                {
                    op.Parametro.Add("CodEntita", "PAZ");
                }
                else
                {
                    if (azione == "EDIT")
                    {
                        op.Parametro.Add("CodEntita", this.tvFolder.ActiveNode.Tag.ToString());
                    }
                }

                op.TimeStamp.CodEntita = EnumEntita.ALL.ToString(); op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_SelMovFolder", spcoll);

                var results = from myRow in ds.Tables[0].AsEnumerable()
                              where myRow.IsNull("IDFolderPadre")
                              select myRow;
                if (results.Count() > 0)
                {
                    DataTable dtItem = results.CopyToDataTable();
                    this.CaricaFolderChild(ds, oNodeRoot, dtItem);
                }

                utv.PerformAction(UltraTreeAction.FirstNode, false, false);
                utv.PerformAction(UltraTreeAction.ExpandNode, false, false);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaFolder", this.Name);
            }

        }

        private void CaricaFolderChild(DataSet ds, UltraTreeNode oNodeParent, DataTable dt)
        {

            try
            {

                foreach (DataRow oRow in dt.Rows)
                {

                    Infragistics.Win.UltraWinTree.UltraTreeNode oNodeChild = new Infragistics.Win.UltraWinTree.UltraTreeNode(oRow["ID"].ToString(), oRow["Descrizione"].ToString());
                    oNodeChild.LeftImages.Add(Risorse.GetImageFromResource((oRow["CodEntita"].ToString() == "CAR" ? Risorse.GC_FOLDERCARTELLA_32 : Risorse.GC_FOLDERPAZIENTE_32)));
                    oNodeChild.Tag = oRow["CodEntita"].ToString();
                    oNodeParent.Nodes.Add(oNodeChild);

                    var results = from myRow in ds.Tables[0].AsEnumerable()
                                  where myRow["IDFolderPadre"].Equals(oRow["ID"])
                                  select myRow;
                    if (results.Count() > 0)
                    {
                        DataTable dtItem = results.CopyToDataTable();
                        this.CaricaFolderChild(ds, oNodeChild, dtItem);
                    }

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaFolderChild", this.Name);
            }

        }

        private void CaricaFolderDettaglio(UltraTreeNode node)
        {

            try
            {

                this.ResetDettaglio();

                if (node != null && node.Key != CoreStatics.TV_ROOT)
                {

                    MovFolder oMovFolder = new MovFolder(node.Key);

                    if (oMovFolder.CodEntita == EnumEntita.PAZ.ToString())
                        this.pbAllegato.Image = Risorse.GetImageFromResource(Risorse.GC_FOLDERPAZIENTE_256);
                    else
                        this.pbAllegato.Image = Risorse.GetImageFromResource(Risorse.GC_FOLDERCARTELLA_256);


                    this.lblAllegato.Text = string.Format("Folder: {0} ({1})\n{2} {3}\n{4}",
                                                            oMovFolder.Descrizione,
                                                            node.FullPath,
                                                            (oMovFolder.DataUltimaModifica == DateTime.MinValue ? "Creato il " + oMovFolder.DataRilevazione : "Modificato il " + oMovFolder.DataUltimaModifica),
                                                            (oMovFolder.CodUtenteUltimaModifica == string.Empty ? "da " + oMovFolder.DescrUtenteRilevazione : "da " + oMovFolder.DescrUtenteUltimaModifica),
                                                            oMovFolder.UA);

                    this.ubModifica.Visible = (oMovFolder.PermessoModifica == 1 ? true : false);
                    this.ubCancella.Visible = (oMovFolder.PermessoCancella == 1 && this.lbAllegati.Items.Count == 0 && node.HasNodes == false ? true : false);

                    oMovFolder = null;

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaFolderDettaglio", this.Name);
            }

        }

        private void CaricaAllegati(UltraTreeNode node)
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Paziente.ID);



                if (node.Key != CoreStatics.TV_ROOT)
                    op.Parametro.Add("IDFolder", node.Key);

                op.TimeStamp.CodEntita = EnumEntita.ALL.ToString(); op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_SelMovAllegati", spcoll);

                this.lbAllegati.Items.Clear();
                this.lbAllegati.View = UltraListViewStyle.Icons;
                this.lbAllegati.ViewSettingsIcons.ImageSize = new Size(64, 64);
                this.lbAllegati.ViewSettingsIcons.MaxLines = 4;
                this.lbAllegati.ViewSettingsIcons.TextAreaAlignment = TextAreaAlignment.Bottom;

                foreach (DataRow oDr in ds.Tables[0].Rows)
                {

                    if ((node.Key == CoreStatics.TV_ROOT && oDr["IDFolder"].ToString() == string.Empty) ||
                        (node.Key != CoreStatics.TV_ROOT && oDr["IDFolder"].ToString() != string.Empty))
                    {

                        UltraListViewItem uli = new UltraListViewItem(oDr["ID"].ToString());
                        RtfTree Tree = new RtfTree();
                        Tree.LoadRtfText(oDr["TestoRTF"].ToString());
                        uli.Value = (Tree.Text != "" ? Tree.Text : "Nessuna descrizione."); ;
                        Tree = null;
                        if (oIcone.ContainsKey(Convert.ToInt32(oDr["IDIcona"])) == false)
                        {
                            oIcone.Add(Convert.ToInt32(oDr["IDIcona"]), CoreStatics.GetImageForGrid(Convert.ToInt32(oDr["IDIcona"]), 256));
                        }
                        uli.Appearance.Image = CoreStatics.ByteToImage(oIcone[Convert.ToInt32(oDr["IDIcona"].ToString())]);
                        this.lbAllegati.Items.Add(uli);

                    }
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaAllegati", this.Name);
            }

        }

        private void CaricaAllegatiDettaglio(UltraListViewItem item)
        {

            try
            {

                this.ResetDettaglio();

                if (item != null)
                {

                    MovAllegato oMovAllegato = new MovAllegato(item.Key);

                    this.pbAllegato.Image = CoreStatics.ByteToImage(oIcone[Convert.ToInt32(oMovAllegato.IDIcona)]);
                    this.lblAllegato.Text = string.Format("Nome File: {0} {1} {5}\n{2} {3}\n{4}",
                                                            oMovAllegato.NomeFile,
                                                            (oMovAllegato.Folder == string.Empty ? "" : "- Folder: " + oMovAllegato.Folder),
                                                            (oMovAllegato.DataModifica == DateTime.MinValue ? "Creato il " + oMovAllegato.DataRilevazione : "Modificato il " + oMovAllegato.DataModifica),
                                                            (oMovAllegato.CodUtenteUltimaModifica == string.Empty ? "da " + oMovAllegato.DescrUtenteRilevazione : "da " + oMovAllegato.DescrUtenteUltimaModifica),
                                                            oMovAllegato.UA,
                                                            (oMovAllegato.InfoFirmaDigitale == string.Empty ? "" : "- " + oMovAllegato.InfoFirmaDigitale));

                    if (oIconeFormato.ContainsKey(oMovAllegato.IDIconaFormato) == false)
                    {
                        oIconeFormato.Add(oMovAllegato.IDIconaFormato, CoreStatics.GetImageForGrid(oMovAllegato.IDIconaFormato, 256));
                    }
                    this.pbFormato.Image = CoreStatics.ByteToImage(oIconeFormato[oMovAllegato.IDIconaFormato]);

                    this.ubModifica.Visible = (oMovAllegato.PermessoModifica == 1 ? true : false);
                    this.ubCancella.Visible = (oMovAllegato.PermessoCancella == 1 ? true : false);
                    this.ubSalva.Visible = (oMovAllegato.PermessoVisualizza == 1 ? true : false); ;
                    this.ubVisualizza.Visible = (oMovAllegato.PermessoVisualizza == 1 ? true : false); ;

                    oMovAllegato = null;

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaAllegatiDettaglio", this.Name);
            }

        }

        private void ResetDettaglio()
        {
            this.pbAllegato.Image = null;
            this.pbFormato.Image = null;
            this.lblAllegato.Text = "";
            this.ubModifica.Visible = false;
            this.ubCancella.Visible = false;
            this.ubSalva.Visible = false;
            this.ubVisualizza.Visible = false;
        }

        #endregion

        #region Events

        private void ucEasyTabControlAllegati_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
        {

            switch (e.Tab.Key)
            {

                case "Folder":
                    this.UltraDockManager.Enabled = false;
                    this.ubAddFolder.Visible = true;
                    CaricaFolder(this.tvFolder, "ADD");
                    break;

                case "Griglia":
                    this.UltraDockManager.Enabled = true;
                    this.ubAddFolder.Visible = false;
                    CaricaGriglia(true);
                    break;

            }

        }

        private void UltraDockManager_InitializePane(object sender, Infragistics.Win.UltraWinDock.InitializePaneEventArgs e)
        {
            e.Pane.Settings.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Medium);
            e.Pane.Settings.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FILTRO_32);

            int filtroWidth = 20 * (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large);
            this.UltraDockManager.ControlPanes[0].FlyoutSize = new Size(filtroWidth, this.UltraDockManager.ControlPanes[0].FlyoutSize.Height);
            this.UltraDockManager.ControlPanes[0].Size = new Size(filtroWidth, this.UltraDockManager.ControlPanes[0].Size.Height);
            this.UltraDockManager.DockAreas[0].Size = new Size(filtroWidth, this.UltraDockManager.DockAreas[0].Size.Height);
            this.pnlFiltro.Width = filtroWidth;
        }

        private void tvFolder_AfterActivate(object sender, NodeEventArgs e)
        {
            this.CaricaAllegati(e.TreeNode);
            this.CaricaFolderDettaglio(e.TreeNode);
        }

        private void tvFolder_Enter(object sender, EventArgs e)
        {
            if (this.tvFolder.ActiveNode != null)
            {
                this.CaricaAllegati(this.tvFolder.ActiveNode);
                this.CaricaFolderDettaglio(this.tvFolder.ActiveNode);
            }
        }

        private void lbAllegati_ItemActivated(object sender, ItemActivatedEventArgs e)
        {
            this.CaricaAllegatiDettaglio(e.Item);
        }

        private void ubModifica_Click(object sender, EventArgs e)
        {

            try
            {

                if (this.lbAllegati.ActiveItem == null)
                {
                    CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerFolderAdd);
                    _ucEasyPopUpFolder = new ucEasyPopUpFolder();
                    _ucEasyPopUpFolder.Tag = "EDIT";
                    _ucEasyPopUpFolder.Init();
                    int iWidthAdd = Convert.ToInt32(easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Medium)) * 40;
                    int iHeightAdd = Convert.ToInt32((double)iWidthAdd / 1.52D);
                    _ucEasyPopUpFolder.Size = new Size(iWidthAdd, iHeightAdd);
                    _ucEasyPopUpFolder.tvFolder.TextFontRelativeDimension = easyStatics.easyRelativeDimensions.Medium;
                    this.CaricaFolder(_ucEasyPopUpFolder.tvFolder, "EDIT");
                    UltraTreeNode oNode = _ucEasyPopUpFolder.tvFolder.GetNodeByKey(this.tvFolder.ActiveNode.Key);
                    _ucEasyPopUpFolder.tvFolder.ActiveNode = oNode;
                    _ucEasyPopUpFolder.tvFolder.ActiveNode.Selected = true;
                    _ucEasyPopUpFolder.txtFolder.Text = this.tvFolder.ActiveNode.Text;
                    _ucEasyPopUpFolder.Focus();
                    this.UltraPopupControlContainerFolderAdd.Show();
                }
                else
                {
                    CoreStatics.CoreApplication.MovAllegatoSelezionato = new MovAllegato(this.lbAllegati.ActiveItem.Key, EnumAzioni.MOD);
                    if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.AllegatiEditing) == DialogResult.OK)
                    {
                        this.CaricaAllegati(this.tvFolder.ActiveNode);
                    }
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ubModifica_Click", this.Name);
            }

        }

        private void ubCancella_Click(object sender, EventArgs e)
        {

            try
            {

                if (this.lbAllegati.ActiveItem == null)
                {
                    if (easyStatics.EasyMessageBox("Sei sicuro di voler CANCELLARE" + Environment.NewLine + "il folder selezionato ?", "Cancellazione Folder", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        MovFolder movfol = new MovFolder(this.tvFolder.ActiveNode.Key);
                        movfol.CodStatoFolder = @"CA";
                        if (movfol.Cancella())
                        {
                            this.CaricaFolder(this.tvFolder, "ADD");
                        }
                        movfol = null;
                    }
                }
                else
                {
                    if (easyStatics.EasyMessageBox("Sei sicuro di voler CANCELLARE" + Environment.NewLine + "l'Allegato selezionato ?", "Cancellazione Allegati", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        MovAllegato movall = new MovAllegato(this.lbAllegati.ActiveItem.Key);
                        movall.CodStatoAllegato = @"CA";
                        if (movall.Cancella())
                        {
                            this.InizializzaFiltri();
                            AggiornaGriglia(true);
                            this.CaricaFolder(this.tvFolder, "ADD");
                        }
                        movall = null;
                    }
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ubCancella_Click", this.Name);
            }

        }

        private void ubSalva_Click(object sender, EventArgs e)
        {

            try
            {

                CoreStatics.CoreApplication.MovAllegatoSelezionato = new MovAllegato(this.lbAllegati.ActiveItem.Key, EnumAzioni.MOD);
                CoreStatics.SalvaAllegato(CoreStatics.CoreApplication.MovAllegatoSelezionato.Documento, CoreStatics.CoreApplication.MovAllegatoSelezionato.NomeFile);
                CoreStatics.CoreApplication.MovAllegatoSelezionato = null;

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ubSalva_Click", this.Name);
            }

        }

        private void ubVisualizza_Click(object sender, EventArgs e)
        {

            try
            {

                CoreStatics.CoreApplication.MovAllegatoSelezionato = new MovAllegato(this.lbAllegati.ActiveItem.Key, EnumAzioni.MOD);
                CoreStatics.ApriAllegato(CoreStatics.CoreApplication.MovAllegatoSelezionato.Documento, CoreStatics.CoreApplication.MovAllegatoSelezionato.NomeFile);
                CoreStatics.CoreApplication.MovAllegatoSelezionato = null;

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ubVisualizza_Click", this.Name);
            }

        }

        private void ucEasyGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {
                int refWidth = (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XLarge) * 3;

                Graphics g = this.CreateGraphics();
                int refBtnWidth = Convert.ToInt32(CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(this.ucEasyGrid.DataRowFontRelativeDimension), g.DpiY) * 2.4);
                g.Dispose();
                g = null;
                e.Layout.Bands[0].HeaderVisible = false;
                e.Layout.Bands[0].ColHeadersVisible = false;
                e.Layout.Bands[0].RowLayoutStyle = RowLayoutStyle.GroupLayout;
                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {

                    try
                    {
                        oCol.SortIndicator = SortIndicator.Disabled;
                        switch (oCol.Key)
                        {
                            case "Icona":
                                oCol.Hidden = false;
                                oCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                                oCol.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.VertScrollBar = false;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.LockedWidth = true;
                                try
                                {
                                    oCol.MaxWidth = refWidth;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 0;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 3;

                                break;

                            case "DataEvento":
                                oCol.Hidden = false;
                                oCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Red;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.Format = "dd/MM/yyyy HH:mm";
                                try
                                {
                                    oCol.MaxWidth = refWidth * 2;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "DescrUtenteRilevazione":
                                oCol.Hidden = false;
                                oCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.LockedWidth = true;
                                try
                                {
                                    oCol.MaxWidth = refWidth * 2;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 1;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "NumeroDocumento":
                                oCol.Hidden = false;
                                oCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.LockedWidth = true;
                                try
                                {
                                    oCol.MaxWidth = refWidth * 2;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 2;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;


                            case CoreStatics.C_COL_SPAZIO:
                                oCol.Hidden = false;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XXSmall);
                                oCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = refWidth * 2;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.RowLayoutColumnInfo.WeightY = 1;
                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 3;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;


                            case "TestoRTF":
                                oCol.Hidden = false;

                                RichTextEditor a = new RichTextEditor();
                                oCol.Editor = a;
                                oCol.CellActivation = Activation.ActivateOnly;
                                oCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.VertScrollBar = true;
                                try
                                {
                                    oCol.MaxWidth = this.ucEasyGrid.Width - (refWidth * 5) - Convert.ToInt32(refBtnWidth * 4.85) - 30;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 2;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 4;

                                break;

                            case "DescrTipoAllegato":
                                oCol.Hidden = false;
                                oCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = refWidth * 2;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 3;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "InfoFirmaDigitale":
                                oCol.Hidden = false;
                                oCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = refWidth * 2;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 3;
                                oCol.RowLayoutColumnInfo.OriginY = 1;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            default:
                                oCol.Hidden = true;
                                break;

                        }
                    }
                    catch (Exception)
                    {
                    }

                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_EDIT))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_EDIT);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_MODIFICA_32);


                    colEdit.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = refBtnWidth;
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;


                    colEdit.RowLayoutColumnInfo.OriginX = 4;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;
                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_EDIT + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_EDIT + @"_SP");
                    colEdit.Hidden = false;
                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                    colEdit.CellActivation = Activation.Disabled;
                    colEdit.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = Convert.ToInt32(refBtnWidth * 0.25);
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;
                    colEdit.RowLayoutColumnInfo.OriginX = 5;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;
                }

                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_DEL))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_DEL);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_ELIMINA_32);


                    colEdit.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = refBtnWidth;
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;


                    colEdit.RowLayoutColumnInfo.OriginX = 6;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;
                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_DEL + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_DEL + @"_SP");
                    colEdit.Hidden = false;
                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                    colEdit.CellActivation = Activation.Disabled;
                    colEdit.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = Convert.ToInt32(refBtnWidth * 0.25);
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;
                    colEdit.RowLayoutColumnInfo.OriginX = 7;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;
                }


                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_COPY))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_COPY);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_SALVA_32);

                    colEdit.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = refBtnWidth;
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;


                    colEdit.RowLayoutColumnInfo.OriginX = 8;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;
                }


                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_COPY + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_COPY + @"_SP");
                    colEdit.Hidden = false;
                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                    colEdit.CellActivation = Activation.Disabled;
                    colEdit.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = Convert.ToInt32(refBtnWidth * 0.25);
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;
                    colEdit.RowLayoutColumnInfo.OriginX = 9;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;
                }



                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_VIEW))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_VIEW);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_DOWNLOADDOCUMENTO_32);

                    colEdit.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = refBtnWidth;
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;


                    colEdit.RowLayoutColumnInfo.OriginX = 10;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;
                }



                if (!e.Layout.Bands[0].Columns.Exists(@"COLFINE_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(@"COLFINE_SP");
                    colEdit.Hidden = false;
                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                    colEdit.CellActivation = Activation.Disabled;
                    colEdit.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = Convert.ToInt32(refBtnWidth * 0.1);
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;
                    colEdit.RowLayoutColumnInfo.OriginX = 11;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;
                }

            }
            catch (Exception)
            {
            }
        }

        private void ucEasyGrid_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            try
            {

                if (e.Row.Cells.Exists("Icona") == true && e.Row.Cells["IDIcona"].Value.ToString() != "")
                {
                    if (oIcone.ContainsKey(Convert.ToInt32(e.Row.Cells["IDIcona"].Value)) == false)
                    {
                        oIcone.Add(Convert.ToInt32(e.Row.Cells["IDIcona"].Value), CoreStatics.GetImageForGrid(Convert.ToInt32(e.Row.Cells["IDIcona"].Value), 256));
                    }
                    e.Row.Cells["Icona"].Value = oIcone[Convert.ToInt32(e.Row.Cells["IDIcona"].Value)];
                    e.Row.Update();
                }

                foreach (UltraGridCell ocell in e.Row.Cells)
                {
                    if (CoreStatics.CoreApplication.Cartella != null && CoreStatics.CoreApplication.Cartella.CartellaChiusa == true)
                    {
                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_DEL)
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_EDIT)
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_VIEW && ocell.Row.Cells["PermessoVisualizza"].Value.ToString() == "0")
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_COPY && ocell.Row.Cells["PermessoVisualizza"].Value.ToString() == "0")
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                    }
                    else
                    {
                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_DEL && ocell.Row.Cells["PermessoCancella"].Value.ToString() == "0")
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_EDIT && ocell.Row.Cells["PermessoModifica"].Value.ToString() == "0")
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_VIEW && ocell.Row.Cells["PermessoVisualizza"].Value.ToString() == "0")
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_COPY && ocell.Row.Cells["PermessoVisualizza"].Value.ToString() == "0")
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                    }
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGrid_InitializeRow", this.Name);
            }
        }

        private void ucEasyGrid_ClickCellButton(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            try
            {
                switch (e.Cell.Column.Key)
                {
                    case CoreStatics.C_COL_BTN_EDIT:
                        if (e.Cell.Row.Cells["PermessoModifica"].Text == "1")
                        {

                            CoreStatics.CoreApplication.MovAllegatoSelezionato = new MovAllegato(e.Cell.Row.Cells["ID"].Text, EnumAzioni.MOD);

                            if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.AllegatiEditing) == DialogResult.OK)
                            {
                                AggiornaGriglia(true);
                                if (CoreStatics.CoreApplication.MovAllegatoSelezionato != null)
                                {
                                    CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "ID", CoreStatics.CoreApplication.MovAllegatoSelezionato.IDMovAllegato);
                                    CoreStatics.CoreApplication.MovAllegatoSelezionato = null;
                                }
                            }

                        }

                        break;

                    case CoreStatics.C_COL_BTN_DEL:
                        if (e.Cell.Row.Cells["PermessoCancella"].Text == "1")
                        {

                            if (easyStatics.EasyMessageBox("Sei sicuro di voler CANCELLARE" + Environment.NewLine + "l'Allegato selezionato ?", "Cancellazione Allegati", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                MovAllegato movall = new MovAllegato(e.Cell.Row.Cells["ID"].Text);
                                movall.CodStatoAllegato = @"CA";
                                if (movall.Cancella())
                                {
                                    this.InizializzaFiltri();
                                    AggiornaGriglia(true);
                                }
                            }
                        }
                        break;

                    case CoreStatics.C_COL_BTN_VIEW:
                        if (e.Cell.Row.Cells["PermessoVisualizza"].Text == "1")
                        {
                            CoreStatics.CoreApplication.MovAllegatoSelezionato = new MovAllegato(e.Cell.Row.Cells["ID"].Text, EnumAzioni.MOD);
                            CoreStatics.ApriAllegato(CoreStatics.CoreApplication.MovAllegatoSelezionato.Documento, CoreStatics.CoreApplication.MovAllegatoSelezionato.NomeFile);
                            CoreStatics.CoreApplication.MovAllegatoSelezionato = null;
                        }
                        break;

                    case CoreStatics.C_COL_BTN_COPY:
                        if (e.Cell.Row.Cells["PermessoVisualizza"].Text == "1")
                        {
                            CoreStatics.CoreApplication.MovAllegatoSelezionato = new MovAllegato(e.Cell.Row.Cells["ID"].Text, EnumAzioni.MOD);
                            CoreStatics.SalvaAllegato(CoreStatics.CoreApplication.MovAllegatoSelezionato.Documento, CoreStatics.CoreApplication.MovAllegatoSelezionato.NomeFile);
                            CoreStatics.CoreApplication.MovAllegatoSelezionato = null;
                        }
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGrid_ClickCellButton", this.Name);
            }
        }

        private void ucEasyGrid_ClickCell(object sender, ClickCellEventArgs e)
        {

            try
            {

                switch (e.Cell.Column.Key)
                {

                    case "TestoRTF":
                        CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainer);
                        _ucRichTextBox = CoreStatics.GetRichTextBoxForPopupControlContainer(e.Cell.Text);

                        Infragistics.Win.UIElement uie = e.Cell.GetUIElement();
                        Point oPoint = new Point(uie.Rect.Left, uie.Rect.Top);

                        this.UltraPopupControlContainer.Show(this.ucEasyGrid, oPoint);
                        break;

                    default:
                        break;

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGrid_ClickCell", this.Name);
            }

        }

        private void ucEasyGridFiltroUA_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            e.Layout.Bands[0].HeaderVisible = false;
            foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
            {
                switch (oCol.Key)
                {
                    case "DescUA":
                        oCol.Hidden = false;
                        oCol.Header.Caption = @"Struttura";
                        break;
                    default:
                        oCol.Hidden = true;
                        break;
                }
            }

        }

        private void ucEasyGridFiltroTipo_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].HeaderVisible = false;
            foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
            {
                switch (oCol.Key)
                {
                    case "DescTipo":
                        oCol.Hidden = false;
                        oCol.Header.Caption = @"Tipo";
                        break;
                    default:
                        oCol.Hidden = true;
                        break;
                }
            }

        }

        private void ubApplicaFiltro_Click(object sender, EventArgs e)
        {
            if (this.UltraDockManager.FlyoutPane != null && !this.UltraDockManager.FlyoutPane.Pinned) this.UltraDockManager.FlyIn();
            this.ucEasyTabControlAllegati.PerformAction(Infragistics.Win.UltraWinTabControl.UltraTabControlAction.NavigateToLastTab);
            this.AggiornaGriglia(true);
        }

        private void uchkFiltro_Click(object sender, EventArgs e)
        {
            if (!this.uchkFiltro.Checked)
            {
                this.InizializzaFiltri();
                this.AggiornaGriglia(false);
            }
            else
                this.uchkFiltro.Checked = !this.uchkFiltro.Checked;
        }

        private void ubAddElettronico_Click(object sender, EventArgs e)
        {
            try
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);


                if (CoreStatics.CoreApplication.Episodio == null)
                {
                    CoreStatics.CoreApplication.MovAllegatoSelezionato = new MovAllegato(CoreStatics.CoreApplication.Paziente.ID,
                                                                                         "NULL",
                                                                                         "NULL",
                                                                                         MovAllegato.FORMATO_ELETTRONICO);

                    CoreStatics.CoreApplication.MovAllegatoSelezionato.CodUA = CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata;
                }
                else
                {
                    CoreStatics.CoreApplication.MovAllegatoSelezionato = new MovAllegato(CoreStatics.CoreApplication.Paziente.ID,
                                                                                         CoreStatics.CoreApplication.Episodio.ID,
                                                                                         CoreStatics.CoreApplication.Trasferimento.ID,
                                                                                         MovAllegato.FORMATO_ELETTRONICO);

                    CoreStatics.CoreApplication.MovAllegatoSelezionato.CodUA = CoreStatics.CoreApplication.Trasferimento.CodUA;
                }

                CoreStatics.CoreApplication.MovAllegatoSelezionato.Azione = EnumAzioni.INS;
                CoreStatics.CoreApplication.MovAllegatoSelezionato.DataEvento = DateTime.Now;

                if (this.ucEasyTabControlAllegati.ActiveTab.Key == "Folder" && this.tvFolder.ActiveNode.Tag.ToString() != CoreStatics.TV_ROOT)
                {
                    CoreStatics.CoreApplication.MovAllegatoSelezionato.CodEntita = this.tvFolder.ActiveNode.Tag.ToString();
                    CoreStatics.CoreApplication.MovAllegatoSelezionato.IDFolder = this.tvFolder.ActiveNode.Key;
                    CoreStatics.CoreApplication.MovAllegatoSelezionato.Folder = this.tvFolder.ActiveNode.Text;
                }

                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneTipoAllegato, false) == DialogResult.OK)
                {
                    if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.AllegatiInserisciElettronico, false) == DialogResult.OK)
                    {
                        this.InizializzaFiltri();
                        AggiornaGriglia(true);
                        if (CoreStatics.CoreApplication.MovAllegatoSelezionato != null)
                            CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "ID", CoreStatics.CoreApplication.MovAllegatoSelezionato.IDMovAllegato);

                        if (this.ucEasyTabControlAllegati.ActiveTab.Key == "Folder")
                        {
                            string skey = this.tvFolder.ActiveNode.Key;
                            this.CaricaFolder(this.tvFolder, "ADD");
                            this.tvFolder.ActiveNode = this.tvFolder.GetNodeByKey(skey);
                            this.tvFolder.ActiveNode.Selected = true;
                            this.CaricaAllegati(this.tvFolder.ActiveNode);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ubAddElettronico_Click", this.Name);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
            }

            CoreStatics.SetNavigazione(true);
        }

        private void ubAddVirtuale_Click(object sender, EventArgs e)
        {

            try
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);


                if (CoreStatics.CoreApplication.Episodio == null)
                {
                    CoreStatics.CoreApplication.MovAllegatoSelezionato = new MovAllegato(CoreStatics.CoreApplication.Paziente.ID,
                                                                                         "NULL",
                                                                                         "NULL",
                                                                                         MovAllegato.FORMATO_VIRTUALE);

                    CoreStatics.CoreApplication.MovAllegatoSelezionato.CodUA = CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata;
                }
                else
                {
                    CoreStatics.CoreApplication.MovAllegatoSelezionato = new MovAllegato(CoreStatics.CoreApplication.Paziente.ID,
                                                                                        CoreStatics.CoreApplication.Episodio.ID,
                                                                                        CoreStatics.CoreApplication.Trasferimento.ID,
                                                                                        MovAllegato.FORMATO_VIRTUALE);

                    CoreStatics.CoreApplication.MovAllegatoSelezionato.CodUA = CoreStatics.CoreApplication.Trasferimento.CodUA;
                }

                CoreStatics.CoreApplication.MovAllegatoSelezionato.Azione = EnumAzioni.INS;
                CoreStatics.CoreApplication.MovAllegatoSelezionato.DataEvento = DateTime.Now;

                if (this.ucEasyTabControlAllegati.ActiveTab.Key == "Folder" && this.tvFolder.ActiveNode.Tag.ToString() != CoreStatics.TV_ROOT)
                {
                    CoreStatics.CoreApplication.MovAllegatoSelezionato.CodEntita = this.tvFolder.ActiveNode.Tag.ToString();
                    CoreStatics.CoreApplication.MovAllegatoSelezionato.IDFolder = this.tvFolder.ActiveNode.Key;
                    CoreStatics.CoreApplication.MovAllegatoSelezionato.Folder = this.tvFolder.ActiveNode.Text;
                }

                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneTipoAllegato, false) == DialogResult.OK)
                {

                    CoreStatics.CoreApplication.MovAllegatoSelezionato.IDDocumento = CoreStatics.GetNuovoIDDocumento().ToString("0000000000");

                    if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.AllegatiInserisciVirtuale, false) == DialogResult.OK)
                    {
                        this.InizializzaFiltri();
                        AggiornaGriglia(true);
                        if (CoreStatics.CoreApplication.MovAllegatoSelezionato != null)
                            CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "ID", CoreStatics.CoreApplication.MovAllegatoSelezionato.IDMovAllegato);

                        if (this.ucEasyTabControlAllegati.ActiveTab.Key == "Folder")
                        {
                            string skey = this.tvFolder.ActiveNode.Key;
                            this.CaricaFolder(this.tvFolder, "ADD");
                            this.tvFolder.ActiveNode = this.tvFolder.GetNodeByKey(skey);
                            this.tvFolder.ActiveNode.Selected = true;
                            this.CaricaAllegati(this.tvFolder.ActiveNode);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ubAddVirtuale_Click", this.Name);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
            }


            CoreStatics.SetNavigazione(true);
        }

        private void ubAddFolder_Click(object sender, EventArgs e)
        {

            try
            {

                CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerFolderAdd);
                _ucEasyPopUpFolder = new ucEasyPopUpFolder();
                _ucEasyPopUpFolder.Tag = "ADD";
                _ucEasyPopUpFolder.Init();
                int iWidthAdd = Convert.ToInt32(easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Medium)) * 40;
                int iHeightAdd = Convert.ToInt32((double)iWidthAdd / 1.52D);
                _ucEasyPopUpFolder.Size = new Size(iWidthAdd, iHeightAdd);
                _ucEasyPopUpFolder.tvFolder.TextFontRelativeDimension = easyStatics.easyRelativeDimensions.Medium;
                this.CaricaFolder(_ucEasyPopUpFolder.tvFolder, "ADD");
                UltraTreeNode oNode = _ucEasyPopUpFolder.tvFolder.GetNodeByKey(this.tvFolder.ActiveNode.Key);
                _ucEasyPopUpFolder.tvFolder.ActiveNode = oNode;
                _ucEasyPopUpFolder.tvFolder.ActiveNode.Selected = true;
                _ucEasyPopUpFolder.Focus();
                this.UltraPopupControlContainerFolderAdd.Show();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ubAddFolder_Click", this.Name);
            }

        }

        private void ucAllegati_Enter(object sender, EventArgs e)
        {
            try
            {
                this.ucEasyGrid.PerformLayout();
            }
            catch (Exception)
            {
            }
        }

        private void utxtIDDocumento_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.AggiornaGriglia(false);
        }

        #endregion

        #region UltraPopupControlContainer

        private void UltraPopupControlContainer_Closed(object sender, EventArgs e)
        {
            _ucRichTextBox.RtfClick -= ucRichTextBox_Click;
        }

        private void UltraPopupControlContainer_Opened(object sender, EventArgs e)
        {
            _ucRichTextBox.RtfClick += ucRichTextBox_Click;
        }

        private void UltraPopupControlContainer_Opening(object sender, CancelEventArgs e)
        {
            Infragistics.Win.Misc.UltraPopupControlContainer popup = sender as Infragistics.Win.Misc.UltraPopupControlContainer;
            popup.PopupControl = _ucRichTextBox;
        }

        private void ucRichTextBox_Click(object sender, EventArgs e)
        {
        }

        #endregion

        #region UltraPopupControlContainerFolderAdd

        private void UltraPopupControlContainerFolderAdd_Closed(object sender, EventArgs e)
        {
            _ucEasyPopUpFolder.ucEasyButtonCancel.Click -= UcEasyButtonCancel_Click;
            _ucEasyPopUpFolder.ucEasyButtonConferma.Click -= UcEasyButtonConferma_Click;
        }

        private void UltraPopupControlContainerFolderAdd_Opened(object sender, EventArgs e)
        {
            _ucEasyPopUpFolder.ucEasyButtonCancel.Click += UcEasyButtonCancel_Click;
            _ucEasyPopUpFolder.ucEasyButtonConferma.Click += UcEasyButtonConferma_Click;
            _ucEasyPopUpFolder.Focus();
        }

        private void UltraPopupControlContainerFolderAdd_Opening(object sender, CancelEventArgs e)
        {
            UltraPopupControlContainer popup = sender as UltraPopupControlContainer;
            popup.PopupControl = _ucEasyPopUpFolder;
        }

        private void UcEasyButtonConferma_Click(object sender, EventArgs e)
        {
            try
            {
                string sIDEpisodio = string.Empty;
                string sCodUA = string.Empty;

                if (_ucEasyPopUpFolder.txtFolder.Text != string.Empty)
                {

                    if (_ucEasyPopUpFolder.Tag.ToString() == "ADD")
                    {

                        if (CoreStatics.CoreApplication.Trasferimento == null)
                        {
                            sIDEpisodio = string.Empty;
                            sCodUA = CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata;
                        }
                        else
                        {
                            sIDEpisodio = (CoreStatics.CoreApplication.Episodio != null ? CoreStatics.CoreApplication.Episodio.ID : "");
                            sCodUA = (CoreStatics.CoreApplication.Trasferimento != null ? CoreStatics.CoreApplication.Trasferimento.CodUA : "");
                        }

                        MovFolder oMovFolder = new MovFolder(CoreStatics.CoreApplication.Paziente.ID,
                                                                sIDEpisodio,
                                                                "",
                                                                (_ucEasyPopUpFolder.tvFolder.ActiveNode.Key == CoreStatics.TV_ROOT ? "" : _ucEasyPopUpFolder.tvFolder.ActiveNode.Key));
                        oMovFolder.CodStatoFolder = "AT";
                        oMovFolder.Descrizione = _ucEasyPopUpFolder.txtFolder.Text;
                        oMovFolder.CodUA = sCodUA;
                        oMovFolder.CodEntita = _ucEasyPopUpFolder.uceCodEntita.Value.ToString();
                        oMovFolder.Salva();

                        this.CaricaFolder(this.tvFolder, "ADD");

                        UltraTreeNode oNode = this.tvFolder.GetNodeByKey(oMovFolder.IDMovFolder);
                        this.tvFolder.ActiveNode = oNode;
                        this.tvFolder.ActiveNode.Selected = true;
                        oMovFolder = null;
                    }
                    else if (_ucEasyPopUpFolder.Tag.ToString() == "EDIT")
                    {
                        MovFolder oMovFolder = new MovFolder(this.tvFolder.ActiveNode.Key);
                        oMovFolder.Descrizione = _ucEasyPopUpFolder.txtFolder.Text;

                        if (_ucEasyPopUpFolder.tvFolder.ActiveNode.Key != CoreStatics.TV_ROOT)
                        {
                            if (_ucEasyPopUpFolder.tvFolder.ActiveNode.Key != oMovFolder.IDMovFolder)
                            {
                                oMovFolder.IDFolderPadre = _ucEasyPopUpFolder.tvFolder.ActiveNode.Key;
                            }
                        }
                        else
                        {
                            oMovFolder.IDFolderPadre = "";
                        }

                        oMovFolder.Salva();
                        this.CaricaFolder(this.tvFolder, "ADD");
                        UltraTreeNode oNode = this.tvFolder.GetNodeByKey(oMovFolder.IDMovFolder);
                        this.tvFolder.ActiveNode = oNode;
                        this.tvFolder.ActiveNode.Selected = true;
                        oMovFolder = null;
                    }

                    this.UltraPopupControlContainerFolderAdd.Close();

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "UcEasyButtonConferma_Click", this.Text);
            }

        }

        private void UcEasyButtonCancel_Click(object sender, EventArgs e)
        {
            this.UltraPopupControlContainerFolderAdd.Close();
        }

        #endregion

    }
}
