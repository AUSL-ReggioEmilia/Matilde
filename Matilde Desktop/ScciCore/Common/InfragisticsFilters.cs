using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Drawing;
using Infragistics.Win;
using Infragistics.Win.UltraWinSchedule;
using Infragistics.Win.UltraWinSchedule.TimelineView;
using Infragistics.Win.UltraWinTree;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci.Model;

namespace UnicodeSrl.ScciCore
{

    #region Delegate

    public delegate void MyClickEventHandler(object sender, UIElementEventArgs e);

    #endregion

    #region IUIElementDrawFilter Members

    public class DrawFilterManager : List<IUIElementDrawFilter>, IUIElementDrawFilter
    {

        public DrawFilterManager()
        { }

        public bool DrawElement(DrawPhase drawPhase, ref UIElementDrawParams drawParams)
        {

            bool result = false;

            foreach (IUIElementDrawFilter filter in this)
            {

                result |= filter.DrawElement(drawPhase, ref drawParams);
            }

            return result;
        }

        public DrawPhase GetPhasesToFilter(ref UIElementDrawParams drawParams)
        {

            DrawPhase result = DrawPhase.None; foreach (IUIElementDrawFilter filter in this)
            {

                result |= filter.GetPhasesToFilter(ref drawParams);
            }

            return result;

        }

    }

    public class DrawFilterUltraTimeLineViewSplitOwner : IUIElementDrawFilter
    {

        private const int C_WITH = 25;
        public Rectangle OwnerRectInsideBorders { get; set; }

        public DrawFilterUltraTimeLineViewSplitOwner()
        {
        }

        public bool DrawElement(DrawPhase drawPhase, ref UIElementDrawParams drawParams)
        {

            bool drawElement = false;

            switch (drawPhase)
            {

                case DrawPhase.BeforeDrawElement:
                    if ((drawParams.Element is TextUIElement) && (drawParams.Element.Parent.GetType() != typeof(ButtonUIElement)))
                    {
                        drawElement = this.SplitOwnerHeaderText(ref drawParams);
                    }
                    break;

                default:
                    break;

            }

            return drawElement;

        }

        public DrawPhase GetPhasesToFilter(ref UIElementDrawParams drawParams)
        {

            if (drawParams.Element is TextUIElement)
            {
                return DrawPhase.BeforeDrawElement;
            }
            else
            {
                if (drawParams.Element is OwnerHeaderUIElement)
                {
                    this.OwnerRectInsideBorders = ((OwnerHeaderUIElement)drawParams.Element).RectInsideBorders;
                }
                return DrawPhase.None;
            }

        }

        private bool SplitOwnerHeaderText(ref UIElementDrawParams drawParams)
        {
            TextUIElement textElement = drawParams.Element as TextUIElement;

            if (textElement != null)
            {

                OwnerHeaderUIElement headerElement = textElement.GetAncestor(typeof(OwnerHeaderUIElement)) as OwnerHeaderUIElement;

                if (headerElement != null)
                {

                    DataRowView oRowView = (System.Data.DataRowView)(headerElement).Owner.BindingListObject;

                    if (oRowView.Row["CodAgenda"].ToString().StartsWith("BLK"))
                    {
                        drawParams.Graphics.DrawString(textElement.Text, new Font(drawParams.Font.Name, (float)(drawParams.Font.Size * 1.2), FontStyle.Bold), new SolidBrush(Color.Black), textElement.RectInsideBorders.X, textElement.RectInsideBorders.Y);
                        return true;
                    }
                    else
                    {

                        string text = oRowView.Row["DescrVoce"].ToString();
                        string space = " ";

                        int spaceCount = 0;
                        int spacePos = -1;

                        do
                        {
                            spaceCount += 1;
                            spacePos = text.IndexOf(space, spacePos + 1);
                        }
                        while (spacePos > 0);

                        spaceCount -= 1;

                        int nthSpace = 0;

                        if (spaceCount > 0)
                        {
                            if ((spaceCount % 2) == 0)
                            {
                                nthSpace = ((spaceCount / 2) + 1);
                            }
                            else
                            {
                                nthSpace = ((spaceCount + 1) / 2);

                            }

                            if (nthSpace > 2)
                            {
                                spaceCount = 0;
                                spacePos = -1;

                                while (spaceCount < nthSpace)
                                {
                                    spaceCount += 1;
                                    spacePos = text.IndexOf(space, spacePos + 1);
                                }

                                string line1 = text.Substring(0, spacePos).Trim();
                                string line2 = text.Substring((spacePos + 1), (text.Length - spacePos - 1)).Trim();

                                textElement.MultiLine = true;

                                Rectangle textRect = textElement.Rect;

                                string aaa = line1.Replace("\t", " ") + Environment.NewLine + line2.Replace("\t", " ");
                                string[] bbb = aaa.Split('\n');
                                var sip = 10 * drawParams.Graphics.DpiY / 72;

                                int iRefWidth = headerElement.Rect.Width - textRect.X - 10;

                                aaa = string.Empty;
                                foreach (string s in bbb)
                                {

                                    if ((s.Length * sip) < iRefWidth)
                                    {
                                        aaa += s.Trim() + Environment.NewLine;
                                    }
                                    else
                                    {
                                        int chunkSize = (int)(iRefWidth / sip) * 2;
                                        int stringLength = s.Length;
                                        for (int i = 0; i < stringLength; i += chunkSize)
                                        {
                                            if (i + chunkSize > stringLength) chunkSize = stringLength - i;
                                            aaa += s.Substring(i, chunkSize).Trim() + Environment.NewLine;
                                        }
                                    }

                                }

                                textElement.Text = aaa;

                                int nlCount = 0;
                                int nlPos = -1;
                                do
                                {
                                    nlCount += 1;
                                    nlPos = textElement.Text.IndexOf("\n", nlPos + 1);
                                }
                                while (nlPos > 0);

                                textRect.Height = ((nlCount + 1) * C_WITH);
                                textRect.Width = iRefWidth; textElement.Rect = textRect;

                            }

                        }

                    }

                }

            }

            return false;
        }

    }

