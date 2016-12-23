using System;
using System.CodeDom;
using System.IO;
using System.Linq;
using k8sdr.Core;
using k8sdr.Model;
using Microsoft.Owin;
using Owin;
using ServiceStack.Text;

namespace k8sdr.Api
{
    public class OwinStartup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Run(context =>
            {
                object response = null;
                if (context.Request.Path.Value.StartsWith("/api/resetmaster")
                && context.Request.Method == "POST")
                {
                    HandleRestoreMaster(context);
                }
                else if (context.Request.Path.Value.StartsWith("/api/restorenodes")
                && context.Request.Method == "POST")
                {
                    HandleConnectNodesToMaster(context);
                }
                else if (context.Request.Path.Value.StartsWith("/api/setlabels")
                && context.Request.Method == "POST")
                {
                    HandleSetLabels(context);
                }
                else if (context.Request.Path.Value.StartsWith("/api/startcoreservices")
                && context.Request.Method == "POST")
                {
                    HandleStartCoreServices(context);
                }
                else if (context.Request.Path.Value.StartsWith("/api/startlizardfs")
                && context.Request.Method == "POST")
                {
                    HandleStartLizardfs(context);
                }
                else if (context.Request.Path.Value.StartsWith("/api/startchunks")
                && context.Request.Method == "POST")
                {
                    HandleStartChunks(context);
                }
                else if (context.Request.Path.Value.StartsWith("/api/setupnamespaces")
                && context.Request.Method == "POST")
                {
                    HandleSetupNamespaces(context);
                }
                else if (context.Request.Path.Value.StartsWith("/api/setupstorage")
                && context.Request.Method == "POST")
                {
                    HandleSetupStorage(context);
                }
                else if (context.Request.Path.Value.StartsWith("/api/startapps")
                && context.Request.Method == "POST")
                {
                    HandleStartApps(context);
                }
                else if (context.Request.Path.Value.StartsWith("/api/setarmed")
                && context.Request.Method == "POST")
                {
                    HandleArmMigrator(context);
                }
                else if (context.Request.Path.Value.StartsWith("/api/setkey")
                && context.Request.Method == "POST")
                {
                    var key = new StreamReader(context.Request.Body).ReadToEnd();
                    Utils.PrivateKey = key;
                }
                else if (context.Request.Path.Value.StartsWith("/api/setdomain")
                && context.Request.Method == "POST")
                {
                    var domain = new StreamReader(context.Request.Body).ReadToEnd();
                    Utils.Domain = domain;
                }
                else if (context.Request.Path.Value.StartsWith("/api/setmaster")
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

        private void HandleStartApps(IOwinContext context)
        {
            if (Utils.ResetState == ResetState.ReadyToStartApps)
            {
                var value = new StreamReader(context.Request.Body).ReadToEnd();
                if (new[] { "master", "reserve" }.Contains(value))
                {
                    Utils.ResetState = ResetState.StartingApps;
                    var master = value == "master";
                    Migrator.StartApps(master);
                    Utils.ResetState = ResetState.Finished;
                }
            }
        }

        private void HandleSetupStorage(IOwinContext context)
        {
            if (Utils.ResetState == ResetState.ReadyToSetupStorage)
            {
                var value = new StreamReader(context.Request.Body).ReadToEnd();
                if (new[] { "master", "reserve" }.Contains(value))
                {
                    Utils.ResetState = ResetState.SettingUpStorage;
                    var master = value == "master";
                    Migrator.SetupStorage(master);
                    Utils.ResetState = ResetState.ReadyToStartApps;
                }
            }
        }

        private void HandleSetupNamespaces(IOwinContext context)
        {
            if (Utils.ResetState == ResetState.ReadyToSetupNamespaces)
            {
                var value = new StreamReader(context.Request.Body).ReadToEnd();
                if (new[] { "master", "reserve" }.Contains(value))
                {
                    Utils.ResetState = ResetState.SettingUpNamespaces;
                    var master = value == "master";
                    Migrator.SetupNamespaces(master);
                    Utils.ResetState = ResetState.ReadyToSetupStorage;
                }
            }
        }

        private void HandleStartChunks(IOwinContext context)
        {
            if (Utils.ResetState == ResetState.ReadyToStartChunks)
            {
                var value = new StreamReader(context.Request.Body).ReadToEnd();
                if (new[] { "master", "reserve" }.Contains(value))
                {
                    Utils.ResetState = ResetState.StartingChunks;
                    var master = value == "master";
                    Migrator.StartChunks(master);
                    Utils.ResetState = ResetState.ReadyToSetupNamespaces;
                }
            }
        }

        private void HandleStartLizardfs(IOwinContext context)
        {
            if (Utils.ResetState == ResetState.ReadyToStartLizardfs)
            {
                var value = new StreamReader(context.Request.Body).ReadToEnd();
                if (new[] { "master", "reserve" }.Contains(value))
                {
                    Utils.ResetState = ResetState.StartingLizardfs;
                    var master = value == "master";
                    Migrator.StartLizardfs(master);
                    Utils.ResetState = ResetState.ReadyToStartChunks;
                }
            }
        }

        private void HandleStartCoreServices(IOwinContext context)
        {
            if (Utils.ResetState == ResetState.ReadyToStartCoreServices)
            {
                var value = new StreamReader(context.Request.Body).ReadToEnd();
                if (new[] { "master", "reserve" }.Contains(value))
                {
                    Utils.ResetState = ResetState.StartingCoreServices;
                    var master = value == "master";
                    Migrator.StartCoreServices(master);
                    Utils.ResetState = ResetState.ReadyToStartLizardfs;
                }
            }
        }

        private void HandleSetLabels(IOwinContext context)
        {
            if (Utils.ResetState == ResetState.ReadyToSetLabels)
            {
                var value = new StreamReader(context.Request.Body).ReadToEnd();
                if (new[] {"master", "reserve"}.Contains(value))
                {
                    Utils.ResetState = ResetState.SettingLabels;
                    var master = value == "master";
                    Migrator.SetLabels(master);
                    Utils.ResetState = ResetState.ReadyToStartCoreServices;
                }
            }
        }


        private static void HandleArmMigrator(IOwinContext context)
        {
            bool armed;
            bool.TryParse(new StreamReader(context.Request.Body).ReadToEnd(), out armed);
            Utils.Armed = armed;
        }

        private static void HandleConnectNodesToMaster(IOwinContext context)
        {
            if (Utils.ResetState == ResetState.ReadyToRestoreNodes)
            {
                var value = new StreamReader(context.Request.Body).ReadToEnd();
                if (new[] {"master", "reserve"}.Contains(value))
                {
                    Utils.ResetState = ResetState.RestoringNodes;
                    var master = value == "master";
                    Migrator.ConnectNodesToMaster(master);
                    Utils.ResetState = ResetState.ReadyToSetLabels;
                }
            }
        }

        private static void HandleRestoreMaster(IOwinContext context)
        {
            if (Utils.ResetState == ResetState.ReadyToRestoreMaster)
            {
                var value = new StreamReader(context.Request.Body).ReadToEnd();
                if (new[] {"master", "reserve"}.Contains(value))
                {
                    Utils.ResetState = ResetState.RestoringMaster;
                    var master = value == "master";
                    Migrator.SetUpMasterNode(master);
                    Utils.ResetState = ResetState.ReadyToRestoreNodes;
                }
            }
        }
    }
}
