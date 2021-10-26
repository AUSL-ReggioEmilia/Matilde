using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.ScciCore
{
    public partial class frmDettaglioNews : frmBaseModale, Interfacce.IViewFormlModal
    {

        public enum ShowCommands : int
        {
            SW_HIDE = 0,
            SW_SHOWNORMAL = 1,
            SW_NORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_SHOWMAXIMIZED = 3,
            SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9,
            SW_SHOWDEFAULT = 10,
            SW_FORCEMINIMIZE = 11,
            SW_MAX = 11
        }

        [System.Runtime.InteropServices.DllImport("shell32.dll")]
        static extern IntPtr ShellExecute(
            IntPtr hwnd,
            string lpOperation,
            string lpFile,
            string lpParameters,
            string lpDirectory,
            ShowCommands nShowCmd);

        public frmDettaglioNews()
        {
            InitializeComponent();
        }

        #region INTERFACCIA

        public void Carica()
        {
            try
            {
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_NEWS_16);
                this.PictureBox.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_NEWS_256);

                this.lblDataOra.Text = "";
                this.lblTitoloNews.Text = "";
                this.ucRichTextBox.ViewInit();
                this.ucRichTextBox.ViewRtf = "";

                if (CoreStatics.CoreApplication.News.NotiziaSelezionata != null)
                {
                    this.lblDataOra.Text = CoreStatics.CoreApplication.News.NotiziaSelezionata.DataOra.ToString(@"dd/MM/yyyy HH:mm");
                    this.lblTitoloNews.Text = CoreStatics.CoreApplication.News.NotiziaSelezionata.Titolo;

                    this.ucRichTextBox.ViewRtf = CoreStatics.CoreApplication.News.NotiziaSelezionata.TestoRTF;
                }

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.ShowDialog();

                CoreStatics.CoreApplication.News.NotiziaSelezionata = null;
            }
            catch (Exception)
            {

            }
        }

        #endregion

        #region Subroutine

        private void InsLogNews()
        {

            try
            {

                                                                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodNews", CoreStatics.CoreApplication.News.NotiziaSelezionata.CodNews);
                op.Parametro.Add("CodUtenteVisione", CoreStatics.CoreApplication.Sessione.Utente.Codice);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                Database.GetDataTableStoredProc("MSP_InsMovNewsLog", spcoll);

            }
            catch (Exception)
            {

            }

        }

        #endregion

        #region EVENTI

        private void ucRichTextBox_RtfLinkClicked(object sender, LinkClickedEventArgs e)
        {

            try
            {
                this.frmDettaglioNews_PulsanteAvantiClick(this, new PulsanteBottomClickEventArgs(Scci.Enums.EnumPulsanteBottom.Avanti));
                ShellExecute(this.Handle, "open", e.LinkText, null, 
                            System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), 
                            ShowCommands.SW_SHOWMAXIMIZED);
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void frmDettaglioNews_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
                        if (CoreStatics.CoreApplication.News.NotiziaSelezionata != null && CoreStatics.CoreApplication.News.NotiziaSelezionata.InsLog == true)
            {
                this.InsLogNews();
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void frmDettaglioNews_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
                        this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        #endregion

    }
}
