using System;
using System.Runtime.InteropServices;

namespace moe.futa.akizuki.cqbridge
{
    public class HostLogLevel
    {
        public const Int32 DEBUG = 0;
        public const Int32 INFO = 10;
        public const Int32 INFO_SUCCESS = 11;
        public const Int32 INFO_RECV = 12;
        public const Int32 INFO_SEND = 13;
        public const Int32 WARNING = 20;
        public const Int32 ERROR = 30;
        public const Int32 FATAL = 40;
    }

    public class Host
    {
        private const String LIBRARY_NAME = "cqp.dll";

        // CQAPI(int32_t) CQ_addLog(int32_t AuthCode, int32_t priority, const char* category, const char* content);
        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "CQ_addLog")]
        public static extern Int32 AppendLog(Int32 authCode, Int32 priority, String category, String content);
    }
}
