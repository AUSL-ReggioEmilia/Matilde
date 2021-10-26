using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnicodeSrl.Scci.Model;
using UnicodeSrl.Framework.Threading;
using System.Threading;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci;
using UnicodeSrl.ScciCore.Debugger;
using UnicodeSrl.ScciCore;
using UnicodeSrl.Framework.UI.Controls;

namespace UnicodeSrl.CdssScreenTiles
{
    public partial class BaseTileControl
        : UserControl,
            ITileUserCtl,
            ICustomWorker

    {
        private readonly Color C_HIGHLIGHT = Color.Yellow;

        public readonly float C_SIZE_NORMAL = 48f; public readonly float C_SIZE_FULL = 64f;
        private bool m_highilghted = false;

        public BaseTileControl()
        {
            InitializeComponent();

            this.Context = SynchronizationContext.Current;
            this.TileWindowState = en_TileWindowState.normal;

        }

        public BaseTileControl(AppDataMarshaler appDataMarshaler, T_ScreenTileRow screenTileRow, SelCdssRuoloRow selCdssRuoloRow) : this()
        {
            this.AppDataMarshaler = appDataMarshaler;
            this.ScreenTileRow = screenTileRow;
            this.SelCdssRuoloRow = selCdssRuoloRow;

            this.Highlighted = (this.ScreenTileRow.InEvidenza == true);
        }

        #region Properties

        [Browsable(true)]
        [Category("Cdss Tile")]
        public Image TileImage
        {
            get { return this.pbTile.Image; }
            set { this.pbTile.Image = value; }
        }

        [Browsable(true)]
        [Category("Cdss Tile")]
        public String TileText
        {
            get { return this.lblTitleTile.Text; }
            set { this.lblTitleTile.Text = value; }
        }


        [Browsable(true)]
        [Category("Cdss Tile")]
        public Boolean TileMinMaxButtonVisible
        {
            get { return this.cmdWnd.Visible; }
            set { this.cmdWnd.Visible = value; }
        }

        public en_TileWindowState TileWindowState
        {
            get;
            private set;
        }

        public AppDataMarshaler AppDataMarshaler
        {
            get; set;
        }

        public T_ScreenTileRow ScreenTileRow
        {
            get; set;
        }

        public SelCdssRuoloRow SelCdssRuoloRow { get; set; }

        public Control Control
        {
            get { return this as Control; }
        }

        public bool Highlighted
        {
            get { return m_highilghted; }
            set
            {
                m_highilghted = value;
                OnPropChange_Highlighted();
            }
        }

        public bool DrawBorder { get; set; }

        protected SynchronizationContext Context { get; set; }

        public ucEasyTableLayoutPanel ParentTable
        {
            get
            {
                if (this.Parent is ucEasyTableLayoutPanel)
                    return (ucEasyTableLayoutPanel)this.Parent;
                else
                    return null;
            }
        }

        private ucScreenView ParentScreenView
        {
            get
            {
                if (this.ParentTable == null) return null;
                var parent = this.ParentTable.Parent;

                if (parent == null) return null;
                if ((parent is ucScreenView) == false) return null;

                ucScreenView view = (ucScreenView)parent;

                return view;

            }
        }

        private TableLayoutPanelCellPosition? ParentTableCell
        {
            get
            {
                ucEasyTableLayoutPanel tab = this.ParentTable;

                if (tab != null)
                    return tab.GetPositionFromControl(this);
                else
                    return null;

            }
        }

        public int ParentTableRow
        {
            get
            {
                TableLayoutPanelCellPosition? cp = this.ParentTableCell;

                if (cp != null)
                    return cp.Value.Row;
                else
                    return -1;
            }
        }

        public int ParentTableColumn
        {
            get
            {
                TableLayoutPanelCellPosition? cp = this.ParentTableCell;

                if (cp != null)
                    return cp.Value.Column;
                else
                    return -1;
            }
        }

        private Paziente CachedPaziente { get; set; }

        private Episodio CachedEpisodio { get; set; }

        private Trasferimento CachedTrasferimento { get; set; }



        #endregion Properties

        #region     Prop & Metodi - Parametri

        public string XpTitolo
        {
            get { return this.lblTitleTile.Text; }
            set { this.lblTitleTile.Text = value; }
        }

        public bool XpVisualizzaTitolo
        {
            get { return this.tlpTile.Visible; }
            set { this.tlpTile.Visible = value; }
        }

        public bool XpVisualizzaMinMax
        {
            get;
            set;
        }

        #endregion  Prop & Metodi - Parametri

        #region     Resize Spec

        private Control OriginalContainer { get; set; }

        private ucEasyTableLayoutPanel OriginalTable { get; set; }

        private TableLayoutPanelCellPosition OriginalTableCellPosition { get; set; }

        private void tileResizeMaximize()
        {
            if (this.ParentScreenView == null) return;
            this.OriginalContainer = this.ParentScreenView;
            this.OriginalTable = this.ParentTable;

            if (ParentTableCell == null) return;
            this.OriginalTableCellPosition = this.ParentTableCell.Value;

            this.ParentTable.Visible = false;

            this.ParentTable.Controls.Remove(this);

            BaseTileControl thisControl = this;
            thisControl.Top = 0;
            thisControl.Left = 0;
            thisControl.Dock = DockStyle.Fill;
            thisControl.Visible = true;

            this.OriginalContainer.Controls.Add(this);

            this.cmdWnd.Appearance.Image = global::UnicodeSrl.ScciCore.Properties.Resources.Tile_Restore;
            this.TileWindowState = en_TileWindowState.maximized;

            this.DisplayUI(null);

        }

        private void tileResizeNormal()
        {
            this.OriginalTable.Dock = DockStyle.Fill;

            this.OriginalContainer.Controls.Remove(this);
            this.OriginalTable.Controls.Add(this, this.OriginalTableCellPosition.Column, this.OriginalTableCellPosition.Row);
            this.OriginalTable.SetColumnSpan(this, this.ScreenTileRow.Larghezza);
            this.OriginalTable.SetRowSpan(this, this.ScreenTileRow.Altezza);
            this.Dock = DockStyle.Fill;

            this.OriginalTable.Visible = true;


            this.cmdWnd.Appearance.Image = global::UnicodeSrl.ScciCore.Properties.Resources.Tile_Expand;
            this.TileWindowState = en_TileWindowState.normal;

            this.DisplayUI(null);
        }

        #endregion  Resize Spec

        public virtual void RefreshData()
        {
            CancellationToken ct = new CancellationToken(false);
            this.LoadData(ct);
            this.DisplayUI(ct);

        }

        public virtual void LoadData(object state)
        {

        }

        public virtual void DisplayUI(object state)
        {
            checkMinMaxEnable();

            removeLoadingControl();

        }
        public virtual void DisplayUiLoading()
        {


            if (this.PicBoxLoading == null)
            {
                this.PicBoxLoading = new PictureBox();

                this.content.Controls.Add(this.PicBoxLoading);

                this.PicBoxLoading.Dock = System.Windows.Forms.DockStyle.Fill;
                this.PicBoxLoading.Image = global::UnicodeSrl.ScciCore.Properties.Resources.W2;
                this.PicBoxLoading.Location = new System.Drawing.Point(0, 0);
                this.PicBoxLoading.Name = "pbLoading";
                this.PicBoxLoading.Size = new System.Drawing.Size(640, 428);
                this.PicBoxLoading.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
                this.PicBoxLoading.Visible = true;
            }
        }

        public virtual void DisplayUiWait()
        {
            UserControl thisControl = this;

            this.cmdWnd.Enabled = false;

            CoreStatics.impostaCursore(ref thisControl, enum_app_cursors.WaitCursor);

            CoreStatics.SetNavigazione(false);

        }

        public virtual void DisplayUiNormal()
        {
            UserControl thisControl = this;

            checkMinMaxEnable();

            CoreStatics.impostaCursore(ref thisControl, enum_app_cursors.DefaultCursor);

            CoreStatics.SetNavigazione(true);
        }




        public void SetCoreStatics()
        {
            this.CachedPaziente = CoreStatics.CoreApplication.Paziente;
            this.CachedEpisodio = CoreStatics.CoreApplication.Episodio;
            this.CachedTrasferimento = CoreStatics.CoreApplication.Trasferimento;

            CoreStatics.CoreApplication.Paziente = new Paziente("", this.AppDataMarshaler.ScciAmbiente.Idepisodio);
            CoreStatics.CoreApplication.Episodio = new Episodio(this.AppDataMarshaler.ScciAmbiente.Idepisodio);
            CoreStatics.CoreApplication.Trasferimento = new Trasferimento(this.AppDataMarshaler.ScciAmbiente.IdTrasferimento, this.AppDataMarshaler.ScciAmbiente);

            if (this.AppDataMarshaler.ScciAmbiente.Contesto.ContainsKey("Paziente") == true)
            {
                this.AppDataMarshaler.ScciAmbiente.Contesto.Remove("Paziente");
            }
            if (CoreStatics.CoreApplication.Paziente != null) { this.AppDataMarshaler.ScciAmbiente.Contesto.Add("Paziente", CoreStatics.CoreApplication.Paziente); }

            if (this.AppDataMarshaler.ScciAmbiente.Contesto.ContainsKey("Episodio") == true)
            {
                this.AppDataMarshaler.ScciAmbiente.Contesto.Remove("Episodio");
            }
            if (CoreStatics.CoreApplication.Episodio != null) { this.AppDataMarshaler.ScciAmbiente.Contesto.Add("Episodio", CoreStatics.CoreApplication.Episodio); }

            if (this.AppDataMarshaler.ScciAmbiente.Contesto.ContainsKey("Trasferimento") == true)
            {
                this.AppDataMarshaler.ScciAmbiente.Contesto.Remove("Trasferimento");
            }
            if (CoreStatics.CoreApplication.Trasferimento != null) { this.AppDataMarshaler.ScciAmbiente.Contesto.Add("Trasferimento", CoreStatics.CoreApplication.Trasferimento); }

        }


        public void ResetCoreStatics()
        {
            CoreStatics.CoreApplication.Paziente = this.CachedPaziente;
            CoreStatics.CoreApplication.Episodio = this.CachedEpisodio;
            CoreStatics.CoreApplication.Trasferimento = this.CachedTrasferimento;

            CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata = "";

            this.AppDataMarshaler.ScciAmbiente.Contesto.Clear();

        }

        private PictureBox PicBoxLoading { get; set; }

        private UniLabelExt TileLabelExt { get; set; }

        #region     ICustomWorker

        public virtual string ThreadName { get; set; }

        public void Worker(object parameters)
        {
            if ((parameters != null) && (parameters is CancellationToken))
            {
                CancellationToken token = (CancellationToken)token;
                Worker(token);
            }
            else
                Worker(null);
        }

        public virtual void Worker(CancellationToken token)
        {
            DateTime start = DateTime.Now;

            this.LoadData(token);



            this.DebugDate = DateTime.Now;
        }

        private DateTime DebugDate;

        public virtual void CancelCallback(object state) { }

        public virtual void OnThreadStarted(ThreadingEventArgs args)
        {
        }

        public virtual void OnThreadCompleted(ThreadingEventArgs args)
        {
            this.Context.Post(this.DisplayUI, args);
        }

        public virtual void OnThreadAborted(ThreadingEventArgs args)
        {

        }

        public virtual void OnThreadException(ThreadingExceptionEventArgs args)
        {
            UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(args.Exception);
        }

        #endregion  ICustomWorker

        #region     pvt e local evts

        private void OnPropChange_Highlighted()
        {
            this.Refresh();
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (this.DrawBorder)
                ControlPaint.DrawBorder(e.Graphics, new Rectangle(0, 0, this.Width, this.Height), Color.Gray, ButtonBorderStyle.Solid);

            if (this.Highlighted)
            {
                this.BackColor = C_HIGHLIGHT;
                this.Padding = new Padding(3);
            }
            else
            {
                this.BackColor = SystemColors.Control;
                this.Padding = new Padding(0);
            }

        }

        private void removeLoadingControl()
        {
            if ((this.PicBoxLoading != null) && (this.PicBoxLoading.Visible))
                this.PicBoxLoading.Visible = false;

            if ((this.TileLabelExt != null) && (this.TileLabelExt.Visible))
                this.TileLabelExt.Visible = false;
        }

        private void checkMinMaxEnable()
        {
            if ((this.XpVisualizzaMinMax) && ((this.ParentScreenView != null) || (this.OriginalContainer != null)))
            {
                this.cmdWnd.Enabled = true;
                this.cmdWnd.Visible = true;
            }
            else
            {
                this.cmdWnd.Enabled = false;
                this.cmdWnd.Visible = false;
            }
        }

        #endregion  pvt e local evts

        private void cmdWnd_Click(object sender, EventArgs e)
        {
            if (this.TileWindowState == en_TileWindowState.normal)
                tileResizeMaximize();
            else
                tileResizeNormal();
        }


    }
}
