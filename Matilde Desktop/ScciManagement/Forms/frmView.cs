using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using UnicodeSrl.ScciResource;
using UnicodeSrl.ScciCore;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinGrid;
using UnicodeSrl.Framework.Data;
using System.Threading.Tasks;
using UnicodeSrl.ScciManagement.Model;
using UnicodeSrl.ScciCommon2;
using UnicodeSrl.ScciCommon2.Extensions;
using System.IO;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmView : Form, Interfacce.IViewFormView
    {
        public frmView()
        {
            InitializeComponent();
        }

        #region Declare

        private Enums.EnumDataNames _ViewDataName;
        private Enums.EnumDataNames _ViewDataNamePU;

        #endregion

        #region Interface

        public Enums.EnumDataNames ViewDataName
        {
            get
            {
                return _ViewDataName;
            }
            set
            {
                _ViewDataName = value;
            }
        }

        public Enums.EnumDataNames ViewDataNamePU
        {
            get
            {
                return _ViewDataNamePU;
            }
            set
            {
                _ViewDataNamePU = value;
            }
        }

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

        public Image ViewImage
        {
            get
            {
                return this.PicImage.Image;
            }
            set
            {
                this.PicImage.Image = value;
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
            this.InitializeUltraGrid();

            MyStatics.InitializeSaveFileDialog(ref this.SaveFileDialog);

            this.LoadUltraGrid();
            this.SetUltraToolBarManager();

            this.ResumeLayout();

        }

        #endregion

        #region UltraToolBar

        private void InitializeUltraToolbarsManager()
        {

            try
            {

                MyStatics.SetUltraToolbarsManager(this.UltraToolbarsManager);

                foreach (ToolBase oTool in this.UltraToolbarsManager.Tools)
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
            bool bVisualizza = true;
            bool bStampa = true;
            bool bAggiorna = true;
            bool bEsporta = true;
            bool bCopia = false;
            bool bCopiaVisible = false;
            bool bModificaVersione = true;
            bool bModificaVersioneVisible = false;
            bool bModificaVersioneCorrente = true;
            bool bModificaVersioneCorrenteVisible = false;
            bool bImportaXML = false;
            bool bEsportaXML = false;
            bool bEsportaXMLVisible = false;
            bool bQuick = false;
            bool bImportaCSV = false;

            if ((this.UltraGrid.Rows.Count > 0 && this.UltraGrid.ActiveRow != null) && this.UltraGrid.ActiveRow.IsDataRow)
            {
                bModifica = true;
                bElimina = true;
                bVisualizza = true;
                bStampa = true;
                bEsporta = true;
                bModificaVersione = true;
            }
            else
            {
                bModifica = false;
                bElimina = false;
                bVisualizza = false;
                bStampa = false;
                bEsporta = false;
                bModificaVersione = false;
            }

            switch (this.ViewDataName)
            {
                case Enums.EnumDataNames.T_StatoEvidenzaClinica:
                case Enums.EnumDataNames.T_StatoAppuntamento:
                case Enums.EnumDataNames.T_StatoDiario:
                case Enums.EnumDataNames.T_StatoEvidenzaClinicaVisione:
                case Enums.EnumDataNames.T_StatoAlertGenerico:
                case Enums.EnumDataNames.T_StatoAlertAllergiaAnamnesi:
                case Enums.EnumDataNames.T_StatoParametroVitale:
                case Enums.EnumDataNames.T_StatoConsegna:
                case Enums.EnumDataNames.T_StatoConsegnaPaziente:
                case Enums.EnumDataNames.T_StatoConsegnaPazienteRuoli:
                case Enums.EnumDataNames.T_StatoConsensoCalcolato:
                case Enums.EnumDataNames.T_StatoScheda:
                case Enums.EnumDataNames.T_Sistemi:
                case Enums.EnumDataNames.T_FormatoAllegati:
                case Enums.EnumDataNames.T_StatoOrdine:
                case Enums.EnumDataNames.T_StatoPrescrizione:
                case Enums.EnumDataNames.T_TipoEpisodio:
                case Enums.EnumDataNames.T_StatoTrasferimento:
                case Enums.EnumDataNames.T_FormatoReport:
                case Enums.EnumDataNames.T_TipoDiario:
                case Enums.EnumDataNames.T_StatoPrescrizioneTempi:
                case Enums.EnumDataNames.T_StatoContinuazione:
                case Enums.EnumDataNames.T_StatoAppuntamentoAgende:
                case Enums.EnumDataNames.T_StatoAllegato:
                case Enums.EnumDataNames.T_StatoCartella:
                case Enums.EnumDataNames.T_StatoCartellaInfo:
                case Enums.EnumDataNames.T_StatoEpisodio:
                case Enums.EnumDataNames.T_Maschere:
                case Enums.EnumDataNames.T_StatoTaskInfermieristico:
                case Enums.EnumDataNames.T_StatoCartellaInVisione:
                case Enums.EnumDataNames.T_EntitaAllegato:
                    bNuovo = false;
                    bElimina = false;
                    break;

                case Enums.EnumDataNames.T_TipoEvidenzaClinica:
                    if ((this.UltraGrid.Rows.Count > 0 && this.UltraGrid.ActiveRow != null) && this.UltraGrid.ActiveRow.IsDataRow)
                        bElimina = (Microsoft.VisualBasic.Information.IsDBNull(this.UltraGrid.ActiveRow.Cells["Riservato"].Value) ||
                                    !(bool)this.UltraGrid.ActiveRow.Cells["Riservato"].Value);
                    break;

                case Enums.EnumDataNames.T_Contatori:
                    if ((this.UltraGrid.Rows.Count > 0 && this.UltraGrid.ActiveRow != null) && this.UltraGrid.ActiveRow.IsDataRow)
                        bElimina = (Microsoft.VisualBasic.Information.IsDBNull(this.UltraGrid.ActiveRow.Cells["Sistema"].Value) ||
                                    !(bool)this.UltraGrid.ActiveRow.Cells["Sistema"].Value);
                    break;

                case Enums.EnumDataNames.T_Report:
                case Enums.EnumDataNames.T_Login:
                    bCopiaVisible = true;
                    if ((this.UltraGrid.Rows.Count > 0 && this.UltraGrid.ActiveRow != null) && this.UltraGrid.ActiveRow.IsDataRow)
                    {
                        bElimina = (Microsoft.VisualBasic.Information.IsDBNull(this.UltraGrid.ActiveRow.Cells["FlagSistema"].Value) ||
                                    !(bool)this.UltraGrid.ActiveRow.Cells["FlagSistema"].Value);
                        bCopia = true;
                    }
                    break;

                case Enums.EnumDataNames.T_Schede:
                    bCopiaVisible = true;
                    bCopia = ((this.UltraGrid.Rows.Count > 0 && this.UltraGrid.ActiveRow != null) && this.UltraGrid.ActiveRow.IsDataRow);
                    bModificaVersioneVisible = true;
                    bModificaVersione = ((this.UltraGrid.Rows.Count > 0 && this.UltraGrid.ActiveRow != null) && this.UltraGrid.ActiveRow.IsDataRow);
                    bModificaVersioneCorrenteVisible = true;
                    bModificaVersioneCorrente = ((this.UltraGrid.Rows.Count > 0 && this.UltraGrid.ActiveRow != null) && this.UltraGrid.ActiveRow.IsDataRow);
                    bImportaXML = true;
                    bEsportaXMLVisible = true;
                    bEsportaXML = ((this.UltraGrid.Rows.Count > 0 && this.UltraGrid.ActiveRow != null) && this.UltraGrid.ActiveRow.IsDataRow);
                    break;

                case Enums.EnumDataNames.T_DCDecodifiche:
                    bCopiaVisible = true;
                    bCopia = ((this.UltraGrid.Rows.Count > 0 && this.UltraGrid.ActiveRow != null) && this.UltraGrid.ActiveRow.IsDataRow);
                    bImportaXML = true;
                    bEsportaXMLVisible = true;
                    bEsportaXML = true;
                    bImportaCSV = true;
                    bQuick = true;
                    break;

                case Enums.EnumDataNames.T_Agende:
                case Enums.EnumDataNames.T_Ruoli:
                    bCopiaVisible = true;
                    bCopia = ((this.UltraGrid.Rows.Count > 0 && this.UltraGrid.ActiveRow != null) && this.UltraGrid.ActiveRow.IsDataRow);
                    break;

                case Enums.EnumDataNames.T_CDSSAzioni:
                case Enums.EnumDataNames.T_CDSSStruttura:
                case Enums.EnumDataNames.T_CDSSStrutturaRuoli:
                case Enums.EnumDataNames.T_UnitaAtomiche:
                case Enums.EnumDataNames.T_Screen:
                    bCopiaVisible = true;
                    bCopia = ((this.UltraGrid.Rows.Count > 0 && this.UltraGrid.ActiveRow != null) && this.UltraGrid.ActiveRow.IsDataRow);
                    break;

                case Enums.EnumDataNames.T_TipoAlertAllergiaAnamnesi:
                case Enums.EnumDataNames.T_TipoAlertGenerico:
                case Enums.EnumDataNames.T_TipoAppuntamento:
                case Enums.EnumDataNames.T_TipoConsegna:
                case Enums.EnumDataNames.T_TipoConsegnaPaziente:
                case Enums.EnumDataNames.T_TipoPrescrizione:
                case Enums.EnumDataNames.T_TipoParametroVitale:
                case Enums.EnumDataNames.T_DiarioMedico:
                case Enums.EnumDataNames.T_DiarioInfermieristico:
                    bModificaVersioneCorrenteVisible = true;
                    bModificaVersioneCorrente = ((this.UltraGrid.Rows.Count > 0 && this.UltraGrid.ActiveRow != null) && this.UltraGrid.ActiveRow.IsDataRow);
                    break;


                default:
                    break;

            }

            this.UltraToolbarsManager.Tools[MyStatics.GC_NUOVO].SharedProps.Enabled = bNuovo;
            this.UltraToolbarsManager.Tools[MyStatics.GC_MODIFICA].SharedProps.Enabled = bModifica;
            this.UltraToolbarsManager.Tools[MyStatics.GC_ELIMINA].SharedProps.Enabled = bElimina;
            this.UltraToolbarsManager.Tools[MyStatics.GC_VISUALIZZA].SharedProps.Enabled = bVisualizza;
            this.UltraToolbarsManager.Tools[MyStatics.GC_STAMPA].SharedProps.Enabled = bStampa;
            this.UltraToolbarsManager.Tools[MyStatics.GC_AGGIORNA].SharedProps.Enabled = bAggiorna;
            this.UltraToolbarsManager.Tools[MyStatics.GC_ESPORTA].SharedProps.Enabled = bEsporta;
            this.UltraToolbarsManager.Tools[MyStatics.GC_COPIA].SharedProps.Visible = bCopiaVisible;
            this.UltraToolbarsManager.Tools[MyStatics.GC_COPIA].SharedProps.Enabled = bCopia;
            this.UltraToolbarsManager.Tools[MyStatics.GC_MODIFICAVERSIONE].SharedProps.Visible = bModificaVersioneVisible;
            this.UltraToolbarsManager.Tools[MyStatics.GC_MODIFICAVERSIONE].SharedProps.Enabled = bModificaVersione;
            this.UltraToolbarsManager.Tools[MyStatics.GC_MODIFICAVERSIONECORRENTE].SharedProps.Visible = bModificaVersioneCorrenteVisible;
            this.UltraToolbarsManager.Tools[MyStatics.GC_MODIFICAVERSIONECORRENTE].SharedProps.Enabled = bModificaVersioneCorrente;
            this.UltraToolbarsManager.Tools[MyStatics.GC_CONVERSIONESCHEDE].SharedProps.Visible = bModificaVersioneCorrenteVisible;
            this.UltraToolbarsManager.Tools[MyStatics.GC_CONVERSIONESCHEDE].SharedProps.Enabled = bModificaVersioneCorrente;
            this.UltraToolbarsManager.Tools[MyStatics.GC_ESPORTAXML].SharedProps.Visible = bEsportaXMLVisible;
            this.UltraToolbarsManager.Tools[MyStatics.GC_ESPORTAXML].SharedProps.Enabled = bEsportaXML;
            this.UltraToolbarsManager.Tools[MyStatics.GC_IMPORTAXML].SharedProps.Visible = bImportaXML;
            this.UltraToolbarsManager.Tools[MyStatics.GC_DIZIONARIOCSV].SharedProps.Visible = bImportaCSV;
            this.UltraToolbarsManager.Tools[MyStatics.GC_DIZIONARIOQUICK].SharedProps.Visible = bQuick;

        }

        private void ActionGridToolClick(ToolBase oTool)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                UltraGridRow activeRow = null;
                if (this.UltraGrid.ActiveRow != null) activeRow = this.UltraGrid.ActiveRow;

                switch (oTool.Key)
                {
                    case MyStatics.GC_NUOVO:
                        if (MyStatics.ActionDataNameFormPU(this.ViewDataNamePU, Enums.EnumModalityPopUp.mpNuovo, this.ViewIcon, this.ViewImage, this.ViewText, ref activeRow) == DialogResult.OK)
                        {
                            this.LoadUltraGrid();
                            this.SetUltraToolBarManager();
                        }
                        break;

                    case MyStatics.GC_MODIFICA:
                        if (MyStatics.ActionDataNameFormPU(this.ViewDataNamePU, Enums.EnumModalityPopUp.mpModifica, this.ViewIcon, this.ViewImage, this.ViewText, ref activeRow) == DialogResult.OK)
                        {
                            this.LoadUltraGrid();
                            this.SetUltraToolBarManager();
                        }
                        break;

                    case MyStatics.GC_COPIA:
                        Copia(this.ViewDataNamePU);
                        break;

                    case MyStatics.GC_ELIMINA:
                        if (MyStatics.ActionDataNameFormPU(this.ViewDataNamePU, Enums.EnumModalityPopUp.mpCancella, this.ViewIcon, this.ViewImage, this.ViewText, ref activeRow) == DialogResult.OK)
                        {
                            this.LoadUltraGrid();
                            this.SetUltraToolBarManager();
                        }
                        break;

                    case MyStatics.GC_VISUALIZZA:
                        MyStatics.ActionDataNameFormPU(this.ViewDataNamePU, Enums.EnumModalityPopUp.mpVisualizza, this.ViewIcon, this.ViewImage, this.ViewText, ref activeRow);
                        break;

                    case MyStatics.GC_STAMPA:
                        try
                        {
                            this.UltraGrid.PrintPreview(Infragistics.Win.UltraWinGrid.RowPropertyCategories.All);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(@"Si sono verificati errori durante il processo di stampa." + Environment.NewLine + ex.Message, @"Errore di stampa", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;

                    case MyStatics.GC_AGGIORNA:
                        this.UltraGrid.Rows.ColumnFilters.ClearAllFilters();
                        this.LoadUltraGrid();
                        this.SetUltraToolBarManager();

                        break;

                    case MyStatics.GC_ESPORTA:
                        if (this.SaveFileDialog.ShowDialog() == DialogResult.OK)
                            this.UltraGridExcelExporter.Export(this.UltraGrid, this.SaveFileDialog.FileName);
                        break;

                    case MyStatics.GC_MODIFICAVERSIONE:
                        string sCodice1 = activeRow.Cells["Codice"].Text;
                        int nVersione1 = this.CheckVersioneScheda(sCodice1);
                        if (nVersione1 != 0)
                        {
                            if (MyStatics.ActionDataNameFormPU(Enums.EnumDataNames.T_SchedeVersioni, Enums.EnumModalityPopUp.mpModifica, this.ViewIcon, this.ViewImage, this.ViewText, sCodice1, nVersione1.ToString()) == DialogResult.OK)
                            {
                                this.LoadUltraGrid();
                                this.SetUltraToolBarManager();
                            }
                        }
                        else
                        {
                            MessageBox.Show(@"Nessuna versione esistente per la scheda '" + sCodice1 + "'.", @"Errore di caricamento", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        break;

                    case MyStatics.GC_MODIFICAVERSIONECORRENTE:
                        string sCodice2 = string.Empty;
                        switch (this.ViewDataName)
                        {

                            case Enums.EnumDataNames.T_TipoAlertAllergiaAnamnesi:
                            case Enums.EnumDataNames.T_TipoAlertGenerico:
                            case Enums.EnumDataNames.T_TipoAppuntamento:
                            case Enums.EnumDataNames.T_TipoConsegna:
                            case Enums.EnumDataNames.T_TipoConsegnaPaziente:
                            case Enums.EnumDataNames.T_TipoPrescrizione:
                            case Enums.EnumDataNames.T_TipoParametroVitale:
                            case Enums.EnumDataNames.T_DiarioMedico:
                            case Enums.EnumDataNames.T_DiarioInfermieristico:
                            case Enums.EnumDataNames.T_TipoTaskInfermieristico:
                                sCodice2 = activeRow.Cells["CodScheda"].Text;
                                break;

                            default:
                                sCodice2 = activeRow.Cells["Codice"].Text;
                                break;

                        }
                        int nVersione2 = this.CheckVersioneCorrenteScheda(sCodice2);
                        if (nVersione2 != 0)
                        {
                            if (MyStatics.ActionDataNameFormPU(Enums.EnumDataNames.T_SchedeVersioni, Enums.EnumModalityPopUp.mpModifica, this.ViewIcon, this.ViewImage, this.ViewText, sCodice2, nVersione2.ToString()) == DialogResult.OK)
                            {
                                this.LoadUltraGrid();
                                this.SetUltraToolBarManager();
                            }
                        }
                        else
                        {
                            MessageBox.Show(@"Nessuna versione esistente per la scheda '" + sCodice2 + "'.", @"Errore di caricamento", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        break;

                    case MyStatics.GC_CONVERSIONESCHEDE:
                        this.ConversioneSchede();
                        break;

                    case MyStatics.GC_IMPORTAXML:
                        this.ImportaXML(this.ViewDataNamePU);
                        break;

                    case MyStatics.GC_ESPORTAXML:
                        this.EsportaXML(this.ViewDataNamePU);
                        break;

                    case MyStatics.GC_DIZIONARIOQUICK:
                        this.DizionarioQuick();
                        break;

                    case MyStatics.GC_DIZIONARIOCSV:
                        this.ImportaCSV(this.ViewDataNamePU);
                        break;

                    default:
                        break;

                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

        }

        #endregion

        #region Void & Functions

        private void Copia(Enums.EnumDataNames dataName)
        {
            try
            {
                UltraGridRow activeRow = null;
                if (this.UltraGrid.ActiveRow != null) activeRow = this.UltraGrid.ActiveRow;

                switch (dataName)
                {

                    case Enums.EnumDataNames.T_Schede:
                        frmPUSchedeCopia oPUSchedeC = new frmPUSchedeCopia();
                        oPUSchedeC.ViewDataNamePU = dataName;
                        oPUSchedeC.ViewModality = Enums.EnumModalityPopUp.mpCopia;
                        oPUSchedeC.ViewText = @"Copia Scheda"; oPUSchedeC.ViewIcon = this.ViewIcon;
                        oPUSchedeC.ViewImage = this.ViewImage;
                        PUDataBindings oPUDataBindingsSchede = oPUSchedeC.ViewDataBindings;
                        DataBase.SetDataBinding(ref oPUDataBindingsSchede, dataName, Enums.EnumModalityPopUp.mpCopia, ref activeRow);
                        oPUSchedeC.ViewInit();
                        oPUSchedeC.ShowDialog();

                        if (oPUSchedeC.DialogResult == System.Windows.Forms.DialogResult.OK)
                        {
                            string codScheda = oPUSchedeC.codiceNewScheda;

                            this.LoadUltraGrid();

                            try
                            {
                                for (int r = 0; r < this.UltraGrid.Rows.Count; r++)
                                {
                                    if (this.UltraGrid.Rows[r].Cells["Codice"].Value.ToString().ToUpper() == codScheda.ToUpper())
                                    {
                                        this.UltraGrid.Selected.Rows.Clear();
                                        this.UltraGrid.Rows[r].Selected = true;
                                        this.UltraGrid.Rows[r].Activate();
                                        this.UltraGrid.ActiveRow = this.UltraGrid.Rows[r];

                                        r = this.UltraGrid.Rows.Count + 1;
                                    }
                                }
                            }
                            catch
                            {
                            }

                            this.SetUltraToolBarManager();

                            ActionGridToolClick(this.UltraToolbarsManager.Tools["Modifica"]);

                        }
                        oPUSchedeC.Close();
                        oPUSchedeC.Dispose();

                        break;

                    case Enums.EnumDataNames.T_Ruoli:
                        frmPURuoliCopia oPURuoliC = new frmPURuoliCopia();
                        oPURuoliC.ViewDataNamePU = dataName;
                        oPURuoliC.ViewModality = Enums.EnumModalityPopUp.mpCopia;
                        oPURuoliC.ViewText = @"Copia Ruolo";
                        oPURuoliC.ViewIcon = this.ViewIcon;
                        oPURuoliC.ViewImage = this.ViewImage;
                        PUDataBindings oPUDataBindingsRuoli = oPURuoliC.ViewDataBindings;
                        DataBase.SetDataBinding(ref oPUDataBindingsRuoli, dataName, Enums.EnumModalityPopUp.mpCopia, ref activeRow);

                        oPURuoliC.ViewInit();
                        oPURuoliC.ShowDialog();

                        if (oPURuoliC.DialogResult == System.Windows.Forms.DialogResult.OK)
                        {
                            string codRuolo = oPURuoliC.codiceNewRuolo;

                            this.LoadUltraGrid();

                            try
                            {
                                for (int r = 0; r < this.UltraGrid.Rows.Count; r++)
                                {
                                    if (this.UltraGrid.Rows[r].Cells["Codice"].Value.ToString().ToUpper() == codRuolo.ToUpper())
                                    {
                                        this.UltraGrid.Selected.Rows.Clear();
                                        this.UltraGrid.Rows[r].Selected = true;
                                        this.UltraGrid.Rows[r].Activate();
                                        this.UltraGrid.ActiveRow = this.UltraGrid.Rows[r];
                                        break;
                                    }
                                }
                            }
                            catch
                            {
                            }

                            this.SetUltraToolBarManager();

                            ActionGridToolClick(this.UltraToolbarsManager.Tools["Modifica"]);

                        }
                        oPURuoliC.Close();
                        oPURuoliC.Dispose();

                        break;

                    case Enums.EnumDataNames.T_Agende:
                        frmPUAgendeCopia oPUAgendeC = new frmPUAgendeCopia();
                        oPUAgendeC.ViewDataNamePU = dataName;
                        oPUAgendeC.ViewModality = Enums.EnumModalityPopUp.mpCopia;
                        oPUAgendeC.ViewText = @"Copia Agenda";
                        oPUAgendeC.ViewIcon = this.ViewIcon;
                        oPUAgendeC.ViewImage = this.ViewImage;
                        PUDataBindings oPUDataBindingsAgende = oPUAgendeC.ViewDataBindings;
                        DataBase.SetDataBinding(ref oPUDataBindingsAgende, dataName, Enums.EnumModalityPopUp.mpCopia, ref activeRow);

                        oPUAgendeC.ViewInit();
                        oPUAgendeC.ShowDialog();

                        if (oPUAgendeC.DialogResult == System.Windows.Forms.DialogResult.OK)
                        {
                            string codAgenda = oPUAgendeC.codiceNewAgenda;

                            this.LoadUltraGrid();

                            try
                            {
                                for (int r = 0; r < this.UltraGrid.Rows.Count; r++)
                                {
                                    if (this.UltraGrid.Rows[r].Cells["Codice"].Value.ToString().ToUpper() == codAgenda.ToUpper())
                                    {
                                        this.UltraGrid.Selected.Rows.Clear();
                                        this.UltraGrid.Rows[r].Selected = true;
                                        this.UltraGrid.Rows[r].Activate();
                                        this.UltraGrid.ActiveRow = this.UltraGrid.Rows[r];
                                        break;
                                    }
                                }
                            }
                            catch
                            {
                            }

                            this.SetUltraToolBarManager();

                            ActionGridToolClick(this.UltraToolbarsManager.Tools["Modifica"]);

                        }
                        oPUAgendeC.Close();
                        oPUAgendeC.Dispose();

                        break;

                    case Enums.EnumDataNames.T_Login:
                        frmPUView oPUView = new frmPUView();
                        oPUView.ViewDataNamePU = dataName;
                        oPUView.ViewModality = Enums.EnumModalityPopUp.mpCopia;
                        oPUView.ViewText = "Copia Utente";
                        oPUView.ViewIcon = this.ViewIcon;
                        oPUView.ViewImage = this.ViewImage;
                        PUDataBindings oPUDataBindings = oPUView.ViewDataBindings;
                        UltraGridRow rowgrid = this.UltraGrid.ActiveRow;
                        DataBase.SetDataBinding(ref oPUDataBindings, dataName, Enums.EnumModalityPopUp.mpCopia, ref rowgrid);

                        oPUView.ViewInit();

                        if (oPUView.ShowDialog() == System.Windows.Forms.DialogResult.OK) ActionGridToolClick(this.UltraToolbarsManager.Tools["Aggiorna"]);

                        break;

                    case Enums.EnumDataNames.T_Report:
                        frmPUView oPUViewRep = new frmPUView();
                        oPUViewRep.ViewDataNamePU = dataName;
                        oPUViewRep.ViewModality = Enums.EnumModalityPopUp.mpCopia;
                        oPUViewRep.ViewText = "Copia Report";
                        oPUViewRep.ViewIcon = this.ViewIcon;
                        oPUViewRep.ViewImage = this.ViewImage;
                        PUDataBindings oPUDataBindingsRep = oPUViewRep.ViewDataBindings;
                        UltraGridRow rowgridrep = this.UltraGrid.ActiveRow;
                        DataBase.SetDataBinding(ref oPUDataBindingsRep, dataName, Enums.EnumModalityPopUp.mpCopia, ref rowgridrep);

                        oPUViewRep.ViewInit();

                        if (oPUViewRep.ShowDialog() == System.Windows.Forms.DialogResult.OK) ActionGridToolClick(this.UltraToolbarsManager.Tools["Aggiorna"]);

                        break;


                    case Enums.EnumDataNames.T_DCDecodifiche:
                        if (this.UltraGrid.ActiveRow != null)
                        {
                            string sNuovoCodice = DataBase.CopiaDCDecodifiche(this.UltraGrid.ActiveRow.Cells["Codice"].Text);

                            if (sNuovoCodice != "")
                            {

                                frmPUView2 oPUViewDiz = new frmPUView2();
                                oPUViewDiz.ViewDataNamePU = dataName;
                                oPUViewDiz.ViewModality = Enums.EnumModalityPopUp.mpModifica;
                                oPUViewDiz.ViewText = "Modifica Dizionario";
                                oPUViewDiz.ViewIcon = this.ViewIcon;
                                oPUViewDiz.ViewImage = this.ViewImage;
                                PUDataBindings oPUDataBindingsDiz = oPUViewDiz.ViewDataBindings;
                                UltraGridRow rowgridDiz = this.UltraGrid.ActiveRow;
                                DataBase.SetDataBinding(ref oPUDataBindingsDiz, dataName, Enums.EnumModalityPopUp.mpModifica, sNuovoCodice);

                                oPUViewDiz.ViewInit();
                                oPUViewDiz.ShowDialog();

                                ActionGridToolClick(this.UltraToolbarsManager.Tools["Aggiorna"]);

                                try
                                {
                                    for (int r = 0; r < this.UltraGrid.Rows.Count; r++)
                                    {
                                        if (this.UltraGrid.Rows[r].Cells["Codice"].Text == sNuovoCodice)
                                        {
                                            this.UltraGrid.ActiveRow = this.UltraGrid.Rows[r];
                                            break;
                                        }
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                        break;

                    case Enums.EnumDataNames.T_CDSSAzioni:
                        frmPUView oPUViewAzioni = new frmPUView();
                        oPUViewAzioni.ViewDataNamePU = dataName;
                        oPUViewAzioni.ViewModality = Enums.EnumModalityPopUp.mpCopia;
                        oPUViewAzioni.ViewText = "Copia Azione";
                        oPUViewAzioni.ViewIcon = this.ViewIcon;
                        oPUViewAzioni.ViewImage = this.ViewImage;
                        PUDataBindings oPUDataBindingAzione = oPUViewAzioni.ViewDataBindings;
                        UltraGridRow rowgridazioni = this.UltraGrid.ActiveRow;
                        DataBase.SetDataBinding(ref oPUDataBindingAzione, dataName, Enums.EnumModalityPopUp.mpCopia, ref rowgridazioni);

                        oPUViewAzioni.ViewInit();

                        if (oPUViewAzioni.ShowDialog() == System.Windows.Forms.DialogResult.OK) ActionGridToolClick(this.UltraToolbarsManager.Tools["Aggiorna"]);

                        break;

                    case Enums.EnumDataNames.T_CDSSStruttura:
                        frmPUCDSSStrutturaCopia ofrmPUCDSSStrutturaCopia = new frmPUCDSSStrutturaCopia();
                        ofrmPUCDSSStrutturaCopia.ViewText = @"Copia Struttura CDSS";
                        ofrmPUCDSSStrutturaCopia.ViewIcon = this.ViewIcon;
                        ofrmPUCDSSStrutturaCopia.ViewInit();
                        if (ofrmPUCDSSStrutturaCopia.ShowDialog() == System.Windows.Forms.DialogResult.OK) ActionGridToolClick(this.UltraToolbarsManager.Tools["Aggiorna"]);

                        break;

                    case Enums.EnumDataNames.T_CDSSStrutturaRuoli:
                        frmPUView2 oPUViewStru = new frmPUView2();
                        oPUViewStru.ViewDataNamePU = dataName;
                        oPUViewStru.ViewModality = Enums.EnumModalityPopUp.mpCopia;
                        oPUViewStru.ViewText = "Copia Struttura";
                        oPUViewStru.ViewIcon = this.ViewIcon;
                        oPUViewStru.ViewImage = this.ViewImage;
                        PUDataBindings oPUDataBindingsStru = oPUViewStru.ViewDataBindings;
                        UltraGridRow rowgridstru = this.UltraGrid.ActiveRow;
                        DataBase.SetDataBinding(ref oPUDataBindingsStru, dataName, Enums.EnumModalityPopUp.mpCopia, ref rowgridstru);

                        oPUViewStru.ViewInit();

                        if (oPUViewStru.ShowDialog() == System.Windows.Forms.DialogResult.OK) ActionGridToolClick(this.UltraToolbarsManager.Tools["Aggiorna"]);

                        break;

                    case Enums.EnumDataNames.T_UnitaAtomiche:
                        frmPUView oPUViewUnitaAtomiche = new frmPUView();
                        oPUViewUnitaAtomiche.ViewDataNamePU = dataName;
                        oPUViewUnitaAtomiche.ViewModality = Enums.EnumModalityPopUp.mpCopia;
                        oPUViewUnitaAtomiche.ViewText = "Copia Azione";
                        oPUViewUnitaAtomiche.ViewIcon = this.ViewIcon;
                        oPUViewUnitaAtomiche.ViewImage = this.ViewImage;
                        PUDataBindings oPUDataBindingUnitaAtomiche = oPUViewUnitaAtomiche.ViewDataBindings;
                        UltraGridRow rowgridunitaatomiche = this.UltraGrid.ActiveRow;
                        DataBase.SetDataBinding(ref oPUDataBindingUnitaAtomiche, dataName, Enums.EnumModalityPopUp.mpCopia, ref rowgridunitaatomiche);

                        oPUViewUnitaAtomiche.ViewInit();

                        if (oPUViewUnitaAtomiche.ShowDialog() == System.Windows.Forms.DialogResult.OK) ActionGridToolClick(this.UltraToolbarsManager.Tools["Aggiorna"]);

                        break;

                    case Enums.EnumDataNames.T_TipoTaskInfermieristico:
                        frmPUView2 oPUViewTTI = new frmPUView2();
                        oPUViewTTI.ViewDataNamePU = dataName;
                        oPUViewTTI.ViewModality = Enums.EnumModalityPopUp.mpCopia;
                        oPUViewTTI.ViewText = "Copia Tipo Task Infermieristico";
                        oPUViewTTI.ViewIcon = this.ViewIcon;
                        oPUViewTTI.ViewImage = this.ViewImage;
                        PUDataBindings oPUDataBindingsTTI = oPUViewTTI.ViewDataBindings;
                        UltraGridRow rowgridtti = this.UltraGrid.ActiveRow;
                        DataBase.SetDataBinding(ref oPUDataBindingsTTI, dataName, Enums.EnumModalityPopUp.mpCopia, ref rowgridtti);

                        oPUViewTTI.ViewInit();

                        if (oPUViewTTI.ShowDialog() == System.Windows.Forms.DialogResult.OK) ActionGridToolClick(this.UltraToolbarsManager.Tools["Aggiorna"]);
                        break;

                    case Enums.EnumDataNames.T_Screen:
                        frmPUScreenCopia oPUScreenCopia = new frmPUScreenCopia();
                        oPUScreenCopia.ViewDataNamePU = dataName;
                        oPUScreenCopia.ViewModality = Enums.EnumModalityPopUp.mpCopia;
                        oPUScreenCopia.ViewText = @"Screen";
                        oPUScreenCopia.ViewIcon = this.ViewIcon;
                        oPUScreenCopia.ViewImage = this.ViewImage;
                        PUDataBindings oPUDataBindingsScreen = oPUScreenCopia.ViewDataBindings;
                        DataBase.SetDataBinding(ref oPUDataBindingsScreen, dataName, Enums.EnumModalityPopUp.mpCopia, ref activeRow);

                        oPUScreenCopia.ViewInit();
                        oPUScreenCopia.ShowDialog();

                        if (oPUScreenCopia.DialogResult == System.Windows.Forms.DialogResult.OK)
                        {

                            string sCodiceScreenNew = oPUScreenCopia.CodiceScreenNew;

                            ActionGridToolClick(this.UltraToolbarsManager.Tools["Aggiorna"]);

                            DataSet dsgrid = (DataSet)this.UltraGrid.DataSource;
                            List<DataRow> query = (from drscreen in dsgrid.Tables[0].AsEnumerable()
                                                   where drscreen.Field<string>("Codice") == sCodiceScreenNew
                                                   select drscreen).ToList();
                            if (query != null && query.Count == 1)
                            {

                                UltraGridRow oUltraGridRow = this.UltraGrid.Rows.GetRowWithListIndex(dsgrid.Tables[0].Rows.IndexOf(query[0]));
                                this.UltraGrid.Selected.Rows.Clear();
                                oUltraGridRow.Selected = true;
                                oUltraGridRow.Activate();
                                this.UltraGrid.ActiveRow = oUltraGridRow;
                                this.SetUltraToolBarManager();

                                ActionGridToolClick(this.UltraToolbarsManager.Tools["Modifica"]);

                            }

                        }
                        oPUScreenCopia.Close();
                        oPUScreenCopia.Dispose();
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "Copia", this.Name);
            }
        }

        private void EsportaXML(Enums.EnumDataNames dataName)
        {
            try
            {

                switch (dataName)
                {
                    case Enums.EnumDataNames.T_Schede:
                        frmPUSchedaEsporta oPUSchedaExport = new frmPUSchedaEsporta();
                        oPUSchedaExport.ViewText = @"Esporta Scheda";
                        oPUSchedaExport.ViewIcon = Risorse.GetIconFromResource(MyStatics.GetNameResource(MyStatics.GC_ESPORTAXML, Enums.EnumImageSize.isz16));
                        oPUSchedaExport.ViewImage = Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_ESPORTAXML, Enums.EnumImageSize.isz256));
                        oPUSchedaExport.codiceScheda = this.UltraGrid.ActiveRow.Cells["Codice"].Value.ToString();

                        oPUSchedaExport.ViewInit();
                        oPUSchedaExport.ShowDialog();

                        oPUSchedaExport.Close();
                        oPUSchedaExport.Dispose();

                        break;

                    case Enums.EnumDataNames.T_DCDecodifiche:
                        frmPUDizionariEsporta oPUDizExport = new frmPUDizionariEsporta();
                        oPUDizExport.ViewText = @"Esporta Dizionari";
                        oPUDizExport.ViewIcon = Risorse.GetIconFromResource(MyStatics.GetNameResource(MyStatics.GC_ESPORTAXML, Enums.EnumImageSize.isz16));
                        oPUDizExport.ViewImage = Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_ESPORTAXML, Enums.EnumImageSize.isz256));

                        oPUDizExport.ViewInit();
                        oPUDizExport.ShowDialog();

                        oPUDizExport.Close();
                        oPUDizExport.Dispose();

                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "EsportaXML", this.Name);
            }
        }

        private void ImportaXML(Enums.EnumDataNames dataName)
        {
            try
            {

                switch (dataName)
                {
                    case Enums.EnumDataNames.T_Schede:
                        frmPUSchedaImporta oPUSchedaImport = new frmPUSchedaImporta();
                        oPUSchedaImport.ViewText = @"Importa Scheda";
                        oPUSchedaImport.ViewIcon = Risorse.GetIconFromResource(MyStatics.GetNameResource(MyStatics.GC_IMPORTAXML, Enums.EnumImageSize.isz16));
                        oPUSchedaImport.ViewImage = Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_IMPORTAXML, Enums.EnumImageSize.isz256));
                        oPUSchedaImport.ViewInit();
                        oPUSchedaImport.ShowDialog();

                        if (oPUSchedaImport.DialogResult == System.Windows.Forms.DialogResult.OK)
                        {
                            this.LoadUltraGrid();

                            UltraGridRow selrow = this.UltraGrid.Rows.ToList<UltraGridRow>().Find
                            (
                                r => r.Cells["Codice"].Value.ToString().ToUpper() == oPUSchedaImport.codiceNewScheda.ToUpper()
                            );

                            this.UltraGrid.Selected.Rows.Clear();

                            if (selrow != null)
                            {
                                selrow.Selected = true;
                                selrow.Activate();
                                this.UltraGrid.ActiveRow = selrow;
                            }

                            this.SetUltraToolBarManager();
                        }

                        oPUSchedaImport.Close();
                        oPUSchedaImport.Dispose();
                        break;

                    case Enums.EnumDataNames.T_DCDecodifiche:
                        frmPUDizionariImporta oPUDizImport = new frmPUDizionariImporta();
                        oPUDizImport.ViewText = @"Importa Dizionari";
                        oPUDizImport.ViewIcon = Risorse.GetIconFromResource(MyStatics.GetNameResource(MyStatics.GC_IMPORTAXML, Enums.EnumImageSize.isz16));
                        oPUDizImport.ViewImage = Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_IMPORTAXML, Enums.EnumImageSize.isz256));
                        oPUDizImport.ViewInit();
                        oPUDizImport.ShowDialog();

                        oPUDizImport.Close();
                        oPUDizImport.Dispose();

                        this.LoadUltraGrid();
                        this.SetUltraToolBarManager();

                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "ImportaXML", this.Name);
            }
        }

        private void ImportaCSV(Enums.EnumDataNames dataName)
        {
            try
            {

                switch (dataName)
                {

                    case Enums.EnumDataNames.T_DCDecodifiche:
                        frmPUDizionariImporta oPUDizImport = new frmPUDizionariImporta();
                        oPUDizImport.ModalityCSV = true;
                        oPUDizImport.ViewText = @"Importa Dizionari";
                        oPUDizImport.ViewIcon = Risorse.GetIconFromResource(MyStatics.GetNameResource(MyStatics.GC_DIZIONARIOCSV, Enums.EnumImageSize.isz16));
                        oPUDizImport.ViewImage = Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_DIZIONARIOCSV, Enums.EnumImageSize.isz256));
                        oPUDizImport.ViewInit();
                        oPUDizImport.ShowDialog();

                        oPUDizImport.Close();
                        oPUDizImport.Dispose();

                        this.LoadUltraGrid();
                        this.SetUltraToolBarManager();

                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "ImportaCSV", this.Name);
            }
        }

        private int CheckVersioneCorrenteScheda(string codiceScheda)
        {
            return int.Parse(DataBase.FindValue("IsNull(Versione,0)", "T_SchedeVersioni", "CodScheda = '" + codiceScheda + "' And FlagAttiva = 1 Order By DtValI desc, DtValF", "0"));
        }

        private int CheckVersioneScheda(string codiceScheda)
        {
            return int.Parse(DataBase.FindValue("IsNull(Max(Versione),0)", "T_SchedeVersioni", "CodScheda = '" + codiceScheda + "'", "0"));
        }

        private void DizionarioQuick()
        {
            try
            {
                string sNuovoCodice = DataBase.DCDecodificheQuick();

                if (sNuovoCodice != "")
                {

                    frmPUView2 oPUViewDiz = new frmPUView2();
                    oPUViewDiz.ViewDataNamePU = Enums.EnumDataNames.T_DCDecodifiche;
                    oPUViewDiz.ViewModality = Enums.EnumModalityPopUp.mpModifica;
                    oPUViewDiz.ViewText = "Dizionario Quick";
                    oPUViewDiz.ViewIcon = this.ViewIcon;
                    oPUViewDiz.ViewImage = this.ViewImage;
                    PUDataBindings oPUDataBindingsDiz = oPUViewDiz.ViewDataBindings;
                    UltraGridRow rowgridDiz = this.UltraGrid.ActiveRow;
                    DataBase.SetDataBinding(ref oPUDataBindingsDiz, Enums.EnumDataNames.T_DCDecodifiche, Enums.EnumModalityPopUp.mpModifica, sNuovoCodice);

                    oPUViewDiz.ViewInit();
                    oPUViewDiz.ShowDialog();

                    ActionGridToolClick(this.UltraToolbarsManager.Tools["Aggiorna"]);

                    try
                    {
                        for (int r = 0; r < this.UltraGrid.Rows.Count; r++)
                        {
                            if (this.UltraGrid.Rows[r].Cells["Codice"].Text == sNuovoCodice)
                            {
                                this.UltraGrid.ActiveRow = this.UltraGrid.Rows[r];
                                break;
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "DizionarioQuick", this.Name);
            }
        }

        private async void ConversioneSchede()
        {

            if (this.UltraGrid.Selected.Rows.Count > 0)
            {

                if (MessageBox.Show($"Sei sicuro di voler convertire {this.UltraGrid.Selected.Rows.Count} sched{(this.UltraGrid.Selected.Rows.Count == 1 ? "a" : "e")}?", "Conversione Schede", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {

                    List<string> lst_check = new List<string>();
                    foreach (UltraGridRow ugr in this.UltraGrid.Selected.Rows)
                    {

                        using (FwDataConnection conn = new FwDataConnection(MyStatics.Configurazione.ConnectionString))
                        {

                            FwDataBufferedList<T_SchedeVersioniRow> oSchedeVersioniBuffer = conn.T_SchedeVersioniAttive(ugr.Cells["Codice"].Text);
                            foreach (T_SchedeVersioniRow oSchedeVersioniRow in oSchedeVersioniBuffer.Buffer)
                            {

                                string[] arvCheckScheda = DataBase.CheckInputScheda(oSchedeVersioniRow.Struttura, oSchedeVersioniRow.Layout);
                                if (arvCheckScheda.Length == 0)
                                {

                                    try
                                    {
                                        EavDataStruct EavDataStructV3 = await EavExtensions.CreateEavDataStruct(oSchedeVersioniRow.CodScheda, null,
    MyStatics.Configurazione.ConnectionString, versione: oSchedeVersioniRow.Versione);

                                        oSchedeVersioniRow.StrutturaV3 = EavDataStructV3.EavSchema.ToXmlString();
                                        oSchedeVersioniRow.LayoutV3 = EavDataStructV3.EavLayout.ToXmlString();
                                        oSchedeVersioniRow.Update();
                                    }
                                    catch (Exception ex)
                                    {
                                        string s_des = $"== Scheda: {oSchedeVersioniRow.CodScheda}-{oSchedeVersioniRow.Descrizione} ==";
                                        lst_check.Add(string.Concat(Enumerable.Repeat("=", s_des.Length)));
                                        lst_check.Add(s_des);
                                        lst_check.Add(string.Concat(Enumerable.Repeat("=", s_des.Length)));
                                        lst_check.Add($"ERR99: {ex.Message}");
                                        lst_check.Add("");
                                    }

                                }
                                else
                                {
                                    lst_check.AddRange(arvCheckScheda);
                                    lst_check.Add("");
                                }

                            }

                        }

                    }

                    if (lst_check.Count > 0)
                    {

                        MessageBox.Show($"Sono state trovate schede con errori.", "Schede con errori", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        lst_check.Insert(0, $"Data conversione: {DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()}");
                        lst_check.Insert(1, $"");
                        string fileName = System.IO.Path.Combine(Scci.Statics.FileStatics.GetSCCITempPath() + @"SCHERR_" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + @".txt");
                        File.WriteAllLines(fileName, lst_check.ToArray());
                        System.Diagnostics.Process.Start("notepad.exe", fileName);

                    }

                    MessageBox.Show("Fine Conversione!", "Conversione Schede", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                }

            }
            else
            {
                MessageBox.Show("Nessuna scheda selezionata!", "Conversione Schede", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        #endregion

        #region UltraGrid

        private void InitializeUltraGrid()
        {
            MyStatics.SetUltraGridLayout(ref this.UltraGrid, true, false);
        }

        private void LoadUltraGrid()
        {

            int nIndex = -1;

            try
            {
                if (this.UltraGrid.ActiveRow != null) nIndex = this.UltraGrid.ActiveRow.Index;

                this.UltraGrid.DataSource = DataBase.GetDataSet(DataBase.GetSqlView(this.ViewDataName));
                this.UltraGrid.Refresh();
                this.UltraGrid.Text = string.Format("{0} ({1:#,##0})", this.Text, this.UltraGrid.Rows.Count);


                foreach (UltraGridColumn oCol in this.UltraGrid.DisplayLayout.Bands[0].Columns)
                {
                    try
                    {
                        switch (oCol.DataType.Name.Trim().ToUpper())
                        {
                            case "DATETIME":
                            case "DATE":
                                if (this.ViewDataName == Enums.EnumDataNames.T_MovNews)
                                    oCol.Format = @"dd/MM/yyyy HH:mm";
                                else
                                    oCol.Format = @"dd/MM/yyyy";
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                break;

                            case "INT":
                            case "INT16":
                            case "INT32":
                            case "INT64":
                            case "LONG":
                            case "INTEGER":
                                if (oCol.Key.Trim().ToUpper().IndexOf("COD") < 0 && oCol.Key.Trim().ToUpper() != "ID") oCol.Format = @"#,##0";
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                                oCol.Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;
                                break;

                            case "DECIMAL":
                            case "DOUBLE":
                            case "SINGLE":
                                if (oCol.Key.Trim().ToUpper().IndexOf("COD") < 0 && oCol.Key.Trim().ToUpper() != "ID") oCol.Format = @"#,##0.00";
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                                oCol.Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;
                                break;

                            case "BOOL":
                            case "BOOLEAN":
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                break;

                            default:
                                break;
                        }
                        if (oCol.Key.Trim().ToUpper().IndexOf("FLAG") == 0)
                            oCol.Header.Caption = oCol.Key.Substring(4);

                        if (oCol.Key.Trim().ToUpper().IndexOf("FLG") == 0)
                            oCol.Header.Caption = oCol.Key.Substring(3);
                    }
                    catch (Exception)
                    {
                    }
                }


                switch (this.ViewDataName)
                {
                    case Enums.EnumDataNames.T_Schede:
                        this.UltraGrid.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.VisibleRows);
                        if (this.UltraGrid.DisplayLayout.Bands[0].Columns.Exists("Descrizione"))
                        {
                            this.UltraGrid.DisplayLayout.Bands[0].Columns["Descrizione"].Width = 240;
                        }
                        if (this.UltraGrid.DisplayLayout.Bands[0].Columns.Exists("Note"))
                        {
                            this.UltraGrid.DisplayLayout.Bands[0].Columns["Note"].Width = 240;
                        }
                        break;

                    default:
                        this.UltraGrid.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);
                        break;
                }


                if (nIndex != -1)
                {
                    try
                    {
                        this.UltraGrid.ActiveRow = this.UltraGrid.Rows[nIndex];
                        this.UltraGrid.ActiveRow.Selected = true;
                    }
                    catch (Exception)
                    {
                        if (this.UltraGrid.Rows.Count > 0)
                        {
                            this.UltraGrid.ActiveRow = this.UltraGrid.Rows[this.UltraGrid.Rows.Count - 1];
                            this.UltraGrid.ActiveRow.Selected = true;
                        }
                    }
                }
                else
                {
                    if (this.UltraGrid.Rows.Count > 0)
                    {
                        this.UltraGrid.ActiveRow = this.UltraGrid.Rows[0];
                        this.UltraGrid.ActiveRow.Selected = true;
                    }
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        #endregion

        #region Events

        private void frmView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.UltraGrid != null) this.UltraGrid.Dispose();
        }

        private void UltraGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {

                switch (this.ViewDataName)
                {

                    case Enums.EnumDataNames.T_UnitaAtomiche:
                        if (e.Layout.Bands[0].Columns.Exists("IntestazioneCartella") == true)
                        {
                            e.Layout.Bands[0].Columns["IntestazioneCartella"].Hidden = true;
                        }
                        if (e.Layout.Bands[0].Columns.Exists("FirmaCartella") == true)
                        {
                            e.Layout.Bands[0].Columns["FirmaCartella"].Hidden = true;
                        }
                        if (e.Layout.Bands[0].Columns.Exists("IntestazioneSintetica") == true)
                        {
                            e.Layout.Bands[0].Columns["IntestazioneSintetica"].Hidden = true;
                        }
                        if (e.Layout.Bands[0].Columns.Exists("SpallaSinistra") == true)
                        {
                            e.Layout.Bands[0].Columns["SpallaSinistra"].Hidden = true;
                        }
                        break;

                    case Enums.EnumDataNames.T_Login:
                        if (e.Layout.Bands[0].Columns.Exists("Foto") == true)
                        {
                            e.Layout.Bands[0].Columns["Foto"].Hidden = true;
                        }
                        break;

                    case Enums.EnumDataNames.T_TipoDiario:
                    case Enums.EnumDataNames.T_TipoScheda:
                    case Enums.EnumDataNames.T_StatoPrescrizione:
                    case Enums.EnumDataNames.T_TipoPrescrizione:
                    case Enums.EnumDataNames.T_StatoTrasferimento:
                    case Enums.EnumDataNames.T_TipoEpisodio:
                    case Enums.EnumDataNames.T_TipoParametroVitale:
                    case Enums.EnumDataNames.T_TipoConsegna:
                    case Enums.EnumDataNames.T_TipoConsegnaPaziente:
                    case Enums.EnumDataNames.T_StatoAppuntamento:
                    case Enums.EnumDataNames.T_TipoEvidenzaClinica:
                    case Enums.EnumDataNames.T_StatoEvidenzaClinica:
                    case Enums.EnumDataNames.T_TipoTaskInfermieristico:
                    case Enums.EnumDataNames.T_StatoTaskInfermieristico:
                    case Enums.EnumDataNames.T_StatoDiario:
                    case Enums.EnumDataNames.T_StatoEvidenzaClinicaVisione:
                    case Enums.EnumDataNames.T_StatoAlertGenerico:
                    case Enums.EnumDataNames.T_TipoAlertAllergiaAnamnesi:
                    case Enums.EnumDataNames.T_TipoAlertGenerico:
                    case Enums.EnumDataNames.T_StatoAlertAllergiaAnamnesi:
                    case Enums.EnumDataNames.T_StatoParametroVitale:
                    case Enums.EnumDataNames.T_StatoConsegna:
                    case Enums.EnumDataNames.T_StatoConsegnaPaziente:
                    case Enums.EnumDataNames.T_StatoConsegnaPazienteRuoli:
                    case Enums.EnumDataNames.T_StatoConsensoCalcolato:
                    case Enums.EnumDataNames.T_StatoScheda:
                    case Enums.EnumDataNames.T_StatoSchedaCalcolato:
                    case Enums.EnumDataNames.T_Sistemi:
                    case Enums.EnumDataNames.T_TipoAllegato:
                    case Enums.EnumDataNames.T_FormatoAllegati:
                    case Enums.EnumDataNames.T_TipoOrdine:
                    case Enums.EnumDataNames.T_StatoOrdine:
                    case Enums.EnumDataNames.T_StatoPrescrizioneTempi:
                    case Enums.EnumDataNames.T_StatoContinuazione:
                    case Enums.EnumDataNames.T_StatoAppuntamentoAgende:
                    case Enums.EnumDataNames.T_StatoAllegato:
                    case Enums.EnumDataNames.T_StatoCartella:
                    case Enums.EnumDataNames.T_StatoCartellaInfo:
                    case Enums.EnumDataNames.T_StatoEpisodio:
                    case Enums.EnumDataNames.T_SezioniFUT:
                    case Enums.EnumDataNames.T_StatoCartellaInVisione:
                    case Enums.EnumDataNames.T_CDSSPlugins:
                    case Enums.EnumDataNames.T_ProtocolliPrescrizioni:
                    case Enums.EnumDataNames.T_EntitaAllegato:
                        if (e.Layout.Bands[0].Columns.Exists("Icona") == true)
                        {
                            e.Layout.Bands[0].Columns["Icona"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                            e.Layout.Bands[0].Columns["Icona"].CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                        }

                        if (e.Layout.Bands[0].Columns.Exists("CodColore") == true)
                        {
                            e.Layout.Bands[0].Columns["CodColore"].Hidden = true;
                        }
                        if (e.Layout.Bands[0].Columns.Exists("Colore") == true)
                        {
                            e.Layout.Bands[0].Columns["Colore"].CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                        }

                        if (e.Layout.Bands[0].Columns.Exists("ColoreGrafico") == true)
                        {
                            e.Layout.Bands[0].Columns["ColoreGrafico"].Hidden = true;
                        }
                        if (e.Layout.Bands[0].Columns.Exists("ColoreGraf") == true)
                        {
                            e.Layout.Bands[0].Columns["ColoreGraf"].Header.Caption = @"Colore Graf.";
                            e.Layout.Bands[0].Columns["ColoreGraf"].CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                        }

                        if (e.Layout.Bands[0].Columns.Exists("CodViaSomministrazione") == true)
                        {
                            e.Layout.Bands[0].Columns["CodViaSomministrazione"].Hidden = true;
                        }
                        if (e.Layout.Bands[0].Columns.Exists("Time Slot") == true)
                        {
                            e.Layout.Bands[0].Columns["Time Slot"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
                            e.Layout.Bands[0].Columns["Time Slot"].ValueList = CoreStatics.GetTimeSlotIntervalVl();
                        }
                        if (e.Layout.Bands[0].Columns.Exists("Anticipo") == true)
                        {
                            e.Layout.Bands[0].Columns["Anticipo"].Header.Caption = @"Anticipo [minuti]";
                        }
                        break;

                    case Enums.EnumDataNames.T_TipoAppuntamento:
                    case Enums.EnumDataNames.T_ProtocolliAttivita:
                        if (e.Layout.Bands[0].Columns.Exists("Icona") == true)
                        {
                            e.Layout.Bands[0].Columns["Icona"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                            e.Layout.Bands[0].Columns["Icona"].CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                        }
                        if (e.Layout.Bands[0].Columns.Exists("Colore") == true)
                        {
                            e.Layout.Bands[0].Columns["Colore"].CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                        }
                        break;

                    case Enums.EnumDataNames.T_Report:
                        if (e.Layout.Bands[0].Columns.Exists("Icona") == true)
                        {
                            e.Layout.Bands[0].Columns["Icona"].Hidden = true;
                        }
                        if (e.Layout.Bands[0].Columns.Exists("CodFormatoReport") == true)
                        {
                            e.Layout.Bands[0].Columns["CodFormatoReport"].Hidden = true;
                        }
                        if (e.Layout.Bands[0].Columns.Exists("CodReportVista") == true)
                        {
                            e.Layout.Bands[0].Columns["CodReportVista"].Hidden = true;
                        }
                        if (e.Layout.Bands[0].Columns.Exists("Variabili") == true)
                        {
                            e.Layout.Bands[0].Columns["Variabili"].Hidden = true;
                        }
                        if (e.Layout.Bands[0].Columns.Exists("ApriBrowser") == true)
                        {
                            e.Layout.Bands[0].Columns["ApriBrowser"].Header.Caption = "Apri in browser";
                        }
                        if (e.Layout.Bands[0].Columns.Exists("ApriIE") == true)
                        {
                            e.Layout.Bands[0].Columns["ApriIE"].Header.Caption = "Internet Explorer";
                        }

                        if (e.Layout.Bands[0].Columns.Exists("Formato") == true)
                        {
                            e.Layout.Bands[0].Columns["Formato"].Header.VisiblePosition = 7;
                        }
                        if (e.Layout.Bands[0].Columns.Exists("DaStoricizzare") == true)
                        {
                            e.Layout.Bands[0].Columns["DaStoricizzare"].Header.VisiblePosition = 7;
                        }

                        break;

                    case Enums.EnumDataNames.T_TestiPredefiniti:
                        if (e.Layout.Bands[0].Columns.Exists("TestoRTF") == true)
                        {
                            e.Layout.Bands[0].Columns["TestoRTF"].Hidden = true;
                        }
                        break;

                    case Enums.EnumDataNames.T_ViaSomministrazione:
                        if (e.Layout.Bands[0].Columns.Exists("Icona") == true)
                        {
                            e.Layout.Bands[0].Columns["Icona"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                            e.Layout.Bands[0].Columns["Icona"].CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                        }
                        break;

                    case Enums.EnumDataNames.T_FormatoReport:
                        if (e.Layout.Bands[0].Columns.Exists("Icona") == true)
                        {
                            e.Layout.Bands[0].Columns["Icona"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                            e.Layout.Bands[0].Columns["Icona"].CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                        }
                        break;

                    case Enums.EnumDataNames.T_MovNews:
                        if (e.Layout.Bands[0].Columns.Exists("TestoRTF") == true)
                        {
                            e.Layout.Bands[0].Columns["TestoRTF"].Hidden = true;
                        }
                        if (e.Layout.Bands[0].Columns.Exists("DataOra") == true)
                        {
                            e.Layout.Bands[0].Columns["DataOra"].Hidden = false;
                        }
                        if (e.Layout.Bands[0].Columns.Exists("DataInizioPubblicazione") == true)
                        {
                            e.Layout.Bands[0].Columns["DataInizioPubblicazione"].Hidden = false;
                        }
                        if (e.Layout.Bands[0].Columns.Exists("DataFinePubblicazione") == true)
                        {
                            e.Layout.Bands[0].Columns["DataFinePubblicazione"].Hidden = false;
                        }
                        if (e.Layout.Bands[0].Columns.Exists("CodTipoNews") == true)
                        {
                            e.Layout.Bands[0].Columns["CodTipoNews"].Header.Caption = "Tipo News";
                            e.Layout.Bands[0].Columns["CodTipoNews"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
                            e.Layout.Bands[0].Columns["CodTipoNews"].ValueList = CoreStatics.GetTipoNewsVl();
                        }

                        if (e.Layout.Bands[0].Columns.Exists("TestRTFbinario") == true)
                        {
                            e.Layout.Bands[0].Columns["TestRTFbinario"].Hidden = true;
                        }
                        break;

                    case Enums.EnumDataNames.T_Settori:
                        if (e.Layout.Bands[0].Columns.Exists("CodAzi") == true)
                        {
                            e.Layout.Bands[0].Columns["CodAzi"].Hidden = true;
                        }
                        break;

                    case Enums.EnumDataNames.T_Letti:
                        if (e.Layout.Bands[0].Columns.Exists("ID") == true)
                        {
                            e.Layout.Bands[0].Columns["ID"].Hidden = true;
                        }
                        if (e.Layout.Bands[0].Columns.Exists("CodAzi") == true)
                        {
                            e.Layout.Bands[0].Columns["CodAzi"].Hidden = true;
                        }
                        if (e.Layout.Bands[0].Columns.Exists("CodSettore") == true)
                        {
                            e.Layout.Bands[0].Columns["CodSettore"].Hidden = true;
                        }
                        if (e.Layout.Bands[0].Columns.Exists("CodStanza") == true)
                        {
                            e.Layout.Bands[0].Columns["CodStanza"].Hidden = true;
                        }
                        break;

                    case Enums.EnumDataNames.T_Schede:
                        if (e.Layout.Bands[0].Columns.Exists("CodEntita") == true)
                        {
                            e.Layout.Bands[0].Columns["CodEntita"].Hidden = true;
                        }
                        if (e.Layout.Bands[0].Columns.Exists("CodTipoScheda") == true)
                        {
                            e.Layout.Bands[0].Columns["CodTipoScheda"].Hidden = true;
                        }

                        if (e.Layout.Bands[0].Columns.Exists("SchedaSemplice") == true)
                        {
                            e.Layout.Bands[0].Columns["SchedaSemplice"].Header.Caption = "Scheda Accessoria";
                        }

                        if (e.Layout.Bands[0].Columns.Exists("Note") == true)
                        {
                            e.Layout.Bands[0].Columns["Note"].Header.VisiblePosition = 2;
                        }

                        break;

                    case Enums.EnumDataNames.T_TipoAgenda:
                        if (e.Layout.Bands[0].Columns.Exists("Icona") == true)
                        {
                            e.Layout.Bands[0].Columns["Icona"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                            e.Layout.Bands[0].Columns["Icona"].CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                        }
                        break;

                    case Enums.EnumDataNames.T_Festivita:
                        break;

                    case Enums.EnumDataNames.T_Agende:
                        if (e.Layout.Bands[0].Columns.Exists("CodColore") == true)
                        {
                            e.Layout.Bands[0].Columns["CodColore"].Hidden = true;
                        }
                        if (e.Layout.Bands[0].Columns.Exists("Colore") == true)
                        {
                            e.Layout.Bands[0].Columns["Colore"].CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                        }
                        if (e.Layout.Bands[0].Columns.Exists("ElencoCampi") == true)
                        {
                            e.Layout.Bands[0].Columns["ElencoCampi"].Hidden = true;
                        }
                        if (e.Layout.Bands[0].Columns.Exists("IntervalloSlot") == true)
                        {
                            e.Layout.Bands[0].Columns["IntervalloSlot"].Hidden = true;
                        }
                        if (e.Layout.Bands[0].Columns.Exists("OrariLavoro") == true)
                        {
                            e.Layout.Bands[0].Columns["OrariLavoro"].Hidden = true;
                        }
                        if (e.Layout.Bands[0].Columns.Exists("CodTipoAgenda") == true)
                        {
                            e.Layout.Bands[0].Columns["CodTipoAgenda"].Hidden = true;
                        }
                        if (e.Layout.Bands[0].Columns.Exists("CodEntita") == true)
                        {
                            e.Layout.Bands[0].Columns["CodEntita"].Hidden = true;
                        }
                        if (e.Layout.Bands[0].Columns.Exists("UsaColoreTipoAppuntamento") == true)
                        {
                            e.Layout.Bands[0].Columns["UsaColoreTipoAppuntamento"].Hidden = false;
                            e.Layout.Bands[0].Columns["UsaColoreTipoAppuntamento"].Header.Caption = @"Usa Colore Tipo App.";
                        }
                        if (e.Layout.Bands[0].Columns.Exists("EscludiFestivita") == true)
                        {
                            e.Layout.Bands[0].Columns["EscludiFestivita"].Hidden = false;
                            e.Layout.Bands[0].Columns["EscludiFestivita"].Header.Caption = @"Escludi Fest.";
                        }
                        break;

                    case Enums.EnumDataNames.T_AssUAUOLetti:
                        if (e.Layout.Bands[0].Columns.Exists("CodUA") == true)
                        {
                            e.Layout.Bands[0].Columns["CodUA"].Header.Caption = @"Codice";
                        }
                        if (e.Layout.Bands[0].Columns.Exists("DescUA") == true)
                        {
                            e.Layout.Bands[0].Columns["DescUA"].Header.Caption = @"Unità Atomica";
                        }
                        if (e.Layout.Bands[0].Columns.Exists("CodAzi") == true)
                        {
                        }
                        if (e.Layout.Bands[0].Columns.Exists("DescAzi") == true)
                        {
                            e.Layout.Bands[0].Columns["DescAzi"].Header.Caption = @"Azienda";
                        }
                        if (e.Layout.Bands[0].Columns.Exists("CodUO") == true)
                        {
                        }
                        if (e.Layout.Bands[0].Columns.Exists("DescUO") == true)
                        {
                            e.Layout.Bands[0].Columns["DescUO"].Header.Caption = @"Unità Operativa";
                        }
                        if (e.Layout.Bands[0].Columns.Exists("CodSettore") == true)
                        {
                        }
                        if (e.Layout.Bands[0].Columns.Exists("DescSettore") == true)
                        {
                            e.Layout.Bands[0].Columns["DescSettore"].Header.Caption = @"Settore";
                        }
                        if (e.Layout.Bands[0].Columns.Exists("Codletto") == true)
                        {
                        }
                        if (e.Layout.Bands[0].Columns.Exists("DescLetto") == true)
                        {
                            e.Layout.Bands[0].Columns["DescLetto"].Header.Caption = @"Letto";
                        }
                        break;

                    default:
                        break;

                }

            }
            catch (Exception)
            {

            }

        }

        private void UltraGrid_InitializePrintPreview(object sender, CancelablePrintPreviewEventArgs e)
        {

            try
            {

                e.PrintDocument.PrinterSettings.PrintRange = System.Drawing.Printing.PrintRange.AllPages;
                e.DefaultLogicalPageLayoutInfo.PageHeader = this.Text;
                e.DefaultLogicalPageLayoutInfo.PageHeaderHeight = 20;
                e.DefaultLogicalPageLayoutInfo.PageHeaderAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                e.DefaultLogicalPageLayoutInfo.PageHeaderAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                e.DefaultLogicalPageLayoutInfo.PageHeaderAppearance.FontData.SizeInPoints = 12;

            }
            catch (Exception)
            {

            }

        }

        private void UltraGrid_AfterRowActivate(object sender, EventArgs e)
        {
            this.SetUltraToolBarManager();
        }

        private void UltraGrid_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {

            if (e.Row.IsDataRow == true)
            {
                if (this.UltraToolbarsManager.Tools[MyStatics.GC_MODIFICA].SharedProps.Enabled == true)
                {
                    ActionGridToolClick(this.UltraToolbarsManager.Tools[MyStatics.GC_MODIFICA]);
                }
                else
                {
                    ActionGridToolClick(this.UltraToolbarsManager.Tools[MyStatics.GC_VISUALIZZA]);
                }
            }

        }

        private void UltraGrid_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            switch (this.ViewDataName)
            {
                case Enums.EnumDataNames.T_StatoTrasferimento:
                case Enums.EnumDataNames.T_StatoPrescrizione:
                case Enums.EnumDataNames.T_TipoParametroVitale:
                case Enums.EnumDataNames.T_TipoConsegna:
                case Enums.EnumDataNames.T_TipoConsegnaPaziente:
                case Enums.EnumDataNames.T_StatoAppuntamento:
                case Enums.EnumDataNames.T_StatoEvidenzaClinica:
                case Enums.EnumDataNames.T_TipoAppuntamento:
                case Enums.EnumDataNames.T_TipoTaskInfermieristico:
                case Enums.EnumDataNames.T_StatoTaskInfermieristico:
                case Enums.EnumDataNames.T_Agende:
                case Enums.EnumDataNames.T_StatoDiario:
                case Enums.EnumDataNames.T_StatoEvidenzaClinicaVisione:
                case Enums.EnumDataNames.T_StatoAlertGenerico:
                case Enums.EnumDataNames.T_StatoAlertAllergiaAnamnesi:
                case Enums.EnumDataNames.T_StatoParametroVitale:
                case Enums.EnumDataNames.T_StatoConsegna:
                case Enums.EnumDataNames.T_StatoConsegnaPaziente:
                case Enums.EnumDataNames.T_StatoConsegnaPazienteRuoli:
                case Enums.EnumDataNames.T_StatoConsensoCalcolato:
                case Enums.EnumDataNames.T_StatoScheda:
                case Enums.EnumDataNames.T_StatoSchedaCalcolato:
                case Enums.EnumDataNames.T_Sistemi:
                case Enums.EnumDataNames.T_TipoAllegato:
                case Enums.EnumDataNames.T_FormatoAllegati:
                case Enums.EnumDataNames.T_StatoOrdine:
                case Enums.EnumDataNames.T_StatoPrescrizioneTempi:
                case Enums.EnumDataNames.T_StatoContinuazione:
                case Enums.EnumDataNames.T_StatoAppuntamentoAgende:
                case Enums.EnumDataNames.T_StatoAllegato:
                case Enums.EnumDataNames.T_StatoCartella:
                case Enums.EnumDataNames.T_StatoCartellaInfo:
                case Enums.EnumDataNames.T_StatoEpisodio:
                case Enums.EnumDataNames.T_SezioniFUT:
                case Enums.EnumDataNames.T_StatoCartellaInVisione:
                case Enums.EnumDataNames.T_ProtocolliAttivita:
                case Enums.EnumDataNames.T_ProtocolliPrescrizioni:
                case Enums.EnumDataNames.T_EntitaAllegato:
                    if (e.Row.Cells.Exists("Colore"))
                    {
                        e.Row.Cells["Colore"].Appearance.Image = MyStatics.CreateSolidBitmap(MyStatics.GetColorFromString(e.Row.Cells["CodColore"].Value.ToString()), 32, 32);
                    }
                    break;

                case Enums.EnumDataNames.T_TipoPrescrizione:
                    if (e.Row.Cells.Exists("ColoreGraf"))
                    {
                        e.Row.Cells["ColoreGraf"].Appearance.Image = MyStatics.CreateSolidBitmap(MyStatics.GetColorFromString(e.Row.Cells["ColoreGrafico"].Value.ToString()), 32, 32);
                    }
                    break;

                default:
                    break;
            }
        }

        private void UltraToolBarManager_ToolClick(object sender, ToolClickEventArgs e)
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

        #endregion

    }
}
