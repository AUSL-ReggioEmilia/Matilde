using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using UnicodeSrl.Scci;
using UnicodeSrl.ScciCore;

namespace UnicodeSrl.ScciCore
{
    public partial class frmSplash : Form
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
            this.Version.Text = System.String.Format(Version.Text, oAN.Version.Major, oAN.Version.Minor, oAN.Version.Build, oAN.Version.Revision);
                        this.Copyright.Text = "";

            Image oImage = UnicodeSrl.Scci.Statics.Database.GetConfigTableImage(Scci.Enums.EnumConfigTable.SplashEasy);
            if (oImage != null)
            {
                this.BackgroundImage = oImage;
            }

                        try
            {
                decimal ratio = Convert.ToDecimal(this.BackgroundImage.Size.Width) / Convert.ToDecimal(this.BackgroundImage.Size.Height + 2);
                int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;

                this.Width = screenWidth - 20;
                this.Height = Convert.ToInt32(Convert.ToDecimal(this.Width) / ratio);

            }
            catch (Exception)
            {
            }

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
