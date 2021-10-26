using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using Infragistics.Win;
using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinSchedule;
using Infragistics.Win.UltraWinTree;
using Infragistics.Win.UltraWinToolbars;
using System.Data;
using System.Xml.Serialization;
using System.Xml;
using System.Threading;

using Microsoft.Data.SqlClient;
using UnicodeSrl.Framework.Data;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.ScciResource;
using UnicodeSrl.ScciCore;
using UnicodeSrl.Scci.PluginClient;

using UnicodeSrl.Scci;

using UnicodeSrl.DatiClinici.Gestore;

using UnicodeSrl.ScciCore.Common;
using UnicodeSrl.Scci.Plugin;
using System.Drawing.Printing;
using O2S.Components.PDFRender4NET;
using O2S.Components.PDFRender4NET.Printing;
using UnicodeSrl.ScciCore.Forms;
using UnicodeSrl.Scci.Model;
using DevExpress.XtraReports.UI;
using UnicodeSrl.DatiClinici.DC;

namespace UnicodeSrl.ScciCore
{

    public static class CoreStatics
    {

        #region Dichiarazioni

        private static CoreApplication m_CoreApplication; private static CoreApplicationContext m_CoreApplicationContext; private static ScciMain m_MainWnd;
        private static string m_getFontPredefinitoForm = string.Empty;

        internal static Cursor m_CursorBusy = null;

        public const string GC_TUTTI = @"Tutti";

        public const string GC_INIZIOGRASSETTO = @"Ç";
        public const string GC_FINEGRASSETTO = @"Ü";
        public const string GC_INIZIOITALICO = @"";
        public const string GC_FINEITALICO = @"";
        public const string GC_INIZIOSOTTOLINEATO = @"";
        public const string GC_FINESOTTOLINEATO = @"";
        public const string GC_INIZIOBARRATO = @"";
        public const string GC_FINEBARRATO = @"";

        public const string GC_DATAINIZIO = "01/01/1900 00:00";
        public const string GC_DATAFINE = "31/12/2100 23:59";

        public const string TV_ROOT = @"Root";
        public const string TV_ROOT_REPORT = @"Elenco Report";
        public const string TV_ROOT_SCHEDE = @"Elenco Schede";
        public const string TV_ROOT_TESTI = @"Elenco Testi";
        public const string TV_ROOT_TIPOPRESCRIZIONI = @"Elenco Tipo Prescrizioni";
        public const string TV_ROOT_PROTOCOLLIPRESCRIZIONI = @"Elenco Protocolli Prescrizioni";
        public const string TV_ROOT_ALLEGATI = @"Allegati";
        public const string TV_PATH = @"Path";
        public const string TV_REPORT = @"Report";
        public const string TV_SCHEDA = @"Scheda";
        public const string TV_SCHEDA_AMBULATORIALE = @"Scheda Ambulatoriale";
        public const string TV_TESTO = @"Testo";
        public const string TV_TIPOPRESCRIZIONE = @"Tipo Prescrizione";
        public const string TV_PROTOCOLLOPRESCRIZIONE = @"Protocollo Prescrizione";

        public const string TV_AGENDE = @"Agende";
        public const string TV_APPUNTAMENTI = @"Appuntamenti";
        public const string TV_RICOVERO = @"Ricovero";
        public const string TV_RICOVEROD = @"RicoveroD";
        public const string TV_CARTELLA = @"Cartella";

        public const string C_COL_BTN_ADD = "BTN_ADD";
        public const string C_COL_BTN_EDIT = "BTN_EDIT";
        public const string C_COL_BTN_VALID = "BTN_VALID";
        public const string C_COL_BTN_VIEW = "BTN_VIEW";
        public const string C_COL_BTN_DEL = "BTN_DEL";
        public const string C_COL_BTN_GRAPH = "BTN_GRAPH";
        public const string C_COL_BTN_ANNULLA = "BTN_ANNULLA";
        public const string C_COL_BTN_COPY = "BTN_COPY";
        public const string C_COL_BTN_EROG = "BTN_EROG";
        public const string C_COL_BTN_EROG_RAPIDA = "BTN_EROG_RAPIDA";
        public const string C_COL_BTN_STATO = "BTN_STATO";
        public const string C_COL_ICO_STATO = "ICO_STATO";
        public const string C_COL_BTN_TASK = "BTN_TASK";
        public const string C_COL_BTN_SOSPENDI = "BTN_SOSPENDI";
        public const string C_COL_BTN_GOTO = "BTN_GOTO";
        public const string C_COL_BTN_ORARIO = "BTN_ORARIO";
        public const string C_COL_BTN_MENU = "BTN_MENU";
        public const string C_COL_BTN_REFERTPDF = "BTN_REFERTOPDF";
        public const string C_COL_BTN_CARTELLA = "BTN_CARTELLA";
        public const string C_COL_BTN_VISTO = "BTN_VISTO";
        public const string C_COL_BTN_CONSEGNA = "BTN_CONSEGNA";
        public const string C_COL_SPAZIO = "COL_SPAZIO";

        public const string C_PDF_LICENCE_KEY = @"PDFVW4WIN50-NVTR2-L66A4-YFLMF-XKS2P-63FJ4";

        public const string C_PARAM_NEW_REV = "NEWREV";

        public static Array g_split;

        #endregion

        #region Proprietà e Metodi x configurazione

        public static CoreApplication CoreApplication
        {
            get
            {
                if (m_CoreApplication == null) m_CoreApplication = new CoreApplication();
                return m_CoreApplication;
            }
            set
            {
                m_CoreApplication = value;
            }
        }

        public static CoreApplicationContext CoreApplicationContext
        {
            get
            {
                if (m_CoreApplicationContext == null) m_CoreApplicationContext = new CoreApplicationContext();
                return m_CoreApplicationContext;
            }
            set
            {
                m_CoreApplicationContext = value;
            }
        }

        public static ScciMain MainWnd
        {
            get
            {
                if (m_MainWnd == null) m_MainWnd = new ScciMain();
                return m_MainWnd;
            }
            set
            {
                m_MainWnd = value;
            }
        }

