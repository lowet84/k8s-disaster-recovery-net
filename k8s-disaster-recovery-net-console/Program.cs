using System;
using System.Net;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using k8s_disaster_recovery_net_console.API;
using k8s_disaster_recovery_net_console.Model;
using ServiceStack;
using ServiceStack.Text;

namespace k8s_disaster_recovery_net_console
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Utils.InitializeSettings(args);

            while (Utils.RunningMode == RunningMode.Node)
            {
                MigrateIfNecessary();
                Console.WriteLine("Sleeping...");
                Thread.Sleep(5000);
            }

            try
            {
                using (Microsoft.Owin.Hosting.WebApp.Start<OwinStartup>("http://*:9000/"))
                {
                    RunMainLoop();
                }
            }
            catch (Exception e)
            {
                using (Microsoft.Owin.Hosting.WebApp.Start<OwinStartup>("http://localhost:9000/"))
                {
                    Console.WriteLine("Unable to accept requests from all hosts, reverting to localhost");
                    RunMainLoop();
                }
            }
        }

        private static void RunMainLoop()
        {
            if (Utils.RunningMode == RunningMode.Reserve)
            {
                Console.WriteLine("Starting reserve update loop");
                Task task = Task.Run((Action)UpdateFromMasterLoop);
            }
            Console.WriteLine("Press any key to exit...");
            Console.Read();
        }

        private static void MigrateIfNecessary()
        {
            var migrationModel = Utils.GetMigrationModelFromMaster();
            if (migrationModel == null)
            {
                Console.WriteLine("Could not get migrationkey from master.");
            }
            else if (migrationModel.MigrationKey != Utils.Migrate.MigrationKey)
            {
                Console.WriteLine("Migrationkey has changed, migrating.");
                Utils.PerformMigration(migrationModel.MigrationKey);
            }
            else
            {
                Console.WriteLine("Migrationkey is unchanged. Doing nothing");
            }
        }

        private static void UpdateFromMasterLoop()
        {
            var wc = new WebClient();
            while (true)
            {
                try
                {
                    var baseUri = new Uri("http://" + Utils.HostUrl);
                    var nodesUri = new Uri(baseUri, "nodes");
                    var nodes = wc.DownloadString(nodesUri);
                    var nodesObject = (NodesModel.Nodes)JsonSerializer.DeserializeFromString(nodes, typeof(NodesModel.Nodes));
                    if (nodesObject != null && !nodesObject.Items.IsNullOrEmpty())
                    {
                        Utils.Nodes = nodesObject;
                    }
                    Console.WriteLine("Successfully saved nodes setup");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                Console.WriteLine("Sleeping 5 seconds.");
                Thread.Sleep(5000);
            }
        }
    }
}