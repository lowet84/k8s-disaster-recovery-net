using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k8sdr.Model
{
    public static class VolumeClaimsModel
    {
        public class Metadata
        {
            public string name { get; set; }
            public string @namespace { get; set; }
        }

        public class Requests
        {
            public string storage { get; set; }
        }

        public class Resources
        {
            public Requests requests { get; set; }
        }

        public class Spec
        {
            public List<string> accessModes { get; set; }
            public Resources resources { get; set; }
            public string volumeName { get; set; }
        }

        public class Item
        {
            public string kind { get; set; }
            public string apiVersion { get; set; }
            public Metadata metadata { get; set; }
            public Spec spec { get; set; }
        }

        public class VolumeClaims
        {
            public string kind { get; set; }
            public string apiVersion { get; set; }
            public List<Item> items { get; set; }
        }
    }
}
