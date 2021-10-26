using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.Scci.Enums;
using UnicodeSrl.ScciResource;
using UnicodeSrl.ScciCore.Common.TimersCB;
using System.Threading;

namespace UnicodeSrl.ScciCore
{
    public partial class ucTopModale : UserControl, I_RefreshTimer_Controllo
    {

        private ucRichTextBox _ucRichTextBox = null;
        private SynchronizationContext m_sync = null;

        public event ImmagineTopClickHandler ImmagineClick;

        public ucTopModale()
        {
            InitializeComponent();
            m_sync = SynchronizationContext.Current;

        }

        #region Properties

        public string Titolo
        {
            get { return this.lblTitolo.Text; }
            set { this.lblTitolo.Text = value; }
        }

        public string CodiceMaschera
        {
            get { return this.lblCodMaschera.Text; }
            set { this.lblCodMaschera.Text = value; }
        }

        public bool PazienteVisibile
        {
            get { return this.lblPaziente.Visible; }
            set
            {
                this.lblPaziente.Visible = value;
                this.lblNosologico.Visible = value;
                this.pbAlert.Visible = value;
                this.pbAllergie.Visible = value;
                this.pbEvidenzaClinica.Visible = value;
                this.pbSegnalibri.Visible = value;
                this.pbSegnalibroAdd.Visible = value;


            }
        }

        public bool Distacco
        {
            get { return this.pbDistacco.Visible; }
            set { this.pbDistacco.Visible = value; }
        }

        #endregion

        #region Events override

