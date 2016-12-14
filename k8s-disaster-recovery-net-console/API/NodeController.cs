using System;
using System.IO;
using System.Net;
using k8s_disaster_recovery_net_console.Model;
using ServiceStack.Text;

namespace k8s_disaster_recovery_net_console.API
{
    public class NodeController : IController
    {
        public object Response
        {
            get
            {
                string nodes;
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    try
                    {
                        var wc = new WebClient();
                        nodes = wc.DownloadString("http://localhost:8080/api/v1/nodes");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        throw;
                    }
                }
                else
                {
                    nodes = File.ReadAllText(@"nodes.json");
                }

                var nodesObj = JsonSerializer.DeserializeFromString(nodes, typeof(NodesModel.Nodes));
                return nodesObj;
            }
        }

        public string ContentType => "application/json";
    }
}
