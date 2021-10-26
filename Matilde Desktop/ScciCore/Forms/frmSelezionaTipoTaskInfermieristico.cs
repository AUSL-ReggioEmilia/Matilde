using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.ScciResource;
using Infragistics.Win.UltraWinGrid;

using Microsoft.Data.SqlClient;
using System.Globalization;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;
using UnicodeSrl.ScciCore.Forms;
using Infragistics.Win.UltraWinEditors;

namespace UnicodeSrl.ScciCore
{
    public partial class frmSelezionaTipoTaskInfermieristico : frmBaseModale, Interfacce.IViewFormlModal
    {
        public frmSelezionaTipoTaskInfermieristico()
        {
            InitializeComponent();
        }

        #region Declare

        private bool _nascondiprotocolli = false;

        private const string C_COL_SEL = "SEL";
        private UnicodeSrl.ScciCore.ucEasyCheckEditor CheckEditorForRendering = new ucEasyCheckEditor();
        private UnicodeSrl.ScciCore.ucEasyCheckEditor CheckEditorForEditing = new ucEasyCheckEditor();
        private Infragistics.Win.UltraWinEditors.UltraControlContainerEditor UltraGridCustomEditorCheck = new UltraControlContainerEditor();

        #endregion

                                private List<String> XpFiltro_CodTipo { get; set; }

                                private Boolean UseFiltersCodTipo
        {
            get
            {
                bool useParamFilters = ((this.XpFiltro_CodTipo != null) && (this.XpFiltro_CodTipo.Count > 0));
                return useParamFilters;
            }
        }

        #region Interface

        public void Carica()
        {

            try
            {
                                if ((this.CustomParamaters != null) && (this.CustomParamaters is List<String>))
                {
                                        this.XpFiltro_CodTipo = this.CustomParamaters as List<String>;
                }
                else
                    this.XpFiltro_CodTipo = null;

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_WORKLIST_256);
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_WORKLIST_16);

                this.InizializzaControlli();

                this.InitializeUltraGrid();
                this.LoadUltraGrid();

