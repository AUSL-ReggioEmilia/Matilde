using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Infragistics.Win;
using UnicodeSrl.ScciResource;
using System.Diagnostics;
using System.Security;
using System.Threading;
using UnicodeSrl.Scci.Statics;
using System.DirectoryServices.AccountManagement;
using System.IO;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;

using System.Runtime.InteropServices;
using static UnicodeSrl.ScciCore.Interfacce;
using DevExpress.XtraRichEdit;

namespace UnicodeSrl.ScciCore
{
    public static class easyStatics
    {

        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        private static Thread _thSplash = null;

        private static float _fontcoeff = -1F;
        private static float _fontcoeffdaconfig = 1F;

        private static frmSplash m_sp = null;

        public static float CoefficienteFontDaConfig
        {
            get
            {
                if (_fontcoeffdaconfig <= 0)
                    return 1F;
                else
                    return _fontcoeffdaconfig;
            }
            set
            {
                if (value <= 0)
                    _fontcoeffdaconfig = 1F;
                else
                    _fontcoeffdaconfig = value;
            }
        }

        #region ENUMS

        public enum easyRelativeDimensions
        {
            _undefined = 0,
            XXXSmall = 111,
            XXSmall = 11,
            XSmall = 1,
            Small = 2,
            Medium = 3,
            Large = 4,
            XLarge = 5,
            XXLarge = 15,
            XXXLarge = 155,

            XXXXLarge = 1555,
            HUGE = 2000,
            XHUGE = 2050,
            XXHUGE = 2100,
            XXXHUGE = 2150,
            XXXXHUGE = 2200
        }

        public enum easyShortcutPosition
        {
            top_right = 0,
            top_left = 1,
            bottom_right = 2,
            bottom_left = 3
        }

        public enum DocumentBookmarkVisibility
        {
            auto = 0,
            hidden = 1,
            visible = 2

        }

        #endregion

        #region PUBLIC METHODS

        public static void setTreeViewCheckBoxesStyle()
        {
            try
            {
                UIElementDrawParams.CheckBoxGlyphInfo = UIElementDrawParams.Office2013CheckBoxGlyphInfo;

            }
            catch
            {
            }
        }

        public static void setTreeViewLargeCheckBoxesStyle()
        {
            try
            {


                CheckBoxImageGlyphInfo checkboxtvstyle = new CheckBoxImageGlyphInfo(new Size(48, 48),
                                                                                    ScciCore.Properties.Resources.unChecked,
                                                                                    ScciCore.Properties.Resources.Checked,
                                                                                    ScciCore.Properties.Resources.unChecked,
                                                                                    ScciCore.Properties.Resources.unChecked,
                                                                                    ScciCore.Properties.Resources.Checked,
                                                                                    ScciCore.Properties.Resources.unChecked,
                                                                                    ScciCore.Properties.Resources.unChecked,
                                                                                    ScciCore.Properties.Resources.Checked,
                                                                                    ScciCore.Properties.Resources.unChecked,
                                                                                    ScciCore.Properties.Resources.unChecked,
                                                                                    ScciCore.Properties.Resources.Checked,
                                                                                    ScciCore.Properties.Resources.unChecked,
                                                                                    ScciCore.Properties.Resources.unChecked,
                                                                                    ScciCore.Properties.Resources.Checked,
                                                                                    ScciCore.Properties.Resources.unChecked,
                                                                                    "ucEasyTreeViewCheckBoxGlyphInfo");

                UIElementDrawParams.CheckBoxGlyphInfo = checkboxtvstyle;

            }
            catch
            {
            }
        }

        public static float getFontSizeInPointsFromRelativeDimension(easyRelativeDimensions relativeDimension)
        {
            float fRet = 12;
            float coeff = coefficientefont();

            switch (relativeDimension)
            {
                case easyRelativeDimensions._undefined:
                    break;

                case easyRelativeDimensions.XXXSmall:
                    fRet = 3F * coeff;
                    break;

                case easyRelativeDimensions.XXSmall:
                    fRet = 5.5F * coeff;
                    break;

                case easyRelativeDimensions.XSmall:
                    fRet = 7F * coeff;
                    break;

                case easyRelativeDimensions.Small:
                    fRet = 9F * coeff;
                    if (fRet < 12) fRet = (float)Math.Ceiling((double)fRet);
                    break;

                case easyRelativeDimensions.Medium:
                    fRet = 13F * coeff;
                    break;

                case easyRelativeDimensions.Large:
                    fRet = 16F * coeff;
                    break;

                case easyRelativeDimensions.XLarge:
                    fRet = 24F * coeff;
                    break;

                case easyRelativeDimensions.XXLarge:
                    fRet = 28F * coeff;
                    break;

                case easyRelativeDimensions.XXXLarge:
                    fRet = 30F * coeff;
                    break;

                case easyRelativeDimensions.XXXXLarge:
                    fRet = 48F * coeff;
                    break;

                case easyRelativeDimensions.HUGE:
                    fRet = 70F * coeff;
                    break;

                case easyRelativeDimensions.XHUGE:
                    fRet = 110F * coeff;
                    break;

                case easyRelativeDimensions.XXHUGE:
                    fRet = 160F * coeff;
                    break;

                case easyRelativeDimensions.XXXHUGE:
                    fRet = 240F * coeff;
                    break;

                case easyRelativeDimensions.XXXXHUGE:
                    fRet = 320F * coeff;
                    break;

                default:
                    break;
            }

            if (CoefficienteFontDaConfig > 0 && CoefficienteFontDaConfig != 1F) fRet *= CoefficienteFontDaConfig;

            return fRet;
        }

