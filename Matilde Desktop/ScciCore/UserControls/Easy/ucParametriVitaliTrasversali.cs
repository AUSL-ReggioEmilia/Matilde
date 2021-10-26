using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;

using Microsoft.Data.SqlClient;
using System.Globalization;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.ScciResource;
using UnicodeSrl.ScciCore.CustomControls.Infra;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;

namespace UnicodeSrl.ScciCore
{
    public partial class ucParametriVitaliTrasversali : UserControl, Interfacce.IViewUserControlMiddle
    {
        public ucParametriVitaliTrasversali()
        {
            InitializeComponent();
        }

        #region INTERFACCIA

        public void Aggiorna()
        {
        }

        public void Carica()
        {

            InizializzaControlli();

            CaricaMultiSelectUA();
            CaricaMultiSelectPV();

        }

        public void Ferma()
        {

            try
            {

                Dictionary<string, string> oDictionaryUA = this.ListaUnitaAtomicheSelezionate;
                CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.UnitaSelezionate = oDictionaryUA.Keys.ToArray();

                Dictionary<string, TipoParametroVitale> oDictionaryPVT = this.ListaTipiParametriVitaliSelezionati;
                CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.ParametriSelezionati = oDictionaryPVT.Keys.ToArray();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Ferma", this.Name);
            }

        }

        #endregion

        #region PROPRIETA' PUBBLICHE

        public Dictionary<string, TipoParametroVitale> ListaTipiParametriVitaliSelezionati
        {
            get
            {
                Dictionary<string, TipoParametroVitale> _return = new Dictionary<string, TipoParametroVitale>();

                try
                {
                    DataSet ds = this.ucMultiSelectPV.ViewDataSetDX.GetChanges();
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        foreach (DataRow oRow in ds.Tables[0].Rows)
                        {
                            if (oRow.RowState == DataRowState.Added)
                            {
                                TipoParametroVitale tpv = new TipoParametroVitale();
                                tpv.Codice = oRow["Codice"].ToString();
                                tpv.Descrizione = oRow["Descrizione"].ToString();
                                tpv.CodScheda = oRow["CodScheda"].ToString();
                                _return.Add(oRow["Codice"].ToString(), tpv);
                            }
                        }
                    }
                    if (ds != null) ds.Dispose();
                }
                catch (Exception)
                {
                }

                return _return;
            }
        }

