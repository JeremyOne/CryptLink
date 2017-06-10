using System;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using NLog;

namespace CryptLink {

    /// <summary>
    /// A server that runs locally, processes requests, caches data
    /// </summary>
    public class Server {

        [JsonConverter(typeof(CertificateSerializer))]
        public X509Certificate2 PrivateKey { get; set; }

        public IObjectCache ObjectCache { get; set; }

        public string ServiceAddress { get; set; }
        public long MessageCount { get; set; }
        
        
        /// <summary>
        /// All known servers
        /// </summary>
		public ConsistentHash<Peer> KnownPeers { get; set; }

        /// <summary>
        /// If true, the server is accepting objects for storage
        /// </summary>
        public bool AcceptingObjects {
            get {
                if (ObjectCache != null) {
                    return ObjectCache.AcceptingObjects;
                } else {
                    return false;
                }
            }
        }

        /// <summary>
        /// If true, the server is currently holding storage objects
        /// </summary>
        public bool HoldingObjects {
            get {
                if (ObjectCache != null) {
                    return ObjectCache.CurrentCollectionCount > 0;
                } else {
                    return false;
                }
            }
        }

        /// <summary>
        /// Objects for all chains this server is participating in
        /// </summary>
        //private Dictionary<Hash, BlockChain> ParticipatingChains = new Dictionary<Hash, BlockChain>();

        //private Dictionary<Hash, User> KnownUsers = new Dictionary<Hash, User>();

        
        public void Initialize(Hash.HashProvider Provider) {
            if (ObjectCache != null) {
                ObjectCache.Initialize();
            }

            KnownPeers = new ConsistentHash<Peer>(Provider);
        }

   //     public Server(Hash.HashProvider _Provider, X509Certificate2 _ServerCert, IObjectCache _ObjectCache, string _ServiceAddress) {

   //         ServerCert = _ServerCert;
   //         ObjectCache = _ObjectCache;
			//ServiceAddress = _ServiceAddress;

   //         KnownPeers = new ConsistentHash<Peer>(Provider);

   //         if (_ServerCert == null) {
   //             throw new ArgumentNullException("ServerCert was null but is required");
   //         }
            
   //         ThisPeerInfo = new Peer() {
   //             PublicKey = Utility.GetPublicKey(_ServerCert),
   //             Version = new AppVersionInfo() {
   //                  ApiCompartibilityVersion = new Version(1, 0, 0, 0),
   //                  ApiVersion = new Version(1, 0, 0, 0),
   //                  Name = Assembly.GetExecutingAssembly().GetName().Name,
   //                  Version = Assembly.GetExecutingAssembly().GetName().Version
   //             },
   //             Provider = _Provider
   //         };

   //     }

        public void Dispose() {
            if (ObjectCache != null) {
                ObjectCache.Dispose();
            }

            foreach (var client in KnownPeers.AllNodes) {
                client.Dispose();
            }
        }

        public T Get<T>(ComparableBytesAbstract Key) where T : Hashable {

            if (ObjectCache.Exists(Key)) {
                return ObjectCache.Get<T>(Key);
            } else {
                throw new NotImplementedException();
            }
        }

        public Peer GetPeer(ComparableBytesAbstract Key) {
            throw new NotImplementedException();
        }
    }

}
