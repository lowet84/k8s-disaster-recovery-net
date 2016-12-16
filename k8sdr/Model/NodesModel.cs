using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k8sdr.Model
{
    public static class NodesModel
    {
        public class Nodes
        {
            public List<Item> Items { get; set; }
        }

        public class Item
        {
            public Metadata Metadata { get; set; }
            public Status Status { get; set; }
        }

        public class Metadata
        {
            public string Name { get; set; }
            public Labels Labels { get; set; }
        }

        public class Labels
        {
            public string Role { get; set; }
        }

        public class Status
        {
            public List<AddressItem> Addresses { get; set; }
        }

        public class AddressItem
        {
            public string Type { get; set; }
            public string Address { get; set; }
        }
    }
}
