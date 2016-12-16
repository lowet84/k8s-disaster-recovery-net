using System;
using k8sdr.Api;
using k8sdr.Core;

namespace k8sdr
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (Microsoft.Owin.Hosting.WebApp.Start<OwinStartup>("http://*:9000/"))
                {
                    new CoreRunner().Run();
                }
            }
            catch (Exception e)
            {
                using (Microsoft.Owin.Hosting.WebApp.Start<OwinStartup>("http://localhost:9000/"))
                {
                    Console.WriteLine("Unable to accept requests from all hosts, reverting to localhost");
                    new CoreRunner().Run();
                }
            }
        }
    }
}
