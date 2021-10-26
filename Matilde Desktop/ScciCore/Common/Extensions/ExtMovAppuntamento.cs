using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.ScciCore.Common.Extensions
{
    public static class ExtMovAppuntamento
    {

        #region     Erogazione

        public static bool Ripianificazione(this MovAppuntamento app, string codUA, ScciAmbiente ambiente)
        {

            bool bret = false;

            try
            {

                if (app.ControlloRipianificazione(ambiente))
                {
                    bret = app.Ripianifica(codUA, ambiente);
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            return bret;

        }

        private static bool ControlloRipianificazione(this MovAppuntamento app, ScciAmbiente ambiente)
        {

            bool result = false;

            XmlParameter xp = new XmlParameter();
            xp.AddParameter("CodTipoAppuntamento", app.CodTipoAppuntamento);
            xp.AddParameter("IDPaziente", app.IDPaziente);
            xp.AddParameter("IDEpisodio", app.IDEpisodio);

            string tsString = ExtMovAppuntamento.GetTimeStampXml(ambiente);
            xp.AddParameter("TimeStamp", tsString);

            using (FwDataConnection fdc = new FwDataConnection(Database.ConnectionString))
            {
                DataTable table = fdc.Query<DataTable>("MSP_ControlloRipianificazioneAppuntamento", xp.ToFwDataParametersList(), CommandType.StoredProcedure);

                if (table == null)
                    throw new Exception("MSP_ControlloRipianificazioneAppuntamento risultato NULL");

                if (table.Rows.Count > 0)
                    result = Convert.ToBoolean(table.Rows[0]["Esito"]);

                fdc.Close();
            }

            return result;
        }

        private static bool Ripianifica(this MovAppuntamento app, string codUA, ScciAmbiente ambiente)
        {

            CoreStatics.SetNavigazione(false);

            try
            {

                if (easyStatics.EasyMessageBox("Si desidera ripianificare l'appuntamento appena erogato ?", "Ripianificazione", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {

                    MovAppuntamento newApp = new MovAppuntamento(app.CodUA,
                                                        app.IDPaziente,
                                                        app.IDEpisodio,
                                                        app.IDTrasferimento);

                    newApp.CodTipoAppuntamento = app.CodTipoAppuntamento;
                    newApp.DescrTipoAppuntamento = app.DescrTipoAppuntamento;
                    newApp.CodScheda = app.CodScheda;
                    newApp.TimeSlotInterval = app.TimeSlotInterval;
                    newApp.Multiplo = app.Multiplo;
                    newApp.SenzaData = app.SenzaData;
                    newApp.SenzaDataSempre = app.SenzaDataSempre;
                    newApp.Settimanale = app.Settimanale;
                    newApp.CaricaAgende();
                    foreach (MovAppuntamentoAgende oMaa in newApp.Elementi)
                    {
                        MovAppuntamentoAgende oItem = app.Elementi.Single(MovAppuntamentoAgende => MovAppuntamentoAgende.CodAgenda == oMaa.CodAgenda);
                        if (oItem != null && oItem.Selezionata)
                        {
                            oMaa.Selezionata = true;
                            oMaa.Modificata = true;
                        }
                    }

                    foreach (MovAppuntamentoAgende oMaa in newApp.ElementiAltri)
                    {
                        MovAppuntamentoAgende oItem = app.ElementiAltri.Single(MovAppuntamentoAgende => MovAppuntamentoAgende.CodAgenda == oMaa.CodAgenda);
                        if (oItem != null && oItem.Selezionata)
                        {
                            oMaa.Selezionata = true;
                            oMaa.Modificata = true;
                        }
                    }

                    CoreStatics.CoreApplication.MovAppuntamentiGenerati = new List<MovAppuntamento>();
                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato = newApp;

                    while (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneAgendeAppuntamento, false) == DialogResult.OK)
                    {

                        if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneAppuntamento, false) == DialogResult.OK)
                        {


                            if (CoreStatics.CoreApplication.MovAppuntamentiGenerati != null && CoreStatics.CoreApplication.MovAppuntamentiGenerati.Count > 1)
                            {

                                for (int ma = 0; ma < CoreStatics.CoreApplication.MovAppuntamentiGenerati.Count; ma++)
                                {

                                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato = CoreStatics.CoreApplication.MovAppuntamentiGenerati[ma];
                                    PluginClientStatics.PluginClient(EnumPluginClient.APP_NUOVO_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(codUA, ambiente));

                                }

                            }
                            else
                            {

                                PluginClientStatics.PluginClient(EnumPluginClient.APP_NUOVO_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(codUA, ambiente));

                            }

                            return true;

                        }

                    }

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                throw ex;
            }
            finally
            {
                CoreStatics.SetNavigazione(true);
            }

            return false;

        }

        #endregion  Erogazione

        #region         Private Util

        private static String GetTimeStampXml(ScciAmbiente ambiente)
        {
            TimeStamp ts = new TimeStamp(ambiente);
            ts.CodEntita = EnumEntita.WKI.ToString();
            ts.CodAzione = EnumAzioni.VIS.ToString();

            string xmlString = ts.ToXmlString();


            XDocument doc = XDocument.Parse(xmlString);
            XElement root = doc.Root;

            foreach (XAttribute attr in root.Attributes())
            {
                attr.Remove();
            }

            return doc.ToString();
        }

        #endregion

    }
}
