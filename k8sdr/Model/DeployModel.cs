using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k8sdr.Model
{
    public static class DeployModel
    {
        public class Labels
        {
            public string app { get; set; }
        }

        public class Metadata2
        {
            public string name { get; set; }
            public string @namespace { get; set; }
            public Labels labels { get; set; }
        }

        public class MatchLabels
        {
            public string app { get; set; }
        }

        public class Selector
        {
            public MatchLabels matchLabels { get; set; }
        }

        public class Labels2
        {
            public string app { get; set; }
        }

        public class Metadata3
        {
            public object creationTimestamp { get; set; }
            public Labels2 labels { get; set; }
        }

        public class PersistentVolumeClaim
        {
            public string claimName { get; set; }
        }

        public class Volume
        {
            public string name { get; set; }
            public PersistentVolumeClaim persistentVolumeClaim { get; set; }
        }

        public class Port
        {
            public int containerPort { get; set; }
            public string protocol { get; set; }
        }

        public class Env
        {
            public string name { get; set; }
            public string value { get; set; }
        }

        public class Resources
        {
        }

        public class VolumeMount
        {
            public string name { get; set; }
            public string mountPath { get; set; }
            public string subPath { get; set; }
        }

        public class Container
        {
            public string name { get; set; }
            public string image { get; set; }
            public List<Port> ports { get; set; }
            public List<Env> env { get; set; }
            public Resources resources { get; set; }
            public List<VolumeMount> volumeMounts { get; set; }
            public string terminationMessagePath { get; set; }
            public string imagePullPolicy { get; set; }
        }

        public class SecurityContext
        {
        }

        public class Spec2
        {
            public List<Volume> volumes { get; set; }
            public List<Container> containers { get; set; }
            public string restartPolicy { get; set; }
            public int terminationGracePeriodSeconds { get; set; }
            public string dnsPolicy { get; set; }
            public SecurityContext securityContext { get; set; }
        }

        public class Template
        {
            public Metadata3 metadata { get; set; }
            public Spec2 spec { get; set; }
        }

        public class RollingUpdate
        {
            public int maxUnavailable { get; set; }
            public int maxSurge { get; set; }
        }

        public class Strategy
        {
            public string type { get; set; }
            public RollingUpdate rollingUpdate { get; set; }
        }

        public class Spec
        {
            public int replicas { get; set; }
            public Selector selector { get; set; }
            public Template template { get; set; }
            public Strategy strategy { get; set; }
        }

        public class Item
        {
            public string kind { get; set; }
            public string apiVersion { get; set; }
            public Metadata2 metadata { get; set; }
            public Spec spec { get; set; }
        }

        public class Deployments
        {
            public string kind { get; set; }
            public string apiVersion { get; set; }
            public List<Item> items { get; set; }
        }
    }
}
