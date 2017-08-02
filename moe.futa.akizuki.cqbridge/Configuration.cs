using System.IO;
using Nett;

namespace moe.futa.akizuki.cqbridge
{
    internal class Configuration
    {
        private static Configuration _instance;

        public static Configuration GetInstance()
        {
            return _instance ?? (_instance = Toml.ReadFile<Configuration>(Path.Combine(
                       Host.GetHostPath(HostClient.AuthCode), "config.toml")));
        }
    }
}
