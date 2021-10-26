using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.ScciResource;
using UDL;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmConfigCE : Form, Interfacce.IViewFormBase
    {
        public frmConfigCE()
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
                this.UltraTabControl.Tabs["tab1"].Text = value;
            }
        }

        public void ViewInit()
        {

            this.SuspendLayout();

            MyStatics.SetUltraToolbarsManager(this.UltraToolbarsManager);
            this.PicView.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_CONFIGCE, Enums.EnumImageSize.isz256));

            this.ucPictureSelectLogoFabbricatore.ViewShowSaveImage = false;
            this.ucPictureSelectLogoFabbricatore.ViewCheckSquareImage = false;
            this.ucPictureSelectLogoFabbricatore.ViewInit();

            this.ucPictureSelectLogoProdotto.ViewShowSaveImage = false;
            this.ucPictureSelectLogoProdotto.ViewCheckSquareImage = false;
            this.ucPictureSelectLogoProdotto.ViewInit();

            this.ucPictureSelectLogoManuale.ViewShowSaveImage = false;
            this.ucPictureSelectLogoManuale.ViewCheckSquareImage = false;
            this.ucPictureSelectLogoManuale.ViewInit();

            this.LoadConfig();

            this.ResumeLayout();
        }

        #endregion

        #region Subroutine

        private void LoadConfig()
        {

            this.ucPictureSelectLogoFabbricatore.ViewImage = Scci.Statics.Database.GetConfigCETableImage(Scci.Enums.EnumConfigCETable.LogoFabbricatore);
            this.heDescrizioneFabbricatore.DocumentText = Scci.Statics.Database.GetConfigCETable(Scci.Enums.EnumConfigCETable.DescrizioneFabbricatore);

            this.ucPictureSelectLogoProdotto.ViewImage = Scci.Statics.Database.GetConfigCETableImage(Scci.Enums.EnumConfigCETable.LogoProdotto);
            this.heDescrizioneProdotto.DocumentText = Scci.Statics.Database.GetConfigCETable(Scci.Enums.EnumConfigCETable.DescrizioneProdotto);

            this.ucPictureSelectLogoManuale.ViewImage = Scci.Statics.Database.GetConfigCETableImage(Scci.Enums.EnumConfigCETable.LogoManuale);
            this.heDescrizioneManuale.DocumentText = Scci.Statics.Database.GetConfigCETable(Scci.Enums.EnumConfigCETable.DescrizioneManuale);

        }

        private void SaveConfig()
        {

            string sSql = "Select * From T_ConfigCE";
            DataSet DsLogPrima = DataBase.GetDataSet(sSql);

            Scci.Statics.Database.SetConfigCETable(Scci.Enums.EnumConfigCETable.LogoFabbricatore, "", this.ucPictureSelectLogoFabbricatore.ViewImage);
            Scci.Statics.Database.SetConfigCETable(Scci.Enums.EnumConfigCETable.DescrizioneFabbricatore, this.heDescrizioneFabbricatore.DocumentText);

            Scci.Statics.Database.SetConfigCETable(Scci.Enums.EnumConfigCETable.LogoProdotto, "", this.ucPictureSelectLogoProdotto.ViewImage);
            Scci.Statics.Database.SetConfigCETable(Scci.Enums.EnumConfigCETable.DescrizioneProdotto, this.heDescrizioneProdotto.DocumentText);

            Scci.Statics.Database.SetConfigCETable(Scci.Enums.EnumConfigCETable.LogoManuale, "", this.ucPictureSelectLogoManuale.ViewImage);
            Scci.Statics.Database.SetConfigCETable(Scci.Enums.EnumConfigCETable.DescrizioneManuale, this.heDescrizioneManuale.DocumentText);

            DataSet DsLogDopo = DataBase.GetDataSet(sSql);

            MyStatics.LogManager(Enums.EnumModalityPopUp.mpModifica, Enums.EnumEntitaLog.T_ConfigCE, DsLogPrima, DsLogDopo);

        }

        #endregion

        #region Events

        private void ubAnnulla_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void ubConferma_Click(object sender, EventArgs e)
        {
            this.SaveConfig();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        #endregion

        private void frmConfigCE_Shown(object sender, EventArgs e)
        {
            this.ucPictureSelectLogoFabbricatore.ViewFitImage();
            this.ucPictureSelectLogoProdotto.ViewFitImage();
            this.ucPictureSelectLogoManuale.ViewFitImage();
        }
    }
}
