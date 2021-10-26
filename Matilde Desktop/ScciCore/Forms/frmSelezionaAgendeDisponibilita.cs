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
using Infragistics.Win.UltraWinTree;
using Infragistics.Win.UltraWinSchedule;
using Infragistics.Win;
using Infragistics.Win.Misc;
using Microsoft.Data.SqlClient;
using System.Globalization;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;

namespace UnicodeSrl.ScciCore
{
    public partial class frmSelezionaAgendeDisponibilita : frmBaseModale, Interfacce.IViewFormlModal
    {

        private bool _runtime = false;

        private ucEasyGrid _ucEasyGrid = null;
                private ucSegnalibri _ucSegnalibri = null;

        #region Properties

        public string AgendaSelezionataCodice { get; set; }
        public string AgendaSelezionataDescrizione { get; set; }


                        public int MaxAnticipo = 0;
        public int MaxRitardo = 0;
        private MassimaliAgenda MassimaliAgenda = null;

        public DateTime DataOraInizioSelezionata { get; set; }
        public DateTime DataOraFineSelezionata { get; set; }

        public int MinutiSlot
        {
            get
            {
                if (this.ucENEMinuti.Value != null)
                    return (int)this.ucENEMinuti.Value;
                else
                    return 10;

            }
        }

        #endregion

        public frmSelezionaAgendeDisponibilita()
        {
            InitializeComponent();
        }

        #region Interface

