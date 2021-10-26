using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.Scci.Enums;
using Infragistics.Win.UltraWinListView;
using Infragistics.Win.UltraWinToolTip;

namespace UnicodeSrl.ScciCore
{

    public partial class ucDatiAggiuntiviPopUp : UserControl
    {
        public ucDatiAggiuntiviPopUp()
        {
            InitializeComponent();
        }

        #region Declare

        EnumTipoDatoAggiuntivo _tipodato = EnumTipoDatoAggiuntivo.Undefined;

        string _listavalori = string.Empty;

        public event EventHandler Annulla_Click;
        public event EventHandler Conferma_Click;

        string[] separatorevalori = { "§;" };
        string[] separatorevaloresingolo = { "#;" };

        #endregion

        #region Properties

        internal string Descrizione
        {
            get
            {
                return this.lblDescrizione.Text;
            }
            set
            {
                this.lblDescrizione.Text = value;
            }
        }

        internal EnumTipoDatoAggiuntivo TipoDato
        {
            get { return _tipodato; }
            set
            {
                _tipodato = value;
                if (value == EnumTipoDatoAggiuntivo.Undefined) this.Descrizione = string.Empty;
                this.SetUltraTabPage();
            }
        }

        internal string Valore
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }

        internal string DescrizioneValore
        {
            get { return CoreStatics.CoreApplication.MovOrdineSelezionato.CaricaDescrizioneDatoAggiuntivo(this.TipoDato, this.Valore, this.ListaValori); }
        }

        internal string ListaValori
        {
            get { return _listavalori; }
            set
            {
                _listavalori = value;
                this.SetValueList();
            }
        }

        #endregion

        #region Private Methods

