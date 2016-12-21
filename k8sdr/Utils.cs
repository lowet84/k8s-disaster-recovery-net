using System;
using System.IO;
using System.Text;
using k8sdr.Model;
using Renci.SshNet;
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
                    : new Settings()
                    {
                        ResetState = ResetState.NotReady
                    };
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

        public static string Domain
        {
            get { return Settings.Domain; }
            set
            {
                var settings = Settings;
                settings.Domain = value;
                Settings = settings;
            }
        }

        public static bool Armed
        {
            get { return Settings.Armed; }
            set
            {
                var settings = Settings;
                settings.Armed = value;
                Settings = settings;
            }
        }

        public static string MasterUrl
        {
            get { return Settings.HostUrl; }
            set
            {
                var settings = Settings;
                settings.HostUrl = value;
                Settings = settings;
            }
        }

        public static string Token
        {
            get { return Settings.Token; }
            set
            {
                var settings = Settings;
                settings.Token = value;
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

        public static ResetState ResetState
        {
            get { return Settings.ResetState; }
            set
            {
                var settings = Settings;
                settings.ResetState = value;
                Settings = settings;
            }
        }

        public static string Message
        {
            get { return Settings.Message; }
            set
            {
                var settings = Settings;
                settings.Message = value;
                Settings = settings;
            }
        }

        public static string[] RunCommands(string host, bool verbose, params string[] commands)
        {
            var ret = new string[commands.Length];

            try
            {
                var sshClient = new SshClient(
                    host,
                    "root",
                    new PrivateKeyFile(
                        new MemoryStream(
                            Encoding.UTF8.GetBytes(
                                PrivateKey))));
                Console.WriteLine($"Connecting to: {host}");
                sshClient.Connect();
                for (var index = 0; index < commands.Length; index++)
                {
                    var command = commands[index];
                    Console.WriteLine($"Running command: {command}");
                    ret[index] = sshClient.RunCommand(command).Result;
                    if (verbose)
                    {
                        Console.WriteLine(ret[index]);
                        Message = ret[index];
                    }
                }
                Console.WriteLine("Finished");
                sshClient.Disconnect();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
            
            return ret;
        }

        public static string[] RunCommands(bool verbose, params string[] commands)
        {
            return RunCommands(MasterUrl, verbose, commands);
        }
    }
}
