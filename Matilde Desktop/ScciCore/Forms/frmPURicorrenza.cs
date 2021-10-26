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
using Infragistics.Win.UltraWinToolbars;

namespace UnicodeSrl.ScciCore
{
    public partial class frmPURicorrenza : frmBaseModale, Interfacce.IViewFormlModal
    {
        public frmPURicorrenza()
        {
            InitializeComponent();
        }

        #region Interface

        public void Carica()
        {

            try
            {

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;

                CoreStatics.SetUltraToolbarsManager(ref this.UltraToolbarsManager);

                this.UltraToolbarsManager.ImageSizeLarge = new Size(128, 128);
                this.UltraToolbarsManager.Tools["Giorno"].SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_AGENDAGIORNALIERA_256);
                this.UltraToolbarsManager.Tools["Settimana"].SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_AGENDASETTIMANALE_256);

                this.uceDaysOfWeekLun.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_NO_32);
                this.uceDaysOfWeekMar.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_NO_32);
                this.uceDaysOfWeekMer.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_NO_32);
                this.uceDaysOfWeekGio.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_NO_32);
                this.uceDaysOfWeekVen.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_NO_32);
                this.uceDaysOfWeekSab.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_NO_32);
                this.uceDaysOfWeekDom.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_NO_32);

                this.uceDaysOfWeekLun.CheckedAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_SI_32);
                this.uceDaysOfWeekMar.CheckedAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_SI_32);
                this.uceDaysOfWeekMer.CheckedAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_SI_32);
                this.uceDaysOfWeekGio.CheckedAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_SI_32);
                this.uceDaysOfWeekVen.CheckedAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_SI_32);
                this.uceDaysOfWeekSab.CheckedAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_SI_32);
                this.uceDaysOfWeekDom.CheckedAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_SI_32);

                var oRecurrence = CoreStatics.CoreApplication.MovNoteAgendeSelezionata.AppointmentRecurrence;

                if (oRecurrence.PatternFrequency == RecurrencePatternFrequency.Daily)
                {
                    ((StateButtonTool)this.UltraToolbarsManager.Tools["Giorno"]).Checked = true;
                }
                else if (oRecurrence.PatternFrequency == RecurrencePatternFrequency.Weekly)
                {
                    ((StateButtonTool)this.UltraToolbarsManager.Tools["Settimana"]).Checked = true;
                }

                this.ucSelInterval.Value = oRecurrence.PatternInterval;

                this.uceDaysOfWeekLun.Checked = (oRecurrence.PatternDaysOfWeek == RecurrencePatternDaysOfWeek.Monday);
                this.uceDaysOfWeekMar.Checked = (oRecurrence.PatternDaysOfWeek == RecurrencePatternDaysOfWeek.Tuesday);
                this.uceDaysOfWeekMer.Checked = (oRecurrence.PatternDaysOfWeek == RecurrencePatternDaysOfWeek.Wednesday);
                this.uceDaysOfWeekGio.Checked = (oRecurrence.PatternDaysOfWeek == RecurrencePatternDaysOfWeek.Thursday);
                this.uceDaysOfWeekVen.Checked = (oRecurrence.PatternDaysOfWeek == RecurrencePatternDaysOfWeek.Friday);
                this.uceDaysOfWeekSab.Checked = (oRecurrence.PatternDaysOfWeek == RecurrencePatternDaysOfWeek.Saturday);
                this.uceDaysOfWeekDom.Checked = (oRecurrence.PatternDaysOfWeek == RecurrencePatternDaysOfWeek.Sunday);

                this.udteDataInizio.Value = oRecurrence.RangeStartDate;
                this.udteDataFine.Value = oRecurrence.RangeEndDate;

                this.ShowDialog();

            }
            catch (Exception ex)
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
                CoreStatics.ExGest(ref ex, "Carica", this.Text);
            }

        }

        #endregion

        #region UltraToolbarsManager

        private void ActionToolClick(Infragistics.Win.UltraWinToolbars.ToolBase Tool)
        {

            try
            {

                switch (Tool.Key)
                {

                    case "Giorno":
                        this.lblInterval.Text = "Ogni giorno(i)";
                        break;

                    case "Settimana":
                        this.lblInterval.Text = "Ogni settimana(e)";
                        break;

                }

            }
            catch (Exception)
            {

            }

        }

        #endregion

        #region Subroutine

        public bool Salva()
        {

            bool bReturn = false;

            try
            {

                if (ControllaValori())
                {

                    var oRecurrence = CoreStatics.CoreApplication.MovNoteAgendeSelezionata.AppointmentRecurrence;

                    if (((StateButtonTool)this.UltraToolbarsManager.Tools["Giorno"]).Checked == true)
                    {
                        oRecurrence.PatternFrequency = RecurrencePatternFrequency.Daily;
                    }
                    else if (((StateButtonTool)this.UltraToolbarsManager.Tools["Settimana"]).Checked == true)
                    {
                        oRecurrence.PatternFrequency = RecurrencePatternFrequency.Weekly;
                    }

                    oRecurrence.PatternInterval = (int)this.ucSelInterval.Value;

                    if (this.uceDaysOfWeekLun.Checked == true)
                    {
                        oRecurrence.PatternDaysOfWeek = oRecurrence.PatternDaysOfWeek | RecurrencePatternDaysOfWeek.Monday;
                    }
                    if (this.uceDaysOfWeekMar.Checked == true)
                    {
                        oRecurrence.PatternDaysOfWeek = oRecurrence.PatternDaysOfWeek | RecurrencePatternDaysOfWeek.Tuesday;
                    }
                    if (this.uceDaysOfWeekMer.Checked == true)
                    {
                        oRecurrence.PatternDaysOfWeek = oRecurrence.PatternDaysOfWeek | RecurrencePatternDaysOfWeek.Wednesday;
                    }
                    if (this.uceDaysOfWeekGio.Checked == true)
                    {
                        oRecurrence.PatternDaysOfWeek = oRecurrence.PatternDaysOfWeek | RecurrencePatternDaysOfWeek.Thursday;
                    }
                    if (this.uceDaysOfWeekVen.Checked == true)
                    {
                        oRecurrence.PatternDaysOfWeek = oRecurrence.PatternDaysOfWeek | RecurrencePatternDaysOfWeek.Friday;
                    }
                    if (this.uceDaysOfWeekSab.Checked == true)
                    {
                        oRecurrence.PatternDaysOfWeek = oRecurrence.PatternDaysOfWeek | RecurrencePatternDaysOfWeek.Saturday;
                    }
                    if (this.uceDaysOfWeekDom.Checked == true)
                    {
                        oRecurrence.PatternDaysOfWeek = oRecurrence.PatternDaysOfWeek | RecurrencePatternDaysOfWeek.Sunday;
                    }

                    oRecurrence.RangeStartDate = (DateTime)this.udteDataInizio.Value;
                    oRecurrence.RangeEndDate = (DateTime)this.udteDataFine.Value;

                    bReturn = true;

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

                        if ((int)this.ucSelInterval.Value <= 0)
            {
                easyStatics.EasyMessageBox("Inserire intervallo!", "Nota", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.ucSelInterval.Focus();
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

        private void UltraToolbarsManager_ToolClick(object sender, ToolClickEventArgs e)
        {
            this.ActionToolClick(e.Tool);
        }

        private void frmPURicorrenza_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void frmPURicorrenza_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
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

        #endregion

    }
}
