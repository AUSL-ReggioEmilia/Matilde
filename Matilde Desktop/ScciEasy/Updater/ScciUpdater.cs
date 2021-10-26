using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UnicodeSrl.Framework.UpdateServer;
using UnicodeSrl.Framework.Updater;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.ScciCore;
using System.Windows.Forms;
using System.Drawing;
using UnicodeSrl.ScciResource;

namespace UnicodeSrl.ScciEasy.Updater
{
    public static class ScciUpdater
    {
        private static string K_APPID = @"SCCI";
        private static Form frmSplashUpdater = null;

        public static bool IsUpdaterUser()
        {
            UpdaterConfig cfg = ScciUpdater.readScciConfig();
            string winUser = UnicodeSrl.Framework.Utility.Windows.CurrentUser();
            string updaterUser = $"{cfg.Domain}\\{cfg.Username}";

            return (winUser.ToUpper() == updaterUser.ToUpper());
        }

        public static bool CheckAndUpdate()
        {
            try
            {
                if (CoreStatics.CoreApplication.Sessione.Computer.IsOSServer) return true;

                UpdaterConfig cfg = ScciUpdater.readScciConfig();

                if (string.IsNullOrEmpty(cfg.ServerAddress) == true) return true;

                AppUpdater au = new AppUpdater(K_APPID, cfg);

                bool ok = au.UpdateAvailable();

                if (ok)
                {
                    ScciUpdater.InfoUpdateShow();
                    UpdateInfo ui = au.GetLastUpdateInfo();

                    if (ui != null)
                    {
                        bool bObb = ui.ForceUpdate;
                        bool updated = au.Update(ui);
                        ScciUpdater.InfoUpdateClose();
                        return updated;
                    };
                    ScciUpdater.InfoUpdateClose();
                }

                return true;

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                return false;
            }
        }


        private static UpdaterConfig readScciConfig()
        {
            UpdaterConfig cfg = new UpdaterConfig();

            DataTable dt = Scci.Statics.Database.GetConfigUpdater();

            string svcSecDomain = "";
            string svcSecUsername = "";
            string svcSecPassword = "";

            string secDomain = "";
            string secUsername = "";
            string secPassword = "";

            Scci.Encryption ocrypt = new Encryption(Scci.Statics.EncryptionStatics.GC_DECRYPTKEY, Scci.Statics.EncryptionStatics.GC_INITVECTOR);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int id = Convert.ToInt32(dt.Rows[i]["ID"]);
                EnumConfigTable ecfg = (EnumConfigTable)id;

                switch (ecfg)
                {
                    case EnumConfigTable.WSUpdaterUseUserFolder:
                        cfg.UserDocsFolder = Convert.ToBoolean(Convert.ToInt32(dt.Rows[i]["Valore"]));
                        break;

                    case EnumConfigTable.WSUpdaterAddress:
                        cfg.ServerAddress = (string)dt.Rows[i]["Valore"];
                        break;
                    case EnumConfigTable.WSUpdaterDomain:
                        secDomain = (string)dt.Rows[i]["Valore"];
                        break;
                    case EnumConfigTable.WSUpdaterUserName:
                        secUsername = (string)dt.Rows[i]["Valore"];
                        break;
                    case EnumConfigTable.WSUpdaterPassword:
                        secPassword = ocrypt.DecryptString((string)dt.Rows[i]["Valore"]);
                        break;
                    case EnumConfigTable.WSUpdaterShowUI:
                        cfg.ShowUI = Convert.ToBoolean(Convert.ToInt32(dt.Rows[i]["Valore"]));
                        break;
                    case EnumConfigTable.WSUpdaterSvcSecDomain:
                        svcSecDomain = (string)dt.Rows[i]["Valore"];
                        break;
                    case EnumConfigTable.WSUpdaterSvcSecUserName:
                        svcSecUsername = (string)dt.Rows[i]["Valore"];
                        break;
                    case EnumConfigTable.WSUpdaterSvcSecPassword:
                        svcSecPassword = ocrypt.DecryptString((string)dt.Rows[i]["Valore"]);
                        break;
                    case EnumConfigTable.WSUpdaterAlwaysCheckFiles:
                        cfg.AlwaysCheckFiles = Convert.ToBoolean(Convert.ToInt32(dt.Rows[i]["Valore"]));
                        break;
                }

            }

            cfg.SetUser(secDomain, secUsername, secPassword);
            cfg.SetWebServiceUser(svcSecDomain, svcSecUsername, svcSecPassword);


            return cfg;
        }

        internal static void InfoUpdateShow()
        {

            frmSplashUpdater = new Form();
            frmSplashUpdater.BackColor = Color.FromArgb(206, 222, 240);
            frmSplashUpdater.ControlBox = false;
            frmSplashUpdater.FormBorderStyle = FormBorderStyle.FixedSingle;
            frmSplashUpdater.ShowIcon = false;
            frmSplashUpdater.ShowInTaskbar = false;
            frmSplashUpdater.Size = new System.Drawing.Size(480, 180);
            frmSplashUpdater.StartPosition = FormStartPosition.CenterScreen;
            frmSplashUpdater.TopLevel = true;
            frmSplashUpdater.TopMost = true;
            frmSplashUpdater.Text = string.Format("Matilde (Versione : {0}) (net 4.7.2) - UPDATER", Application.ProductVersion);
            frmSplashUpdater.WindowState = FormWindowState.Normal;

            PictureBox pbUpdater = new PictureBox();
            pbUpdater.Image = Risorse.GetImageFromResource(Risorse.GC_UPDATER_128);
            pbUpdater.Location = new Point(20, 20);
            pbUpdater.Size = new System.Drawing.Size(100, 100);
            pbUpdater.SizeMode = PictureBoxSizeMode.Zoom;
            frmSplashUpdater.Controls.Add(pbUpdater);

            Label lblInfo1 = new Label();
            lblInfo1.Font = new Font("Calibri", 24, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            lblInfo1.Text = "Aggiornamento in corso...";
            lblInfo1.Location = new Point(126, 20);
            lblInfo1.Size = new System.Drawing.Size(330, 40);
            frmSplashUpdater.Controls.Add(lblInfo1);

            Label lblInfo2 = new Label();
            lblInfo2.Font = new Font("Calibri", 16, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            lblInfo2.Text = "L'operazione potrebbe richiedere un po' di tempo, non interrompere l'aggiornamento.";
            lblInfo2.Location = new Point(129, 58);
            lblInfo2.Size = new System.Drawing.Size(330, 60);
            frmSplashUpdater.Controls.Add(lblInfo2);

            frmSplashUpdater.Show();
            frmSplashUpdater.Refresh();

        }

        internal static void InfoUpdateClose()
        {

            if (frmSplashUpdater != null)
            {
                frmSplashUpdater.Close();
                frmSplashUpdater.Dispose();
                frmSplashUpdater = null;
            }

        }

    }
}
