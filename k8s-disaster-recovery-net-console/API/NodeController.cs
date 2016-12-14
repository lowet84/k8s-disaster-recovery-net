using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using ServiceStack.Text;

namespace k8s_disaster_recovery_net_console.API
{
    public class NodeController : IController
    {
        public object Response
        {
            get
            {
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    try
                    {
                        var wc = new WebClient();
                        var data = wc.DownloadString("http://localhost:8080/api/v1/nodes");
                        File.WriteAllText(@"test.json",data);
                        return data;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        throw e;
                    }
                }
                else
                {
                    return new {Name = "Windows Dummy"};
                }
            }
        }

        public string ContentType => "application/json";
    }
}
