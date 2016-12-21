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
                if (context.Request.Path.Value.StartsWith("/reset")
                && context.Request.Method == "GET")
                {
                    response = HandleGetReset(response);
                }
                else if (context.Request.Path.Value.StartsWith("/resetmaster")
                && context.Request.Method == "POST")
                {
                    HandleRestoreMaster(context);
                }
                else if (context.Request.Path.Value.StartsWith("/restorenodes")
                && context.Request.Method == "POST")
                {
                    HandleConnectNodesToMaster(context);
                }
                else if (context.Request.Path.Value.StartsWith("/setlabels")
                && context.Request.Method == "POST")
                {
                    HandleSetLabels(context);
                }
                else if (context.Request.Path.Value.StartsWith("/setarmed")
                && context.Request.Method == "POST")
                {
                    HandleArmMigrator(context);
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

        private static object HandleGetReset(object response)
        {
            var ret = new ResetModel();
            ret.Message = Utils.Message;

            if (Utils.ResetState == ResetState.Unarmed)
            {
                throw new NotImplementedException("Resetstate should never be set to unarmed in settings.");
            }

            ret.State = !Utils.Armed ? ResetState.Unarmed : Utils.ResetState;
            if (ret.State == ResetState.NotReady)
            {
                var error = Migrator.MigrationError(false);
                ret.State = error == null ? ResetState.ReadyToRestoreMaster : ResetState.NotReady;
                ret.Error = error;
                Utils.ResetState = ret.State;
            }

            response = ret;
            return response;
        }
    }
}
