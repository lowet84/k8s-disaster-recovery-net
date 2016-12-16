using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using k8sdr.Model;
using ServiceStack.Text;

namespace k8sdr
{
    public static class Utils
    {
        private const string SettingsPath = @"settings.json";

        public static Settings Settings
        {
            get
            {
                return File.Exists(SettingsPath)
                    ? (Settings) JsonSerializer.DeserializeFromString(File.ReadAllText(SettingsPath), typeof(Settings))
                    : new Settings();
            }
            set
            {
                Console.WriteLine("Writing settings");
                File.WriteAllText(SettingsPath, JsonSerializer.SerializeToString(value));
            }
        }

        public static string PrivateKey
        {
            get { return Settings.PrivateKey; }
            set
            {
                var settings = Settings;
                settings.PrivateKey = value;
                Settings = settings;
            }
        }

        public static string HostUrl
        {
            get { return Settings.HostUrl; }
            set
            {
                var settings = Settings;
                settings.HostUrl = value;
                Settings = settings;
            }
        }

        public static NodesModel.Nodes Nodes
        {
            get { return Settings.Nodes; }
            set
            {
                var settings = Settings;
                settings.Nodes = value;
                Settings = settings;
            }
        }
    }
}
