using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k8s_disaster_recovery_net_console.Model
{
    public class Settings
    {
        public string HostUrl { get; set; }
        public RunningMode RunningMode { get; set; }
        public MigrationModel Migrate { get; set; }
    }
}
