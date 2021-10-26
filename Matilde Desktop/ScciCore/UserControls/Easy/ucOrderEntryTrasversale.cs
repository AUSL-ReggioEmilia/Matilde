using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.ScciResource;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinListView;
using UnicodeSrl.Framework.Data;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using UnicodeSrl.ScciCore.CustomControls.Infra;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;
using UnicodeSrl.ScciCore.WebSvc;
using UnicodeSrl.Scci.DataContracts;
using System.Globalization;
using UnicodeSrl.ScciCore.SvcOrderEntry;
using UnicodeSrl.Scci.PluginClient;

namespace UnicodeSrl.ScciCore
{

    public partial class ucOrderEntryTrasversale : UserControl, Interfacce.IViewUserControlMiddle
    {

        #region Declare

        private UserControl _ucc = null;

        private const string C_COL_SEL = "SEL";

        private UnicodeSrl.ScciCore.ucEasyCheckEditor CheckEditorForRendering = new ucEasyCheckEditor();
        private UnicodeSrl.ScciCore.ucEasyCheckEditor CheckEditorForEditing = new ucEasyCheckEditor();

        private Infragistics.Win.UltraWinEditors.UltraControlContainerEditor UltraGridCustomEditorCheck = new UltraControlContainerEditor();

        #endregion

        public ucOrderEntryTrasversale()
        {
            InitializeComponent();
        }

        #region Interface

        public void Aggiorna()
        {
            if (this.IsDisposed == false)
            {
                this.CaricaGriglia();
            }
        }

