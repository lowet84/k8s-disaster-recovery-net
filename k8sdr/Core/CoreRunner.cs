using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using k8sdr.Model;
using Renci.SshNet;
using ServiceStack.Text;

namespace k8sdr.Core
{
    public class CoreRunner
    {
        public void Run()
        {
            while (true)
            {
                try
                {
                    if (Utils.LockedForMigration)
                    {
                        while (true)
                        {
                            Thread.Sleep(10000);
                        }
                    }
                    else if (Utils.PrivateKey != null)
                    {
                        var result = Utils.RunCommands(false, "kubectl get nodes -o json");
                        var nodes = JsonSerializer.DeserializeFromString<NodesModel.Nodes>(result[0]);
                        if (nodes.Items == null || nodes.Items.Count == 0)
                        {
                            Console.WriteLine("No nodes found");
                        }
                        else
                        {
                            Console.WriteLine("Saving node configutaion");
                            Utils.Nodes = nodes;
                        }
                    }
                    else
                    {
                        Console.WriteLine("No private key set.");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                Console.WriteLine("Sleeping...");
                Thread.Sleep(30000);
            }
        }
    }
}