                this.ShowDialog();

            }
            catch (Exception)
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }


        }

        #endregion

        #region UltraGrid

        private void InitializeUltraGrid()
        {
            this.UltraGridTipiTask.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Large;
            this.UltraGridProtocolliAttivita.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Large;
            
            
                        this.Controls.Add(CheckEditorForRendering);
            this.Controls.Add(CheckEditorForEditing);

                        this.UltraGridCustomEditorCheck.RenderingControl = this.CheckEditorForRendering;
            this.UltraGridCustomEditorCheck.EditingControl = this.CheckEditorForEditing;

                        this.UltraGridCustomEditorCheck.ApplyOwnerAppearanceToEditingControl = false;
            this.UltraGridCustomEditorCheck.ApplyOwnerAppearanceToRenderingControl = false;

                        this.UltraGridCustomEditorCheck.EditingControlPropertyName = "Checked";
            this.UltraGridCustomEditorCheck.RenderingControlPropertyName = "Checked";

                                    this.UltraGridCustomEditorCheck.EnterEditModeMouseBehavior = EnterEditModeMouseBehavior.EnterEditModeAndClick;

                                    
        }

        private void LoadUltraGrid()
        {

            try
            {

                                this.UltraGridTipiTask.DataSource = null;
                this.UltraGridTipiTask.DataSource = CaricaTipiTaskInfermieristici();
                this.UltraGridTipiTask.Refresh();

                this.UltraGridTipiTask.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);
                this.UltraGridTipiTask.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;


                this.UltraGridTipiTask.UpdateMode = UpdateMode.OnUpdate;

                if (this.uceSelezioneMultipla.Checked)
                {
                    this.UltraGridTipiTask.DisplayLayout.Bands[0].Columns[C_COL_SEL].EditorComponent = this.UltraGridCustomEditorCheck;
                }


                if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodTipoTaskInfermieristico != string.Empty &&
                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodTipoTaskInfermieristico != "")
                {
                    var item = this.UltraGridTipiTask.Rows.Single(UltraGridRow => UltraGridRow.Cells["Codice"].Text ==
                        CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodTipoTaskInfermieristico);

                    if (item != null) { this.UltraGridTipiTask.ActiveRow = item; }
                }

                                if (this.UseFiltersCodTipo)
                {
                                        this.UltraGridProtocolliAttivita.DataSource = null;
                    this.UltraGridProtocolliAttivita.Refresh();
                }
                else
                {
                                        this.UltraGridProtocolliAttivita.DataSource = CaricaProtocolliAttivita();
                    this.UltraGridProtocolliAttivita.Refresh();
                }

                                this.UltraGridProtocolliAttivita.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);
                this.UltraGridProtocolliAttivita.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;

                                if (UltraGridProtocolliAttivita.Rows.Count() == 0 || this._nascondiprotocolli)
                {
                    this.ucEasyTabControl.Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Wizard;
                    this.ucEasyTabControl.Tabs["tab2"].Visible = false;
                }
                else
                {
                    this.ucEasyTabControl.Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Default;
                    this.ucEasyTabControl.Tabs["tab2"].Visible = true;
                }


            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "LoadUltraGrid", this.Name);
            }

        }

        #endregion

        #region Functions

        private void InizializzaControlli()
        {
            try
            {
                if (CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.MascheraPartenza != null &&
                    CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.MascheraPartenza.ID == EnumMaschere.EditingTaskInfermieristici)
                    _nascondiprotocolli = true;
                else
                    _nascondiprotocolli = false;
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "InizializzaControlli", this.Text);
            }
        }

        private DataTable CaricaTipiTaskInfermieristici()
        {
            Parametri op = null;
            DataTable oDt = null;
            DataTable result = null;

            try
            {
                op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodUA", CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodUA);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                op.Parametro.Add("CodAzione", CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Azione.ToString());
                op.Parametro.Add("ErogazioneDiretta", (this.uceErogazioneDiretta.Checked == false ? "0" : "1"));
                op.Parametro.Add("DatiEstesi", "1");

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                oDt = Database.GetDataTableStoredProc("MSP_SelTipoTaskInfermieristico", spcoll);

                result = oDt.Clone();
                result.Clear();

                
                if (this.UseFiltersCodTipo)
                {
                    for (int i = 0; i < oDt.Rows.Count; i++)
                    {
                        DataRow row = oDt.Rows[i];

                        bool found = this.XpFiltro_CodTipo.Exists(x => x == row["Codice"].ToString());

                        if (found)
                            result.Rows.Add(row.ItemArray);

                    }
                }
                else
                    result = oDt;


            }
            catch
            {
                oDt = null;
            }
            finally
            {
                op = null;
            }

            if (oDt != null)
                return DataTableConIcona(result, this.UltraGridTipiTask.DisplayLayout.Override.CellAppearance.FontData.SizeInPoints);
            else
                return null;
        }

        private DataTable CaricaProtocolliAttivita()
        {
            Parametri op = null;
            DataTable oDt = null;

            try
            {
                op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodUA", CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodUA);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                op.Parametro.Add("CodAzione", CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Azione.ToString());
                op.Parametro.Add("DatiEstesi", "1");

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                oDt = Database.GetDataTableStoredProc("MSP_SelProtocolliAttivita", spcoll);

            }
            catch
            {
                oDt = null;
            }
            finally
            {
                op = null;
            }

            if (oDt != null)
                return DataTableConIcona(oDt, this.UltraGridProtocolliAttivita.DisplayLayout.Override.CellAppearance.FontData.SizeInPoints);
            else
                return null;
        }

        private DataTable DataTableConIcona(DataTable DtInput, float DimensioneFont)
        {

            DataTable odtGrid = null;

            try
            {
                Graphics g = this.CreateGraphics();
                int icondimensions = CoreStatics.PointToPixel(DimensioneFont, g.DpiX);
                g.Dispose();
                g = null;

                odtGrid = new DataTable();
                odtGrid.Columns.Add(C_COL_SEL, typeof(bool));
                odtGrid.Columns.Add("Codice", typeof(String));
                odtGrid.Columns.Add("IconaSmall", typeof(Byte[]));
                odtGrid.Columns.Add("Descrizione", typeof(String));
                if (DtInput.Columns.Contains("CodScheda"))
                    odtGrid.Columns.Add("CodScheda", typeof(String));
                if (DtInput.Columns.Contains("Colore"))
                    odtGrid.Columns.Add("Colore", typeof(String));

                foreach (DataRow odr in DtInput.Rows)
                {
                    DataRow odrgrid = odtGrid.NewRow();
                    odrgrid[C_COL_SEL] = false;
                    odrgrid["Codice"] = odr["Codice"];
                    odrgrid["IconaSmall"] = DrawingProcs.GetByteFromImage(DrawingProcs.GetImageFromByte(odr["Icona"], new Size(icondimensions, icondimensions), true)); ;
                    odrgrid["Descrizione"] = odr["Descrizione"];
                    if (DtInput.Columns.Contains("CodScheda"))
                        odrgrid["CodScheda"] = odr["CodScheda"];
                    if (DtInput.Columns.Contains("Colore"))
                        odrgrid["Colore"] = odr["Colore"];

                    odtGrid.Rows.Add(odrgrid);
                }
            }
            catch
            {
                odtGrid = null;
            }

            return odtGrid;
        }

        #endregion

        #region Events

        private void uceSelezioneMultipla_CheckedChanged(object sender, EventArgs e)
        {
            this.LoadUltraGrid();
                    }

        private void uceErogazioneDiretta_CheckedChanged(object sender, EventArgs e)
        {
            this.LoadUltraGrid();
        }

        private void UltraGridTipiTask_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            int refWidth = (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XLarge) * 4;

            try
            {

                e.Layout.Bands[0].ColHeadersVisible = false;

                foreach (UltraGridColumn ocol in e.Layout.Bands[0].Columns)
                {
                    switch (ocol.Key)
                    {

                        case C_COL_SEL:
                            ocol.Header.Caption = string.Empty;
                            ocol.Hidden = !this.uceSelezioneMultipla.Checked;
                            ocol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
                            ocol.CellClickAction = CellClickAction.Edit;
                            ocol.CellActivation = Activation.AllowEdit;
                            ocol.MaxWidth = Convert.ToInt32(refWidth / 2);
                            ocol.MinWidth = ocol.MaxWidth;
                            ocol.Width = ocol.MaxWidth;
                            break;

                        case "Descrizione":
                            ocol.Hidden = false;
                            ocol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                            ocol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                            break;

                        case "IconaSmall":
                            ocol.Hidden = false;
                            ocol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                            ocol.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                            ocol.CellAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                            break;

                        default:
                            ocol.Hidden = true;
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "UltraGrid_InitializeLayout", this.Name);
            }

        }

        private void frmSelezionaTipoTaskInfermieristico_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            try
            {
                if (this.ucEasyTabControl.Tabs["tab1"].Selected && this.UltraGridTipiTask.ActiveRow != null)
                {
                    CoreStatics.CoreApplication.MovProtocolloAttivitaSelezionato = null;
                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodTipoTaskInfermieristico = this.UltraGridTipiTask.ActiveRow.Cells["Codice"].Text;
                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DescrTipoTaskInfermieristico = this.UltraGridTipiTask.ActiveRow.Cells["Descrizione"].Text;
                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodScheda = this.UltraGridTipiTask.ActiveRow.Cells["CodScheda"].Text;
                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.List_CodTipoTaskInfermieristico = null;
                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.ErogazioneDiretta = this.uceErogazioneDiretta.Checked;
                }

                                                                if (this.ucEasyTabControl.Tabs["tab1"].Selected && this.uceSelezioneMultipla.Checked == true)
                {
                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.List_CodTipoTaskInfermieristico = new List<string>();
                    foreach (UltraGridRow gridrow in this.UltraGridTipiTask.Rows.ToList<UltraGridRow>().FindAll(row => (bool)row.Cells[C_COL_SEL].Value == true))
                    {
                        CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.List_CodTipoTaskInfermieristico.Add(gridrow.Cells["Codice"].Value.ToString());
                    }
                }

                if (this.ucEasyTabControl.Tabs["tab2"].Selected && this.UltraGridProtocolliAttivita.ActiveRow != null)
                {
                                        string idSistema = Guid.NewGuid().ToString();
                    string idGruppo = idSistema;

                    CoreStatics.CoreApplication.MovProtocolloAttivitaSelezionato = new MovProtocolloAttivita(
                                                                                        this.UltraGridProtocolliAttivita.ActiveRow.Cells["Codice"].Text,
                                                                                        CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.IDPaziente,
                                                                                        CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodUA,
                                                                                        CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.IDEpisodio,
                                                                                        CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.IDTrasferimento,
                                                                                        EnumCodSistema.WKI,
                                                                                        idSistema,
                                                                                        idGruppo,
                                                                                        EnumTipoRegistrazione.M,
                                                                                        CoreStatics.CoreApplication.Ambiente);

                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = null;
                }

                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "frmSelezionaTipoTaskInfermieristico_PulsanteAvantiClick", this.Name);
            }
        }

        private void frmSelezionaTipoTaskInfermieristico_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        #endregion
        
    }
}
