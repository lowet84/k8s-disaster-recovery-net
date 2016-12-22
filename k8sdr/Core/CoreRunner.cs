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
                    if (Utils.Armed)
                    {
                        Console.WriteLine("Migrator is armed, not running updater.");
                    }
                    else if((Utils.ResetState != ResetState.ReadyToRestoreMaster))
                    {
                        Console.WriteLine("Migration is ongoing, not running updater.");
                    }
                    else if (Utils.PrivateKey != null)
                    {
                        var result = Utils.RunCommands(
                            false, 
                            "kubectl get nodes -o json", 
                            "kubectl get pv -o json",
                            "kubectl get namespaces -o json",
                            "kubectl get --all-namespaces pvc -o json");
                        var nodes = JsonSerializer.DeserializeFromString<NodesModel.Nodes>(result[0]);
                        var volumes = JsonSerializer.DeserializeFromString<VolumesModel.Volumes>(result[1]);
                        var namespaces = JsonSerializer.DeserializeFromString<NamespacesModel.Namespaces>(result[2]);
                        var claims = JsonSerializer.DeserializeFromString<VolumeClaimsModel.VolumeClaims>(result[3]);
                        TrySetValue(nodes.Items,"nodes",()=>Utils.Nodes=nodes);
                        TrySetValue(volumes.items,"volumes",()=>Utils.Volumes=volumes);
                        TrySetValue(namespaces.items,"namespaces",()=>Utils.Namespaces=namespaces);
                        TrySetValue(claims.items,"claims",()=>Utils.VolumeClaims=claims);
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

        private static void TrySetValue<T>(List<T> items, string name, Action action)
        {
            if (items == null || items.Count == 0)
            {
                Console.WriteLine($"No {name} found");
            }
            else
            {
                Console.WriteLine($"Saving {name} configutaion");
                action();
            }
        }
    }
}