        public static bool CheckEsci()
        {

            bool bRet = false;

            var oSessione = CoreStatics.CoreApplication.Sessione;
            if (oSessione.Utente.Ruoli.RuoloSelezionato == null || (oSessione.Utente.Ruoli.RuoloSelezionato != null && oSessione.Utente.Ruoli.RuoloSelezionato.DaFirmare == 0))
            {
                bRet = true;
            }
            else
            {
                switch (UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.BloccoUscita).Trim().ToUpper())
                {
                    case "FALSE":
                    case "FALSO":
                    case "NO":
                    case "NON BLOCCARE":
                    case "0":
                        bRet = true;
                        break;

                    case "AVVISA":
                        if (easyStatics.EasyMessageBox(@"VOCI DI DIARIO CLINICO DA FIRMARE!" + Environment.NewLine + "VUOI USCIRE UGUALMENTE?", "VALIDAZIONE", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                            bRet = false;
                        else
                            bRet = true;
                        break;

                    default:
                        bRet = false;
                        easyStatics.EasyMessageBox(@"VOCI DI DIARIO CLINICO DA FIRMARE!", "VALIDAZIONE", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        break;
                }
            }

            return bRet;

        }

        public static void CheckConfigTable(string ConnectionString)
        {

            try
            {

                foreach (int value in Enum.GetValues(typeof(EnumConfigTable)))
                {

                    SqlDataObject sd = new SqlDataObject(ConnectionString, @"Select * From T_Config Where ID = " + value);
                    DataSet oDs = sd.GetData();
                    if (oDs.Tables[0].Rows.Count == 0)
                    {
                        DataRow _dr = oDs.Tables[0].NewRow();
                        _dr["ID"] = value;
                        _dr["Descrizione"] = GetEnumDescription((EnumConfigTable)Enum.Parse(typeof(EnumConfigTable), value.ToString()));
                        _dr["Valore"] = @"";
                        oDs.Tables[0].Rows.Add(_dr);
                        sd.SaveData(oDs);
                    }
                    oDs.Dispose();

                }

            }
            catch (Exception)
            {

            }

        }

        public static void CheckModules(string ConnectionString)
        {

            try
            {

                foreach (string name in Enum.GetNames(typeof(EnumModules)))
                {

                    SqlDataObject sd = new SqlDataObject(ConnectionString, @"Select * From T_Moduli Where Codice = '" + name + "'");
                    DataSet oDs = sd.GetData();
                    if (oDs.Tables[0].Rows.Count == 0)
                    {
                        DataRow _dr = oDs.Tables[0].NewRow();
                        _dr["Codice"] = name;
                        _dr["Descrizione"] = GetEnumDescription((EnumModules)Enum.Parse(typeof(EnumModules), name));
                        _dr["Note"] = @"";
                        _dr["Path"] = @"Sconosciuto";
                        oDs.Tables[0].Rows.Add(_dr);
                        sd.SaveData(oDs);
                    }
                    oDs.Dispose();

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public static void CheckEntita(string ConnectionString)
        {

            try
            {

                foreach (string name in Enum.GetNames(typeof(EnumEntita)))
                {

                    SqlDataObject sd = new SqlDataObject(ConnectionString, @"Select * From T_Entita Where Codice = '" + name + "'");
                    DataSet oDs = sd.GetData();
                    if (oDs.Tables[0].Rows.Count == 0)
                    {
                        DataRow _dr = oDs.Tables[0].NewRow();
                        _dr["Codice"] = name;
                        _dr["Descrizione"] = GetEnumDescription((EnumEntita)Enum.Parse(typeof(EnumEntita), name));
                        oDs.Tables[0].Rows.Add(_dr);
                        sd.SaveData(oDs);
                    }
                    oDs.Dispose();

                }

            }
            catch (Exception)
            {

            }

        }

        public static void CheckAzioni(string ConnectionString)
        {

            try
            {

                foreach (string name in Enum.GetNames(typeof(EnumAzioni)))
                {

                    SqlDataObject sd = new SqlDataObject(ConnectionString, @"Select * From T_Azioni Where Codice = '" + name + "'");
                    DataSet oDs = sd.GetData();
                    if (oDs.Tables[0].Rows.Count == 0)
                    {
                        DataRow _dr = oDs.Tables[0].NewRow();
                        _dr["Codice"] = name;
                        _dr["Descrizione"] = GetEnumDescription((EnumAzioni)Enum.Parse(typeof(EnumAzioni), name));
                        oDs.Tables[0].Rows.Add(_dr);
                        sd.SaveData(oDs);
                    }
                    oDs.Dispose();

                }

            }
            catch (Exception)
            {

            }

        }

        public static string GetEnumDescription(Enum value)
        {

            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();

        }

        public static string getDateTimeNow()
        {
            return string.Format("{0:00}/{1:00}/{2:0000} {3:00}:{4:00}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year, DateTime.Now.Hour, DateTime.Now.Minute);
        }

        public static string getDateTime(DateTime dt)
        {
            return string.Format("{0:00}/{1:00}/{2:0000} {3:00}:{4:00}", dt.Day, dt.Month, dt.Year, dt.Hour, dt.Minute);
        }

        public static string getFontPredefinitoForm()
        {

            try
            {
                if (m_getFontPredefinitoForm == string.Empty)
                {
                    m_getFontPredefinitoForm = UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.FontPredefinitoForm);
                }
            }
            catch (Exception)
            {
                m_getFontPredefinitoForm = string.Empty;
            }

            return m_getFontPredefinitoForm;

        }

        public static EnumTipoAzienda getTipoAzienda()
        {

            EnumTipoAzienda en = EnumTipoAzienda.ASMN;

            try
            {

                if (CoreStatics.CoreApplication.Episodio != null)
                {

                    if (CoreStatics.CoreApplication.Trasferimento != null && CoreStatics.CoreApplication.Trasferimento.CodAziTrasferimento != string.Empty)
                    {
                        en = (EnumTipoAzienda)Enum.Parse(typeof(EnumTipoAzienda), CoreStatics.CoreApplication.Trasferimento.CodAziTrasferimento);
                    }
                    else
                    {
                        en = (EnumTipoAzienda)Enum.Parse(typeof(EnumTipoAzienda), CoreStatics.CoreApplication.Episodio.CodAzienda);
                    }

                }
                else
                {
                    Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("CodUA", CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.CodUAAmbulatorialeSelezionata);
                    SqlParameterExt[] spcoll = new SqlParameterExt[1];
                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                    DataSet oDs = Database.GetDatasetStoredProc("MSP_SelUA", spcoll);
                    if (oDs.Tables[0].Rows.Count > 0)
                    {
                        en = (EnumTipoAzienda)Enum.Parse(typeof(EnumTipoAzienda), oDs.Tables[0].Rows[0]["CodAzienda"].ToString());
                    }
                }

            }
            catch (Exception)
            {

            }

            return en;

        }

        #endregion

        #region Controllo Versione

        public static bool ControlloVersioneSCCI(bool checkDllVersion, bool showUI, bool closeSplash)
        {
            string configVersioneSCCI = "";
            bool bVersioneOK = true;
            try
            {
                bool bControllaVersione = false;
                string configControllaVersione = "";

                configVersioneSCCI = Database.GetConfigTable(EnumConfigTable.VersioneSCCI);
                if (configVersioneSCCI != null && configVersioneSCCI != string.Empty)
                {
                    configControllaVersione = UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.AbilitaControlloVersioneSCCI);
                    if (configControllaVersione != null && (configControllaVersione.Trim() == "1")) bControllaVersione = true;
                }

                if (bControllaVersione)
                {
                    bVersioneOK = false;

                    Version vrsCorrente = null;
                    if (checkDllVersion)
                        vrsCorrente = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                    else
                        vrsCorrente = new Version(Application.ProductVersion);

                    if (configVersioneSCCI.Split('.').Length != 4)
                    {
                        bVersioneOK = false;
                    }
                    else
                    {
                        Version vrsControllo = new Version(configVersioneSCCI);

                        bVersioneOK = (vrsCorrente == vrsControllo);
                    }

                    if (!bVersioneOK && showUI)
                    {
                        if (closeSplash) easyStatics.CloseEasySplash();

                        string sMsg = @"E' necessario aggiornare l'attuale versione di Matilde ( " + vrsCorrente.ToString() + @" ) all'ultima disponibile ( " + configVersioneSCCI + @" ).";
                        sMsg += Environment.NewLine + @"Contattare il supporto tecnico.";
                        easyStatics.EasyMessageBox(sMsg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Stop);

                    }


                }


            }
            catch (Exception ex)
            {
                bVersioneOK = false;

                if (showUI)
                {
                    if (closeSplash) easyStatics.CloseEasySplash();
                    string sMsg = @"Si è verificato un errore durante il controllo dell'ultima versione disponibile ( " + configVersioneSCCI + @" ).";
                    sMsg += Environment.NewLine + @"Contattare il supporto tecnico.";
                    easyStatics.EasyMessageBox(sMsg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else
                    UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);

            }
            return bVersioneOK;
        }

        public static bool ControlloVersioneSCCIManagement(bool showUI)
        {

            string configVersioneSCCIManagement = "";
            bool bVersioneOK = true;

            try
            {

                bool bControllaVersione = false;
                string configControllaVersione = "";

                configVersioneSCCIManagement = Database.GetConfigTable(EnumConfigTable.VersioneSCCIManagement);
                if (configVersioneSCCIManagement != null && configVersioneSCCIManagement != string.Empty)
                {
                    configControllaVersione = UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.AbilitaControlloVersioneSCCIManagement);
                    if (configControllaVersione != null && (configControllaVersione.Trim() == "1")) bControllaVersione = true;
                }

                if (bControllaVersione)
                {
                    bVersioneOK = false;

                    Version vrsCorrente = new Version(Application.ProductVersion);

                    if (configVersioneSCCIManagement.Split('.').Length != 4)
                    {
                        bVersioneOK = false;
                    }
                    else
                    {
                        Version vrsControllo = new Version(configVersioneSCCIManagement);

                        bVersioneOK = (vrsCorrente == vrsControllo);
                    }

                    if (!bVersioneOK && showUI)
                    {

                        string sMsg = @"E' necessario aggiornare l'attuale versione del Management ( " + vrsCorrente.ToString() + @" ) all'ultima disponibile ( " + configVersioneSCCIManagement + @" ).";
                        sMsg += Environment.NewLine + @"Contattare il supporto tecnico.";

                        MessageBox.Show(sMsg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    }

                }

            }
            catch (Exception ex)
            {
                bVersioneOK = false;

                if (showUI)
                {
                    string sMsg = @"Si è verificato un errore durante il controllo dell'ultima versione disponibile ( " + configVersioneSCCIManagement + @" ).";
                    sMsg += Environment.NewLine + @"Contattare il supporto tecnico.";
                    MessageBox.Show(sMsg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else
                    UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);

            }
            return bVersioneOK;
        }

        #endregion

        #region Infragistics

        public static void SetEasyUltraGridLayout(ref ucEasyGrid roGrid)
        {

            try
            {
                roGrid.DisplayLayout.Appearance.BackColor = System.Drawing.SystemColors.Window;
                roGrid.DisplayLayout.Appearance.BorderColor = System.Drawing.SystemColors.InactiveCaption;
                roGrid.DisplayLayout.AutoFitStyle = AutoFitStyle.None; roGrid.DisplayLayout.Appearance.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Centered;

                roGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;

                roGrid.DisplayLayout.CaptionAppearance.BackColor = Color.White;
                roGrid.DisplayLayout.CaptionAppearance.BackColor2 = Color.Lavender;
                roGrid.DisplayLayout.CaptionAppearance.BackGradientStyle = Infragistics.Win.GradientStyle.ForwardDiagonal;
                roGrid.DisplayLayout.CaptionAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.False;

                roGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;

                roGrid.DisplayLayout.GroupByBox.Hidden = true;

                roGrid.DisplayLayout.MaxColScrollRegions = 1;
                roGrid.DisplayLayout.MaxRowScrollRegions = 1;

                roGrid.DisplayLayout.Override.ActiveCellAppearance.BackColor = System.Drawing.Color.WhiteSmoke;
                roGrid.DisplayLayout.Override.ActiveCellAppearance.ForeColor = System.Drawing.SystemColors.ControlText;
                roGrid.DisplayLayout.Override.ActiveCellAppearance.BorderColor = System.Drawing.Color.Transparent;
                roGrid.DisplayLayout.Override.ActiveRowAppearance.BackColor = Color.WhiteSmoke;
                roGrid.DisplayLayout.Override.ActiveRowAppearance.ForeColor = System.Drawing.SystemColors.ControlText;
                roGrid.DisplayLayout.Override.ActiveRowAppearance.BorderColor = System.Drawing.Color.Transparent;

                roGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
                roGrid.DisplayLayout.Override.AllowColMoving = Infragistics.Win.UltraWinGrid.AllowColMoving.NotAllowed;
                roGrid.DisplayLayout.Override.AllowColSizing = Infragistics.Win.UltraWinGrid.AllowColSizing.None;
                roGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
                roGrid.DisplayLayout.Override.AllowGroupBy = Infragistics.Win.DefaultableBoolean.False;
                roGrid.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
                roGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;

                roGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.None;
                roGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.None;

                roGrid.DisplayLayout.Override.CardAreaAppearance.BackColor = System.Drawing.SystemColors.Window;
                roGrid.DisplayLayout.Override.CellAppearance.BorderColor = System.Drawing.Color.Transparent;
                roGrid.DisplayLayout.Override.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
                roGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.CellSelect;
                roGrid.DisplayLayout.Override.CellPadding = 0;

                roGrid.DisplayLayout.Override.ColumnAutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;

                roGrid.DisplayLayout.Override.HeaderAppearance.BackColor = Color.LightSteelBlue;
                roGrid.DisplayLayout.Override.HeaderAppearance.BackColor2 = Color.Lavender;
                roGrid.DisplayLayout.Override.HeaderAppearance.BackGradientStyle = Infragistics.Win.GradientStyle.ForwardDiagonal;
                roGrid.DisplayLayout.Override.HeaderAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                roGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.Default;
                roGrid.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.Default;

                roGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
                roGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;

                roGrid.DisplayLayout.Override.RowAppearance.BackColor = System.Drawing.SystemColors.Window;
                roGrid.DisplayLayout.Override.RowAppearance.BorderColor = System.Drawing.Color.Silver;
                roGrid.DisplayLayout.Override.RowAlternateAppearance.BackColor = roGrid.DisplayLayout.Override.RowAppearance.BackColor; roGrid.DisplayLayout.Override.RowAlternateAppearance.ForeColor = roGrid.DisplayLayout.Override.RowAppearance.ForeColor; roGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
                roGrid.DisplayLayout.Override.RowSizing = Infragistics.Win.UltraWinGrid.RowSizing.AutoFixed;
                roGrid.DisplayLayout.Override.RowSpacingBefore = 10;

                roGrid.DisplayLayout.Override.TemplateAddRowAppearance.BackColor = System.Drawing.SystemColors.ControlLight;

                roGrid.DisplayLayout.RowConnectorStyle = RowConnectorStyle.None;

                roGrid.DisplayLayout.ScrollStyle = ScrollStyle.Immediate;
                roGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;

                roGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
                roGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;

                roGrid.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;

            }
            catch (Exception ex)
            {
                throw new Exception("CoreStatics.SetEasyUltraGridLayout(): " + ex.Message, ex);
            }
        }

        internal static void SetUltraGridLayout(ref Infragistics.Win.UltraWinGrid.UltraGrid roGrid, bool vAddFilterRow, bool vOperatorStartWith)
        {

            roGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.None;
            roGrid.DisplayLayout.ScrollStyle = ScrollStyle.Immediate;

            roGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.True;
            roGrid.DisplayLayout.CaptionAppearance.BackColor = Color.LightSteelBlue;
            roGrid.DisplayLayout.CaptionAppearance.BackColor2 = Color.Lavender;
            roGrid.DisplayLayout.CaptionAppearance.BackGradientStyle = Infragistics.Win.GradientStyle.ForwardDiagonal;

            roGrid.DisplayLayout.GroupByBox.Hidden = true;
            roGrid.DisplayLayout.GroupByBox.Prompt = @"Trascina un'intestazione della colonna qui per raggrupparla.";
            roGrid.DisplayLayout.GroupByBox.Appearance.BackColor = Color.LightSteelBlue;
            roGrid.DisplayLayout.GroupByBox.Appearance.BackColor2 = Color.Lavender;
            roGrid.DisplayLayout.GroupByBox.Appearance.BackGradientStyle = Infragistics.Win.GradientStyle.ForwardDiagonal;
            roGrid.DisplayLayout.GroupByBox.PromptAppearance.BackColor = Color.Lavender;
            roGrid.DisplayLayout.GroupByBox.PromptAppearance.BackColor2 = Color.Lavender;
            roGrid.DisplayLayout.GroupByBox.PromptAppearance.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            roGrid.DisplayLayout.GroupByBox.PromptAppearance.ForeColor = Color.Black;

            roGrid.DisplayLayout.Override.AllowAddNew = AllowAddNew.No;
            roGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            roGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            roGrid.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            roGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            roGrid.DisplayLayout.Override.ColumnAutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.VisibleRows;
            roGrid.DisplayLayout.Override.HeaderClickAction = HeaderClickAction.SortSingle;
            roGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            roGrid.DisplayLayout.Override.RowSizing = Infragistics.Win.UltraWinGrid.RowSizing.Sychronized;
            roGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.Extended;
            roGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Extended;

            roGrid.DisplayLayout.Override.RowAlternateAppearance.BackColor = Color.WhiteSmoke;
            roGrid.DisplayLayout.Override.RowAlternateAppearance.ForeColor = Color.DarkBlue;

            roGrid.DisplayLayout.Override.HeaderAppearance.BackColor = Color.LightSteelBlue;
            roGrid.DisplayLayout.Override.HeaderAppearance.BackColor2 = Color.Lavender;
            roGrid.DisplayLayout.Override.HeaderAppearance.BackGradientStyle = Infragistics.Win.GradientStyle.ForwardDiagonal;
            roGrid.DisplayLayout.Override.HeaderAppearance.TextHAlign = Infragistics.Win.HAlign.Center;

            roGrid.DisplayLayout.Override.AllowColMoving = AllowColMoving.NotAllowed;

            if (vAddFilterRow)
            {
                UltraGridAddFilterRow(ref roGrid, vOperatorStartWith);
            }

        }

        internal static void UltraGridAddFilterRow(ref Infragistics.Win.UltraWinGrid.UltraGrid roGrid, bool vOperatorStartWith)
        {

            roGrid.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            roGrid.DisplayLayout.Override.RowFilterMode = RowFilterMode.AllRowsInBand;
            roGrid.DisplayLayout.Override.RowFilterMode = RowFilterMode.Default;
            roGrid.DisplayLayout.Override.FilterUIType = FilterUIType.FilterRow;

            roGrid.DisplayLayout.Override.FilterClearButtonLocation = FilterClearButtonLocation.Row;

            roGrid.DisplayLayout.Override.FilterRowAppearance.BackColor = Color.LightYellow;
            roGrid.DisplayLayout.Override.FilterComparisonType = FilterComparisonType.CaseInsensitive;

            if (vOperatorStartWith)
                roGrid.DisplayLayout.Override.FilterOperatorDefaultValue = FilterOperatorDefaultValue.StartsWith;
            else
                roGrid.DisplayLayout.Override.FilterOperatorDefaultValue = FilterOperatorDefaultValue.Contains;
            roGrid.DisplayLayout.Override.FilterOperatorLocation = FilterOperatorLocation.Hidden;
            roGrid.DisplayLayout.Override.FilterOperandStyle = FilterOperandStyle.Edit;

            roGrid.DisplayLayout.Override.FilterRowPrompt = "";
            roGrid.DisplayLayout.Override.FilterRowPromptAppearance.BackColorAlpha = Infragistics.Win.Alpha.Opaque;
            roGrid.DisplayLayout.Override.SpecialRowSeparator = SpecialRowSeparator.FilterRow;

        }

        internal static void UltraGridAddFilterRow(ref ucEasyGrid roGrid, bool vOperatorStartWith)
        {

            roGrid.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            roGrid.DisplayLayout.Override.RowFilterMode = RowFilterMode.AllRowsInBand;
            roGrid.DisplayLayout.Override.RowFilterMode = RowFilterMode.Default;
            roGrid.DisplayLayout.Override.FilterUIType = FilterUIType.FilterRow;

            roGrid.DisplayLayout.Override.FilterClearButtonLocation = FilterClearButtonLocation.Row;

            roGrid.DisplayLayout.Override.FilterRowAppearance.BackColor = Color.LightYellow;
            roGrid.DisplayLayout.Override.FilterComparisonType = FilterComparisonType.CaseInsensitive;

            if (vOperatorStartWith)
                roGrid.DisplayLayout.Override.FilterOperatorDefaultValue = FilterOperatorDefaultValue.StartsWith;
            else
                roGrid.DisplayLayout.Override.FilterOperatorDefaultValue = FilterOperatorDefaultValue.Contains;
            roGrid.DisplayLayout.Override.FilterOperatorLocation = FilterOperatorLocation.Hidden;
            roGrid.DisplayLayout.Override.FilterOperandStyle = FilterOperandStyle.Edit;

            roGrid.DisplayLayout.Override.FilterRowPrompt = "";
            roGrid.DisplayLayout.Override.FilterRowPromptAppearance.BackColorAlpha = Infragistics.Win.Alpha.Opaque;
            roGrid.DisplayLayout.Override.SpecialRowSeparator = SpecialRowSeparator.FilterRow;

        }

        internal static void SetGridWizardFilter(ref Infragistics.Win.UltraWinGrid.UltraGrid roGrid,
                                                   string vsFilterColumnKey, string vsFilterText,
                                                   string vsCaptionColumnKey = "", string vsCaption = "records",
                                                   string vsFilterCaption = "filtrati")
        {

            try
            {

                roGrid.Rows.ColumnFilters[vsFilterColumnKey].FilterConditions.Clear();
                if (vsFilterText != "")
                {
                    roGrid.Rows.ColumnFilters[vsFilterColumnKey].FilterConditions.Add(Infragistics.Win.UltraWinGrid.FilterComparisionOperator.Like, @"*" + vsFilterText + "*");
                }
                if (roGrid.Rows.FilteredInRowCount > 0 & roGrid.Rows.ColumnFilters[vsFilterColumnKey].FilterConditions.Count > 0)
                {
                    roGrid.ActiveRow.Selected = false;
                    roGrid.ActiveRow = roGrid.Rows.GetRowAtVisibleIndex(0);
                    roGrid.ActiveRow.Selected = true;
                }
                else if (roGrid.Rows.FilteredInRowCount == 0 & roGrid.Rows.ColumnFilters[vsFilterColumnKey].FilterConditions.Count > 0)
                {
                    roGrid.ActiveRow.Selected = false;
                    roGrid.ActiveRow = null;
                }
                SetGridWizardCaption(ref roGrid, vsCaptionColumnKey, vsCaption, vsFilterCaption);

            }
            catch (Exception)
            {

            }

        }
        internal static void SetGridWizardFilter(ref ucEasyGrid roGrid,
                                                   string vsFilterColumnKey, string vsFilterText,
                                                   string vsCaptionColumnKey = "", string vsCaption = "records",
                                                   string vsFilterCaption = "filtrati")
        {

            try
            {

                roGrid.Rows.ColumnFilters[vsFilterColumnKey].FilterConditions.Clear();
                if (vsFilterText != "")
                {
                    roGrid.Rows.ColumnFilters[vsFilterColumnKey].FilterConditions.Add(Infragistics.Win.UltraWinGrid.FilterComparisionOperator.Like, @"*" + vsFilterText + "*");
                }
                if (roGrid.Rows.FilteredInRowCount > 0 & roGrid.Rows.ColumnFilters[vsFilterColumnKey].FilterConditions.Count > 0)
                {
                    roGrid.ActiveRow.Selected = false;
                    roGrid.ActiveRow = roGrid.Rows.GetRowAtVisibleIndex(0);
                    roGrid.ActiveRow.Selected = true;
                }
                else if (roGrid.Rows.FilteredInRowCount == 0 & roGrid.Rows.ColumnFilters[vsFilterColumnKey].FilterConditions.Count > 0)
                {
                    roGrid.ActiveRow.Selected = false;
                    roGrid.ActiveRow = null;
                }
                SetGridWizardCaption(ref roGrid, vsCaptionColumnKey, vsCaption, vsFilterCaption);

            }
            catch (Exception)
            {

            }

        }

        internal static void SetGridWizardCaption(ref Infragistics.Win.UltraWinGrid.UltraGrid roGrid,
                                                   string vsCaptionColumnKey,
                                                   string vsCaption = "",
                                                   string vsFilterCaption = "filtrati")
        {

            try
            {

                if (roGrid.DisplayLayout.Bands[0].Columns.Exists(vsCaptionColumnKey) == true)
                {
                    roGrid.DisplayLayout.Bands[0].Columns[vsCaptionColumnKey].Header.Caption = roGrid.Rows.Count.ToString("#,##0");
                    if (vsCaption != "")
                    {
                        roGrid.DisplayLayout.Bands[0].Columns[vsCaptionColumnKey].Header.Caption += " " + vsCaption;
                    }
                    if (roGrid.Rows.FilteredInRowCount != roGrid.Rows.Count)
                    {
                        roGrid.DisplayLayout.Bands[0].Columns[vsCaptionColumnKey].Header.Caption += " (" + roGrid.Rows.FilteredInRowCount.ToString("#,##0");
                        if (vsFilterCaption != "")
                        {
                            roGrid.DisplayLayout.Bands[0].Columns[vsCaptionColumnKey].Header.Caption += " " + vsFilterCaption;
                        }
                        roGrid.DisplayLayout.Bands[0].Columns[vsCaptionColumnKey].Header.Caption += ")";
                    }
                }

            }
            catch (Exception)
            {

            }

        }
        internal static void SetGridWizardCaption(ref ucEasyGrid roGrid,
                                                   string vsCaptionColumnKey,
                                                   string vsCaption = "",
                                                   string vsFilterCaption = "filtrati")
        {

            try
            {

                if (roGrid.DisplayLayout.Bands[0].Columns.Exists(vsCaptionColumnKey) == true)
                {
                    roGrid.DisplayLayout.Bands[0].Columns[vsCaptionColumnKey].Header.Caption = roGrid.Rows.Count.ToString("#,##0");
                    if (vsCaption != "")
                    {
                        roGrid.DisplayLayout.Bands[0].Columns[vsCaptionColumnKey].Header.Caption += " " + vsCaption;
                    }
                    if (roGrid.Rows.FilteredInRowCount != roGrid.Rows.Count)
                    {
                        roGrid.DisplayLayout.Bands[0].Columns[vsCaptionColumnKey].Header.Caption += " (" + roGrid.Rows.FilteredInRowCount.ToString("#,##0");
                        if (vsFilterCaption != "")
                        {
                            roGrid.DisplayLayout.Bands[0].Columns[vsCaptionColumnKey].Header.Caption += " " + vsFilterCaption;
                        }
                        roGrid.DisplayLayout.Bands[0].Columns[vsCaptionColumnKey].Header.Caption += ")";
                    }
                }

            }
            catch (Exception)
            {

            }

        }

        internal static void SetEasyUltraDockManager(ref Infragistics.Win.UltraWinDock.UltraDockManager roDock)
        {
            try
            {
                roDock.AnimationEnabled = false;
                roDock.AnimationSpeed = Infragistics.Win.UltraWinDock.AnimationSpeed.StandardSpeedPlus5;

                roDock.AutoHideDelay = 150;
                roDock.CaptionButtonAlignment = Infragistics.Win.UltraWinDock.CaptionButtonAlignment.Near;
                roDock.CaptionStyle = Infragistics.Win.UltraWinDock.CaptionStyle.Office2007;
                roDock.CompressUnpinnedTabs = false;
                roDock.DefaultPaneSettings.AllowClose = Infragistics.Win.DefaultableBoolean.False;
                roDock.DefaultPaneSettings.AllowFloating = Infragistics.Win.DefaultableBoolean.False;
                roDock.ShowCloseButton = false;
                roDock.UnpinnedTabStyle = Infragistics.Win.UltraWinTabs.TabStyle.Office2007Ribbon;
                roDock.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
                roDock.WindowStyle = Infragistics.Win.UltraWinDock.WindowStyle.Office2007;
            }
            catch (Exception)
            {
            }
        }

        internal static void SetUltraToolbarsManager(ref UltraToolbarsManager UltraToolbars)
        {

            UltraToolbars.LockToolbars = true;
            UltraToolbars.RuntimeCustomizationOptions = RuntimeCustomizationOptions.None;
            UltraToolbars.Style = ToolbarStyle.Office2007;

            UltraToolbars.ToolbarSettings.AllowCustomize = Infragistics.Win.DefaultableBoolean.False;
            UltraToolbars.ToolbarSettings.AllowDockBottom = Infragistics.Win.DefaultableBoolean.False;
            UltraToolbars.ToolbarSettings.AllowDockLeft = Infragistics.Win.DefaultableBoolean.False;
            UltraToolbars.ToolbarSettings.AllowDockRight = Infragistics.Win.DefaultableBoolean.False;
            UltraToolbars.ToolbarSettings.AllowDockTop = Infragistics.Win.DefaultableBoolean.False;
            UltraToolbars.ToolbarSettings.AllowFloating = Infragistics.Win.DefaultableBoolean.False;
            UltraToolbars.ToolbarSettings.AllowHiding = Infragistics.Win.DefaultableBoolean.False;
            UltraToolbars.ToolbarSettings.FillEntireRow = Infragistics.Win.DefaultableBoolean.True;
            UltraToolbars.ToolbarSettings.PaddingBottom = 5;
            UltraToolbars.ToolbarSettings.PaddingLeft = 5;
            UltraToolbars.ToolbarSettings.PaddingRight = 5;
            UltraToolbars.ToolbarSettings.PaddingTop = 5;
            UltraToolbars.ToolbarSettings.ToolSpacing = 5;
            UltraToolbars.ToolbarSettings.UseLargeImages = Infragistics.Win.DefaultableBoolean.True;

            UltraToolbars.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;

        }

        internal static void SetResizeUltraToolbarsManager(ref UltraToolbarsManager UltraToolbars)
        {

            try
            {

                int nTools = 0;

                for (int x = 0; x <= UltraToolbars.Toolbars.Count - 1; x++)
                {
                    if (UltraToolbars.Toolbars[x].Tools.Count > nTools)
                    {
                        nTools = UltraToolbars.Toolbars[x].Tools.Count;
                        for (int y = 0; y <= UltraToolbars.Toolbars[x].Tools.Count - 1; y++)
                        {
                            if (UltraToolbars.Toolbars[x].Tools[y] is PopupControlContainerTool)
                            {
                                nTools += 1;
                            }
                        }
                    }
                }

                int nSize = Convert.ToInt32((double)UltraToolbars.DockWithinContainer.Width / nTools / 100 * 60);
                UltraToolbars.ImageSizeLarge = new Size(nSize, nSize);

            }
            catch (Exception)
            {

            }

        }

        internal static string UltraTreeFindKeyByDescription(ref UnicodeSrl.ScciCore.ucEasyTreeView roUltraTree, string vsdescription)
        {

            string sret = "";

            try
            {
                foreach (UltraTreeNode oNode in roUltraTree.Nodes)
                {
                    sret = FindInNode(oNode, vsdescription);
                    if (sret != "") break;
                }
            }
            catch (Exception)
            {
            }

            return sret;

        }

        private static string FindInNode(UltraTreeNode roNode, string vsdescription)
        {

            string sret = "";

            try
            {
                if (roNode.Nodes.Count > 0)
                    foreach (UltraTreeNode oNode in roNode.Nodes)
                    {
                        sret = FindInNode(oNode, vsdescription);
                        if (sret != "") break;
                    }
                else
                    if (roNode.Text.ToUpper().Contains(vsdescription.ToUpper()))
                {
                    sret = roNode.Key;
                }

            }
            catch (Exception)
            {
            }

            return sret;
        }

        public static void SelezionaItemInComboEditor(ref ucEasyComboEditor combo, string valore)
        {
            try
            {
                int selindex = 0;
                for (int i = 0; i < combo.Items.Count; i++)
                {
                    if (combo.Items[i].DataValue.ToString().ToUpper() == valore.ToUpper()) selindex = i;
                }
                combo.SelectedIndex = selindex;
            }
            catch
            {
                combo.SelectedIndex = 0;
            }
        }

        public static void SelezionaRigaInGriglia(ref ucEasyGrid griglia, string nomecolonna, string valore)
        {
            object item = null;
            if (valore != string.Empty && valore.Trim() != "")
            {
                try
                {
                    item = griglia.Rows.Single(UltraGridRow => UltraGridRow.Cells[nomecolonna].Value.ToString().Trim() == valore);
                }
                catch
                {
                }
                if (item != null)
                    griglia.ActiveRow = (UltraGridRow)item;
                else
                {
                    griglia.ActiveRow = null;
                }
            }
        }
        public static bool SelezionaRigaInGriglia(ref RowsCollection rows, string nomecolonna, string valore)
        {
            bool bSelected = false;
            try
            {
                if (valore != string.Empty && valore.Trim() != "")
                {
                    for (int iRow = 0; iRow < rows.Count; iRow++)
                    {
                        UltraGridRow row = rows[iRow];
                        if (row.IsGroupByRow)
                        {
                            if (row.ChildBands.Count > 0 && row.ChildBands[0].Rows.Count > 0)
                            {
                                RowsCollection childrows = row.ChildBands[0].Rows;
                                bSelected = SelezionaRigaInGriglia(ref childrows, nomecolonna, valore);
                            }
                        }
                        else if (row.IsDataRow && !row.IsDeleted)
                        {
                            if (row.Cells[nomecolonna].Text == valore)
                            {
                                row.Activate();
                                ((UltraGrid)row.Band.Layout.Grid).Selected.Rows.Clear();
                                ((UltraGrid)row.Band.Layout.Grid).Selected.Rows.Add(row);
                                bSelected = true;
                            }

                        }
                        if (bSelected) iRow = rows.Count + 1;
                    }
                }

            }
            catch (Exception)
            {
            }
            return bSelected;
        }

        public static void ImpostaGroupByGriglia(ref ucEasyGrid griglia, ref Graphics g, string colonnadaraggruppare)
        {
            ImpostaGroupByGriglia(ref griglia, ref g, colonnadaraggruppare, true, easyStatics.easyRelativeDimensions.Medium);
        }
        public static void ImpostaGroupByGriglia(ref ucEasyGrid griglia, ref Graphics g, string colonnadaraggruppare, easyStatics.easyRelativeDimensions sizeinpoint)
        {
            ImpostaGroupByGriglia(ref griglia, ref g, colonnadaraggruppare, true, sizeinpoint);
        }
        public static void ImpostaGroupByGriglia(ref ucEasyGrid griglia, ref Graphics g, string colonnadaraggruppare, bool expandall)
        {
            ImpostaGroupByGriglia(ref griglia, ref g, colonnadaraggruppare, expandall, easyStatics.easyRelativeDimensions.Medium);
        }
        public static void ImpostaGroupByGriglia(ref ucEasyGrid griglia, ref Graphics g, string colonnadaraggruppare, bool expandall, easyStatics.easyRelativeDimensions sizeinpoint)
        {

            griglia.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            griglia.DisplayLayout.GroupByBox.Hidden = true;
            if (griglia.DisplayLayout.Bands[0].SortedColumns.Count <= 0)
            {
                try
                {
                    griglia.DisplayLayout.Bands[0].SortedColumns.Add(colonnadaraggruppare, false, true);
                    griglia.DisplayLayout.Bands[0].SortedColumns[colonnadaraggruppare].GroupByRowAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(sizeinpoint);

                    griglia.DisplayLayout.Bands[0].SortedColumns[colonnadaraggruppare].GroupByRowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Opaque;
                    griglia.DisplayLayout.Bands[0].SortedColumns[colonnadaraggruppare].GroupByRowAppearance.BackColor = Color.FromArgb(219, 207, 233);

                }
                catch (Exception)
                {
                }
            }
            griglia.DisplayLayout.Bands[0].Override.ExpansionIndicator = Infragistics.Win.UltraWinGrid.ShowExpansionIndicator.Never;
            griglia.DisplayLayout.Bands[0].Override.GroupByRowExpansionStyle = GroupByRowExpansionStyle.Disabled;
            griglia.DisplayLayout.Bands[0].Override.GroupByRowPadding = CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small), g.DpiY);
            griglia.DisplayLayout.Bands[0].Override.GroupByRowSpacingBefore = CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Medium), g.DpiY);
            griglia.DisplayLayout.Bands[0].Override.GroupByRowDescriptionMask = "[Value]";

            if (expandall)
            {
                griglia.Rows.ExpandAll(true);
            }
            else
            {
                griglia.Rows.CollapseAll(true);
            }

        }

        internal static void SetEasyucMultiSelect(ref ucMultiSelect roMultiSelect, easyStatics.easyRelativeDimensions fontrelativedimension)
        {

            try
            {

                float fontsize = easyStatics.getFontSizeInPointsFromRelativeDimension(fontrelativedimension);

                roMultiSelect.lblCercaDX.Font = new Font(roMultiSelect.lblCercaDX.Font.FontFamily.Name, fontsize);
                roMultiSelect.lblCercaSX.Font = new Font(roMultiSelect.lblCercaDX.Font.FontFamily.Name, fontsize);
                roMultiSelect.uteCercaDX.AutoSize = false;
                roMultiSelect.uteCercaSX.AutoSize = false;
                roMultiSelect.uteCercaDX.Appearance.FontData.SizeInPoints = fontsize;
                roMultiSelect.uteCercaSX.Appearance.FontData.SizeInPoints = fontsize;
                roMultiSelect.ubCancella.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2003ToolbarButton;
                roMultiSelect.ubCancellaAll.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2003ToolbarButton;
                roMultiSelect.ubInserisci.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2003ToolbarButton;
                roMultiSelect.ubInserisciAll.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2003ToolbarButton;


            }
            catch (Exception ex)
            {
                throw new Exception("CoreStatics.SetEasyucMultiSelect(): " + ex.Message, ex);
            }
        }

        internal static void SetEasyucMultiSelectGridInitializeLayout(ref Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e, easyStatics.easyRelativeDimensions fontrelativedimension)
        {
            e.Layout.Override.HeaderAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(fontrelativedimension);
            e.Layout.CaptionAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(fontrelativedimension);
            e.Layout.Override.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(fontrelativedimension);
            e.Layout.Override.FilterRowAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(fontrelativedimension);

            e.Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            e.Layout.Appearance.BackColor = Color.FromKnownColor(KnownColor.Window);
            e.Layout.Appearance.BorderColor = Color.FromKnownColor(KnownColor.InactiveCaption);


            e.Layout.GroupByBox.Hidden = true;
            e.Layout.GroupByBox.Prompt = @"Trascina un'intestazione della colonna qui per raggrupparla.";
            e.Layout.GroupByBox.Appearance.BackColor = Color.LightSteelBlue;
            e.Layout.GroupByBox.Appearance.BackColor2 = Color.Lavender;
            e.Layout.GroupByBox.Appearance.BackGradientStyle = Infragistics.Win.GradientStyle.ForwardDiagonal;
            e.Layout.GroupByBox.PromptAppearance.BackColor = Color.Lavender;
            e.Layout.GroupByBox.PromptAppearance.BackColor2 = Color.Lavender;
            e.Layout.GroupByBox.PromptAppearance.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            e.Layout.GroupByBox.PromptAppearance.ForeColor = Color.Black;

            e.Layout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            e.Layout.CaptionAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
            e.Layout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;

            e.Layout.Override.ActiveCellAppearance.BackColor = Color.FromKnownColor(KnownColor.Window);
            e.Layout.Override.ActiveCellAppearance.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
            e.Layout.Override.ActiveRowAppearance.BackColor = Color.FromKnownColor(KnownColor.Highlight);
            e.Layout.Override.ActiveRowAppearance.ForeColor = Color.FromKnownColor(KnownColor.HighlightText);

            e.Layout.Override.AllowAddNew = AllowAddNew.No;
            e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            e.Layout.Override.AllowColMoving = AllowColMoving.NotAllowed;
            e.Layout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;

            e.Layout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            e.Layout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;

            e.Layout.Override.CellAppearance.BorderColor = Color.Silver;
            e.Layout.Override.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            e.Layout.Override.ColumnAutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.SiblingRowsOnly;

            e.Layout.Override.HeaderAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
            e.Layout.Override.HeaderAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
            e.Layout.Override.HeaderClickAction = HeaderClickAction.SortMulti;
            e.Layout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsVista;

            e.Layout.Override.RowAlternateAppearance.BackColor = Color.WhiteSmoke;
            e.Layout.Override.RowAlternateAppearance.ForeColor = Color.DarkBlue;
            e.Layout.Override.RowAppearance.BackColor = Color.FromKnownColor(KnownColor.Window);
            e.Layout.Override.RowAppearance.BorderColor = Color.Silver;
            e.Layout.Override.RowAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;

            e.Layout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            e.Layout.Override.RowSizing = Infragistics.Win.UltraWinGrid.RowSizing.AutoFree;

            e.Layout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;

            e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            e.Layout.ScrollStyle = ScrollStyle.Immediate;
            e.Layout.TabNavigation = Infragistics.Win.UltraWinGrid.TabNavigation.NextControl;

            e.Layout.ScrollBarLook.ViewStyle = Infragistics.Win.UltraWinScrollBar.ScrollBarViewStyle.Office2010;
            e.Layout.ViewStyleBand = ViewStyleBand.OutlookGroupBy;

            e.Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            e.Layout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
        }

        public static void SetUltraDayView(UltraDayView DayView)
        {
            SetUltraDayView(DayView, TimeSlotInterval.TenMinutes);
        }
        internal static void SetUltraDayView(UltraDayView DayView, TimeSlotInterval oTimeSlotInterval)
        {

            DayView.AllowDrop = true;
            DayView.AutoAppointmentDialog = false;
            DayView.DayTextFormat = "dd MMMM yyyy";
            DayView.GroupingStyle = DayViewGroupingStyle.DateWithinOwner;
            DayView.ScrollbarVisible = true;
            DayView.ShowClickToAddIndicator = Infragistics.Win.DefaultableBoolean.False;
            DayView.TimeSlotInterval = oTimeSlotInterval;
            DayView.TimeSlotDescriptorLabelStyle = TimeSlotDescriptorLabelStyle.EveryTimeSlot;
            DayView.CreationFilter = new HeaderLongDateCreationFilter();

        }

        public static void SetUltraWeekView(UltraWeekView WeekView)
        {

            WeekView.AllowDrop = true;
            WeekView.AppointmentEndTimeVisible = true;
            WeekView.AutoAppointmentDialog = false;
            WeekView.MaximumOwnersInView = 2;
            WeekView.OwnerDisplayStyle = OwnerDisplayStyle.Separate;
            WeekView.OwnerNavigationStyle = OwnerNavigationStyle.Scrollbar;
            WeekView.ScrollbarVisible = false;
            WeekView.ShowClickToAddIndicator = Infragistics.Win.DefaultableBoolean.False;
            WeekView.ShowOwnerHeader = Infragistics.Win.DefaultableBoolean.True;
            WeekView.TimeDisplayStyle = TimeDisplayStyleEnum.Time24Hour;
            WeekView.CreationFilter = new HeaderLongDateCreationFilter();

        }

        public static void SetUltraMonthViewSingle(UltraMonthViewSingle MonthViewSingle)
        {

            MonthViewSingle.AllowDrop = true;
            MonthViewSingle.AppointmentEndTimeVisible = true;
            MonthViewSingle.AutoAppointmentDialog = false;
            MonthViewSingle.DayDisplayStyle = DayDisplayStyleEnum.Full;
            MonthViewSingle.MaximumOwnersInView = 2;
            MonthViewSingle.OwnerDisplayStyle = OwnerDisplayStyle.Separate;
            MonthViewSingle.OwnerNavigationStyle = OwnerNavigationStyle.Scrollbar;
            MonthViewSingle.ScrollbarVisible = false;
            MonthViewSingle.ShowClickToAddIndicator = Infragistics.Win.DefaultableBoolean.False;
            MonthViewSingle.ShowOwnerHeader = Infragistics.Win.DefaultableBoolean.True;
            MonthViewSingle.TimeDisplayStyle = TimeDisplayStyleEnum.Time24Hour;
            MonthViewSingle.CreationFilter = new HeaderLongDateCreationFilter();

        }

        internal static void SetUltraMonthViewMulti(UltraMonthViewMulti MonthViewMulti)
        {

            MonthViewMulti.Appearance.FontData.SizeInPoints = 9;
            MonthViewMulti.MonthScrollButtonAppearance.FontData.SizeInPoints = 12;
            MonthViewMulti.MonthPopupAppearance.FontData.SizeInPoints = 12;

        }

        public static void SetUltraCalendarInfo(UltraCalendarInfo CalendarInfo)
        {

            CalendarInfo.Owners.UnassignedOwner.Visible = false;
            CalendarInfo.SelectTypeActivity = Infragistics.Win.UltraWinSchedule.SelectType.Single;

            CalendarInfo.DaysOfWeek[System.DayOfWeek.Saturday].IsWorkDay = true;
            CalendarInfo.DaysOfWeek[System.DayOfWeek.Sunday].IsWorkDay = true;

            Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
            SqlParameterExt[] spcoll = new SqlParameterExt[1];
            string xmlParam = XmlProcs.XmlSerializeToString(op);
            spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
            DataSet oDsFestivita = Database.GetDatasetStoredProc("MSP_SelFestivita", spcoll);

            CalendarInfo.Holidays.Clear();
            foreach (DataRow odr in oDsFestivita.Tables[0].Rows)
            {
                CalendarInfo.Holidays.Add((DateTime)odr["Data"], odr["Descrizione"].ToString());
            }

        }

        public static void SetUltraCalendarLook(UltraCalendarLook CalendarLook)
        {

            CalendarLook.DayWithActivityAppearance.BackColor = Color.LightGreen;
            CalendarLook.HolidayAppearance.BackColor = CoreStatics.GetColorFromString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.ColoreFestivitaCalendari));
            CalendarLook.ScrollBarLook.ViewStyle = Infragistics.Win.UltraWinScrollBar.ScrollBarViewStyle.Default;
            CalendarLook.ViewStyle = Infragistics.Win.UltraWinSchedule.ViewStyle.Office2003;

        }

