using System;
using System.Runtime.InteropServices;

namespace moe.futa.akizuki.cqbridge
{
    internal class HostLogLevel
    {
        public const Int32 Debug = 0;
        public const Int32 Info = 10;
        public const Int32 InfoSuccess = 11;
        public const Int32 InfoRecv = 12;
        public const Int32 InfoSend = 13;
        public const Int32 Warning = 20;
        public const Int32 Error = 30;
        public const Int32 Fatal = 40;
    }

    internal class Host
    {
        private const String LibraryName = "cqp.dll";

        // CQAPI(int32_t) CQ_addLog(int32_t AuthCode, int32_t priority, const char* category, const char* content);
        [DllImport(LibraryName, CallingConvention = CallingConvention.StdCall, EntryPoint = "CQ_addLog")]
        public static extern Int32 AppendLog(Int32 authCode, Int32 priority, String category, String content);

        // CQAPI(const char *) CQ_getAppDirectory(int32_t AuthCode);
        [DllImport(LibraryName, CallingConvention = CallingConvention.StdCall, EntryPoint = "CQ_getAppDirectory")]
        public static extern String GetHostPath(Int32 authCode);

        // CQAPI(int32_t) CQ_sendGroupMsg(int32_t AuthCode, int64_t groupid, const char *msg);
        [DllImport(LibraryName, CallingConvention = CallingConvention.StdCall, EntryPoint = "CQ_sendGroupMsg")]
        public static extern Int32 SendGroupMessage(Int32 authCode, Int64 groupId, String message);

        // CQAPI(int32_t) CQ_sendPrivateMsg(int32_t AuthCode, int64_t QQID, const char *msg);
        [DllImport(LibraryName, CallingConvention = CallingConvention.StdCall, EntryPoint = "CQ_sendPrivateMsg")]
        public static extern Int32 SendPrivateMessage(Int32 authCode, Int64 userId, String message);
    }
}