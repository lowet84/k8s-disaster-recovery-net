using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k8sdr.Model
{
    public static class IngressModel
    {
        public class Metadata2
        {
            public string name { get; set; }
            public string @namespace { get; set; }
            public string selfLink { get; set; }
            public string uid { get; set; }
            public string resourceVersion { get; set; }
            public int generation { get; set; }
            public string creationTimestamp { get; set; }
        }

        public class Backend
        {
            public string serviceName { get; set; }
            public int servicePort { get; set; }
        }

        public class Path
        {
            public Backend backend { get; set; }
        }

        public class Http
        {
            public List<Path> paths { get; set; }
        }

        public class Rule
        {
            public string host { get; set; }
            public Http http { get; set; }
        }

        public class Spec
        {
            public List<Rule> rules { get; set; }
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

        public class RootObject
        {
            public string kind { get; set; }
            public string apiVersion { get; set; }
            public List<Item> items { get; set; }
        }
    }
}
