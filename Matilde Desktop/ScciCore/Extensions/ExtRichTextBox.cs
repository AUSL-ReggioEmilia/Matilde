using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace UnicodeSrl.ScciCore.Extensions
{
    public static class ExtRichTextBox
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public Int32 left;
            public Int32 top;
            public Int32 right;
            public Int32 bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SCROLLBARINFO
        {
            public Int32 cbSize;
            public RECT rcScrollBar;
            public Int32 dxyLineButton;
            public Int32 xyThumbTop;
            public Int32 xyThumbBottom;
            public Int32 reserved;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public Int32[] rgstate;
        }

        private static UInt32 SB_VERT = 1;
        private static UInt32 OBJID_VSCROLL = 0xFFFFFFFB;

        [DllImport("user32.dll")]
        private static extern
            Int32 GetScrollRange(IntPtr hWnd, UInt32 nBar, out Int32 lpMinPos, out Int32 lpMaxPos);

        [DllImport("user32.dll")]
        private static extern
            Int32 GetScrollBarInfo(IntPtr hWnd, UInt32 idObject, ref SCROLLBARINFO psbi);

        public static int CalculateRichTextHeight(this RichTextBox richTextBox)
        {
            int height = 0;
            richTextBox.WordWrap = true;

            int nMin = 0, nMax = 0;

            SCROLLBARINFO psbi = new SCROLLBARINFO();
            psbi.cbSize = Marshal.SizeOf(psbi);

            richTextBox.ScrollBars = RichTextBoxScrollBars.Vertical;

            int nResult = GetScrollBarInfo(richTextBox.Handle, OBJID_VSCROLL, ref psbi);
            if (psbi.rgstate[0] == 0)
            {
                GetScrollRange(richTextBox.Handle, SB_VERT, out nMin, out nMax);
                height = (nMax - nMin);
            }

            return height;
        }

        public static bool IsValidRtfText(string text)
        {
            try
            {
                using (RichTextBox rtb = new RichTextBox())
                {
                    rtb.Rtf = text;
                }
            }
            catch (ArgumentException)
            {
                return false;

            }

            return true;
        }

    }
}
