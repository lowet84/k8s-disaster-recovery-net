using System;
using k8s_disaster_recovery_net_console.API;

namespace k8s_disaster_recovery_net_console
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            using (Microsoft.Owin.Hosting.WebApp.Start<OwinStartup>("http://*:9000/"))
            {
                Console.WriteLine("Press [enter] to quit...");
                Console.ReadLine();
            }
        }
    }
}