        public void Carica()
        {

            try
            {

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_ZOOM_256);

                                                                this.WindowState = FormWindowState.Maximized;
                                                var screen = Screen.FromControl(this);
                                this.Width = screen.WorkingArea.Width;
                this.Height = screen.WorkingArea.Height;
                this.Left = screen.WorkingArea.X;
                this.Top = screen.WorkingArea.Y;


                this.InitializeVincoliPrenotazione();
                this.InitializeFilters();
                this.InitializeUltraCalendar();
                this.InitializeUltraGrid();

                Infragistics.Win.UltraWinSchedule.Day day = this.UltraCalendarInfo.GetDay(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio, true);
                this.UltraCalendarInfo.ActiveDay = day;

                this.SelectActiveDay();

                this.setAgendaDayView();

                this.UltraDayView.SelectedTimeSlotRange.SetRange(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio, CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine);
                if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Azione == EnumAzioni.MOD)
                {

                    foreach (Infragistics.Win.UltraWinSchedule.Appointment _App in this.UltraCalendarInfo.Appointments)
                    {
                        if (((System.Data.DataRowView)_App.BindingListObject)["IDAppuntamento"].ToString() == CoreStatics.CoreApplication.MovAppuntamentoSelezionato.IDAppuntamento)
                        {
                            this.UltraCalendarInfo.SelectedAppointments.Add(_App);
                            this.UltraDayView.SelectedTimeSlotRange.SetRange(_App.StartDateTime, _App.EndDateTime);
                            break;
                        }
                    }

                }

                                                
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Carica", this.Name);
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }

        }

        #endregion

        #region Functions

                                private void InitializeVincoliPrenotazione()
        {
                                                MaxAnticipo = 0;
            MaxRitardo = 0;
            try
            {
                if (this.AgendaSelezionataCodice != null && this.AgendaSelezionataCodice.Trim() != "")
                {
                    Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                    op.Parametro.Add("CodAzione", EnumAzioni.VIS.ToString());
                    op.Parametro.Add("CodAgenda", this.AgendaSelezionataCodice);
                    op.Parametro.Add("DatiEstesi", "0");

                    SqlParameterExt[] spcoll = new SqlParameterExt[1];
                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                    DataSet dsTmp = Database.GetDatasetStoredProc("MSP_SelAgende", spcoll);

                    if (dsTmp != null)
                    {
                        if (dsTmp.Tables[0].Rows.Count > 0)
                        {
                            if (!dsTmp.Tables[0].Rows[0].IsNull("MassimoAnticipoPrenotazione")) MaxAnticipo = (int)dsTmp.Tables[0].Rows[0]["MassimoAnticipoPrenotazione"];
                            if (!dsTmp.Tables[0].Rows[0].IsNull("MassimoRitardoPrenotazione")) MaxRitardo = (int)dsTmp.Tables[0].Rows[0]["MassimoRitardoPrenotazione"];
                            if (!dsTmp.Tables[0].Rows[0].IsNull("Risorse")) MassimaliAgenda = XmlProcs.XmlDeserializeFromString<MassimaliAgenda>(dsTmp.Tables[0].Rows[0]["Risorse"].ToString());
                        }

                        dsTmp.Dispose();
                    }

                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        private void InitializeFilters()
        {
            try
            {

                                if (MaxAnticipo > 0 && MaxRitardo >= MaxAnticipo)
                {
                                        this.ucEasyDateRange.Enabled = false;
                    this.ucEasyDateRange.Visible = false;

                    this.udtFiltroDataDa.MinDate = DateTime.Now.Date.AddDays(MaxAnticipo);
                    this.udtFiltroDataDa.Value = this.udtFiltroDataDa.MinDate;
                                        this.udtFiltroDataA.Value = DateTime.Now.Date.AddDays(MaxRitardo);
                }
                else
                {
                    this.ucEasyDateRange.Enabled = true;
                    this.ucEasyDateRange.Visible = true;
                    this.udtFiltroDataDa.MinDate = new DateTime(1900, 1, 1);

                    this.ucEasyDateRange.DisponibilitaAgende = true;
                    this.ucEasyDateRange.Domani = false;
                    this.ucEasyDateRange.DateFuture = false;
                                        string sEasyDateRange = Database.FindValue("ISNULL(CodPeriodoDisponibilita,'')", "T_Agende", $"Codice = '{this.AgendaSelezionataCodice}'", "");
                    if (sEasyDateRange == string.Empty)
                    {
                        this.ucEasyDateRange.Value = ucEasyDateRange.C_RNG_N1Y;
                    }
                    else
                    {
                        this.ucEasyDateRange.Value = sEasyDateRange;
                    }
                }

                                this.ucENEMinuti.Value = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.TimeSlotInterval;

                                this.lblAgenda.Text = AgendaSelezionataDescrizione;

            }
            catch (Exception ex)
            {
                string ss = "";
                ss = ex.Message;
                ss += "";
            }
        }

        private void InitializeUltraGrid()
        {
            this.UltraGridDate.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Medium;
        }

        private void LoadGrid()
        {
            try
            {
                bool bCaricaGrid = true;

                                                                if (bCaricaGrid)
                {
                    if (this.udtFiltroDataDa.Value == null || this.udtFiltroDataA.Value == null)
                    {
                        bCaricaGrid = false;
                        easyStatics.EasyMessageBox(@"Inserire date per la ricerca!", @"Trova Disponibilità", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
                if (bCaricaGrid)
                {
                    if ((DateTime)this.udtFiltroDataDa.Value > (DateTime)this.udtFiltroDataA.Value)
                    {
                        bCaricaGrid = false;
                        easyStatics.EasyMessageBox(@"Inserire date coerenti per la ricerca!", @"Trova Disponibilità", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
                if (bCaricaGrid)
                {
                    if (this.ucENEMinuti.Value == null || (int)this.ucENEMinuti.Value <= 0)
                    {
                        bCaricaGrid = false;
                        easyStatics.EasyMessageBox(@"Valore Minuti non corretto!", @"Trova Disponibilità", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }

                if (bCaricaGrid)
                {
                                                            
                    this.ImpostaCursore(enum_app_cursors.DefaultCursor);


                    Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                                        op.Parametro.Add("CodAgenda", AgendaSelezionataCodice);
                                        DateTime dtI = new DateTime(((DateTime)this.udtFiltroDataDa.Value).Year,
                                                ((DateTime)this.udtFiltroDataDa.Value).Month,
                                                ((DateTime)this.udtFiltroDataDa.Value).Day,
                                                0, 0, 0);
                    DateTime dtF = new DateTime(((DateTime)this.udtFiltroDataA.Value).Year,
                                                ((DateTime)this.udtFiltroDataA.Value).Month,
                                                ((DateTime)this.udtFiltroDataA.Value).Day, 23, 59, 59);
                                        op.Parametro.Add("DataInizio", Database.dataOra105PerParametri(dtI));
                    op.Parametro.Add("DataFine", Database.dataOra105PerParametri(dtF));
                    op.Parametro.Add("DimensioneSlotMinuti", this.ucENEMinuti.Value.ToString());
                                        SqlParameterExt[] spcoll = new SqlParameterExt[1];
                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                    DataTable oDt = Database.GetDataTableStoredProc("MSP_CercaPrimaDisponibilita", spcoll);

                    this.UltraGridDate.DataSource = oDt;
                    this.UltraGridDate.Refresh();

                    this.UltraGridDate.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);
                    this.UltraGridDate.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;

                    if (this.UltraGridDate.Rows.Count > 0)
                    {
                        this.UltraGridDate.ActiveRow = this.UltraGridDate.Rows[0];
                    }

                }
            }
            catch (Exception ex)
            {

                CoreStatics.ExGest(ref ex, "LoadGrid", this.Name);
            }
            finally
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }
        }

                                        private DateTime getDataSelezionata()
        {
            DateTime dtReturn = DateTime.MinValue;
            try
            {
                if (this.UltraGridDate.ActiveRow != null)
                {
                    if (this.UltraGridDate.ActiveRow.Cells["Data"].Value != null)
                        dtReturn = (DateTime)this.UltraGridDate.ActiveRow.Cells["Data"].Value;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            return dtReturn;
        }

        #endregion

        #region UltraCalender

        private void InitializeUltraCalendar()
        {

            CoreStatics.SetUltraDayView(this.UltraDayView);
            this.UltraDayView.CreationFilter = new HeaderToolTipCreationFilter();
            CoreStatics.SetUltraCalendarInfo(this.UltraCalendarInfo);
            CoreStatics.SetUltraCalendarLook(this.UltraCalendarLook);

        }

        private void LoadCalendario()
        {

            try
            {
                DateTime dtSelezionata = getDataSelezionata();
                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                                Parametri opWH = new Parametri(CoreStatics.CoreApplication.Ambiente);

                List<string> codiciAgenda = new List<string>();

                if (dtSelezionata != null && dtSelezionata > DateTime.MinValue)
                {
                                                            
                    Infragistics.Win.UltraWinSchedule.Day day = this.UltraCalendarInfo.GetDay(dtSelezionata, true);
                    this.UltraCalendarInfo.ActiveDay = day;

                                        codiciAgenda.Add(AgendaSelezionataCodice);
                }


                                if (codiciAgenda.Count == 0)
                {
                    codiciAgenda.Add("__NOAGENDA__");
                }

                op.ParametroRipetibile.Add("CodAgenda", codiciAgenda.ToArray());
                opWH.ParametroRipetibile.Add("CodAgenda", codiciAgenda.ToArray());
                if (this.UltraCalendarInfo.SelectedDateRanges.Count > 0)
                {

                    DateTime dtI = new DateTime(this.UltraCalendarInfo.SelectedDateRanges[0].StartDate.Year,
                                                this.UltraCalendarInfo.SelectedDateRanges[0].StartDate.Month,
                                                1, 0, 0, 0);
                    DateTime dtF = new DateTime(this.UltraCalendarInfo.SelectedDateRanges[0].StartDate.Year,
                                                this.UltraCalendarInfo.SelectedDateRanges[0].StartDate.Month,
                                                1, 0, 0, 0);
                    dtF = dtF.AddMonths(2);
                    dtF = dtF.AddDays(-1);

                    op.Parametro.Add("DataInizio", Database.dataOra105PerParametri(dtI));
                    op.Parametro.Add("DataFine", Database.dataOra105PerParametri(dtF));
                    opWH.Parametro.Add("DataInizio", Database.dataOra105PerParametri(dtI));
                    opWH.Parametro.Add("DataFine", Database.dataOra105PerParametri(dtF));
                }
                op.Parametro.Add("DatiEstesi", "1");
                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataSet oDsOwners = Database.GetDatasetStoredProc("MSP_SelMovAppuntamentiAgende", spcoll);


                                                                this.UltraCalendarInfo.Owners.Clear();
                this.UltraCalendarInfo.DataBindingsForOwners.SetDataBinding(oDsOwners, "Table");
                this.UltraCalendarInfo.DataBindingsForOwners.BindingContextControl = this;

                this.UltraCalendarInfo.DataBindingsForOwners.KeyMember = "Codice";
                this.UltraCalendarInfo.DataBindingsForOwners.NameMember = "Descrizione";

                this.UltraCalendarInfo.DataBindingsForOwners.RefreshData();

                if (oDsOwners.Tables[0].Rows.Count != 0)
                {
                    this.UltraDayView.TimeSlotInterval = (Infragistics.Win.UltraWinSchedule.TimeSlotInterval)int.Parse(oDsOwners.Tables[0].Rows[0]["IntervalloSlot"].ToString());

                    this.UltraDayView.EnsureTimeSlotVisible(8, 0, true);
                }

                op.Parametro.Remove("DatiEstesi");
                op.Parametro.Add("DatiEstesi", "0");
                spcoll = new SqlParameterExt[1];
                xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataSet oDsAppointments = Database.GetDatasetStoredProc("MSP_SelMovAppuntamentiAgende", spcoll);

                                                                this.UltraCalendarInfo.DataBindingsForAppointments.SetDataBinding(oDsAppointments, "Table");
                this.UltraCalendarInfo.DataBindingsForAppointments.BindingContextControl = this;

                                this.UltraCalendarInfo.DataBindingsForAppointments.SubjectMember = "Oggetto";
                this.UltraCalendarInfo.DataBindingsForAppointments.DescriptionMember = "Descrizione";
                this.UltraCalendarInfo.DataBindingsForAppointments.StartDateTimeMember = "DataInizio";
                this.UltraCalendarInfo.DataBindingsForAppointments.EndDateTimeMember = "DataFine";
                                this.UltraCalendarInfo.DataBindingsForAppointments.OwnerKeyMember = "CodAgenda";
                
                this.UltraCalendarInfo.DataBindingsForAppointments.RefreshData();

                                                                spcoll = new SqlParameterExt[1];
                xmlParam = XmlProcs.XmlSerializeToString(opWH);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataSet oDsWorkingHours = Database.GetDatasetStoredProc("MSP_SelAgendePeriodi", spcoll);

                this.UltraCalendarInfo.DateSettings.Clear();
                                foreach (DataRow oDr in oDsWorkingHours.Tables[0].Rows)
                {

                    Owner oOwner = this.UltraCalendarInfo.Owners[oDr["CodAgenda"].ToString()];

                    if (!oDr.IsNull("OrariLavoro") && oDr["OrariLavoro"].ToString() != string.Empty)
                    {

                                                List<WorkingHourTimeDaysOfWeek> o_WorkingHourTimeDaysOfWeek = new List<WorkingHourTimeDaysOfWeek>();
                        o_WorkingHourTimeDaysOfWeek = UnicodeSrl.Scci.Statics.XmlProcs.XmlDeserializeFromString<List<WorkingHourTimeDaysOfWeek>>(oDr["OrariLavoro"].ToString());

                                                DateTime[] dates = Enumerable.Range(0, 1 + ((DateTime)oDr["DataFine"]).Subtract((DateTime)oDr["DataInizio"]).Days).Select(i => ((DateTime)oDr["DataInizio"]).AddDays(i)).ToArray();

                                                foreach (WorkingHourTimeDaysOfWeek oWhtdow in o_WorkingHourTimeDaysOfWeek)
                        {

                                                        var varDayOfWeek = dates.Where(d => d.DayOfWeek == (System.DayOfWeek)oWhtdow.DaysOfWeek);

                                                        foreach (DateTime dtDayOfWeek in varDayOfWeek)
                            {

                                OwnerDateSettings cds = oOwner.DateSettings[dtDayOfWeek];
                                if (cds == null)
                                {
                                    cds = new OwnerDateSettings(dtDayOfWeek);
                                    oOwner.DateSettings.Add(cds);
                                }
                                DateTime dtI;
                                DateTime dtF;
                                if (DateTime.TryParse(oWhtdow.HourI, out dtI) && DateTime.TryParse(oWhtdow.HourF, out dtF))
                                {
                                    cds.WorkingHours.Add(new TimeSpan(dtI.Hour, dtI.Minute, 0), new TimeSpan(dtF.Hour, dtF.Minute, 0));
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

        private void SelectActiveDay()
        {

            this.UltraCalendarInfo.SelectedDateRanges.Clear();
            DateTime _date = (this.UltraCalendarInfo.ActiveDay != null ? this.UltraCalendarInfo.ActiveDay.Date : DateTime.Today);
            this.UltraCalendarInfo.SelectedDateRanges.Add(_date, 0);

        }

        private Owner GetOwner()
        {

            Owner oOwner = null;

            try
            {

                if (this.UltraDayView.Visible == true)
                {
                    oOwner = this.UltraDayView.ActiveOwner;
                }

                if (oOwner == null && this.UltraCalendarInfo.Owners.Count > 1)
                {
                    oOwner = this.UltraCalendarInfo.Owners[1];
                }

            }
            catch (Exception)
            {

            }

            return oOwner;

        }

        #endregion

        #region Events Form

        private void frmSelezionaTipoAppuntamento_ImmagineClick(object sender, ImmagineTopClickEventArgs e)
        {

            try
            {

                switch (e.Tipo)
                {

                    case EnumImmagineTop.Segnalibri:
                        if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Bookmarks > 0)
                        {
                            CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerSegnalibri);
                            _ucSegnalibri = new ucSegnalibri();
                            int iWidth = Convert.ToInt32(easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large)) * 40;
                            int iHeight = Convert.ToInt32((double)iWidth / 1.52D);
                            _ucSegnalibri.Size = new Size(iWidth, iHeight);
                            _ucSegnalibri.ViewInit();
                            _ucSegnalibri.Focus();
                            this.UltraPopupControlContainerSegnalibri.Show();
                        }
                        break;

                    case EnumImmagineTop.SegnalibroAdd:
                        break;

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void frmSelezionaTipoAppuntamento_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {

            try
            {
                                                                bool bContinue = true;
                this.DataOraInizioSelezionata = DateTime.MinValue;
                this.DataOraFineSelezionata = DateTime.MinValue;

                if (bContinue)
                {
                                        DateTime dtSelezionata = getDataSelezionata();
                    if (dtSelezionata <= DateTime.MinValue)
                    {
                        bContinue = false;
                        easyStatics.EasyMessageBox(@"Selezionare una data!", @"Cerca Disponibilità", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                                    }
                if (bContinue)
                {
                                        if (this.UltraDayView.SelectedTimeSlotRange == null)
                    {
                        bContinue = false;
                        easyStatics.EasyMessageBox(@"Selezionare un orario nell'agenda!", @"Cerca Disponibilità", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
                if (bContinue)
                {
                                        if (this.UltraGridDate.ActiveRow != null && this.UltraGridDate.ActiveRow.Appearance.BackColor == Color.Red)
                    {
                        bContinue = false;
                        easyStatics.EasyMessageBox(@"Orario selezionato con massimale superato!", @"Cerca Disponibilità", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }

                if (bContinue)
                {
                                        int iTimeSlotInterval = 0;
                    if (this.ucENEMinuti.Value != null && (int)this.ucENEMinuti.Value > 0)
                        iTimeSlotInterval = (int)this.ucENEMinuti.Value;
                    else if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.TimeSlotInterval != 0)
                        iTimeSlotInterval = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.TimeSlotInterval;

                    if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Azione == EnumAzioni.INS)
                    {
                        this.DataOraInizioSelezionata =
                            new DateTime(this.UltraCalendarInfo.ActiveDay.Date.Year,
                                            this.UltraCalendarInfo.ActiveDay.Date.Month,
                                            this.UltraCalendarInfo.ActiveDay.Date.Day,
                                            this.UltraDayView.SelectedTimeSlotRange.StartDateTime.Hour,
                                            this.UltraDayView.SelectedTimeSlotRange.StartDateTime.Minute,
                                            0);
                        if (iTimeSlotInterval != 0)
                        {
                            this.DataOraFineSelezionata = this.DataOraInizioSelezionata.AddMinutes(iTimeSlotInterval);
                        }
                        else
                        {
                            this.DataOraFineSelezionata =
                                new DateTime(this.UltraCalendarInfo.ActiveDay.Date.Year,
                                                this.UltraCalendarInfo.ActiveDay.Date.Month,
                                                this.UltraCalendarInfo.ActiveDay.Date.Day,
                                                this.UltraDayView.SelectedTimeSlotRange.EndDateTime.Hour,
                                                this.UltraDayView.SelectedTimeSlotRange.EndDateTime.Minute,
                                                0);
                        }
                    }
                    else
                    {
                        TimeSpan oTs = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine - CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio;
                        this.DataOraInizioSelezionata =
                           new DateTime(this.UltraCalendarInfo.ActiveDay.Date.Year,
                                           this.UltraCalendarInfo.ActiveDay.Date.Month,
                                           this.UltraCalendarInfo.ActiveDay.Date.Day,
                                           this.UltraDayView.SelectedTimeSlotRange.StartDateTime.Hour,
                                           this.UltraDayView.SelectedTimeSlotRange.StartDateTime.Minute,
                                           0);
                        this.DataOraFineSelezionata = this.DataOraInizioSelezionata.Add(oTs);

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

        private void frmSelezionaTipoAppuntamento_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void frmSelezionaAgendeDisponibilita_Shown(object sender, EventArgs e)
        {

                                    LoadGrid();
        }

        #endregion

        #region Events

        private void ucEasyDateRange_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!_runtime)
                {
                    _runtime = true;
                    if (this.ucEasyDateRange.DataOraDa != null) this.udtFiltroDataDa.Value = this.ucEasyDateRange.DataOraDa;
                    if (this.ucEasyDateRange.DataOraA != null) this.udtFiltroDataA.Value = this.ucEasyDateRange.DataOraA;
                    _runtime = false;
                }
            }
            catch
            {
                _runtime = false;
            }
        }

        private void udtFiltroData_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!_runtime)
                {
                    _runtime = true;
                    this.ucEasyDateRange.Value = "";
                    _runtime = false;
                }
            }
            catch
            {
                _runtime = false;
            }
        }

        private void UltraGridDate_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                foreach (UltraGridColumn col in e.Layout.Bands[0].Columns)
                {
                    switch (col.Key)
                    {
                        case "Data":
                            col.Hidden = false;
                            col.Header.Caption = "Data";
                            col.CellAppearance.TextHAlign = HAlign.Left;
                            col.Format = @"dddd dd/MM/yyyy";
                            break;

                        case "NumAppPren":
                            col.Hidden = false;
                            col.Header.Caption = "N. App.";
                            col.CellAppearance.TextHAlign = HAlign.Right;
                            col.Format = @"#,##0";
                            break;

                        case "MinutiDisp":
                            col.Hidden = false;
                            col.Header.Caption = "Minuti Disp.";
                            col.CellAppearance.TextHAlign = HAlign.Right;
                            col.Format = @"#,##0";
                            break;

                        default:
                            col.Hidden = true;
                            break;
                    }
                }
            }
            catch
            {
            }
        }

        private void UltraGridDate_InitializeRow(object sender, InitializeRowEventArgs e)
        {

            try
            {

                bool bok = true;

                if (e.Row.Cells.Exists("Data") == true && e.Row.Cells.Exists("NumAppPren") == true && MassimaliAgenda != null)
                {

                    DateTime dt = (DateTime)e.Row.Cells["Data"].Value;
                    if (MassimaliAgenda.Massimale[(int)dt.DayOfWeek] > 0)
                    {

                        if ((int)e.Row.Cells["NumAppPren"].Value >= MassimaliAgenda.Massimale[(int)dt.DayOfWeek])
                        {
                            bok = false;
                        }

                    }

                }

                if (bok == false)
                {
                    e.Row.Appearance.BackColor = Color.Red;
                }

            }
            catch (Exception)
            {

            }

        }

        private void ubTrovaDisp_Click(object sender, EventArgs e)
        {
            LoadGrid();
        }

        private void UltraGridDate_AfterRowActivate(object sender, EventArgs e)
        {
            LoadCalendario();
        }

        private void UltraCalendarInfo_AfterActiveDayChanged(object sender, Infragistics.Win.UltraWinSchedule.AfterActiveDayChangedEventArgs e)
        {
            this.SelectActiveDay();
            this.setAgendaDayView();
        }

        private void UltraCalendarInfo_AppointmentDataInitialized(object sender, Infragistics.Win.UltraWinSchedule.AppointmentDataInitializedEventArgs e)
        {

            try
            {

                DataRowView oRowView = (System.Data.DataRowView)e.Appointment.BindingListObject;

                if (oRowView["Colore"] != null)
                {
                    e.Appointment.Appearance.BackColor = CoreStatics.GetColorFromString(oRowView["Colore"].ToString());
                }
                e.Appointment.Appearance.BackGradientStyle = Infragistics.Win.GradientStyle.None;

            }
            catch (Exception)
            {

            }

            e.Appointment.Locked = true;

        }

        private void UltraCalendarInfo_OwnerDataInitialized(object sender, Infragistics.Win.UltraWinSchedule.OwnerDataInitializedEventArgs e)
        {

            try
            {

                if (((System.Data.DataRowView)e.Owner.BindingListObject)["Colore"] != null)
                {
                                        Color Colore = CoreStatics.GetColorFromString(((System.Data.DataRowView)e.Owner.BindingListObject)["Colore"].ToString());
                    e.Owner.DayAppearance.BackColor = Colore;
                    e.Owner.WorkingHourTimeSlotAppearance.BackColor = Color.LightGreen;
                    e.Owner.HeaderAppearance.BackColor = Colore;
                }

                if (((System.Data.DataRowView)e.Owner.BindingListObject)["Icona"] != null)
                {
                    e.Owner.HeaderAppearance.Image = DrawingProcs.GetImageFromByte(((System.Data.DataRowView)e.Owner.BindingListObject)["Icona"], new Size(48, 48));
                    e.Owner.HeaderAppearance.ImageHAlign = Infragistics.Win.HAlign.Left;
                }

                WorkingHourTime oWht = XmlProcs.XmlDeserializeFromString<WorkingHourTime>(((System.Data.DataRowView)e.Owner.BindingListObject)["OrariLavoro"].ToString());

                                                e.Owner.ResetDateSettings();

                for (int x = 0; x < oWht.HourI.Length; x++)
                {
                    e.Owner.DayOfWeekSettings[(System.DayOfWeek)x].WorkingHours.Add(
                        new TimeSpan(int.Parse(oWht.HourI[x].ToString().Substring(0, 2)), int.Parse(oWht.HourI[x].ToString().Substring(3, 2)), 0),
                        new TimeSpan(int.Parse(oWht.HourF[x].ToString().Substring(0, 2)), int.Parse(oWht.HourF[x].ToString().Substring(3, 2)), 0));
                }

            }
            catch (Exception)
            {

            }

            e.Owner.Locked = true;

        }

        private void UltraDayView_BeforeTimeSlotSelectionChanged(object sender, Infragistics.Win.UltraWinSchedule.BeforeTimeSlotSelectionChangedEventArgs e)
        {
            this.UltraCalendarInfo.EventManager.SetEnabled(Infragistics.Win.UltraWinSchedule.CalendarInfoEventGroups.AllEvents, false);
            Infragistics.Win.UltraWinSchedule.Day day = this.UltraCalendarInfo.GetDay(e.NewSelectedTimeSlotRange.StartDateTime, true);
            this.UltraCalendarInfo.ActiveDay = day;
            this.UltraCalendarInfo.EventManager.SetEnabled(Infragistics.Win.UltraWinSchedule.CalendarInfoEventGroups.AllEvents, true);
        }

        private void UltraView_Click(object sender, EventArgs e)
        {

            try
            {

                Point oPoint;
                UIElement oUIElement = null;
                DateTime oDate = DateTime.MinValue;
                Owner oOwner = this.GetOwner();

                if (sender.GetType() == typeof(UltraDayView))
                {
                    oPoint = this.UltraDayView.PointToClient(MousePosition);
                    oUIElement = this.UltraDayView.UIElement.ElementFromPoint(oPoint);
                }

                if (oUIElement.GetType() == typeof(Infragistics.Win.UltraWinSchedule.DayView.DayHeaderUIElement) ||
                    oUIElement.Parent.GetType() == typeof(Infragistics.Win.UltraWinSchedule.DayView.DayHeaderUIElement) ||
                    oUIElement.GetType() == typeof(Infragistics.Win.UltraWinSchedule.WeekView.DayNumberUIElement) ||
                    oUIElement.Parent.GetType() == typeof(Infragistics.Win.UltraWinSchedule.WeekView.DayNumberUIElement) ||
                    oUIElement.Parent.GetType() == typeof(Infragistics.Win.UltraWinSchedule.WeekView.CurrentDayNumberSelectionAreaUIElement) ||
                    oUIElement.GetType() == typeof(Infragistics.Win.UltraWinSchedule.MonthViewSingle.DayNumberUIElement) ||
                    oUIElement.Parent.GetType() == typeof(Infragistics.Win.UltraWinSchedule.MonthViewSingle.DayNumberUIElement) ||
                    oUIElement.Parent.GetType() == typeof(Infragistics.Win.UltraWinSchedule.MonthViewSingle.CurrentDayNumberSelectionAreaUIElement))
                {

                    if (oUIElement.SelectableItem.GetType() == typeof(Infragistics.Win.UltraWinSchedule.Day))
                    {
                                                Infragistics.Win.UltraWinSchedule.Day day = (Infragistics.Win.UltraWinSchedule.Day)oUIElement.SelectableItem;
                        oDate = day.Date;
                    }
                    else if (oUIElement.SelectableItem.GetType() == typeof(Infragistics.Win.UltraWinSchedule.VisibleDay))
                    {
                                                Infragistics.Win.UltraWinSchedule.VisibleDay visibleday = (Infragistics.Win.UltraWinSchedule.VisibleDay)oUIElement.SelectableItem;
                        oDate = visibleday.Date;
                    }

                }

                _ucEasyGrid = null;
                _ucEasyGrid = CoreStatics.getGridAppuntamentixTipo(new DateTime(oDate.Year, oDate.Month, oDate.Day, 0, 0, 0),
                                                                    new DateTime(oDate.Year, oDate.Month, oDate.Day, 23, 59, 59),
                                                                    (oOwner != null ? oOwner.Key : ""));

                if (_ucEasyGrid != null && _ucEasyGrid.DataSource != null)
                {
                    
                    _ucEasyGrid.Size = new Size(400, 200);

                    _ucEasyGrid.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;

                    CoreStatics.SetEasyUltraGridLayout(ref _ucEasyGrid);

                                        CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainer);

                                        

                    this.UltraPopupControlContainer.Show();


                                        _ucEasyGrid.DisplayLayout.Bands[0].ClearGroupByColumns();

                                        _ucEasyGrid.DisplayLayout.Bands[0].Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
                    _ucEasyGrid.Refresh();
                }

            }
            catch (Exception)
            {

            }

        }

        private void UltraDayView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                e.SuppressKeyPress = true;
            }
        }

        private void setAgendaDayView()
        {

            this.UltraDayView.Visible = true;
            this.UltraDayView.EnsureTimeSlotVisible(8, 0, true);
            this.LoadCalendario();

        }

        #endregion

        #region UltraPopupControlContainer

        private void UltraPopupControlContainer_Closed(object sender, EventArgs e)
        {
            _ucEasyGrid.ClickCell -= ucEasyGrid_ClickCell;
        }

        private void UltraPopupControlContainer_Opened(object sender, EventArgs e)
        {
            _ucEasyGrid.ClickCell += ucEasyGrid_ClickCell;
            _ucEasyGrid.Focus();
        }

        private void UltraPopupControlContainer_Opening(object sender, CancelEventArgs e)
        {
            Infragistics.Win.Misc.UltraPopupControlContainer popup = sender as Infragistics.Win.Misc.UltraPopupControlContainer;
            popup.PopupControl = _ucEasyGrid;
        }

        private void ucEasyGrid_ClickCell(object sender, ClickCellEventArgs e)
        {
            this.UltraPopupControlContainer.Close();
        }

        #endregion

        #region UltraPopupControlContainerSegnalibri

        private void UltraPopupControlContainerSegnalibri_Closed(object sender, EventArgs e)
        {
            _ucSegnalibri.SegnalibriClick -= UltraPopupControlContainerSegnalibri_ModificaClick;
        }

        private void UltraPopupControlContainerSegnalibri_Opened(object sender, EventArgs e)
        {
            _ucSegnalibri.SegnalibriClick += UltraPopupControlContainerSegnalibri_ModificaClick;
            _ucSegnalibri.Focus();
        }

        private void UltraPopupControlContainerSegnalibri_Opening(object sender, CancelEventArgs e)
        {
            UltraPopupControlContainer popup = sender as UltraPopupControlContainer;
            popup.PopupControl = _ucSegnalibri;
        }

        private void UltraPopupControlContainerSegnalibri_ModificaClick(object sender, SegnalibriClickEventArgs e)
        {

            try
            {

                switch (e.Pulsante)
                {

                    case EnumPulsanteSegnalibri.Modifica:
                        this.UltraPopupControlContainerSegnalibri.Close();
                        this.ucTopModale.Focus();
                        CoreStatics.CaricaPopup(EnumMaschere.Scheda, EnumEntita.SCH, e.ID);
                        break;

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        #endregion

    }
}
