using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static UnicodeSrl.ScciCore.Interfacce;
using UnicodeSrl.Scci.Model;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.ScciCore.UserControls.ScreenAndTiles;
using UnicodeSrl.Framework.Threading;
using System.Threading;
using UnicodeSrl.Scci.DataContracts;

namespace UnicodeSrl.ScciCore
{
    public partial class ucScreenView : UserControl,
        IViewUserControlMiddle
    {

        public ucScreenView(string codUA, string codRuolo, T_ScreenRow screenObj)
        {
            InitializeComponent();

            this.CodRuolo = codRuolo;
            this.CodUA = codUA;
            this.ScreenRow = screenObj;

            using (FwDataConnection conn = new FwDataConnection(Database.ConnectionString))
            {
                this.ScreenTiles = conn.MSP_SelScreenTile(this.ScreenRow.Codice);
            }

        }


        #region     Props

        internal ClrThreadPool ThreadPool { get; set; }

        public T_ScreenRow ScreenRow { get; private set; }

        public FwDataBufferedList<T_ScreenTileRow> ScreenTiles { get; private set; }

        public string CodRuolo { get; private set; }

        public string CodUA { get; private set; }

        #endregion  Props


        #region IViewUserControlMiddle

        public void Aggiorna()
        {
            loadControls();
        }

        public void Carica()
        {
            loadUiGrid();

            loadControls();
        }

        public void Ferma()
        {
            this.ThreadPool.Abort();

            try
            {
                int count = this.tlpMain.Controls.Count;
                for (int i = 0; i < count; i++)
                {
                    Control c = this.tlpMain.Controls[0];
                    if (c is ITileUserCtl)
                    {
                        this.tlpMain.Controls.Remove(c);
                        c.Dispose();
                        c = null;
                    }
                }

                List<Control> listToRemove = new List<Control>();

                foreach (Control c in this.Controls)
                {
                    if (c is ITileUserCtl) listToRemove.Add(c);
                }

                foreach (Control c in listToRemove)
                {
                    this.Controls.Remove(c);
                    c.Dispose();
                }

                listToRemove.Clear();
                this.tlpMain.Controls.Clear();

                this.tlpMain.Visible = true;

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }

        }

        #endregion IViewUserControlMiddle


        #region     UI

        private void loadUiGrid()
        {
            RowStyle rs = null;
            ColumnStyle cs = null;

            int cWidth = Convert.ToInt32(this.Width);
            int cHeight = Convert.ToInt32(this.Height * .98);

            int wCol = Convert.ToInt32(cWidth / this.ScreenRow.Colonne);
            int hRow = Convert.ToInt32(cHeight / this.ScreenRow.Righe);

            this.tlpMain.RowStyles.Clear();


            for (int i = 0; i < this.ScreenRow.Righe; i++)
            {
                rs = new RowStyle(SizeType.Absolute, hRow);
                this.tlpMain.RowStyles.Add(rs);
            }

            this.tlpMain.ColumnStyles.Clear();
            for (int i = 0; i < this.ScreenRow.Colonne; i++)
            {
                cs = new ColumnStyle(SizeType.Absolute, wCol);
                this.tlpMain.ColumnStyles.Add(cs);
            }


            this.tlpMain.RowCount = this.ScreenRow.Righe;
            this.tlpMain.ColumnCount = this.ScreenRow.Colonne;


        }

        private void loadControls()
        {
            this.ThreadPool = new ClrThreadPool();
            this.ThreadPool.SynchronizationContext = SynchronizationContext.Current;

            bool created = (this.tlpMain.Controls.Count > 0);
            AppDataMarshaler appData = loadAppData();

            if (created)
            {
                foreach (Control c in this.tlpMain.Controls)
                {
                    if (c is ITileUserCtl)
                    {
                        ITileUserCtl tileControl = (ITileUserCtl)c;
                        tileControl.AppDataMarshaler = appData;
                        tileControl.DisplayUiLoading();
                        this.ThreadPool.QueueWorker(tileControl);
                    }
                }

                foreach (Control c in this.Controls)
                {
                    if (c is ITileUserCtl)
                    {
                        ITileUserCtl tileControl = (ITileUserCtl)c;
                        tileControl.AppDataMarshaler = appData;
                        tileControl.DisplayUiLoading();
                        this.ThreadPool.QueueWorker(tileControl);
                    }
                }
            }
            else
            {

                foreach (T_ScreenTileRow tileObject in this.ScreenTiles.Buffer)
                {
                    try
                    {
                        ITileUserCtl tileControl = TileContolFactory.CreateTileUserCtl(appData, tileObject);

                        this.tlpMain.AddTileControl(tileControl, tileObject);
                        tileControl.DisplayUiLoading();

                        this.ThreadPool.QueueWorker(tileControl);

                    }
                    catch (Exception ex)
                    {
                        UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                    }
                }
            }


        }

        private void ucScreenView_VisibleChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.Visible) checkTlpRowColSize();
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }
        }

        private void ucScreenView_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.Visible) checkTlpRowColSize();
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }
        }

        private void checkTlpRowColSize()
        {
            int cWidth = Convert.ToInt32(this.Width);
            int cHeight = Convert.ToInt32(this.Height * .98);

            int wCol = Convert.ToInt32(cWidth / this.ScreenRow.Colonne);
            int hRow = Convert.ToInt32(cHeight / this.ScreenRow.Righe);

            foreach (RowStyle rs in this.tlpMain.RowStyles)
            {
                rs.Height = hRow;
            }

            foreach (ColumnStyle cs in this.tlpMain.ColumnStyles)
            {
                cs.Width = wCol;
            }

        }

        #endregion  UI

        private AppDataMarshaler loadAppData()
        {
            string xmlAmbiente = CoreStatics.CoreApplication.Ambiente.XmlSerializeToString();


            this.CodUA = "";
            if (CoreStatics.CoreApplication.Trasferimento != null)
                this.CodUA = CoreStatics.CoreApplication.Trasferimento.CodUA;

            if (CoreStatics.CoreApplication.Paziente != null && CoreStatics.CoreApplication.Paziente.CodUAAmbulatoriale != null && CoreStatics.CoreApplication.Paziente.CodUAAmbulatoriale.Trim() != "")
                this.CodUA = CoreStatics.CoreApplication.Paziente.CodUAAmbulatoriale;

            AppDataMarshaler appData = new AppDataMarshaler();
            appData.CodRuolo = this.CodRuolo;
            appData.CodUA = this.CodUA;
            appData.CartellaChiusa = CoreStatics.CoreApplication.Cartella.CartellaChiusa;
            appData.CodUA_Ambulatoriale = CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata;
            appData.CodUO = CoreStatics.CoreApplication.CodUOSelezionata;

            appData.NumeroCartella = CoreStatics.CoreApplication.Cartella.NumeroCartella;
            appData.CodStatoCartella = CoreStatics.CoreApplication.Cartella.CodStatoCartella;
            appData.DecrStatoCartella = "";

            appData.ScciAmbiente = new ScciAmbiente();
            appData.ScciAmbiente = XmlProcs.XmlDeserializeFromString<ScciAmbiente>(xmlAmbiente);


            return appData;
        }


    }

}
