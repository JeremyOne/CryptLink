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
        public Hash.HashProvider Provider { get; set; }
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

        /// <summary>
        /// Objects for all chains this server is participating in
        /// </summary>
        //private Dictionary<Hash, BlockChain> ParticipatingChains = new Dictionary<Hash, BlockChain>();

        //private Dictionary<Hash, User> KnownUsers = new Dictionary<Hash, User>();

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
                     Name = Assembly.GetExecutingAssembly().GetName().FullName,
                     Version = Assembly.GetExecutingAssembly().GetName().Version
                },
                Provider = _Provider
            };

        }

		public void StartServices(){
			
            ServiceHost = new Services.ServiceAppHost()
            .Init()
            .Start(ServiceAddress);

			HostContext.Container.AddSingleton<Server>(c => this);

            logger.Info("ServiceAppHost Created at {0}, listening on {1}", DateTime.Now, ServiceAddress);
		}

        public void Dispose() {
            logger.Info(this.ToString() + " Shutdown...");
            ObjectCache.Dispose();
            ServiceHost.Dispose();

            foreach (var client in KnownPeers.AllNodes) {
                client.Dispose();
            }
        }

        public T Get<T>(CByte Key) where T : Hashable {

            if (ObjectCache.Exists(Key)) {
                return ObjectCache.Get<T>(Key);
            } else {
                throw new NotImplementedException();
            }
        }

        public Peer GetPeer(CByte Key) {
            throw new NotImplementedException();
        }
    }

}
