using System;
using System.CodeDom;
using System.Collections.Generic;
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

                if (context.Request.Path.Value.StartsWith("/api/setarmed")
                       && context.Request.Method == "POST")
                {
                    HandleArmMigrator(context);
                }
                else if (context.Request.Path.Value.StartsWith("/api/forcereset")
                       && context.Request.Method == "POST")
                {
                    HandleForceReset();
                }
                else if (Utils.Armed)
                {
                    HandleArmed(context);
                }
                else
                {
                    HandleUnarmed(context);
                }

                var settings = Utils.Settings;
                settings.PrivateKey = settings.PrivateKey == null ? "N/A" : "Hidden";
                settings.Token = settings.Token == null ? "N/A" : "Hidden";
                settings.Domain = Utils.Domain ?? "N/A";
                settings.HostUrl = Utils.MasterUrl ?? "N/A";

                context.Response.ContentType = "application/json";
                var responseValue = JsonSerializer.SerializeToString(response ?? settings);
                return context.Response.WriteAsync(responseValue);
            });
        }

        private void HandleForceReset()
        {
            Utils.ResetState = ResetState.ReadyToRestoreMaster;
            Utils.Armed = false;
        }

        private void HandleUnarmed(IOwinContext context)
        {

            if (context.Request.Path.Value.StartsWith("/api/setkey")
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
        }

        private void HandleArmed(IOwinContext context)
        {
            var api = context.Request.Path.Value.TrimEnd('/');
            var actions = new Dictionary<string, ArmedAction>
            {
                {"/api/resetmaster", new ArmedAction(Migrator.SetUpMasterNode, ResetState.ReadyToRestoreMaster)},
                {"/api/restorenodes", new ArmedAction(Migrator.ConnectNodesToMaster, ResetState.ReadyToRestoreNodes)},
                {"/api/setlabels", new ArmedAction(Migrator.SetLabels, ResetState.ReadyToSetLabels)},
                {"/api/startcoreservices", new ArmedAction(Migrator.StartCoreServices, ResetState.ReadyToStartCoreServices)},
                {"/api/startlizardfs", new ArmedAction(Migrator.StartLizardfs, ResetState.ReadyToStartLizardfs)},
                {"/api/startchunks", new ArmedAction(Migrator.StartChunks, ResetState.ReadyToStartChunks)},
                {"/api/repairfiles", new ArmedAction(Migrator.RepairFiles, ResetState.ReadyToRepairFiles)},
                {"/api/setupnamespaces", new ArmedAction(Migrator.SetupNamespaces, ResetState.ReadyToSetupNamespaces)},
                {"/api/setupstorage", new ArmedAction(Migrator.SetupStorage, ResetState.ReadyToSetupStorage)},
                {"/api/startapps", new ArmedAction(Migrator.StartApps, ResetState.ReadyToStartApps)}
            };
            if (actions.ContainsKey(api))
            {
                HandleArmedAction(actions[api], context);
            }
        }

        private void HandleArmedAction(ArmedAction armedAction, IOwinContext context)
        {
            if (Utils.ResetState >= armedAction.State)
            {
                var value = new StreamReader(context.Request.Body).ReadToEnd();
                if (new[] { "master", "reserve" }.Contains(value))
                {
                    Utils.ResetState = armedAction.State + 1;
                    var master = value == "master";
                    armedAction.Action(master);
                    Utils.ResetState = armedAction.State + 2;
                }
            }
        }

        private static void HandleArmMigrator(IOwinContext context)
        {
            bool armed;
            bool.TryParse(new StreamReader(context.Request.Body).ReadToEnd(), out armed);
            Utils.Armed = armed;
        }

        private class ArmedAction
        {
            public ResetState State { get; }
            public Action<bool> Action { get; }

            public ArmedAction(Action<bool> action, ResetState state)
            {
                Action = action;
                State = state;
            }
        }
    }
}
