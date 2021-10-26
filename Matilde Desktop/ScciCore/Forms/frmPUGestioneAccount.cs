using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.ScciResource;

namespace UnicodeSrl.ScciCore
{
    public partial class frmPUGestioneAccount : frmBaseModale, Interfacce.IViewFormlModal
    {
        public frmPUGestioneAccount()
        {
            InitializeComponent();
        }

        #region Declare

        private ucEasyGrid _ucEasyGridStatoLogin = null;
        private ucEasyGrid _ucEasyGridLoginUA = null;
        private ucEasyPopUpMHContatti _ucEasyPopUpMHContatti = null;

        #endregion

        #region Interface

        public new void Carica()
        {

            try
            {

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_MATHOME_16);

                this.InizializzaControlli();
                this.CaricaControlli();

                this.ShowDialog();

            }
            catch (Exception)
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }

        }

        #endregion

        #region Subroutine

        private void InizializzaControlli()
        {

            try
            {

                this.pbAccount.Image = Risorse.GetImageFromResource(Risorse.GC_KEY_256);

                this.ubStatoMHLogin.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_SOSPENDI_256);
                this.ubStatoMHLogin.PercImageFill = 0.75F;
                this.ubStatoMHLogin.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.ubPassword.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_WIZARD_256);
                this.ubPassword.PercImageFill = 0.75F;
                this.ubPassword.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.ubStampa.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_STAMPA_256);
                this.ubStampa.PercImageFill = 0.75F;
                this.ubStampa.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.pbContatti.Image = Risorse.GetImageFromResource(Risorse.GC_CONTATTI_256);

                this.ubContattiAdd.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_NUOVO_256);
                this.ubContattiAdd.PercImageFill = 0.75F;
                this.ubContattiAdd.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.ubUAAdd.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_NUOVO_256);
                this.ubUAAdd.PercImageFill = 0.75F;
                this.ubUAAdd.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

            }
            catch (Exception)
            {

            }

        }

        private void CaricaControlli()
        {

            try
            {

                var oPaziente = CoreStatics.CoreApplication.Paziente;
                if (oPaziente != null)
                {
                    this.lblPaziente.Text = string.Format("{0} {1} ({2}) ({3})", oPaziente.Cognome, oPaziente.Nome, oPaziente.Sesso, oPaziente.Eta);
                    this.lblIndirizzo.Text = string.Format("Nato a {0}, il {1}", oPaziente.ComuneNascita, oPaziente.DataNascita.ToShortDateString());
                    this.lblCodiceFiscale.Text = string.Format("C.F. {0}", oPaziente.CodiceFiscale);
                    this.pbPaziente.Image = oPaziente.Foto;
                }

                this.lblStatoMHLogin.Text = CoreStatics.CoreApplication.MH_LoginSelezionato.StatoMHLogin;
                this.lblStatoMHLogin.Appearance.BackColor = CoreStatics.GetColorFromString(CoreStatics.CoreApplication.MH_LoginSelezionato.ColoreMHLogin);

                this.uteUtente.Text = CoreStatics.CoreApplication.MH_LoginSelezionato.Codice;
                this.utePassword.Text = CoreStatics.CoreApplication.MH_LoginSelezionato.PasswordAccesso;
                this.udteDataScadenza.Value = CoreStatics.CoreApplication.MH_LoginSelezionato.DataScadenza;

                this.CaricaContatti();
                this.CaricaUA();
                this.CaricaMovAccessi();

            }
            catch (Exception)
            {

            }

        }

        private void CaricaContatti()
        {

            try
            {

                                                                SqlParameterExt[] spcoll = new SqlParameterExt[2];
                                spcoll[0] = new SqlParameterExt("sCodice", CoreStatics.CoreApplication.MH_LoginSelezionato.Codice, ParameterDirection.Input, SqlDbType.VarChar);
                                TimeStamp ts = new TimeStamp(CoreStatics.CoreApplication.Ambiente);
                ts.CodAzione = EnumAzioni.VIS.ToString();
                ts.CodEntita = EnumEntita.XXX.ToString();
                string xmlParam = XmlProcs.XmlSerializeToString(ts);
                spcoll[1] = new SqlParameterExt("xTimeStamp", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                                DataTable dt = Database.GetDataTableStoredProc("MSP_MH_ContattiAttivi", spcoll);

                this.UltraGridContatti.DisplayLayout.Bands[0].Columns.ClearUnbound();

                                this.UltraGridContatti.DataSource = dt;
                this.UltraGridContatti.Refresh();

                this.UltraGridContatti.PerformAction(UltraGridAction.FirstRowInBand, false, false);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaContatti", this.Name);
            }

        }

        private void SalvaContatti(string numerotelefono, string descrizione, string tipo, string id)
        {

            try
            {

                                                                SqlParameterExt[] spcoll = new SqlParameterExt[5];
                TimeStamp ts = new TimeStamp(CoreStatics.CoreApplication.Ambiente);
                string xmlParam = string.Empty;

                switch (tipo)
                {

                    case "INS":                       
                                                spcoll[0] = new SqlParameterExt("sCodMHLogin", CoreStatics.CoreApplication.MH_LoginSelezionato.Codice, ParameterDirection.Input, SqlDbType.VarChar);
                        spcoll[1] = new SqlParameterExt("sNumeroTelefono", numerotelefono, ParameterDirection.Input, SqlDbType.VarChar);
                        spcoll[2] = new SqlParameterExt("sDescrizione", descrizione, ParameterDirection.Input, SqlDbType.VarChar);
                        spcoll[3] = new SqlParameterExt("sCodStatoMHContatto", "AT", ParameterDirection.Input, SqlDbType.VarChar);
                                                ts.CodAzione = EnumAzioni.INS.ToString();
                        ts.CodEntita = EnumEntita.XXX.ToString();
                        xmlParam = XmlProcs.XmlSerializeToString(ts);
                        spcoll[4] = new SqlParameterExt("xTimeStamp", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                                                Database.ExecStoredProc("MSP_MH_InsLoginContatti", spcoll);
                        break;

                    case "EDIT":
                                                spcoll[0] = new SqlParameterExt("uID", new Guid(id), ParameterDirection.Input, SqlDbType.UniqueIdentifier);
                        spcoll[1] = new SqlParameterExt("sNumeroTelefono", numerotelefono, ParameterDirection.Input, SqlDbType.VarChar);
                        spcoll[2] = new SqlParameterExt("sDescrizione", descrizione, ParameterDirection.Input, SqlDbType.VarChar);
                        spcoll[3] = new SqlParameterExt("sCodStatoMHContatto", DBNull.Value, ParameterDirection.Input, SqlDbType.VarChar);
                                                ts = new TimeStamp(CoreStatics.CoreApplication.Ambiente);
                        ts.CodAzione = EnumAzioni.MOD.ToString();
                        ts.CodEntita = EnumEntita.XXX.ToString();
                        xmlParam = XmlProcs.XmlSerializeToString(ts);
                        spcoll[4] = new SqlParameterExt("xTimeStamp", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                                                Database.ExecStoredProc("MSP_MH_AggLoginContatti", spcoll);
                        break;

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "SalvaContatti", this.Name);
            }

        }

        private void CancellaContatto(string sid)
        {

            try
            {

                                                                SqlParameterExt[] spcoll = new SqlParameterExt[5];
                                spcoll[0] = new SqlParameterExt("uID", new Guid(sid), ParameterDirection.Input, SqlDbType.UniqueIdentifier);
                spcoll[1] = new SqlParameterExt("sNumeroTelefono", DBNull.Value, ParameterDirection.Input, SqlDbType.VarChar);
                spcoll[2] = new SqlParameterExt("sDescrizione", DBNull.Value, ParameterDirection.Input, SqlDbType.VarChar);
                spcoll[3] = new SqlParameterExt("sCodStatoMHContatto", "CA", ParameterDirection.Input, SqlDbType.VarChar);
                                TimeStamp ts = new TimeStamp(CoreStatics.CoreApplication.Ambiente);
                ts.CodAzione = EnumAzioni.CAN.ToString();
                ts.CodEntita = EnumEntita.XXX.ToString();
                string xmlParam = XmlProcs.XmlSerializeToString(ts);
                spcoll[4] = new SqlParameterExt("xTimeStamp", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                                Database.ExecStoredProc("MSP_MH_AggLoginContatti", spcoll);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CancellaContatto", this.Name);
            }

        }

        private void CaricaUA()
        {

            try
            {

                                                                SqlParameterExt[] spcoll = new SqlParameterExt[3];
                                spcoll[0] = new SqlParameterExt("sCodMHLogin", CoreStatics.CoreApplication.MH_LoginSelezionato.Codice, ParameterDirection.Input, SqlDbType.VarChar);
                spcoll[1] = new SqlParameterExt("sCodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice, ParameterDirection.Input, SqlDbType.VarChar);
                                TimeStamp ts = new TimeStamp(CoreStatics.CoreApplication.Ambiente);
                ts.CodAzione = EnumAzioni.VIS.ToString();
                ts.CodEntita = EnumEntita.XXX.ToString();
                string xmlParam = XmlProcs.XmlSerializeToString(ts);
                spcoll[2] = new SqlParameterExt("xTimeStamp", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                                DataTable dt = Database.GetDataTableStoredProc("MSP_MH_SelLoginUA", spcoll);

                this.UltraGridUA.DisplayLayout.Bands[0].Columns.ClearUnbound();

                                this.UltraGridUA.DataSource = dt;
                this.UltraGridUA.Refresh();

                this.UltraGridUA.PerformAction(UltraGridAction.FirstRowInBand, false, false);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaContatti", this.Name);
            }

        }

        private void SalvaUA(string codice)
        {

            try
            {

                                                                SqlParameterExt[] spcoll = new SqlParameterExt[4];
                                spcoll[0] = new SqlParameterExt("sCodMHLogin", CoreStatics.CoreApplication.MH_LoginSelezionato.Codice, ParameterDirection.Input, SqlDbType.VarChar);
                spcoll[1] = new SqlParameterExt("sCodUA", codice, ParameterDirection.Input, SqlDbType.VarChar);
                spcoll[2] = new SqlParameterExt("sCodStatoMHLoginUA", "AT", ParameterDirection.Input, SqlDbType.VarChar);
                                TimeStamp ts = new TimeStamp(CoreStatics.CoreApplication.Ambiente);
                ts.CodAzione = EnumAzioni.INS.ToString();
                ts.CodEntita = EnumEntita.XXX.ToString();
                string xmlParam = XmlProcs.XmlSerializeToString(ts);
                spcoll[3] = new SqlParameterExt("xTimeStamp", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                                Database.ExecStoredProc("MSP_MH_InsLoginUA", spcoll);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "SalvaUA", this.Name);
            }

        }

        private void CancellaUA(string sid)
        {

            try
            {

                                                                SqlParameterExt[] spcoll = new SqlParameterExt[4];
                                spcoll[0] = new SqlParameterExt("uID", new Guid(sid), ParameterDirection.Input, SqlDbType.UniqueIdentifier);
                spcoll[1] = new SqlParameterExt("sCodUA", DBNull.Value, ParameterDirection.Input, SqlDbType.VarChar);
                spcoll[2] = new SqlParameterExt("sCodStatoMHLoginUA", "CA", ParameterDirection.Input, SqlDbType.VarChar);
                                TimeStamp ts = new TimeStamp(CoreStatics.CoreApplication.Ambiente);
                ts.CodAzione = EnumAzioni.CAN.ToString();
                ts.CodEntita = EnumEntita.XXX.ToString();
                string xmlParam = XmlProcs.XmlSerializeToString(ts);
                spcoll[3] = new SqlParameterExt("xTimeStamp", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                                Database.ExecStoredProc("MSP_MH_AggLoginUA", spcoll);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CancellaUA", this.Name);
            }

        }

        private void CaricaMovAccessi()
        {

            try
            {

                                                                SqlParameterExt[] spcoll = new SqlParameterExt[2];
                                spcoll[0] = new SqlParameterExt("sCodMHLogin", CoreStatics.CoreApplication.MH_LoginSelezionato.Codice, ParameterDirection.Input, SqlDbType.VarChar);
                                TimeStamp ts = new TimeStamp(CoreStatics.CoreApplication.Ambiente);
                ts.CodAzione = EnumAzioni.VIS.ToString();
                ts.CodEntita = EnumEntita.XXX.ToString();
                string xmlParam = XmlProcs.XmlSerializeToString(ts);
                spcoll[1] = new SqlParameterExt("xTimeStamp", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                                DataTable dt = Database.GetDataTableStoredProc("MSP_MH_SelMovAccessi", spcoll);

                this.UltraGridMovAccessi.DisplayLayout.Bands[0].Columns.ClearUnbound();

                                this.UltraGridMovAccessi.DataSource = dt;
                this.UltraGridMovAccessi.Refresh();

                this.UltraGridMovAccessi.PerformAction(UltraGridAction.FirstRowInBand, false, false);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaMovAccessi", this.Name);
            }

        }

        private bool Salva()
        {

            bool bReturn = false;

            try
            {

                bool bOK = true;
                if (CoreStatics.CoreApplication.MH_LoginSelezionato.CodStatoMHLogin == "AT")
                {
                    if (bOK && this.UltraGridContatti.Rows.Count < 1)
                    {
                        easyStatics.EasyMessageBox("Inserire almeno un contatto valido!", "Contatto", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        bOK = false;
                    }
                    if (bOK && this.UltraGridUA.Rows.Count < 1)
                    {
                        easyStatics.EasyMessageBox("Inserire almeno una struttura!", "Struttura", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        bOK = false;
                    }
                }

                if (bOK)
                {

                    CoreStatics.CoreApplication.MH_LoginSelezionato.PasswordAccesso = this.utePassword.Text;
                    CoreStatics.CoreApplication.MH_LoginSelezionato.DataScadenza = (DateTime)this.udteDataScadenza.Value;
                    bReturn = CoreStatics.CoreApplication.MH_LoginSelezionato.Salva();

                }

            }
            catch (Exception)
            {

            }

            return bReturn;

        }

        private ucEasyGrid getGridStatoLogin()
        {

            ucEasyGrid gridret = null;

            try
            {

                gridret = new ucEasyGrid();

                                SqlParameterExt[] spcoll = null;

                DataSet ds = Database.GetDatasetStoredProc("MSP_MH_SelStatoLogin", spcoll);

                gridret.DataSource = ds;
                gridret.Refresh();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "getGridStatoLogin", this.Name);
            }

            return gridret;

        }

        private ucEasyGrid getGridLoginUA()
        {

            ucEasyGrid gridret = null;

            try
            {

                gridret = new ucEasyGrid();

                                                                SqlParameterExt[] spcoll = new SqlParameterExt[2];
                                spcoll[0] = new SqlParameterExt("sCodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice, ParameterDirection.Input, SqlDbType.VarChar);
                                TimeStamp ts = new TimeStamp(CoreStatics.CoreApplication.Ambiente);
                ts.CodAzione = EnumAzioni.VIS.ToString();
                ts.CodEntita = EnumEntita.XXX.ToString();
                string xmlParam = XmlProcs.XmlSerializeToString(ts);
                spcoll[1] = new SqlParameterExt("xTimeStamp", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                                DataSet ds = Database.GetDatasetStoredProc("MSP_MH_SelRuoloUA", spcoll);

                gridret.DataSource = ds;
                gridret.Refresh();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "getGridLoginUA", this.Name);
            }

            return gridret;

        }

        #endregion

        #region Events Form

        private void frmPUGestioneAccount_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
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

        private void frmPUGestioneAccount_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        #endregion

        #region Events

        private void ubStatoMHLogin_Click(object sender, EventArgs e)
        {

            try
            {

                _ucEasyGridStatoLogin = null;
                _ucEasyGridStatoLogin = getGridStatoLogin();
                if (_ucEasyGridStatoLogin != null && _ucEasyGridStatoLogin.DataSource != null)
                {

                                        _ucEasyGridStatoLogin.Size = new Size(600, 450);
                    _ucEasyGridStatoLogin.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
                    CoreStatics.SetEasyUltraGridLayout(ref _ucEasyGridStatoLogin);
                    _ucEasyGridStatoLogin.DisplayLayout.CaptionVisible = DefaultableBoolean.True;

                                        CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerStatoLogin);

                    this.UltraPopupControlContainerStatoLogin.Show(sender as ucEasyGrid);

                                        _ucEasyGridStatoLogin.DisplayLayout.Bands[0].ClearGroupByColumns();

                    foreach (UltraGridColumn oCol in _ucEasyGridStatoLogin.DisplayLayout.Bands[0].Columns)
                    {

                        oCol.SortIndicator = SortIndicator.Disabled;
                        switch (oCol.Key)
                        {

                            case "Codice":
                                oCol.Hidden = false;
                                oCol.Header.Caption = "Codice";
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.Header.VisiblePosition = 1;
                                break;

                            case "Descrizione":
                                oCol.Hidden = false;
                                oCol.Header.Caption = "Stato";
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.Header.VisiblePosition = 2;
                                break;

                            default:
                                oCol.Hidden = true;
                                break;

                        }

                    }

                                        _ucEasyGridStatoLogin.DisplayLayout.Bands[0].Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
                    _ucEasyGridStatoLogin.Refresh();
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ubStatoMHLogin_Click", this.Name);
            }

        }

        private void ubPassword_Click(object sender, EventArgs e)
        {
            CoreStatics.CoreApplication.MH_LoginSelezionato.GeneraNuovaPassword();
            CoreStatics.CoreApplication.MH_LoginSelezionato.Salva();
            this.CaricaControlli();
        }

        private void ubStampa_Click(object sender, EventArgs e)
        {

            try
            {

                var item = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Reports.Elementi.Single(Report => Report.Codice == UnicodeSrl.ScciCore.Report.COD_REPORT_MH_ACCOUNT);
                if (item != null)
                {
                    CoreStatics.CoreApplication.ReportSelezionato = item;
                    CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.Report);
                    CoreStatics.CoreApplication.ReportSelezionato = null;
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ubStampa_Click", this.Name);
            }

        }

        private void ubContattiAdd_Click(object sender, EventArgs e)
        {

            try
            {

                _ucEasyPopUpMHContatti = new ucEasyPopUpMHContatti();
                _ucEasyPopUpMHContatti.Size = new Size(600, 450);
                _ucEasyPopUpMHContatti.Tag = "INS";

                CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerLoginContatti);
                this.UltraPopupControlContainerLoginContatti.Show(sender as ucEasyPopUpMHContatti);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ubContattiAdd_Click", this.Name);
            }

        }

        private void ubUAAdd_Click(object sender, EventArgs e)
        {

            try
            {

                _ucEasyGridLoginUA = null;
                _ucEasyGridLoginUA = getGridLoginUA();
                if (_ucEasyGridLoginUA != null && _ucEasyGridLoginUA.DataSource != null)
                {

                                        _ucEasyGridLoginUA.Size = new Size(600, 450);
                    _ucEasyGridLoginUA.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
                    CoreStatics.SetEasyUltraGridLayout(ref _ucEasyGridLoginUA);
                    _ucEasyGridLoginUA.DisplayLayout.CaptionVisible = DefaultableBoolean.True;

                                        CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerLoginUA);

                    this.UltraPopupControlContainerLoginUA.Show(sender as ucEasyGrid);

                                        _ucEasyGridLoginUA.DisplayLayout.Bands[0].ClearGroupByColumns();

                    foreach (UltraGridColumn oCol in _ucEasyGridLoginUA.DisplayLayout.Bands[0].Columns)
                    {

                        oCol.SortIndicator = SortIndicator.Disabled;
                        switch (oCol.Key)
                        {

                            case "Codice":
                                oCol.Hidden = true;
                                oCol.Header.Caption = "Codice";
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.Header.VisiblePosition = 1;
                                break;

                            case "Descrizione":
                                oCol.Hidden = false;
                                oCol.Header.Caption = "UA";
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.Header.VisiblePosition = 2;
                                break;

                            default:
                                oCol.Hidden = true;
                                break;

                        }

                    }

                                        _ucEasyGridLoginUA.DisplayLayout.Bands[0].Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
                    _ucEasyGridLoginUA.Refresh();
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ubUAAdd_Click", this.Name);
            }

        }

        private void UltraGridContatti_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {

                int refWidth = (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large) * 3;

                Graphics g = this.CreateGraphics();
                int refBtnWidth = Convert.ToInt32(CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(this.UltraGridContatti.DataRowFontRelativeDimension), g.DpiY) * 3);
                int iSpz = Convert.ToInt32(refBtnWidth * 0.15);
                g.Dispose();
                g = null;
                                e.Layout.Bands[0].HeaderVisible = false;
                e.Layout.Bands[0].ColHeadersVisible = false;
                e.Layout.Bands[0].RowLayoutStyle = RowLayoutStyle.GroupLayout;
                                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {

                    try
                    {

                        oCol.SortIndicator = SortIndicator.Disabled;
                        switch (oCol.Key)
                        {

                            case "NumeroTelefono":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Medium);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = (int) (refWidth * 2.5);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 0;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "Descrizione":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Medium);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = this.UltraGridContatti.Width - (int) (refWidth * 2.5) - Convert.ToInt32(refBtnWidth * 2.2) - 16;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            default:
                                oCol.Hidden = true;
                                break;

                        }

                    }
                    catch (Exception)
                    {

                    }

                }

                                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_EDIT))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_EDIT);
                    colEdit.Hidden = false;

                                        colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_MODIFICA_32);

                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = refBtnWidth;
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;

                    colEdit.RowLayoutColumnInfo.OriginX = 2;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 1;
                }
                                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_EDIT + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_EDIT + @"_SP");
                    colEdit.Hidden = false;
                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                    colEdit.CellActivation = Activation.Disabled;
                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = iSpz;
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;
                    colEdit.RowLayoutColumnInfo.OriginX = 3;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 1;
                }

                                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_DEL))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_DEL);
                    colEdit.Hidden = false;

                                        colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_ELIMINA_32);


                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = refBtnWidth;
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;


                    colEdit.RowLayoutColumnInfo.OriginX = 4;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 1;
                }
                                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_DEL + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_DEL + @"_SP");
                    colEdit.Hidden = false;
                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                    colEdit.CellActivation = Activation.Disabled;
                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = iSpz;
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;
                    colEdit.RowLayoutColumnInfo.OriginX = 5;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 1;
                }

            }
            catch (Exception)
            {
            }

        }

        private void UltraGridContatti_InitializeRow(object sender, InitializeRowEventArgs e)
        {

            try
            {

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "UltraGridContatti_InitializeRow", this.Name);
            }

        }

        private void UltraGridContatti_ClickCellButton(object sender, CellEventArgs e)
        {

            try
            {

                switch (e.Cell.Column.Key)
                {

                    case CoreStatics.C_COL_BTN_EDIT:
                        _ucEasyPopUpMHContatti = new ucEasyPopUpMHContatti();
                        _ucEasyPopUpMHContatti.Size = new Size(600, 450);
                        _ucEasyPopUpMHContatti.ID = e.Cell.Row.Cells["ID"].Text;
                        _ucEasyPopUpMHContatti.NumeroTelefono = e.Cell.Row.Cells["NumeroTelefono"].Text;
                        _ucEasyPopUpMHContatti.Descrizione = e.Cell.Row.Cells["Descrizione"].Text;
                        _ucEasyPopUpMHContatti.Tag = "EDIT";
                        CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerLoginContatti);
                        this.UltraPopupControlContainerLoginContatti.Show(sender as ucEasyPopUpMHContatti);
                        this.CaricaContatti();
                        break;

                    case CoreStatics.C_COL_BTN_DEL:
                        if (easyStatics.EasyMessageBox("Confermi cancellazione contatto selezionato ?", "Cancellazione Contatto", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            this.CancellaContatto(e.Cell.Row.Cells["ID"].Text);
                            this.CaricaContatti();
                        }
                        break;

                    default:
                        break;

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "UltraGridContatti_ClickCellButton", this.Name);
            }

        }

        private void UltraGridUA_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {

                int refWidth = (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large) * 3;

                Graphics g = this.CreateGraphics();
                int refBtnWidth = Convert.ToInt32(CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(this.UltraGridUA.DataRowFontRelativeDimension), g.DpiY) * 3);
                int iSpz = Convert.ToInt32(refBtnWidth * 0.15);
                g.Dispose();
                g = null;
                                e.Layout.Bands[0].HeaderVisible = false;
                e.Layout.Bands[0].ColHeadersVisible = false;
                e.Layout.Bands[0].RowLayoutStyle = RowLayoutStyle.GroupLayout;
                                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {

                    try
                    {

                        oCol.SortIndicator = SortIndicator.Disabled;
                        switch (oCol.Key)
                        {

                            case "Descrizione":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Medium);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = this.UltraGridUA.Width - (refWidth * 0) - Convert.ToInt32(refBtnWidth * 1.1) - 8;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 0;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            default:
                                oCol.Hidden = true;
                                break;

                        }

                    }
                    catch (Exception)
                    {

                    }

                }


                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_DEL))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_DEL);
                    colEdit.Hidden = false;

                                        colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_ELIMINA_32);


                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = refBtnWidth;
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;


                    colEdit.RowLayoutColumnInfo.OriginX = 1;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 1;
                }
                                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_DEL + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_DEL + @"_SP");
                    colEdit.Hidden = false;
                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                    colEdit.CellActivation = Activation.Disabled;
                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = iSpz;
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;
                    colEdit.RowLayoutColumnInfo.OriginX = 2;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 1;
                }

            }
            catch (Exception)
            {
            }

        }

        private void UltraGridUA_InitializeRow(object sender, InitializeRowEventArgs e)
        {

            try
            {

                foreach (UltraGridCell ocell in e.Row.Cells)
                {

                    if (ocell.Column.Key == CoreStatics.C_COL_BTN_DEL && ocell.Row.Cells["PermessoModifica"].Value.ToString() == "0")
                        ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "UltraGridUA_InitializeRow", this.Name);
            }

        }

        private void UltraGridUA_ClickCellButton(object sender, CellEventArgs e)
        {

            try
            {

                switch (e.Cell.Column.Key)
                {

                    case CoreStatics.C_COL_BTN_DEL:
                        if (e.Cell.Row.Cells["PermessoModifica"].Text == "1")
                        {
                            if (easyStatics.EasyMessageBox("Confermi cancellazione UA selezionata ?", "Cancellazione UA", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                this.CancellaUA(e.Cell.Row.Cells["ID"].Text);
                                this.CaricaUA();
                            }
                        }
                        break;

                    default:
                        break;

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "UltraGridUA_ClickCellButton", this.Name);
            }

        }

        private void UltraGridMovAccessi_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {

                int refWidth = (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large) * 3;

                Graphics g = this.CreateGraphics();
                int refBtnWidth = Convert.ToInt32(CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(this.UltraGridMovAccessi.DataRowFontRelativeDimension), g.DpiY) * 3);
                int iSpz = Convert.ToInt32(refBtnWidth * 0.15);
                g.Dispose();
                g = null;
                                e.Layout.Bands[0].HeaderVisible = false;
                e.Layout.Bands[0].ColHeadersVisible = false;
                e.Layout.Bands[0].RowLayoutStyle = RowLayoutStyle.GroupLayout;
                                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {

                    try
                    {

                        oCol.SortIndicator = SortIndicator.Disabled;
                        switch (oCol.Key)
                        {

                            case "DataEvento":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.Format = "dd/MM/yyyy HH:mm";
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = (int) (refWidth * 2.2);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }
                                oCol.RowLayoutColumnInfo.OriginX = 0;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;
                                break;

                            case "NumeroTelefono":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = (int) (refWidth * 1.8);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;
                                break;

                            case "Note":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = this.UltraGridMovAccessi.Width - (refWidth * 6) - 8;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 2;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;
                                break;

                            default:
                                oCol.Hidden = true;
                                break;

                        }

                    }
                    catch (Exception)
                    {

                    }

                }
               
            }
            catch (Exception)
            {
            }

        }

        #endregion

        #region Events UltraPopupControlContainerStatoLogin

        private void UltraPopupControlContainerStatoLogin_Closed(object sender, EventArgs e)
        {
            _ucEasyGridStatoLogin.ClickCell -= ucEasyGridStatoLogin_ClickCell;
        }

        private void UltraPopupControlContainerStatoLogin_Opened(object sender, EventArgs e)
        {
            _ucEasyGridStatoLogin.ClickCell += ucEasyGridStatoLogin_ClickCell;
            _ucEasyGridStatoLogin.Focus();
        }

        private void UltraPopupControlContainerStatoLogin_Opening(object sender, CancelEventArgs e)
        {
            Infragistics.Win.Misc.UltraPopupControlContainer popup = sender as Infragistics.Win.Misc.UltraPopupControlContainer;
            popup.PopupControl = _ucEasyGridStatoLogin;
        }

        private void ucEasyGridStatoLogin_ClickCell(object sender, ClickCellEventArgs e)
        {
            CoreStatics.CoreApplication.MH_LoginSelezionato.CodStatoMHLogin = e.Cell.Row.Cells["Codice"].Text;
            CoreStatics.CoreApplication.MH_LoginSelezionato.Salva();
            this.CaricaControlli();
            this.UltraPopupControlContainerStatoLogin.Close();
        }

        #endregion

        #region Events UltraPopupControlContainerLoginUA

        private void UltraPopupControlContainerLoginUA_Closed(object sender, EventArgs e)
        {
            _ucEasyGridLoginUA.ClickCell -= ucEasyGridLoginUA_ClickCell;
        }

        private void UltraPopupControlContainerLoginUA_Opened(object sender, EventArgs e)
        {
            _ucEasyGridLoginUA.ClickCell += ucEasyGridLoginUA_ClickCell;
            _ucEasyGridLoginUA.Focus();
        }

        private void UltraPopupControlContainerLoginUA_Opening(object sender, CancelEventArgs e)
        {
            Infragistics.Win.Misc.UltraPopupControlContainer popup = sender as Infragistics.Win.Misc.UltraPopupControlContainer;
            popup.PopupControl = _ucEasyGridLoginUA;
        }

        private void ucEasyGridLoginUA_ClickCell(object sender, ClickCellEventArgs e)
        {
            this.SalvaUA(e.Cell.Row.Cells["Codice"].Text);
            this.CaricaUA();
            this.UltraPopupControlContainerLoginUA.Close();
        }

        #endregion

        #region Events UltraPopupControlContainerLoginContatti

        private void UltraPopupControlContainerLoginContatti_Closed(object sender, EventArgs e)
        {
            _ucEasyPopUpMHContatti.AnnullaClick -= _ucEasyPopUpMHContatti_AnnullaClick;
            _ucEasyPopUpMHContatti.ConfermaClick -= _ucEasyPopUpMHContatti_ConfermaClick;
        }

        private void UltraPopupControlContainerLoginContatti_Opened(object sender, EventArgs e)
        {
            _ucEasyPopUpMHContatti.AnnullaClick += _ucEasyPopUpMHContatti_AnnullaClick;
            _ucEasyPopUpMHContatti.ConfermaClick += _ucEasyPopUpMHContatti_ConfermaClick;
            _ucEasyPopUpMHContatti.Focus();
        }

        private void UltraPopupControlContainerLoginContatti_Opening(object sender, CancelEventArgs e)
        {
            Infragistics.Win.Misc.UltraPopupControlContainer popup = sender as Infragistics.Win.Misc.UltraPopupControlContainer;
            popup.PopupControl = _ucEasyPopUpMHContatti;
        }

        private void _ucEasyPopUpMHContatti_AnnullaClick(object sender, EventArgs e)
        {
            this.UltraPopupControlContainerLoginContatti.Close();
        }

        private void _ucEasyPopUpMHContatti_ConfermaClick(object sender, EventArgs e)
        {
            this.SalvaContatti(_ucEasyPopUpMHContatti.NumeroTelefono, _ucEasyPopUpMHContatti.Descrizione, _ucEasyPopUpMHContatti.Tag.ToString(), _ucEasyPopUpMHContatti.ID);
            this.CaricaContatti();
            this.UltraPopupControlContainerLoginContatti.Close();
        }

        #endregion
    
    }
}
