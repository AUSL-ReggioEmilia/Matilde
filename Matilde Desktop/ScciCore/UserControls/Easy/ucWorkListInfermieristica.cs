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

using UnicodeSrl.Scci.PluginClient;
using System.Threading;
using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.DatiClinici.DC;
using UnicodeSrl.ScciCore.ViewController;
using UnicodeSrl.ScciCore.Common.Extensions;

namespace UnicodeSrl.ScciCore
{
    public partial class ucWorkListInfermieristica : UserControl, Interfacce.IViewUserControlMiddle
    {
        private UserControl _ucc = null;
        private ucRichTextBox _ucRichTextBox = null;
        private string _popuporarioID = "";

        private Dictionary<int, byte[]> oIcone = new Dictionary<int, byte[]>();
        private Dictionary<int, byte[]> oIconeVS = new Dictionary<int, byte[]>();

        private Gestore oGestore = null;

        private bool _runtimecheck = false;

        public ucWorkListInfermieristica()
        {
            InitializeComponent();

            _runtimecheck = false;
            _ucc = (UserControl)this;
        }

        #region Interface

        public void Aggiorna()
        {
            if (this.IsDisposed == false)
            {
                VerificaSicurezza();
                string s = string.Empty;
                if (this.ucEasyGrid.ActiveRow != null) { s = this.ucEasyGrid.ActiveRow.Cells["ID"].Text; }

                CoreStatics.GCCollectUltraGrid(ref this.ucEasyGrid);

                this.AggiornaGriglia(true);
                if (s != string.Empty) { CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "ID", s); }
            }

        }

        public void Carica()
        {
            try
            {
                InizializzaControlli();
                InizializzaUltraGridLayout();
                VerificaSicurezza();
                InizializzaFiltri();

                this.CaricaGriglia(true);

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
                oIconeVS = new Dictionary<int, byte[]>();

                CoreStatics.SetContesto(EnumEntita.WKI, null);

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
                CoreStatics.SetEasyUltraDockManager(ref this.ultraDockManager);

                this.ubAdd.Appearance.Image = Properties.Resources.Aggiungi_256;
                this.ubSpostaTask.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_SPOSTA_IN_ALTRA_CARTELLA_256);
                this.ubAdmin.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_CONFIGURAZIONE_256);
                this.uchkFiltro.UNCheckedImage = Risorse.GetImageFromResource(Risorse.GC_FILTRO_256);
                this.uchkFiltro.CheckedImage = Risorse.GetImageFromResource(Risorse.GC_FILTROAPPLICATO_256);
                this.uchkFiltro.Checked = false;

