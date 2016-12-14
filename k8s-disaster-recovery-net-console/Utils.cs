using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using k8s_disaster_recovery_net_console.Model;
using ServiceStack.Text;

namespace k8s_disaster_recovery_net_console
{
    public static class Utils
    {
        private const string SettingsPath = @"settings.json";

        public static void InitializeSettings(string[] args)
        {
            if (!File.Exists(SettingsPath))
            {
                RunningMode rm;
                if (args.Length >= 2 && args[0] == "master")
                {
                    rm = RunningMode.Master;
                    var key = args[1];
                    Settings = new Settings
                    {
                        RunningMode = rm,
                        Migrate = new MigrationModel { MigrationKey = key }
                    };
                }
                else if(args.Length >=1 && args[0]=="reserve") Settings = new Settings { RunningMode = RunningMode.Reserve };
                else Settings = new Settings { RunningMode = RunningMode.Node };

            }
        }

        public static RunningMode RunningMode => Settings.RunningMode;

        public static MigrationModel Migrate => Settings.Migrate;

        private static Settings Settings
        {
            get { return (Settings)JsonSerializer.DeserializeFromString(File.ReadAllText(SettingsPath), typeof(Settings)); }
            set
            {
                File.WriteAllText(SettingsPath, JsonSerializer.SerializeToString(value));
            }
        }

        public static void Reset()
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                Console.WriteLine("Resetting Kubernetes");
            }
            else
            {
                Console.WriteLine("On windows, just dummy.");
            }
        }
    }
}
