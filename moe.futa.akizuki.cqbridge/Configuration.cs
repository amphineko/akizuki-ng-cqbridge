using System;
using System.IO;
using Nett;

namespace moe.futa.akizuki.cqbridge
{
    internal class Configuration
    {
        public ServerConfiguration Server { get; set; }

        private static Configuration _instance;

        public static Configuration GetInstance()
        {
            return _instance ?? (_instance = Toml.ReadFile<Configuration>(Path.Combine(
                       Host.GetHostPath(HostClient.AuthCode), "config.toml")));
        }
    }

    internal class ServerConfiguration
    {
        public String InboundEndpoint { get; set; }
        public String OutboundEndpoint { get; set; }
    }
}
