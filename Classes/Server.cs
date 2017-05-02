﻿//using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading;
using ServiceStack;
using NLog;
using System.Reflection;

namespace CryptLink {

    /// <summary>
    /// A server that runs locally, processes requests, caches data
    /// </summary>
    public class Server : IServer {
        public Hash.HashProvider Provider {
            get {
                if (ThisPeerInfo != null) {
                    return ThisPeerInfo.Provider;
                } else {
                    throw new NullReferenceException("Peer info is not set.");
                }
            }
        }

        public X509Certificate2 ServerCert { get; set; }
        public IObjectCache ObjectCache { get; set; }
		public string ServiceAddress { get; set; }
        public long MessageCount { get; set; }
	    public Logger logger { get; set; } = LogManager.GetCurrentClassLogger();
        public Peer ThisPeerInfo { get; set; }
        
		public ServiceStackHost ServiceHost { get; set; }

        /// <summary>
        /// All known servers
        /// </summary>
		public ConsistentHash<Peer> KnownPeers { get; set; }

        public bool AcceptingObjects {
            get {
                if (ObjectCache != null) {
                    return ObjectCache.AcceptingObjects;
                } else {
                    return false;
                }
            }
        }

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

        /// <summary>
        /// Creates a new server object with settings from StaticConfig
        /// </summary>
        public Server() {
            var c = CryptLink.ConfigStatic.Config;
            ThisPeerInfo = c.PeerDetail;
            Provider = c.PeerDetail.Provider;
            ServerCert = c.ServerPrivateKey;
            ObjectCache = c.DefaultCache;

            //ServerCert = new X509Certificate2Builder { SubjectName = "CN=Default Cert", KeyStrength = 1024 }.Build();
            ObjectCache = new DictionaryCache() {
                AcceptingObjects = true,
                ManageEvery = new TimeSpan(0, 0, 0),
                ManageEveryIOCount = 0,
                MaxCollectionCount = 10000000,
                MaxCollectionSize = 1024 * 1024 * 1024,
                MaxExpiration = new TimeSpan(0, 0, 20),
                MaxObjectSize = 1024 * 1024
            };

            ObjectCache.Initialize();

            KnownPeers = new ConsistentHash<Peer>(Provider);

            //update 
            foreach (var peer in c.KnownPeers) {
                KnownPeers.Add(peer, false, peer.Weight);
            }
            KnownPeers.UpdateKeyArray();

			//ServiceAddress = "http://127.0.0.1:12345";

		}

        public Server(Hash.HashProvider _Provider, X509Certificate2 _ServerCert, IObjectCache _ObjectCache, string _ServiceAddress) {

            Provider = _Provider;
            ServerCert = _ServerCert;
            ObjectCache = _ObjectCache;
			ServiceAddress = _ServiceAddress;

            KnownPeers = new ConsistentHash<Peer>(Provider);

            if (_ServerCert == null) {
                throw new ArgumentNullException("ServerCert was null but is required");
            }
            
            ThisPeerInfo = new Peer() {
                PublicKey = Utility.GetPublicKey(_ServerCert),
                Version = new AppVersionInfo() {
                     ApiCompartibilityVersion = new Version(1, 0, 0, 0),
                     ApiVersion = new Version(1, 0, 0, 0),
                     Name = Assembly.GetExecutingAssembly().GetName().Name,
                     Version = Assembly.GetExecutingAssembly().GetName().Version
                },
                Provider = _Provider
            };

        }

		public void Init(){		

		}

        public void Dispose() {
            logger.Info(this.ToString() + " Shutdown...");
            if (ObjectCache != null) {
                ObjectCache.Dispose();
            }

            if (ServiceHost != null) {
                ServiceHost.Dispose();
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
