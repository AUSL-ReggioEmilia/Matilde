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
using Infragistics.Win.UltraWinGrid;

namespace UnicodeSrl.ScciCore
{
    public partial class ucAgende : UserControl
    {
        public ucAgende()
        {
            InitializeComponent();
        }

        #region Declare

        bool _FirstLoad = true;
        string _CodAgenda = string.Empty;

        public event EventHandler Agende_AfterRowActivate;
        public event EventHandler ButtonSeleziona;

        #endregion

        #region Property

        public string CodAgenda
        {
            get { return _CodAgenda; }
            set
            {
                _CodAgenda = value;

                try
                {
                    CoreStatics.SelezionaRigaInGriglia(ref this.UltraGridAgende, "Codice", _CodAgenda);
                }
                catch (Exception)
                {
                }
            }
        }

        public string Agenda
        {
            get
            {
                if (this.UltraGridAgende.ActiveRow != null)
                {
                    return this.UltraGridAgende.ActiveRow.Cells["Descrizione"].Text;
                }
                else
                {
                    return "";
                }
            }
        }

        public bool IsLista
        {
            get
            {
                if (this.UltraGridAgende.ActiveRow != null)
                {
                    return bool.Parse(this.UltraGridAgende.ActiveRow.Cells["Lista"].Text);
                }
                else
                {
                    return false;
                }
            }
        }

        public string ParametriLista
        {
            get
            {
                if (this.UltraGridAgende.ActiveRow != null)
                {
                    return this.UltraGridAgende.ActiveRow.Cells["ParametriLista"].Text;
                }
                else
                {
                    return "";
                }
            }
        }


        #endregion

        #region UltraGrid

        private void LoadUltraGridAgende()
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                op.Parametro.Add("CodAzione", EnumAzioni.VIS.ToString());
                op.Parametro.Add("DatiEstesi", "1");

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataSet oDs = Database.GetDatasetStoredProc("MSP_SelAgende", spcoll);

                this.UltraGridAgende.DataSource = oDs;
                this.UltraGridAgende.Refresh();

                CoreStatics.SelezionaRigaInGriglia(ref this.UltraGridAgende, "Codice", _CodAgenda);
                if (this.UltraGridAgende.ActiveRow == null && this.UltraGridAgende.Rows.Count > 0)
                {
                    this.UltraGridAgende.ActiveRow = this.UltraGridAgende.Rows.GetRowWithListIndex(0);
                    this.UltraGridAgende.ActiveRow.Selected = true;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        #endregion

        #region Events

        private void UltraGridAgende_AfterRowActivate(object sender, EventArgs e)
        {
            if (this.UltraGridAgende.ActiveRow != null)
            {
                _CodAgenda = this.UltraGridAgende.ActiveRow.Cells["Codice"].Text;
            }
            else
            {
                _CodAgenda = "";
            }
            if (Agende_AfterRowActivate != null) { Agende_AfterRowActivate(sender, e); }

        }

        private void UltraGridAgende_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
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
            catch (Exception)
            {

            }

        }

        private void ubSeleziona_Click(object sender, EventArgs e)
        {
            if (ButtonSeleziona != null) { ButtonSeleziona(sender, e); }
        }

        #endregion

        #region Public Events

        public void RefreshData()
        {

            try
            {

                CoreStatics.SetEasyUltraGridLayout(ref this.UltraGridAgende);
                if (_FirstLoad == true) { this.LoadUltraGridAgende(); }
                _FirstLoad = false;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        #endregion

    }
}
