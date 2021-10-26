using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace UnicodeSrl.ScciCore.CustomControls.Infra
{
    [System.Security.SuppressUnmanagedCodeSecurityAttribute()]
    internal class NativeWindowMethods
    {
        [DllImport("gdi32", SetLastError = true)]
        internal static extern int SelectClipRgn(IntPtr hdc, IntPtr hRgn);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        internal static extern int SaveDC(IntPtr hdc);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        internal static extern int RestoreDC(IntPtr hdc, int savedState);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("gdi32", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        internal static extern bool DeleteObject(IntPtr hObject);
    }
}