    public class DrawFilterUltraTimeLineViewCurrentTime : IUIElementDrawFilter
    {

        public DrawFilterUltraTimeLineViewCurrentTime()
        {
        }

        public bool DrawElement(DrawPhase drawPhase, ref UIElementDrawParams drawParams)
        {

            bool drawElement = false;

            switch (drawPhase)
            {

                case DrawPhase.BeforeDrawBackColor:
                    if ((drawParams.Element is TextUIElement) && (drawParams.Element.Parent.GetType() == typeof(ColumnHeaderUIElement)))
                    {

                        if (((ColumnHeaderUIElement)drawParams.Element.Parent).DateTimeInterval.IsPrimaryInterval == true)
                        {
                            DateTimeRange header = (DateTimeRange)drawParams.Element.Parent.GetContext(typeof(DateTimeRange));
                            if (header.StartDateTime < DateTime.Now && header.EndDateTime > DateTime.Now)
                            {
                                drawParams.Graphics.FillRectangle(new SolidBrush(Color.Yellow), drawParams.Element.RectInsideBorders);
                                drawElement = true;
                            }
                            else if (header.StartDateTime < DateTime.Now)
                            {
                                drawParams.Graphics.DrawRectangle(new Pen(Color.Red, 2), drawParams.Element.RectInsideBorders);
                                drawElement = true;
                            }
                        }

                    }
                    break;

                case DrawPhase.BeforeDrawForeground:


                    break;

                default:
                    break;

            }

            return drawElement;

        }

        public DrawPhase GetPhasesToFilter(ref UIElementDrawParams drawParams)
        {

            if (drawParams.Element is TextUIElement)
            {
                return DrawPhase.BeforeDrawBackColor;
            }
            else
            {
                return DrawPhase.None;
            }

        }

    }

    public class DrawFilterUltraTimeLineViewMoreApp : IUIElementDrawFilter
    {

        public bool DrawElement(DrawPhase drawPhase, ref UIElementDrawParams drawParams)
        {

            bool drawElement = false;

            switch (drawPhase)
            {

                case DrawPhase.BeforeDrawElement:
                    if (drawParams.Element is ActivityScrollButtonUIElement)
                    {


                        drawParams.Graphics.DrawRectangle(new Pen(Color.Red, 2), drawParams.Element.Parent.RectInsideBorders);

                    }
                    break;

                default:
                    break;

            }

            return drawElement;

        }

        public DrawPhase GetPhasesToFilter(ref UIElementDrawParams drawParams)
        {

            if (drawParams.Element is ActivityScrollButtonUIElement)
            {
                return DrawPhase.BeforeDrawElement;
            }
            else
            {
                return DrawPhase.None;
            }

        }

    }

    public class DrawFilterUltraTimeLineViewAgendaBlank : IUIElementDrawFilter
    {

