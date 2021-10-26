using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinListView;
using Infragistics.Win.UltraWinToolTip;

namespace UnicodeSrl.ScciCore
{
    public partial class ucListboxSearch : UserControl
    {

        #region Declarations

        private bool _searchBoxVisible = false;

        private string _valueList = string.Empty;

        private string[] separatorevaloresingolo = { "#;" };
        private string[] separatorevalori = { "§;" };

        public new event ItemActivatedEventHandler ListBoxItemClick;

        #endregion

        public ucListboxSearch()
        {
            InitializeComponent();
        }

        #region Properties

        public bool SearchBoxVisible
        {
            get { return this._searchBoxVisible; }
            set
            {
                this._searchBoxVisible = value;
                this.SetLayout();
            }
        }

        public bool MultipleSelection
        {
            get { return this.ListBox.ViewSettingsList.CheckBoxStyle == CheckBoxStyle.CheckBox; }
            set
            {
                if (value)
                    this.ListBox.ViewSettingsList.CheckBoxStyle = CheckBoxStyle.CheckBox;
                else
                    this.ListBox.ViewSettingsList.CheckBoxStyle = CheckBoxStyle.None;
            }
        }

        public string Value
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }

        public string ValueList
        {
            get { return this._valueList; }
            set
            {
                this._valueList = value;
                this.LoadListBox();
            }
        }

        public bool HideSelection
        {
            get { return this.ListBox.ItemSettings.HideSelection; }
            set
            {
                this.ListBox.ItemSettings.HideSelection = value;
            }
        }

        #endregion

        #region Public Methods

        public void ClearSelection()
        {
            this.ListBox.SelectedItems.Clear();
        }

        public void ClearList()
        {
            this.ListBox.Items.Clear();
        }

        #endregion

        #region Private Methods

        private void SetLayout()
        {
            RowStyle rs = new RowStyle();
            rs.SizeType = SizeType.Absolute;

            if (this.SearchBoxVisible)
            {
                rs.Height = 53;
            }
            else
            {
                rs.Height = 0;
            }

            tlp.RowStyles[0] = rs;
        }

        private void LoadListBox()
        {
            string[] arvalori = null;
            UltraListViewItem item = null;

            this.ListBox.Items.Clear();

            try
            {
                arvalori = this.ValueList.Split(separatorevalori, StringSplitOptions.RemoveEmptyEntries);

                foreach (string valore in arvalori)
                {
                    item = this.GetListBoxItem(valore, separatorevaloresingolo, ref this.txtSearchListBox);
                    if (item != null) this.ListBox.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }

        }

        private UltraListViewItem GetListBoxItem(string valore, string[] separatorevaloresingolo, ref ucEasyTextBox searchTextBox)
        {
            UltraListViewItem ret = null;
            string sKey = string.Empty;
            string sValue = string.Empty;

            try
            {
                string[] arvalore = valore.Split(separatorevaloresingolo, StringSplitOptions.RemoveEmptyEntries);

                sKey = arvalore[0];

                if (arvalore.Length > 1)
                    sValue = arvalore[1];
                else
                    sValue = arvalore[0];

                if (searchTextBox.Visible && searchTextBox.Text != string.Empty)
                {
                    if (sValue.ToUpper().Contains(searchTextBox.Text.ToUpper()))
                    {
                        ret = new UltraListViewItem(sKey);
                        ret.Value = sValue;
                    }
                    else
                    {
                        ret = null;
                    }
                }
                else
                {
                    ret = new UltraListViewItem(sKey);
                    ret.Value = sValue;
                }
            }
            catch
            {
                ret = null;
            }

            return ret;
        }

        private void SetValue(string value)
        {
            string[] separatorevalori = { "§;" };

            try
            {
                this.ListBox.SelectedItems.Clear();

                if (value != string.Empty)
                {
                    if (this.MultipleSelection)
                    {
                        string[] multiselectvalues = value.Split(separatorevalori, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string multiselectvalue in multiselectvalues)
                        {
                            if (this.ListBox.ActiveItem == null) this.ListBox.ActiveItem = this.ListBox.Items[multiselectvalue];
                            this.ListBox.Items[multiselectvalue].CheckState = CheckState.Checked;
                        }
                    }
                    else
                    {
                        this.ListBox.ActiveItem = this.ListBox.Items[value];
                    }

                    ListBox.PerformAction(UltraListViewAction.SelectItem);
                }
                else
                {
                    this.ListBox.ActiveItem = null;
                }

                if (this.SearchBoxVisible)
                {
                    this.txtSearchListBox.Text = string.Empty;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                this.ListBox.SelectedItems.Clear();
            }
        }

        private string GetValue()
        {
            string sret = string.Empty;

            try
            {
                if (this.MultipleSelection)
                {
                    string multiselectvalue = string.Empty;
                    foreach (Infragistics.Win.UltraWinListView.UltraListViewItem listitem in this.ListBox.Items)
                    {
                        if (listitem.CheckState == CheckState.Checked)
                            multiselectvalue += listitem.Key.ToString() + "§;";
                    }
                    if (multiselectvalue != string.Empty) multiselectvalue = multiselectvalue.Remove(multiselectvalue.Length - 2);
                    sret = multiselectvalue;

                }
                else
                {
                    sret = this.ListBox.SelectedItems[0].Key.ToString();
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                sret = string.Empty;
            }

            return sret;

        }

        private UltraListViewItem GetItemFromPointerPosition()
        {
            UltraListViewItem listviewitem = null;

            try
            {
                Point mousepointerposition = Cursor.Position;
                Point listboxClientAreaPosition = this.ListBox.PointToClient(mousepointerposition);
                listviewitem = this.ListBox.ItemFromPoint(listboxClientAreaPosition);
            }
            catch
            {
                listviewitem = null;
            }

            return listviewitem;
        }

        #endregion

        #region Events

        private void ucListboxSearch_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                this.ListBox.Focus();
            }
        }

        private void ListBox_MouseEnterElement(object sender, Infragistics.Win.UIElementEventArgs e)
        {
            try
            {
                UltraListViewItem listviewitem = GetItemFromPointerPosition();

                UltraToolTipInfo tip = new UltraToolTipInfo();
                tip.ToolTipText = listviewitem.Text;
                ultraToolTipManager.SetUltraToolTip(this.ListBox, tip);
                ultraToolTipManager.ShowToolTip(this.ListBox);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                this.ultraToolTipManager.HideToolTip();
            }
        }

        private void ListBox_MouseLeaveElement(object sender, Infragistics.Win.UIElementEventArgs e)
        {
            if (this.ultraToolTipManager.IsToolTipVisible()) this.ultraToolTipManager.HideToolTip();
        }

        private void ListBox_ToolTipDisplaying(object sender, ToolTipDisplayingEventArgs e)
        {
            e.Cancel = true;
        }

        private void ListBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.ListBoxItemClick != null)
            {
                UltraListViewItem item = this.GetItemFromPointerPosition();
                if (item != null)
                {
                    ItemActivatedEventArgs eventargs = new ItemActivatedEventArgs(item);
                    this.ListBoxItemClick(sender, eventargs);
                    this.ListBox.ActiveItem = null;
                }
            }
        }

        private void txtSearchMultiBox_ValueChanged(object sender, EventArgs e)
        {
            this.LoadListBox();
        }

        #endregion

    }
}
