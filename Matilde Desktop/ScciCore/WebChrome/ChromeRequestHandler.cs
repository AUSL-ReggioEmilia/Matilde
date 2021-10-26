using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using UnicodeSrl.Framework.Security;

namespace UnicodeSrl.ScciCore.WebChrome
{

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct CREDUI_INFO
    {
        public int cbSize;
        public IntPtr hwndParent;
        public string pszMessageText;
        public string pszCaptionText;
        public IntPtr hbmBanner;
    }

    internal class ChromeRequestHandler : IRequestHandler
    {
        public ChromeRequestHandler()
        {
            this.HWndParentWindow = IntPtr.Zero;
        }

        public ChromeRequestHandler(IntPtr hWndParentWindow)
        {
            this.HWndParentWindow = hWndParentWindow;
        }

        IntPtr HWndParentWindow { get; set; }

        public bool GetAuthCredentials(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
        {

            NetworkCredential nc = AppSecurity.ShowCredentialDialog(this.HWndParentWindow, originUrl);

            if (nc == null) return false;
            else
            {
                callback.Continue(nc.UserName, nc.Password);
                return true;
            }

        }

        public IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            return null;
        }

        public bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
        {
            return false;
        }

        public bool OnCertificateError(IWebBrowser chromiumWebBrowser, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback)
        {
            return false;
        }

        public void OnDocumentAvailableInMainFrame(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
        }

        public bool OnOpenUrlFromTab(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
        {
            return false;
        }

        public void OnPluginCrashed(IWebBrowser chromiumWebBrowser, IBrowser browser, string pluginPath)
        {
        }

        public bool OnQuotaRequest(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, long newSize, IRequestCallback callback)
        {
            return false;
        }

        public void OnRenderProcessTerminated(IWebBrowser chromiumWebBrowser, IBrowser browser, CefTerminationStatus status)
        {
        }

        public void OnRenderViewReady(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
        }

        public bool OnSelectClientCertificate(IWebBrowser chromiumWebBrowser, IBrowser browser, bool isProxy, string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback)
        {
            return false;
        }
    }

    internal static class AppSecurity
    {

        internal const int CREDUIWIN_GENERIC = 0x1;
        internal const int CREDUIWIN_AUTHPACKAGE_ONLY = 0x10;

        internal static NetworkCredential ShowCredentialDialog(IntPtr hwndParent, string url)
        {
            bool save = false;
            int errorcode = 0;
            uint dialogReturn;
            uint authPackage = 0;
            IntPtr outCredBuffer;
            uint outCredSize;

            CREDUI_INFO credui = new CREDUI_INFO();
            credui.cbSize = Marshal.SizeOf(credui);
            credui.pszCaptionText = "Connessione all' area riservata";
            credui.pszMessageText = "Specifica le credenziali di accesso";
            credui.hwndParent = IntPtr.Zero;

            while (true)
            {
                dialogReturn = CredentialsAPI.CredUIPromptForWindowsCredentials(ref credui,
errorcode,
ref authPackage,
(IntPtr)0,
0,
out outCredBuffer,
out outCredSize,
ref save,
CREDUIWIN_GENERIC);

                if (dialogReturn != 0) break;
                var maxUserName = 100;
                var maxDomain = 100;
                var maxPassword = 100;
                var usernameBuf = new StringBuilder(maxUserName);
                var domainBuf = new StringBuilder(maxDomain);
                var passwordBuf = new StringBuilder(maxPassword);

                var packAuthRes = CredentialsAPI.CredUnPackAuthenticationBuffer(0, outCredBuffer, outCredSize,
                    usernameBuf, ref maxUserName,
                    domainBuf, ref maxDomain,
                    passwordBuf, ref maxPassword);

                var userName = usernameBuf.ToString();
                var domain = domainBuf.ToString();
                var password = passwordBuf.ToString();

                CredentialsAPI.CoTaskMemFree(outCredBuffer);

                NetworkCredential nc = new NetworkCredential
                {
                    UserName = userName,
                    Domain = domain,
                    Password = password,
                };

                return nc;

            }

            return null;
        }

    }

    internal static class CredentialsAPI
    {
        [DllImport("ole32.dll")]
        internal static extern void CoTaskMemFree(IntPtr ptr);

        [DllImport("credui.dll", CharSet = CharSet.Unicode)]
        internal static extern uint CredUIPromptForWindowsCredentials(ref
              CREDUI_INFO notUsedHere,
              int authError,
              ref uint authPackage,
              IntPtr InAuthBuffer,
              uint InAuthBufferSize,
              out IntPtr refOutAuthBuffer,
              out uint refOutAuthBufferSize,
              ref bool fSave,
              int flags);

        [DllImport("credui.dll", CharSet = CharSet.Auto)]
        internal static extern bool CredUnPackAuthenticationBuffer(int dwFlags, IntPtr pAuthBuffer, uint cbAuthBuffer,
            StringBuilder pszUserName, ref int pcchMaxUserName, StringBuilder pszDomainName, ref int pcchMaxDomainame, StringBuilder pszPassword, ref int pcchMaxPassword);

        internal const int CREDUIWIN_AUTHPACKAGE_ONLY = 0x10;

        internal static bool ShowDialog(out string authUser, out string errors)
        {
            CREDUI_INFO credui = new CREDUI_INFO();
            credui.cbSize = Marshal.SizeOf(credui);
            credui.pszCaptionText = "OlyENDO Security";
            credui.pszMessageText = "Inserire le Credenziali di validazione";
            uint authPackage = 0;
            IntPtr outCredBuffer;
            uint outCredSize;
            bool save = false;

            uint ret = CredUIPromptForWindowsCredentials(ref credui,
              0,
              ref authPackage,
              IntPtr.Zero,
              0,
              out outCredBuffer,
              out outCredSize,
              ref save,
              CREDUIWIN_AUTHPACKAGE_ONLY);

            authUser = null;
            errors = null;

            if (ret == 0)
            {
                StringBuilder usernameBuf = new StringBuilder(100);
                StringBuilder passwordBuf = new StringBuilder(100);

                int maxUserName = 100;
                int maxPassword = 100;
                int zero = 0;

                if (CredUnPackAuthenticationBuffer(0, outCredBuffer, outCredSize, usernameBuf, ref maxUserName, null, ref zero, passwordBuf, ref maxPassword))
                {
                    CoTaskMemFree(outCredBuffer);

                    authUser = usernameBuf.ToString();

                    string[] user = usernameBuf.ToString().Split('\\');

                    string domain = user[0];
                    string userName = user[1];

                    bool result = LogonHelper.CheckLogon(userName, domain, passwordBuf.ToString(), out errors);


                    return result;
                }

                return false;
            }
            else
                return false;

        }

    }

}
