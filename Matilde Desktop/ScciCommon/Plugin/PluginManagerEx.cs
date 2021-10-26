using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using UnicodeSrl.Framework.UpdateServer;
using UnicodeSrl.Framework.Updater;
using UnicodeSrl.Scci.Enums;

namespace UnicodeSrl.Scci.Plugin
{
    public class PluginManagerEx :
        IDisposable
    {
        private const string SlotName = "Slot.01F1C93D-90F9-4D8A-86E4-44BE215E6CAE";
        private const string TempPref = @".dll.@ucdl_";

        private AppDomain m_appDomain = null;
        private bool m_useCurrentAppDomain;
        private bool mb_IsOSServer;


        public PluginManagerEx(bool useCurrAppDomain = false, bool isosserver = false)
        {
            UseCurrentAppDomain = useCurrAppDomain;
            IsOSServer = isosserver;
        }

        ~PluginManagerEx()
        {
            Dispose();
        }

        public bool CheckUpdates(string appToCheck, UpdaterConfig cfg)
        {
            if (UseCurrentAppDomain == true)
                return _checkUpdatesCurrentAppDomain(appToCheck, cfg);
            else
                return _checkUpdatesNewAppDomain(appToCheck, cfg);

        }

        public bool CheckUpdates(string appToCheck)
        {
#if !DEBUG
            try
            {
                if (IsOSServer) return true;
                UpdaterConfig cfg = readScciConfig();

                if (cfg.ServerAddress == "")                   
                    return true;
                else
                    return CheckUpdates(appToCheck, cfg);
            }
            catch (Exception ex)
            {               
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                return true;
            }
#endif

            return true;
        }

        private bool _checkUpdatesNewAppDomain(string appToCheck, UpdaterConfig cfg)
        {
            AppUpdater au = new AppUpdater(appToCheck, cfg);

            bool ok = au.UpdateAvailable();


            if (ok)
            {

                UpdateInfo ui = au.GetLastUpdateInfo();


                if (ui != null)
                {
                    bool bObb = ui.ForceUpdate;
                    bool updated = au.Update(ui);

                    if (updated) return true;
                    else
                    {
                        if (bObb) return false;
                        else return true;
                    }

                };

            }
            else
                return true;

            return false;

        }

        private bool _checkUpdatesCurrentAppDomain(string appToCheck, UpdaterConfig cfg)
        {
            AppUpdater au = new AppUpdater(appToCheck, cfg);

            bool ok = au.UpdateAvailable();

            if (ok)
            {
                UpdateInfo ui = au.GetLastUpdateInfo();


                if (ui != null)
                {
                    bool bObb = ui.ForceUpdate;
                    bool updated = au.Update(ui);

                    if (updated) return true;
                    else
                    {
                        if (bObb) return false;
                        else return true;
                    }

                };

            }
            else
                return true;

            return false;

        }

        public bool UseCurrentAppDomain
        {
            get { return m_useCurrentAppDomain; }
            private set { m_useCurrentAppDomain = value; }
        }

        public bool IsOSServer
        {
            get { return mb_IsOSServer; }
            private set { mb_IsOSServer = value; }

        }
        public PluginProxy LoadPlugin(string path, string pluginName)
        {

            if (this.UseCurrentAppDomain)
            {
                m_appDomain = AppDomain.CurrentDomain;

                string rootPath = System.IO.Path.GetTempPath();

                _cleanUpTemp(rootPath, pluginName);

                string newFile = rootPath + pluginName + TempPref + System.IO.Path.GetRandomFileName();
                File.Copy(path, newFile);
                path = newFile;
            }
            else
            {
                AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
                setup.ShadowCopyFiles = "true";

                System.IO.FileInfo f = new System.IO.FileInfo(path);
                setup.ApplicationBase = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                setup.PrivateBinPath = f.DirectoryName;

                m_appDomain = AppDomain.CreateDomain(pluginName, AppDomain.CurrentDomain.Evidence, setup);
                m_appDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
                m_appDomain.SetThreadPrincipal(new WindowsPrincipal(WindowsIdentity.GetCurrent()));
            }

            var handle = m_appDomain.CreateInstanceFrom(Assembly.GetExecutingAssembly().Location, typeof(PluginProxy).FullName);

            PluginProxy h = (PluginProxy)handle.Unwrap();
            h.CreateInstance(path);

            return h;

        }


        private void _cleanUpTemp(string path, string pluginName)
        {
            try
            {
                string srcPattern = pluginName + TempPref + @"*";

                IEnumerable<string> files = Directory.EnumerateFiles(path, srcPattern);

                foreach (string item in files)
                {
                    try
                    {
                        File.Delete(item);
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }
        }

        internal UpdaterConfig readScciConfig()
        {
            UpdaterConfig cfg = new UpdaterConfig();

            DataTable dt = Scci.Statics.Database.GetConfigUpdater();

            string _svcSecDomain = "";
            string _svcSecUsername = "";
            string _svcSecPassword = "";

            string _secDomain = "";
            string _secUsername = "";
            string _secPassword = "";

            Scci.Encryption ocrypt = new Encryption(Scci.Statics.EncryptionStatics.GC_DECRYPTKEY, Scci.Statics.EncryptionStatics.GC_INITVECTOR);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int id = Convert.ToInt32(dt.Rows[i]["ID"]);
                Enums.EnumConfigTable ecfg = (Enums.EnumConfigTable)id;

                switch (ecfg)
                {
                    case EnumConfigTable.WSUpdaterUseUserFolder:
                        cfg.UserDocsFolder = Convert.ToBoolean(Convert.ToInt32(dt.Rows[i]["Valore"]));
                        break;

                    case EnumConfigTable.WSUpdaterAddress:
                        cfg.ServerAddress = (string)dt.Rows[i]["Valore"];
                        break;
                    case EnumConfigTable.WSUpdaterDomain:
                        _secDomain = (string)dt.Rows[i]["Valore"];
                        break;
                    case EnumConfigTable.WSUpdaterUserName:
                        _secUsername = (string)dt.Rows[i]["Valore"];
                        break;
                    case EnumConfigTable.WSUpdaterPassword:
                        _secPassword = ocrypt.DecryptString((string)dt.Rows[i]["Valore"]);
                        break;
                    case EnumConfigTable.WSUpdaterShowUI:
                        cfg.ShowUI = Convert.ToBoolean(Convert.ToInt32(dt.Rows[i]["Valore"]));
                        break;
                    case EnumConfigTable.WSUpdaterSvcSecDomain:
                        _svcSecDomain = (string)dt.Rows[i]["Valore"];
                        break;
                    case EnumConfigTable.WSUpdaterSvcSecUserName:
                        _svcSecUsername = (string)dt.Rows[i]["Valore"];
                        break;
                    case EnumConfigTable.WSUpdaterSvcSecPassword:
                        _svcSecPassword = ocrypt.DecryptString((string)dt.Rows[i]["Valore"]);
                        break;
                    case EnumConfigTable.WSUpdaterAlwaysCheckFiles:
                        cfg.AlwaysCheckFiles = Convert.ToBoolean(Convert.ToInt32(dt.Rows[i]["Valore"]));
                        break;
                }

            }

            cfg.SetUser(_secDomain, _secUsername, _secPassword);
            cfg.SetWebServiceUser(_svcSecDomain, _svcSecUsername, _svcSecPassword);


            return cfg;
        }

        public void Dispose()
        {
            if (m_appDomain != null)
            {
                try
                {
                    if (this.UseCurrentAppDomain == false)
                    {
                        AppDomain.Unload(m_appDomain);
                        m_appDomain = null;
                    }

                }
                catch (Exception ex)
                {
                    Console.Write(ex.ToString());
                }

            }
        }
    }
}
