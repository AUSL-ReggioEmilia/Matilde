using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using ScciCommon.Model;
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
using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.ScciCore;

namespace UnicodeSrl.ScciManagement.UserControls
{
    public partial class ucAllineaADT : UserControl
    {
        public ucAllineaADT()
        {
            InitializeComponent();
        }

        #region Declare

        Color ColorAllOk = Color.LimeGreen;
        Color ColorDateOk = Color.GreenYellow;
        Color ColorError = Color.OrangeRed;
        Color ColorTrasferimentoSS = Color.OrangeRed;
        Color ColorTrasferimentoAllineato = Color.Yellow;
        Color ColorTrasferimentoCollegato = Color.LightBlue;

        #endregion

        #region UltraGrid

        private void LoadUltraGrid()
        {

            this.ugEpisodi.DataSource = null;
            this.ugEpisodioMatilde.DataSource = null;
            this.ugEpisodioDWH.DataSource = null;
            this.ugTrasferimentiMatilde.DataSource = null;
            this.ugTrasferimentiDWH.DataSource = null;
            this.ugSimulazione.DataSource = null;
            this.ugCorrezione.DataSource = null;
            Application.DoEvents();
            this.LoadPaziente();
            this.LoadUltraGridEpisodi();
            this.LoadUltraGridEpisodioMatilde();
            this.LoadUltraGridTrasferimentiMatilde();
            this.LoadUltraGridEpisodioDWH();
            this.LoadUltraGridTrasferimentiDWH();

        }

