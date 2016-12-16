using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using k8s_disaster_recovery_net_console.Model;
using ServiceStack.Text;

namespace k8s_disaster_recovery_net_console
{
    public static class Utils
    {
        private const string SettingsPath = @"settings.json";
        private const string NodesPath = @"nodesBackup.json";

        public static void InitializeSettings(string[] args)
        {
            if (!File.Exists(SettingsPath))
            {
                RunningMode rm;
                if (args.Length == 3 && args[0] == "master")
                {
                    rm = RunningMode.Master;
                    var key = args[1];
                    Settings = new Settings
                    {
                        RunningMode = rm,
                        HostUrl = args[2],
                        Migrate = new MigrationModel { MigrationKey = key }
                    };
                }
                else if (args.Length == 2 && args[0] == "reserve")
                    Settings = new Settings
                    {
                        RunningMode = RunningMode.Reserve,
                        HostUrl = args[1]
                    };
                else if (args.Length == 1)
                {
                    Settings = new Settings
                    {
                        RunningMode = RunningMode.Node,
                        HostUrl = args[0]
                    };
                    var settings = Settings;
                    settings.Migrate = GetMigrationModelFromMaster();
                    Settings = settings;
                }

            }
        }

        public static RunningMode RunningMode => Settings.RunningMode;

        public static MigrationModel Migrate => Settings.Migrate;

        public static string HostUrl => Settings.HostUrl;

        public static void Promote()
        {
            var settings = Settings;
            settings.RunningMode = RunningMode.Master;
            Settings = settings;
        }

        internal static void PerformMigration(string migrationKey)
        {
            Console.WriteLine("Migrating to new cluster");
            Console.WriteLine(RunApp("kubeadm", "reset"));
            Console.WriteLine(RunApp("systemctl", "start", "kubelet"));
            Console.WriteLine(RunApp("kubeadm", "join", $"--token={migrationKey}"));
            var settings = Settings;
            settings.Migrate.MigrationKey = migrationKey;
            Settings = settings;
        }

        public static NodesModel.Nodes Nodes
        {
            get { return (NodesModel.Nodes)JsonSerializer.DeserializeFromString(File.ReadAllText(NodesPath), typeof(NodesModel.Nodes)); }
            set
            {
                File.WriteAllText(NodesPath, JsonSerializer.SerializeToString(value));
            }
        }

        private static Settings Settings
        {
            get { return (Settings)JsonSerializer.DeserializeFromString(File.ReadAllText(SettingsPath), typeof(Settings)); }
            set
            {
                Console.WriteLine("Writing settings");
                File.WriteAllText(SettingsPath, JsonSerializer.SerializeToString(value));
            }
        }

        public static void Reset()
        {
            var token = RandomString(6) + "." + RandomString(16);
            Console.WriteLine(RunApp("kubeadm", "reset"));
            Console.WriteLine(RunApp("systemctl", "start", "kubelet"));
            Console.WriteLine(RunApp("kubeadm", "init", $"--token={token}", $"--api-external-dns-names={HostUrl}"));
            Console.WriteLine(RunApp("kubectl", "apply", "-f", "/setup/weave.yml"));

            var settings = Settings;
            settings.Migrate.MigrationKey = token;
            Settings = settings;
        }

        private static string RunApp(params string[] arguments)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                Console.WriteLine("Running: " + string.Join(" ", arguments));
                var procStartInfo = new ProcessStartInfo("/bin/bash", "-c '" + string.Join(" ", arguments) + "'")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var proc = new Process { StartInfo = procStartInfo };
                proc.Start();

                var result = proc.StandardOutput.ReadToEnd();
                Console.WriteLine("Done");
                return result;
            }
            else
            {
                Console.WriteLine("On windows, just dummy.");
                return "Windows dummy: " + string.Join(" ", arguments);
            }
        }

        public static MigrationModel GetMigrationModelFromMaster()
        {
            try
            {
                var wc = new WebClient();
                var baseUri = new Uri("http://" + HostUrl);
                var migrationUri = new Uri(baseUri, "migration");
                var data = wc.DownloadString(migrationUri);
                var ret = (MigrationModel)JsonSerializer.DeserializeFromString(data, typeof(MigrationModel));
                return ret;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        private static readonly Random Random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
}