        public bool DrawElement(DrawPhase drawPhase, ref UIElementDrawParams drawParams)
        {

            bool drawElement = false;

            switch (drawPhase)
            {

                case DrawPhase.AfterDrawElement:
                    if (drawParams.Element is TimeSlotUIElement)
                    {
                        if (((TimeSlotUIElement)drawParams.Element).Owner != null && ((TimeSlotUIElement)drawParams.Element).Owner.Key.StartsWith("BLK"))
                        {

                            SolidBrush oBrush = new SolidBrush(Color.FromArgb(75, 105, 105, 105));
                            drawParams.Graphics.FillRectangle(oBrush, drawParams.Element.RectInsideBorders);

                        }
                    }
                    break;

                default:
                    break;

            }

            return drawElement;

        }

        public DrawPhase GetPhasesToFilter(ref UIElementDrawParams drawParams)
        {

            if (drawParams.Element is TimeSlotUIElement)
            {
                return DrawPhase.AfterDrawElement;
            }
            else
            {
                return DrawPhase.None;
            }

        }

    }

    #endregion

    #region IUIElementCreationFilter Members

    public class CreationFilterManager : List<IUIElementCreationFilter>, IUIElementCreationFilter
    {

        public CreationFilterManager()
        { }

        public void AfterCreateChildElements(UIElement parent)
        {

            foreach (IUIElementCreationFilter filter in this)
            {

                filter.AfterCreateChildElements(parent);
            }

        }

        public bool BeforeCreateChildElements(UIElement parent)
        {

            bool result = false;

            foreach (IUIElementCreationFilter filter in this)
            {

                result |= filter.BeforeCreateChildElements(parent);
            }

            return result;

        }

    }

    public class CreationFilterUltraTimeLineViewButton : IUIElementCreationFilter
    {

        #region Declare

        private const int C_WITH = 25;
        private bool _cartellachiusa = false;


        public event MyClickEventHandler MenuClick;

        #endregion

        #region Costructor

        public CreationFilterUltraTimeLineViewButton(bool cartellachiusa)
        {
            _cartellachiusa = cartellachiusa;
        }

        #endregion

        #region Interface

