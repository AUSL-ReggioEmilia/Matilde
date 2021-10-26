using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.Scci.Enums;
using UnicodeSrl.ScciResource;
using Infragistics.Win.UltraWinSchedule;

namespace UnicodeSrl.ScciCore
{
    public partial class frmPUNota : frmBaseModale, Interfacce.IViewFormlModal
    {
        public frmPUNota()
        {
            InitializeComponent();
        }

        #region Interface

        public void Carica()
        {

            try
            {

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                switch (CoreStatics.CoreApplication.MovNoteAgendeSelezionata.Azione)
                {

                    case EnumAzioni.INS:
                        this.Icon = Risorse.GetIconFromResource(Risorse.GC_NOTAAGGIUNGI);
                        break;

                    case EnumAzioni.MOD:
                        this.Icon = Risorse.GetIconFromResource(Risorse.GC_NOTAMODIFICA);
                        break;

                    default:
                        this.Icon = Risorse.GetIconFromResource(Risorse.GC_NOTAAGGIUNGI);
                        break;

                }

                this.ucEasyLabelAgenda.Text = CoreStatics.CoreApplication.MovNoteAgendeSelezionata.DescrAgenda;
                this.ucEasyTextBoxOggetto.Text = CoreStatics.CoreApplication.MovNoteAgendeSelezionata.Oggetto;
                this.ucEasyTextBoxDescrizione.Text = CoreStatics.CoreApplication.MovNoteAgendeSelezionata.Descrizione;
                this.udteDataInizio.Value = CoreStatics.CoreApplication.MovNoteAgendeSelezionata.DataInizio;
                this.udteDataFine.Value = CoreStatics.CoreApplication.MovNoteAgendeSelezionata.DataFine;
                this.ucpColore.Value = CoreStatics.GetColorFromString(CoreStatics.CoreApplication.MovNoteAgendeSelezionata.Colore);
                this.chkTuttoIlGiorno.Checked = CoreStatics.CoreApplication.MovNoteAgendeSelezionata.TuttoIlGiorno;
                this.chkEscludiDisponibilita.Checked = CoreStatics.CoreApplication.MovNoteAgendeSelezionata.EscludiDisponibilita;
                this.ucEasyButtonRipetibilita.Visible = (CoreStatics.CoreApplication.MovNoteAgendeSelezionata.Azione == EnumAzioni.INS);

                this.ShowDialog();

            }
            catch (Exception)
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }

        }

        #endregion

        #region SubRoutine

