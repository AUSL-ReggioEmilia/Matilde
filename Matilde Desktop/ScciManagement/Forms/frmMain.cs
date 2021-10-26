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
using UnicodeSrl.ScciCore;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.PluginClient;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        #region UltraToolBarManager

        private void InitializeToolbar()
        {

            try
            {

                MyStatics.SetUltraToolbarsManager(this.UltraToolbarsManager);

                foreach (ToolBase oTool in this.UltraToolbarsManager.Tools)
                {
                    switch (oTool.Key)
                    {
                        case MyStatics.GC_SCHEDETREEVIEW:
                            oTool.SharedProps.Visible = false;
                            break;


                        default:
                            oTool.SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(oTool.Key, Enums.EnumImageSize.isz32));
                            oTool.SharedProps.AppearancesSmall.Appearance.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(oTool.Key, Enums.EnumImageSize.isz16));
                            break;
                    }
                }

                var _with1 = this.UltraToolbarsManager.Ribbon;
                _with1.Visible = true;
                _with1.Caption = string.Format("Scci Management (Versione : {0})", Application.ProductVersion);
                _with1.ApplicationMenuButtonImage = Risorse.GetImageFromResource(Risorse.GC_SCCIMANAGEMENT);
                _with1.ApplicationMenu.ToolAreaLeft.Settings.UseLargeImages = Infragistics.Win.DefaultableBoolean.True;
                _with1.ApplicationMenu.ToolAreaRight.Settings.UseLargeImages = Infragistics.Win.DefaultableBoolean.True;

                foreach (RibbonTab oTab in _with1.Tabs)
                {

                    oTab.Settings.Appearance.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(oTab.Key, Enums.EnumImageSize.isz16));

                    foreach (RibbonGroup oGrp in oTab.Groups)
                    {
                        oGrp.PreferredToolSize = RibbonToolSize.Large;
                    }

                }

            }
            catch (Exception)
            {

            }

        }

        private void ActionToolClick(Infragistics.Win.UltraWinToolbars.ToolBase Tool)
        {

            try
            {

                switch (Tool.Key)
                {

                    case MyStatics.GC_ESCI:
                        this.Close();
                        break;

                    case MyStatics.GC_CHIUDIFINESTRE:
                        foreach (Form childForm in MdiChildren)
                        {
                            childForm.Close();
                        }
                        break;

                    default:
                        if (Tool.OwnerIsMenu == true && Tool.OwningMenu.Key == MyStatics.GC_INTEGRAZIONIALTRE)
                        {
                            ActionCDSSClient(ref Tool);
                        }
                        else
                        {
                            ActionView(ref Tool);
                        }
                        break;

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion

        #region UltraStatusBar

        private void InitializeStatusBar()
        {

            try
            {

                this.UltraStatusBar.ViewStyle = Infragistics.Win.UltraWinStatusBar.ViewStyle.Office2007;
                this.UltraStatusBar.Panels.Add("Testo");
                this.UltraStatusBar.Panels["Testo"].SizingMode = Infragistics.Win.UltraWinStatusBar.PanelSizingMode.Spring;
                this.UltraStatusBar.Panels.Add("DataBase");
                this.UltraStatusBar.Panels["DataBase"].SizingMode = Infragistics.Win.UltraWinStatusBar.PanelSizingMode.Automatic;
                this.UltraStatusBar.Panels.Add("Data");
                this.UltraStatusBar.Panels["Data"].Style = Infragistics.Win.UltraWinStatusBar.PanelStyle.Date;
                this.UltraStatusBar.Panels["Data"].SizingMode = Infragistics.Win.UltraWinStatusBar.PanelSizingMode.Automatic;
                this.UltraStatusBar.Panels.Add("Ora");
                this.UltraStatusBar.Panels["Ora"].Style = Infragistics.Win.UltraWinStatusBar.PanelStyle.Time;
                this.UltraStatusBar.Panels["Ora"].SizingMode = Infragistics.Win.UltraWinStatusBar.PanelSizingMode.Automatic;

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        #endregion

        #region Subroutine

        private void InitializeMain()
        {
#if NET40
            this.Text = string.Format("Scci Management (Versione : {0}) (net40)", Application.ProductVersion);
#elif NET472
            this.Text = string.Format("Scci Management (Versione : {0}) (net472)", Application.ProductVersion);
#endif
            this.Icon = Risorse.GetIconFromResource(Risorse.GC_SCCIMANAGEMENT);
            this.UltraStatusBar.Panels["DataBase"].Text = MyStatics.Configurazione.GetPropertyConnectionString();

        }

        private void ActionView(ref Infragistics.Win.UltraWinToolbars.ToolBase roTool)
        {

            this.Cursor = Cursors.WaitCursor;

            try
            {

                if (this.UltraTabbedMdiManager.TabFromKey(roTool.SharedProps.Caption) == null)
                {

                    switch (roTool.Key)
                    {

                        case MyStatics.GC_CONFIGURAZIONE:
                            frmConfigurazione fConfigurazione = new frmConfigurazione();
                            fConfigurazione.ViewIcon = Risorse.GetIconFromResource(MyStatics.GetNameResource(MyStatics.GC_CONFIGURAZIONE, Enums.EnumImageSize.isz32));
                            fConfigurazione.ViewText = roTool.SharedProps.Caption;
                            fConfigurazione.ViewInit();
                            fConfigurazione.ShowDialog();
                            this.InitializeMain();
                            break;

                        case MyStatics.GC_CONFIGCE:
                            frmConfigCE fConfigCE = new frmConfigCE();
                            fConfigCE.ViewIcon = Risorse.GetIconFromResource(MyStatics.GetNameResource(MyStatics.GC_CONFIGCE, Enums.EnumImageSize.isz32));
                            fConfigCE.ViewText = roTool.SharedProps.Caption;
                            fConfigCE.ViewInit();
                            fConfigCE.ShowDialog();
                            this.InitializeMain();
                            break;

                        case MyStatics.GC_CONFIGURAZIONE_AMBIENTE:
                            frmConfigTable fConfigTable = new frmConfigTable();
                            fConfigTable.ViewIcon = Risorse.GetIconFromResource(MyStatics.GetNameResource(MyStatics.GC_CONFIGURAZIONE_AMBIENTE, Enums.EnumImageSize.isz32));
                            fConfigTable.ViewText = roTool.SharedProps.Caption;
                            fConfigTable.ViewInit();
                            fConfigTable.ShowDialog();
                            this.InitializeMain();
                            break;

                        case MyStatics.GC_MODULI:
                            frmModuli fModuli = new frmModuli();
                            fModuli.ViewIcon = Risorse.GetIconFromResource(MyStatics.GetNameResource(MyStatics.GC_MODULI, Enums.EnumImageSize.isz32));
                            fModuli.ViewText = roTool.SharedProps.Caption;
                            fModuli.ViewInit();
                            fModuli.ShowDialog();
                            break;

                        case MyStatics.GC_UNITAATOMICHETREEVIEW:
                            frmUATreeView fUATreeView = new frmUATreeView();
                            fUATreeView.ViewIcon = Risorse.GetIconFromResource(MyStatics.GetNameResource(MyStatics.GC_UNITAATOMICHE, Enums.EnumImageSize.isz32));
                            fUATreeView.ViewText = roTool.SharedProps.Caption;
                            fUATreeView.ViewInit();
                            fUATreeView.ShowDialog();
                            break;

                        case MyStatics.GC_REPORTTREEVIEW:
                            frmReportTreeView fReportTreeView = new frmReportTreeView();
                            fReportTreeView.ViewIcon = Risorse.GetIconFromResource(MyStatics.GetNameResource(MyStatics.GC_REPORT, Enums.EnumImageSize.isz32));
                            fReportTreeView.ViewText = roTool.SharedProps.Caption;
                            fReportTreeView.ViewInit();
                            fReportTreeView.ShowDialog();
                            break;

                        case MyStatics.GC_ICONE:
                            frmIcone fIcone = new frmIcone();
                            fIcone.ViewIcon = Risorse.GetIconFromResource(MyStatics.GetNameResource(MyStatics.GC_ICONE, Enums.EnumImageSize.isz32));
                            fIcone.ViewText = roTool.SharedProps.Caption;
                            fIcone.ViewInit();
                            fIcone.ShowDialog();
                            break;

                        case MyStatics.GC_TESTIPREDEFINITITREEVIEW:
                            frmTestiPTreeView fTestiPTreeView = new frmTestiPTreeView();
                            fTestiPTreeView.ViewIcon = Risorse.GetIconFromResource(MyStatics.GetNameResource(MyStatics.GC_TESTIPREDEFINITI, Enums.EnumImageSize.isz32));
                            fTestiPTreeView.ViewText = roTool.SharedProps.Caption;
                            fTestiPTreeView.ViewInit();
                            fTestiPTreeView.ShowDialog();
                            break;

                        case MyStatics.GC_SCHEDEESPORTA:
                            frmSchedeEsporta ofrmSchedeEsporta = new frmSchedeEsporta();
                            ofrmSchedeEsporta.ViewIcon = Risorse.GetIconFromResource(MyStatics.GetNameResource(MyStatics.GC_SCHEDE, Enums.EnumImageSize.isz32));
                            ofrmSchedeEsporta.ViewText = roTool.SharedProps.Caption;
                            ofrmSchedeEsporta.ViewInit();
                            ofrmSchedeEsporta.ShowDialog();
                            break;

                        case MyStatics.GC_SCHEDETREEVIEW:
                            frmSchedeTreeView fSchedeTreeView = new frmSchedeTreeView();
                            fSchedeTreeView.ViewIcon = Risorse.GetIconFromResource(MyStatics.GetNameResource(MyStatics.GC_SCHEDE, Enums.EnumImageSize.isz32));
                            fSchedeTreeView.ViewText = roTool.SharedProps.Caption;
                            fSchedeTreeView.ViewInit();
                            fSchedeTreeView.ShowDialog();
                            break;

                        case MyStatics.GC_CONSULTAEPISODIO:
                            frmAmministrazione fAmministrazione = new frmAmministrazione();
                            fAmministrazione.ViewText = roTool.SharedProps.Caption;
                            fAmministrazione.ViewIcon = Risorse.GetIconFromResource(MyStatics.GetNameResource(roTool.Key, Enums.EnumImageSize.isz16));
                            fAmministrazione.ViewImage = Risorse.GetImageFromResource(MyStatics.GetNameResource(roTool.Key, Enums.EnumImageSize.isz256));
                            fAmministrazione.MdiParent = this;
                            fAmministrazione.Name = roTool.SharedProps.Caption;
                            fAmministrazione.ViewInit();
                            fAmministrazione.Show();
                            break;

                        case MyStatics.GC_MANUTENZIONEDATI:
                            frmManutenzioneDati fManutenzioneDati = new frmManutenzioneDati();
                            fManutenzioneDati.ViewText = roTool.SharedProps.Caption;
                            fManutenzioneDati.ViewIcon = Risorse.GetIconFromResource(MyStatics.GetNameResource(roTool.Key, Enums.EnumImageSize.isz16));
                            fManutenzioneDati.ViewImage = Risorse.GetImageFromResource(MyStatics.GetNameResource(roTool.Key, Enums.EnumImageSize.isz256));
                            fManutenzioneDati.Name = roTool.SharedProps.Caption;
                            fManutenzioneDati.ViewInit();
                            fManutenzioneDati.ShowDialog();
                            break;

                        case MyStatics.GC_ELABORAZIONI:
                            frmElaborazioni fElaborazioni = new frmElaborazioni();
                            fElaborazioni.ViewText = roTool.SharedProps.Caption;
                            fElaborazioni.ViewIcon = Risorse.GetIconFromResource(MyStatics.GetNameResource(roTool.Key, Enums.EnumImageSize.isz16));
                            fElaborazioni.ViewImage = Risorse.GetImageFromResource(MyStatics.GetNameResource(roTool.Key, Enums.EnumImageSize.isz256));
                            fElaborazioni.MdiParent = this;
                            fElaborazioni.Name = roTool.SharedProps.Caption;
                            fElaborazioni.ViewInit();
                            fElaborazioni.Show();
                            break;

                        case MyStatics.GC_NORMALIZZAZIONE:
                            frmNormalizzazione fNormalizzazione = new frmNormalizzazione();
                            fNormalizzazione.ViewText = roTool.SharedProps.Caption;
                            fNormalizzazione.ViewIcon = Risorse.GetIconFromResource(MyStatics.GetNameResource(roTool.Key, Enums.EnumImageSize.isz16));
                            fNormalizzazione.ViewImage = Risorse.GetImageFromResource(MyStatics.GetNameResource(roTool.Key, Enums.EnumImageSize.isz256));
                            fNormalizzazione.MdiParent = this;
                            fNormalizzazione.Name = roTool.SharedProps.Caption;
                            fNormalizzazione.ViewInit();
                            fNormalizzazione.Show();
                            break;

                        case MyStatics.GC_DOCUMENTIFIRMATI:
                            frmDocumentiFirmati fDocumentiFirmati = new frmDocumentiFirmati();
                            fDocumentiFirmati.ViewText = roTool.SharedProps.Caption;
                            fDocumentiFirmati.ViewIcon = Risorse.GetIconFromResource(MyStatics.GetNameResource(roTool.Key, Enums.EnumImageSize.isz16));
                            fDocumentiFirmati.ViewImage = Risorse.GetImageFromResource(MyStatics.GetNameResource(roTool.Key, Enums.EnumImageSize.isz256));
                            fDocumentiFirmati.MdiParent = this;
                            fDocumentiFirmati.Name = roTool.SharedProps.Caption;
                            fDocumentiFirmati.ViewInit();
                            fDocumentiFirmati.Show();
                            break;

                        case MyStatics.GC_REPORTSTORICIZZATI:
                            frmReportStoricizzati fReportStoricizzati = new frmReportStoricizzati();
                            fReportStoricizzati.ViewText = roTool.SharedProps.Caption;
                            fReportStoricizzati.ViewIcon = Risorse.GetIconFromResource(MyStatics.GetNameResource(roTool.Key, Enums.EnumImageSize.isz16));
                            fReportStoricizzati.ViewImage = Risorse.GetImageFromResource(MyStatics.GetNameResource(roTool.Key, Enums.EnumImageSize.isz256));
                            fReportStoricizzati.MdiParent = this;
                            fReportStoricizzati.Name = roTool.SharedProps.Caption;
                            fReportStoricizzati.ViewInit();
                            fReportStoricizzati.Show();
                            break;

                        default:
                            frmView f = new frmView();
                            f.ViewText = roTool.SharedProps.Caption;
                            f.ViewIcon = Risorse.GetIconFromResource(MyStatics.GetNameResource(roTool.Key, Enums.EnumImageSize.isz16));
                            f.ViewImage = Risorse.GetImageFromResource(MyStatics.GetNameResource(roTool.Key, Enums.EnumImageSize.isz256));
                            f.ViewDataName = (Enums.EnumDataNames)System.Enum.Parse(typeof(Enums.EnumDataNames), DataBase.GetViewDataName(ref roTool));
                            f.ViewDataNamePU = (Enums.EnumDataNames)System.Enum.Parse(typeof(Enums.EnumDataNames), DataBase.GetViewDataName(ref roTool));
                            f.MdiParent = this;
                            f.Name = roTool.SharedProps.Caption;
                            f.ViewInit();
                            f.Show();
                            break;

                    }

                }
                else
                {
                    this.UltraTabbedMdiManager.TabFromKey(roTool.SharedProps.Caption).Activate();
                }

            }
            catch (Exception)
            {

            }

            this.Cursor = Cursors.Default;

        }

        private void ActionCDSSClient(ref Infragistics.Win.UltraWinToolbars.ToolBase roTool)
        {

            this.Cursor = Cursors.WaitCursor;

            CoreStatics.CoreApplication.Ambiente.Codruolo = UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.ElabSistemiCodRuolo);
            PluginClientStatics.Pcm = PluginClientStatics.SetPluginClientManager(CoreStatics.CoreApplication.Ambiente, CoreStatics.CoreApplication.Ambiente.Codruolo, false, false);

            try
            {

                MyStatics.g_split = MyStatics.SetSplit(roTool.Key, @"|");

                if (PluginClientStatics.Pcm.DataBindingsForAzioni.Azioni[EnumPluginClient.MANAGEMENT_INTEGRA.ToString()].Plugins.Count != 0)
                {

                    object[] myparam = new object[1] { new object() };

                    IList<Plugin> results = PluginClientStatics.Pcm.DataBindingsForAzioni.Azioni[EnumPluginClient.MANAGEMENT_INTEGRA.ToString()].Plugins
                                  .Where(p => p.Codice == MyStatics.g_split.GetValue(2).ToString() && p.CodUA == MyStatics.g_split.GetValue(0).ToString())
                                  .ToList();

                    if (results != null && results.Count == 1)
                    {

                        Risposta oRispostaMenuEsegui = PluginClientStatics.PluginClientMenuEsegui((Plugin)results[0], myparam);
                        if (oRispostaMenuEsegui.Successo == true)
                        {

                        }

                    }

                }

            }
            catch (Exception)
            {

            }

            CoreStatics.CoreApplication.Ambiente.Codruolo = string.Empty;
            PluginClientStatics.Pcm = null;

            this.Cursor = Cursors.Default;

        }

        #endregion

        #region Events Form

        private void frmMain_Load(object sender, EventArgs e)
        {

            this.InitializeToolbar();
            this.InitializeStatusBar();
            this.InitializeMain();

            try
            {
                foreach (Control oCtl in Controls)
                {
                    if (oCtl.GetType() == typeof(MdiClient))
                    {
                        oCtl.BackColor = Color.FromArgb(255, 108, 145, 193);
                    }
                }
            }
            catch (Exception)
            {
            }

        }

        private void frmMain_VisibleChanged(object sender, EventArgs e)
        {
            try
            {
                Program.CloseSplash();
            }
            catch { }
        }

        #endregion

        #region Events

        private void UltraTabbedMdiManager_InitializeTab(object sender, Infragistics.Win.UltraWinTabbedMdi.MdiTabEventArgs e)
        {

            try
            {
                e.Tab.Settings.DisplayFormIcon = Infragistics.Win.DefaultableBoolean.True;
                e.Tab.Key = e.Tab.Form.Text;
            }
            catch (Exception)
            {
            }

        }

        private void UltraToolbarsManager_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {

            if (e.Tool.Key == MyStatics.GC_INTEGRAZIONIALTRE)
            {

                var utbm = (PopupMenuTool)e.Tool;
                utbm.Settings.UseLargeImages = Infragistics.Win.DefaultableBoolean.True;
                utbm.Tools.Clear();

                string sSql = @"Select * From T_CDSSStruttura " +
                                "Where CodAzione = '" + UnicodeSrl.Scci.Enums.EnumPluginClient.MANAGEMENT_INTEGRA.ToString() + "'";
                DataSet oDs = DataBase.GetDataSet(sSql);

                foreach (DataRow oDr in oDs.Tables[0].Rows)
                {

                    string sCodPlugin = oDr["CodPlugin"].ToString();
                    string sCodice = string.Format("{0}|{1}|{2}", oDr["CodUA"].ToString(), oDr["CodAzione"].ToString(), sCodPlugin);
                    string sDescrizione = DataBase.FindValue("Descrizione", "T_CDSSPlugins", "Codice = '" + sCodPlugin + "'", "Non Trovato");
                    byte[] byteIcona = UnicodeSrl.ScciCore.CoreStatics.ImageToByte(Risorse.GetImageFromResource(Risorse.GC_CDSSPLUGIN_32));
                    if (DataBase.FindValue("Icona", "T_CDSSPlugins", "Codice = '" + sCodPlugin + "'", "") != string.Empty)
                    {
                        byteIcona = (byte[])DataBase.FindValue("Icona", "T_CDSSPlugins", "Codice = '" + sCodPlugin + "'");
                    }

                    if (this.UltraToolbarsManager.Tools.Exists(sCodice) == false)
                    {
                        ButtonTool oBt = new ButtonTool(sCodice);
                        this.UltraToolbarsManager.Tools.Add(oBt);
                    }
                    ((ButtonTool)this.UltraToolbarsManager.Tools[sCodice]).SharedProps.Caption = sDescrizione;
                    ((ButtonTool)this.UltraToolbarsManager.Tools[sCodice]).SharedProps.AppearancesLarge.Appearance.Image = Scci.Statics.DrawingProcs.GetImageFromByte(byteIcona); ;

                    utbm.Tools.AddTool(this.UltraToolbarsManager.Tools[sCodice].Key);

                }

            }

        }

        private void UltraToolbarsManager_ToolClick(object sender, ToolClickEventArgs e)
        {
            this.ActionToolClick(e.Tool);
        }

        #endregion

    }
}
