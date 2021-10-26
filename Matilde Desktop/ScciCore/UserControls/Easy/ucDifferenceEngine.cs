using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnicodeSrl.ScciCore.DifferenceEngine;
using System.Collections;

namespace UnicodeSrl.ScciCore
{
    public partial class ucDifferenceEngine : UserControl, Interfacce.IViewUserControlBase
    {
        public ucDifferenceEngine()
        {
            InitializeComponent();
        }

        #region Declare

        private IDiffList _source = null;
        private IDiffList _destination = null;
        private DiffEngineLevel _level = DiffEngineLevel.FastImperfect;

        #endregion

        #region Interface

        public void ViewInit()
        {

        }

        #endregion

        #region properties

        public IDiffList Source
        {
            get { return _source; }
            set { _source = value; }
        }

        public IDiffList Destination
        {
            get { return _destination; }
            set { _destination = value; }
        }

        public DiffEngineLevel Level
        {
            get { return _level; }
            set { _level = value; }
        }

        private ArrayList DiffLines { get; set; }

        #endregion

        #region Subroutine

        private void Process()
        {

            this.Cursor = Cursors.WaitCursor;

            DiffEngine de = new DiffEngine();
            de.ProcessDiff(_source, _destination, _level);
            DiffLines = de.DiffReport();
            de = null;

            this.RefreshUI();

            this.Cursor = Cursors.Default;

        }

        private void RefreshUI()
        {

            try
            {

                ListViewItem lviS;
                ListViewItem lviD;
                int cnt = 1;
                int i;

                this.lvSource.Items.Clear();
                this.lvDestination.Items.Clear();

                this.lvSource.Columns[1].Text = _source.Caption;
                this.lvDestination.Columns[1].Text = _destination.Caption;

                foreach (DiffResultSpan drs in DiffLines)
                {

                    switch (drs.Status)
                    {

                        case DiffResultSpanStatus.DeleteSource:
                            for (i = 0; i < drs.Length; i++)
                            {
                                lviS = new ListViewItem(cnt.ToString("00000"));
                                lviD = new ListViewItem(cnt.ToString("00000"));
                                lviS.BackColor = Color.Red;
                                lviS.SubItems.Add(((TextLine)_source.GetByIndex(drs.SourceIndex + i)).Line);
                                lviD.BackColor = Color.LightGray;
                                lviD.SubItems.Add("");

                                lvSource.Items.Add(lviS);
                                lvDestination.Items.Add(lviD);
                                cnt++;
                            }
                            break;

                        case DiffResultSpanStatus.NoChange:
                            for (i = 0; i < drs.Length; i++)
                            {
                                lviS = new ListViewItem(cnt.ToString("00000"));
                                lviD = new ListViewItem(cnt.ToString("00000"));
                                lviS.BackColor = Color.White;
                                lviS.SubItems.Add(((TextLine)_source.GetByIndex(drs.SourceIndex + i)).Line);
                                lviD.BackColor = Color.White;
                                lviD.SubItems.Add(((TextLine)_destination.GetByIndex(drs.DestIndex + i)).Line);

                                lvSource.Items.Add(lviS);
                                lvDestination.Items.Add(lviD);
                                cnt++;
                            }
                            break;

                        case DiffResultSpanStatus.AddDestination:
                            for (i = 0; i < drs.Length; i++)
                            {
                                lviS = new ListViewItem(cnt.ToString("00000"));
                                lviD = new ListViewItem(cnt.ToString("00000"));
                                lviS.BackColor = Color.LightGray;
                                lviS.SubItems.Add("");
                                lviD.BackColor = Color.LightGreen;
                                lviD.SubItems.Add(((TextLine)_destination.GetByIndex(drs.DestIndex + i)).Line);

                                lvSource.Items.Add(lviS);
                                lvDestination.Items.Add(lviD);
                                cnt++;
                            }
                            break;

                        case DiffResultSpanStatus.Replace:
                            for (i = 0; i < drs.Length; i++)
                            {
                                lviS = new ListViewItem(cnt.ToString("00000"));
                                lviD = new ListViewItem(cnt.ToString("00000"));
                                lviS.BackColor = Color.Red;
                                lviS.SubItems.Add(((TextLine)_source.GetByIndex(drs.SourceIndex + i)).Line);
                                lviD.BackColor = Color.LightGreen;
                                lviD.SubItems.Add(((TextLine)_destination.GetByIndex(drs.DestIndex + i)).Line);

                                lvSource.Items.Add(lviS);
                                lvDestination.Items.Add(lviD);
                                cnt++;
                            }
                            break;

                    }

                }

            }
            catch (Exception)
            {

            }

        }

        #endregion

        #region Public Method

        public void RefreshDiff()
        {
            this.Process();
        }
        public void RefreshDiff(IDiffList source, IDiffList destination, DiffEngineLevel level)
        {

            _source = source;
            _destination = destination;
            _level = level;

            this.Process();

        }

        #endregion

        #region Events

        private void lvSource_Resize(object sender, System.EventArgs e)
        {
            if (lvSource.Width > 100)
            {
                lvSource.Columns[1].Width = -2;
            }
        }

        private void lvDestination_Resize(object sender, System.EventArgs e)
        {
            if (lvDestination.Width > 100)
            {
                lvDestination.Columns[1].Width = -2;
            }
        }

        private void lvSource_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (lvSource.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvDestination.Items[lvSource.SelectedItems[0].Index];
                lvi.Selected = true;
                lvi.EnsureVisible();
            }
        }

        private void lvDestination_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (lvDestination.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvSource.Items[lvDestination.SelectedItems[0].Index];
                lvi.Selected = true;
                lvi.EnsureVisible();
            }
        }

        #endregion

    }
}