        private void LoadPaziente()
        {

            this.Cursor = Cursors.WaitCursor;

            this.lblPaziente.Text = string.Empty;

            try
            {

                if (this.ugEpisodi.ActiveRow != null)
                {
                    Paziente oPaziente = new Paziente("", this.ugEpisodi.ActiveRow.Cells["ID"].Text);
                    this.lblPaziente.Text = oPaziente.Descrizione;
                    oPaziente = null;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

        }

        private void LoadUltraGridEpisodi()
        {

            this.Cursor = Cursors.WaitCursor;

            MyStatics.SetUltraGridLayout(ref this.ugEpisodi, false, false);
            this.ugEpisodi.DisplayLayout.GroupByBox.Hidden = true;

            try
            {

                this.ugEpisodi.DataSource = null;
                if (this.uteIDEpisodio.Text != string.Empty || this.uteNosologico.Text != string.Empty || this.uteListaAttesa.Text != string.Empty)
                {

                    using (FwDataConnection conn = new FwDataConnection(Database.ConnectionString))
                    {

                        string ssql = $"Select * From T_MovEpisodi\n";
                        List<string> lst_where = new List<string>();

                        if (this.uteIDEpisodio.Text != string.Empty)
                        {
                            lst_where.Add($"ID = '{this.uteIDEpisodio.Text}'");
                        }
                        if (this.uteNosologico.Text != string.Empty)
                        {
                            lst_where.Add($"NumeroNosologico = '{this.uteNosologico.Text}'");
                        }
                        if (this.uteListaAttesa.Text != string.Empty)
                        {
                            lst_where.Add($"NumeroListaAttesa = '{this.uteListaAttesa.Text}'");
                        }
                        if (lst_where.Count > 0)
                        {
                            ssql += $"Where {String.Join(" OR ", lst_where)}";
                            this.ugEpisodi.DataSource = conn.Query<DataTable>(ssql);
                        }

                    }

                }

                this.ugEpisodi.Refresh();
                this.ugEpisodi.Text = string.Format("{0} ({1:#,##0})", "Episodi", this.ugEpisodi.Rows.Count);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

        }

        private void LoadUltraGridEpisodioMatilde()
        {

            this.Cursor = Cursors.WaitCursor;

            MyStatics.SetUltraGridLayout(ref this.ugEpisodioMatilde, false, false);
            this.ugEpisodioMatilde.DisplayLayout.GroupByBox.Hidden = true;
            this.ugEpisodioMatilde.DisplayLayout.Override.RowAppearance.ResetBackColor();
            this.ugEpisodioMatilde.DisplayLayout.Override.SelectedAppearancesEnabled = Infragistics.Win.DefaultableBoolean.False;
            this.ugEpisodioMatilde.DisplayLayout.Override.ActiveAppearancesEnabled = Infragistics.Win.DefaultableBoolean.False;

            try
            {

                this.ugEpisodioMatilde.DataSource = null;
                if (this.ugEpisodi.ActiveRow != null)
                {

                    using (FwDataConnection conn = new FwDataConnection(Database.ConnectionString))
                    {
                        string ssql = $"Select * " +
                                        $"From T_MovEpisodi " +
                                        $"Where ID = '{this.ugEpisodi.ActiveRow.Cells["ID"].Text}'";
                        this.ugEpisodioMatilde.DataSource = conn.Query<DataTable>(ssql);
                    }

                }

                this.ugEpisodioMatilde.Refresh();
                this.ugEpisodioMatilde.Text = string.Format("{0} ({1:#,##0})", "Episodio Matilde", this.ugEpisodioMatilde.Rows.Count);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

        }

        private void LoadUltraGridTrasferimentiMatilde()
        {

            this.Cursor = Cursors.WaitCursor;

            MyStatics.SetUltraGridLayout(ref this.ugTrasferimentiMatilde, false, false);
            this.ugTrasferimentiMatilde.DisplayLayout.GroupByBox.Hidden = true;
            this.ugTrasferimentiMatilde.DisplayLayout.Override.SelectedAppearancesEnabled = Infragistics.Win.DefaultableBoolean.False;
            this.ugTrasferimentiMatilde.DisplayLayout.Override.ActiveAppearancesEnabled = Infragistics.Win.DefaultableBoolean.False;

            try
            {

                this.ugTrasferimentiMatilde.DataSource = null;
                if (this.ugEpisodioMatilde.ActiveRow != null)
                {

                    using (FwDataConnection conn = new FwDataConnection(Database.ConnectionString))
                    {
                        string ssql = $"Select * " +
                                        $"From T_MovTrasferimenti " +
                                        $"Where IDEpisodio = '{this.ugEpisodioMatilde.ActiveRow.Cells["ID"].Text}'" +
                                        $"Order By DataIngresso";
                        this.ugTrasferimentiMatilde.DataSource = conn.Query<List<T_MovTrasferimentiEx>>(ssql, null, CommandType.Text);
                    }

                }

                this.ugTrasferimentiMatilde.Refresh();
                this.ugTrasferimentiMatilde.Text = string.Format("{0} ({1:#,##0})", "Trasferimenti Matilde", this.ugTrasferimentiMatilde.Rows.Count);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

        }

        private void LoadUltraGridEpisodioDWH()
        {

            this.Cursor = Cursors.WaitCursor;

            MyStatics.SetUltraGridLayout(ref this.ugEpisodioDWH, false, false);
            this.ugEpisodioDWH.DisplayLayout.GroupByBox.Hidden = true;
            this.ugEpisodioDWH.DisplayLayout.Override.RowAppearance.ResetBackColor();
            this.ugEpisodioDWH.DisplayLayout.Override.SelectedAppearancesEnabled = Infragistics.Win.DefaultableBoolean.False;
            this.ugEpisodioDWH.DisplayLayout.Override.ActiveAppearancesEnabled = Infragistics.Win.DefaultableBoolean.False;

            try
            {

                this.ugEpisodioDWH.DataSource = null;
                if (this.ugEpisodi.ActiveRow != null)
                {
                    Paziente oPaziente = new Paziente("", this.ugEpisodi.ActiveRow.Cells["ID"].Text);
                    DataTable dtEpisodi = DBUtils.getEpisodiPazienteDatatable(oPaziente.CodSAC, DateTime.MinValue, DateTime.MinValue);

                    DataView dv = new DataView(dtEpisodi);
                    string filter = $"Nosologico = '{this.ugEpisodi.ActiveRow.Cells["NumeroNosologico"].Text}' OR Nosologico = '{this.ugEpisodi.ActiveRow.Cells["NumeroListaAttesa"].Text}'";
                    dv.RowFilter = filter;
                    dv.Sort = "DataInizioRicovero,Nosologico";
                    this.ugEpisodioDWH.DataSource = dv;
                }

                this.ugEpisodioDWH.Refresh();
                this.ugEpisodioDWH.Text = string.Format("{0} ({1:#,##0})", "Episodio DWH", this.ugEpisodioDWH.Rows.Count);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

        }

        private void LoadUltraGridTrasferimentiDWH()
        {

            this.Cursor = Cursors.WaitCursor;

            MyStatics.SetUltraGridLayout(ref this.ugTrasferimentiDWH, false, false);
            this.ugTrasferimentiDWH.DisplayLayout.GroupByBox.Hidden = true;
            this.ugTrasferimentiDWH.DisplayLayout.Override.SelectedAppearancesEnabled = Infragistics.Win.DefaultableBoolean.False;
            this.ugTrasferimentiDWH.DisplayLayout.Override.ActiveAppearancesEnabled = Infragistics.Win.DefaultableBoolean.False;

            try
            {

                this.ugTrasferimentiDWH.DataSource = null;
                if (this.ugEpisodioDWH.Rows.Count > 0)
                {
                    DataTable dteventi = CoreStatics.CreateDataTable<EventoDWH>();
                    foreach (UltraGridRow ugr in this.ugEpisodioDWH.Rows)
                    {
                        RicoveroDWH objRicovero = DBUtils.getRicoveroDWH(ugr.Cells["IDRicovero"].Text);
                        if (objRicovero != null && objRicovero.IDRicovero != null && objRicovero.IDRicovero != string.Empty && objRicovero.IDRicovero.Trim() != "")
                        {
                            CoreStatics.FillDataTable<EventoDWH>(dteventi, objRicovero.EventiDWH);
                        }

                    }
                    dteventi.DefaultView.Sort = "DataEvento, Nosologico";
                    dteventi = dteventi.DefaultView.ToTable();

                    var query = dteventi.AsEnumerable()
                                .Select(p => new
                                {
                                    CodTipoEvento = p.Field<string>("CodTipoEvento"),
                                    DescTipoEvento = p.Field<string>("DescTipoEvento"),
                                    DataEvento = p.Field<DateTime>("DataEvento"),
                                    CodReparto = p.Field<string>("CodReparto"),
                                    DescReparto = p.Field<string>("DescReparto"),
                                    CodSettore = p.Field<string>("CodSettore"),
                                    DescSettore = p.Field<string>("DescSettore"),
                                    CodLetto = p.Field<string>("CodLetto"),
                                    DescLetto = p.Field<string>("DescLetto"),
                                    Nosologico = p.Field<string>("Nosologico"),
                                    AziendaErogante = p.Field<string>("AziendaErogante"),
                                    SistemaErogante = p.Field<string>("SistemaErogante"),
                                    CodTipoEpisodio = p.Field<string>("CodTipoEpisodio"),
                                    DescTipoEpisodio = p.Field<string>("DescTipoEpisodio"),
                                    IDEvento = p.Field<string>("IDEvento"),
                                    RepartoErogante = p.Field<string>("RepartoErogante"),
                                    Diagnosi = p.Field<string>("Diagnosi")
                                }).ToList();

                    this.ugTrasferimentiDWH.DataSource = query;
                }

                this.ugTrasferimentiDWH.Refresh();
                this.ugTrasferimentiDWH.Text = string.Format("{0} ({1:#,##0})", "Trasferimenti DWH", this.ugTrasferimentiDWH.Rows.Count);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

        }

        private void LoadUltraGridSimulazione(List<T_MovTrasferimentiEx> lst_MovTrasferimenti)
        {

            this.Cursor = Cursors.WaitCursor;

            MyStatics.SetUltraGridLayout(ref this.ugSimulazione, false, false);
            this.ugSimulazione.DisplayLayout.GroupByBox.Hidden = true;
            this.ugSimulazione.DisplayLayout.Override.SelectedAppearancesEnabled = Infragistics.Win.DefaultableBoolean.False;
            this.ugSimulazione.DisplayLayout.Override.ActiveAppearancesEnabled = Infragistics.Win.DefaultableBoolean.False;

            try
            {

                this.ugSimulazione.DataSource = null;
                if (this.ugEpisodioMatilde.ActiveRow != null)
                {
                    this.ugSimulazione.DataSource = lst_MovTrasferimenti.OrderBy(o => o.DataIngresso).ToList();
                }

                this.ugSimulazione.Refresh();
                this.ugSimulazione.Text = string.Format("{0} ({1:#,##0})", "Trasferimenti Matilde Simulazione", this.ugSimulazione.Rows.Count);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

        }

        private void LoadUltraGridCorrezione(List<T_MovTrasferimentiEx> lst_MovTrasferimenti)
        {

            this.Cursor = Cursors.WaitCursor;

            MyStatics.SetUltraGridLayout(ref this.ugCorrezione, false, false);
            this.ugCorrezione.DisplayLayout.GroupByBox.Hidden = true;
            this.ugCorrezione.DisplayLayout.Override.SelectedAppearancesEnabled = Infragistics.Win.DefaultableBoolean.False;
            this.ugCorrezione.DisplayLayout.Override.ActiveAppearancesEnabled = Infragistics.Win.DefaultableBoolean.False;
            this.ugCorrezione.DisplayLayout.Override.CellClickAction = CellClickAction.EditAndSelectText;
            this.ugCorrezione.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.True;
            this.ugCorrezione.DisplayLayout.Override.AllowDelete = DefaultableBoolean.True;
            this.ugCorrezione.AllowDrop = true;

            try
            {

                this.ugCorrezione.DataSource = null;
                if (this.ugEpisodioMatilde.ActiveRow != null)
                {
                    this.ugCorrezione.DataSource = lst_MovTrasferimenti.OrderBy(o => o.DataIngresso).ToList();
                }

                this.ugCorrezione.Refresh();
                this.ugCorrezione.Text = string.Format("{0} ({1:#,##0})", "Trasferimenti Matilde Correzione", this.ugCorrezione.Rows.Count);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

        }

        #endregion

        #region Subroutine

        private void Check()
        {

            this.Cursor = Cursors.WaitCursor;

            try
            {

                List<string> lst_response = new List<string>();

                if (this.ugEpisodi.ActiveRow != null)
                {

                    if (this.ugEpisodioMatilde.Rows.Count == 1 && this.ugEpisodioDWH.Rows.Count >= 1)
                    {

                        UltraGridRow ugrMatilde = this.ugEpisodioMatilde.Rows[0];
                        foreach (UltraGridRow ugrDWH in this.ugEpisodioDWH.Rows)
                        {

                            bool bTipoMovimento = (ugrDWH.Cells["Nosologico"].Text == ugrMatilde.Cells["NumeroNosologico"].Text ||
ugrDWH.Cells["Nosologico"].Text == ugrMatilde.Cells["NumeroListaAttesa"].Text);
                            ugrDWH.Cells["Nosologico"].Appearance.BackColor = getColorCheck(bTipoMovimento);
                            if (bTipoMovimento)
                            {

                                if (ugrDWH.Cells["Nosologico"].Text == ugrMatilde.Cells["NumeroNosologico"].Text)
                                {
                                    ugrMatilde.Cells["NumeroNosologico"].Appearance.BackColor = getColorCheck(true);
                                }
                                else if (ugrDWH.Cells["Nosologico"].Text == ugrMatilde.Cells["NumeroListaAttesa"].Text)
                                {
                                    ugrMatilde.Cells["NumeroListaAttesa"].Appearance.BackColor = getColorCheck(true);
                                }

                                bool bTipoEpisodio = (getTipoEpisodio(ugrMatilde.Cells["CodTipoEpisodio"].Text) == ugrDWH.Cells["DescTipoEpisodio"].Text);
                                ugrMatilde.Cells["CodTipoEpisodio"].Appearance.BackColor = getColorCheck(bTipoEpisodio);
                                ugrDWH.Cells["DescTipoEpisodio"].Appearance.BackColor = getColorCheck(bTipoEpisodio);

                                bool bAzienda = (ugrMatilde.Cells["CodAzi"].Text == ugrDWH.Cells["AziendaErogante"].Text);
                                ugrMatilde.Cells["CodAzi"].Appearance.BackColor = getColorCheck(bAzienda);
                                ugrDWH.Cells["AziendaErogante"].Appearance.BackColor = getColorCheck(bAzienda);

                            }
                            else
                            {
                                lst_response.Add("Episodio Matilde e Episodio DWH NON confrontabile(i)!!!");
                            }


                            if (ugrMatilde.Cells["NumeroNosologico"].Text == ugrDWH.Cells["Nosologico"].Text)
                            {

                                bool bDataRicovero = ((DateTime)ugrMatilde.Cells["DataRicovero"].Value == (DateTime)ugrDWH.Cells["DataInizioRicovero"].Value);
                                ugrMatilde.Cells["DataRicovero"].Appearance.BackColor = getColorCheck(bDataRicovero);
                                ugrDWH.Cells["DataInizioRicovero"].Appearance.BackColor = getColorCheck(bDataRicovero);

                                if (ugrMatilde.Cells["DataDimissione"].Text != string.Empty)
                                {
                                    bool bDataDimissione = ((DateTime)ugrMatilde.Cells["DataDimissione"].Value == (DateTime)ugrDWH.Cells["DataFineRicovero"].Value);
                                    ugrMatilde.Cells["DataDimissione"].Appearance.BackColor = getColorCheck(bDataDimissione);
                                    ugrDWH.Cells["DataFineRicovero"].Appearance.BackColor = getColorCheck(bDataDimissione);
                                }
                            }

                            if (ugrMatilde.Cells["NumeroListaAttesa"].Text == ugrDWH.Cells["Nosologico"].Text)
                            {
                                bool bDataOraListaAttesa = ((DateTime)ugrMatilde.Cells["DataListaAttesa"].Value == (DateTime)ugrDWH.Cells["DataInizioRicovero"].Value);
                                if (bDataOraListaAttesa == false)
                                {
                                    bool bDataListaAttesa = (((DateTime)ugrMatilde.Cells["DataListaAttesa"].Value).Date == ((DateTime)ugrDWH.Cells["DataInizioRicovero"].Value).Date);
                                    ugrMatilde.Cells["DataListaAttesa"].Appearance.BackColor = getColorCheck(bDataListaAttesa, ColorDateOk);
                                    ugrDWH.Cells["DataInizioRicovero"].Appearance.BackColor = getColorCheck(bDataListaAttesa, ColorDateOk);
                                }
                                else
                                {
                                    ugrMatilde.Cells["DataListaAttesa"].Appearance.BackColor = getColorCheck(bDataOraListaAttesa);
                                    ugrDWH.Cells["DataInizioRicovero"].Appearance.BackColor = getColorCheck(bDataOraListaAttesa);
                                }
                            }

                        }

                        List<UltraGridRow> ugrRowAT = this.ugTrasferimentiMatilde.Rows.Where<UltraGridRow>(r => r.Cells["CodStatoTrasferimento"].Value.ToString() == "AT").ToList();
                        List<UltraGridRow> ugrRowDM = this.ugTrasferimentiMatilde.Rows.Where<UltraGridRow>(r => r.Cells["CodStatoTrasferimento"].Value.ToString() == "DM").ToList();
                        List<UltraGridRow> ugrRowPA = this.ugTrasferimentiMatilde.Rows.Where<UltraGridRow>(r => r.Cells["CodStatoTrasferimento"].Value.ToString() == "PA").ToList();
                        List<UltraGridRow> ugrRowPC = this.ugTrasferimentiMatilde.Rows.Where<UltraGridRow>(r => r.Cells["CodStatoTrasferimento"].Value.ToString() == "PC").ToList();
                        List<UltraGridRow> ugrRowPR = this.ugTrasferimentiMatilde.Rows.Where<UltraGridRow>(r => r.Cells["CodStatoTrasferimento"].Value.ToString() == "PR").ToList();

                        if (ugrRowAT.Count > 1) { lst_response.Add("Stato Trasferimento 'AT' NON univoco!!!"); }
                        if (ugrRowDM.Count > 1) { lst_response.Add("Stato Trasferimento 'DM' NON univoco!!!"); }
                        if (ugrRowPA.Count > 1) { lst_response.Add("Stato Trasferimento 'PA' NON univoco!!!"); }
                        if (ugrRowPC.Count > 1) { lst_response.Add("Stato Trasferimento 'PC' NON univoco!!!"); }
                        if (ugrRowPR.Count > 1) { lst_response.Add("Stato Trasferimento 'PR' NON univoco!!!"); }
                        if (ugrRowAT.Count == 0 && ugrRowDM.Count == 0 && ugrRowPA.Count == 0 && ugrRowPC.Count == 0 && ugrRowPR.Count == 0)
                        {
                            lst_response.Add("Nessuno Stato (AT-DM-PA-PC-PR) presente!!!");
                        }

                        this.CheckTrasferimentiMatilde(this.ugTrasferimentiMatilde);

                        this.CheckTrasferimentiDWH(this.ugTrasferimentiMatilde);

                    }
                    else
                    {
                        lst_response.Add("Episodio Matilde e Episodio DWH NON paragonabili!!!");
                    }

                }
                else
                {
                    lst_response.Add("Selezionare un episodio!!!");
                }

                if (lst_response.Count > 0)
                {
                    MessageBox.Show(string.Join("\n", lst_response), "Check");
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

        }

        private void CheckTrasferimentiDWH(UltraGrid ug)
        {

            try
            {

                foreach (UltraGridRow ugrRowDWH in this.ugTrasferimentiDWH.Rows)
                {

                    UltraGridRow ugrRowMatilde = null;
                    bool bCheck = false;

                    switch (ugrRowDWH.Cells["CodTipoEvento"].Text.ToUpper())
                    {

                        case "IL":
                            break;

                        case "ML":
                            break;

                        case "A":
                            if (ugrRowDWH.Cells["CodReparto"].Value.ToString().Contains("SM") || ugrRowDWH.Cells["CodReparto"].Value.ToString().Contains("AU"))
                            {
                                ugrRowDWH.Appearance.FontData.Strikeout = DefaultableBoolean.True;
                            }
                            else
                            {
                                ugrRowMatilde = ug.Rows.FirstOrDefault(r => r.Cells["CodStatoTrasferimento"].Value.ToString() == "TR");
                                if (ugrRowMatilde != null)
                                {
                                    bCheck = ((DateTime)ugrRowMatilde.Cells["DataIngresso"].Value == (DateTime)ugrRowDWH.Cells["DataEvento"].Value
                                                && ugrRowMatilde.Cells["CodUO"].Value.ToString() == ugrRowDWH.Cells["CodReparto"].Value.ToString()
                                                && ugrRowMatilde.Cells["CodSettore"].Value.ToString() == ugrRowDWH.Cells["CodSettore"].Value.ToString()
                                                && ugrRowMatilde.Cells["CodLetto"].Value.ToString() == ugrRowDWH.Cells["CodLetto"].Value.ToString());
                                    ugrRowMatilde.Appearance.BackColor = getColorCheck(bCheck, ColorTrasferimentoAllineato);
                                    ugrRowDWH.Appearance.BackColor = getColorCheck(bCheck, ColorTrasferimentoAllineato);
                                    ugrRowMatilde.Cells["DataIngresso"].Appearance.BackColor = getColorCheck(true, ColorTrasferimentoCollegato);
                                }
                                else
                                {
                                    ugrRowMatilde = ug.Rows.FirstOrDefault(r => r.Cells["CodStatoTrasferimento"].Value.ToString() == "DM");
                                    if (ugrRowMatilde != null)
                                    {
                                        bCheck = ((DateTime)ugrRowMatilde.Cells["DataIngresso"].Value == (DateTime)ugrRowDWH.Cells["DataEvento"].Value
                                                    && ugrRowMatilde.Cells["CodUO"].Value.ToString() == ugrRowDWH.Cells["CodReparto"].Value.ToString()
                                                    && ugrRowMatilde.Cells["CodSettore"].Value.ToString() == ugrRowDWH.Cells["CodSettore"].Value.ToString()
                                                    && ugrRowMatilde.Cells["CodLetto"].Value.ToString() == ugrRowDWH.Cells["CodLetto"].Value.ToString());
                                        ugrRowMatilde.Appearance.BackColor = getColorCheck(bCheck, ColorTrasferimentoAllineato);
                                        ugrRowDWH.Appearance.BackColor = getColorCheck(bCheck, ColorTrasferimentoAllineato);
                                    }
                                }
                            }
                            break;

                        case "T":
                            ugrRowMatilde = ug.Rows.FirstOrDefault(r => (r.Cells["CodStatoTrasferimento"].Value.ToString() == "TR"
                || r.Cells["CodStatoTrasferimento"].Value.ToString() == "DM"
                || r.Cells["CodStatoTrasferimento"].Value.ToString() == "AT")
            && (DateTime)r.Cells["DataIngresso"].Value == (DateTime)ugrRowDWH.Cells["DataEvento"].Value);
                            if (ugrRowMatilde != null)
                            {
                                ugrRowMatilde.Appearance.BackColor = getColorCheck(true, ColorTrasferimentoAllineato);
                                ugrRowDWH.Appearance.BackColor = getColorCheck(true, ColorTrasferimentoAllineato);
                                ugrRowDWH.Cells["DataEvento"].Appearance.BackColor = getColorCheck(true, ColorTrasferimentoCollegato);
                                if (ugrRowMatilde.Cells["CodStatoTrasferimento"].Value.ToString() == "TR"
                                    || ugrRowMatilde.Cells["CodStatoTrasferimento"].Value.ToString() == "AT"
                                    || ugrRowMatilde.Cells["CodStatoTrasferimento"].Value.ToString() == "DM")
                                {
                                    ugrRowMatilde.Cells["DataIngresso"].Appearance.BackColor = getColorCheck(true, ColorTrasferimentoCollegato);
                                    ugrRowMatilde = ug.Rows.FirstOrDefault(r => (r.Cells["CodStatoTrasferimento"].Value.ToString() == "TR")
                                                                                                    && r.Cells["DataUscita"].Value != System.DBNull.Value
                                                                                                    && (DateTime)r.Cells["DataUscita"].Value == (DateTime)ugrRowDWH.Cells["DataEvento"].Value);
                                    if (ugrRowMatilde != null)
                                    {
                                        ugrRowMatilde.Cells["DataUscita"].Appearance.BackColor = getColorCheck(true, ColorTrasferimentoCollegato);
                                    }
                                }
                            }
                            break;

                        case "D":
                            UltraGridRow ugrRowDWH_TR = this.ugTrasferimentiDWH.Rows.LastOrDefault(r => r.Cells["CodTipoEvento"].Value.ToString() == "T");
                            if (ugrRowDWH_TR != null && ugrRowDWH_TR.Cells["AziendaErogante"].Value.ToString() != ugrRowDWH.Cells["AziendaErogante"].Value.ToString())
                            {
                                ugrRowDWH.Appearance.FontData.Strikeout = DefaultableBoolean.True;
                            }
                            else
                            {
                                ugrRowMatilde = ug.Rows.FirstOrDefault(r => r.Cells["CodStatoTrasferimento"].Value.ToString() == "DM");
                                if (ugrRowMatilde != null)
                                {
                                    bCheck = ((DateTime)ugrRowMatilde.Cells["DataUscita"].Value == (DateTime)ugrRowDWH.Cells["DataEvento"].Value);
                                    ugrRowMatilde.Appearance.BackColor = getColorCheck(bCheck, ColorTrasferimentoAllineato);
                                    ugrRowDWH.Appearance.BackColor = getColorCheck(bCheck, ColorTrasferimentoAllineato);
                                }
                            }
                            break;

                    }

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            finally
            {

            }

        }

        private void CheckTrasferimentiMatilde(UltraGrid ug)
        {

            try
            {

                foreach (UltraGridRow ugrTrasferimentoMatilde in ug.Rows)
                {

                    ugrTrasferimentoMatilde.Appearance.ResetBackColor();

                    UltraGridRow ugrRowDWH = null;
                    bool bCheck = false;

                    switch ((EnumStatoTrasferimento)Enum.Parse(typeof(EnumStatoTrasferimento), ugrTrasferimentoMatilde.Cells["CodStatoTrasferimento"].Value.ToString()))
                    {

                        case EnumStatoTrasferimento.DM:
                            ugrRowDWH = this.ugTrasferimentiDWH.Rows.LastOrDefault(r => r.Cells["CodTipoEvento"].Value.ToString() == "T");
                            if (ugrRowDWH == null)
                            {
                                ugrRowDWH = this.ugTrasferimentiDWH.Rows.FirstOrDefault(r => r.Cells["CodTipoEvento"].Value.ToString() == "A" && r.Appearance.FontData.Strikeout != DefaultableBoolean.True);
                            }
                            if (ugrRowDWH != null)
                            {
                                bCheck = ((DateTime)ugrTrasferimentoMatilde.Cells["DataIngresso"].Value == (DateTime)ugrRowDWH.Cells["DataEvento"].Value);
                                if (bCheck == false)
                                {
                                    bCheck = (((DateTime)ugrTrasferimentoMatilde.Cells["DataIngresso"].Value).Date == ((DateTime)ugrRowDWH.Cells["DataEvento"].Value).Date);
                                    ugrTrasferimentoMatilde.Cells["DataIngresso"].Appearance.BackColor = getColorCheck(bCheck, ColorDateOk);
                                    ugrRowDWH.Cells["DataEvento"].Appearance.BackColor = getColorCheck(bCheck, ColorDateOk);
                                }
                                else
                                {
                                    ugrTrasferimentoMatilde.Cells["DataIngresso"].Appearance.BackColor = getColorCheck(bCheck);
                                    ugrRowDWH.Cells["DataEvento"].Appearance.BackColor = getColorCheck(bCheck);
                                }
                            }
                            else
                            {
                                ugrTrasferimentoMatilde.Cells["DataIngresso"].Appearance.BackColor = getColorCheck(false);
                            }

                            ugrRowDWH = this.ugTrasferimentiDWH.Rows.FirstOrDefault(r => r.Cells["CodTipoEvento"].Value.ToString() == "D" && r.Appearance.FontData.Strikeout != DefaultableBoolean.True);
                            if (ugrRowDWH != null)
                            {
                                bCheck = ((DateTime)ugrTrasferimentoMatilde.Cells["DataUscita"].Value == (DateTime)ugrRowDWH.Cells["DataEvento"].Value);
                                ugrTrasferimentoMatilde.Cells["DataUscita"].Appearance.BackColor = getColorCheck(bCheck);
                                ugrRowDWH.Cells["DataEvento"].Appearance.BackColor = getColorCheck(bCheck);
                            }
                            else
                            {
                                ugrTrasferimentoMatilde.Cells["DataUscita"].Appearance.BackColor = getColorCheck(false);
                            }
                            break;

                        case EnumStatoTrasferimento.TR:
                            break;

                        case EnumStatoTrasferimento.AT:
                            ugrRowDWH = this.ugTrasferimentiDWH.Rows.LastOrDefault(r => r.Cells["CodTipoEvento"].Value.ToString() == "T");
                            if (ugrRowDWH == null)
                            {
                                ugrRowDWH = this.ugTrasferimentiDWH.Rows.FirstOrDefault(r => r.Cells["CodTipoEvento"].Value.ToString() == "A" && r.Appearance.FontData.Strikeout != DefaultableBoolean.True);
                            }
                            if (ugrRowDWH != null)
                            {
                                bCheck = ((DateTime)ugrTrasferimentoMatilde.Cells["DataIngresso"].Value == (DateTime)ugrRowDWH.Cells["DataEvento"].Value);
                                if (bCheck == false)
                                {
                                    bCheck = (((DateTime)ugrTrasferimentoMatilde.Cells["DataIngresso"].Value).Date == ((DateTime)ugrRowDWH.Cells["DataEvento"].Value).Date);
                                    ugrTrasferimentoMatilde.Cells["DataIngresso"].Appearance.BackColor = getColorCheck(bCheck, ColorDateOk);
                                    ugrRowDWH.Cells["DataEvento"].Appearance.BackColor = getColorCheck(bCheck, ColorDateOk);
                                }
                                else
                                {
                                    ugrTrasferimentoMatilde.Cells["DataIngresso"].Appearance.BackColor = getColorCheck(bCheck);
                                    ugrRowDWH.Cells["DataEvento"].Appearance.BackColor = getColorCheck(bCheck);
                                }
                            }
                            else
                            {
                                ugrTrasferimentoMatilde.Cells["DataIngresso"].Appearance.BackColor = getColorCheck(false);
                            }
                            break;

                        case EnumStatoTrasferimento.SS:
                            ugrTrasferimentoMatilde.Cells["CodStatoTrasferimento"].Appearance.BackColor = getColorCheck(true, ColorTrasferimentoSS);
                            break;

                        case EnumStatoTrasferimento.CA:
                            break;

                        case EnumStatoTrasferimento.PR:
                            ugrRowDWH = this.ugTrasferimentiDWH.Rows.FirstOrDefault(r => r.Cells["CodTipoEvento"].Value.ToString() == "IL");
                            if (ugrRowDWH != null)
                            {
                                bCheck = ((DateTime)ugrTrasferimentoMatilde.Cells["DataIngresso"].Value == (DateTime)ugrRowDWH.Cells["DataEvento"].Value);
                                if (bCheck == false)
                                {
                                    bCheck = (((DateTime)ugrTrasferimentoMatilde.Cells["DataIngresso"].Value).Date == ((DateTime)ugrRowDWH.Cells["DataEvento"].Value).Date);
                                    ugrTrasferimentoMatilde.Cells["DataIngresso"].Appearance.BackColor = getColorCheck(bCheck, ColorDateOk);
                                    ugrRowDWH.Cells["DataEvento"].Appearance.BackColor = getColorCheck(bCheck, ColorDateOk);
                                }
                                else
                                {
                                    ugrTrasferimentoMatilde.Cells["DataIngresso"].Appearance.BackColor = getColorCheck(bCheck);
                                    ugrRowDWH.Cells["DataEvento"].Appearance.BackColor = getColorCheck(bCheck);
                                }
                            }
                            else
                            {
                                ugrTrasferimentoMatilde.Cells["DataIngresso"].Appearance.BackColor = getColorCheck(false);
                            }
                            break;

                        case EnumStatoTrasferimento.PC:
                            ugrRowDWH = this.ugTrasferimentiDWH.Rows.FirstOrDefault(r => r.Cells["CodTipoEvento"].Value.ToString() == "IL");
                            if (ugrRowDWH != null)
                            {
                                bCheck = ((DateTime)ugrTrasferimentoMatilde.Cells["DataIngresso"].Value == (DateTime)ugrRowDWH.Cells["DataEvento"].Value);
                                if (bCheck == false)
                                {
                                    bCheck = (((DateTime)ugrTrasferimentoMatilde.Cells["DataIngresso"].Value).Date == ((DateTime)ugrRowDWH.Cells["DataEvento"].Value).Date);
                                    ugrTrasferimentoMatilde.Cells["DataIngresso"].Appearance.BackColor = getColorCheck(bCheck, ColorDateOk);
                                    ugrRowDWH.Cells["DataEvento"].Appearance.BackColor = getColorCheck(bCheck, ColorDateOk);
                                }
                                else
                                {
                                    ugrTrasferimentoMatilde.Cells["DataIngresso"].Appearance.BackColor = getColorCheck(bCheck);
                                    ugrRowDWH.Cells["DataEvento"].Appearance.BackColor = getColorCheck(bCheck);
                                }
                            }
                            else
                            {
                                ugrTrasferimentoMatilde.Cells["DataIngresso"].Appearance.BackColor = getColorCheck(false);
                            }
                            ugrRowDWH = this.ugTrasferimentiDWH.Rows.FirstOrDefault(r => r.Cells["CodTipoEvento"].Value.ToString() == "A" && r.Appearance.FontData.Strikeout != DefaultableBoolean.True);
                            if (ugrRowDWH != null)
                            {
                                bCheck = ((DateTime)ugrTrasferimentoMatilde.Cells["DataUscita"].Value == (DateTime)ugrRowDWH.Cells["DataEvento"].Value);
                                if (bCheck == false)
                                {
                                    bCheck = (((DateTime)ugrTrasferimentoMatilde.Cells["DataUscita"].Value).Date == ((DateTime)ugrRowDWH.Cells["DataEvento"].Value).Date);
                                    ugrTrasferimentoMatilde.Cells["DataUscita"].Appearance.BackColor = getColorCheck(bCheck, ColorDateOk);
                                    ugrRowDWH.Cells["DataEvento"].Appearance.BackColor = getColorCheck(bCheck, ColorDateOk);
                                }
                                else
                                {
                                    ugrTrasferimentoMatilde.Cells["DataUscita"].Appearance.BackColor = getColorCheck(bCheck);
                                    ugrRowDWH.Cells["DataEvento"].Appearance.BackColor = getColorCheck(bCheck);
                                }
                            }
                            else
                            {
                                ugrTrasferimentoMatilde.Cells["DataUscita"].Appearance.BackColor = getColorCheck(false);
                            }
                            break;

                        case EnumStatoTrasferimento.PT:
                            break;

                        case EnumStatoTrasferimento.PA:
                            ugrRowDWH = this.ugTrasferimentiDWH.Rows.FirstOrDefault(r => r.Cells["CodTipoEvento"].Value.ToString() == "IL");
                            if (ugrRowDWH != null)
                            {
                                bCheck = ((DateTime)ugrTrasferimentoMatilde.Cells["DataIngresso"].Value == (DateTime)ugrRowDWH.Cells["DataEvento"].Value);
                                if (bCheck == false)
                                {
                                    bCheck = (((DateTime)ugrTrasferimentoMatilde.Cells["DataIngresso"].Value).Date == ((DateTime)ugrRowDWH.Cells["DataEvento"].Value).Date);
                                    ugrTrasferimentoMatilde.Cells["DataIngresso"].Appearance.BackColor = getColorCheck(bCheck, ColorDateOk);
                                    ugrRowDWH.Cells["DataEvento"].Appearance.BackColor = getColorCheck(bCheck, ColorDateOk);
                                }
                                else
                                {
                                    ugrTrasferimentoMatilde.Cells["DataIngresso"].Appearance.BackColor = getColorCheck(bCheck);
                                    ugrRowDWH.Cells["DataEvento"].Appearance.BackColor = getColorCheck(bCheck);
                                }
                            }
                            else
                            {
                                ugrTrasferimentoMatilde.Cells["DataIngresso"].Appearance.BackColor = getColorCheck(false);
                            }
                            ugrRowDWH = this.ugTrasferimentiDWH.Rows.LastOrDefault(r => r.Cells["CodTipoEvento"].Value.ToString() == "ML");
                            if (ugrRowDWH != null)
                            {
                                bCheck = ((DateTime)ugrTrasferimentoMatilde.Cells["DataUscita"].Value == (DateTime)ugrRowDWH.Cells["DataEvento"].Value);
                                if (bCheck == false)
                                {
                                    bCheck = (((DateTime)ugrTrasferimentoMatilde.Cells["DataUscita"].Value).Date == ((DateTime)ugrRowDWH.Cells["DataEvento"].Value).Date);
                                    ugrTrasferimentoMatilde.Cells["DataUscita"].Appearance.BackColor = getColorCheck(bCheck, ColorDateOk);
                                    ugrRowDWH.Cells["DataEvento"].Appearance.BackColor = getColorCheck(bCheck, ColorDateOk);
                                }
                                else
                                {
                                    ugrTrasferimentoMatilde.Cells["DataUscita"].Appearance.BackColor = getColorCheck(bCheck);
                                    ugrRowDWH.Cells["DataEvento"].Appearance.BackColor = getColorCheck(bCheck);
                                }
                            }
                            else
                            {
                                ugrTrasferimentoMatilde.Cells["DataUscita"].Appearance.BackColor = getColorCheck(false);
                            }
                            break;

                    }

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            finally
            {

            }

        }

        private string getTipoEpisodio(string codtipoepisodio)
        {

            switch (codtipoepisodio.ToUpper())
            {

                case "DS":
                    return "Day Service";

                case "RO":
                    return "Ordinario";

                case "PS":
                    return "Pronto Soccorso";

                case "DH":
                    return "Day Hospital";

                case "OB":
                    return "OBI";

                default:
                    return "NON CONFRONTABILE";

            }

        }

        private Color getColorCheck(bool bChechOk)
        {
            return getColorCheck(bChechOk, ColorAllOk, ColorError);
        }
        private Color getColorCheck(bool bChechOk, Color colorallok)
        {
            return getColorCheck(bChechOk, colorallok, ColorError);
        }
        private Color getColorCheck(bool bChechOk, Color colorallok, Color colorerror)
        {
            return (bChechOk ? colorallok : colorerror);
        }

        private void Simula()
        {

            this.Cursor = Cursors.WaitCursor;

            try
            {

                List<T_MovTrasferimentiEx> lst_MovTrasferimenti = new List<T_MovTrasferimentiEx>();

                foreach (UltraGridRow ugrRowDWH in this.ugTrasferimentiDWH.Rows)
                {


                    T_MovTrasferimentiEx oTrasferimento = null;

                    switch (ugrRowDWH.Cells["CodTipoEvento"].Text.ToUpper())
                    {

                        case "IL":
                            oTrasferimento = lst_MovTrasferimenti.FirstOrDefault(r => r.CodStatoTrasferimento == "TR");
                            if (oTrasferimento == null)
                            {
                                using (oTrasferimento = getMovTrasferimenti(ugrRowDWH))
                                {
                                    oTrasferimento.IDEpisodio = Guid.Parse(this.ugEpisodi.ActiveRow.Cells["ID"].Text);
                                    oTrasferimento.CodStatoTrasferimento = "PR";
                                    lst_MovTrasferimenti.Add(oTrasferimento);
                                }
                            }
                            break;

                        case "ML":
                            oTrasferimento = lst_MovTrasferimenti.FirstOrDefault(r => r.CodStatoTrasferimento == "PR");
                            if (oTrasferimento != null)
                            {
                                oTrasferimento.IDEpisodio = Guid.Parse(this.ugEpisodi.ActiveRow.Cells["ID"].Text);
                                oTrasferimento.CodUO = ugrRowDWH.Cells["CodReparto"].Text;
                                oTrasferimento.DescrUO = ugrRowDWH.Cells["DescReparto"].Text;
                                oTrasferimento.CodSettore = ugrRowDWH.Cells["CodSettore"].Text;
                                oTrasferimento.DescrSettore = ugrRowDWH.Cells["DescSettore"].Text;
                                oTrasferimento.CodLetto = ugrRowDWH.Cells["CodLetto"].Text;
                                oTrasferimento.DescrLetto = ugrRowDWH.Cells["DescLetto"].Text;
                                oTrasferimento.CodUA = DecodificaUnitaAtomica(oTrasferimento.CodAziTrasferimento, oTrasferimento.CodUO, oTrasferimento.CodSettore, oTrasferimento.CodLetto);
                            }
                            break;

                        case "A":
                            if (ugrRowDWH.Appearance.FontData.Strikeout != DefaultableBoolean.True)
                            {
                                oTrasferimento = lst_MovTrasferimenti.FirstOrDefault(r => r.CodStatoTrasferimento == "PR");
                                if (oTrasferimento == null)
                                {
                                    using (oTrasferimento = getMovTrasferimenti(ugrRowDWH))
                                    {
                                        oTrasferimento.IDEpisodio = Guid.Parse(this.ugEpisodi.ActiveRow.Cells["ID"].Text);
                                        oTrasferimento.CodStatoTrasferimento = "AT";
                                        lst_MovTrasferimenti.Add(oTrasferimento);
                                    }
                                }
                                else
                                {
                                    oTrasferimento.IDEpisodio = Guid.Parse(this.ugEpisodi.ActiveRow.Cells["ID"].Text);
                                    oTrasferimento.CodStatoTrasferimento = "PC";
                                    oTrasferimento.DataUscita = (DateTime)ugrRowDWH.Cells["DataEvento"].Value;
                                    oTrasferimento.DataUscitaUTC = ((DateTime)ugrRowDWH.Cells["DataEvento"].Value).ToUniversalTime();
                                    using (oTrasferimento = getMovTrasferimenti(ugrRowDWH))
                                    {
                                        oTrasferimento.CodStatoTrasferimento = "AT";
                                        lst_MovTrasferimenti.Add(oTrasferimento);
                                    }
                                }
                            }
                            break;

                        case "T":
                            oTrasferimento = lst_MovTrasferimenti.FirstOrDefault(r => r.CodStatoTrasferimento == "AT");
                            if (oTrasferimento != null)
                            {
                                oTrasferimento.IDEpisodio = Guid.Parse(this.ugEpisodi.ActiveRow.Cells["ID"].Text);
                                oTrasferimento.CodStatoTrasferimento = "TR";
                                oTrasferimento.DataUscita = (DateTime)ugrRowDWH.Cells["DataEvento"].Value;
                                oTrasferimento.DataUscitaUTC = ((DateTime)ugrRowDWH.Cells["DataEvento"].Value).ToUniversalTime();
                                using (oTrasferimento = getMovTrasferimenti(ugrRowDWH))
                                {
                                    oTrasferimento.IDEpisodio = Guid.Parse(this.ugEpisodi.ActiveRow.Cells["ID"].Text);
                                    oTrasferimento.CodStatoTrasferimento = "AT";
                                    lst_MovTrasferimenti.Add(oTrasferimento);
                                }
                            }
                            break;

                        case "D":
                            if (ugrRowDWH.Appearance.FontData.Strikeout != DefaultableBoolean.True)
                            {
                                oTrasferimento = lst_MovTrasferimenti.FirstOrDefault(r => r.CodStatoTrasferimento == "AT");
                                if (oTrasferimento != null)
                                {
                                    oTrasferimento.IDEpisodio = Guid.Parse(this.ugEpisodi.ActiveRow.Cells["ID"].Text);
                                    oTrasferimento.CodStatoTrasferimento = "DM";
                                    oTrasferimento.DataUscita = (DateTime)ugrRowDWH.Cells["DataEvento"].Value;
                                    oTrasferimento.DataUscitaUTC = ((DateTime)ugrRowDWH.Cells["DataEvento"].Value).ToUniversalTime();
                                }
                            }
                            break;

                    }

                }

                this.LoadUltraGridSimulazione(lst_MovTrasferimenti);

                this.CheckTrasferimentiMatilde(this.ugSimulazione);
                this.CheckTrasferimentiDWH(this.ugSimulazione);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            finally
            {
                if (this.UltraSplitterSimulazione.Collapsed) { this.UltraSplitterSimulazione.Collapsed = false; }
                this.Cursor = Cursors.Default;
            }

        }

        private T_MovTrasferimentiEx getMovTrasferimenti(UltraGridRow ugrRowDWH)
        {

            T_MovTrasferimentiEx oTrasferimento = new T_MovTrasferimentiEx();

            try
            {

                oTrasferimento.Sequenza = 0;
                oTrasferimento.DataIngresso = (DateTime)ugrRowDWH.Cells["DataEvento"].Value;
                oTrasferimento.DataIngressoUTC = ((DateTime)ugrRowDWH.Cells["DataEvento"].Value).ToUniversalTime();
                oTrasferimento.CodUO = ugrRowDWH.Cells["CodReparto"].Text;
                oTrasferimento.DescrUO = ugrRowDWH.Cells["DescReparto"].Text;
                oTrasferimento.CodSettore = ugrRowDWH.Cells["CodSettore"].Text;
                oTrasferimento.DescrSettore = ugrRowDWH.Cells["DescSettore"].Text;
                oTrasferimento.CodLetto = ugrRowDWH.Cells["CodLetto"].Text;
                oTrasferimento.DescrLetto = $"Letto {ugrRowDWH.Cells["DescLetto"].Text}";
                oTrasferimento.CodAziTrasferimento = ugrRowDWH.Cells["AziendaErogante"].Text;
                oTrasferimento.CodUA = DecodificaUnitaAtomica(oTrasferimento.CodAziTrasferimento, oTrasferimento.CodUO, oTrasferimento.CodSettore, oTrasferimento.CodLetto);

            }
            catch (Exception)
            {

            }

            return oTrasferimento;

        }

        private string DecodificaUnitaAtomica(string CodAzi, string CodUO, string CodSettore, string CodLetto)
        {

            string sSql = string.Empty;
            DataSet oDs = null;
            string sRet = string.Empty;
            DataRow[] oDrFilter = null;

            string sCodSettore = (CodSettore == null ? "" : CodSettore);
            string sCodLetto = (CodLetto == null ? "" : CodLetto);

            try
            {
                sSql = @"Select * From T_AssUAUOLetti Where CodAzi = '" + CodAzi + "' AND CodUO = '" + CodUO + "'";
                oDs = DataBase.GetDataSet(sSql);

                if (oDs != null && oDs.Tables[0].Rows.Count > 0)
                {
                    if (sCodSettore == string.Empty && sCodLetto == string.Empty)
                    {
                        oDrFilter = oDs.Tables[0].Select("CodSettore = '*' AND CodLetto = '*'");
                        if (oDrFilter.Length > 0)
                        {
                            sRet = oDrFilter[0]["CodUA"].ToString();
                        }
                        else
                            sRet = string.Empty;
                    }

                    if (sCodSettore != string.Empty && sCodLetto == string.Empty)
                    {
                        oDrFilter = oDs.Tables[0].Select("CodSettore = '" + sCodSettore + "' AND CodLetto = '*'");
                        if (oDrFilter.Length > 0)
                        {
                            sRet = oDrFilter[0]["CodUA"].ToString();
                        }
                        else
                        {
                            oDrFilter = oDs.Tables[0].Select("CodSettore = '*' AND CodLetto = '*'");
                            if (oDrFilter.Length > 0)
                            {
                                sRet = oDrFilter[0]["CodUA"].ToString();
                            }
                            else
                                sRet = string.Empty;
                        }
                    }

                    if (sCodSettore == string.Empty && sCodLetto != string.Empty)
                    {
                        oDrFilter = oDs.Tables[0].Select("CodSettore = '*' AND CodLetto = '" + sCodLetto + "'");
                        if (oDrFilter.Length > 0)
                        {
                            sRet = oDrFilter[0]["CodUA"].ToString();
                        }
                        else
                        {
                            oDrFilter = oDs.Tables[0].Select("CodSettore = '*' AND CodLetto = '*'");
                            if (oDrFilter.Length > 0)
                            {
                                sRet = oDrFilter[0]["CodUA"].ToString();
                            }
                            else
                                sRet = string.Empty;
                        }
                    }

                    if (sCodSettore != string.Empty && sCodLetto != string.Empty)
                    {
                        oDrFilter = oDs.Tables[0].Select("CodSettore = '" + sCodSettore + "' AND CodLetto = '" + sCodLetto + "'");
                        if (oDrFilter.Length > 0)
                        {
                            sRet = oDrFilter[0]["CodUA"].ToString();
                        }
                        else
                        {
                            oDrFilter = oDs.Tables[0].Select("CodSettore = '*' AND CodLetto = '" + sCodLetto + "'");
                            if (oDrFilter.Length > 0)
                            {
                                sRet = oDrFilter[0]["CodUA"].ToString();
                            }
                            else
                            {
                                oDrFilter = oDs.Tables[0].Select("CodSettore = '" + sCodSettore + "' AND CodLetto = '*'");
                                if (oDrFilter.Length > 0)
                                {
                                    sRet = oDrFilter[0]["CodUA"].ToString();
                                }
                                else
                                {
                                    oDrFilter = oDs.Tables[0].Select("CodSettore = '*' AND CodLetto = '*'");
                                    if (oDrFilter.Length > 0)
                                    {
                                        sRet = oDrFilter[0]["CodUA"].ToString();
                                    }
                                    else
                                        sRet = string.Empty;
                                }
                            }
                        }
                    }
                }
                else
                {
                    sSql = @"Select * From T_AssUAUOLetti Where CodAzi = '" + CodAzi + "' AND CodUO = '*' AND CodSettore = '*' AND CodLetto = '*'";
                    oDs = oDs = DataBase.GetDataSet(sSql);

                    if (oDs != null && oDs.Tables[0].Rows.Count > 0)
                    {
                        sRet = oDs.Tables[0].Rows[0]["CodUA"].ToString();
                    }
                    else
                        sRet = string.Empty;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                sRet = string.Empty;
            }

            if (oDs != null)
            {
                oDs.Dispose();
                oDs = null;
            }

            return sRet;

        }

        private void Collegamenti()
        {

            this.Cursor = Cursors.WaitCursor;

            try
            {

                List<T_MovTrasferimentiEx> lst_TrasferimentiMatilde = this.ugTrasferimentiMatilde.DataSource as List<T_MovTrasferimentiEx>;
                List<T_MovTrasferimentiEx> lst_Simulazione = this.ugSimulazione.DataSource as List<T_MovTrasferimentiEx>;

                int nLink = 0;
                foreach (T_MovTrasferimentiEx oMovSimulazione in lst_Simulazione)
                {

                    nLink++;
                    oMovSimulazione.Link = nLink;
                    T_MovTrasferimentiEx oMovMatilde = lst_TrasferimentiMatilde.FirstOrDefault(r =>
                                                                            r.CodStatoTrasferimento == oMovSimulazione.CodStatoTrasferimento
                                                                            && (r.DataIngresso == oMovSimulazione.DataIngresso || r.DataIngresso.Value.Date == oMovSimulazione.DataIngresso.Value.Date)
                                                                            && (r.DataUscita == oMovSimulazione.DataUscita || r.DataUscita.Value.Date == oMovSimulazione.DataUscita.Value.Date)
                                                                            && r.CodUO == oMovSimulazione.CodUO
                                                                            && r.CodUA == oMovSimulazione.CodUA
                                                                            && (r.CodSettore ?? string.Empty) == (oMovSimulazione.CodSettore ?? string.Empty)
                                                                            && (r.CodLetto ?? string.Empty) == (oMovSimulazione.CodLetto ?? string.Empty)
                                                                            && r.CodAziTrasferimento == oMovSimulazione.CodAziTrasferimento);
                    if (oMovMatilde != null) { oMovMatilde.Link = nLink; }

                }

                this.ugSimulazione.DataBind();
                this.ugTrasferimentiMatilde.DataBind();

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            finally
            {
                if (this.UltraSplitterCorrezione.Collapsed) { this.UltraSplitterCorrezione.Collapsed = false; }
                this.Cursor = Cursors.Default;
            }
        }

        private void Correzione()
        {

            this.Cursor = Cursors.WaitCursor;

            try
            {

                List<T_MovTrasferimentiEx> lst_MovTrasferimenti = new List<T_MovTrasferimentiEx>();
                List<T_MovTrasferimentiEx> lst_Matilde = this.ugTrasferimentiMatilde.DataSource as List<T_MovTrasferimentiEx>;
                List<T_MovTrasferimentiEx> lst_Simulazione = this.ugSimulazione.DataSource as List<T_MovTrasferimentiEx>;

                foreach (T_MovTrasferimentiEx oMovSimulazione in lst_Simulazione)
                {
                    T_MovTrasferimentiEx oMovMatilde = lst_Matilde.LastOrDefault(r => r.Link == oMovSimulazione.Link);
                    if (oMovMatilde != null)
                    {
                        T_MovTrasferimentiEx oMovCorrezione = getMovTrasferimentoCorretto(oMovSimulazione, oMovMatilde);
                        lst_MovTrasferimenti.Add(oMovCorrezione);
                    }
                }

                foreach (T_MovTrasferimentiEx oMovMatilde in lst_Matilde)
                {
                    if (oMovMatilde.Link == 0)
                    {
                        T_MovTrasferimentiEx oMovCorrezione = getMovTrasferimentoCorretto(oMovMatilde, oMovMatilde);
                        lst_MovTrasferimenti.Add(oMovCorrezione);
                    }
                }


                this.LoadUltraGridCorrezione(lst_MovTrasferimenti);

                this.CheckTrasferimentiMatilde(this.ugCorrezione);

                this.CheckTrasferimentiDWH(this.ugCorrezione);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            finally
            {
                if (this.UltraSplitterCorrezione.Collapsed) { this.UltraSplitterCorrezione.Collapsed = false; }
                this.Cursor = Cursors.Default;
            }

        }

        private T_MovTrasferimentiEx getMovTrasferimentoCorretto(T_MovTrasferimentiEx oSimulazione, T_MovTrasferimentiEx oMatilde)
        {

            T_MovTrasferimentiEx oTrasferimento = new T_MovTrasferimentiEx();

            try
            {

                oTrasferimento.Link = oSimulazione.Link;
                oTrasferimento.ID = oMatilde.ID;
                oTrasferimento.IDEpisodio = oMatilde.IDEpisodio;
                oTrasferimento.Sequenza = oMatilde.Sequenza;
                oTrasferimento.CodStatoTrasferimento = oSimulazione.CodStatoTrasferimento;
                oTrasferimento.DataIngresso = oSimulazione.DataIngresso;
                oTrasferimento.DataIngressoUTC = oSimulazione.DataIngressoUTC;
                oTrasferimento.DataUscita = oSimulazione.DataUscita;
                oTrasferimento.DataUscitaUTC = oSimulazione.DataUscitaUTC;
                oTrasferimento.CodUO = oSimulazione.CodUO;
                oTrasferimento.DescrUO = oSimulazione.DescrUO;
                oTrasferimento.CodSettore = oSimulazione.CodSettore;
                oTrasferimento.DescrSettore = oSimulazione.DescrSettore;
                oTrasferimento.CodStanza = oMatilde.CodStanza;
                oTrasferimento.DescrStanza = oMatilde.DescrStanza;
                oTrasferimento.CodLetto = oSimulazione.CodLetto;
                oTrasferimento.DescrLetto = oSimulazione.DescrLetto;
                oTrasferimento.IDCartella = oMatilde.IDCartella;
                oTrasferimento.CodAziTrasferimento = oSimulazione.CodAziTrasferimento;
                oTrasferimento.CodUA = oSimulazione.CodUA;

            }
            catch (Exception)
            {

            }

            return oTrasferimento;

        }

        private ValueList getStatoTrasferimentoVl()
        {

            ValueList oVl = new ValueList();

            try
            {

                using (FwDataConnection conn = new FwDataConnection(Database.ConnectionString))
                {
                    DataTable dt = conn.Query<DataTable>("Select Codice, Descrizione From T_StatoTrasferimento Order By Ordine");
                    foreach (DataRow dr in dt.Rows)
                    {
                        oVl.ValueListItems.Add(dr[0].ToString(), dr[0].ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            return oVl;

        }

        private void Correggi()
        {

            this.Cursor = Cursors.WaitCursor;

            try
            {

                if (this.ugCorrezione.Rows.Count > 0)
                {

                    List<T_MovTrasferimentiEx> lst_Matilde = this.ugTrasferimentiMatilde.DataSource as List<T_MovTrasferimentiEx>;
                    List<T_MovTrasferimentiEx> lst_Correzione = this.ugCorrezione.DataSource as List<T_MovTrasferimentiEx>;

                    List<T_MovTrasferimentiEx> lst_diff = lst_Correzione.Except(lst_Matilde, new T_MovTrasferimentiExComparer()).ToList();
                    if (lst_diff.Count > 0)
                    {

                        if (MessageBox.Show($"Vuoi applicare {lst_diff.Count} correzione(i)?", "Correzione",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        {

                            using (FwDataConnection conn = new FwDataConnection(Database.ConnectionString))
                            {

                                string ssql = $"Select * " +
                                                $"From T_MovTrasferimenti " +
                                                $"Where IDEpisodio = '{this.ugEpisodioMatilde.ActiveRow.Cells["ID"].Text}'";
                                DataSet oDsPrima = conn.Query<DataSet>(ssql, null, CommandType.Text);

                                conn.Open();
                                IDbTransaction oIDbTransaction = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                                try
                                {

                                    foreach (T_MovTrasferimentiEx oCorrezione in lst_diff)
                                    {
                                        if (oCorrezione.ID != Guid.Empty)
                                        {
                                            oCorrezione.Update(oIDbTransaction);
                                        }
                                        else
                                        {
                                            oCorrezione.ID = Guid.Parse(Database.GetNewID());
                                            oCorrezione.Insert(oIDbTransaction);
                                        }
                                    }
                                    oIDbTransaction.Commit();

                                    DataSet oDsDopo = conn.Query<DataSet>(ssql, null, CommandType.Text);
                                    MyStatics.LogManager(Enums.EnumModalityPopUp.mpModifica, Enums.EnumEntitaLog.T_MovTrasferimenti, oDsPrima, oDsDopo);

                                    MessageBox.Show($"Correzione completata.", "Correzione", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    this.LoadUltraGrid();

                                }
                                catch (Exception exTransaction)
                                {
                                    oIDbTransaction.Rollback();
                                    MessageBox.Show($"Errore nella correzione!!!\n\nErrore:\n{exTransaction.Message}", "Correzione", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                finally
                                {
                                    if (oIDbTransaction != null) oIDbTransaction.Dispose();
                                    conn.Close();
                                }

                            }

                        }

                    }
                    else
                    {
                        MessageBox.Show($"Nessuna riga da correggere!!!", "Correzione", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                }
                else
                {
                    MessageBox.Show($"Nessuna correzione applicabile!!!", "Correzione", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

        }

        #endregion

        #region Metodi Pubblici

        public override void Refresh()
        {
            this.UltraPanelSX.Width = (this.ugbDati.Width / 2 - this.UltraSplitterDati.Width / 2);
            this.LoadUltraGrid();
            base.Refresh();
        }

        #endregion

        #region Events

        private void ubRicerca_Click(object sender, EventArgs e)
        {
#if DEBUG
            if (this.uteIDEpisodio.Text == string.Empty && this.uteNosologico.Text == string.Empty && this.uteListaAttesa.Text == string.Empty)
            {
                this.uteIDEpisodio.Text = this.uteIDEpisodio.Tag.ToString();
            }
#endif
            this.LoadUltraGrid();
        }

        private void ubCheck_Click(object sender, EventArgs e)
        {
            this.Check();
        }

        private void ubSimula_Click(object sender, EventArgs e)
        {

            bool bCheck = true;

            if (this.ugSimulazione.Rows.Count > 0 || this.ugCorrezione.Rows.Count > 0)
            {
                if (MessageBox.Show($"ATTENZIONE: Tutti i dati di simulazione e correzione saranno ricalcolati.\n\nVuoi continuare?", "Simulazione",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    bCheck = false;
                }
            }
            if (bCheck)
            {
                this.Simula();
                this.Collegamenti();
                this.Correzione();
            }

        }

        private void ubCorreggi_Click(object sender, EventArgs e)
        {
            this.Correggi();
        }

        private void ugEpisodi_AfterRowActivate(object sender, EventArgs e)
        {
            this.LoadPaziente();
            this.LoadUltraGridEpisodioMatilde();
            this.LoadUltraGridTrasferimentiMatilde();
            this.LoadUltraGridEpisodioDWH();
            this.LoadUltraGridTrasferimentiDWH();
        }

        private void ugEpisodi_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {

                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {

                    try
                    {

                        oCol.SortIndicator = SortIndicator.Disabled;
                        switch (oCol.Key)
                        {

                            case "ID":
                            case "IDNum":
                            case "DataListaAttesaUTC":
                            case "DataRicoveroUTC":
                            case "DataDimissioneUTC":
                            case "DataAnnullamentoListaAttesaUTC":
                                oCol.Hidden = true;
                                break;

                            case "DataListaAttesa":
                            case "DataRicovero":
                            case "DataDimissione":
                            case "DataAnnullamentoListaAttesa":
                                oCol.Hidden = false;
                                oCol.Format = @"dd/MM/yyyy HH:mm:ss";
                                oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                                break;

                            default:
                                oCol.Hidden = false;
                                break;

                        }

                    }
                    catch (Exception)
                    {
                    }

                }

            }
            catch (Exception)
            {
            }

        }

        private void ugEpisodioMatilde_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {

                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {

                    try
                    {

                        oCol.SortIndicator = SortIndicator.Disabled;
                        switch (oCol.Key)
                        {

                            case "ID":
                            case "IDNum":
                            case "DataListaAttesaUTC":
                            case "DataRicoveroUTC":
                            case "DataDimissioneUTC":
                            case "DataAnnullamentoListaAttesaUTC":
                                oCol.Hidden = true;
                                break;

                            case "DataListaAttesa":
                            case "DataRicovero":
                            case "DataDimissione":
                            case "DataAnnullamentoListaAttesa":
                                oCol.Hidden = false;
                                oCol.Format = @"dd/MM/yyyy HH:mm:ss";
                                oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                                break;

                            default:
                                oCol.Hidden = false;
                                break;

                        }

                    }
                    catch (Exception)
                    {
                    }

                }

            }
            catch (Exception)
            {
            }

        }

        private void ugTrasferimentiMatilde_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {

                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {

                    try
                    {

                        oCol.SortIndicator = SortIndicator.Disabled;
                        switch (oCol.Key)
                        {

                            case "ID":
                            case "IDNum":
                            case "Sequenza":
                            case "IDEpisodio":
                            case "DataIngressoUTC":
                            case "DataUscitaUTC":
                                oCol.Hidden = true;
                                break;

                            case "DataIngresso":
                            case "DataUscita":
                                oCol.Hidden = false;
                                oCol.Format = @"dd/MM/yyyy HH:mm:ss";
                                oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                                break;

                            default:
                                oCol.Hidden = false;
                                break;

                        }

                    }
                    catch (Exception)
                    {
                    }

                }

            }
            catch (Exception)
            {
            }

        }

        private void ugEpisodioDWH_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {

                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {

                    try
                    {

                        oCol.SortIndicator = SortIndicator.Disabled;
                        switch (oCol.Key)
                        {

                            case "IDRicovero":
                            case "IDPaziente":
                                oCol.Hidden = true;
                                break;

                            case "DataInizioRicovero":
                            case "DataFineRicovero":
                                oCol.Hidden = false;
                                oCol.Format = @"dd/MM/yyyy HH:mm:ss";
                                oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                                break;

                            default:
                                oCol.Hidden = false;
                                break;

                        }

                    }
                    catch (Exception)
                    {
                    }

                }

            }
            catch (Exception)
            {
            }

        }

        private void ugTrasferimentiDWH_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {

                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {

                    try
                    {

                        oCol.SortIndicator = SortIndicator.Disabled;
                        switch (oCol.Key)
                        {

                            case "IDEvento":
                            case "RepartoErogante":
                            case "Diagnosi":
                                oCol.Hidden = true;
                                break;

                            case "DataEvento":
                                oCol.Hidden = false;
                                oCol.Format = @"dd/MM/yyyy HH:mm:ss";
                                oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                                break;

                            default:
                                oCol.Hidden = false;
                                break;

                        }

                    }
                    catch (Exception)
                    {
                    }

                }

            }
            catch (Exception)
            {
            }

        }

        private void ugSimulazione_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {

                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {

                    try
                    {

                        oCol.SortIndicator = SortIndicator.Disabled;
                        switch (oCol.Key)
                        {

                            case "ID":
                            case "IDNum":
                            case "Sequenza":
                            case "IDEpisodio":
                            case "DataIngressoUTC":
                            case "DataUscitaUTC":
                                oCol.Hidden = true;
                                break;

                            case "DataIngresso":
                            case "DataUscita":
                                oCol.Hidden = false;
                                oCol.Format = @"dd/MM/yyyy HH:mm:ss";
                                oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                                break;

                            default:
                                oCol.Hidden = false;
                                break;

                        }

                    }
                    catch (Exception)
                    {
                    }

                }

            }
            catch (Exception)
            {
            }

        }

        private void ugCorrezione_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
        {

            e.DisplayPromptMsg = false;

            if (e.Rows.Length > 0 && (int)e.Rows[0].Cells["Link"].Value == 0)
            {
                if (MessageBox.Show("Vuoi cancellare la riga selezionata?", "Cancellazione", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
            else
            {
                e.Cancel = true;
            }

        }

        private void ugCorrezione_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {

                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {

                    try
                    {

                        oCol.SortIndicator = SortIndicator.Disabled;
                        switch (oCol.Key)
                        {

                            case "ID":
                            case "IDNum":
                            case "Sequenza":
                            case "IDEpisodio":
                            case "DataIngressoUTC":
                            case "DataUscitaUTC":
                                oCol.Hidden = true;
                                break;

                            case "CodStatoTrasferimento":
                                oCol.Hidden = false;
                                oCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDown;
                                oCol.ValueList = getStatoTrasferimentoVl();
                                break;

                            case "DataIngresso":
                            case "DataUscita":
                                oCol.Hidden = false;
                                oCol.Format = @"dd/MM/yyyy HH:mm:ss";
                                oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                                oCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DateTime;
                                break;

                            default:
                                oCol.Hidden = false;
                                break;

                        }

                    }
                    catch (Exception)
                    {
                    }

                }

            }
            catch (Exception)
            {
            }

        }

        #endregion

        #region Events Drag & Drop

        private void ugSimulazione_SelectionDrag(object sender, CancelEventArgs e)
        {

            try
            {

                UltraGrid oUg = sender as UltraGrid;
                if (oUg.Selected.Rows.Count == 1)
                {
                    oUg.DoDragDrop(oUg.Selected.Rows, DragDropEffects.Copy);
                }

            }
            catch (Exception)
            {

            }

        }

        private void ugCorrezione_DragOver(object sender, DragEventArgs e)
        {

            try
            {

                if (e.Data.GetDataPresent(typeof(SelectedRowsCollection)) == true)
                {

                    SelectedRowsCollection oSourceNodes = e.Data.GetData(typeof(SelectedRowsCollection)) as SelectedRowsCollection;
                    if (oSourceNodes.Count == 0)
                    {
                        e.Effect = DragDropEffects.None;
                    }
                    else
                    {
                        e.Effect = DragDropEffects.Copy;
                    }

                }

            }
            catch (Exception)
            {

            }

        }

        private void ugCorrezione_DragDrop(object sender, DragEventArgs e)
        {

            try
            {

                if (e.Data.GetDataPresent(typeof(SelectedRowsCollection)) == true)
                {

                    SelectedRowsCollection oSourceNodes = e.Data.GetData(typeof(SelectedRowsCollection)) as SelectedRowsCollection;

                    UltraGrid oUg = sender as UltraGrid;
                    Point PointInGrid = oUg.PointToClient(new Point(e.X, e.Y));
                    UIElement element = oUg.DisplayLayout.UIElement.ElementFromPoint(PointInGrid);
                    UltraGridCell cell = element.GetContext(typeof(UltraGridCell)) as UltraGridCell;
                    if (cell != null)
                    {

                        if (MessageBox.Show($"Vuoi sostiture la riga selezionata della simulazione con questa?", "Sostituzione",
                                            MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        {

                            T_MovTrasferimentiEx oMovSource = oSourceNodes[0].ListObject as T_MovTrasferimentiEx;
                            T_MovTrasferimentiEx oMovCorrezione = cell.Row.ListObject as T_MovTrasferimentiEx;

                            oMovCorrezione.CodStatoTrasferimento = oMovSource.CodStatoTrasferimento;
                            oMovCorrezione.DataIngresso = oMovSource.DataIngresso;
                            oMovCorrezione.DataIngressoUTC = oMovSource.DataIngressoUTC;
                            oMovCorrezione.DataUscita = oMovSource.DataUscita;
                            oMovCorrezione.DataUscitaUTC = oMovSource.DataUscitaUTC;
                            oMovCorrezione.CodUO = oMovSource.CodUO;
                            oMovCorrezione.DescrUO = oMovSource.DescrUO;
                            oMovCorrezione.CodSettore = oMovSource.CodSettore;
                            oMovCorrezione.DescrSettore = oMovSource.DescrSettore;
                            oMovCorrezione.CodStanza = oMovSource.CodStanza;
                            oMovCorrezione.DescrStanza = oMovSource.DescrStanza;
                            oMovCorrezione.CodLetto = oMovSource.CodLetto;
                            oMovCorrezione.DescrLetto = oMovSource.DescrLetto;
                            oMovCorrezione.CodAziTrasferimento = oMovSource.CodAziTrasferimento;
                            oMovCorrezione.CodUA = oMovSource.CodUA;

                        }

                    }
                    else
                    {

                        if (MessageBox.Show($"Vuoi inserire la riga selezionata della simulazione nella correzione?", "Inserimento",
                                            MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        {

                            T_MovTrasferimentiEx oMovSource = oSourceNodes[0].ListObject as T_MovTrasferimentiEx;
                            T_MovTrasferimentiEx oMovCorrezione = getMovTrasferimentoCorretto(oMovSource, oMovSource);

                            List<T_MovTrasferimentiEx> lst_Matilde = this.ugTrasferimentiMatilde.DataSource as List<T_MovTrasferimentiEx>;
                            T_MovTrasferimentiEx oMovMatilde = lst_Matilde.FirstOrDefault(r => r.Link == oMovSource.Link);
                            if (oMovMatilde != null)
                            {
                                oMovCorrezione.IDCartella = oMovMatilde.IDCartella;
                                oMovCorrezione.Link = 0;
                            }

                            List<T_MovTrasferimentiEx> lst_MovTrasferimenti = oUg.DataSource as List<T_MovTrasferimentiEx>;
                            lst_MovTrasferimenti.Add(oMovCorrezione);

                        }

                    }

                    oUg.DataBind();
                    oUg.Refresh();

                    this.LoadUltraGridCorrezione(oUg.DataSource as List<T_MovTrasferimentiEx>);

                    this.CheckTrasferimentiMatilde(oUg);

                    this.CheckTrasferimentiDWH(oUg);

                }

            }
            catch (Exception)
            {

            }

        }

        #endregion

    }

    public class T_MovTrasferimentiExComparer : IEqualityComparer<T_MovTrasferimentiEx>
    {

        public bool Equals(T_MovTrasferimentiEx x, T_MovTrasferimentiEx y)
        {

            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;

            return x.Link == y.Link
    && x.CodUA == y.CodUA
    && x.CodStatoTrasferimento == y.CodStatoTrasferimento
    && x.DataIngresso.Equals(y.DataIngresso)
    && x.DataUscita.Equals(y.DataUscita)
    && x.CodUO == y.CodUO
    && x.DescrUO == y.DescrUO
    && x.CodSettore == y.CodSettore
    && x.DescrSettore == y.DescrSettore
    && x.CodStanza == y.CodStanza
    && x.DescrStanza == y.DescrStanza
    && x.CodLetto == y.CodLetto
    && x.DescrLetto == y.DescrLetto
    && x.IDCartella.Equals(y.IDCartella)
    && x.CodAziTrasferimento == y.CodAziTrasferimento;

        }

        public int GetHashCode(T_MovTrasferimentiEx x)
        {
            return x.Link
                ^ (x.CodUA != null ? x.CodUA.GetHashCode() : 0)
                ^ (x.CodStatoTrasferimento != null ? x.CodStatoTrasferimento.GetHashCode() : 0)
                ^ (x.DataIngresso != null ? x.DataIngresso.GetHashCode() : 0)
                ^ (x.DataUscita != null ? x.DataUscita.GetHashCode() : 0)
                ^ (x.CodUO != null ? x.CodUO.GetHashCode() : 0)
                ^ (x.DescrUO != null ? x.DescrUO.GetHashCode() : 0)
                ^ (x.CodSettore != null ? x.CodSettore.GetHashCode() : 0)
                ^ (x.DescrSettore != null ? x.DescrSettore.GetHashCode() : 0)
                ^ (x.CodStanza != null ? x.CodStanza.GetHashCode() : 0)
                ^ (x.DescrStanza != null ? x.DescrStanza.GetHashCode() : 0)
                ^ (x.CodLetto != null ? x.CodLetto.GetHashCode() : 0)
                ^ (x.DescrLetto != null ? x.DescrLetto.GetHashCode() : 0)
                ^ (x.IDCartella != null ? x.IDCartella.GetHashCode() : 0)
                ^ (x.CodAziTrasferimento != null ? x.CodAziTrasferimento.GetHashCode() : 0);
        }

    }

}
