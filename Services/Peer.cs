using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CryptLink {

    /// <summary>
    /// Peer - A network accessible host
    /// </summary>
    public class Peer : Hashable, IDisposable {
        public bool PubicallyAccessible { get; set; }
        public DateTime LastSeen { get; set; }
        public DateTime FirstSeen { get; set; }
        public Uri LastKnownPublicUri { get; set; }
        public List<Uri> KnownPublicUris { get; set; }
        public List<Hash> KnownPeers { get; set; }
        public Hash ServerOperator { get; set; }
        public X509Certificate2 PublicKey { get; set; }
        public TimeSpan ConnectTimeOut { get; set; }
        public int ConnectRetryMax { get; set; }
        public int ConnectRetries { get; private set; }
        public AppVersionInfo Version { get; set; }
		public override Hash.HashProvider Provider { get; set; }
        public int Weight { get; set; } = 50;

        public IServiceClient LocalClient { get; set; }

		[JsonIgnore]
        public override bool HashIsImmutable {
            get {
                return true;
            }
        }

        public override byte[] HashableData() {
            if (PublicKey == null) {
                throw new NullReferenceException("Public key of a peer can not be null");
            } else {
                return PublicKey.PublicKey.EncodedKeyValue.RawData;
            }
        }

        public bool TryConnect() {
            throw new NotImplementedException();
        }

        public bool Connected() {
            throw new NotImplementedException();
        }

        public void Dispose() {
            throw new NotImplementedException();
        }

    }
}
