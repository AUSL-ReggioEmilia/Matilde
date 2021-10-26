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

using Microsoft.Data.SqlClient;
using System.Globalization;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;

namespace UnicodeSrl.ScciCore
{
    public partial class frmSelezionaTipoVoceDiario : frmBaseModale, Interfacce.IViewFormlModal
    {
        public frmSelezionaTipoVoceDiario()
        {
            InitializeComponent();
        }

        #region Interface

        public void Carica()
        {

            try
            {

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_DIARIOMEDICO_256);
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_DIARIOMEDICO_16);

                setControlliCopiaDaPrecedente(false);

                this.InitializeUltraGrid();
                this.LoadUltraGrid();

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

        #region UltraGrid

        private void InitializeUltraGrid()
        {
                        this.UltraGrid.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Large;
        }

        private void LoadUltraGrid()
        {

            try
            {

                                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodUA", CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.CodUA);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                op.Parametro.Add("CodAzione", CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.Azione.ToString());

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable oDt = Database.GetDataTableStoredProc("MSP_SelTipoDiarioClinico", spcoll);

                this.UltraGrid.DataSource = oDt;
                this.UltraGrid.Refresh();

                this.UltraGrid.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);
                this.UltraGrid.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;

                if (CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.CodTipoVoceDiario != string.Empty && CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.CodTipoVoceDiario != "")
                {
                    var item = this.UltraGrid.Rows.Single(UltraGridRow => UltraGridRow.Cells["CodVoce"].Text == CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.CodTipoVoceDiario);
                    if (item != null) { this.UltraGrid.ActiveRow = item; }
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void setControlliCopiaDaPrecedente(bool abilita)
        {
            if (!abilita) this.uceCopiaDaPrecedente.Checked = false;

            this.uceCopiaDaPrecedente.Enabled = abilita;
            this.lblCopiaDaPrecedente.Enabled = this.uceCopiaDaPrecedente.Enabled;
        }

        private bool CopiaDaPrecedenteAbilitata()
        {
            bool bAbilitato = false;
            try
            {
                if (this.UltraGrid.ActiveRow != null)
                {
                    if (this.UltraGrid.ActiveRow.Cells.Exists("CopiaDaPrecedente"))
                    {
                                                if (this.UltraGrid.ActiveRow.Cells["CopiaDaPrecedente"].Text.ToString().Trim() != "")
                        {
                            bAbilitato = (bool)this.UltraGrid.ActiveRow.Cells["CopiaDaPrecedente"].Value;
                        }
                    }
                    else
                    {
                        
                        string sql = @" Select IsNull(CopiaDaPrecedente, 0) As AbilitaCopia
                                    From T_TipoVoceDiario
                                    Where Codice = '" + this.UltraGrid.ActiveRow.Cells["CodVoce"].Text + @"'";

                        DataTable dt = Database.GetDatatable(sql);
                        if (dt != null)
                        {
                            if (dt.Rows.Count > 0 && !dt.Rows[0].IsNull("AbilitaCopia")) bAbilitato = (bool)dt.Rows[0]["AbilitaCopia"];

                            dt.Dispose();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            return bAbilitato;
        }

        #endregion

        #region Events

        private void UltraGrid_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {

                e.Layout.Bands[0].ColHeadersVisible = false;

                foreach (UltraGridColumn ocol in e.Layout.Bands[0].Columns)
                {
                    switch (ocol.Key)
                    {
                        case "Descrizione":
                            ocol.Hidden = false;
                            break;
                        default:
                            ocol.Hidden = true;
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        private void frmSelezionaTipoVoceDiario_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            try
            {

                if (this.UltraGrid.ActiveRow != null)
                {
                    CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.CodTipoVoceDiario = this.UltraGrid.ActiveRow.Cells["CodVoce"].Text;
                    CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.DescrTipoVoceDiario = this.UltraGrid.ActiveRow.Cells["Descrizione"].Text;
                    CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.CodScheda = this.UltraGrid.ActiveRow.Cells["CodScheda"].Text;

                    bool bContinua = true;
                    if (this.uceCopiaDaPrecedente.Checked && CopiaDaPrecedenteAbilitata())
                    {
                        switch (CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.PopolaDaPrecedente(true))
                        {
                            case MovDiarioClinico.enumPopolaDaPrecedenteReturn.precedente_non_trovato:
                                easyStatics.EasyMessageBox("Non è stata trovata una Voce Diario Clinico precedente da copiare.", "Nuovo Diario Clinico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                bContinua = true;
                                break;
                            case MovDiarioClinico.enumPopolaDaPrecedenteReturn.errori:
                                bContinua = false;
                                break;
                            default:
                                bContinua = true;
                                break;
                        }
                    }
                        

                    if (bContinua)
                    {
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                        this.Close();
                    }

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        private void frmSelezionaTipoVoceDiario_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void UltraGrid_AfterRowActivate(object sender, EventArgs e)
        {
                        bool bAbilita = CopiaDaPrecedenteAbilitata();
            setControlliCopiaDaPrecedente(bAbilita);
        }

        #endregion

    }
}
