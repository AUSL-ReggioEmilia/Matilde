using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.Scci.Enums;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinListView;
using UnicodeSrl.Scci;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.ScciCore
{
    public partial class frmStoricoReport : frmBaseModale, Interfacce.IViewFormlModal
    {
        public frmStoricoReport()
        {
            InitializeComponent();
        }

        #region Declare

        private ucEasyListBox _ucEasyListBox = null;

        #endregion

        #region Interface

        public void Carica()
        {

            try
            {

                this.ImpostaCursore(enum_app_cursors.WaitCursor);

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_REPORTHISTORY_128);

                this.InitializeUltraGrid();

                this.udteDataOraInizio.Value = DateTime.Now.AddYears(-1);

                this.ShowDialog();

            }
            catch (Exception ex)
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }


        }

        #endregion

        #region UltraGrid

        private void InitializeUltraGrid()
        {
            this.UltraGridSX.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
            this.UltraGridDX.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
        }

        private void LoadUltraGrid()
        {

            try
            {

                                                                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("DataInizio", Database.dataOra105PerParametri((DateTime)this.udteDataOraInizio.Value));
                op.Parametro.Add("DataFine", Database.dataOra105PerParametri(DateTime.Now));
                if (CoreStatics.CoreApplication.Paziente != null)
                {
                    op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Paziente.ID);
                }

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable oDt1 = Database.GetDataTableStoredProc("MSP_SelMovReport", spcoll);

                this.UltraGridSX.DataSource = oDt1;
                this.UltraGridSX.Refresh();

                this.UltraGridSX.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);
                this.UltraGridSX.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;

                                                                op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("DataInizio", Database.dataOra105PerParametri((DateTime)this.udteDataOraInizio.Value));
                op.Parametro.Add("DataFine", Database.dataOra105PerParametri(DateTime.Now));
                op.Parametro.Add("CodUtente", CoreStatics.CoreApplication.Sessione.Utente.Codice);

                spcoll = new SqlParameterExt[1];

                xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable oDt2 = Database.GetDataTableStoredProc("MSP_SelMovReport", spcoll);

                this.UltraGridDX.DataSource = oDt2;
                this.UltraGridDX.Refresh();

                this.UltraGridDX.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);
                this.UltraGridDX.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        #endregion

        #region Functions

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

        #region Events Form

        private void frmStoricoReport_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void frmStoricoReport_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void frmStoricoReport_Shown(object sender, EventArgs e)
        {
            try
            {
                this.udteDataOraInizio.Focus();
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }
            catch
            {
            }
        }

        #endregion

        #region Events

        private void UltraGrid_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {

                
                foreach (UltraGridColumn ocol in e.Layout.Bands[0].Columns)
                {

                    switch (ocol.Key)
                    {

                        case "Data":
                            ocol.Hidden = false;
                            ocol.Format = "dd/MM/yyyy HH:mm";
                            break;

                        case "Paziente":
                            ocol.Hidden = false;
                            break;

                        case "Stampa":
                            ocol.Hidden = false;
                            break;

                        case "Utente":
                            ocol.Hidden = false;
                            break;

                        default:
                            ocol.Hidden = true;
                            break;

                    }

                }

            }
            catch (Exception)
            {

            }

        }

        private void udteDataOraInizio_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

            if (e.Button.Key == "Orari" && ((ucEasyDateTimeEditor)sender).Value != null)
            {
                CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainer);
                _ucEasyListBox = GetEasyListBoxForPopupControlContainer(sender);
                this.UltraPopupControlContainer.Show((ucEasyDateTimeEditor)sender);
            }

        }

        private void udteDataOraInizio_ValueChanged(object sender, EventArgs e)
        {
            this.LoadUltraGrid();
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

        #endregion

    }
}
