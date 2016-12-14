using System;
using Microsoft.Owin;
using Owin;
using ServiceStack.Text;

[assembly: OwinStartup(typeof(k8s_disaster_recovery_net_console.API.OwinStartup))]

namespace k8s_disaster_recovery_net_console.API
{
    public class OwinStartup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Run(context =>
            {
                IController controller;
                if (context.Request.Path.Value == "/nodes")
                {
                    controller = new NodeController();
                }
                else if (context.Request.Path.Value == "/favicon.ico")
                {
                    controller = new ErrorController("Icon");
                }
                else
                {
                    controller = new ErrorController("Nothing here :(");
                }

                var response = controller.Response;
                context.Response.ContentType = controller.ContentType;
                var responseString = response as string;
                try
                {
                    var responseValue = responseString ?? JsonSerializer.SerializeToString(response);
                    return context.Response.WriteAsync(responseValue);
                }
                catch (Exception e)
                {
                    PrintExceptions(e);
                    throw;
                }
            });
        }

        private void PrintExceptions(Exception e)
        {
            Console.WriteLine(e.Message);
            if (e.InnerException != null)
                PrintExceptions(e.InnerException);
        }
    }
}