        public bool Salva()
        {

            bool bReturn = false;

            try
            {

                if (ControllaValori())
                {
                    CoreStatics.CoreApplication.MovNoteAgendeSelezionata.Oggetto = this.ucEasyTextBoxOggetto.Text;
                    CoreStatics.CoreApplication.MovNoteAgendeSelezionata.Descrizione = this.ucEasyTextBoxDescrizione.Text;
                    CoreStatics.CoreApplication.MovNoteAgendeSelezionata.DataInizio = (DateTime)this.udteDataInizio.Value;
                    CoreStatics.CoreApplication.MovNoteAgendeSelezionata.DataFine = (DateTime)this.udteDataFine.Value;
                    CoreStatics.CoreApplication.MovNoteAgendeSelezionata.Colore = this.ucpColore.Value.ToString();
                    CoreStatics.CoreApplication.MovNoteAgendeSelezionata.TuttoIlGiorno = this.chkTuttoIlGiorno.Checked;
                    CoreStatics.CoreApplication.MovNoteAgendeSelezionata.EscludiDisponibilita = this.chkEscludiDisponibilita.Checked;

                    bReturn = CoreStatics.CoreApplication.MovNoteAgendeSelezionata.Salva();
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Salva", this.Text);
            }

            return bReturn;

        }

        private bool ControllaValori()
        {

            bool bOK = true;

                        if (this.ucEasyTextBoxOggetto.Text == "")
            {
                easyStatics.EasyMessageBox("Inserire Oggetto!", "Nota", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.ucEasyTextBoxOggetto.Focus();
                bOK = false;
            }

                        int result = DateTime.Compare((DateTime)this.udteDataInizio.Value, (DateTime)this.udteDataFine.Value);
            if (result >= 0)
            {
                easyStatics.EasyMessageBox("Data e ora Inizio/Fine NON corrette !", "Nota", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.udteDataInizio.Focus();
                bOK = false;
            }

            return bOK;

        }

        #endregion

        #region Events

        private void ucEasyButtonTestiPredefiniti_Click(object sender, EventArgs e)
        {

            try
            {

                CoreStatics.CoreApplication.MovNoteAgendeSelezionata.Oggetto = this.ucEasyTextBoxOggetto.Text;
                CoreStatics.CoreApplication.MovNoteAgendeSelezionata.Descrizione = this.ucEasyTextBoxDescrizione.Text;
                CoreStatics.CoreApplication.MovNoteAgendeSelezionata.Colore = this.ucpColore.Value.ToString();

                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.TestiNotePredefiniti) == System.Windows.Forms.DialogResult.OK)
                {
                    this.ucEasyTextBoxOggetto.Text = CoreStatics.CoreApplication.MovNoteAgendeSelezionata.Oggetto;
                    this.ucEasyTextBoxDescrizione.Text = CoreStatics.CoreApplication.MovNoteAgendeSelezionata.Descrizione;
                    this.ucpColore.Value = CoreStatics.GetColorFromString(CoreStatics.CoreApplication.MovNoteAgendeSelezionata.Colore);
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyButtonTestiPredefiniti_Click", this.Name);
            }

        }

        private void ucEasyButtonRipetibilita_Click(object sender, EventArgs e)
        {

            try
            {

                AppointmentRecurrence oAppointmentRecurrence = new AppointmentRecurrence();

                oAppointmentRecurrence.PatternFrequency = RecurrencePatternFrequency.Weekly;

                switch (((DateTime)this.udteDataInizio.Value).DayOfWeek)
                {

                    case System.DayOfWeek.Monday:
                        oAppointmentRecurrence.PatternDaysOfWeek = RecurrencePatternDaysOfWeek.Monday;
                        break;
                    case System.DayOfWeek.Tuesday:
                        oAppointmentRecurrence.PatternDaysOfWeek = RecurrencePatternDaysOfWeek.Tuesday;
                        break;
                    case System.DayOfWeek.Wednesday:
                        oAppointmentRecurrence.PatternDaysOfWeek = RecurrencePatternDaysOfWeek.Wednesday;
                        break;
                    case System.DayOfWeek.Thursday:
                        oAppointmentRecurrence.PatternDaysOfWeek = RecurrencePatternDaysOfWeek.Thursday;
                        break;
                    case System.DayOfWeek.Friday:
                        oAppointmentRecurrence.PatternDaysOfWeek = RecurrencePatternDaysOfWeek.Friday;
                        break;
                    case System.DayOfWeek.Saturday:
                        oAppointmentRecurrence.PatternDaysOfWeek = RecurrencePatternDaysOfWeek.Saturday;
                        break;
                    case System.DayOfWeek.Sunday:
                        oAppointmentRecurrence.PatternDaysOfWeek = RecurrencePatternDaysOfWeek.Sunday;
                        break;

                }

                oAppointmentRecurrence.PatternInterval = 1;
                oAppointmentRecurrence.RangeLimit = RecurrenceRangeLimit.LimitByDate;
                oAppointmentRecurrence.RangeStartDate = (DateTime)this.udteDataInizio.Value;
                oAppointmentRecurrence.RangeEndDate = oAppointmentRecurrence.RangeStartDate.AddDays(7);

                CoreStatics.CoreApplication.MovNoteAgendeSelezionata.AppointmentRecurrence = oAppointmentRecurrence;

                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.RicorrenzaNota) != System.Windows.Forms.DialogResult.OK)
                {
                    CoreStatics.CoreApplication.MovNoteAgendeSelezionata.AppointmentRecurrence = null;
                }                  

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyButtonRipetibilita_Click", this.Name);
            }

        }

        private void frmPUNota_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {

            try
            {

                if (this.Salva() == true)
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void frmPUNota_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        #endregion

    }
}
