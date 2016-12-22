using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using k8sdr.Model;

namespace k8sdr.Core
{
    public class Migrator
    {
        public static void SetUpMasterNode(bool master)
        {
            Console.WriteLine(master ? "Resetting master" : "Upgrading reserve to Master");
            var host = master ? Utils.MasterUrl : Utils.Nodes.Items.FirstOrDefault(d => d.Metadata?.Labels?.Role == "reserve").Status.Addresses.First().Address;
            var token = RandomString(6) + "." + RandomString(16);
            Utils.Token = token;
            var commands = new[]
            {
                "kubeadm reset",
                "systemctl start kubelet",
                $"kubeadm init --token={token}",
                "kubectl apply -f https://git.io/weave-kube"
            };
            Utils.RunCommands(host, true, commands);
        }

        public static void ConnectNodesToMaster(bool master)
        {
            var token = Utils.Token;
            var newMaster =
                Utils.Nodes.Items.FirstOrDefault(d => d.Metadata.Labels.Role == (master ? "master" : "reserve"));
            if (newMaster == null)
            {
                return;
            }
            var otherNodes = Utils.Nodes.Items.Where(d => d.Metadata.Name != newMaster.Metadata.Name).ToList();

            Console.WriteLine("Connecting nodes to new master");
            var tasks = new List<Task>();
            foreach (var node in otherNodes)
            {
                var task = new Task(() =>
                {
                    var commands = new[]
                    {
                        "kubeadm reset",
                        "systemctl start kubelet",
                        $"kubeadm join --token={token} {newMaster.Status.Addresses.First().Address}"
                    };
                    Utils.RunCommands(node.Status.Addresses.First().Address, true, commands);
                });
                tasks.Add(task);
                task.Start();
            }

            while (tasks.Any(d => !d.IsCompleted))
            {
            }
        }

        public static void SetLabels(bool master)
        {
            Console.WriteLine("Setting labels.");

            var newMaster = Utils.Nodes.Items.FirstOrDefault(d => d.Metadata.Labels.Role == (master ? "master" : "reserve"));
            if (newMaster == null)
            {
                return;
            }

            var labelCommands = new List<string>();
            foreach (var node in Utils.Nodes.Items)
            {
                if (node.Metadata.Labels.Role == "master")
                {
                    labelCommands.AddRange(new[]
                    {
                        $"kubectl label nodes {node.Metadata.Name} role=master"
                    });
                }
                if (node.Metadata.Labels.Role == "storage")
                {
                    labelCommands.AddRange(new[]
                    {
                        $"kubectl taint nodes {node.Metadata.Name} dedicated=storage:NoSchedule",
                        $"kubectl label nodes {node.Metadata.Name} role=storage"
                    });
                }
                else if ((node.Metadata.Labels.Role == "reserve" && master) || (node.Metadata.Labels.Role == "master" && !master))
                {
                    labelCommands.AddRange(new[]
                    {
                        $"kubectl taint nodes {node.Metadata.Name} dedicated=reserve:NoSchedule",
                        $"kubectl label nodes {node.Metadata.Name} role=reserve"
                    });
                }
            }
            Utils.RunCommands(newMaster.Status.Addresses.First().Address, true, labelCommands.ToArray());
        }

        public static void StartCoreServices(bool master)
        {
            Console.WriteLine("Starting core services");

            var newMaster = Utils.Nodes.Items.FirstOrDefault(d => d.Metadata.Labels.Role == (master ? "master" : "reserve"));
            if (newMaster == null || string.IsNullOrEmpty(Utils.Domain))
            {
                return;
            }

            const string traefikYamlUrl = "https://raw.githubusercontent.com/lowet84/k8s-config/master/traefik-kube/traefik.yml";
            var traefikYaml = new WebClient().DownloadString(traefikYamlUrl);
            traefikYaml = traefikYaml.Replace("traefik.kube", $"traefik.{Utils.Domain}");

            const string dashYamlUrl = "https://raw.githubusercontent.com/lowet84/k8s-config/master/dashboard/kubernetes-dashboard.yaml";
            var dashYaml = new WebClient().DownloadString(dashYamlUrl);
            dashYaml = dashYaml.Replace("kubernetes.kube", $"kubernetes.{Utils.Domain}");

            var commands = new[]
            {
                $@"echo '{traefikYaml}' | kubectl apply -f -",
                $@"echo '{dashYaml}' | kubectl apply -f -",
                "docker run -d -it --restart always --name nginx-kube-proxy --net=host lowet84/nginx-kube-proxy"
            };
            Utils.RunCommands(newMaster.Status.Addresses.First().Address, true, commands);
        }

        public static void StartLizardfs(bool master)
        {
            Console.WriteLine("Starting lizardfs");

            var newMaster = Utils.Nodes.Items.FirstOrDefault(d => d.Metadata.Labels.Role == (master ? "master" : "reserve"));
            if (newMaster == null || string.IsNullOrEmpty(Utils.Domain))
            {
                return;
            }

            const string lizardfsYamlUrl = "https://raw.githubusercontent.com/lowet84/k8s-config/master/lizardfs/lizardfs.yml";
            var lizardfsYaml = new WebClient().DownloadString(lizardfsYamlUrl);
            lizardfsYaml = lizardfsYaml.Replace("lizardfs.kube", $"lizardfs.{Utils.Domain}");
            //var chunkYaml = new WebClient().DownloadString("https://raw.githubusercontent.com/lowet84/k8s-config/master/lizardfs/chunk.yml");
            var lizardFsCommands = new[]
            {
                "kubectl create namespace lizardfs",
                $@"echo '{lizardfsYaml}' | kubectl apply -f -",
                //$@"echo '{chunkYaml}' | kubectl apply -f -"
            };
            Utils.RunCommands(newMaster.Status.Addresses.First().Address, true, lizardFsCommands);
        }

        public static void StartChunks(bool master)
        {
            Console.WriteLine("Starting lizardfs chunks");

            var newMaster = Utils.Nodes.Items.FirstOrDefault(d => d.Metadata.Labels.Role == (master ? "master" : "reserve"));
            if (newMaster == null || string.IsNullOrEmpty(Utils.Domain))
            {
                return;
            }

            var chunkYaml = new WebClient().DownloadString("https://raw.githubusercontent.com/lowet84/k8s-config/master/lizardfs/chunk.yml");
            var lizardFsCommands = new[]
            {
                "kubectl create namespace lizardfs",
                $@"echo '{chunkYaml}' | kubectl apply -f -"
            };
            Utils.RunCommands(newMaster.Status.Addresses.First().Address, true, lizardFsCommands);
        }

        private static void FinishMigrationAndCloseApp()
        {
            Utils.Armed = false;
            Utils.MasterUrl = null;
            Console.WriteLine("Migration complete. Shutting down application");
            Thread.Sleep(10000);
            Environment.Exit(0);
        }

        public static string MigrationError(bool resetMaster)
        {
            var reserve = Utils.Nodes.Items.FirstOrDefault(d => d.Metadata?.Labels?.Role == "reserve");
            if (reserve == null && !resetMaster)
            {
                const string message = "Cannot find reserve node to promote";
                Console.WriteLine(message);
                return message;
            }
            if (Utils.Domain == null)
            {
                const string message = "No base domain is set";
                Console.WriteLine(message);
                return message;
            }
            return null;
        }

        

        

        private static readonly Random Random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
}
