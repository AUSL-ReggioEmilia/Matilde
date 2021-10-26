using DevExpress.XtraRichEdit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmPUDocViewer : Form, Interfacce.IViewFormPUView
    {

        #region DECLARE

        private string _DOCXFileFullPath = "";

        #endregion

        public frmPUDocViewer()
        {
            InitializeComponent();
        }

        #region PROPERTIES

        public string DOCXFileFullPath
        {
            get
            {
                return _DOCXFileFullPath;
            }
            set
            {
                _DOCXFileFullPath = value;
                if (_DOCXFileFullPath != null && _DOCXFileFullPath != string.Empty && _DOCXFileFullPath.Trim() != "" && System.IO.File.Exists(_DOCXFileFullPath))
                {
                    this.pdfViewer.LoadDocument(ScciCore.easyStatics.getPathDocumentDE(_DOCXFileFullPath));
                }
            }
        }

        #endregion

        #region INTERFACCIA

        public PUDataBindings ViewDataBindings
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public Enums.EnumDataNames ViewDataNamePU
        {
            get
            {
                return Enums.EnumDataNames.Nessuno;
            }
            set
            {
            }
        }

        public Image ViewImage
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public Enums.EnumModalityPopUp ViewModality
        {
            get
            {
                return Enums.EnumModalityPopUp.mpVisualizza;
            }
            set
            {
            }
        }

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

            this.ShowDialog();

        }

        #endregion

    }
}