        public static float aspectRatio()
        {

            return (float)(Screen.PrimaryScreen.WorkingArea.Width) / (float)(Screen.PrimaryScreen.WorkingArea.Height);
        }


        public static void checkShortcutKeyDown_MultiKey(KeyEventArgs args, Control.ControlCollection controls)
        {

            foreach (Control ctl in controls)
            {
                if (
                        (ctl is IEasyShortcutMultiKey) &&
                        (ctl.Visible == true)
                    )
                {
                    IEasyShortcutMultiKey ptrControl = (IEasyShortcutMultiKey)ctl;

                    if (ptrControl.ShortcutKeys.Exists(k => k == args.KeyCode))
                    {
                        ptrControl.ActionKeyDown(args.KeyCode, args.Modifiers);
                    }

                }

                if ((ctl.Visible == true) && (ctl.Controls != null))
                    checkShortcutKeyDown_MultiKey(args, ctl.Controls);

            }

        }

        public static bool checkShortcutKeyDown(Keys keyCode, Control.ControlCollection controls, bool donotskiphidden)
        {
            try
            {
                return checkShortcutKeyDown(keyCode, controls, donotskiphidden, Keys.None);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool checkShortcutKeyDown(Keys keyCode, Control.ControlCollection controls, bool donotskiphidden, Keys modifiers)
        {
            bool bFound = false;
            try
            {
                if (skipKeyOnTextBox(keyCode, modifiers, controls))
                    bFound = true;
                else
                {
                    for (int iCtrl = 0; iCtrl < controls.Count; iCtrl++)
                    {
                        Control oCtrl = controls[iCtrl];

                        if (oCtrl is Interfacce.IEasyShortcut)
                        {

                            if (confrontaKeyDown_ControlShortcut(keyCode, modifiers, (oCtrl as Interfacce.IEasyShortcut).ShortcutKey))
                            {
                                bFound = (oCtrl.Visible || donotskiphidden);

                                if ((oCtrl.Visible || donotskiphidden) && oCtrl.Enabled)
                                {
                                    (oCtrl as Interfacce.IEasyShortcut).PerformActionShortcut();

                                    iCtrl = controls.Count;

                                }

                            }

                        }

                        if (!bFound && (oCtrl.Visible || donotskiphidden) && oCtrl.Enabled && oCtrl.Controls.Count > 0 && iCtrl < controls.Count) bFound = checkShortcutKeyDown(keyCode, oCtrl.Controls, donotskiphidden, modifiers);

                    }
                }

                return bFound;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool confrontaKeyDown_ControlShortcut(Keys pressedKeyCode, Keys pressedModifiers, Keys shortcutKey)
        {
            bool bOK = false;

            Keys cleanShortcutKey = shortcutKey;
            bool shortcutAlt = false;
            bool shortcutCtrl = false;
            bool shortcutShift = false;
            Keys shortcutModifiers = Keys.None;
            extractModifiersFromKey(shortcutKey, out cleanShortcutKey, out shortcutAlt, out shortcutCtrl, out shortcutShift);
            if (shortcutAlt) shortcutModifiers |= Keys.Alt;
            if (shortcutCtrl) shortcutModifiers |= Keys.Control;
            if (shortcutShift) shortcutModifiers |= Keys.Shift;

            if (pressedKeyCode == cleanShortcutKey && pressedModifiers == shortcutModifiers)
            {
                bOK = true;
            }

            return bOK;
        }

        public static void maximizeForm(ref Form frm)
        {
            maximizeForm(ref frm, System.Windows.Forms.FormBorderStyle.FixedSingle);
        }
        public static void maximizeForm(ref Form frm, System.Windows.Forms.FormBorderStyle formborderstyle)
        {
            try
            {

                if (CoreStatics.CoreApplication.Sessione.Sala == true)
                {
                    frm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

                    frm.MaximizeBox = false;
                    frm.MinimizeBox = false;

                    frm.WindowState = FormWindowState.Maximized;
                }
                else
                {
                    frm.FormBorderStyle = formborderstyle;

                    frm.MaximizeBox = (formborderstyle == FormBorderStyle.Sizable);
                    frm.MinimizeBox = (formborderstyle != FormBorderStyle.None);

                }

                frm.Location = new Point(CoreStatics.CoreApplication.Schermo.WorkingArea.X, CoreStatics.CoreApplication.Schermo.WorkingArea.Y);
                frm.Size = CoreStatics.CoreApplication.Schermo.WorkingArea.Size;

                frm.AutoScaleMode = AutoScaleMode.Inherit;

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        public static object EasyFormZoom(string titolo, string caption, object customparamaters, bool multiselezione)
        {

            object ret = null;

            try
            {

                using (frmZoom ofrmZoom = new frmZoom())
                {

                    ofrmZoom.ucTopModale.Refresh();
                    ofrmZoom.ucBottomModale.ubIndietro.Visible = true;
                    ofrmZoom.ucBottomModale.ubIndietro.Text = "ANNULLA";
                    ofrmZoom.ucBottomModale.ubAvanti.Visible = true;
                    ofrmZoom.ucBottomModale.ubAvanti.Text = "CONFERMA";
                    ofrmZoom.ucBottomModale.Refresh();

                    Form frm = ofrmZoom;
                    easyStatics.maximizeForm(ref frm);

                    ofrmZoom.Text = titolo;
                    ofrmZoom.Icon = ScciResource.Risorse.GetIconFromResource(caption);
                    ofrmZoom.PictureBox.Image = ScciResource.Risorse.GetImageFromResource(caption);
                    ofrmZoom.CustomParamaters = customparamaters;
                    ofrmZoom.MultiSelezione = multiselezione;
                    ofrmZoom.Carica();

                    ret = ofrmZoom.CustomParamatersOutput;

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            return ret;

        }

        public static DialogResult EasyMessageBox(string messaggio, string caption)
        {
            DialogResult ret = DialogResult.OK;
            try
            {

                frmMessageBox frmMsgBox = new frmMessageBox();

                Form frm = frmMsgBox;
                easyStatics.maximizeForm(ref frm);

                ret = frmMsgBox.CaricaMessageBox(messaggio, caption);

                frmMsgBox.Dispose();
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            return ret;
        }

        public static DialogResult EasyMessageBox(string messaggio, string caption, MessageBoxButtons bottoni, MessageBoxIcon icona, bool NascondiTitolo)
        {
            DialogResult ret = DialogResult.Cancel;
            try
            {

                frmMessageBox frmMsgBox = new frmMessageBox();
                frmMsgBox.Icon = Risorse.GetIconFromResource(Risorse.GC_SCCIEASY);

                Form frm = frmMsgBox;
                easyStatics.maximizeForm(ref frm);

                ret = frmMsgBox.CaricaMessageBox(messaggio, caption, bottoni, icona, NascondiTitolo);

                frmMsgBox.Dispose();
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            return ret;
        }

        public static DialogResult EasyMessageBox(string messaggio, string caption, MessageBoxButtons bottoni, MessageBoxIcon icona)
        {
            return EasyMessageBox(messaggio, caption, bottoni, icona, false);
        }

        public static DialogResult EasyInputBox(string messaggio, string caption, out string testoInserito)
        {
            testoInserito = "";
            DialogResult ret = DialogResult.Cancel;
            try
            {

                frmInputBox frmInput = new frmInputBox();
                frmInput.Icon = Risorse.GetIconFromResource(Risorse.GC_SCCIEASY);

                Form frm = frmInput;
                easyStatics.maximizeForm(ref frm);

                ret = frmInput.CaricaInputBox(messaggio, caption);

                if (ret == DialogResult.OK) testoInserito = frmInput.TestoInserito;

                frmInput.Dispose();
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            return ret;
        }
        public static DialogResult EasyInputBox(string messaggio, string caption, int maxLength, out string testoInserito)
        {
            testoInserito = "";
            DialogResult ret = DialogResult.Cancel;
            try
            {

                frmInputBox frmInput = new frmInputBox();
                frmInput.Icon = Risorse.GetIconFromResource(Risorse.GC_SCCIEASY);

                Form frm = frmInput;
                easyStatics.maximizeForm(ref frm);

                ret = frmInput.CaricaInputBox(messaggio, caption, "", false, maxLength);

                if (ret == DialogResult.OK) testoInserito = frmInput.TestoInserito;

                frmInput.Dispose();
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            return ret;
        }
        public static DialogResult EasyInputBox(string messaggio, string caption, string testoIniziale, int maxLength, out string testoInserito)
        {
            testoInserito = "";
            DialogResult ret = DialogResult.Cancel;
            try
            {

                frmInputBox frmInput = new frmInputBox();
                frmInput.Icon = Risorse.GetIconFromResource(Risorse.GC_SCCIEASY);

                Form frm = frmInput;
                easyStatics.maximizeForm(ref frm);

                ret = frmInput.CaricaInputBox(messaggio, caption, testoIniziale, false, maxLength);

                if (ret == DialogResult.OK) testoInserito = frmInput.TestoInserito;

                frmInput.Dispose();
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            return ret;
        }
        public static DialogResult EasyInputBox(string messaggio, string caption, string testoIniziale, bool multiline, int maxLength, MessageBoxIcon icona, bool NascondiTitolo, out string testoInserito)
        {
            testoInserito = "";
            DialogResult ret = DialogResult.Cancel;
            try
            {

                frmInputBox frmInput = new frmInputBox();
                frmInput.Icon = Risorse.GetIconFromResource(Risorse.GC_SCCIEASY);

                Form frm = frmInput;
                easyStatics.maximizeForm(ref frm);

                ret = frmInput.CaricaInputBox(messaggio, caption, testoIniziale, multiline, maxLength, icona, NascondiTitolo);

                if (ret == DialogResult.OK) testoInserito = frmInput.TestoInserito;

                frmInput.Dispose();
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            return ret;
        }

        public static DialogResult EasyErrorMessageBox(string messaggio, string caption)
        {
            DialogResult ret = DialogResult.OK;
            try
            {

                frmMessageBox frmMsgBox = new frmMessageBox();

                Form frm = frmMsgBox;
                easyStatics.maximizeForm(ref frm);

                ret = frmMsgBox.CaricaErrorMessageBox(messaggio, caption);

                frmMsgBox.Dispose();
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            return ret;
        }
        public static DialogResult EasyErrorMessageBox(string messaggio, string caption, MessageBoxButtons bottoni, MessageBoxIcon icona)
        {
            DialogResult ret = DialogResult.Cancel;
            try
            {

                frmMessageBox frmMsgBox = new frmMessageBox();
                frmMsgBox.Icon = Risorse.GetIconFromResource(Risorse.GC_SCCIEASY);

                Form frm = frmMsgBox;
                easyStatics.maximizeForm(ref frm);

                ret = frmMsgBox.CaricaErrorMessageBox(messaggio, caption, bottoni, icona);

                frmMsgBox.Dispose();
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            return ret;
        }
        public static DialogResult EasyErrorMessageBox(string messaggio, string errore, string caption, MessageBoxButtons bottoni, MessageBoxIcon icona)
        {
            DialogResult ret = DialogResult.Cancel;
            try
            {

                frmMessageBox frmMsgBox = new frmMessageBox();
                frmMsgBox.Icon = Risorse.GetIconFromResource(Risorse.GC_SCCIEASY);

                Form frm = frmMsgBox;
                easyStatics.maximizeForm(ref frm);

                ret = frmMsgBox.CaricaErrorMessageBox(messaggio, errore, caption, bottoni, icona);

                frmMsgBox.Dispose();
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            return ret;
        }

        public static DialogResult EasyMessageBoxInfo(string messaggio,
                                                      string titolo,
                                                      string formcaption,
                                                      MessageBoxButtons bottoni,
                                                      MessageBoxIcon icona,
                                                      string captionAvanti = null,
                                                      string captionIndietro = null,
                                                      easyStatics.easyRelativeDimensions messaggioFontRelativeDimension = easyStatics.easyRelativeDimensions.Medium,
                                                      easyStatics.easyRelativeDimensions titoloFontRelativeDimension = easyStatics.easyRelativeDimensions.XLarge,
                                                      bool bottomHomeHidden = false,
                                                      bool skipSelectAll = false)
        {
            DialogResult ret = DialogResult.Cancel;
            try
            {

                frmMessageBoxInfo frmMsgBox = new frmMessageBoxInfo();
                frmMsgBox.Icon = Risorse.GetIconFromResource(Risorse.GC_SCCIEASY);

                Form frm = frmMsgBox;
                easyStatics.maximizeForm(ref frm);

                ret = frmMsgBox.CaricaInfoMessageBox(messaggio, titolo, formcaption, bottoni, icona, captionAvanti, captionIndietro, messaggioFontRelativeDimension, titoloFontRelativeDimension, bottomHomeHidden, skipSelectAll);

                frmMsgBox.Dispose();
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            return ret;
        }

        public static bool RunAs(string userName, string domain, string pwd, string appToExec, string appArgs = "")
        {
            try
            {
                string fileExec = appToExec;

                Process process = new Process();

                process.StartInfo.FileName = fileExec;
                process.StartInfo.Arguments = appArgs;
                process.StartInfo.LoadUserProfile = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;

                if ((userName != "") && (pwd != ""))
                {
                    process.StartInfo.Domain = domain;
                    process.StartInfo.UserName = userName;
                    process.StartInfo.Verb = @"runas";
                    process.StartInfo.Password = getSecurePwd(pwd);
                }

                process.Start();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static System.Security.SecureString getSecurePwd(string configPwd)
        {
            SecureString spwd = new SecureString();

            char[] cpwd = configPwd.ToCharArray();

            for (int i = 0; i < cpwd.Length; i++)
                spwd.AppendChar(cpwd[i]);

            return spwd;

        }

        public static bool ValidaLogin(string nomeutentesenzadominio, string password, string dominio, out string nomecompleto, out string messaggioerrore)
        {
            bool loginvalidato = false;
            nomecompleto = "";
            messaggioerrore = "";
            try
            {
                using (PrincipalContext principalContext = new PrincipalContext(ContextType.Domain, dominio))
                {
                    if (principalContext.ValidateCredentials(nomeutentesenzadominio, password))
                    {
                        UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(principalContext, nomeutentesenzadominio);

                        loginvalidato = (userPrincipal.Enabled == true);
                        nomecompleto = nomeutentesenzadominio;
                        try
                        {
                            System.DirectoryServices.DirectoryEntry ADEntry = new System.DirectoryServices.DirectoryEntry("WinNT://" + dominio + "/" + nomeutentesenzadominio);
                            nomecompleto = ADEntry.Properties["fullName"].Value.ToString();
                            ADEntry.Dispose();
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            catch (PrincipalServerDownException pex)
            {
                loginvalidato = false;
                nomecompleto = "";
                messaggioerrore = pex.Message;
            }
            catch (Exception ex)
            {
                loginvalidato = false;
                nomecompleto = "";
                messaggioerrore = ex.Message;
            }

            return loginvalidato;
        }

        public static bool ShellExecute(string fullfilepath, string appArgs)
        {
            return ShellExecute(fullfilepath, appArgs, false, string.Empty, false);
        }
        public static bool ShellExecute(string fullfilepath, string appArgs, bool forzausoshell)
        {
            return ShellExecute(fullfilepath, appArgs, forzausoshell, string.Empty, false);
        }
        public static bool ShellExecute(string fullfilepath, string appArgs, bool forzausoshell, string codreport, bool abilitaVisto)
        {
            try
            {
                if (!forzausoshell
                    && System.IO.Path.GetExtension(fullfilepath).Replace(@".", "").ToUpper() == "PDF"
                    && UnicodeSrl.Scci.Statics.Database.GetConfigTable(Scci.Enums.EnumConfigTable.ApriPDFtramiteshell) != "1")
                {
                    frmPDFViewer frmpdf = new frmPDFViewer();

                    Form frm = frmpdf;
                    easyStatics.maximizeForm(ref frm);

                    if (abilitaVisto)
                        frmpdf.AbilitaVisto(Keys.V, Risorse.GetImageFromResource(Risorse.GC_FIRMA_256));

                    frmpdf.pdfFullPath = fullfilepath;
                    frmpdf.Carica();

                    if (codreport != string.Empty && frmpdf.Stampa == true)
                    {
                        bool bret = false;
                        bret = DBUtils.storicizzaReport(codreport, fullfilepath, false);
                    }
                    frmpdf.Dispose();
                    return true;
                }
                else
                {

                    Process process = new Process();

                    process.StartInfo.FileName = fullfilepath;
                    process.StartInfo.Arguments = appArgs;
                    process.StartInfo.CreateNoWindow = false;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;

                    process.Start();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static void StartEasySplash()
        {
            _thSplash = new Thread(new ThreadStart(DoSplash));
            _thSplash.Start();

            Thread.Sleep(1000);
        }

        public static void CloseEasySplash()
        {
            try
            {
                if (m_sp != null)
                    m_sp.RequestClose();

            }
            catch
            {
            }
        }

        public static void MergePDFFiles(List<string> sourceFiles, string destinationFile, bool addblankpage)
        {
            try
            {
                int f = 0;
                PdfReader reader = new PdfReader(sourceFiles[f]);
                int n = reader.NumberOfPages;

                Document document = new Document(reader.GetPageSizeWithRotation(1));
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(destinationFile, FileMode.Create));
                document.Open();
                PdfContentByte cb = writer.DirectContent;
                PdfImportedPage page;
                int rotation;
                while (f < sourceFiles.Count)
                {
                    int i = 0;
                    while (i < n)
                    {
                        i++;
                        document.SetPageSize(reader.GetPageSizeWithRotation(i));
                        document.NewPage();
                        page = writer.GetImportedPage(reader, i);
                        rotation = reader.GetPageRotation(i);
                        if (rotation == 90 || rotation == 270)
                        {
                            cb.AddTemplate(page, 0, -1f, 1f, 0, 0, reader.GetPageSizeWithRotation(i).Height);
                        }
                        else
                        {
                            cb.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
                        }
                    }
                    if (addblankpage && f < sourceFiles.Count - 1)
                    {
                        if (i > 0 && (i == 1 || i % 2 != 0))
                        {
                            document.NewPage();
                            document.Add(new Paragraph(" "));
                        }
                    }

                    f++;

                    if (f < sourceFiles.Count)
                    {
                        reader = new PdfReader(sourceFiles[f]);
                        n = reader.NumberOfPages;
                    }
                }
                try
                {
                    document.Close();
                }
                catch
                {
                }
                try
                {
                    if (reader != null)
                    {
                        reader.Close();
                        reader.Dispose();
                    }
                }
                catch
                {
                }
                try
                {
                    if (writer != null)
                    {
                        writer.Close();
                        writer.Dispose();
                    }
                }
                catch
                {
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        #endregion

        #region INTERNAL

        internal static void DoSplash()
        {
            m_sp = new frmSplash();
            m_sp.ViewInit();
            m_sp.ShowDialog();
        }

        internal static bool isKeyForTextBox(Keys keyCode)
        {
            return isKeyForTextBox(keyCode, Keys.None);
        }
        internal static bool isKeyForTextBox(Keys keyCode, Keys modifiers)
        {
            bool bRet = false;

            if ((modifiers & Keys.Alt) == Keys.Alt)
                bRet = false;
            else
            {
                switch (keyCode)
                {
                    case Keys.Escape:
                    case Keys.F1:
                    case Keys.F10:
                    case Keys.F11:
                    case Keys.F12:
                    case Keys.F13:
                    case Keys.F14:
                    case Keys.F15:
                    case Keys.F16:
                    case Keys.F17:
                    case Keys.F18:
                    case Keys.F19:
                    case Keys.F2:
                    case Keys.F20:
                    case Keys.F21:
                    case Keys.F22:
                    case Keys.F23:
                    case Keys.F24:
                    case Keys.F3:
                    case Keys.F4:
                    case Keys.F5:
                    case Keys.F6:
                    case Keys.F7:
                    case Keys.F8:
                    case Keys.F9:
                    case Keys.PageDown:
                    case Keys.PageUp:
                    case Keys.Print:
                    case Keys.PrintScreen:
                        bRet = false;
                        break;
                    default:
                        bRet = true;
                        break;
                }
            }

            return bRet;
        }
        internal static bool isKeyForDateTimeEditor(Keys keyCode, Keys modifiers)
        {
            bool bRet = false;

            if ((modifiers & Keys.Alt) == Keys.Alt)
                bRet = false;
            else
            {
                switch (keyCode)
                {
                    case Keys.D0:
                    case Keys.D1:
                    case Keys.D2:
                    case Keys.D3:
                    case Keys.D4:
                    case Keys.D5:
                    case Keys.D6:
                    case Keys.D7:
                    case Keys.D8:
                    case Keys.D9:
                    case Keys.PageDown:
                    case Keys.PageUp:
                    case Keys.Left:
                    case Keys.Right:
                    case Keys.OemMinus:
                    case Keys.Divide:
                        bRet = true;
                        break;
                    default:
                        bRet = false;
                        break;
                }
            }

            return bRet;
        }

        internal static int ScreenWidth
        {
            get
            {
                return Screen.PrimaryScreen.WorkingArea.Width;
            }
        }

        internal static int ScreenHeight
        {
            get
            {
                return Screen.PrimaryScreen.WorkingArea.Height;
            }
        }

        internal static string getTextFromKey(Keys keyCode, bool returnAfterModifiers = false)
        {
            string sRet = "";

            Keys cleanKey = keyCode;
            bool alt = false;
            bool ctrl = false;
            bool shift = false;

            extractModifiersFromKey(keyCode, out cleanKey, out alt, out ctrl, out shift);

            switch (cleanKey)
            {
                case Keys.A:
                    sRet = "A";
                    break;
                case Keys.Add:
                    sRet = "+";
                    break;
                case Keys.B:
                    sRet = "B";
                    break;
                case Keys.Back:
                    sRet = "Back";
                    break;
                case Keys.C:
                    sRet = "C";
                    break;
                case Keys.Cancel:
                    sRet = "Canc";
                    break;
                case Keys.Control:
                case Keys.ControlKey:
                    sRet = "CTRL";
                    break;
                case Keys.D:
                    sRet = "D";
                    break;
                case Keys.D0:
                    sRet = "0";
                    break;
                case Keys.D1:
                    sRet = "1";
                    break;
                case Keys.D2:
                    sRet = "2";
                    break;
                case Keys.D3:
                    sRet = "3";
                    break;
                case Keys.D4:
                    sRet = "4";
                    break;
                case Keys.D5:
                    sRet = "5";
                    break;
                case Keys.D6:
                    sRet = "6";
                    break;
                case Keys.D7:
                    sRet = "7";
                    break;
                case Keys.D8:
                    sRet = "8";
                    break;
                case Keys.D9:
                    sRet = "9";
                    break;
                case Keys.Delete:
                    sRet = "Del";
                    break;
                case Keys.Divide:
                    sRet = @"/";
                    break;
                case Keys.Down:
                    sRet = "Giù";
                    break;
                case Keys.E:
                    sRet = "E";
                    break;
                case Keys.Enter:
                    sRet = "Invio";
                    break;
                case Keys.Escape:
                    sRet = "Esc";
                    break;
                case Keys.F:
                    sRet = "F";
                    break;
                case Keys.F1:
                    sRet = "F1";
                    break;
                case Keys.F10:
                    sRet = "F10";
                    break;
                case Keys.F11:
                    sRet = "F11";
                    break;
                case Keys.F12:
                    sRet = "F12";
                    break;
                case Keys.F2:
                    sRet = "F2";
                    break;
                case Keys.F3:
                    sRet = "F3";
                    break;
                case Keys.F4:
                    sRet = "F4";
                    break;
                case Keys.F5:
                    sRet = "F5";
                    break;
                case Keys.F6:
                    sRet = "F6";
                    break;
                case Keys.F7:
                    sRet = "F7";
                    break;
                case Keys.F8:
                    sRet = "F8";
                    break;
                case Keys.F9:
                    sRet = "F9";
                    break;
                case Keys.G:
                    sRet = "G";
                    break;
                case Keys.H:
                    sRet = "H";
                    break;
                case Keys.I:
                    sRet = "I";
                    break;
                case Keys.Insert:
                    sRet = "Ins";
                    break;
                case Keys.J:
                    sRet = "J";
                    break;
                case Keys.K:
                    sRet = "K";
                    break;
                case Keys.L:
                    sRet = "L";
                    break;
                case Keys.Left:
                    sRet = "Sx";
                    break;
                case Keys.M:
                    sRet = "M";
                    break;
                case Keys.Menu:
                    sRet = "Alt";
                    break;
                case Keys.Multiply:
                    sRet = "*";
                    break;
                case Keys.N:
                    sRet = "N";
                    break;
                case Keys.NumPad0:
                    sRet = "0";
                    break;
                case Keys.NumPad1:
                    sRet = "1";
                    break;
                case Keys.NumPad2:
                    sRet = "2";
                    break;
                case Keys.NumPad3:
                    sRet = "3";
                    break;
                case Keys.NumPad4:
                    sRet = "4";
                    break;
                case Keys.NumPad5:
                    sRet = "5";
                    break;
                case Keys.NumPad6:
                    sRet = "6";
                    break;
                case Keys.NumPad7:
                    sRet = "7";
                    break;
                case Keys.NumPad8:
                    sRet = "8";
                    break;
                case Keys.NumPad9:
                    sRet = "9";
                    break;
                case Keys.O:
                    sRet = "O";
                    break;
                case Keys.P:
                    sRet = "P";
                    break;
                case Keys.PageDown:
                    sRet = "PgDn";
                    break;
                case Keys.PageUp:
                    sRet = "PgUp";
                    break;
                case Keys.Pause:
                    sRet = "Pausa";
                    break;
                case Keys.Print:
                    sRet = "Stamp";
                    break;
                case Keys.Q:
                    sRet = "Q";
                    break;
                case Keys.R:
                    sRet = "R";
                    break;
                case Keys.Right:
                    sRet = "Dx";
                    break;
                case Keys.S:
                    sRet = "S";
                    break;
                case Keys.ShiftKey:
                    sRet = "Shift";
                    break;
                case Keys.Space:
                    sRet = "Spazio";
                    break;
                case Keys.Subtract:
                    sRet = "-";
                    break;
                case Keys.T:
                    sRet = "T";
                    break;
                case Keys.Tab:
                    sRet = "Tab";
                    break;
                case Keys.U:
                    sRet = "U";
                    break;
                case Keys.Up:
                    sRet = "Up";
                    break;
                case Keys.V:
                    sRet = "V";
                    break;
                case Keys.W:
                    sRet = "W";
                    break;
                case Keys.X:
                    sRet = "X";
                    break;
                case Keys.Y:
                    sRet = "Y";
                    break;
                case Keys.Z:
                    sRet = "Z";
                    break;
                default:
                    break;
            }

            string sModifiers = "";
            if (shift) sModifiers = "Shift+" + sModifiers;
            if (alt) sModifiers = "Alt+" + sModifiers;
            if (ctrl) sModifiers = "Ctrl+" + sModifiers;

            if (sModifiers.Trim() != "")
            {
                if (returnAfterModifiers) sModifiers += Environment.NewLine;
                sRet = sModifiers + sRet;
            }

            return sRet;
        }

        internal static void extractModifiersFromKey(Keys keyCode, out Keys cleanKey, out bool alt, out bool ctrl, out bool shift)
        {
            cleanKey = keyCode;
            alt = false;
            ctrl = false;
            shift = false;

            if ((keyCode & Keys.Alt) == Keys.Alt) alt = true;
            if ((keyCode & Keys.Control) == Keys.Control) ctrl = true;
            if ((keyCode & Keys.Shift) == Keys.Shift) shift = true;

            if (alt) cleanKey -= Keys.Alt;
            if (ctrl) cleanKey -= Keys.Control;
            if (shift) cleanKey -= Keys.Shift;

        }

        internal static Bitmap drawBmpText(Size sz, string text, Color textColor, System.Drawing.Font f, easyShortcutPosition position)
        {

            Bitmap bmp = new Bitmap(sz.Width, sz.Height);
            System.Drawing.Graphics _g = System.Drawing.Graphics.FromImage(bmp);

            RectangleF pRect = new RectangleF();

            float fTxtSizeW = f.Size * text.Length;
            float fTxtSizeH = f.Size;

            int numRighe = 1;
            if (text != null && text.Trim() != "" && text.IndexOf("\n") > 0)
            {
                numRighe = 0;
                string[] righetesto = text.Split('\n');
                for (int r = 0; r < righetesto.Length; r++)
                {
                    if (righetesto[r].Trim() != "" && righetesto[r].Trim() != "\r") numRighe += 1;
                }
            }
            if (numRighe > 1) fTxtSizeH = f.Size * numRighe;

            switch (position)
            {
                case easyShortcutPosition.top_right:
                    pRect = new RectangleF(sz.Width - fTxtSizeW - 2, 1, fTxtSizeW, fTxtSizeH + 2);
                    break;
                case easyShortcutPosition.top_left:
                    pRect = new RectangleF(1, 1, fTxtSizeW, fTxtSizeH + 2);
                    break;
                case easyShortcutPosition.bottom_right:
                    pRect = new RectangleF(sz.Width - fTxtSizeW - 2, sz.Height - fTxtSizeH - 4, fTxtSizeW, fTxtSizeH + 2);
                    break;
                case easyShortcutPosition.bottom_left:
                    pRect = new RectangleF(1, sz.Height - fTxtSizeH - 4, fTxtSizeW, fTxtSizeH + 2);
                    break;
                default:
                    break;
            }


            Brush brsh = new SolidBrush(Color.Transparent);
            _g.FillRectangle(brsh, pRect);

            brsh = new SolidBrush(textColor);
            StringFormat sFmt = new StringFormat();

            switch (position)
            {
                case easyShortcutPosition.top_left:
                case easyShortcutPosition.bottom_left:
                    sFmt.Alignment = StringAlignment.Near;
                    break;

                case easyShortcutPosition.top_right:
                case easyShortcutPosition.bottom_right:
                default:
                    sFmt.Alignment = StringAlignment.Far;
                    break;
            }

            _g.DrawString(text, f, brsh, pRect, sFmt);

            return bmp;
        }

        internal static bool skipKeyOnTextBox(Keys keyCode, Control.ControlCollection controls)
        {
            return skipKeyOnTextBox(keyCode, Keys.None, controls);
        }
        internal static bool skipKeyOnTextBox(Keys keyCode, Keys modifiers, Control.ControlCollection controls)
        {
            bool bRet = false;
            try
            {
                if (ScciCore.easyStatics.isKeyForTextBox(keyCode, modifiers))
                {
                    foreach (Control c in controls)
                    {

                        if (!bRet && c.GetType() == typeof(TextBox))
                        {
                            if (!((TextBox)c).ReadOnly && ((TextBox)c).Enabled && ((TextBox)c).Focused) bRet = true;
                        }

                        if (!bRet && c.GetType() == typeof(Infragistics.Win.UltraWinEditors.UltraTextEditor))
                        {
                            if (!((Infragistics.Win.UltraWinEditors.UltraTextEditor)c).ReadOnly && ((Infragistics.Win.UltraWinEditors.UltraTextEditor)c).Enabled && ((Infragistics.Win.UltraWinEditors.UltraTextEditor)c).Focused) bRet = true;
                        }

                        if (!bRet && c.GetType() == typeof(ucEasyTextBox))
                        {
                            if (!((ucEasyTextBox)c).ReadOnly && ((ucEasyTextBox)c).Enabled && ((ucEasyTextBox)c).Focused) bRet = true;
                        }

                        if (!bRet && c.GetType() == typeof(RichTextBox))
                        {
                            if (!((RichTextBox)c).ReadOnly && ((RichTextBox)c).Enabled && ((RichTextBox)c).Focused) bRet = true;
                        }

                        if (!bRet && c.GetType() == typeof(ucRichTextBox))
                        {
                            if (!((ucRichTextBox)c).ViewReadOnly && ((ucRichTextBox)c).Enabled && (((ucRichTextBox)c).Focused || ((ucRichTextBox)c).rtbRichTextBox.Focused)) bRet = true;
                        }

                        if (!bRet && c.GetType() == typeof(Infragistics.Win.UltraWinEditors.UltraDateTimeEditor))
                        {
                            if (!((Infragistics.Win.UltraWinEditors.UltraDateTimeEditor)c).ReadOnly && ((Infragistics.Win.UltraWinEditors.UltraDateTimeEditor)c).Enabled && ((Infragistics.Win.UltraWinEditors.UltraDateTimeEditor)c).Focused) bRet = isKeyForDateTimeEditor(keyCode, modifiers);
                        }

                        if (!bRet && c.GetType() == typeof(ucEasyDateTimeEditor))
                        {
                            if (!((ucEasyDateTimeEditor)c).ReadOnly && ((ucEasyDateTimeEditor)c).Enabled && ((ucEasyDateTimeEditor)c).Focused) bRet = isKeyForDateTimeEditor(keyCode, modifiers);
                        }

                        if (!bRet && c.GetType() == typeof(Infragistics.Win.UltraWinEditors.UltraComboEditor))
                        {
                            if (!((Infragistics.Win.UltraWinEditors.UltraComboEditor)c).ReadOnly && ((Infragistics.Win.UltraWinEditors.UltraComboEditor)c).Enabled && ((Infragistics.Win.UltraWinEditors.UltraComboEditor)c).Focused) bRet = true;
                        }

                        if (!bRet && c.GetType() == typeof(ucEasyComboEditor))
                        {
                            if (!((ucEasyComboEditor)c).ReadOnly && ((ucEasyComboEditor)c).Enabled && ((ucEasyComboEditor)c).Focused) bRet = true;
                        }

                        if (!bRet && c.GetType() == typeof(ucEasyDateRange))
                        {
                            if (!((ucEasyDateRange)c).ReadOnly && ((ucEasyDateRange)c).Enabled && ((ucEasyDateRange)c).Focused) bRet = true;
                        }

                        if (!bRet && c.Controls != null && c.Controls.Count > 0)
                        {
                            bRet = skipKeyOnTextBox(keyCode, modifiers, c.Controls);
                        }
                    }
                }

            }
            catch (Exception)
            {
            }
            return bRet;
        }

        internal static float coefficientefont()
        {
            try
            {
                if (_fontcoeff <= 0F)
                {

                    float screenwidt = (float)ScreenWidth;
                    float screenheight = (float)ScreenHeight;

                    float coeffX = screenwidt / 1024F;
                    float coeffY = screenheight / 670F;

                    if (screenwidt == 1920 && screenheight == 1144)
                    {
                        coeffX = screenwidt / 1920F;
                        coeffY = screenheight / 1144F;
                    }

                    _fontcoeff = coeffX;
                    if (_fontcoeff > coeffY) _fontcoeff = coeffY;
                }
            }
            catch (Exception)
            {
                _fontcoeff = 1F;
            }
            return _fontcoeff;
        }

        #endregion

        #region Dpi

        private enum ProcessDPIAwareness
        {
            ProcessDPIUnaware = 0,
            ProcessSystemDPIAware = 1,
            ProcessPerMonitorDPIAware = 2
        }

        [DllImport("shcore.dll")]
        private static extern int SetProcessDpiAwareness(ProcessDPIAwareness value);

        public static void SetDpiAwareness()
        {
            try
            {

                if (OSVersionHelper.IsWindowsVersionOrGreater(6, 3, 0))
                {
                    SetProcessDpiAwareness(ProcessDPIAwareness.ProcessPerMonitorDPIAware);
                }

            }
            catch (EntryPointNotFoundException)
            {
            }
        }

        #endregion

        #region LogTrace

        public static class LogTrace
        {

            public static string TracePath { get; set; }
            public static bool TraceEnabled { get; set; }

            private static object lockObject = new object();

            private static string TraceFilePath
            {
                get
                {
                    string path;

                    if (TracePath == "")
                    {
                        path = System.IO.Path.GetDirectoryName(
                                System.Reflection.Assembly.GetExecutingAssembly().Location);
                        path += @"\svc-logs\";
                    }
                    else
                        path = TracePath;

                    path += DateTime.Now.ToString("yyyyMMdd") + @".uctrace";

                    return path;
                }
            }

            public static void Add(string info)
            {
                try
                {
                    if (TraceEnabled == false)
                        return;


                    lock (lockObject)
                    {
                        string _trace = TraceFilePath;
                        StringBuilder sb = new StringBuilder();

                        string outp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.ffff") + " - " + info;

                        outp += " - ProcessID=" + Process.GetCurrentProcess().Id.ToString() + " ";

                        if (File.Exists(_trace) == false)
                        {
                            using (StreamWriter sw = File.CreateText(_trace))
                            {
                                string startOutp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.ffff") + " - Trace Created ";
                            }
                        }
                        using (StreamWriter sw = File.AppendText(_trace))
                        {
                            sw.WriteLine(outp);
                        }

                    }
                }
                catch
                {
                }
            }

        }

        #endregion

        #region DevExpress

        public static string getPathDocumentDE(string path)
        {

            string sPath = string.Empty;

            if (path.ToUpper().Contains(".PDF"))
            {
                sPath = path;
            }
            else if (path.ToUpper().Contains(".DOC"))
            {
                RichEditDocumentServer wordProcessor = new RichEditDocumentServer();
                wordProcessor.LoadDocument(path, DocumentFormat.Doc);
                string newPath = "TMP" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + @".pdf";
                newPath = System.IO.Path.Combine(Scci.Statics.FileStatics.GetSCCITempPath() + newPath);
                using (FileStream pdfFileStream = new FileStream(newPath, FileMode.Create))
                {
                    wordProcessor.ExportToPdf(pdfFileStream);
                }
                sPath = newPath;
            }

            return sPath;

        }

        #endregion
    }

}