                this.ubAdd.PercImageFill = 0.75F;
                this.ubAdd.ShortcutKey = Keys.Add;
                this.ubAdd.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.XSmall;
                this.ubAdd.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.ubSpostaTask.PercImageFill = 0.75F;
                this.ubSpostaTask.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.ubAdmin.PercImageFill = 0.75F;
                this.ubAdmin.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.uchkFiltro.PercImageFill = 0.75F;
                this.uchkFiltro.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.XSmall;
                this.uchkFiltro.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.ucEasyTreeViewVie.Override.Multiline = Infragistics.Win.DefaultableBoolean.True;
                this.ucEasyTreeViewVie.PerformLayout();

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
                if (CoreStatics.CoreApplication.Cartella.CartellaChiusa == true)
                {
                    this.ubAdd.Enabled = false;
                    this.ubSpostaTask.Enabled = false;
                    this.ubAdmin.Visible = false;
                }
                else
                {
                    this.ubAdd.Enabled = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.WorkL_Inserisci);
                    this.ubAdmin.Visible = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.WorkL_Admin);
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "VerificaSicurezza", this.Name);
            }
        }

        private void InizializzaFiltri()
        {
            if (this.IsDisposed == false)
            {
                try
                {
                    this.ubApplicaFiltro.TextFontRelativeDimension = easyStatics.easyRelativeDimensions.Large;
                    this.drFiltro.Value = null;
                    this.drFiltro.DateFuture = true;
                    this.udteFiltroDA.Value = null;
                    this.udteFiltroA.Value = null;
                    this.utxtFiltroContenuto.Text = "";

                    if (this.ucEasyTreeViewTipo.Nodes.Count > 0)
                    {
                        foreach (Infragistics.Win.UltraWinTree.UltraTreeNode oNode in this.ucEasyTreeViewTipo.Nodes[CoreStatics.GC_TUTTI].Nodes)
                        {
                            if (oNode.Override.NodeStyle == Infragistics.Win.UltraWinTree.NodeStyle.CheckBox)
                            {
                                oNode.CheckedState = CheckState.Checked;
                            }
                        }
                    }
                    if (this.ucEasyGridFiltroStato.Rows.Count > 0)
                    {
                        this.ucEasyGridFiltroStato.ActiveRow = null;
                        this.ucEasyGridFiltroStato.Selected.Rows.Clear();
                        CoreStatics.SelezionaRigaInGriglia(ref ucEasyGridFiltroStato, "CodStato", CoreStatics.GC_TUTTI);
                    }

                    if (this.ucEasyTreeViewVie.Nodes.Count > 0)
                    {
                        foreach (Infragistics.Win.UltraWinTree.UltraTreeNode oNode in this.ucEasyTreeViewVie.Nodes[CoreStatics.GC_TUTTI].Nodes)
                        {
                            if (oNode.Override.NodeStyle == Infragistics.Win.UltraWinTree.NodeStyle.CheckBox)
                            {
                                oNode.CheckedState = CheckState.Checked;
                            }
                        }
                    }

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

                CoreStatics.CoreApplication.IDTaskInfermieristicoSelezionato = "";
                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDEpisodio", CoreStatics.CoreApplication.Episodio.ID);
                if (datiEstesi)
                    op.Parametro.Add("DatiEstesi", "1");
                else
                    op.Parametro.Add("DatiEstesi", "0");

                if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata != null)
                {
                    op.Parametro.Add("CodSistema", EnumSistemi.PRF.ToString());
                    op.Parametro.Add("IDSistema", CoreStatics.CoreApplication.MovPrescrizioneSelezionata.IDPrescrizione);
                    op.Parametro.Add("Ordinamento", "M.DataProgrammata");
                }

                op.TimeStamp.CodEntita = EnumEntita.WKI.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_SelMovTaskInfermieristici", spcoll);

                DataTable dtEdit = ds.Tables[0].Copy();

                DataColumn colsp = new DataColumn(CoreStatics.C_COL_SPAZIO, typeof(string));
                colsp.AllowDBNull = true;
                colsp.DefaultValue = "";
                colsp.Unique = false;
                dtEdit.Columns.Add(colsp);

                foreach (DataColumn dcCol in dtEdit.Columns)
                {
                    if (dcCol.ColumnName.ToUpper().IndexOf("PERMESSOMODIFICA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOEROGAZIONE") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOANNULLA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOCANCELLA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOCOPIA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOMODIFICAORARIO") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("ICON") >= 0)
                        dcCol.ReadOnly = false;
                }
                if (this.ucEasyGrid.DisplayLayout != null)
                {
                    this.ucEasyGrid.DataSource = null;

                    if (datiEstesi)
                    {
                        this.ucEasyTreeViewTipo.Nodes.Clear();

                        Infragistics.Win.UltraWinTree.UltraTreeNode oNodeRoot = new Infragistics.Win.UltraWinTree.UltraTreeNode(CoreStatics.GC_TUTTI, "Tipo Task");
                        oNodeRoot.Override.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBox;
                        oNodeRoot.CheckedState = CheckState.Checked;
                        foreach (DataRow oDr in ds.Tables[1].Rows)
                        {
                            Infragistics.Win.UltraWinTree.UltraTreeNode oNode = new Infragistics.Win.UltraWinTree.UltraTreeNode(oDr["CodTipo"].ToString(), oDr["DescTipo"].ToString());
                            oNode.Override.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBox;
                            oNode.CheckedState = CheckState.Checked;
                            if (ds.Tables[1].Columns.Contains("Icona") && oDr["Icona"] != null)
                            {
                                oNode.LeftImages.Add(CoreStatics.ByteToImage((byte[])oDr["Icona"]));
                            }
                            oNodeRoot.Nodes.Add(oNode);
                        }
                        this.ucEasyTreeViewTipo.Nodes.Add(oNodeRoot);
                        this.ucEasyTreeViewTipo.ExpandAll();

                        this.ucEasyGridFiltroStato.DataSource = CoreStatics.AggiungiTuttiDataTable(ds.Tables[2], true);
                        this.ucEasyGridFiltroStato.Refresh();

                        this.ucEasyTreeViewVie.Nodes.Clear();

                        oNodeRoot = new Infragistics.Win.UltraWinTree.UltraTreeNode(CoreStatics.GC_TUTTI, "Vie di Somministrazione");
                        oNodeRoot.Override.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBox;
                        oNodeRoot.CheckedState = CheckState.Checked;
                        foreach (DataRow oDr in ds.Tables[3].Rows)
                        {
                            Infragistics.Win.UltraWinTree.UltraTreeNode oNode = new Infragistics.Win.UltraWinTree.UltraTreeNode(oDr["Codice"].ToString(), oDr["Descrizione"].ToString());
                            oNode.Override.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBox;
                            oNode.CheckedState = CheckState.Checked;
                            if (oDr["Icona"] != null)
                            {
                                oNode.LeftImages.Add(CoreStatics.ByteToImage((byte[])oDr["Icona"]));
                            }
                            oNodeRoot.Nodes.Add(oNode);
                        }
                        this.ucEasyTreeViewVie.Nodes.Add(oNodeRoot);
                        this.ucEasyTreeViewVie.ExpandAll();

                    }

                    if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata != null)
                    {
                        this.ucEasyGrid.DataSource = dtEdit;
                        this.ucEasyGrid.Refresh();
                        this.ucEasyGrid.PerformAction(UltraGridAction.FirstRowInBand, false, false);

                        if (this.ucEasyGrid.Rows.Count > 0)
                            this.ucEasyGrid.ActiveRow = this.ucEasyGrid.Rows[0];
                        else
                            this.ucEasyGrid.ActiveRow = null;

                        this.uchkFiltro.Checked = true;
                    }
                    else
                    {

                        if (this.ucEasyGridFiltroStato.Rows.Count > 0)
                        {
                            this.ucEasyGridFiltroStato.ActiveRow = null;
                            this.ucEasyGridFiltroStato.Selected.Rows.Clear();
                            CoreStatics.SelezionaRigaInGriglia(ref ucEasyGridFiltroStato, "CodStato", EnumStatoTaskInfermieristico.PR.ToString());
                        }
                        ubApplicaFiltro_Click(this.ubApplicaFiltro, new EventArgs());
                    }

                    if (CoreStatics.CoreApplication.Cartella != null && CoreStatics.CoreApplication.Cartella.CartellaChiusa == true)
                    {
                        this.ubSpostaTask.Enabled = false;
                    }
                    else
                    {
                        this.ubSpostaTask.Enabled = (this.ucEasyGrid.ActiveRow != null && (int.Parse(ucEasyGrid.ActiveRow.Cells["PERMESSOSPOSTA"].Value.ToString()) == 1));
                    }

                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaGriglia", this.Name);
            }
        }

        private void AggiornaGriglia(bool datiEstesi)
        {

            string codtiposelezionato = string.Empty;
            string codstatoselezionato = string.Empty;
            CoreStatics.CoreApplication.IDTaskInfermieristicoSelezionato = "";

            CoreStatics.SetNavigazione(false);

            try
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);
                CoreStatics.SetContesto(EnumEntita.WKI, null);

                bool bFiltro = false;

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDEpisodio", CoreStatics.CoreApplication.Episodio.ID);
                if (datiEstesi)
                    op.Parametro.Add("DatiEstesi", "1");
                else
                    op.Parametro.Add("DatiEstesi", "0");

                if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata != null)
                {
                    op.Parametro.Add("CodSistema", EnumSistemi.PRF.ToString());
                    op.Parametro.Add("IDSistema", CoreStatics.CoreApplication.MovPrescrizioneSelezionata.IDPrescrizione);
                    op.Parametro.Add("Ordinamento", "M.DataProgrammata");
                    bFiltro = true;
                }

                if (this.udteFiltroDA.Value != null)
                {
                    op.Parametro.Add("DataProgrammataInizio", ((DateTime)this.udteFiltroDA.Value).ToString("dd/MM/yyyy HH:mm").Replace(".", ":"));
                    bFiltro = true;
                }
                if (this.udteFiltroA.Value != null)
                {

                    if (this.udteFiltroDA.Value == null)
                    {
                        op.Parametro.Add("DataProgrammataInizio", CoreStatics.GC_DATAINIZIO);
                    }

                    op.Parametro.Add("DataProgrammataFine", ((DateTime)this.udteFiltroA.Value).ToString("dd/MM/yyyy HH:mm").Replace(".", ":"));
                    bFiltro = true;
                }

                Dictionary<string, string> listatipo = new Dictionary<string, string>();
                if (this.ucEasyTreeViewTipo.Nodes.Exists(CoreStatics.GC_TUTTI) && this.ucEasyTreeViewTipo.Nodes[CoreStatics.GC_TUTTI].CheckedState != CheckState.Checked)
                {
                    foreach (Infragistics.Win.UltraWinTree.UltraTreeNode oNode in this.ucEasyTreeViewTipo.Nodes[CoreStatics.GC_TUTTI].Nodes)
                    {
                        if (oNode.Override.NodeStyle == Infragistics.Win.UltraWinTree.NodeStyle.CheckBox && oNode.CheckedState == CheckState.Checked)
                        {
                            listatipo.Add(oNode.Key, oNode.Text);
                        }
                        else
                        {
                            bFiltro = true;
                        }
                    }
                }
                string[] codtipo = listatipo.Keys.ToArray();
                op.ParametroRipetibile.Add("CodTipoTaskInfermieristico", codtipo);

                if (this.ucEasyGridFiltroStato.ActiveRow != null && this.ucEasyGridFiltroStato.ActiveRow.Cells["CodStato"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                {
                    op.Parametro.Add("CodStatoTaskInfermieristico", this.ucEasyGridFiltroStato.ActiveRow.Cells["CodStato"].Text);
                    bFiltro = true;
                }

                op.TimeStamp.CodEntita = EnumEntita.WKI.ToString(); op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                Dictionary<string, string> listavie = new Dictionary<string, string>();
                if (this.ucEasyTreeViewVie.Nodes.Exists(CoreStatics.GC_TUTTI) && this.ucEasyTreeViewVie.Nodes[CoreStatics.GC_TUTTI].CheckedState != CheckState.Checked)
                {
                    foreach (Infragistics.Win.UltraWinTree.UltraTreeNode oNode in this.ucEasyTreeViewVie.Nodes[CoreStatics.GC_TUTTI].Nodes)
                    {
                        if (oNode.Override.NodeStyle == Infragistics.Win.UltraWinTree.NodeStyle.CheckBox && oNode.CheckedState == CheckState.Checked)
                        {
                            listavie.Add(oNode.Key, oNode.Text);
                        }
                        else
                        {
                            bFiltro = true;
                        }
                    }
                }
                string[] codvie = listavie.Keys.ToArray();

                op.ParametroRipetibile.Add("ViaSomministrazione", codvie);

                if (this.utxtFiltroContenuto.Text.Trim() != "")
                {
                    op.Parametro.Add("Descrizione", this.utxtFiltroContenuto.Text);
                    bFiltro = true;
                }


                this.uchkFiltro.Checked = bFiltro;

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_SelMovTaskInfermieristici", spcoll);

                DataTable dtEdit = ds.Tables[0].Copy();

                DataColumn colsp = new DataColumn(CoreStatics.C_COL_SPAZIO, typeof(string));
                colsp.AllowDBNull = true;
                colsp.DefaultValue = "";
                colsp.Unique = false;
                dtEdit.Columns.Add(colsp);

                foreach (DataColumn dcCol in dtEdit.Columns)
                {
                    if (dcCol.ColumnName.ToUpper().IndexOf("PERMESSOBLOCCATO") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOMODIFICA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOEROGAZIONE") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOCOPIA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOMODIFICAORARIO") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("ICONA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("ICONAVS") == 0)
                        dcCol.ReadOnly = false;
                }
                if (this.ucEasyGrid.DisplayLayout != null)
                {
                    if (this.ucEasyGrid.DisplayLayout.Bands.Count > 0) { this.ucEasyGrid.DataSource = null; }
                    this.ucEasyGrid.DataSource = dtEdit;
                    this.ucEasyGrid.Refresh();

                    if (this.ucEasyGrid.Rows.Count > 0)
                        this.ucEasyGrid.ActiveRow = this.ucEasyGrid.Rows[0];
                    else
                        this.ucEasyGrid.ActiveRow = null;

                    if (CoreStatics.CoreApplication.Cartella != null && CoreStatics.CoreApplication.Cartella.CartellaChiusa == true)
                    {
                        this.ubSpostaTask.Enabled = false;
                    }
                    else
                    {
                        this.ubSpostaTask.Enabled = (this.ucEasyGrid.ActiveRow != null && (int.Parse(ucEasyGrid.ActiveRow.Cells["PERMESSOSPOSTA"].Value.ToString()) == 1));
                    }

                    if (datiEstesi)
                    {

                        this.ucEasyTreeViewTipo.Nodes.Clear();

                        Infragistics.Win.UltraWinTree.UltraTreeNode oNodeRoot = new Infragistics.Win.UltraWinTree.UltraTreeNode(CoreStatics.GC_TUTTI, "Tipo Task");
                        oNodeRoot.Override.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBox;
                        oNodeRoot.CheckedState = CheckState.Checked;
                        foreach (DataRow oDr in ds.Tables[1].Rows)
                        {
                            Infragistics.Win.UltraWinTree.UltraTreeNode oNode = new Infragistics.Win.UltraWinTree.UltraTreeNode(oDr["CodTipo"].ToString(), oDr["DescTipo"].ToString());
                            oNode.Override.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBox;
                            oNode.CheckedState = CheckState.Checked;
                            if (ds.Tables[1].Columns.Contains("Icona") && oDr["Icona"] != null)
                            {
                                oNode.LeftImages.Add(CoreStatics.ByteToImage((byte[])oDr["Icona"]));
                            }
                            oNodeRoot.Nodes.Add(oNode);
                        }
                        this.ucEasyTreeViewTipo.Nodes.Add(oNodeRoot);
                        this.ucEasyTreeViewTipo.ExpandAll();

                        if (this.ucEasyGridFiltroStato.ActiveRow != null && this.ucEasyGridFiltroStato.ActiveRow.Cells["CodStato"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                            codstatoselezionato = this.ucEasyGridFiltroStato.ActiveRow.Cells["CodStato"].Text;
                        if (this.ucEasyGridFiltroStato.DisplayLayout.Bands.Count > 0) { this.ucEasyGridFiltroStato.DisplayLayout.Bands[0].Columns.ClearUnbound(); }
                        this.ucEasyGridFiltroStato.DataSource = CoreStatics.AggiungiTuttiDataTable(ds.Tables[2], true);
                        this.ucEasyGridFiltroStato.Refresh();
                        if (codstatoselezionato != null && codstatoselezionato.Contains(CoreStatics.GC_TUTTI) != true)
                            CoreStatics.SelezionaRigaInGriglia(ref ucEasyGridFiltroStato, "CodStato", codstatoselezionato);

                        this.ucEasyTreeViewVie.Nodes.Clear();

                        oNodeRoot = new Infragistics.Win.UltraWinTree.UltraTreeNode(CoreStatics.GC_TUTTI, "Vie di Somministrazione");
                        oNodeRoot.Override.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBox;
                        oNodeRoot.CheckedState = CheckState.Checked;
                        foreach (DataRow oDr in ds.Tables[3].Rows)
                        {
                            Infragistics.Win.UltraWinTree.UltraTreeNode oNode = new Infragistics.Win.UltraWinTree.UltraTreeNode(oDr["Codice"].ToString(), oDr["Descrizione"].ToString());
                            oNode.Override.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBox;
                            oNode.CheckedState = CheckState.Checked;
                            if (oDr["Icona"] != null)
                            {
                                oNode.LeftImages.Add(CoreStatics.ByteToImage((byte[])oDr["Icona"]));
                            }
                            oNodeRoot.Nodes.Add(oNode);
                        }
                        this.ucEasyTreeViewVie.Nodes.Add(oNodeRoot);
                        this.ucEasyTreeViewVie.ExpandAll();

                    }
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

        private void CancellaGruppo(string codsistema, string idsistema, string idgruppo)
        {
            if (easyStatics.EasyMessageBox("Confermi la cancellazione di TUTTO IL GRUPPO del task selezionato ?", "Cancellazione Task Infermieristici",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDGruppo", idgruppo);

                op.TimeStamp.CodEntita = EnumEntita.WKI.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.CAN.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovTaskInfermieristiciGruppo", spcoll);

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                        this.CancellaSingolo(row["ID"].ToString(), false);
                }

                this.AggiornaGriglia(true);
            }
        }

        private void CancellaSingolo(string idsingolotask, bool elabinterattiva)
        {
            bool bCancella = true;

            if (elabinterattiva)
            {
                bCancella = false;
                if (easyStatics.EasyMessageBox("Confermi la cancellazione del task selezionato ?", "Cancellazione Task Infermieristici",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    bCancella = true;
                }
            }

            if (bCancella)
            {
                CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = new MovTaskInfermieristico(idsingolotask, EnumAzioni.CAN, CoreStatics.CoreApplication.Ambiente);

                Risposta oRispostaElaboraPrima = PluginClientStatics.PluginClient(EnumPluginClient.WKI_CANCELLA_PRIMA.ToString(), new object[1] { "CANCELLAZIONE" }, CommonStatics.UAPadri(CoreStatics.CoreApplication.Trasferimento.CodUA, CoreStatics.CoreApplication.Ambiente));
                if (oRispostaElaboraPrima.Successo)
                {
                    if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Cancella() && elabinterattiva)
                    {
                        this.AggiornaGriglia(true);
                    }
                }

                CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = null;
            }
        }

        #endregion

        #region Erogazione Rapida

        private bool ErogazioneRapida(string id)
        {

            try
            {

                CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.WaitCursor);

                CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = new MovTaskInfermieristico(id, EnumAzioni.VAL, CoreStatics.CoreApplication.Ambiente);

                bool result = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.ErogaRapida(CoreStatics.CoreApplication.Trasferimento.CodUA, CoreStatics.CoreApplication.Ambiente, true);

                return true;

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ErogazioneRapida", this.Name);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);
            }

            return false;
        }

        #endregion

        private bool Eroga(string id)
        {
            try
            {

                CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = new MovTaskInfermieristico(id, EnumAzioni.VAL, CoreStatics.CoreApplication.Ambiente);

                CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.WaitCursor);

                bool result = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Eroga(CoreStatics.CoreApplication.Trasferimento.CodUA, CoreStatics.CoreApplication.Ambiente, true);

                return result;

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Eroga Task", this.Name);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);
            }

            return false;
        }

        #region Events

        private void ultraDockManager_InitializePane(object sender, Infragistics.Win.UltraWinDock.InitializePaneEventArgs e)
        {
            e.Pane.Settings.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large);
            e.Pane.Settings.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FILTRO_32);

            int filtroWidth = 40 * (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large);
            this.ultraDockManager.ControlPanes[0].FlyoutSize = new Size(filtroWidth, this.ultraDockManager.ControlPanes[0].FlyoutSize.Height);
            this.ultraDockManager.ControlPanes[0].Size = new Size(filtroWidth, this.ultraDockManager.ControlPanes[0].Size.Height);
            this.ultraDockManager.DockAreas[0].Size = new Size(filtroWidth, this.ultraDockManager.DockAreas[0].Size.Height);
            this.pnlFiltro.Width = filtroWidth;
        }

        private void ucEasyGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                int refWidth = (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XLarge) * 3;

                Graphics g = this.CreateGraphics();
                int refBtnWidth = Convert.ToInt32(CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(this.ucEasyGrid.DataRowFontRelativeDimension), g.DpiY) * 3);
                int iBtn2 = refBtnWidth; int iSpz = Convert.ToInt32(refBtnWidth * 0.15);
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
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                                oCol.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellAppearance.ImageVAlign = Infragistics.Win.VAlign.Top;
                                oCol.VertScrollBar = false;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
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
                                oCol.RowLayoutColumnInfo.SpanY = 4;

                                break;

                            case "DataProgrammata":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.ForeColor = Color.Red;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.Format = "dd/MM/yyyy HH:mm";
                                try
                                {
                                    oCol.MaxWidth = refWidth + iBtn2;
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


                            case "DescrUtente":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.ForeColor = Color.Red;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = refWidth + iBtn2;
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

                            case "DataUltimaModifica":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.FontData.Italic = Infragistics.Win.DefaultableBoolean.True;
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.Format = "dd/MM/yyyy HH:mm";
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = refWidth + iBtn2;
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

                            case "DescrUtenteUltimaModifica":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.FontData.Italic = Infragistics.Win.DefaultableBoolean.True;
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = refWidth + iBtn2;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 3;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;
                                break;







                            case "DescTipo":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.LockedWidth = true;
                                try
                                {
                                    oCol.MaxWidth = refWidth + 6;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 2;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 2;

                                break;

                            case "IconaVS":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                                oCol.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellAppearance.ImageVAlign = Infragistics.Win.VAlign.Top;
                                oCol.VertScrollBar = false;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.LockedWidth = true;
                                try
                                {
                                    oCol.MaxWidth = refWidth + 6;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 2;
                                oCol.RowLayoutColumnInfo.OriginY = 2;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 2;

                                break;

                            case "DescStatoEsteso":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
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

                                oCol.RowLayoutColumnInfo.OriginX = 3;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 4;

                                break;

                            case "AnteprimaRTF":
                                oCol.Hidden = false;

                                RichTextEditor a = new RichTextEditor();
                                oCol.Editor = a;
                                oCol.CellActivation = Activation.ActivateOnly;
                                oCol.CellClickAction = CellClickAction.CellSelect;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.VertScrollBar = true;
                                try
                                {
                                    oCol.MaxWidth = this.ucEasyGrid.Width - (refWidth * 4) - iBtn2 - Convert.ToInt32(refBtnWidth * 8.1) - Convert.ToInt32(iSpz * 7) - 24;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 4;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 4;

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
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_STATO))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_STATO);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_STATUSFLAGRED_32);


                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
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


                    colEdit.RowLayoutColumnInfo.OriginX = 5;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;
                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_STATO + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_STATO + @"_SP");
                    colEdit.Hidden = false;
                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                    colEdit.CellActivation = Activation.Disabled;
                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = iSpz;
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

                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_EDIT))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_EDIT);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_MODIFICA_32);


                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
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


                    colEdit.RowLayoutColumnInfo.OriginX = 7;
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
                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = iSpz;
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
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_ORARIO))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_ORARIO);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_MODIFICA_ORARIO_32);


                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
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


                    colEdit.RowLayoutColumnInfo.OriginX = 9;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;
                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_ORARIO + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_ORARIO + @"_SP");
                    colEdit.Hidden = false;
                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                    colEdit.CellActivation = Activation.Disabled;
                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = iSpz;
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

                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_EROG_RAPIDA))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_EROG_RAPIDA);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_EROGAZIONERAPIDA_32);


                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
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


                    colEdit.RowLayoutColumnInfo.OriginX = 11;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;
                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_EROG_RAPIDA + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_EROG_RAPIDA + @"_SP");
                    colEdit.Hidden = false;
                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                    colEdit.CellActivation = Activation.Disabled;
                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = iSpz;
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;
                    colEdit.RowLayoutColumnInfo.OriginX = 12;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;
                }


                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_EROG))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_EROG);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_SI_32);


                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
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


                    colEdit.RowLayoutColumnInfo.OriginX = 13;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;
                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_EROG + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_EROG + @"_SP");
                    colEdit.Hidden = false;
                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                    colEdit.CellActivation = Activation.Disabled;
                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = iSpz;
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;
                    colEdit.RowLayoutColumnInfo.OriginX = 14;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;
                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_ANNULLA))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_ANNULLA);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_NO_32);


                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
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


                    colEdit.RowLayoutColumnInfo.OriginX = 15;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;
                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_ANNULLA + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_ANNULLA + @"_SP");
                    colEdit.Hidden = false;
                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                    colEdit.CellActivation = Activation.Disabled;
                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = iSpz;
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;
                    colEdit.RowLayoutColumnInfo.OriginX = 16;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;
                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_DEL))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_DEL);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_ELIMINA_32);


                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
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


                    colEdit.RowLayoutColumnInfo.OriginX = 17;
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
                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = iSpz;
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;
                    colEdit.RowLayoutColumnInfo.OriginX = 18;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;
                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_COPY))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_COPY);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_COPIA_32);

                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
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


                    colEdit.RowLayoutColumnInfo.OriginX = 19;
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
                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
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
                    colEdit.RowLayoutColumnInfo.OriginX = 20;
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

                if (e.Row.Cells.Exists("IconaVS") == true && e.Row.Cells["IDIconaVS"].Value.ToString() != "")
                {
                    if (oIconeVS.ContainsKey(Convert.ToInt32(e.Row.Cells["IDIconaVS"].Value)) == false)
                    {
                        oIconeVS.Add(Convert.ToInt32(e.Row.Cells["IDIconaVS"].Value), CoreStatics.GetImageForGrid(Convert.ToInt32(e.Row.Cells["IDIconaVS"].Value), 48));
                    }
                    e.Row.Cells["IconaVS"].Value = oIconeVS[Convert.ToInt32(e.Row.Cells["IDIconaVS"].Value)];
                    e.Row.Update();
                }

                foreach (UltraGridCell ocell in e.Row.Cells)
                {
                    if (CoreStatics.CoreApplication.Cartella.CartellaChiusa == true)
                    {
                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_EDIT)
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_EROG_RAPIDA)
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_EROG)
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_ANNULLA)
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_DEL)
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_COPY)
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_ORARIO)
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_STATO)
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                    }
                    else
                    {
                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_EDIT)
                            if (ocell.Row.Cells["PermessoBloccato"].Value.ToString() == "1" || ocell.Row.Cells["PermessoModifica"].Value.ToString() == "0")
                                ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_EROG_RAPIDA && ocell.Row.Cells["PermessoErogazione"].Value.ToString() == "0")
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_EROG && ocell.Row.Cells["PermessoErogazione"].Value.ToString() == "0")
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_ANNULLA && ocell.Row.Cells["PermessoAnnulla"].Value.ToString() == "0")
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_DEL && ocell.Row.Cells["PermessoCancella"].Value.ToString() == "0")
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_COPY && ocell.Row.Cells["PermessoCopia"].Value.ToString() == "0")
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_ORARIO && ocell.Row.Cells["PermessoModificaOrario"].Value.ToString() == "0")
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_STATO && ocell.Row.Cells["NoteCalc"].Value.ToString() == string.Empty)
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                    }
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGrid_InitializeRow", this.Name);
            }
        }

        private void ucEasyGrid_ClickCellButton(object sender, CellEventArgs e)
        {

            try
            {

                if (Interlocked.Equals(Maschere._navigare, 0))
                {
                    return;
                }
                else
                {
                    CoreStatics.SetNavigazione(false);

                    switch (e.Cell.Column.Key)
                    {

                        case CoreStatics.C_COL_BTN_STATO:
                            CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainer);
                            _ucRichTextBox = CoreStatics.GetRichTextBoxForPopupControlContainer(e.Cell.Row.Cells["NoteCalc"].Text, false);
                            Infragistics.Win.UIElement uieS = e.Cell.GetUIElement();
                            Point oPointS = new Point(uieS.Rect.Left, uieS.Rect.Top);
                            this.UltraPopupControlContainer.Show(this.ucEasyGrid, oPointS);
                            break;

                        case CoreStatics.C_COL_BTN_EDIT:
                            if (e.Cell.Row.Cells["PermessoModifica"].Text == "1")
                            {

                                CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = new MovTaskInfermieristico(e.Cell.Row.Cells["ID"].Text, EnumAzioni.MOD, CoreStatics.CoreApplication.Ambiente);

                                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingTaskInfermieristici) == DialogResult.OK)
                                {
                                    this.AggiornaGriglia(true);
                                    if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato != null)
                                    {
                                        CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "ID", CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.IDMovTaskInfermieristico);
                                        CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = null;
                                    }
                                }

                            }

                            break;

                        case CoreStatics.C_COL_BTN_EROG_RAPIDA:
                            this.ErogazioneRapida(e.Cell.Row.Cells["ID"].Text);
                            this.AggiornaGriglia(false);

                            break;

                        case CoreStatics.C_COL_BTN_EROG:
                            bool result = this.Eroga(e.Cell.Row.Cells["ID"].Text);

                            if (result)
                                this.AggiornaGriglia(false);

                            break;

                        case CoreStatics.C_COL_BTN_ORARIO:
                            CoreStatics.SetUltraPopupControlContainer(this.ultraPopupControlContainerOra);

                            this.ucEasyPopUpOrario.DataOra = e.Cell.Row.Cells["DataProgrammata"].Value;
                            this.ucEasyPopUpOrario.Note = e.Cell.Row.Cells["Note"].Text;
                            _popuporarioID = e.Cell.Row.Cells["ID"].Text;

                            Infragistics.Win.UIElement uie = e.Cell.GetUIElement();
                            Point oPoint = new Point(uie.Rect.Left, uie.Rect.Top);

                            this.ultraPopupControlContainerOra.Show(this.ucEasyGrid, oPoint);

                            break;

                        case CoreStatics.C_COL_BTN_ANNULLA:
                            CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = new MovTaskInfermieristico(e.Cell.Row.Cells["ID"].Text, EnumAzioni.ANN, CoreStatics.CoreApplication.Ambiente);
                            if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.ErogazioneTaskInfermieristici) == DialogResult.OK)
                            {
                                this.AggiornaGriglia(true);
                                if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato != null)
                                {
                                    CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "ID", CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.IDMovTaskInfermieristico);
                                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = null;
                                }
                            }
                            break;

                        case CoreStatics.C_COL_BTN_DEL:
                            if (e.Cell.Row.Cells["PermessoCancella"].Text == "1")
                            {
                                if (e.Cell.Row.Cells["IDGruppo"].Text != "")
                                {
                                    if (easyStatics.EasyMessageBox("Il task infermieristico selezionato fa parte di un gruppo." +
    Environment.NewLine + "Eseguire la cancellazione di tutto il gruppo ?", "Cancellazione Task Infermieristici",
    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                    {
                                        this.CancellaGruppo(e.Cell.Row.Cells["CodSistema"].Text, e.Cell.Row.Cells["IDSistema"].Text, e.Cell.Row.Cells["IDGruppo"].Text);
                                    }
                                    else
                                    {
                                        this.CancellaSingolo(e.Cell.Row.Cells["ID"].Text, true);
                                    }
                                }
                                else
                                {
                                    this.CancellaSingolo(e.Cell.Row.Cells["ID"].Text, true);
                                }
                            }
                            break;

                        case CoreStatics.C_COL_BTN_COPY:
                            if (easyStatics.EasyMessageBox("Confermi la copia del task selezionato ?", "Copia Task Infermieristici",
    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                CoreStatics.SetNavigazione(false);

                                MovTaskInfermieristico movtiorigine = new MovTaskInfermieristico(e.Cell.Row.Cells["ID"].Text, EnumAzioni.MOD, CoreStatics.CoreApplication.Ambiente);
                                CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = new MovTaskInfermieristico(CoreStatics.CoreApplication.Trasferimento.CodUA,
                                                                    CoreStatics.CoreApplication.Paziente.ID,
                                                                    CoreStatics.CoreApplication.Episodio.ID,
                                                                    CoreStatics.CoreApplication.Trasferimento.ID,
                                                                    EnumCodSistema.WKI, EnumTipoRegistrazione.M,
                                                                    CoreStatics.CoreApplication.Ambiente);

                                CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CopiaDaOrigine(ref movtiorigine);
                                movtiorigine = null;

                                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingTaskInfermieristici, false) == DialogResult.OK)
                                {
                                    this.AggiornaGriglia(true);
                                    if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato != null)
                                    {
                                        CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "ID", CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.IDMovTaskInfermieristico);
                                        CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = null;
                                    }
                                }

                                CoreStatics.SetNavigazione(true);
                            }
                            break;

                        default:
                            break;

                    }

                    CoreStatics.SetNavigazione(true);
                }
            }
            catch (Exception ex)
            {
                CoreStatics.SetNavigazione(true);

                CoreStatics.ExGest(ref ex, "ucEasyGrid_ClickCellButton", this.Name);

            }

        }

        private void ucEasyGrid_ClickCell(object sender, ClickCellEventArgs e)
        {

            try
            {
                if (Interlocked.Equals(Maschere._navigare, 0))
                {
                    return;
                }

                switch (e.Cell.Column.Key)
                {

                    case "AnteprimaRTF":
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

        private void ucEasyGrid_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            if (Interlocked.Equals(Maschere._navigare, 0))
            {
                return;
            }

            if (CoreStatics.CoreApplication.Cartella != null && CoreStatics.CoreApplication.Cartella.CartellaChiusa == true)
            {
                this.ubSpostaTask.Enabled = false;
            }
            else
            {
                if (ucEasyGrid.ActiveRow != null)
                    this.ubSpostaTask.Enabled = (int.Parse(ucEasyGrid.ActiveRow.Cells["PERMESSOSPOSTA"].Value.ToString()) == 1);
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

        private void ucEasyGridFiltroStato_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].HeaderVisible = false;
            foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
            {
                switch (oCol.Key)
                {
                    case "DescStato":
                        oCol.Hidden = false;
                        oCol.Header.Caption = @"Stato";
                        break;
                    default:
                        oCol.Hidden = true;
                        break;
                }
            }
        }

        private void uchkFiltro_Click(object sender, EventArgs e)
        {
            if (!this.uchkFiltro.Checked)
            {
                if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata != null) CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                this.InizializzaFiltri();
                this.AggiornaGriglia(true);
            }
            else
                this.uchkFiltro.Checked = !this.uchkFiltro.Checked;
        }

        private void drFiltro_ValueChanged(object sender, EventArgs e)
        {
            this.udteFiltroDA.Value = drFiltro.DataOraDa;
            this.udteFiltroA.Value = drFiltro.DataOraA;
        }

        private void ucEasyTreeView_AfterCheck(object sender, Infragistics.Win.UltraWinTree.NodeEventArgs e)
        {

            if (!_runtimecheck)
            {
                _runtimecheck = true;
                try
                {
                    if (e.TreeNode.Key == CoreStatics.GC_TUTTI)
                    {
                        foreach (Infragistics.Win.UltraWinTree.UltraTreeNode oNode in ((ucEasyTreeView)sender).Nodes[CoreStatics.GC_TUTTI].Nodes)
                        {
                            if (oNode.Override.NodeStyle == Infragistics.Win.UltraWinTree.NodeStyle.CheckBox)
                            {
                                oNode.CheckedState = e.TreeNode.CheckedState;
                            }
                        }
                    }
                    else
                    {
                        if (e.TreeNode.CheckedState != CheckState.Checked)
                        {
                            ((ucEasyTreeView)sender).Nodes[CoreStatics.GC_TUTTI].CheckedState = CheckState.Unchecked;
                        }
                        else
                        {
                            bool bCheckTutti = true;

                            foreach (Infragistics.Win.UltraWinTree.UltraTreeNode oNode in ((ucEasyTreeView)sender).Nodes[CoreStatics.GC_TUTTI].Nodes)
                            {
                                if (oNode.Override.NodeStyle == Infragistics.Win.UltraWinTree.NodeStyle.CheckBox)
                                {
                                    if (oNode.CheckedState != CheckState.Checked) bCheckTutti = false;
                                }
                            }

                            if (bCheckTutti)
                                ((ucEasyTreeView)sender).Nodes[CoreStatics.GC_TUTTI].CheckedState = CheckState.Checked;
                        }
                    }
                }
                catch (Exception ex)
                {
                    UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                }
                finally
                {
                    _runtimecheck = false;
                }
            }

        }

        private void ubApplicaFiltro_Click(object sender, EventArgs e)
        {
            if (this.ultraDockManager.FlyoutPane != null && !this.ultraDockManager.FlyoutPane.Pinned) this.ultraDockManager.FlyIn();
            this.AggiornaGriglia(false);
            this.ucEasyGrid.Focus();
        }

        private void ubAdd_Click(object sender, EventArgs e)
        {

            try
            {

                if (Interlocked.Equals(Maschere._navigare, 0))
                {
                    return;
                }
                else
                {

                    CoreStatics.SetNavigazione(false);

                    CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = new MovTaskInfermieristico(CoreStatics.CoreApplication.Trasferimento.CodUA,
                                                            CoreStatics.CoreApplication.Paziente.ID,
                                                            CoreStatics.CoreApplication.Episodio.ID,
                                                            CoreStatics.CoreApplication.Trasferimento.ID,
                                                            EnumCodSistema.WKI, EnumTipoRegistrazione.M,
                                                            CoreStatics.CoreApplication.Ambiente);

                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Azione = EnumAzioni.INS;
                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataEvento = DateTime.Now;

                    if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneTipoTaskInfermieristici, false) == DialogResult.OK)
                    {
                        DialogResult result = DialogResult.Cancel;
                        if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato != null)
                        {
                            if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.ErogazioneDiretta == false)
                            {
                                if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.List_CodTipoTaskInfermieristico == null)
                                {
                                    result = CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingTaskInfermieristici, false);
                                }
                                else
                                {
                                    ViewControllerMultiTaskInfermieristico oVC = new ViewControllerMultiTaskInfermieristico();
                                    oVC.Paziente = CoreStatics.CoreApplication.Paziente;
                                    oVC.Episodio = CoreStatics.CoreApplication.Episodio;
                                    oVC.Trasferimento = CoreStatics.CoreApplication.Trasferimento;
                                    oVC.Cartella = CoreStatics.CoreApplication.Cartella;
                                    oVC.MovTaskInfermieristico = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato;
                                    oVC = (ViewControllerMultiTaskInfermieristico)CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.MultiTaskInfermieristici, false, oVC);
                                    result = oVC.DialogResultReturn;
                                }
                            }
                            else
                            {
                                List<string> lstresult = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.GeneraErogazioneDiretta();
                                if (lstresult.Count > 0)
                                {
                                    var vmessaggio = String.Join(Environment.NewLine, lstresult.ToArray());
                                    easyStatics.EasyMessageBox(vmessaggio, "Generazione Erogazione Diretta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                }
                                result = DialogResult.OK;
                                CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = null;
                            }
                        }
                        else
                        {
                            result = CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingTaskInfermieristiciProtocollo, false);
                        }
                        if (result == DialogResult.OK)
                        {
                            this.AggiornaGriglia(true);
                            if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato != null)
                                CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "ID", CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.IDMovTaskInfermieristico);
                        }
                    }

                    CoreStatics.SetNavigazione(true);

                }

            }
            catch (Exception ex)
            {
                CoreStatics.SetNavigazione(true);

                CoreStatics.ExGest(ref ex, "ubAdd_Click", this.Name);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
            }

            CoreStatics.SetNavigazione(true);

        }

        private void ubSpostaTask_Click(object sender, EventArgs e)
        {
            try
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

                CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = new MovTaskInfermieristico(this.ucEasyGrid.ActiveRow.Cells["ID"].Text, EnumAzioni.MOD, CoreStatics.CoreApplication.Ambiente);

                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SpostaTaskInAltraCartella) == DialogResult.OK)
                {
                    this.AggiornaGriglia(true);
                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = null;
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ubAdd_Click", this.Name);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
            }
        }

        private void ubAdmin_Click(object sender, EventArgs e)
        {

            try
            {

                if (this.ucEasyGrid.ActiveRow != null &&
                    (this.ucEasyGrid.ActiveRow.Cells["CodStato"].Text.Trim().ToUpper() == EnumStatoTaskInfermieristico.ER.ToString() ||
                    this.ucEasyGrid.ActiveRow.Cells["CodStato"].Text.Trim().ToUpper() == EnumStatoTaskInfermieristico.AN.ToString()))
                {

                    if (easyStatics.EasyMessageBox("Confermi la modifica di stato in 'Programmato' del task selezionato ?", "Imposta in Programmato Task Infermieristico",
                                   MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

                        String sLogin = CoreStatics.CoreApplication.Ambiente.Codlogin;

                        CoreStatics.CoreApplication.Ambiente.Codlogin = UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.ElabSistemiUserName);

                        CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = new MovTaskInfermieristico(this.ucEasyGrid.ActiveRow.Cells["ID"].Text.Trim().ToUpper(), EnumAzioni.MOD, CoreStatics.CoreApplication.Ambiente);
                        CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.ReImpostaProgrammato();

                        CoreStatics.CoreApplication.Ambiente.Codlogin = sLogin;

                        this.AggiornaGriglia(true);
                        if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato != null)
                            CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "ID", CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.IDMovTaskInfermieristico);
                    }

                }
                else
                {
                    easyStatics.EasyMessageBox("Impossibile modificare lo stato in 'Programmato' del task selezionato." + Environment.NewLine +
                                                "Gli stati ammessi sono 'Eseguito' ed 'Annullato'.", "Imposta in Programmato Task Infermieristico",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ubAdmin_Click", this.Name);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
            }

        }

        private void ucWorkListInfermieristica_Enter(object sender, EventArgs e)
        {
            try
            {
                this.ucEasyGrid.PerformLayout();
            }
            catch (Exception)
            {
            }
        }

        private void ucEasyGrid_AfterRowActivate(object sender, EventArgs e)
        {
            CoreStatics.SetContesto(EnumEntita.WKI, this.ucEasyGrid.ActiveRow);
            CoreStatics.CoreApplication.IDTaskInfermieristicoSelezionato = "";

            if (this.ucEasyGrid.ActiveRow != null)
            {
                CoreStatics.CoreApplication.IDTaskInfermieristicoSelezionato = this.ucEasyGrid.ActiveRow.Cells["ID"].Text;
            }
        }

        private void utxtFiltroContenuto_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (this.ubApplicaFiltro.Enabled && e.KeyCode == Keys.Enter) ubApplicaFiltro_Click(this.ubApplicaFiltro, new EventArgs());
            }
            catch (Exception)
            {
            }
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

        private void ucEasyPopUpOrario_AnnullaClick(object sender, EventArgs e)
        {
            this.ultraPopupControlContainerOra.Close();
            _popuporarioID = "";
        }

        private void ucEasyPopUpOrario_ConfermaClick(object sender, EventArgs e)
        {
            if (this.ucEasyPopUpOrario.DataOra != null)
            {
                try
                {
                    this.ultraPopupControlContainerOra.Close();

                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = new MovTaskInfermieristico(_popuporarioID, EnumAzioni.MOD, CoreStatics.CoreApplication.Ambiente);
                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataProgrammata = (DateTime)this.ucEasyPopUpOrario.DataOra;
                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Note = this.ucEasyPopUpOrario.Note;

                    Risposta oRispostaElaboraPrima = PluginClientStatics.PluginClient(EnumPluginClient.WKI_MODIFICA_PRIMA_PU.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.Trasferimento.CodUA, CoreStatics.CoreApplication.Ambiente));
                    if (oRispostaElaboraPrima.Successo)
                    {
                        if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Salva())
                        {
                            AggiornaGriglia(false);


                            if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato != null)
                            {
                                CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "ID", CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.IDMovTaskInfermieristico);
                                CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = null;
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    CoreStatics.ExGest(ref ex, "ucEasyPopUpOrario_ConfermaClick", this.Name);
                }
                finally
                {
                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = null;
                }
            }
        }

        private void ultraPopupControlContainerOra_Opening(object sender, CancelEventArgs e)
        {
            Infragistics.Win.Misc.UltraPopupControlContainer popupora = sender as Infragistics.Win.Misc.UltraPopupControlContainer;


            int iWidth = Convert.ToInt32(easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large)) * 20;
            int iHeight = Convert.ToInt32((double)iWidth / 1.52D);
            this.ucEasyPopUpOrario.Size = new Size(iWidth, iHeight);
            popupora.PopupControl = this.ucEasyPopUpOrario;

        }

        #endregion

    }
}
