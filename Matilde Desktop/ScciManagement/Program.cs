using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;


using UnicodeSrl.ScciCore;

namespace UnicodeSrl.ScciManagement
{
    static class Program
    {

        public static Thread _thSplash = null;
        public static frmSplash _frmSplash = null;

        [STAThread]
        static void Main()
        {


            UnicodeSrl.Framework.Diagnostics.Statics.SetDiagnosticsDefaults();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            CultureInfo culture = CultureInfo.CreateSpecificCulture("it-IT");

            Thread.CurrentThread.CurrentUICulture = culture;

            Thread.CurrentThread.CurrentCulture = culture;

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            while (MyStatics.Configurazione.TestConnection() == false)
            {
                if (MyStatics.Configurazione.OpenConnection() == DialogResult.Cancel)
                {
                    System.Environment.Exit(0);
                }
            }
            CoreStatics.CoreApplication.ConnectionString = MyStatics.Configurazione.ConnectionString;


#if !DEBUG
            if (!CoreStatics.ControlloVersioneSCCIManagement(true))
            {
                System.Environment.Exit(0);
            }
#endif
            _thSplash = new Thread(new ThreadStart(DoSplash));
            _thSplash.IsBackground = true;
            _thSplash.Start();

            CoreStatics.CheckConfigTable(MyStatics.Configurazione.ConnectionString);

            string sWebServiceUrl = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.Diagnostics);
            if (sWebServiceUrl != "")
            {
                UnicodeSrl.Framework.Diagnostics.Statics.DefDbgSettings.WebServiceUrl = sWebServiceUrl;
                UnicodeSrl.Framework.Diagnostics.Statics.DefDbgSettings.SendToWebService = true;
            }
            CoreStatics.CheckModules(MyStatics.Configurazione.ConnectionString);

            CoreStatics.CheckEntita(MyStatics.Configurazione.ConnectionString);

            CoreStatics.CheckAzioni(MyStatics.Configurazione.ConnectionString);

            Application.Run(new frmMain());

        }

        static void DoSplash()
        {

            _frmSplash = new frmSplash();
            _frmSplash.ViewInit();
            _frmSplash.ShowDialog();

        }

        public static void CloseSplash()
        {

            try
            {
                if (_frmSplash != null)
                    _frmSplash.RequestClose();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

    }
}
