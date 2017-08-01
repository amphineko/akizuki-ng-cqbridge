using System;
using System.Reflection;
using System.Runtime.InteropServices;
using moe.futa.akizuki.cqbridge.Messages;

namespace moe.futa.akizuki.cqbridge
{
    public class HostClient
    {
        private const String Appid = "moe.futa.akizuki.cqbridge";
        private const String StatusTrigger = "0cba061c25c86424187f08b0344c13d1";

        private const Int32 EventHandled = 1;
        private const Int32 EventIgnored = 0;

        private static DateTime _startupTime;
        private static OutboundServer _outbound;

        public static Int32 AuthCode;

        // CQEVENT(const char*, AppInfo, 0)()
        [DllExport("AppInfo", CallingConvention.StdCall)]
        public static String GetAppInfo()
        {
            return "9," + Appid;
        }

        private static String GetStatusString()
        {
            return $@"{typeof(HostClient).FullName}
- active subscribers {_outbound.ConnectionCount}
- hash {Assembly.GetExecutingAssembly().GetHashCode():X}
- host {Environment.MachineName}
- system {Environment.OSVersion.VersionString}
- uptime {DateTime.Now - _startupTime}";
        }

        // CQEVENT(int32_t, __eventStartup, 0)()
        [DllExport("OnHostStartup", CallingConvention.StdCall)]
        public static Int32 Initialize()
        {
            Host.AppendLog(AuthCode, HostLogLevel.Info, typeof(HostClient).Name, "Akizuki.CQBridge initializing.");
            _outbound = new OutboundServer("tcp://0.0.0.0:11451");
            _startupTime = DateTime.Now;
            Host.AppendLog(AuthCode, HostLogLevel.Info, typeof(HostClient).Name, "Akizuki.CQBridge initialized.");
            return 0;
        }

        // CQEVENT(int32_t, __eventGroupMsg, 36)(int32_t subType, int32_t sendTime, int64_t fromGroup, int64_t fromQQ, const char *fromAnonymous, const char *msg, int32_t font)
        [DllExport("OnGroupMessage", CallingConvention.StdCall)]
        public static Int32 OnGroupMessage(Int32 subType, Int32 time, Int64 groupId, Int64 userId, String anonymousNick,
            String message, Int32 font)
        {
            if (anonymousNick != "")
                return EventIgnored;
            if (message == StatusTrigger)
                Host.SendGroupMessage(AuthCode, groupId, GetStatusString());
            _outbound.PushGroupMessage(new GroupMessage(message, groupId.ToString(), time, userId.ToString()));
            return EventHandled;
        }

        // CQEVENT(int32_t, __eventPrivateMsg, 24)(int32_t subType, int32_t sendTime, int64_t fromQQ, const char *msg, int32_t font)
        [DllExport("OnPrivateMessage", CallingConvention.StdCall)]
        public static Int32 OnPrivateMessage(Int32 subType, Int32 time, Int64 userId, String message, Int32 font)
        {
            if (message == StatusTrigger)
                Host.SendPrivateMessage(AuthCode, userId, GetStatusString());
            _outbound.PushPrivateMessage(new PrivateMessage(message, time, userId.ToString()));
            return EventHandled;
        }

        // CQEVENT(int32_t, Initialize, 4)(int32_t AuthCode)
        [DllExport("Initialize", CallingConvention.StdCall)]
        public static Int32 SetAuthCode(Int32 authCode)
        {
            AuthCode = authCode;
            return 0;
        }

        // CQEVENT(int32_t, __eventExit, 0)()
        [DllExport("OnHostExit", CallingConvention.StdCall)]
        public static Int32 Shutdown()
        {
            Server.Shutdown();
            return 0;
        }
    }
}