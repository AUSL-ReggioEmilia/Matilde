using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnicodeSrl.Scci.Model;
using UnicodeSrl.Scci.Plugin;

namespace UnicodeSrl.ScciCore
{
    public class TilePluginLoader
    {
        private static readonly List<Assembly> tileControlsCache = new List<Assembly>();
        private static readonly object Locker = new object();

        private const String C_PLUGIN_PATH = @"CDSSClient\";

        public static Assembly GetTilePluginAssembly(SelCdssRuoloRow cdssRow)
        {
            lock (Locker)
            {
                Assembly pluginAssembly = null;

                string assName = cdssRow.NomePlugin;
                string currPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

                string fullPath = Path.Combine(currPath, C_PLUGIN_PATH, assName);

                pluginAssembly = tileControlsCache.FirstOrDefault(p => ((p != null) && (p.Location == fullPath)));

                if (pluginAssembly == null)
                {

                    PluginManagerEx m_man = new PluginManagerEx(true, CoreStatics.CoreApplication.Sessione.Computer.IsOSServer);
                    bool upd = m_man.CheckUpdates(cdssRow.NomePlugin);

                    pluginAssembly = Assembly.LoadFrom(fullPath);
                    tileControlsCache.Add(pluginAssembly);
                }


                return pluginAssembly;
            }

        }

    }
}
