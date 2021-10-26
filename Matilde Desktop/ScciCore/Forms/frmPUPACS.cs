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
using UnicodeSrl.Scci.Model;
using UnicodeSrl.Scci.PluginClient;

namespace UnicodeSrl.ScciCore
{
    public partial class frmPUPACS : frmBaseModale, Interfacce.IViewFormlModal
    {
        UnicodeSrl.ScciCore.Interfacce.IViewUserControlMiddle _ucEasyPACS1 = null;
        bool _caricato = false;

        public frmPUPACS()
        {
            InitializeComponent();
        }

        #region INTERFACCIA

        public void Carica()
        {
            try
            {
                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_EVIDENZACLINICA_16);
                _caricato = false;

                CaricaCDSS();

                this.ShowDialog();
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "Carica", this.Text);
            }
        }

        #endregion

        #region PRIVATE

        private void CaricaPACS()
        {
            try
            {
                const string C_pluginname = @"PluginScciLAB";

                string path = Application.StartupPath + @"\Plugins\" + C_pluginname + @"\" + C_pluginname + @".dll";

                if (!System.IO.File.Exists(path))
                    easyStatics.EasyMessageBox(@"Impossibile recuperare il plugin:" + Environment.NewLine + C_pluginname + @".dll !", "Stampa Plugin", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                else
                {
                    this.ImpostaCursore(enum_app_cursors.WaitCursor);

                    try
                    {
                        PluginCaller caller = new PluginCaller(path, C_pluginname, true,
                                                                SessioneRemota: CoreStatics.CoreApplication.Sessione.Computer.SessioneRemota,
                                                                IsOSServer: CoreStatics.CoreApplication.Sessione.Computer.IsOSServer);

                        Dictionary<string, object> oDictionary = new Dictionary<string, object>();

                        oDictionary.Add("StringaConnessione", Database.ConnectionString);
                        oDictionary.Add("AccessNumber", CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.AccessNumber);
                        oDictionary.Add("CodTipoEvidenzaClinica", CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.CodTipoEvidenzaClinica);

                        oDictionary.Add("ReturnPACS", true);
                        _ucEasyPACS1 = (UnicodeSrl.ScciCore.Interfacce.IViewUserControlMiddle)caller.Esegui(oDictionary);

                        ((Control)_ucEasyPACS1).BackColor = System.Drawing.Color.Transparent;
                        ((Control)_ucEasyPACS1).Location = new System.Drawing.Point(0, 0);
                        ((Control)_ucEasyPACS1).Name = "_ucEasyPACS1";
                        ((Control)_ucEasyPACS1).Size = new System.Drawing.Size(689, 555);
                        ((Control)_ucEasyPACS1).TabIndex = 1;

                        this.Controls.Add(((Control)_ucEasyPACS1));

                        ((Control)_ucEasyPACS1).BringToFront();
                        ((Control)_ucEasyPACS1).Dock = System.Windows.Forms.DockStyle.Fill;

                        caller.Dispose();
                    }
                    catch (Exception ex)
                    {
                        UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);

                        easyStatics.EasyMessageBox("Errore LAB" + Environment.NewLine + ex.Message,
                            "EVIDENZA CLINICA", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaPACS", this.Text);
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
                oCDSSPlugins.Codice = @"CDSSLAB-PACS";
                if (oCDSSPlugins.TrySelect())
                {

                    Plugin oPlugin = new Plugin(oCDSSPlugins.Codice, oCDSSPlugins.Descrizione, oCDSSPlugins.NomePlugin, oCDSSPlugins.Comando, "", "", 0, null);
                    Risposta oRispostaMenuEsegui = PluginClientStatics.PluginClientMenuEsegui(oPlugin, new object[1] { new object() });
                    if (oRispostaMenuEsegui.Successo == true)
                    {

                        _ucEasyPACS1 = ((Interfacce.IViewUserControlMiddle)oRispostaMenuEsegui.Parameters[0]);



                        ((Control)_ucEasyPACS1).BackColor = System.Drawing.Color.Transparent;
                        ((Control)_ucEasyPACS1).Location = new System.Drawing.Point(0, 0);
                        ((Control)_ucEasyPACS1).Name = "_ucEasyPACS1";
                        ((Control)_ucEasyPACS1).Size = new System.Drawing.Size(689, 555);
                        ((Control)_ucEasyPACS1).TabIndex = 1;

                        this.Controls.Add(((Control)_ucEasyPACS1));

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


                if (_ucEasyPACS1 != null)
                {
                    Dictionary<string, object> oDictionary = new Dictionary<string, object>();
                    oDictionary.Add("Disconnetti", true);

                    ((UnicodeSrl.ScciCore.Interfacce.IViewUserControlMiddlePlugin)_ucEasyPACS1).EseguiComando(oDictionary);
                }

            }
            catch
            {
            }
        }

        #endregion

        #region EVENTI

        private void frmPUPACS_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            Disconnetti();
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void frmPUPACS_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            Disconnetti();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void frmPUPACS_Shown(object sender, EventArgs e)
        {

            try
            {

                if (_ucEasyPACS1 != null)
                {
                    if (!_caricato)
                    {
                        ((Control)_ucEasyPACS1).BringToFront();
                        ((Control)_ucEasyPACS1).Dock = System.Windows.Forms.DockStyle.Fill;
                        _ucEasyPACS1.Carica();
                        _caricato = true;
                    }
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "frmPUPACS_Shown", this.Name);
            }
        }

        #endregion

    }
}
