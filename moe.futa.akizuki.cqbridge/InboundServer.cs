using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using ZeroMQ;

namespace moe.futa.akizuki.cqbridge
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class InboundHandler : Attribute
    {
        public readonly String Topic;

        public InboundHandler(String topic)
        {
            Topic = topic;
        }
    }

    internal class InboundServer : Server
    {
        private static Dictionary<String, Action<ZMessage>> _handlers;

        public InboundServer(String endpoint) : base(ZSocketType.SUB, endpoint)
        {
            _handlers = GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(method => method.IsDefined(typeof(InboundHandler)))
                .ToDictionary(
                    method => method.GetCustomAttribute<InboundHandler>().Topic,
                    method => (Action<ZMessage>) Delegate.CreateDelegate(typeof(Action<ZMessage>), this, method)
                );
            Socket.SubscribeAll();
            new Thread(RetrieveMessages).Start();
        }

        [InboundHandler("message.group")]
        private void OnGroupMessage(ZMessage message)
        {
            Host.SendGroupMessage(HostClient.AuthCode, Convert.ToInt64(message[1].ReadString()),
                message[2].ReadString());
        }

        [InboundHandler("message.private")]
        private void OnPrivateMessage(ZMessage message)
        {
            Host.SendPrivateMessage(HostClient.AuthCode, Convert.ToInt64(message[1].ReadString()),
                message[2].ReadString());
        }

        public void RetrieveMessages()
        {
            while (true)
                using (var message = Socket.ReceiveMessage(out ZError error))
                {
                    if (Equals(error, ZError.ETERM))
                        // context shutdown
                        return;
                    var topic = message[0].ReadString();
                    if (_handlers.ContainsKey(topic))
                        _handlers[topic](message);
                    else
                        Host.AppendLog(HostClient.AuthCode, HostLogLevel.Error, GetType().Name,
                            $"unregistered topic {topic}");
                }
        }
    }
}