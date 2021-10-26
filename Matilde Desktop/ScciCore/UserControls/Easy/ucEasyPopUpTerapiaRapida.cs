using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using Infragistics.Win.UltraWinListView;

namespace UnicodeSrl.ScciCore
{
    public partial class ucEasyPopUpTerapiaRapida : UserControl
    {
        public ucEasyPopUpTerapiaRapida()
        {
            InitializeComponent();
            loadCombo();
        }

        #region Declare

        public event EventHandler ConfermaClick;
        public event EventHandler AnnullaClick;

        private ucEasyListBox _ucEasyListBox = null;

        #endregion

        #region Properties

        public object DataOra
        {
            get
            { return this.ucEasyDateTimeEditorDataOra.Value; }
            set
            { this.ucEasyDateTimeEditorDataOra.Value = value; }
        }

        public string ViaSomministrazione
        {
            get
            {
                return (this.uceViaSomministrazione.Value != null ? this.uceViaSomministrazione.Value.ToString() : string.Empty);
            }
            set
            { this.uceViaSomministrazione.Value = value; }
        }

        public string ViaSomministrazioneDes
        {
            get
            { return this.uceViaSomministrazione.Text; }
        }

        public string Prescrizione
        {
            get
            { return this.ucEasyTextBoxPrescrizione.Text; }
            set
            { this.ucEasyTextBoxPrescrizione.Text = value; }
        }

        public string Posologia
        {
            get
            { return this.ucEasyTextBoxPosologia.Text; }
            set
            { this.ucEasyTextBoxPosologia.Text = value; }
        }

        #endregion

        #region Sub

        private void loadCombo()
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("DatiEstesi", "0");

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataTable dt = Database.GetDataTableStoredProc("MSP_SelViaSomministrazione", spcoll);

                if (dt != null)
                {

                    this.uceViaSomministrazione.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending;
                    this.uceViaSomministrazione.ValueMember = "Codice";
                    this.uceViaSomministrazione.DisplayMember = "Descrizione";
                    this.uceViaSomministrazione.DataSource = dt;
                    this.uceViaSomministrazione.Refresh();

                }

            }
            catch (Exception)
            {

            }

        }

        #endregion

        #region Events

        private void ucEasyDateTimeEditorDataOra_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

            if (e.Button.Key == "Orari" && ((ucEasyDateTimeEditor)sender).Value != null)
            {
                CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainer);
                _ucEasyListBox = GetEasyListBoxForPopupControlContainer(sender);
                this.UltraPopupControlContainer.Show((ucEasyDateTimeEditor)sender);
            }

        }

        private void ucEasyButtonConferma_Click(object sender, EventArgs e)
        {
            if (ConfermaClick != null) { ConfermaClick(sender, e); }

        }

        private void ucEasyButtonCancel_Click(object sender, EventArgs e)
        {
            if (AnnullaClick != null) { AnnullaClick(sender, e); }
        }

        #endregion

        #region UltraPopupControlContainer ucEasyListBox

        private void UltraPopupControlContainer_Closed(object sender, EventArgs e)
        {
            _ucEasyListBox.ItemSelectionChanged -= ucEasyListBox_ItemSelectionChanged;
        }

        private void UltraPopupControlContainer_Opened(object sender, EventArgs e)
        {
            _ucEasyListBox.ItemSelectionChanged += ucEasyListBox_ItemSelectionChanged;
        }

        private void UltraPopupControlContainer_Opening(object sender, CancelEventArgs e)
        {
            Infragistics.Win.Misc.UltraPopupControlContainer popup = sender as Infragistics.Win.Misc.UltraPopupControlContainer;
            popup.PopupControl = _ucEasyListBox;
        }

        private void ucEasyListBox_ItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {

            ucEasyDateTimeEditor source = this.UltraPopupControlContainer.SourceControl as ucEasyDateTimeEditor;
            ucEasyListBox popup = this.UltraPopupControlContainer.PopupControl as ucEasyListBox;

            DateTime dt = (DateTime)source.Value;
            if (popup.ActiveItem != null)
            {
                string[] orari = popup.ActiveItem.Text.Split(':');
                DateTime newdt = new DateTime(dt.Year, dt.Month, dt.Day, int.Parse(orari[0]), int.Parse(orari[1]), 0);
                source.Value = newdt;
            }


            this.UltraPopupControlContainer.Close();
        }

        private ucEasyListBox GetEasyListBoxForPopupControlContainer(object sender)
        {

            DateTime dt = (DateTime)((ucEasyDateTimeEditor)sender).Value;

            ucEasyListBox _ucEasyListBox = new ucEasyListBox();

            try
            {

                _ucEasyListBox.Size = new Size(150, 300);
                _ucEasyListBox.ItemSettings.SelectionType = Infragistics.Win.UltraWinListView.SelectionType.Single;
                _ucEasyListBox.TextFontRelativeDimension = ((ucEasyDateTimeEditor)sender).TextFontRelativeDimension;
                _ucEasyListBox.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
                _ucEasyListBox.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.Details;
                _ucEasyListBox.ViewSettingsDetails.AllowColumnMoving = false;
                _ucEasyListBox.ViewSettingsDetails.AllowColumnSizing = false;
                _ucEasyListBox.ViewSettingsDetails.AllowColumnSorting = false;
                _ucEasyListBox.ViewSettingsDetails.AutoFitColumns = Infragistics.Win.UltraWinListView.AutoFitColumns.ResizeAllColumns;
                _ucEasyListBox.ViewSettingsDetails.FullRowSelect = true;
                _ucEasyListBox.ViewSettingsDetails.ImageSize = new System.Drawing.Size(0, 0);
                _ucEasyListBox.ViewSettingsList.ImageSize = new System.Drawing.Size(0, 0);

                UltraListViewItem oVal = null;
                DateTime valoreitem = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);

                _ucEasyListBox.Items.Clear();

                for (int i = 0; i < 24; i++)
                {
                    oVal = new Infragistics.Win.UltraWinListView.UltraListViewItem(i.ToString());
                    oVal.Value = valoreitem.ToString("HH:mm");
                    _ucEasyListBox.Items.Add(oVal);
                    valoreitem = valoreitem.AddHours(1);
                }

            }
            catch (Exception)
            {

            }

            return _ucEasyListBox;

        }

        #endregion

    }
}
