namespace UnicodeSrl.ScciManagement
{
    partial class frmCrypt
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

        #region Windows Form Designer generated code

                                        private void InitializeComponent()
        {
            this.txtSource = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmdCrypt = new System.Windows.Forms.Button();
            this.cmdDecrypt = new System.Windows.Forms.Button();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
                                                this.txtSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSource.Location = new System.Drawing.Point(12, 24);
            this.txtSource.Name = "txtSource";
            this.txtSource.Size = new System.Drawing.Size(662, 21);
            this.txtSource.TabIndex = 0;
                                                this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(156, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Stringa da Criptare/Decriptare:";
                                                this.cmdCrypt.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.cmdCrypt.Location = new System.Drawing.Point(180, 51);
            this.cmdCrypt.Name = "cmdCrypt";
            this.cmdCrypt.Size = new System.Drawing.Size(157, 23);
            this.cmdCrypt.TabIndex = 2;
            this.cmdCrypt.Text = "Cripta";
            this.cmdCrypt.UseVisualStyleBackColor = true;
            this.cmdCrypt.Click += new System.EventHandler(this.cmdCrypt_Click);
                                                this.cmdDecrypt.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.cmdDecrypt.Location = new System.Drawing.Point(360, 51);
            this.cmdDecrypt.Name = "cmdDecrypt";
            this.cmdDecrypt.Size = new System.Drawing.Size(157, 23);
            this.cmdDecrypt.TabIndex = 3;
            this.cmdDecrypt.Text = "DEcripta";
            this.cmdDecrypt.UseVisualStyleBackColor = true;
            this.cmdDecrypt.Click += new System.EventHandler(this.cmdDecrypt_Click);
                                                this.txtResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtResult.Location = new System.Drawing.Point(12, 80);
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtResult.Size = new System.Drawing.Size(662, 127);
            this.txtResult.TabIndex = 4;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 219);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.cmdDecrypt);
            this.Controls.Add(this.cmdCrypt);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSource);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frmCrypt";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Crypt/Decrypt";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSource;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cmdCrypt;
        private System.Windows.Forms.Button cmdDecrypt;
        private System.Windows.Forms.TextBox txtResult;
    }
}