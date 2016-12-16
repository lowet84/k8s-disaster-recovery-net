using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k8sdr.Model
{
    public class Settings
    {
        public string PrivateKey { get; set; }
        public string HostUrl { get; set; }
        public NodesModel.Nodes Nodes { get; set; }
    }
}
