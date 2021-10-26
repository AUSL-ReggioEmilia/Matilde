using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnicodeSrl.ScciResource;

namespace UnicodeSrl.ScciCore
{
    public partial class ucEasyPopUpFolder : UserControl
    {
        public ucEasyPopUpFolder()
        {
            InitializeComponent();
        }


        public void Init(string codentita = "")
        {

            this.uceCodEntita.Items.Clear();
            if (CoreStatics.CoreApplication.Trasferimento != null)
            {
                this.uceCodEntita.Items.Add("CAR", "Cartella");
            }

            this.uceCodEntita.Items.Add("PAZ", "Paziente");
            if (codentita != string.Empty)
            {
                this.uceCodEntita.Value = codentita;
                this.uceCodEntita.Enabled = false;
            }

        }

        #region Events

        private void tvFolder_AfterActivate(object sender, Infragistics.Win.UltraWinTree.NodeEventArgs e)
        {

            try
            {

                if (e.TreeNode.Tag.ToString() == CoreStatics.TV_ROOT)
                {
                    if (this.Tag.ToString() == "ADD")
                    {
                    }
                    else
                    {
                        this.uceCodEntita.Enabled = false;
                    }
                }
                else
                {
                    this.uceCodEntita.Enabled = false;
                    this.uceCodEntita.Value = e.TreeNode.Tag.ToString();
                }

            }
            catch (Exception)
            {

            }

        }

        #endregion

    }
}
