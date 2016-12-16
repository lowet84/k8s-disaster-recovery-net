using System;
using System.IO;
using k8sdr.Core;
using Owin;
using ServiceStack.Text;

namespace k8sdr.Api
{
    public class OwinStartup
    {
        public void Configuration(IAppBuilder app)
        {
            object response = null;
            app.Run(context =>
            {
                if (context.Request.Path.Value.StartsWith("/reset"))
                {
                    if (Utils.Armed)
                    {
                        Console.WriteLine("Starting migration");
                        new Migrator().StartMigration();
                    }
                    else
                    {
                        Console.WriteLine("Reserve is not armed");
                    }
                }
                else if (context.Request.Path.Value.StartsWith("/setarmed")
                && context.Request.Method == "POST")
                {
                    bool armed;
                    bool.TryParse(new StreamReader(context.Request.Body).ReadToEnd(),out armed);
                    Utils.Armed = armed;
                }
                else if (context.Request.Path.Value.StartsWith("/setkey")
                && context.Request.Method == "POST")
                {
                    var key = new StreamReader(context.Request.Body).ReadToEnd();
                    Utils.PrivateKey = key;
                }
                else if (context.Request.Path.Value.StartsWith("/setdomain")
                && context.Request.Method == "POST")
                {
                    var domain = new StreamReader(context.Request.Body).ReadToEnd();
                    Utils.Domain = domain;
                }
                else if (context.Request.Path.Value.StartsWith("/setmaster")
                && context.Request.Method == "POST")
                {
                    var masterUrl = new StreamReader(context.Request.Body).ReadToEnd();
                    Utils.MasterUrl = masterUrl;
                }

                var settings = Utils.Settings;
                settings.PrivateKey = settings.PrivateKey == null ? "N/A" : "Hidden";
                settings.Token = settings.Token == null ? "N/A" : "Hidden";

                context.Response.ContentType = "application/json";
                var responseValue = JsonSerializer.SerializeToString(response ?? settings);
                return context.Response.WriteAsync(responseValue);
            });
        }
    }
}