        public static void SetUltraSchedulePrintDocument(UltraSchedulePrintDocument SchedulePrintDocument)
        {

            SchedulePrintDocument.DefaultPageSettings.Margins.Top = 5;

            SchedulePrintDocument.DefaultPageSettings.Margins.Bottom = 5;
            SchedulePrintDocument.DefaultPageSettings.Margins.Left = 5;
            SchedulePrintDocument.DefaultPageSettings.Margins.Right = 5;

            SchedulePrintDocument.Header.Margins.Top = 0;
            SchedulePrintDocument.Header.Margins.Left = 5;
            SchedulePrintDocument.Header.Margins.Right = 30;
            SchedulePrintDocument.Header.Margins.Bottom = 0;
            SchedulePrintDocument.Header.Appearance.FontData.Name = "Tahoma";
            SchedulePrintDocument.Header.TextCenter = "Data di stampa : " + DateAndTime.Now.ToLongDateString() + " " + DateAndTime.Now.ToLongTimeString();

            SchedulePrintDocument.PageBody.Margins.Top = 0;
            SchedulePrintDocument.PageBody.Margins.Left = 0;
            SchedulePrintDocument.PageBody.Margins.Right = 30;
            SchedulePrintDocument.PageBody.Margins.Bottom = 0;

            SchedulePrintDocument.TriFoldLayoutStyle = TriFoldLayoutStyle.PagePerOwner;
            SchedulePrintDocument.CreationFilter = new HeaderLongDateCreationFilter();

        }

        public static void SetUltraPopupControlContainer(UltraPopupControlContainer PopupControlContainer)
        {

            PopupControlContainer.DropDownResizeHandleStyle = Infragistics.Win.DropDownResizeHandleStyle.DiagonalResize;
            PopupControlContainer.DropDownResizeHandleAppearance.BackColor = Color.SkyBlue;

        }

        internal static void SetUltraTabControl(Infragistics.Win.UltraWinTabControl.UltraTabControl TabControl)
        {
            TabControl.ViewStyle = Infragistics.Win.UltraWinTabControl.ViewStyle.Office2007;
            if (TabControl.Tabs.Count > 0)
                TabControl.Tabs[0].Visible = true;
        }

        internal static void GCCollectUltraGrid(ref ucEasyGrid roGrid)
        {
            roGrid.DataSource = null;
            roGrid.Refresh();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        #endregion

        #region Image

        public static string ImageToBase64(Image image, ImageFormat format)
        {

            try
            {
                MemoryStream ms = new MemoryStream();
                image.Save(ms, format);
                Byte[] imageBytes = ms.ToArray();
                String base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
            catch (Exception)
            {
                return @"";
            }

        }

        public static Image Base64ToImage(String base64String)
        {

            try
            {

                Byte[] imageBytes = Convert.FromBase64String(base64String);
                MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
                ms.Write(imageBytes, 0, imageBytes.Length);
                Image image = Image.FromStream(ms, true);
                return image;

            }
            catch (Exception)
            {
                return null;
            }

        }

        public static Image ByteToImage(Byte[] imageBytes)
        {

            try
            {

                MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
                ms.Write(imageBytes, 0, imageBytes.Length);
                Image image = Image.FromStream(ms, true);
                return image;

            }
            catch (Exception)
            {
                return null;
            }

        }

        public static byte[] ImageToByte(Image image)
        {

            try
            {

                ImageConverter _imageConverter = new ImageConverter();
                byte[] xByte = (byte[])_imageConverter.ConvertTo(image, typeof(byte[]));
                return xByte;

            }
            catch (Exception)
            {
                return null;
            }

        }

        public static Image GetImageFromMaschera(EnumMaschere maschera)
        {

            try
            {

                switch (maschera)
                {
                    case EnumMaschere.MenuPrincipale:
                        return null;

                    case EnumMaschere.WorklistInfermieristicaTrasversale:
                        return ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_WORKLIST_32);

                    case EnumMaschere.EvidenzaClinicaTrasversale:
                        return ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_EVIDENZACLINICA_32);

                    case EnumMaschere.ParametriVitaliTrasversali:
                        return ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_PARAMETRIVITALI_32);

                    case EnumMaschere.RicercaPazienti:
                        return ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_PAZIENTI_32);

                    case EnumMaschere.CartellaPaziente:
                        return ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_CARTELLACLINICA_32);

                    case EnumMaschere.Ambulatoriale_Schede:
                    case EnumMaschere.Schede:
                        return ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_SCHEDA_32);

