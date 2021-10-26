using CefSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnicodeSrl.ScciCore
{
    internal class ChromeDownloadHandler : IDownloadHandler
    {

        public ChromeDownloadHandler()
        {

        }

        public void OnBeforeDownload(IWebBrowser webBrowser, IBrowser browser, DownloadItem item, IBeforeDownloadCallback callback)
        {
            if (!callback.IsDisposed)
            {
                using (callback)
                {
                    SaveFileDialog sfd = new SaveFileDialog()
                    {
                        Title = "Salva File",
                        FileName = item.SuggestedFileName
                    };

                    DialogResult r = sfd.ShowDialog();

                    if (r == DialogResult.OK)
                    {
                        string path = sfd.FileName;
                        callback.Continue(path, false);
                    }
                    else
                        item.IsCancelled = true;

                    sfd.Dispose();

                }
            }
        }

        public void OnDownloadUpdated(IWebBrowser webBrowser, IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
        {
            if ((downloadItem != null) && (downloadItem.IsComplete))
            {
                FileInfo fileInfo = new FileInfo(downloadItem.FullPath);
                Process.Start(fileInfo.DirectoryName);
            }

        }
    }

    internal class ChromeDownloadHandler2 : IDownloadHandler
    {

        public ChromeDownloadHandler2()
        {

        }

        public void OnBeforeDownload(IWebBrowser webBrowser, IBrowser browser, DownloadItem item, IBeforeDownloadCallback callback)
        {
            if (!callback.IsDisposed)
            {
                using (callback)
                {
                    callback.Continue(Path.Combine(@"C:\Temp", item.SuggestedFileName), showDialog: false);
                }
            }
        }

        public void OnDownloadUpdated(IWebBrowser webBrowser, IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
        {
            if ((downloadItem != null) && (downloadItem.IsComplete))
            {
                Process.Start(downloadItem.FullPath);
            }
        }

    }

}
