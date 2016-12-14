using System.Collections.Generic;

namespace k8s_disaster_recovery_net_console.API
{
    public class NodeController : IController
    {
        public object Response => new List<string> {"En sak"};

        public string ContentType => "application/json";
    }
}
