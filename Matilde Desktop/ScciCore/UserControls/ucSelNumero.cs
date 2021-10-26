using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UnicodeSrl.ScciCore
{
    public partial class ucSelNumero : UserControl, Interfacce.IEasyResizableText
    {
        public ucSelNumero()
        {
            InitializeComponent();
        }

        #region declare

        public event ChangeEventHandler ValueChanged;

        #endregion

        #region properties

        public easyStatics.easyRelativeDimensions TextFontRelativeDimension
        {
            get
            {
                return this.ucEasyNumericEditor.TextFontRelativeDimension;
            }
            set
            {
                this.ucEasyNumericEditor.TextFontRelativeDimension = value;
            }
        }

        public object Value
        {
            get { return this.ucEasyNumericEditor.Value; }
            set { this.ucEasyNumericEditor.Value = value; }
        }

        public char PromptChar
        {
            get { return this.ucEasyNumericEditor.PromptChar; }
            set { this.ucEasyNumericEditor.PromptChar = value; }
        }

        public object MaxValue
        {
            get { return this.ucEasyNumericEditor.MaxValue; }
            set { this.ucEasyNumericEditor.MaxValue = value; }
        }

        public object MinValue
        {
            get { return this.ucEasyNumericEditor.MinValue; }
            set { this.ucEasyNumericEditor.MinValue = value; }
        }

        public string MaskInput
        {
            get { return this.ucEasyNumericEditor.MaskInput; }
            set { this.ucEasyNumericEditor.MaskInput = value; }
        }

        #endregion

        #region private events

        private void ucEasyNumericEditor_ValueChanged(object sender, EventArgs e)
        {
            if (ValueChanged != null) { ValueChanged(sender, e); }
        }

        private void ucEasyButtonUp_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.ucEasyNumericEditor.MaxValue == null || (int)this.ucEasyNumericEditor.MaxValue >= ((int)this.ucEasyNumericEditor.Value + 1))
                    this.ucEasyNumericEditor.Value = (int)this.ucEasyNumericEditor.Value + 1;
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyButtonUp_Click", this.Name.ToString());
            }
        }

        private void ucEasyButtonDown_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.ucEasyNumericEditor.MinValue == null || (int)this.ucEasyNumericEditor.MinValue <= ((int)this.ucEasyNumericEditor.Value - 1))
                    this.ucEasyNumericEditor.Value = (int)this.ucEasyNumericEditor.Value - 1;
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyButtonUp_Click", this.Name.ToString());
            }
        }

        #endregion

    }
}
