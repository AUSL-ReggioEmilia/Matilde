using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinSchedule;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.ScciResource;
using UnicodeSrl.Framework.Data;

namespace UnicodeSrl.ScciCore
{
    public partial class ucCalendario : UserControl, Interfacce.IViewCalendario
    {

        private UserControl _ucc = null;

        public ucCalendario()
        {
            InitializeComponent();

            _ucc = (UserControl)this;
        }

        #region Declare

        private Calendario _calendario = new Calendario();
        private Single _Size = 10;
        private DateTime _RangeStartDate;
        private DateTime _RangeEndDate;
        private bool _FirstLoad = true;

        private bool _buttonChangeDate = false;

        private string _filtro = string.Empty;

        public event SelectedCalendarioEventHandler SelectedCalendario;
        public delegate void SelectedCalendarioEventHandler(object sender, CalendarioEventArgs e);

        public event AppointmentDataInitializedEventHandler CalendarInfoAppointmentDataInitialized;
        public delegate void AppointmentDataInitializedEventHandler(object sender, AppointmentDataInitializedEventArgs e);

        public event OwnerDataInitializedEventHandler CalendarInfoOwnerDataInitialized;
        public delegate void OwnerDataInitializedEventHandler(object sender, OwnerDataInitializedEventArgs e);

        public event DayNumberClickEventHandler DayNumberClick;
        public delegate void DayNumberClickEventHandler(object sender, DayNumberClickEventArgs e);

        public event DragEventHandler CalendarioDragDrop;
        public event DragEventHandler CalendarioDragOver;
        public event MouseEventHandler CalendarioMouseDown;
        public event MouseEventHandler CalendarioMouseMove;

        #endregion

        #region Interface

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Calendario Calendario
        {
            get
            {
                return _calendario;
            }
            set
            {
                _calendario = value;
                this.UltraCalendarInfo.ActiveDay = this.UltraCalendarInfo.GetDay(_calendario.ActiveDay, true);
                this.UcAgende.CodAgenda = _calendario.CodAgenda;
                this.UcAgende.RefreshData();
                this.UltraToolbarsManager.OptionSets["DateButtons"].SelectedTool = (StateButtonTool)this.UltraToolbarsManager.Tools[_calendario.TipoCalendario];
            }
        }

