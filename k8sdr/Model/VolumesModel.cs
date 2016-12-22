using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace k8sdr.Model
{
    public static class VolumesModel
    {
        public class Labels
        {
            public string type { get; set; }
        }

        public class Metadata
        {
            public string name { get; set; }
            public Labels labels { get; set; }
        }

        public class Capacity
        {
            public string storage { get; set; }
        }

        public class Options
        {
            public string host { get; set; }
            public string port { get; set; }
        }

        public class FlexVolume
        {
            public string driver { get; set; }
            public Options options { get; set; }
        }

        public class Spec
        {
            public Capacity capacity { get; set; }
            public FlexVolume flexVolume { get; set; }
            public List<string> accessModes { get; set; }
            public string persistentVolumeReclaimPolicy { get; set; }
        }

        public class Item
        {
            public string kind { get; set; }
            public string apiVersion { get; set; }
            public Metadata metadata { get; set; }
            public Spec spec { get; set; }
        }

        public class Volumes
        {
            public string kind { get; set; }
            public string apiVersion { get; set; }
            public List<Item> items { get; set; }
        }

    }
}
