using System;
using System.Collections.Generic;
using System.Linq;
using ZeroMQ;
using ZeroMQ.Monitoring;

namespace moe.futa.akizuki.cqbridge
{
    public class Server
    {
        private static ZContext _context;
        private static readonly List<ZMonitor> Monitors = new List<ZMonitor>();

        private readonly ZSocket _socket;

        protected Server(ZSocketType type, String endpoint)
        {
            _socket = new ZSocket(Context, type);
            InstallMonitor();
            _socket.Bind(endpoint);
        }

        private static ZContext Context => _context ?? (_context = new ZContext());

        private void InstallMonitor()
        {
            var address = "inproc://monitor." + Guid.NewGuid();
            _socket.Monitor(address);
            var monitor = ZMonitor.Create(Context, address);
            Monitors.Add(monitor);

            monitor.Accepted += OnAccepted;
            monitor.BindFailed += OnBindFailed;
            monitor.Listening += OnListening;

            monitor.Start();
        }

        private void OnAccepted(Object sender, ZMonitorFileDescriptorEventArgs e)
        {
            Host.AppendLog(HostClient.AuthCode, HostLogLevel.Info, GetType().Name, "Accepted connection");
        }

        private void OnBindFailed(Object sender, ZMonitorEventArgs e)
        {
            Host.AppendLog(HostClient.AuthCode, HostLogLevel.Fatal, GetType().Name, "Failed to bind address");
        }

        private void OnListening(Object sender, ZMonitorFileDescriptorEventArgs e)
        {
            Host.AppendLog(HostClient.AuthCode, HostLogLevel.Info, GetType().Name, "Entered listening state");
        }

        protected void SendTopicMessage(String topic, String content)
        {
            using (var message = new ZMessage(new List<String>
            {
                topic,
                content
            }.Select(s => new ZFrame(s))))
            {
                _socket.SendMessage(message);
            }
        }

        public static void Shutdown()
        {
            Monitors.ForEach(monitor =>
            {
                // the following is necessary since Dispose is not properly implemented
                monitor.Close();
                monitor.Join();
            });
            Context.Shutdown();
        }
    }
}