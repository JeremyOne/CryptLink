﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CryptLink {
    class Server {
        public Hash ID { get; set; }
        public bool DirectConnectionPossible { get; set; }
        public DateTime LastSeen { get; set; }
        public DateTime FirstSeen { get; set; }
        public IPAddress LastKnownPublicIP { get; set; }
        public List<IPAddress> KnownPublicIPs { get; set; }
        public List<Hash> KnownPeers { get; set; }
    }
}
