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
        public string Domain { get; set; }
        public bool Armed { get; set; }
        public string HostUrl { get; set; }
        public NodesModel.Nodes Nodes { get; set; }
        public VolumesModel.Volumes Volumes { get; set; }
        public NamespacesModel.Namespaces Namespaces { get; set; }
        public VolumeClaimsModel.VolumeClaims VolumeClaims { get; set; }

        public string Token { get; set; }
        public ResetState ResetState { get; set; }
        public string Message { get; set; }
    }
}
