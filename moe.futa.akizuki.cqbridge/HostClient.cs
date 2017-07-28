using System;
using System.Runtime.InteropServices;

namespace moe.futa.akizuki.cqbridge
{
    public class HostClient
    {
        private const String APPID = "moe.futa.akizuki.cqbridge";

        private const Int32 EVENT_IGNORED = 0;
        private const Int32 EVENT_BLOCK = 1;

        public static Int32 AuthCode;

        // CQEVENT(const char*, AppInfo, 0)()
        [DllExport("AppInfo", CallingConvention.StdCall)]
        public static String GetAppInfo()
        {
            return "9," + APPID;
        }

        // CQEVENT(int32_t, __eventStartup, 0)()
        [DllExport("OnHostStartup", CallingConvention.StdCall)]
        public static Int32 Initialize()
        {
            Host.AppendLog(AuthCode, HostLogLevel.INFO, typeof(HostClient).Name, "Akizuki.CQBridge initialized.");
            return 0;
        }

        // CQEVENT(int32_t, Initialize, 4)(int32_t AuthCode)
        [DllExport("Initialize", CallingConvention.StdCall)]
        public static Int32 SetAuthCode(Int32 authCode)
        {
            AuthCode = authCode;
            return 0;
        }
    }
}
