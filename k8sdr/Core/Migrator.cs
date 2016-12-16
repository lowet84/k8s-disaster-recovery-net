using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using k8sdr.Model;

namespace k8sdr.Core
{
    public class Migrator
    {
        public void StartMigration()
        {
            var reserve = Utils.Nodes.Items.FirstOrDefault(d => d.Metadata?.Labels?.Role == "reserve");
            if (reserve == null)
            {
                Console.WriteLine("Cannot find reserve node to promote");
                return;
            }
            var otherNodes = Utils.Nodes.Items.Where(d => d.Metadata?.Labels?.Role != "reserve");

            Utils.LockedForMigration = true;

            Console.WriteLine("Upgrading reserve to Master");
            var token = RandomString(6) + "." + RandomString(16);
            Utils.Token = token;
            var commands = new[]
            {
                "kubeadm reset",
                "systemctl start kubelet",
                $"kubeadm init --token={token}",
                "kubectl apply -f https://git.io/weave-kube"
            };
            Utils.RunCommands(reserve.Status.Addresses.First().Address, true, commands);

            Console.WriteLine("Connecting nodes to new master");
            var tasks = new List<Task>();
            foreach (var node in otherNodes)
            {
                var task = new Task(() =>
                {
                    commands = new[]
                    {
                        "kubeadm reset",
                        "systemctl start kubelet",
                        $"kubeadm join --token={token} {reserve.Status.Addresses.First().Address}"
                    };
                    Utils.RunCommands(node.Status.Addresses.First().Address, true, commands);
                });
                tasks.Add(task);
                task.Start();
            }

            while (tasks.Any(d => !d.IsCompleted))
            {

            }

            var labelCommands = new List<string>();
            foreach (var node in otherNodes)
            {
                if (node.Metadata.Labels.Role == "storage")
                {
                    labelCommands.AddRange(new string[]
                    {
                        $"kubectl taint nodes {node.Metadata.Name} dedicated=storage:NoSchedule",
                        $"kubectl label nodes {node.Metadata.Name} role = storage"
                    });
                }
            }
            Utils.RunCommands(reserve.Status.Addresses.First().Address, true, labelCommands.ToArray());

            Utils.Armed = false;
            Utils.MasterUrl = null;
            Console.WriteLine("Migration complete. Shutting down application");
            Thread.Sleep(10000);
            Environment.Exit(0);
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