        public Dictionary<string, string> ListaUnitaAtomicheSelezionate
        {
            get
            {
                Dictionary<string, string> _return = new Dictionary<string, string>();

                try
                {
                    DataSet ds = this.ucMultiSelectUA.ViewDataSetDX.GetChanges();
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        foreach (DataRow oRow in ds.Tables[0].Rows)
                        {
                            if (oRow.RowState == DataRowState.Added)
                            {
                                _return.Add(oRow["Codice"].ToString(), oRow["Descrizione"].ToString());
                            }
                        }
                    }
                    if (ds != null) ds.Dispose();
                }
                catch (Exception)
                {
                }

                return _return;
            }
        }

        #endregion

        #region PRIVATE

        private void InizializzaControlli()
        {
            try
            {
                CoreStatics.SetEasyucMultiSelect(ref this.ucMultiSelectUA, easyStatics.easyRelativeDimensions.Medium);
                CoreStatics.SetEasyucMultiSelect(ref this.ucMultiSelectPV, easyStatics.easyRelativeDimensions.Medium);

            }
            catch (Exception)
            {
            }
        }

        private void CaricaMultiSelectUA()
        {

            try
            {

                DataSet dsUASX = null;
                DataSet dsUADX = null;

                this.ucMultiSelectUA.ViewShowAll = true;
                this.ucMultiSelectUA.ViewShowFind = true;

                this.ucMultiSelectUA.GridSXCaption = "Unità Disponibili";
                this.ucMultiSelectUA.GridSXCaptionColumnKey = "Descrizione";
                this.ucMultiSelectUA.GridDXCaption = "Unità Selezionate";
                this.ucMultiSelectUA.GridDXCaptionColumnKey = "Descrizione";

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);

                op.TimeStamp.CodEntita = EnumEntita.PVT.ToString(); op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                dsUASX = Database.GetDatasetStoredProc("MSP_SelUADaRuolo", spcoll);
                this.ucMultiSelectUA.ViewDataSetSX = dsUASX;

                dsUADX = dsUASX.Copy();
                for (int iRow = dsUADX.Tables[0].Rows.Count - 1; iRow >= 0; iRow--)
                {
                    dsUADX.Tables[0].Rows[iRow].Delete();
                }

                if (CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.UnitaSelezionate != null)
                {
                    foreach (string s in CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.UnitaSelezionate)
                    {
                        DataRow[] result = dsUASX.Tables[0].Select("Codice = '" + s + "'");
                        if (result.Length == 1)
                        {
                            DataRow oDr = dsUADX.Tables[0].NewRow();
                            for (int x = 0; x <= dsUADX.Tables[0].Columns.Count - 1; x++)
                            {
                                oDr[x] = result[0][x];
                            }
                            dsUADX.Tables[0].Rows.Add(oDr);
                            dsUASX.Tables[0].Rows.Remove(result[0]);
                        }
                    }
                }

                this.ucMultiSelectUA.ViewDataSetDX = dsUADX;

                this.ucMultiSelectUA.RefreshData();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaMultiSelectUA", this.Name);
            }


        }

        private void CaricaMultiSelectPV()
        {

            try
            {
                DataSet dsPVSX = null;
                DataSet dsPVDX = null;

                this.ucMultiSelectPV.ViewShowAll = true;
                this.ucMultiSelectPV.ViewShowFind = true;

                this.ucMultiSelectPV.GridSXCaption = "Parametri Disponibili";
                this.ucMultiSelectPV.GridSXCaptionColumnKey = "Descrizione";
                this.ucMultiSelectPV.GridDXCaption = "Parametri Selezionati";
                this.ucMultiSelectPV.GridDXCaptionColumnKey = "Descrizione";

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);


                Dictionary<string, string> listaUAsel = this.ListaUnitaAtomicheSelezionate;
                if (listaUAsel.Count <= 0) listaUAsel.Add("WXYZ", "");
                string[] coduas = listaUAsel.Keys.ToArray();
                op.ParametroRipetibile.Add("CodUA", coduas);

                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                op.Parametro.Add("CodAzione", EnumAzioni.INS.ToString());
                op.Parametro.Add("DatiEstesi", "0");

                op.TimeStamp.CodEntita = EnumEntita.PVT.ToString(); op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                dsPVSX = Database.GetDatasetStoredProc("MSP_SelTipoParametroVitale", spcoll);
                this.ucMultiSelectPV.ViewDataSetSX = dsPVSX;

                dsPVDX = dsPVSX.Copy();
                for (int iRow = dsPVDX.Tables[0].Rows.Count - 1; iRow >= 0; iRow--)
                {
                    dsPVDX.Tables[0].Rows[iRow].Delete();
                }

                if (CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.ParametriSelezionati != null)
                {
                    foreach (string s in CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.ParametriSelezionati)
                    {
                        DataRow[] result = dsPVSX.Tables[0].Select("Codice = '" + s + "'");
                        if (result.Length == 1)
                        {
                            DataRow oDr = dsPVDX.Tables[0].NewRow();
                            for (int x = 0; x <= dsPVDX.Tables[0].Columns.Count - 1; x++)
                            {
                                oDr[x] = result[0][x];
                            }
                            dsPVDX.Tables[0].Rows.Add(oDr);
                            dsPVSX.Tables[0].Rows.Remove(result[0]);
                        }
                    }
                }

                this.ucMultiSelectPV.ViewDataSetDX = dsPVDX;

                this.ucMultiSelectPV.RefreshData();
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaMultiSelectPV", this.Name);
            }


        }

        #endregion

        #region EVENTI

        private void ucMultiSelect_GridDXInitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            CoreStatics.SetEasyucMultiSelectGridInitializeLayout(ref e, easyStatics.easyRelativeDimensions.Medium);

            foreach (UltraGridColumn oGCol in e.Layout.Bands[0].Columns)
            {
                if (oGCol.Key.Trim().ToUpper().IndexOf("DESCR") == 0)
                    oGCol.Hidden = false;
                else
                    oGCol.Hidden = true;
            }

        }

        private void ucMultiSelectUA_GridChange(object sender, EventArgs e)
        {
            CaricaMultiSelectPV();
        }

        #endregion

    }
}
