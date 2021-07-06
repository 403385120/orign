using System;
using System.Runtime.InteropServices;

namespace ZYXray.Models
{
    public enum WTSInfoClass
    {
        WTSInitialProgram,
        WTSApplicationName,
        WTSWorkingDirectory,
        WTSOEMId,
        WTSSessionId,
        WTSUserName,
        WTSWinStationName,
        WTSDomainName,
        WTSConnectState,
        WTSClientBuildNumber,
        WTSClientName,
        WTSClientDirectory,
        WTSClientProductId,
        WTSClientHardwareId,
        WTSClientAddress,
        WTSClientDisplay,
        WTSClientProtocolType,
        WTSIdleTime,
        WTSLogonTime,
        WTSIncomingBytes,
        WTSOutgoingBytes,
        WTSIncomingFrames,
        WTSOutgoingFrames,
        WTSClientInfo,
        WTSSessionInfo
    }

    public class WinAPI
    {
        [DllImport("Wtsapi32.dll")]
        protected static extern void WTSFreeMemory(IntPtr pointer);

        [DllImport("Wtsapi32.dll")]
        protected static extern bool WTSQuerySessionInformation(IntPtr hServer, int sessionId, WTSInfoClass wtsInfoClass, out IntPtr ppBuffer, out uint pBytesReturned);


        /// <summary>
        /// 获取当前登录用户(可用于管理员身份运行)
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentUser()
        {
            IntPtr buffer;
            uint strLen;
            int cur_session = -1;
            var username = "SYSTEM"; // assume SYSTEM as this will return "\0" below
            if (WTSQuerySessionInformation(IntPtr.Zero, cur_session, WTSInfoClass.WTSUserName, out buffer, out strLen) && strLen > 1)
            {
                username = Marshal.PtrToStringAnsi(buffer); // don't need length as these are null terminated strings
                WTSFreeMemory(buffer);
                if (WTSQuerySessionInformation(IntPtr.Zero, cur_session, WTSInfoClass.WTSDomainName, out buffer, out strLen) && strLen > 1)
                {
                    username = Marshal.PtrToStringAnsi(buffer) + "\\" + username; // prepend domain name
                    WTSFreeMemory(buffer);
                }
            }
            return username;
        }
    }
}
