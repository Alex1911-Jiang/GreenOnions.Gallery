using Consul;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GreenOnions.Gallery.Common
{
    public static class ConsulHelper
    {
        public static void ConsulRegist( this IConfiguration configuration, string groupName)
        {
            ConsulClient client = new(c =>
            {
                c.Address = new Uri("http://127.0.0.1:8500/");
                c.Datacenter = "dcl";
            });
            string ip = configuration["ip"] ?? "127.0.0.1";

            string iDebugPort = groupName switch
            {
                "GreenOnionsAuthentication" => "9100",
                "GreenOnionsApi" => "9200",
                "GreenOnionsGallery" => "9300",
                _ => "1911"
            };
            int port = int.Parse(configuration["port"] ?? iDebugPort);

            Console.WriteLine($"服务成功启动在:{port}端口");

            client.Agent.ServiceRegister(new AgentServiceRegistration()
            {
                ID = "service" + Guid.NewGuid(),
                Name = groupName,
                Address = ip,
                Port = port,

                //心跳检查
                Check = new AgentServiceCheck()
                {
                    Interval = TimeSpan.FromSeconds(12),
                    HTTP = $"http://{ip}:{port}/api/Health",
                    Timeout = TimeSpan.FromSeconds(5),
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5)
                }
            });
        }

        public static AgentService FindService(string groupName, string consulUrl = "http://127.0.0.1:8500/")
        {
            using ConsulClient client = new(c =>
            {
                c.Address = new Uri(consulUrl);
                c.Datacenter = "dcl";
            });
            Dictionary<string, AgentService> dictionary = client.Agent.Services().Result.Response;
            KeyValuePair<string, AgentService>[] list = dictionary.Where(k => k.Value.Service.Equals(groupName, StringComparison.OrdinalIgnoreCase)).ToArray();

            AgentService service = list[new Random(Guid.NewGuid().GetHashCode()).Next(0, list.Length)].Value;

            return service;
        }
    }
}