        public override void Refresh()
        {

            try
            {

                var oSessione = CoreStatics.CoreApplication.Sessione;
                var oPaziente = CoreStatics.CoreApplication.Paziente;
                var oEpisodio = CoreStatics.CoreApplication.Episodio;
                var oNavigazione = CoreStatics.CoreApplication.Navigazione;

                if (oSessione.Connettivita == true)
                {
                    if (oPaziente != null && oPaziente.Attivo == true && this.PazienteVisibile == true)
                    {
                        this.pbAllergie.Visible = true;
                        if (oPaziente.Allergie.Numero > 0)
                            this.pbAllergie.Image = Risorse.GetImageFromResource(Risorse.GC_ALERTALLERGIA_256);
                        else
                            this.pbAllergie.Image = Risorse.GetImageFromResource(Risorse.GC_ALERTALLERGIA_DISABLED_256);


                        this.lblPaziente.Text = oPaziente.Descrizione;

                    }
                    else
                    {
                        this.pbAllergie.Visible = false;
                        this.lblPaziente.Text = "";
                    }


                    if (oEpisodio != null && oEpisodio.Attivo == true && this.PazienteVisibile == true)
                    {
                        this.pbAlert.Visible = true;
                        if (oEpisodio.AlertsGenerici.DaVistare > 0)
                            this.pbAlert.Image = Risorse.GetImageFromResource(Risorse.GC_ALERTGENERICO_256);
                        else
                            this.pbAlert.Image = Risorse.GetImageFromResource(Risorse.GC_ALERTGENERICO_DISABLED_256);
                        this.pbEvidenzaClinica.Visible = true;
                        if (oEpisodio.EvidenzeCliniche.DaVistare > 0)
                            this.pbEvidenzaClinica.Image = Risorse.GetImageFromResource(Risorse.GC_EVIDENZACLINICAALERT_256);
                        else
                            this.pbEvidenzaClinica.Image = Risorse.GetImageFromResource(Risorse.GC_EVIDENZACLINICAALERTDISABLE_256);
                        this.lblNosologico.Text = string.Format("N.: {0}", (oEpisodio.NumeroEpisodio != string.Empty ? oEpisodio.NumeroEpisodio : oEpisodio.NumeroListaAttesa));
                    }
                    else
                    {
                        this.pbAlert.Visible = false;
                        this.pbEvidenzaClinica.Visible = false;
                        this.lblNosologico.Text = "";
                    }

                    if (oSessione.Utente.Ruoli.RuoloSelezionato != null && this.PazienteVisibile == true)
                    {
                        if (oSessione.Utente.Ruoli.RuoloSelezionato.Bookmarks == 0)
                        {
                            this.pbSegnalibri.Image = null;
                        }
                        else
                        {
                            this.pbSegnalibri.Image = Risorse.GetImageFromResourceWithTip(Risorse.GC_SEGNALIBRI_256, oSessione.Utente.Ruoli.RuoloSelezionato.Bookmarks.ToString());
                        }
                    }
                    else
                    {
                        this.pbSegnalibri.Image = null;
                    }

                }

                if (oNavigazione != null && oNavigazione.Maschere != null && oNavigazione.Maschere.MascheraSelezionata != null)
                {
                    this.lblTitolo.Text = oNavigazione.Maschere.MascheraSelezionata.Descrizione;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            base.Refresh();

        }

        protected override void OnHandleCreated(EventArgs e)
        {
            if (this.DesignMode == false)
            {


                this.pbSegnalibri.Visible = (CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata != null && CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.SegnalibroVisualizza);
                this.pbSegnalibroAdd.Visible = (CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata != null && CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.SegnalibroAdd);
                if (this.pbSegnalibroAdd.Visible == true)
                {
                    this.pbSegnalibroAdd.Image = Risorse.GetImageFromResource(Risorse.GC_SEGNALIBROADD_256);
                }
                else
                {
                    this.pbSegnalibroAdd.Image = null;
                }
                this.pbDistacco.Image = Risorse.GetImageFromResource(Risorse.GC_DISTACCO_256);

                CoreStatics.MainWnd.RefreshControllo_Subscribers.Add(this);

            }
            base.OnHandleCreated(e);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (this.DesignMode == false)
            {
                this.pbAllergie.Image = null;
                this.pbAlert.Image = null;
                this.pbEvidenzaClinica.Image = null;
                this.pbSegnalibroAdd.Image = null;
                this.pbDistacco.Image = null;

                CoreStatics.MainWnd.RefreshControllo_Subscribers.Remove(this);
            }
            base.OnHandleDestroyed(e);
        }

        public void RemoveSubscriber()
        {
            if (this.DesignMode == false)
            {
                CoreStatics.MainWnd.RefreshControllo_Subscribers.Remove(this);
            }
        }

        #endregion

        #region Events

        private void lblPaziente_Click(object sender, EventArgs e)
        {
            if (this.lblPaziente.Text != string.Empty)
            {

                var sb = new StringBuilder();
                sb.Append(this.lblPaziente.Text);
                sb.AppendLine();
                sb.AppendLine();
                sb.Append(this.lblNosologico.Text);

                _ucRichTextBox = CoreStatics.GetRichTextBoxForPopupControlContainer(sb.ToString(), false);
                this.UltraPopupControlContainer.Show((ucEasyLabel)sender);

            }
        }

        private void lblCodMaschera_Click(object sender, EventArgs e)
        {

            try
            {

                if (CoreStatics.CoreApplication.Sessione.Utente.Admin == true)
                {

                    this.Cursor = Cursors.WaitCursor;

                    StringBuilder sb = CoreStatics.getPlaceHolder();
                    if (sb.Length > 0)
                    {
                        _ucRichTextBox = CoreStatics.GetRichTextBoxForPopupControlContainer(sb.ToString(), false);
                        _ucRichTextBox.Size = new Size(800, 600);
                        this.UltraPopupControlContainer.Show((ucEasyLabel)sender);
                    }

                    this.Cursor = Cursors.Default;

                }

            }
            catch (Exception)
            {

            }

        }

        private void pbSegnalibri_Click(object sender, EventArgs e)
        {
            if (ImmagineClick != null) { ImmagineClick(sender, new ImmagineTopClickEventArgs(EnumImmagineTop.Segnalibri)); }
        }

        private void pbSegnalibroAdd_Click(object sender, EventArgs e)
        {
            if (ImmagineClick != null) { ImmagineClick(sender, new ImmagineTopClickEventArgs(EnumImmagineTop.SegnalibroAdd)); }
        }

        private void pbDistacco_Click(object sender, EventArgs e)
        {
            if (ImmagineClick != null) { ImmagineClick(sender, new ImmagineTopClickEventArgs(EnumImmagineTop.Distacco)); }
        }

        #endregion

        #region UltraPopupControlContainer

        private void UltraPopupControlContainer_Closed(object sender, EventArgs e)
        {
            _ucRichTextBox.RtfClick -= ucRichTextBox_Click;
        }

        private void UltraPopupControlContainer_Opened(object sender, EventArgs e)
        {
            _ucRichTextBox.RtfClick += ucRichTextBox_Click;
            _ucRichTextBox.Focus();
        }

        private void UltraPopupControlContainer_Opening(object sender, CancelEventArgs e)
        {
            Infragistics.Win.Misc.UltraPopupControlContainer popup = sender as Infragistics.Win.Misc.UltraPopupControlContainer;
            popup.PopupControl = _ucRichTextBox;
        }

        private void ucRichTextBox_Click(object sender, EventArgs e)
        {
        }

        #endregion

        #region     I_RefreshTimer_Controllo

        public SynchronizationContext SyncContext
        {
            get
            {
                return m_sync;
            }
        }

        public void RefreshData(object state)
        {
            try
            {
                this.Refresh();

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        #endregion I_RefreshTimer_Controllo

    }
}