        public void AfterCreateChildElements(UIElement parent)
        {

            try
            {

                ButtonUIElement elementToAdd = null;
                Rectangle rc;
                int nPos = 1;

                if (parent.GetType() == typeof(OwnerHeaderUIElement))
                {

                    DataRowView oRowView = (System.Data.DataRowView)((OwnerHeaderUIElement)parent).Owner.BindingListObject;

                    Infragistics.Win.Appearance oAppButton = new Infragistics.Win.Appearance();
                    oAppButton.BackColor = System.Drawing.Color.SteelBlue;
                    oAppButton.BackColor2 = System.Drawing.Color.LightSteelBlue;
                    oAppButton.FontData.Bold = DefaultableBoolean.True;
                    oAppButton.ForeColor = System.Drawing.Color.White;
                    oAppButton.ThemedElementAlpha = Alpha.Transparent;

                    if ((int.Parse(oRowView["PermessoInserimento"].ToString()) == 1 && this.CartellaChiusa == false) ||
                        (oRowView["Azione"].ToString() != string.Empty && this.CartellaChiusa == false) ||
                        (int.Parse(oRowView["PermessoSospendi"].ToString()) == 1 && this.CartellaChiusa == false) ||
                        (int.Parse(oRowView["PermessoGrafico"].ToString()) == 1) ||
                        (int.Parse(oRowView["PermessoDettaglio"].ToString()) == 1) ||
                        (oRowView["RTF"].ToString() != string.Empty))
                    {
                        elementToAdd = new ButtonUIElement(parent);
                        elementToAdd.Text = "M";
                        elementToAdd.Appearance = oAppButton;

                        elementToAdd.ElementClick += this.RaiseMenuClick;

                        rc = parent.RectInsideBorders;
                        rc.X = rc.Width - (C_WITH * nPos) - 2;
                        rc.Y = rc.Y + rc.Height - C_WITH - 2;
                        rc.Height = C_WITH;
                        rc.Width = C_WITH;
                        elementToAdd.Rect = rc;
                        parent.ChildElements.Add(elementToAdd);
                        nPos += 1;
                    }




















                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        public bool BeforeCreateChildElements(UIElement parent)
        {
            return false;
        }

        #endregion

        #region Property

        public bool CartellaChiusa
        {
            get { return _cartellachiusa; }
        }

        #endregion

        #region Events







        public void RaiseMenuClick(object sender, UIElementEventArgs e)
        {
            this.MenuClick(sender, e);
        }

        #endregion

    }

    public class CreationFilterUltraTreeButton : IUIElementCreationFilter
    {

        #region Declare

        private const int C_WITH = 25;
        private bool _cartellachiusa = false;

        public event MyClickEventHandler AddClick;

        #endregion

        #region Costructor

        public CreationFilterUltraTreeButton(bool cartellachiusa)
        {
            _cartellachiusa = cartellachiusa;
        }

        #endregion

        #region Interface

        public void AfterCreateChildElements(UIElement parent)
        {

            try
            {

                ButtonUIElement elementToAdd = null;
                Rectangle rc;
                Rectangle childRect;

                if (this.CartellaChiusa == false)
                {
                    if (parent is NodeSelectableAreaUIElement)
                    {

                        if (((UltraTreeNode)parent.SelectableItem).Tag.ToString() != string.Empty && ((UltraTreeNode)parent.SelectableItem).Level == 0)
                        {
                            Infragistics.Win.Appearance oAppButton = new Infragistics.Win.Appearance();
                            oAppButton.BackColor = System.Drawing.Color.SteelBlue;
                            oAppButton.BackColor2 = System.Drawing.Color.LightSteelBlue;
                            oAppButton.FontData.Bold = DefaultableBoolean.True;
                            oAppButton.ForeColor = System.Drawing.Color.White;
                            oAppButton.ThemedElementAlpha = Alpha.Transparent;

                            elementToAdd = new ButtonUIElement(parent);
                            elementToAdd.Text = "+";
                            elementToAdd.Appearance = oAppButton;

                            elementToAdd.ElementClick += this.RaiseAddClick;

                            rc = parent.RectInsideBorders;
                            rc.Width = C_WITH;
                            elementToAdd.Rect = rc;

                            foreach (UIElement child in parent.ChildElements)
                            {
                                childRect = child.Rect;
                                childRect.X += C_WITH + 1;
                                child.Rect = childRect;
                            }

                            parent.ChildElements.Add(elementToAdd);

                        }
                        else
                        {
                            foreach (UIElement child in parent.ChildElements)
                            {
                                childRect = child.Rect;
                                childRect.X += C_WITH + 1;
                                child.Rect = childRect;
                            }
                        }

                    }
                    else if (parent.GetType() == typeof(TreeNodeUIElement))
                    {
                        rc = parent.RectInsideBorders;
                        rc.Width += C_WITH;
                        parent.Rect = rc;
                    }

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        public bool BeforeCreateChildElements(UIElement parent)
        {
            return false;
        }

        #endregion

        #region Property

        public bool CartellaChiusa
        {
            get { return _cartellachiusa; }
        }

        #endregion

        #region Events

        public void RaiseAddClick(object sender, UIElementEventArgs e)
        {
            this.AddClick(sender, e);
        }

        #endregion

    }

    public class HeaderLongDateCreationFilter : IUIElementCreationFilter
    {

        #region Interface

        public void AfterCreateChildElements(UIElement parent)
        {

            try
            {

                if (parent.GetType() == typeof(TextUIElement) &&
                    (parent.Parent.GetType() == typeof(Infragistics.Win.UltraWinSchedule.DayView.DayHeaderUIElement) ||
                    parent.Parent.GetType() == typeof(Infragistics.Win.UltraWinSchedule.WeekView.DayNumberUIElement) ||
                    parent.Parent.GetType() == typeof(Infragistics.Win.UltraWinSchedule.MonthViewSingle.DayNumberUIElement)))
                {
                    TextUIElement textElement = (TextUIElement)parent;

                    if (parent.SelectableItem.GetType() == typeof(Day))
                    {
                        Day day = (Day)parent.SelectableItem;
                        int nMax = GetMassimali(day);
                        if (nMax > 0)
                        {
                            textElement.Text = string.Format("{0} ({1} di {2})", day.Date.ToString("ddd dd MMM yyyy"), day.Appointments.Count, nMax);
                        }
                        else
                        {
                            textElement.Text = string.Format("{0} ({1})", day.Date.ToString("dddd dd MMMM yyyy"), day.Appointments.Count);
                        }
                        textElement.ToolTipEnabled = true;
                    }
                    else if (parent.SelectableItem.GetType() == typeof(VisibleDay))
                    {
                        VisibleDay visibleday = (VisibleDay)parent.SelectableItem;

                        int nApp = ContaAppuntamenti(visibleday.Day.Appointments);

                        int nMax = GetMassimali(visibleday.Day);
                        if (nMax > 0)
                        {
                            visibleday.Text = string.Format("{0} ({1} di {3} {2})", visibleday.Date.ToString("ddd dd MMM yyyy"), nApp, (nApp == 1 ? "Appuntamento" : "Appuntamenti"), nMax);
                        }
                        else
                        {
                            visibleday.Text = string.Format("{0} ({1} {2})", visibleday.Date.ToLongDateString(), nApp, (nApp == 1 ? "Appuntamento" : "Appuntamenti"));
                        }
                        textElement.ToolTipEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }

        }

        public bool BeforeCreateChildElements(UIElement parent)
        {
            return false;
        }

        #endregion

        #region Private Methods

        private int ContaNote(AppointmentsSubsetCollection Apps)
        {
            int nret = 0;

            try
            {
                if (Apps != null)
                {
                    foreach (var app in Apps)
                    {
                        if (app.BindingListObject != null)
                        {
                            DataRowView dr = (DataRowView)app.BindingListObject;
                            nret += int.Parse(dr["FlagNota"].ToString()) == 1 ? 1 : 0;
                        }
                    }
                }
                else
                    nret = 0;
            }
            catch
            {
                nret = 0;
            }

            return nret;
        }

        private int ContaAppuntamenti(AppointmentsSubsetCollection Apps)
        {
            int nret = 0;

            try
            {
                if (Apps != null)
                {
                    foreach (var app in Apps)
                    {
                        if (app.BindingListObject != null)
                        {
                            DataRowView dr = (DataRowView)app.BindingListObject;
                            nret += int.Parse(dr["FlagNota"].ToString()) == 1 ? 0 : 1;
                        }
                    }
                }
                else
                    nret = 0;
            }
            catch
            {
                nret = 0;
            }

            return nret;
        }

        private int GetMassimali(Day day)
        {

            int nret = 0;

            try
            {

                string codagenda = day.CalendarInfo.Owners[1].Key;

                if (TableCache.IsInTableCache("T_Agende") == false)
                {
                    using (FwDataConnection conn = new FwDataConnection(Database.ConnectionString))
                    {
                        FwDataBufferedList<T_AgendeRow> result = conn.Query<FwDataBufferedList<T_AgendeRow>>("Select * From T_Agende", null, CommandType.Text);
                        List<object> list = result.Buffer.ToList<object>();
                        TableCache.AddToCache("T_Agende", list);
                    }
                }

                T_AgendeRow row = TableCache.GetCachedRow<T_AgendeRow>("T_Agende", (x => x.Codice == codagenda));

                if (row.Risorse != null && row.Risorse != string.Empty)
                {

                    MassimaliAgenda MassimaliAgenda = XmlProcs.XmlDeserializeFromString<MassimaliAgenda>(row.Risorse);
                    if (MassimaliAgenda.Massimale[(int)day.Date.DayOfWeek] > 0)
                    {
                        nret = MassimaliAgenda.Massimale[(int)day.Date.DayOfWeek];
                    }

                }

            }
            catch
            {
                nret = 0;
            }

            return nret;

        }

        #endregion

    }

    public class HeaderToolTipCreationFilter : IUIElementCreationFilter
    {

        #region Interface

        public void AfterCreateChildElements(UIElement parent)
        {

            try
            {

                if (parent.GetType() == typeof(TextUIElement) &&
                    (parent.Parent.GetType() == typeof(Infragistics.Win.UltraWinSchedule.DayView.OwnerHeaderUIElement)))
                {

                    TextUIElement textElement = (TextUIElement)parent;
                    textElement.ToolTipItem = new MyToolTipItem();

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        public bool BeforeCreateChildElements(UIElement parent)
        {
            return false;
        }

        #endregion

    }

    public class MyToolTipItem : IToolTipItem
    {

        public MyToolTipItem()
        {
        }

        public ToolTipInfo GetToolTipInfo(Point mousePosition, UIElement element, UIElement previousToolTipElement, ToolTipInfo toolTipInfoDefault)
        {

            if (element.ToolTipItem != null)
            {
                toolTipInfoDefault.ToolTipText = ((TextUIElement)element).Text;
            }

            return toolTipInfoDefault;

        }

    }

    #endregion

}

