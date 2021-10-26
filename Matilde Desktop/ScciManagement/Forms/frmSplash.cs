using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using UnicodeSrl.ScciResource;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmSplash : Form, Interfacce.IViewFormBase
    {

        SynchronizationContext m_syncObj = SynchronizationContext.Current;

        public frmSplash()
        {
            InitializeComponent();
        }

        #region Interface

        public Icon ViewIcon
        {
            get
            {
                return this.Icon;
            }
            set
            {
                this.Icon = value;
            }
        }

        public string ViewText
        {
            get
            {
                return this.Text;
            }
            set
            {
                this.Text = value;
            }
        }

        public void ViewInit()
        {

            this.SuspendLayout();

            System.Reflection.AssemblyName oAN = System.Reflection.Assembly.GetEntryAssembly().GetName();
            Image oImage = UnicodeSrl.Scci.Statics.Database.GetConfigTableImage(Scci.Enums.EnumConfigTable.SplashManagement);
            if (oImage == null)
            {
                oImage = Risorse.GetImageFromResource(Risorse.GC_SCCIMANAGEMENTSPLASH);
            }
            this.BackgroundImage = oImage;

            this.Version.Text = System.String.Format(Version.Text, oAN.Version.Major, oAN.Version.Minor, oAN.Version.Build, oAN.Version.Revision);
            this.Copyright.Text = Application.CompanyName;

            this.ResumeLayout();

        }

        #endregion

        private void frmSplash_Load(object sender, EventArgs e)
        {
            m_syncObj = SynchronizationContext.Current;
        }

        public void RequestClose()
        {
            m_syncObj.Post(SyncRequestClose, null);
        }

        private void SyncRequestClose(object state)
        {
            try
            {
                this.Hide();
                this.Close();
                m_syncObj = null;

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

    }
}
