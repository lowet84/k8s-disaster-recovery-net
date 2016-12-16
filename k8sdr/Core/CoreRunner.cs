using System;
using System.Collections.Generic;
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
                    if (Utils.PrivateKey != null)
                    {
                        var result = RunCommands(false, "kubectl get nodes -o json");
                        var nodes = JsonSerializer.DeserializeFromString<NodesModel.Nodes>(result[0]);
                        Utils.Nodes = nodes;
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

        private string[] RunCommands(bool verbose, params string[] commands)
        {
            var ret = new string[commands.Length];
            var sshClient = new SshClient(
                            Utils.HostUrl,
                            "root",
                            new PrivateKeyFile(
                                new MemoryStream(
                                    Encoding.UTF8.GetBytes(
                                        Utils.PrivateKey))));
            Console.WriteLine($"Connecting to: {Utils.HostUrl}");
            sshClient.Connect();
            for (var index = 0; index < commands.Length; index++)
            {
                var command = commands[index];
                Console.WriteLine($"Running command: {command}");
                ret[index] = sshClient.RunCommand(command).Result;
                if (verbose)
                {
                    Console.WriteLine(ret[index]);
                }
            }
            Console.WriteLine("Finished");
            sshClient.Disconnect();
            return ret;
        }
    }
}
