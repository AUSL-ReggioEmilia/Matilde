using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.Scci;
using UnicodeSrl.ScciResource;
using UnicodeSrl.ScciCore;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.ScciCore.Common;
using UnicodeSrl.Scci.PluginClient;
using UnicodeSrl.Scci.Model;

namespace UnicodeSrl.ScciCore
{
    public partial class frmPUMow : frmBaseModale, Interfacce.IViewFormlModal
    {

        #region Declare

        UnicodeSrl.ScciCore.Interfacce.IViewUserControlMiddle _ucEasyMow = null;
        bool _caricato = false;

        #endregion

        public frmPUMow()
        {
            InitializeComponent();
        }

        #region Interfaccia

        public void Carica()
        {

            try
            {

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_MOW_16);

                                this.CaricaCDSS();

                this.ShowDialog();

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "Carica", this.Text);
            }

        }

        #endregion

        #region Private

        private void CaricaPlugIn()
        {

            try
            {

                const string C_pluginname = @"PluginScciMow";

                string path = Application.StartupPath + @"\Plugins\" + C_pluginname + @"\" + C_pluginname + @".dll";

                this.ImpostaCursore(enum_app_cursors.WaitCursor);

                PluginCaller caller = new PluginCaller(path, C_pluginname, true,
                                                        SessioneRemota: CoreStatics.CoreApplication.Sessione.Computer.SessioneRemota,
                                                        IsOSServer: CoreStatics.CoreApplication.Sessione.Computer.IsOSServer);

                Dictionary<string, object> oDictionary = new Dictionary<string, object>();
                oDictionary.Add("StringaConnessione", Database.ConnectionString);
                oDictionary.Add("IDSac", CoreStatics.CoreApplication.Paziente.CodSAC);

                                if (CoreStatics.CoreApplication.Episodio != null)
                {
                    oDictionary.Add("IDEpisodio", CoreStatics.CoreApplication.Episodio.ID);
                    oDictionary.Add("NumeroNosologico", CoreStatics.CoreApplication.Episodio.NumeroEpisodio);
                    oDictionary.Add("NumeroListaAttesa", CoreStatics.CoreApplication.Episodio.NumeroListaAttesa);
                }

                oDictionary.Add("TipoAzienda", CoreStatics.getTipoAzienda().ToString());

                _ucEasyMow = (UnicodeSrl.ScciCore.Interfacce.IViewUserControlMiddle)caller.Esegui(oDictionary);

                ((Control)_ucEasyMow).BackColor = System.Drawing.Color.Transparent;
                ((Control)_ucEasyMow).Location = new System.Drawing.Point(0, 0);
                ((Control)_ucEasyMow).Name = "_ucEasyMow";
                ((Control)_ucEasyMow).Size = new System.Drawing.Size(689, 555);
                ((Control)_ucEasyMow).TabIndex = 1;

                this.Controls.Add(((Control)_ucEasyMow));

                ((Control)_ucEasyMow).BringToFront();
                ((Control)_ucEasyMow).Dock = System.Windows.Forms.DockStyle.Fill;

                caller.Dispose();


            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaPlugIn", this.Text);
            }
            finally
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }

        }

        private void CaricaCDSS()
        {

            try
            {

                T_CDSSPlugins oCDSSPlugins = new T_CDSSPlugins();
                oCDSSPlugins.Codice = @"CDSSMow";
                if (oCDSSPlugins.TrySelect())
                {

                    Plugin oPlugin = new Plugin(oCDSSPlugins.Codice, oCDSSPlugins.Descrizione, oCDSSPlugins.NomePlugin, oCDSSPlugins.Comando, "", "", 0, null);
                    Risposta oRispostaMenuEsegui = PluginClientStatics.PluginClientMenuEsegui(oPlugin, new object[1] { new object() });
                    if (oRispostaMenuEsegui.Successo == true)
                    {

                        _ucEasyMow = ((Interfacce.IViewUserControlMiddle)oRispostaMenuEsegui.Parameters[0]);
                        _ucEasyMow.Carica();

                        ((Control)_ucEasyMow).BackColor = System.Drawing.Color.Transparent;
                        ((Control)_ucEasyMow).Location = new System.Drawing.Point(0, 0);
                        ((Control)_ucEasyMow).Name = "_ucEasyMow";
                        ((Control)_ucEasyMow).Size = new System.Drawing.Size(689, 555);
                        ((Control)_ucEasyMow).TabIndex = 1;

                        this.Controls.Add(((Control)_ucEasyMow));

                        ((Control)_ucEasyMow).BringToFront();
                        ((Control)_ucEasyMow).Dock = System.Windows.Forms.DockStyle.Fill;

                    }
                    else
                    {
                        Exception rex = oRispostaMenuEsegui.ex;
                        CoreStatics.ExGest(ref rex, oRispostaMenuEsegui.Parameters[0].ToString(), "CaricaCDSS", this.Text, true, this.Name, "Errore", MessageBoxIcon.Warning, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);
                    }

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaCDSS", this.Text);
            }
            finally
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }

        }

        private void Disconnetti()
        {

            try
            {


                if (_ucEasyMow != null)
                {
                    Dictionary<string, object> oDictionary = new Dictionary<string, object>();
                    oDictionary.Add("Disconnetti", true);

                    ((UnicodeSrl.ScciCore.Interfacce.IViewUserControlMiddlePlugin)_ucEasyMow).EseguiComando(oDictionary);
                }

            }
            catch
            {
            }

        }

        #endregion

        #region Events Form

        private void frmPUMow_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            Disconnetti();
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void frmPUMow_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            Disconnetti();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void frmPUMow_Shown(object sender, EventArgs e)
        {

            try
            {

                if (_ucEasyMow != null)
                {
                    if (!_caricato)
                    {
                        _ucEasyMow.Carica();
                        _caricato = true;
                    }
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "frmPUMow_Shown", this.Name);
            }

        }

        #endregion

    }
}
