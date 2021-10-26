using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Model;
using UnicodeSrl.Scci.PluginClient;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.ScciCore.Common;

namespace UnicodeSrl.ScciCore
{
    public partial class frmPUFarmaci : frmBaseModale, Interfacce.IViewFormlModal
    {

        #region Declare

        UnicodeSrl.ScciCore.Interfacce.IViewUserControlMiddle _ucFarmaci = null;
        bool _caricato = false;

        #endregion

        public frmPUFarmaci()
        {
            InitializeComponent();
        }

        #region Interfaccia

        public void Carica()
        {

            try
            {

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_FARMACI_16);

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

                const string C_pluginname = @"PluginScciFarmaci";

                string path = Application.StartupPath + @"\Plugins\" + C_pluginname + @"\" + C_pluginname + @".dll";

                this.ImpostaCursore(enum_app_cursors.WaitCursor);

                PluginCaller caller = new PluginCaller(path, C_pluginname, true,
                                                        SessioneRemota: CoreStatics.CoreApplication.Sessione.Computer.SessioneRemota,
                                                        IsOSServer: CoreStatics.CoreApplication.Sessione.Computer.IsOSServer);

                Dictionary<string, object> oDictionary = new Dictionary<string, object>();
                oDictionary.Add("StringaConnessione", Database.ConnectionString);
                oDictionary.Add("Nosologico", CoreStatics.CoreApplication.Episodio.NumeroEpisodio);
                oDictionary.Add("TipoAzienda", CoreStatics.getTipoAzienda().ToString());

                _ucFarmaci = (UnicodeSrl.ScciCore.Interfacce.IViewUserControlMiddle)caller.Esegui(oDictionary);

                ((Control)_ucFarmaci).BackColor = System.Drawing.Color.Transparent;
                ((Control)_ucFarmaci).Location = new System.Drawing.Point(0, 0);
                ((Control)_ucFarmaci).Name = "_ucFarmaci";
                ((Control)_ucFarmaci).Size = new System.Drawing.Size(689, 555);
                ((Control)_ucFarmaci).TabIndex = 1;

                this.Controls.Add(((Control)_ucFarmaci));

                ((Control)_ucFarmaci).BringToFront();
                ((Control)_ucFarmaci).Dock = System.Windows.Forms.DockStyle.Fill;

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
                oCDSSPlugins.Codice = @"CDSSFarmaci";
                if (oCDSSPlugins.TrySelect())
                {

                    Plugin oPlugin = new Plugin(oCDSSPlugins.Codice, oCDSSPlugins.Descrizione, oCDSSPlugins.NomePlugin, oCDSSPlugins.Comando, "", "", 0, null);
                    Risposta oRispostaMenuEsegui = PluginClientStatics.PluginClientMenuEsegui(oPlugin, new object[1] { new object() });
                    if (oRispostaMenuEsegui.Successo == true)
                    {

                        _ucFarmaci = ((Interfacce.IViewUserControlMiddle)oRispostaMenuEsegui.Parameters[0]);
                        _ucFarmaci.Carica();

                        ((Control)_ucFarmaci).BackColor = System.Drawing.Color.Transparent;
                        ((Control)_ucFarmaci).Location = new System.Drawing.Point(0, 0);
                        ((Control)_ucFarmaci).Name = "_ucFarmaci";
                        ((Control)_ucFarmaci).Size = new System.Drawing.Size(689, 555);
                        ((Control)_ucFarmaci).TabIndex = 1;

                        this.Controls.Add(((Control)_ucFarmaci));

                        ((Control)_ucFarmaci).BringToFront();
                        ((Control)_ucFarmaci).Dock = System.Windows.Forms.DockStyle.Fill;

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


                if (_ucFarmaci != null)
                {
                    Dictionary<string, object> oDictionary = new Dictionary<string, object>();
                    oDictionary.Add("Disconnetti", true);

                    ((UnicodeSrl.ScciCore.Interfacce.IViewUserControlMiddlePlugin)_ucFarmaci).EseguiComando(oDictionary);
                }

            }
            catch
            {
            }

        }

        #endregion

        #region Events Form

        private void frmPUFarmaci_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            Disconnetti();
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void frmPUFarmaci_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            Disconnetti();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void frmPUFarmaci_Shown(object sender, EventArgs e)
        {

            try
            {

                if (_ucFarmaci != null)
                {
                    if (!_caricato)
                    {
                        _ucFarmaci.Carica();
                        _caricato = true;
                    }
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "frmPUFarmaci_Shown", this.Name);
            }

        }

        #endregion

    }
}