        private void SetUltraTabPage()
        {
            try
            {
                this.ultraTabControl.Tabs[this.TipoDato.ToString()].Selected = true;

                switch (this.TipoDato)
                {
                    case EnumTipoDatoAggiuntivo.ComboBox:
                        this.ubZoomComboBox.Appearance.Image = Properties.Resources.arrow_down;
                        this.ubZoomComboBox.PercImageFill = 0.75F;
                        this.ubZoomComboBox.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                        this.ListBoxCombo.Visible = false;
                        this.ubZoomComboBox.Focus();
                        break;
                    case EnumTipoDatoAggiuntivo.DateBox:
                        this.DateBox.Focus();
                        break;
                    case EnumTipoDatoAggiuntivo.DateTimeBox:
                        this.DateTimeBox.Focus();
                        break;
                    case EnumTipoDatoAggiuntivo.FloatBox:
                        this.FloatBox.Focus();
                        break;
                    case EnumTipoDatoAggiuntivo.ListBox:
                        this.ListBox.Focus();
                        break;
                    case EnumTipoDatoAggiuntivo.ListMultiBox:
                        this.ListMultiBox.Focus();
                        break;
                    case EnumTipoDatoAggiuntivo.NumberBox:
                        this.NumberBox.Focus();
                        break;
                    case EnumTipoDatoAggiuntivo.TextBox:
                        this.TextBox.Focus();
                        break;
                    case EnumTipoDatoAggiuntivo.TimeBox:
                        this.TimeBox.Focus();
                        break;
                    case EnumTipoDatoAggiuntivo.Tempi:
                        this.DateTimeBoxInizio.Focus();
                        break;
                    case EnumTipoDatoAggiuntivo.Undefined:
                        this.ultraTabControl.Tabs[this.TipoDato.ToString()].Selected = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }
        }

        private void SetValue(string value)
        {
            try
            {

                switch (this.TipoDato)
                {
                    case EnumTipoDatoAggiuntivo.ComboBox:
                        if (value != string.Empty)
                        {
                            string[] arvalori = this.ListaValori.Split(separatorevalori, StringSplitOptions.RemoveEmptyEntries);
                            string valore = arvalori.ToList<string>().Find(s => s.Contains(value));
                            string[] valorearray = null;

                            this.txtComboBox.Tag = value;

                            if (valore != null && valore != string.Empty)
                            {
                                valorearray = valore.Split(separatorevaloresingolo, StringSplitOptions.RemoveEmptyEntries);
                                if (valorearray.Count() > 1)
                                    this.txtComboBox.Text = valorearray[1];
                                else
                                    this.txtComboBox.Text = valorearray[0];
                            }
                            else
                                this.txtComboBox.Text = value;
                        }
                        else
                        {
                            this.txtComboBox.Text = string.Empty;
                            this.txtComboBox.Text = string.Empty;
                        }
                        break;

                    case EnumTipoDatoAggiuntivo.DateBox:
                        if (value != null && value != string.Empty)
                            DateBox.Value = DateTime.Parse(value).ToString("dd/MM/yyyy");
                        else
                            DateBox.Value = null;
                        break;

                    case EnumTipoDatoAggiuntivo.DateTimeBox:
                        if (value != null && value != string.Empty)
                            DateTimeBox.Value = DateTime.Parse(value).ToString("dd/MM/yyyy HH:mm");
                        else
                            DateTimeBox.Value = null;
                        break;

                    case EnumTipoDatoAggiuntivo.FloatBox:
                        this.FloatBox.Value = decimal.Parse(value);
                        break;

                    case EnumTipoDatoAggiuntivo.ListBox:
                        this.ListBox.Value = value;
                        break;

                    case EnumTipoDatoAggiuntivo.ListMultiBox:
                        this.ListMultiBox.Value = value;
                        break;

                    case EnumTipoDatoAggiuntivo.NumberBox:
                        this.NumberBox.Value = value;
                        break;

                    case EnumTipoDatoAggiuntivo.TextBox:
                        this.TextBox.Text = value;
                        break;

                    case EnumTipoDatoAggiuntivo.TimeBox:
                        if (value != null && value != string.Empty)
                            TimeBox.Value = DateTime.Parse(value).ToString("hh:mm");
                        else
                            TimeBox.Value = null;
                        break;

                    case EnumTipoDatoAggiuntivo.Tempi:
                        string[] valori = value.Split('|');
                        DateTime dt = DateTime.MinValue;
                        try
                        {
                            dt = DateTime.Parse(valori[0]);
                        }
                        catch (Exception)
                        {
                            dt = DateTime.MinValue;
                        }
                        this.DateTimeBoxInizio.Value = dt;

                        try
                        {
                            dt = DateTime.Parse(valori[1]);
                            this.DateTimeBoxFine.Visible = true;
                            this.DateTimeBoxFine.Value = dt;
                        }
                        catch (Exception)
                        {
                            this.DateTimeBoxFine.Visible = false;
                        }

                        this.txtNote.Text = valori[2];
                        break;

                    case EnumTipoDatoAggiuntivo.Undefined:
                        break;
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                this.ListBoxCombo.ClearSelection();
                this.FloatBox.Value = 0;
                this.DateBox.Value = null;
                this.DateBox.Value = null;
                this.ListBox.ClearSelection();
                this.ListMultiBox.ClearSelection();
                this.NumberBox.Value = 0;
                this.TextBox.Text = string.Empty;
                TimeBox.Value = null;
            }
        }

        private string GetValue()
        {
            string sret = string.Empty;

            try
            {
                switch (this.TipoDato)
                {
                    case EnumTipoDatoAggiuntivo.ComboBox:
                        sret = (string)this.txtComboBox.Tag;
                        break;

                    case EnumTipoDatoAggiuntivo.DateBox:
                        if (DateBox.Value != null)
                            sret = ((DateTime)DateBox.Value).ToString("dd/MM/yyyy");
                        else
                            sret = string.Empty;
                        break;

                    case EnumTipoDatoAggiuntivo.DateTimeBox:
                        if (DateTimeBox.Value != null)
                            sret = ((DateTime)DateTimeBox.Value).ToString("dd/MM/yyyy HH:mm");
                        else
                            sret = string.Empty;
                        break;

                    case EnumTipoDatoAggiuntivo.FloatBox:
                        sret = this.FloatBox.Value.ToString();
                        break;

                    case EnumTipoDatoAggiuntivo.ListBox:
                        sret = this.ListBox.Value;
                        break;

                    case EnumTipoDatoAggiuntivo.ListMultiBox:
                        sret = this.ListMultiBox.Value;
                        break;

                    case EnumTipoDatoAggiuntivo.NumberBox:
                        sret = this.NumberBox.Value.ToString();
                        break;

                    case EnumTipoDatoAggiuntivo.TextBox:
                        sret = this.TextBox.Text;
                        break;

                    case EnumTipoDatoAggiuntivo.TimeBox:
                        if (TimeBox.Value != null)
                            sret = ((DateTime)TimeBox.Value).ToString("HH:mm");
                        else
                            sret = string.Empty;
                        break;

                    case EnumTipoDatoAggiuntivo.Tempi:
                        sret = string.Format("{0}|{1}|{2}",
                                            ((DateTime)DateTimeBoxInizio.Value).ToString("dd/MM/yyyy HH:mm"),
                                            (DateTimeBoxFine.Value == null ? "" : ((DateTime)DateTimeBoxFine.Value).ToString("dd/MM/yyyy HH:mm")),
                                            txtNote.Text);
                        break;

                    case EnumTipoDatoAggiuntivo.Undefined:
                        sret = string.Empty;
                        break;
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                sret = string.Empty;
            }

            return sret;

        }

        private void SetValueList()
        {
            string sret = string.Empty;
            string[] arvalori = null;

            try
            {

                arvalori = this.ListaValori.Split(separatorevalori, StringSplitOptions.RemoveEmptyEntries);
                this.ListBoxCombo.HideSelection = false;

                switch (this.TipoDato)
                {
                    case EnumTipoDatoAggiuntivo.ComboBox:
                        this.ListBoxCombo.SearchBoxVisible = (arvalori.Count() > 7);
                        this.ListBoxCombo.ValueList = this.ListaValori;
                        break;

                    case EnumTipoDatoAggiuntivo.ListBox:
                        this.ListBox.SearchBoxVisible = (arvalori.Count() > 7);
                        this.ListBox.ValueList = this.ListaValori;
                        break;

                    case EnumTipoDatoAggiuntivo.ListMultiBox:
                        this.ListMultiBox.SearchBoxVisible = (arvalori.Count() > 7);
                        this.ListMultiBox.ValueList = this.ListaValori;

                        break;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                this.ListBoxCombo.ClearList();
                this.ListBox.ClearList();
                this.ListMultiBox.ClearList();
            }
        }

        private void DropDownListBox()
        {
            try
            {
                Console.WriteLine("DropDownListBox");
                if (this.ListBoxCombo.Visible)
                {
                    this.ListBoxCombo.Visible = false;
                    this.ubZoomComboBox.Focus();
                }
                else
                {
                    this.ListBoxCombo.Value = (string)this.txtComboBox.Tag;
                    this.ListBoxCombo.Visible = true;
                    this.ListBoxCombo.Focus();
                }
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "DropDownListBox", this.Text);
            }
        }

        #endregion

        #region Events

        private void ultraTabControl_Enter(object sender, EventArgs e)
        {
            this.SetUltraTabPage();
        }

        private void ubZoomComboBox_Click(object sender, EventArgs e)
        {
            DropDownListBox();
        }

        private void txtComboBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left) this.DropDownListBox();
        }

        private void ListBoxCombo_ListBoxItemClick(object sender, ItemActivatedEventArgs e)
        {
            this.SetValue(this.ListBoxCombo.Value);
            this.ListBoxCombo.Visible = false;
            this.ubZoomComboBox.Focus();
        }

        private void ubAnnulla_Click(object sender, EventArgs e)
        {
            if (Annulla_Click != null) { Annulla_Click(sender, e); }
        }

        private void ubConferma_Click(object sender, EventArgs e)
        {
            if (Conferma_Click != null) { Conferma_Click(sender, e); }
        }

        #endregion

    }
}
