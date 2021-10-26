using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win.UltraWinListView;

namespace UnicodeSrl.ScciCore
{
    public partial class ucEasyPopUpOrario : UserControl
    {

        #region DECLARE

        public event EventHandler ConfermaClick;
        public event EventHandler AnnullaClick;

        private ucEasyListBox _ucEasyListBox = null;

        #endregion

        public ucEasyPopUpOrario()
        {
            InitializeComponent();
        }

        #region PROPERTIES

        public string TestoEtichetta
        {
            get
            { return this.ucEasyLabel.Text; }
            set
            { this.ucEasyLabel.Text = value; }
        }

        public object DataOra
        {
            get
            { return this.ucEasyDateTimeEditor.Value; }
            set
            { this.ucEasyDateTimeEditor.Value = value; }
        }

        public string TestoTextBox
        {
            get
            { return this.ucEasyLabelTextBox.Text; }
            set
            { this.ucEasyLabelTextBox.Text = value; }
        }

        public string Note
        {
            get
            { return this.ucEasyTextBox.Text; }
            set
            { this.ucEasyTextBox.Text = value; }
        }

        public easyStatics.easyRelativeDimensions TestoEtichettaDimensioneFont
        {
            get
            {
                return this.ucEasyLabel.TextFontRelativeDimension;
            }
            set
            {
                this.ucEasyLabel.TextFontRelativeDimension = value;
            }
        }

        public easyStatics.easyRelativeDimensions DataOraDimensioneFont
        {
            get
            {
                return this.ucEasyDateTimeEditor.TextFontRelativeDimension;
            }
            set
            {
                this.ucEasyDateTimeEditor.TextFontRelativeDimension = value;
            }
        }

        #endregion

        #region EVENTI

        private void ucEasyDateTimeEditor_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
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
