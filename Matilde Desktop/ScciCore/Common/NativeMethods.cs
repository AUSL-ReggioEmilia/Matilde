using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnicodeSrl.ScciCore
{
    public static class NativeMethods
    {
        public static readonly IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;
        public const int WTS_CURRENT_SESSION = -1;

        public enum WTS_INFO_CLASS
        {
            WTSInitialProgram = 0,
            WTSApplicationName = 1,
            WTSWorkingDirectory = 2,
            WTSOEMId = 3,
            WTSSessionId = 4,
            WTSUserName = 5,
            WTSWinStationName = 6,
            WTSDomainName = 7,
            WTSConnectState = 8,
            WTSClientBuildNumber = 9,
            WTSClientName = 10,
            WTSClientDirectory = 11,
            WTSClientProductId = 12,
            WTSClientHardwareId = 13,
            WTSClientAddress = 14,
            WTSClientDisplay = 15,
            WTSClientProtocolType = 16,
            WTSIdleTime = 17,
            WTSLogonTime = 18,
            WTSIncomingBytes = 19,
            WTSOutgoingBytes = 20,
            WTSIncomingFrames = 21,
            WTSOutgoingFrames = 22,
            WTSClientInfo = 23,
            WTSSessionInfo = 24,
            WTSSessionInfoEx = 25,
            WTSConfigInfo = 26,
            WTSValidationInfo = 27,
            WTSSessionAddressV4 = 28,
            WTSIsRemoteSession = 29
        }

        [DllImport("Wtsapi32.dll", CharSet = CharSet.Unicode)]
        public static extern bool WTSQuerySessionInformation(
            IntPtr hServer,
            Int32 sessionId,
            WTS_INFO_CLASS wtsInfoClass,
            out IntPtr ppBuffer,
            out Int32 pBytesReturned);

        [DllImport("wtsapi32.dll", ExactSpelling = true, SetLastError = false)]
        public static extern void WTSFreeMemory(IntPtr memory);

        [StructLayout(LayoutKind.Sequential)]
        public struct WTS_CLIENT_ADDRESS
        {
            public uint AddressFamily;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] Address;
        }

        public const int OS_ANYSERVER = 29;
        [DllImport("shlwapi.dll", SetLastError = true, EntryPoint = "#437")]
        public static extern bool IsOS(int os);


    }
}
