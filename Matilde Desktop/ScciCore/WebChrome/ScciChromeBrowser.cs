using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using CefSharp;
using System.IO;
using CefSharp.WinForms;

namespace UnicodeSrl.ScciCore.WebChrome
{
    public partial class ScciChromeBrowser : UserControl
    {
        public ScciChromeBrowser()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;

            bool designMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);
            if (designMode == false)
            {
                this.createChromium();
            }

        }

        public void NavigateTo(string url, bool openalways = false)
        {
            try
            {
                if (openalways)
                {
                    this.Chrome.DownloadHandler = new ChromeDownloadHandler2();
                }
                this.Chrome.Load(url);
            }
            catch (Exception ex)
            {
                throw new Exception(@"ScciChromeBrowser: " + ex.Message, ex);
            }
        }

        private ChromiumWebBrowser Chrome { get; set; }

        private void createChromium()
        {
            if (Cef.IsInitialized == false)
            {
                var settings = new CefSettings()
                {
                    BrowserSubprocessPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                    Environment.Is64BitProcess ? "x64" : "x86",
                    "CefSharp.BrowserSubprocess.exe")
                };
                settings.CachePath = UnicodeSrl.Scci.Statics.FileStatics.GetSCCITempPath();
                settings.LogFile = Path.Combine(UnicodeSrl.Scci.Statics.FileStatics.GetSCCITempPath(), "debugCef.log");
                Cef.Initialize(settings);
            }

            BrowserSettings browserSettings = new BrowserSettings();
            browserSettings.Javascript = CefState.Enabled;
            browserSettings.Plugins = CefState.Enabled;

            this.Chrome = new ChromiumWebBrowser();
            this.Chrome.BrowserSettings = browserSettings;
            this.Chrome.MenuHandler = new ChromeMenuHandler();
            this.Chrome.DownloadHandler = new ChromeDownloadHandler();
            this.Chrome.RequestHandler = new ChromeRequestHandler();
            this.Chrome.Dock = DockStyle.Fill;
            this.panelBrowser.Controls.Add(this.Chrome);
            this.Chrome.BringToFront();
        }

        private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("CefSharp"))
            {
                string assemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
                string architectureSpecificPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                    Environment.Is64BitProcess ? "x64" : "x86",
                    assemblyName);

                return File.Exists(architectureSpecificPath)
                    ? Assembly.LoadFile(architectureSpecificPath)
                    : null;
            }

            return null;
        }
    }
}
