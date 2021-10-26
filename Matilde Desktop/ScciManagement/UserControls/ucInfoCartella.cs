using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace UnicodeSrl.ScciManagement.UserControls
{
    public partial class ucInfoCartella : UserControl
    {

        bool _errorecartella = false;

        public ucInfoCartella()
        {
            InitializeComponent();
        }

        #region Properties

        public string CodUnitaAtomica
        {
            get { return this.utxtCodUA.Text; }
            set { this.utxtCodUA.Text = value; }
        }

        public string NumeroCartella
        {
            get { return this.utxtNumCartella.Text; }
            set { this.utxtNumCartella.Text = value; }
        }

        public string InfoCartella
        {
            get { return this.utxtInfoCartella.Text; }
        }

        public bool ErroreCartella
        {
            get { return _errorecartella; }
        }

        #endregion

        #region Events

        private void GestValueChanged(object sender, EventArgs e)
        {
            SqlParameter[] sqlParams = new SqlParameter[2];
            SqlParameterCollection spcollout;
            DataSet oDs = null;

            try
            {

                if (((Infragistics.Win.UltraWinEditors.UltraTextEditor)sender).Name == "utxtCodUA")
                {
                    this.lblCodUADes.Text = DataBase.FindValue("Descrizione", "T_UnitaAtomiche", "Codice = '" + this.utxtCodUA.Text + "'", string.Empty);
                }

                if (this.utxtCodUA.Text != string.Empty && this.lblCodUADes.Text != string.Empty && this.utxtNumCartella.Text != string.Empty)
                {
                    sqlParams[0] = new SqlParameter("CodUA", this.utxtCodUA.Text);
                    sqlParams[1] = new SqlParameter("NumCartella", this.utxtNumCartella.Text);

                    oDs = UnicodeSrl.ScciManagement.DataBase.ExecStoredProc("MSP_BO_SelInfoCartella", sqlParams, out spcollout);

                    if (oDs != null)
                    {
                        if (oDs.Tables[0].Rows.Count > 0)
                        {
                            this.utxtInfoCartella.Text = oDs.Tables[0].Rows[0]["Info"].ToString();
                            bool.TryParse(oDs.Tables[0].Rows[0]["Errore"].ToString(), out this._errorecartella);
                        }
                        else
                        {
                            this.utxtInfoCartella.Text = "";
                            this._errorecartella = true;
                        }
                    }
                    else
                    {
                        this.utxtInfoCartella.Text = "";
                        this._errorecartella = true;
                    }
                }
                else
                {
                    this.utxtInfoCartella.Text = "";
                    this._errorecartella = true;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        private void utxtCodUA_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodUA.Text;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_UnitaAtomiche";
                f.ViewSqlStruct.Where = "Codice <> '" + this.utxtCodUA.Text + @"'";
                f.ViewSqlStruct.OrderBy = "Descrizione";

                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.utxtCodUA.Text = f.ViewActiveRow.Cells["Codice"].Text;
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
