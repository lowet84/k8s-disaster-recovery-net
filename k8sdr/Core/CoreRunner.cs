using System;
using System.Collections.Generic;
using System.Threading;
using k8sdr.Model;
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
                    if (string.IsNullOrEmpty(Utils.MasterUrl))
                    {
                        Console.WriteLine("No Master url set.");
                    }
                    else if (Utils.Armed)
                    {
                        Console.WriteLine("Migrator is armed, not running updater.");
                    }
                    else if((Utils.ResetState != ResetState.ReadyToRestoreMaster))
                    {
                        Console.WriteLine("Migration is ongoing, not running updater.");
                    }
                    else if (Utils.PrivateKey == null)
                    {
                        Console.WriteLine("No private key set.");
                    }
                    else
                    {
                        var result = Utils.RunCommands(
                            false,
                            "kubectl get nodes -o json",
                            "kubectl get pv -o json",
                            "kubectl get namespaces -o json",
                            "kubectl get --all-namespaces pvc -o json",
                            "kubectl get deploy --all-namespaces -o json",
                            "kubectl get svc --all-namespaces -o json",
                            "kubectl get svc --all-namespaces -o json");

                        var nodes = JsonSerializer.DeserializeFromString<NodesModel.Nodes>(result[0]);
                        var volumes = JsonSerializer.DeserializeFromString<VolumesModel.Volumes>(result[1]);
                        var namespaces = JsonSerializer.DeserializeFromString<NamespacesModel.Namespaces>(result[2]);
                        var claims = JsonSerializer.DeserializeFromString<VolumeClaimsModel.VolumeClaims>(result[3]);
                        var deploys = JsonSerializer.DeserializeFromString<DeployModel.Deployments>(result[4]);
                        var services = JsonSerializer.DeserializeFromString<ServiceModel.Services>(result[5]);
                        var ingresses = JsonSerializer.DeserializeFromString<IngressModel.Ingresses>(result[6]);

                        TrySetValue(nodes.Items, "nodes", () => Utils.Nodes = nodes);
                        TrySetValue(volumes.items, "volumes", () => Utils.Volumes = volumes);
                        TrySetValue(namespaces.items, "namespaces", () => Utils.Namespaces = namespaces);
                        TrySetValue(claims.items, "claims", () => Utils.VolumeClaims = claims);
                        TrySetValue(deploys.items, "deploys", () => Utils.Deployments = deploys);
                        TrySetValue(services.items, "services", () => Utils.Services = services);
                        TrySetValue(ingresses.items, "ingresses", () => Utils.Ingresses = ingresses);
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
                Console.WriteLine($"Saving {name} configuration");
                action();
            }
        }
    }
}