        public void Carica()
        {
            try
            {

                this.InizializzaControlli();
                this.InizializzaUltraGridLayout();
                this.InizializzaFiltri();
                this.InizializzaCustomControUltraGrid();

                this.CaricaGriglia();
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
                CoreStatics.SetContesto(EnumEntita.OE, null);

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

                this.ubInoltra.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FRECCIADX_256);
                this.ubInoltra.PercImageFill = 0.75F;
                this.ubInoltra.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.ubCancella.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_ELIMINA_256);
                this.ubCancella.PercImageFill = 0.75F;
                this.ubCancella.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.uchkFiltro.UNCheckedImage = Risorse.GetImageFromResource(Risorse.GC_FILTRO_256);
                this.uchkFiltro.CheckedImage = Risorse.GetImageFromResource(Risorse.GC_FILTROAPPLICATO_256);
                this.uchkFiltro.Checked = false;
                this.uchkFiltro.PercImageFill = 0.75F;
                this.uchkFiltro.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.XSmall;
                this.uchkFiltro.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.ubSelezionaTutti.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_SELEZIONATUTTI_256);
                this.ubSelezionaTutti.PercImageFill = 0.75F;
                this.ubSelezionaTutti.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.ubDeSelezionaTutti.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_DESELEZIONATUTTI_256);
                this.ubDeSelezionaTutti.PercImageFill = 0.75F;
                this.ubDeSelezionaTutti.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.SetProgressBar(false);

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
                this.ucEasyGrid.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.True;
                this.ucEasyGrid.UpdateMode = UpdateMode.OnCellChangeOrLostFocus;

                CoreStatics.SetEasyUltraGridLayout(ref this.ucEasyGridFiltroStato);
                this.ucEasyGridFiltroStato.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.True;
                this.ucEasyGridFiltroStato.Text = "Stato";
                this.ucEasyGridFiltroStato.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.True;
                this.ucEasyGridFiltroStato.UpdateMode = UpdateMode.OnCellChangeOrLostFocus;

                CoreStatics.SetEasyUltraGridLayout(ref this.ucEasyGridFiltroTipo);
                this.ucEasyGridFiltroTipo.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.True;
                this.ucEasyGridFiltroTipo.Text = "Erogante";
                this.ucEasyGridFiltroTipo.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.True;
                this.ucEasyGridFiltroTipo.UpdateMode = UpdateMode.OnCellChangeOrLostFocus;

                CoreStatics.SetEasyUltraGridLayout(ref this.ucEasyGridFiltroUA);
                this.ucEasyGridFiltroUA.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.True;
                this.ucEasyGridFiltroUA.Text = "Struttura";

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaUltraGridLayout", this.Name);
            }
        }


        #region Filtri


        private async void loadFilter_SpecialiAsync()
        {
            try
            {
                using (FwDataConnection fdc = new FwDataConnection(Database.ConnectionString))
                {
                    XmlParameter xmlParameter = new XmlParameter();
                    xmlParameter.AddParameter("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                    xmlParameter.AddParameter("CodTipoFiltroSpeciale", "OETRA");

                    DataTable dataTable = await fdc.QueryAsync<DataTable>(@"MSP_SelFiltriSpeciali", xmlParameter.ToFwDataParametersList(), CommandType.StoredProcedure);

                    this.Invoke(new MethodInvoker(() =>
{
    this.cboFiltriSpec.ValueMember = "Codice";
    this.cboFiltriSpec.DisplayMember = "Descrizione";
    this.cboFiltriSpec.DataSource = CoreStatics.AggiungiTuttiDataTable(dataTable, false);
    this.cboFiltriSpec.Refresh();
    this.cboFiltriSpec.SelectedIndex = 0;

}));

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "loadFilter_SpecialiAsync", this.Name);
            }

        }

        private void InizializzaFiltri()
        {
            if (this.IsDisposed) return;

            try
            {
                this.ubApplicaFiltro.TextFontRelativeDimension = easyStatics.easyRelativeDimensions.Large;
                this.drFiltro.DateFuture = true;
                this.drFiltro.Domani = true;
                this.drFiltro.Value = ucEasyDateRange.C_RNG_DOMANI;

                loadFilter_SpecialiAsync();

                initFilter_Stato();

                initFilter_Tipo();

                initFilter_UA();


            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaFiltri: Stato", this.Name);
            }
        }

        private void initFilter_Stato()
        {
            if ((this.ucEasyGridFiltroStato.Rows == null) || (this.ucEasyGridFiltroStato.Rows.Count == 0)) return;

            this.ucEasyGridFiltroStato.ActiveRow = null;
            this.ucEasyGridFiltroStato.Selected.Rows.Clear();
            try
            {
                foreach (UltraGridRow row in this.ucEasyGridFiltroStato.Rows)
                {
                    if (row.Cells["CodStato"].Text == "IS" || row.Cells["CodStato"].Text == "ET")
                        row.Cells[C_COL_SEL].Value = true;
                    else
                        row.Cells[C_COL_SEL].Value = false;
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void initFilter_Tipo()

        {
            if ((this.ucEasyGridFiltroTipo.Rows == null) || (this.ucEasyGridFiltroTipo.Rows.Count == 0)) return;

            this.ucEasyGridFiltroTipo.ActiveRow = null;
            this.ucEasyGridFiltroTipo.Selected.Rows.Clear();

            try
            {
                foreach (UltraGridRow row in this.ucEasyGridFiltroTipo.Rows)
                {
                    if (row.Cells["CodTipoOrdine"].Text.Contains(CoreStatics.GC_TUTTI))
                        row.Cells[C_COL_SEL].Value = true;
                    else
                        row.Cells[C_COL_SEL].Value = false;
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaFiltri: Tipo", this.Name);
            }
        }

        private void initFilter_UA()
        {
            if ((this.ucEasyGridFiltroUA.Rows == null) || (this.ucEasyGridFiltroUA.Rows.Count == 0)) return;

            this.ucEasyGridFiltroUA.ActiveRow = null;
            this.ucEasyGridFiltroUA.Selected.Rows.Clear();
            CoreStatics.SelezionaRigaInGriglia(ref ucEasyGridFiltroUA, "CodUA", CoreStatics.GC_TUTTI);
        }

        #endregion Filtri

        private void CaricaGriglia()
        {
            string filtrogenerico = string.Empty;

            string codstatoselezionato = string.Empty;
            string codUAselezionata = string.Empty;
            string codTipoSelezionato = string.Empty;

            try
            {
                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);

                op.Parametro.Add("DatiEstesi", "1");

                op.TimeStamp.CodEntita = EnumEntita.OE.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_SelMovOrdiniTrasversali", spcoll);



                this.ucEasyGridFiltroStato.DisplayLayout.Bands[0].Columns.ClearUnbound();

                if (this.ucEasyGridFiltroStato.ActiveRow != null && this.ucEasyGridFiltroStato.ActiveRow.Cells["CodStato"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                    codstatoselezionato = this.ucEasyGridFiltroStato.ActiveRow.Cells["CodStato"].Text;
                else
                    codstatoselezionato = EnumStatoOrdine.IS.ToString();

                DataTable dtFiltroStato = new DataTable();
                DataTable dttemp = CoreStatics.AggiungiTuttiDataTable(ds.Tables[1].Copy(), true);

                dtFiltroStato.Columns.Add(C_COL_SEL, typeof(bool));

                foreach (DataColumn dtc in dttemp.Columns)
                {
                    dtFiltroStato.Columns.Add(dtc.ColumnName, dtc.DataType);
                }

                foreach (DataRow dtr in dttemp.Rows)
                {
                    DataRow dradd = dtFiltroStato.NewRow();

                    if (dtr["CodStato"].ToString() == "IS" || dtr["CodStato"].ToString() == "ET")
                        dradd[C_COL_SEL] = true;
                    else
                        dradd[C_COL_SEL] = false;

                    foreach (DataColumn dtc in dttemp.Columns)
                    {
                        dradd[dtc.ColumnName] = dtr[dtc.ColumnName];
                    }

                    dtFiltroStato.Rows.Add(dradd);
                }

                this.ucEasyGridFiltroStato.DataSource = dtFiltroStato;
                this.ucEasyGridFiltroStato.Refresh();

                if (codstatoselezionato != null && codstatoselezionato != "" && codstatoselezionato != CoreStatics.GC_TUTTI)
                    CoreStatics.SelezionaRigaInGriglia(ref ucEasyGridFiltroStato, "CodStato", codstatoselezionato);



                this.ucEasyGridFiltroTipo.DisplayLayout.Bands[0].Columns.ClearUnbound();

                if (this.ucEasyGridFiltroTipo.ActiveRow != null && this.ucEasyGridFiltroTipo.ActiveRow.Cells["CodTipoOrdine"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                    codTipoSelezionato = this.ucEasyGridFiltroTipo.ActiveRow.Cells["CodTipoOrdine"].Text;


                DataTable dtFiltroTipo = new DataTable();
                dttemp = CoreStatics.AggiungiTuttiDataTable(ds.Tables[2].Copy(), true);

                dtFiltroTipo.Columns.Add(C_COL_SEL, typeof(bool));

                foreach (DataColumn dtc in dttemp.Columns)
                {
                    dtFiltroTipo.Columns.Add(dtc.ColumnName, dtc.DataType);
                }

                foreach (DataRow dtr in dttemp.Rows)
                {
                    DataRow dradd = dtFiltroTipo.NewRow();

                    if (dtr["CodTipoOrdine"].ToString().Contains(CoreStatics.GC_TUTTI))
                        dradd[C_COL_SEL] = true;
                    else
                        dradd[C_COL_SEL] = false;

                    foreach (DataColumn dtc in dttemp.Columns)
                    {
                        dradd[dtc.ColumnName] = dtr[dtc.ColumnName];
                    }

                    dtFiltroTipo.Rows.Add(dradd);
                }

                this.ucEasyGridFiltroTipo.DataSource = dtFiltroTipo;
                this.ucEasyGridFiltroTipo.Refresh();

                if (codTipoSelezionato != null && codTipoSelezionato != "" && codTipoSelezionato != CoreStatics.GC_TUTTI)
                    CoreStatics.SelezionaRigaInGriglia(ref ucEasyGridFiltroTipo, "CodTipoOrdine", codTipoSelezionato);



                this.ucEasyGridFiltroUA.DisplayLayout.Bands[0].Columns.ClearUnbound();

                if (this.ucEasyGridFiltroUA.ActiveRow != null && this.ucEasyGridFiltroUA.ActiveRow.Cells["CodUA"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                    codUAselezionata = this.ucEasyGridFiltroUA.ActiveRow.Cells["CodUA"].Text;

                this.ucEasyGridFiltroUA.DataSource = CoreStatics.AggiungiTuttiDataTable(ds.Tables[3], true);
                this.ucEasyGridFiltroUA.Refresh();

                if (codUAselezionata != null && codUAselezionata != "" && codUAselezionata != CoreStatics.GC_TUTTI)
                    CoreStatics.SelezionaRigaInGriglia(ref ucEasyGridFiltroUA, "CodUA", codUAselezionata);

                AggiornaGriglia();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaGriglia", this.Name);
            }
        }

        private void AggiornaGriglia()
        {
            bool bFiltro = false;

            string codstatoselezionato = string.Empty;
            string codUAselezionata = string.Empty;
            string codTipoSelezionato = string.Empty;

            CoreStatics.SetNavigazione(false);

            try
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);
                CoreStatics.SetContesto(EnumEntita.OE, null);

                #region valutazione filtri e lancio stored di alimentazione tabella

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);

                if (this.txtRicerca.Text != string.Empty)
                {

                    string filtrogenerico = string.Empty;

                    string[] ricerche = this.txtRicerca.Text.Split(' ');
                    foreach (string ricerca in ricerche)
                    {

                        string format = "dd/MM/yyyy";
                        DateTime dateTime;
                        if (DateTime.TryParseExact(ricerca, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                        {
                            op.Parametro.Add("DataNascita", ricerca);
                        }
                        else
                        {
                            filtrogenerico += ricerca + " ";
                        }

                    }

                    op.Parametro.Add("FiltroGenerico", filtrogenerico);

                }

                op.Parametro.Add("DatiEstesi", "0");

                if (this.udteFiltroDA.Value != null)
                {
                    op.Parametro.Add("DataProgrammazioneOEInizio", ((DateTime)this.udteFiltroDA.Value).ToString("dd/MM/yyyy HH:mm").Replace(".", ":"));
                    bFiltro = true;
                }

                if (this.udteFiltroA.Value != null)
                {
                    op.Parametro.Add("DataProgrammazioneOEFine", ((DateTime)this.udteFiltroA.Value).ToString("dd/MM/yyyy HH:mm").Replace(".", ":"));
                    bFiltro = true;
                }

                List<UltraGridRow> stati = this.ucEasyGridFiltroStato.Rows.ToList<UltraGridRow>().FindAll(r => !r.Cells["CodStato"].Text.Contains(CoreStatics.GC_TUTTI) && (bool)r.Cells[C_COL_SEL].Value == true);
                if (stati != null && stati.Count > 0)
                {
                    List<string> codstati = new List<string>();

                    foreach (var row in stati)
                    {
                        if ((bool)row.Cells[C_COL_SEL].Value)
                        {
                            codstati.Add(row.Cells["CodStato"].Text);
                        }
                    }

                    op.ParametroRipetibile.Add("CodStatoOrdine", codstati.ToArray());
                    bFiltro = true;
                }

                List<UltraGridRow> eroganti = this.ucEasyGridFiltroTipo.Rows.ToList<UltraGridRow>().FindAll(r => !r.Cells["CodTipoOrdine"].Text.Contains(CoreStatics.GC_TUTTI) && (bool)r.Cells[C_COL_SEL].Value == true);
                if (eroganti != null && eroganti.Count > 0)
                {
                    List<string> coderoganti = new List<string>();

                    foreach (var row in eroganti)
                    {
                        if ((bool)row.Cells[C_COL_SEL].Value)
                        {
                            coderoganti.Add(row.Cells["CodTipoOrdine"].Text);
                        }
                    }

                    op.ParametroRipetibile.Add("CodTipoOrdine", coderoganti.ToArray());
                    bFiltro = true;
                }

                if (this.ucEasyGridFiltroUA.ActiveRow != null && this.ucEasyGridFiltroUA.ActiveRow.Cells["CodUA"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                {
                    op.Parametro.Add("CodUA", this.ucEasyGridFiltroUA.ActiveRow.Cells["CodUA"].Text);
                    bFiltro = true;
                }

                if (this.cboFiltriSpec.SelectedIndex > 0)
                {
                    string codFiltro = Convert.ToString(this.cboFiltriSpec.Value);
                    op.Parametro.Add("CodFiltroSpeciale", codFiltro);
                }


                this.uchkFiltro.Checked = bFiltro;

                op.TimeStamp.CodEntita = EnumEntita.OE.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_SelMovOrdiniTrasversali", spcoll);

                #endregion

                #region Preparazione tabella di destinazione per UltraGrid

                DataTable dtEdit = new DataTable();

                dtEdit.Columns.Add(C_COL_SEL, typeof(bool));

                foreach (DataColumn dtc in ds.Tables[0].Columns)
                {
                    dtEdit.Columns.Add(dtc.ColumnName, dtc.DataType);
                }

                dtEdit.Columns.Add("DescInserimento", typeof(string));
                dtEdit.Columns.Add("UtenteInserimento", typeof(string));
                dtEdit.Columns.Add("DescModifica", typeof(string));
                dtEdit.Columns.Add("DescNumeroPrestazioni", typeof(string));
                dtEdit.Columns.Add("PERMESSOCANCELLA", typeof(bool));
                dtEdit.Columns.Add("PERMESSOINOLTRA", typeof(bool));
                dtEdit.Columns.Add("StatoIcona", typeof(byte[]));

                foreach (DataRow dtr in ds.Tables[0].Rows)
                {
                    DataRow dradd = dtEdit.NewRow();

                    foreach (DataColumn dtc in ds.Tables[0].Columns)
                    {
                        dradd[dtc.ColumnName] = dtr[dtc.ColumnName];
                    }

                    dtEdit.Rows.Add(dradd);
                }

                foreach (DataRow row in dtEdit.Rows)
                {
                    row[C_COL_SEL] = false;
                    DateTime dttemp = DateTime.MinValue;
                    DateTime.TryParse(row["DataInserimento"].ToString(), out dttemp);
                    if (dttemp != DateTime.MinValue)
                        row["DescInserimento"] = "Inserito il: " + dttemp.ToString("dd/MM/yyyy HH:mm");
                    else
                        row["DescInserimento"] = string.Empty;

                    row["UtenteInserimento"] = "Da: " + row["DescrUtenteInserimento"].ToString();

                    DateTime.TryParse(row["DataUltimaModifica"].ToString(), out dttemp);
                    if (dttemp != DateTime.MinValue)
                        row["DescModifica"] = "Modificato il: " + dttemp.ToString("dd/MM/yyyy HH:mm");
                    else
                        row["DescModifica"] = string.Empty;

                    switch ((EnumStatoOrdine)Enum.Parse(typeof(EnumStatoOrdine), row["CodStatoOrdine"].ToString()))
                    {
                        case EnumStatoOrdine.CA:
                        case EnumStatoOrdine.AN:
                            row["PERMESSOCANCELLA"] = false;
                            row["PERMESSOINOLTRA"] = false;
                            break;

                        case EnumStatoOrdine.IS:
                            row["PERMESSOCANCELLA"] = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Ordini_Cancella);
                            row["PERMESSOINOLTRA"] = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Ordini_Inoltra);
                            break;

                        default:
                            row["PERMESSOCANCELLA"] = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Ordini_Cancella);
                            row["PERMESSOINOLTRA"] = false;
                            break;

                    }

                    switch ((EnumStatoOrdine)Enum.Parse(typeof(EnumStatoOrdine), row["CodStatoOrdine"].ToString()))
                    {

                        case EnumStatoOrdine.IS:
                        case EnumStatoOrdine.VA:
                        case EnumStatoOrdine.AC:
                            row["StatoIcona"] = CoreStatics.ImageToByte(Risorse.GetImageFromResource(Risorse.GC_INPROGRESS_256));
                            break;

                        case EnumStatoOrdine.IC:
                        case EnumStatoOrdine.PR:
                        case EnumStatoOrdine.ER:
                            row["StatoIcona"] = CoreStatics.ImageToByte(Risorse.GetImageFromResource(Risorse.GC_MODIFICACHECK_256));
                            break;

                        case EnumStatoOrdine.CA:
                        case EnumStatoOrdine.ET:
                        case EnumStatoOrdine.AN:
                            row["StatoIcona"] = CoreStatics.ImageToByte(Risorse.GetImageFromResource(Risorse.GC_ELIMINA_256));
                            break;

                    }

                }

                #endregion

                this.ucEasyGrid.DataSource = null;
                this.ucEasyGrid.DataSource = dtEdit;
                this.ucEasyGrid.Refresh();
                this.ucEasyGrid.UpdateMode = UpdateMode.OnUpdate;

                this.ucEasyGrid.DisplayLayout.Bands[0].Columns[C_COL_SEL].EditorComponent = this.UltraGridCustomEditorCheck;

                Graphics g = this.CreateGraphics();
                CoreStatics.ImpostaGroupByGriglia(ref this.ucEasyGrid, ref g, "DescrPaziente");
                this.ucEasyGrid.PerformLayout();

                if (this.ucEasyGrid.Rows.Count > 0 && this.ucEasyGrid.Rows[0].ChildBands.Count > 0 && this.ucEasyGrid.Rows[0].ChildBands[0].Rows.Count > 0)
                {
                    this.ucEasyGrid.Selected.Rows.Clear();
                    this.ucEasyGrid.Selected.Rows.Add(this.ucEasyGrid.Rows[0].ChildBands[0].Rows[0]);
                    this.ucEasyGrid.Rows[0].ChildBands[0].Rows[0].Activate();
                }
                else
                {
                    CaricaDettaglio(true);
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

        private void CaricaDettaglio(bool clearonly)
        {
            try
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

                this.txtDataProgrammazione.Text = string.Empty;
                this.txtEroganti.Text = string.Empty;
                this.txtPrestazioni.Text = string.Empty;

                if (!clearonly && ucEasyGrid.ActiveRow != null && ucEasyGrid.Rows.GetFilteredInNonGroupByRows().Contains(ucEasyGrid.ActiveRow) && !ucEasyGrid.ActiveRow.Hidden)
                {
                    DateTime dataprogrammazione = DateTime.MinValue;
                    DateTime.TryParse(ucEasyGrid.ActiveRow.Cells["DataProgrammazioneOE"].Value.ToString(), out dataprogrammazione);

                    this.txtDataProgrammazione.Text = (dataprogrammazione != DateTime.MinValue ? dataprogrammazione.ToString("dd/MM/yyyy HH:mm") : string.Empty);

                    this.txtEroganti.Text = ucEasyGrid.ActiveRow.Cells["Eroganti"].Value.ToString();
                    this.txtPrestazioni.Text = ucEasyGrid.ActiveRow.Cells["Prestazioni"].Value.ToString();

                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Errore caricamento dettaglio ordine", "CaricaDettaglio", this.Name);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
            }
        }

        private bool ControlloSelezioni()
        {
            bool bRet = false;

            if (this.ucEasyGrid.Rows.Count > 0)
            {
                foreach (UltraGridRow row in this.ucEasyGrid.Rows)
                {
                    if (row.IsGroupByRow)
                    {
                        foreach (UltraGridRow rowg in row.ChildBands[0].Rows)
                        {
                            bRet = bool.Parse(rowg.Cells[C_COL_SEL].Value.ToString());
                            if (bRet) break;
                        }
                    }
                    if (bRet) break;
                }
            }
            else
                bRet = false;

            return bRet;
        }

        private int ContaSelezionati()
        {
            int nRet = 0;

            if (this.ucEasyGrid.Rows.Count > 0)
            {
                foreach (UltraGridRow row in this.ucEasyGrid.Rows)
                {

                    if (row.IsGroupByRow)
                    {
                        foreach (UltraGridRow rowg in row.ChildBands[0].Rows)
                        {
                            if (bool.Parse(rowg.Cells[C_COL_SEL].Value.ToString())) nRet += 1;
                        }
                    }
                }
            }
            else
                nRet = 0;

            return nRet;
        }

        private void SetProgressBar(bool showprogressbar)
        {
            this.SetProgressBar(showprogressbar, 0, 0);
        }
        private void SetProgressBar(bool showprogressbar, int maxvalue)
        {
            this.SetProgressBar(showprogressbar, 0, maxvalue);
        }
        private void SetProgressBar(bool showprogressbar, int minvalue, int maxvalue)
        {
            if (showprogressbar)
            {
                this.ucEasyTableLayoutPanel.RowStyles[3] = new RowStyle(SizeType.Absolute, 50);
            }
            else
            {
                this.ucEasyTableLayoutPanel.RowStyles[3] = new RowStyle(SizeType.Absolute, 0);
            }
            this.pbAvanzamento.Minimum = minvalue;
            this.pbAvanzamento.Maximum = maxvalue;
            this.pbAvanzamento.Value = minvalue;
        }

        private void CheckRighe(ref RowsCollection rows, bool checkstate)
        {

            for (int iRow = 0; iRow < rows.Count; iRow++)
            {
                UltraGridRow row = rows[iRow];
                if (row.IsGroupByRow)
                {
                    if (row.ChildBands.Count > 0 && row.ChildBands[0].Rows.Count > 0)
                    {
                        RowsCollection childrows = row.ChildBands[0].Rows;
                        this.CheckRighe(ref childrows, checkstate);
                    }
                }
                else
                {
                    if (!row.Cells[C_COL_SEL].Hidden)
                    {
                        row.Cells[C_COL_SEL].Value = checkstate;
                    }
                }
            }
        }

        #endregion

        #region UltraGrid Custom Control

        private void InizializzaCustomControUltraGrid()
        {
            CheckEditorForRendering.Dock = DockStyle.None;
            CheckEditorForEditing.Dock = DockStyle.None;
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

        #endregion

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

        private void uchkFiltro_Click(object sender, EventArgs e)
        {
            if (!this.uchkFiltro.Checked)
            {
                this.InizializzaFiltri();

                this.drFiltro.Value = ucEasyDateRange.C_RNG_DOMANI;
                this.udteFiltroDA.Value = drFiltro.DataOraDa;
                this.udteFiltroA.Value = drFiltro.DataOraA;

                CoreStatics.SelezionaRigaInGriglia(ref ucEasyGridFiltroStato, "CodStato", EnumStatoOrdine.IS.ToString());
                CoreStatics.SelezionaRigaInGriglia(ref ucEasyGridFiltroTipo, "CodTipoOrdine", CoreStatics.GC_TUTTI);
                CoreStatics.SelezionaRigaInGriglia(ref ucEasyGridFiltroUA, "CodUA", CoreStatics.GC_TUTTI);

                this.AggiornaGriglia();

            }
            else
                this.uchkFiltro.Checked = !this.uchkFiltro.Checked;
        }

        private void drFiltro_ValueChanged(object sender, EventArgs e)
        {
            this.udteFiltroDA.Value = drFiltro.DataOraDa;
            this.udteFiltroA.Value = drFiltro.DataOraA;
        }

        private void GridFiltri_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].ColHeadersVisible = false;
            foreach (var col in e.Layout.Bands[0].Columns)
            {
                if (col.Key.Contains("Cod"))
                {
                    col.Hidden = true;
                }
            }
        }

        private void ucEasyGridFiltroStato_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].ColHeadersVisible = false;
            foreach (var col in e.Layout.Bands[0].Columns)
            {
                switch (col.Key)
                {
                    case "CodStato":
                        col.Hidden = true;
                        break;
                    case C_COL_SEL:
                        col.Header.CheckBoxSynchronization = HeaderCheckBoxSynchronization.Band;
                        col.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
                        col.CellClickAction = CellClickAction.Edit;
                        col.CellActivation = Activation.AllowEdit;
                        break;
                }


            }
        }

        private void ucEasyGridFiltroStato_ClickCell(object sender, ClickCellEventArgs e)
        {
            if (e.Cell.Column.Key == C_COL_SEL)
            {
                if (e.Cell.Row.Cells["CodStato"].Text.Contains(CoreStatics.GC_TUTTI))
                {
                    List<UltraGridRow> rows = this.ucEasyGridFiltroStato.Rows.ToList<UltraGridRow>().FindAll(r => !r.Cells["CodStato"].Text.Contains(CoreStatics.GC_TUTTI));
                    if (rows != null)
                    {
                        foreach (UltraGridRow row in rows)
                        {
                            row.Cells[C_COL_SEL].Value = false;
                        }
                    }
                }
                else
                {
                    UltraGridRow rowtutti = this.ucEasyGridFiltroStato.Rows.ToList<UltraGridRow>().Find(r => r.Cells["CodStato"].Text.Contains(CoreStatics.GC_TUTTI));
                    if (rowtutti != null)
                    {
                        rowtutti.Cells[C_COL_SEL].Value = false;
                    }
                }

                e.Cell.Row.Update();
            }
        }

        private void ucEasyGridFiltroTipo_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].ColHeadersVisible = false;
            foreach (var col in e.Layout.Bands[0].Columns)
            {
                switch (col.Key)
                {
                    case "CodTipoOrdine":
                        col.Hidden = true;
                        break;
                    case C_COL_SEL:
                        col.Header.CheckBoxSynchronization = HeaderCheckBoxSynchronization.Band;
                        col.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
                        col.CellClickAction = CellClickAction.Edit;
                        col.CellActivation = Activation.AllowEdit;
                        break;
                }


            }
        }

        private void ucEasyGridFiltroTipo_ClickCell(object sender, ClickCellEventArgs e)
        {
            if (e.Cell.Column.Key == C_COL_SEL)
            {
                if (e.Cell.Row.Cells["CodTipoOrdine"].Text.Contains(CoreStatics.GC_TUTTI))
                {
                    List<UltraGridRow> rows = this.ucEasyGridFiltroTipo.Rows.ToList<UltraGridRow>().FindAll(r => !r.Cells["CodTipoOrdine"].Text.Contains(CoreStatics.GC_TUTTI));
                    if (rows != null)
                    {
                        foreach (UltraGridRow row in rows)
                        {
                            row.Cells[C_COL_SEL].Value = false;
                        }
                    }
                }
                else
                {
                    UltraGridRow rowtutti = this.ucEasyGridFiltroTipo.Rows.ToList<UltraGridRow>().Find(r => r.Cells["CodTipoOrdine"].Text.Contains(CoreStatics.GC_TUTTI));
                    if (rowtutti != null)
                    {
                        rowtutti.Cells[C_COL_SEL].Value = false;
                    }
                }

                e.Cell.Row.Update();
            }
        }

        private void ucEasyGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                int refWidth = (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XLarge) * 3;

                e.Layout.Bands[0].HeaderVisible = false;
                e.Layout.Bands[0].ColHeadersVisible = false;
                e.Layout.Bands[0].RowLayoutStyle = RowLayoutStyle.GroupLayout;

                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {

                    try
                    {
                        oCol.SortIndicator = SortIndicator.Disabled;

                        #region formattazione colonne griglia

                        switch (oCol.Key)
                        {

                            case C_COL_SEL:
                                oCol.Header.CheckBoxSynchronization = HeaderCheckBoxSynchronization.Band;
                                oCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
                                oCol.CellClickAction = CellClickAction.Edit;
                                oCol.CellActivation = Activation.AllowEdit;

                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                try
                                {
                                    oCol.MinWidth = Convert.ToInt32(refWidth * 0.40);
                                    oCol.MaxWidth = oCol.MinWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.LockedWidth = true;

                                oCol.RowLayoutColumnInfo.OriginX = 0;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 2;

                                break;

                            case "DataProgrammazioneOE":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Red;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.CellAppearance.FontData.Bold = DefaultableBoolean.True;
                                oCol.Format = "dd/MM/yyyy HH:mm";
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 1.5);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 2;
                                oCol.RowLayoutColumnInfo.SpanY = 1;
                                break;

                            case "StatoIcona":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;

                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 0.30);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 1;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 2;

                                break;

                            case "NumeroOrdineOE":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 1.0);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 2;
                                oCol.RowLayoutColumnInfo.OriginY = 1;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "DescInserimento":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 2);
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

                            case "UtenteInserimento":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 2);
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

                            case "DescModifica":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 2);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 3;
                                oCol.RowLayoutColumnInfo.OriginY = 2;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "Eroganti":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = this.ucEasyGrid.Width - Convert.ToInt32(refWidth * 6.85) - 30;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 4;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "Prestazioni":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = this.ucEasyGrid.Width - Convert.ToInt32(refWidth * 6.85) - 30;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 4;
                                oCol.RowLayoutColumnInfo.OriginY = 1;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 2;

                                break;

                            case "DescrStatoOrdine":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.FontData.Italic = Infragistics.Win.DefaultableBoolean.True;
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = refWidth;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 5;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;
                                break;

                            case "DescrPrioritaOrdine":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.FontData.Italic = Infragistics.Win.DefaultableBoolean.True;
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = refWidth;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 5;
                                oCol.RowLayoutColumnInfo.OriginY = 2;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;
                                break;


                            default:
                                oCol.Hidden = true;
                                break;
                        }

                        #endregion

                    }
                    catch (Exception)
                    {
                    }

                }

            }
            catch
            {
            }
        }

        private void ucEasyGrid_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            try
            {
                if (e.Row.Cells["CodStatoOrdine"].Value.ToString() != EnumStatoOrdine.IS.ToString())
                {
                    e.Row.Cells[C_COL_SEL].Hidden = true;
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGrid_InitializeRow", this.Name);
            }
        }

        private void ucEasyGrid_AfterRowActivate(object sender, EventArgs e)
        {
            CoreStatics.SetContesto(EnumEntita.OE, this.ucEasyGrid.ActiveRow);
            this.CaricaDettaglio(false);
        }

        private void ucEasyGrid_AfterCellUpdate(object sender, CellEventArgs e)
        {
            e.Cell.Row.Update();
        }

        private void ubApplicaFiltro_Click(object sender, EventArgs e)
        {
            if (this.ultraDockManager.FlyoutPane != null && !this.ultraDockManager.FlyoutPane.Pinned) this.ultraDockManager.FlyIn();
            this.AggiornaGriglia();
            this.ucEasyGrid.Focus();
        }

        private void txtRicerca_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.AggiornaGriglia();
                    this.ucEasyGrid.Focus();
                }
            }
            catch (Exception)
            {
            }
        }

        private void ubInoltra_Click(object sender, EventArgs e)
        {
            ScciAmbiente AmbienteOE = CoreStatics.CoreApplication.Ambiente;
            AmbienteOE.Nomepc = CoreStatics.CoreApplication.Sessione.Computer.NomeDominioCompleto;

            Paziente paziente = null;
            Episodio episodio = null;
            Trasferimento trasferimento = null;
            MovOrdine ordine = null;
            OEOrdineDettaglio ordineOE = null;
            ScciOrderEntryClient oeclient = null;

            string sLog = string.Empty;

            if (this.ControlloSelezioni())
            {
                this.Cursor = CoreStatics.setCursor(enum_app_cursors.WaitCursor);
                this.SetProgressBar(true, this.ContaSelezionati());

                foreach (UltraGridRow row in this.ucEasyGrid.Rows)
                {
                    if (row.IsGroupByRow)
                    {
                        foreach (UltraGridRow rowg in row.ChildBands[0].Rows)
                        {
                            if (bool.Parse(rowg.Cells[C_COL_SEL].Value.ToString()))
                            {
                                this.pbAvanzamento.Value += 1;
                                this.Refresh();

                                if (bool.Parse(rowg.Cells["PERMESSOINOLTRA"].Value.ToString()))
                                {

                                    paziente = new Paziente(rowg.Cells["IDPaziente"].Value.ToString(), rowg.Cells["IDEpisodio"].Value.ToString());

                                    if (rowg.Cells["IDEpisodio"].Value.ToString() == string.Empty)
                                    {
                                        episodio = null;
                                        trasferimento = null;


                                        if (oeclient == null) oeclient = ScciSvcRef.GetSvcOrderEntry();
                                        ordineOE = oeclient.GetOrdineDettaglio(AmbienteOE.Codlogin, rowg.Cells["NumeroOrdineOE"].Value.ToString());

                                        if (ordineOE.OrdineTestata != null)
                                        {
                                            ordine = new MovOrdine(rowg.Cells["NumeroOrdineOE"].Value.ToString(),
                                                               AmbienteOE, string.Empty,
                                                               "AMB",
                                                               string.Empty,
                                                               string.Empty,
                                                               ordineOE.OrdineTestata.UnitaOperativaAziendaCodice,
                                                               string.Empty,
                                                               ordineOE.OrdineTestata.UnitaOperativaCodice,
                                                               CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata,
                                                               paziente);
                                        }
                                        else
                                        {
                                            ordine = null;
                                        }
                                    }
                                    else
                                    {
                                        episodio = new Episodio(rowg.Cells["IDEpisodio"].Value.ToString());
                                        trasferimento = new Trasferimento(rowg.Cells["IDTrasferimento"].Value.ToString(), AmbienteOE);

                                        string sCodAziOrdine = string.Empty;
                                        if (trasferimento.CodAziTrasferimento != null && trasferimento.CodAziTrasferimento != string.Empty)
                                        { sCodAziOrdine = trasferimento.CodAziTrasferimento; }
                                        else
                                        { sCodAziOrdine = episodio.CodAzienda; }

                                        ordine = new MovOrdine(rowg.Cells["NumeroOrdineOE"].Value.ToString(),
                                                               AmbienteOE, episodio.ID,
                                                               episodio.CodTipoEpisodio,
                                                               episodio.NumeroEpisodio,
                                                               episodio.NumeroListaAttesa,
                                                               sCodAziOrdine,
                                                               trasferimento.ID,
                                                               trasferimento.CodUO,
                                                               trasferimento.CodUA,
                                                               paziente);
                                    }

                                    if (ordine != null)
                                    {
                                        if (ordine.StatoValiditaOrdine == OEValiditaOrdine.Valido)
                                        {
                                            if (ordine.Prestazioni.Count > 0)
                                            {
                                                if (!ordine.InoltraOrdine())
                                                {
                                                    sLog += "Ordine " + rowg.Cells["NumeroOrdineOE"].Value.ToString() + " non inoltrato. " + Environment.NewLine;
                                                    if (ordine.UltimaEccezioneGenerata != null)
                                                        sLog += "Errore ritornato: " + Environment.NewLine + ordine.UltimaEccezioneGenerata.Message + Environment.NewLine;
                                                }
                                                else
                                                {
                                                    if (ordine.StatoValiditaOrdine != OEValiditaOrdine.Valido)
                                                    {
                                                        sLog += "Ordine " + rowg.Cells["NumeroOrdineOE"].Value.ToString() + " non inoltrato. " + Environment.NewLine;
                                                        sLog += "Errore ritornato: stato validazione non valido." + Environment.NewLine;
                                                    }
                                                    else
                                                    {

                                                        CoreStatics.CoreApplication.MovOrdineSelezionato = ordine;

                                                        if (ordine.Paziente.ID != string.Empty)
                                                            CoreStatics.CoreApplication.Paziente = new Paziente(ordine.Paziente.ID, ordine.IDEpisodio);


                                                        if (ordine.IDEpisodio != string.Empty)
                                                            CoreStatics.CoreApplication.Episodio = new Episodio(ordine.IDEpisodio);

                                                        if (ordine.IDTrasferimento != string.Empty)
                                                        {
                                                            CoreStatics.CoreApplication.Trasferimento = new Trasferimento(rowg.Cells["IDTrasferimento"].Value.ToString(), AmbienteOE);
                                                            CoreStatics.CoreApplication.Cartella = new Cartella(trasferimento.IDCartella, string.Empty, AmbienteOE);
                                                        }

                                                        Risposta oRispostaElaboraPrima = PluginClientStatics.PluginClient(EnumPluginClient.OE_INOLTRA_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovOrdineSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));
                                                        if (oRispostaElaboraPrima.Successo == true)
                                                        {
                                                        }

                                                        CoreStatics.CoreApplication.Cartella = null;
                                                        CoreStatics.CoreApplication.Episodio = null;
                                                        CoreStatics.CoreApplication.Paziente = null;
                                                        CoreStatics.CoreApplication.Trasferimento = null;
                                                    }

                                                }
                                            }
                                            else
                                            {
                                                sLog += "Ordine " + rowg.Cells["NumeroOrdineOE"].Value.ToString() + " non inoltrato. " + Environment.NewLine;
                                                sLog += "Errore ritornato: Nessuna prestazione inserita per l'ordine " + rowg.Cells["NumeroOrdineOE"].Value.ToString() + Environment.NewLine;
                                            }
                                        }
                                        else
                                        {
                                            sLog += "Ordine " + rowg.Cells["NumeroOrdineOE"].Value.ToString() + " non inoltrato." + Environment.NewLine;
                                            sLog += "Errore ritornato: Stato validazione ordine non valido" + Environment.NewLine;
                                        }
                                    }
                                    else
                                    {
                                        sLog += "Ordine " + rowg.Cells["NumeroOrdineOE"].Value.ToString() + " non inoltrato." + Environment.NewLine;
                                        sLog += "Errore ritornato: Impossibile caricare l'ordine" + Environment.NewLine;
                                    }

                                    episodio = null;
                                    trasferimento = null;
                                    paziente = null;
                                    ordine = null;
                                    ordineOE = null;
                                }
                                else
                                {
                                    sLog += "Ordine " + rowg.Cells["NumeroOrdineOE"].Value.ToString() + " non inoltrato. " + Environment.NewLine;
                                    sLog += "Permesso di inoltro negato all'utente: " + CoreStatics.CoreApplication.Ambiente.Codlogin + Environment.NewLine;
                                }

                                if (sLog != string.Empty) sLog += Environment.NewLine;
                            }
                        }
                    }
                }

                this.SetProgressBar(false);

                this.Cursor = CoreStatics.setCursor(enum_app_cursors.DefaultCursor);

                if (oeclient != null) oeclient = null;

                if (sLog != string.Empty)
                {
                    easyStatics.EasyMessageBoxInfo(sLog, "Errori di inoltro ordini", "Gestione Ordini", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                this.AggiornaGriglia();
            }
            else
            {
                easyStatics.EasyMessageBox("Nessun ordine selezionato per l'inoltro", "Gestione ordini", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void ubCancella_Click(object sender, EventArgs e)
        {

            ScciAmbiente AmbienteOE = CoreStatics.CoreApplication.Ambiente;
            AmbienteOE.Nomepc = CoreStatics.CoreApplication.Sessione.Computer.NomeDominioCompleto;

            Paziente paziente = null;
            Episodio episodio = null;
            Trasferimento trasferimento = null;
            MovOrdine ordine = null;
            OEOrdineDettaglio ordineOE = null;
            ScciOrderEntryClient oeclient = null;

            string sLog = string.Empty;

            if (this.ControlloSelezioni())
            {
                if (easyStatics.EasyMessageBox("Confermi la cancellazione degli ordini selezionati ?", "Cancellazione ordini", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    this.Cursor = CoreStatics.setCursor(enum_app_cursors.WaitCursor);

                    this.SetProgressBar(true, this.ContaSelezionati());

                    foreach (UltraGridRow row in this.ucEasyGrid.Rows)
                    {
                        if (row.IsGroupByRow)
                        {
                            foreach (UltraGridRow rowg in row.ChildBands[0].Rows)
                            {
                                if (bool.Parse(rowg.Cells[C_COL_SEL].Value.ToString()))
                                {
                                    this.pbAvanzamento.Value += 1;
                                    this.Refresh();

                                    if (bool.Parse(rowg.Cells["PERMESSOCANCELLA"].Value.ToString()))
                                    {
                                        paziente = new Paziente(rowg.Cells["IDPaziente"].Value.ToString(), rowg.Cells["IDEpisodio"].Value.ToString());

                                        if (rowg.Cells["IDEpisodio"].Value.ToString() == string.Empty)
                                        {
                                            episodio = null;
                                            trasferimento = null;


                                            if (oeclient == null) oeclient = ScciSvcRef.GetSvcOrderEntry();
                                            ordineOE = oeclient.GetOrdineDettaglio(AmbienteOE.Codlogin, rowg.Cells["NumeroOrdineOE"].Value.ToString());

                                            if (ordineOE.OrdineTestata != null)
                                            {
                                                ordine = new MovOrdine(rowg.Cells["NumeroOrdineOE"].Value.ToString(),
                                                                   AmbienteOE, string.Empty,
                                                                   "AMB",
                                                                   string.Empty,
                                                                   string.Empty,
                                                                   ordineOE.OrdineTestata.UnitaOperativaAziendaCodice,
                                                                   string.Empty,
                                                                   ordineOE.OrdineTestata.UnitaOperativaCodice,
                                                                   CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata,
                                                                   paziente);
                                            }
                                            else
                                            {
                                                ordine = null;
                                            }
                                        }
                                        else
                                        {
                                            episodio = new Episodio(rowg.Cells["IDEpisodio"].Value.ToString());
                                            trasferimento = new Trasferimento(rowg.Cells["IDTrasferimento"].Value.ToString(), AmbienteOE);

                                            string sCodAziOrdine = string.Empty;
                                            if (trasferimento.CodAziTrasferimento != null && trasferimento.CodAziTrasferimento != string.Empty)
                                            { sCodAziOrdine = trasferimento.CodAziTrasferimento; }
                                            else
                                            { sCodAziOrdine = episodio.CodAzienda; }

                                            ordine = new MovOrdine(rowg.Cells["NumeroOrdineOE"].Value.ToString(),
                                                                   AmbienteOE, episodio.ID,
                                                                   episodio.CodTipoEpisodio,
                                                                   episodio.NumeroEpisodio,
                                                                   episodio.NumeroListaAttesa,
                                                                   sCodAziOrdine,
                                                                   trasferimento.ID,
                                                                   trasferimento.CodUO,
                                                                   trasferimento.CodUA,
                                                                   paziente);
                                        }

                                        if (ordine != null)
                                        {
                                            if (ordine.StatoOrdine != OEStato.Cancellato)
                                            {
                                                if (ordine.Cancellabile)
                                                {
                                                    bool bDelete = true;
                                                    if (ordine.StatoOrdine != OEStato.Inserito)
                                                    {
                                                        string sMsg = @"";
                                                        sMsg += @"L'ordine " + rowg.Cells["NumeroOrdineOE"].Value.ToString() + @" è già stato inoltrato";
                                                        if (ordine.UtenteInoltro != null && ordine.UtenteInoltro.Trim() != "")
                                                            sMsg += @" da " + ordine.UtenteInoltro;
                                                        if (ordine.DataInoltro > DateTime.MinValue)
                                                            sMsg += @" in data/ora " + ordine.DataInoltro.ToString(@"dd/MM/yyyy HH:mm");
                                                        sMsg += @"." + Environment.NewLine;
                                                        sMsg += @"L'operazione potrebbe non andare a buon fine o bloccare l'operatività clinica." + Environment.NewLine;
                                                        sMsg += @"Sei sicuro?";
                                                        if (easyStatics.EasyMessageBox(sMsg, "Cancellazione Ordini", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                                                            bDelete = false;
                                                    }

                                                    if (bDelete)
                                                    {
                                                        if (!ordine.CancellaOrdine())
                                                        {
                                                            sLog += "Ordine " + rowg.Cells["NumeroOrdineOE"].Value.ToString() + " non cancellato." + Environment.NewLine;
                                                            if (ordine.UltimaEccezioneGenerata != null)
                                                                sLog += "Errore ritornato: " + Environment.NewLine + ordine.UltimaEccezioneGenerata.Message + Environment.NewLine;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    sLog += "Ordine " + rowg.Cells["NumeroOrdineOE"].Value.ToString() + " non cancellato." + Environment.NewLine;
                                                    sLog += "Errore ritornato: Ordine NON cancellabile" + Environment.NewLine;
                                                }
                                            }
                                            else
                                            {
                                                sLog += "Ordine " + rowg.Cells["NumeroOrdineOE"].Value.ToString() + " non cancellato." + Environment.NewLine;
                                                sLog += "Errore ritornato: Impossibile cancellare un ordine già in stato cancellato" + Environment.NewLine;
                                            }
                                        }
                                        else
                                        {
                                            sLog += "Ordine " + rowg.Cells["NumeroOrdineOE"].Value.ToString() + " non cancellato." + Environment.NewLine;
                                            sLog += "Errore ritornato: Impossibile caricare l'ordine" + Environment.NewLine;
                                        }

                                        episodio = null;
                                        trasferimento = null;
                                        paziente = null;
                                        ordine = null;
                                        ordineOE = null;
                                    }
                                    else
                                    {
                                        sLog += "Ordine " + rowg.Cells["NumeroOrdineOE"].Value.ToString() + " non cancellato." + Environment.NewLine;
                                        sLog += "Permesso di cancellazione negato all'utente: " + CoreStatics.CoreApplication.Ambiente.Codlogin + Environment.NewLine;
                                    }

                                    if (sLog != string.Empty) sLog += Environment.NewLine;
                                }
                            }
                        }
                    }

                    this.SetProgressBar(false);

                    this.Cursor = CoreStatics.setCursor(enum_app_cursors.DefaultCursor);

                    if (oeclient != null) oeclient = null;

                    if (sLog != string.Empty)
                    {
                        easyStatics.EasyMessageBoxInfo(sLog, "Errori di cancellazione ordini", "Gestione Ordini", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                    this.AggiornaGriglia();
                }
            }
            else
            {
                easyStatics.EasyMessageBox("Nessun ordine selezionato per la cancellazione", "Gestione ordini", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void ubSelezionaTutti_Click(object sender, EventArgs e)
        {
            RowsCollection rows = ucEasyGrid.Rows;
            this.CheckRighe(ref rows, true);
        }

        private void ubDeSelezionaTutti_Click(object sender, EventArgs e)
        {
            RowsCollection rows = ucEasyGrid.Rows;
            this.CheckRighe(ref rows, false);
        }

        #endregion

    }
}
