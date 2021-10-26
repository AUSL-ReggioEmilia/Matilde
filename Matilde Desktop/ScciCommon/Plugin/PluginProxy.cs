using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UnicodeSrl.Scci.Plugin
{
    public class PluginProxy
                :
                MarshalByRefObject,
                IScciPlugin
    {
        private IScciPlugin m_instance = null;

        public void CreateInstance(string path)
        {
            Assembly asm = Assembly.LoadFrom(path);

            foreach (Type t in asm.GetExportedTypes())
            {
                if (typeof(IScciPlugin).IsAssignableFrom(t))
                {
                    m_instance = (IScciPlugin)asm.CreateInstance(t.FullName);
                    return;
                }
            }
        }

        public object Esegui(Dictionary<string, object> dict_pars)
        {
            return m_instance.Esegui(dict_pars);
        }

        public void Dispose()
        {
            try
            {
                m_instance.Dispose();
                m_instance = null;
            }
            catch (Exception ex)
            {
                Console.Write("");
            }

        }
    }
}
