using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.Plugin;
using UnicodeSrl.Scci.PluginClient;

namespace UnicodeSrl.Scci.Statics
{
    public static class PluginClientStatics
    {

        private static void Esempio()
        {

            DataContracts.ScciAmbiente ambiente = null;

            PluginClientStatics.Pcm = PluginClientStatics.SetPluginClientManager(ambiente, "CHI1", false, false);

            object[] myparam = new object[1] { new object() };

            List<string> codua = new List<string>() { "1.1.1.1" };

            Risposta oRispostaElaboraPrima = PluginClientStatics.PluginClient(@"VALPRIMA", myparam, codua);
            if (oRispostaElaboraPrima.Successo == true)
            {

                PluginClientStatics.PluginClient(@"VALDOPO", myparam, codua);

            }
            else
            {
                PluginClientStatics.PluginClient(@"VALALTRIMENTI", myparam, codua);

            }

        }

        public static PluginClientManager Pcm { get; set; }

        public static PluginClientManager SetPluginClientManager(DataContracts.ScciAmbiente ambiente, string codruolo, bool sessioneremota, bool isosserver)
        {

            PluginClientManager Pcm = new PluginClientManager();

            try
            {

                Pcm.Clear();
                Pcm.SessioneRemota = sessioneremota;
                Pcm.IsOSServer = isosserver;

                Pcm.PathPlugin = @"CDSSClient\";

                Parametri op = new Parametri(ambiente);
                op.Parametro.Add("CodRuolo", codruolo);

                op.TimeStamp.CodAzione = Enums.EnumAzioni.VIS.ToString();
                op.TimeStamp.CodEntita = Enums.EnumEntita.XXX.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelCDSSClient", spcoll);

                Pcm.DataBindingsForAzioni.CodUAMember = "CodUA";
                Pcm.DataBindingsForAzioni.CodAzioneMember = "CodAzione";
                Pcm.DataBindingsForAzioni.DescrizioneAzioneMember = "DescrizioneAzione";
                Pcm.DataBindingsForAzioni.CodPluginMember = "CodPlugin";
                Pcm.DataBindingsForAzioni.DescrizionePluginMember = "DescrizionePlugin";
                Pcm.DataBindingsForAzioni.NomePluginMember = "NomePlugin";
                Pcm.DataBindingsForAzioni.ComandoPluginMember = "ComandoPlugin";
                Pcm.DataBindingsForAzioni.ModalitaPluginMember = "ModalitaPlugin";
                Pcm.DataBindingsForAzioni.OrdinePluginMember = "OrdinePlugin";
                Pcm.DataBindingsForAzioni.IconaPluginMember = "IconaPlugin";

                Pcm.DataBindingsForAzioni.SetDataBinding(dt, "");

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            return Pcm;

        }

        public static Risposta PluginClient(string azione, object[] myparam, List<string> codua)
        {
            Risposta oRispostaElabora = new Risposta();

            try
            {

                RispostaCerca oRispostaCerca = Pcm.Cerca(azione, codua);
                if (oRispostaCerca.Successo == true)
                {

                    if (oRispostaCerca.DaEseguire == true)
                    {

                        if (Pcm.IsOSServer == false)
                        {
                            if (oRispostaCerca.PluginDaScaricare.Count > 0)
                            {

                                foreach (UnicodeSrl.Scci.PluginClient.Plugin oPlugin in oRispostaCerca.PluginDaScaricare)
                                {

                                    PluginManagerEx m_man = new PluginManagerEx(true);
                                    bool upd = m_man.CheckUpdates(oPlugin.NomePlugin);

                                }

                            }

                        }

                        oRispostaElabora = Pcm.Elabora(azione, myparam, codua);

                    }
                    else
                    {
                        oRispostaElabora.Successo = true;
                    }

                }

            }
            catch (Exception ex)
            {
                oRispostaElabora.Parameters = new object[1] { ex.Message };
                oRispostaElabora.ex = ex;

            }

            return oRispostaElabora;
        }

        public static RispostaCerca PluginClientMenu(string azione, List<string> codua)
        {

            RispostaCerca oRispostaCerca = new RispostaCerca();

            try
            {

                oRispostaCerca = Pcm.Cerca(azione, codua);

            }
            catch (Exception ex)
            {
                oRispostaCerca.Parameters = new object[1] { ex.Message };
                oRispostaCerca.ex = ex;
            }

            return oRispostaCerca;

        }

        public static Risposta PluginClientMenuEsegui(UnicodeSrl.Scci.PluginClient.Plugin plugin, object[] myparam)
        {

            Risposta oRispostaElabora = new Risposta();

            try
            {

                RispostaCerca oRispostaCerca = Pcm.CercaPlugin(plugin);
                if (oRispostaCerca.Successo == true)
                {

                    if (oRispostaCerca.DaEseguire == true)
                    {

                        if (Pcm.IsOSServer == false)
                        {
                            if (oRispostaCerca.PluginDaScaricare.Count > 0)
                            {

                                foreach (UnicodeSrl.Scci.PluginClient.Plugin oPlugin in oRispostaCerca.PluginDaScaricare)
                                {

                                    PluginManagerEx m_man = new PluginManagerEx(true);
                                    bool upd = m_man.CheckUpdates(oPlugin.NomePlugin);

                                }

                            }

                        }

                        oRispostaElabora = Pcm.ElaboraPlugin(plugin, myparam);

                    }
                    else
                    {
                        oRispostaElabora.Successo = true;
                    }

                }

            }
            catch (Exception ex)
            {
                oRispostaElabora.Parameters = new object[1] { ex.Message };
                oRispostaElabora.ex = ex;

            }

            return oRispostaElabora;

        }

    }

}
