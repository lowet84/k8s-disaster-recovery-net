using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k8sdr.Model
{
    public static class NamespacesModel
    {
        public class Metadata
        {
            public string name { get; set; }
        }

        public class Item
        {
            public string kind { get; set; }
            public string apiVersion { get; set; }
            public Metadata metadata { get; set; }
        }

        public class Namespaces
        {
            public string kind { get; set; }
            public string apiVersion { get; set; }
            public List<Item> items { get; set; }
        }
    }
}
