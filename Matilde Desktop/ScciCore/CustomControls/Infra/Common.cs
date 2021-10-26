using Infragistics.Shared;
using System;

namespace UnicodeSrl.ScciCore.CustomControls.Infra
{
    public class Common
    {
        #region Constructor
        private Common()
        {
        }
        #endregion

        #region Constants

        private const string DataRegistryKey = @"Infragistics\NetAdvantage\Net\Full\WinForms\CLR4x\Version" + AssemblyVersion.MajorMinor + @"\WinDataDir";
        #endregion

        #region DataPath
        public static string DataPath
        {
            get
            {
                Microsoft.Win32.RegistryKey dataRegKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(DataRegistryKey);

                string path = null;

                if (dataRegKey != null)
                {
                    path = dataRegKey.GetValue(null) as string;
                    dataRegKey.Close();
                }

                return path;
            }
        }
        #endregion
    }
}
