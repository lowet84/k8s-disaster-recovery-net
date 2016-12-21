using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet.Messages;

namespace k8sdr.Model
{
    public class ResetModel
    {
        public ResetState State { get; set; }
        public string Error { get; set; }
        public string Message { get; set; }
    }
}