        public void ViewInit()
        {

            try
            {
                _buttonChangeDate = false;
                this.SetStyleUcCalendario();


                this.SetZoom();
                this.SetRange(true);


                this.UcUltraMonthViewMulti.UltraMonthViewMulti.CalendarInfo = this.UltraCalendarInfo;
                this.UcUltraMonthViewMulti.UltraMonthViewMulti.CalendarLook = this.UltraCalendarLook;
                this.UcUltraMonthViewMulti.RefreshData();

                this.SelectActiveDay();
                this.UcUltraMonthViewMulti.UltraMonthViewMulti.EnsureVisible(this.UltraCalendarInfo.ActiveDay.Month);
                this.UltraWeekView.ScrollDayIntoView(this.UltraCalendarInfo.ActiveDay.Date);
                this.SetRange(false);
                _FirstLoad = false;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        #endregion

        #region Property

        public bool FirstLoad
        {
            get { return _FirstLoad; }
        }

        private Owner Owner
        {
            get { return GetOwner(); }
        }

        public int TimeSlotInterval
        {
            get
            {
                try
                {
                    return int.Parse(((System.Data.DataRowView)this.Owner.BindingListObject)["IntervalloSlot"].ToString());
                }
                catch (Exception)
                {
                    return 10;
                }
            }
        }

        public string Colore
        {
            get
            {
                try
                {
                    return ((System.Data.DataRowView)this.Owner.BindingListObject)["Colore"].ToString();
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

        #endregion

        #region SubRoutine

        private void SetStyleUcCalendario()
        {

            try
            {

                var oUtbm = this.UltraToolbarsManager;

                oUtbm.LockToolbars = true;
                oUtbm.RuntimeCustomizationOptions = RuntimeCustomizationOptions.None;
                oUtbm.Style = ToolbarStyle.Office2010;
                oUtbm.UseOsThemes = DefaultableBoolean.False;

                oUtbm.ToolbarSettings.AllowCustomize = DefaultableBoolean.False;
                oUtbm.ToolbarSettings.AllowDockBottom = DefaultableBoolean.False;
                oUtbm.ToolbarSettings.AllowDockLeft = DefaultableBoolean.False;
                oUtbm.ToolbarSettings.AllowDockRight = DefaultableBoolean.False;
                oUtbm.ToolbarSettings.AllowDockTop = DefaultableBoolean.False;
                oUtbm.ToolbarSettings.AllowFloating = DefaultableBoolean.False;
                oUtbm.ToolbarSettings.AllowHiding = DefaultableBoolean.False;
                oUtbm.ToolbarSettings.FillEntireRow = DefaultableBoolean.True;
                oUtbm.ToolbarSettings.PaddingBottom = 5;
                oUtbm.ToolbarSettings.PaddingLeft = 5;
                oUtbm.ToolbarSettings.PaddingRight = 5;
                oUtbm.ToolbarSettings.PaddingTop = 5;
                oUtbm.ToolbarSettings.ToolSpacing = 5;
                oUtbm.ToolbarSettings.UseLargeImages = DefaultableBoolean.True;

                CoreStatics.SetUltraDayView(this.UltraDayView);
                CoreStatics.SetUltraWeekView(this.UltraWeekView);
                CoreStatics.SetUltraMonthViewSingle(this.UltraMonthViewSingle);
                CoreStatics.SetUltraCalendarInfo(this.UltraCalendarInfo);
                CoreStatics.SetUltraCalendarLook(this.UltraCalendarLook);

                oUtbm.Tools["Oggi"].SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_AGENDAOGGI_32);
                oUtbm.Tools["Giorno"].SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_AGENDAGIORNALIERA_32);
                oUtbm.Tools["SettimanaLav"].SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_AGENDAGIORNALIERALAV_32);
                oUtbm.Tools["SettimanaLav5"].SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_AGENDAGIORNALIERALAV5_32);
                oUtbm.Tools["Settimana"].SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_AGENDASETTIMANALE_32);
                oUtbm.Tools["Mese"].SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_AGENDAMENSILE_32);
                oUtbm.Tools["ZoomOut"].SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.ZoomOut_32;
                oUtbm.Tools["ZoomIn"].SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.ZoomIn_32;
                oUtbm.Tools["Stampa"].SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_REPORT_32);
                oUtbm.Tools["Refresh"].SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_AGGIORNA_32);
                oUtbm.Tools["Agende"].SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_AGENDAELENCO_32);
                oUtbm.Tools["Data"].SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_AGENDA_32);

                oUtbm.Tools["btnAvanti"].SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FRECCIADX_32);
                oUtbm.Tools["btnIndietro"].SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FRECCIASX_32);

            }
            catch (Exception)
            {

            }

        }

        private void SetUltraToolbars()
        {

            try
            {

                bool bEnable = !this.UcAgende.IsLista;

                this.UltraToolbarsManager.Tools["Oggi"].SharedProps.Enabled = bEnable;
                this.UltraToolbarsManager.Tools["Giorno"].SharedProps.Enabled = bEnable;
                this.UltraToolbarsManager.Tools["Settimana"].SharedProps.Enabled = bEnable;
                this.UltraToolbarsManager.Tools["SettimanaLav"].SharedProps.Enabled = bEnable;
                this.UltraToolbarsManager.Tools["SettimanaLav5"].SharedProps.Enabled = bEnable;
                this.UltraToolbarsManager.Tools["Mese"].SharedProps.Enabled = bEnable;
                this.UltraToolbarsManager.Tools["ZoomOut"].SharedProps.Enabled = bEnable;
                this.UltraToolbarsManager.Tools["ZoomIn"].SharedProps.Enabled = bEnable;
                this.UltraToolbarsManager.Tools["btnIndietro"].SharedProps.Enabled = bEnable;
                this.UltraToolbarsManager.Tools["btnAvanti"].SharedProps.Enabled = bEnable;

            }
            catch (Exception)
            {

            }

        }

        private void ActionBeforeToolDropdown(ToolBase Tool)
        {

            try
            {

                switch (Tool.Key)
                {

                    case "Agende":
                        this.UcAgende.RefreshData();
                        break;

                }

            }
            catch (Exception)
            {

            }

        }

        private void ActionToolClick(ToolBase Tool)
        {

            bool bEnableToolbar = false;

            Infragistics.Win.UltraWinSchedule.Day day = this.UltraCalendarInfo.ActiveDay;
            DateTime _date = (day != null ? day.Date : DateTime.Today);

            try
            {


                switch (Tool.Key)
                {

                    case "btnAvanti":
                    case "btnIndietro":
                        bool bIndietro = (Tool.Key == "btnIndietro");
                        if (this.UltraCalendarInfo.ActiveDay != null)
                        {

                            Tool.ToolbarsManager.Enabled = false;
                            bEnableToolbar = true;

                            if (this.UltraDayView.Visible)
                            {
                                CambiaGiorno(bIndietro);

                            }
                            else if (this.UltraWeekView.Visible)
                            {
                                CambiaSettimana(bIndietro);
                            }
                            else if (this.UltraMonthViewSingle.Visible)
                            {
                                CambiaMese(bIndietro);
                            }
                        }

                        break;

                    case "Oggi":
                        day = this.UltraCalendarInfo.GetDay(DateTime.Today, true);
                        this.UltraCalendarInfo.ActiveDay = day;
                        this.SelectActiveDay();
                        this.UcUltraMonthViewMulti.UltraMonthViewMulti.EnsureVisible(day.Month);
                        this.SetRange(false);
                        this.LoadUltraCalendarInfo();
                        break;

                    case "Giorno":
                    case "Settimana":
                    case "SettimanaLav":
                    case "SettimanaLav5":
                    case "Mese":
                        this.UltraDayView.Visible = false;
                        this.UltraWeekView.Visible = false;
                        this.UltraMonthViewSingle.Visible = false;

                        if (Tool.Key == "Giorno")
                        {
                            this.UltraDayView.Dock = DockStyle.Fill;
                            this.UltraDayView.Visible = true;
                            this.UltraDayView.TimeSlotInterval = (Infragistics.Win.UltraWinSchedule.TimeSlotInterval)this.TimeSlotInterval;
                            this.UltraDayView.EnsureTimeSlotVisible(8, 0, true);
                            this.SelectActiveDay();
                        }
                        else if (Tool.Key == "SettimanaLav")
                        {
                            this.UltraDayView.Dock = DockStyle.Fill;
                            this.UltraDayView.Visible = true;
                            this.UltraDayView.TimeSlotInterval = (Infragistics.Win.UltraWinSchedule.TimeSlotInterval)this.TimeSlotInterval;
                            this.UltraDayView.EnsureTimeSlotVisible(8, 0, true);
                            this.SelectActiveDay(6);
                        }
                        else if (Tool.Key == "SettimanaLav5")
                        {
                            this.UltraDayView.Dock = DockStyle.Fill;
                            this.UltraDayView.Visible = true;
                            this.UltraDayView.TimeSlotInterval = (Infragistics.Win.UltraWinSchedule.TimeSlotInterval)this.TimeSlotInterval;
                            this.UltraDayView.EnsureTimeSlotVisible(8, 0, true);
                            this.SelectActiveWeek();
                        }
                        else
                        {
                            if (Tool.Key == "Settimana")
                            {

                                this.UltraWeekView.Dock = DockStyle.Fill;
                                this.UltraWeekView.Visible = true;
                                this.UltraWeekView.VisibleWeek = this.UltraCalendarInfo.ActiveDay.Week;
                            }
                            else
                            {
                                if (Tool.Key == "Mese")
                                {
                                    this.UltraMonthViewSingle.Dock = DockStyle.Fill;
                                    this.UltraMonthViewSingle.Visible = true;
                                }
                            }
                            this.SelectActiveDay();
                        }
                        _calendario.TipoCalendario = Tool.Key;
                        break;

                    case "ZoomOut":
                    case "ZoomIn":
                        if (Tool.Key == "ZoomOut")
                        {
                            if (_Size > 1) { _Size -= 1; }
                        }
                        else if (Tool.Key == "ZoomIn")
                        {
                            _Size += 1;
                        }
                        this.SetZoom();
                        break;

                    case "Stampa":
                        this.StampaCalendario(true);
                        break;

                    case "Refresh":
                        this.SetRange(false);
                        this.LoadUltraCalendarInfo();
                        if (SelectedCalendario != null) { SelectedCalendario(this.UltraCalendarInfo, new CalendarioEventArgs(this.GetOwner())); }
                        break;

                    case "Agende":
                        this.UcAgende.RefreshData();
                        break;

                }

            }
            catch (Exception)
            {

            }
            finally
            {
                if (bEnableToolbar && Tool != null) Tool.ToolbarsManager.Enabled = true;
            }

        }

        private void SelectActiveDay()
        {
            this.SelectActiveDay(0);
        }
        private void SelectActiveDay(int gg)
        {

            try
            {

                this.UltraCalendarInfo.SelectedDateRanges.Clear();
                DateTime _date = (this.UltraCalendarInfo.ActiveDay != null ? this.UltraCalendarInfo.ActiveDay.Date : DateTime.Today);
                this.UltraCalendarInfo.SelectedDateRanges.Add(_date, gg);
                _calendario.ActiveDay = this.UltraCalendarInfo.ActiveDay.Date;

            }
            catch (Exception)
            {

            }

        }

        private void SelectActiveWeek()
        {

            try
            {

                this.UltraCalendarInfo.SelectedDateRanges.Clear();
                DateTime _date = (this.UltraCalendarInfo.ActiveDay != null ? this.UltraCalendarInfo.ActiveDay.Date : DateTime.Today);
                _date = _date.AddDays(((int)_date.DayOfWeek - 1) * -1);
                this.UltraCalendarInfo.SelectedDateRanges.Add(_date, 4);
                _calendario.ActiveDay = this.UltraCalendarInfo.ActiveDay.Date;

            }
            catch (Exception)
            {

            }

        }

        private void SetZoom()
        {

            try
            {

                var oUcl = this.UltraCalendarLook;
                oUcl.DayHeaderAppearance.FontData.SizeInPoints = _Size;
                oUcl.AppointmentAppearance.FontData.SizeInPoints = _Size;
                this.UltraDayView.TimeSlotDescriptorAppearance.FontData.SizeInPoints = _Size;

            }
            catch (Exception)
            {

            }

        }

        private void SetRange(bool bLoad)
        {

            if (bLoad == true)
            {
                _RangeStartDate = new DateTime(this.Calendario.ActiveDay.Year, this.Calendario.ActiveDay.Month, 1).AddMonths(-1);
                _RangeEndDate = new DateTime(this.Calendario.ActiveDay.Year, this.Calendario.ActiveDay.Month, DateTime.DaysInMonth(this.Calendario.ActiveDay.Year, this.Calendario.ActiveDay.Month));
            }
            else
            {
                _RangeStartDate = new DateTime(this.UltraCalendarInfo.ActiveDay.Date.Year, this.UltraCalendarInfo.ActiveDay.Date.Month, 1).AddMonths(-1);
                _RangeEndDate = new DateTime(this.UltraCalendarInfo.ActiveDay.Date.Year, this.UltraCalendarInfo.ActiveDay.Date.Month, DateTime.DaysInMonth(this.UltraCalendarInfo.ActiveDay.Date.Year, this.UltraCalendarInfo.ActiveDay.Date.Month));
            }
            _RangeEndDate = _RangeEndDate.AddDays(1);
            _RangeEndDate = _RangeEndDate.AddMonths(1);
            _RangeEndDate = _RangeEndDate.AddDays(-1);

        }

        private void LoadUltraCalendarInfo(bool bChangeMonth = false)
        {

            try
            {

                if (_FirstLoad == false)
                {

                    Parametri opWH = new Parametri(CoreStatics.CoreApplication.Ambiente);

                    Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                    op.Parametro.Add("CodAzione", EnumAzioni.VIS.ToString());
                    op.Parametro.Add("CodAgenda", this.UcAgende.CodAgenda);
                    op.Parametro.Add("DatiEstesi", "1");

                    SqlParameterExt[] spcoll = new SqlParameterExt[1];
                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                    DataSet oDsOwners = Database.GetDatasetStoredProc("MSP_SelAgende", spcoll); ;

                    this.UltraCalendarInfo.Owners.Clear();
                    this.UltraCalendarInfo.DataBindingsForOwners.SetDataBinding(oDsOwners, "Table");
                    this.UltraCalendarInfo.DataBindingsForOwners.BindingContextControl = this;

                    this.UltraCalendarInfo.DataBindingsForOwners.KeyMember = "Codice";
                    this.UltraCalendarInfo.DataBindingsForOwners.NameMember = "Descrizione";

                    this.UltraCalendarInfo.DataBindingsForOwners.RefreshData();

                    op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("CodAgenda", this.UcAgende.CodAgenda);
                    opWH.Parametro.Add("CodAgenda", this.UcAgende.CodAgenda);
                    if (this.UcAgende.IsLista)
                    {
                        op.Parametro.Add("Lista", "1");
                    }
                    else
                    {
                        if (bChangeMonth == false)
                        {
                            if (this.UltraCalendarInfo.SelectedDateRanges.Count > 0)
                            {

                                DateTime dtI = new DateTime(this.UltraCalendarInfo.SelectedDateRanges[0].StartDate.Year,
                                                            this.UltraCalendarInfo.SelectedDateRanges[0].StartDate.Month,
                                                            1, 0, 0, 0);
                                DateTime dtF = new DateTime(this.UltraCalendarInfo.SelectedDateRanges[0].StartDate.Year,
                                                            this.UltraCalendarInfo.SelectedDateRanges[0].StartDate.Month,
                                                            1, 0, 0, 0);
                                dtF = dtF.AddMonths(3);
                                dtF = dtF.AddDays(-1);

                                op.Parametro.Add("DataInizio", Database.dataOra105PerParametri(dtI));
                                op.Parametro.Add("DataFine", Database.dataOra105PerParametri(dtF));
                                opWH.Parametro.Add("DataInizio", Database.dataOra105PerParametri(dtI));
                                opWH.Parametro.Add("DataFine", Database.dataOra105PerParametri(dtF));
                            }
                            else
                            {
                                op.Parametro.Add("DataInizio", Database.dataOra105PerParametri(_RangeStartDate));
                                op.Parametro.Add("DataFine", Database.dataOra105PerParametri(_RangeEndDate));
                                opWH.Parametro.Add("DataInizio", Database.dataOra105PerParametri(_RangeStartDate));
                                opWH.Parametro.Add("DataFine", Database.dataOra105PerParametri(_RangeEndDate));
                            }
                        }
                        else
                        {
                            op.Parametro.Add("DataInizio", Database.dataOra105PerParametri(_RangeStartDate));
                            op.Parametro.Add("DataFine", Database.dataOra105PerParametri(_RangeEndDate));
                            opWH.Parametro.Add("DataInizio", Database.dataOra105PerParametri(_RangeStartDate));
                            opWH.Parametro.Add("DataFine", Database.dataOra105PerParametri(_RangeEndDate));
                        }
                    }
                    op.Parametro.Add("FiltroGenerico", _filtro);
                    op.Parametro.Add("DatiEstesi", "0");
                    spcoll = new SqlParameterExt[1];
                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                    DataSet oDsAppointments = Database.GetDatasetStoredProc("MSP_SelMovAppuntamentiAgende", spcoll);

                    this.UltraCalendarInfo.DataBindingsForAppointments.Reset();
                    this.UltraCalendarInfo.DataBindingsForAppointments.SetDataBinding(oDsAppointments, "Table");
                    this.UltraCalendarInfo.DataBindingsForAppointments.BindingContextControl = this;
                    this.UltraCalendarInfo.DataBindingsForAppointments.SubjectMember = "Oggetto";
                    this.UltraCalendarInfo.DataBindingsForAppointments.DescriptionMember = "Descrizione";

                    if (this.UcAgende.IsLista == false)
                    {
                        this.UltraCalendarInfo.DataBindingsForAppointments.StartDateTimeMember = "DataInizio";
                        this.UltraCalendarInfo.DataBindingsForAppointments.EndDateTimeMember = "DataFine";
                    }
                    this.UltraCalendarInfo.DataBindingsForAppointments.AllDayEventMember = "TuttoIlGiorno";
                    this.UltraCalendarInfo.DataBindingsForAppointments.OwnerKeyMember = "CodAgenda";



                    this.UltraCalendarInfo.DataBindingsForAppointments.RefreshData();

                    spcoll = new SqlParameterExt[1];
                    xmlParam = XmlProcs.XmlSerializeToString(opWH);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                    DataSet oDsWorkingHours = Database.GetDatasetStoredProc("MSP_SelAgendePeriodi", spcoll);

                    this.UltraCalendarInfo.DateSettings.Clear();
                    foreach (DataRow oDr in oDsWorkingHours.Tables[0].Rows)
                    {

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

                                    CalendarDateSettings cds = this.UltraCalendarInfo.DateSettings[dtDayOfWeek];
                                    if (cds == null)
                                    {
                                        cds = new CalendarDateSettings(dtDayOfWeek);
                                        this.UltraCalendarInfo.DateSettings.Add(cds);
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

                    this.SetUltraToolbars();
                    if (this.UcAgende.IsLista)
                    {
                        this.ucAgendaListaView.Dock = DockStyle.Fill;
                        this.ucAgendaListaView.Visible = true;
                        this.ucAgendaListaView.ViewCodAgenda = this.UcAgende.CodAgenda;
                        this.ucAgendaListaView.ViewDescrizioneAgenda = this.UcAgende.Agenda;
                        this.ucAgendaListaView.ViewParametriLista = this.UcAgende.ParametriLista;
                        this.ucAgendaListaView.ViewFiltroGenerico = _filtro;
                        this.ucAgendaListaView.RefreshData();
                        this.UltraDayView.Visible = false;
                        this.UltraWeekView.Visible = false;
                        this.UltraMonthViewSingle.Visible = false;
                    }
                    else
                    {
                        if (this.UltraToolbarsManager.OptionSets["DateButtons"].SelectedTool != null)
                        {
                            this.ActionToolClick(this.UltraToolbarsManager.OptionSets["DateButtons"].SelectedTool);
                        }
                        this.ucAgendaListaView.Dock = DockStyle.None;
                        this.ucAgendaListaView.Visible = false;
                    }

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "LoadUltraCalendarInfo", "ucCalendario");
            }

        }

        private Owner GetOwner()
        {

            Owner oOwner = null;

            try
            {


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


        private void CambiaGiorno(bool indietro)
        {
            try
            {
                _buttonChangeDate = true;

                int numGiorni = 1;
                if (indietro) numGiorni = -1;

                DateTime dtRangeInizio = DateTime.MinValue;
                DateTime dtRangeFine = DateTime.MinValue;
                int rangeDays = 0;
                if (this.UltraCalendarInfo.SelectedDateRanges.Count == 1)
                {
                    dtRangeInizio = this.UltraCalendarInfo.SelectedDateRanges[0].StartDate;
                    dtRangeFine = this.UltraCalendarInfo.SelectedDateRanges[0].EndDate;
                    rangeDays = dtRangeFine.Subtract(dtRangeInizio).Days;
                }

                DateTime newDate = this.UltraCalendarInfo.ActiveDay.Date.AddDays(numGiorni);
                if (rangeDays > 0)
                {
                    if (indietro)
                        newDate = dtRangeInizio.AddDays(-7);
                    else
                        newDate = dtRangeInizio.AddDays(7);
                }

                Infragistics.Win.UltraWinSchedule.Day day = this.UltraCalendarInfo.GetDay(newDate, true);
                this.UltraCalendarInfo.ActiveDay = day;


                this.UltraCalendarInfo.ActiveDay = day;

                this.UltraCalendarInfo.SelectedDateRanges.Clear();
                DateTime _date = (this.UltraCalendarInfo.ActiveDay != null ? this.UltraCalendarInfo.ActiveDay.Date : DateTime.Today);
                this.UltraCalendarInfo.SelectedDateRanges.Add(_date, rangeDays);



            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, @"CambiaGiorno", this.Name);
            }
            finally
            {
                _buttonChangeDate = false;
            }
        }

        private void CambiaSettimana(bool indietro)
        {
            try
            {
                _buttonChangeDate = true;

                int numGiorni = 7;
                if (indietro) numGiorni = -7;


                DateTime newDate = this.UltraCalendarInfo.ActiveDay.Date.AddDays(numGiorni);

                Infragistics.Win.UltraWinSchedule.Day day = this.UltraCalendarInfo.GetDay(newDate, true);
                this.UltraCalendarInfo.ActiveDay = day;


                this.UltraCalendarInfo.ActiveDay = day;

                this.UltraCalendarInfo.SelectedDateRanges.Clear();
                DateTime _date = (this.UltraCalendarInfo.ActiveDay != null ? this.UltraCalendarInfo.ActiveDay.Date : DateTime.Today);
                this.UltraCalendarInfo.SelectedDateRanges.Add(_date, 0);



            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, @"CambiaSettimana", this.Name);
            }
            finally
            {
                _buttonChangeDate = false;
            }
        }

        private void CambiaMese(bool indietro)
        {
            try
            {
                _buttonChangeDate = true;

                int numMesi = 1;
                if (indietro) numMesi = -1;

                DateTime newDate = this.UltraCalendarInfo.ActiveDay.Date.AddMonths(numMesi);
                newDate = new DateTime(newDate.Year, newDate.Month, 1);

                Infragistics.Win.UltraWinSchedule.Day day = this.UltraCalendarInfo.GetDay(newDate, true);
                this.UltraCalendarInfo.ActiveDay = day;


                this.UcUltraMonthViewMulti.UltraMonthViewMulti.EnsureVisible(day.Month);
                DateTime endDate = newDate.AddMonths(1).AddDays(-1);
                this.UltraCalendarInfo.ActiveDay = day;

                this.UltraCalendarInfo.SelectedDateRanges.Clear();
                this.UltraCalendarInfo.SelectedDateRanges.Add(newDate, endDate, true);

                this.UltraMonthViewSingle.ScrollDayIntoView(newDate, true);
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, @"CambiaMese", this.Name);
            }
            finally
            {
                _buttonChangeDate = false;
            }
        }

        private void InitializeUltraSchedulePrintDocument()
        {
            CoreStatics.SetUltraSchedulePrintDocument(this.UltraSchedulePrintDocument);
        }

        private void InitializePrintDocumentWithDisplay(bool bType)
        {

            this.UltraSchedulePrintDocument.Reset(true);
            this.InitializeUltraSchedulePrintDocument();
            if (bType)
            {
                this.UltraSchedulePrintDocument.PrintColorStyle = Infragistics.Win.ColorRenderMode.Color;
            }
            else
            {
                this.UltraSchedulePrintDocument.PrintColorStyle = Infragistics.Win.ColorRenderMode.Monochrome;
            }
            this.UltraSchedulePrintDocument.ExcludeNonWorkingDays = false;
            if (this.UltraDayView.Visible == true)
            {
                this.UltraSchedulePrintDocument.IncludeDateHeaderArea = false;
                this.UltraSchedulePrintDocument.PrintStyle = SchedulePrintStyle.Daily;
                this.UltraSchedulePrintDocument.StartTime = new TimeSpan(this.UltraCalendarInfo.ActiveDay.DayOfWeek.WorkDayStartTime.Hour, this.UltraCalendarInfo.ActiveDay.DayOfWeek.WorkDayStartTime.Minute, this.UltraCalendarInfo.ActiveDay.DayOfWeek.WorkDayStartTime.Second).Subtract(new TimeSpan(1, 0, 0));
                this.UltraSchedulePrintDocument.EndTime = new TimeSpan(this.UltraCalendarInfo.ActiveDay.DayOfWeek.WorkDayEndTime.Hour, this.UltraCalendarInfo.ActiveDay.DayOfWeek.WorkDayEndTime.Minute, this.UltraCalendarInfo.ActiveDay.DayOfWeek.WorkDayEndTime.Second).Add(new TimeSpan(1, 0, 0));

            }
            else if (this.UltraWeekView.Visible == true)
            {
                this.UltraSchedulePrintDocument.IncludeDateHeaderArea = false;
                this.UltraSchedulePrintDocument.PrintStyle = SchedulePrintStyle.Weekly;
                this.UltraSchedulePrintDocument.WeeklyLayoutStyle = WeeklyLayoutStyle.WeekView;
            }
            else if (this.UltraMonthViewSingle.Visible == true)
            {
                this.UltraSchedulePrintDocument.IncludeDateHeaderArea = false;
                this.UltraSchedulePrintDocument.PrintStyle = SchedulePrintStyle.Monthly;
            }

        }

        #endregion

        #region Public Method

        public Appointment GetAppointments(string ID)
        {

            foreach (Appointment _App in this.UltraCalendarInfo.Appointments)
            {
                if (((System.Data.DataRowView)_App.BindingListObject)["IDAppuntamento"].ToString() == ID)
                {
                    return _App;
                }
            }

            return null;

        }

        public void RefreshData(string filtro)
        {

            _filtro = filtro;

            try
            {
                this.LoadUltraCalendarInfo();
                if (SelectedCalendario != null) { SelectedCalendario(this.UltraCalendarInfo, new CalendarioEventArgs(this.GetOwner())); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        public void StampaCalendario(bool bColor = false)
        {

            if (this.UcAgende.IsLista == false)
            {
                this.InitializePrintDocumentWithDisplay(bColor);
                this.PrintPreviewDialog.Text = this.Text;
                this.PrintPreviewDialog.WindowState = FormWindowState.Maximized;
                this.PrintPreviewDialog.ShowDialog(this);
            }
            else
            {
                this.ucAgendaListaView.ugAppuntamenti.PrintPreview(Infragistics.Win.UltraWinGrid.RowPropertyCategories.All);
            }

        }

        #endregion

        #region Events

        private void UltraToolbarsManager_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {
            this.ActionBeforeToolDropdown(e.Tool);
        }

        private void UltraToolbarsManager_ToolClick(object sender, ToolClickEventArgs e)
        {
            this.ActionToolClick(e.Tool);
        }

        private void UltraCalendarInfo_AfterActiveDayChanged(object sender, AfterActiveDayChangedEventArgs e)
        {

            if (e.Day.Date < _RangeStartDate)
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);
                _RangeStartDate = e.Day.Date;
                this.LoadUltraCalendarInfo();
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
            }
            if (e.Day.Date > _RangeEndDate)
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);
                _RangeEndDate = e.Day.Date;
                this.LoadUltraCalendarInfo();
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
            }

            _calendario.ActiveDay = e.Day.Date;

            if (SelectedCalendario != null) { SelectedCalendario(this.UltraCalendarInfo, new CalendarioEventArgs(this.GetOwner())); }

        }

        private void UltraCalendarInfo_AfterSelectedAppointmentsChange(object sender, EventArgs e)
        {
            if (SelectedCalendario != null) { SelectedCalendario(this.UltraCalendarInfo, new CalendarioEventArgs(this.GetOwner())); }
        }

        private void UltraCalendarInfo_BeforeSelectedAppointmentsChange(object sender, BeforeSelectedAppointmentsEventArgs e)
        {
            if (e.NewSelectedAppointments.Count > 0) { this.UltraCalendarInfo.ActiveDay = e.NewSelectedAppointments[0].Day; }
        }

        private void UltraCalendarInfo_AppointmentDataInitialized(object sender, AppointmentDataInitializedEventArgs e)
        {
            if (CalendarInfoAppointmentDataInitialized != null) { CalendarInfoAppointmentDataInitialized(sender, e); }
        }

        private void UltraCalendarInfo_OwnerDataInitialized(object sender, OwnerDataInitializedEventArgs e)
        {
            if (CalendarInfoOwnerDataInitialized != null) { CalendarInfoOwnerDataInitialized(sender, e); }
        }

        private void UltraPanelControl_Enter(object sender, EventArgs e)
        {
            this.UltraPanelControl.Appearance.BackColor = Color.Yellow;
            if (SelectedCalendario != null) { SelectedCalendario(this.UltraCalendarInfo, new CalendarioEventArgs(this.GetOwner())); }
        }

        private void UltraPanelControl_Leave(object sender, EventArgs e)
        {
            this.UltraPanelControl.Appearance.BackColor = Color.Empty;
        }

        private void UcUltraMonthViewMulti_ButtonSeleziona(object sender, EventArgs e)
        {
            _calendario.ActiveDay = this.UltraCalendarInfo.ActiveDay.Date;
            ((PopupControlContainerTool)this.UltraToolbarsManager.Tools["Data"]).ClosePopup();
        }

        private void UcUltraMonthViewMulti_BeforeMonthScroll(object sender, BeforeMonthScrollEventArgs e)
        {

            DateTime _StartDate = new DateTime(e.NewFirstMonth.Year.YearNumber, e.NewFirstMonth.MonthNumber, 1);
            DateTime _EndDate = new DateTime(e.NewFirstMonth.Year.YearNumber, e.NewFirstMonth.MonthNumber, e.NewFirstMonth.DaysInMonth);

            if (_StartDate < _RangeStartDate)
            {
                _RangeStartDate = _StartDate;
                this.LoadUltraCalendarInfo(true);
            }
            else if (_EndDate > _RangeEndDate)
            {
                _RangeEndDate = _EndDate;
                this.LoadUltraCalendarInfo(true);
            }
            else
            {
                this.LoadUltraCalendarInfo(true);
            }

        }

        private void UcUltraMonthViewMulti_VisibleMonthsChanged(object sender, EventArgs e)
        {



        }

        private void UcAgende_Agende_AfterRowActivate(object sender, EventArgs e)
        {

        }

        private void UcAgende_ButtonSeleziona(object sender, EventArgs e)
        {

            _calendario.CodAgenda = this.UcAgende.CodAgenda;
            this.LoadUltraCalendarInfo();
            if (SelectedCalendario != null) { SelectedCalendario(this.UltraCalendarInfo, new CalendarioEventArgs(this.GetOwner())); }
            ((PopupControlContainerTool)this.UltraToolbarsManager.Tools["Agende"]).ClosePopup();

        }

        private void UltraDayView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                e.SuppressKeyPress = true;
            }
        }

        private void UltraWeekView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right
                || e.KeyCode == Keys.Up || e.KeyCode == Keys.Down
                || e.KeyCode == Keys.PageUp || e.KeyCode == Keys.PageDown)
            {
                e.SuppressKeyPress = true;
            }
        }

        private void UltraMonthViewSingle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right
                || e.KeyCode == Keys.Up || e.KeyCode == Keys.Down
                || e.KeyCode == Keys.PageUp || e.KeyCode == Keys.PageDown)
            {
                e.SuppressKeyPress = true;
            }
        }

        private void ucAgendaListaView_UltraGridAfterRowActivate(object sender, EventArgs e)
        {

            try
            {

                this.UltraCalendarInfo.EventManager.SetEnabled(CalendarInfoEventIds.BeforeSelectedAppointmentsChange, false);
                this.UltraCalendarInfo.EventManager.SetEnabled(CalendarInfoEventIds.AfterSelectedAppointmentsChange, false);
                this.UltraCalendarInfo.SelectedAppointments.Clear();
                if (((ucAgendaListaView)sender).ugAppuntamenti.ActiveRow != null && ((ucAgendaListaView)sender).ugAppuntamenti.ActiveRow.IsDataRow)
                {
                    this.UltraCalendarInfo.SelectedAppointments.Add(this.GetAppointments(((ucAgendaListaView)sender).ugAppuntamenti.ActiveRow.Cells["IDAppuntamento"].Text));
                }
                if (SelectedCalendario != null) { SelectedCalendario(this.UltraCalendarInfo, new CalendarioEventArgs(this.GetOwner())); }
                this.UltraCalendarInfo.EventManager.AllEventsEnabled = true;

            }
            catch (Exception)
            {

            }

        }

        #endregion

        #region Events Drag & Drop & Click

        private void UltraView_Click(object sender, EventArgs e)
        {

            try
            {

                Point oPoint;
                UIElement oUIElement = null;

                if (sender.GetType() == typeof(UltraDayView))
                {
                    oPoint = this.UltraDayView.PointToClient(MousePosition);
                    oUIElement = this.UltraDayView.UIElement.ElementFromPoint(oPoint);
                }
                else if (sender.GetType() == typeof(UltraWeekView))
                {
                    oPoint = this.UltraWeekView.PointToClient(MousePosition);
                    oUIElement = this.UltraWeekView.UIElement.ElementFromPoint(oPoint);
                }
                else if (sender.GetType() == typeof(UltraMonthViewSingle))
                {
                    oPoint = this.UltraMonthViewSingle.PointToClient(MousePosition);
                    oUIElement = this.UltraMonthViewSingle.UIElement.ElementFromPoint(oPoint);
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
                        if (DayNumberClick != null) { DayNumberClick(sender, new DayNumberClickEventArgs(day.Date, this.GetOwner())); }
                    }
                    else if (oUIElement.SelectableItem.GetType() == typeof(Infragistics.Win.UltraWinSchedule.VisibleDay))
                    {
                        Infragistics.Win.UltraWinSchedule.VisibleDay visibleday = (Infragistics.Win.UltraWinSchedule.VisibleDay)oUIElement.SelectableItem;
                        if (DayNumberClick != null) { DayNumberClick(sender, new DayNumberClickEventArgs(visibleday.Date, this.GetOwner())); }
                    }

                }


            }
            catch (Exception)
            {

            }

        }

        private void UltraView_DragDrop(object sender, DragEventArgs e)
        {
            if (CalendarioDragDrop != null) { CalendarioDragDrop(sender, e); }
        }

        private void UltraView_DragOver(object sender, DragEventArgs e)
        {
            if (CalendarioDragOver != null) { CalendarioDragOver(sender, e); }
        }

        private void UltraView_MouseDown(object sender, MouseEventArgs e)
        {
            if (CalendarioMouseDown != null) { CalendarioMouseDown(sender, e); }
        }

        private void UltraView_MouseMove(object sender, MouseEventArgs e)
        {
            if (CalendarioMouseMove != null) { CalendarioMouseMove(sender, e); }
        }

        #endregion


    }

}
