﻿using System;
using k8s_disaster_recovery_net_console.Model;
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
                if (Utils.RunningMode == RunningMode.Master || Utils.RunningMode == RunningMode.Reserve)
                {
                    IController controller = null;
                    if (Utils.RunningMode == RunningMode.Master
                    && context.Request.Path.Value.StartsWith("/nodes"))
                    {
                        controller = new NodeController();
                    }
                    if (context.Request.Path.Value.StartsWith("/migration"))
                    {
                        controller = new MigrationController(context);
                    }
                    if (controller == null)
                    {
                        controller = new ErrorController("Nothing here :(");
                    }

                    var response = controller.Response;
                    context.Response.ContentType = controller.ContentType;
                    var responseString = response as string;
                    var responseValue = responseString ?? JsonSerializer.SerializeToString(response);
                    return context.Response.WriteAsync(responseValue);
                }

                return null;
            });
        }
    }
}

