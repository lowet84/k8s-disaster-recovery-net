using System;
using System.Threading;
using k8s_disaster_recovery_net_console.API;
using k8s_disaster_recovery_net_console.Model;

namespace k8s_disaster_recovery_net_console
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Utils.InitializeSettings(args);

            while (Utils.RunningMode == RunningMode.Node)
            {
                Console.WriteLine("Sleeping...");
                Thread.Sleep(5000);
            }

            try
            {
                using (Microsoft.Owin.Hosting.WebApp.Start<OwinStartup>("http://*:9000/"))
                {
                    Console.WriteLine("Press [enter] to quit...");
                    Console.ReadLine();
                }
            }
            catch (Exception e)
            {
                using (Microsoft.Owin.Hosting.WebApp.Start<OwinStartup>("http://localhost:9000/"))
                {
                    Console.WriteLine("Unable to accept requests from all hosts, reverting to localhost");
                    Console.WriteLine("Press [enter] to quit...");
                    Console.ReadLine();
                }
            }
        }
    }
}