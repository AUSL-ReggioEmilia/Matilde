namespace UnicodeSrl.ScciManagement.UserControls
{
    partial class ucInfoCartella
    {
                                private System.ComponentModel.IContainer components = null;

                                        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

                                        private void InitializeComponent()
        {
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton("Zoom");
            this.utxtNumCartella = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.lblCodUA = new System.Windows.Forms.Label();
            this.lblNumCartella = new System.Windows.Forms.Label();
            this.utxtInfoCartella = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.lblCodUADes = new System.Windows.Forms.Label();
            this.utxtCodUA = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            ((System.ComponentModel.ISupportInitialize)(this.utxtNumCartella)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.utxtInfoCartella)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.utxtCodUA)).BeginInit();
            this.SuspendLayout();
                                                this.utxtNumCartella.Location = new System.Drawing.Point(106, 52);
            this.utxtNumCartella.MaxLength = 50;
            this.utxtNumCartella.Name = "utxtNumCartella";
            this.utxtNumCartella.Size = new System.Drawing.Size(142, 21);
            this.utxtNumCartella.TabIndex = 4;
            this.utxtNumCartella.ValueChanged += new System.EventHandler(this.GestValueChanged);
                                                this.lblCodUA.Location = new System.Drawing.Point(3, 3);
            this.lblCodUA.Name = "lblCodUA";
            this.lblCodUA.Size = new System.Drawing.Size(97, 22);
            this.lblCodUA.TabIndex = 0;
            this.lblCodUA.Text = "Unità Atomica";
            this.lblCodUA.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                                                this.lblNumCartella.Location = new System.Drawing.Point(3, 52);
            this.lblNumCartella.Name = "lblNumCartella";
            this.lblNumCartella.Size = new System.Drawing.Size(97, 22);
            this.lblNumCartella.TabIndex = 3;
            this.lblNumCartella.Text = "Numero Cartella";
            this.lblNumCartella.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                                                this.utxtInfoCartella.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.TextHAlignAsString = "Left";
            appearance1.TextVAlignAsString = "Top";
            this.utxtInfoCartella.Appearance = appearance1;
            this.utxtInfoCartella.AutoSize = false;
            this.utxtInfoCartella.Location = new System.Drawing.Point(254, 4);
            this.utxtInfoCartella.Multiline = true;
            this.utxtInfoCartella.Name = "utxtInfoCartella";
            this.utxtInfoCartella.ReadOnly = true;
            this.utxtInfoCartella.Scrollbars = System.Windows.Forms.ScrollBars.Both;
            this.utxtInfoCartella.Size = new System.Drawing.Size(193, 103);
            this.utxtInfoCartella.TabIndex = 5;
                                                this.lblCodUADes.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCodUADes.Location = new System.Drawing.Point(106, 28);
            this.lblCodUADes.Name = "lblCodUADes";
            this.lblCodUADes.Size = new System.Drawing.Size(142, 21);
            this.lblCodUADes.TabIndex = 2;
            this.lblCodUADes.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                                                editorButton1.Key = "Zoom";
            editorButton1.Text = "...";
            this.utxtCodUA.ButtonsRight.Add(editorButton1);
            this.utxtCodUA.Location = new System.Drawing.Point(106, 4);
            this.utxtCodUA.MaxLength = 20;
            this.utxtCodUA.Name = "utxtCodUA";
            this.utxtCodUA.Size = new System.Drawing.Size(142, 21);
            this.utxtCodUA.TabIndex = 1;
            this.utxtCodUA.ValueChanged += new System.EventHandler(this.GestValueChanged);
            this.utxtCodUA.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.utxtCodUA_EditorButtonClick);
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.lblCodUADes);
            this.Controls.Add(this.utxtCodUA);
            this.Controls.Add(this.utxtInfoCartella);
            this.Controls.Add(this.lblNumCartella);
            this.Controls.Add(this.lblCodUA);
            this.Controls.Add(this.utxtNumCartella);
            this.Name = "ucInfoCartella";
            this.Size = new System.Drawing.Size(450, 110);
            ((System.ComponentModel.ISupportInitialize)(this.utxtNumCartella)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.utxtInfoCartella)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.utxtCodUA)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraTextEditor utxtNumCartella;
        private System.Windows.Forms.Label lblCodUA;
        private System.Windows.Forms.Label lblNumCartella;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor utxtInfoCartella;
        private System.Windows.Forms.Label lblCodUADes;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor utxtCodUA;
    }
}
