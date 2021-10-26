using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnicodeSrl.ScciCore
{
    public partial class ucHelp : UserControl, Interfacce.IViewUserControlBase
    {

        #region Declare

        public event EventHandler ChiudiClick;

        #endregion

        public ucHelp()
        {
            InitializeComponent();
        }

        #region Interface

        public void ViewInit()
        {

            this.pbLogoFabbricatore.Image = Scci.Statics.Database.GetConfigCETableImage(Scci.Enums.EnumConfigCETable.LogoFabbricatore);
            this.wbDescrizioneFabbricatore.DocumentText = Scci.Statics.Database.GetConfigCETable(Scci.Enums.EnumConfigCETable.DescrizioneFabbricatore);

            this.pbLogoProdotto.Image = Scci.Statics.Database.GetConfigCETableImage(Scci.Enums.EnumConfigCETable.LogoProdotto);
            this.wbDescrizioneProdotto.DocumentText = Scci.Statics.Database.GetConfigCETable(Scci.Enums.EnumConfigCETable.DescrizioneProdotto);

            this.pbLogoManuale.Image = Scci.Statics.Database.GetConfigCETableImage(Scci.Enums.EnumConfigCETable.LogoManuale);
            this.wbDescrizioneManuale.DocumentText = Scci.Statics.Database.GetConfigCETable(Scci.Enums.EnumConfigCETable.DescrizioneManuale);

        }

        #endregion

        #region Events

        private void ubChiudi_Click(object sender, EventArgs e)
        {
            if (ChiudiClick != null) { ChiudiClick(sender, e); }
        }

        #endregion

    }
}
