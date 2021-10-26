using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UnicodeSrl.ScciCore.Forms
{
    public partial class frmDocxStampa : Form
    {
        public frmDocxStampa()
        {
            InitializeComponent();
        }

                                private PrintDialog PrintDialog { get; set; }

                                                private void dcv_DocLoaded(object sender, EventArgs args)
        {  
            try
            {
                this.PrintDialog.Document.Print();

                this.Close();

            }
            catch (Exception ex) 
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

                                        public void LoadDocX(string path, PrintDialog pd)
        {
                        
            this.PrintDialog = pd;
            this.dcv.LoadDocument(easyStatics.getPathDocumentDE(path));
            this.Size = new Size(0, 0);

            this.Show();
        }

    }
}
