﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k8s_disaster_recovery_net_console.Model
{
    public class NodesModel
    {
        public class Nodes
        {
            public List<Item> Items { get; set; }
        }

        public class Item
        {
            public Metadata Metadata { get; set; }
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
    }
}
