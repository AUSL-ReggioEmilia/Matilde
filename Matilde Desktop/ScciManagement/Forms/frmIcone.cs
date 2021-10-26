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
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win;
using System.IO;
using System.Drawing.Imaging;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci;
using UnicodeSrl.Framework.Security;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmIcone : Form, Interfacce.IViewFormBase
    {
        public frmIcone()
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
            MyStatics.SetUltraGridLayout(ref this.UltraGrid, false, false);
            this.UltraGrid.DisplayLayout.GroupByBox.Hidden = true;
            this.UltraGrid.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            this.UltraGrid.DisplayLayout.Override.RowSizing = RowSizing.Default;
            this.UltraGrid.DisplayLayout.Override.MinRowHeight = 64;

            this.picView.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_ICONE, Enums.EnumImageSize.isz256));

            this.ResumeLayout();

        }

        #endregion

        #region UltraToolBar

        private void InitializeUltraToolbarsManager()
        {

            try
            {

                MyStatics.SetUltraToolbarsManager(this.UltraToolbarsManager);

                foreach (ToolBase oTool in this.UltraToolbarsManagerGrid.Tools)
                {
                    oTool.SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(oTool.Key, Enums.EnumImageSize.isz32));
                    oTool.SharedProps.AppearancesSmall.Appearance.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(oTool.Key, Enums.EnumImageSize.isz16));
                }

                ComboBoxTool oCbt = (ComboBoxTool)this.UltraToolbarsManagerGrid.Tools["Entità"];
                ValueList oVl = new ValueList();
                oVl.ValueListItems.Add("Tutti", "Tutti");
                oVl.ValueListItems.Add("ALG", "Alert Generici");
                oVl.ValueListItems.Add("ALA", "Alert Allergie Anamesi");
                oVl.ValueListItems.Add("ALL", "Allegati");
                oVl.ValueListItems.Add("ALLFMT", "Allegati Formato");
                oVl.ValueListItems.Add("APP", "Appuntamenti");
                oVl.ValueListItems.Add("CNC", "Consensi");
                oVl.ValueListItems.Add("CSG", "Consegne");
                oVl.ValueListItems.Add("CSP", "Consegne Paziente");
                oVl.ValueListItems.Add("DCL", "Diario Clinico");
                oVl.ValueListItems.Add("EPI", "Episodio");
                oVl.ValueListItems.Add("EVC", "Evidenza Clinica");
                oVl.ValueListItems.Add("FUT", "Foglio Unico");
                oVl.ValueListItems.Add("OE", "Order Entry");
                oVl.ValueListItems.Add("PVT", "Parametri Vitali");
                oVl.ValueListItems.Add("PRF", "Prescrizioni");
                oVl.ValueListItems.Add("TAG", "Tipo Agenda");
                oVl.ValueListItems.Add("VSM", "Via Somministrazione");
                oVl.ValueListItems.Add("WKI", "Work List Infermieristica");
                oCbt.ValueList = oVl;
                oCbt.SelectedIndex = 0;

            }
            catch (Exception)
            {

            }

        }

        private void ActionGridToolClick(ToolBase oTool)
        {

            switch (oTool.Key)
            {
                case MyStatics.GC_GENERA:
                    if (MessageBox.Show("Sei sicuro di voler rigenerare le icone dell'entià selezionata?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this.GeneraTabelle(((ValueListItem)((ComboBoxTool)this.UltraToolbarsManagerGrid.Tools["Entità"]).SelectedItem).DataValue.ToString());
                        this.GeneraIcone(((ValueListItem)((ComboBoxTool)this.UltraToolbarsManagerGrid.Tools["Entità"]).SelectedItem).DataValue.ToString());
                        this.LoadUltraGrid(((ValueListItem)((ComboBoxTool)this.UltraToolbarsManagerGrid.Tools["Entità"]).SelectedItem).DataValue.ToString());
                        this.Cursor = Cursors.Default;
                    }
                    break;

                case MyStatics.GC_AGGIORNA:
                    this.LoadUltraGrid(((ValueListItem)((ComboBoxTool)this.UltraToolbarsManagerGrid.Tools["Entità"]).SelectedItem).DataValue.ToString());
                    break;

                case "Icone":
                    if (MessageBox.Show("Sei sicuro di voler salvare le icone di tutte le entità?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this.SetProgressBarTool(true);
                        this.GeneraIconeWebDaConfig();
                        this.SetProgressBarTool(false);
                        ((ComboBoxTool)this.UltraToolbarsManagerGrid.Tools["Entità"]).SelectedIndex = 0;
                        this.LoadUltraGrid(((ValueListItem)((ComboBoxTool)this.UltraToolbarsManagerGrid.Tools["Entità"]).SelectedItem).DataValue.ToString());
                        this.Cursor = Cursors.Default;
                    }
                    break;

                default:
                    break;

            }

        }

        #endregion

        #region UltraGrid

        private void LoadUltraGrid(string sCodEntita)
        {

            try
            {

                string sSql = @"Select * From T_Icone" + Environment.NewLine + (sCodEntita == "Tutti" ? @"" : @"Where CodEntita = '" + sCodEntita + "'");

                this.UltraGrid.DataSource = DataBase.GetDataSet(sSql);
                this.UltraGrid.Refresh();
                this.UltraGrid.Text = string.Format("{0} ({1:#,##0})", this.Text, this.UltraGrid.Rows.Count);

                this.UltraGrid.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        #endregion

        #region Subroutine

        private void GeneraTabelle(string sCodEntita)
        {

            try
            {

                System.Data.SqlClient.SqlParameter[] oSqlParams = new System.Data.SqlClient.SqlParameter[1];
                System.Data.SqlClient.SqlParameter oSqlParameter = new System.Data.SqlClient.SqlParameter();
                oSqlParameter.ParameterName = "@sCodEntita";
                oSqlParameter.SqlDbType = System.Data.SqlDbType.VarChar;
                oSqlParameter.Size = 20;
                oSqlParameter.Direction = ParameterDirection.Input;
                oSqlParameter.Value = (sCodEntita == "Tutti" ? @"" : sCodEntita);
                oSqlParams[0] = oSqlParameter;
                DataBase.ExecStoredProcNQ("MSP_CreaIcone", ref oSqlParams);
                oSqlParameter = null;
                oSqlParams = null;

            }
            catch (Exception)
            {

            }

        }

        private void GeneraIcone(string sCodEntita)
        {

            try
            {

                string sSql = @"Select * From T_Icone" + Environment.NewLine + (sCodEntita == "Tutti" ? @"" : @"Where CodEntita = '" + sCodEntita + "'");

                DataSet oDs = DataBase.GetDataSet(sSql);
                foreach (DataRow oDr in oDs.Tables[0].Rows)
                {

                    Image oImgTipo = null;
                    Image oImgStato = null;

                    switch (oDr["CodEntita"].ToString())
                    {
                        case @"ALA":
                            oImgTipo = Scci.Statics.DrawingProcs.GetImageFromByte(DataBase.FindValue("Icona", "T_TipoAlertAllergiaAnamnesi", "Codice = '" + oDr["CodTipo"] + "'"));
                            oImgStato = Scci.Statics.DrawingProcs.GetImageFromByte(DataBase.FindValue("Icona", "T_StatoAlertAllergiaAnamnesi", "Codice = '" + oDr["CodStato"] + "'"));
                            break;

                        case @"ALL":
                            oImgTipo = Scci.Statics.DrawingProcs.GetImageFromByte(DataBase.FindValue("Icona", "T_TipoAllegato", "Codice = '" + oDr["CodTipo"] + "'"));
                            oImgStato = Scci.Statics.DrawingProcs.GetImageFromByte(DataBase.FindValue("Icona", "T_EntitaAllegato", "Codice = '" + oDr["CodStato"] + "'"));
                            break;

                        case @"ALLFMT":
                            oImgTipo = Scci.Statics.DrawingProcs.GetImageFromByte(DataBase.FindValue("Icona", "T_FormatoAllegati", "Codice = '" + oDr["CodTipo"] + "'"));
                            oImgStato = Scci.Statics.DrawingProcs.GetImageFromByte(DataBase.FindValue("Icona", "T_FormatoAllegati", "Codice = '" + oDr["CodTipo"] + "'"));
                            break;

                        case @"APP":
                            oImgTipo = Scci.Statics.DrawingProcs.GetImageFromByte(DataBase.FindValue("Icona", "T_TipoAppuntamento", "Codice = '" + oDr["CodTipo"] + "'"));
                            oImgStato = Scci.Statics.DrawingProcs.GetImageFromByte(DataBase.FindValue("Icona", "T_StatoAppuntamento", "Codice = '" + oDr["CodStato"] + "'"));
                            break;

                        case @"CNC":
                            oImgTipo = Scci.Statics.DrawingProcs.GetImageFromByte(DataBase.FindValue("Icona", "T_StatoConsensoCalcolato", "Codice = '" + oDr["CodTipo"] + "'"));
                            oImgStato = null;
                            break;

                        case @"CSG":
                            oImgTipo = Scci.Statics.DrawingProcs.GetImageFromByte(DataBase.FindValue("Icona", "T_TipoConsegna", "Codice = '" + oDr["CodTipo"] + "'"));
                            oImgStato = Scci.Statics.DrawingProcs.GetImageFromByte(DataBase.FindValue("Icona", "T_StatoConsegna", "Codice = '" + oDr["CodStato"] + "'"));
                            break;

                        case @"CSP":
                            oImgTipo = Scci.Statics.DrawingProcs.GetImageFromByte(DataBase.FindValue("Icona", "T_TipoConsegnaPaziente", "Codice = '" + oDr["CodTipo"] + "'"));
                            oImgStato = Scci.Statics.DrawingProcs.GetImageFromByte(DataBase.FindValue("Icona", "T_StatoConsegnaPaziente", "Codice = '" + oDr["CodStato"] + "'"));
                            break;

                        case @"DCL":
                            oImgTipo = Scci.Statics.DrawingProcs.GetImageFromByte(DataBase.FindValue("Icona", "T_TipoDiario", "Codice = '" + oDr["CodTipo"] + "'"));
                            oImgStato = Scci.Statics.DrawingProcs.GetImageFromByte(DataBase.FindValue("Icona", "T_StatoDiario", "Codice = '" + oDr["CodStato"] + "'"));
                            break;

                        case @"EPI":
                            oImgTipo = Scci.Statics.DrawingProcs.GetImageFromByte(DataBase.FindValue("Icona", "T_TipoEpisodio", "Codice = '" + oDr["CodTipo"] + "'"));
                            oImgStato = Scci.Statics.DrawingProcs.GetImageFromByte(DataBase.FindValue("Icona", "T_StatoEpisodio", "Codice = '" + oDr["CodStato"] + "'"));
                            break;

                        case @"EVC":
                            oImgTipo = Scci.Statics.DrawingProcs.GetImageFromByte(DataBase.FindValue("Icona", "T_TipoEvidenzaClinica", "Codice = '" + oDr["CodTipo"] + "'"));
                            oImgStato = Scci.Statics.DrawingProcs.GetImageFromByte(DataBase.FindValue("Icona", "T_StatoEvidenzaClinica", "Codice = '" + oDr["CodStato"] + "'"));
                            break;

                        case @"PVT":
                            oImgTipo = Scci.Statics.DrawingProcs.GetImageFromByte(DataBase.FindValue("Icona", "T_TipoParametroVitale", "Codice = '" + oDr["CodTipo"] + "'"));
                            oImgStato = Scci.Statics.DrawingProcs.GetImageFromByte(DataBase.FindValue("Icona", "T_StatoParametroVitale", "Codice = '" + oDr["CodStato"] + "'"));
                            break;

                        case @"PRF":
                            oImgTipo = Scci.Statics.DrawingProcs.GetImageFromByte(DataBase.FindValue("Icona", "T_TipoPrescrizione", "Codice = '" + oDr["CodTipo"] + "'"));
                            oImgStato = Scci.Statics.DrawingProcs.GetImageFromByte(DataBase.FindValue("Icona", "T_StatoPrescrizione", "Codice = '" + oDr["CodStato"] + "'"));
                            break;

                        case @"WKI":
                            oImgTipo = Scci.Statics.DrawingProcs.GetImageFromByte(DataBase.FindValue("Icona", "T_TipoTaskInfermieristico", "Codice = '" + oDr["CodTipo"] + "'"));
                            oImgStato = Scci.Statics.DrawingProcs.GetImageFromByte(DataBase.FindValue("Icona", "T_StatoTaskInfermieristico", "Codice = '" + oDr["CodStato"] + "'"));
                            break;

                        case @"ALG":
                            oImgTipo = Scci.Statics.DrawingProcs.GetImageFromByte(DataBase.FindValue("Icona", "T_TipoAlertGenerico", "Codice = '" + oDr["CodTipo"] + "'"));
                            oImgStato = Scci.Statics.DrawingProcs.GetImageFromByte(DataBase.FindValue("Icona", "T_StatoAlertGenerico", "Codice = '" + oDr["CodStato"] + "'"));
                            break;

                        case @"OE":
                            oImgTipo = Scci.Statics.DrawingProcs.GetImageFromByte(DataBase.FindValue("Icona", "T_TipoOrdine", "Codice = '" + oDr["CodTipo"] + "'"));
                            oImgStato = Scci.Statics.DrawingProcs.GetImageFromByte(DataBase.FindValue("Icona", "T_StatoOrdine", "Codice = '" + oDr["CodStato"] + "'"));
                            break;

                        case @"VSM":
                            oImgTipo = Scci.Statics.DrawingProcs.GetImageFromByte(DataBase.FindValue("Icona", "T_ViaSomministrazione", "Codice = '" + oDr["CodTipo"] + "'"));
                            oImgStato = Scci.Statics.DrawingProcs.GetImageFromByte(DataBase.FindValue("Icona", "T_StatoContinuazione", "Codice = '" + oDr["CodStato"] + "'"));
                            break;

                        case @"TAG":
                            oImgTipo = Scci.Statics.DrawingProcs.GetImageFromByte(DataBase.FindValue("Icona", "T_TipoAgenda", "Codice = '" + oDr["CodTipo"] + "'"));
                            oImgStato = null;
                            break;

                        case @"FUT":
                            oImgTipo = Scci.Statics.DrawingProcs.GetImageFromByte(DataBase.FindValue("Icona", "T_SezioniFUT", "Codice = '" + oDr["CodTipo"] + "'"));
                            oImgStato = null;
                            break;
                    }

                    if (oImgTipo != null && oImgStato != null)
                    {
                        oDr["Icona16"] = Scci.Statics.DrawingProcs.GetByteFromImage(MyStatics.MergeTwoImages(oImgTipo, oImgStato, Enums.EnumImageSize.isz16));
                        oDr["Icona32"] = Scci.Statics.DrawingProcs.GetByteFromImage(MyStatics.MergeTwoImages(oImgTipo, oImgStato, Enums.EnumImageSize.isz32));
                        oDr["Icona48"] = Scci.Statics.DrawingProcs.GetByteFromImage(MyStatics.MergeTwoImages(oImgTipo, oImgStato, Enums.EnumImageSize.isz48));
                        oDr["Icona256"] = Scci.Statics.DrawingProcs.GetByteFromImage(MyStatics.MergeTwoImages(oImgTipo, oImgStato, Enums.EnumImageSize.isz256));
                    }
                    else if (oImgTipo != null && oImgStato == null)
                    {
                        oDr["Icona16"] = Scci.Statics.DrawingProcs.GetByteFromImage(MyStatics.ResizeBitmap(oImgTipo, 16, 16));
                        oDr["Icona32"] = Scci.Statics.DrawingProcs.GetByteFromImage(MyStatics.ResizeBitmap(oImgTipo, 32, 32));
                        oDr["Icona48"] = Scci.Statics.DrawingProcs.GetByteFromImage(MyStatics.ResizeBitmap(oImgTipo, 48, 48));
                        oDr["Icona256"] = Scci.Statics.DrawingProcs.GetByteFromImage(oImgTipo);
                    }

                }
                DataBase.SaveDataSet(oDs, sSql);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void GeneraIconeWeb(string sCodEntita)
        {

            try
            {

                using (var fbd = new FolderBrowserDialog())
                {
                    DialogResult result = fbd.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {

                        string[] files = Directory.GetFiles(fbd.SelectedPath, "*.png");
                        foreach (string f in files)
                        {
                            File.Delete(f);
                        }

                        string sSql = @"Select * From T_Icone" + Environment.NewLine + (sCodEntita == "Tutti" ? @"" : @"Where CodEntita = '" + sCodEntita + "'");
                        DataSet oDs = DataBase.GetDataSet(sSql);
                        foreach (DataRow oDr in oDs.Tables[0].Rows)
                        {

                            if (!oDr.IsNull("Icona256"))
                            {
                                byte[] bitmap = (byte[])oDr["Icona256"];
                                using (Image image = Image.FromStream(new MemoryStream(bitmap)))
                                {
                                    string sPathFile = Path.Combine(fbd.SelectedPath, string.Format("{0}.png", oDr["IDNum"].ToString()));
                                    image.Save(sPathFile, ImageFormat.Png);
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

        }

        private void GeneraIconeWebDaConfig()
        {

            string susername = "";
            string suserdomain = "";
            int nItem = 0;

            try
            {

                string sScciWebPathIcone1 = Scci.Statics.Database.GetConfigTable(EnumConfigTable.ScciWebPathIcone1);
                string sScciWebPathIcone2 = Scci.Statics.Database.GetConfigTable(EnumConfigTable.ScciWebPathIcone2);
                string sScciWebPathIconeUtente = Scci.Statics.Database.GetConfigTable(EnumConfigTable.ScciWebPathIconeUtente);
                Scci.Encryption ocrypt = new Encryption(Scci.Statics.EncryptionStatics.GC_DECRYPTKEY, Scci.Statics.EncryptionStatics.GC_INITVECTOR);
                string sScciWebPathIconePassword = ocrypt.DecryptString(Scci.Statics.Database.GetConfigTable(EnumConfigTable.ScciWebPathIconePassword));
                ocrypt = null;

                if (!string.IsNullOrWhiteSpace(sScciWebPathIconeUtente))
                {
                    susername = sScciWebPathIconeUtente.Split(@"\".ToCharArray())[1]; suserdomain = sScciWebPathIconeUtente.Split(@"\".ToCharArray())[0];
                }

                if (!string.IsNullOrWhiteSpace(sScciWebPathIcone1))
                {

                    Action DeleteAction = () =>
{

string[] files1 = Directory.GetFiles(sScciWebPathIcone1, "*.png");
nItem = 1;
this.SetProgressBarTool(nItem, files1.Length, nItem, $"Cancella icone in {sScciWebPathIcone1}");
foreach (string f in files1)
{
File.Delete(f);
nItem += 1;
this.SetProgressBarTool(nItem);
}

if (!string.IsNullOrWhiteSpace(sScciWebPathIcone2))
{

string[] files2 = Directory.GetFiles(sScciWebPathIcone2, "*.png");
nItem = 1;
this.SetProgressBarTool(nItem, files2.Length, nItem, $"Cancella icone in {sScciWebPathIcone2}");
foreach (string f in files2)
{
File.Delete(f);
nItem += 1;
this.SetProgressBarTool(nItem);
}
}

};

                    if (string.IsNullOrWhiteSpace(sScciWebPathIconeUtente) && string.IsNullOrWhiteSpace(sScciWebPathIconePassword))
                    {
                        DeleteAction();
                    }
                    else
                    {
                        Impersonation.RunImpersonated(susername, suserdomain, sScciWebPathIconePassword, DeleteAction);
                    }

                    Action SaveAction = () =>
{

string sSql = @"Select * From T_Icone";
DataSet oDs = DataBase.GetDataSet(sSql);
nItem = 1;
this.SetProgressBarTool(nItem, oDs.Tables[0].Rows.Count, nItem, $"Genera icone");
foreach (DataRow oDr in oDs.Tables[0].Rows)
{

if (!oDr.IsNull("Icona256"))
{

byte[] bitmap = (byte[])oDr["Icona256"];
using (Image image = Image.FromStream(new MemoryStream(bitmap)))
{

string sPathFile = Path.Combine(sScciWebPathIcone1, string.Format("{0}.png", oDr["IDNum"].ToString()));
image.Save(sPathFile, ImageFormat.Png);

if (!string.IsNullOrWhiteSpace(sScciWebPathIcone2))
{
sPathFile = Path.Combine(sScciWebPathIcone2, string.Format("{0}.png", oDr["IDNum"].ToString()));
image.Save(sPathFile, ImageFormat.Png);
}

}

}

nItem += 1;
this.SetProgressBarTool(nItem);

}

};

                    if (string.IsNullOrWhiteSpace(sScciWebPathIconeUtente) && string.IsNullOrWhiteSpace(sScciWebPathIconePassword))
                    {
                        SaveAction();
                    }
                    else
                    {
                        Impersonation.RunImpersonated(susername, suserdomain, sScciWebPathIconePassword, SaveAction);
                    }

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void SetProgressBarTool(int minimum, int maximum, int value, string label)
        {

            try
            {

                this.UltraStatusBar.Panels["ProgressBarAction"].ProgressBarInfo.Minimum = minimum;
                this.UltraStatusBar.Panels["ProgressBarAction"].ProgressBarInfo.Maximum = maximum;
                this.UltraStatusBar.Panels["ProgressBarAction"].ProgressBarInfo.Label = $"{label} [Formatted]";
                SetProgressBarTool(value);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }
        private void SetProgressBarTool(int value)
        {

            try
            {

                this.UltraStatusBar.Panels["ProgressBarAction"].ProgressBarInfo.Value = value;
                Application.DoEvents();

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }
        private void SetProgressBarTool(bool visible)
        {

            try
            {

                this.UltraStatusBar.Panels["ProgressBarAction"].Visible = visible;
                Application.DoEvents();

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }
        #endregion

        #region Events

        private void frmIcone_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.UltraGrid != null) this.UltraGrid.Dispose();
        }

        private void UltraToolbarsManagerGrid_ToolClick(object sender, ToolClickEventArgs e)
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

        private void UltraToolbarsManagerGrid_ToolValueChanged(object sender, ToolEventArgs e)
        {
            this.LoadUltraGrid(((ValueListItem)((ComboBoxTool)e.Tool).SelectedItem).DataValue.ToString());
        }

        private void UltraGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {

                if (e.Layout.Bands[0].Columns.Exists("Icona16") == true)
                {
                    e.Layout.Bands[0].Columns["Icona16"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                    e.Layout.Bands[0].Columns["Icona16"].CellAppearance.ImageHAlign = HAlign.Center;
                }
                if (e.Layout.Bands[0].Columns.Exists("Icona32") == true)
                {
                    e.Layout.Bands[0].Columns["Icona32"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                    e.Layout.Bands[0].Columns["Icona32"].CellAppearance.ImageHAlign = HAlign.Center;
                }
                if (e.Layout.Bands[0].Columns.Exists("Icona48") == true)
                {
                    e.Layout.Bands[0].Columns["Icona48"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                    e.Layout.Bands[0].Columns["Icona48"].CellAppearance.ImageHAlign = HAlign.Center;
                }
                if (e.Layout.Bands[0].Columns.Exists("Icona256") == true)
                {
                    e.Layout.Bands[0].Columns["Icona256"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                    e.Layout.Bands[0].Columns["Icona256"].CellAppearance.ImageHAlign = HAlign.Center;
                }

            }
            catch (Exception)
            {

            }

        }

        private void ubChiudi_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

    }
}
