using System;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using NLog;

namespace CryptLink {


    /// <summary>
    /// Data and config for the server
    /// </summary>
    public class Server {

        public Cert Cert { get; set; }

        public IObjectCache StoreCache { get; set; }
        public IObjectCache SendCache { get; set; }

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
                if (StoreCache != null) {
                    return StoreCache.AcceptingObjects;
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
                if (StoreCache != null) {
                    return StoreCache.CurrentCollectionCount > 0;
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
            if (StoreCache != null) {
                StoreCache.Initialize();
            }

            KnownPeers = new ConsistentHash<Peer>(Provider);
        }

        public void Dispose() {
            if (StoreCache != null) {
                StoreCache.Dispose();
            }

            foreach (var client in KnownPeers.AllNodes) {
                client.Dispose();
            }
        }

        public T Get<T>(ComparableBytesAbstract Key) where T : Hashable {

            if (StoreCache.Exists(Key)) {
                return StoreCache.Get<T>(Key);
            } else {
                //try and get from the closest peer
                if (KnownPeers.NodeCount > 0) {
                    var closestPeer = GetPeer(Key);

                    if (closestPeer != null) {
                        return closestPeer.TryGet<T>(Key);
                    }
                }
            }

            return null;
        }

        public Peer GetPeer(ComparableBytesAbstract Key) {
            throw new NotImplementedException();
        }
    }

}
