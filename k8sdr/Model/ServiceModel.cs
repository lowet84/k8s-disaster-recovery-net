using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k8sdr.Model
{
    public static class ServiceModel
    {
        public class Metadata2
        {
            public string name { get; set; }
            public string @namespace { get; set; }
        }

        public class Port
        {
            public string protocol { get; set; }
            public int port { get; set; }
            public int targetPort { get; set; }
        }

        public class Selector
        {
            public string app { get; set; }
        }

        public class Spec
        {
            public List<Port> ports { get; set; }
            public Selector selector { get; set; }
            public string clusterIP { get; set; }
            public string type { get; set; }
            public string sessionAffinity { get; set; }
        }

        public class LoadBalancer
        {
        }

        public class Status
        {
            public LoadBalancer loadBalancer { get; set; }
        }

        public class Item
        {
            public string kind { get; set; }
            public string apiVersion { get; set; }
            public Metadata2 metadata { get; set; }
            public Spec spec { get; set; }
            public Status status { get; set; }
        }

        public class Services
        {
            public string kind { get; set; }
            public string apiVersion { get; set; }
            public List<Item> items { get; set; }
        }
    }
}
