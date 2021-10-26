using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Plugin;

namespace UnicodeSrl.ScciCore.Common
{
    internal class PluginCaller : IScciPlugin
    {
        private string m_path;
        private string m_name;
        private bool m_useCurrAD;
        private bool m_SessioneRemota;
        private bool m_IsOSServer;

        private PluginManagerEx m_man;

        public PluginCaller(string pluginPath, string pluginName,
                            bool useCurrentAppDomain = false, bool SessioneRemota = false, bool IsOSServer = false)
        {
            m_path = pluginPath;
            m_name = pluginName;
            m_useCurrAD = useCurrentAppDomain;
            m_SessioneRemota = SessioneRemota;
            m_IsOSServer = IsOSServer;
        }

        public object Esegui(Dictionary<string, object> dict_pars)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString(@"HH:mm:ss.ffff") + @" - new PluginManagerEx(m_useCurrAD)...");
            m_man = new PluginManagerEx(m_useCurrAD);

            bool upd = true;

            if (m_IsOSServer == false)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString(@"HH:mm:ss.ffff") + @" - m_man.CheckUpdates(m_name)...");
                upd = m_man.CheckUpdates(m_name);
            }
            PluginProxy p = null;

            if (upd == false)
            {
                string s = "Non è possibile aggiornare il plugin: " + m_name;
                Exception ex = new Exception(s);
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                throw ex;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString(@"HH:mm:ss.ffff") + @" - p = (PluginProxy)m_man.LoadPlugin(m_path, m_name)...");
                p = (PluginProxy)m_man.LoadPlugin(m_path, m_name);


                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString(@"HH:mm:ss.ffff") + @" - Esegui...");

                Dictionary<string, object> validParams = new Dictionary<string, object>();

                foreach (KeyValuePair<String, object> item in dict_pars)
                {
                    Type t = item.Value.GetType();
                    if (t.IsSerializable) validParams.Add(item.Key, item.Value);
                }

                object result = p.Esegui(validParams);

                p.Dispose();
                p = null;

                return result;

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                throw (ex);
            }
        }

        public void Dispose()
        {
            if (m_man != null)
                m_man.Dispose();

            m_man = null;
        }

    }
}
