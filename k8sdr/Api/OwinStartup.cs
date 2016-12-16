using System;
using System.IO;
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
                    response = new { Text = "Hello world" };
                }
                else if (context.Request.Path.Value.StartsWith("/setkey")
                && context.Request.Method == "POST")
                {
                    var key = new StreamReader(context.Request.Body).ReadToEnd();
                    Utils.PrivateKey = key;
                }
                else if (context.Request.Path.Value.StartsWith("/sethost")
                && context.Request.Method == "POST")
                {
                    var hostUrl = new StreamReader(context.Request.Body).ReadToEnd();
                    Utils.HostUrl = hostUrl;
                }

                var settings = Utils.Settings;
                settings.PrivateKey = settings.PrivateKey == null ? "N/A" : "Hidden";

                context.Response.ContentType = "application/json";
                var responseValue = JsonSerializer.SerializeToString(response ?? settings);
                return context.Response.WriteAsync(responseValue);
            });
        }
    }
}
