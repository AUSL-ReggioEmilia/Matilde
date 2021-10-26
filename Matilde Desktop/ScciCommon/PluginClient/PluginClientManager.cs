using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Reflection;
using System.Windows.Forms;
using UnicodeSrl.Framework.Utility;

namespace UnicodeSrl.Scci.PluginClient
{

    public class PluginClientManager : IDisposable
    {

        public PluginClientManager()
        {
            this.Clear();
        }

        private Dictionary<string, object> CacheAssembly { get; set; }
        private Dictionary<string, object> CachePlugin { get; set; }

        public DataBindingsForAzioni DataBindingsForAzioni { get; set; }
        public string PathPlugin { get; set; }
        public bool SessioneRemota { get; set; }
        public bool IsOSServer { get; set; }

        private Risposta CaricaPlugin(Plugin plugin)
        {

            Risposta oRisposta = new Risposta();

            try
            {

                if (CacheAssembly.ContainsKey(plugin.NomePlugin) == false)
                {

                    string assembly = Path.Combine(AssemblyHelper.AssemblyPath, PathPlugin, string.Format("{0}", plugin.NomePlugin));

                    Assembly oAssembly = Assembly.LoadFrom(assembly);

                    CacheAssembly.Add(plugin.NomePlugin, oAssembly);

                }

                if (CachePlugin.ContainsKey(plugin.NomePlugin + plugin.Comando) == true)
                {
                    CachePlugin.Remove(plugin.NomePlugin + plugin.Comando);
                }
                Type _type = ((Assembly)CacheAssembly[plugin.NomePlugin]).GetType(plugin.Comando);

                IPluginClient PluginClient = (IPluginClient)Activator.CreateInstance(_type);

                CachePlugin.Add(plugin.NomePlugin + plugin.Comando, PluginClient);



                oRisposta.Successo = true;

                return oRisposta;

            }
            catch (Exception ex)
            {
                oRisposta.Parameters = new object[1] { ex.Message };
                oRisposta.ex = ex;
            }

            return oRisposta;

        }

        private Risposta EseguiPlugin(Plugin plugin, object[] myparam)
        {

            Risposta oRisposta = new Risposta();

            try
            {

                IPluginClient PluginClient = (IPluginClient)CachePlugin[plugin.NomePlugin + plugin.Comando];
                oRisposta = PluginClient.Execute(myparam, plugin.Modalita);

            }
            catch (Exception ex)
            {
                oRisposta.Parameters = new object[1] { ex.Message };
                oRisposta.ex = ex;
            }

            return (Risposta)Convert.ChangeType(oRisposta, typeof(Risposta));

        }

        public void Clear()
        {
            DataBindingsForAzioni = new DataBindingsForAzioni();
            CachePlugin = new Dictionary<string, object>();
            CacheAssembly = new Dictionary<string, object>();

        }

        public RispostaCerca Cerca(string azione, List<string> codua)
        {

            RispostaCerca oRispostaCerca = new RispostaCerca();

            try
            {

                if (DataBindingsForAzioni.Azioni.ContainsKey(azione) == true)
                {

                    if (DataBindingsForAzioni.Azioni[azione].Plugins.Count > 0)
                    {

                        foreach (Plugin oPlugin in DataBindingsForAzioni.Azioni[azione].Plugins)
                        {

                            if (codua.Contains(oPlugin.CodUA))
                            {

                                oRispostaCerca.Plugin.Add(oPlugin);

                                oRispostaCerca.DaEseguire = true;

                                if (CacheAssembly.ContainsKey(oPlugin.NomePlugin) == false)
                                {

                                    oRispostaCerca.PluginDaScaricare.Add(oPlugin);

                                }

                            }

                        }

                    }
                    else
                    {
                        oRispostaCerca.Parameters = new object[1] { @"Plugin NON trovati!!!" };
                    }

                }
                else
                {
                    oRispostaCerca.Parameters = new object[1] { @"Azione NON trovata!!!" };
                }

                oRispostaCerca.Successo = true;

            }
            catch (Exception ex)
            {
                oRispostaCerca.Parameters = new object[1] { ex.Message };
                oRispostaCerca.ex = ex;
            }

            return oRispostaCerca;

        }

        public Risposta Elabora(string azione, object[] myparam, List<string> codua)
        {

            Risposta oRispostaElabora = new Risposta();

            try
            {

                Dictionary<string, Plugin> _Plugins = new Dictionary<string, Plugin>();
                foreach (string value in codua)
                {
                    foreach (Plugin oPlugin in DataBindingsForAzioni.Azioni[azione].Plugins)
                    {
                        if (oPlugin.CodUA == value)
                        {
                            if (_Plugins.ContainsKey(oPlugin.Codice) == false)
                            {
                                _Plugins.Add(oPlugin.Codice, oPlugin);
                            }
                        }
                    }
                }

                var items = from pair in _Plugins
                            orderby pair.Value.Ordine ascending
                            select pair.Value;

                foreach (Plugin oPlugin in items)
                {

                    Risposta oRispostaCarica = CaricaPlugin(oPlugin);
                    if (oRispostaCarica.Successo == true)
                    {
                        oRispostaElabora = EseguiPlugin(oPlugin, myparam);
                        if (oRispostaElabora.Successo == false)
                        {
                            break;
                        }
                    }
                    else
                    {
                        oRispostaElabora.Parameters = new object[1] { string.Format(@"Plugin ({0}) NON caricato!!!", oPlugin.Codice) };
                        break;
                    }

                    oRispostaElabora.Successo = true;

                }

            }
            catch (Exception ex)
            {
                oRispostaElabora.Parameters = new object[1] { ex.Message };
                oRispostaElabora.ex = ex;
            }

            return oRispostaElabora;

        }

        public RispostaCerca CercaPlugin(Plugin plugin)
        {

            RispostaCerca oRispostaCerca = new RispostaCerca();

            try
            {

                oRispostaCerca.DaEseguire = true;

                if (CacheAssembly.ContainsKey(plugin.NomePlugin) == false)
                {
                    oRispostaCerca.PluginDaScaricare.Add(plugin);
                }

                oRispostaCerca.Successo = true;

            }
            catch (Exception ex)
            {
                oRispostaCerca.Parameters = new object[1] { ex.Message };
                oRispostaCerca.ex = ex;
            }

            return oRispostaCerca;

        }

        public Risposta ElaboraPlugin(Plugin plugin, object[] myparam)
        {

            Risposta oRispostaElabora = new Risposta();

            try
            {

                Risposta oRispostaCarica = CaricaPlugin(plugin);
                if (oRispostaCarica.Successo == true)
                {
                    oRispostaElabora = EseguiPlugin(plugin, myparam);
                }
                else
                {
                    oRispostaElabora.Parameters = new object[1] { string.Format(@"Plugin ({0}) NON caricato!!!", plugin.Codice) };
                }

            }
            catch (Exception ex)
            {
                oRispostaElabora.Parameters = new object[1] { ex.Message };
                oRispostaElabora.ex = ex;
            }

            return oRispostaElabora;

        }

        public void Dispose()
        {
            this.Clear();
        }

    }

}
