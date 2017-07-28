using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace moe.futa.akizuki.cqbridge
{
    public class HostClient
    {
        private const String APPID = "moe.futa.akizuki.cqbridge";
        private const String STATUS_TRIGGER = "0cba061c25c86424187f08b0344c13d1";

        private const Int32 EVENT_IGNORED = 0;
        private const Int32 EVENT_BLOCK = 1;

        private static DateTime StartupTime;

        public static Int32 AuthCode;

        // CQEVENT(const char*, AppInfo, 0)()
        [DllExport("AppInfo", CallingConvention.StdCall)]
        public static String GetAppInfo()
        {
            return "9," + APPID;
        }

        private static String GetStatusString()
        {
            return
                typeof(HostClient).FullName + "\n" +
                "- hash " + Assembly.GetExecutingAssembly().GetHashCode().ToString("X") + "\n" + 
                "- host " + Environment.MachineName + "\n" +
                "- system " + Environment.OSVersion.VersionString + "\n" +
                "- uptime " + (DateTime.Now - StartupTime);
        }

        // CQEVENT(int32_t, __eventStartup, 0)()
        [DllExport("OnHostStartup", CallingConvention.StdCall)]
        public static Int32 Initialize()
        {
            StartupTime = DateTime.Now;
            Host.AppendLog(AuthCode, HostLogLevel.INFO, typeof(HostClient).Name, "Akizuki.CQBridge initialized.");
            return 0;
        }

        // CQEVENT(int32_t, __eventGroupMsg, 36)(int32_t subType, int32_t sendTime, int64_t fromGroup, int64_t fromQQ, const char *fromAnonymous, const char *msg, int32_t font)
        [DllExport("OnGroupMessage", CallingConvention.StdCall)]
        public static Int32 OnGroupMessage(Int32 subType, Int32 time, Int64 groupId, Int64 userId, String anonymousNick, String message, Int32 font)
        {
            if (message == STATUS_TRIGGER)
                Host.SendGroupMessage(AuthCode, groupId, GetStatusString());
            return EVENT_IGNORED;
        }

        // CQEVENT(int32_t, __eventPrivateMsg, 24)(int32_t subType, int32_t sendTime, int64_t fromQQ, const char *msg, int32_t font)
        [DllExport("OnPrivateMessage", CallingConvention.StdCall)]
        public static Int32 OnPrivateMessage(Int32 subType, Int32 time, Int64 userId, String message, Int32 font)
        {
            if (message == STATUS_TRIGGER)
                Host.SendPrivateMessage(AuthCode, userId, GetStatusString());
            return EVENT_IGNORED;
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
