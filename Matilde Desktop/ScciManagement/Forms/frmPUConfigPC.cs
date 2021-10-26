using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.ScciResource;
using UnicodeSrl.ScciCore;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Statics;
using Infragistics.Win.UltraWinToolbars;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmPUConfigPC : Form, Interfacce.IViewFormPUView
    {
        public frmPUConfigPC()
        {
            InitializeComponent();
        }

        #region Declare

        private PUDataBindings _DataBinds = new PUDataBindings();
        private Enums.EnumModalityPopUp _Modality;
        private Enums.EnumDataNames _ViewDataNamePU;
        private DialogResult _DialogResult = DialogResult.Cancel;

        #endregion

        #region Interface

        public System.Windows.Forms.DialogResult ViewDialogResult
        {
            get
            {
                return _DialogResult;
            }
            set
            {
                _DialogResult = value;
            }
        }

        public PUDataBindings ViewDataBindings
        {
            get
            {
                return _DataBinds;
            }
            set
            {
                _DataBinds = value;
            }
        }

        public Enums.EnumDataNames ViewDataNamePU
        {
            get
            {
                return _ViewDataNamePU;
            }
            set
            {
                _ViewDataNamePU = value;
            }
        }

        public Image ViewImage
        {
            get
            {
                return this.picView.Image;
            }
            set
            {
                this.picView.Image = value;
            }
        }

        public Enums.EnumModalityPopUp ViewModality
        {
            get
            {
                return _Modality;
            }
            set
            {
                _Modality = value;
            }
        }

        public Icon ViewIcon
        {
            get
            {
                return this.Icon;
            }
            set
            {
                this.Icon = value;
            }
        }

        public string ViewText
        {
            get
            {
                return (string)this.Tag;
            }
            set
            {
                this.Tag = value;
                this.Text = string.Format("{0} - {1}", MyStatics.GetDataNameModalityFormPU(_Modality), value);
            }
        }

        public void ViewInit()
        {

            this.SuspendLayout();

            SetBindings();

            switch (_Modality)
            {

                case Enums.EnumModalityPopUp.mpNuovo:
                    MyStatics.SetControls(this.ultraGroupBoxForm.Controls, true);
                    break;

                case Enums.EnumModalityPopUp.mpModifica:
                    MyStatics.SetControls(this.ultraGroupBoxForm.Controls, true);
                    this.uteCodPC.Enabled = false;
                    break;

                case Enums.EnumModalityPopUp.mpCancella:
                    MyStatics.SetControls(this.ultraGroupBoxForm.Controls, false);
                    break;

                case Enums.EnumModalityPopUp.mpVisualizza:
                    MyStatics.SetControls(this.ultraGroupBoxForm.Controls, false);
                    this.ubConferma.Enabled = false;
                    break;

                default:
                    break;

            }

            this.ResumeLayout();

        }

        #endregion

        #region subroutines

        private void SetBindings()
        {
            DataColumn _dcol = null;

            try
            {

                _DataBinds.DataBindings.Add("Text", "CodPC", this.uteCodPC);
                _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione);

                _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodPC"];
                if (_dcol.MaxLength > 0) this.uteCodPC.MaxLength = _dcol.MaxLength;
                _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                if (_dcol.MaxLength > 0) this.uteDescrizione.MaxLength = _dcol.MaxLength;

                _DataBinds.DataBindings.Load();

                ConfigPC confPC = Database.GetConfigPCTable(this.uteCodPC.Text);

                this.chkTrace.Checked = confPC.configDebug.EnableTrace;

                this.umeRtfZoom.Value = confPC.configRtf.Zoom;

                this.umeFontCoefficiente.Value = confPC.configFont.Coefficiente;

                this.utxtAgendaChiamataNumeri.Text = confPC.configChiamataNumeri.CodiceAgenda;
                this.chkApriCartellaChiamata.Checked = confPC.configChiamataNumeri.ApriCartellaSuChiamata;

                this.chkIpovedente.Checked = confPC.configIpovedente.Ipovedente;

            }
            catch (Exception)
            {

            }
        }

        private bool CheckInput()
        {
            bool bReturn = true;

            if (bReturn)
            {
                if (this.uteCodPC.Text.Trim() == "")
                {
                    MessageBox.Show(@"Inserire " + this.lblCodPC.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    this.uteCodPC.Focus();
                    bReturn = false;
                }
            }

            if (bReturn)
            {
                if (this.utxtAgendaChiamataNumeri.Text.Trim() != "" && this.lblAgendaChiamataNumeriDes.Text == "")
                {
                    if (MessageBox.Show(@"Il Codice Agenda " + this.utxtAgendaChiamataNumeri.Text + @" non esiste!" + Environment.NewLine + @"Vuoi salvare ugualmente?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No)
                    {
                        this.utxtAgendaChiamataNumeri.Focus();
                        bReturn = false;
                    }
                }
            }

            return bReturn;
        }

        private ConfigPC GetConfigPC()
        {
            ConfigPC oRet = new ConfigPC();

            if (oRet.configDebug == null) oRet.configDebug = new ConfigDebug();
            oRet.configDebug.EnableTrace = this.chkTrace.Checked;

            oRet.configRtf.Zoom = Convert.ToSingle(this.umeRtfZoom.Value);

            oRet.configFont.Coefficiente = Convert.ToSingle(this.umeFontCoefficiente.Value);

            oRet.configChiamataNumeri.CodiceAgenda = this.utxtAgendaChiamataNumeri.Text;
            oRet.configChiamataNumeri.ApriCartellaSuChiamata = this.chkApriCartellaChiamata.Checked;

            oRet.configIpovedente.Ipovedente = this.chkIpovedente.Checked;

            return oRet;
        }

        #endregion

        #region EVENTI

        private void ubAnnulla_Click(object sender, EventArgs e)
        {
            _DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void ubConferma_Click(object sender, EventArgs e)
        {
            try
            {
                switch (this.ViewModality)
                {
                    case Enums.EnumModalityPopUp.mpModifica:
                    case Enums.EnumModalityPopUp.mpNuovo:
                        if (CheckInput())
                        {
                            Database.SetConfigPCTable(this.uteCodPC.Text, GetConfigPC());
                            Database.ExecuteSql(@"Update T_ConfigPc Set Descrizione = '" + Database.testoSQL(this.uteDescrizione.Text) + @"' Where CodPc = '" + Database.testoSQL(this.uteCodPC.Text) + @"'");
                            _DialogResult = System.Windows.Forms.DialogResult.OK;
                            this.Close();
                        }
                        break;

                    case Enums.EnumModalityPopUp.mpCancella:
                        string sMsg = @"Confermi la cancellazione della Configurazione del Pc?";
                        if (MessageBox.Show(sMsg, "Cancellazione Configurazione Pc", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
                        {
                            Database.ExecuteSql(this.ViewDataBindings.SqlDelete.Sql);
                            _DialogResult = System.Windows.Forms.DialogResult.OK;
                            this.Close();
                        }
                        break;
                    case Enums.EnumModalityPopUp.mpVisualizza:
                        break;
                    case Enums.EnumModalityPopUp.mpCopia:
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }


        #endregion

        private void utxtAgendaChiamataNumeri_ValueChanged(object sender, EventArgs e)
        {
            this.lblAgendaChiamataNumeriDes.Text = DataBase.FindValue("Descrizione", "T_Agende", string.Format("Codice = '{0}'", DataBase.Ax2(this.utxtAgendaChiamataNumeri.Text)), "");
        }

        private void utxtAgendaChiamataNumeri_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblAgendaChiamataNumeri.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_Agende";
                f.ViewSqlStruct.Where = "Codice <> '" + this.utxtAgendaChiamataNumeri.Text + @"'";

                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.utxtAgendaChiamataNumeri.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }


    }
}