                    case EnumMaschere.DiarioClinico:
                        return ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_DIARIOMEDICO_32);

                    case EnumMaschere.ParametriVitali:
                        return ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_PARAMETRIVITALI_32);

                    case EnumMaschere.EvidenzaClinica:
                        return ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_EVIDENZACLINICA_32);

                    case EnumMaschere.Allegati:
                        return ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_ALLEGATI_32);

                    case EnumMaschere.TerapieFarmacologiche:
                        return ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_PRESCRIZIONE_32);

                    case EnumMaschere.WorklistInfermieristica:
                        return ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_WORKLIST_32);

                    case EnumMaschere.Ambulatoriale_AgendePaziente:
                    case EnumMaschere.AgendeTrasversali:
                    case EnumMaschere.AgendePaziente:
                        return ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_AGENDA_32);

                    case EnumMaschere.TrackerPaziente:
                        return ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_TRACKER_256);

                    case EnumMaschere.Ambulatoriale_GestioneOrdini:
                    case EnumMaschere.GestioneOrdini:
                        return ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_ORDINE_32);

                    case EnumMaschere.LetteraDiDimissione:
                        return ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_LETTERA_DIMISSIONE_32);

                    case EnumMaschere.Ambulatoriale_EBM:
                    case EnumMaschere.EBM:
                        return ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_EBM_32);

                    case EnumMaschere.FoglioUnico:
                        return ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_FOGLIOUNICO_32);

                    case EnumMaschere.Consulenze_Login:
                    case EnumMaschere.Consulenze_Refertazione:
                    case EnumMaschere.Consulenze_RicercaPaziente:
                        return ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_CONSULENZA_32);

                    case EnumMaschere.Ambulatoriale_RicercaPaziente:
                        return ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_PAZIENTI_32);

                    case EnumMaschere.PreTrasferimento_RicercaPaziente:
                        return ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_PAZIENTI_32);

                    case EnumMaschere.Ambulatoriale_Cartella:
                        return ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_CARTELLACLINICA_32);

                    case EnumMaschere.Ambulatoriale_Allegati:
                        return ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_ALLEGATI_32);

                    case EnumMaschere.Ambulatoriale_EvidenzaClinica:
                        return ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_EVIDENZACLINICA_32);

                    case EnumMaschere.Mow:
                    case EnumMaschere.Ambulatoriale_Mow:
                        return ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_MOW_32);

                    case EnumMaschere.Psc:
                        return ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_FARMACI_32);

                    default:
                        return null;

                }

            }
            catch (Exception)
            {
                return null;
            }

        }

        public static int PointToPixel(float pointdimension, float dpix)
        {
            return (int)Math.Round(pointdimension / 72 * dpix, 0);
        }

        public static byte[] GetImageForGrid(int idicona, int formato)
        {

            byte[] data = null;

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDIcona", idicona.ToString());
                op.Parametro.Add("Formato", formato.ToString());

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_SelIconaDaID", spcoll);
                if (ds != null && ds.Tables.Count == 1 && ds.Tables[0].Rows.Count == 1 && ds.Tables[0].Rows[0][1] != System.DBNull.Value)
                {
                    data = (byte[])ds.Tables[0].Rows[0][1];
                }
                else
                {
                    MemoryStream ms = new MemoryStream();
                    Bitmap oBitmap = new Bitmap(formato, formato);
                    oBitmap.Save(ms, ImageFormat.Png);
                    data = ms.ToArray();
                }
                return data;

            }
            catch (Exception)
            {
                return null;
            }

        }

        public static Image resizeImage(Image imgToResize, Size size)
        {
            return (Image)(new Bitmap(imgToResize, size));
        }

        #endregion

        #region DataSet



        public static DataTable CreateDataTable<T>()
        {

            var dt = new DataTable();

            var propList = typeof(T).GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (MemberInfo info in propList)
            {
                if (info is PropertyInfo)
                    dt.Columns.Add(new DataColumn(info.Name, (info as PropertyInfo).PropertyType));
                else if (info is FieldInfo)
                    dt.Columns.Add(new DataColumn(info.Name, (info as FieldInfo).FieldType));
            }

            return dt;
        }

        public static void FillDataTable<T>(DataTable dt, List<T> items)
        {

            var propList = typeof(T).GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (T t in items)
            {
                var row = dt.NewRow();
                foreach (MemberInfo info in propList)
                {
                    if (info is PropertyInfo)
                        row[info.Name] = (info as PropertyInfo).GetValue(t, null);
                    else if (info is FieldInfo)
                        row[info.Name] = (info as FieldInfo).GetValue(t);
                }
                dt.Rows.Add(row);
            }
        }

        public static DataTable AggiungiTuttiDataTable(DataTable dt, bool bSort)
        {

            try
            {
                string sortcolumnname = "";
                if (bSort) sortcolumnname = "Codice";
                return AggiungiTuttiDataTable(dt, sortcolumnname);

            }
            catch (Exception)
            {
                return dt;
            }

        }

        public static DataTable AggiungiTuttiDataTable(DataTable dt, string sortColumnName)
        {

            try
            {
                bool bAdd = true;
                try
                {
                    dt.DefaultView.RowFilter = dt.Columns[0].ColumnName + @" = ' " + GC_TUTTI + @"'";
                    bAdd = (dt.DefaultView.Count <= 0);
                    dt.DefaultView.RowFilter = @"";
                }
                catch (Exception)
                {
                }

                if (bAdd)
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = " " + GC_TUTTI;
                    dr[1] = " " + GC_TUTTI;
                    dt.Rows.InsertAt(dr, 0);
                }
                if (sortColumnName != null && sortColumnName != string.Empty && sortColumnName.Trim() != "")
                {
                    if (dt.Columns.Contains(sortColumnName))
                        dt.DefaultView.Sort = sortColumnName;
                }

                return dt.DefaultView.ToTable(true);

            }
            catch (Exception)
            {
                return dt;
            }

        }

        #endregion

        #region Variabili per Report

        public static DataTable getVariables(string xml)
        {
            DataTable dtReturn = new DataTable();

            if (xml != "")
            {
                XmlSerializer mySerializer = new XmlSerializer(dtReturn.GetType());

                System.IO.MemoryStream ms = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(xml));
                dtReturn = mySerializer.Deserialize(ms) as DataTable;
            }

            return dtReturn;
        }

        public static string getVariablesXML(DataTable dtVariables)
        {
            string xml = "";

            if (dtVariables != null)
            {
                XmlSerializer mySerializer = new XmlSerializer(dtVariables.GetType());
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                mySerializer.Serialize(ms, dtVariables);
                xml = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            }

            return xml;
        }

        #endregion

        #region Stampe / Reports / Stampa diretta


        public static bool stampaMultiplaDiretta(ref List<Report> rlstReports)
        {
            bool bReturn = false;
            PrintDialog dialog = null;
            try
            {

                if (rlstReports != null && rlstReports.Count > 0)
                {
                    bReturn = true;

                    if (bReturn)
                    {
                        dialog = new PrintDialog();
                        dialog.AllowCurrentPage = false;
                        dialog.AllowSomePages = false;
                        dialog.UseEXDialog = true;
                        DialogResult result = dialog.ShowDialog();
                        bReturn = (result == DialogResult.OK);

                    }

                    if (bReturn)
                    {

                        bReturn = false;

                        Gestore oGestore = null;


                        for (int r = 0; r < rlstReports.Count; r++)
                        {
                            Report currReport = rlstReports[r];

                            switch (currReport.CodFormatoReport)
                            {
                                case Report.COD_FORMATO_REPORT_WORD:

                                    if (caricaEStampaWord(ref currReport, ref oGestore, ref dialog))
                                        bReturn = true;

                                    break;

                                case Report.COD_FORMATO_REPORT_CABLATO:

                                    if (caricaEStampaCablato(ref currReport, ref dialog))
                                        bReturn = true;

                                    break;

                                default:
                                    break;

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bReturn = false;
                ExGest(ref ex, @"stampaMultiplaDiretta", @"CoreStatics");
            }

            return bReturn;
        }

        public static bool caricaEStampaWord(ref Report reportWord,
ref Gestore roGestore,
ref PrintDialog rprintDialog)
        {
            bool bReturn = false;
            string docxdacancellare = "";
            try
            {
                if (reportWord.Modello == null || reportWord.Modello.Length <= 0)
                    easyStatics.EasyMessageBox(@"Impossibile recuperare il modello per " + reportWord.Descrizione + @"!", "Stampa Word", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                {

                    if (roGestore == null) roGestore = CoreStatics.GetGestore(true);

                    DocxProcs.CreaDocxReturn ret = DocxProcs.CreaReportDOCX(reportWord.Modello, roGestore);

                    if (ret.docxgeneratofullpath != null && ret.docxgeneratofullpath != string.Empty && ret.docxgeneratofullpath.Trim() != "" && System.IO.File.Exists(ret.docxgeneratofullpath))
                    {
                        bReturn = true;
                        docxdacancellare = ret.docxgeneratofullpath;

                        if (ret.errori != null && ret.errori != string.Empty && ret.errori.Trim() != "")
                            easyStatics.EasyMessageBox(@"Documento " + reportWord.Descrizione + @" generato con errori:" + Environment.NewLine + ret.errori, "Stampa Word", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        using (DevExpress.XtraPdfViewer.PdfViewer pdfviewer = new DevExpress.XtraPdfViewer.PdfViewer())
                        {
                            pdfviewer.LoadDocument(easyStatics.getPathDocumentDE(ret.docxgeneratofullpath));
                            DevExpress.Pdf.PdfPrinterSettings pps = new DevExpress.Pdf.PdfPrinterSettings(rprintDialog.PrinterSettings);
                            pps.EnableLegacyPrinting = true;
                            pdfviewer.Print(pps);
                        }

                        DBUtils.storicizzaReport(reportWord.Codice, ret.docxgeneratofullpath, reportWord.DaStoricizzare);


                    }
                    else
                        easyStatics.EasyMessageBox(@"Impossibile generare il documento " + reportWord.Descrizione + @"!" + Environment.NewLine + ret.errori, "Stampa Word", MessageBoxButtons.OK, MessageBoxIcon.Warning);


                }
            }
            catch (Exception ex)
            {
                bReturn = false;
                ExGest(ref ex, @"caricaEStampaWord", @"CoreStatics");
            }


            try
            {
                if (docxdacancellare != null && docxdacancellare.Trim() != "" && System.IO.File.Exists(docxdacancellare))
                    System.IO.File.Delete(docxdacancellare);
            }
            catch
            {
            }

            return bReturn;
        }

        public static bool caricaEStampaCablato(ref Report reportCAB,
ref PrintDialog rprintDialog)
        {

            bool bReturn = true;
            bool bPDFCartellaChiusa = false;
            PluginCaller caller = null;
            string pdffullpath = "";

            try
            {
                bReturn = controlloPreliminareParametriPlugin(reportCAB, true, ref rprintDialog, out bPDFCartellaChiusa);

                if (bReturn)
                {

                    bReturn = false;

                    if (reportCAB.NomePlugIn == null || reportCAB.NomePlugIn.Trim() == "")
                        easyStatics.EasyMessageBox(@"Impossibile recuperare il nome del plugin!", "Stampa Plugin", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    else
                    {

                        Dictionary<string, object> oDictionary = getDictionaryParametriPlugin(reportCAB);
                        rprintDialog = new PrintDialog();
                        rprintDialog.AllowSomePages = false;
                        rprintDialog.AllowSelection = false;
                        rprintDialog.AllowCurrentPage = false;
                        oDictionary.Add("PrinterSettings", rprintDialog.PrinterSettings);

                        T_CDSSPlugins oCDSSPlugins = new T_CDSSPlugins();
                        oCDSSPlugins.Codice = reportCAB.Codice;
                        if (oCDSSPlugins.TrySelect())
                        {

                            Plugin oPlugin = new Plugin(oCDSSPlugins.Codice, oCDSSPlugins.Descrizione, oCDSSPlugins.NomePlugin, oCDSSPlugins.Comando, "", "", 0, null);
                            object[] myparam = new object[1] { oDictionary };

                            Risposta oRisposta = PluginClientStatics.PluginClientMenuEsegui(oPlugin, myparam);

                            if (oRisposta.Parameters == null)
                            {
                                switch (reportCAB.Codice)
                                {

                                    case Report.COD_REPORT_PDF_TUTTI_REFERTI:
                                        easyStatics.EasyMessageBox("Nessun referto da stampare.", "Stampa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        break;

                                    default:
                                        easyStatics.EasyMessageBox("Stampa non generata correttamente.", "Stampa", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                        break;
                                }
                                bReturn = false;
                            }
                            else
                            {

                                switch (reportCAB.Codice)
                                {
                                    case Report.COD_REPORT_PDF_TUTTI_REFERTI:
                                        bool bOK = false;
                                        if (oRisposta.Parameters.Length == 1 && oRisposta.Parameters[0].GetType() == typeof(string))
                                        {

                                            string spdf = oRisposta.Parameters[0] as string;
                                            if (System.IO.File.Exists(spdf))
                                            {
                                                bOK = true;
                                                bool bCaricamentoViaShell = false;
                                                string sAR = UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.ParolaChiaveAperturaAcrobat);
                                                if (sAR != null && sAR != string.Empty && sAR != "" && reportCAB.Descrizione.IndexOf(sAR) >= 0) bCaricamentoViaShell = true;
                                                easyStatics.ShellExecute(spdf, "", bCaricamentoViaShell, reportCAB.Codice, false);
                                            }

                                        }
                                        if (!bOK)
                                        {
                                            easyStatics.EasyMessageBox(@"Impossibile recuperare i referti!", "Stampa Referti", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                        }
                                        break;

                                    case Report.COD_REPORT_RPTWKIANCI:
                                        if (oRisposta.Successo && oRisposta.Parameters.Length == 1 && oRisposta.Parameters[0].GetType() == typeof(string))
                                        {
                                            string spdf = oRisposta.Parameters[0] as string;
                                            if (System.IO.File.Exists(spdf))
                                            {
                                                easyStatics.ShellExecute(spdf, "", false, reportCAB.Codice, false);
                                            }
                                        }
                                        else if (oRisposta.Parameters.Length == 1 && oRisposta.Parameters[0].GetType() == typeof(string))
                                        {
                                            easyStatics.EasyMessageBox(oRisposta.Parameters[0].ToString(), "Stampa Etichette", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                        }
                                        break;

                                    default:
                                        if (oRisposta.Parameters.Length == 1 && oRisposta.Parameters[0].GetType() == typeof(string))
                                        {
                                            easyStatics.EasyMessageBox(oRisposta.Parameters[0] as string, "Stampa", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                        }
                                        else
                                        {


                                            DevExpress.XtraReports.UI.XtraReport oar = (DevExpress.XtraReports.UI.XtraReport)oRisposta.Parameters[0];

                                            using (DevExpress.XtraPrinting.Preview.DocumentViewer oViewer = new DevExpress.XtraPrinting.Preview.DocumentViewer())
                                            {
                                                oViewer.Visible = false;
                                                oViewer.Name = "tmpViewer";

                                                oViewer.DocumentSource = oar;

                                                oViewer.Refresh();






                                                ReportPrintTool printTool = new ReportPrintTool(oar);
                                                printTool.Print();

                                                if (reportCAB.DaStoricizzare == true)
                                                {

                                                    pdffullpath = esportaActiveReportPDF(ref oar);

                                                    DBUtils.storicizzaReport(reportCAB.Codice, pdffullpath, reportCAB.DaStoricizzare);
                                                }

                                            }

                                            if (bReturn)
                                            {
                                                switch (reportCAB.Codice)
                                                {
                                                    case Report.COD_REPORT_CARTELLA_PAZIENTE:

                                                        if (CoreStatics.CoreApplication.Cartella.PDFCartellaAggiornabile)
                                                        {
                                                            if (pdffullpath == null || pdffullpath == string.Empty || pdffullpath.Trim() == "" || !System.IO.File.Exists(pdffullpath))
                                                            {
                                                                pdffullpath = esportaActiveReportPDF(ref oar);
                                                            }

                                                            if (pdffullpath != null && pdffullpath != string.Empty && pdffullpath.Trim() != "" && System.IO.File.Exists(pdffullpath))
                                                            {
                                                                CoreStatics.CoreApplication.Cartella.archiviaPDF(pdffullpath);
                                                            }

                                                        }

                                                        break;

                                                    default:
                                                        break;
                                                }
                                            }

                                            try
                                            {
                                                if (pdffullpath != null && pdffullpath != string.Empty && pdffullpath.Trim() != "" && System.IO.File.Exists(pdffullpath))
                                                    System.IO.File.Delete(pdffullpath);
                                            }
                                            catch (Exception)
                                            {
                                            }

                                        }
                                        break;
                                }

                            }
                        }

                    }
                }
                else
                    bReturn = bPDFCartellaChiusa;

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "caricaEStampaCablato", "CoreStatics");
                bReturn = false;
            }
            finally
            {
                if (caller != null) caller.Dispose();
            }
            return bReturn;
        }

        public static Dictionary<string, object> getDictionaryParametriPlugin(Report rep)
        {

            Dictionary<string, object> dictReturn = new Dictionary<string, object>();

            switch (rep.Codice)
            {
                case Report.COD_REPORT_CARTELLA_PAZIENTE:
                    dictReturn.Add("StringaConnessione", Database.ConnectionString);
                    dictReturn.Add("NumeroCartella", CoreStatics.CoreApplication.Cartella.NumeroCartella);
                    dictReturn.Add("IDCartella", CoreStatics.CoreApplication.Cartella.ID);
                    dictReturn.Add("XmlAmbiente", CoreStatics.CoreApplication.Ambiente.XmlSerializeToString());
                    dictReturn.Add("FirmaDigitale", @"0");
                    dictReturn.Add("UtenteFirma", CoreStatics.CoreApplication.Sessione.Utente.Descrizione);

                    break;

                case Report.COD_REPORT_SCHEDE_PAZIENTE:
                case Report.COD_REPORT_SCHEDE_PAZIENTE_C:
                    dictReturn.Add("StringaConnessione", Database.ConnectionString);
                    dictReturn.Add("IDScheda", CoreStatics.CoreApplication.IDSchedaSelezionata);
                    dictReturn.Add("CodUAAmbulatoriale", CoreStatics.CoreApplication.Paziente.CodUAAmbulatoriale);
                    dictReturn.Add("XmlAmbiente", CoreStatics.CoreApplication.Ambiente.XmlSerializeToString());

                    break;

                case Report.COD_REPORT_PERAMB1:
                    dictReturn.Add("StringaConnessione", Database.ConnectionString);
                    dictReturn.Add("IDScheda", CoreStatics.CoreApplication.IDSchedaSelezionata);
                    dictReturn.Add("XmlAmbiente", CoreStatics.CoreApplication.Ambiente.XmlSerializeToString());

                    break;

                case Report.COD_REPORT_SCHEDA_ETICHETTA:
                    dictReturn.Add("StringaConnessione", Database.ConnectionString);
                    dictReturn.Add("IDScheda", CoreStatics.CoreApplication.IDSchedaSelezionata);
                    if (CoreStatics.CoreApplication.Paziente != null)
                        dictReturn.Add("CodUAAmbulatoriale", CoreStatics.CoreApplication.Paziente.CodUAAmbulatoriale);
                    else
                        dictReturn.Add("CodUAAmbulatoriale", "");
                    dictReturn.Add("XmlAmbiente", CoreStatics.CoreApplication.Ambiente.XmlSerializeToString());

                    break;

                case Report.COD_REPORT_DIARIO_CLINICO_RPT:
                    dictReturn.Add("StringaConnessione", Database.ConnectionString);
                    dictReturn.Add("IDMovDiarioClinico", CoreStatics.CoreApplication.IDDiarioClinicoSelezionato);
                    dictReturn.Add("CodUAAmbulatoriale", CoreStatics.CoreApplication.Paziente.CodUAAmbulatoriale);
                    dictReturn.Add("XmlAmbiente", CoreStatics.CoreApplication.Ambiente.XmlSerializeToString());

                    break;

                case Report.COD_REPORT_ETICHETTA_ALLEGATO:
                    dictReturn.Add("NomePaziente", CoreStatics.CoreApplication.Paziente.Cognome + " " + CoreStatics.CoreApplication.Paziente.Nome);
                    dictReturn.Add("Sesso", CoreStatics.CoreApplication.Paziente.Sesso);
                    dictReturn.Add("NumeroAllegato", CoreStatics.CoreApplication.MovAllegatoSelezionato.IDDocumento);
                    dictReturn.Add("Nosologico", CoreStatics.CoreApplication.Episodio.NumeroEpisodio);
                    dictReturn.Add("DataNascita", CoreStatics.CoreApplication.Paziente.DataNascita);

                    break;

                case Report.COD_REPORT_ETICHETTA_CART_INT_REP:
                case Report.COD_REPORT_ETICHETTA_CART_INT_REP_QR:
                case Report.COD_REPORT_ETICHETTA_CARTELLA:
                case Report.COD_REPORT_ETICHETTA_CARTELLA_QR:
                    dictReturn.Add("NomePaziente", CoreStatics.CoreApplication.Paziente.Cognome + " " + CoreStatics.CoreApplication.Paziente.Nome);
                    dictReturn.Add("NumeroCartella", CoreStatics.CoreApplication.Cartella.NumeroCartella);
                    dictReturn.Add("Sesso", CoreStatics.CoreApplication.Paziente.Sesso);
                    dictReturn.Add("Nosologico", CoreStatics.CoreApplication.Episodio.NumeroEpisodio);
                    dictReturn.Add("DataNascita", CoreStatics.CoreApplication.Paziente.DataNascita);

                    dictReturn.Add("IdCartella", CoreStatics.CoreApplication.Cartella.ID);
                    if (CoreStatics.CoreApplication.Trasferimento != null)
                    {
                        dictReturn.Add("CodUA", CoreStatics.CoreApplication.Trasferimento.CodUA);
                        dictReturn.Add("DescrizioneUA", CoreStatics.CoreApplication.Trasferimento.Descrizione);
                    }
                    dictReturn.Add("XmlAmbiente", CoreStatics.CoreApplication.Ambiente.XmlSerializeToString());

                    break;

                case Report.COD_REPORT_SCHEDA_ANTIBLASTICA:
                    dictReturn.Add("StringaConnessione", Database.ConnectionString);
                    dictReturn.Add("IDTaskInfermieristico", CoreStatics.CoreApplication.IDTaskInfermieristicoSelezionato);
                    dictReturn.Add("XmlAmbiente", CoreStatics.CoreApplication.Ambiente.XmlSerializeToString());

                    break;

                case Report.COD_REPORT_PDF_TUTTI_REFERTI:
                    dictReturn.Add("StringaConnessione", Database.ConnectionString);
                    dictReturn.Add("IDEpisodio", CoreStatics.CoreApplication.Episodio.ID);
                    dictReturn.Add("TempFolderPath", FileStatics.GetSCCITempPath());
                    dictReturn.Add("PaginaBianca", "0"); dictReturn.Add("XmlAmbiente", CoreStatics.CoreApplication.Ambiente.XmlSerializeToString());

                    break;

                case Report.COD_REPORT_GRAFICI1:
                case Report.COD_REPORT_GRAFICI2:
                    dictReturn.Add("StringaConnessione", Database.ConnectionString);
                    dictReturn.Add("CodUAAmbulatoriale", CoreStatics.CoreApplication.Paziente.CodUAAmbulatoriale);
                    dictReturn.Add("XmlAmbiente", CoreStatics.CoreApplication.Ambiente.XmlSerializeToString());
                    dictReturn.Add("Contesto", CoreStatics.CoreApplication.Ambiente.Contesto[EnumEntita.XXX.ToString()]);

                    break;

                case Report.COD_REPORT_FRONTESPIZIO_CARTELLA:
                    dictReturn.Add("StringaConnessione", Database.ConnectionString);
                    dictReturn.Add("Nosologico", CoreStatics.CoreApplication.Episodio.NumeroEpisodio);
                    dictReturn.Add("IDEpisodio", CoreStatics.CoreApplication.Episodio.ID);
                    dictReturn.Add("IDSac", CoreStatics.CoreApplication.Paziente.CodSAC);
                    break;

                case Report.COD_REPORT_FRONTESPIZIO_CARTELLA3:
                    dictReturn.Add("StringaConnessione", Database.ConnectionString);
                    dictReturn.Add("Nosologico", CoreStatics.CoreApplication.Episodio.NumeroEpisodio);
                    dictReturn.Add("IDEpisodio", CoreStatics.CoreApplication.Episodio.ID);
                    dictReturn.Add("IDSac", CoreStatics.CoreApplication.Paziente.CodSAC);
                    dictReturn.Add("IDTrasferimento", CoreStatics.CoreApplication.Trasferimento.ID);
                    break;

                case Report.COD_REPORT_BRACCIALE:
                    dictReturn.Add("StringaConnessione", Database.ConnectionString);
                    if (CoreStatics.CoreApplication.Trasferimento != null)
                        dictReturn.Add("IDTrasferimento", CoreStatics.CoreApplication.Trasferimento.ID);
                    if (CoreStatics.CoreApplication.IDSchedaSelezionata != null)
                        dictReturn.Add("IDScheda", CoreStatics.CoreApplication.IDSchedaSelezionata);
                    dictReturn.Add("XmlAmbiente", CoreStatics.CoreApplication.Ambiente.XmlSerializeToString());
                    break;

                case Report.COD_REPORT_MH_ACCOUNT:
                    dictReturn.Add("StringaConnessione", Database.ConnectionString);
                    dictReturn.Add("NomePaziente", CoreStatics.CoreApplication.Paziente.Cognome + " " + CoreStatics.CoreApplication.Paziente.Nome);
                    dictReturn.Add("DataNascita", CoreStatics.CoreApplication.Paziente.DataNascita);
                    dictReturn.Add("Sesso", CoreStatics.CoreApplication.Paziente.Sesso);
                    dictReturn.Add("Utente", CoreStatics.CoreApplication.MH_LoginSelezionato.Codice);
                    dictReturn.Add("Password", CoreStatics.CoreApplication.MH_LoginSelezionato.PasswordAccesso);
                    dictReturn.Add("DataScadenza", CoreStatics.CoreApplication.MH_LoginSelezionato.DataScadenza);

                    break;

                default:

                    dictReturn.Add("StringaConnessione", Database.ConnectionString);
                    dictReturn.Add("CoreApplication", CoreStatics.CoreApplication);
                    dictReturn.Add("ExportPDF", "0");
                    dictReturn.Add("FirmaDigitale", "0");

                    break;
            }



            string sWebServiceUrl = Database.GetConfigTable(EnumConfigTable.Diagnostics);
            dictReturn.Add("WebServiceUrl", sWebServiceUrl);

            return dictReturn;

        }

        public static bool controlloPreliminareParametriPlugin(Report rep)
        {
            bool pdfCartellaChiusa = false;
            PrintDialog x = null;
            return controlloPreliminareParametriPlugin(rep, false, ref x, out pdfCartellaChiusa);
        }
        public static bool controlloPreliminareParametriPlugin(Report rep, bool stampaDiretta, ref PrintDialog rPrintDialog, out bool pdfCartellaChiusa)
        {
            bool bOK = true;
            pdfCartellaChiusa = false;

            switch (rep.Codice)
            {
                case Report.COD_REPORT_CARTELLA_PAZIENTE:
                    if (CoreStatics.CoreApplication.Cartella == null)
                    {
                        bOK = false;
                        easyStatics.EasyMessageBox("Non è stata selezionata alcuna cartella paziente!", "Cartella Paziente", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    else if (CoreStatics.CoreApplication.Cartella.CodStatoCartella == EnumStatoCartella.CH.ToString())
                    {
                        bOK = false;
                        pdfCartellaChiusa = true;
                        if (stampaDiretta)
                            CoreStatics.apriPDFCartella(CoreStatics.CoreApplication.Cartella, true, ref rPrintDialog);
                        else
                            CoreStatics.apriPDFCartella(CoreStatics.CoreApplication.Cartella);
                    }
                    break;
                case Report.COD_REPORT_ETICHETTA_CARTELLA:
                case Report.COD_REPORT_ETICHETTA_CARTELLA_QR:
                    if (CoreStatics.CoreApplication.Cartella == null)
                    {
                        bOK = false;
                        easyStatics.EasyMessageBox("Non è stata selezionata alcuna cartella paziente!", "Etichetta Paziente", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    break;
                case Report.COD_REPORT_ETICHETTA_CART_INT_REP:
                case Report.COD_REPORT_ETICHETTA_CART_INT_REP_QR:
                    if (CoreStatics.CoreApplication.Cartella == null)
                    {
                        bOK = false;
                        easyStatics.EasyMessageBox("Non è stata selezionata alcuna cartella paziente!", "Etichetta Paziente", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    if (bOK)
                    {
                        if (CoreStatics.CoreApplication.Trasferimento == null)
                        {
                            bOK = false;
                            easyStatics.EasyMessageBox("Non è stato selezionato alcun trasferimento!", "Etichetta Paziente", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                    }
                    break;
                case Report.COD_REPORT_SCHEDE_PAZIENTE:
                case Report.COD_REPORT_SCHEDE_PAZIENTE_C:
                case Report.COD_REPORT_PERAMB1:
                    if (CoreStatics.CoreApplication.IDSchedaSelezionata == null || CoreStatics.CoreApplication.IDSchedaSelezionata == string.Empty || CoreStatics.CoreApplication.IDSchedaSelezionata.Trim() == "")
                    {
                        bOK = false;
                        easyStatics.EasyMessageBox("Non è stata selezionata alcuna scheda paziente!", "Schede Paziente", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    break;
                case Report.COD_REPORT_SCHEDA_ETICHETTA:
                    if (CoreStatics.CoreApplication.IDSchedaSelezionata == null || CoreStatics.CoreApplication.IDSchedaSelezionata == string.Empty || CoreStatics.CoreApplication.IDSchedaSelezionata.Trim() == "")
                    {
                        bOK = false;
                        easyStatics.EasyMessageBox("Non è stata selezionata alcuna scheda!", "Etichetta Scheda", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    break;
                case Report.COD_REPORT_DIARIO_CLINICO_RPT:
                    if (CoreStatics.CoreApplication.IDDiarioClinicoSelezionato == null || CoreStatics.CoreApplication.IDDiarioClinicoSelezionato == string.Empty || CoreStatics.CoreApplication.IDDiarioClinicoSelezionato.Trim() == "")
                    {
                        bOK = false;
                        easyStatics.EasyMessageBox("Non è stato selezionata alcun diario clinico!", "Ddiario Clinico", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    break;
                case Report.COD_REPORT_SCHEDA_ANTIBLASTICA:
                    if (CoreStatics.CoreApplication.IDTaskInfermieristicoSelezionato == null || CoreStatics.CoreApplication.IDTaskInfermieristicoSelezionato == string.Empty || CoreStatics.CoreApplication.IDTaskInfermieristicoSelezionato.Trim() == "")
                    {
                        bOK = false;
                        easyStatics.EasyMessageBox("Non è stato selezionato alcun task infermieristico valido!", "Scheda Antiblastica", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    break;
                case Report.COD_REPORT_PDF_TUTTI_REFERTI:
                    if (CoreStatics.CoreApplication.Episodio == null)
                    {
                        bOK = false;
                        easyStatics.EasyMessageBox("Non è stata selezionato alcun Episodio paziente!", "Stampa Referti", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    break;
                case Report.COD_REPORT_GRAFICI1:
                case Report.COD_REPORT_GRAFICI2:
                    if (CoreStatics.CoreApplication.Episodio == null)
                    {
                        bOK = false;
                        easyStatics.EasyMessageBox("Non è stata selezionato alcun Episodio paziente!", "Stampa Grafici", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    break;
                case Report.COD_REPORT_BRACCIALE:

                    break;
                case Report.COD_REPORT_MH_ACCOUNT:
                    if (CoreStatics.CoreApplication.MH_LoginSelezionato == null)
                    {
                        bOK = false;
                        easyStatics.EasyMessageBox("Non è stato selezionato nessun account!", "Matilde Home - Account", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    break;
                default:
                    break;
            }

            return bOK;
        }

        public static bool stampaDirettaPDF(string vsPDFFileFullPath, ref PrintDialog roPrintDialog)
        {
            bool bReturn = false;
            PDFFile filepdf = null;
            bool bDisposePrintDialog = false;
            try
            {
                filepdf = PDFFile.Open(vsPDFFileFullPath);
                filepdf.SerialNumber = CoreStatics.C_PDF_LICENCE_KEY;

                System.Windows.Forms.DialogResult ret = System.Windows.Forms.DialogResult.OK;
                if (roPrintDialog == null)
                {
                    bDisposePrintDialog = true;
                    roPrintDialog = new PrintDialog();
                    roPrintDialog.ShowHelp = false;
                    roPrintDialog.AllowCurrentPage = false;
                    roPrintDialog.AllowSelection = false;
                    roPrintDialog.AllowSomePages = true;
                    roPrintDialog.PrinterSettings.MaximumPage = filepdf.PageCount;
                    roPrintDialog.PrinterSettings.FromPage = 1;
                    roPrintDialog.PrinterSettings.ToPage = filepdf.PageCount;
                    ret = roPrintDialog.ShowDialog();
                }

                if (ret == System.Windows.Forms.DialogResult.OK)
                {

                    PrinterSettings settings = roPrintDialog.PrinterSettings;

                    PDFPrintSettings pdfPrintSettings = new PDFPrintSettings(settings);
                    pdfPrintSettings.PageScaling = PageScaling.FitToPrinterMargins;


                    filepdf.Print(pdfPrintSettings);
                    bReturn = true;
                }

            }
            catch (Exception ex)
            {
                bReturn = false;
                CoreStatics.ExGest(ref ex, @"stampaDirettaPDF", "CoreStatics");
            }
            finally
            {
                if (filepdf != null) filepdf.Dispose();
                if (bDisposePrintDialog && roPrintDialog != null) roPrintDialog.Dispose();
            }
            return bReturn;
        }

        public static string esportaActiveReportPDF(ref DevExpress.XtraReports.UI.XtraReport rActiveReport)
        {
            string pdffullpath = "";

            if (rActiveReport != null)
            {
                pdffullpath = "CC" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + @".pdf";
                pdffullpath = System.IO.Path.Combine(FileStatics.GetSCCITempPath() + pdffullpath);

                rActiveReport.ExportToPdf(pdffullpath);

            }

            return pdffullpath;
        }

        #endregion

        #region Colori

        public static Color GetColorFromString(string Stringa)
        {

            Color Colore = default(Color);
            string sColore = Stringa;
            char[] splitchar1 = ",".ToCharArray();
            char[] splitchar2 = "=".ToCharArray();

            if (sColore.LastIndexOf("[") > 0)
            {
                sColore = sColore.Substring(sColore.LastIndexOf("[") + 1, sColore.LastIndexOf("]") - sColore.LastIndexOf("[") - 1);

                if (!Information.IsDBNull(sColore) & sColore != "Empty")
                {
                    Colore = Color.FromName(sColore);
                    if (Colore.ToArgb() == 0 && Information.IsArray(sColore.Split(splitchar1)))
                    {
                        int A = 0;
                        int R = 0;
                        int G = 0;
                        int B = 0;
                        if (Information.IsNumeric(sColore.Split(splitchar1)[0].Split(splitchar2)[1])) A = Convert.ToInt32(sColore.Split(splitchar1)[0].Split(splitchar2)[1]);
                        if (Information.IsNumeric(sColore.Split(splitchar1)[1].Split(splitchar2)[1])) R = Convert.ToInt32(sColore.Split(splitchar1)[1].Split(splitchar2)[1]);
                        if (Information.IsNumeric(sColore.Split(splitchar1)[2].Split(splitchar2)[1])) G = Convert.ToInt32(sColore.Split(splitchar1)[2].Split(splitchar2)[1]);
                        if (Information.IsNumeric(sColore.Split(splitchar1)[3].Split(splitchar2)[1])) B = Convert.ToInt32(sColore.Split(splitchar1)[3].Split(splitchar2)[1]);

                        Colore = Color.FromArgb(A, R, G, B);
                    }
                }
                else
                {
                    Colore = Color.FromArgb(0, 0, 0, 0);
                }

            }

            if (Colore.ToArgb() == 0 & Information.IsNumeric(sColore))
            {
                Colore = Color.FromArgb(Convert.ToInt32(sColore));
            }

            return Colore;

        }

        public static Bitmap CreateSolidBitmap(Color Colore, int x = 16, int y = 16)
        {

            Bitmap oRet = new Bitmap(x, y);

            for (int nx = 0; nx <= x - 1; nx++)
            {
                for (int ny = 0; ny <= y - 1; ny++)
                {
                    if (nx == 0 || nx == x - 1 || ny == 0 || ny == y - 1)
                    {
                        oRet.SetPixel(nx, ny, Color.Black);
                    }
                    else
                    {
                        oRet.SetPixel(nx, ny, Colore);
                    }
                }
            }

            return oRet;

        }

        #endregion

        #region Agende

        public static string GetTimeSlotIntervalIta(TimeSlotInterval TimeSlot)
        {
            switch (TimeSlot)
            {
                case TimeSlotInterval.OneMinute: return "1 Minuto";
                case TimeSlotInterval.TwoMinutes: return "2 Minuti";
                case TimeSlotInterval.ThreeMinutes: return "3 Minuti";
                case TimeSlotInterval.FourMinutes: return "4 Minuti";
                case TimeSlotInterval.FiveMinutes: return "5 Minuti";
                case TimeSlotInterval.SixMinutes: return "6 Minuti";
                case TimeSlotInterval.TenMinutes: return "10 Minuti";
                case TimeSlotInterval.TwelveMinutes: return "12 Minuti";
                case TimeSlotInterval.FifteenMinutes: return "15 Minuti";
                case TimeSlotInterval.TwentyMinutes: return "20 Minuti";
                case TimeSlotInterval.ThirtyMinutes: return "30 Minuti";
                case TimeSlotInterval.SixtyMinutes: return "60 Minuti";
                default: return "";

            }

        }

        public static SortedList GetTimeSlotInterval()
        {
            SortedList oSL = new SortedList();

            foreach (int i in Enum.GetValues(typeof(TimeSlotInterval)))
            {
                string sDes = "";
                sDes = GetTimeSlotIntervalIta((TimeSlotInterval)i);
                oSL.Add(i, sDes);
            }

            return oSL;
        }

        public static DataSet GetTimeSlotIntervalDs()
        {
            try
            {
                DataSet oDs = new DataSet();
                DataTable oDt = new DataTable();
                DataColumn oDc = null;
                oDc = new DataColumn("Codice", typeof(string));
                oDt.Columns.Add(oDc);
                oDc = new DataColumn("Descrizione", typeof(string));
                oDt.Columns.Add(oDc);
                oDs.Tables.Add(oDt);

                SortedList slDati = GetTimeSlotInterval();
                for (int i = 0; i < slDati.Count; i++)
                {
                    DataRow oDr = oDs.Tables[0].NewRow();
                    oDr["Codice"] = slDati.GetKey(i);
                    oDr["Descrizione"] = slDati.GetByIndex(i);
                    oDs.Tables[0].Rows.Add(oDr);
                }

                return oDs;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public static ValueList GetTimeSlotIntervalVl()
        {

            ValueList oVl = new ValueList();

            try
            {

                SortedList slDati = GetTimeSlotInterval();

                for (int i = 0; i < slDati.Count; i++)
                {
                    oVl.ValueListItems.Add(slDati.GetKey(i), slDati.GetByIndex(i).ToString());
                }


            }
            catch (Exception)
            {

            }

            return oVl;

        }

        public static SortedList GetTipoRaggruppamentoAgenda()
        {
            SortedList oSL = new SortedList();

            foreach (int i in Enum.GetValues(typeof(EnumTipoRaggruppamentoAgenda)))
            {
                oSL.Add(i.ToString(), GetEnumDescription((EnumTipoRaggruppamentoAgenda)i));
            }

            return oSL;
        }

        public static DataSet GetTipoRaggruppamentoAgendaDs()
        {
            try
            {
                DataSet oDs = new DataSet();
                DataTable oDt = new DataTable();
                DataColumn oDc = null;
                oDc = new DataColumn("Codice", typeof(string));
                oDt.Columns.Add(oDc);
                oDc = new DataColumn("Descrizione", typeof(string));
                oDt.Columns.Add(oDc);
                oDs.Tables.Add(oDt);

                SortedList slDati = GetTipoRaggruppamentoAgenda();
                for (int i = 0; i < slDati.Count; i++)
                {
                    DataRow oDr = oDs.Tables[0].NewRow();
                    oDr["Codice"] = slDati.GetKey(i);
                    oDr["Descrizione"] = slDati.GetByIndex(i);
                    oDs.Tables[0].Rows.Add(oDr);
                }

                return oDs;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public static List<KeyValuePair<string, string>> GetCodPeriodoDisponibilita()
        {
            List<KeyValuePair<string, string>> oSL = new List<KeyValuePair<string, string>>();

            oSL.Add(new KeyValuePair<string, string>(ucEasyDateRange.C_RNG_N1M, ucEasyDateRange.getRangeDescription(ucEasyDateRange.C_RNG_N1M)));
            oSL.Add(new KeyValuePair<string, string>(ucEasyDateRange.C_RNG_N6M, ucEasyDateRange.getRangeDescription(ucEasyDateRange.C_RNG_N6M)));
            oSL.Add(new KeyValuePair<string, string>(ucEasyDateRange.C_RNG_N1Y, ucEasyDateRange.getRangeDescription(ucEasyDateRange.C_RNG_N1Y)));
            oSL.Add(new KeyValuePair<string, string>(ucEasyDateRange.C_RNG_N5Y, ucEasyDateRange.getRangeDescription(ucEasyDateRange.C_RNG_N5Y)));
            oSL.Add(new KeyValuePair<string, string>(ucEasyDateRange.C_RNG_OGGI, ucEasyDateRange.getRangeDescription(ucEasyDateRange.C_RNG_OGGI)));

            return oSL;
        }

        public static DataSet GetCodPeriodoDisponibilitaDs()
        {
            try
            {
                DataSet oDs = new DataSet();
                DataTable oDt = new DataTable();
                DataColumn oDc = null;
                oDc = new DataColumn("Codice", typeof(string));
                oDt.Columns.Add(oDc);
                oDc = new DataColumn("Descrizione", typeof(string));
                oDt.Columns.Add(oDc);
                oDs.Tables.Add(oDt);

                List<KeyValuePair<string, string>> kvDati = GetCodPeriodoDisponibilita();
                foreach (KeyValuePair<string, string> kv in kvDati)
                {
                    DataRow oDr = oDs.Tables[0].NewRow();
                    oDr["Codice"] = kv.Key;
                    oDr["Descrizione"] = kv.Value;
                    oDs.Tables[0].Rows.Add(oDr);
                }

                return oDs;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public static SortedList GetTipoNews()
        {
            SortedList oSL = new SortedList();

            foreach (int i in Enum.GetValues(typeof(EnumTipoNews)))
            {
                oSL.Add(((EnumTipoNews)i).ToString(), GetEnumDescription((EnumTipoNews)i));
            }

            return oSL;
        }

        public static ValueList GetTipoNewsVl()
        {

            ValueList oVl = new ValueList();

            try
            {

                SortedList slDati = GetTipoNews();

                for (int i = 0; i < slDati.Count; i++)
                {
                    oVl.ValueListItems.Add(slDati.GetKey(i), slDati.GetByIndex(i).ToString());
                }


            }
            catch (Exception)
            {

            }

            return oVl;

        }

        public static DataSet GetTipoNewsDs()
        {
            try
            {
                DataSet oDs = new DataSet();
                DataTable oDt = new DataTable();
                DataColumn oDc = null;
                oDc = new DataColumn("Codice", typeof(string));
                oDt.Columns.Add(oDc);
                oDc = new DataColumn("Descrizione", typeof(string));
                oDt.Columns.Add(oDc);
                oDs.Tables.Add(oDt);

                SortedList slDati = GetTipoNews();
                for (int i = 0; i < slDati.Count; i++)
                {
                    DataRow oDr = oDs.Tables[0].NewRow();
                    oDr["Codice"] = slDati.GetKey(i);
                    oDr["Descrizione"] = slDati.GetByIndex(i);
                    oDs.Tables[0].Rows.Add(oDr);
                }

                return oDs;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public static DataSet GetUnitaScadenzaDs()
        {
            try
            {
                DataSet oDs = new DataSet();
                DataTable oDt = new DataTable();
                DataColumn oDc = null;
                oDc = new DataColumn("Codice", typeof(string));
                oDt.Columns.Add(oDc);
                oDc = new DataColumn("Descrizione", typeof(string));
                oDt.Columns.Add(oDc);
                oDs.Tables.Add(oDt);

                SortedList slDati = GetUnitaScadenza();
                for (int i = 0; i < slDati.Count; i++)
                {
                    DataRow oDr = oDs.Tables[0].NewRow();
                    oDr["Codice"] = slDati.GetKey(i);
                    oDr["Descrizione"] = slDati.GetByIndex(i);
                    oDs.Tables[0].Rows.Add(oDr);
                }

                return oDs;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public static SortedList GetUnitaScadenza()
        {
            SortedList oSL = new SortedList();

            foreach (int i in Enum.GetValues(typeof(EnumUnitaScadenza)))
            {
                oSL.Add(((EnumUnitaScadenza)i).ToString(), GetEnumDescription((EnumUnitaScadenza)i));
            }

            return oSL;
        }

        public static SortedList GetTipoProtocolli()
        {
            SortedList oSL = new SortedList();

            foreach (int i in Enum.GetValues(typeof(EnumTipoProtocollo)))
            {
                oSL.Add(((EnumTipoProtocollo)i).ToString(), ((EnumTipoProtocollo)i).ToString());
            }

            return oSL;
        }

        public static DataSet GetTipoProtocolliDs()
        {
            try
            {
                DataSet oDs = new DataSet();
                DataTable oDt = new DataTable();
                DataColumn oDc = null;
                oDc = new DataColumn("Codice", typeof(string));
                oDt.Columns.Add(oDc);
                oDc = new DataColumn("Descrizione", typeof(string));
                oDt.Columns.Add(oDc);
                oDs.Tables.Add(oDt);

                SortedList slDati = GetTipoProtocolli();
                for (int i = 0; i < slDati.Count; i++)
                {
                    DataRow oDr = oDs.Tables[0].NewRow();
                    oDr["Codice"] = slDati.GetKey(i);
                    oDr["Descrizione"] = slDati.GetByIndex(i);
                    oDs.Tables[0].Rows.Add(oDr);
                }

                return oDs;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }



        public static List<string> DeserializeXmlToElencoCampi(string xml)
        {
            List<string> ooo = new List<string>();
            try
            {
                System.Xml.Serialization.XmlSerializer mySerializer = new System.Xml.Serialization.XmlSerializer(ooo.GetType());
                System.IO.MemoryStream ms = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(xml));
                ooo = (List<string>)mySerializer.Deserialize(ms);
            }
            catch (Exception)
            {
            }
            return ooo;
        }
        public static string SerializeElencoCampi(List<string> MyObj)
        {
            try
            {
                System.Xml.Serialization.XmlSerializer mySerializer = new System.Xml.Serialization.XmlSerializer(MyObj.GetType());
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                mySerializer.Serialize(ms, MyObj);
                return System.Text.ASCIIEncoding.UTF8.GetString(ms.ToArray());

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public static DataTable getDataTableFestivita(List<string> codiciAgende)
        {
            Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);

            if (codiciAgende != null && codiciAgende.Count > 0)
                op.ParametroRipetibile.Add("CodAgenda", codiciAgende.ToArray());

            SqlParameterExt[] spcoll = new SqlParameterExt[1];
            string xmlParam = XmlProcs.XmlSerializeToString(op);
            spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
            return Database.GetDataTableStoredProc("MSP_SelFestivita", spcoll);
        }

        public static void CompletaDatiAppuntamento(MovAppuntamento movapp, Scci.DataContracts.ScciAmbiente ambiente)
        {
            if (movapp != null && ambiente != null)
            {
                if (string.IsNullOrEmpty(movapp.IDEpisodio) == false &&
    string.IsNullOrEmpty(movapp.IDTrasferimento) == true)
                {
                    if ((ambiente != null) && (string.IsNullOrEmpty(ambiente.IdTrasferimento) == false)
    && string.IsNullOrEmpty(ambiente.Idepisodio) == false &&
    movapp.IDEpisodio == ambiente.Idepisodio
    )
                        movapp.IDTrasferimento = ambiente.IdTrasferimento;

                    if (movapp.MovScheda != null)
                    {
                        movapp.MovScheda.IDTrasferimento = ambiente.IdTrasferimento;
                    }
                }
            }
        }
        public static void CompletaDatiAppuntamento(MovAppuntamento movapp)
        {

            try
            {

                if (string.IsNullOrEmpty(movapp.IDEpisodio) == false &&
                    string.IsNullOrEmpty(movapp.IDTrasferimento) == true)
                {

                    Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("IDEpisodio", movapp.IDEpisodio);
                    op.ParametroRipetibile.Add("CodStatoCartella", new List<string>() { EnumStatoCartella.AP.ToString() }.ToArray());
                    SqlParameterExt[] spcoll = new SqlParameterExt[1];
                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                    DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovCartelleDaEpisodio", spcoll);

                    switch (dt.Rows.Count)
                    {

                        case 0:
                            break;

                        case 1:
                            if (!dt.Rows[0].IsNull("IDTrasferimento"))
                            {
                                movapp.IDTrasferimento = dt.Rows[0]["IDTrasferimento"].ToString();
                                if (movapp.MovScheda != null)
                                {
                                    movapp.MovScheda.IDEpisodio = movapp.IDEpisodio;
                                    movapp.MovScheda.IDTrasferimento = dt.Rows[0]["IDTrasferimento"].ToString();
                                }
                            }
                            break;

                        default:
                            string sNomeMaschera = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                            try
                            {

                                Form frm = new frmSelezionaCartella();
                                easyStatics.maximizeForm(ref frm);
                                ((frmSelezionaCartella)frm).Carica(dt);
                                if (frm.DialogResult == System.Windows.Forms.DialogResult.OK)
                                {

                                    movapp.IDTrasferimento = ((frmSelezionaCartella)frm).UltraGrid.ActiveRow.Cells["IDTrasferimento"].Text;
                                    if (movapp.MovScheda != null)
                                    {
                                        movapp.MovScheda.IDEpisodio = movapp.IDEpisodio;
                                        movapp.MovScheda.IDTrasferimento = ((frmSelezionaCartella)frm).UltraGrid.ActiveRow.Cells["IDTrasferimento"].Text;
                                    }

                                }

                            }
                            catch (Exception)
                            {

                            }
                            finally
                            {
                                CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione = sNomeMaschera;
                            }
                            break;

                    }

                }

            }
            catch (Exception)
            {

            }

        }

        #endregion

        #region Allegati

        internal static void ApriAllegato(byte[] documento, string nomefile)
        {

            try
            {

                if (documento != null && nomefile.Trim() != "")
                {

                    string snewfilename = System.IO.Path.Combine(FileStatics.GetSCCITempPath() + nomefile);
                    if (UnicodeSrl.Framework.Utility.FileSystem.ByteArrayToFile(snewfilename, ref documento))
                    {
                        if (System.IO.File.Exists(snewfilename))
                        {
                            easyStatics.ShellExecute(snewfilename, "");
                        }
                    }





                }

            }
            catch (Exception ex)
            {
                throw new Exception("<ApriAllegato> " + ex.Message, ex);
            }

        }

        internal static void SalvaAllegato(byte[] documento, string nomefile)
        {

            try
            {

                if (documento != null && nomefile.Trim() != "")
                {

                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.AddExtension = false;
                    sfd.CheckFileExists = false;
                    sfd.CheckPathExists = true;
                    sfd.FileName = nomefile;
                    sfd.Filter = "Tutti i documenti (*.*)|*.*";
                    sfd.FilterIndex = 0;
                    sfd.OverwritePrompt = true;
                    sfd.ShowHelp = false;
                    sfd.Title = "Salva l'Allegato";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        if (System.IO.File.Exists(sfd.FileName)) System.IO.File.Delete(sfd.FileName);
                        UnicodeSrl.Framework.Utility.FileSystem.ByteArrayToFile(sfd.FileName, ref documento);
                    }
                    sfd.Dispose();

                }

            }
            catch (Exception ex)
            {
                throw new Exception("<SalvaAllegato> " + ex.Message, ex);
            }

        }

        internal static int GetNuovoIDDocumento()
        {
            try
            {
                int iRet = 1;

                Parametri op = null;
                SqlParameterExt[] spcoll;
                string xmlParam = "";

                op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.TimeStamp.CodEntita = EnumEntita.ALL.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.MOD.ToString();

                spcoll = new SqlParameterExt[1];

                xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelContatoreEtichettaAllegati", spcoll);
                if (dt != null && dt.Rows.Count > 0 && !dt.Rows[0].IsNull(0))
                {
                    if (!int.TryParse(dt.Rows[0][0].ToString(), out iRet)) iRet = 1;

                }


                return iRet;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region CARTELLA

        public static bool RicercaCartella()
        {
            try
            {
                return RicercaCartella(CoreStatics.CoreApplication.Sessione.Nosologico);
            }
            catch (Exception)
            {
                throw;
            }

        }
        public static bool RicercaCartella(string filtroGenerico)
        {

            bool bret = false;

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                op.Parametro.Add("FiltroGenerico", filtroGenerico);

                op.Parametro.Add("Ordinamento", "P.Cognome, P.Nome");


                op.Parametro.Add("CodStatoCartella", "AP");
                op.Parametro.Add("SoloUltimoTrasferimentoCartella", "1");

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_CercaEpisodio", spcoll);

                if (dt != null)
                {

                    switch (dt.Rows.Count)
                    {

                        case 0:
                            easyStatics.EasyMessageBox("La ricerca non ha restituito alcun risultato.", "Ricerca Paziente", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            break;

                        case 1:

                            bool clearSelection = true;
                            DataRow oUgr = dt.Rows[0];

                            CoreStatics.CoreApplication.Paziente = new Paziente("", oUgr["IDEpisodio"].ToString());
                            CoreStatics.CoreApplication.Episodio = new Episodio(oUgr["IDEpisodio"].ToString());
                            CoreStatics.CoreApplication.Trasferimento = new Trasferimento(oUgr["IDTrasferimento"].ToString(), CoreStatics.CoreApplication.Ambiente);
                            if (!oUgr.IsNull("IDCartella") && oUgr["IDCartella"].ToString() != "")
                            {
                                CoreStatics.CoreApplication.Cartella = new Cartella(oUgr["IDCartella"].ToString(), oUgr["NumeroCartella"].ToString(), CoreStatics.CoreApplication.Ambiente);
                            }

                            clearSelection = !CoreStatics.CaricaCartellaPaziente(oUgr, EnumMaschere.MenuPrincipale);

                            if (clearSelection)
                            {
                                CoreStatics.CoreApplication.Paziente = null;
                                CoreStatics.CoreApplication.Episodio = null;
                                CoreStatics.CoreApplication.Trasferimento = null;
                                CoreStatics.CoreApplication.Cartella = null;
                            }
                            else
                            {
                                bret = true;
                            }
                            break;

                        default:
                            if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezionaCartella) == DialogResult.OK)
                            {

                                bool clearSelectionmore = true;
                                DataRow oUgrmore = dt.Rows[CoreStatics.CoreApplication.Sessione.NosologicoIndex];

                                CoreStatics.CoreApplication.Paziente = new Paziente("", oUgrmore["IDEpisodio"].ToString());
                                CoreStatics.CoreApplication.Episodio = new Episodio(oUgrmore["IDEpisodio"].ToString());
                                CoreStatics.CoreApplication.Trasferimento = new Trasferimento(oUgrmore["IDTrasferimento"].ToString(), CoreStatics.CoreApplication.Ambiente);
                                if (!oUgrmore.IsNull("IDCartella") && oUgrmore["IDCartella"].ToString() != "")
                                {
                                    CoreStatics.CoreApplication.Cartella = new Cartella(oUgrmore["IDCartella"].ToString(), oUgrmore["NumeroCartella"].ToString(), CoreStatics.CoreApplication.Ambiente);
                                }

                                clearSelectionmore = !CoreStatics.CaricaCartellaPaziente(oUgrmore, EnumMaschere.MenuPrincipale);

                                if (clearSelectionmore)
                                {
                                    CoreStatics.CoreApplication.Paziente = null;
                                    CoreStatics.CoreApplication.Episodio = null;
                                    CoreStatics.CoreApplication.Trasferimento = null;
                                    CoreStatics.CoreApplication.Cartella = null;
                                }
                                else
                                {
                                    bret = true;
                                }

                            }
                            break;

                    }

                    dt.Dispose();

                }

            }
            catch (Exception)
            {
                throw;
            }

            return bret;

        }

        public static bool CaricaCartellaPaziente(DataRow rowPaziente, EnumMaschere mascheracorrente)
        {
            return CaricaCartellaPaziente(rowPaziente["CodStatoTrasferimento"].ToString(),
                                            rowPaziente["CodStatoCartella"].ToString(),
                                            rowPaziente["IDTrasferimento"].ToString(),
                                            rowPaziente["DecrStato"].ToString(),
                                            mascheracorrente);
        }
        public static bool CaricaCartellaPaziente(string sCodStatoTrasferimento, string sCodStatoCartella, string sIDTrasferimento, string sDecrStato, EnumMaschere mascheracorrente)
        {
            try
            {

                bool bRet = false;
                bool bContinue = false;

                if (CoreStatics.CoreApplication.Cartella != null && CoreStatics.CoreApplication.Cartella.CodStatoCartella == EnumStatoCartella.CH.ToString())
                {
                    CoreStatics.CoreApplication.Navigazione.Maschere.TornaARicercaAbilitato = true;
                    CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.CartellaPaziente);
                    CoreStatics.CoreApplication.Navigazione.Maschere.RimuoviMaschera(mascheracorrente);
                    bRet = true;
                }
                else
                {

                    if ((sCodStatoTrasferimento == Scci.Enums.EnumStatoTrasferimento.AT.ToString() ||
                          sCodStatoTrasferimento == Scci.Enums.EnumStatoTrasferimento.PR.ToString() ||
                          sCodStatoTrasferimento == Scci.Enums.EnumStatoTrasferimento.PT.ToString()
    )
    &&
    (sCodStatoCartella == Scci.Enums.EnumStatoCartella.DA.ToString()))
                    {
                        if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Cartella_Apri))
                        {
                            if (Database.GetConfigTable(EnumConfigTable.AggiornaSACAperturaCartella) == "1")
                                try
                                {
                                    Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);

                                    op.Parametro.Add("CodUA", CoreStatics.CoreApplication.Trasferimento.CodUA);
                                    op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Paziente.ID);
                                    op.Parametro.Add("CodSAC", CoreStatics.CoreApplication.Paziente.CodSAC);
                                    op.Parametro.Add("IDEpisodio", CoreStatics.CoreApplication.Episodio.ID);
                                    op.Parametro.Add("IDTrasferimento", CoreStatics.CoreApplication.Trasferimento.ID);

                                    SqlParameterExt[] spcoll = new SqlParameterExt[1];
                                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                                    DataSet ds = Database.GetDatasetStoredProc("MSP_ControlloPreAperturaCartella", spcoll);

                                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                                    {
                                        if ((bool)ds.Tables[0].Rows[0]["Esito"])
                                            if (easyStatics.EasyMessageBox(ds.Tables[0].Rows[0]["Messaggio"].ToString(), "Apertura Cartella", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                                                bContinue = true;
                                            else
                                                bContinue = false;
                                        else
                                            bContinue = true;
                                    }
                                    else
                                    {
                                        bContinue = false;
                                    }

                                }
                                catch
                                {
                                    bContinue = false;
                                }
                            else
                                bContinue = true;

                            if (bContinue)
                            {
                                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.ConfermaPresainCarico) == DialogResult.OK)
                                {
                                    CoreStatics.CoreApplication.PreTrasferimentoUACodiceSelezionata = string.Empty;
                                    CoreStatics.CoreApplication.PreTrasferimentoUADescrizioneSelezionata = string.Empty;
                                    CoreStatics.CoreApplication.PreTrasferimentoUOCodiceSelezionata = string.Empty;
                                    CoreStatics.CoreApplication.PreTrasferimentoUODescrizioneSelezionata = string.Empty;
                                    CoreStatics.CoreApplication.Trasferimento = new Trasferimento(sIDTrasferimento, CoreStatics.CoreApplication.Ambiente);
                                    if (CoreStatics.CoreApplication.Trasferimento.IDCartella != null && CoreStatics.CoreApplication.Trasferimento.IDCartella.Trim() != "")
                                        CoreStatics.CoreApplication.Cartella = new Cartella(CoreStatics.CoreApplication.Trasferimento.IDCartella, CoreStatics.CoreApplication.Trasferimento.NumeroCartella, CoreStatics.CoreApplication.Ambiente);

                                    CoreStatics.CoreApplication.Navigazione.Maschere.TornaARicercaAbilitato = true;

                                    CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.CartellaPaziente);
                                    CoreStatics.CoreApplication.Navigazione.Maschere.RimuoviMaschera(mascheracorrente);

                                    bRet = true;
                                }
                                else
                                {
                                    if (mascheracorrente == EnumMaschere.PreTrasferimento_RicercaPaziente)
                                    {
                                        CoreStatics.CancellaTrasferimento(sIDTrasferimento);

                                    }
                                }
                            }
                            else
                            {
                                if (mascheracorrente == EnumMaschere.PreTrasferimento_RicercaPaziente)
                                {
                                    CoreStatics.CancellaTrasferimento(sIDTrasferimento);

                                }
                            }

                        }
                        else
                        {
                            easyStatics.EasyMessageBox("Permessi mancanti per l'apertura cartella !", "Apertura Cartella", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                    }
                    else
                    {
                        CoreStatics.CoreApplication.Navigazione.Maschere.TornaARicercaAbilitato = true;

                        if ((sCodStatoTrasferimento == Scci.Enums.EnumStatoTrasferimento.DM.ToString() ||
    sCodStatoTrasferimento == Scci.Enums.EnumStatoTrasferimento.PC.ToString() ||
    sCodStatoTrasferimento == Scci.Enums.EnumStatoTrasferimento.SS.ToString() ||
    sCodStatoTrasferimento == Scci.Enums.EnumStatoTrasferimento.TR.ToString() ||
    sCodStatoTrasferimento == Scci.Enums.EnumStatoTrasferimento.PA.ToString())
    &&
    (sCodStatoCartella == Scci.Enums.EnumStatoCartella.DA.ToString()))
                        {
                            easyStatics.EasyMessageBox("Impossibile aprire la cartella per il trasferimento in stato '" +
                                                        sDecrStato + @"'", "Apertura Cartella", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            bRet = false;

                        }
                        else
                        {
                            CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.CartellaPaziente);
                            CoreStatics.CoreApplication.Navigazione.Maschere.RimuoviMaschera(mascheracorrente);
                            bRet = true;
                        }


                    }
                }
                return bRet;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool CaricaCartellaAmbulatoriale()
        {

            bool bRet = false;

            try
            {

                CoreStatics.CoreApplication.CartellaAmbulatoriale = new CartellaAmbulatoriale(CoreStatics.CoreApplication.Ambiente);
                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.ConfermaPresainCaricoAmbulatoriale) == DialogResult.OK)
                {
                    bRet = CoreStatics.CoreApplication.CartellaAmbulatoriale.Salva();
                }
                else
                {
                    CoreStatics.CoreApplication.CartellaAmbulatoriale = null;
                }

            }
            catch (Exception)
            {
                throw;
            }

            return bRet;

        }

        internal static string CreaPreTraferimento(DataRow rowPaziente)
        {
            try
            {

                string sID = string.Empty;

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDEpisodio", rowPaziente["IDEpisodio"].ToString());
                op.Parametro.Add("CodUA", CoreStatics.CoreApplication.PreTrasferimentoUACodiceSelezionata);
                op.Parametro.Add("CodUO", CoreStatics.CoreApplication.PreTrasferimentoUOCodiceSelezionata);
                op.Parametro.Add("DescrUO", CoreStatics.CoreApplication.PreTrasferimentoUODescrizioneSelezionata);
                op.Parametro.Add("CodStatoTrasferimento", EnumStatoTrasferimento.PT.ToString());
                op.Parametro.Add("DataIngresso", Database.dataOraMinutiSecondi105PerParametri(DateTime.Now));
                op.Parametro.Add("DataIngressoUTC", Database.dataOraMinutiSecondi105PerParametri(DateTime.Now.ToUniversalTime()));

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_InsMovTrasferimenti", spcoll);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && !ds.Tables[0].Rows[0].IsNull(0))
                {
                    {
                        sID = ds.Tables[0].Rows[0][0].ToString();
                    }
                }

                return sID;

            }
            catch (Exception)
            {
                throw;
            }

        }

        internal static bool CancellaTrasferimento(string sIDTrasferimento)
        {
            bool bRet = false;
            try
            {


                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);

                op.Parametro.Add("IDTrasferimento", sIDTrasferimento);
                op.Parametro.Add("CodStatoTrasferimento", EnumStatoTrasferimento.CA.ToString());

                op.TimeStamp.CodEntita = EnumEntita.TRA.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.CAN.ToString();
                op.TimeStamp.IDEntita = sIDTrasferimento;


                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_AggMovTrasferimenti", spcoll);

                bRet = true;
            }
            catch (Exception)
            {
                throw;
            }

            return bRet;
        }

        internal static void apriPDFCartella(Cartella cartella)
        {
            try
            {
                PrintDialog dlg = null;
                apriPDFCartella(cartella, false, ref dlg);
            }
            catch (Exception)
            {
                throw;
            }
        }
        internal static void apriPDFCartella(Cartella cartella, bool stampaDiretta, ref PrintDialog roPrintDialog)
        {
            try
            {

                if (cartella != null)
                {
                    if (cartella.PDFCartella != null && cartella.PDFCartella.Length > 0)
                    {
                        byte[] documento = (byte[])cartella.PDFCartella;
                        string snewfilename = System.IO.Path.Combine(FileStatics.GetSCCITempPath() + DateTime.Now.ToString("yyyyMMddHHmmss") + @".pdf");

                        if (UnicodeSrl.Framework.Utility.FileSystem.ByteArrayToFile(snewfilename, ref documento))
                        {
                            if (System.IO.File.Exists(snewfilename))
                            {
                                if (stampaDiretta)
                                    stampaDirettaPDF(snewfilename, ref roPrintDialog);
                                else
                                    easyStatics.ShellExecute(snewfilename, "", false);
                            }
                        }
                    }
                    else
                    {
                        if (cartella.CodStatoCartella == EnumStatoCartella.CH.ToString()) easyStatics.EasyMessageBox(@"Cartella Chiusa e PDF mancante." + Environment.NewLine + @"Impossibile consultare la cartella.", "Cartella Paziente", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        internal static void apriPDFRefertiCartella(Episodio episodio, Cartella cartella, byte[] pdffirmato = null)
        {
            try
            {
                bool bOK = false;
                if (episodio != null)
                {
                    string snewfilename = generaPDFReferti(episodio, cartella, pdffirmato);

                    if (System.IO.File.Exists(snewfilename))
                    {
                        easyStatics.ShellExecute(snewfilename, "", false);
                        bOK = true;
                    }
                }

                if (!bOK) easyStatics.EasyMessageBox(@"Impossibile consultare i referti.", "Cartella Paziente", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);

            }
            catch (Exception)
            {
                throw;
            }
        }

        private static string generaPDFReferti(Episodio episodio, Cartella cartella, byte[] pdffirmato = null)
        {
            try
            {
                string sReturn = "";
                if (episodio != null && episodio.ID != null && episodio.ID != string.Empty && episodio.ID.Trim() != "")
                {
                    List<string> filesreferti = new List<string>();

                    if (cartella != null)
                    {
                        try
                        {
                            byte[] documento = (pdffirmato == null ? (byte[])cartella.PDFCartella : pdffirmato);
                            if (documento != null && documento.Length > 0)
                            {

                                string snewfilename = System.IO.Path.Combine(FileStatics.GetSCCITempPath() + @"CC" + DateTime.Now.ToString("yyyyMMddHHmmss") + @".pdf");

                                if (UnicodeSrl.Framework.Utility.FileSystem.ByteArrayToFile(snewfilename, ref documento))
                                {
                                    if (System.IO.File.Exists(snewfilename))
                                    {
                                        filesreferti.Add(snewfilename);
                                    }
                                }

                            }
                            else
                            {
                                easyStatics.EasyMessageBox(@"PDF della Cartella mancante." + Environment.NewLine + @"Impossibile consultare la cartella.", "Cartella Paziente", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                            }
                        }
                        catch (Exception ex)
                        {
                            UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                        }
                    }

                    try
                    {

                        Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                        op.Parametro.Add("Codice", Report.COD_REPORT_PDF_TUTTI_REFERTI);
                        op.TimeStamp.CodEntita = EnumEntita.RPT.ToString(); op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                        SqlParameterExt[] spcoll = new SqlParameterExt[1];

                        string xmlParam = XmlProcs.XmlSerializeToString(op);
                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                        Report rep = null;
                        DataTable dt = Database.GetDataTableStoredProc("MSP_SelReport", spcoll);
                        if (dt != null)
                        {
                            if (dt.Rows.Count > 0)
                            {
                                DataRow oDr = dt.Rows[0];
                                string nomeplugin = "";
                                bool dastoricizzare = false;
                                bool apriBrowser = false;
                                bool apriIE = false;
                                bool richiediStampante = false;
                                byte[] modello = null;
                                if (!oDr.IsNull("NomePlugin")) nomeplugin = oDr["NomePlugin"].ToString();
                                if (!oDr.IsNull("DaStoricizzare")) dastoricizzare = (bool)oDr["DaStoricizzare"];
                                if (!oDr.IsNull("Modello")) modello = (byte[])oDr["Modello"];
                                if (!oDr.IsNull("ApriBrowser")) apriBrowser = (bool)oDr["ApriBrowser"];
                                if (!oDr.IsNull("ApriIE")) apriIE = (bool)oDr["ApriIE"];
                                if (dt.Columns.Contains("FlagRichiediStampante") && !oDr.IsNull("FlagRichiediStampante")) richiediStampante = (bool)oDr["FlagRichiediStampante"];

                                rep = new Report(oDr["Codice"].ToString(), oDr["Descrizione"].ToString(), oDr["Path"].ToString(), oDr["CodFormatoReport"].ToString(), oDr["Parametri"].ToString(), nomeplugin, dastoricizzare, modello, apriBrowser, apriIE, richiediStampante);

                            }
                            else
                                easyStatics.EasyMessageBox(@"Impossibile recuperare i referti!", @"Stampa Referti", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            dt.Dispose();
                        }
                        else
                            easyStatics.EasyMessageBox(@"Impossibile recuperare i referti!", @"Stampa Referti", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        if (rep != null)
                        {
                            if (rep.NomePlugIn == null || rep.NomePlugIn.Trim() == "")
                                easyStatics.EasyMessageBox(@"Impossibile recuperare il nome del plugin!", "Stampa Plugin", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            else
                            {
                                T_CDSSPlugins oCDSSPlugins = new T_CDSSPlugins();
                                oCDSSPlugins.Codice = rep.Codice;
                                if (oCDSSPlugins.TrySelect())
                                {
                                    bool bcontinua = true;
                                    bcontinua = CoreStatics.controlloPreliminareParametriPlugin(rep);
                                    if (bcontinua)
                                    {
                                        Dictionary<string, object> oDictionary = new Dictionary<string, object>();
                                        oDictionary = CoreStatics.getDictionaryParametriPlugin(rep);
                                        Plugin oPlugin = new Plugin(oCDSSPlugins.Codice, oCDSSPlugins.Descrizione, oCDSSPlugins.NomePlugin, oCDSSPlugins.Comando, rep.ParametriXML, "", 0, null);
                                        object[] myparam = new object[1] { oDictionary };
                                        Risposta oRisposta = PluginClientStatics.PluginClientMenuEsegui(oPlugin, myparam);
                                        if (oRisposta.Parameters != null
                                            && oRisposta.Parameters.Length == 1
                                            && oRisposta.Parameters[0].GetType() == typeof(string))
                                        {
                                            string spdf = oRisposta.Parameters[0] as string;
                                            if (System.IO.File.Exists(spdf))
                                            {
                                                filesreferti.Add(spdf);
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ExGest(ref ex, "generaPDFReferti", "CoreStatics");
                    }
















                    if (filesreferti != null && filesreferti.Count > 0)
                    {
                        sReturn = "REFS" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + @".pdf";
                        sReturn = System.IO.Path.Combine(FileStatics.GetSCCITempPath() + sReturn);

                        easyStatics.MergePDFFiles(filesreferti, sReturn, false);

                        if (!System.IO.File.Exists(sReturn))
                        {
                            sReturn = "";
                        }

                    }

                }
                return sReturn;

            }
            catch (Exception)
            {
                throw;
            }
        }

        internal static StringBuilder getPlaceHolder()
        {

            StringBuilder sb = new StringBuilder();

            Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);

            SqlParameterExt[] spcoll = new SqlParameterExt[1];
            string xmlParam = XmlProcs.XmlSerializeToString(op);
            spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
            DataTable oDt = Database.GetDataTableStoredProc("MSP_SelPlaceHolder", spcoll);

            if (oDt.Rows.Count > 0)
            {

                Gestore oGestore = CoreStatics.GetGestore(true);

                foreach (DataRow oDr in oDt.Rows)
                {
                    sb.Append(string.Format("{0} : {1}", oDr["Descrizione"].ToString(),
                                                            oGestore.Valutatore.Process(oDr["Codice"].ToString(), oGestore.Contesto)));
                    sb.AppendLine();
                }

                oGestore = null;

            }

            return sb;

        }

        public static byte[] GeneraPDFValidazioneDiario(string idMovDiarioClinico, bool bFirmaDigitale)
        {
            try
            {
                byte[] pdfContent = null;

                if (idMovDiarioClinico != null && idMovDiarioClinico.Trim() != "")
                {
                    Report _report = new Report(Report.COD_REPORT_DIARIO_CLINICO);
                    if (_report != null)
                    {
                        if (_report.NomePlugIn != null && _report.NomePlugIn.Trim() != "")
                        {
                            string pluginfullpath = System.Windows.Forms.Application.StartupPath + @"\Plugins\" + _report.NomePlugIn + @"\" + _report.NomePlugIn + @".dll";

                            string pdfdagenerarefullpath = "";
                            string pdfgeneratofullpath = "";

                            Dictionary<string, object> oDictionary = new Dictionary<string, object>();
                            oDictionary.Add("StringaConnessione", Database.ConnectionString);
                            oDictionary.Add("IDMovDiarioClinico", idMovDiarioClinico);
                            oDictionary.Add("XmlAmbiente", CoreStatics.CoreApplication.Ambiente.XmlSerializeToString());

                            oDictionary.Add("ExportPDF", "1");
                            if (bFirmaDigitale)
                                oDictionary.Add("FirmaDigitale", "1");
                            else
                                oDictionary.Add("FirmaDigitale", "0");

                            oDictionary.Add("UtenteFirma", CoreStatics.CoreApplication.Sessione.Utente.Descrizione);


                            pdfdagenerarefullpath = "DCL" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + @".pdf";
                            pdfdagenerarefullpath = System.IO.Path.Combine(System.IO.Path.GetTempPath() + pdfdagenerarefullpath);
                            oDictionary.Add("ExportPDFFullPath", pdfdagenerarefullpath);

                            T_CDSSPlugins oCDSSPlugins = new T_CDSSPlugins();
                            oCDSSPlugins.Codice = Report.COD_REPORT_DIARIO_CLINICO;
                            if (oCDSSPlugins.TrySelect())
                            {

                                Scci.PluginClient.Plugin oPlugin = new Scci.PluginClient.Plugin(oCDSSPlugins.Codice, oCDSSPlugins.Descrizione, oCDSSPlugins.NomePlugin, oCDSSPlugins.Comando, "", "", 0, null);
                                object[] myparam = new object[1] { oDictionary };

                                Risposta oRisposta = PluginClientStatics.PluginClientMenuEsegui(oPlugin, myparam);
                                if (oRisposta.Parameters != null)
                                {

                                    if (oRisposta.Parameters.Length == 1 && oRisposta.Parameters[0].GetType() == typeof(string))
                                    {

                                        string spdf = oRisposta.Parameters[0] as string;
                                        if (System.IO.File.Exists(spdf))
                                        {
                                            pdfgeneratofullpath = spdf;
                                        }

                                    }

                                }

                            }
                            else
                            {

                                string nomeplugin = System.IO.Path.GetFileNameWithoutExtension(pluginfullpath);
                                PluginCaller proxy = new PluginCaller(pluginfullpath, nomeplugin,
                                                                    SessioneRemota: CoreStatics.CoreApplication.Sessione.Computer.SessioneRemota,
                                                                    IsOSServer: CoreStatics.CoreApplication.Sessione.Computer.IsOSServer);
                                if (proxy != null)
                                {
                                    object ret = proxy.Esegui(oDictionary);
                                    if (ret != null) pdfgeneratofullpath = (string)ret;
                                }
                                else
                                {
                                    throw new Exception("Errore nel caricamento del plugin");
                                }

                                proxy.Dispose();
                                proxy = null;
                            }


                            if (pdfgeneratofullpath != null && pdfgeneratofullpath.Trim() != "" && System.IO.File.Exists(pdfgeneratofullpath))
                            {
                                pdfContent = System.IO.File.ReadAllBytes(pdfgeneratofullpath);
                                try
                                {
                                    System.IO.File.Delete(pdfgeneratofullpath);
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                }

                return pdfContent;
            }
            catch (Exception)
            {

                throw;
            }
        }

        internal static byte[] GeneraPDFValidazioneScheda(string idMovScheda, bool bFirmaDigitale)
        {

            byte[] pdfContent = null;

            try
            {

                if (idMovScheda != null && idMovScheda.Trim() != "")
                {

                    Report _report = new Report(Report.COD_REPORT_SCHEDA_FIRMA_DIGITALE);
                    if (_report != null)
                    {

                        if (_report.NomePlugIn != null && _report.NomePlugIn.Trim() != "")
                        {

                            string pluginfullpath = System.Windows.Forms.Application.StartupPath + @"\Plugins\" + _report.NomePlugIn + @"\" + _report.NomePlugIn + @".dll";

                            string pdfdagenerarefullpath = "";
                            string pdfgeneratofullpath = "";

                            string nomeplugin = System.IO.Path.GetFileNameWithoutExtension(pluginfullpath);

                            PluginCaller proxy = new PluginCaller(pluginfullpath, nomeplugin,
                                                                SessioneRemota: CoreStatics.CoreApplication.Sessione.Computer.SessioneRemota,
                                                                IsOSServer: CoreStatics.CoreApplication.Sessione.Computer.IsOSServer);

                            if (proxy != null)
                            {

                                Dictionary<string, object> oDictionary = new Dictionary<string, object>();
                                oDictionary.Add("StringaConnessione", Database.ConnectionString);
                                oDictionary.Add("IDScheda", idMovScheda);
                                oDictionary.Add("CodUAAmbulatoriale", CoreStatics.CoreApplication.Paziente.CodUAAmbulatoriale);
                                oDictionary.Add("XmlAmbiente", CoreStatics.CoreApplication.Ambiente.XmlSerializeToString());

                                oDictionary.Add("ExportPDF", "1");
                                oDictionary.Add("FirmaDigitale", (bFirmaDigitale == true ? "1" : "0"));
                                oDictionary.Add("UtenteFirma", CoreStatics.CoreApplication.Sessione.Utente.Descrizione);

                                pdfdagenerarefullpath = "SCH" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + @".pdf";
                                pdfdagenerarefullpath = System.IO.Path.Combine(System.IO.Path.GetTempPath() + pdfdagenerarefullpath);
                                oDictionary.Add("ExportPDFFullPath", pdfdagenerarefullpath);

                                object ret = proxy.Esegui(oDictionary);

                                if (ret != null) pdfgeneratofullpath = (string)ret;

                            }
                            else
                            {
                                throw new Exception("Errore nel caricamento del plugin");
                            }

                            proxy.Dispose();
                            proxy = null;

                            if (pdfgeneratofullpath != null && pdfgeneratofullpath.Trim() != "" && System.IO.File.Exists(pdfgeneratofullpath))
                            {
                                pdfContent = System.IO.File.ReadAllBytes(pdfgeneratofullpath);
                                try
                                {
                                    System.IO.File.Delete(pdfgeneratofullpath);
                                }
                                catch
                                {
                                }
                            }

                        }

                    }

                }

            }
            catch (Exception)
            {
                throw;
            }

            return pdfContent;

        }

        public static byte[] GeneraPDFPrescrizioneTempi(string idMovPrescrizioneTempi, EnumStatoPrescrizioneTempi statoPerStampa, bool firmaDigitale)
        {
            try
            {
                byte[] pdfContent = null;

                if (idMovPrescrizioneTempi != null && idMovPrescrizioneTempi.Trim() != "")
                {
                    Report _report = new Report(Report.COD_REPORT_PRESCRIZIONE_TEMPI);
                    if (_report != null)
                    {
                        if (_report.NomePlugIn != null && _report.NomePlugIn.Trim() != "")
                        {
                            string pluginfullpath = System.Windows.Forms.Application.StartupPath + @"\Plugins\" + _report.NomePlugIn + @"\" + _report.NomePlugIn + @".dll";

                            string pdfdagenerarefullpath = "";
                            string pdfgeneratofullpath = "";

                            Dictionary<string, object> oDictionary = new Dictionary<string, object>();
                            oDictionary.Add("StringaConnessione", Database.ConnectionString);
                            oDictionary.Add("IDMovPrescrizioneTempi", idMovPrescrizioneTempi);
                            oDictionary.Add("StatoPerStampa", statoPerStampa.ToString());
                            oDictionary.Add("XmlAmbiente", CoreStatics.CoreApplication.Ambiente.XmlSerializeToString());

                            oDictionary.Add("ExportPDF", "1");

                            if (firmaDigitale)
                                oDictionary.Add("FirmaDigitale", "1");
                            else
                                oDictionary.Add("FirmaDigitale", "0");

                            oDictionary.Add("UtenteFirma", CoreStatics.CoreApplication.Sessione.Utente.Descrizione);

                            pdfdagenerarefullpath = "PRT" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + @".pdf";
                            pdfdagenerarefullpath = System.IO.Path.Combine(System.IO.Path.GetTempPath() + pdfdagenerarefullpath);
                            oDictionary.Add("ExportPDFFullPath", pdfdagenerarefullpath);

                            T_CDSSPlugins oCDSSPlugins = new T_CDSSPlugins();
                            oCDSSPlugins.Codice = Report.COD_REPORT_PRESCRIZIONE_TEMPI;
                            if (oCDSSPlugins.TrySelect())
                            {

                                Scci.PluginClient.Plugin oPlugin = new Scci.PluginClient.Plugin(oCDSSPlugins.Codice, oCDSSPlugins.Descrizione, oCDSSPlugins.NomePlugin, oCDSSPlugins.Comando, "", "", 0, null);
                                object[] myparam = new object[1] { oDictionary };

                                Risposta oRisposta = PluginClientStatics.PluginClientMenuEsegui(oPlugin, myparam);
                                if (oRisposta.Parameters != null)
                                {

                                    if (oRisposta.Parameters.Length == 1 && oRisposta.Parameters[0].GetType() == typeof(string))
                                    {

                                        string spdf = oRisposta.Parameters[0] as string;
                                        if (System.IO.File.Exists(spdf))
                                        {
                                            pdfgeneratofullpath = spdf;
                                        }

                                    }

                                }

                            }
                            else
                            {

                                string nomeplugin = System.IO.Path.GetFileNameWithoutExtension(pluginfullpath);
                                PluginCaller proxy = new PluginCaller(pluginfullpath, nomeplugin,
                                                                    SessioneRemota: CoreStatics.CoreApplication.Sessione.Computer.SessioneRemota,
                                                                    IsOSServer: CoreStatics.CoreApplication.Sessione.Computer.IsOSServer);
                                if (proxy != null)
                                {
                                    object ret = proxy.Esegui(oDictionary);

                                    if (ret != null) pdfgeneratofullpath = (string)ret;

                                }
                                else
                                {
                                    throw new Exception("Errore nel caricamento del plugin");
                                }

                                proxy.Dispose();
                                proxy = null;

                            }

                            if (pdfgeneratofullpath != null && pdfgeneratofullpath.Trim() != "" && System.IO.File.Exists(pdfgeneratofullpath))
                            {
                                pdfContent = System.IO.File.ReadAllBytes(pdfgeneratofullpath);
                                try
                                {
                                    System.IO.File.Delete(pdfgeneratofullpath);
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                }

                return pdfContent;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region Utilità

        internal static Array SetSplit(string Key)
        {
            return SetSplit(Key, "|");
        }
        internal static Array SetSplit(string Key, string Separator)
        {
            Array ar = Key.Split(Separator.ToCharArray());
            return ar;
        }

        public static string FixNewLine(string sText)
        {
            string sReturn = sText;
            try
            {
                sReturn = sReturn.Replace("\n", "\r\n");
                sReturn = sReturn.Replace("\r\r\n", "\r\n");
            }
            catch
            {
                sReturn = sText;
            }
            return sReturn;
        }

        internal static Cursor setCursor(enum_app_cursors cursorType)
        {

            switch (cursorType)
            {

                case enum_app_cursors.DefaultCursor:
                    return Cursors.Default;

                case enum_app_cursors.WaitCursor:
                    if (CoreStatics.CoreApplication.Sessione.Computer.IsOSServer)
                        return Cursors.WaitCursor;
                    else
                    {
                        if (m_CursorBusy == null)
                        {
                            m_CursorBusy = new Cursor(GetCursorResize(cursorType, 20).GetHicon());
                        }
                        return m_CursorBusy;
                    }

                default:
                    return Cursors.Default;

            }

        }

        private static Bitmap GetCursorResize(enum_app_cursors cursorType, int nResize)
        {
            Screen oScreen = Screen.PrimaryScreen;
            int nSize = oScreen.WorkingArea.Height / 100 * nResize;
            Bitmap oThumb = null;

            switch (cursorType)
            {

                case enum_app_cursors.DefaultCursor:
                    break;

                case enum_app_cursors.WaitCursor:
                    oThumb = new Bitmap(ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_CURSORWAIT), nSize, nSize);
                    break;

            }

            return oThumb;
        }

        public static void impostaCursore(ref Form rfrm, enum_app_cursors cursorType)
        {
            impostaCursore(ref rfrm, cursorType, false);
        }
        public static void impostaCursore(ref Form rfrm, enum_app_cursors cursorType, bool skipDoEvents)
        {
            try
            {
                if (rfrm != null)
                {
                    rfrm.Cursor = setCursor(cursorType);
                    if (rfrm.Controls != null && rfrm.Controls.Count > 0)
                    {
                        for (int i = 0; i < rfrm.Controls.Count; i++)
                        {
                            try
                            {
                                Control ctrl = rfrm.Controls[i];
                                impostaCursore(ref ctrl, cursorType);
                            }
                            catch
                            {
                            }
                        }
                    }

                    if (skipDoEvents) System.Windows.Forms.Application.DoEvents();
                }
            }
            catch
            {
            }
        }
        public static void impostaCursore(ref UserControl ructrl, enum_app_cursors cursorType)
        {
            try
            {
                if (ructrl != null)
                {
                    ructrl.Cursor = setCursor(cursorType);
                    if (ructrl.Controls != null && ructrl.Controls.Count > 0)
                    {
                        for (int i = 0; i < ructrl.Controls.Count; i++)
                        {
                            try
                            {
                                Control ctrl = ructrl.Controls[i];
                                impostaCursore(ref ctrl, cursorType);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }
        internal static void impostaCursore(ref Control rctrl, enum_app_cursors cursorType)
        {
            try
            {
                if (rctrl != null)
                {
                    rctrl.Cursor = setCursor(cursorType);
                    if (rctrl.Controls != null && rctrl.Controls.Count > 0)
                    {
                        for (int i = 0; i < rctrl.Controls.Count; i++)
                        {
                            try
                            {
                                Control ctrl = rctrl.Controls[i];
                                impostaCursore(ref ctrl, cursorType);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public static void ScrollUp(ScrollableControl control, bool isLargeChange)
        {
            if (control != null)
            {
                int changeAmount;

                if (isLargeChange == false)
                {
                    changeAmount = control.VerticalScroll.SmallChange * 3;
                }
                else
                {
                    changeAmount = control.VerticalScroll.LargeChange;
                }

                int currentPosition = control.VerticalScroll.Value;

                if ((currentPosition - changeAmount) > control.VerticalScroll.Minimum)
                {
                    control.VerticalScroll.Value -= changeAmount;
                }
                else
                {
                    control.VerticalScroll.Value = control.VerticalScroll.Minimum;
                }

                control.PerformLayout();
            }
        }

        public static void ScrollDown(ScrollableControl control, bool isLargeChange)
        {
            if (control != null)
            {
                int changeAmount;


                if (isLargeChange == false)
                {
                    changeAmount = control.VerticalScroll.SmallChange * 3;
                }
                else
                {
                    changeAmount = control.VerticalScroll.LargeChange;
                }

                int currentPosition = control.VerticalScroll.Value;


                if ((currentPosition + changeAmount) > control.VerticalScroll.Maximum)
                {
                    control.VerticalScroll.Value += changeAmount;
                }
                else
                {
                    control.VerticalScroll.Value = control.VerticalScroll.Maximum;
                }

                control.PerformLayout();
            }
        }

        #endregion

        #region Errori

        public static DialogResult ExGest(ref Exception roEx, string vsSubRoutine, string vsModuleName)
        {
            return ExGest(ref roEx, vsSubRoutine, vsModuleName, true, vsModuleName, @"Errore", MessageBoxIcon.Error, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);
        }
        public static DialogResult ExGest(ref Exception roEx, string errormessage, string vsSubRoutine, string vsModuleName)
        {
            return ExGest(ref roEx, errormessage, vsSubRoutine, vsModuleName, true, vsModuleName, @"Errore", MessageBoxIcon.Error, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);
        }
        public static DialogResult ExGest(ref Exception roEx, string vsSubRoutine, string vsModuleName, bool vAddDebugInfo, string vsMessage, string vsMessageTitle, MessageBoxIcon vMessageIcon, MessageBoxButtons vMessageButtons, MessageBoxDefaultButton vMessageDefButton)
        {

            DialogResult vRet = DialogResult.OK;
            string sMsg = "";
            string sTitle = "";

            try
            {
                if (vAddDebugInfo) UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(roEx);
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }

            try
            {
                sMsg += roEx.Message;
                if (roEx.Source != null && roEx.Source.Trim() != "")
                    sMsg += Environment.NewLine + @"Source: " + roEx.Source;

                if (roEx.InnerException != null && roEx.InnerException.Message != "")
                    sMsg += Environment.NewLine + @"Inner: " + roEx.InnerException.Message;

                if (vsSubRoutine != null && vsSubRoutine.Trim() != "")
                    sMsg += Environment.NewLine + @"Sub: " + vsSubRoutine;

                if (vsModuleName != null && vsModuleName.Trim() != "")
                    sMsg += Environment.NewLine + @"Module: " + vsModuleName;

                if (vsMessage != null && vsMessage.Trim() != "")
                    sMsg += Environment.NewLine + vsMessage;

                if (vsMessageTitle == null || vsMessageTitle.Trim() == "")
                    sTitle = @"Errore";
                else
                    sTitle = vsMessageTitle;

                vRet = easyStatics.EasyErrorMessageBox(sMsg, sTitle, vMessageButtons, vMessageIcon);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }

            return vRet;
        }
        public static DialogResult ExGest(ref Exception roEx, string errormessage, string vsSubRoutine, string vsModuleName, bool vAddDebugInfo, string vsMessage, string vsMessageTitle, MessageBoxIcon vMessageIcon, MessageBoxButtons vMessageButtons, MessageBoxDefaultButton vMessageDefButton)
        {

            DialogResult vRet = DialogResult.OK;
            string sMsg = "";
            string sTitle = "";

            try
            {
                if (vAddDebugInfo) UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(errormessage + Environment.NewLine + roEx.Message);
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }

            try
            {
                sMsg += roEx.Message;
                if (roEx.Source != null && roEx.Source.Trim() != "")
                    sMsg += Environment.NewLine + @"Source: " + roEx.Source;

                if (roEx.InnerException != null && roEx.InnerException.Message != "")
                    sMsg += Environment.NewLine + @"Inner: " + roEx.InnerException.Message;

                if (vsSubRoutine != null && vsSubRoutine.Trim() != "")
                    sMsg += Environment.NewLine + @"Sub: " + vsSubRoutine;

                if (vsModuleName != null && vsModuleName.Trim() != "")
                    sMsg += Environment.NewLine + @"Module: " + vsModuleName;

                if (vsMessage != null && vsMessage.Trim() != "")
                    sMsg += Environment.NewLine + vsMessage;

                if (vsMessageTitle == null || vsMessageTitle.Trim() == "")
                    sTitle = @"Errore";
                else
                    sTitle = vsMessageTitle;

                vRet = easyStatics.EasyErrorMessageBox(sMsg, errormessage, sTitle, vMessageButtons, vMessageIcon);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }

            return vRet;
        }

        #endregion

        #region RTF

        internal static void BarraTestoRTF(ref ucRichTextBox ucrtfbox)
        {
            ucrtfbox.ViewRtf = ucrtfbox.ViewRtf.Insert(ucrtfbox.ViewRtf.LastIndexOf("}", ucrtfbox.ViewRtf.Length - 4) + 1, @"\strike");
            ucrtfbox.ViewRtf = ucrtfbox.ViewRtf.Insert(ucrtfbox.ViewRtf.LastIndexOf("}", ucrtfbox.ViewRtf.Length) - 1, @"\strike0");
        }

        public static ucRichTextBox GetRichTextBoxForPopupControlContainer(string testo, bool isRtf = true)
        {

            ucRichTextBox _ucRichTextBox = new ucRichTextBox();

            try
            {
                _ucRichTextBox.Size = new Size(400, 300);
                _ucRichTextBox.ViewReadOnly = true;
                if (isRtf == true)
                {
                    _ucRichTextBox.ViewRtf = testo;
                }
                else
                {
                    _ucRichTextBox.ViewText = testo;
                }
                _ucRichTextBox.rtbRichTextBox.BorderStyle = BorderStyle.None;
                _ucRichTextBox.rtbRichTextBox.BackColor = Color.WhiteSmoke;
                _ucRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            }
            catch (Exception)
            {

            }

            return _ucRichTextBox;

        }

        #endregion

        #region Gestore x Dati Clinici, Valutatore e dipendenze

        public static Gestore GetGestore()
        {
            return GetGestore(false);
        }
        public static Gestore GetGestore(bool datiselezionati)
        {
            return GetGestore(datiselezionati, false);
        }
        public static Gestore GetGestore(bool datiselezionati, bool solalettura)
        {

            try
            {

                Gestore oGestore = new Gestore();

                oGestore.Valutatore.Fillers.Add("FillerSistema", new EvaluatorFillerSistema(solalettura));
                oGestore.Valutatore.Fillers.Add("FillerScci", new EvaluatorFillerScci());
                oGestore.Valutatore.Fillers.Add("FillerScheda", new EvaluatorFillerScheda());
                oGestore.Valutatore.Fillers.Add("FillerAltraScheda", new EvaluatorFillerAltraScheda());
                oGestore.Valutatore.Fillers.Add("FillerDLookUp", new EvaluatorFillerDLookUp());
                oGestore.Valutatore.Fillers.Add("FillerGeneric", new UnicodeSrl.Evaluator.EvaluatorFillerGeneric());

                oGestore.Contesto = new Dictionary<string, object>();
                oGestore.Contesto = (from x in CoreStatics.CoreApplication.Ambiente.Contesto
                                     where (x.Value != null &&
                                            x.Value.GetType() != typeof(UnicodeSrl.DatiClinici.DC.DcSchedaDati) &&
                                            x.Value.GetType() != typeof(string))
                                     select x).ToDictionary(x => x.Key, x => x.Value);

                if (datiselezionati == true)
                {


                    Dictionary<string, object> odict = new Dictionary<string, object>();

                    odict = (from x in CoreStatics.CoreApplication.Ambiente.Contesto
                             where (x.Value != null &&
                                    x.Value.GetType() != typeof(UnicodeSrl.DatiClinici.DC.DcSchedaDati) &&
                                    x.Value.GetType() == typeof(string))
                             select x).ToDictionary(x => x.Key, x => x.Value);

                    foreach (KeyValuePair<string, object> pair in odict)
                    {
                        switch ((EnumEntita)Enum.Parse(typeof(EnumEntita), pair.Key))
                        {

                            case EnumEntita.EVC:
                                if (CoreApplication.MovEvidenzaClinicaSelezionato == null)
                                {
                                    MovEvidenzaClinica oMovEvidenzaClinica = new MovEvidenzaClinica("", "", "", "", "", "", DateTime.MinValue, DateTime.MinValue);
                                    oMovEvidenzaClinica.IDRefertoDWH = pair.Value.ToString();
                                    oGestore.Contesto.Add("MovEvidenzaClinica", oMovEvidenzaClinica);
                                }
                                else
                                {
                                    oGestore.Contesto.Add("MovEvidenzaClinica", CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato);
                                }
                                break;

                            case EnumEntita.PRF:
                                MovPrescrizione oMovPrescrizione = new MovPrescrizione("", "", "", "", null);
                                oMovPrescrizione.IDPrescrizione = pair.Value.ToString();
                                oGestore.Contesto.Add("MovPrescrizione", oMovPrescrizione);
                                break;

                            case EnumEntita.PVT:
                                MovParametroVitale oMovParametroVitale = new MovParametroVitale("", "", "", "", null);
                                oMovParametroVitale.IDMovParametroVitale = pair.Value.ToString();
                                oGestore.Contesto.Add("MovParametroVitale", oMovParametroVitale);
                                break;

                            case EnumEntita.OE:
                                MovOrdine oMovOrdine = new MovOrdine(null, "", "", "", "", "", "", "", "", null);
                                oMovOrdine.IDOrdine = pair.Value.ToString();
                                oGestore.Contesto.Add("MovOrdine", oMovOrdine);
                                break;

                            case EnumEntita.DCL:
                                MovDiarioClinico oMovDiarioClinico = new MovDiarioClinico("", "", "", "", null);
                                oMovDiarioClinico.IDMovDiario = pair.Value.ToString();
                                oGestore.Contesto.Add("MovDiarioClinico", oMovDiarioClinico);
                                break;

                            case EnumEntita.WKI:
                                MovTaskInfermieristico oMovTaskInfermieristico = new MovTaskInfermieristico("", "", "", "", EnumCodSistema.WKI, EnumTipoRegistrazione.A, null);
                                oMovTaskInfermieristico.IDMovTaskInfermieristico = pair.Value.ToString();
                                oGestore.Contesto.Add("MovTaskInfermieristico", oMovTaskInfermieristico);
                                break;

                            case EnumEntita.SCH:
                                MovScheda oMovScheda = new MovScheda("", EnumEntita.SCH, "", "", "", "", null);
                                oMovScheda.IDMovScheda = pair.Value.ToString();
                                oGestore.Contesto.Add("MovScheda", oMovScheda);
                                break;

                            case EnumEntita.APP:
                                MovAppuntamento oMovAppuntamento = new MovAppuntamento("", "", "", "");
                                oMovAppuntamento.IDAppuntamento = pair.Value.ToString();
                                oGestore.Contesto.Add("MovAppuntamento", oMovAppuntamento);
                                break;

                            case EnumEntita.EPI:
                                Episodio oEpisodio = new Episodio();
                                oEpisodio.ID = pair.Value.ToString();
                                oGestore.Contesto.Add("MovEpisodio", oEpisodio);
                                break;

                            case EnumEntita.PAZ:
                                Paziente oPaziente = new Paziente();
                                oPaziente.ID = pair.Value.ToString();
                                oGestore.Contesto.Add("MovPaziente", oPaziente);
                                break;

                            case EnumEntita.ALG:
                                MovAlertGenerico oMovAlertGenerico = new MovAlertGenerico("", "", "", "");
                                oMovAlertGenerico.IDMovAlertGenerico = pair.Value.ToString();
                                oGestore.Contesto.Add("MovAlertGenerico", oMovAlertGenerico);
                                break;

                            default:
                                break;

                        }

                    }

                }


                return oGestore;

            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static Gestore GetGestore(MovScheda movscheda)
        {

            Gestore gestore = CoreStatics.GetGestore();

            try
            {

                gestore.SchedaXML = movscheda.Scheda.StrutturaXML;
                gestore.SchedaLayoutsXML = movscheda.Scheda.LayoutXML;
                gestore.Decodifiche = movscheda.Scheda.DizionarioValori();
                gestore.SchedaDatiXML = movscheda.DatiXML;

            }
            catch (Exception)
            {


            }

            return gestore;

        }

        public static void SetContesto(EnumEntita entita, object value)
        {

            try
            {
                CoreStatics.CoreApplication.Ambiente.Contesto[entita.ToString()] = value.ToString();
            }
            catch (Exception)
            {

            }

        }
        public static void SetContesto(EnumEntita entita, Infragistics.Win.UltraWinGrid.UltraGridRow row)
        {

            try
            {

                if (CoreStatics.CoreApplication.Ambiente.Contesto.ContainsKey(entita.ToString()) == false)
                {
                    CoreStatics.CoreApplication.Ambiente.Contesto.Add(entita.ToString(), null);
                }

                if (row != null && row.IsDataRow == true)
                {
                    switch (entita)
                    {

                        case EnumEntita.EVC:
                            if (row.Cells.Exists("IDSCCI") == true)
                            {
                                CoreStatics.CoreApplication.Ambiente.Contesto[entita.ToString()] = row.Cells["IDRefertoDWH"].Text;
                            }
                            break;

                        case EnumEntita.OE:
                            if (row.Cells.Exists("NumeroOrdine") == true)
                            {
                                CoreStatics.CoreApplication.Ambiente.Contesto[entita.ToString()] = row.Cells["NumeroOrdine"].Text;
                            }
                            break;

                        case EnumEntita.APP:
                        case EnumEntita.DCL:
                        case EnumEntita.PRF:
                        case EnumEntita.PVT:
                        case EnumEntita.WKI:
                        case EnumEntita.ALG:
                        case EnumEntita.CSG:
                        case EnumEntita.CSP:
                            if (row.Cells.Exists("ID") == true)
                            {
                                CoreStatics.CoreApplication.Ambiente.Contesto[entita.ToString()] = row.Cells["ID"].Text;
                            }
                            break;

                        default:
                            break;

                    }
                }
                else
                {
                    CoreStatics.CoreApplication.Ambiente.Contesto[entita.ToString()] = null;
                }

            }
            catch (Exception)
            {

            }

        }

        public static bool SchedaBloccata(EnumEntita entita, string id)
        {
            return SchedaBloccata(entita.ToString(), id);
        }
        public static bool SchedaBloccata(string entita, string id)
        {

            bool bret = false;

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodAzioneLock", EnumLock.INFO.ToString());
                op.TimeStamp.CodEntita = entita;
                op.TimeStamp.IDEntita = id;
                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataTable dt = Database.GetDataTableStoredProc("MSP_InfoMovLock", spcoll);

                if (dt.Rows[0]["DataLock"] == DBNull.Value)
                {
                    bret = false;
                }
                else
                {
                    bret = true;
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "SchedaBloccata", "CoreStatics");
            }

            return bret;

        }

        public static void OrdinaSelezioneMultipla(ref string valore, ref string transcodifica)
        {

            try
            {

                if (valore.Count(f => f == '|') > 1)
                {

                    string[] arv_val = valore.Split('|');
                    string[] arv_tra = transcodifica.Split('|');

                    SortedList<string, string> sl_valtra = new SortedList<string, string>();
                    for (int i = 0; i < arv_val.Length; i++)
                    {
                        if (arv_val[i] != string.Empty)
                        {
                            sl_valtra.Add(arv_tra[i], arv_val[i]);
                        }
                    }

                    valore = string.Join("|", sl_valtra.Values) + "|";
                    transcodifica = string.Join("|", sl_valtra.Keys) + "|";

                }

            }
            catch (Exception)
            {

            }

        }

        #endregion

        #region Anteprime Worklist Prescrizioni Tempi

        public static ucEasyGrid getGridOrariValidati(string idPrescrizioneTempi, string idPrescrizione)
        {

            ucEasyGrid gridret = null;

            try
            {

                gridret = new ucEasyGrid();

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodSistema", EnumSistemi.PRF.ToString());
                op.Parametro.Add("IDSistema", idPrescrizione);
                op.Parametro.Add("IDGruppo", idPrescrizioneTempi);

                op.TimeStamp.CodEntita = EnumEntita.PRF.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_SelMovTaskInfermieristici", spcoll);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    ds.Tables[0].DefaultView.Sort = "DataRiferimento desc";
                    gridret.DataSource = ds.Tables[0].DefaultView;
                    gridret.Text = string.Format("Numero Task: {0}", ds.Tables[0].Rows.Count.ToString()) + getInfoPrescrizioniTempi(idPrescrizioneTempi, idPrescrizione);

                }
                else
                {
                    gridret.DataSource = null;
                }

                gridret.Refresh();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "getGridOrariValidati", "CoreStatics");
            }
            return gridret;
        }

        internal static ucEasyGrid getGridOrariDaValidare(string idPrescrizioneTempi, string idPrescrizione, ref Boolean bVisualizzaOraFine, ref Boolean bVisualizzaEtichetta)
        {
            ucEasyGrid gridret = null;
            try
            {
                gridret = new ucEasyGrid();

                MovPrescrizioneTempi movtmp = new MovPrescrizioneTempi(idPrescrizioneTempi, idPrescrizione, CoreStatics.CoreApplication.Ambiente);

                List<IntervalloTempi> tempi = movtmp.GeneraPeriodicita();


                if (tempi.Count > 0)
                {
                    gridret.DataSource = tempi;
                    gridret.DataMember = "";
                    bVisualizzaOraFine = movtmp.Continuita;
                    bVisualizzaEtichetta = (movtmp.CodProtocollo != string.Empty);
                }
                else
                {
                    gridret.DataSource = null;
                    bVisualizzaOraFine = false;
                    bVisualizzaEtichetta = false;
                }

                gridret.Refresh();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "getGridOrariDaValidare", "CoreStatics");
            }
            return gridret;
        }

        internal static ucEasyGrid getGridOrariPrescrizioni(string idPrescrizione)
        {

            ucEasyGrid gridret = null;

            try
            {

                gridret = new ucEasyGrid();

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodSistema", EnumSistemi.PRF.ToString());
                op.Parametro.Add("IDSistema", idPrescrizione);

                op.TimeStamp.CodEntita = EnumEntita.PRF.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_SelMovTaskInfermieristici", spcoll);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    ds.Tables[0].DefaultView.Sort = "DataRiferimento desc";
                    gridret.DataSource = ds.Tables[0].DefaultView;
                    gridret.Text = string.Format("Numero Task: {0}", ds.Tables[0].Rows.Count.ToString()) + getInfoPrescrizioni(idPrescrizione);
                }
                else
                {
                    gridret.DataSource = null;
                }
                gridret.Refresh();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "getGridOrariValidati", "CoreStatics");
            }
            return gridret;
        }

        internal static string getInfoPrescrizioni(string idPrescrizione)
        {

            string sret = string.Empty;

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDEpisodio", CoreStatics.CoreApplication.Episodio.ID);
                op.Parametro.Add("IDPrescrizione", idPrescrizione);
                op.Parametro.Add("DatiEstesi", "0");

                op.TimeStamp.CodEntita = EnumEntita.PRF.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_SelMovPrescrizioni", spcoll);

                if (ds != null && ds.Tables[0].Rows.Count == 1)
                {

                    sret = (ds.Tables[0].Rows[0]["US"].ToString() != string.Empty ? Environment.NewLine + ds.Tables[0].Rows[0]["US"].ToString() : "") +
                            (ds.Tables[0].Rows[0]["PP"].ToString() != string.Empty ? Environment.NewLine + ds.Tables[0].Rows[0]["PP"].ToString() : "") +
                            (ds.Tables[0].Rows[0]["UP"].ToString() != string.Empty ? Environment.NewLine + ds.Tables[0].Rows[0]["UP"].ToString() : "");

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "getInfoPrescrizioni", "CoreStatics");
            }

            return sret;

        }

        internal static string getInfoPrescrizioniTempi(string idPrescrizioneTempi, string idPrescrizione)
        {

            string sret = string.Empty;

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDPrescrizione", idPrescrizione);
                op.Parametro.Add("IDPrescrizioneTempi", idPrescrizioneTempi);
                op.Parametro.Add("DatiEstesi", "0");

                op.TimeStamp.CodEntita = EnumEntita.PRF.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_SelMovPrescrizioniTempi", spcoll);

                if (ds != null && ds.Tables[0].Rows.Count == 1)
                {

                    sret = (ds.Tables[0].Rows[0]["US"].ToString() != string.Empty ? Environment.NewLine + ds.Tables[0].Rows[0]["US"].ToString() : "") +
                            (ds.Tables[0].Rows[0]["PP"].ToString() != string.Empty ? Environment.NewLine + ds.Tables[0].Rows[0]["PP"].ToString() : "") +
                            (ds.Tables[0].Rows[0]["UP"].ToString() != string.Empty ? Environment.NewLine + ds.Tables[0].Rows[0]["UP"].ToString() : "");

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "getInfoPrescrizioniTempi", "CoreStatics");
            }

            return sret;

        }

        public static ucEasyGrid getGridAppuntamentixTipo(DateTime datainizio, DateTime datafine, string codagenda)
        {

            ucEasyGrid gridret = null;

            try
            {

                gridret = new ucEasyGrid();

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodAgenda", codagenda);
                op.Parametro.Add("DataInizio", Database.dataOra105PerParametri(datainizio));
                op.Parametro.Add("DataFine", Database.dataOra105PerParametri(datafine));

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_SelInfoAppuntamentiAgende", spcoll);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    gridret.DataSource = ds.Tables[0].DefaultView;
                }
                else
                {
                    gridret.DataSource = null;
                }

                gridret.Refresh();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "getGridAppuntamentixTipo", "CoreStatics");
            }

            return gridret;

        }

        #endregion

        #region Navigazione Popup comuni

        public static void CaricaPopup(EnumMaschere _mascherascheda, EnumEntita _entita, string id, ref UserControl ucc, bool ricaricamovimento, bool attivanavigazione, object customParameters = null)
        {

            string sID = id;

            try
            {

                switch (_entita)
                {

                    case EnumEntita.SCH:
                        do
                        {
                            Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                            op.Parametro.Add("CodAzioneLock", EnumLock.LOCK.ToString());
                            op.TimeStamp.CodEntita = _entita.ToString();
                            op.TimeStamp.IDEntita = id;
                            SqlParameterExt[] spcoll = new SqlParameterExt[1];
                            string xmlParam = XmlProcs.XmlSerializeToString(op);
                            spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                            DataTable dt = Database.GetDataTableStoredProc("MSP_InfoMovLock", spcoll);

                            if (dt.Rows.Count == 1 && int.Parse(dt.Rows[0]["Esito"].ToString()) == 1)
                            {

                                bool bNuovaRevisione = false;

                                if (customParameters != null)
                                {
                                    if (customParameters is Dictionary<String, object>)
                                    {
                                        Dictionary<String, object> dict = customParameters as Dictionary<String, object>;
                                        if (dict.Keys.Contains(CoreStatics.C_PARAM_NEW_REV) && dict[CoreStatics.C_PARAM_NEW_REV] != null && dict[CoreStatics.C_PARAM_NEW_REV] is bool)
                                            bNuovaRevisione = (bool)dict[CoreStatics.C_PARAM_NEW_REV];
                                    }
                                }

                                CoreStatics.CoreApplication.MovSchedaSelezionata = new MovScheda(sID, CoreStatics.CoreApplication.Ambiente);

                                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(_mascherascheda, attivanavigazione, customParameters: customParameters) == DialogResult.OK)
                                {

                                    if (ucc != null)
                                    {
                                        CoreStatics.impostaCursore(ref ucc, enum_app_cursors.WaitCursor);
                                    }

                                    bool bValidataNew = CoreStatics.CoreApplication.MovSchedaSelezionata.ValidataNew;

                                    CoreStatics.CoreApplication.MovSchedaSelezionata.Salva(ricaricamovimento, false);

                                    if (CoreStatics.CoreApplication.MovSchedaSelezionata.Scheda.Validabile == true)
                                    {
                                        if (CoreStatics.CoreApplication.MovSchedaSelezionata.Validata != CoreStatics.CoreApplication.MovSchedaSelezionata.ValidataNew
                                            || bNuovaRevisione)
                                        {
                                            CoreStatics.Validazione(CoreStatics.CoreApplication.MovSchedaSelezionata.IDMovScheda, (bValidataNew == true ? "0" : "1"), ref ucc, false, bNuovaRevisione);
                                        }
                                    }
                                    else
                                    {
                                        string sCodUA = string.Empty;

                                        if (CoreStatics.CoreApplication.Trasferimento != null)
                                        {
                                            sCodUA = CoreStatics.CoreApplication.Trasferimento.CodUA;
                                        }
                                        else
                                        {
                                            sCodUA = CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.CodUAAmbulatorialeSelezionata;
                                        }

                                        Risposta oRispostaElabora = PluginClientStatics.PluginClient(EnumPluginClient.SCH_MODIFICA_DOPO.ToString(),
                                                                                                    new object[1] { new object() },
                                                                                                    CommonStatics.UAPadri(sCodUA, CoreStatics.CoreApplication.Ambiente));
                                    }

                                    if (ucc != null)
                                    {
                                        CoreStatics.impostaCursore(ref ucc, enum_app_cursors.DefaultCursor);
                                    }
                                }

                            }
                            else
                            {
                                easyStatics.EasyMessageBox("Scheda bloccata da altro operatore!", "Informazioni Scheda");
                            }

                            op.Parametro.Remove("CodAzioneLock");
                            op.Parametro.Add("CodAzioneLock", EnumLock.UNLOCK.ToString());
                            spcoll = new SqlParameterExt[1];
                            xmlParam = XmlProcs.XmlSerializeToString(op);
                            spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                            dt = Database.GetDataTableStoredProc("MSP_InfoMovLock", spcoll);

                            if (CoreStatics.CoreApplication.MovSchedaSelezionata != null)
                            {
                                sID = CoreStatics.CoreApplication.MovSchedaSelezionata.IDPin;
                            }
                            else
                            {
                                sID = string.Empty;
                            }

                        } while (sID != string.Empty);
                        CoreStatics.CoreApplication.MovSchedaSelezionata = null;
                        break;

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaPopup", "CoreStatics");
            }

        }
        internal static void CaricaPopup(EnumMaschere _mascherascheda, EnumEntita _entita, string id)
        {
            UserControl ucc = null;
            CaricaPopup(_mascherascheda, _entita, id, ref ucc, true, true);

        }

        public static void SetNavigazione(bool enable)
        {
            if (CoreStatics.CoreApplicationContext.MainForm == null)
                return;

            if ((CoreStatics.CoreApplicationContext.MainForm is frmMain) == false)
                return;


            if (enable == false) { CoreStatics.CoreApplication.Navigazione.Maschere.SetNavigare(enable); }

            var ucT = ((frmMain)CoreStatics.CoreApplicationContext.MainForm).ucTop;
            var ucB = ((frmMain)CoreStatics.CoreApplicationContext.MainForm).ucBottom;

            ucT.Enabled = enable;
            ucB.Enabled = enable;
            ucB.MenuPulsanteAvantiEnabled = ucB.Enabled;

            if (enable == true) { CoreStatics.CoreApplication.Navigazione.Maschere.SetNavigare(enable); }

        }

        public static void Validazione(string idmovscheda, string validata, ref UserControl _ucc, bool bCheckBlock, bool bNuovaRevisione = false)
        {

            CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

            CoreStatics.SetNavigazione(false);

            try
            {

                if (bCheckBlock == true)
                {

                    if (CoreStatics.SchedaBloccata(EnumEntita.SCH, idmovscheda) == false)
                    {
                        CoreStatics.Validazione(idmovscheda, validata, bNuovaRevisione);
                    }
                    else
                    {
                        easyStatics.EasyMessageBox("Scheda bloccata da altro operatore!", "Informazioni Scheda");
                    }

                }
                else
                {
                    CoreStatics.Validazione(idmovscheda, validata, bNuovaRevisione);
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Validazione", "ucSchede");
            }

            CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);

            CoreStatics.SetNavigazione(true);

        }
        private static void Validazione(string idmovscheda, string validata, bool bNuovaRevisione = false)
        {

            Parametri op = null;
            SqlParameterExt[] spcoll;
            string xmlParam = "";

            try
            {

                op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDScheda", idmovscheda);
                op.Parametro.Add("Validata", validata == "1" ? "0" : "1");
                op.TimeStamp.CodEntita = EnumEntita.SCH.ToString();

                if (validata == "1")
                {
                    op.TimeStamp.CodAzione = EnumAzioni.MOD.ToString();
                }
                else
                {
                    op.TimeStamp.CodAzione = EnumAzioni.VAL.ToString();
                }

                if (bNuovaRevisione == true)
                {
                    op.Parametro.Add("NuovaRevisione", "1");
                }


                spcoll = new SqlParameterExt[1];

                xmlParam = XmlProcs.XmlSerializeToString(op);
                xmlParam = XmlProcs.CheckXMLDati(xmlParam);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);



                DataTable dt = Database.GetDataTableStoredProc("MSP_AggMovSchede", spcoll);

                string sCodUA = string.Empty;
                if (CoreStatics.CoreApplication.Trasferimento != null)
                {
                    sCodUA = CoreStatics.CoreApplication.Trasferimento.CodUA;
                }
                else
                {
                    sCodUA = CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.CodUAAmbulatorialeSelezionata;
                }

                if (validata == "0")
                {

                    bool bOK = FirmaDigitale(idmovscheda);

                    if (bOK)
                    {

                        CoreStatics.CoreApplication.MovSchedaSelezionata = new MovScheda(idmovscheda, CoreStatics.CoreApplication.Ambiente);
                        Risposta oRispostaElaboraPrima = PluginClientStatics.PluginClient(EnumPluginClient.SCH_VALIDA_DOPO.ToString(),
                                                                                            new object[1] { new object() },
                                                                                            CommonStatics.UAPadri(sCodUA, CoreStatics.CoreApplication.Ambiente));

                    }
                    else
                    {

                        op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                        op.Parametro.Add("IDScheda", idmovscheda);
                        op.Parametro.Add("Validata", "0");
                        op.TimeStamp.CodEntita = EnumEntita.SCH.ToString();
                        op.TimeStamp.CodAzione = EnumAzioni.MOD.ToString();

                        spcoll = new SqlParameterExt[1];

                        xmlParam = XmlProcs.XmlSerializeToString(op);
                        xmlParam = XmlProcs.CheckXMLDati(xmlParam);
                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                        DataTable dtsvalida = Database.GetDataTableStoredProc("MSP_AggMovSchede", spcoll);

                    }

                }
                else
                {
                    CoreStatics.CoreApplication.MovSchedaSelezionata = new MovScheda(idmovscheda, CoreStatics.CoreApplication.Ambiente);
                    Risposta oRispostaElaboraPrima = PluginClientStatics.PluginClient(EnumPluginClient.SCH_SVALIDA_DOPO.ToString(),
                                                                                        new object[1] { new object() },
                                                                                        CommonStatics.UAPadri(sCodUA, CoreStatics.CoreApplication.Ambiente));
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "uchkValida_Click", "ucSchede");
            }

        }

        public static void ChiusuraCartellaAmbulatoriale(string idcartellaambulatoriale, ref UserControl _ucc)
        {

            CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

            CoreStatics.SetNavigazione(false);

            CartellaAmbulatoriale oCartellaAmbulatoriale = new CartellaAmbulatoriale(idcartellaambulatoriale, "", CoreStatics.CoreApplication.Ambiente);

            try
            {

                if (easyStatics.EasyMessageBox($"Sei sicuro di voler chiudera la cartella ambulatoriale {oCartellaAmbulatoriale.NumeroCartella} ?", "Chiusura cartella ambulatoriale", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    oCartellaAmbulatoriale.CodStatoCartella = "CH";
                    oCartellaAmbulatoriale.Salva();
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ChiusuraCartellaAmbulatoriale", "ucSchede");
            }
            finally
            {
                oCartellaAmbulatoriale = null;
            }

            CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);

            CoreStatics.SetNavigazione(true);

        }

        private static bool FirmaDigitale(string idmovscheda)
        {

            bool bret = false;

            frmSmartCardProgress frmSC = null;

            MovScheda oMovScheda = new MovScheda(idmovscheda, CoreStatics.CoreApplication.Ambiente);

            if (oMovScheda.PermessoUAFirma == 1)
            {


                try
                {

                    frmSC = new frmSmartCardProgress();
                    frmSC.InizializzaEMostra(0, 4, new Form());
                    frmSC.SetCursore(enum_app_cursors.WaitCursor);

                    frmSC.SetStato(@"Validazione Scheda");

                    try
                    {

                        frmSC.SetStato(@"Generazione Documento...");

                        byte[] pdfContent = CoreStatics.GeneraPDFValidazioneScheda(idmovscheda, true);

                        if (pdfContent == null || pdfContent.Length <= 0)
                        {
                            frmSC.SetLog(@"Errore Generazione documento", true);
                        }
                        else
                        {
                            bret = frmSC.ProvaAFirmare(ref pdfContent, EnumTipoDocumentoFirmato.SCHFM01, "Firma Digitale...", EnumEntita.SCH, idmovscheda);
                        }

                    }
                    catch (Exception ex)
                    {
                        if (frmSC != null)
                        {
                            frmSC.SetLog(@"Errore Generazione documento: " + ex.Message, true);
                        }
                    }

                }
                catch (Exception ex)
                {
                    CoreStatics.ExGest(ref ex, "Salva", "Firma Digitale");
                }
                finally
                {
                    if (frmSC != null)
                    {
                        frmSC.Close();
                        frmSC.Dispose();
                    }
                }

            }
            else
            {

                bret = true;

            }

            oMovScheda = null;

            return bret;

        }

        public static void RefreshUcTop()
        {

            var ucT = ((frmMain)CoreStatics.CoreApplicationContext.MainForm).ucTop;
            ucT.Refresh();
            ucT = null;

        }

        public static void ImpostaCursoreMainForm(enum_app_cursors cur)
        {
            try
            {
                if (CoreStatics.CoreApplicationContext.MainForm == null)
                    return;

                if ((CoreStatics.CoreApplicationContext.MainForm is frmMain) == false)
                    return;

                Form frm = CoreStatics.CoreApplicationContext.MainForm;
                CoreStatics.impostaCursore(ref frm, cur);
            }
            catch
            {
            }
        }

        #endregion

        #region Controllo Numerosità poup di selezione Tipi

        public static bool CheckSelezionaTipoVoceDiario()
        {

            bool bret = false;

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodUA", CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.CodUA);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                op.Parametro.Add("CodAzione", CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.Azione.ToString());

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable oDt = Database.GetDataTableStoredProc("MSP_SelTipoDiarioClinico", spcoll);

                if (oDt.Rows.Count == 1)
                {
                    CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.CodTipoVoceDiario = oDt.Rows[0]["CodVoce"].ToString();
                    CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.DescrTipoVoceDiario = oDt.Rows[0]["Descrizione"].ToString();
                    CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.CodScheda = oDt.Rows[0]["CodScheda"].ToString();
                    bret = true;
                }

            }
            catch (Exception)
            {

            }

            return bret;

        }

        public static bool CheckSelezionaTipoConsegnaPaziente(string scodua, string scodruolo, MovDiarioClinico movdiarioclinico)
        {

            bool bRet = false;

            string sCampo = "Consegna";

            try
            {

                string sTipoConsegnaDaDCL = UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.TipoConsegnaDaDCL).Trim().ToUpper();
                if (sTipoConsegnaDaDCL != string.Empty)
                {

                    Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("CodUA", scodua);
                    op.Parametro.Add("CodRuolo", scodruolo);

                    op.Parametro.Add("CodAzione", EnumAzioni.CND.ToString());


                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    FwDataParametersList procParams = new FwDataParametersList();
                    procParams.Add("xParametri", xmlParam, ParameterDirection.Input, DbType.Xml);

                    using (FwDataConnection conn = new FwDataConnection(Database.ConnectionString))
                    {

                        DataTable oDt = conn.Query<DataTable>("MSP_SelTipoConsegnaPaziente", procParams, CommandType.StoredProcedure);

                        List<DataRow> lstRow = oDt.Rows.OfType<DataRow>().Where(s => s["Codice"].ToString() == sTipoConsegnaDaDCL).ToList();
                        if (lstRow.Count == 1)
                        {

                            if (CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata != null && movdiarioclinico != null)
                            {

                                CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.CodTipoConsegnaPaziente = lstRow[0]["Codice"].ToString();
                                CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.DescrTipoConsegnaPaziente = lstRow[0]["Descrizione"].ToString();
                                CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.CodScheda = lstRow[0]["CodScheda"].ToString();
                                CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.CodUA = scodua;

                                Gestore oGestore = CoreStatics.GetGestore();

                                oGestore.SchedaXML = CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.MovScheda.Scheda.StrutturaXML;
                                oGestore.SchedaLayoutsXML = CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.MovScheda.Scheda.LayoutXML;

                                oGestore.Decodifiche = CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.MovScheda.Scheda.DizionarioValori();

                                oGestore.SchedaDati = new DcSchedaDati();
                                oGestore.NuovaScheda();

                                if (oGestore.LeggeVoce(sCampo) != null)
                                {
                                    if (oGestore.SchedaLayouts.Layouts[sCampo].TipoVoce == DatiClinici.Common.Enums.enumTipoVoce.TestoRtf)
                                    {
                                        oGestore.ModificaValore(sCampo, 1, movdiarioclinico.MovScheda.AnteprimaRTF);
                                        CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.MovScheda.DatiXML = oGestore.SchedaDatiXML;
                                        bRet = true;
                                    }
                                    else
                                    {
                                        easyStatics.EasyMessageBox($"Il campo '{sCampo}' " +
                                                                    $"della scheda '{CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.DescrTipoConsegnaPaziente} ({CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.CodScheda}) " +
                                                                    $"non è di tipo RTF !!!",
                                                                    "Creazione consegna paziente", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                                else
                                {
                                    easyStatics.EasyMessageBox($"La scheda '{CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.DescrTipoConsegnaPaziente} ({CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.CodScheda}) " +
                                                                $"non contiene il campo '{sCampo}' !!!",
                                                                "Creazione consegna paziente", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }

                            }
                            else
                            {
                                bRet = true;
                            }

                        }

                    }

                }

            }
            catch (Exception)
            {

            }

            return bRet;

        }

        #endregion

        #region Warning Configurabili

        public static DialogResult CheckWarnings(EnumTipoFiltroSpeciale vCodTipoFiltroSpeciale)
        {
            DialogResult ret = DialogResult.OK;

            try
            {

                DataTable dtSelFiltriSpeciali = CheckWarningsDT(vCodTipoFiltroSpeciale);

                if (dtSelFiltriSpeciali != null)
                {
                    if (dtSelFiltriSpeciali.Rows.Count > 0)
                    {

                        string sMainMsg = "";
                        bool bQuestion = CheckWarningsMessaggio(dtSelFiltriSpeciali, ref sMainMsg);
                        string sTitolo = "ATTENZIONE";

                        switch (vCodTipoFiltroSpeciale)
                        {
                            case EnumTipoFiltroSpeciale.TCDASS:
                                sTitolo = @"ASSEGNAZIONE NUMERO";
                                break;

                            case EnumTipoFiltroSpeciale.TCDCHI:
                                sTitolo = @"CHIAMATA NUMERO";
                                break;

                            case EnumTipoFiltroSpeciale.AMBCAR:
                                sTitolo = @"CARTELLA AMBULATORIALE";
                                break;

                            case EnumTipoFiltroSpeciale.EPICAR:
                                sTitolo = @"CARTELLA PAZIENTE";
                                break;

                            default:
                                sTitolo = "ATTENZIONE";
                                break;
                        }

                        if (sMainMsg != null && sMainMsg.Trim() != "")
                        {

                            if (bQuestion)
                            {
                                ret = easyStatics.EasyMessageBoxInfo(sMainMsg, sTitolo, sTitolo,
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question,
                                    @"SI", @"NO",
                                    easyStatics.easyRelativeDimensions.XLarge,
                                    easyStatics.easyRelativeDimensions.XXLarge,
                                    true, true);
                            }
                            else
                            {
                                easyStatics.EasyMessageBoxInfo(sMainMsg, sTitolo, sTitolo,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning,
                                null, null,
                                easyStatics.easyRelativeDimensions.XLarge,
                                easyStatics.easyRelativeDimensions.XXLarge,
                                true, true);

                            }
                        }

                    }
                    dtSelFiltriSpeciali.Dispose();
                }
            }
            catch (Exception ex)
            {
                ExGest(ref ex, "CheckWarnings-0", "Common");
            }

            return ret;
        }

        public static DataTable CheckWarningsDT(EnumTipoFiltroSpeciale vCodTipoFiltroSpeciale)
        {

            DataTable dtret = null;

            try
            {

                string sCodUA = "";
                string sCodRuolo = "";
                string sIdPaziente = "";

                sIdPaziente = CoreStatics.CoreApplication.Paziente.ID;
                sCodRuolo = CoreStatics.CoreApplication.Ambiente.Codruolo;
                if (CoreStatics.CoreApplication.Trasferimento != null && CoreStatics.CoreApplication.Episodio != null)
                {
                    sCodUA = CoreStatics.CoreApplication.Trasferimento.CodUA;
                }
                else
                {
                    sCodUA = CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata;
                }

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);

                op.Parametro.Add("CodUA", sCodUA);
                op.Parametro.Add("CodRuolo", sCodRuolo);
                op.Parametro.Add("CodTipoFiltroSpeciale", vCodTipoFiltroSpeciale.ToString());


                op.TimeStamp.CodEntita = EnumEntita.FLS.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                dtret = Database.GetDataTableStoredProc("MSP_SelFiltriSpeciali", spcoll);

            }
            catch (Exception ex)
            {
                ExGest(ref ex, "CheckWarningsDT", "Common");
            }

            return dtret;

        }

        public static bool CheckWarningsMessaggio(DataTable dtSelFiltriSpeciali, ref string messaggio)
        {

            bool bQuestion = false;

            try
            {

                if (dtSelFiltriSpeciali != null)
                {
                    if (dtSelFiltriSpeciali.Rows.Count > 0)
                    {

                        foreach (DataRow drSelFiltriSpeciali in dtSelFiltriSpeciali.Rows)
                        {
                            try
                            {
                                if (!drSelFiltriSpeciali.IsNull("SQL") && drSelFiltriSpeciali["SQL"].ToString().Trim() != "")
                                {
                                    string sSql = drSelFiltriSpeciali["SQL"].ToString().Trim();

                                    sSql = ValorizzaPlaceHolderInSQL(sSql);

                                    DataTable dtMsg = Database.GetDatatable(sSql);
                                    if (dtMsg != null)
                                    {
                                        if (dtMsg.Rows.Count > 0)
                                        {
                                            string sTmp = "";
                                            foreach (DataRow drMsg in dtMsg.Rows)
                                            {
                                                if (!drMsg.IsNull(0) && drMsg[0].ToString().Trim() != "")
                                                {
                                                    if (sTmp != "") sTmp += Environment.NewLine;
                                                    sTmp += drMsg[0].ToString();
                                                }
                                            }

                                            if (sTmp != null && sTmp.Trim() != "")
                                            {

                                                sTmp = sTmp.Replace("\r\n", "\n");
                                                sTmp = sTmp.Replace("\n", "\r\n");

                                                if (sTmp.IndexOf(@"?") > 0) bQuestion = true;

                                                if (messaggio != "") messaggio += Environment.NewLine + Environment.NewLine;
                                                messaggio += sTmp;

                                            }

                                        }
                                        dtMsg.Dispose();
                                    }

                                }
                            }
                            catch (Exception ex)
                            {
                                ExGest(ref ex, "CheckWarningsMessaggio-1", "Common");
                            }

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ExGest(ref ex, "CheckWarningsMessaggio-0", "Common");
            }

            return bQuestion;

        }

        public static string ValorizzaPlaceHolderInSQL(string vsSQL)
        {
            string sSqlReturn = vsSQL;
            StringBuilder sb = new StringBuilder();

            Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);

            SqlParameterExt[] spcoll = new SqlParameterExt[1];
            string xmlParam = XmlProcs.XmlSerializeToString(op);
            spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
            DataTable oDt = Database.GetDataTableStoredProc("MSP_SelPlaceHolder", spcoll);

            if (oDt.Rows.Count > 0)
            {

                Gestore oGestore = CoreStatics.GetGestore(true);

                foreach (DataRow oDr in oDt.Rows)
                {

                    if (sSqlReturn.ToUpper().IndexOf(oDr["Codice"].ToString().ToUpper()) >= 0)
                    {
                        string sValore = oGestore.Valutatore.Process(oDr["Codice"].ToString(), oGestore.Contesto);


                        string sPlaceHolder = @"'" + oDr["Codice"].ToString().ToUpper() + @"'".ToUpper();
                        int iStart = sSqlReturn.ToUpper().IndexOf(sPlaceHolder);
                        while (iStart >= 0)
                        {
                            string sLeft = sSqlReturn.Substring(0, iStart);
                            string sRight = sSqlReturn.Substring(iStart + sPlaceHolder.Length);

                            sSqlReturn = sLeft + @"'" + sValore + @"'" + sRight;

                            iStart = sSqlReturn.ToUpper().IndexOf(sPlaceHolder);
                        }

                        iStart = -1;
                        iStart = sSqlReturn.ToUpper().IndexOf(oDr["Codice"].ToString().ToUpper());
                        while (iStart >= 0)
                        {
                            string sLeft = sSqlReturn.Substring(0, iStart);
                            string sRight = sSqlReturn.Substring(iStart + oDr["Codice"].ToString().Length);

                            sSqlReturn = sLeft + @"'" + sValore + @"'" + sRight;

                            iStart = sSqlReturn.ToUpper().IndexOf(oDr["Codice"].ToString().ToUpper());
                        }

                    }



                }

                oGestore = null;

            }

            const string C_WARNING_PH_IDPAZIENTE = @"#[TagliaCode.IDPaziente]§"; if (sSqlReturn.ToUpper().IndexOf(C_WARNING_PH_IDPAZIENTE.ToUpper()) >= 0)
            {

                string sIdPaziente = "";
                if (CoreStatics.CoreApplication.Paziente != null) sIdPaziente = CoreStatics.CoreApplication.Paziente.ID;


                string sIDPazienteConApici = @"'" + C_WARNING_PH_IDPAZIENTE.ToUpper() + @"'".ToUpper();
                int iStart = sSqlReturn.ToUpper().IndexOf(sIDPazienteConApici);
                while (iStart >= 0)
                {
                    string sLeft = sSqlReturn.Substring(0, iStart);
                    string sRight = sSqlReturn.Substring(iStart + sIDPazienteConApici.Length);

                    sSqlReturn = sLeft + @"'" + sIdPaziente + @"'" + sRight;

                    iStart = sSqlReturn.ToUpper().IndexOf(sIDPazienteConApici);
                }

                iStart = -1;
                iStart = sSqlReturn.ToUpper().IndexOf(C_WARNING_PH_IDPAZIENTE.ToUpper());
                while (iStart >= 0)
                {
                    string sLeft = sSqlReturn.Substring(0, iStart);
                    string sRight = sSqlReturn.Substring(iStart + C_WARNING_PH_IDPAZIENTE.Length);

                    sSqlReturn = sLeft + @"'" + sIdPaziente + @"'" + sRight;

                    iStart = sSqlReturn.ToUpper().IndexOf(C_WARNING_PH_IDPAZIENTE.ToUpper());
                }
            }

            return sSqlReturn;

        }

        #endregion

        #region Ordini

        public static dynamic getParameterCopiaOrdine(bool b_ambulatoriale)
        {

            dynamic cust = new System.Dynamic.ExpandoObject();

            cust.Nosologico = "";
            cust.CodAzienda = "";
            cust.CodUO = "";

            try
            {

                if (b_ambulatoriale)
                {


                    cust.Nosologico = "";

                    cust.CodAzienda = UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.Azienda);

                    cust.CodUO = CoreStatics.CoreApplication.CodUOSelezionata;

                }
                else
                {


                    cust.Nosologico = "";
                    if (CoreStatics.CoreApplication.Episodio.NumeroEpisodio.Length > 0)
                    {
                        cust.Nosologico = CoreStatics.CoreApplication.Episodio.NumeroEpisodio;
                    }
                    else
                    {
                        cust.Nosologico = CoreStatics.CoreApplication.Episodio.NumeroListaAttesa;
                    }

                    if (CoreStatics.CoreApplication.Trasferimento.CodAziTrasferimento != null && CoreStatics.CoreApplication.Trasferimento.CodAziTrasferimento != string.Empty)
                    {
                        cust.CodAzienda = CoreStatics.CoreApplication.Trasferimento.CodAziTrasferimento;
                    }
                    else
                    {
                        cust.CodAzienda = CoreStatics.CoreApplication.Episodio.CodAzienda;
                    }

                    cust.CodUO = CoreStatics.CoreApplication.Trasferimento.CodUO;

                }

            }
            catch (Exception)
            {

            }

            return cust;

        }

        #endregion

    }

}
