using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmPUPdfViewer : Form, Interfacce.IViewFormPUView
    {

        #region DECLARE

        private const string PDF_LICENCE_KEY = @"PDFVW4WIN-YA3MW-NF22B-DOI1O-UQFAU-SL8Y5";
        private string _PDFFileFullPath = "";

        #endregion

        public frmPUPdfViewer()
        {
            InitializeComponent();
        }

        #region PROPERTIES

        public string PDFFileFullPath
        {
            get
            {
                return _PDFFileFullPath;
            }
            set
            {
                _PDFFileFullPath = value;
                this.ucEasyO2SPDFView.Carica();
                this.ucEasyO2SPDFView.PDFFileFullPath = _PDFFileFullPath;
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
