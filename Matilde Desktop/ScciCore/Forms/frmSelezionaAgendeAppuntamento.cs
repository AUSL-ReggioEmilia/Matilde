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
using Infragistics.Win.UltraWinTabControl;

namespace UnicodeSrl.ScciCore
{
    public partial class frmSelezionaAgendeAppuntamento : frmBaseModale, Interfacce.IViewFormlModal
    {

        private ucEasyGrid _ucEasyGrid = null;
                private ucSegnalibri _ucSegnalibri = null;

        private bool _buttonChangeDate = false;

                private bool _toolsNavigazioneLocked = false;

        private string toolpress = string.Empty;

        public frmSelezionaAgendeAppuntamento()
        {
            InitializeComponent();
        }

        #region Interface

        public void Carica()
        {

            try
            {

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_AGENDA_16);

                this.ucEasyButtonOggi.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_AGENDAOGGI_256);
                this.ucEasyButtonGiorno.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_AGENDAGIORNALIERA_256);
                this.ucEasyButtonGiornoLav.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_AGENDAGIORNALIERALAV_256);
                this.ucEasyButtonGiornoLav5.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_AGENDAGIORNALIERALAV5_256);
                this.ucEasyButtonSettimana.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_AGENDASETTIMANALE_256);
                this.ucEasyButtonMese.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_AGENDAMENSILE_256);
                this.ucEasyButtonRefresh.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_AGGIORNA_256);
                this.ucEasyButtonDisponibilita.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_ZOOM_256);
                this.ucEasyButtonIndietro.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FRECCIASX_256);
                this.ucEasyButtonAvanti.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FRECCIADX_256);

                this.ucEasyButtonOggi.PercImageFill = this.ucEasyButtonOggi.PercImageFill;
                this.ucEasyButtonGiorno.PercImageFill = this.ucEasyButtonGiorno.PercImageFill;
                this.ucEasyButtonGiornoLav.PercImageFill = this.ucEasyButtonGiornoLav.PercImageFill;
                this.ucEasyButtonGiornoLav5.PercImageFill = this.ucEasyButtonGiornoLav5.PercImageFill;
                this.ucEasyButtonSettimana.PercImageFill = this.ucEasyButtonSettimana.PercImageFill;
                this.ucEasyButtonMese.PercImageFill = this.ucEasyButtonMese.PercImageFill;
                this.ucEasyButtonRefresh.PercImageFill = this.ucEasyButtonRefresh.PercImageFill;
                this.ucEasyButtonDisponibilita.PercImageFill = this.ucEasyButtonDisponibilita.PercImageFill;
                this.ucEasyButtonIndietro.PercImageFill = this.ucEasyButtonIndietro.PercImageFill;
                this.ucEasyButtonAvanti.PercImageFill = this.ucEasyButtonAvanti.PercImageFill;

                this.ucEasyButtonDisponibilita.ShortcutKey = Keys.D;
                this.ucEasyButtonIndietro.ShortcutKey = Keys.Subtract;
                this.ucEasyButtonAvanti.ShortcutKey = Keys.Add;
                this.ucEasyButtonDisponibilita.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
                this.ucEasyButtonIndietro.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
                this.ucEasyButtonAvanti.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;

                                this.ucEasyButtonDisponibilita.Enabled = false;
                this.usAgendeLista.Collapsed = true;
                this.usAgendeLista.Visible = false;

                this.InitializeUltraCalendar();
                this.InitializeUltraGrid();

                this.UltraMonthViewMulti.EventManager.SetEnabled(Infragistics.Win.UltraWinSchedule.MonthViewMultiEventGroups.AllEvents, false);

                this.LoadAgende();

                                Infragistics.Win.UltraWinSchedule.Day day = this.UltraCalendarInfo.GetDay(DateTime.Now, true);
                if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio > DateTime.MinValue) day = this.UltraCalendarInfo.GetDay(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio, true);
                this.UltraCalendarInfo.ActiveDay = day;

                this.SelectActiveDay();
                this.ucEasyButtonGiorno_Click(this.ucEasyButtonGiorno, new EventArgs());
                
                if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio > DateTime.MinValue && CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine > DateTime.MinValue)
                    this.UltraDayView.SelectedTimeSlotRange.SetRange(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio, CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine);
                else
                    this.UltraDayView.SelectedTimeSlotRange.SetRange(DateTime.Now, DateTime.Now.AddMinutes(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.TimeSlotInterval));

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
                this.UltraMonthViewMulti.EventManager.SetEnabled(Infragistics.Win.UltraWinSchedule.MonthViewMultiEventGroups.AllEvents, true);

                this.ShowDialog();
                if (this.DialogResult == System.Windows.Forms.DialogResult.OK)
                {

                }

            }
            catch (Exception)
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }

        }

        #endregion

        #region UltraGrid & UltraTree

        private void InitializeUltraGrid()
        {
            this.UltraGridAgendeAltre.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Large;
        }

        private void LoadAgende()
        {

            try
            {

                                this.ucEasyButtonDisponibilita.Enabled = false;
                this.ucEasyButtonIndietro.Enabled = false;
                this.ucEasyButtonAvanti.Enabled = false;

                this.utvAgende.Nodes.Clear();

                                foreach (MovAppuntamentoAgende oMaa in CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Elementi)
                {

                    UltraTreeNode oNode = new UltraTreeNode(oMaa.CodAgenda, oMaa.Descrizione);
                    oNode.Override.NodeStyle = NodeStyle.CheckBox;

                    if (oMaa.Selezionata == true)
                    {
                        oNode.CheckedState = CheckState.Checked;
                    }
                    else
                    {
                        oNode.CheckedState = CheckState.Unchecked;
                    }

                    if (oMaa.Icona != null)
                    {
                        oNode.LeftImages.Add(DrawingProcs.GetImageFromByte(oMaa.Icona, this.utvAgende.LeftImagesSize));
                    }

                    this.utvAgende.Nodes.Add(oNode);

                }
                if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Elementi.Count == 1)
                {
                    this.utvAgende.EventManager.SetEnabled(Infragistics.Win.UltraWinTree.EventGroups.AllEvents, false);
                    this.utvAgende.Nodes[0].CheckedState = CheckState.Checked;
                    this.utvAgende.EventManager.SetEnabled(Infragistics.Win.UltraWinTree.EventGroups.AllEvents, true);
                }

                                List<KeyValuePair<string, string>> oItems = new List<KeyValuePair<string, string>>();
                foreach (MovAppuntamentoAgende oMaa in CoreStatics.CoreApplication.MovAppuntamentoSelezionato.ElementiAltri)
                {
                    oItems.Add(new KeyValuePair<string, string>(oMaa.CodAgenda, oMaa.Descrizione));
                }
                this.UltraGridAgendeAltre.DataSource = oItems;
                this.UltraGridAgendeAltre.Visible = (oItems.Count == 0 ? false : true);
                this.UltraGridAgendeAltre.DisplayLayout.Bands[0].Columns[0].Hidden = true;
                this.UltraGridAgendeAltre.DisplayLayout.Bands[0].Columns[1].Header.Caption = "Altre Agende";
                this.UltraGridAgendeAltre.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
                this.UltraGridAgendeAltre.Refresh();
                this.UltraGridAgendeAltre.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);
                this.UltraGridAgendeAltre.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

                                        private bool getRicercaDisponibilitaAbilitata()
        {
            bool bAbilita = false;
            try
            {
                int iCheckedNodesCount = 0;
                foreach (UltraTreeNode oNode in this.utvAgende.Nodes)
                {
                    if (oNode.CheckedState == CheckState.Checked)
                    {
                        iCheckedNodesCount += 1;
                    }
                }

                bAbilita = (iCheckedNodesCount == 1);
            }
            catch (Exception ex)
            {
                bAbilita = false;
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }
            return bAbilita;
        }

                                        private bool AgendeSelezionate()
        {
            bool bSelezione = false;
            try
            {
                foreach (UltraTreeNode oNode in this.utvAgende.Nodes)
                {
                    if (oNode.CheckedState == CheckState.Checked)
                    {
                        bSelezione = true;
                        break;
                    }
                }

            }
            catch (Exception ex)
            {
                bSelezione = false;
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }
            return bSelezione;
        }

        #endregion

        #region UltraCalender

        private void InitializeUltraCalendar()
        {

            CoreStatics.SetUltraDayView(this.UltraDayView);
                                    CoreStatics.SetUltraWeekView(this.UltraWeekView);
                        CoreStatics.SetUltraMonthViewSingle(this.UltraMonthViewSingle);
                        CoreStatics.SetUltraMonthViewMulti(this.UltraMonthViewMulti);
            CoreStatics.SetUltraCalendarInfo(this.UltraCalendarInfo);
            CoreStatics.SetUltraCalendarLook(this.UltraCalendarLook);

        }

        private void LoadCalendari()
        {

            try
            {
                                this.ucEasyButtonDisponibilita.Enabled = (!_toolsNavigazioneLocked && getRicercaDisponibilitaAbilitata());

                this.ucEasyButtonIndietro.Enabled = (!_toolsNavigazioneLocked && AgendeSelezionate());
                this.ucEasyButtonAvanti.Enabled = (!_toolsNavigazioneLocked && AgendeSelezionate());

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);

                                Parametri opWH = new Parametri(CoreStatics.CoreApplication.Ambiente);

                List<string> oD = new List<string>();

                foreach (UltraTreeNode oNode in this.utvAgende.Nodes)
                {
                    if (oNode.CheckedState == CheckState.Checked)
                    {
                        oD.Add(oNode.Key);
                    }
                }

                                if (oD.Count == 0)
                {
                    oD.Add("__NOAGENDA__");
                }
                    
                op.ParametroRipetibile.Add("CodAgenda", oD.ToArray());
                opWH.ParametroRipetibile.Add("CodAgenda", oD.ToArray());
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
                op.Parametro.Add("DatiEstesi", "1");
                op.Parametro.Add("Lista", "0");
                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataSet oDsOwners = Database.GetDatasetStoredProc("MSP_SelMovAppuntamentiAgende", spcoll);
                                                                op.Parametro.Remove("Lista");
                op.Parametro.Add("Lista", "1");
                spcoll = new SqlParameterExt[1];
                xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataSet oDsOwnersLista = Database.GetDatasetStoredProc("MSP_SelMovAppuntamentiAgende", spcoll);

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
                op.Parametro.Remove("Lista");
                op.Parametro.Add("Lista", "0");
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

                if (oDsOwners.Tables[0].Rows.Count == 0 && oDsOwnersLista.Tables[0].Rows.Count == 0)
                {
                                        this.ResetAgende();
                    this.UltraDayView.Visible = (toolpress == "Giorno" || toolpress == "GiornoLav" || toolpress == "GiornoLav5" ? true : false);
                    this.UltraWeekView.Visible = (toolpress == "Settimana" ? true : false);
                    this.UltraMonthViewSingle.Visible = (toolpress == "Mese" ? true : false);
                    this.UltraDayView.Dock = (toolpress == "Giorno" || toolpress == "GiornoLav" || toolpress == "GiornoLav5" ? DockStyle.Fill : DockStyle.None);
                    this.UltraWeekView.Dock = (toolpress == "Settimana" ? DockStyle.Fill : DockStyle.None);
                    this.UltraMonthViewSingle.Dock = (toolpress == "Mese" ? DockStyle.Fill : DockStyle.None);
                    this.utcAgendeLista.Dock = DockStyle.Bottom;
                    this.usAgendeLista.Collapsed = true;
                    this.usAgendeLista.Visible = false;
                    this.ucEasyTableLayoutPanelTop.Enabled = false;
                    Application.DoEvents();
                }
                else if (oDsOwners.Tables[0].Rows.Count > 0 && oDsOwnersLista.Tables[0].Rows.Count == 0)
                {
                                        this.ResetAgende();
                    this.UltraDayView.Visible = (toolpress == "Giorno" || toolpress == "GiornoLav" || toolpress == "GiornoLav5" ? true : false);
                    this.UltraWeekView.Visible = (toolpress == "Settimana" ? true : false);
                    this.UltraMonthViewSingle.Visible = (toolpress == "Mese" ? true : false);
                    this.UltraDayView.Dock = (toolpress == "Giorno" || toolpress == "GiornoLav" || toolpress == "GiornoLav5" ? DockStyle.Fill : DockStyle.None);
                    this.UltraWeekView.Dock = (toolpress == "Settimana" ? DockStyle.Fill : DockStyle.None);
                    this.UltraMonthViewSingle.Dock = (toolpress == "Mese" ? DockStyle.Fill : DockStyle.None);
                    this.utcAgendeLista.Dock = DockStyle.Bottom;
                    this.usAgendeLista.Collapsed = true;
                    this.usAgendeLista.Visible = false;
                    this.ucEasyTableLayoutPanelTop.Enabled = true;
                    Application.DoEvents();
                }
                else if (oDsOwners.Tables[0].Rows.Count == 0 && oDsOwnersLista.Tables[0].Rows.Count > 0)
                {
                                        this.ResetAgende();
                    this.LoadAgendeListe(oDsOwnersLista);
                    this.utcAgendeLista.Dock = DockStyle.Fill;
                    this.usAgendeLista.Collapsed = false;
                    this.usAgendeLista.Visible = true;
                    this.ucEasyTableLayoutPanelTop.Enabled = false;
                    Application.DoEvents();
                }
                else
                {
                                        this.ResetAgende();
                    this.UltraDayView.Visible = (toolpress == "Giorno" || toolpress == "GiornoLav" || toolpress == "GiornoLav5" ? true : false);
                    this.UltraWeekView.Visible = (toolpress == "Settimana" ? true : false);
                    this.UltraMonthViewSingle.Visible = (toolpress == "Mese" ? true : false);
                    this.UltraDayView.Dock = (toolpress == "Giorno" || toolpress == "GiornoLav" || toolpress == "GiornoLav5" ? DockStyle.Fill : DockStyle.None);
                    this.UltraWeekView.Dock = (toolpress == "Settimana" ? DockStyle.Fill : DockStyle.None);
                    this.UltraMonthViewSingle.Dock = (toolpress == "Mese" ? DockStyle.Fill : DockStyle.None);
                    this.LoadAgendeListe(oDsOwnersLista);
                    this.utcAgendeLista.Dock = DockStyle.Bottom;
                    this.usAgendeLista.Collapsed = false;
                    this.usAgendeLista.Visible = true;
                    this.ucEasyTableLayoutPanelTop.Enabled = true;
                    Application.DoEvents();
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void ResetAgende()
        {

            this.UltraDayView.Visible = false;
            this.UltraWeekView.Visible = false;
            this.UltraMonthViewSingle.Visible = false;
            this.UltraDayView.Dock = DockStyle.None;
            this.UltraWeekView.Dock = DockStyle.None;
            this.UltraMonthViewSingle.Dock = DockStyle.None;
            Application.DoEvents();

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
                else if (this.UltraWeekView.Visible == true)
                {
                    oOwner = this.UltraWeekView.ActiveOwner;
                }
                else if (this.UltraMonthViewSingle.Visible == true)
                {
                    oOwner = this.UltraMonthViewSingle.ActiveOwner;
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

        private void ResetAgendeListe()
        {

            try
            {

                for (int x = this.utcAgendeLista.Tabs.Count - 1; x >= 0; x--)
                {
                    this.utcAgendeLista.Tabs.RemoveAt(x);
                }

            }
            catch (Exception)
            {

            }

        }

        #endregion

        #region UltraCalenderGrid

        private void LoadAgendeListe(DataSet agende)
        {

            try
            {

                this.ResetAgendeListe();

                this.utcAgendeLista.BeginUpdate();

                foreach (DataRow dr in agende.Tables[0].Rows)
                {

                                                                                UltraTab newTab = utcAgendeLista.Tabs.Add(dr["Codice"].ToString(), dr["Descrizione"].ToString());

                                                                                ParametriListaAgenda mo_ParametriListaAgenda = new ParametriListaAgenda();
                    mo_ParametriListaAgenda = UnicodeSrl.Scci.Statics.XmlProcs.XmlDeserializeFromString<ParametriListaAgenda>(dr["ParametriLista"].ToString());

                                                                                if (mo_ParametriListaAgenda.TipoRaggruppamentoAgenda1 == EnumTipoRaggruppamentoAgenda.Dizionario ||
                        mo_ParametriListaAgenda.TipoRaggruppamentoAgenda2 == EnumTipoRaggruppamentoAgenda.Dizionario ||
                        mo_ParametriListaAgenda.TipoRaggruppamentoAgenda3 == EnumTipoRaggruppamentoAgenda.Dizionario)
                    {

                                                ucEasyTableLayoutPanel panel = new ucEasyTableLayoutPanel();
                        panel.Dock = DockStyle.Top;
                        panel.ColumnCount = 3;
                        panel.RowCount = 1;
                        panel.Size = new Size(100, 76);
                                                
                                                panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(SizeType.Percent, 33F));
                        panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(SizeType.Percent, 33F));
                        panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(SizeType.Percent, 33F));

                                                panel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
                        if (mo_ParametriListaAgenda.TipoRaggruppamentoAgenda1 == EnumTipoRaggruppamentoAgenda.Dizionario)
                        {
                            panel.Controls.Add(new ucEasyLabel() { Text = mo_ParametriListaAgenda.DescrizioneRaggruppamentoAgenda1, Dock = DockStyle.Fill, TextFontRelativeDimension = easyStatics.easyRelativeDimensions.Medium }, 0, 0);
                        }
                        if (mo_ParametriListaAgenda.TipoRaggruppamentoAgenda2 == EnumTipoRaggruppamentoAgenda.Dizionario)
                        {
                            panel.Controls.Add(new ucEasyLabel() { Text = mo_ParametriListaAgenda.DescrizioneRaggruppamentoAgenda2, Dock = DockStyle.Fill, TextFontRelativeDimension = easyStatics.easyRelativeDimensions.Medium }, 1, 0);
                        }
                        if (mo_ParametriListaAgenda.TipoRaggruppamentoAgenda3 == EnumTipoRaggruppamentoAgenda.Dizionario)
                        {
                            panel.Controls.Add(new ucEasyLabel() { Text = mo_ParametriListaAgenda.DescrizioneRaggruppamentoAgenda3, Dock = DockStyle.Fill, TextFontRelativeDimension = easyStatics.easyRelativeDimensions.Medium }, 2, 0);
                        }

                                                panel.RowCount = panel.RowCount + 1;
                        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
                        List<string> lstdict = new List<string>();
                        if (mo_ParametriListaAgenda.TipoRaggruppamentoAgenda1 == EnumTipoRaggruppamentoAgenda.Dizionario)
                        {

                            ucEasyComboEditor oComboEditor = new ucEasyComboEditor();
                            oComboEditor.Name = string.Format("uce|{0}|{1}", dr["Codice"].ToString(), "1");
                            oComboEditor.TextFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
                            oComboEditor.Clear();
                            oComboEditor.ValueMember = "CodValore";
                            oComboEditor.DisplayMember = "Descrizione";
                            oComboEditor.DataSource = LoadDizionario(mo_ParametriListaAgenda.RaggruppamentoAgenda1.First().Key);
                            oComboEditor.Refresh();
                            oComboEditor.Dock = DockStyle.Fill;

                            panel.Controls.Add(oComboEditor, 0, panel.RowCount - 1);

                            lstdict.Add(oComboEditor.Name);

                        }
                        if (mo_ParametriListaAgenda.TipoRaggruppamentoAgenda2 == EnumTipoRaggruppamentoAgenda.Dizionario)
                        {

                            ucEasyComboEditor oComboEditor = new ucEasyComboEditor();
                            oComboEditor.Name = string.Format("uce|{0}|{1}", dr["Codice"].ToString(), "2");
                            oComboEditor.TextFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
                            oComboEditor.Clear();
                            oComboEditor.ValueMember = "CodValore";
                            oComboEditor.DisplayMember = "Descrizione";
                            oComboEditor.DataSource = LoadDizionario(mo_ParametriListaAgenda.RaggruppamentoAgenda2.First().Key);
                            oComboEditor.Refresh();

                            oComboEditor.Dock = DockStyle.Fill;

                            panel.Controls.Add(oComboEditor, 1, panel.RowCount - 1);

                            lstdict.Add(oComboEditor.Name);

                        }
                        if (mo_ParametriListaAgenda.TipoRaggruppamentoAgenda3 == EnumTipoRaggruppamentoAgenda.Dizionario)
                        {

                            ucEasyComboEditor oComboEditor = new ucEasyComboEditor();
                            oComboEditor.Name = string.Format("uce|{0}|{1}", dr["Codice"].ToString(), "3");
                            oComboEditor.TextFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
                            oComboEditor.Clear();
                            oComboEditor.ValueMember = "CodValore";
                            oComboEditor.DisplayMember = "Descrizione";
                            oComboEditor.DataSource = LoadDizionario(mo_ParametriListaAgenda.RaggruppamentoAgenda3.First().Key);
                            oComboEditor.Refresh();
                            oComboEditor.Dock = DockStyle.Fill;

                            panel.Controls.Add(oComboEditor, 2, panel.RowCount - 1);

                            lstdict.Add(oComboEditor.Name);

                                                    }

                                                newTab.TabPage.Controls.Add(panel);
                        panel.SendToBack();

                                                                                                foreach (string s in lstdict)
                        {

                            ucEasyComboEditor uce = this.Controls.Find(s, true).FirstOrDefault() as ucEasyComboEditor;
                            if (uce != null)
                            {

                                string[] arrkey = s.Split('|');
                                string sval = string.Empty;

                                switch (int.Parse(arrkey[2]))
                                {

                                    case 1:
                                        sval = LoadDizionarioValore(dr["Codice"].ToString()).CodRaggr1;
                                        break;

                                    case 2:
                                        sval = LoadDizionarioValore(dr["Codice"].ToString()).CodRaggr2;
                                        break;

                                    case 3:
                                        sval = LoadDizionarioValore(dr["Codice"].ToString()).CodRaggr3;
                                        break;

                                }

                                uce.Value = sval;
                            }

                        }

                    }

                                                                                ucAgendaListaView newGrid = new ucAgendaListaView();
                    newGrid.Name = string.Format("ug|{0}", dr["Codice"].ToString());
                    newGrid.ViewCodAgenda = dr["Codice"].ToString();
                    newGrid.ViewParametriLista = dr["ParametriLista"].ToString();
                    newGrid.ViewPinnedFiltri = true;
                    newGrid.ViewVisibleFiltri = true;

                    newGrid.Dock = DockStyle.Fill;
                    newGrid.ViewInit();
                    newGrid.RefreshData();

                                        newTab.TabPage.Controls.Add(newGrid);
                    newGrid.BringToFront();

                    newTab.Tag = newGrid;

                }

                this.utcAgendeLista.EndUpdate();

            }
            catch (Exception)
            {

            }

        }

                
        
                
                                                                                        
                        
        
        
        
        private DataTable LoadDizionario(string codice)
        {

            DataTable dt = null;

            try
            {

                                                                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodDizionario", codice);
                op.Parametro.Add("DatiEstesi", "0");
                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                dt = Database.GetDataTableStoredProc("MSP_SelDizionarioValori", spcoll);

            }
            catch (Exception)
            {

            }

            return dt;

        }

        private MovAppuntamentoAgende LoadDizionarioValore(string codice)
        {

            MovAppuntamentoAgende maa = new MovAppuntamentoAgende();

            try
            {

                
                MovAppuntamentoAgende oItem = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Elementi.Single(MovAppuntamentoAgende => MovAppuntamentoAgende.CodAgenda == codice);
                if (oItem != null)
                {
                    maa = oItem;
                }

            }
            catch (Exception ex)
            {

            }

            return maa;

        }

        #endregion

        #region Sub & Functions

                                        private void ConfermaSelezioni(bool skipSelezioneCalendario)
        {

            try
            {

                bool bSelect = false;
                foreach (UltraTreeNode oNode in this.utvAgende.Nodes)
                {

                    MovAppuntamentoAgende oItem = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Elementi.Single(MovAppuntamentoAgende => MovAppuntamentoAgende.CodAgenda == oNode.Key);
                    if (oItem != null)
                    {
                        if (oItem.Selezionata != (oNode.CheckedState == CheckState.Checked))
                        {
                            oItem.Selezionata = (oNode.CheckedState == CheckState.Checked);
                            oItem.Modificata = true;
                        }
                        if (oItem.Selezionata) { ConfermaSelezioneAgenda(ref oItem); }
                    }

                    if (oNode.CheckedState == CheckState.Checked) { bSelect = true; }

                }

                if (bSelect == true)
                {
                    if (!skipSelezioneCalendario)
                    {
                        if (this.UltraDayView.Visible == true && CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Azione == EnumAzioni.INS)
                        {
                                                                                                                                                                                                                                DateTime dtstart = this.UltraDayView.SelectedTimeSlotRange.StartDateTime;                             DateTime dtend = this.UltraDayView.SelectedTimeSlotRange.EndDateTime;                                                         if (this.UltraCalendarInfo.SelectedAppointments.Count == 1 && dtstart.ToString("HHmm") == @"0800")
                            {
                                dtstart = this.UltraCalendarInfo.SelectedAppointments[0].StartDateTime;
                                dtend = this.UltraCalendarInfo.SelectedAppointments[0].EndDateTime;
                            }

                            CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio =
                                new DateTime(this.UltraCalendarInfo.ActiveDay.Date.Year,
                                                this.UltraCalendarInfo.ActiveDay.Date.Month,
                                                this.UltraCalendarInfo.ActiveDay.Date.Day,
                                                dtstart.Hour,
                                                dtstart.Minute,
                                                0);
                            if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.TimeSlotInterval != 0)
                            {
                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio.AddMinutes(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.TimeSlotInterval);
                            }
                            else
                            {
                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine =
                                    new DateTime(this.UltraCalendarInfo.ActiveDay.Date.Year,
                                                    this.UltraCalendarInfo.ActiveDay.Date.Month,
                                                    this.UltraCalendarInfo.ActiveDay.Date.Day,
                                                    dtend.Hour,
                                                    dtend.Minute,
                                                    0);
                            }
                        }
                        else
                        {
                            TimeSpan oTs = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine - CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio;

                                                                                                                                                                                                                                DateTime dtstart = this.UltraDayView.SelectedTimeSlotRange.StartDateTime;                                                         if (this.UltraCalendarInfo.SelectedAppointments.Count == 1 && dtstart.ToString("HHmm") == @"0800")
                                dtstart = this.UltraCalendarInfo.SelectedAppointments[0].StartDateTime;

                            CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio =
                               new DateTime(this.UltraCalendarInfo.ActiveDay.Date.Year,
                                               this.UltraCalendarInfo.ActiveDay.Date.Month,
                                               this.UltraCalendarInfo.ActiveDay.Date.Day,
                                               dtstart.Hour,
                                               dtstart.Minute,
                                               0);
                            CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio.Add(oTs);

                        }
                    }

                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }

            }
            catch (Exception ex)
            {
                                CoreStatics.ExGest(ref ex, "ConfermaSelezioni", this.Name);
            }
        }

        private void ConfermaSelezioneAgenda(ref MovAppuntamentoAgende maa)
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodAgenda", maa.CodAgenda);
                op.Parametro.Add("DatiEstesi", "1");
                op.Parametro.Add("Lista", "1");
                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataTable oDt = Database.GetDataTableStoredProc("MSP_SelMovAppuntamentiAgende", spcoll);

                if (oDt.Rows.Count == 1)
                {

                    ParametriListaAgenda mo_ParametriListaAgenda = new ParametriListaAgenda();
                    mo_ParametriListaAgenda = UnicodeSrl.Scci.Statics.XmlProcs.XmlDeserializeFromString<ParametriListaAgenda>(oDt.Rows[0]["ParametriLista"].ToString());

                    switch (mo_ParametriListaAgenda.TipoRaggruppamentoAgenda1)
                    {

                        case EnumTipoRaggruppamentoAgenda.Nessuno:
                        case EnumTipoRaggruppamentoAgenda.Campo:
                            maa.CodRaggr1 = "";
                            maa.DescrRaggr1 = "";
                            break;

                        case EnumTipoRaggruppamentoAgenda.Dizionario:
                            ucEasyComboEditor uce = this.Controls.Find(string.Format("uce|{0}|{1}", maa.CodAgenda, "1"), true).FirstOrDefault() as ucEasyComboEditor;
                            if (uce != null)
                            {
                                if (maa.CodRaggr1 != uce.Value.ToString() || maa.DescrRaggr1 != uce.Text)
                                {
                                    maa.CodRaggr1 = uce.Value.ToString();
                                    maa.DescrRaggr1 = uce.Text;
                                    maa.Modificata = true;
                                }
                            }
                            break;

                        case EnumTipoRaggruppamentoAgenda.Scheda:
                            break;

                    }

                    switch (mo_ParametriListaAgenda.TipoRaggruppamentoAgenda2)
                    {

                        case EnumTipoRaggruppamentoAgenda.Nessuno:
                        case EnumTipoRaggruppamentoAgenda.Campo:
                            maa.CodRaggr2 = "";
                            maa.DescrRaggr2 = "";
                            break;

                        case EnumTipoRaggruppamentoAgenda.Dizionario:
                            ucEasyComboEditor uce = this.Controls.Find(string.Format("uce|{0}|{1}", maa.CodAgenda, "2"), true).FirstOrDefault() as ucEasyComboEditor;
                            if (uce != null)
                            {
                                if (maa.CodRaggr2 != uce.Value.ToString() || maa.DescrRaggr2 != uce.Text)
                                {
                                    maa.CodRaggr2 = uce.Value.ToString();
                                    maa.DescrRaggr2 = uce.Text;
                                    maa.Modificata = true;
                                }
                            }
                            break;

                        case EnumTipoRaggruppamentoAgenda.Scheda:
                            break;

                    }

                    switch (mo_ParametriListaAgenda.TipoRaggruppamentoAgenda3)
                    {

                        case EnumTipoRaggruppamentoAgenda.Nessuno:
                        case EnumTipoRaggruppamentoAgenda.Campo:
                            maa.CodRaggr3 = "";
                            maa.DescrRaggr3 = "";
                            break;

                        case EnumTipoRaggruppamentoAgenda.Dizionario:
                            ucEasyComboEditor uce = this.Controls.Find(string.Format("uce|{0}|{1}", maa.CodAgenda, "3"), true).FirstOrDefault() as ucEasyComboEditor;
                            if (uce != null)
                            {
                                if (maa.CodRaggr3 != uce.Value.ToString() || maa.DescrRaggr3 != uce.Text)
                                {
                                    maa.CodRaggr3 = uce.Value.ToString();
                                    maa.DescrRaggr3 = uce.Text;
                                    maa.Modificata = true;
                                }
                            }
                            break;

                        case EnumTipoRaggruppamentoAgenda.Scheda:
                            break;

                    }

                }

            }
            catch (Exception)
            {

            }

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

                                
                                this.UltraCalendarInfo.ActiveDay = day;

                                this.UltraCalendarInfo.SelectedDateRanges.Clear();
                this.UltraCalendarInfo.SelectedDateRanges.Add(newDate, 0);



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

                                        private void LockToolsNavigazione(bool bLocked)
        {

            try
            {
                _toolsNavigazioneLocked = bLocked;
                if (bLocked)
                    this.ImpostaCursore(enum_app_cursors.WaitCursor);
                else
                    this.ImpostaCursore(enum_app_cursors.DefaultCursor);

                this.ucEasyTableLayoutPanelTop.SuspendLayout();

                this.ucEasyButtonGiorno.Enabled = !bLocked;
                this.ucEasyButtonGiornoLav.Enabled = !bLocked;
                this.ucEasyButtonGiornoLav5.Enabled = !bLocked;
                this.ucEasyButtonMese.Enabled = !bLocked;
                this.ucEasyButtonOggi.Enabled = !bLocked;
                this.ucEasyButtonRefresh.Enabled = !bLocked;
                this.ucEasyButtonSettimana.Enabled = !bLocked;

                if (bLocked)
                {
                                        this.ucEasyButtonDisponibilita.Enabled = false;
                    this.ucEasyButtonIndietro.Enabled = false;
                    this.ucEasyButtonAvanti.Enabled = false;
                }
                else
                {
                                        this.ucEasyButtonDisponibilita.Enabled = getRicercaDisponibilitaAbilitata();
                    this.ucEasyButtonIndietro.Enabled = AgendeSelezionate();
                    this.ucEasyButtonAvanti.Enabled = AgendeSelezionate();
                }

            }
            catch
            {
                _toolsNavigazioneLocked = false;
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }
            finally
            {
                this.ucEasyTableLayoutPanelTop.ResumeLayout();
            }
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
            ConfermaSelezioni(false);
        }

        private void frmSelezionaTipoAppuntamento_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        #endregion

        #region Events

        private void UltraMonthViewMulti_BeforeMonthScroll(object sender, Infragistics.Win.UltraWinSchedule.BeforeMonthScrollEventArgs e)
        {
            this.UltraMonthViewMulti.Enabled = false;
            try
            {
                Infragistics.Win.UltraWinSchedule.Day day = this.UltraCalendarInfo.GetDay(new DateTime(e.NewFirstMonth.Year.YearNumber, e.NewFirstMonth.MonthNumber, 1), true);
                this.UltraCalendarInfo.ActiveDay = day;
                if (_buttonChangeDate)
                {
                    this.UltraCalendarInfo.SelectedDateRanges.Clear();
                    DateTime _date = (this.UltraCalendarInfo.ActiveDay != null ? this.UltraCalendarInfo.ActiveDay.Date : DateTime.Today);
                    this.UltraCalendarInfo.SelectedDateRanges.Add(_date, 0);
                }

                this.LoadCalendari();
            }
            catch (Exception)
            {

            }
            finally
            {
                this.UltraMonthViewMulti.Enabled = true;
            }
        }

        private void UltraCalendarInfo_AfterActiveDayChanged(object sender, Infragistics.Win.UltraWinSchedule.AfterActiveDayChangedEventArgs e)
        {
            if (!_buttonChangeDate)
            {
                this.SelectActiveDay();
                                            }
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

        private void utvAgende_AfterCheck(object sender, NodeEventArgs e)
        {
            this.LoadCalendari();
        }

        private void ucEasyButtonOggi_Click(object sender, EventArgs e)
        {

            Infragistics.Win.UltraWinSchedule.Day day = this.UltraCalendarInfo.ActiveDay;
            day = this.UltraCalendarInfo.GetDay(DateTime.Today, true);
            this.UltraCalendarInfo.ActiveDay = day;
            this.SelectActiveDay();
            this.UltraMonthViewMulti.EnsureVisible(day.Month);
            this.LoadCalendari();

        }

        private void ucEasyButtonGiorno_Click(object sender, EventArgs e)
        {

            toolpress = "Giorno";
            this.UltraDayView.Visible = true;
            this.UltraWeekView.Visible = false;
            this.UltraMonthViewSingle.Visible = false;
            this.UltraDayView.Dock = DockStyle.Fill;
            this.UltraDayView.EnsureTimeSlotVisible(8, 0, true);
            this.LoadCalendari();

        }

        private void ucEasyButtonGiornoLav5_Click(object sender, EventArgs e)
        {
            toolpress = "GiornoLav5";
            this.UltraDayView.Visible = true;
            this.UltraWeekView.Visible = false;
            this.UltraMonthViewSingle.Visible = false;
            this.UltraDayView.Dock = DockStyle.Fill;

            this.UltraCalendarInfo.SelectedDateRanges.Clear();
            DateTime _date = (this.UltraCalendarInfo.ActiveDay != null ? this.UltraCalendarInfo.ActiveDay.Date : DateTime.Today);
            _date = _date.AddDays(((int)_date.DayOfWeek - 1) * -1);
            this.UltraCalendarInfo.SelectedDateRanges.Add(_date, 4);
            this.UltraDayView.EnsureTimeSlotVisible(8, 0, true);
            this.LoadCalendari();
        }


        private void ucEasyButtonGiornoLav_Click(object sender, EventArgs e)
        {

            toolpress = "GiornoLav";
            this.UltraDayView.Visible = true;
            this.UltraWeekView.Visible = false;
            this.UltraMonthViewSingle.Visible = false;
            this.UltraDayView.Dock = DockStyle.Fill;

            this.UltraCalendarInfo.SelectedDateRanges.Clear();
            DateTime _date = (this.UltraCalendarInfo.ActiveDay != null ? this.UltraCalendarInfo.ActiveDay.Date : DateTime.Today);
            _date = _date.AddDays(((int)_date.DayOfWeek - 1) * -1);
            this.UltraCalendarInfo.SelectedDateRanges.Add(_date, 6);
            this.UltraDayView.EnsureTimeSlotVisible(8, 0, true);
            this.LoadCalendari();

        }

        private void ucEasyButtonSettimana_Click(object sender, EventArgs e)
        {
            toolpress = "Settimana";
            this.UltraDayView.Visible = false;
            Application.DoEvents();
                                    this.UltraWeekView.Visible = true;
            Application.DoEvents();
            this.UltraMonthViewSingle.Visible = false;
            this.UltraWeekView.Dock = DockStyle.Fill;
            this.UltraWeekView.VisibleWeek = this.UltraCalendarInfo.ActiveDay.Week;
            this.LoadCalendari();

        }

        private void ucEasyButtonMese_Click(object sender, EventArgs e)
        {
            toolpress = "Mese";
            this.UltraDayView.Visible = false;
            this.UltraWeekView.Visible = false;
            this.UltraMonthViewSingle.Visible = true;
            this.UltraMonthViewSingle.Dock = DockStyle.Fill;
            this.LoadCalendari();
        }

        private void ucEasyButtonRefresh_Click(object sender, EventArgs e)
        {
            this.LoadCalendari();

        }

        private void ucEasyButtonDisponibilita_Click(object sender, EventArgs e)
        {
            string sNomeMaschera = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;

            try
            {
                                                
                                bool bRicercaAbilitata = true;
                bRicercaAbilitata = getRicercaDisponibilitaAbilitata();

                if (bRicercaAbilitata)
                {

                    this.ImpostaCursore(enum_app_cursors.WaitCursor);

                    CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione = @"Cerca Prima Disponibilità";
                    frmSelezionaAgendeDisponibilita frm = new frmSelezionaAgendeDisponibilita();
                    foreach (UltraTreeNode oNode in this.utvAgende.Nodes)
                    {
                        if (oNode.CheckedState == CheckState.Checked)
                        {
                            frm.AgendaSelezionataCodice = oNode.Key;
                            frm.AgendaSelezionataDescrizione = oNode.Text;
                        }
                    }
                    frm.Carica();
                    frm.ShowDialog();

                    if (frm.DialogResult == System.Windows.Forms.DialogResult.OK)
                    {
                        if (frm.DataOraInizioSelezionata > DateTime.MinValue && frm.DataOraFineSelezionata > DateTime.MinValue)
                        {

                            if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Azione == EnumAzioni.INS)
                            {
                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio = frm.DataOraInizioSelezionata;
                                if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.TimeSlotInterval != 0)
                                {
                                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio.AddMinutes(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.TimeSlotInterval);
                                }
                                else
                                {
                                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine = frm.DataOraInizioSelezionata.AddMinutes(frm.MinutiSlot);
                                }
                            }
                            else
                            {
                                TimeSpan oTs = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine - CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio;
                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio = frm.DataOraInizioSelezionata;
                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio.Add(oTs);

                            }




                                                        ConfermaSelezioni(true);

                        }
                    }

                }
                else
                    easyStatics.EasyMessageBox(@"Per cercare la prima diponibilità occorre selezionare una ed una sola agenda!", @"Ricerca Disponibilità", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, @"ucEasyButtonDisponibilita_Click", this.Name);
            }
            finally
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
                CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione = sNomeMaschera;
            }
        }

        private void ucEasyButtonAvanti_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.UltraCalendarInfo.ActiveDay != null)
                {
                                                            LockToolsNavigazione(true);

                    if (this.UltraDayView.Visible)
                    {
                                                CambiaGiorno(false);

                    }
                    else if (this.UltraWeekView.Visible)
                    {
                                                CambiaSettimana(false);
                    }
                    else if (this.UltraMonthViewSingle.Visible)
                    {
                                                CambiaMese(false);
                    }
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, @"ucEasyButtonAvanti_Click", this.Name);
            }
            finally
            {
                LockToolsNavigazione(false);
            }
        }

        private void ucEasyButtonIndietro_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.UltraCalendarInfo.ActiveDay != null)
                {
                                                            LockToolsNavigazione(true);

                    if (this.UltraDayView.Visible)
                    {
                                                CambiaGiorno(true);
                    }
                    else if (this.UltraWeekView.Visible)
                    {
                                                CambiaSettimana(true);
                    }
                    else if (this.UltraMonthViewSingle.Visible)
                    {
                                                CambiaMese(true);
                    }
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, @"ucEasyButtonIndietro_Click", this.Name);
            }
            finally
            {
                LockToolsNavigazione(false);
            }
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
